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
Imports System.Diagnostics
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.ComponentModel.Design
Imports Microsoft.Win32
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.Shell
Imports MsVsShell = Microsoft.VisualStudio.Shell
Imports EnvDTE
Imports System.Collections.Generic
Imports Microsoft.VisualStudio

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    '///////////////////////////////////////////////////////////////////////////
    ' VSPackage
    <MsVsShell.InstalledProductRegistration("#100", "#102", "1.0.0.0", IconResourceID:=400), MsVsShell.ProvideMenuResource(1000, 1), Guid(GuidStrings.GuidVSPackagePkg), MsVsShell.ProvideAutoLoad("ADFC4E66-0397-11D1-9F4E-00A0C911004F"), CLSCompliant(False)> _
    Public NotInheritable Class VSPackage
        Inherits MsVsShell.Package
        Public Sub New()
            Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", Me.ToString()))
            Factory.ServiceProvider = Me
            Factory.GetBuildManager().CreatePerUserFilesAsNecessary()
        End Sub

        Public Shared Function GetResourceString(ByVal resourceName As String) As String
            Dim resourceValue As String = Nothing
            Dim resourceManager As IVsResourceManager = CType(GetGlobalService(GetType(SVsResourceManager)), IVsResourceManager)
            Dim packageGuid As Guid = GetType(VSPackage).GUID
            Dim hr As Integer = resourceManager.LoadResourceString(packageGuid, -1, resourceName, resourceValue)
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(hr)
            Return resourceValue
        End Function

        Public Shared Function GetResourceString(ByVal resourceID As Integer) As String
            Return GetResourceString(String.Format(CultureInfo.InvariantCulture, "#{0}", resourceID))
        End Function

        '///////////////////////////////////////////////////////////////////////////
        ' Overriden Package Implementation.
#Region "Package Members"

        Protected Overrides Sub Initialize()
            Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", Me.ToString()))
            MyBase.Initialize()

            ' Add our command handlers for menu (commands must exist in the .vsct file).
            Dim mcs As MsVsShell.OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), MsVsShell.OleMenuCommandService)
            If Not Nothing Is mcs Then
                ' Create the command for the menu item.
                Dim menuCommandID As New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidConfig)))
                Dim menuItem As New OleMenuCommand(New EventHandler(AddressOf MenuItemCallback), menuCommandID)
                AddHandler menuItem.BeforeQueryStatus, AddressOf QueryStatus
                mcs.AddCommand(menuItem)

                ' Create the commands for the tasklist toolbar.
                menuCommandID = New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidStopScan)))
                menuItem = New OleMenuCommand(New EventHandler(AddressOf StopScan), menuCommandID)
                menuItem.Enabled = False
                mcs.AddCommand(menuItem)

                menuCommandID = New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidRepeatLastScan)))
                menuItem = New OleMenuCommand(New EventHandler(AddressOf RepeatLastScan), menuCommandID)
                menuItem.Enabled = False
                mcs.AddCommand(menuItem)
            End If

            Factory.GetBuildManager().IsListeningToBuildEvents = True
            AddHandler Factory.GetBuildManager().BuildStarted, AddressOf BuildManager_BuildStarted
            AddHandler Factory.GetBuildManager().BuildStopped, AddressOf BuildManager_BuildStopped
            AddHandler Factory.GetBackgroundScanner().Started, AddressOf BackgroundScanner_Started
            AddHandler Factory.GetBackgroundScanner().Stopped, AddressOf BackgroundScanner_Stopped
        End Sub

        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing Then
                    Trace.WriteLine(String.Format(CultureInfo.CurrentCulture, "Entering Dispose() of: {0}", Me.ToString()))
                    Factory.GetBuildManager().IsListeningToBuildEvents = False
                    Factory.CleanupFactory()
                    GC.SuppressFinalize(Me)
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

#End Region

        Private Sub BuildManager_BuildStopped()
            Dim mcs As MsVsShell.OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), MsVsShell.OleMenuCommandService)
            If mcs IsNot Nothing Then
                mcs.FindCommand(New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidConfig)))).Enabled = True
            End If
        End Sub

        Private Sub BuildManager_BuildStarted()
            Dim mcs As MsVsShell.OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), MsVsShell.OleMenuCommandService)
            If mcs IsNot Nothing Then
                mcs.FindCommand(New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidConfig)))).Enabled = False
            End If
        End Sub

        Private Sub BackgroundScanner_Stopped(ByVal sender As Object, ByVal e As EventArgs)
            Dim mcs As MsVsShell.OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), MsVsShell.OleMenuCommandService)
            If mcs IsNot Nothing Then
                Dim stopCommand As MenuCommand = mcs.FindCommand(New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidStopScan))))
                stopCommand.Enabled = False
                stopCommand.Checked = False
                mcs.FindCommand(New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidRepeatLastScan)))).Enabled = True
            End If
        End Sub

        Private Sub BackgroundScanner_Started(ByVal sender As Object, ByVal e As EventArgs)
            Dim mcs As MsVsShell.OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), MsVsShell.OleMenuCommandService)
            If mcs IsNot Nothing Then
                mcs.FindCommand(New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidStopScan)))).Enabled = True
                mcs.FindCommand(New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidRepeatLastScan)))).Enabled = False
            End If
        End Sub

        Private Sub StopScan(ByVal sender As Object, ByVal e As EventArgs)
            Factory.GetBackgroundScanner().StopIfRunning(False)
            If Factory.GetBackgroundScanner().IsRunning Then
                Dim mcs As MsVsShell.OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), MsVsShell.OleMenuCommandService)
                mcs.FindCommand(New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidStopScan)))).Checked = True
            End If
        End Sub

        Private Sub RepeatLastScan(ByVal sender As Object, ByVal e As EventArgs)
            Factory.GetBackgroundScanner().RepeatLast()
        End Sub

        ''' <summary>
        ''' This function is the callback used to execute a command when the a menu item is clicked.
        ''' </summary>
        Private Sub MenuItemCallback(ByVal sender As Object, ByVal e As EventArgs)
            Factory.GetDialog().Invoke(ProjectUtilities.GetProjectsOfCurrentSelections())
        End Sub

        ''' <summary>
        ''' This function is the callback used to query the status of the Code Sweep... project menu item.
        ''' </summary>
        Private Sub QueryStatus(ByVal sender As Object, ByVal e As EventArgs)
            Dim menuVisible As Boolean = False
            Dim dte As DTE = TryCast(GetService(GetType(DTE)), DTE)
            For Each dteProject As EnvDTE.Project In dte.Solution.Projects
                Dim SolutionFolder As New Guid(EnvDTE.Constants.vsProjectKindSolutionItems)
                Dim MiscellaneousFiles As New Guid(EnvDTE.Constants.vsProjectKindMisc)
                Dim currentProjectKind As New Guid(dteProject.Kind)
                If currentProjectKind <> SolutionFolder AndAlso currentProjectKind <> MiscellaneousFiles Then
                    menuVisible = True
                End If
            Next dteProject

            Dim menuCommand As OleMenuCommand = TryCast(sender, OleMenuCommand)
            menuCommand.Visible = menuVisible
        End Sub
    End Class
End Namespace
