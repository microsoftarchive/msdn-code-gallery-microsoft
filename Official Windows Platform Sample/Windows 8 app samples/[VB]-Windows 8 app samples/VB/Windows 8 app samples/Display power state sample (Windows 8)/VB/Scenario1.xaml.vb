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
Imports Windows.System.Display

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario1
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private g_DisplayRequest As DisplayRequest
    Private drCount As Integer = 0

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Activate_Click(sender As Object, e As RoutedEventArgs)
        ErrorTestBlock.Text = String.Empty
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            Try
                If g_DisplayRequest Is Nothing Then
                    ' This call creates an instance of the displayRequest object
                    g_DisplayRequest = New DisplayRequest()
                End If
            Catch ex As Exception
                rootPage.NotifyUser("Error Creating Display Request: " & ex.Message, NotifyType.ErrorMessage)
            End Try

            If g_DisplayRequest IsNot Nothing Then
                Try
                    ' This call activates a display-required request. If successful, 
                    ' the screen is guaranteed not to turn off automatically due to user inactivity.
                    g_DisplayRequest.RequestActive()
                    drCount += 1
                    rootPage.NotifyUser("Display request activated (" & drCount & ")", NotifyType.StatusMessage)
                Catch ex As Exception
                    rootPage.NotifyUser("Error:" & ex.Message, NotifyType.ErrorMessage)
                End Try
            End If
        End If
    End Sub

    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Release_Click(sender As Object, e As RoutedEventArgs)
        ErrorTestBlock.Text = String.Empty
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            If g_DisplayRequest IsNot Nothing Then
                Try
                    ' This call de-activates the display-required request. If successful, the screen
                    ' might be turned off automatically due to a user inactivity, depending on the
                    ' power policy settings of the system. The requestRelease method throws an exception 
                    ' if it is called before a successful requestActive call on this object.
                    g_DisplayRequest.RequestRelease()
                    drCount -= 1
                    rootPage.NotifyUser("Display request released (" & drCount & ")", NotifyType.StatusMessage)
                Catch ex As Exception
                    rootPage.NotifyUser("Error: " & ex.Message, NotifyType.ErrorMessage)
                End Try
            End If
        End If
    End Sub

End Class
