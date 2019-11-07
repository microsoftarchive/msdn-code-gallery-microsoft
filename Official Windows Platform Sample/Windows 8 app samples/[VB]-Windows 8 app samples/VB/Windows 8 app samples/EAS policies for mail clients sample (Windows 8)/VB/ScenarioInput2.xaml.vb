' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Graphics.Display
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation
Imports Windows.Security.Authentication.Web
Imports Windows.Security.Cryptography.Core
Imports Windows.Security.Cryptography
Imports Windows.Storage.Streams
Imports System.Text
Imports System.Net
Imports System.IO
Imports SDKTemplate
Imports Windows.Security.ExchangeActiveSyncProvisioning

Partial Public NotInheritable Class ScenarioInput2
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub
#End Region

#Region "Use this code if you need access to elements in the output frame - otherwise delete"
    Private Sub rootPage_OutputFrameLoaded(sender As Object, e As Object)
        ' At this point, we know that the Output Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Output Frame.

        ' Get a pointer to the content within the OutputFrame.
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)

        ' Go find the elements that we need for this scenario.
        ' ex: flipView1 = outputFrame.FindName("FlipView1") as FlipView;

    End Sub
#End Region

    Private Sub DebugPrint(Trace As String)
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
        Dim Scenario2DebugArea As TextBox = TryCast(outputFrame.FindName("Scenario2DebugArea"), TextBox)
        Scenario2DebugArea.Text &= Trace & vbCr & vbLf
    End Sub

    Private Sub Launch_Click(sender As Object, e As RoutedEventArgs)
        Try
            Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
            Dim RequestedPolicy As EasClientSecurityPolicy = New Windows.Security.ExchangeActiveSyncProvisioning.EasClientSecurityPolicy()
            Dim ComplianceResult As EasComplianceResults = Nothing


            Dim RequireEncryptionValue As CheckBox = TryCast(outputFrame.FindName("RequireEncryptionValue"), CheckBox)
            If RequireEncryptionValue Is Nothing Then
                RequestedPolicy.RequireEncryption = False
            Else
                If RequireEncryptionValue.IsChecked = True Then
                    RequestedPolicy.RequireEncryption = True
                Else
                    RequestedPolicy.RequireEncryption = False
                End If
            End If

            Dim MinPasswordLengthValue As TextBox = TryCast(outputFrame.FindName("MinPasswordLengthValue"), TextBox)
            If MinPasswordLengthValue Is Nothing OrElse MinPasswordLengthValue.Text.Length = 0 Then
                RequestedPolicy.MinPasswordLength = 0
            Else
                RequestedPolicy.MinPasswordLength = Convert.ToByte(MinPasswordLengthValue.Text)
            End If

            Dim DisallowConvenienceLogonValue As CheckBox = TryCast(outputFrame.FindName("DisallowConvenienceLogonValue"), CheckBox)
            If DisallowConvenienceLogonValue Is Nothing Then
                RequestedPolicy.DisallowConvenienceLogon = False
            Else
                If DisallowConvenienceLogonValue.IsChecked = True Then
                    RequestedPolicy.DisallowConvenienceLogon = True
                Else
                    RequestedPolicy.DisallowConvenienceLogon = False
                End If
            End If

            Dim MinPasswordComplexCharactersValue As TextBox = TryCast(outputFrame.FindName("MinPasswordComplexCharactersValue"), TextBox)
            If MinPasswordComplexCharactersValue Is Nothing OrElse MinPasswordComplexCharactersValue.Text.Length = 0 Then
                RequestedPolicy.MinPasswordComplexCharacters = 0
            Else
                RequestedPolicy.MinPasswordComplexCharacters = Convert.ToByte(MinPasswordComplexCharactersValue.Text)
            End If

            Dim PasswordExpirationValue As TextBox = TryCast(outputFrame.FindName("PasswordExpirationValue"), TextBox)
            If PasswordExpirationValue Is Nothing OrElse PasswordExpirationValue.Text.Length = 0 Then
                RequestedPolicy.PasswordExpiration = TimeSpan.Parse("0")
            Else
                RequestedPolicy.PasswordExpiration = TimeSpan.FromDays(Convert.ToDouble(PasswordExpirationValue.Text))
            End If

            Dim PasswordHistoryValue As TextBox = TryCast(outputFrame.FindName("PasswordHistoryValue"), TextBox)
            If PasswordHistoryValue Is Nothing OrElse PasswordHistoryValue.Text.Length = 0 Then
                RequestedPolicy.PasswordHistory = 0
            Else
                RequestedPolicy.PasswordHistory = Convert.ToByte(PasswordHistoryValue.Text)
            End If

            Dim MaxPasswordFailedAttemptsValue As TextBox = TryCast(outputFrame.FindName("MaxPasswordFailedAttemptsValue"), TextBox)
            If MaxPasswordFailedAttemptsValue Is Nothing OrElse MaxPasswordFailedAttemptsValue.Text.Length = 0 Then
                RequestedPolicy.MaxPasswordFailedAttempts = 0
            Else
                RequestedPolicy.MaxPasswordFailedAttempts = Convert.ToByte(MaxPasswordFailedAttemptsValue.Text)
            End If

            Dim MaxInactivityTimeLockValue As TextBox = TryCast(outputFrame.FindName("MaxInactivityTimeLockValue"), TextBox)
            If MaxInactivityTimeLockValue Is Nothing OrElse MaxInactivityTimeLockValue.Text.Length = 0 Then
                RequestedPolicy.MaxInactivityTimeLock = TimeSpan.Parse("0")
            Else
                RequestedPolicy.MaxInactivityTimeLock = TimeSpan.FromSeconds(Convert.ToDouble(MaxInactivityTimeLockValue.Text))
            End If

            ComplianceResult = RequestedPolicy.CheckCompliance()

            Dim RequireEncryptionResult As TextBox = TryCast(outputFrame.FindName("RequireEncryptionResult"), TextBox)
            RequireEncryptionResult.Text = ComplianceResult.RequireEncryptionResult.ToString

            Dim MinPasswordLengthResult As TextBox = TryCast(outputFrame.FindName("MinPasswordLengthResult"), TextBox)
            MinPasswordLengthResult.Text = ComplianceResult.MinPasswordLengthResult.ToString

            Dim DisallowConvenienceLogonResult As TextBox = TryCast(outputFrame.FindName("DisallowConvenienceLogonResult"), TextBox)
            DisallowConvenienceLogonResult.Text = ComplianceResult.DisallowConvenienceLogonResult.ToString

            Dim MinPasswordComplexCharactersResult As TextBox = TryCast(outputFrame.FindName("MinPasswordComplexCharactersResult"), TextBox)
            MinPasswordComplexCharactersResult.Text = ComplianceResult.MinPasswordComplexCharactersResult.ToString

            Dim PasswordExpirationResult As TextBox = TryCast(outputFrame.FindName("PasswordExpirationResult"), TextBox)
            PasswordExpirationResult.Text = ComplianceResult.PasswordExpirationResult.ToString

            Dim PasswordHistoryResult As TextBox = TryCast(outputFrame.FindName("PasswordHistoryResult"), TextBox)
            PasswordHistoryResult.Text = ComplianceResult.PasswordHistoryResult.ToString

            Dim MaxPasswordFailedAttemptsResult As TextBox = TryCast(outputFrame.FindName("MaxPasswordFailedAttemptsResult"), TextBox)
            MaxPasswordFailedAttemptsResult.Text = ComplianceResult.MaxPasswordFailedAttemptsResult.ToString

            Dim MaxInactivityTimeLockResult As TextBox = TryCast(outputFrame.FindName("MaxInactivityTimeLockResult"), TextBox)
            MaxInactivityTimeLockResult.Text = ComplianceResult.MaxInactivityTimeLockResult.ToString
        Catch [Error] As Exception
            '
            ' Bad Parameter, COM Unavailable errors are to be handled here.
            '
            DebugPrint([Error].ToString)
        End Try

    End Sub

    Private Sub Reset_Click(sender As Object, e As RoutedEventArgs)
        Try
            Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)

            Dim RequireEncryptionValue As CheckBox = TryCast(outputFrame.FindName("RequireEncryptionValue"), CheckBox)
            RequireEncryptionValue.IsChecked = False

            Dim MinPasswordLengthValue As TextBox = TryCast(outputFrame.FindName("MinPasswordLengthValue"), TextBox)
            MinPasswordLengthValue.Text = ""

            Dim DisallowConvenienceLogonValue As CheckBox = TryCast(outputFrame.FindName("DisallowConvenienceLogonValue"), CheckBox)
            DisallowConvenienceLogonValue.IsChecked = False

            Dim MinPasswordComplexCharactersValue As TextBox = TryCast(outputFrame.FindName("MinPasswordComplexCharactersValue"), TextBox)
            MinPasswordComplexCharactersValue.Text = ""

            Dim PasswordExpirationValue As TextBox = TryCast(outputFrame.FindName("PasswordExpirationValue"), TextBox)
            PasswordExpirationValue.Text = ""

            Dim PasswordHistoryValue As TextBox = TryCast(outputFrame.FindName("PasswordHistoryValue"), TextBox)
            PasswordHistoryValue.Text = ""

            Dim MaxPasswordFailedAttemptsValue As TextBox = TryCast(outputFrame.FindName("MaxPasswordFailedAttemptsValue"), TextBox)
            MaxPasswordFailedAttemptsValue.Text = ""

            Dim MaxInactivityTimeLockValue As TextBox = TryCast(outputFrame.FindName("MaxInactivityTimeLockValue"), TextBox)
            MaxInactivityTimeLockValue.Text = ""

            Dim RequireEncryptionResult As TextBox = TryCast(outputFrame.FindName("RequireEncryptionResult"), TextBox)
            RequireEncryptionResult.Text = ""

            Dim MinPasswordLengthResult As TextBox = TryCast(outputFrame.FindName("MinPasswordLengthResult"), TextBox)
            MinPasswordLengthResult.Text = ""

            Dim DisallowConvenienceLogonResult As TextBox = TryCast(outputFrame.FindName("DisallowConvenienceLogonResult"), TextBox)
            DisallowConvenienceLogonResult.Text = ""

            Dim MinPasswordComplexCharactersResult As TextBox = TryCast(outputFrame.FindName("MinPasswordComplexCharactersResult"), TextBox)
            MinPasswordComplexCharactersResult.Text = ""

            Dim PasswordExpirationResult As TextBox = TryCast(outputFrame.FindName("PasswordExpirationResult"), TextBox)
            PasswordExpirationResult.Text = ""

            Dim PasswordHistoryResult As TextBox = TryCast(outputFrame.FindName("PasswordHistoryResult"), TextBox)
            PasswordHistoryResult.Text = ""

            Dim MaxPasswordFailedAttemptsResult As TextBox = TryCast(outputFrame.FindName("MaxPasswordFailedAttemptsResult"), TextBox)
            MaxPasswordFailedAttemptsResult.Text = ""

            Dim MaxInactivityTimeLockResult As TextBox = TryCast(outputFrame.FindName("MaxInactivityTimeLockResult"), TextBox)
            MaxInactivityTimeLockResult.Text = ""
        Catch [Error] As Exception
            '
            ' Bad Parameter, COM Unavailable errors are to be handled here.
            '
            DebugPrint([Error].ToString)
        End Try

    End Sub
End Class
