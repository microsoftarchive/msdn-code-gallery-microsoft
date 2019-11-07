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
Imports Windows.Security.Cryptography
Imports Windows.Storage.Streams

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
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
        Scenario2Text.Text = ""
        Dim algName As String = AlgorithmNames.SelectionBoxItem.ToString

        ' Create a HashAlgorithmProvider object.
        Dim Algorithm As HashAlgorithmProvider = HashAlgorithmProvider.OpenAlgorithm(algName)
        Dim vector As IBuffer = CryptographicBuffer.DecodeFromBase64String("uiwyeroiugfyqcajkds897945234==")

        Scenario2Text.Text &= vbCrLf & "*** Sample Hash Algorithm: " & Algorithm.AlgorithmName & vbCrLf
        Scenario2Text.Text &= "    Initial vector:  uiwyeroiugfyqcajkds897945234==" & vbCrLf

        ' Compute the hash in one call.
        Dim digest As IBuffer = Algorithm.HashData(vector)

        If digest.Length <> Algorithm.HashLength Then
            Scenario2Text.Text &= "HashAlgorithmProvider failed to generate a hash of proper length!" & vbCrLf
            Exit Sub
        End If

        Scenario2Text.Text &= "    Hash:  " & CryptographicBuffer.EncodeToHexString(digest) & vbCrLf

        ' Use a reusable hash object to hash the data by using multiple calls.
        Dim reusableHash As CryptographicHash = Algorithm.CreateHash

        reusableHash.Append(vector)

        ' Note that calling GetValue resets the data that has been appended to the
        ' CryptographicHash object.
        Dim digest2 As IBuffer = reusableHash.GetValueAndReset

        If Not CryptographicBuffer.Compare(digest, digest2) Then
            Scenario2Text.Text &= "CryptographicHash failed to generate the same hash data!" & vbCrLf
            Exit Sub
        End If

        reusableHash.Append(vector)
        digest2 = reusableHash.GetValueAndReset

        If Not CryptographicBuffer.Compare(digest, digest2) Then
            Scenario2Text.Text &= "Reusable CryptographicHash failed to generate the same hash data!" & vbCrLf
            Exit Sub
        End If
    End Sub
End Class
