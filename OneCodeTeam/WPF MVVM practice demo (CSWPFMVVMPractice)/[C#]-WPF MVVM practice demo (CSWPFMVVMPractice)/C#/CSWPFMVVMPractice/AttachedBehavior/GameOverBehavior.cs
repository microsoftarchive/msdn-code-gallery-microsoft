/****************************** Module Header ******************************\
*   Module Name:  <GameOverBehavior.cs>
*   Project:	<CSWPFMVVMPractice>
*   Copyright (c) Microsoft Corporation.
* 
* The GameOverBehavior is an attached behavior that is used to report the result to the user
* when a game is over.
*  
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/  

using System;
using System.Windows;

namespace CSWPFMVVMPractice
{
    public class GameOverBehavior : DependencyObject
    {
        public static Nullable<bool> GetReportResult(DependencyObject obj)
        {
            return (Nullable<bool>)obj.GetValue(ReportResultProperty);
        }

        public static void SetReportResult(DependencyObject obj, Nullable<bool> value)
        {
            obj.SetValue(ReportResultProperty, value);
        }
               
        // Using a DependencyProperty as the backing store for ReportResult.  This enables 
        // animation, styling, binding, etc...        
        public static readonly DependencyProperty ReportResultProperty =
            DependencyProperty.RegisterAttached("ReportResult", typeof(Nullable<bool>), typeof(GameOverBehavior),
            new UIPropertyMetadata(null, (sender, e) => OnReportResultPropertyChanged(sender, e)));

        /// <summary>
        /// The ReportResultProperty's PropertyChangedCallback method
        /// In the method, get the TicTacToeViewModel instance and subscribe to its GameOverEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnReportResultPropertyChanged(DependencyObject sender, 
            DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement obj = sender as FrameworkElement;            
            if (obj.DataContext != null && obj.DataContext.GetType() == typeof(TicTacToeViewModel))
            {         
                // This ensures that the GameOverEvent is subscribed only once             
                (obj.DataContext as TicTacToeViewModel).GameOverEvent -= 
                    new TicTacToeViewModel.GameOverEventHandler(GameOverBehavior_GameOverEvent);
                (obj.DataContext as TicTacToeViewModel).GameOverEvent += 
                    new TicTacToeViewModel.GameOverEventHandler(GameOverBehavior_GameOverEvent);
            }
        }
        /// <summary>
        /// In the GameOver event handler, show the result of the current game via a MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void GameOverBehavior_GameOverEvent(object sender, TicTacToeViewModel.GameOverEventArgs e)
        {
            if (e.IsTie)
            {
                MessageBox.Show("No winner!", "Game Over");
            }
            else
            {
                MessageBox.Show(e.WinnerName + " has won, Congratulations!", "Game Over");
            }

        }
    }


}
