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
Partial Public NotInheritable Class Scenario6
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    ' Initialize a nonce value.
    Shared NonceBytes As Byte() = {0, 0, 0, 0, 0, 0,0, 0, 0, 0, 0, 0}

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
    ''' This utility function returns a nonce value for authenticated encryption modes.
    ''' </summary>
    ''' <returns></returns>
    Private Function GetNonce() As IBuffer
        ' NOTE: 
        ' 
        ' The security best practises require that the Encrypt operation
        ' not be called more than once with the same nonce for the same key.
        ' 
        ' Nonce can be predictable, but must be unique per secure session.
        NonceBytes(0) = CByte(NonceBytes(0) + 1)
        For i = 0 To NonceBytes.Length - 2
            If NonceBytes(i) = 255 Then
                NonceBytes(i + 1) = CByte(NonceBytes(i + 1) + 1)
            End If
        Next

        Return CryptographicBuffer.CreateFromByteArray(NonceBytes)
    End Function

    ''' <summary>
    ''' This is the click handler for the 'RunSample' button.  It is responsible for executing the sample code.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RunSample_Click(sender As Object, e As RoutedEventArgs)
        Dim algName As String = AlgorithmNames.SelectionBoxItem.ToString
        Dim keySize As UInt32 = UInt32.Parse(KeySizes.SelectionBoxItem.ToString)
        Scenario6Text.Text = ""

        Dim Decrypted As IBuffer
        Dim Data As IBuffer
        Dim Nonce As IBuffer
        Dim Cookie As String = "Some Cookie to Encrypt"

        ' Data to encrypt.
        Data = CryptographicBuffer.ConvertStringToBinary(Cookie, BinaryStringEncoding.Utf16LE)

        ' Created a SymmetricKeyAlgorithmProvider object for the algorithm specified on input.
        Dim Algorithm As SymmetricKeyAlgorithmProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(algName)

        Scenario6Text.Text &= "*** Sample Authenticated Encryption" & vbCrLf
        Scenario6Text.Text &= "    Algorithm Name: " & Algorithm.AlgorithmName & vbCrLf
        Scenario6Text.Text &= "    Key Size: " & keySize & vbCrLf
        Scenario6Text.Text &= "    Block length: " & Algorithm.BlockLength & vbCrLf

        ' Generate a random key.
        Dim keymaterial As IBuffer = CryptographicBuffer.GenerateRandom(CUInt((keySize + 7) / 8 - 1))
        Dim key As CryptographicKey = Algorithm.CreateSymmetricKey(keymaterial)


        ' Microsoft GCM implementation requires a 12 byte Nonce.
        ' Microsoft CCM implementation requires a 7-13 byte Nonce.
        Nonce = GetNonce()

        ' Encrypt and create an authenticated tag on the encrypted data.
        Dim Encrypted As EncryptedAndAuthenticatedData = CryptographicEngine.EncryptAndAuthenticate(key, Data, Nonce, Nothing)

        Scenario6Text.Text &= "    Plain text: " & Data.Length & " bytes" & vbCrLf
        Scenario6Text.Text &= "    Encrypted: " & Encrypted.EncryptedData.Length & " bytes" & vbCrLf
        Scenario6Text.Text &= "    AuthTag: " & Encrypted.AuthenticationTag.Length & " bytes" & vbCrLf

        ' Create another instance of the key from the same material.
        Dim key2 As CryptographicKey = Algorithm.CreateSymmetricKey(keymaterial)

        If key.KeySize <> key2.KeySize Then
            Scenario6Text.Text &= "CreateSymmetricKey failed!  The imported key's size did not match the original's!"
            Exit Sub
        End If

        ' Decrypt and verify the authenticated tag.
        Decrypted = CryptographicEngine.DecryptAndAuthenticate(key2, Encrypted.EncryptedData, Nonce, Encrypted.AuthenticationTag, Nothing)

        If Not CryptographicBuffer.Compare(Decrypted, Data) Then
            Scenario6Text.Text &= "Decrypted does not match original!"
            Exit Sub
        End If
    End Sub
End Class
