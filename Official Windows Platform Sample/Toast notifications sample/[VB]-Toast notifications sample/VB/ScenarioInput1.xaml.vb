' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System
Imports NotificationsExtensions.ToastContent
Imports Windows.UI.Notifications

Partial Public NotInheritable Class ScenarioInput1
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()

        AddHandler Scenario1DisplayToastText01.Click, Sub(sender, e) DisplayTextToast(ToastTemplateType.ToastText01)
        AddHandler Scenario1DisplayToastText02.Click, Sub(sender, e) DisplayTextToast(ToastTemplateType.ToastText02)
        AddHandler Scenario1DisplayToastText03.Click, Sub(sender, e) DisplayTextToast(ToastTemplateType.ToastText03)
        AddHandler Scenario1DisplayToastText04.Click, Sub(sender, e) DisplayTextToast(ToastTemplateType.ToastText04)

        AddHandler Scenario1DisplayToastText01String.Click, Sub(sender, e) DisplayTextToastWithStringManipulation(ToastTemplateType.ToastText01)
        AddHandler Scenario1DisplayToastText02String.Click, Sub(sender, e) DisplayTextToastWithStringManipulation(ToastTemplateType.ToastText02)
        AddHandler Scenario1DisplayToastText03String.Click, Sub(sender, e) DisplayTextToastWithStringManipulation(ToastTemplateType.ToastText03)
        AddHandler Scenario1DisplayToastText04String.Click, Sub(sender, e) DisplayTextToastWithStringManipulation(ToastTemplateType.ToastText04)
    End Sub

    Private Sub DisplayTextToast(ByVal templateType As ToastTemplateType)
        ' Creates a toast using the notification object model, which is another project
        ' in this solution.  For an example using Xml manipulation, see the function
        ' DisplayToastUsingXmlManipulation below.
        Dim toastContent As IToastNotificationContent = Nothing

        If templateType = ToastTemplateType.ToastText01 Then
            Dim templateContent As IToastText01 = ToastContentFactory.CreateToastText01()
            templateContent.TextBodyWrap.Text = "Body text that wraps over three lines"
            toastContent = templateContent
        ElseIf templateType = ToastTemplateType.ToastText02 Then
            Dim templateContent As IToastText02 = ToastContentFactory.CreateToastText02()
            templateContent.TextHeading.Text = "Heading text"
            templateContent.TextBodyWrap.Text = "Body text that wraps over two lines"
            toastContent = templateContent
        ElseIf templateType = ToastTemplateType.ToastText03 Then
            Dim templateContent As IToastText03 = ToastContentFactory.CreateToastText03()
            templateContent.TextHeadingWrap.Text = "Heading text that is very long and wraps over two lines"
            templateContent.TextBody.Text = "Body text"
            toastContent = templateContent
        ElseIf templateType = ToastTemplateType.ToastText04 Then
            Dim templateContent As IToastText04 = ToastContentFactory.CreateToastText04()
            templateContent.TextHeading.Text = "Heading text"
            templateContent.TextBody1.Text = "First body text"
            templateContent.TextBody2.Text = "Second body text"
            toastContent = templateContent
        End If

        rootPage.NotifyUser(toastContent.GetContent(), NotifyType.StatusMessage)

        ' Create a toast, then create a ToastNotifier object to show
        ' the toast
        Dim toast As ToastNotification = toastContent.CreateNotification()

        ' If you have other applications in your package, you can specify the AppId of
        ' the app to create a ToastNotifier for that application
        ToastNotificationManager.CreateToastNotifier().Show(toast)
    End Sub

    Private Sub DisplayTextToastWithStringManipulation(ByVal templateType As ToastTemplateType)
        Dim toastXmlString As String = String.Empty
        If templateType = ToastTemplateType.ToastText01 Then
            toastXmlString = "<toast>" & "<visual version='1'>" & "<binding template='ToastText01'>" & "<text id='1'>Body text that wraps over three lines</text>" & "</binding>" & "</visual>" & "</toast>"
        ElseIf templateType = ToastTemplateType.ToastText02 Then
            toastXmlString = "<toast>" & "<visual version='1'>" & "<binding template='ToastText02'>" & "<text id='1'>Heading text</text>" & "<text id='2'>Body text that wraps over two lines</text>" & "</binding>" & "</visual>" & "</toast>"
        ElseIf templateType = ToastTemplateType.ToastText03 Then
            toastXmlString = "<toast>" & "<visual version='1'>" & "<binding template='ToastText03'>" & "<text id='1'>Heading text that is very long and wraps over two lines</text>" & "<text id='2'>Body text</text>" & "</binding>" & "</visual>" & "</toast>"
        ElseIf templateType = ToastTemplateType.ToastText04 Then
            toastXmlString = "<toast>" & "<visual version='1'>" & "<binding template='ToastText04'>" & "<text id='1'>Heading text</text>" & "<text id='2'>First body text</text>" & "<text id='3'>Second body text</text>" & "</binding>" & "</visual>" & "</toast>"
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
