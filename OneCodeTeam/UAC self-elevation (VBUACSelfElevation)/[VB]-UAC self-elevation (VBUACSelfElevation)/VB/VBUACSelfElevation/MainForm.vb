'********************************* Module Header ***********************************\
' Module Name:  MainForm.vb
' Project:      VBUACSelfElevation
' Copyright (c) Microsoft Corporation.
' 
' User Account Control (UAC) is a new security component in Windows Vista and newer 
' operating systems. With UAC fully enabled, interactive administrators normally run 
' with least user privileges. This example demonstrates how to check the privilege 
' level of the current process, and how to self-elevate the process by giving 
' explicit consent with the Consent UI. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
' EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
' MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***********************************************************************************/

#Region "Imports directives"

Imports Microsoft.Win32.SafeHandles
Imports System.Runtime.InteropServices
Imports System.Security.Principal
Imports System.ComponentModel

#End Region


Public Class MainForm


#Region "Helper Functions for Admin Privileges and Elevation Status"

    ''' <summary>
    ''' The function checks whether the primary access token of the process belongs 
    ''' to user account that is a member of the local Administrators group, even if 
    ''' it currently is not elevated.
    ''' </summary>
    ''' <returns>
    ''' Returns True if the primary access token of the process belongs to user 
    ''' account that is a member of the local Administrators group. Returns False 
    ''' if the token does not.
    ''' </returns>
    ''' <exception cref="System.ComponentModel.Win32Exception">
    ''' When any native Windows API call fails, the function throws a Win32Exception 
    ''' with the last error code.
    ''' </exception>
    Friend Function IsUserInAdminGroup() As Boolean
        Dim fInAdminGroup As Boolean = False
        Dim hToken As SafeTokenHandle = Nothing
        Dim hTokenToCheck As SafeTokenHandle = Nothing
        Dim pElevationType As IntPtr = IntPtr.Zero
        Dim pLinkedToken As IntPtr = IntPtr.Zero
        Dim cbSize As Integer = 0

        Try
            ' Open the access token of the current process for query and duplicate.
            If (Not NativeMethods.OpenProcessToken(Process.GetCurrentProcess.Handle, _
                NativeMethods.TOKEN_QUERY Or NativeMethods.TOKEN_DUPLICATE, hToken)) Then
                Throw New Win32Exception
            End If

            ' Determine whether system is running Windows Vista or later operating 
            ' systems (major version >= 6) because they support linked tokens, but 
            ' previous versions (major version < 6) do not.
            If (Environment.OSVersion.Version.Major >= 6) Then
                ' Running Windows Vista or later (major version >= 6). 
                ' Determine token type: limited, elevated, or default. 

                ' Allocate a buffer for the elevation type information.
                cbSize = 4  ' Size of TOKEN_ELEVATION_TYPE
                pElevationType = Marshal.AllocHGlobal(cbSize)
                If (pElevationType = IntPtr.Zero) Then
                    Throw New Win32Exception
                End If

                ' Retrieve token elevation type information.
                If (Not NativeMethods.GetTokenInformation(hToken, _
                    TOKEN_INFORMATION_CLASS.TokenElevationType, _
                    pElevationType, cbSize, cbSize)) Then
                    Throw New Win32Exception
                End If

                ' Marshal the TOKEN_ELEVATION_TYPE enum from native to .NET.
                Dim elevType As TOKEN_ELEVATION_TYPE = Marshal.ReadInt32(pElevationType)

                ' If limited, get the linked elevated token for further check.
                If (elevType = TOKEN_ELEVATION_TYPE.TokenElevationTypeLimited) Then
                    ' Allocate a buffer for the linked token.
                    cbSize = IntPtr.Size
                    pLinkedToken = Marshal.AllocHGlobal(cbSize)
                    If (pLinkedToken = IntPtr.Zero) Then
                        Throw New Win32Exception
                    End If

                    ' Get the linked token.
                    If (Not NativeMethods.GetTokenInformation(hToken, _
                        TOKEN_INFORMATION_CLASS.TokenLinkedToken, _
                        pLinkedToken, cbSize, cbSize)) Then
                        Throw New Win32Exception
                    End If

                    ' Marshal the linked token value from native to .NET.
                    Dim hLinkedToken As IntPtr = Marshal.ReadIntPtr(pLinkedToken)
                    hTokenToCheck = New SafeTokenHandle(hLinkedToken)
                End If
            End If

            ' CheckTokenMembership requires an impersonation token. If we just got a 
            ' linked token, it already is an impersonation token.  If we did not get 
            ' a linked token, duplicate the original into an impersonation token for 
            ' CheckTokenMembership.
            If (hTokenToCheck Is Nothing) Then
                If (Not NativeMethods.DuplicateToken(hToken, _
                    SECURITY_IMPERSONATION_LEVEL.SecurityIdentification, _
                    hTokenToCheck)) Then
                    Throw New Win32Exception
                End If
            End If

            ' Check if the token to be checked contains admin SID.
            Dim id As New WindowsIdentity(hTokenToCheck.DangerousGetHandle)
            Dim principal As New WindowsPrincipal(id)
            fInAdminGroup = principal.IsInRole(WindowsBuiltInRole.Administrator)

        Finally
            ' Centralized cleanup for all allocated resources. 
            If (Not hToken Is Nothing) Then
                hToken.Close()
                hToken = Nothing
            End If
            If (Not hTokenToCheck Is Nothing) Then
                hTokenToCheck.Close()
                hTokenToCheck = Nothing
            End If
            If (pElevationType <> IntPtr.Zero) Then
                Marshal.FreeHGlobal(pElevationType)
                pElevationType = IntPtr.Zero
            End If
            If (pLinkedToken <> IntPtr.Zero) Then
                Marshal.FreeHGlobal(pLinkedToken)
                pLinkedToken = IntPtr.Zero
            End If
        End Try

        Return fInAdminGroup
    End Function


    ''' <summary>
    ''' The function checks whether the current process is run as administrator.
    ''' In other words, it dictates whether the primary access token of the 
    ''' process belongs to user account that is a member of the local 
    ''' Administrators group and it is elevated.
    ''' </summary>
    ''' <returns>
    ''' Returns True if the primary access token of the process belongs to user 
    ''' account that is a member of the local Administrators group and it is 
    ''' elevated. Returns False if the token does not.
    ''' </returns>
    Friend Function IsRunAsAdmin() As Boolean
        Dim principal As New WindowsPrincipal(WindowsIdentity.GetCurrent)
        Return principal.IsInRole(WindowsBuiltInRole.Administrator)
    End Function


    ''' <summary>
    ''' The function gets the elevation information of the current process. It 
    ''' dictates whether the process is elevated or not. Token elevation is only 
    ''' available on Windows Vista and newer operating systems, thus 
    ''' IsProcessElevated throws a C++ exception if it is called on systems prior 
    ''' to Windows Vista. It is not appropriate to use this function to determine 
    ''' whether a process is run as administartor.
    ''' </summary>
    ''' <returns>
    ''' Returns True if the process is elevated. Returns False if it is not.
    ''' </returns>
    ''' <exception cref="System.ComponentModel.Win32Exception">
    ''' When any native Windows API call fails, the function throws a Win32Exception
    ''' with the last error code.
    ''' </exception>
    ''' <remarks>
    ''' TOKEN_INFORMATION_CLASS provides TokenElevationType to check the elevation 
    ''' type (TokenElevationTypeDefault / TokenElevationTypeLimited / 
    ''' TokenElevationTypeFull) of the process. It is different from TokenElevation 
    ''' in that, when UAC is turned off, elevation type always returns 
    ''' TokenElevationTypeDefault even though the process is elevated (Integrity 
    ''' Level == High). In other words, it is not safe to say if the process is 
    ''' elevated based on elevation type. Instead, we should use TokenElevation.
    ''' </remarks>
    Friend Function IsProcessElevated() As Boolean
        Dim fIsElevated As Boolean = False
        Dim hToken As SafeTokenHandle = Nothing
        Dim cbTokenElevation As Integer = 0
        Dim pTokenElevation As IntPtr = IntPtr.Zero

        Try
            ' Open the access token of the current process with TOKEN_QUERY.
            If (Not NativeMethods.OpenProcessToken(Process.GetCurrentProcess.Handle, _
                NativeMethods.TOKEN_QUERY, hToken)) Then
                Throw New Win32Exception
            End If

            ' Allocate a buffer for the elevation information.
            cbTokenElevation = Marshal.SizeOf(GetType(TOKEN_ELEVATION))
            pTokenElevation = Marshal.AllocHGlobal(cbTokenElevation)
            If (pTokenElevation = IntPtr.Zero) Then
                Throw New Win32Exception
            End If

            ' Retrieve token elevation information.
            If (Not NativeMethods.GetTokenInformation(hToken, _
                TOKEN_INFORMATION_CLASS.TokenElevation, _
                pTokenElevation, cbTokenElevation, cbTokenElevation)) Then
                ' When the process is run on operating systems prior to 
                ' Windows Vista, GetTokenInformation returns false with the 
                ' ERROR_INVALID_PARAMETER error code because 
                ' TokenIntegrityLevel is not supported on those OS's.
                Throw New Win32Exception
            End If

            ' Marshal the TOKEN_ELEVATION struct from native to .NET
            Dim elevation As TOKEN_ELEVATION = Marshal.PtrToStructure( _
            pTokenElevation, GetType(TOKEN_ELEVATION))

            ' TOKEN_ELEVATION.TokenIsElevated is a non-zero value if the 
            ' token has elevated privileges; otherwise, a zero value.
            fIsElevated = (elevation.TokenIsElevated <> 0)

        Finally
            ' Centralized cleanup for all allocated resources.
            If (Not hToken Is Nothing) Then
                hToken.Close()
                hToken = Nothing
            End If
            If (pTokenElevation <> IntPtr.Zero) Then
                Marshal.FreeHGlobal(pTokenElevation)
                pTokenElevation = IntPtr.Zero
                cbTokenElevation = 0
            End If
        End Try

        Return fIsElevated
    End Function


    ''' <summary>
    ''' The function gets the integrity level of the current process. Integrity 
    ''' level is only available on Windows Vista and newer operating systems, thus 
    ''' GetProcessIntegrityLevel throws a C++ exception if it is called on systems 
    ''' prior to Windows Vista.
    ''' </summary>
    ''' <returns>
    ''' Returns the integrity level of the current process. It is usually one of 
    ''' these values:
    ''' 
    '''    SECURITY_MANDATORY_UNTRUSTED_RID - means untrusted level. It is used by 
    '''    processes started by the Anonymous group. Blocks most write access.
    '''    (SID: S-1-16-0x0)
    '''    
    '''    SECURITY_MANDATORY_LOW_RID - means low integrity level. It is used by 
    '''    Protected Mode Internet Explorer. Blocks write acess to most objects 
    '''    (such as files and registry keys) on the system. (SID: S-1-16-0x1000)
    ''' 
    '''    SECURITY_MANDATORY_MEDIUM_RID - means medium integrity level. It is used 
    '''    by normal applications being launched while UAC is enabled. 
    '''    (SID: S-1-16-0x2000)
    '''    
    '''    SECURITY_MANDATORY_HIGH_RID - means high integrity level. It is used by 
    '''    administrative applications launched through elevation when UAC is 
    '''    enabled, or normal applications if UAC is disabled and the user is an 
    '''    administrator. (SID: S-1-16-0x3000)
    '''    
    '''    SECURITY_MANDATORY_SYSTEM_RID - means system integrity level. It is used 
    '''    by services and other system-level applications (such as Wininit, 
    '''    Winlogon, Smss, etc.)  (SID: S-1-16-0x4000)
    ''' 
    ''' </returns>
    ''' <exception cref="System.ComponentModel.Win32Exception">
    ''' When any native Windows API call fails, the function throws a Win32Exception 
    ''' with the last error code.
    ''' </exception>
    Friend Function GetProcessIntegrityLevel() As Integer
        Dim IL As Integer = -1
        Dim hToken As SafeTokenHandle = Nothing
        Dim cbTokenIL As Integer = 0
        Dim pTokenIL As IntPtr = IntPtr.Zero

        Try
            ' Open the access token of the current process with TOKEN_QUERY.
            If (Not NativeMethods.OpenProcessToken(Process.GetCurrentProcess.Handle, _
                NativeMethods.TOKEN_QUERY, hToken)) Then
                Throw New Win32Exception
            End If

            ' Then we must query the size of the integrity level information 
            ' associated with the token. Note that we expect GetTokenInformation to 
            ' return False with the ERROR_INSUFFICIENT_BUFFER error code because we 
            ' have given it a null buffer. On exit cbTokenIL will tell the size of 
            ' the group information.
            If (Not NativeMethods.GetTokenInformation(hToken, _
                TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, _
                IntPtr.Zero, 0, cbTokenIL)) Then
                Dim err As Integer = Marshal.GetLastWin32Error
                If (err <> NativeMethods.ERROR_INSUFFICIENT_BUFFER) Then
                    ' When the process is run on operating systems prior to Windows 
                    ' Vista, GetTokenInformation returns false with the 
                    ' ERROR_INVALID_PARAMETER error code because TokenIntegrityLevel 
                    ' is not supported on those OS's.
                    Throw New Win32Exception(err)
                End If
            End If

            ' Now we allocate a buffer for the integrity level information.
            pTokenIL = Marshal.AllocHGlobal(cbTokenIL)
            If (pTokenIL = IntPtr.Zero) Then
                Throw New Win32Exception
            End If

            ' Now we ask for the integrity level information again. This may fail if 
            ' an administrator has added this account to an additional group between 
            ' our first call to GetTokenInformation and this one.
            If (Not NativeMethods.GetTokenInformation(hToken, _
                TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, _
                pTokenIL, cbTokenIL, cbTokenIL)) Then
                Throw New Win32Exception
            End If

            ' Marshal the TOKEN_MANDATORY_LABEL struct from native to .NET object.
            Dim tokenIL As TOKEN_MANDATORY_LABEL = Marshal.PtrToStructure( _
            pTokenIL, GetType(TOKEN_MANDATORY_LABEL))

            ' Integrity Level SIDs are in the form of S-1-16-0xXXXX. (e.g. 
            ' S-1-16-0x1000 stands for low integrity level SID). There is one and 
            ' only one subauthority.
            Dim pIL As IntPtr = NativeMethods.GetSidSubAuthority(tokenIL.Label.Sid, 0)
            IL = Marshal.ReadInt32(pIL)

        Finally
            ' Centralized cleanup for all allocated resources. 
            If (Not hToken Is Nothing) Then
                hToken.Close()
                hToken = Nothing
            End If
            If (pTokenIL <> IntPtr.Zero) Then
                Marshal.FreeHGlobal(pTokenIL)
                pTokenIL = IntPtr.Zero
                cbTokenIL = 0
            End If
        End Try

        Return IL
    End Function

#End Region


    Private Sub MainForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles MyBase.Load

        ' Get and display whether the primary access token of the process belongs 
        ' to user account that is a member of the local Administrators group even 
        ' if it currently is not elevated (IsUserInAdminGroup).
        Try
            Dim fInAdminGroup As Boolean = Me.IsUserInAdminGroup
            Me.lbInAdminGroup.Text = fInAdminGroup.ToString
        Catch ex As Exception
            Me.lbInAdminGroup.Text = "N/A"
            MessageBox.Show(ex.Message, "An error occurred in IsUserInAdminGroup", _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        ' Get and display whether the process is run as administrator or not 
        ' (IsRunAsAdmin).
        Try
            Dim fIsRunAsAdmin As Boolean = Me.IsRunAsAdmin
            Me.lbIsRunAsAdmin.Text = fIsRunAsAdmin.ToString
        Catch ex As Exception
            Me.lbIsRunAsAdmin.Text = "N/A"
            MessageBox.Show(ex.Message, "An error occurred in IsRunAsAdmin", _
                            MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        ' Get and display the process elevation information (IsProcessElevated) 
        ' and integrity level (GetProcessIntegrityLevel). The information is not 
        ' available on operating systems prior to Windows Vista.
        If (Environment.OSVersion.Version.Major >= 6) Then
            ' Running Windows Vista or later (major version >= 6). 

            Try
                ' Get and display the process elevation information.
                Dim fIsElevated As Boolean = Me.IsProcessElevated
                Me.lbIsElevated.Text = fIsElevated.ToString

                ' Update the Self-elevate button to show the UAC shield icon on 
                ' the UI if the process is not elevated.
                Me.btnElevate.FlatStyle = FlatStyle.System
                NativeMethods.SendMessage(Me.btnElevate.Handle, NativeMethods.BCM_SETSHIELD, _
                                         0, IIf(fIsElevated, IntPtr.Zero, New IntPtr(1)))
            Catch ex As Exception
                Me.lbIsElevated.Text = "N/A"
                MessageBox.Show(ex.Message, "An error occurred in IsProcessElevated", _
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try

            Try
                ' Get and display the process integrity level.
                Dim IL As Integer = Me.GetProcessIntegrityLevel
                Select Case IL
                    Case NativeMethods.SECURITY_MANDATORY_UNTRUSTED_RID
                        Me.lbIntegrityLevel.Text = "Untrusted"
                    Case NativeMethods.SECURITY_MANDATORY_LOW_RID
                        Me.lbIntegrityLevel.Text = "Low"
                    Case NativeMethods.SECURITY_MANDATORY_MEDIUM_RID
                        Me.lbIntegrityLevel.Text = "Medium"
                    Case NativeMethods.SECURITY_MANDATORY_HIGH_RID
                        Me.lbIntegrityLevel.Text = "High"
                    Case NativeMethods.SECURITY_MANDATORY_SYSTEM_RID
                        Me.lbIntegrityLevel.Text = "System"
                    Case Else
                        Me.lbIntegrityLevel.Text = "Unknown"
                End Select
            Catch ex As Exception
                Me.lbIntegrityLevel.Text = "N/A"
                MessageBox.Show(ex.Message, "An error occurred in GetProcessIntegrityLevel!", _
                                MessageBoxButtons.OK, MessageBoxIcon.Hand)
            End Try

        Else
            Me.lbIsElevated.Text = "N/A"
            Me.lbIntegrityLevel.Text = "N/A"
        End If

    End Sub


    Private Sub btnElevate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) _
    Handles btnElevate.Click
        ' Elevate the process if it is not run as administrator.
        If (Not Me.IsRunAsAdmin) Then
            ' Launch itself as administrator
            Dim proc As New ProcessStartInfo
            proc.UseShellExecute = True
            proc.WorkingDirectory = Environment.CurrentDirectory
            proc.FileName = Application.ExecutablePath
            proc.Verb = "runas"

            Try
                Process.Start(proc)
            Catch
                ' The user refused the elevation.
                ' Do nothing and return directly ...
                Return
            End Try

            Application.Exit()  ' Quit itself
        Else
            MessageBox.Show("The process is running as administrator", "UAC")
        End If
    End Sub

End Class
