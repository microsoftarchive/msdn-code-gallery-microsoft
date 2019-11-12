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
Imports Windows.Networking.NetworkOperators
Imports System.Text

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class AccountWatcher
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Private networkAccountWatcher As New MobileBroadbandAccountWatcher()

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        PrepScenario2Sample()
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Start' button.  
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub StartMonitoring_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            If (networkAccountWatcher.Status = MobileBroadbandAccountWatcherStatus.Started) OrElse (networkAccountWatcher.Status = MobileBroadbandAccountWatcherStatus.EnumerationCompleted) Then
                rootPage.NotifyUser("Watcher is already started.", NotifyType.StatusMessage)
            Else
                networkAccountWatcher.Start()

                rootPage.NotifyUser("Watcher is started successfully.", NotifyType.StatusMessage)
            End If
        End If
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Stop' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub StopMonitoring_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            If (networkAccountWatcher.Status = MobileBroadbandAccountWatcherStatus.Started) OrElse (networkAccountWatcher.Status = MobileBroadbandAccountWatcherStatus.EnumerationCompleted) Then
                networkAccountWatcher.Stop()

                rootPage.NotifyUser("Watcher is stopped successfully.", NotifyType.StatusMessage)
            Else
                rootPage.NotifyUser("Watcher is already stopped.", NotifyType.StatusMessage)
            End If
        End If
    End Sub

    Private Sub PrepScenario2Sample()
        rootPage.NotifyUser("", NotifyType.StatusMessage)

        AddHandler networkAccountWatcher.AccountAdded, Sub(sender As MobileBroadbandAccountWatcher, args As MobileBroadbandAccountEventArgs)
                                                           Dim message As String = "[accountadded] "

                                                           Try
                                                               message &= args.NetworkAccountId

                                                               Dim account = MobileBroadbandAccount.CreateFromNetworkAccountId(args.NetworkAccountId)

                                                               message &= ", service provider name: " & account.ServiceProviderName
                                                           Catch ex As Exception
                                                               message &= ex.Message
                                                           End Try

                                                           DisplayWatcherOutputFromCallback(message)

                                                       End Sub


        AddHandler networkAccountWatcher.AccountUpdated, Sub(sender As MobileBroadbandAccountWatcher, args As MobileBroadbandAccountUpdatedEventArgs)
                                                             Dim message As String = "[accountupdated] "

                                                             Try
                                                                 message &= args.NetworkAccountId & ", (network = " & args.HasNetworkChanged & "; deviceinformation = " & args.HasDeviceInformationChanged & ")" & Environment.NewLine
                                                                 message &= DumpPropertyData(args.NetworkAccountId, args.HasNetworkChanged, args.HasDeviceInformationChanged)
                                                             Catch ex As Exception
                                                                 message &= ex.Message
                                                             End Try

                                                             DisplayWatcherOutputFromCallback(message)
                                                         End Sub

        AddHandler networkAccountWatcher.AccountRemoved, Sub(sender As MobileBroadbandAccountWatcher, args As MobileBroadbandAccountEventArgs)
                                                             Dim message As String = "[accountremoved] "

                                                             Try
                                                                 message &= args.NetworkAccountId
                                                             Catch ex As Exception
                                                                 message &= ex.Message
                                                             End Try

                                                             DisplayWatcherOutputFromCallback(message)
                                                         End Sub

        AddHandler networkAccountWatcher.EnumerationCompleted, Sub(sender As MobileBroadbandAccountWatcher, args As Object)
                                                                   Dim message As String = "[enumerationcompleted]"

                                                                   DisplayWatcherOutputFromCallback(message)
                                                               End Sub

        AddHandler networkAccountWatcher.Stopped, Sub(sender As MobileBroadbandAccountWatcher, args As Object)
                                                      Dim message As String = "[stopped] Watcher is stopped successfully"
                                                      DisplayWatcherOutputFromCallback(message)
                                                  End Sub
		End Sub

    Private Async Sub DisplayWatcherOutputFromCallback(value As String)
        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, Sub()
                                                                                     WatcherOutput.Text += value + vbNewLine
                                                                                 End Sub)
    End Sub

    Private Sub DumpAccountDeviceInformation(message As StringBuilder, deviceInformation As MobileBroadbandDeviceInformation)
        message.AppendLine("NetworkDeviceStatus: " & deviceInformation.NetworkDeviceStatus.ToString)
        message.AppendLine("MobileEquipmentId: " & deviceInformation.MobileEquipmentId)
        message.AppendLine("SubscriberId: " & deviceInformation.SubscriberId)
        message.AppendLine("SimIccId: " & deviceInformation.SimIccId)
        message.AppendLine("DeviceId: " & deviceInformation.DeviceId)
        message.AppendLine("RadioState: " & deviceInformation.CurrentRadioState)
    End Sub

    Private Sub DumpAccountNetwork(message As StringBuilder, network As MobileBroadbandNetwork)
        Dim accessPointName As String = network.AccessPointName
        If String.IsNullOrEmpty(accessPointName) Then
            accessPointName = "(not connected)"
        End If

        message.AppendLine("NetworkRegistrationState: " & network.NetworkRegistrationState.ToString)
        message.AppendLine("RegistrationNetworkError: " & NetErrorToString(network.RegistrationNetworkError))
        message.AppendLine("PacketAttachNetworkError: " & NetErrorToString(network.PacketAttachNetworkError))
        message.AppendLine("ActivationNetworkError: " & NetErrorToString(network.ActivationNetworkError))
        message.AppendLine("AccessPointName: " & accessPointName)
    End Sub

    Private Function DumpPropertyData(networkAccountId As String, hasDeviceInformationChanged As Boolean, hasNetworkChanged As Boolean) As String
        Dim message As New StringBuilder()

        Dim account = MobileBroadbandAccount.CreateFromNetworkAccountId(networkAccountId)

        If hasDeviceInformationChanged Then
            DumpAccountDeviceInformation(message, account.CurrentDeviceInformation)
        End If

        If hasNetworkChanged Then
            DumpAccountNetwork(message, account.CurrentNetwork)
        End If

        Return message.ToString
    End Function

    Private Function NetErrorToString(netError As UInteger) As String
        Return If(netError = 0, "none", netError.ToString)
    End Function
End Class
