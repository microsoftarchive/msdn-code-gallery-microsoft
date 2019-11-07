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
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;

namespace CertificateEnrollment
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImportPfx : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        string pfxCertificate = null;
        string pfxPassword = "";

        public ImportPfx()
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
        private async void RunSample_Click(object sender, RoutedEventArgs e)
        {
            CheckBox storeSelectionCheckbox = rootPage.FindName("UserStoreCheckBox") as CheckBox;
            TextBlock outputTextBlock = rootPage.FindName("OutputTextBlock") as TextBlock;
            PasswordBox pfxPasswordBox = rootPage.FindName("PfxPasswordBox") as PasswordBox;

            if (String.IsNullOrEmpty(pfxCertificate))
            {
                outputTextBlock.Text = "Please select a valid PFX file\n";
                return;
            }

            try
            {
                // Import PFX
                outputTextBlock.Text = "Importing PFX certificate ...";

                string friendlyName = "test pfx certificate";
                pfxPassword = pfxPasswordBox.Password;

                if (true == storeSelectionCheckbox.IsChecked)
                {
                    // target store is User's Certificate Store
                    // call User Certificate Enrollment function importPfxData to install the certificate
                    await CertificateEnrollmentManager.UserCertificateEnrollmentManager.ImportPfxDataAsync(
                        pfxCertificate,
                        pfxPassword,
                        ExportOption.NotExportable,
                        KeyProtectionLevel.NoConsent,
                        InstallOptions.None,
                        friendlyName);

                    outputTextBlock.Text += "\nCertificate installation succeeded. The certificate is in the User's certificate store";
                }
                else
                {
                    // target store is App's certificate store
                    // call Certificate Enrollment function importPFXData to install the certificate
                    await CertificateEnrollmentManager.ImportPfxDataAsync(
                        pfxCertificate,
                        pfxPassword,
                        ExportOption.NotExportable,
                        KeyProtectionLevel.NoConsent,
                        InstallOptions.None,
                        friendlyName);

                    outputTextBlock.Text += "\nCertificate installation succeeded. The certificate is in the App's certificate store";
                }
            }
            catch (Exception ex)
            {
                outputTextBlock.Text += "\nCertificate installation failed with error: " + ex.ToString();
            }
        }

        /// <summary>
        /// Browse filesystem for PFX file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Browse_Click(object sender, RoutedEventArgs e)
        {
            // create FileOpen picker with filter .pfx
            FileOpenPicker filePicker = new FileOpenPicker();
            filePicker.FileTypeFilter.Add(".pfx");
            filePicker.CommitButtonText = "Open";

            TextBlock outputTextBlock = rootPage.FindName("OutputTextBlock") as TextBlock;

            try
            {
                StorageFile file = await filePicker.PickSingleFileAsync();
                if (file != null)
                {
                    // file was picked and is available for read
                    // try to read the file content
                    IBuffer buffer = await FileIO.ReadBufferAsync(file);
                    using (DataReader dataReader = DataReader.FromBuffer(buffer))
                    {
                        byte[] bytes = new byte[buffer.Length];
                        dataReader.ReadBytes(bytes);
                        // convert to Base64 for using with ImportPfx
                        pfxCertificate = System.Convert.ToBase64String(bytes);
                    }
                    
                    // update UI elements
                    TextBox pfxFileName = rootPage.FindName("PfxFileName") as TextBox;
                    StackPanel spPassword = rootPage.FindName("PasswordPanel") as StackPanel;
                    StackPanel spImport = rootPage.FindName("ImportPanel") as StackPanel;
                    
                    PfxFileName.Text = file.Path;
                    spPassword.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    spImport.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                outputTextBlock.Text += "\nPFX file selection failed with error: " + ex.ToString();
            }
        }
    }
}
