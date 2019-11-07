'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.Networking.Proximity
Imports SDKTemplate
Imports System

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ProximityDeviceEvents
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current
    Private _proximityDevice As Windows.Networking.Proximity.ProximityDevice

    Public Sub New()
        Me.InitializeComponent()
        _proximityDevice = ProximityDevice.GetDefault()
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        If _proximityDevice IsNot Nothing Then
            AddHandler _proximityDevice.DeviceArrived, AddressOf DeviceArrived
            AddHandler _proximityDevice.DeviceDeparted, AddressOf DeviceDeparted
        Else
            rootPage.NotifyUser("No proximity device found", NotifyType.ErrorMessage)
        End If
    End Sub
    ' Invoked when the main page navigates to a different scenario
    Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
        If _proximityDevice IsNot Nothing Then
            RemoveHandler _proximityDevice.DeviceArrived, AddressOf DeviceArrived
            RemoveHandler _proximityDevice.DeviceDeparted, AddressOf DeviceDeparted
        End If
    End Sub

    Private Sub DeviceArrived(proximityDevice As ProximityDevice)
        rootPage.UpdateLog("Proximate device arrived", ProximityDeviceEventsOutputText)
    End Sub

    Private Sub DeviceDeparted(proximityDevice As ProximityDevice)
        rootPage.UpdateLog("Proximate device departed", ProximityDeviceEventsOutputText)
    End Sub
End Class
