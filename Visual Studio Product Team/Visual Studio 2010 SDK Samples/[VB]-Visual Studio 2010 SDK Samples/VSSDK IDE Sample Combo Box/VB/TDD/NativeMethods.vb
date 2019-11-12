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
Imports System.Collections.Generic
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Threading
Imports Microsoft.VisualStudio.Shell.Interop
Namespace Microsoft.Samples.VisualStudio.ComboBox.IntegrationTest

    ''' <summary>
    ''' Defines pinvoked utility methods and internal VS Constants.
    ''' </summary>
    Friend Class NativeMethods
        Friend Delegate Function CallBack(ByVal hwnd As IntPtr, ByVal lParam As IntPtr) As Boolean

        ' Declare two overloaded SendMessage functions.
        <DllImport("user32.dll")> _
        Friend Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As UInt32, ByVal wParam As UInt32, ByVal lParam As IntPtr) As UInt32
        End Function

        <DllImport("user32.dll", CharSet:=System.Runtime.InteropServices.CharSet.Auto)> _
        Friend Shared Function PeekMessage(<[In](), Out()> ByRef msg As Microsoft.VisualStudio.OLE.Interop.MSG, ByVal hwnd As HandleRef, ByVal msgMin As Integer, ByVal msgMax As Integer, ByVal remove As Integer) As Boolean
        End Function

        <DllImport("user32.dll", CharSet:=System.Runtime.InteropServices.CharSet.Auto)> _
        Friend Shared Function TranslateMessage(<[In](), Out()> ByRef msg As Microsoft.VisualStudio.OLE.Interop.MSG) As Boolean
        End Function

        <DllImport("user32.dll", CharSet:=System.Runtime.InteropServices.CharSet.Auto)> _
        Friend Shared Function DispatchMessage(ByRef msg As Microsoft.VisualStudio.OLE.Interop.MSG) As Integer
        End Function

        <DllImport("user32.dll", ExactSpelling:=True, CharSet:=CharSet.Auto)> _
        Friend Shared Function MsgWaitForMultipleObjects(ByVal nCount As Integer, ByVal pHandles As Integer, ByVal fWaitAll As Boolean, ByVal dwMilliseconds As Integer, ByVal dwWakeMask As Integer) As Integer
        End Function

        <DllImport("user32.dll", CharSet:=System.Runtime.InteropServices.CharSet.Auto)> _
        Friend Shared Function AttachThreadInput(ByVal idAttach As UInteger, ByVal idAttachTo As UInteger, ByVal attach As Boolean) As Boolean
        End Function

        <DllImport("user32.dll", CharSet:=System.Runtime.InteropServices.CharSet.Auto)> _
        Friend Shared Function GetWindowThreadProcessId(ByVal hWnd As IntPtr, <System.Runtime.InteropServices.Out()> ByRef lpdwProcessId As UInteger) As UInteger
        End Function

        <DllImport("kernel32.dll", CharSet:=System.Runtime.InteropServices.CharSet.Auto)> _
        Friend Shared Function GetCurrentThreadId() As UInteger
        End Function

        <DllImport("user32")> _
        Friend Shared Function EnumChildWindows(ByVal hwnd As IntPtr, ByVal x As CallBack, ByVal y As IntPtr) As Integer
        End Function

        <DllImport("user32")> _
        Friend Shared Function IsWindowVisible(ByVal hDlg As IntPtr) As Boolean
        End Function

        <DllImport("user32.dll", CharSet:=System.Runtime.InteropServices.CharSet.Auto)> _
        Friend Shared Function SetFocus(ByVal hWnd As IntPtr) As IntPtr
        End Function

        <DllImport("user32")> _
        Friend Shared Function GetClassName(ByVal hWnd As IntPtr, ByVal className As StringBuilder, ByVal stringLength As Integer) As Integer
        End Function
        <DllImport("user32")> _
        Friend Shared Function GetWindowText(ByVal hWnd As IntPtr, ByVal className As StringBuilder, ByVal stringLength As Integer) As Integer
        End Function


        <DllImport("user32")> _
        Friend Shared Function EndDialog(ByVal hDlg As IntPtr, ByVal result As Integer) As Boolean
        End Function

        <DllImport("Kernel32")> _
        Friend Shared Function GetLastError() As Long
        End Function

        Friend Const QS_KEY As Integer = &H1, QS_MOUSEMOVE As Integer = &H2, QS_MOUSEBUTTON As Integer = &H4, QS_POSTMESSAGE As Integer = &H8, QS_TIMER As Integer = &H10, QS_PAINT As Integer = &H20, QS_SENDMESSAGE As Integer = &H40, QS_HOTKEY As Integer = &H80, QS_ALLPOSTMESSAGE As Integer = &H100, QS_MOUSE As Integer = QS_MOUSEMOVE Or QS_MOUSEBUTTON, QS_INPUT As Integer = QS_MOUSE Or QS_KEY, QS_ALLEVENTS As Integer = QS_INPUT Or QS_POSTMESSAGE Or QS_TIMER Or QS_PAINT Or QS_HOTKEY, QS_ALLINPUT As Integer = QS_INPUT Or QS_POSTMESSAGE Or QS_TIMER Or QS_PAINT Or QS_HOTKEY Or QS_SENDMESSAGE

        Friend Const Facility_Win32 As Integer = 7

        Friend Const WM_CLOSE As Integer = &H10

        Friend Const S_FALSE As Integer = &H1, S_OK As Integer = &H0, IDOK As Integer = 1, IDCANCEL As Integer = 2, IDABORT As Integer = 3, IDRETRY As Integer = 4, IDIGNORE As Integer = 5, IDYES As Integer = 6, IDNO As Integer = 7, IDCLOSE As Integer = 8, IDHELP As Integer = 9, IDTRYAGAIN As Integer = 10, IDCONTINUE As Integer = 11

        Private Sub New()
        End Sub
        Friend Shared Function HResultFromWin32(ByVal [error] As Long) As Long
            If [error] <= 0 Then
                Return [error]
            End If

            Return (([error] And &HFFFF) Or (Facility_Win32 << 16) Or &H80000000L)
        End Function

        ''' <devdoc>
        ''' Please use this "approved" method to compare file names.
        ''' </devdoc>
        Public Shared Function IsSamePath(ByVal file1 As String, ByVal file2 As String) As Boolean
            If file1 Is Nothing OrElse file1.Length = 0 Then
                Return (file2 Is Nothing OrElse file2.Length = 0)
            End If

            Dim uri1 As Uri = Nothing
            Dim uri2 As Uri = Nothing

            Try
                If (Not Uri.TryCreate(file1, UriKind.Absolute, uri1)) OrElse (Not Uri.TryCreate(file2, UriKind.Absolute, uri2)) Then
                    Return False
                End If

                If uri1 IsNot Nothing AndAlso uri1.IsFile AndAlso uri2 IsNot Nothing AndAlso uri2.IsFile Then
                    Return 0 = String.Compare(uri1.LocalPath, uri2.LocalPath, StringComparison.OrdinalIgnoreCase)
                End If

                Return file1 = file2
            Catch e As UriFormatException
                System.Diagnostics.Trace.WriteLine("Exception " & e.Message)
            End Try

            Return False
        End Function

    End Class
End Namespace
