' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports NotificationsExtensions.ToastContent
Imports Windows.UI.Notifications

Partial Public NotInheritable Class ScenarioInput4
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()
        AddHandler Scenario4DisplayToastSilent.Click, Sub(sender, e) DisplayAudioToast("Silent")
        AddHandler Scenario4DisplayToastDefault.Click, Sub(sender, e) DisplayAudioToast("Default")
        AddHandler Scenario4DisplayToastMail.Click, Sub(sender, e) DisplayAudioToast("Mail")
        AddHandler Scenario4DisplayToastSMS.Click, Sub(sender, e) DisplayAudioToast("SMS")
        AddHandler Scenario4DisplayToastIM.Click, Sub(sender, e) DisplayAudioToast("IM")
        AddHandler Scenario4DisplayToastReminder.Click, Sub(sender, e) DisplayAudioToast("Reminder")
        AddHandler Scenario4DisplayToastSilentString.Click, Sub(sender, e) DisplayAudioToastWithStringManipulation("Silent")
        AddHandler Scenario4DisplayToastDefaultString.Click, Sub(sender, e) DisplayAudioToastWithStringManipulation("Default")
        AddHandler Scenario4DisplayToastMailString.Click, Sub(sender, e) DisplayAudioToastWithStringManipulation("Mail")
    End Sub

    Private Sub DisplayAudioToast(ByVal audioSrc As String)
        Dim toastContent As IToastText02 = ToastContentFactory.CreateToastText02()
        toastContent.TextHeading.Text = "Sound:"
        toastContent.TextBodyWrap.Text = audioSrc
        toastContent.Audio.Content = CType(System.Enum.Parse(GetType(ToastAudioContent), audioSrc), ToastAudioContent)

        rootPage.NotifyUser(toastContent.GetContent(), NotifyType.StatusMessage)

        ' Create a toast, then create a ToastNotifier object to show
        ' the toast
        Dim toast As ToastNotification = toastContent.CreateNotification()

        ' If you have other applications in your package, you can specify the AppId of
        ' the app to create a ToastNotifier for that application
        ToastNotificationManager.CreateToastNotifier().Show(toast)
    End Sub

    Private Sub DisplayAudioToastWithStringManipulation(ByVal audioSrc As String)
        Dim toastXmlString As String = String.Empty

        If audioSrc.Equals("Silent") Then
            toastXmlString = "<toast>" & "<visual version='1'>" & "<binding template='ToastText02'>" & "<text id='1'>Sound:</text>" & "<text id='2'>" & audioSrc & "</text>" & "</binding>" & "</visual>" & "<audio silent='true'/>" & "</toast>"
        ElseIf audioSrc.Equals("Default") Then
            toastXmlString = "<toast>" & "<visual version='1'>" & "<binding template='ToastText02'>" & "<text id='1'>Sound:</text>" & "<text id='2'>" & audioSrc & "</text>" & "</binding>" & "</visual>" & "</toast>"
        Else
            toastXmlString = "<toast>" & "<visual version='1'>" & "<binding template='ToastText02'>" & "<text id='1'>Sound:</text>" & "<text id='2'>" & audioSrc & "</text>" & "</binding>" & "</visual>" & "<audio src='ms-winsoundevent:Notification." & audioSrc & "'/>" & "</toast>"
        End If

        Dim toastDOM As New Windows.Data.Xml.Dom.XmlDocument()
        Try
            toastDOM.LoadXml(toastXmlString)

            rootPage.NotifyUser(toastDOM.GetXml(), NotifyType.StatusMessage)

            ' Create a toast, then create a ToastNotifier object to show
            ' the toast
            Dim toast As New ToastNotification(toastDOM)

            ' If you have other applications in your package, you can specify the AppId of
            ' the app to create a ToastNotifier for that application
            ToastNotificationManager.CreateToastNotifier().Show(toast)
        Catch e1 As Exception
            rootPage.NotifyUser("Error loading the xml, check for invalid characters in the input", NotifyType.ErrorMessage)
        End Try
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub
#End Region
End Class
