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
Imports SDKTemplate
Imports System

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class PhoneCall
    Inherits SDKTemplate.Common.LayoutAwarePage
    Private _cw As CoreWindow
    Private callControls As Windows.Media.Devices.CallControl = Nothing
    Private callToken As ULong = 0

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
        AddHandler ButtonInitialize.Click, AddressOf initDevice
        AddHandler ButtonIncomingCall.Click, AddressOf newIncomingCall
        AddHandler ButtonHangUp.Click, AddressOf hangUp

        _cw = Window.Current.CoreWindow
    End Sub

#Region "Scenario Specific Code"

    Private Sub initDevice(sender As [Object], e As RoutedEventArgs)
        If callControls Is Nothing Then
            Try
                callControls = Windows.Media.Devices.CallControl.GetDefault()

                If callControls IsNot Nothing Then
                    ' Add the event listener to listen for the various button presses
                    AddHandler callControls.AnswerRequested, AddressOf answerButton
                    AddHandler callControls.HangUpRequested, AddressOf hangupButton
                    AddHandler callControls.AudioTransferRequested, AddressOf audiotransferButton
                    AddHandler callControls.RedialRequested, AddressOf redialButton
                    AddHandler callControls.DialRequested, AddressOf dialButton
                    enableIncomingCallButton()
                    disableInitializeButton()
                    dispatchMessage("Call Controls Initialized")
                Else
                    dispatchMessage("Call Controls Failed to Initialized")
                End If
            Catch exception As Exception
                dispatchMessage("Call Controls Failed to Initialized in Try Catch" & exception.Message.ToString)

            End Try
        End If
    End Sub

    Private Sub newIncomingCall(sender As [Object], e As RoutedEventArgs)
        ' Indicate a new incoming call and ring the headset.
        callToken = callControls.IndicateNewIncomingCall(True, "5555555555")
        disableIncomingCallButton()
        dispatchMessage("Call Token: " & callToken.ToString)
    End Sub

    Private Sub hangUp(sender As Object, e As RoutedEventArgs)
        ' Hang up request received.  The application should end the active call and stop
        ' streaming to the headset
        dispatchMessage("Hangup requested")
        callControls.EndCall(callToken)
        enableIncomingCallButton()
        disableHangUpButton()
        stopAudioElement()
        callToken = 0
    End Sub

    Private Sub answerButton(sender As Windows.Media.Devices.CallControl)
        ' When the answer button is pressed indicate to the device that the call was answered
        ' and start a song on the headset (this is done by streaming music to the bluetooth
        ' device in this sample)
        dispatchMessage("Answer Requested: " & callToken.ToString)
        callControls.IndicateActiveCall(callToken)
        SetAudioSource()
        enableHangUpButton()
        playAudioElement()

    End Sub

    Private Async Sub SetAudioSource()
        Dim AudioFile = Await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync("folk_rock.mp3")
        Dim AudioStream = Await AudioFile.OpenAsync(Windows.Storage.FileAccessMode.Read)

        Await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                         AudioElement.SetSource(AudioStream, AudioFile.ContentType)
                                                                                     End Sub)
    End Sub

    Private Sub hangupButton(sender As Windows.Media.Devices.CallControl)
        ' Hang up request received.  The application should end the active call and stop
        ' streaming to the headset
        dispatchMessage("Hangup requested")
        callControls.EndCall(callToken)
        enableIncomingCallButton()
        disableHangUpButton()
        stopAudioElement()
        callToken = 0
    End Sub

    Private Sub audiotransferButton(sender As Windows.Media.Devices.CallControl)
        ' Handle the audio transfer request here
        enableHangUpButton()
        dispatchMessage("Audio Transfer Requested")
    End Sub

    Private Sub redialButton(sender As Windows.Media.Devices.CallControl, redialRequestedEventArgs As Windows.Media.Devices.RedialRequestedEventArgs)
        ' Handle the redial request here.  Indicate to the device that the request was handled.
        dispatchMessage("Redial Requested")
        redialRequestedEventArgs.Handled()
    End Sub

    Private Sub dialButton(sender As Windows.Media.Devices.CallControl, dialRequestedEventArgs As Windows.Media.Devices.DialRequestedEventArgs)
        ' A device may send a dial request by either sending a URI or if it is a speed dial,
        ' an integer with the number to dial.
        If TypeOf (dialRequestedEventArgs.Contact) Is UInt32 Then
            dispatchMessage("Dial requested: " & dialRequestedEventArgs.Contact.ToString)
            dialRequestedEventArgs.Handled()
        Else
            ' Dialing a URI
            Dim uri As Uri = DirectCast(dialRequestedEventArgs.Contact, Uri)
            If uri.Scheme.Equals("tel", StringComparison.OrdinalIgnoreCase) Then
                Dim host As String = uri.Host
                Dim path As String = uri.PathAndQuery
                dispatchMessage("Dial requested: " & path)


            End If
        End If
    End Sub

    Private Async Sub playAudioElement()
        Await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                         AudioElement.Play()
                                                                                     End Sub)
    End Sub

    Private Async Sub stopAudioElement()
        Await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                         AudioElement.Stop()
                                                                                     End Sub)
    End Sub

    Private Async Sub enableIncomingCallButton()
        Await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                         ButtonIncomingCall.IsEnabled = True
                                                                                     End Sub)
    End Sub

    Private Async Sub enableHangUpButton()
        Await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                         ButtonHangUp.IsEnabled = True
                                                                                     End Sub)
    End Sub

    Private Async Sub disableInitializeButton()
        Await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                         ButtonInitialize.IsEnabled = False
                                                                                     End Sub)
    End Sub

    Private Async Sub disableIncomingCallButton()
        Await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                         ButtonIncomingCall.IsEnabled = False
                                                                                     End Sub)
    End Sub

    Private Async Sub disableHangUpButton()
        Await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                         ButtonHangUp.IsEnabled = False
                                                                                     End Sub)
    End Sub

    Private Sub dispatchMessage(message As String)
        dispatchStatusMessage(message)
    End Sub

    Private Async Sub dispatchErrorMessage(message As String)
        Await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                         rootPage.NotifyUser(getTimeStampedMessage(message), NotifyType.ErrorMessage)
                                                                                     End Sub)
    End Sub

    Private Async Sub dispatchStatusMessage(message As String)
        Await _cw.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                         rootPage.NotifyUser(getTimeStampedMessage(message), NotifyType.StatusMessage)
                                                                                     End Sub)
    End Sub

    Private Function getTimeStampedMessage(eventText As String) As String
        Dim dateTimeFormat As New Windows.Globalization.DateTimeFormatting.DateTimeFormatter("longtime")
        Dim dateTime__1 As String = dateTimeFormat.Format(DateTime.Now)
        Return eventText & "   " & dateTime__1
    End Function

#End Region
End Class
