' Copyright (c) Microsoft Corporation. All rights reserved.
Imports System.Runtime.InteropServices
Imports System.Text

' Class to wrap up Windows 32 API constants and functions.
Public Class Win32API
    <StructLayout(LayoutKind.Sequential)> _
    Public Structure OSVersionInfo
        Public OSVersionInfoSize As Integer
        Public majorVersion As Integer
        Public minorVersion As Integer
        Public buildNumber As Integer
        Public platformId As Integer
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)> _
        Public versionString As String
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Public Structure SECURITY_ATTRIBUTES
        Public nLength As Integer 'dword
        Public lpSecurityDescriptor As Integer 'lpvoid
        Public bInheritHandle As Integer 'bool
    End Structure

    Public Const GWL_EXSTYLE As Integer = (-20)
    Public Const GW_OWNER As Integer = 4
    Public Const SW_RESTORE As Integer = 9
    Public Const SW_SHOW As Integer = 5
    Public Const WS_EX_TOOLWINDOW As Integer = &H80
    Public Const WS_EX_APPWINDOW As Integer = &H40000

    Public Declare Function CreateDirectory Lib "kernel32" _
       Alias "CreateDirectoryA" (ByVal lpPathName As String, _
                                 ByRef lpSecurityAttributes _
                                     As SECURITY_ATTRIBUTES) As Boolean


    Public Delegate Function EnumWindowsCallback(ByVal hWnd As Integer, _
                                                 ByVal lParam As Integer) As Boolean

    Public Declare Function EnumWindows Lib "user32.dll" _
        Alias "EnumWindows" (ByVal callback As EnumWindowsCallback, _
                             ByVal lParam As Integer) As Integer

    <DllImport("user32.dll", EntryPoint:="EnumWindows", SetLastError:=True, _
    CharSet:=CharSet.Ansi, ExactSpelling:=True, _
    CallingConvention:=CallingConvention.StdCall)> _
    Public Shared Function EnumWindowsDllImport(ByVal callback As EnumWindowsCallback, _
                                                ByVal lParam As Integer) As Integer
    End Function

    Public Declare Auto Function FindWindow Lib "user32.dll" _
        Alias "FindWindow" (ByVal lpClassName As String, _
                            ByVal lpWindowName As String) As Integer

    Public Declare Auto Function FindWindowAny Lib "user32.dll" _
    Alias "FindWindow" (ByVal lpClassName As Integer, _
                        ByVal lpWindowName As Integer) As Integer

    Public Declare Auto Function FindWindowNullClassName Lib "user32.dll" _
        Alias "FindWindow" (ByVal lpClassName As Integer, _
                            ByVal lpWindowName As String) As Integer

    Public Declare Auto Function FindWindowNullWindowCaption Lib "user32.dll" _
       Alias "FindWindow" (ByVal lpClassName As String, _
                           ByVal lpWindowName As Integer) As Integer


    Public Declare Function GetClassName Lib "user32.dll" _
       Alias "GetClassNameA" (ByVal hwnd As Integer, _
                              ByVal lpClassName As StringBuilder, _
                              ByVal cch As Integer) As Integer

    Public Declare Function GetDiskFreeSpace Lib "kernel32" _
        Alias "GetDiskFreeSpaceA" (ByVal lpRootPathName As String, _
                                   ByRef lpSectorsPerCluster As Integer, _
                                   ByRef lpBytesPerSector As Integer, _
                                   ByRef lpNumberOfFreeClusters As Integer, _
                                   ByRef lpTotalNumberOfClusters As Integer) As Integer


    Public Declare Function GetDiskFreeSpaceEx Lib "kernel32" _
        Alias "GetDiskFreeSpaceExA" (ByVal lpRootPathName As String, _
                                     ByRef lpFreeBytesAvailableToCaller As Integer, _
                                     ByRef lpTotalNumberOfBytes As Integer, _
                                     ByRef lpTotalNumberOfFreeBytes As UInt32) As Integer

    Public Declare Function GetDriveType Lib "kernel32" _
        Alias "GetDriveTypeA" (ByVal nDrive As String) As Integer

    Public Declare Function GetParent Lib "user32.dll" _
        Alias "GetParent" (ByVal hwnd As Integer) As Integer


    Declare Ansi Function GetVersionEx Lib "kernel32.dll" _
        Alias "GetVersionExA" (ByRef osvi As OSVersionInfo) As Boolean

    Public Declare Function GetWindow Lib "user32.dll" _
        Alias "GetWindow" (ByVal hwnd As Integer, _
                           ByVal wCmd As Integer) As Integer

    Public Declare Function GetWindowLong Lib "user32.dll" _
        Alias "GetWindowLongA" (ByVal hwnd As Integer, _
                                ByVal nIndex As Integer) As Integer

    Public Declare Sub GetWindowText Lib "user32.dll" _
        Alias "GetWindowTextA" (ByVal hWnd As Integer, _
                                ByVal lpString As StringBuilder, _
                                ByVal nMaxCount As Integer)

    Public Declare Function IsIconic Lib "user32.dll" _
        Alias "IsIconic" (ByVal hwnd As Integer) As Boolean

    Public Declare Function IsPwrHibernateAllowed Lib "Powrprof.dll" _
        Alias "IsPwrHibernateAllowed" () As Integer

    Public Declare Function IsWindowVisible Lib "user32.dll" _
        Alias "IsWindowVisible" (ByVal hwnd As Integer) As Boolean

    Public Declare Function SetForegroundWindow Lib "user32.dll" _
        Alias "SetForegroundWindow" (ByVal hwnd As Integer) As Integer

    Public Declare Function SetSuspendState Lib "Powrprof.dll" _
        Alias "SetSuspendState" (ByVal Hibernate As Integer, _
                                 ByVal ForceCritical As Integer, _
                                 ByVal DisableWakeEvent As Integer) As Integer

    Public Declare Function ShowWindow Lib "user32.dll" _
        Alias "ShowWindow" (ByVal hwnd As Integer, ByVal nCmdShow As Integer) As Integer

    Declare Function SwapMouseButton Lib "user32.dll" _
        Alias "SwapMouseButton" (ByVal bSwap As Integer) As Integer

End Class
