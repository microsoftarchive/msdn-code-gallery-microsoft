//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using Windows.UI.Xaml;

namespace Navigation
{
    public sealed partial class Scenario2
    {
		// This Scenario shows how it is possible to Navigate to other page passing parameters
        public Scenario2()
        {
            this.InitializeComponent();
        }

        private void NavigateButtonClick(object sender, RoutedEventArgs e)
        {
			//We have created a class to serialize the set of parameters that we need to pass
			//the page.
            PageWithParametersConfiguration pageParameters = new PageWithParametersConfiguration();
            
            if (MessageTextBox.Text == String.Empty)
            {
                pageParameters.Message = "This Page was created on: " + DateTime.Now.ToLocalTime();
            }
            else
            {
                pageParameters.Message = MessageTextBox.Text;
            }

			//The second parameter of navigate method contains the parameters that 
			//will be passed to the page.
            MyFrame.Navigate(typeof(PageWithParameters), pageParameters);
        }
    }
}
