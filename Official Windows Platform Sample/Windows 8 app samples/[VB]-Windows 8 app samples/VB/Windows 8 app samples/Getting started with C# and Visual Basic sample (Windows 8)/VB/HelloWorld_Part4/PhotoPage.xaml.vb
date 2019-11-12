'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************


' The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

''' <summary>
''' A basic page that provides characteristics common to most applications.
''' </summary>
Public NotInheritable Class PhotoPage
    Inherits Common.LayoutAwarePage

    Private mruToken As String = Nothing

    ''' <summary>
    ''' Populates the page with content passed during navigation.  Any saved state is also
    ''' provided when recreating a page from a prior session.
    ''' </summary>
    ''' <param name="navigationParameter">The parameter value passed to
    ''' <see cref="Frame.Navigate"/> when this page was initially requested.
    ''' </param>
    ''' <param name="pageState">A dictionary of state preserved by this page during an earlier
    ''' session.  This will be null the first time a page is visited.</param>
    Protected Overrides Async Sub LoadState(navigationParameter As Object, pageState As Dictionary(Of String, Object))
        If pageState IsNot Nothing AndAlso pageState.ContainsKey("mruToken") Then
            Dim value As Object = Nothing
            If pageState.TryGetValue("mruToken", value) Then

                If (Not String.IsNullOrEmpty(value)) Then
                    mruToken = value.ToString()

                    ' Open the file via the token that you stored when adding this file into the MRU list.
                    Dim file =
                        Await Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.GetFileAsync(mruToken)

                    If (file IsNot Nothing) Then
                        ' Open a stream for the selected file.
                        Dim fileStream = Await file.OpenAsync(Windows.Storage.FileAccessMode.Read)

                        ' Set the image source to a bitmap.
                        Dim BitmapImage = New Windows.UI.Xaml.Media.Imaging.BitmapImage()

                        BitmapImage.SetSource(fileStream)
                        displayImage.Source = BitmapImage

                        ' Set the data context for the page.
                        Me.DataContext = file
                    End If
                End If
            End If
        End If
    End Sub

    ''' <summary>
    ''' Preserves state associated with this page in case the application is suspended or the
    ''' page is discarded from the navigation cache.  Values must conform to the serialization
    ''' requirements of <see cref="Common.SuspensionManager.SessionState"/>.
    ''' </summary>
    ''' <param name="pageState">An empty dictionary to be populated with serializable state.</param>
    Protected Overrides Sub SaveState(pageState As Dictionary(Of String, Object))
        If Not String.IsNullOrEmpty(mruToken) Then
            pageState("mruToken") = mruToken
        End If
    End Sub

    Private Async Sub GetPhotoButton_Click(sender As Object, e As RoutedEventArgs)
        ' File picker APIs don't work if the app is in a snapped state.
        ' If the app is snapped, try to unsnap it first. Only show the picker if it unsnaps.
        If Windows.UI.ViewManagement.ApplicationView.Value <> ApplicationViewState.Snapped OrElse
             Windows.UI.ViewManagement.ApplicationView.TryUnsnap() = True Then

            Dim openPicker = New Windows.Storage.Pickers.FileOpenPicker()
            openPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary
            openPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail

            ' Filter to include a sample subset of file types.
            openPicker.FileTypeFilter.Clear()
            openPicker.FileTypeFilter.Add(".bmp")
            openPicker.FileTypeFilter.Add(".png")
            openPicker.FileTypeFilter.Add(".jpeg")
            openPicker.FileTypeFilter.Add(".jpg")

            ' Open the file picker.
            Dim file = Await openPicker.PickSingleFileAsync()

            ' file is null if user cancels the file picker.
            If file IsNot Nothing Then
                ' Open a stream for the selected file.
                Dim fileStream = Await file.OpenAsync(Windows.Storage.FileAccessMode.Read)

                ' Set the image source to the selected bitmap.
                Dim BitmapImage = New Windows.UI.Xaml.Media.Imaging.BitmapImage()
                BitmapImage.SetSource(fileStream)
                displayImage.Source = BitmapImage
                Me.DataContext = file

                ' Add picked file to MostRecentlyUsedList.
                mruToken = Windows.Storage.AccessCache.StorageApplicationPermissions.MostRecentlyUsedList.Add(file)
            End If
        End If
    End Sub
End Class
