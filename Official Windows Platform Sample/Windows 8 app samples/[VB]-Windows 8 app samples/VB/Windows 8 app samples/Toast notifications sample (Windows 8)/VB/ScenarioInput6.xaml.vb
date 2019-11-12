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

Partial Public NotInheritable Class ScenarioInput6
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing
    Private scenario6Toast As ToastNotification = Nothing

    Public Sub New()
        InitializeComponent()
        AddHandler Scenario6Looping.Click, Sub(sender, e)
                                               DisplayLongToast(True)
                                           End Sub

        AddHandler Scenario6NoLooping.Click, Sub(sender, e)
                                                 DisplayLongToast(False)
                                             End Sub


        AddHandler Scenario6HideToast.Click, AddressOf HideToast_Click
    End Sub

    Private Sub DisplayLongToast(loopAudio As Boolean)
        Dim toastContent As IToastText02 = ToastContentFactory.CreateToastText02

        ' Toasts can optionally be set to long duration
        toastContent.Duration = ToastDuration.Long

        toastContent.TextHeading.Text = "Long Duration Toast"

        If loopAudio Then
            toastContent.Audio.Loop = True
            toastContent.Audio.Content = ToastAudioContent.LoopingAlarm
            toastContent.TextBodyWrap.Text = "Looping audio"
        Else
            toastContent.Audio.Content = ToastAudioContent.IM
        End If

        scenario6Toast = toastContent.CreateNotification
        ToastNotificationManager.CreateToastNotifier.Show(scenario6Toast)

        rootPage.NotifyUser(toastContent.GetContent, NotifyType.StatusMessage)
    End Sub

    Private Sub HideToast_Click(sender As Object, e As RoutedEventArgs)
        If scenario6Toast IsNot Nothing Then
            ToastNotificationManager.CreateToastNotifier.Hide(scenario6Toast)
            scenario6Toast = Nothing
        Else
            rootPage.NotifyUser("No toast has been displayed from Scenario 6", NotifyType.StatusMessage)
        End If
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub
#End Region

End Class
