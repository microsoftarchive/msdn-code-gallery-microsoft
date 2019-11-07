'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Foundation
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports Windows.UI.Core
Imports Windows.Networking.Proximity
Imports Windows.Networking.Sockets
Imports Windows.Storage.Streams
Imports SDKTemplate
Imports System
Imports System.Collections.Generic
Imports System.Diagnostics

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class PeerFinderScenario
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    Implements System.IDisposable

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as rootPage.NotifyUser()
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current

    Private _peerInformationList As IReadOnlyList(Of PeerInformation)
    Private _requestingPeer As PeerInformation
    Private _socket As StreamSocket = Nothing
    Private _socketClosed As Boolean = True
    Private _dataWriter As DataWriter = Nothing
    Private _triggeredConnectSupported As Boolean = False
    Private _browseConnectSupported As Boolean = False

    Public Sub New()
        Me.InitializeComponent()
        ' Scenario 1 init
        _triggeredConnectSupported = (PeerFinder.SupportedDiscoveryTypes And PeerDiscoveryTypes.Triggered) = PeerDiscoveryTypes.Triggered
        _browseConnectSupported = (PeerFinder.SupportedDiscoveryTypes And PeerDiscoveryTypes.Browse) = PeerDiscoveryTypes.Browse
        If _triggeredConnectSupported OrElse _browseConnectSupported Then
            ' This scenario demonstrates "PeerFinder" to tap or browse for peers to connect to using a StreamSocket
            AddHandler PeerFinder_StartFindingPeersButton.Click, AddressOf PeerFinder_StartFindingPeers
            AddHandler PeerFinder_BrowsePeersButton.Click, AddressOf PeerFinder_BrowsePeers
            AddHandler PeerFinder_ConnectButton.Click, AddressOf PeerFinder_Connect
            AddHandler PeerFinder_AcceptButton.Click, AddressOf PeerFinder_Accept
            AddHandler PeerFinder_SendButton.Click, AddressOf PeerFinder_Send
            PeerFinder_StartFindingPeersButton.Visibility = Visibility.Visible
        End If
    End Sub

    'connection states
    Private rgConnectState As String() = {"PeerFound", "Listening", "Connecting", "Completed", "Canceled", "Failed"}

    Private Async Sub TriggeredConnectionStateChangedEventHandler(sender As Object, eventArgs As TriggeredConnectionStateChangedEventArgs)
        rootPage.UpdateLog("TriggeredConnectionStateChangedEventHandler - " & rgConnectState(CInt(eventArgs.State)).ToString, PeerFinderOutputText)

        If eventArgs.State = TriggeredConnectState.PeerFound Then
            rootPage.NotifyUser("", NotifyType.ErrorMessage)
            ' Use this state to indicate to users that the tap is complete and
            ' they can pull their devices away.
            rootPage.NotifyUser("Tap complete, socket connection starting!", NotifyType.StatusMessage)
        End If

        If eventArgs.State = TriggeredConnectState.Completed Then
            rootPage.NotifyUser("Socket connect success!", NotifyType.StatusMessage)
            ' Start using the socket that just connected.
            Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                         Me.PeerFinder_StartSendReceive(eventArgs.Socket)
                                                                     End Sub)
        End If

        If eventArgs.State = TriggeredConnectState.Failed Then
            rootPage.NotifyUser("Socket connect failed!", NotifyType.ErrorMessage)
        End If
    End Sub

    Private _peerFinderStarted As Boolean = False

    Private Sub SocketError(errMessage As String)
        rootPage.NotifyUser(errMessage, NotifyType.ErrorMessage)
        PeerFinder_StartFindingPeersButton.Visibility = Visibility.Visible
        If _browseConnectSupported Then
            PeerFinder_BrowsePeersButton.Visibility = Visibility.Visible
        End If
        PeerFinder_SendButton.Visibility = Visibility.Collapsed
        PeerFinder_MessageBox.Visibility = Visibility.Collapsed
        CloseSocket()
    End Sub

    Private Async Sub PeerFinder_Send(sender As Object, e As RoutedEventArgs)
        rootPage.NotifyUser("", NotifyType.ErrorMessage)
        Dim message As String = PeerFinder_MessageBox.Text
        PeerFinder_MessageBox.Text = "" ' clear the input now that the message is being sent.
        If Not _socketClosed Then
            If message.Length > 0 Then
                Try
                    Dim strLength As UInteger = _dataWriter.MeasureString(message)
                    _dataWriter.WriteUInt32(strLength)
                    _dataWriter.WriteString(message)
                    Dim numBytesWritten As UInteger = Await _dataWriter.StoreAsync()
                    If numBytesWritten > 0 Then
                        rootPage.NotifyUser("Sent message: " & message & ", number of bytes written: " & numBytesWritten.ToString, NotifyType.StatusMessage)
                    Else
                        SocketError("The remote side closed the socket")
                    End If
                Catch err As Exception
                    If Not _socketClosed Then
                        SocketError("Failed to send message with error: " & err.Message)
                    End If
                End Try
            Else
                rootPage.NotifyUser("Please type a message", NotifyType.ErrorMessage)
            End If
        Else
            SocketError("The remote side closed the socket")
        End If
    End Sub

    Private Async Sub PeerFinder_Accept(sender As Object, e As RoutedEventArgs)
        rootPage.NotifyUser("Connecting to " & _requestingPeer.DisplayName & "....", NotifyType.StatusMessage)
        PeerFinder_AcceptButton.Visibility = Visibility.Collapsed
        Try
            Dim socket As StreamSocket = Await PeerFinder.ConnectAsync(_requestingPeer)
            rootPage.NotifyUser("Connection succeeded", NotifyType.StatusMessage)
            PeerFinder_StartSendReceive(socket)
        Catch err As Exception
            rootPage.NotifyUser("Connection to " & _requestingPeer.DisplayName & " failed: " & err.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

    Private Async Sub PeerConnectionRequested(sender As Object, args As ConnectionRequestedEventArgs)
        _requestingPeer = args.PeerInformation
        Await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, Sub()
                                                                     rootPage.NotifyUser("Connection requested from peer " & args.PeerInformation.DisplayName, NotifyType.StatusMessage)

                                                                     Me.PeerFinder_AcceptButton.Visibility = Visibility.Visible
                                                                     Me.PeerFinder_SendButton.Visibility = Visibility.Collapsed
                                                                     Me.PeerFinder_MessageBox.Visibility = Visibility.Collapsed
                                                                 End Sub)
    End Sub

    Private Async Sub PeerFinder_StartReader(socketReader As DataReader)
        Try
            Dim bytesRead As UInteger = Await socketReader.LoadAsync(4)
            If bytesRead > 0 Then
                Dim strLength As UInteger = CUInt(socketReader.ReadUInt32())
                bytesRead = Await socketReader.LoadAsync(strLength)
                If bytesRead > 0 Then
                    Dim message As String = socketReader.ReadString(strLength)
                    rootPage.NotifyUser("Got message: " & message, NotifyType.StatusMessage)
                    PeerFinder_StartReader(socketReader) ' Start another reader
                Else
                    SocketError("The remote side closed the socket")
                    socketReader.Dispose()
                End If
            Else
                SocketError("The remote side closed the socket")
                socketReader.Dispose()
            End If
        Catch e As Exception
            If Not _socketClosed Then
                SocketError("Reading from socket failed: " & e.Message)
            End If
            socketReader.Dispose()
        End Try
    End Sub

    ' Start the send receive operations
    Private Sub PeerFinder_StartSendReceive(socket As StreamSocket)
        _socket = socket
        ' If the scenario was switched just as the socket connection completed, just close the socket.
        If Not _peerFinderStarted Then
            CloseSocket()
            Return
        End If

        PeerFinder_SendButton.Visibility = Visibility.Visible
        PeerFinder_MessageBox.Visibility = Visibility.Visible

        ' Hide the controls related to setting up a connection
        PeerFinder_ConnectButton.Visibility = Visibility.Collapsed
        PeerFinder_AcceptButton.Visibility = Visibility.Collapsed
        PeerFinder_FoundPeersList.Visibility = Visibility.Collapsed
        PeerFinder_BrowsePeersButton.Visibility = Visibility.Collapsed
        PeerFinder_StartFindingPeersButton.Visibility = Visibility.Collapsed
        _dataWriter = New DataWriter(_socket.OutputStream)
        _socketClosed = False
        PeerFinder_StartReader(New DataReader(_socket.InputStream))
    End Sub

    Private Async Sub PeerFinder_Connect(sender As Object, e As RoutedEventArgs)
        rootPage.NotifyUser("", NotifyType.ErrorMessage)
        Dim peerToConnect As PeerInformation = Nothing
        Try
            ' If nothing is selected, select the first peer
            If PeerFinder_FoundPeersList.SelectedIndex = -1 Then
                peerToConnect = _peerInformationList(0)
            Else
                peerToConnect = _peerInformationList(PeerFinder_FoundPeersList.SelectedIndex)
            End If

            rootPage.NotifyUser("Connecting to " & peerToConnect.DisplayName & "....", NotifyType.StatusMessage)
            Dim socket As StreamSocket = Await PeerFinder.ConnectAsync(peerToConnect)
            rootPage.NotifyUser("Connection succeeded", NotifyType.StatusMessage)
            PeerFinder_StartSendReceive(socket)
        Catch err As Exception
            rootPage.NotifyUser("Connection to " & peerToConnect.DisplayName & " failed: " & err.Message, NotifyType.ErrorMessage)
        End Try
    End Sub


    Private Async Sub PeerFinder_BrowsePeers(sender As Object, e As RoutedEventArgs)
        rootPage.NotifyUser("Finding Peers...", NotifyType.StatusMessage)
        Try
            _peerInformationList = Await PeerFinder.FindAllPeersAsync()
        Catch ex As Exception
            Debug.WriteLine("FindAll throws exception" & ex.Message)
        End Try
        Debug.WriteLine("Async operation completed")
        rootPage.NotifyUser("No peers found", NotifyType.StatusMessage)

        If _peerInformationList.Count > 0 Then
            PeerFinder_FoundPeersList.Items.Clear()
            For i As Integer = 0 To _peerInformationList.Count - 1
                Dim item As New ListBoxItem()
                item.Content = _peerInformationList(i).DisplayName
                PeerFinder_FoundPeersList.Items.Add(item)
            Next
            PeerFinder_ConnectButton.Visibility = Visibility.Visible
            PeerFinder_FoundPeersList.Visibility = Visibility.Visible
            rootPage.NotifyUser("Finding Peers Done", NotifyType.StatusMessage)
        Else
            rootPage.NotifyUser("No peers found", NotifyType.StatusMessage)
            PeerFinder_ConnectButton.Visibility = Visibility.Collapsed
            PeerFinder_FoundPeersList.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub PeerFinder_StartFindingPeers(sender As Object, e As RoutedEventArgs)
        rootPage.NotifyUser("", NotifyType.ErrorMessage)
        If Not _peerFinderStarted Then
            ' attach the callback handler (there can only be one PeerConnectProgress handler).
            AddHandler PeerFinder.TriggeredConnectionStateChanged, AddressOf TriggeredConnectionStateChangedEventHandler
            ' attach the incoming connection request event handler
            AddHandler PeerFinder.ConnectionRequested, AddressOf PeerConnectionRequested
            ' start listening for proximate peers
            PeerFinder.Start()
            _peerFinderStarted = True
            If _browseConnectSupported AndAlso _triggeredConnectSupported Then
                rootPage.NotifyUser("Tap another device to connect to a peer or click Browse for Peers button.", NotifyType.StatusMessage)
                PeerFinder_BrowsePeersButton.Visibility = Visibility.Visible
            ElseIf _triggeredConnectSupported Then
                rootPage.NotifyUser("Tap another device to connect to a peer.", NotifyType.StatusMessage)
            ElseIf _browseConnectSupported Then
                rootPage.NotifyUser("Click Browse for Peers button.", NotifyType.StatusMessage)
                PeerFinder_BrowsePeersButton.Visibility = Visibility.Visible
            End If
        End If
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        If _triggeredConnectSupported OrElse _browseConnectSupported Then
            ' Initially only the advertise button should be visible.
            PeerFinder_StartFindingPeersButton.Visibility = Visibility.Visible
            PeerFinder_BrowsePeersButton.Visibility = Visibility.Collapsed
            PeerFinder_ConnectButton.Visibility = Visibility.Collapsed
            PeerFinder_FoundPeersList.Visibility = Visibility.Collapsed
            PeerFinder_SendButton.Visibility = Visibility.Collapsed
            PeerFinder_AcceptButton.Visibility = Visibility.Collapsed
            PeerFinder_MessageBox.Visibility = Visibility.Collapsed
            PeerFinder_MessageBox.Text = "Hello World"
            If rootPage.IsLaunchedByTap() Then
                rootPage.NotifyUser("Launched by tap", NotifyType.StatusMessage)
                PeerFinder_StartFindingPeers(Nothing, Nothing)
            Else
                If Not _triggeredConnectSupported Then
                    rootPage.NotifyUser("Tap based discovery of peers not supported", NotifyType.ErrorMessage)
                ElseIf Not _browseConnectSupported Then
                    rootPage.NotifyUser("Browsing for peers not supported", NotifyType.ErrorMessage)
                End If
            End If
        Else
            rootPage.NotifyUser("Tap based discovery of peers not supported \nBrowsing for peers not supported", NotifyType.ErrorMessage)
        End If
    End Sub

    ' Invoked when the main page navigates to a different scenario
    Protected Overrides Sub OnNavigatingFrom(e As NavigatingCancelEventArgs)
        If _peerFinderStarted Then
            ' detach the callback handler (there can only be one PeerConnectProgress handler).
            RemoveHandler PeerFinder.TriggeredConnectionStateChanged, AddressOf TriggeredConnectionStateChangedEventHandler
            ' detach the incoming connection request event handler
            RemoveHandler PeerFinder.ConnectionRequested, AddressOf PeerConnectionRequested
            PeerFinder.[Stop]()
            CloseSocket()
            _peerFinderStarted = False
        End If
    End Sub

    Private Sub CloseSocket()
        If _socket IsNot Nothing Then
            _socketClosed = True
            _socket.Dispose()
            _socket = Nothing
        End If

        If _dataWriter IsNot Nothing Then
            _dataWriter.Dispose()
            _dataWriter = Nothing
        End If
    End Sub

    Public Overloads Sub Dispose() Implements System.IDisposable.Dispose
        CloseSocket()
    End Sub

End Class
