/* 
    Copyright (c) 2011 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Text;
using Microsoft.Phone.Shell;
using System.Windows;

namespace sdkSocketsCS
{
    public partial class MainPage : basepage
    {
        // Cached brushes for use by the textblocks
        private readonly Brush _greenBrush = new SolidColorBrush(Colors.Green);
        private readonly Brush _whiteBrush = new SolidColorBrush(Colors.White);

        // The gamepieces
        private const string GAMEPIECE_X = "X";
        private const string GAMEPIECE_0 = "O";

        // Store gamepiece and its current value ("X", "0" or "" (empty))
        private Dictionary<string, string> _pieceMap;

        // ProgressIndicator that is shown during server communication
        private ProgressIndicator _progressIndicator;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Create a ProgressIndicator and add it to the status bar (SystemTray)
            _progressIndicator = new ProgressIndicator();
            _progressIndicator.IsVisible = false;
            _progressIndicator.IsIndeterminate = true;
            SystemTray.SetProgressIndicator(this, _progressIndicator);
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
        }

        private void appbarNewGame_Click(object sender, EventArgs e)
        {
            // If there is no server name set, then the game can't start
            if (checkServerName())
            {
                NewGame();
            }
        }

        /// <summary>
        /// Refreshes the game board and informs the user what gamepiece they are playing
        /// </summary>
        private void NewGame()
        {
            if (this.PlayAsX)
                UpdateStatus(String.Format("Playing as '{0}'", GAMEPIECE_X));
            else
                UpdateStatus(String.Format("Playing as '{0}'", GAMEPIECE_0));

            // The board may be "dirty" from a previosu game, so wipe it clean
            InitializeBoard();

        }

        #region Update Status
        /// <summary>
        /// Helper method to write a simple status update to a TextBlock on the page.
        /// Uses the Dispatcher object if currently not on UI Thread.
        /// </summary>
        /// <param name="status">The status message to write</param>
        private void UpdateStatus(string status)
        {

            if (System.Windows.Deployment.Current.Dispatcher.CheckAccess())
            {
                // Since I want to call the same logic here and in the Dispatcher case, 
                // I encapsulate it in a method they can both call.
                _updateStatus(status);
            }
            else
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    _updateStatus(status);

                });
            }

        }

        private void _updateStatus(string status)
        {
            // If the ProgressIndicator is visible, hide it
            SystemTray.ProgressIndicator.IsVisible = false;
            tbStatus.Text = status;
        }
        #endregion

        /// <summary>
        /// Clear all board pieces and reset their text color. 
        /// Also resets the mapping of gamepiece to value
        /// </summary>
        private void InitializeBoard()
        {
            if (System.Windows.Deployment.Current.Dispatcher.CheckAccess())
            {
                // Since I want to call the same logic here and in the Dispatcher case, 
                // I encapsulate it in a method they can both call.
                _initBoard();
            }
            else
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    _initBoard();

                });
            }

        }

        /// <summary>
        /// Clear all text on the board, reset font color and clear out internal gamepiece map
        /// </summary>
        private void _initBoard()
        {
            _pieceMap = new Dictionary<string, string>();

            // Each gamepiece is represented as a TextBlock inside the "gBoard"  
            // grid on the on the page
            foreach (TextBlock textBlock in gBoard.Children.OfType<TextBlock>())
            {
                _pieceMap.Add(textBlock.Name, string.Empty);
                textBlock.Text = string.Empty;
                textBlock.Foreground = _whiteBrush;
            }

            // The user can start to tap on the grid and the pieces
            gBoard.IsHitTestVisible = true;
        }

        /// <summary>
        /// Event handler for the Tap event on each gamepiece
        /// </summary>
        /// <param name="sender">The gamepiece that fired the event</param>
        /// <param name="e">The GestureEventArgs</param>
        private void tb_Tap(object sender, GestureEventArgs e)
        {
            // Don't do anything if it is the server's turn
            if (_waitingOnServerMove)
            {
                UpdateStatus("Waiting on Server!");

            }
            else
            {
                TextBlock tapped = (TextBlock)sender;

                // Check whether the TextBlock (gamepiece) being tapped is playable
                if (!String.IsNullOrWhiteSpace(tapped.Text))
                {
                    UpdateStatus(String.Format("Oops! That square is taken.", tapped.Name, tapped.Text));
                }
                else
                {
                    UpdateStatus(String.Empty);

                    // Play it!
                    Play(tapped);
                }
            }

        }

        /// <summary>
        /// Play the selected piece.
        /// </summary>
        /// <param name="tbTapped">The (TextBlock) that was played</param>
        private void Play(TextBlock tbTapped)
        {
            // Make sure the status text is clear, because if we update the status message we want the
            // user to notice it
            tbStatus.Text = String.Empty;

            // The user can tap on the gameboard without clicking "New Game". In this case we may 
            // need to initialize the board if it has not been initialized already.
            // Initialization is signified by the _pieceMap not being null.
            if (_pieceMap == null)
                InitializeBoard();

            // Make sure the user has entered a server name
            if (checkServerName())
            {

                // Set the square to the user's gamepiece
                tbTapped.Text = (this.PlayAsX) ? GAMEPIECE_X : GAMEPIECE_0;

                // Record the updated value for that square
                _pieceMap[tbTapped.Name] = tbTapped.Text;

                // Based on the above move, it is possible that the game has been won
                if (SomebodyWon)
                {
                    GameOver();
                }
                else
                {
                    /// Is there still a piece for the server to play?
                    if (PiecesAvailable())
                    {
                        // Ask the server for a move
                        GetMoveFromServer();
                    }
                    else
                    {
                        // Nobody wins - end the game
                        GameOver();
                        UpdateStatus("Nobody Won!");
                    }

                }
            }

        }

        private bool checkServerName()
        {
            bool result = true;
            if (String.IsNullOrWhiteSpace(this.ServerName))
            {
                MessageBox.Show("You can set the server name on the settings page", "Missing Server Name", MessageBoxButton.OK);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Check to see if somebody has one the game based on the current state of the board
        /// </summary>
        private bool SomebodyWon
        {
            get
            {
                // Get the player who won or else string.empty if nobody has won yet
                string winner = DidSomeoneWin();
                if (!String.IsNullOrEmpty(winner))
                {
                    if (
                        (this.PlayAsX && winner.ToLower() == GAMEPIECE_X.ToLower()) // If 'X' won and I am playing as 'X'
                        ||
                        (!this.PlayAsX && winner.ToLower() == GAMEPIECE_0.ToLower())) // if 'O' won and I am playing as 'O'
                    {
                        UpdateStatus("You Win!");
                        return true;
                    }
                    else
                    {
                        UpdateStatus("Server Wins!");
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Check to see if there are any squares on the board still in play
        /// </summary>
        /// <returns>True if there is at least one square that is playable, False otherwise</returns>
        private bool PiecesAvailable()
        {
            bool piecesAvailable = false;
            foreach (string value in _pieceMap.Values)
            {
                if (String.IsNullOrEmpty(value))
                {
                    piecesAvailable = true;
                    break;
                }
            }

            return piecesAvailable;
        }

        bool _waitingOnServerMove = false;

        /// <summary>
        ///  Call the server asking for a move, passing the current board state
        /// </summary>
        /// <remarks>Communciation with the server is asynchronous. We await a response in 
        /// the ResponseReceived handler</remarks>
        private void GetMoveFromServer()
        {
            LockGameboard();

            // Package up the board state and send to the server so it can
            // make it's move
            string boardState = GetBoardState();

            // Start the progress indicator
            SystemTray.ProgressIndicator.IsIndeterminate = true;
            SystemTray.ProgressIndicator.IsVisible = true;
            SystemTray.ProgressIndicator.Text = "Getting move from server";

            /// Remove hard-coded port number
            AsynchronousClient client = new AsynchronousClient(this.ServerName, this.PortNumber);
            client.ResponseReceived += new ResponseReceivedEventHandler(ac_GotMove);
            _waitingOnServerMove = true;

            client.SendData(boardState);

        }

        /// <summary>
        /// Callback for SendData call to server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">The result from the server</param>
        void ac_GotMove(object sender, ResponseReceivedEventArgs e)
        {
            if (e.isError)
            {
                ReportMoveError(e.response);
            }
            else
            {
                UnLockGameboard();
                ClearStatusText();

                // As its next move, the server sends back one value - the name of the textblock which
                // is its gamepiece to play That is all we need from the server to make the move on the board
                foreach (TextBlock tb in gBoard.Children.OfType<TextBlock>())
                {
                    if (tb.Name == e.response)
                    {
                        // Play thsi piece for the server
                        tb.Text = (this.PlayAsX) ? GAMEPIECE_0 : GAMEPIECE_X;
                        _pieceMap[tb.Name] = tb.Text;
                        break;
                    }
                }

                if (SomebodyWon)
                {
                    GameOver();
                }

            }
            _waitingOnServerMove = false;

        }

        /// <summary>
        /// Hard-coded game logic to keep things simple.
        /// There are 8 possible ways someone can get a winning line. 
        /// I check for each one here.
        /// </summary>
        /// <returns>Indicates where 'X' won or 'O' won</returns>
        private string DidSomeoneWin()
        {
            // Top Horizontal line
            if (!String.IsNullOrWhiteSpace(tb_00.Text) && tb_00.Text == tb_01.Text && tb_01.Text == tb_02.Text)
            {
                ShowWinningLine(tb_00, tb_01, tb_02);
                return tb_00.Text;
            }

            // Left Vertical Line
            if (!String.IsNullOrWhiteSpace(tb_00.Text) && tb_00.Text == tb_10.Text && tb_10.Text == tb_20.Text)
            {
                ShowWinningLine(tb_00, tb_10, tb_20);
                return tb_00.Text;
            }

            // Top Left to Bottom Right Line
            if (!String.IsNullOrWhiteSpace(tb_00.Text) && tb_00.Text == tb_11.Text && tb_11.Text == tb_22.Text)
            {
                ShowWinningLine(tb_00, tb_11, tb_22);
                return tb_00.Text;
            }

            // Bottom Left to Top Right Line
            if (!String.IsNullOrWhiteSpace(tb_02.Text) && tb_02.Text == tb_11.Text && tb_11.Text == tb_20.Text)
            {
                ShowWinningLine(tb_02, tb_11, tb_20);
                return tb_02.Text;
            }

            // Middle Vertical Line
            if (!String.IsNullOrWhiteSpace(tb_01.Text) && tb_01.Text == tb_11.Text && tb_11.Text == tb_21.Text)
            {
                ShowWinningLine(tb_01, tb_11, tb_21);
                return tb_01.Text;
            }

            // Right Vertical Line
            if (!String.IsNullOrWhiteSpace(tb_02.Text) && tb_02.Text == tb_12.Text && tb_12.Text == tb_22.Text)
            {
                ShowWinningLine(tb_02, tb_12, tb_22);
                return tb_02.Text;
            }

            // Middle Horizontal
            if (!String.IsNullOrWhiteSpace(tb_10.Text) && tb_10.Text == tb_11.Text && tb_11.Text == tb_12.Text)
            {
                ShowWinningLine(tb_10, tb_11, tb_12);
                return tb_10.Text;
            }

            // Bottom Horizontal
            if (!String.IsNullOrWhiteSpace(tb_20.Text) && tb_20.Text == tb_21.Text && tb_21.Text == tb_22.Text)
            {
                ShowWinningLine(tb_20, tb_21, tb_22);
                return tb_20.Text;
            }

            // Nobody won
            return string.Empty;

        }

        /// <summary>
        /// Return the game board state as a string 
        /// </summary>
        /// <returns>A string representing the board state</returns>
        /// <remarks>This serialized form of board state is sent to the server.
        /// It could also be used as the state to be saved during application lifetime 
        /// changes (dormant, tombstoned etc.)</remarks>
        private string GetBoardState()
        {
            StringBuilder sb = new StringBuilder();

            // First entry is the indication as to what piece the server is playing
            if (this.PlayAsX)
                sb.Append(GAMEPIECE_0);
            else
                sb.Append(GAMEPIECE_X);

            // gamepiece seperator
            sb.Append("|");

            // Add an indication of the current value of each gamepiece
            foreach (string key in _pieceMap.Keys)
            {
                sb.Append(key);

                // Key, Value seperator
                sb.Append("*");
                sb.Append(_pieceMap[key]);
                sb.Append("|");

            }

            // Remove the extra "|"
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        /// <summary>
        /// Report an error receive when attempting to get a game move from the server
        /// </summary>
        /// <param name="error"></param>
        private void ReportMoveError(string error)
        {
            if (System.Windows.Deployment.Current.Dispatcher.CheckAccess())
            {
                // Since I want to call the same logic here and in the Dispatcher case, 
                // I encapsulate it in a method they can both call.
                _reportMoveError(error);
            }
            else
            {
                System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    _reportMoveError(error);

                });
            }
        }

        private void _reportMoveError(string error)
        {

            UnLockGameboard();
            ClearStatusText();

            error = String.Format("Error: {0}", error) + Environment.NewLine + "Press OK to try again or Cancel to canel game";
            if (MessageBox.Show(error, "Error getting move from server", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {

                GetMoveFromServer();
            }
            else
            {
                NewGame();
            }
        }



        private void ClearStatusText()
        {
            UpdateStatus(String.Empty);
        }

        /// <summary>
        /// Add a little bling to the gameboard by showing the winning line in a different color
        /// </summary>
        /// <param name="w1">Winning piece 1</param>
        /// <param name="w2">Winning piece 2</param>
        /// <param name="w3">Winning piece 3</param>
        private void ShowWinningLine(TextBlock w1, TextBlock w2, TextBlock w3)
        {
            w1.Foreground = _greenBrush;
            w2.Foreground = _greenBrush;
            w3.Foreground = _greenBrush;
        }

        /// <summary>
        /// Lock the gameboard when the game is over
        /// </summary>
        private void GameOver()
        {
            LockGameboard();
        }

        private void LockGameboard()
        {
            gBoard.IsHitTestVisible = false;
        }

        private void UnLockGameboard()
        {
            gBoard.IsHitTestVisible = true;
        }

        private void appbarHideDiag_Click(object sender, EventArgs e)
        {
            ApplicationBarMenuItem toggle = (ApplicationBarMenuItem)sender;

            spDiagnostics.Visibility = (spDiagnostics.Visibility == System.Windows.Visibility.Visible) ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible;
            toggle.Text = (spDiagnostics.Visibility == System.Windows.Visibility.Visible) ? "Hide Diagnostics" : "Show Diagnostics";
        }


        private void appbarSettings_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/settings.xaml", UriKind.RelativeOrAbsolute));
        }

    }
}
