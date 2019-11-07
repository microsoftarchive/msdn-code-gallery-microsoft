'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System
Imports Windows.Networking.NetworkOperators
Imports System.Collections.Generic

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class ShowConnection
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private deviceAccountId As IReadOnlyList(Of String) = Nothing

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        PrepareScenario()
    End Sub


    ''' <summary>
    ''' This is the click handler for the 'Default' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub ShowConnectionUI_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            Try
                Dim mobileBroadbandAccount__1 = MobileBroadbandAccount.CreateFromNetworkAccountId(deviceAccountId(0))
                mobileBroadbandAccount__1.CurrentNetwork.ShowConnectionUI()
            Catch ex As Exception
                rootPage.NotifyUser("Error:" & ex.Message, NotifyType.ErrorMessage)
            End Try
        End If
    End Sub


    Private Sub PrepareScenario()
        rootPage.NotifyUser("", NotifyType.StatusMessage)

        Try
            deviceAccountId = MobileBroadbandAccount.AvailableNetworkAccountIds

            If deviceAccountId.Count <> 0 Then
                ShowConnectionUI.Content = "Show Connection UI"
                ShowConnectionUI.IsEnabled = True
            Else
                ShowConnectionUI.Content = "No available accounts detected"
                ShowConnectionUI.IsEnabled = False
            End If
        Catch ex As Exception
            rootPage.NotifyUser("Error:" & ex.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

End Class
