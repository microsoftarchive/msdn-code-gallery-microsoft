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
Partial Public NotInheritable Class Scenario8
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
        Dim KeySize As UInt32 = UInt32.Parse(KeySizes.SelectionBoxItem.ToString)
        Scenario8Text.Text = ""

        Dim keyPair As CryptographicKey
        Dim blobOfPublicKey As IBuffer
        Dim blobOfKeyPair As IBuffer
        Dim cookie As String = "Some Data to sign"
        Dim Data As IBuffer = CryptographicBuffer.ConvertStringToBinary(cookie, BinaryStringEncoding.Utf16BE)

        ' Create an AsymmetricKeyAlgorithmProvider object for the algorithm specified on input.
        Dim Algorithm As AsymmetricKeyAlgorithmProvider = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(algName)

        Scenario8Text.Text &= "*** Sample Signature Algorithm" & vbCrLf
        Scenario8Text.Text &= "    Algorithm Name: " & Algorithm.AlgorithmName & vbCrLf
        Scenario8Text.Text &= "    Key Size: " & KeySize & vbCrLf

        ' Generate a key pair.
        Try
            keyPair = Algorithm.CreateKeyPair(KeySize)
        Catch ex As ArgumentException
            Scenario8Text.Text &= ex.Message & vbCrLf
            Scenario8Text.Text &= "An invalid key size was specified for the given algorithm."
            Exit Sub
        End Try
        ' Sign the data by using the generated key.
        Dim Signature As IBuffer = CryptographicEngine.Sign(keyPair, Data)
        Scenario8Text.Text &= "    Data was successfully signed." & vbCrLf

        ' Export the public key.
        blobOfPublicKey = keyPair.ExportPublicKey
        blobOfKeyPair = keyPair.Export
        Scenario8Text.Text &= "    Key pair was successfully exported." & vbCrLf

        ' Import the public key.
        Dim keyPublic As CryptographicKey = Algorithm.ImportPublicKey(blobOfPublicKey)

        ' Check the key size.
        If keyPublic.KeySize <> keyPair.KeySize Then
            Scenario8Text.Text &= "ImportPublicKey failed!  The imported key's size did not match the original's!"
            Exit Sub
        End If
        Scenario8Text.Text &= "    Public key was successfully imported." & vbCrLf

        ' Import the key pair.
        keyPair = Algorithm.ImportKeyPair(blobOfKeyPair)

        ' Check the key size.
        If keyPublic.KeySize <> keyPair.KeySize Then
            Scenario8Text.Text &= "ImportKeyPair failed!  The imported key's size did not match the original's!"
            Exit Sub
        End If
        Scenario8Text.Text &= "    Key pair was successfully imported." & vbCrLf

        ' Verify the signature by using the public key.
        If Not CryptographicEngine.VerifySignature(keyPublic, Data, Signature) Then
            Scenario8Text.Text &= "Signature verification failed!"
            Exit Sub
        End If
        Scenario8Text.Text &= "    Signature was successfully verified." & vbCrLf
    End Sub
End Class

