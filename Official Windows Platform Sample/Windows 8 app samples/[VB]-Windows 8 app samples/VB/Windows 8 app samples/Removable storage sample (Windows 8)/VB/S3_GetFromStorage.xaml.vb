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
Imports Windows.Devices.Enumeration
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
Partial Public NotInheritable Class S3_GetFromStorage
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    ' Contains the device information used for populating the device selection list
    Private _deviceInfoCollection As DeviceInformationCollection = Nothing

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        DeviceSelector.Visibility = Visibility.Collapsed
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Get Image' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub GetImage_Click(sender As Object, e As RoutedEventArgs)
        Await ShowDeviceSelectorAsync()
    End Sub

    ''' <summary>
    ''' Enumerates all storages and populates the device selection list.
    ''' </summary>
    Private Async Function ShowDeviceSelectorAsync() As Task
        _deviceInfoCollection = Nothing

        ' Find all storage devices using Windows.Devices.Enumeration
        _deviceInfoCollection = Await DeviceInformation.FindAllAsync(StorageDevice.GetDeviceSelector())
        If _deviceInfoCollection.Count > 0 Then
            Dim items = New List(Of Object)()
            For Each deviceInfo As DeviceInformation In _deviceInfoCollection
                items.Add(New With { _
                    .Name = deviceInfo.Name _
                })
            Next
            DeviceList.ItemsSource = items
            DeviceSelector.Visibility = Visibility.Visible
        Else
            rootPage.NotifyUser("No removable storages were found. Please attach a removable storage to the system (e.g. a camera or camera memory)", NotifyType.StatusMessage)
        End If
    End Function

    ''' <summary>
    ''' This is the tapped handler for the device selection list. It runs the scenario
    ''' for the selected storage.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub DeviceList_Tapped(sender As Object, e As Windows.UI.Xaml.Input.TappedRoutedEventArgs)
        DeviceSelector.Visibility = Visibility.Collapsed
        If _deviceInfoCollection Is Nothing Then
            ' not yet populated
            Return
        End If

        Dim deviceInfo = _deviceInfoCollection(DeviceList.SelectedIndex)
        Await GetFirstImageFromStorageAsync(deviceInfo)
    End Sub

    ''' <summary>
    ''' Finds and displays the first image file on the storage referenced by the device information element.
    ''' </summary>
    ''' <param name="deviceInfoElement">Contains information about a selected device.</param>
    Private Async Function GetFirstImageFromStorageAsync(deviceInfoElement As DeviceInformation) As Task
        ' Convert the selected device information element to a StorageFolder
        Dim storage = StorageDevice.FromId(deviceInfoElement.Id)
        Dim storageName = deviceInfoElement.Name

        ' Construct the query for image files
        Dim queryOptions = New QueryOptions(CommonFileQuery.OrderByName, New List(Of String)() From {".jpg", ".png", ".gif"})
        Dim imageFileQuery = storage.CreateFileQueryWithOptions(QueryOptions)

        ' Run the query for image files
        rootPage.NotifyUser("Looking for images on " & storageName & " ...", NotifyType.StatusMessage)
        Dim imageFiles = Await imageFileQuery.GetFilesAsync()
        If imageFiles.Count > 0 Then
            Dim imageFile = imageFiles(0)
            rootPage.NotifyUser("Found " & imageFile.Name & " on " & storageName, NotifyType.StatusMessage)
            Await DisplayImageAsync(imageFile)
        Else
            rootPage.NotifyUser("No images were found on " & storageName & ". You can use scenario 2 to transfer an image to it", NotifyType.StatusMessage)
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
