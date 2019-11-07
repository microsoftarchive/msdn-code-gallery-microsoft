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
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.Shell
Imports EnvDTE
Imports System.Collections.Generic
Imports Microsoft.VisualStudio
Imports System.Runtime.Remoting
Imports System.Runtime.Remoting.Channels
Imports System.Runtime.Remoting.Channels.Tcp

Namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
    <
    InstalledProductRegistration("#100", "#102", "1.0.0.0"),
    Guid(GuidStrings.GuidVSPackagePkg),
    ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string),
    ProvideMenuResource("Menus.ctmenu", 1),
    CLSCompliant(False),
    ProvideBindingPath()
    >
    Public NotInheritable Class VSPackage
        Inherits Package
        Public Sub New()
            Factory.ServiceProvider = Me
            Factory.GetBuildManager().CreatePerUserFilesAsNecessary()
        End Sub

        '///////////////////////////////////////////////////////////////////////////
        ' Overriden Package Implementation.
#Region "Package Members"

        Protected Overrides Sub Initialize()
            MyBase.Initialize()

            ' Add our command handlers for menu (commands must exist in the .vsct file).
            Dim mcs As OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), OleMenuCommandService)
            If mcs IsNot Nothing Then
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
            Else
                Debug.Fail("Failed to get IMenuCommandService service.")
            End If

            Factory.GetBuildManager().IsListeningToBuildEvents = True
            AddHandler Factory.GetBuildManager().BuildStarted, AddressOf BuildManager_BuildStarted
            AddHandler Factory.GetBuildManager().BuildStopped, AddressOf BuildManager_BuildStopped
            AddHandler Factory.GetBackgroundScanner().Started, AddressOf BackgroundScanner_Started
            AddHandler Factory.GetBackgroundScanner().Stopped, AddressOf BackgroundScanner_Stopped

            ChannelServices.RegisterChannel(New TcpChannel(Utilities.RemotingChannel), False)
            RemotingConfiguration.RegisterWellKnownServiceType(
                GetType(ScannerHost),
                Utilities.GetRemotingUri(System.Diagnostics.Process.GetCurrentProcess().Id, False),
                WellKnownObjectMode.Singleton
                )
        End Sub

        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing Then
                    Factory.GetBuildManager().IsListeningToBuildEvents = False
                    Factory.CleanupFactory()
                    GC.SuppressFinalize(Me)
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

#End Region ' Package Members

        Private Sub BuildManager_BuildStopped()
            Dim mcs As OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), OleMenuCommandService)
            If mcs Is Nothing Then
                Debug.Fail("Failed to get IMenuCommandService service.")
                Return
            End If
            mcs.FindCommand(New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidConfig)))).Enabled = True
        End Sub

        Private Sub BuildManager_BuildStarted()
            Dim mcs As OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), OleMenuCommandService)
            If mcs Is Nothing Then
                Debug.Fail("Failed to get IMenuCommandService service.")
                Return
            End If
            mcs.FindCommand(New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidConfig)))).Enabled = False
        End Sub

        Private Sub BackgroundScanner_Stopped(ByVal sender As Object, ByVal e As EventArgs)
            Dim mcs As OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), OleMenuCommandService)
            If mcs Is Nothing Then
                Debug.Fail("Failed to get IMenuCommandService service.")
                Return
            End If

            Dim stopCommand As MenuCommand = mcs.FindCommand(New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidStopScan))))
            stopCommand.Enabled = False
            stopCommand.Checked = False
            mcs.FindCommand(New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidRepeatLastScan)))).Enabled = True
        End Sub

        Private Sub BackgroundScanner_Started(ByVal sender As Object, ByVal e As EventArgs)
            Dim mcs As OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), OleMenuCommandService)
            If mcs Is Nothing Then
                Debug.Fail("Failed to get IMenuCommandService service.")
                Return
            End If

            mcs.FindCommand(New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidStopScan)))).Enabled = True
            mcs.FindCommand(New CommandID(GuidList.guidVSPackageCmdSet, CInt(Fix(PkgCmdIDList.cmdidRepeatLastScan)))).Enabled = False
        End Sub

        Private Sub StopScan(ByVal sender As Object, ByVal e As EventArgs)
            Factory.GetBackgroundScanner().StopIfRunning(False)
            If Factory.GetBackgroundScanner().IsRunning Then
                Dim mcs As OleMenuCommandService = TryCast(GetService(GetType(IMenuCommandService)), OleMenuCommandService)
                If mcs Is Nothing Then
                    Debug.Fail("Failed to get IMenuCommandService service.")
                    Return
                End If
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
            If dte Is Nothing Then
                Debug.Fail("Failed to get DTE service.")
                Return
            End If
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
