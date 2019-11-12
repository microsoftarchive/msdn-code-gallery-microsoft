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
Imports Windows.Storage.Pickers
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class S2_SendToStorage
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
    ''' This is the click handler for the 'Send Image' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub SendImage_Click(sender As Object, e As RoutedEventArgs)
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
                items.Add(New With {.Name = deviceInfo.Name})
            Next
            DeviceList.ItemsSource = items
            DeviceSelector.Visibility = Visibility.Visible
        Else
            rootPage.NotifyUser("No removable storages were found. Please attach a removable storage to the system (e.g. a camera or camera memory)", NotifyType.StatusMessage)
        End If
    End Function

    ''' <summary>
    ''' This is the tapped handler for the device selection list. It runs this scenario
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
        Await SendImageFileToStorageAsync(deviceInfo)
    End Sub

    ''' <summary>
    ''' Sends a user-selected image file to the storage referenced by the device information element.
    ''' </summary>
    ''' <param name="deviceInfoElement">Contains information about a selected device.</param>
    Private Async Function SendImageFileToStorageAsync(deviceInfoElement As DeviceInformation) As Task

        ' Verify that we are currently not snapped, or that we can unsnap to open the file picker
        Dim unsnapped As Boolean = ((ApplicationView.Value <> ApplicationViewState.Snapped) OrElse ApplicationView.TryUnsnap)
        If Not unsnapped Then
            rootPage.NotifyUser("Could not unsnap (required to launch the file open picker). Please unsnap this app before proceeding", NotifyType.ErrorMessage)
        Else
            ' Launch the picker to select an image file
            Dim picker = New FileOpenPicker() With {.SuggestedStartLocation = PickerLocationId.PicturesLibrary}
            picker.FileTypeFilter.Add(".jpg")
            picker.FileTypeFilter.Add(".png")
            picker.FileTypeFilter.Add(".gif")

            Dim sourceFile = Await picker.PickSingleFileAsync()
            If sourceFile IsNot Nothing Then

                ' Convert the selected device information element to a StorageFolder
                Dim storage = StorageDevice.FromId(deviceInfoElement.Id)
                Dim storageName = deviceInfoElement.Name

                rootPage.NotifyUser("Copying image: " & sourceFile.Name & " to " & storageName & " ...", NotifyType.StatusMessage)
                Await CopyFileToFolderOnStorageAsync(sourceFile, storage)
            Else
                rootPage.NotifyUser("No file was selected", NotifyType.StatusMessage)
            End If
        End If
    End Function

    ''' <summary>
    ''' Copies a file to the first folder on a storage.
    ''' </summary>
    ''' <param name="sourceFile"></param>
    ''' <param name="storage"></param>
    Private Async Function CopyFileToFolderOnStorageAsync(sourceFile As StorageFile, storage As StorageFolder) As Task
        Dim storageName = storage.Name

        ' Construct a folder search to find sub-folders under the current storage.
        ' The default (shallow) query should be sufficient in finding the first level of sub-folders.
        ' If the first level of sub-folders are not writable, a deep query + recursive copy may be needed.
        Dim folders = Await storage.GetFoldersAsync()
        If folders.Count > 0 Then
            Dim destinationFolder = folders(0)
            Dim destinationFolderName = destinationFolder.Name

            rootPage.NotifyUser("Trying the first sub-folder: " & destinationFolderName & "...", NotifyType.StatusMessage)
            Try
                Dim newFile = Await sourceFile.CopyAsync(destinationFolder, sourceFile.Name, NameCollisionOption.GenerateUniqueName)
                rootPage.NotifyUser("Image " & newFile.Name & " created in folder: " & destinationFolderName & " on " & storageName, NotifyType.StatusMessage)
            Catch e As Exception
                rootPage.NotifyUser("Failed to copy image to the first sub-folder: " & destinationFolderName & ", " & storageName & " may not allow sending files to its top level folders. Error: " & e.Message, NotifyType.ErrorMessage)
            End Try
        Else
            rootPage.NotifyUser("No sub-folders found on " & storageName & " to copy to", NotifyType.StatusMessage)
        End If
    End Function
End Class
