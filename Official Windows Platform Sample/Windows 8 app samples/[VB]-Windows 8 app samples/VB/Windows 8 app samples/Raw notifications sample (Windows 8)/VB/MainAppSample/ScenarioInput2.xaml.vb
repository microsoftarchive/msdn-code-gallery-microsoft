' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports Windows.ApplicationModel.Background
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.Graphics.Display
Imports Windows.Networking.PushNotifications
Imports Windows.Storage
Imports Windows.UI.Core
Imports Windows.UI.ViewManagement
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate

Partial Public NotInheritable Class ScenarioInput2
    Inherits Page
    ' A pointer back to the main page which is used to gain access to the input and output frames and their content
    Private rootPage As MainPage = Nothing
    Private _dispatcher As CoreDispatcher
    Private eventAdded As Boolean = False

    Public Sub New()
        InitializeComponent()
        _dispatcher = Window.Current.Dispatcher
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub
#End Region

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        RemoveHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
        UpdateListener(False)
    End Sub


#Region "Use this code if you need access to elements in the output frame - otherwise delete"
    Private Sub rootPage_OutputFrameLoaded(sender As Object, e As Object)
        ' At this point, we know that the Output Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Output Frame

        ' Get a pointer to the content within the OutputFrame
        Dim outputFrame As Page = DirectCast(rootPage.OutputFrame.Content, Page)
    End Sub

#End Region

    Private Sub Scenario2AddListener_Click(sender As Object, e As RoutedEventArgs)
        If UpdateListener(True) Then
            rootPage.NotifyUser("Now listening for raw notifications", NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("Channel not open--open a channel in Scenario 1", NotifyType.ErrorMessage)
        End If
    End Sub

    Private Sub Scenario2RemoveListener_Click(sender As Object, e As RoutedEventArgs)
        If UpdateListener(False) Then
            rootPage.NotifyUser("No longer listening for raw notifications", NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("Channel not open--open a channel in Scenario 1", NotifyType.ErrorMessage)
        End If
    End Sub

    Private Function UpdateListener(add As Boolean) As Boolean
        If rootPage.Channel IsNot Nothing Then

            If add AndAlso Not eventAdded Then
                AddHandler rootPage.Channel.PushNotificationReceived, AddressOf OnPushNotificationReceived
                eventAdded = True
            ElseIf Not add AndAlso eventAdded Then
                RemoveHandler rootPage.Channel.PushNotificationReceived, AddressOf OnPushNotificationReceived
                eventAdded = False
            End If
            Return True
        End If
        Return False
    End Function

    Private Async Sub OnPushNotificationReceived(sender As PushNotificationChannel, e As PushNotificationReceivedEventArgs)
        If e.NotificationType = PushNotificationType.Raw Then
            e.Cancel = True
            Await _dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                          rootPage.NotifyUser("Raw notification received with content: " & e.RawNotification.Content, NotifyType.StatusMessage)
                                                                      End Sub)
        End If
    End Sub
End Class
