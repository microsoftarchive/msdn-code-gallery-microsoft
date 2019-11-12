' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports NotificationsExtensions.ToastContent
Imports Windows.UI.Notifications

Partial Public NotInheritable Class ScenarioInput2
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing
    Private Const TOAST_IMAGE_SRC As String = "images/toastImageAndText.png"
    Private Const ALT_TEXT As String = "Placeholder image"

    Public Sub New()
        InitializeComponent()
        AddHandler Scenario2DisplayToastImage01.Click, Sub(sender, e) DisplayPackageImageToast(ToastTemplateType.ToastImageAndText01)
        AddHandler Scenario2DisplayToastImage02.Click, Sub(sender, e) DisplayPackageImageToast(ToastTemplateType.ToastImageAndText02)
        AddHandler Scenario2DisplayToastImage03.Click, Sub(sender, e) DisplayPackageImageToast(ToastTemplateType.ToastImageAndText03)
        AddHandler Scenario2DisplayToastImage04.Click, Sub(sender, e) DisplayPackageImageToast(ToastTemplateType.ToastImageAndText04)

        AddHandler Scenario2DisplayToastImage01String.Click, Sub(sender, e) DisplayPackageImageToastWithStringManipulation(ToastTemplateType.ToastImageAndText01)
        AddHandler Scenario2DisplayToastImage02String.Click, Sub(sender, e) DisplayPackageImageToastWithStringManipulation(ToastTemplateType.ToastImageAndText02)
        AddHandler Scenario2DisplayToastImage03String.Click, Sub(sender, e) DisplayPackageImageToastWithStringManipulation(ToastTemplateType.ToastImageAndText03)
        AddHandler Scenario2DisplayToastImage04String.Click, Sub(sender, e) DisplayPackageImageToastWithStringManipulation(ToastTemplateType.ToastImageAndText04)
    End Sub

    Private Sub DisplayPackageImageToast(ByVal templateType As ToastTemplateType)
        Dim toastContent As IToastNotificationContent = Nothing

        If templateType = ToastTemplateType.ToastImageAndText01 Then
            Dim templateContent As IToastImageAndText01 = ToastContentFactory.CreateToastImageAndText01()
            templateContent.TextBodyWrap.Text = "Body text that wraps"
            templateContent.Image.Src = TOAST_IMAGE_SRC
            templateContent.Image.Alt = ALT_TEXT
            toastContent = templateContent
        ElseIf templateType = ToastTemplateType.ToastImageAndText02 Then
            Dim templateContent As IToastImageAndText02 = ToastContentFactory.CreateToastImageAndText02()
            templateContent.TextHeading.Text = "Heading text"
            templateContent.TextBodyWrap.Text = "Body text that wraps."
            templateContent.Image.Src = TOAST_IMAGE_SRC
            templateContent.Image.Alt = ALT_TEXT
            toastContent = templateContent
        ElseIf templateType = ToastTemplateType.ToastImageAndText03 Then
            Dim templateContent As IToastImageAndText03 = ToastContentFactory.CreateToastImageAndText03()
            templateContent.TextHeadingWrap.Text = "Heading text that wraps"
            templateContent.TextBody.Text = "Body text"
            templateContent.Image.Src = TOAST_IMAGE_SRC
            templateContent.Image.Alt = ALT_TEXT
            toastContent = templateContent
        ElseIf templateType = ToastTemplateType.ToastImageAndText04 Then
            Dim templateContent As IToastImageAndText04 = ToastContentFactory.CreateToastImageAndText04()
            templateContent.TextHeading.Text = "Heading text"
            templateContent.TextBody1.Text = "Body text"
            templateContent.TextBody2.Text = "Another body text"
            templateContent.Image.Src = TOAST_IMAGE_SRC
            templateContent.Image.Alt = ALT_TEXT
            toastContent = templateContent
        End If

        rootPage.NotifyUser(toastContent.GetContent(), NotifyType.StatusMessage)

        ' Create a toast from the Xml, then create a ToastNotifier object to show
        ' the toast
        Dim toast As ToastNotification = toastContent.CreateNotification()

        ' If you have other applications in your package, you can specify the AppId of
        ' the app to create a ToastNotifier for that application
        ToastNotificationManager.CreateToastNotifier().Show(toast)
    End Sub

    Private Sub DisplayPackageImageToastWithStringManipulation(ByVal templateType As ToastTemplateType)
        Dim toastXmlString As String = String.Empty

        If templateType = ToastTemplateType.ToastImageAndText01 Then
            toastXmlString = "<toast>" & "<visual version='1'>" & "<binding template='toastImageAndText01'>" & "<text id='1'>Body text that wraps over three lines</text>" & "<image id='1' src='" & TOAST_IMAGE_SRC & "' alt='" & ALT_TEXT & "'/>" & "</binding>" & "</visual>" & "</toast>"
        ElseIf templateType = ToastTemplateType.ToastImageAndText02 Then
            toastXmlString = "<toast>" & "<visual version='1'>" & "<binding template='toastImageAndText02'>" & "<text id='1'>Heading text</text>" & "<text id='2'>Body text that wraps over two lines</text>" & "<image id='1' src='" & TOAST_IMAGE_SRC & "' alt='" & ALT_TEXT & "'/>" & "</binding>" & "</visual>" & "</toast>"
        ElseIf templateType = ToastTemplateType.ToastImageAndText03 Then
            toastXmlString = "<toast>" & "<visual version='1'>" & "<binding template='toastImageAndText03'>" & "<text id='1'>Heading text that wraps over two lines</text>" & "<text id='2'>Body text</text>" & "<image id='1' src='" & TOAST_IMAGE_SRC & "' alt='" & ALT_TEXT & "'/>" & "</binding>" & "</visual>" & "</toast>"
        ElseIf templateType = ToastTemplateType.ToastImageAndText04 Then
            toastXmlString = "<toast>" & "<visual version='1'>" & "<binding template='toastImageAndText04'>" & "<text id='1'>Heading text</text>" & "<text id='2'>First body text</text>" & "<text id='3'>Second body text</text>" & "<image id='1' src='" & TOAST_IMAGE_SRC & "' alt='" & ALT_TEXT & "'/>" & "</binding>" & "</visual>" & "</toast>"
        End If

        Dim toastDOM As New Windows.Data.Xml.Dom.XmlDocument()
        toastDOM.LoadXml(toastXmlString)

        rootPage.NotifyUser(toastDOM.GetXml(), NotifyType.StatusMessage)

        ' Create a toast, then create a ToastNotifier object to show
        ' the toast
        Dim toast As New ToastNotification(toastDOM)

        ' If you have other applications in your package, you can specify the AppId of
        ' the app to create a ToastNotifier for that application
        ToastNotificationManager.CreateToastNotifier().Show(toast)
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub
#End Region
End Class
