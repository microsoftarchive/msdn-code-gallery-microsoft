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

Partial Public NotInheritable Class ScenarioInput4
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
        AddHandler Scenario4DisplayToastSilent.Click, Sub(sender, e)
                                                          DisplayAudioToast("Silent")
                                                      End Sub
        AddHandler Scenario4DisplayToastDefault.Click, Sub(sender, e)
                                                           DisplayAudioToast("Default")
                                                       End Sub
        AddHandler Scenario4DisplayToastMail.Click, Sub(sender, e)
                                                        DisplayAudioToast("Mail")
                                                    End Sub
        AddHandler Scenario4DisplayToastSMS.Click, Sub(sender, e)
                                                       DisplayAudioToast("SMS")
                                                   End Sub
        AddHandler Scenario4DisplayToastIM.Click, Sub(sender, e)
                                                      DisplayAudioToast("IM")
                                                  End Sub
        AddHandler Scenario4DisplayToastReminder.Click, Sub(sender, e)
                                                            DisplayAudioToast("Reminder")
                                                        End Sub
    End Sub

    Private Sub DisplayAudioToast(audioSrc As String)
        Dim toastContent As IToastText02 = ToastContentFactory.CreateToastText02()
        toastContent.TextHeading.Text = "Sound:"
        toastContent.TextBodyWrap.Text = audioSrc
        toastContent.Audio.Content = DirectCast([Enum].Parse(GetType(ToastAudioContent), audioSrc), ToastAudioContent)

        rootPage.NotifyUser(toastContent.GetContent(), NotifyType.StatusMessage)

        ' Create a toast, then create a ToastNotifier object to show
        ' the toast
        Dim toast As ToastNotification = toastContent.CreateNotification()

        ' If you have other applications in your package, you can specify the AppId of
        ' the app to create a ToastNotifier for that application
        ToastNotificationManager.CreateToastNotifier.Show(toast)
    End Sub


#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub
#End Region
End Class
