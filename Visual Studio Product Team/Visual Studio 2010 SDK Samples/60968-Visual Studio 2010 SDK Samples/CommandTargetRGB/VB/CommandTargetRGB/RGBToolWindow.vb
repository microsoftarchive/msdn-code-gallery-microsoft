Imports System
Imports System.Collections
Imports System.ComponentModel
Imports System.Drawing
Imports System.Data
Imports System.Windows
Imports System.Windows.Controls
Imports System.Runtime.InteropServices
Imports Microsoft.VisualStudio
Imports Microsoft.VisualStudio.Shell.Interop
Imports Microsoft.VisualStudio.Shell
Imports Microsoft.VisualStudio.OLE.Interop

''' <summary>
''' This class implements the tool window exposed by this package and hosts a user control.
'''
''' In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
''' usually implemented by the package implementer.
'''
''' This class derives from the ToolWindowPane class provided from the MPF in order to use its 
''' implementation of the IVsUIElementPane interface.
''' </summary>
<Guid("0bdb1e08-ed8b-47e8-91b2-e9bd814b4ebb")> _
Public Class RGBToolWindow
    Inherits ToolWindowPane
    Implements IOleCommandTarget

    Private control As RGBControl
    Private latchedCommand As UInteger = PkgCmdIDList.cmdidRed

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
        Me.BitmapIndex = 1

        'This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
        'we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
        'the object returned by the Content property.
        control = New RGBControl()
        Me.Content = control
    End Sub

    Overrides Sub OnToolWindowCreated()
        MyBase.OnToolWindowCreated()

        ' Force initialization of the control
        control.InitializeComponent()
        CreateToolBar()
    End Sub

    Private Sub CreateToolBar()
        ' Retrieve the shell UI object
        Dim shell4 As IVsUIShell4 = TryCast(ServiceProvider.GlobalProvider.GetService(GetType(SVsUIShell)), IVsUIShell4)
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

    ' Handle Exec commands for commands in the guidCommandTargetRGBCmdSet
    Private Overloads Function Exec(ByRef guidGroup As Guid, ByVal nCmdId As UInteger, ByVal nCmdExcept As UInteger, ByVal pIn As IntPtr, ByVal vOut As IntPtr) As Integer Implements IOleCommandTarget.Exec
        If guidGroup = GuidList.guidCommandTargetRGBCmdSet Then
            ' Determine the command and call the appropriate method on the RGBControl
            If nCmdId = PkgCmdIDList.cmdidRed Then
                control.Color = RGBControlColor.Red
            ElseIf nCmdId = PkgCmdIDList.cmdidGreen Then
                control.Color = RGBControlColor.Green
            ElseIf nCmdId = PkgCmdIDList.cmdidBlue Then
                control.Color = RGBControlColor.Blue
            Else
                ' We don't support any other commands, including cmdidShowToolWindow, because we want
                ' the package to handle the command to create new tool windows
                Return CType(Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDERR_E_NOTSUPPORTED, Integer)
            End If

            ' Set latched command
            latchedCommand = nCmdId
        Else
            ' Requested command group is unknown
            Return CType(Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDERR_E_UNKNOWNGROUP, Integer)
        End If

        Return VSConstants.S_OK
    End Function

    ' Handle QueryStatus for commands in the guidCommandTargetRGBCmdSet
    Function QueryStatus(ByRef guidGroup As Guid, ByVal cCmds As UInteger, <OutAttribute()> ByVal prgCmds As OLECMD(), <OutAttribute()> ByVal pCmdText As IntPtr) As Integer Implements IOleCommandTarget.QueryStatus
        ' All commands in guidCommandTargetRGBSet are supported except cmdidShowToolWindow
        If guidGroup = GuidList.guidCommandTargetRGBCmdSet Then
            For i As Integer = 0 To cCmds - 1
                If prgCmds(i).cmdID = PkgCmdIDList.cmdidShowToolWindow Then
                    ' We do not support the cmdidShowToolWindow command
                    Return CType(Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDERR_E_NOTSUPPORTED, Integer)
                Else
                    ' Mark this command as supported and enabled
                    prgCmds(i).cmdf = CType(OLECMDF.OLECMDF_SUPPORTED, UInteger) Or CType(OLECMDF.OLECMDF_ENABLED, UInteger)

                    ' Display highlight for latched command
                    If prgCmds(i).cmdID = latchedCommand Then
                        prgCmds(i).cmdf = prgCmds(i).cmdf Or CType(OLECMDF.OLECMDF_LATCHED, UInteger)
                    End If
                End If
            Next i
        Else
            ' Requested command group is unknown
            Return CType(Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDERR_E_UNKNOWNGROUP, Integer)
        End If

        Return VSConstants.S_OK
    End Function
End Class
