'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports System.Text
Imports Windows.ApplicationModel
Imports Windows.ApplicationModel.DataTransfer
Imports Windows.ApplicationModel.DataTransfer.ShareTarget
Imports Windows.Data.Json
Imports Windows.Storage
Imports Windows.Storage.Streams
Imports Windows.UI.Text
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Documents
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Media.Imaging

Partial Public NotInheritable Class MainPage
    Inherits Page

    Private shareOperation As ShareOperation
    Private sharedDataTitle As String
    Private sharedDataDescription As String
    Private sharedDataPackageFamilyName As String
    Private sharedDataContentSourceWebLink As Uri
    Private sharedDataContentSourceApplicationLink As Uri
    Private sharedDataLogoBackgroundColor As Color
    Private sharedDataSquare30x30Logo As IRandomAccessStreamReference
    Private shareQuickLinkId As String
    Private sharedText As String
    Private sharedWebLink As Uri
    Private sharedApplicationLink As Uri
    Private sharedStorageItems As IReadOnlyList(Of IStorageItem)
    Private sharedCustomData As String
    Private sharedHtmlFormat As String
    Private sharedResourceMap As IReadOnlyDictionary(Of String, RandomAccessStreamReference)
    Private sharedBitmapStreamRef As IRandomAccessStreamReference
    Private sharedThumbnailStreamRef As IRandomAccessStreamReference
    Private Const dataFormatName As String = "http://schema.org/Book"

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Protected Overrides Async Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' It is recommended to only retrieve the ShareOperation object in the activation handler, return as
        ' quickly as possible, and retrieve all data from the share target asynchronously.

        Me.shareOperation = CType(e.Parameter, ShareOperation)

        Await Task.Factory.StartNew(Async Sub()
                                        ' Retrieve the data package properties.
                                        ' Retrieve the data package content.
                                        ' The GetWebLinkAsync(), GetTextAsync(), GetStorageItemsAsync(), etc. APIs will throw if there was an error retrieving the data from the source app.
                                        ' In this sample, we just display the error. It is recommended that a share target app handles these in a way appropriate for that particular app.
                                        ' In this sample, we just display the shared data content.
                                        ' Get back to the UI thread using the dispatcher.
                                        ' Display the name of the files being shared.
                                        ' This is an area to be especially careful parsing data from the source app to avoid buffer overruns.
                                        ' This sample doesn't perform data validation but will catch any exceptions thrown.
                                        ' This sample expects the custom format to be of type http://schema.org/Book
                                        ' Check if there are any local images in the resource map.
                                        Me.sharedDataTitle = Me.shareOperation.Data.Properties.Title
                                        Me.sharedDataDescription = Me.shareOperation.Data.Properties.Description
                                        Me.sharedDataPackageFamilyName = Me.shareOperation.Data.Properties.PackageFamilyName
                                        Me.sharedDataContentSourceWebLink = Me.shareOperation.Data.Properties.ContentSourceWebLink
                                        Me.sharedDataContentSourceApplicationLink = Me.shareOperation.Data.Properties.ContentSourceApplicationLink
                                        Me.sharedDataLogoBackgroundColor = Me.shareOperation.Data.Properties.LogoBackgroundColor
                                        Me.sharedDataSquare30x30Logo = Me.shareOperation.Data.Properties.Square30x30Logo
                                        Me.sharedThumbnailStreamRef = Me.shareOperation.Data.Properties.Thumbnail
                                        Me.shareQuickLinkId = Me.shareOperation.QuickLinkId
                                        If Me.shareOperation.Data.Contains(StandardDataFormats.WebLink) Then
                                            Try
                                                Me.sharedWebLink = Await Me.shareOperation.Data.GetWebLinkAsync()
                                            Catch ex As Exception
                                                NotifyUserBackgroundThread("Failed GetWebLinkAsync - " & ex.Message, NotifyType.ErrorMessage)
                                            End Try
                                        End If
                                        If Me.shareOperation.Data.Contains(StandardDataFormats.ApplicationLink) Then
                                            Try
                                                Me.sharedApplicationLink = Await Me.shareOperation.Data.GetApplicationLinkAsync()
                                            Catch ex As Exception
                                                NotifyUserBackgroundThread("Failed GetApplicationLinkAsync - " & ex.Message, NotifyType.ErrorMessage)
                                            End Try
                                        End If
                                        If Me.shareOperation.Data.Contains(StandardDataFormats.Text) Then
                                            Try
                                                Me.sharedText = Await Me.shareOperation.Data.GetTextAsync()
                                            Catch ex As Exception
                                                NotifyUserBackgroundThread("Failed GetTextAsync - " & ex.Message, NotifyType.ErrorMessage)
                                            End Try
                                        End If
                                        If Me.shareOperation.Data.Contains(StandardDataFormats.StorageItems) Then
                                            Try
                                                Me.sharedStorageItems = Await Me.shareOperation.Data.GetStorageItemsAsync()
                                            Catch ex As Exception
                                                NotifyUserBackgroundThread("Failed GetStorageItemsAsync - " & ex.Message, NotifyType.ErrorMessage)
                                            End Try
                                        End If
                                        If Me.shareOperation.Data.Contains(dataFormatName) Then
                                            Try
                                                Me.sharedCustomData = Await Me.shareOperation.Data.GetTextAsync(dataFormatName)
                                            Catch ex As Exception
                                                NotifyUserBackgroundThread("Failed GetTextAsync(" & dataFormatName & ") - " & ex.Message, NotifyType.ErrorMessage)
                                            End Try
                                        End If
                                        If Me.shareOperation.Data.Contains(StandardDataFormats.Html) Then
                                            Try
                                                Me.sharedHtmlFormat = Await Me.shareOperation.Data.GetHtmlFormatAsync()
                                            Catch ex As Exception
                                                NotifyUserBackgroundThread("Failed GetHtmlFormatAsync - " & ex.Message, NotifyType.ErrorMessage)
                                            End Try
                                            Try
                                                Me.sharedResourceMap = Await Me.shareOperation.Data.GetResourceMapAsync()
                                            Catch ex As Exception
                                                NotifyUserBackgroundThread("Failed GetResourceMapAsync - " & ex.Message, NotifyType.ErrorMessage)
                                            End Try
                                        End If
                                        If Me.shareOperation.Data.Contains(StandardDataFormats.Bitmap) Then
                                            Try
                                                Me.sharedBitmapStreamRef = Await Me.shareOperation.Data.GetBitmapAsync()
                                            Catch ex As Exception
                                                NotifyUserBackgroundThread("Failed GetBitmapAsync - " & ex.Message, NotifyType.ErrorMessage)
                                            End Try
                                        End If
                                        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Async Sub()
                                                                                                     DataPackageTitle.Text = Me.sharedDataTitle
                                                                                                     DataPackageDescription.Text = Me.sharedDataDescription
                                                                                                     DataPackagePackageFamilyName.Text = Me.sharedDataPackageFamilyName
                                                                                                     If Me.sharedDataContentSourceWebLink IsNot Nothing Then
                                                                                                         DataPackageContentSourceWebLink.Text = Me.sharedDataContentSourceWebLink.AbsoluteUri
                                                                                                     End If
                                                                                                     If Me.sharedDataContentSourceApplicationLink IsNot Nothing Then
                                                                                                         DataPackageContentSourceApplicationLink.Text = Me.sharedDataContentSourceApplicationLink.AbsoluteUri
                                                                                                     End If
                                                                                                     If Me.sharedDataSquare30x30Logo IsNot Nothing Then
                                                                                                         Dim logoStream As IRandomAccessStreamWithContentType = Await Me.sharedDataSquare30x30Logo.OpenReadAsync()
                                                                                                         Dim bitmapImage As New BitmapImage()
                                                                                                         bitmapImage.SetSource(logoStream)
                                                                                                         LogoHolder.Source = bitmapImage
                                                                                                         LogoBackground.Background = New SolidColorBrush(Me.sharedDataLogoBackgroundColor)
                                                                                                         LogoArea.Visibility = Visibility.Visible
                                                                                                     End If
                                                                                                     If Not String.IsNullOrEmpty(Me.shareOperation.QuickLinkId) Then
                                                                                                         SelectedQuickLinkId.Text = Me.shareQuickLinkId
                                                                                                     End If
                                                                                                     If Me.sharedThumbnailStreamRef IsNot Nothing Then
                                                                                                         Dim thumbnailStream As IRandomAccessStreamWithContentType = Await Me.sharedThumbnailStreamRef.OpenReadAsync()
                                                                                                         Dim bitmapImage As New BitmapImage()
                                                                                                         bitmapImage.SetSource(thumbnailStream)
                                                                                                         ThumbnailHolder.Source = bitmapImage
                                                                                                         ThumbnailArea.Visibility = Visibility.Visible
                                                                                                     End If
                                                                                                     If Me.sharedWebLink IsNot Nothing Then
                                                                                                         AddContentValue("WebLink: ", Me.sharedWebLink.AbsoluteUri)
                                                                                                     End If
                                                                                                     If Me.sharedApplicationLink IsNot Nothing Then
                                                                                                         AddContentValue("ApplicationLink: ", Me.sharedApplicationLink.AbsoluteUri)
                                                                                                     End If
                                                                                                     If Me.sharedText IsNot Nothing Then
                                                                                                         AddContentValue("Text: ", Me.sharedText)
                                                                                                     End If
                                                                                                     If Me.sharedStorageItems IsNot Nothing Then
                                                                                                         Dim fileNames As New StringBuilder()
                                                                                                         For index As Integer = 0 To Me.sharedStorageItems.Count - 1
                                                                                                             fileNames.Append(Me.sharedStorageItems(index).Name)
                                                                                                             If index < Me.sharedStorageItems.Count - 1 Then
                                                                                                                 fileNames.Append(", ")
                                                                                                             End If
                                                                                                         Next index
                                                                                                         fileNames.Append(".")
                                                                                                         AddContentValue("StorageItems: ", fileNames.ToString())
                                                                                                     End If
                                                                                                     If Me.sharedCustomData IsNot Nothing Then
                                                                                                         Try
                                                                                                             Dim receivedStrings As New StringBuilder()
                                                                                                             Dim customObject As JsonObject = JsonObject.Parse(Me.sharedCustomData)
                                                                                                             If customObject.ContainsKey("type") Then
                                                                                                                 If customObject("type").GetString() = "http://schema.org/Book" Then
                                                                                                                     receivedStrings.AppendLine("Type: " & customObject("type").Stringify())
                                                                                                                     Dim properties As JsonObject = customObject("properties").GetObject()
                                                                                                                     receivedStrings.AppendLine("Image: " & properties("image").Stringify())
                                                                                                                     receivedStrings.AppendLine("Name: " & properties("name").Stringify())
                                                                                                                     receivedStrings.AppendLine("Book Format: " & properties("bookFormat").Stringify())
                                                                                                                     receivedStrings.AppendLine("Author: " & properties("author").Stringify())
                                                                                                                     receivedStrings.AppendLine("Number of Pages: " & properties("numberOfPages").Stringify())
                                                                                                                     receivedStrings.AppendLine("Publisher: " & properties("publisher").Stringify())
                                                                                                                     receivedStrings.AppendLine("Date Published: " & properties("datePublished").Stringify())
                                                                                                                     receivedStrings.AppendLine("In Language: " & properties("inLanguage").Stringify())
                                                                                                                     receivedStrings.Append("ISBN: " & properties("isbn").Stringify())
                                                                                                                     AddContentValue("Custom format data:" & Environment.NewLine, receivedStrings.ToString())
                                                                                                                 Else
                                                                                                                     NotifyUser("The custom format from the source app is not of type http://schema.org/Book", NotifyType.ErrorMessage)
                                                                                                                 End If
                                                                                                             Else
                                                                                                                 NotifyUser("The custom format from the source app doesn't contain a type", NotifyType.ErrorMessage)
                                                                                                             End If
                                                                                                         Catch ex As Exception
                                                                                                             NotifyUser("Failed to parse the custom data - " & ex.Message, NotifyType.ErrorMessage)
                                                                                                         End Try
                                                                                                     End If
                                                                                                     If Me.sharedHtmlFormat IsNot Nothing Then
                                                                                                         Dim htmlFragment As String = HtmlFormatHelper.GetStaticFragment(Me.sharedHtmlFormat)
                                                                                                         If Not String.IsNullOrEmpty(htmlFragment) Then
                                                                                                             AddContentValue("HTML: ")
                                                                                                             ShareWebView.Visibility = Visibility.Visible
                                                                                                             ShareWebView.NavigateToString("<html><body>" & htmlFragment & "</body></html>")
                                                                                                         Else
                                                                                                             NotifyUser("GetStaticFragment failed to parse the HTML from the source app", NotifyType.ErrorMessage)
                                                                                                         End If
                                                                                                         If Me.sharedResourceMap.Count > 0 Then
                                                                                                             ResourceMapValue.Text = ""
                                                                                                             For Each item As KeyValuePair(Of String, RandomAccessStreamReference) In Me.sharedResourceMap
                                                                                                                 ResourceMapValue.Text += vbLf & "Key: " & item.Key
                                                                                                             Next item
                                                                                                             ResourceMapArea.Visibility = Visibility.Visible
                                                                                                         End If
                                                                                                     End If
                                                                                                     If Me.sharedBitmapStreamRef IsNot Nothing Then
                                                                                                         Dim bitmapStream As IRandomAccessStreamWithContentType = Await Me.sharedBitmapStreamRef.OpenReadAsync()
                                                                                                         Dim bitmapImage As New BitmapImage()
                                                                                                         bitmapImage.SetSource(bitmapStream)
                                                                                                         ImageHolder.Source = bitmapImage
                                                                                                         ImageArea.Visibility = Visibility.Visible
                                                                                                     End If
                                                                                                 End Sub)
                                    End Sub)
    End Sub

    Private Sub QuickLinkSectionLabel_Tapped(ByVal sender As Object, ByVal e As TappedRoutedEventArgs)
        ' Trigger the appropriate Checked/Unchecked event if the user taps on the text instead of the checkbox.
        AddQuickLink.IsChecked = Not AddQuickLink.IsChecked
    End Sub

    Private Sub AddQuickLink_Checked(ByVal sender As Object, ByVal e As RoutedEventArgs)
        QuickLinkCustomization.Visibility = Visibility.Visible
    End Sub

    Private Sub AddQuickLink_Unchecked(ByVal sender As Object, ByVal e As RoutedEventArgs)
        QuickLinkCustomization.Visibility = Visibility.Collapsed
    End Sub

    Private Async Sub ReportCompleted_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        If AddQuickLink.IsChecked.Equals(True) Then
            Dim quickLinkInfo As QuickLink = New QuickLink With {.Id = QuickLinkId.Text, .Title = QuickLinkTitle.Text}

            quickLinkInfo.SupportedFileTypes.Add("*")

            quickLinkInfo.SupportedDataFormats.Add(StandardDataFormats.Text)
            quickLinkInfo.SupportedDataFormats.Add(StandardDataFormats.WebLink)
            quickLinkInfo.SupportedDataFormats.Add(StandardDataFormats.ApplicationLink)
            quickLinkInfo.SupportedDataFormats.Add(StandardDataFormats.Bitmap)
            quickLinkInfo.SupportedDataFormats.Add(StandardDataFormats.StorageItems)
            quickLinkInfo.SupportedDataFormats.Add(StandardDataFormats.Html)
            quickLinkInfo.SupportedDataFormats.Add(dataFormatName)

            Try
                Dim iconFile As StorageFile = Await Package.Current.InstalledLocation.CreateFileAsync("assets\user.png", CreationCollisionOption.OpenIfExists)
                quickLinkInfo.Thumbnail = RandomAccessStreamReference.CreateFromFile(iconFile)
                Me.shareOperation.ReportCompleted(quickLinkInfo)
            Catch e1 As Exception
                ' Even if the QuickLink cannot be created it is important to call ReportCompleted. Otherwise, if this is a long-running share,
                ' the app will stick around in the long-running share progress list.
                Me.shareOperation.ReportCompleted()
                Throw
            End Try
        Else
            Me.shareOperation.ReportCompleted()
        End If
    End Sub

    Private Sub DismissUI_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Me.shareOperation.DismissUI()
    End Sub

    Private Sub LongRunningShareLabel_Tapped(ByVal sender As Object, ByVal e As TappedRoutedEventArgs)
        ' Trigger the appropriate Checked/Unchecked event if the user taps on the text instead of the checkbox.
        ExpandLongRunningShareSection.IsChecked = Not ExpandLongRunningShareSection.IsChecked
    End Sub

    Private Sub ExpandLongRunningShareSection_Checked(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ExtendedSharingArea.Visibility = Visibility.Visible
    End Sub

    Private Sub ExpandLongRunningShareSection_Unchecked(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ExtendedSharingArea.Visibility = Visibility.Collapsed
    End Sub

    Private Sub ReportStarted_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Me.shareOperation.ReportStarted()
        Me.NotifyUser("Started...", NotifyType.StatusMessage)
    End Sub

    Private Sub ReportDataRetrieved_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Me.shareOperation.ReportDataRetrieved()
        Me.NotifyUser("Data retrieved...", NotifyType.StatusMessage)
    End Sub

    Private Sub ReportSubmittedBackgroundTask_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Me.shareOperation.ReportSubmittedBackgroundTask()
        Me.NotifyUser("Submitted background task...", NotifyType.StatusMessage)
    End Sub

    Private Sub ReportErrorButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Me.shareOperation.ReportError(ReportError.Text)
    End Sub

    Private Async Sub Footer_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        Await Windows.System.Launcher.LaunchUriAsync(New Uri(CType(sender, HyperlinkButton).Tag.ToString()))
    End Sub

    Private Sub NotifyUser(ByVal strMessage As String, ByVal type As NotifyType)
        Select Case type
            ' Use the status message style.
            Case NotifyType.StatusMessage
                StatusBlock.Style = TryCast(Resources("StatusStyle"), Style)
                ' Use the error message style.
            Case NotifyType.ErrorMessage
                StatusBlock.Style = TryCast(Resources("ErrorStyle"), Style)
        End Select
        StatusBlock.Text = strMessage
    End Sub

    Private Async Sub NotifyUserBackgroundThread(ByVal message As String, ByVal type As NotifyType)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub() NotifyUser(message, type))
    End Sub

    Private Sub AddContentValue(ByVal title As String, Optional ByVal description As String = Nothing)
        Dim contentType As New Run()
        contentType.FontWeight = FontWeights.Bold
        contentType.Text = title
        ContentValue.Inlines.Add(contentType)

        If description IsNot Nothing Then
            Dim _contentValue As New Run()
            _contentValue.Text = description & Environment.NewLine
            ContentValue.Inlines.Add(_contentValue)
        End If
    End Sub
End Class

Public Enum NotifyType
    StatusMessage
    ErrorMessage
End Enum
