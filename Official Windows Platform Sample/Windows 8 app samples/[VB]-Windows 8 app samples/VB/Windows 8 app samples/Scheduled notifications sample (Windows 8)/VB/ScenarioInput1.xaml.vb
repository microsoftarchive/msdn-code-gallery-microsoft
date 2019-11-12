' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports Windows.UI.Notifications
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports NotificationsExtensionsVB.ToastContent
Imports NotificationsExtensionsVB.NotificationsExtensions.ToastContent
Imports NotificationsExtensionsVB.TileContent

Partial Public NotInheritable Class ScenarioInput1
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
        AddHandler ScheduleButton.Click, AddressOf ScheduleButton_Click
    End Sub

    Private Sub ScheduleButton_Click(sender As Object, e As RoutedEventArgs)
        Try
            Dim dueTimeInSeconds As Int16 = Int16.Parse(FutureTimeBox.Text)
            If dueTimeInSeconds <= 0 Then
                Throw New ArgumentException
            End If

            Dim updateString As String = StringBox.Text
            Dim dueTime As DateTime = DateTime.Now.AddSeconds(dueTimeInSeconds)

            Dim rand As New Random
            Dim idNumber As Integer = rand.Next(0, 10000000)

            If ToastRadio.IsChecked IsNot Nothing AndAlso CBool(ToastRadio.IsChecked) Then
                ScheduleToast(updateString, dueTime, idNumber)
            Else
                ScheduleTile(updateString, dueTime, idNumber)
            End If
        Catch generatedExceptionName As Exception
            rootPage.NotifyUser("You must input a valid time in seconds.", NotifyType.ErrorMessage)
        End Try
    End Sub

    Private Sub ScheduleToast(updateString As String, dueTime As DateTime, idNumber As Integer)
        ' Scheduled toasts use the same toast templates as all other kinds of toasts.
        Dim toastContent As IToastText02 = ToastContentFactory.CreateToastText02
        toastContent.TextHeading.Text = updateString
        toastContent.TextBodyWrap.Text = "Received: " & dueTime.ToLocalTime

        Dim toast As ScheduledToastNotification
        If RepeatBox.IsChecked IsNot Nothing AndAlso CBool(RepeatBox.IsChecked) Then
            toast = New ScheduledToastNotification(toastContent.GetXml, dueTime, TimeSpan.FromSeconds(60), 5)

            ' You can specify an ID so that you can manage toasts later.
            ' Make sure the ID is 15 characters or less.
            toast.Id = "Repeat" & idNumber.ToString
        Else
            toast = New ScheduledToastNotification(toastContent.GetXml, dueTime)
            toast.Id = "Toast" & idNumber.ToString
        End If

        ToastNotificationManager.CreateToastNotifier().AddToSchedule(toast)
        rootPage.NotifyUser("Scheduled a toast with ID: " & toast.Id, NotifyType.StatusMessage)
    End Sub

    Private Sub ScheduleTile(updateString As String, dueTime As DateTime, idNumber As Integer)
        ' Set up the wide tile text
        Dim tileContent As ITileWideText09 = TileContentFactory.CreateTileWideText09
        tileContent.TextHeading.Text = updateString
        tileContent.TextBodyWrap.Text = "Received: " & dueTime.ToLocalTime

        ' Set up square tile text
        Dim squareContent As ITileSquareText04 = TileContentFactory.CreateTileSquareText04
        squareContent.TextBodyWrap.Text = updateString

        tileContent.SquareContent = squareContent

        ' Create the notification object
        Dim futureTile As New ScheduledTileNotification(tileContent.GetXml, dueTime)
        futureTile.Id = "Tile" & idNumber.ToString

        ' Add to schedule
        ' You can update a secondary tile in the same manner using CreateTileUpdaterForSecondaryTile(tileId)
        ' See "Tiles" sample for more details
        TileUpdateManager.CreateTileUpdaterForApplication().AddToSchedule(futureTile)
        rootPage.NotifyUser("Scheduled a tile with ID: " & futureTile.Id, NotifyType.StatusMessage)
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub
#End Region

End Class
