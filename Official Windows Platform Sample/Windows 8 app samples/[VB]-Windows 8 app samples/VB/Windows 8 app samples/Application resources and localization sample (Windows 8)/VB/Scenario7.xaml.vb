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
Imports Windows.ApplicationModel.Resources.Core

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario7
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        Me.Scenario7TextBlock.Text = ""
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Scenario7Button_Show' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Scenario7Button_Show_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            Me.Scenario7TextBlock.Text = ResourceManager.Current.MainResourceMap.GetValue("Resources/string1").ValueAsString

            AddHandler ResourceManager.Current.DefaultContext.QualifierValues.MapChanged, AddressOf MapChanged
        End If
		End Sub

    Async Sub MapChanged(s As IObservableMap(Of String, String), m As IMapChangedEventArgs(Of String))
        Await Me.Scenario7TextBlock.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                                           Me.Scenario7TextBlock.Text = ResourceManager.Current.MainResourceMap.GetValue("Resources/string1").ValueAsString
                                                                                                       End Sub)
    End Sub
End Class
