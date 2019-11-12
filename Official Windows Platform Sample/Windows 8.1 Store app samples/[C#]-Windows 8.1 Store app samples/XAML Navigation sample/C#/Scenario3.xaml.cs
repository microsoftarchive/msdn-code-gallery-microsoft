//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;

namespace Navigation
{
    public sealed partial class Scenario3
    {
		// This scenario shows how navigation can be cancelled
        public Scenario3()
        {
            this.InitializeComponent();
        }
        
        private void NavigateButtonClick(object sender, RoutedEventArgs e)
        {
			// CancelNavigationPage will implement the logic to cancel a navigation
            MyFrame.Navigate(typeof(CancelNavigationPage));
        }
    }
}
