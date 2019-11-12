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
Imports Windows.Storage.Streams
Imports Windows.Security.Cryptography

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
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
    ''' This is the click handler for the 'RunSample' button.  It is responsible for executing the sample code.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub RunSample_Click(sender As Object, e As RoutedEventArgs)
        Dim buffer As IBuffer
        Scenario1Text.Text = ""

        ' Initialize example data.
        Dim ByteArray As Byte() = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}
        Dim base64String As String = "uiwyeroiugfyqcajkds897945234=="
        Dim hexString As String = "30313233"
        Dim inputString As String = "Input string"

        ' Generate random bytes.
        buffer = CryptographicBuffer.GenerateRandom(32)
        Scenario1Text.Text &= "GenerateRandom" & vbCrLf
        Scenario1Text.Text &= "  Buffer: " & CryptographicBuffer.EncodeToHexString(buffer) & vbCrLf & vbCrLf

        ' Convert from a byte array.
        buffer = CryptographicBuffer.CreateFromByteArray(ByteArray)
        Scenario1Text.Text &= "CreateFromByteArray" & vbCrLf
        Scenario1Text.Text &= "  Buffer: " & CryptographicBuffer.EncodeToHexString(buffer) & vbCrLf & vbCrLf

        ' Decode a Base64 encoded string to binary.
        buffer = CryptographicBuffer.DecodeFromBase64String(base64String)
        Scenario1Text.Text &= "DecodeFromBase64String" & vbCrLf
        Scenario1Text.Text &= "  Base64 String: " & base64String & vbCrLf
        Scenario1Text.Text &= "  Buffer:        " & CryptographicBuffer.EncodeToHexString(buffer) & vbCrLf & vbCrLf

        ' Decode a hexadecimal string to binary.
        buffer = CryptographicBuffer.DecodeFromHexString(hexString)
        Scenario1Text.Text &= "DecodeFromHexString" & vbCrLf
        Scenario1Text.Text &= "  Hex String: " & hexString & vbCrLf
        Scenario1Text.Text &= "  Buffer:     " & CryptographicBuffer.EncodeToHexString(buffer) & vbCrLf & vbCrLf

        ' Convert a string to UTF16BE binary data.
        buffer = CryptographicBuffer.ConvertStringToBinary(inputString, BinaryStringEncoding.Utf16BE)
        Scenario1Text.Text &= "ConvertStringToBinary (Utf16BE)" & vbCrLf
        Scenario1Text.Text &= "  String: " & inputString & vbCrLf
        Scenario1Text.Text &= "  Buffer: " & CryptographicBuffer.EncodeToHexString(buffer) & vbCrLf & vbCrLf

        ' Convert a string to UTF16LE binary data.
        buffer = CryptographicBuffer.ConvertStringToBinary(inputString, BinaryStringEncoding.Utf16LE)
        Scenario1Text.Text &= "ConvertStringToBinary (Utf16LE)" & vbCrLf
        Scenario1Text.Text &= "  String: " & inputString & vbCrLf
        Scenario1Text.Text &= "  Buffer: " & CryptographicBuffer.EncodeToHexString(buffer) & vbCrLf & vbCrLf

        ' Convert a string to UTF8 binary data.
        buffer = CryptographicBuffer.ConvertStringToBinary(inputString, BinaryStringEncoding.Utf8)
        Scenario1Text.Text &= "ConvertStringToBinary (Utf8)" & vbCrLf
        Scenario1Text.Text &= "  String: " & inputString & vbCrLf
        Scenario1Text.Text &= "  Buffer: " & CryptographicBuffer.EncodeToHexString(buffer) & vbCrLf & vbCrLf

        ' Decode from a Base64 encoded string.
        buffer = CryptographicBuffer.DecodeFromBase64String(base64String)
        Scenario1Text.Text &= "DecodeFromBase64String " & vbCrLf
        Scenario1Text.Text &= "  String: " & base64String & vbCrLf
        Scenario1Text.Text &= "  Buffer (Hex): " & CryptographicBuffer.EncodeToHexString(buffer) & vbCrLf & vbCrLf
        Scenario1Text.Text &= "  Buffer (Base64): " & CryptographicBuffer.EncodeToBase64String(buffer) & vbCrLf & vbCrLf

        ' Decode from a hexadecimal encoded string.
        buffer = CryptographicBuffer.DecodeFromHexString(hexString)
        Scenario1Text.Text &= "DecodeFromHexString " & vbCrLf
        Scenario1Text.Text &= "  String: " & hexString & vbCrLf
        Scenario1Text.Text &= "  Buffer: " & CryptographicBuffer.EncodeToHexString(buffer) & vbCrLf & vbCrLf
    End Sub
End Class
