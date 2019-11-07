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
Imports Windows.Storage
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation


''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class S1_ListStorages
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'List Storages' button.
    ''' </summary>
    ''' <remarks>
    ''' There are two ways to find removable storages:
    ''' The first way uses the Removable Devices KnownFolder to get a snapshot of the currently
    ''' connected devices as StorageFolders.  This is demonstrated in this scenario.
    ''' The second way uses Windows.Devices.Enumeration and is demonstrated in the second scenario.
    ''' Windows.Devices.Enumeration supports more advanced scenarios such as subscibing for device
    ''' arrival, removal and updates. Refer to the DeviceEnumeration sample for details on
    ''' Windows.Devices.Enumeration.
    ''' </remarks>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub ListStorages_Click(sender As Object, e As RoutedEventArgs)
        ScenarioOutput.Text = ""

        ' Find all storage devices using the known folder
        Dim removableStorages = Await KnownFolders.RemovableDevices.GetFoldersAsync()
        If removableStorages.Count > 0 Then
            ' Display each storage device
            For Each storage As StorageFolder In removableStorages
                ScenarioOutput.Text &= storage.DisplayName & vbLf
            Next
        Else
            rootPage.NotifyUser("No removable storages were found. Please attach a removable storage to the system (e.g. a camera or camera memory)", NotifyType.StatusMessage)
        End If
    End Sub
End Class
