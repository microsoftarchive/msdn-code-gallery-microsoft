'************************** Module Header ******************************'
' Module Name:  NoActivateWindow.vb
' Project:      VBSoftKeyboard
' Copyright (c) Microsoft Corporation.
' 
' The class represents a form that will not be activated until the user
' presses the left mouse button within its nonclient area(such as the title
' bar, menu bar, or window frame). When the left mouse button is released,
' this window will activate the previous foreground Window.
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'************************************************************************'

Imports System.Security.Permissions

Namespace NoActivate

    Public Class NoActivateWindow
        Inherits Form

        ' The WS_EX_NOACTIVATE value for dwExStyle prevents foreground
        ' activation by the system.
        Private Const WS_EX_NOACTIVATE As Long = &H8000000L

        ' WM_NCMOUSEMOVE Message is posted to a window when the cursor is 
        ' moved within the nonclient area of the window. 
        Private Const WM_NCMOUSEMOVE As Integer = &HA0

        ' WM_NCLBUTTONDOWN Message is posted when the user presses the left
        ' mouse button while the cursor is within the nonclient area of a window. 
        Private Const WM_NCLBUTTONDOWN As Integer = &HA1

        ' The handle of the previous foreground Window.
        Private previousForegroundWindow As IntPtr = IntPtr.Zero

        ''' <summary>
        ''' Set the form style to WS_EX_NOACTIVATE so that it will not get focus. 
        ''' </summary>
        Protected Overrides ReadOnly Property CreateParams() As CreateParams
            <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
            Get
                Dim cp As CreateParams = MyBase.CreateParams
                cp.ExStyle = cp.ExStyle Or CInt(Fix(WS_EX_NOACTIVATE))
                Return cp
            End Get
        End Property

        ''' <summary>
        ''' Process Windows messages.
        ''' 
        ''' When the user presses the left mouse button while the cursor is within the
        ''' nonclient area of this window, the it will store the handle of previous 
        ''' foreground Window, and then activate itself.
        ''' 
        ''' When the cursor is moved within the nonclient area of the window, which means
        ''' that the left mouse button is released, this window will activate the previous 
        ''' foreground Window.
        ''' </summary>
        ''' <param name="m"></param>
        <PermissionSetAttribute(SecurityAction.LinkDemand, Name:="FullTrust")>
        Protected Overrides Sub WndProc(ByRef m As Message)
            Select Case m.Msg
                Case WM_NCLBUTTONDOWN

                    ' Get the current foreground window.
                    Dim foregroundWindow = UnsafeNativeMethods.GetForegroundWindow()

                    ' If this window is not the current foreground window, then activate
                    ' itself.
                    If foregroundWindow = Me.Handle Then
                        UnsafeNativeMethods.SetForegroundWindow(Me.Handle)

                        ' Store the handle of previous foreground window.
                        If foregroundWindow = IntPtr.Zero Then
                            previousForegroundWindow = foregroundWindow
                        End If
                    End If

                Case WM_NCMOUSEMOVE

                    ' Determine whether previous window still exist. If yes, then 
                    ' activate it.
                    ' Note: There is a scenario that the previous window is closed, but 
                    '       the same handle is assgined to a new window.
                    If UnsafeNativeMethods.IsWindow(previousForegroundWindow) Then
                        UnsafeNativeMethods.SetForegroundWindow(previousForegroundWindow)
                        previousForegroundWindow = IntPtr.Zero
                    End If

                Case Else
            End Select

            MyBase.WndProc(m)
        End Sub

    End Class

End Namespace