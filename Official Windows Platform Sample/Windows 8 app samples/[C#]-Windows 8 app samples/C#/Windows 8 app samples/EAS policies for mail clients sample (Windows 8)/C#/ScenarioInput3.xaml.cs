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
using Windows.Security.Authentication.Web;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using System.Text;
using System.Net;
using System.IO;
using SDKTemplateCS;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace SDKTemplateCS
{
	public sealed partial class ScenarioInput3 : Page
	{
        
		// A pointer back to the main page which is used to gain access to the input and output frames and their content.
		MainPage rootPage = null;        

		public ScenarioInput3()
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
            Page outputFrame = (Page)rootPage.OutputFrame.Content;
            TextBox Scenario2DebugArea = outputFrame.FindName("Scenario2DebugArea") as TextBox;
            Scenario2DebugArea.Text += Trace + "\r\n";
        }

        private async void Launch_Click2(object sender, RoutedEventArgs e)
        {
            try
            {
                Page outputFrame = (Page)rootPage.OutputFrame.Content;
                EasClientSecurityPolicy RequestedPolicy = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientSecurityPolicy();
                
                CheckBox RequireEncryptionValue = outputFrame.FindName("RequireEncryptionValue") as CheckBox;
                if (RequireEncryptionValue == null)
                    RequestedPolicy.RequireEncryption = false;
                else
                {
                    if (RequireEncryptionValue.IsChecked == true)
                        RequestedPolicy.RequireEncryption = true;
                    else
                        RequestedPolicy.RequireEncryption = false;
                }

                TextBox MinPasswordLengthValue = outputFrame.FindName("MinPasswordLengthValue") as TextBox;
                if (MinPasswordLengthValue == null || MinPasswordLengthValue.Text.Length == 0)
                    RequestedPolicy.MinPasswordLength = 0;
                else
                {
                    RequestedPolicy.MinPasswordLength = Convert.ToByte(MinPasswordLengthValue.Text);
                }

                CheckBox DisallowConvenienceLogonValue = outputFrame.FindName("DisallowConvenienceLogonValue") as CheckBox;
                if (DisallowConvenienceLogonValue == null)
                    RequestedPolicy.DisallowConvenienceLogon = false;
                else
                {
                    if (DisallowConvenienceLogonValue.IsChecked == true)
                        RequestedPolicy.DisallowConvenienceLogon = true;
                    else
                        RequestedPolicy.DisallowConvenienceLogon = false;
                }

                TextBox MinPasswordComplexCharactersValue = outputFrame.FindName("MinPasswordComplexCharactersValue") as TextBox;
                if (MinPasswordComplexCharactersValue == null || MinPasswordComplexCharactersValue.Text.Length == 0)
                    RequestedPolicy.MinPasswordComplexCharacters = 0;
                else
                {
                    RequestedPolicy.MinPasswordComplexCharacters = Convert.ToByte(MinPasswordComplexCharactersValue.Text);
                }

                TextBox PasswordExpirationValue = outputFrame.FindName("PasswordExpirationValue") as TextBox;
                if (PasswordExpirationValue == null || PasswordExpirationValue.Text.Length == 0)
                    RequestedPolicy.PasswordExpiration = TimeSpan.Parse("0");
                else
                {
                    RequestedPolicy.PasswordExpiration = TimeSpan.FromDays(Convert.ToDouble(PasswordExpirationValue.Text));
                }

                TextBox PasswordHistoryValue = outputFrame.FindName("PasswordHistoryValue") as TextBox;
                if (PasswordHistoryValue == null || PasswordHistoryValue.Text.Length == 0)
                    RequestedPolicy.PasswordHistory = 0;
                else
                {
                    RequestedPolicy.PasswordHistory = Convert.ToByte(PasswordHistoryValue.Text);
                }

                TextBox MaxPasswordFailedAttemptsValue = outputFrame.FindName("MaxPasswordFailedAttemptsValue") as TextBox;
                if (MaxPasswordFailedAttemptsValue == null || MaxPasswordFailedAttemptsValue.Text.Length == 0)
                    RequestedPolicy.MaxPasswordFailedAttempts = 0;
                else
                {
                    RequestedPolicy.MaxPasswordFailedAttempts = Convert.ToByte(MaxPasswordFailedAttemptsValue.Text);
                }

                TextBox MaxInactivityTimeLockValue = outputFrame.FindName("MaxInactivityTimeLockValue") as TextBox;
                if (MaxInactivityTimeLockValue == null || MaxInactivityTimeLockValue.Text.Length == 0)
                    RequestedPolicy.MaxInactivityTimeLock = TimeSpan.Parse("0");
                else
                {
                    RequestedPolicy.MaxInactivityTimeLock = TimeSpan.FromSeconds(Convert.ToDouble(MaxInactivityTimeLockValue.Text));
                }

                Windows.Security.ExchangeActiveSyncProvisioning.EasComplianceResults ApplyResult = await RequestedPolicy.ApplyAsync();



                TextBox RequireEncryptionResult = outputFrame.FindName("RequireEncryptionResult") as TextBox;
                RequireEncryptionResult.Text = ApplyResult.RequireEncryptionResult.ToString();

                TextBox MinPasswordLengthResult = outputFrame.FindName("MinPasswordLengthResult") as TextBox;
                MinPasswordLengthResult.Text = ApplyResult.MinPasswordLengthResult.ToString();

                TextBox DisallowConvenienceLogonResult = outputFrame.FindName("DisallowConvenienceLogonResult") as TextBox;
                DisallowConvenienceLogonResult.Text = ApplyResult.DisallowConvenienceLogonResult.ToString();

                TextBox MinPasswordComplexCharactersResult = outputFrame.FindName("MinPasswordComplexCharactersResult") as TextBox;
                MinPasswordComplexCharactersResult.Text = ApplyResult.MinPasswordComplexCharactersResult.ToString();

                TextBox PasswordExpirationResult = outputFrame.FindName("PasswordExpirationResult") as TextBox;
                PasswordExpirationResult.Text = ApplyResult.PasswordExpirationResult.ToString();

                TextBox PasswordHistoryResult = outputFrame.FindName("PasswordHistoryResult") as TextBox;
                PasswordHistoryResult.Text = ApplyResult.PasswordHistoryResult.ToString();

                TextBox MaxPasswordFailedAttemptsResult = outputFrame.FindName("MaxPasswordFailedAttemptsResult") as TextBox;
                MaxPasswordFailedAttemptsResult.Text = ApplyResult.MaxPasswordFailedAttemptsResult.ToString();

                TextBox MaxInactivityTimeLockResult = outputFrame.FindName("MaxInactivityTimeLockResult") as TextBox;
                MaxInactivityTimeLockResult.Text = ApplyResult.MaxInactivityTimeLockResult.ToString();
            }
            catch (Exception Error)
            {
                //
                // Bad Parameter, COM Unavailable errors are to be handled here.
                //
                DebugPrint(Error.ToString());
            }

        }

        private void Reset_Click2(object sender, RoutedEventArgs e)
        {
            try
            {
                Page outputFrame = (Page)rootPage.OutputFrame.Content;

                CheckBox RequireEncryptionValue = outputFrame.FindName("RequireEncryptionValue") as CheckBox;
                RequireEncryptionValue.IsChecked = false;

                TextBox MinPasswordLengthValue = outputFrame.FindName("MinPasswordLengthValue") as TextBox;
                MinPasswordLengthValue.Text = "";

                CheckBox DisallowConvenienceLogonValue = outputFrame.FindName("DisallowConvenienceLogonValue") as CheckBox;
                DisallowConvenienceLogonValue.IsChecked = false;

                TextBox MinPasswordComplexCharactersValue = outputFrame.FindName("MinPasswordComplexCharactersValue") as TextBox;
                MinPasswordComplexCharactersValue.Text = "";

                TextBox PasswordExpirationValue = outputFrame.FindName("PasswordExpirationValue") as TextBox;
                PasswordExpirationValue.Text = "";

                TextBox PasswordHistoryValue = outputFrame.FindName("PasswordHistoryValue") as TextBox;
                PasswordHistoryValue.Text = "";

                TextBox MaxPasswordFailedAttemptsValue = outputFrame.FindName("MaxPasswordFailedAttemptsValue") as TextBox;
                MaxPasswordFailedAttemptsValue.Text = "";

                TextBox MaxInactivityTimeLockValue = outputFrame.FindName("MaxInactivityTimeLockValue") as TextBox;
                MaxInactivityTimeLockValue.Text = "";

                TextBox RequireEncryptionResult = outputFrame.FindName("RequireEncryptionResult") as TextBox;
                RequireEncryptionResult.Text = "";

                TextBox MinPasswordLengthResult = outputFrame.FindName("MinPasswordLengthResult") as TextBox;
                MinPasswordLengthResult.Text = "";

                TextBox DisallowConvenienceLogonResult = outputFrame.FindName("DisallowConvenienceLogonResult") as TextBox;
                DisallowConvenienceLogonResult.Text = "";

                TextBox MinPasswordComplexCharactersResult = outputFrame.FindName("MinPasswordComplexCharactersResult") as TextBox;
                MinPasswordComplexCharactersResult.Text = "";

                TextBox PasswordExpirationResult = outputFrame.FindName("PasswordExpirationResult") as TextBox;
                PasswordExpirationResult.Text = "";

                TextBox PasswordHistoryResult = outputFrame.FindName("PasswordHistoryResult") as TextBox;
                PasswordHistoryResult.Text = "";

                TextBox MaxPasswordFailedAttemptsResult = outputFrame.FindName("MaxPasswordFailedAttemptsResult") as TextBox;
                MaxPasswordFailedAttemptsResult.Text = "";

                TextBox MaxInactivityTimeLockResult = outputFrame.FindName("MaxInactivityTimeLockResult") as TextBox;
                MaxInactivityTimeLockResult.Text = "";
            }
            catch (Exception Error)
            {
                //
                // Bad Parameter, COM Unavailable errors are to be handled here.
                //
                DebugPrint(Error.ToString());
            }

        }

/*
        public EasComplianceResults ApplyResult { get; set; }*/
    }
}
