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
Imports SDKTemplate
Imports Windows.Security.Credentials

Partial Public NotInheritable Class ScenarioInput4
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
        Dim inputFrame As Page = DirectCast(rootPage.InputFrame.Content, Page)
        Dim DeleteSummary As TextBox = TryCast(inputFrame.FindName("DeleteSummary"), TextBox)
        DeleteSummary.Text = Trace + vbCr & vbLf
    End Sub


    Private Sub Scenario4Button_Click(sender As Object, e As RoutedEventArgs)
        Try
            Dim v As New Windows.Security.Credentials.PasswordVault()
            Dim creds As IReadOnlyList(Of PasswordCredential) = v.RetrieveAll()
            DeleteSummary.Text = "Number of credentials deleted: " & creds.Count
            For Each c As PasswordCredential In creds
                v.Remove(c)
            Next
            ' GetAll is a snapshot in time, so to reflect the updated vault, get all credentials again
            ' The credentials should now be empty

            creds = v.RetrieveAll()
        Catch ErrorMessage As Exception
            DebugPrint(ErrorMessage.ToString)
        End Try
    End Sub
End Class
