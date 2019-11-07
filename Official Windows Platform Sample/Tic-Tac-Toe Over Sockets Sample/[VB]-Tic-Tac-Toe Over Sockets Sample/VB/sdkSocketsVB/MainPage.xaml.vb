'
'   Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
'   Use of this sample source code is subject to the terms of the Microsoft license 
'   agreement under which you licensed this sample source code and is provided AS-IS.
'   If you did not accept the terms of the license agreement, you are not authorized 
'   to use this sample source code.  For the terms of the license, please see the 
'   license agreement between you and Microsoft.
'  
'   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
'  
'
Imports System.Text



Partial Public Class MainPage
    Inherits basepage
    ' Cached brushes for use by the textblocks
    Private ReadOnly _greenBrush As Brush = New SolidColorBrush(Colors.Green)
    Private ReadOnly _whiteBrush As Brush = New SolidColorBrush(Colors.White)

    ' The gamepieces
    Private Const GAMEPIECE_X As String = "X"
    Private Const GAMEPIECE_0 As String = "O"

    ' Store gamepiece and its current value ("X", "0" or "" (empty))
    Private _pieceMap As Dictionary(Of String, String)

    ' ProgressIndicator that is shown during server communication
    Private _progressIndicator As ProgressIndicator

    ' Constructor
    Public Sub New()
        InitializeComponent()

        ' Create a ProgressIndicator and add it to the status bar (SystemTray)
        _progressIndicator = New ProgressIndicator()
        _progressIndicator.IsVisible = False
        _progressIndicator.IsIndeterminate = True
        SystemTray.SetProgressIndicator(Me, _progressIndicator)
    End Sub

    Protected Overrides Sub OnNavigatingFrom(ByVal e As System.Windows.Navigation.NavigatingCancelEventArgs)
        MyBase.OnNavigatingFrom(e)
    End Sub

    Private Sub appbarNewGame_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' If there is no server name set, then the game can't start
        If checkServerName() Then
            NewGame()
        End If
    End Sub

    ''' <summary>
    ''' Refreshes the game board and informs the user what gamepiece they are playing
    ''' </summary>
    Private Sub NewGame()
        If Me.PlayAsX Then
            UpdateStatus(String.Format("Playing as '{0}'", GAMEPIECE_X))
        Else
            UpdateStatus(String.Format("Playing as '{0}'", GAMEPIECE_0))
        End If

        ' The board may be "dirty" from a previosu game, so wipe it clean
        InitializeBoard()

    End Sub

#Region "Update Status"
    ''' <summary>
    ''' Helper method to write a simple status update to a TextBlock on the page.
    ''' Uses the Dispatcher object if currently not on UI Thread.
    ''' </summary>
    ''' <param name="status">The status message to write</param>
    Private Sub UpdateStatus(ByVal status As String)

        If System.Windows.Deployment.Current.Dispatcher.CheckAccess() Then
            ' Since I want to call the same logic here and in the Dispatcher case, 
            ' I encapsulate it in a method they can both call.
            _updateStatus(status)
        Else
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(Sub() _updateStatus(status))
        End If

    End Sub

    Private Sub _updateStatus(ByVal status As String)
        ' If the ProgressIndicator is visible, hide it
        SystemTray.ProgressIndicator.IsVisible = False
        tbStatus.Text = status
    End Sub
#End Region

    ''' <summary>
    ''' Clear all board pieces and reset their text color. 
    ''' Also resets the mapping of gamepiece to value
    ''' </summary>
    Private Sub InitializeBoard()
        If System.Windows.Deployment.Current.Dispatcher.CheckAccess() Then
            ' Since I want to call the same logic here and in the Dispatcher case, 
            ' I encapsulate it in a method they can both call.
            _initBoard()
        Else
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(Sub() _initBoard())
        End If

    End Sub

    ''' <summary>
    ''' Clear all text on the board, reset font color and clear out internal gamepiece map
    ''' </summary>
    Private Sub _initBoard()
        _pieceMap = New Dictionary(Of String, String)()

        ' Each gamepiece is represented as a TextBlock inside the "gBoard"  
        ' grid on the on the page
        For Each textBlock In gBoard.Children.OfType(Of TextBlock)()
            _pieceMap.Add(textBlock.Name, String.Empty)
            textBlock.Text = String.Empty
            textBlock.Foreground = _whiteBrush
        Next textBlock

        ' The user can start to tap on the grid and the pieces
        gBoard.IsHitTestVisible = True
    End Sub

    ''' <summary>
    ''' Event handler for the Tap event on each gamepiece
    ''' </summary>
    ''' <param name="sender">The gamepiece that fired the event</param>
    ''' <param name="e">The GestureEventArgs</param>
    Private Sub tb_Tap(ByVal sender As Object, ByVal e As GestureEventArgs)
        ' Don't do anything if it is the server's turn
        If _waitingOnServerMove Then
            UpdateStatus("Waiting on Server!")

        Else
            Dim tapped As TextBlock = CType(sender, TextBlock)

            ' Check whether the TextBlock (gamepiece) being tapped is playable
            If Not String.IsNullOrWhiteSpace(tapped.Text) Then
                UpdateStatus(String.Format("Oops! That square is taken.", tapped.Name, tapped.Text))
            Else
                UpdateStatus(String.Empty)

                ' Play it!
                Play(tapped)
            End If
        End If

    End Sub

    ''' <summary>
    ''' Play the selected piece.
    ''' </summary>
    ''' <param name="tbTapped">The (TextBlock) that was played</param>
    Private Sub Play(ByVal tbTapped As TextBlock)
        ' Make sure the status text is clear, because if we update the status message we want the
        ' user to notice it
        tbStatus.Text = String.Empty

        ' The user can tap on the gameboard without clicking "New Game". In this case we may 
        ' need to initialize the board if it has not been initialized already.
        ' Initialization is signified by the _pieceMap not being null.
        If _pieceMap Is Nothing Then
            InitializeBoard()
        End If

        ' Make sure the user has entered a server name
        If checkServerName() Then

            ' Set the square to the user's gamepiece
            tbTapped.Text = If(Me.PlayAsX, GAMEPIECE_X, GAMEPIECE_0)

            ' Record the updated value for that square
            _pieceMap(tbTapped.Name) = tbTapped.Text

            ' Based on the above move, it is possible that the game has been won
            If SomebodyWon Then
                GameOver()
            Else
                ' Is there still a piece for the server to play?
                If PiecesAvailable() Then
                    ' Ask the server for a move
                    GetMoveFromServer()
                Else
                    ' Nobody wins - end the game
                    GameOver()
                    UpdateStatus("Nobody Won!")
                End If

            End If
        End If

    End Sub

    Private Function checkServerName() As Boolean
        Dim result As Boolean = True
        If String.IsNullOrWhiteSpace(Me.ServerName) Then
            MessageBox.Show("You can set the server name on the settings page", "Missing Server Name", MessageBoxButton.OK)
            result = False
        End If

        Return result
    End Function

    ''' <summary>
    ''' Check to see if somebody has one the game based on the current state of the board
    ''' </summary>
    Private ReadOnly Property SomebodyWon() As Boolean
        Get
            ' Get the player who won or else string.empty if nobody has won yet
            Dim winner As String = DidSomeoneWin()
            If Not String.IsNullOrEmpty(winner) Then
                If (Me.PlayAsX AndAlso winner.ToLower() = GAMEPIECE_X.ToLower()) OrElse ((Not Me.PlayAsX) AndAlso winner.ToLower() = GAMEPIECE_0.ToLower()) Then ' if 'O' won and I am playing as 'O' -  If 'X' won and I am playing as 'X'
                    UpdateStatus("You Win!")
                    Return True
                Else
                    UpdateStatus("Server Wins!")
                    Return True
                End If
            End If

            Return False
        End Get
    End Property

    ''' <summary>
    ''' Check to see if there are any squares on the board still in play
    ''' </summary>
    ''' <returns>True if there is at least one square that is playable, False otherwise</returns>
    Private Function PiecesAvailable() As Boolean

        Dim _piecesAvailable As Boolean = False
        For Each value In _pieceMap.Values
            If String.IsNullOrEmpty(value) Then
                _piecesAvailable = True
                Exit For
            End If
        Next value

        Return _piecesAvailable
    End Function

    Private _waitingOnServerMove As Boolean = False

    ''' <summary>
    '''  Call the server asking for a move, passing the current board state
    ''' </summary>
    ''' <remarks>Communciation with the server is asynchronous. We await a response in 
    ''' the ResponseReceived handler</remarks>
    Private Sub GetMoveFromServer()
        LockGameboard()

        ' Package up the board state and send to the server so it can
        ' make it's move
        Dim boardState As String = GetBoardState()

        ' Start the progress indicator
        SystemTray.ProgressIndicator.IsIndeterminate = True
        SystemTray.ProgressIndicator.IsVisible = True
        SystemTray.ProgressIndicator.Text = "Getting move from server"

        ' Remove hard-coded port number
        Dim client As New AsynchronousClient(Me.ServerName, Me.PortNumber)
        AddHandler client.ResponseReceived, AddressOf ac_GotMove
        _waitingOnServerMove = True

        client.SendData(boardState)

    End Sub

    ''' <summary>
    ''' Callback for SendData call to server
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e">The result from the server</param>
    Private Sub ac_GotMove(ByVal sender As Object, ByVal e As ResponseReceivedEventArgs)
        If e.isError Then
            ReportMoveError(e.response)
        Else
            UnLockGameboard()
            ClearStatusText()

            ' As its next move, the server sends back one value - the name of the textblock which
            ' is its gamepiece to play That is all we need from the server to make the move on the board
            For Each tb In gBoard.Children.OfType(Of TextBlock)()
                If tb.Name = e.response Then
                    ' Play thsi piece for the server
                    tb.Text = If(Me.PlayAsX, GAMEPIECE_0, GAMEPIECE_X)
                    _pieceMap(tb.Name) = tb.Text
                    Exit For
                End If
            Next tb

            If SomebodyWon Then
                GameOver()
            End If

        End If
        _waitingOnServerMove = False

    End Sub

    ''' <summary>
    ''' Hard-coded game logic to keep things simple.
    ''' There are 8 possible ways someone can get a winning line. 
    ''' I check for each one here.
    ''' </summary>
    ''' <returns>Indicates where 'X' won or 'O' won</returns>
    Private Function DidSomeoneWin() As String
        ' Top Horizontal line
        If (Not String.IsNullOrWhiteSpace(tb_00.Text)) AndAlso tb_00.Text = tb_01.Text AndAlso tb_01.Text = tb_02.Text Then
            ShowWinningLine(tb_00, tb_01, tb_02)
            Return tb_00.Text
        End If

        ' Left Vertical Line
        If (Not String.IsNullOrWhiteSpace(tb_00.Text)) AndAlso tb_00.Text = tb_10.Text AndAlso tb_10.Text = tb_20.Text Then
            ShowWinningLine(tb_00, tb_10, tb_20)
            Return tb_00.Text
        End If

        ' Top Left to Bottom Right Line
        If (Not String.IsNullOrWhiteSpace(tb_00.Text)) AndAlso tb_00.Text = tb_11.Text AndAlso tb_11.Text = tb_22.Text Then
            ShowWinningLine(tb_00, tb_11, tb_22)
            Return tb_00.Text
        End If

        ' Bottom Left to Top Right Line
        If (Not String.IsNullOrWhiteSpace(tb_02.Text)) AndAlso tb_02.Text = tb_11.Text AndAlso tb_11.Text = tb_20.Text Then
            ShowWinningLine(tb_02, tb_11, tb_20)
            Return tb_02.Text
        End If

        ' Middle Vertical Line
        If (Not String.IsNullOrWhiteSpace(tb_01.Text)) AndAlso tb_01.Text = tb_11.Text AndAlso tb_11.Text = tb_21.Text Then
            ShowWinningLine(tb_01, tb_11, tb_21)
            Return tb_01.Text
        End If

        ' Right Vertical Line
        If (Not String.IsNullOrWhiteSpace(tb_02.Text)) AndAlso tb_02.Text = tb_12.Text AndAlso tb_12.Text = tb_22.Text Then
            ShowWinningLine(tb_02, tb_12, tb_22)
            Return tb_02.Text
        End If

        ' Middle Horizontal
        If (Not String.IsNullOrWhiteSpace(tb_10.Text)) AndAlso tb_10.Text = tb_11.Text AndAlso tb_11.Text = tb_12.Text Then
            ShowWinningLine(tb_10, tb_11, tb_12)
            Return tb_10.Text
        End If

        ' Bottom Horizontal
        If (Not String.IsNullOrWhiteSpace(tb_20.Text)) AndAlso tb_20.Text = tb_21.Text AndAlso tb_21.Text = tb_22.Text Then
            ShowWinningLine(tb_20, tb_21, tb_22)
            Return tb_20.Text
        End If

        ' Nobody won
        Return String.Empty

    End Function

    ''' <summary>
    ''' Return the game board state as a string 
    ''' </summary>
    ''' <returns>A string representing the board state</returns>
    ''' <remarks>This serialized form of board state is sent to the server.
    ''' It could also be used as the state to be saved during application lifetime 
    ''' changes (dormant, tombstoned etc.)</remarks>
    Private Function GetBoardState() As String
        Dim sb As New StringBuilder()

        ' First entry is the indication as to what piece the server is playing
        If Me.PlayAsX Then
            sb.Append(GAMEPIECE_0)
        Else
            sb.Append(GAMEPIECE_X)
        End If

        ' gamepiece seperator
        sb.Append("|")

        ' Add an indication of the current value of each gamepiece
        For Each key In _pieceMap.Keys
            sb.Append(key)

            ' Key, Value seperator
            sb.Append("*")
            sb.Append(_pieceMap(key))
            sb.Append("|")

        Next key

        ' Remove the extra "|"
        sb.Remove(sb.Length - 1, 1)

        Return sb.ToString()
    End Function

    ''' <summary>
    ''' Report an error receive when attempting to get a game move from the server
    ''' </summary>
    ''' <param name="error"></param>
    Private Sub ReportMoveError(ByVal [error] As String)
        If System.Windows.Deployment.Current.Dispatcher.CheckAccess() Then
            ' Since I want to call the same logic here and in the Dispatcher case, 
            ' I encapsulate it in a method they can both call.
            _reportMoveError([error])
        Else
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(Sub() _reportMoveError([error]))
        End If
    End Sub

    Private Sub _reportMoveError(ByVal [error] As String)

        UnLockGameboard()
        ClearStatusText()

        [error] = String.Format("Error: {0}", [error]) & Environment.NewLine & "Press OK to try again or Cancel to canel game"
        If MessageBox.Show([error], "Error getting move from server", MessageBoxButton.OKCancel) = MessageBoxResult.OK Then

            GetMoveFromServer()
        Else
            NewGame()
        End If
    End Sub



    Private Sub ClearStatusText()
        UpdateStatus(String.Empty)
    End Sub

    ''' <summary>
    ''' Add a little bling to the gameboard by showing the winning line in a different color
    ''' </summary>
    ''' <param name="w1">Winning piece 1</param>
    ''' <param name="w2">Winning piece 2</param>
    ''' <param name="w3">Winning piece 3</param>
    Private Sub ShowWinningLine(ByVal w1 As TextBlock, ByVal w2 As TextBlock, ByVal w3 As TextBlock)
        w1.Foreground = _greenBrush
        w2.Foreground = _greenBrush
        w3.Foreground = _greenBrush
    End Sub

    ''' <summary>
    ''' Lock the gameboard when the game is over
    ''' </summary>
    Private Sub GameOver()
        LockGameboard()
    End Sub

    Private Sub LockGameboard()
        gBoard.IsHitTestVisible = False
    End Sub

    Private Sub UnLockGameboard()
        gBoard.IsHitTestVisible = True
    End Sub

    Private Sub appbarHideDiag_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim toggle As ApplicationBarMenuItem = CType(sender, ApplicationBarMenuItem)

        spDiagnostics.Visibility = If(spDiagnostics.Visibility = System.Windows.Visibility.Visible, System.Windows.Visibility.Collapsed, System.Windows.Visibility.Visible)
        toggle.Text = If(spDiagnostics.Visibility = System.Windows.Visibility.Visible, "Hide Diagnostics", "Show Diagnostics")
    End Sub


    Private Sub appbarSettings_Click(ByVal sender As Object, ByVal e As EventArgs)
        NavigationService.Navigate(New Uri("/settings.xaml", UriKind.RelativeOrAbsolute))
    End Sub

End Class
