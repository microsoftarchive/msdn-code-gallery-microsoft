/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Windows;

namespace PhoneVoIPApp.UI
{
    public partial class MainPage : BasePage
    {
        // Constructor
        public MainPage() : base(new MainViewModel())
        {
            InitializeComponent();
        }

        private void MakeOutgoingCallButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/OutgoingCallPage.xaml", UriKind.Relative));
        }

        private void SimulateIncomingCallButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/IncomingCallPage.xaml", UriKind.Relative));
        }

        private void ViewCallStatusButton_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.NavigateToCallStatusPage();
        }

        private void EmailPushUriButton_Click(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)this.ViewModel).EmailPushUri();
        }
    }
}
