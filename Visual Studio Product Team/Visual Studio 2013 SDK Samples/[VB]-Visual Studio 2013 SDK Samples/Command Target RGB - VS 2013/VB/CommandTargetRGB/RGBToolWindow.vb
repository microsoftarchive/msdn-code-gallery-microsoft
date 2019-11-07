'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports System
Imports System.ComponentModel.Design
Imports System.Runtime.InteropServices
Imports System.Windows.Controls
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.OLE.Interop
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.Shell.Interop

''' <summary>
''' This class implements the tool window exposed by this package and hosts a user control.
'''
''' In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
''' usually implemented by the package implementer.
'''
''' This class derives from the ToolWindowPane class provided from the MPF in order to use its 
''' implementation of the IVsUIElementPane interface.
''' </summary>
<Guid(GuidList.guidToolWindowPersistenceString)> _
Public Class RGBToolWindow
    Inherits ToolWindowPane

    Private control As RGBControl
    Private checkedCommand As UInteger = PkgCmdIDList.cmdidRed

    ''' <summary>
    ''' Standard constructor for the tool window.
    ''' </summary>
    Public Sub New()
        MyBase.New(Nothing)
        ' Set the window title reading it from the resources.
        Me.Caption = Resources.ToolWindowTitle

        ' Set the image that will appear on the tab of the window frame
        ' when docked with an other window
        ' The resource ID correspond to the one defined in the resx file
        ' while the Index is the offset in the bitmap strip. Each image in
        ' the strip being 16x16.
        Me.BitmapResourceID = 301
        Me.BitmapIndex = 0

        'This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
        'we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
        'the object returned by the Content property.
        control = New RGBControl()
        Me.Content = control

        Dim mcs As OleMenuCommandService = TryCast(Me.GetService(GetType(IMenuCommandService)), OleMenuCommandService)
        If Nothing IsNot mcs Then
            Me.AddCommand(mcs, PkgCmdIDList.cmdidRed, New EventHandler(AddressOf OnRedCommand))
            Me.AddCommand(mcs, PkgCmdIDList.cmdidGreen, New EventHandler(AddressOf OnGreenCommand))
            Me.AddCommand(mcs, PkgCmdIDList.cmdidBlue, New EventHandler(AddressOf OnBlueCommand))
        End If
    End Sub

    Private Sub AddCommand(mcs As OleMenuCommandService, cmdid As Integer, handler As EventHandler)
        Dim commandID As CommandID = New CommandID(GuidList.guidCommandTargetRGBCmdSet, cmdid)
        Dim command As OleMenuCommand = New OleMenuCommand(handler, commandID)
        AddHandler command.BeforeQueryStatus, AddressOf OnBeforeQueryStatus

        mcs.AddCommand(command)
    End Sub

    Private Sub OnBeforeQueryStatus(sender As Object, e As EventArgs)
        Dim command As MenuCommand = CType(sender, MenuCommand)

        command.Enabled = True
        command.Visible = True
        command.Checked = (CLng(command.CommandID.ID) = CLng((CULng(Me.checkedCommand))))
    End Sub

    Private Sub OnRedCommand(sender As Object, e As EventArgs)
        Me.control.Color = RGBControlColor.Red
        Me.checkedCommand = PkgCmdIDList.cmdidRed
    End Sub

    Private Sub OnGreenCommand(sender As Object, e As EventArgs)
        Me.control.Color = RGBControlColor.Green
        Me.checkedCommand = PkgCmdIDList.cmdidGreen
    End Sub

    Private Sub OnBlueCommand(sender As Object, e As EventArgs)
        Me.control.Color = RGBControlColor.Blue
        Me.checkedCommand = PkgCmdIDList.cmdidBlue
    End Sub

    Overrides Sub OnToolWindowCreated()
        MyBase.OnToolWindowCreated()

        ' Force initialization of the control
        control.InitializeComponent()
        CreateToolBar()
    End Sub

    Private Sub CreateToolBar()
        ' Retrieve the shell UI object
        Dim shell4 As IVsUIShell4 = TryCast(GetService(GetType(SVsUIShell)), IVsUIShell4)
        If Not shell4 Is Nothing Then
            ' Create the toolbar tray
            Dim host As IVsToolbarTrayHost = Nothing
            If ErrorHandler.Succeeded(shell4.CreateToolbarTray(Me, host)) Then
                Dim uiElement As IVsUIElement = Nothing
                Dim uiObject As Object = Nothing
                Dim frameworkElement As Object = Nothing

                ' Add the toolbar as defined in vsct
                host.AddToolbar(GuidList.guidCommandTargetRGBCmdSet, PkgCmdIDList.RGBToolbar)
                host.GetToolbarTray(uiElement)

                ' Get the WPF element
                uiElement.GetUIObject(uiObject)
                Dim wpfe As IVsUIWpfElement = TryCast(uiObject, IVsUIWpfElement)

                ' Retrieve and set the toolbar tray
                wpfe.GetFrameworkElement(frameworkElement)
                control.SetTray(TryCast(frameworkElement, ToolBarTray))
            End If
        End If
    End Sub

End Class
