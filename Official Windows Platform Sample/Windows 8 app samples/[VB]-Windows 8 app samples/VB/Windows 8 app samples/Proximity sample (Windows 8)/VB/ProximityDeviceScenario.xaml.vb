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
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ProximityDeviceScenario
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current
    Private _proximityDevice As Windows.Networking.Proximity.ProximityDevice
    Private _publishedMessageId As Long = -1
    Private _subscribedMessageId As Long = -1

    Public Sub New()
        Me.InitializeComponent()
        _proximityDevice = ProximityDevice.GetDefault()
        If _proximityDevice IsNot Nothing Then
            ' This scenario demonstrates using publish/subscribe in order to publish a message from on PC to the other using
            ' the proximity infrastructure.
            ' For example, a PC device can publish a contact card or a photo url which can be then used by a PC that 
            ' subscribed to the message to identify the device involved in the proximity "tap".
            AddHandler ProximityDevice_PublishMessageButton.Click, AddressOf ProximityDevice_PublishMessage
            AddHandler ProximityDevice_SubscribeForMessageButton.Click, AddressOf ProximityDevice_SubscribeForMessage
            ProximityDevice_PublishMessageButton.Visibility = Visibility.Visible
            ProximityDevice_SubscribeForMessageButton.Visibility = Visibility.Visible
            ProximityDevice_PublishMessageText.Visibility = Visibility.Visible
        End If
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ProximityDevice_PublishMessageText.Text = "Hello World"
        If _proximityDevice Is Nothing Then
            rootPage.NotifyUser("No proximity device found", NotifyType.ErrorMessage)
        End If
        rootPage.ClearLog(ProximityDeviceOutputText)
    End Sub

    Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
        If _proximityDevice IsNot Nothing Then
            If _publishedMessageId <> -1 Then
                _proximityDevice.StopPublishingMessage(_publishedMessageId)
                _publishedMessageId = -1
            End If
            If _subscribedMessageId <> -1 Then
                _proximityDevice.StopSubscribingForMessage(_subscribedMessageId)
                _subscribedMessageId = 1
            End If
        End If
    End Sub

    Private Sub ProximityDevice_PublishMessage(sender As Object, e As RoutedEventArgs)
        If _publishedMessageId = -1 Then
            rootPage.NotifyUser("", NotifyType.ErrorMessage)
            Dim publishText As String = ProximityDevice_PublishMessageText.Text
            ProximityDevice_PublishMessageText.Text = ""
            ' clear the input after publishing.
            If publishText.Length > 0 Then
                _publishedMessageId = _proximityDevice.PublishMessage("Windows.SampleMessageType", publishText)
                rootPage.NotifyUser("Message published, tap another device to transmit.", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("Please type a message", NotifyType.ErrorMessage)
            End If
        Else
            rootPage.NotifyUser("This sample only supports publishing one message at a time.", NotifyType.ErrorMessage)
        End If
    End Sub

    Private Sub MessageReceived(proximityDevice As ProximityDevice, message As ProximityMessage)
        rootPage.UpdateLog("Message received: " & message.DataAsString, ProximityDeviceOutputText)
    End Sub

    Private Sub ProximityDevice_SubscribeForMessage(sender As Object, e As RoutedEventArgs)
        If _subscribedMessageId = -1 Then
            _subscribedMessageId = _proximityDevice.SubscribeForMessage("Windows.SampleMessageType", AddressOf MessageReceived)
            rootPage.NotifyUser("Subscribed for proximity message, enter proximity to receive.", NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("This sample only supports subscribing for one message at a time.", NotifyType.ErrorMessage)
        End If
    End Sub
End Class
