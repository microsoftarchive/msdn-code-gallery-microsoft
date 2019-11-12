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
Imports Windows.Foundation
Imports Windows.UI.Core
Imports Windows.Storage

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class HighPriority
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private roamingSettings As ApplicationDataContainer = Nothing

    Public Sub New()
        Me.InitializeComponent()

        Dim applicationData__1 As ApplicationData = ApplicationData.Current

        roamingSettings = applicationData__1.RoamingSettings
        AddHandler applicationData__1.DataChanged, AddressOf DataChangedHandler

        DisplayOutput(False)
    End Sub

    ' Guidance for using the HighPriority setting.
    '
    ' Writing to the HighPriority setting enables a developer to store a small amount of
    ' data that will be roamed out to the cloud with higher priority than other roaming
    ' data, when possible.
    '
    ' Applications should carefully consider which data should be stored in the 
    ' HighPriority setting.  "Context" data such as the user's location within
    ' media, or their current game-baord and high-score, can make the most sense to
    ' roam with high priority.  By using the HighPriority setting, this information has
    ' a higher likelihood of being available to the user when they begin to use another
    ' machine.
    '
    ' Applications should update their HighPriority setting when the user makes
    ' a significant change to the data it represents.  Examples could include changing
    ' music tracks, turning the page in a book, or finishing a level in a game.

    Private Sub IncrementHighPriority_Click(sender As Object, e As RoutedEventArgs)
        Dim counter As Integer = CInt(roamingSettings.Values("HighPriority"))
        roamingSettings.Values("HighPriority") = counter + 1
        DisplayOutput(False)
    End Sub

    Private Async Sub DataChangedHandler(appData As Windows.Storage.ApplicationData, o As Object)
    ' DataChangeHandler may be invoked on a background thread, so use the Dispatcher to invoke the UI-related code on the UI thread.
        Await Me.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                        DisplayOutput(True)
                                                                    End Sub)
    End Sub

    Private Sub DisplayOutput(remoteUpdate As Boolean)
        Dim counter As Integer = Convert.ToInt32(roamingSettings.Values("HighPriority"))

        OutputTextBlock.Text = "Counter: " & counter & (If(remoteUpdate, " (updated remotely)", ""))
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub
End Class
