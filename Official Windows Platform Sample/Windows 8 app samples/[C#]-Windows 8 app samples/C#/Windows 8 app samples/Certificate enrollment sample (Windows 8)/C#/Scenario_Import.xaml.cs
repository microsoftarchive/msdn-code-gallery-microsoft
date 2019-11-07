//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Windows.Security.Cryptography.Certificates;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace CertificateEnrollment
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario_Import : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario_Import()
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


        /// <summary>
        /// Import an existing pfx certificate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ImportPfx_Click(object sender, RoutedEventArgs e)
        {
            TextBlock outputTextBlock = rootPage.FindName("OutputTextBlock") as TextBlock;

            try
            {
                outputTextBlock.Text = "Importing PFX certificate ...";

                // Load the pfx certificate from resource string.
                ResourceLoader rl = new ResourceLoader();
                string pfxCertificate = rl.GetString("Certificate");

                string password = "sampletest";     //password to access the certificate in PFX format
                string friendlyName = "test pfx certificate";
                
                //call Certificate Enrollment funciton importPFXData to install the certificate
                await CertificateEnrollmentManager.ImportPfxDataAsync(pfxCertificate,
                    password,
                    ExportOption.NotExportable,
                    KeyProtectionLevel.NoConsent,
                    InstallOptions.None,
                    friendlyName);

                outputTextBlock.Text += "\nCertificate installation succeeded. The certificate is in the appcontainer Personal certificate store";

            }
            catch (Exception ex)
            {
                outputTextBlock.Text += "\nCertificate installation failed with error: " + ex.ToString();
            }
        }

    }
}
