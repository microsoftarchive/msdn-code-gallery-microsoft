'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports Windows.Security.Cryptography.Core
Imports Windows.Storage.Streams
Imports Windows.Security.Cryptography

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario3
    Inherits Global.SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AlgorithmNames.SelectedIndex = 0
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
        Scenario3Text.Text = ""

        ' Create a sample message.
        Dim Message As String = "Some message to authenticate"

        ' Created a MacAlgorithmProvider object for the algorithm specified on input.
        Dim Algorithm As MacAlgorithmProvider = MacAlgorithmProvider.OpenAlgorithm(algName)

        Scenario3Text.Text &= "*** Sample Hmac Algorithm: " & Algorithm.AlgorithmName & vbCrLf

        ' Create a key.
        Dim keymaterial As IBuffer = CryptographicBuffer.GenerateRandom(Algorithm.MacLength)
        Dim hmacKey As CryptographicKey = Algorithm.CreateKey(keymaterial)

        ' Sign the message by using the key.
        Dim signature As IBuffer = Windows.Security.Cryptography.Core.CryptographicEngine.Sign(hmacKey, CryptographicBuffer.ConvertStringToBinary(Message, BinaryStringEncoding.Utf8))

        Scenario3Text.Text &= "    Signature:  " & CryptographicBuffer.EncodeToHexString(signature) & vbCrLf

        ' Verify the signature.
        hmacKey = Algorithm.CreateKey(keymaterial)

        Dim IsAuthenticated As Boolean = Windows.Security.Cryptography.Core.CryptographicEngine.VerifySignature(hmacKey, CryptographicBuffer.ConvertStringToBinary(Message, BinaryStringEncoding.Utf8), signature)

        If Not IsAuthenticated Then
            Scenario3Text.Text &= "HashAlgorithmProvider failed to generate a hash of proper length!" & vbCrLf
            Exit Sub
        End If
    End Sub
End Class
