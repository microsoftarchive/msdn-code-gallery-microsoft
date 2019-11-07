/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using Microsoft.Phone.Controls;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
namespace PhoneVoIPApp.UI
{
    public partial class CallStatusPage : BasePage
    {
        public CallStatusPage()
            : base(new CallStatusViewModel())
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs nee)
        {
            base.OnNavigatedTo(nee);
            // Register for the Obscured/Unobscured events
            PhoneApplicationFrame rootFrame = ((App)Application.Current).RootFrame;
            rootFrame.Obscured += ((CallStatusViewModel)this.ViewModel).RootFrame_Obscured;
            rootFrame.Unobscured += ((CallStatusViewModel)this.ViewModel).RootFrame_Unobscured;

            // Re-bind MediaElements explictly, so video will play after app has been resumed
            bigHead.SetBinding(MediaElement.SourceProperty, new System.Windows.Data.Binding("BigHeadPreviewUri"));
            littleHead.SetBinding(MediaElement.SourceProperty, new System.Windows.Data.Binding("LittleHeadPreviewUri"));
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs nee)
        {
            base.OnNavigatedFrom(nee);
            // Unregister for the Obscured/Unobscured events
            PhoneApplicationFrame rootFrame = ((App)Application.Current).RootFrame;
            rootFrame.Obscured -= ((CallStatusViewModel)this.ViewModel).RootFrame_Obscured;
            rootFrame.Unobscured -= ((CallStatusViewModel)this.ViewModel).RootFrame_Unobscured;
        }

        private void HangUpButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((CallStatusViewModel)this.ViewModel).HangUp();
        }

        private void HoldButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((CallStatusViewModel)this.ViewModel).ToggleHold();
        }

        private void CameraToggleButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((CallStatusViewModel)this.ViewModel).ToggleCamera();
        }

        private void EarpieceButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((CallStatusViewModel)this.ViewModel).SetAudioRoute(BackEnd.CallAudioRoute.Earpiece);
        }

        private void SpeakerButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((CallStatusViewModel)this.ViewModel).SetAudioRoute(BackEnd.CallAudioRoute.Speakerphone);
        }

        private void BluetoothButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            ((CallStatusViewModel)this.ViewModel).SetAudioRoute(BackEnd.CallAudioRoute.Bluetooth);
        }

        private void littleHead_MediaOpened_1(object sender, System.Windows.RoutedEventArgs e)
        {
            Debug.WriteLine("[CallStatusPage] LittleHead Opened: " + ((MediaElement)sender).Source.AbsoluteUri);            
        }

        private void bigHead_MediaOpened_1(object sender, System.Windows.RoutedEventArgs e)
        {
            Debug.WriteLine("[CallStatusPage] BigHead Opened: " + ((MediaElement)sender).Source.AbsoluteUri);
        }

        private void bigHead_MediaFailed_1(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine("[CallStatusPage] BigHead Failed: " + e.ErrorException.Message);
        }

        private void littleHead_MediaFailed_1(object sender, System.Windows.ExceptionRoutedEventArgs e)
        {
            Debug.WriteLine("[CallStatusPage] LittleHead Failed: " + e.ErrorException.Message);
        }
    }
}
