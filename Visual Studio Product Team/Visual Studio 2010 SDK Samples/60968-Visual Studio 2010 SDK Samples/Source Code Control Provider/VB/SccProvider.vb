'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports System
Imports System.IO
Imports System.Resources
Imports System.Diagnostics
Imports System.Globalization
Imports System.Collections
Imports System.Collections.Generic
Imports System.Runtime.InteropServices
Imports System.ComponentModel.Design
Imports Microsoft.Win32
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio
Imports System.Runtime.Serialization.Formatters.Binary

Imports MsVsShell = Microsoft.VisualStudio.Shell
Imports ErrorHandler = Microsoft.VisualStudio.ErrorHandler

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider
	'///////////////////////////////////////////////////////////////////////////
	' SccProvider
	' Register the package to have information displayed in Help/About dialog box
	' Declare that resources for the package are to be found in the managed assembly resources, and not in a satellite dll
	' Register the resource ID of the CTMENU section (generated from compiling the VSCT file), so the IDE will know how to merge this package's menus with the rest of the IDE when "devenv /setup" is run
	' The menu resource ID needs to match the ResourceName number defined in the csproj project file in the VSCTCompile section
	' Everytime the version number changes VS will automatically update the menus on startup; if the version doesn't change, you will need to run manually "devenv /setup /rootsuffix:Exp" to see VSCT changes reflected in IDE
	' Register a sample options page visible as Tools/Options/SourceControl/SampleOptionsPage when the provider is active
	' Register a sample tool window visible only when the provider is active
	' Register the source control provider's service (implementing IVsScciProvider interface)
	' Register the source control provider to be visible in Tools/Options/SourceControl/Plugin dropdown selector
	' Pre-load the package when the command UI context is asserted (the provider will be automatically loaded after restarting the shell if it was active last time the shell was shutdown)
	' Register the key used for persisting solution properties, so the IDE will know to load the source control package when opening a controlled solution containing properties written by this package
	' Declare the package guid
    <MsVsShell.DefaultRegistryRoot("Software\Microsoft\VisualStudio\10.0Exp"), MsVsShell.PackageRegistration(UseManagedResourcesOnly:=True), MsVsShell.ProvideMenuResource(1000, 1), MsVsShell.ProvideOptionPageAttribute(GetType(SccProviderOptions), "Source Control", "Sample Options Page", 106, 107, False), ProvideToolsOptionsPageVisibility("Source Control", "Sample Options Page", GuidStrings.GuidSccProvider), MsVsShell.ProvideToolWindow(GetType(SccProviderToolWindow)), MsVsShell.ProvideToolWindowVisibility(GetType(SccProviderToolWindow), GuidStrings.GuidSccProvider), MsVsShell.ProvideService(GetType(SccProviderService), ServiceName:="Source Control Sample Provider Service"), ProvideSourceControlProvider("Managed Source Control Sample Provider", "#100"), MsVsShell.ProvideAutoLoad(GuidStrings.GuidSccProvider), ProvideSolutionProps(Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider.SccProvider._strSolutionPersistanceKey), Guid(GuidStrings.GuidSccProviderPkg)> _
    Public NotInheritable Class SccProvider ' We'll write properties in the solution file to track when solution is controlled; the interface needs to be implemented by the package object
        Inherits MsVsShell.Package
        Implements IOleCommandTarget, IVsPersistSolutionProps
        ' The service provider implemented by the package.
        Private sccService As SccProviderService = Nothing
        ' The name of this provider (to be written in solution and project files).
        ' As a best practice, to be sure the provider has an unique name, a guid like the provider guid can be used as a part of the name.
        Private Const _strProviderName As String = "Sample Source Control Provider:{8C902FDC-CE93-49b7-BF66-0144082555F9}"
        ' The name of the solution section used to persist provider options (should be unique).
        Public Const _strSolutionPersistanceKey As String = "SampleSourceControlProviderSolutionProperties"
        ' The name of the section in the solution user options file used to persist user-specific options (should be unique, shorter than 31 characters and without dots).
        Private Const _strSolutionUserOptionsKey As String = "SampleSourceControlProvider"
        ' The names of the properties stored by the provider in the solution file.
        Private Const _strSolutionControlledProperty As String = "SolutionIsControlled"
        Private Const _strSolutionBindingsProperty As String = "SolutionBindings"
        ' Whether the solution was just added to source control and the provider needs to saved source control properties in the solution file when the solution is saved.
        Private _solutionHasDirtyProps As Boolean = False
        ' The guid of solution folders.
        Private guidSolutionFolderProject As New Guid(&H2150E333, &H8FDC, &H42A3, &H94, &H74, &H1A, &H39, &H56, &HD4, &H6D, &HE8)

        Public Sub New()
            Trace.WriteLine(String.Format(CultureInfo.CurrentUICulture, "Entering constructor for: {0}", Me.ToString()))

            ' The provider implements the IVsPersistSolutionProps interface which is derived from IVsPersistSolutionOpts,
            ' The base class MsVsShell.Package also implements IVsPersistSolutionOpts, so we're overriding its functionality
            ' Therefore, to persist user options in the suo file we will not use the set of AddOptionKey/OnLoadOptions/OnSaveOptions 
            ' set of functions, but instead we'll use the IVsPersistSolutionProps functions directly.
        End Sub

        '///////////////////////////////////////////////////////////////////////////
        ' SccProvider Package Implementation
#Region "Package Members"

        Public Shadows Function GetService(ByVal serviceType As Type) As Object
            Return MyBase.GetService(serviceType)
        End Function

        Protected Overrides Sub Initialize()
            Trace.WriteLine(String.Format(CultureInfo.CurrentUICulture, "Entering Initialize() of: {0}", Me.ToString()))
            MyBase.Initialize()

            ' Proffer the source control service implemented by the provider.
            sccService = New SccProviderService(Me)
            CType(Me, IServiceContainer).AddService(GetType(SccProviderService), sccService, True)

            ' Add our command handlers for menu (commands must exist in the .vsct file).
            Dim mcs As MsVsShell.OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), MsVsShell.OleMenuCommandService)
            If mcs IsNot Nothing Then
                ' ToolWindow Command.
                Dim cmd As New System.ComponentModel.Design.CommandID(GuidList.guidSccProviderCmdSet, CommandId.icmdViewToolWindow)
                Dim menuCmd As New MenuCommand(New EventHandler(AddressOf Exec_icmdViewToolWindow), cmd)
                mcs.AddCommand(menuCmd)

                ' ToolWindow's ToolBar Command.
                cmd = New System.ComponentModel.Design.CommandID(GuidList.guidSccProviderCmdSet, CommandId.icmdToolWindowToolbarCommand)
                menuCmd = New MenuCommand(New EventHandler(AddressOf Exec_icmdToolWindowToolbarCommand), cmd)
                mcs.AddCommand(menuCmd)

                ' Source control menu commmads.
                cmd = New System.ComponentModel.Design.CommandID(GuidList.guidSccProviderCmdSet, CommandId.icmdAddToSourceControl)
                menuCmd = New MenuCommand(New EventHandler(AddressOf Exec_icmdAddToSourceControl), cmd)
                mcs.AddCommand(menuCmd)

                cmd = New System.ComponentModel.Design.CommandID(GuidList.guidSccProviderCmdSet, CommandId.icmdCheckin)
                menuCmd = New MenuCommand(New EventHandler(AddressOf Exec_icmdCheckin), cmd)
                mcs.AddCommand(menuCmd)

                cmd = New System.ComponentModel.Design.CommandID(GuidList.guidSccProviderCmdSet, CommandId.icmdCheckout)
                menuCmd = New MenuCommand(New EventHandler(AddressOf Exec_icmdCheckout), cmd)
                mcs.AddCommand(menuCmd)

                cmd = New System.ComponentModel.Design.CommandID(GuidList.guidSccProviderCmdSet, CommandId.icmdUseSccOffline)
                menuCmd = New MenuCommand(New EventHandler(AddressOf Exec_icmdUseSccOffline), cmd)
                mcs.AddCommand(menuCmd)
            End If

            ' Register the provider with the source control manager.
            ' If the package is to become active, this will also callback on OnActiveStateChange and the menu commands will be enabled.
            Dim rscp As IVsRegisterScciProvider = CType(GetService(GetType(IVsRegisterScciProvider)), IVsRegisterScciProvider)
            rscp.RegisterSourceControlProvider(GuidList.guidSccProvider)
        End Sub

        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            Trace.WriteLine(String.Format(CultureInfo.CurrentUICulture, "Entering Dispose() of: {0}", Me.ToString()))

            sccService.Dispose()

            MyBase.Dispose(disposing)
        End Sub

        ' This function is called by the IVsSccProvider service implementation when the active state of the provider changes.
        ' If the package needs to refresh UI or perform other tasks, this is a good place to add the code.
        Public Sub OnActiveStateChange()
        End Sub

        ' Returns the name of the source control provider.
        Public ReadOnly Property ProviderName() As String
            Get
                Return _strProviderName
            End Get
        End Property

#End Region

        '--------------------------------------------------------------------------------
        ' IVsPersistSolutionProps specific functions
        '--------------------------------------------------------------------------------
#Region "IVsPersistSolutionProps interface functions"

        Public Function OnProjectLoadFailure(<InAttribute()> ByVal pStubHierarchy As IVsHierarchy, <InAttribute()> ByVal pszProjectName As String, <InAttribute()> ByVal pszProjectMk As String, <InAttribute()> ByVal pszKey As String) As Integer Implements IVsPersistSolutionProps.OnProjectLoadFailure
            Return VSConstants.S_OK
        End Function

        Public Function QuerySaveSolutionProps(<InAttribute()> ByVal pHierarchy As IVsHierarchy, <OutAttribute()> ByVal pqsspSave As VSQUERYSAVESLNPROPS()) As Integer Implements IVsPersistSolutionProps.QuerySaveSolutionProps
            ' This function is called by the IDE to determine if something needs to be saved in the solution.
            ' If the package returns that it has dirty properties, the shell will callback on SaveSolutionProps.

            ' We will write solution properties only for the solution.
            ' A provider may consider writing in the solution project-binding-properties for each controlled project
            ' that could help it locating the projects in the store during operations like OpenFromSourceControl.
            If (Not sccService.IsProjectControlled(Nothing)) Then
                pqsspSave(0) = VSQUERYSAVESLNPROPS.QSP_HasNoProps
            Else
                If SolutionHasDirtyProps Then
                    pqsspSave(0) = VSQUERYSAVESLNPROPS.QSP_HasDirtyProps
                Else
                    pqsspSave(0) = VSQUERYSAVESLNPROPS.QSP_HasNoDirtyProps
                End If
            End If

            Return VSConstants.S_OK
        End Function

        Public Function SaveSolutionProps(<InAttribute()> ByVal pHierarchy As IVsHierarchy, <InAttribute()> ByVal pPersistence As IVsSolutionPersistence) As Integer Implements IVsPersistSolutionProps.SaveSolutionProps
            ' This function gets called by the shell after determining the package has dirty props.
            ' The package will pass in the key under which it wants to save its properties, 
            ' and the IDE will call back on WriteSolutionProps.

            ' The properties will be saved in the Pre-Load section.
            ' When the solution will be reopened, the IDE will call our package to load them back before the projects in the solution are actually open.
            ' This could help if the source control package needs to persist information like projects translation tables, that should be read from the suo file
            ' and should be available by the time projects are opened and the shell start calling IVsSccEnlistmentPathTranslation functions.
            pPersistence.SavePackageSolutionProps(1, Nothing, Me, _strSolutionPersistanceKey)

            ' Once we saved our props, the solution is not dirty anymore.
            SolutionHasDirtyProps = False

            Return VSConstants.S_OK
        End Function

        Public Function WriteSolutionProps(<InAttribute()> ByVal pHierarchy As IVsHierarchy, <InAttribute()> ByVal pszKey As String, <InAttribute()> ByVal pPropBag As IPropertyBag) As Integer Implements IVsPersistSolutionProps.WriteSolutionProps
            ' The package will only save one property in the solution, to indicate that solution is controlled.

            ' A good provider may need to persist as solution properties the controlled status of projects and their locations, too.
            ' If an operation like OpenFromSourceControl has sense for the provider, and the user has selected to open from 
            ' source control the solution file, the bindings written as solution properties will help identifying where the 
            ' project files are in the source control database. The source control provider can download the project files 
            ' before they are needed by the IDE to be opened.
            Dim strControlled As String = True.ToString()
            Dim obj As Object = strControlled
            pPropBag.Write(_strSolutionControlledProperty, obj)
            Dim strSolutionLocation As String = "<Solution Location In Database>"
            obj = strSolutionLocation
            pPropBag.Write(_strSolutionBindingsProperty, obj)

            Return VSConstants.S_OK
        End Function

        Public Function ReadSolutionProps(<InAttribute()> ByVal pHierarchy As IVsHierarchy, <InAttribute()> ByVal pszProjectName As String, <InAttribute()> ByVal pszProjectMk As String, <InAttribute()> ByVal pszKey As String, <InAttribute()> ByVal fPreLoad As Integer, <InAttribute()> ByVal pPropBag As IPropertyBag) As Integer Implements IVsPersistSolutionProps.ReadSolutionProps
            ' This function gets called by the shell when a solution controlled by this provider is opened in IDE.
            ' The shell encounters the _strSolutionPersistanceKey section in the solution, and based based on 
            ' registration info written by ProvideSolutionProps identifies this package as the section owner, 
            ' loads this package if necessary and call the package to read the persisted solution options.

            If _strSolutionPersistanceKey.CompareTo(pszKey) = 0 Then
                ' We were called to read the key written by this source control provider.
                ' First thing to do: register the source control provider with the source control manager.
                ' This allows the scc manager to switch the active source control provider if necessary,
                ' and set this provider active; the provider will be later called to provide source control services for this solution.
                ' (This is how automatic source control provider switching on solution opening is implemented)
                Dim rscp As IVsRegisterScciProvider = CType(GetService(GetType(IVsRegisterScciProvider)), IVsRegisterScciProvider)
                rscp.RegisterSourceControlProvider(GuidList.guidSccProvider)

                ' Now we can read all the data and store it in memory.
                ' The read data will be used when the solution has completed opening.
                Dim pVar As Object = Nothing
                pPropBag.Read(_strSolutionControlledProperty, pVar, Nothing, 0, Nothing)
                If pVar.ToString().CompareTo(True.ToString()) = 0 Then
                    pPropBag.Read(_strSolutionBindingsProperty, pVar, Nothing, 0, Nothing)
                    sccService.LoadingControlledSolutionLocation = pVar.ToString()
                End If
            End If
            Return VSConstants.S_OK
        End Function

        Public Function SaveUserOptions(<InAttribute()> ByVal pPersistence As IVsSolutionPersistence) As Integer Implements IVsPersistSolutionProps.SaveUserOptions
            ' This function gets called by the shell when the SUO file is saved.
            ' The provider calls the shell back to let it know which options keys it will use in the suo file.
            ' The shell will create a stream for the section of interest, and will call back the provider on 
            ' IVsPersistSolutionProps.WriteUserOptions() to save specific options under the specified key.
            Dim pfResult As Integer = 0
            sccService.AnyItemsUnderSourceControl(pfResult)
            If pfResult > 0 Then
                pPersistence.SavePackageUserOpts(Me, _strSolutionUserOptionsKey)
            End If
            Return VSConstants.S_OK
        End Function

        Public Function WriteUserOptions(<InAttribute()> ByVal pOptionsStream As IStream, <InAttribute()> ByVal pszKey As String) As Integer Implements IVsPersistSolutionProps.WriteUserOptions
            ' This function gets called by the shell to let the package write user options under the specified key.
            ' The key was declared in SaveUserOptions(), when the shell started saving the suo file.
            Debug.Assert(pszKey.CompareTo(_strSolutionUserOptionsKey) = 0, "The shell called to read an key that doesn't belong to this package")

            Dim hashProjectsUserData As New Hashtable()
            Dim solution As IVsSolution = CType(GetService(GetType(SVsSolution)), IVsSolution)
            ' Get the list of controllable projects.
            Dim hash As Hashtable = GetLoadedControllableProjectsEnum()
            ' Add the solution to the controllable projects list.
            Dim solHier As IVsHierarchy = CType(GetService(GetType(SVsSolution)), IVsHierarchy)
            hash(solHier) = True
            ' Collect all projects that are controlled and offline.
            For Each pHier As IVsHierarchy In hash.Keys
                If sccService.IsProjectControlled(pHier) AndAlso sccService.IsProjectOffline(pHier) Then
                    ' The information we'll persist in the suo file needs to be usable if the solution is moved in a diffrent location
                    ' therefore we'll store project names as known by the solution (mostly relativized to the solution's folder).
                    Dim projUniqueName As String = String.Empty
                    If solution.GetUniqueNameOfProject(pHier, projUniqueName) = VSConstants.S_OK Then
                        hashProjectsUserData(projUniqueName) = True
                    End If
                End If
            Next pHier

            ' The easiest way to read/write the data of interest is by using a binary formatter class.
            ' This way, we can write a map of information about projects with one call 
            ' (each element in the map needs to be serializable though).
            ' The alternative is to write binary data in any byte format you'd like using pOptionsStream.Write.
            Dim pStream As New DataStreamFromComStream(pOptionsStream)
            Dim formatter As New BinaryFormatter()
            formatter.Serialize(pStream, hashProjectsUserData)

            Return VSConstants.S_OK
        End Function

        Public Function LoadUserOptions(<InAttribute()> ByVal pPersistence As IVsSolutionPersistence, <InAttribute()> ByVal grfLoadOpts As UInteger) As Integer Implements IVsPersistSolutionProps.LoadUserOptions
            ' This function gets called by the shell when a solution is opened and the SUO file is read.
            ' Note this can be during opening a new solution, or may be during merging of 2 solutions.
            ' The provider calls the shell back to let it know which options keys from the suo file were written by this provider.
            ' If the shell will find in the suo file a section that belong to this package, it will create a stream, 
            ' and will call back the provider on IVsPersistSolutionProps.ReadUserOptions() to read specific options 
            ' under that option key.
            pPersistence.LoadPackageUserOpts(Me, _strSolutionUserOptionsKey)
            Return VSConstants.S_OK
        End Function

        Public Function ReadUserOptions(<InAttribute()> ByVal pOptionsStream As IStream, <InAttribute()> ByVal pszKey As String) As Integer Implements IVsPersistSolutionProps.ReadUserOptions
            ' This function is called by the shell if the _strSolutionUserOptionsKey section declared
            ' in LoadUserOptions() as being written by this package has been found in the suo file. 
            ' Note this can be during opening a new solution, or may be during merging of 2 solutions.
            ' A good source control provider may need to persist this data until OnAfterOpenSolution or OnAfterMergeSolution is called.

            ' The easiest way to read/write the data of interest is by using a binary formatter class.
            Dim pStream As New DataStreamFromComStream(pOptionsStream)
            Dim hashProjectsUserData As New Hashtable()
            If pStream.Length > 0 Then
                Dim formatter As New BinaryFormatter()
                hashProjectsUserData = TryCast(formatter.Deserialize(pStream), Hashtable)
            End If

            Dim solution As IVsSolution = CType(GetService(GetType(SVsSolution)), IVsSolution)
            For Each projUniqueName As String In hashProjectsUserData.Keys
                ' If this project is recognizable as part of the solution.
                Dim pHier As IVsHierarchy = Nothing
                If solution.GetProjectOfUniqueName(projUniqueName, pHier) = VSConstants.S_OK AndAlso pHier IsNot Nothing Then
                    sccService.ToggleOfflineStatus(pHier)
                End If
            Next projUniqueName

            Return VSConstants.S_OK
        End Function

#End Region

#Region "Source Control Command Enabling"

        ''' <summary>
        ''' The shell call this function to know if a menu item should be visible and
        ''' if it should be enabled/disabled.
        ''' Note that this function will only be called when an instance of this editor
        ''' is open.
        ''' </summary>
        ''' <param name="guidCmdGroup">Guid describing which set of command the current command(s) belong to</param>
        ''' <param name="cCmds">Number of command which status are being asked for</param>
        ''' <param name="prgCmds">Information for each command</param>
        ''' <param name="pCmdText">Used to dynamically change the command text</param>
        ''' <returns>HRESULT</returns>
        Public Function QueryStatus(ByRef guidCmdGroup As Guid, ByVal cCmds As UInteger, ByVal prgCmds As OLECMD(), ByVal pCmdText As System.IntPtr) As Integer Implements IOleCommandTarget.QueryStatus
            Debug.Assert(cCmds = 1, "Multiple commands")
            Debug.Assert(prgCmds IsNot Nothing, "NULL argument")

            If (prgCmds Is Nothing) Then
                Return VSConstants.E_INVALIDARG
            End If

            ' Filter out commands that are not defined by this package.
            If guidCmdGroup <> GuidList.guidSccProviderCmdSet Then
                Return CInt(Fix(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED))

            End If

            Dim cmdf As OLECMDF = OLECMDF.OLECMDF_SUPPORTED

            ' All source control commands needs to be hidden and disabled when the provider is not active.
            If (Not sccService.Active) Then
                cmdf = cmdf Or OLECMDF.OLECMDF_INVISIBLE
                cmdf = cmdf And Not (OLECMDF.OLECMDF_ENABLED)

                prgCmds(0).cmdf = CUInt(cmdf)
                Return VSConstants.S_OK
            End If

            ' Process our Commands.
            Select Case prgCmds(0).cmdID
                Case CommandId.icmdAddToSourceControl
                    cmdf = cmdf Or QueryStatus_icmdAddToSourceControl()

                Case CommandId.icmdCheckin
                    cmdf = cmdf Or QueryStatus_icmdCheckin()

                Case CommandId.icmdCheckout
                    cmdf = cmdf Or QueryStatus_icmdCheckout()

                Case CommandId.icmdUseSccOffline
                    cmdf = cmdf Or QueryStatus_icmdUseSccOffline()

                Case CommandId.icmdViewToolWindow, CommandId.icmdToolWindowToolbarCommand
                    ' These commmands are always enabled when the provider is active.
                    cmdf = cmdf Or OLECMDF.OLECMDF_ENABLED

                Case Else
                    Return CInt(Fix(Microsoft.VisualStudio.OLE.Interop.Constants.OLECMDERR_E_NOTSUPPORTED))
            End Select

            prgCmds(0).cmdf = CUInt(cmdf)

            Return VSConstants.S_OK
        End Function

        Private Function QueryStatus_icmdCheckin() As OLECMDF
            If (Not IsThereASolution()) Then
                Return OLECMDF.OLECMDF_INVISIBLE
            End If

            Dim files As IList(Of String) = GetSelectedFilesInControlledProjects()
            For Each file As String In files
                Dim status As SourceControlStatus = sccService.GetFileStatus(file)
                If status = SourceControlStatus.scsCheckedIn Then
                    Continue For
                End If

                If status = SourceControlStatus.scsCheckedOut Then
                    Return OLECMDF.OLECMDF_ENABLED
                End If

                ' If the file is uncontrolled, enable the command only if the file is part of a controlled project.
                Dim nodes As IList(Of VSITEMSELECTION) = sccService.GetControlledProjectsContainingFile(file)
                If nodes.Count > 0 Then
                    Return OLECMDF.OLECMDF_ENABLED
                End If
            Next file

            Return OLECMDF.OLECMDF_SUPPORTED
        End Function

        Private Function QueryStatus_icmdCheckout() As OLECMDF
            If (Not IsThereASolution()) Then
                Return OLECMDF.OLECMDF_INVISIBLE
            End If

            Dim files As IList(Of String) = GetSelectedFilesInControlledProjects()
            For Each file As String In files
                If sccService.GetFileStatus(file) = SourceControlStatus.scsCheckedIn Then
                    Return OLECMDF.OLECMDF_ENABLED
                End If
            Next file

            Return OLECMDF.OLECMDF_SUPPORTED
        End Function

        Private Function QueryStatus_icmdAddToSourceControl() As OLECMDF
            If (Not IsThereASolution()) Then
                Return OLECMDF.OLECMDF_INVISIBLE
            End If

            Dim sel As IList(Of VSITEMSELECTION) = GetSelectedNodes()
            Dim isSolutionSelected As Boolean = False
            Dim hash As Hashtable = GetSelectedHierarchies(sel, isSolutionSelected)

            ' The command is enabled when the solution is selected and is uncontrolled yet
            ' or when an uncontrolled project is selected.
            If isSolutionSelected Then
                If (Not sccService.IsProjectControlled(Nothing)) Then
                    Return OLECMDF.OLECMDF_ENABLED
                End If
            Else
                For Each pHier As IVsHierarchy In hash.Keys
                    If (Not sccService.IsProjectControlled(pHier)) Then
                        Return OLECMDF.OLECMDF_ENABLED
                    End If
                Next pHier
            End If

            Return OLECMDF.OLECMDF_SUPPORTED
        End Function

        Private Function QueryStatus_icmdUseSccOffline() As OLECMDF
            If (Not IsThereASolution()) Then
                Return OLECMDF.OLECMDF_INVISIBLE
            End If

            Dim sel As IList(Of VSITEMSELECTION) = GetSelectedNodes()
            Dim isSolutionSelected As Boolean = False
            Dim hash As Hashtable = GetSelectedHierarchies(sel, isSolutionSelected)
            If isSolutionSelected Then
                Dim solHier As IVsHierarchy = CType(GetService(GetType(SVsSolution)), IVsHierarchy)
                hash(solHier) = Nothing
            End If

            Dim selectedOffline As Boolean = False
            Dim selectedOnline As Boolean = False
            For Each pHier As IVsHierarchy In hash.Keys
                If (Not sccService.IsProjectControlled(pHier)) Then
                    ' If a project is not controlled, set both flags to disalbe the command.
                    selectedOnline = True
                    selectedOffline = selectedOnline
                End If

                If sccService.IsProjectOffline(pHier) Then
                    selectedOffline = True
                Else
                    selectedOnline = True
                End If
            Next pHier

            ' For mixed selection, or if nothing is selected, disable the command.
            If selectedOnline AndAlso selectedOffline OrElse (Not selectedOnline) AndAlso (Not selectedOffline) Then
                Return OLECMDF.OLECMDF_SUPPORTED
            End If

            If selectedOffline Then
                Return OLECMDF.OLECMDF_ENABLED Or (OLECMDF.OLECMDF_LATCHED)
            Else
                Return OLECMDF.OLECMDF_ENABLED Or (OLECMDF.OLECMDF_ENABLED)
            End If

        End Function

#End Region

#Region "Source Control Commands Execution"

        Private Sub Exec_icmdCheckin(ByVal sender As Object, ByVal e As EventArgs)
            If (Not IsThereASolution()) Then
                Return
            End If

            Dim selectedNodes As IList(Of VSITEMSELECTION) = Nothing
            Dim files As IList(Of String) = GetSelectedFilesInControlledProjects(selectedNodes)
            For Each file As String In files
                Dim status As SourceControlStatus = sccService.GetFileStatus(file)
                If status = SourceControlStatus.scsCheckedOut Then
                    sccService.CheckinFile(file)
                ElseIf status = SourceControlStatus.scsUncontrolled Then
                    sccService.AddFileToSourceControl(file)
                End If
            Next file

            ' Now refresh the selected nodes' glyphs.
            RefreshNodesGlyphs(selectedNodes)
        End Sub

        Private Sub Exec_icmdCheckout(ByVal sender As Object, ByVal e As EventArgs)
            If (Not IsThereASolution()) Then
                Return
            End If

            Dim selectedNodes As IList(Of VSITEMSELECTION) = Nothing
            Dim files As IList(Of String) = GetSelectedFilesInControlledProjects(selectedNodes)
            For Each file As String In files
                Dim status As SourceControlStatus = sccService.GetFileStatus(file)
                If status = SourceControlStatus.scsCheckedIn Then
                    sccService.CheckoutFile(file)
                End If
            Next file

            ' Now refresh the selected nodes' glyphs.
            RefreshNodesGlyphs(selectedNodes)
        End Sub

        Private Sub Exec_icmdAddToSourceControl(ByVal sender As Object, ByVal e As EventArgs)
            If (Not IsThereASolution()) Then
                Debug.Assert(False, "The command should have been disabled")
                Return
            End If

            Dim sel As IList(Of VSITEMSELECTION) = GetSelectedNodes()
            Dim isSolutionSelected As Boolean = False
            Dim hash As Hashtable = GetSelectedHierarchies(sel, isSolutionSelected)

            Dim hashUncontrolledProjects As New Hashtable()
            If isSolutionSelected Then
                ' When the solution is selected, all the uncontrolled projects in the solution will be added to scc.
                hash = GetLoadedControllableProjectsEnum()
            End If

            For Each pHier As IVsHierarchy In hash.Keys
                If (Not sccService.IsProjectControlled(pHier)) Then
                    hashUncontrolledProjects(pHier) = True
                End If
            Next pHier

            sccService.AddProjectsToSourceControl(hashUncontrolledProjects, isSolutionSelected)
        End Sub

        Private Sub Exec_icmdUseSccOffline(ByVal sender As Object, ByVal e As EventArgs)
            If (Not IsThereASolution()) Then
                Return
            End If

            Dim sel As IList(Of VSITEMSELECTION) = GetSelectedNodes()
            Dim isSolutionSelected As Boolean = False
            Dim hash As Hashtable = GetSelectedHierarchies(sel, isSolutionSelected)
            If isSolutionSelected Then
                Dim solHier As IVsHierarchy = CType(GetService(GetType(SVsSolution)), IVsHierarchy)
                hash(solHier) = Nothing
            End If

            For Each pHier As IVsHierarchy In hash.Keys
                ' If a project is not controlled, skip it.
                If (Not sccService.IsProjectControlled(pHier)) Then
                    Continue For
                End If

                sccService.ToggleOfflineStatus(pHier)
            Next pHier
        End Sub

        ' The function can be used to bring back the provider's toolwindow if it was previously closed.
        Private Sub Exec_icmdViewToolWindow(ByVal sender As Object, ByVal e As EventArgs)
            Dim window As MsVsShell.ToolWindowPane = Me.FindToolWindow(GetType(SccProviderToolWindow), 0, True)
            Dim windowFrame As IVsWindowFrame = Nothing
            If window IsNot Nothing AndAlso window.Frame IsNot Nothing Then
                windowFrame = CType(window.Frame, IVsWindowFrame)
            End If
            If windowFrame IsNot Nothing Then
                ErrorHandler.ThrowOnFailure(windowFrame.Show())
            End If
        End Sub

        Private Sub Exec_icmdToolWindowToolbarCommand(ByVal sender As Object, ByVal e As EventArgs)
            Dim window As SccProviderToolWindow = CType(Me.FindToolWindow(GetType(SccProviderToolWindow), 0, True), SccProviderToolWindow)

            If window IsNot Nothing Then
                window.ToolWindowToolbarCommand()
            End If
        End Sub

#End Region

#Region "Source Control Utility Functions"

        ''' <summary>
        ''' Returns whether suorce control properties must be saved in the solution file.
        ''' </summary>
        Public Property SolutionHasDirtyProps() As Boolean
            Get
                Return _solutionHasDirtyProps
            End Get
            Set(ByVal value As Boolean)
                _solutionHasDirtyProps = value
            End Set
        End Property

        ''' <summary>
        ''' Returns a list of controllable projects in the solution.
        ''' </summary>
        Private Function GetLoadedControllableProjectsEnum() As Hashtable
            Dim mapHierarchies As New Hashtable()

            Dim sol As IVsSolution = CType(GetService(GetType(SVsSolution)), IVsSolution)
            Dim rguidEnumOnlyThisType As New Guid()
            Dim ppenum As IEnumHierarchies = Nothing
            ErrorHandler.ThrowOnFailure(sol.GetProjectEnum(CUInt(__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION), rguidEnumOnlyThisType, ppenum))

            Dim rgelt As IVsHierarchy() = New IVsHierarchy(0) {}
            Dim pceltFetched As UInteger = 0
            Do While ppenum.Next(1, rgelt, pceltFetched) = VSConstants.S_OK AndAlso pceltFetched = 1
                Dim sccProject2 As IVsSccProject2 = TryCast(rgelt(0), IVsSccProject2)
                If sccProject2 IsNot Nothing Then
                    mapHierarchies(rgelt(0)) = True
                End If
            Loop

            Return mapHierarchies
        End Function

        ''' <summary>
        ''' Checks whether a solution exist.
        ''' </summary>
        ''' <returns>True if a solution was created.</returns>
        Private Function IsThereASolution() As Boolean
            Return (GetSolutionFileName() IsNot Nothing)
        End Function

        ''' <summary>
        ''' Gets the list of selected controllable project hierarchies.
        ''' </summary>
        ''' <returns>True if a solution was created.</returns>
        Private Function GetSelectedHierarchies(ByRef sel As IList(Of VSITEMSELECTION), <System.Runtime.InteropServices.Out()> ByRef solutionSelected As Boolean) As Hashtable
            ' Initialize output arguments.
            solutionSelected = False

            Dim mapHierarchies As New Hashtable()
            For Each vsItemSel As VSITEMSELECTION In sel
                If vsItemSel.pHier Is Nothing OrElse (TryCast(vsItemSel.pHier, IVsSolution)) IsNot Nothing Then
                    solutionSelected = True
                End If

                ' See if the selected hierarchy implements the IVsSccProject2 interface.
                ' Exclude from selection projects like FTP web projects that don't support SCC.
                Dim sccProject2 As IVsSccProject2 = TryCast(vsItemSel.pHier, IVsSccProject2)
                If sccProject2 IsNot Nothing Then
                    mapHierarchies(vsItemSel.pHier) = True
                End If
            Next vsItemSel

            Return mapHierarchies
        End Function

        ''' <summary>
        ''' Gets the list of directly selected VSITEMSELECTION objects.
        ''' </summary>
        ''' <returns>A list of VSITEMSELECTION objects</returns>
        Private Function GetSelectedNodes() As IList(Of VSITEMSELECTION)
            ' Retrieve shell interface in order to get current selection.
            Dim monitorSelection As IVsMonitorSelection = TryCast(Me.GetService(GetType(IVsMonitorSelection)), IVsMonitorSelection)
            Debug.Assert(monitorSelection IsNot Nothing, "Could not get the IVsMonitorSelection object from the services exposed by this project")
            If monitorSelection Is Nothing Then
                Throw New InvalidOperationException()
            End If

            Dim selectedNodes As List(Of VSITEMSELECTION) = New List(Of VSITEMSELECTION)()
            Dim hierarchyPtr As IntPtr = IntPtr.Zero
            Dim selectionContainer As IntPtr = IntPtr.Zero
            Try
                ' Get the current project hierarchy, project item, and selection container for the current selection.
                ' If the selection spans multiple hierachies, hierarchyPtr is Zero.
                Dim itemid As UInteger
                Dim multiItemSelect As IVsMultiItemSelect = Nothing
                ErrorHandler.ThrowOnFailure(monitorSelection.GetCurrentSelection(hierarchyPtr, itemid, multiItemSelect, selectionContainer))

                If itemid <> VSConstants.VSITEMID_SELECTION Then
                    ' We only care if there are nodes selected in the tree.
                    If itemid <> VSConstants.VSITEMID_NIL Then
                        If hierarchyPtr = IntPtr.Zero Then
                            ' Solution is selected.
                            Dim vsItemSelection As VSITEMSELECTION
                            vsItemSelection.pHier = Nothing
                            vsItemSelection.itemid = itemid
                            selectedNodes.Add(vsItemSelection)
                        Else
                            Dim hierarchy As IVsHierarchy = CType(Marshal.GetObjectForIUnknown(hierarchyPtr), IVsHierarchy)
                            ' Single item selection.
                            Dim vsItemSelection As VSITEMSELECTION
                            vsItemSelection.pHier = hierarchy
                            vsItemSelection.itemid = itemid
                            selectedNodes.Add(vsItemSelection)
                        End If
                    End If
                Else
                    If multiItemSelect IsNot Nothing Then
                        ' This is a multiple item selection.

                        ' Get number of items selected and also determine if the items are located in more than one hierarchy.
                        Dim numberOfSelectedItems As UInteger
                        Dim isSingleHierarchyInt As Integer
                        ErrorHandler.ThrowOnFailure(multiItemSelect.GetSelectionInfo(numberOfSelectedItems, isSingleHierarchyInt))
                        Dim isSingleHierarchy As Boolean = (isSingleHierarchyInt <> 0)

                        ' Now loop all selected items and add them to the list. 
                        Debug.Assert(numberOfSelectedItems > 0, "Bad number of selected itemd")
                        If numberOfSelectedItems > 0 Then
                            Dim vsItemSelections As VSITEMSELECTION() = New VSITEMSELECTION(CInt(numberOfSelectedItems - 1)) {}
                            ErrorHandler.ThrowOnFailure(multiItemSelect.GetSelectedItems(0, numberOfSelectedItems, vsItemSelections))
                            For Each vsItemSelection As VSITEMSELECTION In vsItemSelections
                                selectedNodes.Add(vsItemSelection)
                            Next vsItemSelection
                        End If
                    End If
                End If
            Finally
                If hierarchyPtr <> IntPtr.Zero Then
                    Marshal.Release(hierarchyPtr)
                End If
                If selectionContainer <> IntPtr.Zero Then
                    Marshal.Release(selectionContainer)
                End If
            End Try

            Return selectedNodes
        End Function

        ''' <summary>
        ''' Returns a list of source controllable files in the selection (recursive).
        ''' </summary>
        Private Function GetSelectedFilesInControlledProjects() As IList(Of String)
            Dim selectedNodes As IList(Of VSITEMSELECTION) = Nothing
            Return GetSelectedFilesInControlledProjects(selectedNodes)
        End Function

        ''' <summary>
        ''' Returns a list of source controllable files in the selection (recursive).
        ''' </summary>
        Private Function GetSelectedFilesInControlledProjects(<System.Runtime.InteropServices.Out()> ByRef selectedNodes As IList(Of VSITEMSELECTION)) As IList(Of String)
            Dim sccFiles As New List(Of String)()

            selectedNodes = GetSelectedNodes()
            Dim isSolutionSelected As Boolean = False
            Dim hash As Hashtable = GetSelectedHierarchies(selectedNodes, isSolutionSelected)
            If isSolutionSelected Then
                ' Replace the selection with the root items of all controlled projects.
                selectedNodes.Clear()
                Dim hashControllable As Hashtable = GetLoadedControllableProjectsEnum()
                For Each pHier As IVsHierarchy In hashControllable.Keys
                    If sccService.IsProjectControlled(pHier) Then
                        Dim vsItemSelection As VSITEMSELECTION
                        vsItemSelection.pHier = pHier
                        vsItemSelection.itemid = VSConstants.VSITEMID_ROOT
                        selectedNodes.Add(vsItemSelection)
                    End If
                Next pHier

                ' Add the solution file to the list.
                If sccService.IsProjectControlled(Nothing) Then
                    Dim solHier As IVsHierarchy = CType(GetService(GetType(SVsSolution)), IVsHierarchy)
                    Dim vsItemSelection As VSITEMSELECTION
                    vsItemSelection.pHier = solHier
                    vsItemSelection.itemid = VSConstants.VSITEMID_ROOT
                    selectedNodes.Add(vsItemSelection)
                End If
            End If

            ' Now look in the rest of selection and accumulate scc files.
            For Each vsItemSel As VSITEMSELECTION In selectedNodes
                Dim pscp2 As IVsSccProject2 = TryCast(vsItemSel.pHier, IVsSccProject2)
                If pscp2 Is Nothing Then
                    ' Solution case.
                    sccFiles.Add(GetSolutionFileName())
                Else
                    Dim nodefilesrec As IList(Of String) = GetProjectFiles(pscp2, vsItemSel.itemid)
                    For Each file As String In nodefilesrec
                        sccFiles.Add(file)
                    Next file
                End If
            Next vsItemSel

            Return sccFiles
        End Function

        ''' <summary>
        ''' Returns a list of source controllable files associated with the specified node.
        ''' </summary>
        Public Function GetNodeFiles(ByVal hier As IVsHierarchy, ByVal itemid As UInteger) As IList(Of String)
            Dim pscp2 As IVsSccProject2 = TryCast(hier, IVsSccProject2)
            Return GetNodeFiles(pscp2, itemid)
        End Function

        ''' <summary>
        ''' Returns a list of source controllable files associated with the specified node.
        ''' </summary>
        Private Function GetNodeFiles(ByVal pscp2 As IVsSccProject2, ByVal itemid As UInteger) As IList(Of String)
            ' NOTE: the function returns only a list of files, containing both regular files and special files
            ' If you want to hide the special files (similar with solution explorer), you may need to return 
            ' the special files in a hastable (key=master_file, values=special_file_list).

            ' Initialize output parameters.
            Dim sccFiles As New List(Of String)()
            If pscp2 IsNot Nothing Then
                Dim pathStr As CALPOLESTR() = New CALPOLESTR(0) {}
                Dim flags As CADWORD() = New CADWORD(0) {}

                If pscp2.GetSccFiles(itemid, pathStr, flags) = VSConstants.S_OK Then
                    For elemIndex As Integer = 0 To CInt(pathStr(0).cElems - 1)
                        Dim pathIntPtr As IntPtr = Marshal.ReadIntPtr(pathStr(0).pElems, elemIndex * IntPtr.Size)
                        Dim path As String = Marshal.PtrToStringAuto(pathIntPtr)

                        sccFiles.Add(path)

                        ' See if there are special files.
                        If flags.Length > 0 AndAlso flags(0).cElems > 0 Then
                            Dim flag As Integer = Marshal.ReadInt32(flags(0).pElems, elemIndex * IntPtr.Size)

                            If flag <> 0 Then
                                ' We have special files.
                                Dim specialFiles As CALPOLESTR() = New CALPOLESTR(0) {}
                                Dim specialFlags As CADWORD() = New CADWORD(0) {}

                                If (pscp2.GetSccSpecialFiles(itemid, path, specialFiles, specialFlags) = VSConstants.S_OK) Then
                                    For i As Integer = 0 To CInt(specialFiles(0).cElems - 1)
                                        Dim specialPathIntPtr As IntPtr = Marshal.ReadIntPtr(specialFiles(0).pElems, i * IntPtr.Size)
                                        Dim specialPath As String = Marshal.PtrToStringAuto(specialPathIntPtr)

                                        sccFiles.Add(specialPath)
                                        Marshal.FreeCoTaskMem(specialPathIntPtr)
                                    Next i

                                    If specialFiles(0).cElems > 0 Then
                                        Marshal.FreeCoTaskMem(specialFiles(0).pElems)
                                    End If
                                End If
                            End If
                        End If

                        Marshal.FreeCoTaskMem(pathIntPtr)
                    Next elemIndex
                    If pathStr(0).cElems > 0 Then
                        Marshal.FreeCoTaskMem(pathStr(0).pElems)
                    End If
                End If
            End If

            Return sccFiles
        End Function

        ''' <summary>
        ''' Refreshes the glyphs of the specified hierarchy nodes.
        ''' </summary>
        Public Sub RefreshNodesGlyphs(ByVal selectedNodes As IList(Of VSITEMSELECTION))
            For Each vsItemSel As VSITEMSELECTION In selectedNodes
                Dim sccProject2 As IVsSccProject2 = TryCast(vsItemSel.pHier, IVsSccProject2)
                If vsItemSel.itemid = VSConstants.VSITEMID_ROOT Then
                    If sccProject2 Is Nothing Then
                        ' Note: The solution's hierarchy does not implement IVsSccProject2, IVsSccProject interfaces
                        ' It may be a pain to treat the solution as special case everywhere; a possible workaround is 
                        ' to implement a solution-wrapper class, that will implement IVsSccProject2, IVsSccProject and
                        ' IVsHierarhcy interfaces, and that could be used in provider's code wherever a solution is needed.
                        ' This approach could unify the treatment of solution and projects in the provider's code.

                        ' Until then, solution is treated as special case.
                        Dim rgpszFullPaths As String() = New String(0) {}
                        rgpszFullPaths(0) = GetSolutionFileName()
                        Dim rgsiGlyphs As VsStateIcon() = New VsStateIcon(0) {}
                        Dim rgdwSccStatus As UInteger() = New UInteger(0) {}
                        sccService.GetSccGlyph(1, rgpszFullPaths, rgsiGlyphs, rgdwSccStatus)

                        ' Set the solution's glyph directly in the hierarchy.
                        Dim solHier As IVsHierarchy = CType(GetService(GetType(SVsSolution)), IVsHierarchy)
                        solHier.SetProperty(VSConstants.VSITEMID_ROOT, CInt(Fix(__VSHPROPID.VSHPROPID_StateIconIndex)), rgsiGlyphs(0))
                    Else
                        ' Refresh all the glyphs in the project; the project will call back GetSccGlyphs() 
                        ' with the files for each node that will need new glyph.
                        sccProject2.SccGlyphChanged(0, Nothing, Nothing, Nothing)
                    End If
                Else
                    ' It may be easier/faster to simply refresh all the nodes in the project, 
                    ' and let the project call back on GetSccGlyphs, but just for the sake of the demo, 
                    ' let's refresh ourselves only one node at a time.
                    Dim sccFiles As IList(Of String) = GetNodeFiles(sccProject2, vsItemSel.itemid)

                    ' We'll use for the node glyph just the Master file's status (ignoring special files of the node).
                    If sccFiles.Count > 0 Then
                        Dim rgpszFullPaths As String() = New String(0) {}
                        rgpszFullPaths(0) = sccFiles(0)
                        Dim rgsiGlyphs As VsStateIcon() = New VsStateIcon(0) {}
                        Dim rgdwSccStatus As UInteger() = New UInteger(0) {}
                        sccService.GetSccGlyph(1, rgpszFullPaths, rgsiGlyphs, rgdwSccStatus)

                        Dim rguiAffectedNodes As UInteger() = New UInteger(0) {}
                        rguiAffectedNodes(0) = vsItemSel.itemid
                        sccProject2.SccGlyphChanged(1, rguiAffectedNodes, rgsiGlyphs, rgdwSccStatus)
                    End If
                End If
            Next vsItemSel
        End Sub


        ''' <summary>
        ''' Returns the filename of the solution.
        ''' </summary>
        Public Function GetSolutionFileName() As String
            Dim sol As IVsSolution = CType(GetService(GetType(SVsSolution)), IVsSolution)
            Dim solutionDirectory As String = String.Empty
            Dim solutionFile As String = String.Empty
            Dim solutionUserOptions As String = String.Empty
            If sol.GetSolutionInfo(solutionDirectory, solutionFile, solutionUserOptions) = VSConstants.S_OK Then
                Return solutionFile
            Else
                Return Nothing
            End If
        End Function

        ''' <summary>
        ''' Returns the filename of the specified controllable project .
        ''' </summary>
        Public Function GetProjectFileName(ByVal pscp2Project As IVsSccProject2) As String
            ' Note: Solution folders return currently a name like "NewFolder1{1DBFFC2F-6E27-465A-A16A-1AECEA0B2F7E}.storage"
            ' Your provider may consider returning the solution file as the project name for the solution, if it has to persist some properties in the "project file"
            ' UNDONE: What to return for web projects? They return a folder name, not a filename! Consider returning a pseudo-project filename instead of folder.

            Dim hierProject As IVsHierarchy = CType(pscp2Project, IVsHierarchy)
            Dim project As IVsProject = CType(pscp2Project, IVsProject)

            ' Attempt to get first the filename controlled by the root node.
            Dim sccFiles As IList(Of String) = GetNodeFiles(pscp2Project, VSConstants.VSITEMID_ROOT)
            If sccFiles.Count > 0 AndAlso sccFiles(0) IsNot Nothing AndAlso sccFiles(0).Length > 0 Then
                Return sccFiles(0)
            End If

            ' If that failed, attempt to get a name from the IVsProject interface.
            Dim bstrMKDocument As String = String.Empty
            If project.GetMkDocument(VSConstants.VSITEMID_ROOT, bstrMKDocument) = VSConstants.S_OK AndAlso bstrMKDocument IsNot Nothing AndAlso bstrMKDocument.Length > 0 Then
                Return bstrMKDocument
            End If

            ' If that failes, attempt to get the filename from the solution.
            Dim sol As IVsSolution = CType(GetService(GetType(SVsSolution)), IVsSolution)
            Dim uniqueName As String = String.Empty
            If sol.GetUniqueNameOfProject(hierProject, uniqueName) = VSConstants.S_OK AndAlso uniqueName IsNot Nothing AndAlso uniqueName.Length > 0 Then
                ' uniqueName may be a full-path or may be relative to the solution's folder.
                If uniqueName.Length > 2 AndAlso uniqueName.Chars(1) = ":"c Then
                    Return uniqueName
                End If

                ' Try to get the solution's folder and relativize the project name to it.
                Dim solutionDirectory As String = String.Empty
                Dim solutionFile As String = String.Empty
                Dim solutionUserOptions As String = String.Empty
                If sol.GetSolutionInfo(solutionDirectory, solutionFile, solutionUserOptions) = VSConstants.S_OK Then
                    uniqueName = solutionDirectory & "\" & uniqueName

                    ' UNDONE: eliminate possible "..\\.." from path.
                    Return uniqueName
                End If
            End If

            ' If that failed, attempt to get the project name from. 
            Dim bstrName As String = String.Empty
            If hierProject.GetCanonicalName(VSConstants.VSITEMID_ROOT, bstrName) = VSConstants.S_OK Then
                Return bstrName
            End If

            ' If everything we tried fail, return null string.
            Return Nothing
        End Function

        Private Sub DebugWalkingNode(ByVal pHier As IVsHierarchy, ByVal itemid As UInteger)
            Dim [property] As Object = Nothing
            If pHier.GetProperty(itemid, CInt(Fix(__VSHPROPID.VSHPROPID_Name)), [property]) = VSConstants.S_OK Then
                Trace.WriteLine(String.Format(CultureInfo.CurrentUICulture, "Walking hierarchy node: {0}", CType([property], String)))
            End If
        End Sub

        ''' <summary>
        ''' Gets the list of ItemIDs that are nodes in the specified project.
        ''' </summary>
        Private Function GetProjectItems(ByVal pHier As IVsHierarchy) As IList(Of UInteger)
            ' Start with the project root and walk all expandable nodes in the project.
            Return GetProjectItems(pHier, VSConstants.VSITEMID_ROOT)
        End Function

        ''' <summary>
        ''' Gets the list of ItemIDs that are nodes in the specified project, starting with the specified item.
        ''' </summary>
        Private Function GetProjectItems(ByVal pHier As IVsHierarchy, ByVal startItemid As UInteger) As IList(Of UInteger)
            Dim projectNodes As List(Of UInteger) = New List(Of UInteger)()

            ' The method does a breadth-first traversal of the project's hierarchy tree.
            Dim nodesToWalk As Queue(Of UInteger) = New Queue(Of UInteger)()
            nodesToWalk.Enqueue(startItemid)

            Do While nodesToWalk.Count > 0
                Dim node As UInteger = nodesToWalk.Dequeue()
                projectNodes.Add(node)

                DebugWalkingNode(pHier, node)

                Dim [property] As Object = Nothing
                If pHier.GetProperty(node, CInt(Fix(__VSHPROPID.VSHPROPID_FirstChild)), [property]) = VSConstants.S_OK Then
                    Dim childnode As UInteger = Nothing
                    If CType([property], Integer) = 0 Then
                        childnode = 0
                    Else
                        childnode = CType(4294967294 - CType([property], Integer), UInteger)
                    End If
                    If childnode = VSConstants.VSITEMID_NIL Then
                        Continue Do
                    End If

                    DebugWalkingNode(pHier, childnode)

                    If (pHier.GetProperty(childnode, CInt(Fix(__VSHPROPID.VSHPROPID_Expandable)), [property]) = VSConstants.S_OK AndAlso CType([property], Integer) <> 0) OrElse (pHier.GetProperty(childnode, CInt(Fix(__VSHPROPID2.VSHPROPID_Container)), [property]) = VSConstants.S_OK AndAlso CType([property], Boolean)) Then
                        nodesToWalk.Enqueue(childnode)
                    Else
                        projectNodes.Add(childnode)
                    End If

                    Do While pHier.GetProperty(childnode, CInt(Fix(__VSHPROPID.VSHPROPID_NextSibling)), [property]) = VSConstants.S_OK
                        childnode = CType(4294967294 - CType([property], Integer), UInteger)
                        If childnode = VSConstants.VSITEMID_NIL Then
                            Exit Do
                        End If

                        DebugWalkingNode(pHier, childnode)

                        If (pHier.GetProperty(childnode, CInt(Fix(__VSHPROPID.VSHPROPID_Expandable)), [property]) = VSConstants.S_OK AndAlso CType([property], Integer) <> 0) OrElse (pHier.GetProperty(childnode, CInt(Fix(__VSHPROPID2.VSHPROPID_Container)), [property]) = VSConstants.S_OK AndAlso CType([property], Boolean)) Then
                            nodesToWalk.Enqueue(childnode)
                        Else
                            projectNodes.Add(childnode)
                        End If
                    Loop
                End If

            Loop

            Return projectNodes
        End Function

        ''' <summary>
        ''' Gets the list of source controllable files in the specified project.
        ''' </summary>
        Public Function GetProjectFiles(ByVal pscp2Project As IVsSccProject2) As IList(Of String)
            Return GetProjectFiles(pscp2Project, VSConstants.VSITEMID_ROOT)
        End Function

        ''' <summary>
        ''' Gets the list of source controllable files in the specified project.
        ''' </summary>
        Public Function GetProjectFiles(ByVal pscp2Project As IVsSccProject2, ByVal startItemId As UInteger) As IList(Of String)
            Dim projectFiles As New List(Of String)()
            Dim hierProject As IVsHierarchy = CType(pscp2Project, IVsHierarchy)
            Dim projectItems As IList(Of UInteger) = GetProjectItems(hierProject, startItemId)

            For Each itemid As UInteger In projectItems
                Dim sccFiles As IList(Of String) = GetNodeFiles(pscp2Project, itemid)
                For Each file As String In sccFiles
                    projectFiles.Add(file)
                Next file
            Next itemid

            Return projectFiles
        End Function

        ''' <summary>
        ''' Checks whether the provider is invoked in command line mode.
        ''' </summary>
        Public Function InCommandLineMode() As Boolean
            Dim shell As IVsShell = CType(GetService(GetType(SVsShell)), IVsShell)
            Dim pvar As Object = Nothing
            If shell.GetProperty(CInt(Fix(__VSSPROPID.VSSPROPID_IsInCommandLineMode)), pvar) = VSConstants.S_OK AndAlso CBool(pvar) Then
                Return True
            End If

            Return False
        End Function

        ''' <summary>
        ''' Checks whether the specified project is a solution folder.
        ''' </summary>
        Public Function IsSolutionFolderProject(ByVal pHier As IVsHierarchy) As Boolean
            Dim pFileFormat As IPersistFileFormat = TryCast(pHier, IPersistFileFormat)
            If pFileFormat IsNot Nothing Then
                Dim guidClassID As Guid
                If pFileFormat.GetClassID(guidClassID) = VSConstants.S_OK AndAlso guidClassID.CompareTo(guidSolutionFolderProject) = 0 Then
                    Return True
                End If
            End If

            Return False
        End Function

        ''' <summary>
        ''' Returns a list of solution folders projects in the solution.
        ''' </summary>
        Public Function GetSolutionFoldersEnum() As Hashtable
            Dim mapHierarchies As New Hashtable()

            Dim sol As IVsSolution = CType(GetService(GetType(SVsSolution)), IVsSolution)
            Dim rguidEnumOnlyThisType As Guid = guidSolutionFolderProject
            Dim ppenum As IEnumHierarchies = Nothing
            ErrorHandler.ThrowOnFailure(sol.GetProjectEnum(CUInt(__VSENUMPROJFLAGS.EPF_LOADEDINSOLUTION), rguidEnumOnlyThisType, ppenum))

            Dim rgelt As IVsHierarchy() = New IVsHierarchy(0) {}
            Dim pceltFetched As UInteger = 0
            Do While ppenum.Next(1, rgelt, pceltFetched) = VSConstants.S_OK AndAlso pceltFetched = 1
                mapHierarchies(rgelt(0)) = True
            Loop

            Return mapHierarchies
        End Function



#End Region
    End Class
End Namespace