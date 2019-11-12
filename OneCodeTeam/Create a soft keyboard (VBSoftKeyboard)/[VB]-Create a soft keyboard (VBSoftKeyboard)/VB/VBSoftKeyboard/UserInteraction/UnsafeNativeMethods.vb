'*************************** Module Header ******************************'
' Module Name:  UnsafeNativeMethods.vb
' Project:	    VBSoftKeyboard
' Copyright (c) Microsoft Corporation.
' 
' This class wraps the GetKeyState and SendInput in User32.dll.
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Runtime.InteropServices

Namespace UserInteraction
    Friend NotInheritable Class UnsafeNativeMethods

        ''' <summary>
        ''' Retrieves the status of the specified virtual key. The status specifies whether
        ''' the key is up, down, or toggled (on, off—alternating each time the key is pressed).
        ''' </summary>
        ''' <param name="nVirtKey">A virtual key. </param>
        ''' <returns>
        ''' If the high-order bit is 1, the key is down; otherwise, it is up.
        ''' If the low-order bit is 1, the key is toggled. A key, such as the CAPS LOCK key,
        ''' is toggled if it is turned on. The key is off and untoggled if the low-order bit
        ''' is 0. A toggle key's indicator light (if any) on the keyboard will be on when the
        ''' key is toggled, and off when the key is untoggled.
        ''' </returns>
        <DllImport("User32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Public Shared Function GetKeyState(ByVal nVirtKey As Integer) As Short
        End Function

        ''' <summary>
        ''' Synthesizes keystrokes, mouse motions, and button clicks.
        ''' </summary>
        ''' <param name="nInputs">The number of structures in the pInputs array.</param>
        ''' <param name="pInputs">
        ''' An array of INPUT structures. Each structure represents an event to be inserted 
        ''' into the keyboard or mouse input stream.
        ''' </param>
        ''' <param name="cbSize">
        ''' The size, in bytes, of an INPUT structure. If cbSize is not the size of an 
        ''' INPUT structure, the function fails.
        ''' </param>
        ''' <returns>
        ''' If the function returns zero, the input was already blocked by another thread. 
        ''' </returns>
        <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)>
        Public Shared Function SendInput(ByVal nInputs As UInteger,
                                         ByVal pInputs() As NativeMethods.INPUT,
                                         ByVal cbSize As Integer) As UInteger
        End Function

    End Class
End Namespace
