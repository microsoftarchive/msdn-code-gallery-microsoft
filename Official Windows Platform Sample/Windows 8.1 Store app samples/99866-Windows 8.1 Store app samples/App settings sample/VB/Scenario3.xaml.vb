' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario3
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()

        rootPage.NotifyUser("Swipe the right edge of the screen to invoke the Charms bar and select Settings.  Alternatively, press Windows+I.", NotifyType.StatusMessage)
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        AddHandler SettingsPane.GetForCurrentView().CommandsRequested, AddressOf onCommandsRequested
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        MyBase.OnNavigatedFrom(e)

        RemoveHandler SettingsPane.GetForCurrentView().CommandsRequested, AddressOf onCommandsRequested
    End Sub

    ''' <summary>
    ''' Handler for the CommandsRequested event. Add custom SettingsCommands here.
    ''' </summary>
    ''' <param name="e">Event data that includes a vector of commands (ApplicationCommands)</param>
    Private Sub onCommandsRequested(ByVal settingsPane As SettingsPane, ByVal e As SettingsPaneCommandsRequestedEventArgs)
        Dim defaultsCommand As New SettingsCommand("defaults", "Defaults", Sub(handler)
                                                                               ' SettingsFlyout1 is defined in "SettingsFlyout1.xaml"
                                                                               rootPage.NotifyUser("You opened the 'Defaults' SettingsFlyout.", NotifyType.StatusMessage)
                                                                               Dim sf As New SettingsFlyout1()
                                                                               sf.Show()
                                                                           End Sub)
        e.Request.ApplicationCommands.Add(defaultsCommand)

    End Sub

End Class
