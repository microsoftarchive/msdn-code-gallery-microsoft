'************************** Module Header ******************************'
' Module Name:  UnsafeNativeMethods.vb
' Project:      VBSoftKeyboard
' Copyright (c) Microsoft Corporation.
' 
' These methods are used to get/set the foreground window.
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


Imports System
Imports System.Runtime.InteropServices

Namespace NoActivate

    Friend NotInheritable Class UnsafeNativeMethods

        ''' <summary>
        ''' Retrieve a handle to the foreground window.
        ''' http://msdn.microsoft.com/en-us/library/ms633505(VS.85).aspx
        ''' </summary>
        <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Public Shared Function GetForegroundWindow() As IntPtr
        End Function

        ''' <summary>
        ''' Bring the thread that created the specified window into the foreground
        ''' and activates the window. 
        ''' http://msdn.microsoft.com/en-us/library/ms633539(VS.85).aspx
        ''' </summary>
        <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Public Shared Function SetForegroundWindow(ByVal hWnd As IntPtr) As Boolean
        End Function

        ''' <summary>
        ''' Determine whether the specified window handle identifies an existing window. 
        ''' http://msdn.microsoft.com/en-us/library/ms633528(VS.85).aspx
        ''' </summary>
        <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Public Shared Function IsWindow(ByVal hWnd As IntPtr) As Boolean
        End Function

    End Class

End Namespace