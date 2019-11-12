' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports System.Text
Imports Windows.Data.Xml.Dom
Imports Windows.UI.Notifications
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports NotificationsExtensionsVB.ToastContent
Imports NotificationsExtensionsVB.NotificationsExtensions.ToastContent

Partial Public NotInheritable Class ScenarioInput1
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing

    Public Sub New()
        InitializeComponent()

        AddHandler Scenario1DisplayToastText01.Click, Sub(sender, e)
                                                          DisplayTextToast(ToastTemplateType.ToastText01)
                                                      End Sub
        AddHandler Scenario1DisplayToastText02.Click, Sub(sender, e)
                                                          DisplayTextToast(ToastTemplateType.ToastText02)
                                                      End Sub
        AddHandler Scenario1DisplayToastText03.Click, Sub(sender, e)
                                                          DisplayTextToast(ToastTemplateType.ToastText03)
                                                      End Sub
        AddHandler Scenario1DisplayToastText04.Click, Sub(sender, e)
                                                          DisplayTextToast(ToastTemplateType.ToastText04)
                                                      End Sub
    End Sub

    Private Sub DisplayTextToast(templateType As ToastTemplateType)
        ' Creates a toast using the notification object model, which is another project
        ' in this solution.  For an example using Xml manipulation, see the function
        ' DisplayToastUsingXmlManipulation below.
        Dim toastContent As IToastNotificationContent = Nothing

        If templateType = ToastTemplateType.ToastText01 Then
            Dim templateContent As IToastText01 = ToastContentFactory.CreateToastText01
            templateContent.TextBodyWrap.Text = "Body text that wraps"
            toastContent = templateContent
        ElseIf templateType = ToastTemplateType.ToastText02 Then
            Dim templateContent As IToastText02 = ToastContentFactory.CreateToastText02
            templateContent.TextHeading.Text = "Heading text"
            templateContent.TextBodyWrap.Text = "Body text that wraps."
            toastContent = templateContent
        ElseIf templateType = ToastTemplateType.ToastText03 Then
            Dim templateContent As IToastText03 = ToastContentFactory.CreateToastText03
            templateContent.TextHeadingWrap.Text = "Heading text that wraps"
            templateContent.TextBody.Text = "Body text"
            toastContent = templateContent
        ElseIf templateType = ToastTemplateType.ToastText04 Then
            Dim templateContent As IToastText04 = ToastContentFactory.CreateToastText04
            templateContent.TextHeading.Text = "Heading text"
            templateContent.TextBody1.Text = "Body text"
            templateContent.TextBody2.Text = "Another body text"
            toastContent = templateContent
        End If

        rootPage.NotifyUser(toastContent.GetContent, NotifyType.StatusMessage)

        ' Create a toast, then create a ToastNotifier object to show
        ' the toast
        Dim toast As ToastNotification = toastContent.CreateNotification

        ' If you have other applications in your package, you can specify the AppId of
        ' the app to create a ToastNotifier for that application
        ToastNotificationManager.CreateToastNotifier.Show(toast)
    End Sub

    Private Sub DisplayToastUsingXmlManipulation(templateType As ToastTemplateType)
        ' GetTemplateContent returns a Windows.Data.Xml.Dom.XmlDocument object containing
        ' the toast XML
        Dim toastXml As XmlDocument = ToastNotificationManager.GetTemplateContent(templateType)

        ' You can use the methods from the XML document to specify all of the
        ' required parameters for the toast
        Dim stringElements As Windows.Data.Xml.Dom.XmlNodeList = toastXml.GetElementsByTagName("text")

        For i As UInteger = 0 To CUInt(stringElements.Length - 1)
            ' In order to see which text inputs will wrap across lines, a very long string for each text
            ' input will be created using this loop
            Dim text As New StringBuilder(150)
            Dim inputNumber As UInteger = CUInt(i + 1)
            For j = 0 To 9
                text.Append("Text input " & inputNumber.ToString & " ")
            Next

            toastXml.AppendChild(toastXml.CreateTextNode(text.ToString))
        Next

        ' Create a toast from the Xml, then create a ToastNotifier object to show
        ' the toast
        Dim toast As New ToastNotification(toastXml)

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
