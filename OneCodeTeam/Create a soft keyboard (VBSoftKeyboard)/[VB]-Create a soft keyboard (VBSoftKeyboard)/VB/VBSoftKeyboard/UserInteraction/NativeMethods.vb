'*************************** Module Header ******************************'
' Project:	    VBSoftKeyboard
' Copyright (c) Microsoft Corporation.
' 
' This class contains the structures and the constants which are used in
' the SendInput method. 
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
    Friend NotInheritable Class NativeMethods

        ' The constants used in the INPUT structure.
        Public Const INPUT_MOUSE As Integer = 0
        Public Const INPUT_KEYBOARD As Integer = 1
        Public Const INPUT_HARDWARE As Integer = 2

        ' The constants used in the KEYBDINPUT structure.
        Public Const KEYEVENTF_EXTENDEDKEY As Integer = &H1
        Public Const KEYEVENTF_KEYUP As Integer = &H2

        ''' <summary>
        ''' Used by SendInput to store information for synthesizing input events such 
        ''' as keystrokes, mouse movement, and mouse clicks.
        ''' http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx
        ''' </summary>
        <StructLayout(LayoutKind.Sequential)>
        Public Structure INPUT
            ''' <summary>
            ''' INPUT_MOUSE    0
            ''' INPUT_KEYBOARD 1
            ''' INPUT_HARDWARE 2
            ''' </summary>
            Public type As Integer
            Public inputUnion As NativeMethods.INPUTUNION
        End Structure


        ''' <summary>
        ''' An INPUTUNION structure only contains one field. 
        ''' http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx
        ''' </summary>
        <StructLayout(LayoutKind.Explicit)>
        Public Structure INPUTUNION
            <FieldOffset(0)>
            Public hi As NativeMethods.HARDWAREINPUT
            <FieldOffset(0)>
            Public ki As NativeMethods.KEYBDINPUT
            <FieldOffset(0)>
            Public mi As NativeMethods.MOUSEINPUT
        End Structure

        ''' <summary>
        ''' The information about a simulated hardware event.
        ''' http://msdn.microsoft.com/en-us/library/ms646269(VS.85).aspx
        ''' </summary>
        <StructLayout(LayoutKind.Sequential)>
        Public Structure HARDWAREINPUT
            Public uMsg As Integer
            Public wParamL As Short
            Public wParamH As Short
        End Structure

        ''' <summary>
        ''' The information about a simulated keyboard event.
        ''' http://msdn.microsoft.com/en-us/library/ms646271(VS.85).aspx
        ''' </summary>
        <StructLayout(LayoutKind.Sequential)>
        Public Structure KEYBDINPUT
            Public wVk As Short
            Public wScan As Short
            Public dwFlags As Integer
            Public time As Integer
            Public dwExtraInfo As IntPtr
        End Structure

        ''' <summary>
        ''' The information about a simulated mouse event.
        ''' http://msdn.microsoft.com/en-us/library/ms646273(VS.85).aspx
        ''' </summary>
        <StructLayout(LayoutKind.Sequential)>
        Public Structure MOUSEINPUT
            Public dx As Integer
            Public dy As Integer
            Public mouseData As Integer
            Public dwFlags As Integer
            Public time As Integer
            Public dwExtraInfo As IntPtr
        End Structure
    End Class

End Namespace
