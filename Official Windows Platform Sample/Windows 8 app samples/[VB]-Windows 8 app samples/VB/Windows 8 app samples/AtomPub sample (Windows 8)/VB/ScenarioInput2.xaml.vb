' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.Threading.Tasks
Imports Windows.Security.Credentials
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.Web
Imports Windows.Web.AtomPub
Imports Windows.Web.Syndication
Imports SDKTemplate

Partial Public NotInheritable Class ScenarioInput2
    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    ' Controls from the output frame.
    Private outputField As TextBox
    Private titleField As TextBox
    Private contentField As TextBox

    Public Sub New()
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, Global.SDKTemplate.MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded

        CommonData.Restore(Me)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

    Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
        CommonData.Save(Me)
    End Sub

    Private Sub rootPage_OutputFrameLoaded(sender As Object, e As Object)
        ' At this point, we know that the Output Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Output Frame.

        ' Get a pointer to the content within the OutputFrame
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)

        outputField = TryCast(outputFrame.FindName("OutputField"), TextBox)
        titleField = TryCast(outputFrame.FindName("TitleField"), TextBox)
        contentField = TryCast(outputFrame.FindName("ContentField"), TextBox)
    End Sub

    ' Submit an item.
    Private Async Sub SubmitItem_Click(sender As Object, e As RoutedEventArgs)
        Try
            If String.IsNullOrWhiteSpace(titleField.Text) Then
                outputField.Text = "Post title cannot be blank" & vbCrLf
                Exit Sub
            End If
            Dim serviceUri As New Uri(ServiceAddressField.Text.Trim & CommonData.ServiceDocUri)

            outputField.Text = "Fetching Service document: " & serviceUri.ToString & vbCr & vbCrLf

            ' The result here is usually the same as:
            ' Dim resourceUri As New Uri(ServiceAddressField.Text.Trim & Common.EditUri)
            Dim resourceUri As Uri = Await FindEditUri(serviceUri)

            outputField.Text &= "Uploading Post: " & resourceUri.ToString & vbCr & vbCrLf

            Dim item As New SyndicationItem
            item.Title = New SyndicationText(titleField.Text, SyndicationTextType.Text)
            item.Content = New SyndicationContent(contentField.Text, SyndicationTextType.Html)

            Dim result As SyndicationItem = Await CommonData.GetClient().CreateResourceAsync(resourceUri, item.Title.Text, item)

            outputField.Text &= "Posted at " & result.ItemUri.ToString & vbCr & vbCrLf
            outputField.Text &= "Complete" & vbCr & vbCrLf
        Catch ex As Exception
            Dim status = WebError.GetStatus(ex.HResult)
            If status = WebErrorStatus.Unauthorized Then
                outputField.Text &= "Wrong user name or password!" & vbCr & vbCrLf
            End If
            outputField.Text &= ex.ToString & vbCr & vbCrLf
        End Try
    End Sub

    ' Read the service document to find the URI we're suposed to use when uploading content.
    Private Async Function FindEditUri(serviceUri As Uri) As Task(Of Uri)
        Dim serviceDocument As ServiceDocument = Await CommonData.GetClient.RetrieveServiceDocumentAsync(serviceUri)

        For Each workspace In serviceDocument.Workspaces
            For Each collection In workspace.Collections
                If String.Join(";", collection.Accepts) = "application/atom+xml;type=entry" Then
                    Return collection.Uri
                End If
            Next
        Next

        Throw New ArgumentException("Edit Uri not found in service document")
    End Function

    Private Sub UserNameField_TextChanged(sender As Object, e As TextChangedEventArgs)
        CommonData.Save(Me)
    End Sub

    Private Sub PasswordField_PasswordChanged(sender As Object, e As RoutedEventArgs)
        CommonData.Save(Me)
    End Sub
End Class
