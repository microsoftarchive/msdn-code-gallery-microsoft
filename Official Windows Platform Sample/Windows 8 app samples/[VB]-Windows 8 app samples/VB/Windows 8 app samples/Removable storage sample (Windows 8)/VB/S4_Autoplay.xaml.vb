'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports SDKTemplate
Imports System
Imports System.Collections.Generic
Imports System.Threading.Tasks
Imports Windows.Devices.Portable
Imports Windows.Storage
Imports Windows.Storage.Search
Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class S4_Autoplay
    Inherits SDKTemplate.Common.LayoutAwarePage
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
        ' Enable the button only when launched from Autoplay or File Activation
        ScenarioInput.IsEnabled = (rootPage.AutoplayFileSystemDeviceFolder IsNot Nothing OrElse rootPage.AutoplayNonFileSystemDeviceId IsNot Nothing OrElse rootPage.FileActivationFiles IsNot Nothing)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Get Image' button.
    ''' If launched by Autoplay, this will find and display the first image file on the storage.
    ''' If launched by file activation, this will display the first image file from the activation file list.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub GetImage_Click(sender As Object, e As RoutedEventArgs)
        If rootPage.FileActivationFiles IsNot Nothing Then
            If rootPage.FileActivationFiles.Count > 0 Then
                ' Because this sample only supports image file types in its manifest,
                ' we can know that all files in the array of files will be image files.
                rootPage.NotifyUser("[File Activation] Displaying first image file ...", NotifyType.StatusMessage)
                Dim imageFile = TryCast(rootPage.FileActivationFiles(0), StorageFile)
                ' Pick the first file to display
                Await DisplayImageAsync(imageFile)
            Else
                rootPage.NotifyUser("[File Activation] File activation occurred but 0 files were received", NotifyType.ErrorMessage)
            End If
        Else
            If rootPage.AutoplayFileSystemDeviceFolder IsNot Nothing Then
                Await GetFirstImageFromStorageAsync(rootPage.AutoplayFileSystemDeviceFolder)
            Else
                Dim storage = StorageDevice.FromId(rootPage.AutoplayNonFileSystemDeviceId)
                Await GetFirstImageFromStorageAsync(storage)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Finds and displays the first image file on the storage.
    ''' </summary>
    ''' <param name="storage"></param>
    Private Async Function GetFirstImageFromStorageAsync(storage As StorageFolder) As Task
        Dim storageName = storage.Name

        ' Construct the query for image files
        Dim queryOptions = New QueryOptions(CommonFileQuery.OrderByName, New List(Of String)() From {".jpg", ".png", ".gif"})
        Dim imageFileQuery = storage.CreateFileQueryWithOptions(queryOptions)

        ' Run the query for image files
        rootPage.NotifyUser("[Launched by Autoplay] Looking for images on " & storageName & " ...", NotifyType.StatusMessage)
        Dim imageFiles = Await imageFileQuery.GetFilesAsync()
        If imageFiles.Count > 0 Then
            Dim imageFile = imageFiles(0)
            rootPage.NotifyUser("[Launched by Autoplay] Found " & imageFile.Name & " on " & storageName, NotifyType.StatusMessage)
            Await DisplayImageAsync(imageFile)
        Else
            rootPage.NotifyUser("[Launched by Autoplay] No images were found on " & storageName & ". You can use scenario 2 to transfer an image to it", NotifyType.StatusMessage)
        End If
    End Function

    ''' <summary>
    ''' Displays an image file in the 'ScenarioOutputImage' element.
    ''' </summary>
    ''' <param name="imageFile">The image file to display.</param>
    Private Async Function DisplayImageAsync(imageFile As StorageFile) As Task
        Dim imageProperties = Await imageFile.GetBasicPropertiesAsync()
        If imageProperties.Size > 0 Then
            rootPage.NotifyUser("Displaying: " & imageFile.Name & ", date modified: " & imageProperties.DateModified.ToString & ", size: " & imageProperties.Size & " bytes", NotifyType.StatusMessage)
            Dim stream = Await imageFile.OpenAsync(FileAccessMode.Read)

            ' BitmapImage.SetSource needs to be called in the UI thread
            Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                         Dim bitmap = New BitmapImage()
                                                                         bitmap.SetSource(stream)
                                                                         ScenarioOutputImage.SetValue(Image.SourceProperty, bitmap)
                                                                     End Sub)
        Else
            rootPage.NotifyUser("Cannot display " & imageFile.Name & " because its size is 0", NotifyType.ErrorMessage)
        End If
    End Function
End Class
