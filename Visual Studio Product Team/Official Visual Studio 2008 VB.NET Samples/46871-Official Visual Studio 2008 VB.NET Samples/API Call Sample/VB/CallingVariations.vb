' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Runtime.InteropServices

' This class encapsulates the calling variations of the function Beep
Public Class CallingVariations

    ' Declare version 
    Public Declare Function DeclareBeep Lib "kernel32" Alias "Beep" _
        (ByVal dwFreq As Integer, ByVal dwDuration As Integer) As Integer

    ' DLLImport version
    <DllImport("kernel32.dll", EntryPoint:="Beep")> _
    Public Shared Function DLLImportBeep(ByVal dwFreq As Integer, _
        ByVal dwDuration As Integer) As Integer
    End Function

    ' Specifying  Unicode
    Public Declare Unicode Function UnicodeBeep Lib "kernel32" Alias "Beep" _
        (ByVal dwFreq As Integer, ByVal dwDuration As Integer) As Integer

    ' Specifying Ansi
    Public Declare Ansi Function ANSIBeep Lib "kernel32" Alias "Beep" _
        (ByVal dwFreq As Integer, ByVal dwDuration As Integer) As Integer

    ' Specifying Auto
    Public Declare Auto Function AutoBeep Lib "kernel32" Alias "Beep" _
        (ByVal dwFreq As Integer, ByVal dwDuration As Integer) As Integer

    ' Using Exact Spelling
    ' The default value of ExactSpelling is False.
    ' If False an A is appended under ANSI and a W under Unicode so that Beep becomes BeepW.
    <DllImport("kernel32.dll", ExactSpelling:=True, CharSet:=CharSet.Ansi, EntryPoint:="BeepW")> _
    Public Shared Function ExactSpellingBeep(ByVal dwFreq As Integer, _
        ByVal dwDuration As Integer) As Integer
    End Function

End Class

