'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports Windows.UI.Core
Imports Windows.Storage

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class DataChangedEvent
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private applicationData As ApplicationData = Nothing
    Private roamingSettings As ApplicationDataContainer = Nothing

    Const settingName As String = "userName"

    Public Sub New()
        Me.InitializeComponent()

        applicationData = applicationData.Current
        roamingSettings = applicationData.RoamingSettings
        AddHandler applicationData.DataChanged, AddressOf DataChangedHandler

        DisplayOutput()
    End Sub

    Private Sub SimulateRoaming_Click(sender As Object, e As RoutedEventArgs)
        roamingSettings.Values(settingName) = UserName.Text

        ' Simulate roaming by intentionally signaling a data changed event.
        applicationData.SignalDataChanged()
    End Sub

    Private Async Sub DataChangedHandler(appData As Windows.Storage.ApplicationData, o As Object)
    ' DataChangeHandler may be invoked on a background thread, so use the Dispatcher to invoke the UI-related code on the UI thread.
        Await Me.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                        DisplayOutput()
                                                                    End Sub)
    End Sub

    Private Sub DisplayOutput()
        Dim value As Object = roamingSettings.Values(settingName)
        OutputTextBlock.Text = "Name: " & (If(value Is Nothing, "<empty>", ("""" & value.ToString & """")))
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub
End Class
