//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace EAS
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario2 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario2()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void DebugPrint(String Trace)
        {
            TextBox Scenario2DebugArea = rootPage.FindName("Scenario2DebugArea") as TextBox;
            Scenario2DebugArea.Text += Trace + "\r\n";
        }

        private void Launch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                EasClientSecurityPolicy RequestedPolicy = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientSecurityPolicy();
                EasComplianceResults ComplianceResult = null;


                CheckBox RequireEncryptionValue = rootPage.FindName("RequireEncryptionValue") as CheckBox;
                if (RequireEncryptionValue == null)
                    RequestedPolicy.RequireEncryption = false;
                else
                {
                    if (RequireEncryptionValue.IsChecked == true)
                        RequestedPolicy.RequireEncryption = true;
                    else
                        RequestedPolicy.RequireEncryption = false;
                }

                TextBox MinPasswordLengthValue = rootPage.FindName("MinPasswordLengthValue") as TextBox;
                if (MinPasswordLengthValue == null || MinPasswordLengthValue.Text.Length == 0)
                    RequestedPolicy.MinPasswordLength = 0;
                else
                {
                    RequestedPolicy.MinPasswordLength = Convert.ToByte(MinPasswordLengthValue.Text);
                }

                CheckBox DisallowConvenienceLogonValue = rootPage.FindName("DisallowConvenienceLogonValue") as CheckBox;
                if (DisallowConvenienceLogonValue == null)
                    RequestedPolicy.DisallowConvenienceLogon = false;
                else
                {
                    if (DisallowConvenienceLogonValue.IsChecked == true)
                        RequestedPolicy.DisallowConvenienceLogon = true;
                    else
                        RequestedPolicy.DisallowConvenienceLogon = false;
                }

                TextBox MinPasswordComplexCharactersValue = rootPage.FindName("MinPasswordComplexCharactersValue") as TextBox;
                if (MinPasswordComplexCharactersValue == null || MinPasswordComplexCharactersValue.Text.Length == 0)
                    RequestedPolicy.MinPasswordComplexCharacters = 0;
                else
                {
                    RequestedPolicy.MinPasswordComplexCharacters = Convert.ToByte(MinPasswordComplexCharactersValue.Text);
                }

                TextBox PasswordExpirationValue = rootPage.FindName("PasswordExpirationValue") as TextBox;
                if (PasswordExpirationValue == null || PasswordExpirationValue.Text.Length == 0)
                    RequestedPolicy.PasswordExpiration = TimeSpan.Parse("0");
                else
                {
                    RequestedPolicy.PasswordExpiration = TimeSpan.FromDays(Convert.ToDouble(PasswordExpirationValue.Text));
                }

                TextBox PasswordHistoryValue = rootPage.FindName("PasswordHistoryValue") as TextBox;
                if (PasswordHistoryValue == null || PasswordHistoryValue.Text.Length == 0)
                    RequestedPolicy.PasswordHistory = 0;
                else
                {
                    RequestedPolicy.PasswordHistory = Convert.ToByte(PasswordHistoryValue.Text);
                }

                TextBox MaxPasswordFailedAttemptsValue = rootPage.FindName("MaxPasswordFailedAttemptsValue") as TextBox;
                if (MaxPasswordFailedAttemptsValue == null || MaxPasswordFailedAttemptsValue.Text.Length == 0)
                    RequestedPolicy.MaxPasswordFailedAttempts = 0;
                else
                {
                    RequestedPolicy.MaxPasswordFailedAttempts = Convert.ToByte(MaxPasswordFailedAttemptsValue.Text);
                }

                TextBox MaxInactivityTimeLockValue = rootPage.FindName("MaxInactivityTimeLockValue") as TextBox;
                if (MaxInactivityTimeLockValue == null || MaxInactivityTimeLockValue.Text.Length == 0)
                    RequestedPolicy.MaxInactivityTimeLock = TimeSpan.Parse("0");
                else
                {
                    RequestedPolicy.MaxInactivityTimeLock = TimeSpan.FromSeconds(Convert.ToDouble(MaxInactivityTimeLockValue.Text));
                }

                ComplianceResult = RequestedPolicy.CheckCompliance();

                TextBox RequireEncryptionResult = rootPage.FindName("RequireEncryptionResult") as TextBox;
                RequireEncryptionResult.Text = ComplianceResult.RequireEncryptionResult.ToString();

                TextBox EncryptionProviderTypeResult = rootPage.FindName("EncryptionProviderTypeResult") as TextBox;
                EncryptionProviderTypeResult.Text = ComplianceResult.EncryptionProviderType.ToString();

                TextBox MinPasswordLengthResult = rootPage.FindName("MinPasswordLengthResult") as TextBox;
                MinPasswordLengthResult.Text = ComplianceResult.MinPasswordLengthResult.ToString();

                TextBox DisallowConvenienceLogonResult = rootPage.FindName("DisallowConvenienceLogonResult") as TextBox;
                DisallowConvenienceLogonResult.Text = ComplianceResult.DisallowConvenienceLogonResult.ToString();

                TextBox MinPasswordComplexCharactersResult = rootPage.FindName("MinPasswordComplexCharactersResult") as TextBox;
                MinPasswordComplexCharactersResult.Text = ComplianceResult.MinPasswordComplexCharactersResult.ToString();

                TextBox PasswordExpirationResult = rootPage.FindName("PasswordExpirationResult") as TextBox;
                PasswordExpirationResult.Text = ComplianceResult.PasswordExpirationResult.ToString();

                TextBox PasswordHistoryResult = rootPage.FindName("PasswordHistoryResult") as TextBox;
                PasswordHistoryResult.Text = ComplianceResult.PasswordHistoryResult.ToString();

                TextBox MaxPasswordFailedAttemptsResult = rootPage.FindName("MaxPasswordFailedAttemptsResult") as TextBox;
                MaxPasswordFailedAttemptsResult.Text = ComplianceResult.MaxPasswordFailedAttemptsResult.ToString();

                TextBox MaxInactivityTimeLockResult = rootPage.FindName("MaxInactivityTimeLockResult") as TextBox;
                MaxInactivityTimeLockResult.Text = ComplianceResult.MaxInactivityTimeLockResult.ToString();
            }
            catch (Exception Error)
            {
                //
                // Bad Parameter, COM Unavailable errors are to be handled here.
                //
                DebugPrint(Error.ToString());
            }

        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                

                CheckBox RequireEncryptionValue = rootPage.FindName("RequireEncryptionValue") as CheckBox;
                RequireEncryptionValue.IsChecked = false;

                TextBox MinPasswordLengthValue = rootPage.FindName("MinPasswordLengthValue") as TextBox;
                MinPasswordLengthValue.Text = "";

                CheckBox DisallowConvenienceLogonValue = rootPage.FindName("DisallowConvenienceLogonValue") as CheckBox;
                DisallowConvenienceLogonValue.IsChecked = false;

                TextBox MinPasswordComplexCharactersValue = rootPage.FindName("MinPasswordComplexCharactersValue") as TextBox;
                MinPasswordComplexCharactersValue.Text = "";

                TextBox PasswordExpirationValue = rootPage.FindName("PasswordExpirationValue") as TextBox;
                PasswordExpirationValue.Text = "";

                TextBox PasswordHistoryValue = rootPage.FindName("PasswordHistoryValue") as TextBox;
                PasswordHistoryValue.Text = "";

                TextBox MaxPasswordFailedAttemptsValue = rootPage.FindName("MaxPasswordFailedAttemptsValue") as TextBox;
                MaxPasswordFailedAttemptsValue.Text = "";

                TextBox MaxInactivityTimeLockValue = rootPage.FindName("MaxInactivityTimeLockValue") as TextBox;
                MaxInactivityTimeLockValue.Text = "";

                TextBox RequireEncryptionResult = rootPage.FindName("RequireEncryptionResult") as TextBox;
                RequireEncryptionResult.Text = "";

                TextBox EncryptionProviderTypeResult = rootPage.FindName("EncryptionProviderTypeResult") as TextBox;
                EncryptionProviderTypeResult.Text = "";

                TextBox MinPasswordLengthResult = rootPage.FindName("MinPasswordLengthResult") as TextBox;
                MinPasswordLengthResult.Text = "";

                TextBox DisallowConvenienceLogonResult = rootPage.FindName("DisallowConvenienceLogonResult") as TextBox;
                DisallowConvenienceLogonResult.Text = "";

                TextBox MinPasswordComplexCharactersResult = rootPage.FindName("MinPasswordComplexCharactersResult") as TextBox;
                MinPasswordComplexCharactersResult.Text = "";

                TextBox PasswordExpirationResult = rootPage.FindName("PasswordExpirationResult") as TextBox;
                PasswordExpirationResult.Text = "";

                TextBox PasswordHistoryResult = rootPage.FindName("PasswordHistoryResult") as TextBox;
                PasswordHistoryResult.Text = "";

                TextBox MaxPasswordFailedAttemptsResult = rootPage.FindName("MaxPasswordFailedAttemptsResult") as TextBox;
                MaxPasswordFailedAttemptsResult.Text = "";

                TextBox MaxInactivityTimeLockResult = rootPage.FindName("MaxInactivityTimeLockResult") as TextBox;
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
    }
}
