'*************************** Module Header ******************************'
' Module Name:  KeyboardInput.vb
' Project:	    VBSoftKeyboard
' Copyright (c) Microsoft Corporation.
' 
' This class wraps the UnsafeNativeMethods.SendInput to synthesizes keystrokes.
' 
' There are 3 scenarios:
' 1. A single key is pressed, such as "A".
' 2. A key with modifier keys is pressed, such as "Ctrl+A".
' 3. A key that could be toggled is pressed, such as Caps Lock, Num Lock or
'    Scroll Lock. 
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

    Public NotInheritable Class KeyboardInput

        ''' <summary>
        ''' A single key is pressed.
        ''' </summary>
        Private Sub New()
        End Sub
        Public Shared Sub SendKey(ByVal key As Integer)
            SendKey(Nothing, key)
        End Sub

        ''' <summary>
        ''' A key with modifier keys is pressed. 
        ''' </summary>
        Public Shared Sub SendKey(ByVal modifierKeys As IEnumerable(Of Integer), ByVal key As Integer)
            If key <= 0 Then
                Return
            End If

            ' Only a single key is pressed.
            If modifierKeys Is Nothing OrElse modifierKeys.Count() = 0 Then
                Dim inputs = New NativeMethods.INPUT(0) {}
                inputs(0).type = NativeMethods.INPUT_KEYBOARD
                inputs(0).inputUnion.ki.wVk = CShort(Fix(key))
                UnsafeNativeMethods.SendInput(1, inputs, Marshal.SizeOf(inputs(0)))

                ' A key with modifier keys is pressed. 
            Else

                ' To simulate this scenario, the inputs contains the toggling 
                ' modifier keys, pressing the key and releasing modifier keys events.
                '
                ' For example, to simulate Ctrl+C, we have to send 3 inputs:
                ' 1. Ctrl is pressed.
                ' 2. C is pressed. 
                ' 3. Ctrl is released.
                Dim inputs = New NativeMethods.INPUT(modifierKeys.Count() * 2) {}

                Dim i As Integer = 0

                ' Simulate toggling the modifier keys.
                For Each modifierKey In modifierKeys
                    inputs(i).type = NativeMethods.INPUT_KEYBOARD
                    inputs(i).inputUnion.ki.wVk = CShort(Fix(modifierKey))
                    i += 1
                Next modifierKey

                ' Simulate pressing the key.
                inputs(i).type = NativeMethods.INPUT_KEYBOARD
                inputs(i).inputUnion.ki.wVk = CShort(Fix(key))
                i += 1

                For Each modifierKey In modifierKeys
                    inputs(i).type = NativeMethods.INPUT_KEYBOARD
                    inputs(i).inputUnion.ki.wVk = CShort(Fix(modifierKey))

                    ' 0x0002 means that the key-up event. 
                    inputs(i).inputUnion.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP
                    i += 1
                Next modifierKey

                UnsafeNativeMethods.SendInput(CUInt(inputs.Length), inputs, Marshal.SizeOf(inputs(0)))
            End If
        End Sub

        ''' <summary>
        ''' Represents that a key that could be toggled is pressed. The key should be released.
        ''' </summary>
        Public Shared Sub SendToggledKey(ByVal key As Integer)
            Dim inputs = New NativeMethods.INPUT(1) {}

            ' Press the key.
            inputs(0).type = NativeMethods.INPUT_KEYBOARD
            inputs(0).inputUnion.ki.wVk = CShort(Fix(key))

            ' Release the key.
            inputs(1).type = NativeMethods.INPUT_KEYBOARD
            inputs(1).inputUnion.ki.wVk = CShort(Fix(key))
            inputs(1).inputUnion.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP

            UnsafeNativeMethods.SendInput(2, inputs, Marshal.SizeOf(inputs(0)))
        End Sub
    End Class
End Namespace
