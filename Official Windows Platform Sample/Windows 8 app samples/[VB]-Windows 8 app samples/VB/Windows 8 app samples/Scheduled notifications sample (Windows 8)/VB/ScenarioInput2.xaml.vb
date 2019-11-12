' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.Collections.Generic
Imports Windows.UI.Notifications
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports Windows.Data.Xml.Dom

Friend Class NotificationData
    Public Property ItemType As String
        Get
            Return m_ItemType
        End Get
        Set(value As String)
            m_ItemType = value
        End Set
    End Property
    Private m_ItemType As String

    Public Property ItemId As String
        Get
            Return m_ItemId
        End Get
        Set(value As String)
            m_ItemId = value
        End Set
    End Property
    Private m_ItemId As String

    Public Property DueTime As String
        Get
            Return m_DueTime
        End Get
        Set(value As String)
            m_DueTime = value
        End Set
    End Property
    Private m_DueTime As String

    Public Property InputString As String
        Get
            Return m_InputString
        End Get
        Set(value As String)
            m_InputString = value
        End Set
    End Property
    Private m_InputString As String

    Public Property IsTile As Boolean
        Get
            Return m_IsTile
        End Get
        Set(value As Boolean)
            m_IsTile = value
        End Set
    End Property
    Private m_IsTile As Boolean
End Class

Partial Public NotInheritable Class ScenarioInput2
    Inherits Page
    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
        AddHandler RefreshListButton.Click, AddressOf RefreshList_Click
        AddHandler RemoveButton.Click, AddressOf Remove_Click
    End Sub

    ' Remove the notification by checking the list of scheduled notifications for a notification with matching ID.
    ' While it would be possible to manage the notifications by storing a reference to each notification, such practice
    ' causes memory leaks by not allowing the notifications to be collected once they have shown.
    ' It's important to create unique IDs for each notification if they are to be managed later.
    Private Sub Remove_Click(sender As Object, e As RoutedEventArgs)
        Dim items As IList(Of Object) = ItemGridView.SelectedItems
        For i = 0 To items.Count - 1
            Dim item As NotificationData = DirectCast(items(i), NotificationData)
            Dim itemId As String = item.ItemId
            If item.IsTile Then
                Dim updater As TileUpdater = TileUpdateManager.CreateTileUpdaterForApplication()
                Dim scheduled As IReadOnlyList(Of ScheduledTileNotification) = updater.GetScheduledTileNotifications()
                For j = 0 To scheduled.Count - 1
                    If scheduled(j).Id = itemId Then
                        updater.RemoveFromSchedule(scheduled(j))
                    End If
                Next
            Else
                Dim notifier As ToastNotifier = ToastNotificationManager.CreateToastNotifier
                Dim scheduled As IReadOnlyList(Of ScheduledToastNotification) = notifier.GetScheduledToastNotifications
                For j = 0 To scheduled.Count - 1
                    If scheduled(j).Id = itemId Then
                        notifier.RemoveFromSchedule(scheduled(j))
                    End If
                Next
            End If
        Next
        rootPage.NotifyUser("Removed selected scheduled notifications", NotifyType.StatusMessage)
        RefreshListView()
    End Sub

    Private Sub RefreshList_Click(sender As Object, e As RoutedEventArgs)
        RefreshListView()
    End Sub

    Private Sub RefreshListView()
        Dim scheduledToasts As IReadOnlyList(Of ScheduledToastNotification) = ToastNotificationManager.CreateToastNotifier.GetScheduledToastNotifications
        Dim scheduledTiles As IReadOnlyList(Of ScheduledTileNotification) = TileUpdateManager.CreateTileUpdaterForApplication.GetScheduledTileNotifications

        Dim toastLength As Integer = scheduledToasts.Count
        Dim tileLength As Integer = scheduledTiles.Count

        Dim bindingList As New List(Of NotificationData)(toastLength + tileLength)
        For i = 0 To toastLength - 1
            Dim toast As ScheduledToastNotification = scheduledToasts(i)

            bindingList.Add(New NotificationData With {.ItemType = "Toast", .ItemId = toast.Id,
                                                         .DueTime = toast.DeliveryTime.ToLocalTime().ToString,
                                                         .InputString = Function(S As XmlNodeList) As List(Of String)
                                                                            Dim l As New List(Of String)
                                                                            For Each i1 In S
                                                                                l.Add(i1.InnerText)
                                                                            Next
                                                                            Return l
                                                                        End Function.Invoke(toast.Content.GetElementsByTagName("text")).Item(0),
                                                         .IsTile = False})

        Next

        For i = 0 To tileLength - 1
            Dim tile As ScheduledTileNotification = scheduledTiles(i)

            bindingList.Add(New NotificationData With {.ItemType = "Tile",
                                                         .ItemId = tile.Id,
                                                         .DueTime = tile.DeliveryTime.ToLocalTime().ToString,
                                                         .InputString = Function(S As XmlNodeList) As List(Of String)
                                                                            Dim l As New List(Of String)
                                                                            For Each i1 In S
                                                                                l.Add(i1.InnerText)
                                                                            Next
                                                                            Return l
                                                                        End Function.Invoke(tile.Content.GetElementsByTagName("text")).Item(0),
                                                        .IsTile = False
                                                        })
        Next

        ItemGridView.ItemsSource = bindingList
    End Sub


    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
        RefreshListView()
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        MyBase.OnNavigatedFrom(e)
        ItemGridView.ItemsSource = Nothing
    End Sub
End Class
