/****************************** Module Header ******************************\
*   Module Name:  <App.xaml.cs>
*   Project:	<CSWPFMVVMPractice>
* Copyright (c) Microsoft Corporation.
* 
*  The CSWPFMVVMPractice demo demonstrates how to implement the MVVM patten in a 
*  WPF application.
*  
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Windows;

namespace CSWPFMVVMPractice
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);         
            // Create a TicTacToeViewModel object with dimension of 3 and start a new game            
            TicTacToeViewModel game = new TicTacToeViewModel(3);
            game.NewGame();
            
            
            // Create an instance of the MainWindow and set the TicTacToeViewModel instance 
            // as the data context of the MainWindow            
            MainWindow win = new MainWindow();
            win.DataContext = game;

            // Set the attached GameoverBehavior.ReportResult property to true on the MainWindow            
            win.SetValue(GameOverBehavior.ReportResultProperty, true);
            
            // Show the MainWindow            
            win.Show();
        }
    }
}
