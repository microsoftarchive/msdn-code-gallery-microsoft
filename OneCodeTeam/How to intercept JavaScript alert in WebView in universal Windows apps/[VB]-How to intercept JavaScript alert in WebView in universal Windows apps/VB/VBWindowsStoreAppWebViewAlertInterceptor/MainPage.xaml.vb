'***************************** Module Header ******************************\
' Module Name:  MainPage.xaml.vb
' Project:      VBWindowsStoreAppWebViewAlertInterceptor
' Copyright (c) Microsoft Corporation.
'
' This code sample shows you how to intercept JavaScript alert in WebView and 
' display the alert message in Windows Store apps.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

' The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class MainPage
    Inherits Page
    Public Sub New()
        Me.InitializeComponent()
        AddHandler Window.Current.SizeChanged, AddressOf MainPage_SizeChanged
    End Sub

    Private Sub MainPage_SizeChanged(sender As Object, e As Windows.UI.Core.WindowSizeChangedEventArgs)
        If e.Size.Width <= 500 Then
            VisualStateManager.GoToState(Me, "MinimalLayout", True)
        ElseIf e.Size.Width > e.Size.Height Then
            VisualStateManager.GoToState(Me, "DefaultLayout", True)
        Else
            VisualStateManager.GoToState(Me, "PortraitLayout", True)
        End If
    End Sub

    Private Async Sub WebViewWithJSInjection_NavigationCompleted(sender As WebView, args As WebViewNavigationCompletedEventArgs)
        Dim result As String = Await Me.WebViewWithJSInjection.InvokeScriptAsync("eval", New String() {"window.alert = function (AlertMessage) {window.external.notify(AlertMessage)}"})
    End Sub

    Private Async Sub WebViewWithJSInjection_ScriptNotify(sender As Object, e As NotifyEventArgs)
        Dim dialog As New Windows.UI.Popups.MessageDialog(e.Value)
        Await dialog.ShowAsync()
    End Sub

    Private Async Sub Footer_Click(sender As Object, e As RoutedEventArgs)
        Await Windows.System.Launcher.LaunchUriAsync(New Uri(TryCast(sender, HyperlinkButton).Tag.ToString()))
    End Sub
End Class

