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

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario5
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AlgorithmNames.SelectedIndex = 0
        KeySizes.SelectedIndex = 0
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'RunSample' button.  It is responsible for executing the sample code.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RunSample_Click(sender As Object, e As RoutedEventArgs)
        Dim algName As String = AlgorithmNames.SelectionBoxItem.ToString
        Dim keySize As UInt32 = UInt32.Parse(KeySizes.SelectionBoxItem.ToString)
        Scenario5Text.Text = ""

        Dim encrypted As IBuffer
        Dim decrypted As IBuffer
        Dim buffer As IBuffer
        Dim iv As IBuffer = Nothing
        Dim blockCookie As String = "1234567812345678"
        ' 16 bytes
        ' Open the algorithm provider for the algorithm specified on input.
        Dim Algorithm As SymmetricKeyAlgorithmProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(algName)

        Scenario5Text.Text &= vbCrLf & "*** Sample Cipher Encryption" & vbCrLf
        Scenario5Text.Text &= "    Algorithm Name: " & Algorithm.AlgorithmName & vbCrLf
        Scenario5Text.Text &= "    Key Size: " & keySize & vbCrLf
        Scenario5Text.Text &= "    Block length: " & Algorithm.BlockLength & vbCrLf

        ' Generate a symmetric key.
        Dim keymaterial As IBuffer = CryptographicBuffer.GenerateRandom(CUInt((keySize + 7) / 8 - 1))
        Dim key As CryptographicKey
        Try
            key = Algorithm.CreateSymmetricKey(keymaterial)
        Catch ex As ArgumentException
            Scenario5Text.Text &= ex.Message & vbCrLf
            Scenario5Text.Text &= "An invalid key size was selected for the given algorithm." & vbCrLf
            Exit Sub
        End Try

        ' CBC mode needs Initialization vector, here just random data.
        ' IV property will be set on "Encrypted".
        If algName.Contains("CBC") Then
            iv = CryptographicBuffer.GenerateRandom(Algorithm.BlockLength)
        End If

        ' Set the data to encrypt. 
        buffer = CryptographicBuffer.ConvertStringToBinary(blockCookie, BinaryStringEncoding.Utf8)

        ' Encrypt and create an authenticated tag.
        encrypted = Windows.Security.Cryptography.Core.CryptographicEngine.Encrypt(key, buffer, iv)

        Scenario5Text.Text &= "    Plain text: " & buffer.Length & " bytes" & vbCrLf
        Scenario5Text.Text &= "    Encrypted: " & encrypted.Length & " bytes" & vbCrLf

        ' Create another instance of the key from the same material.
        Dim key2 As CryptographicKey = Algorithm.CreateSymmetricKey(keymaterial)

        If key.KeySize <> key2.KeySize Then
            Scenario5Text.Text &= "CreateSymmetricKey failed!  The imported key's size did not match the original's!"
            Exit Sub
        End If

        ' Decrypt and verify the authenticated tag.
        decrypted = Windows.Security.Cryptography.Core.CryptographicEngine.Decrypt(key2, encrypted, iv)

        If Not CryptographicBuffer.Compare(decrypted, buffer) Then
            Scenario5Text.Text &= "Decrypted does not match original!"
            Exit Sub
        End If
    End Sub
End Class
