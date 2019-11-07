'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.System.Threading
Imports SDKTemplate
Imports Threadpool.ThreadPool

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class DelayTimer
    Inherits SDKTemplate.Common.LayoutAwarePage
    '
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    '
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()

        '
        ' Take a reference to the main window dispatcher object to the UI.
        '
        ThreadPoolSample.DelayTimerScenario = Me
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        DelayMs.SelectedIndex = ThreadPoolSample.DelayTimerSelectedIndex
        UpdateUI(ThreadPoolSample.DelayTimerStatus)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        ThreadPoolSample.DelayTimerSelectedIndex = DelayMs.SelectedIndex
    End Sub

    ''' <summary>
    ''' Create a delay timer that fires once after the specified delay elapses.
    ''' When the timer expires, its callback hander is called.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CreateDelayTimer(sender As Object, args As RoutedEventArgs)
        If Integer.TryParse(DelayMs.SelectionBoxItem.ToString, ThreadPoolSample.DelayTimerMilliseconds) Then
            Dim delay As TimeSpan = TimeSpan.FromMilliseconds(ThreadPoolSample.DelayTimerMilliseconds)
            ThreadPoolSample.DelayTimer = ThreadPoolTimer.CreateTimer(New TimerElapsedHandler(Async Function(source)
                                                                                                  Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                                                                                                               ThreadPoolSample.DelayTimerScenario.UpdateUI(Status.Completed)
                                                                                                                                                           End Sub)
                                                                                              End Function), delay)

            UpdateUI(Status.Started)
        End If
    End Sub

    ''' <summary>
    ''' Cancels the Delay timer.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CancelDelayTimer(sender As Object, args As RoutedEventArgs)
        If ThreadPoolSample.DelayTimer IsNot Nothing Then
            ThreadPoolSample.DelayTimer.Cancel()
            UpdateUI(Status.Canceled)
        End If
    End Sub

    ''' <summary>
    ''' Update the scenario UI.
    ''' </summary>
    Public Sub UpdateUI(status__1 As Status)
        ThreadPoolSample.DelayTimerStatus = status__1
        DelayTimerInfo.Text = String.Format("Timer delay = {0} ms.", ThreadPoolSample.DelayTimerMilliseconds)
        DelayTimerStatus.Text = status__1.ToString("g")

        Dim createButtonEnabled = (status__1 = Status.Started)
        CreateDelayTimerButton.IsEnabled = Not createButtonEnabled
        CancelDelayTimerButton.IsEnabled = createButtonEnabled
    End Sub
End Class
