'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.ComponentModel.Design
Imports Microsoft.Win32
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.Shell

''' <summary>
''' This is the class that implements the package exposed by this assembly.
'''
''' The minimum requirement for a class to be considered a valid package for Visual Studio
''' is to implement the IVsPackage interface and register itself with the shell.
''' This package uses the helper classes defined inside the Managed Package Framework (MPF)
''' to do it: it derives from the Package class that provides the implementation of the 
''' IVsPackage interface and uses the registration attributes defined in the framework to 
''' register itself and its components with the shell.
''' </summary>
' The PackageRegistration attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class
' is a package.
'
' The InstalledProductRegistration attribute is used to register the information needed to show this package
' in the Help/About dialog of Visual Studio.
'
' The ProvideMenuResource attribute is needed to let the shell know that this package exposes some menus.
'
' The ProvideToolWindow attribute registers a tool window exposed by this package.

<PackageRegistration(UseManagedResourcesOnly:=True), _
InstalledProductRegistration("#110", "#112", "1.0", IconResourceID:=400), _
ProvideMenuResource("Menus.ctmenu", 1), _
ProvideToolWindow(GetType(BuildProgressToolWindow)), _
Guid(GuidList.guidBuildProgressBarPkgString)> _
Public NotInheritable Class BuildProgressBarPackage
    Inherits Package
    Implements IVsShellPropertyEvents
    Implements IVsSolutionEvents
    Implements IVsUpdateSolutionEvents2

    Private visualEffectsAllowed As Integer = 0
    Private shellPropertyChangesCookie As UInteger
    Private solutionEventsCookie As UInteger
    Private updateSolutionEventsCookie As UInteger

    Private vsShell As IVsShell = Nothing
    Private solution As IVsSolution2 = Nothing
    Private sbm As IVsSolutionBuildManager2 = Nothing

    Private toolWindow As BuildProgressToolWindow = Nothing

    Private totalProjects As Double = 0
    Private currProject As Double = 0

    ''' <summary>
    ''' Default constructor of the package.
    ''' Inside this method you can place any initialization code that does not require 
    ''' any Visual Studio service because at this point the package object is created but 
    ''' not sited yet inside Visual Studio environment. The place to do all the other 
    ''' initialization is the Initialize method.
    ''' </summary>
    Public Sub New()
    End Sub

    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        MyBase.Dispose(disposing)

        ' Unadvise all events
        If Not vsShell Is Nothing And shellPropertyChangesCookie <> 0 Then
            vsShell.UnadviseShellPropertyChanges(shellPropertyChangesCookie)
        End If

        If Not sbm Is Nothing And updateSolutionEventsCookie <> 0 Then
            sbm.UnadviseUpdateSolutionEvents(updateSolutionEventsCookie)
        End If

        If Not solution Is Nothing And solutionEventsCookie <> 0 Then
            solution.UnadviseSolutionEvents(solutionEventsCookie)
        End If
    End Sub

    ''' <summary>
    ''' This function is called when the user clicks the menu item that shows the 
    ''' tool window. See the Initialize method to see how the menu item is associated to 
    ''' this function using the OleMenuCommandService service and the MenuCommand class.
    ''' </summary>
    Private Sub ShowToolWindow(ByVal sender As Object, ByVal e As EventArgs)
        ' Get the instance number 0 of this tool window. This window is single instance so this instance
        ' is actually the only one.
        ' The last flag is set to true so that if the tool window does not exists it will be created.
        Dim window As ToolWindowPane = Me.FindToolWindow(GetType(BuildProgressToolWindow), 0, True)
        If (window Is Nothing) Or (window.Frame Is Nothing) Then
            Throw New NotSupportedException(Resources.CanNotCreateWindow)
        End If

        Dim windowFrame As IVsWindowFrame = TryCast(window.Frame, IVsWindowFrame)
        Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show())
    End Sub


    ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
    ' Overridden Package Implementation
#Region "Package Members"

    ''' <summary>
    ''' Initialization of the package; this method is called right after the package is sited, so this is the place
    ''' where you can put all the initialization code that rely on services provided by VisualStudio.
    ''' </summary>
    Protected Overrides Sub Initialize()
        MyBase.Initialize()

        ' Add our command handlers for menu (commands must exist in the .vsct file)
        Dim mcs As OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), OleMenuCommandService)
        If Not mcs Is Nothing Then
            ' Create the command for the tool window
            Dim toolwndCommandID As New CommandID(GuidList.guidBuildProgressBarCmdSet, CInt(PkgCmdIDList.cmdidBuildProgress))
            Dim menuToolWin As New MenuCommand(New EventHandler(AddressOf ShowToolWindow), toolwndCommandID)
            mcs.AddCommand(menuToolWin)
        End If

        ' Get shell object
        vsShell = CType(ServiceProvider.GlobalProvider.GetService(GetType(SVsShell)), IVsShell)
        If Not vsShell Is Nothing Then
            ' Initialize VisualEffects value, so themes can determine if various effects are supported by the environment
            Dim effectsAllowed As Object = Nothing
            If ErrorHandler.Succeeded(vsShell.GetProperty(__VSSPROPID4.VSSPROPID_VisualEffectsAllowed, effectsAllowed)) Then
                ' VSSPROPID_VisualEffectsAllowed is a VT_I4 property, so casting to int should be safe
                Debug.Assert(TypeOf effectsAllowed Is Integer, "VSSPROPID_VisualEffectsAllowed should be of type int")
                Me.visualEffectsAllowed = CType(effectsAllowed, Integer)
            Else
                Debug.Fail("Failed to get the VSSPROPID_VisualEffectsAllowed property value.")
            End If

            ' Subscribe to shell property changes to update VisualEffects values
            vsShell.AdviseShellPropertyChanges(Me, shellPropertyChangesCookie)
        End If

        ' Get solution
        solution = CType(ServiceProvider.GlobalProvider.GetService(GetType(SVsSolution)), IVsSolution2)
        If Not solution Is Nothing Then
            ' Get count of any currently loaded projects
            Dim count As Object = Nothing
            solution.GetProperty(__VSPROPID.VSPROPID_ProjectCount, count)
            totalProjects = count

            ' Register for solution events
            solution.AdviseSolutionEvents(Me, solutionEventsCookie)
        End If

        ' Get solution build manager
        sbm = CType(ServiceProvider.GlobalProvider.GetService(GetType(SVsSolutionBuildManager)), IVsSolutionBuildManager2)
        If Not sbm Is Nothing Then
            sbm.AdviseUpdateSolutionEvents(Me, updateSolutionEventsCookie)
        End If

        ' Get tool window
        If toolWindow Is Nothing Then
            toolWindow = Me.FindToolWindow(GetType(BuildProgressToolWindow), 0, True)
        End If

        ' Set initial value of EffectsEnabled in tool window
        toolWindow.EffectsEnabled = visualEffectsAllowed <> 0

    End Sub
#End Region

    Function OnShellPropertyChange(ByVal propid As Integer, ByVal var As Object) As Integer Implements IVsShellPropertyEvents.OnShellPropertyChange
        ' If the propid matches the value we're interested in, get the new value
        If propid = __VSSPROPID4.VSSPROPID_VisualEffectsAllowed Then
            Me.visualEffectsAllowed = (CType(var, Integer))
            toolWindow.EffectsEnabled = visualEffectsAllowed <> 0
        End If

        Return VSConstants.S_OK
    End Function

    Public Function OnAfterCloseSolution(ByVal pUnkReserved As Object) As Integer Implements VisualStudio.Shell.Interop.IVsSolutionEvents.OnAfterCloseSolution
        ' Reset progress bar after closing solution
        totalProjects = 0

        If Not toolWindow Is Nothing Then
            toolWindow.BarText = ""
            toolWindow.Progress = 0
        End If

        Return VSConstants.S_OK
    End Function

    Public Function OnAfterLoadProject(ByVal pStubHierarchy As VisualStudio.Shell.Interop.IVsHierarchy, ByVal pRealHierarchy As VisualStudio.Shell.Interop.IVsHierarchy) As Integer Implements VisualStudio.Shell.Interop.IVsSolutionEvents.OnAfterLoadProject
        Return VSConstants.S_OK
    End Function

    Public Function OnAfterOpenProject(ByVal pHierarchy As VisualStudio.Shell.Interop.IVsHierarchy, ByVal fAdded As Integer) As Integer Implements VisualStudio.Shell.Interop.IVsSolutionEvents.OnAfterOpenProject
        ' Track the number of open projects
        totalProjects += 1

        Return VSConstants.S_OK
    End Function

    Public Function OnAfterOpenSolution(ByVal pUnkReserved As Object, ByVal fNewSolution As Integer) As Integer Implements VisualStudio.Shell.Interop.IVsSolutionEvents.OnAfterOpenSolution
        Return VSConstants.S_OK
    End Function

    Public Function OnBeforeCloseProject(ByVal pHierarchy As VisualStudio.Shell.Interop.IVsHierarchy, ByVal fRemoved As Integer) As Integer Implements VisualStudio.Shell.Interop.IVsSolutionEvents.OnBeforeCloseProject
        ' Track the number of open projects
        totalProjects -= 1

        Return VSConstants.S_OK
    End Function

    Public Function OnBeforeCloseSolution(ByVal pUnkReserved As Object) As Integer Implements VisualStudio.Shell.Interop.IVsSolutionEvents.OnBeforeCloseSolution
        Return VSConstants.S_OK
    End Function

    Public Function OnBeforeUnloadProject(ByVal pRealHierarchy As VisualStudio.Shell.Interop.IVsHierarchy, ByVal pStubHierarchy As VisualStudio.Shell.Interop.IVsHierarchy) As Integer Implements VisualStudio.Shell.Interop.IVsSolutionEvents.OnBeforeUnloadProject
        Return VSConstants.S_OK
    End Function

    Public Function OnQueryCloseProject(ByVal pHierarchy As VisualStudio.Shell.Interop.IVsHierarchy, ByVal fRemoving As Integer, ByRef pfCancel As Integer) As Integer Implements VisualStudio.Shell.Interop.IVsSolutionEvents.OnQueryCloseProject
        Return VSConstants.S_OK
    End Function

    Public Function OnQueryCloseSolution(ByVal pUnkReserved As Object, ByRef pfCancel As Integer) As Integer Implements VisualStudio.Shell.Interop.IVsSolutionEvents.OnQueryCloseSolution
        Return VSConstants.S_OK
    End Function

    Public Function OnQueryUnloadProject(ByVal pRealHierarchy As VisualStudio.Shell.Interop.IVsHierarchy, ByRef pfCancel As Integer) As Integer Implements VisualStudio.Shell.Interop.IVsSolutionEvents.OnQueryUnloadProject
        Return VSConstants.S_OK
    End Function

    Public Function OnActiveProjectCfgChange(ByVal pIVsHierarchy As VisualStudio.Shell.Interop.IVsHierarchy) As Integer Implements VisualStudio.Shell.Interop.IVsUpdateSolutionEvents.OnActiveProjectCfgChange
        Return VSConstants.S_OK
    End Function

    Public Function UpdateSolution_Begin(ByRef pfCancelUpdate As Integer) As Integer Implements VisualStudio.Shell.Interop.IVsUpdateSolutionEvents.UpdateSolution_Begin
        ' This method is called when the entire solution starts to build.
        currProject = 0
        If Not toolWindow Is Nothing Then
            toolWindow.BarText = "Starting build."
            toolWindow.Progress = 0
        End If

        Return VSConstants.S_OK
    End Function

    Public Function UpdateSolution_Cancel() As Integer Implements VisualStudio.Shell.Interop.IVsUpdateSolutionEvents.UpdateSolution_Cancel
        Return VSConstants.S_OK
    End Function

    Public Function UpdateSolution_Done(ByVal fSucceeded As Integer, ByVal fModified As Integer, ByVal fCancelCommand As Integer) As Integer Implements VisualStudio.Shell.Interop.IVsUpdateSolutionEvents.UpdateSolution_Done
        ' This method is called when the entire solution is done building.
        If Not toolWindow Is Nothing Then
            If fSucceeded <> 0 Then
                toolWindow.BarText = "Build completed."
                toolWindow.Progress = 1
            ElseIf fCancelCommand <> 0 Then
                toolWindow.BarText = "Build canceled."
            End If
        End If

        Return VSConstants.S_OK
    End Function

    Public Function UpdateSolution_StartUpdate(ByRef pfCancelUpdate As Integer) As Integer Implements VisualStudio.Shell.Interop.IVsUpdateSolutionEvents.UpdateSolution_StartUpdate
        Return VSConstants.S_OK
    End Function

    Public Function OnActiveProjectCfgChange1(ByVal pIVsHierarchy As VisualStudio.Shell.Interop.IVsHierarchy) As Integer Implements VisualStudio.Shell.Interop.IVsUpdateSolutionEvents2.OnActiveProjectCfgChange
        Return VSConstants.S_OK
    End Function

    Public Function UpdateProjectCfg_Begin(ByVal pHierProj As VisualStudio.Shell.Interop.IVsHierarchy, ByVal pCfgProj As VisualStudio.Shell.Interop.IVsCfg, ByVal pCfgSln As VisualStudio.Shell.Interop.IVsCfg, ByVal dwAction As UInteger, ByRef pfCancel As Integer) As Integer Implements VisualStudio.Shell.Interop.IVsUpdateSolutionEvents2.UpdateProjectCfg_Begin
        ' This method is called when a specific project begins building.  Based on the total number of open projects, we can estimate
        ' how far along in the build we are.
        currProject += 1

        If Not toolWindow Is Nothing Then
            ' Update progress bar text
            Dim o As Object = Nothing
            pHierProj.GetProperty(VSConstants.VSITEMID.Root, __VSHPROPID.VSHPROPID_Name, o)
            Dim name As String = o
            toolWindow.BarText = "Building " + name + "..."

            ' Update bar value; estimate percentage completion
            If totalProjects <> 0 Then
                Dim value As Double = currProject / (totalProjects * 2)
                toolWindow.Progress = value
            End If
        End If

        Return VSConstants.S_OK

    End Function

    Public Function UpdateProjectCfg_Done(ByVal pHierProj As VisualStudio.Shell.Interop.IVsHierarchy, ByVal pCfgProj As VisualStudio.Shell.Interop.IVsCfg, ByVal pCfgSln As VisualStudio.Shell.Interop.IVsCfg, ByVal dwAction As UInteger, ByVal fSuccess As Integer, ByVal fCancel As Integer) As Integer Implements VisualStudio.Shell.Interop.IVsUpdateSolutionEvents2.UpdateProjectCfg_Done
        ' This method is called when a specific project finishes building.  Move the progress bar value accordginly.
        If Not toolWindow Is Nothing Then
            toolWindow.BarText = ""

            If totalProjects <> 0 Then
                currProject += 1
                toolWindow.Progress = currProject / (totalProjects * 2)
            End If
        End If

        Return VSConstants.S_OK
    End Function

    Public Function UpdateSolution_Begin1(ByRef pfCancelUpdate As Integer) As Integer Implements VisualStudio.Shell.Interop.IVsUpdateSolutionEvents2.UpdateSolution_Begin
        Return VSConstants.S_OK
    End Function

    Public Function UpdateSolution_Cancel1() As Integer Implements VisualStudio.Shell.Interop.IVsUpdateSolutionEvents2.UpdateSolution_Cancel
        Return VSConstants.S_OK
    End Function

    Public Function UpdateSolution_Done1(ByVal fSucceeded As Integer, ByVal fModified As Integer, ByVal fCancelCommand As Integer) As Integer Implements VisualStudio.Shell.Interop.IVsUpdateSolutionEvents2.UpdateSolution_Done
        Return VSConstants.S_OK
    End Function

    Public Function UpdateSolution_StartUpdate1(ByRef pfCancelUpdate As Integer) As Integer Implements VisualStudio.Shell.Interop.IVsUpdateSolutionEvents2.UpdateSolution_StartUpdate
        Return VSConstants.S_OK
    End Function
End Class
