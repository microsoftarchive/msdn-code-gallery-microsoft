/****************************** Module Header ******************************\
Module Name:	MainWindow.xaml.cs
Project:	    CSWPFAnimatedGIF
Copyright (c) Microsoft Corporation.

The CSWPFAnimatedGIF demonstrates how to implement 
an animated GIF image in WPF.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Windows;

namespace CSWPFAnimatedGIF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// "Start" button click event handler, start to play the animation
        /// </summary>
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            GIFCtrl.StartAnimate();
        }

        /// <summary>
        /// "Stop" button click event handler, stop to play the animation
        /// </summary>
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            GIFCtrl.StopAnimate();
        }

    }
}
