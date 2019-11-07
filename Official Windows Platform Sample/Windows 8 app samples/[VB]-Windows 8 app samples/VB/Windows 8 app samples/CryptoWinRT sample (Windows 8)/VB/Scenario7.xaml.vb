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
Partial Public NotInheritable Class Scenario7
    Inherits SDKTemplate.Common.LayoutAwarePage

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
        Dim KeySize As UInt32 = UInt32.Parse(KeySizes.SelectionBoxItem.ToString)
        Scenario7Text.Text = ""

        Dim Data As IBuffer = Nothing
        Dim cookie As String = "Some cookie to encrypt"

        Select Case AlgorithmNames.SelectedIndex
            Case 0
                Data = CryptographicBuffer.ConvertStringToBinary(cookie, BinaryStringEncoding.Utf16LE)
                Exit Select

                ' OAEP Padding depends on key size, message length and hash block length
                ' 
                ' The maximum plaintext length is KeyLength - 2*HashBlock - 2
                '
                ' OEAP padding supports an optional label with the length is restricted by plaintext/key/hash sizes.
                ' Here we just use a small label.
            Case 1
                Data = CryptographicBuffer.GenerateRandom(CUInt(1024 / 8 - 2 * 20 - 2))
                Exit Select
            Case 2
                Data = CryptographicBuffer.GenerateRandom(CUInt(1024 / 8 - 2 * (256 / 8) - 2))
                Exit Select
            Case 3
                Data = CryptographicBuffer.GenerateRandom(CUInt(2048 / 8 - 2 * (384 / 8) - 2))
                Exit Select
            Case 4
                Data = CryptographicBuffer.GenerateRandom(CUInt(2048 / 8 - 2 * (512 / 8) - 2))
                Exit Select
            Case Else
                Scenario7Text.Text &= "An invalid algorithm was selected"
                Exit Select
        End Select

        Dim Encrypted As IBuffer
        Dim Decrypted As IBuffer
        Dim blobOfPublicKey As IBuffer
        Dim blobOfKeyPair As IBuffer

        ' Crate an AsymmetricKeyAlgorithmProvider object for the algorithm specified on input.
        Dim Algorithm As AsymmetricKeyAlgorithmProvider = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(algName)

        Scenario7Text.Text &= "*** Sample Encryption Algorithm" & vbCrLf
        Scenario7Text.Text &= "    Algorithm Name: " & Algorithm.AlgorithmName & vbCrLf
        Scenario7Text.Text &= "    Key Size: " & KeySize & vbCrLf

        ' Generate a random key.
        Dim keyPair As CryptographicKey = Algorithm.CreateKeyPair(KeySize)

        ' Encrypt the data.
        Try
            Encrypted = CryptographicEngine.Encrypt(keyPair, Data, Nothing)
        Catch ex As ArgumentException
            Scenario7Text.Text &= ex.Message & vbCrLf
            Scenario7Text.Text &= "An invalid key size was selected for the given algorithm." & vbCrLf
            Exit Sub
        End Try

        Scenario7Text.Text &= "    Plain text: " & Data.Length & " bytes" & vbCrLf
        Scenario7Text.Text &= "    Encrypted: " & Encrypted.Length & " bytes" & vbCrLf

        ' Export the public key.
        blobOfPublicKey = keyPair.ExportPublicKey
        blobOfKeyPair = keyPair.Export

        ' Import the public key.
        Dim keyPublic As CryptographicKey = Algorithm.ImportPublicKey(blobOfPublicKey)
        If keyPublic.KeySize <> keyPair.KeySize Then
            Scenario7Text.Text &= "ImportPublicKey failed!  The imported key's size did not match the original's!"
            Exit Sub
        End If

        ' Import the key pair.
        keyPair = Algorithm.ImportKeyPair(blobOfKeyPair)

        ' Check the key size of the imported key.
        If keyPublic.KeySize <> keyPair.KeySize Then
            Scenario7Text.Text &= "ImportKeyPair failed!  The imported key's size did not match the original's!"
            Exit Sub
        End If

        ' Decrypt the data.
        Decrypted = CryptographicEngine.Decrypt(keyPair, Encrypted, Nothing)

        If Not CryptographicBuffer.Compare(Decrypted, Data) Then
            Scenario7Text.Text &= "Decrypted data does not match original!"
            Exit Sub
        End If
    End Sub
End Class
