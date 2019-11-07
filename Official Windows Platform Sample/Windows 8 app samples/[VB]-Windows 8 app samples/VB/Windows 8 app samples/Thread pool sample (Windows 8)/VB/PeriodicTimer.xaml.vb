'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.System.Threading
Imports Windows.UI.Core
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports Threadpool.ThreadPool

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class PeriodicTimer
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()

        '
        ' Take a reference to the main window dispatcher object to the UI.
        '
        ThreadPoolSample.PeriodicTimerScenario = Me
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        PeriodMs.SelectedIndex = ThreadPoolSample.PeriodicTimerSelectedIndex
        UpdateUI(ThreadPoolSample.PeriodicTimerStatus)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        ThreadPoolSample.PeriodicTimerSelectedIndex = PeriodMs.SelectedIndex
    End Sub

    ''' <summary>
    ''' Create a Periodic timer that fires once after the specified Periodic elapses.
    ''' When the timer expires, its callback hander is called.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CreatePeriodicTimer(sender As Object, args As RoutedEventArgs)
        If Integer.TryParse(PeriodMs.SelectionBoxItem.ToString(), ThreadPoolSample.PeriodicTimerMilliseconds) Then
            ThreadPoolSample.PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer(Async Sub(timer)
                                                                                     System.Threading.Interlocked.Increment(ThreadPoolSample.PeriodicTimerCount)
                                                                                     Await Dispatcher.RunAsync(CoreDispatcherPriority.High, Sub()
                                                                                                                                                ThreadPoolSample.PeriodicTimerScenario.UpdateUI(Status.Completed)
                                                                                                                                            End Sub)
                                                                                 End Sub, TimeSpan.FromMilliseconds(ThreadPoolSample.PeriodicTimerMilliseconds))

            UpdateUI(Status.Started)
        End If
    End Sub

    ''' <summary>
    ''' Cancels the Periodic timer.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub CancelPeriodicTimer(sender As Object, args As RoutedEventArgs)
        If ThreadPoolSample.PeriodicTimer IsNot Nothing Then
            ThreadPoolSample.PeriodicTimer.Cancel()
            ThreadPoolSample.PeriodicTimerCount = 0
            UpdateUI(Status.Canceled)
        End If
    End Sub

    ''' <summary>
    ''' Update the scenario UI.
    ''' </summary>
    ''' <param name="status"></param>
    Public Sub UpdateUI(status__1 As Status)
        ThreadPoolSample.PeriodicTimerStatus = status__1

        Select Case status__1
            Case Status.Completed
                PeriodicTimerStatus.Text = String.Format("Completion count: {0}", ThreadPoolSample.PeriodicTimerCount)
                Exit Select
            Case Else
                PeriodicTimerStatus.Text = status__1.ToString("g")
                Exit Select
        End Select

        PeriodicTimerInfo.Text = String.Format("Timer Period = {0} ms.", ThreadPoolSample.PeriodicTimerMilliseconds)

        Dim createButtonEnabled = ((status__1 <> Status.Started) AndAlso (status__1 <> Status.Completed))
        CreatePeriodicTimerButton.IsEnabled = createButtonEnabled
        CancelPeriodicTimerButton.IsEnabled = Not createButtonEnabled
    End Sub
End Class
