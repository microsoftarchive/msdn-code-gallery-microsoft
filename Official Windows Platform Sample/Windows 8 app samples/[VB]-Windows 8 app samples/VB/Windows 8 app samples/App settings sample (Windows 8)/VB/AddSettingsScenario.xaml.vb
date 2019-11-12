'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.ApplicationSettings
Imports Windows.UI.Popups
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class AddSettingsScenario
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private isEventRegistered As Boolean

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        rootPage.NotifyUser("To show the settings charm window, invoke the charm bar by swiping your finger on the right edge of the screen or bringing your mouse to the lower right corner of the screen, then select Settings. Or you can just press Windows logo + i. To dismiss the settings charm, tap in the application, swipe a screen edge, right click, invoke another charm or application.", NotifyType.StatusMessage)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        MyBase.OnNavigatedFrom(e)

        ' Added to make sure the event handler for CommandsRequested in cleaned up before other scenarios
        If Me.isEventRegistered Then
            RemoveHandler SettingsPane.GetForCurrentView.CommandsRequested, AddressOf onCommandsRequested
            Me.isEventRegistered = False
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'addSettingsScenarioAdd' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub addSettingsScenarioAdd_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            rootPage.NotifyUser("You selected the " & b.Content & " button", NotifyType.StatusMessage)
            If Not Me.isEventRegistered Then
                ''' Listening for this event lets the app initialize the settings commands and pause its UI until the user closes the pane
                ''' To ensure your settings are available at all times in your app, place your CommandsRequested handler in the overridden
                ''' OnWindowCreated of App.xaml.vb
                AddHandler SettingsPane.GetForCurrentView.CommandsRequested, AddressOf onCommandsRequested
                Me.isEventRegistered = True
            End If
        End If
    End Sub

    Private Sub onSettingsCommand(command As IUICommand)
        Dim settingsCommand As SettingsCommand = DirectCast(command, SettingsCommand)
        rootPage.NotifyUser("You selected the " & settingsCommand.Label & " settings command which originated from the " & SettingsPane.Edge.ToString(), NotifyType.StatusMessage)
    End Sub

    Private Sub onCommandsRequested(settingsPane As SettingsPane, eventArgs As SettingsPaneCommandsRequestedEventArgs)
        Dim handler As New UICommandInvokedHandler(AddressOf onSettingsCommand)

        Dim generalCommand As New SettingsCommand("generalSettings", "General", handler)
        eventArgs.Request.ApplicationCommands.Add(generalCommand)

        Dim helpCommand As New SettingsCommand("helpPage", "Help", handler)
        eventArgs.Request.ApplicationCommands.Add(helpCommand)
    End Sub


    ''' <summary>
    ''' This is the click handler for the 'addSettingsScenarioShow' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub addSettingsScenarioShow_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            rootPage.NotifyUser("You selected the " & b.Content & " button", NotifyType.StatusMessage)
            SettingsPane.Show()
        End If
    End Sub

End Class
