'***************************** Module Header *******************************\ 
' Module Name: MainForm.vb 
' Project:     VBSystemInfo.proj
' Copyright (c) Microsoft Corporation. 
' 
' The project illustrates how to get System Information programmatically.
' 
' This source is subject to the Microsoft Public License. 
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL 
' All other rights reserved. ' 
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
'***************************************************************************/ 

Imports System.Runtime.InteropServices
Imports System.Management
Imports System.DirectoryServices.ActiveDirectory

Public Class MainForm
    ' Declare the OSVERSIONINFO structure which will contain operating system version information
    Public Structure OSVERSIONINFO
        Public dwOSVersionInfoSize As Integer
        Public dwMajorVersion As Integer
        Public dwMinorVersion As Integer
        Public dwBuildNumber As Integer
        Public dwPlatformId As Integer
        <MarshalAs(UnmanagedType.ByValTStr, SizeConst:=128)> _
        Public szCSDVersion As String
    End Structure

    ' <summary>
    ' This function returns the service pack level on the machine this program is running on.
    ' </summary>
    ' <param name="o">OSVERSIONINFO structure</param>
    ' <returns>Return the tservice pack level as string</returns>
    <DllImport("kernel32.Dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function GetVersionEx(ByRef o As OSVERSIONINFO) As Short
    End Function
    Public Shared Function GetServicePack() As String
        Dim os As New OSVERSIONINFO()
        os.dwOSVersionInfoSize = Marshal.SizeOf(GetType(OSVERSIONINFO))
        GetVersionEx(os)
        If os.szCSDVersion = "" Then
            Return "No Service Pack Installed"
        Else
            Return os.szCSDVersion
        End If
    End Function

    ' <summary>
    ' This function is used to check if it is a Server version of OS on the machine.
    ' </summary>
    ' <returns>Return true if it is a Server version of OS else returns false</returns>

    Public Function IsServerVersion() As Boolean
        Using searcher As New ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem")
            For Each managementObject As ManagementObject In searcher.[Get]()
                ' ProductType will be one of:
                ' 1: Workstation
                ' 2: Domain Controller
                ' 3: Server
                Dim productType As UInteger = CUInt(managementObject.GetPropertyValue("ProductType"))
                Return productType <> 1
            Next
        End Using
        Return False
    End Function

    ' <summary>
    ' Form Load event handler function.
    ' </summary>

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Set textBox5 and textBox6 appropriately based on the bitness of the machine and the process
        If Environment.Is64BitOperatingSystem Then
            textBox5.Text = "64-Bit"
        Else
            textBox5.Text = "32-Bit"
        End If

        If Environment.Is64BitProcess Then
            textBox6.Text = "64-Bit"
        Else
            textBox6.Text = "32-Bit"
        End If

        ' Set the textbox1 to the Operating System name by checking the OS version. 
        Dim vs As Version = Environment.OSVersion.Version

        Dim isServer As Boolean = IsServerVersion()

        Select Case vs.Major
            Case 3
                textBox1.Text = "Windows NT 3.51"
                Exit Select
            Case 4
                textBox1.Text = "Windows NT 4.0"
                Exit Select
            Case 5
                If vs.Minor = 0 Then
                    textBox1.Text = "Windows 2000"
                ElseIf vs.Minor = 1 Then
                    textBox1.Text = "Windows XP"
                Else
                    If isServer Then
                        If WindowsAPI.GetSystemMetrics(89) = 0 Then
                            textBox1.Text = "Windows Server 2003"
                        Else
                            textBox1.Text = "Windows Server 2003 R2"
                        End If
                    Else
                        textBox1.Text = "Windows XP"
                    End If
                End If
                Exit Select
            Case 6
                If vs.Minor = 0 Then
                    If isServer Then
                        textBox1.Text = "Windows Server 2008"
                    Else
                        textBox1.Text = "Windows Vista"
                    End If
                ElseIf vs.Minor = 1 Then
                    If isServer Then
                        textBox1.Text = "Windows Server 2008 R2"
                    Else
                        textBox1.Text = "Windows 7"
                    End If
                ElseIf vs.Minor = 2 Then
                    textBox1.Text = "Windows 8"
                Else
                    If isServer Then
                        textBox1.Text = "Windows Server 2012 R2"
                    Else
                        textBox1.Text = "Windows 8.1"
                    End If
                End If
                Exit Select
        End Select

        ' Set the textBox2 to the machine name
        textBox2.Text = Environment.MachineName

        ' Set the textBox4 to the domain name to which the machine is connected else set it to Workgroup
        Try
            textBox4.Text = System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain().Name
        Catch ex As ActiveDirectoryObjectNotFoundException
            textBox4.Text = "WORKGROUP"
        End Try

        ' Set textBox3 to the current service pack level installed on the machine
        textBox3.Text = GetServicePack()
    End Sub
End Class

'Interop class to call GetSystemMetrics which will help us distinguish between Windows Server 2003 and 
'    Windows Server 2003 R2

Partial Public Class WindowsAPI
    <DllImport("user32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function GetSystemMetrics(smIndex As Integer) As Integer
    End Function
End Class

