'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Security.Cryptography
Imports Windows.Security.Cryptography.Core
Imports Windows.Storage.Streams
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports Windows.Security.Cryptography.DataProtection

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario9
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="descriptor">The descriptor string used to protect the data</param>
    Public Async Sub SampleDataProtection(descriptor As String)
        Scenario9Text.Text &= "*** Sample Data Protection for " & descriptor & " ***" & vbCrLf

        Dim Provider As New DataProtectionProvider(descriptor)
        Scenario9Text.Text &= "    DataProtectionProvider is Ready" & vbCrLf

        ' Create random data for protection
        Dim data As IBuffer = CryptographicBuffer.GenerateRandom(73)
        Scenario9Text.Text &= "    Original Data: " & CryptographicBuffer.EncodeToHexString(data) & vbCrLf

        ' Protect the random data
        Dim protectedData As IBuffer = Await Provider.ProtectAsync(data)
        Scenario9Text.Text &= "    Protected Data: " & CryptographicBuffer.EncodeToHexString(protectedData) & vbCrLf

        If CryptographicBuffer.Compare(data, protectedData) Then
            Scenario9Text.Text &= "ProtectAsync returned unprotected data"
            Exit Sub
        End If

        Scenario9Text.Text &= "    ProtectAsync succeeded" & vbCrLf

        ' Unprotect
        Dim Provider2 As New DataProtectionProvider
        Dim unprotectedData As IBuffer = Await Provider2.UnprotectAsync(protectedData)

        If Not CryptographicBuffer.Compare(data, unprotectedData) Then
            Scenario9Text.Text &= "UnprotectAsync returned invalid data"
            Exit Sub
        End If

        Scenario9Text.Text &= "    Unprotected Data: " & CryptographicBuffer.EncodeToHexString(unprotectedData) & vbCrLf
        Scenario9Text.Text &= "*** Done!" & vbCrLf
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Default' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RunSample_Click(sender As Object, e As RoutedEventArgs)
        Dim descriptor As String = tbDescriptor.Text
        SampleDataProtection(descriptor)
    End Sub
End Class
