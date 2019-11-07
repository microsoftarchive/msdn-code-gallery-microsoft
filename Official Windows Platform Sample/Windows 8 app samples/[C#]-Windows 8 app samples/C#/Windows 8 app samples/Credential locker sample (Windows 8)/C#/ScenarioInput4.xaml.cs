// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SDKTemplateCS;
using Windows.Security.Credentials;

namespace SDKTemplateCS
{
	public sealed partial class ScenarioInput4 : Page
	{
		// A pointer back to the main page which is used to gain access to the input and output frames and their content.
		MainPage rootPage = null;

		public ScenarioInput4()
		{
			InitializeComponent();
		}

		#region Template-Related Code - Do not remove
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			// Get a pointer to our main page
			rootPage = e.Parameter as MainPage;

			// We want to be notified with the OutputFrame is loaded so we can get to the content.
			rootPage.OutputFrameLoaded += new System.EventHandler(rootPage_OutputFrameLoaded);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			rootPage.OutputFrameLoaded -= new System.EventHandler(rootPage_OutputFrameLoaded);
		}

		#endregion

        #region Use this code if you need access to elements in the output frame - otherwise delete
        void rootPage_OutputFrameLoaded(object sender, object e)
        {
            // At this point, we know that the Output Frame has been loaded and we can go ahead
            // and reference elements in the page contained in the Output Frame.

            // Get a pointer to the content within the OutputFrame.
            Page outputFrame = (Page)rootPage.OutputFrame.Content;

            // Go find the elements that we need for this scenario.
            // ex: flipView1 = outputFrame.FindName("FlipView1") as FlipView;

        }
        #endregion

		private void DebugPrint(String Trace)
        {
            Page inputFrame = (Page)rootPage.InputFrame.Content;
            TextBox DeleteSummary = inputFrame.FindName("DeleteSummary") as TextBox;
            DeleteSummary.Text = Trace + "\r\n";
        }


        void Scenario4Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Windows.Security.Credentials.PasswordVault v = new Windows.Security.Credentials.PasswordVault();
                IReadOnlyList<PasswordCredential> creds = v.RetrieveAll();
                DeleteSummary.Text = "Number of credentials deleted: " + creds.Count;
                foreach (PasswordCredential c in creds)
                {
                    v.Remove(c);
                }
                // GetAll is a snapshot in time, so to reflect the updated vault, get all credentials again
                creds = v.RetrieveAll();
                // The credentials should now be empty

            }
            catch (Exception Error)
            {
                DebugPrint(Error.ToString());
            }
        }
	}
}
