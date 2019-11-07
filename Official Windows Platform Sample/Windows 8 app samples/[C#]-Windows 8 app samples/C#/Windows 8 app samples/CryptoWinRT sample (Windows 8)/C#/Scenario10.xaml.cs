//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography.DataProtection;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;

namespace CryptoWinRT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario10 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario10()
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

        public async void SampleDataProtectionStream(String descriptor)
        {
            Scenario10Text.Text += "*** Sample Stream Data Protection for " + descriptor + " ***\n";

            IBuffer data = CryptographicBuffer.GenerateRandom(10000);
            DataReader reader1, reader2;
            IBuffer buff1, buff2;

            DataProtectionProvider Provider = new DataProtectionProvider(descriptor);
            InMemoryRandomAccessStream originalData = new InMemoryRandomAccessStream();

            //Populate the new memory stream
            IOutputStream outputStream = originalData.GetOutputStreamAt(0);
            DataWriter writer = new DataWriter(outputStream);
            writer.WriteBuffer(data);
            await writer.StoreAsync();
            await outputStream.FlushAsync();

            //open new memory stream for read
            IInputStream source = originalData.GetInputStreamAt(0);

            //Open the output memory stream
            InMemoryRandomAccessStream protectedData = new InMemoryRandomAccessStream();
            IOutputStream dest = protectedData.GetOutputStreamAt(0);

            // Protect
            await Provider.ProtectStreamAsync(source, dest);

            //Flush the output
            if (await dest.FlushAsync())
                Scenario10Text.Text += "    Protected output was successfully flushed\n";


            //Verify the protected data does not match the original
            reader1 = new DataReader(originalData.GetInputStreamAt(0));
            reader2 = new DataReader(protectedData.GetInputStreamAt(0));

            await reader1.LoadAsync((uint)originalData.Size);
            await reader2.LoadAsync((uint)protectedData.Size);

            Scenario10Text.Text += "    Size of original stream:  " + originalData.Size + "\n";
            Scenario10Text.Text += "    Size of protected stream:  " + protectedData.Size + "\n";

            if (originalData.Size == protectedData.Size)
            {
                buff1 = reader1.ReadBuffer((uint)originalData.Size);
                buff2 = reader2.ReadBuffer((uint)protectedData.Size);
                if (CryptographicBuffer.Compare(buff1, buff2))
                {
                    Scenario10Text.Text += "ProtectStreamAsync returned unprotected data";
                    return;
                }
            }

            Scenario10Text.Text += "    Stream Compare completed.  Streams did not match.\n";

            source = protectedData.GetInputStreamAt(0);

            InMemoryRandomAccessStream unprotectedData = new InMemoryRandomAccessStream();
            dest = unprotectedData.GetOutputStreamAt(0);

            // Unprotect
            DataProtectionProvider Provider2 = new DataProtectionProvider();
            await Provider2.UnprotectStreamAsync(source, dest);

            if (await dest.FlushAsync())
                Scenario10Text.Text += "    Unprotected output was successfully flushed\n";

            //Verify the unprotected data does match the original
            reader1 = new DataReader(originalData.GetInputStreamAt(0));
            reader2 = new DataReader(unprotectedData.GetInputStreamAt(0));

            await reader1.LoadAsync((uint)originalData.Size);
            await reader2.LoadAsync((uint)unprotectedData.Size);

            Scenario10Text.Text += "    Size of original stream:  " + originalData.Size + "\n";
            Scenario10Text.Text += "    Size of unprotected stream:  " + unprotectedData.Size + "\n";

            buff1 = reader1.ReadBuffer((uint)originalData.Size);
            buff2 = reader2.ReadBuffer((uint)unprotectedData.Size);
            if (!CryptographicBuffer.Compare(buff1, buff2))
            {
                Scenario10Text.Text += "UnrotectStreamAsync did not return expected data";
                    return;
            }

            Scenario10Text.Text += "*** Done!\n";
        }

        /// <summary>
        /// This is the click handler for the 'Default' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunSample_Click(object sender, RoutedEventArgs e)
        {
            String descriptor = tbDescriptor.Text;
            SampleDataProtectionStream(descriptor);
        }
    }
}
