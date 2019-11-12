' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports Windows.UI.Core
Imports Windows.UI.Notifications
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports NotificationsExtensionsVB.ToastContent
Imports NotificationsExtensionsVB.NotificationsExtensions.ToastContent

Partial Public NotInheritable Class ScenarioInput5
    Inherits Page
    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing
    Private scenario5Toast As ToastNotification
    Private _dispatcher As CoreDispatcher

    Public Sub New()
        InitializeComponent()
        _dispatcher = Window.Current.Dispatcher
        AddHandler Scenario5DisplayToastWithCallbacks.Click, AddressOf Scenario5DisplayToastWithCallbacks_Click
        AddHandler Scenario5HideToast.Click, AddressOf Scenario5HideToast_Click
    End Sub

    Private Sub Scenario5DisplayToastWithCallbacks_Click(sender As Object, e As RoutedEventArgs)
        Dim toastContent As IToastText02 = ToastContentFactory.CreateToastText02

        ' Set the launch activation context parameter on the toast.
        ' The context can be recovered through the app Activation event
        toastContent.Launch = "Context123"

        toastContent.TextHeading.Text = "Tap toast"
        toastContent.TextBodyWrap.Text = "Or swipe to dismiss"

        ' You can listen for the "Activated" event provided on the toast object
        ' or listen to the "OnLaunched" event off the Windows.UI.Xaml.Application
        ' object to tell when the user clicks the toast.
        '
        ' The difference is that the OnLaunched event will
        ' be raised by local, scheduled and cloud toasts, while the event provided by the 
        ' toast object will only be raised by local toasts. 
        '
        ' In this example, we'll use the event off the CoreApplication object.
        scenario5Toast = toastContent.CreateNotification()
        AddHandler scenario5Toast.Dismissed, AddressOf toast_Dismissed
        AddHandler scenario5Toast.Failed, AddressOf toast_Failed

        ToastNotificationManager.CreateToastNotifier.Show(scenario5Toast)
    End Sub

    Private Sub Scenario5HideToast_Click(sender As Object, e As RoutedEventArgs)
        If scenario5Toast IsNot Nothing Then
            ToastNotificationManager.CreateToastNotifier.Hide(scenario5Toast)
            scenario5Toast = Nothing
        Else
            rootPage.NotifyUser("No toast has been displayed from Scenario 5", NotifyType.StatusMessage)
        End If
    End Sub

    Private Async Sub toast_Failed(sender As ToastNotification, e As ToastFailedEventArgs)
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     rootPage.NotifyUser("The toast encountered an error", NotifyType.ErrorMessage)
                                                                 End Sub)
    End Sub

    Private Async Sub toast_Dismissed(sender As ToastNotification, e As ToastDismissedEventArgs)
        Dim outputText As String = ""

        Select Case e.Reason
            Case ToastDismissalReason.ApplicationHidden
                outputText = "The app hid the toast using ToastNotifier.Hide(toast)"
                Exit Select
            Case ToastDismissalReason.UserCanceled
                outputText = "The user dismissed this toast"
                Exit Select
            Case ToastDismissalReason.TimedOut
                outputText = "The toast has timed out"
                Exit Select
        End Select

        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     rootPage.NotifyUser(outputText, NotifyType.StatusMessage)
                                                                 End Sub)
    End Sub

    Public Sub LaunchedFromToast(arguments As String)
        rootPage.NotifyUser("A toast was clicked on with activation arguments: " & arguments.ToString, NotifyType.StatusMessage)
    End Sub

#Region "Template-Related Code - Do not remove"

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)
    End Sub

#End Region
End Class
