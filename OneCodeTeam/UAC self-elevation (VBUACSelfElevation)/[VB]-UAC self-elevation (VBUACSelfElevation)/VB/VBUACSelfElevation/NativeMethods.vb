'***************************** Module Header *******************************\
' Module Name:  NativeMethod.vb
' Project:      VBUACSelfElevation
' Copyright (c) Microsoft Corporation.
' 
' The file defines the P/Invoke signatures and native data structures.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

#Region "Imports directives"

Imports System.Runtime.InteropServices
Imports Microsoft.Win32.SafeHandles

#End Region


''' <summary>
''' The TOKEN_INFORMATION_CLASS enumeration type contains values that specify 
''' the type of information being assigned to or retrieved from an access 
''' token.
''' </summary>
Friend Enum TOKEN_INFORMATION_CLASS
    TokenUser = 1
    TokenGroups
    TokenPrivileges
    TokenOwner
    TokenPrimaryGroup
    TokenDefaultDacl
    TokenSource
    TokenType
    TokenImpersonationLevel
    TokenStatistics
    TokenRestrictedSids
    TokenSessionId
    TokenGroupsAndPrivileges
    TokenSessionReference
    TokenSandBoxInert
    TokenAuditPolicy
    TokenOrigin
    TokenElevationType
    TokenLinkedToken
    TokenElevation
    TokenHasRestrictions
    TokenAccessInformation
    TokenVirtualizationAllowed
    TokenVirtualizationEnabled
    TokenIntegrityLevel
    TokenUIAccess
    TokenMandatoryPolicy
    TokenLogonSid
    MaxTokenInfoClass
End Enum


''' <summary>
''' The WELL_KNOWN_SID_TYPE enumeration type is a list of commonly used 
''' security identifiers (SIDs). Programs can pass these values to the 
''' CreateWellKnownSid function to create a SID from this list.
''' </summary>
Friend Enum WELL_KNOWN_SID_TYPE
    WinNullSid = 0
    WinWorldSid = 1
    WinLocalSid = 2
    WinCreatorOwnerSid = 3
    WinCreatorGroupSid = 4
    WinCreatorOwnerServerSid = 5
    WinCreatorGroupServerSid = 6
    WinNtAuthoritySid = 7
    WinDialupSid = 8
    WinNetworkSid = 9
    WinBatchSid = 10
    WinInteractiveSid = 11
    WinServiceSid = 12
    WinAnonymousSid = 13
    WinProxySid = 14
    WinEnterpriseControllersSid = 15
    WinSelfSid = 16
    WinAuthenticatedUserSid = 17
    WinRestrictedCodeSid = 18
    WinTerminalServerSid = 19
    WinRemoteLogonIdSid = 20
    WinLogonIdsSid = 21
    WinLocalSystemSid = 22
    WinLocalServiceSid = 23
    WinNetworkServiceSid = 24
    WinBuiltinDomainSid = 25
    WinBuiltinAdministratorsSid = 26
    WinBuiltinUsersSid = 27
    WinBuiltinGuestsSid = 28
    WinBuiltinPowerUsersSid = 29
    WinBuiltinAccountOperatorsSid = 30
    WinBuiltinSystemOperatorsSid = 31
    WinBuiltinPrintOperatorsSid = 32
    WinBuiltinBackupOperatorsSid = 33
    WinBuiltinReplicatorSid = 34
    WinBuiltinPreWindows2000CompatibleAccessSid = 35
    WinBuiltinRemoteDesktopUsersSid = 36
    WinBuiltinNetworkConfigurationOperatorsSid = 37
    WinAccountAdministratorSid = 38
    WinAccountGuestSid = 39
    WinAccountKrbtgtSid = 40
    WinAccountDomainAdminsSid = 41
    WinAccountDomainUsersSid = 42
    WinAccountDomainGuestsSid = 43
    WinAccountComputersSid = 44
    WinAccountControllersSid = 45
    WinAccountCertAdminsSid = 46
    WinAccountSchemaAdminsSid = 47
    WinAccountEnterpriseAdminsSid = 48
    WinAccountPolicyAdminsSid = 49
    WinAccountRasAndIasServersSid = 50
    WinNTLMAuthenticationSid = 51
    WinDigestAuthenticationSid = 52
    WinSChannelAuthenticationSid = 53
    WinThisOrganizationSid = 54
    WinOtherOrganizationSid = 55
    WinBuiltinIncomingForestTrustBuildersSid = 56
    WinBuiltinPerfMonitoringUsersSid = 57
    WinBuiltinPerfLoggingUsersSid = 58
    WinBuiltinAuthorizationAccessSid = 59
    WinBuiltinTerminalServerLicenseServersSid = 60
    WinBuiltinDCOMUsersSid = 61
    WinBuiltinIUsersSid = 62
    WinIUserSid = 63
    WinBuiltinCryptoOperatorsSid = 64
    WinUntrustedLabelSid = 65
    WinLowLabelSid = 66
    WinMediumLabelSid = 67
    WinHighLabelSid = 68
    WinSystemLabelSid = 69
    WinWriteRestrictedCodeSid = 70
    WinCreatorOwnerRightsSid = 71
    WinCacheablePrincipalsGroupSid = 72
    WinNonCacheablePrincipalsGroupSid = 73
    WinEnterpriseReadonlyControllersSid = 74
    WinAccountReadonlyControllersSid = 75
    WinBuiltinEventLogReadersGroup = 76
    WinNewEnterpriseReadonlyControllersSid = 77
    WinBuiltinCertSvcDComAccessGroup = 78
End Enum


''' <summary>
''' The SECURITY_IMPERSONATION_LEVEL enumeration type contains values 
''' that specify security impersonation levels. Security impersonation 
''' levels govern the degree to which a server process can act on behalf 
''' of a client process.
''' </summary>
Friend Enum SECURITY_IMPERSONATION_LEVEL
    SecurityAnonymous = 0
    SecurityIdentification
    SecurityImpersonation
    SecurityDelegation
End Enum


''' <summary>
''' The TOKEN_ELEVATION_TYPE enumeration indicates the elevation type of 
''' token being queried by the GetTokenInformation function or set by 
''' the SetTokenInformation function.
''' </summary>
Friend Enum TOKEN_ELEVATION_TYPE
    TokenElevationTypeDefault = 1
    TokenElevationTypeFull
    TokenElevationTypeLimited
End Enum


''' <summary>
''' The structure represents a security identifier (SID) and its attributes.
''' SIDs are used to uniquely identify users or groups.
''' </summary>
<StructLayout(LayoutKind.Sequential)> _
Friend Structure SID_AND_ATTRIBUTES
    Public Sid As IntPtr
    Public Attributes As UInteger
End Structure


''' <summary>
''' The structure indicates whether a token has elevated privileges.
''' </summary>
<StructLayout(LayoutKind.Sequential)> _
Friend Structure TOKEN_ELEVATION
    Public TokenIsElevated As Integer
End Structure


''' <summary>
''' The structure specifies the mandatory integrity level for a token.
''' </summary>
<StructLayout(LayoutKind.Sequential)> _
Friend Structure TOKEN_MANDATORY_LABEL
    Public Label As SID_AND_ATTRIBUTES
End Structure


''' <summary>
''' Represents a wrapper class for a token handle.
''' </summary>
Friend Class SafeTokenHandle
    Inherits SafeHandleZeroOrMinusOneIsInvalid

    Private Sub New()
        MyBase.New(True)
    End Sub

    Friend Sub New(ByVal handle As IntPtr)
        MyBase.New(True)
        MyBase.SetHandle(handle)
    End Sub

    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Friend Shared Function CloseHandle(ByVal handle As IntPtr) As Boolean
    End Function

    Protected Overrides Function ReleaseHandle() As Boolean
        Return SafeTokenHandle.CloseHandle(MyBase.handle)
    End Function

End Class


Friend Class NativeMethods

    ' Token Specific Access Rights

    Public Const STANDARD_RIGHTS_REQUIRED As UInt32 = &HF0000
    Public Const STANDARD_RIGHTS_READ As UInt32 = &H20000
    Public Const TOKEN_ASSIGN_PRIMARY As UInt32 = 1
    Public Const TOKEN_DUPLICATE As UInt32 = 2
    Public Const TOKEN_IMPERSONATE As UInt32 = 4
    Public Const TOKEN_QUERY As UInt32 = 8
    Public Const TOKEN_QUERY_SOURCE As UInt32 = &H10
    Public Const TOKEN_ADJUST_PRIVILEGES As UInt32 = &H20
    Public Const TOKEN_ADJUST_GROUPS As UInt32 = &H40
    Public Const TOKEN_ADJUST_DEFAULT As UInt32 = &H80
    Public Const TOKEN_ADJUST_SESSIONID As UInt32 = &H100
    Public Const TOKEN_READ As UInt32 = (STANDARD_RIGHTS_READ Or TOKEN_QUERY)
    Public Const TOKEN_ALL_ACCESS As UInt32 = (STANDARD_RIGHTS_REQUIRED Or _
        TOKEN_ASSIGN_PRIMARY Or TOKEN_DUPLICATE Or TOKEN_IMPERSONATE Or _
        TOKEN_QUERY Or TOKEN_QUERY_SOURCE Or TOKEN_ADJUST_PRIVILEGES Or _
        TOKEN_ADJUST_GROUPS Or TOKEN_ADJUST_DEFAULT Or TOKEN_ADJUST_SESSIONID)


    Public Const ERROR_INSUFFICIENT_BUFFER As Int32 = 122


    ' Integrity Levels

    Public Const SECURITY_MANDATORY_UNTRUSTED_RID As Integer = 0
    Public Const SECURITY_MANDATORY_LOW_RID As Integer = &H1000
    Public Const SECURITY_MANDATORY_MEDIUM_RID As Integer = &H2000
    Public Const SECURITY_MANDATORY_HIGH_RID As Integer = &H3000
    Public Const SECURITY_MANDATORY_SYSTEM_RID As Integer = &H4000


    ''' <summary>
    ''' The function opens the access token associated with a process.
    ''' </summary>
    ''' <param name="hProcess">
    ''' A handle to the process whose access token is opened.
    ''' </param>
    ''' <param name="desiredAccess">
    ''' Specifies an access mask that specifies the requested types of access 
    ''' to the access token. 
    ''' </param>
    ''' <param name="hToken">
    ''' Outputs a handle that identifies the newly opened access token 
    ''' when the function returns.
    ''' </param>
    ''' <returns></returns>
    <DllImport("advapi32", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function OpenProcessToken( _
        ByVal hProcess As IntPtr, _
        ByVal desiredAccess As UInt32, _
        <Out()> ByRef hToken As SafeTokenHandle) _
        As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function


    ''' <summary>
    ''' The function creates a new access token that duplicates one already
    ''' in existence.
    ''' </summary>
    ''' <param name="ExistingTokenHandle">
    ''' A handle to an access token opened with TOKEN_DUPLICATE access.
    ''' </param>
    ''' <param name="ImpersonationLevel">
    ''' Specifies a SECURITY_IMPERSONATION_LEVEL enumerated type that 
    ''' supplies the impersonation level of the new token.
    ''' </param>
    ''' <param name="DuplicateTokenHandle">
    ''' Outputs a handle to the duplicate token. 
    ''' </param>
    ''' <returns></returns>
    <DllImport("advapi32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function DuplicateToken( _
        ByVal ExistingTokenHandle As SafeTokenHandle, _
        ByVal ImpersonationLevel As SECURITY_IMPERSONATION_LEVEL, _
        <Out()> ByRef DuplicateTokenHandle As SafeTokenHandle) _
        As Boolean
    End Function


    ''' <summary>
    ''' The function retrieves a specified type of information about an 
    ''' access token. The calling process must have appropriate access rights
    ''' to obtain the information.
    ''' </summary>
    ''' <param name="hToken">
    ''' A handle to an access token from which information is retrieved.
    ''' </param>
    ''' <param name="tokenInfoClass">
    ''' Specifies a value from the TOKEN_INFORMATION_CLASS enumerated type to 
    ''' identify the type of information the function retrieves.
    ''' </param>
    ''' <param name="pTokenInfo">
    ''' A pointer to a buffer the function fills with the requested 
    ''' information.
    ''' </param>
    ''' <param name="tokenInfoLength">
    ''' Specifies the size, in bytes, of the buffer pointed to by the 
    ''' TokenInformation parameter. 
    ''' </param>
    ''' <param name="returnLength">
    ''' A pointer to a variable that receives the number of bytes needed for 
    ''' the buffer pointed to by the TokenInformation parameter. 
    ''' </param>
    ''' <returns></returns>
    <DllImport("advapi32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function GetTokenInformation( _
        ByVal hToken As SafeTokenHandle, _
        ByVal tokenInfoClass As TOKEN_INFORMATION_CLASS, _
        ByVal pTokenInfo As IntPtr, _
        ByVal tokenInfoLength As Integer, _
        <Out()> ByRef returnLength As Integer) _
        As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function


    ''' <summary>
    ''' Sets the elevation required state for a specified button or command 
    ''' link to display an elevated icon. 
    ''' </summary>
    ''' <remarks></remarks>
    Public Const BCM_SETSHIELD As UInt32 = &H160C


    ''' <summary>
    ''' Sends the specified message to a window or windows. The function 
    ''' calls the window procedure for the specified window and does not 
    ''' return until the window procedure has processed the message. 
    ''' </summary>
    ''' <param name="hWnd">
    ''' Handle to the window whose window procedure will receive the message.
    ''' </param>
    ''' <param name="Msg">Specifies the message to be sent.</param>
    ''' <param name="wParam">
    ''' Specifies additional message-specific information.
    ''' </param>
    ''' <param name="lParam">
    ''' Specifies additional message-specific information.
    ''' </param>
    ''' <returns></returns>
    <DllImport("user32", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function SendMessage( _
        ByVal hWnd As IntPtr, _
        ByVal Msg As UInt32, _
        ByVal wParam As Integer, _
        ByVal lParam As IntPtr) _
        As Integer
    End Function


    ''' <summary>
    ''' The function returns a pointer to a specified subauthority in a 
    ''' security identifier (SID). The subauthority value is a relative 
    ''' identifier (RID).
    ''' </summary>
    ''' <param name="pSid">
    ''' A pointer to the SID structure from which a pointer to a subauthority
    ''' is to be returned.
    ''' </param>
    ''' <param name="nSubAuthority">
    ''' Specifies an index value identifying the subauthority array element 
    ''' whose address the function will return.
    ''' </param>
    ''' <returns>
    ''' If the function succeeds, the return value is a pointer to the 
    ''' specified SID subauthority. To get extended error information, call
    ''' GetLastError. If the function fails, the return value is undefined.
    ''' The function fails if the specified SID structure is not valid or if 
    ''' the index value specified by the nSubAuthority parameter is out of
    ''' bounds. 
    ''' </returns>
    <DllImport("advapi32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Public Shared Function GetSidSubAuthority( _
        ByVal pSid As IntPtr, _
        ByVal nSubAuthority As UInt32) _
        As IntPtr
    End Function

End Class
