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

Partial Public NotInheritable Class ScenarioInput3
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing
    Const ALT_TEXT As String = "Web image"

    Public Sub New()
        InitializeComponent()
        AddHandler Scenario3DisplayToastImage01.Click, Sub(sender, e)
                                                           DisplayWebImageToast(ToastTemplateType.ToastImageAndText01)
                                                       End Sub
        AddHandler Scenario3DisplayToastImage02.Click, Sub(sender, e)
                                                           DisplayWebImageToast(ToastTemplateType.ToastImageAndText02)
                                                       End Sub
        AddHandler Scenario3DisplayToastImage03.Click, Sub(sender, e)
                                                           DisplayWebImageToast(ToastTemplateType.ToastImageAndText03)
                                                       End Sub
        AddHandler Scenario3DisplayToastImage04.Click, Sub(sender, e)
                                                           DisplayWebImageToast(ToastTemplateType.ToastImageAndText04)
                                                       End Sub
    End Sub

    Private Sub DisplayWebImageToast(templateType As ToastTemplateType)
        Dim toastContent As IToastNotificationContent = Nothing
        Dim toastImageSrc As String = Scenario3ImageUrl.Text

        If templateType = ToastTemplateType.ToastImageAndText01 Then
            Dim templateContent As IToastImageAndText01 = ToastContentFactory.CreateToastImageAndText01()
            templateContent.TextBodyWrap.Text = "Body text that wraps"
            templateContent.Image.Src = toastImageSrc
            templateContent.Image.Alt = ALT_TEXT
            toastContent = templateContent
        ElseIf templateType = ToastTemplateType.ToastImageAndText02 Then
            Dim templateContent As IToastImageAndText02 = ToastContentFactory.CreateToastImageAndText02()
            templateContent.TextHeading.Text = "Heading text"
            templateContent.TextBodyWrap.Text = "Body text that wraps."
            templateContent.Image.Src = toastImageSrc
            templateContent.Image.Alt = ALT_TEXT
            toastContent = templateContent
        ElseIf templateType = ToastTemplateType.ToastImageAndText03 Then
            Dim templateContent As IToastImageAndText03 = ToastContentFactory.CreateToastImageAndText03()
            templateContent.TextHeadingWrap.Text = "Heading text that wraps"
            templateContent.TextBody.Text = "Body text"
            templateContent.Image.Src = toastImageSrc
            templateContent.Image.Alt = ALT_TEXT
            toastContent = templateContent
        ElseIf templateType = ToastTemplateType.ToastImageAndText04 Then
            Dim templateContent As IToastImageAndText04 = ToastContentFactory.CreateToastImageAndText04()
            templateContent.TextHeading.Text = "Heading text"
            templateContent.TextBody1.Text = "Body text"
            templateContent.TextBody2.Text = "Another body text"
            templateContent.Image.Src = toastImageSrc
            templateContent.Image.Alt = ALT_TEXT
            toastContent = templateContent
        End If

        rootPage.NotifyUser(toastContent.GetContent, NotifyType.StatusMessage)

        ' Create a toast from the Xml, then create a ToastNotifier object to show
        ' the toast
        Dim toast As ToastNotification = toastContent.CreateNotification

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
