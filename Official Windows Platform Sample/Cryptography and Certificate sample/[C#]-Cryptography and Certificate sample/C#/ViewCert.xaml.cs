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
using System.Net;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using Windows.Data.Xml.Dom;
using System.Collections.Generic;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace CertificateEnrollment
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewCert : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        string verifytext = string.Empty;
        IReadOnlyList<Certificate> certList;
        
        //string certificateRequest;

        public ViewCert()
        {
            this.InitializeComponent();
            EnumerateCertificateList();
            EnumerateVerifyList();
      
        }

        private void EnumerateVerifyList()
        {
            VerifyCert.Items.Clear();
            VerifyCert.Items.Add("Verify Certificate");
            VerifyCert.Items.Add("Sign/Verify using certificate key");
            VerifyCert.Items.Add("Sign/Verify using CMS based format");
            VerifyCert.Items.Add("Get certificate and show details");
            VerifyCert.SelectedIndex = 0;
        }

        private void EnumerateCertificateList()
        {
            CertificateList.Visibility = Visibility.Visible;
            CertificateList.Items.Clear();
            var task = CertificateStores.FindAllAsync();
            task.AsTask().Wait();
            var certlist = task.GetResults();
            LoadCertList(certlist);
            
            if (CertificateList.Items.Count == 0)
            {
                VerifyCert.IsEnabled = false;
                RunSample.IsEnabled = false;
                CertificateList.Items.Add("No certificates found");
            }
            else
            {
                VerifyCert.IsEnabled = true;
                RunSample.IsEnabled = true;
            }
            CertificateList.SelectedIndex = 0;
        }

        public void LoadCertList(IReadOnlyList<Certificate> certificateList)
        {
            this.certList = certificateList;
            this.CertificateList.Items.Clear();

            for (int i = 0; i < certList.Count; i++)
            {
                this.CertificateList.Items.Add(certList[i].Subject);
            }
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
        /// This is the click handler for the 'RunSample' button.  It is responsible for executing the sample code.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void RunSample_Click(object sender, RoutedEventArgs e)
        {
            Certificate selectedCertificate = null;
            string verifyselection = VerifyCert.SelectionBoxItem.ToString();

            //get the selected certificate
            if (CertificateList.SelectedIndex >= 0 && CertificateList.SelectedIndex < certList.Count)
            {
                selectedCertificate = certList[CertificateList.SelectedIndex];
            }

            if (selectedCertificate == null)
            {
                ViewCertText.Text = "Please select a certificate first.";
                return;
            }

            // a certificate was selected, do the desired operation
            if (verifyselection.Equals("Verify Certificate"))
            {
                //Build the chain
                var chain = await selectedCertificate.BuildChainAsync(null, null);
                //Validate the chain
                var result = chain.Validate();
                verifytext = "\n Verification Result :" + result.ToString();
               
            }
            else if (verifyselection.Equals("Sign/Verify using certificate key"))
            {
                // get private key
                CryptographicKey keyPair = await PersistedKeyProvider.OpenKeyPairFromCertificateAsync(selectedCertificate, HashAlgorithmNames.Sha1, CryptographicPadding.RsaPkcs1V15);
                String cookie = "Some Data to sign";
                IBuffer Data = CryptographicBuffer.ConvertStringToBinary(cookie, BinaryStringEncoding.Utf16BE);

                try
                {
                    //sign the data by using the key
                    IBuffer Signed = CryptographicEngine.Sign(keyPair, Data);
                    bool bresult = CryptographicEngine.VerifySignature(keyPair, Data, Signed);

                    if (bresult == true)
                    {
                        verifytext = "\n Verification Result : Successfully signed and verified signature";
                    }
                    else
                    {
                        verifytext = "\n Verification Result : Verify Signature Failed";
                    }
                }
                catch (Exception exp)
                {
                    verifytext = "\n Verification Failed. Exception Occurred :" + exp.Message;
                }
            }
            else if (verifyselection.Equals("Sign/Verify using CMS based format"))
            {
                IInputStream pdfInputstream;
                InMemoryRandomAccessStream originalData = new InMemoryRandomAccessStream();
                //Populate the new memory stream              
                pdfInputstream = originalData.GetInputStreamAt(0);
                CmsSignerInfo signer = new CmsSignerInfo();
                signer.Certificate = selectedCertificate;
                signer.HashAlgorithmName = HashAlgorithmNames.Sha1;
                IList<CmsSignerInfo> signers = new List<CmsSignerInfo>();
               
                signers.Add(signer);
                try
                {
                   IBuffer signature = await CmsDetachedSignature.GenerateSignatureAsync(pdfInputstream, signers, null);
                   CmsDetachedSignature cmsSignedData = new CmsDetachedSignature(signature);
                   pdfInputstream = originalData.GetInputStreamAt(0);
                   SignatureValidationResult validationResult = await cmsSignedData.VerifySignatureAsync(pdfInputstream);
                   if (SignatureValidationResult.Success == validationResult)
                    {
                        verifytext = "\n Verification Result : Successfully signed and verified Signature";
                    }
                   else
                    {
                        verifytext = "\n Verification Result : Verify Signature using CMS based format Failed";
                    }
                }
                catch (Exception exp)
                {
                    verifytext = "\n Verification Failed. Exception Occurred :" + exp.Message;
                }

            }
            else if(verifyselection.Equals("Get certificate and show details"))
            {
                DisplayCertificate(selectedCertificate);
            }
            
            ViewCertText.Text += verifytext;
            verifytext = string.Empty;
        }

        /// <summary>
        /// This function displays the selected Certificate details
        /// </summary>
        private void DisplayCertificate(Certificate selectedcertificate)
        {
            //this function displays the certificate details
            ViewCertText.Text = " ";
            ViewCertText.Text += "\n Certificate Selected :";
            ViewCertText.Text += "\n";
            ViewCertText.Text += "\n Subject : " + selectedcertificate.Subject;
            ViewCertText.Text += "\n Issuer  : " + selectedcertificate.Issuer;
            ViewCertText.Text += "\n Friendly Name : " + selectedcertificate.FriendlyName;
            ViewCertText.Text += "\n Thumbprint : " + CryptographicBuffer.EncodeToHexString(CryptographicBuffer.CreateFromByteArray(selectedcertificate.GetHashValue()));
            ViewCertText.Text += "\n Serial Number : " + CryptographicBuffer.EncodeToHexString(CryptographicBuffer.CreateFromByteArray(selectedcertificate.SerialNumber));
            ViewCertText.Text += "\n ValidFrom : " + selectedcertificate.ValidFrom;
            ViewCertText.Text += "\n ValidTo : " + selectedcertificate.ValidTo;
        }

        private void CertificateList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewCertText.Text = string.Empty;
        }

        private void VerifyCert_SelectionChaged(object sender, SelectionChangedEventArgs e)
        {
            ViewCertText.Text = string.Empty;
        }
    }
 }
