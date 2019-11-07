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
Partial Public NotInheritable Class Scenario4
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
        Dim Secret As IBuffer = CryptographicBuffer.ConvertStringToBinary("Master key to derive from", BinaryStringEncoding.Utf8)
        Dim TargetSize As UInt32 = UInt32.Parse(KeySizes.SelectionBoxItem.ToString)
        Scenario4Text.Text = ""
        Dim Params As KeyDerivationParameters

        If algName.Contains("PBKDF2") Then
            ' Password based key derivation function (PBKDF2).
            ' Salt
            ' PBKDF2 Iteration Count
            Params = KeyDerivationParameters.BuildForPbkdf2(CryptographicBuffer.GenerateRandom(16), 10000)
        ElseIf algName.Contains("SP800_108") Then
            ' SP800_108_CTR_HMAC key derivation function.
            ' Label
            ' Context
            Params = KeyDerivationParameters.BuildForSP800108(CryptographicBuffer.ConvertStringToBinary("Label", BinaryStringEncoding.Utf8), CryptographicBuffer.DecodeFromHexString("303132333435363738"))
        ElseIf algName.Contains("SP800_56A") Then
            Params = KeyDerivationParameters.BuildForSP80056a(CryptographicBuffer.ConvertStringToBinary("AlgorithmId", BinaryStringEncoding.Utf8), CryptographicBuffer.ConvertStringToBinary("VParty", BinaryStringEncoding.Utf8), CryptographicBuffer.ConvertStringToBinary("UParty", BinaryStringEncoding.Utf8), CryptographicBuffer.ConvertStringToBinary("SubPubInfo", BinaryStringEncoding.Utf8), CryptographicBuffer.ConvertStringToBinary("SubPrivInfo", BinaryStringEncoding.Utf8))
        Else
            Scenario4Text.Text &= "    An invalid algorithm was specified." & vbCrLf
            Exit Sub
        End If

        ' Create a KeyDerivationAlgorithmProvider object for the algorithm specified on input.
        Dim Algorithm As KeyDerivationAlgorithmProvider = KeyDerivationAlgorithmProvider.OpenAlgorithm(algName)

        Scenario4Text.Text &= "*** Sample Kdf Algorithm: " & Algorithm.AlgorithmName & vbCrLf
        Scenario4Text.Text &= "    Secrect Size: " & Secret.Length & vbCrLf
        Scenario4Text.Text &= "    Target Size: " & TargetSize & vbCrLf

        ' Create a key.
        Dim key As CryptographicKey = Algorithm.CreateKey(Secret)

        ' Derive a key from the created key.
        Dim derived As IBuffer = CryptographicEngine.DeriveKeyMaterial(key, Params, TargetSize)
        Scenario4Text.Text &= "    Derived  " & derived.Length & " bytes" & vbCrLf
        Scenario4Text.Text &= "    Derived: " & CryptographicBuffer.EncodeToHexString(derived) & vbCrLf
    End Sub
End Class
