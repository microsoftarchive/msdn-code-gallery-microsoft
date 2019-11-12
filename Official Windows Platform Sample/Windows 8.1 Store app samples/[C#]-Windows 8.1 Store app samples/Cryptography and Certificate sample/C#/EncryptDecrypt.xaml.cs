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
    public sealed partial class EncryptDecrypt : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // Initialize a nonce value.
        static byte[] NonceBytes = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public EncryptDecrypt()
        {
            this.InitializeComponent();
            AlgorithmNames.SelectedIndex = 0;
            KeySizes.SelectedIndex = 0;
            bAsymAlgs.IsChecked = true;
            bEncryption.IsChecked = true;
            bFixedInput.IsChecked = true;
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
        private void RunEncryption_Click(object sender, RoutedEventArgs e)
        {
            EncryptDecryptText.Text = "";

            IBuffer encrypted;
            IBuffer decrypted;
            IBuffer iv = null;
            IBuffer data;
            IBuffer nonce;
            String algName = AlgorithmNames.SelectionBoxItem.ToString();

            CryptographicKey key = null;
            if (bSymAlgs.IsChecked.Value || bAuthEncrypt.IsChecked.Value)
                key = GenerateSymmetricKey();
            else
                key = GenerateAsymmetricKey();

            data = GenearetData();

            if ((bool)bAuthEncrypt.IsChecked)
            {
                nonce = GetNonce();

                EncryptedAndAuthenticatedData encryptedData = CryptographicEngine.EncryptAndAuthenticate(key, data, nonce, null);

                EncryptDecryptText.Text += "    Plain text: " + data.Length + " bytes\n";
                EncryptDecryptText.Text += "    Encrypted: " + encryptedData.EncryptedData.Length + " bytes\n";
                EncryptDecryptText.Text += "    AuthTag: " + encryptedData.AuthenticationTag.Length + " bytes\n";

                decrypted = CryptographicEngine.DecryptAndAuthenticate(key, encryptedData.EncryptedData, nonce, encryptedData.AuthenticationTag, null);

                if (!CryptographicBuffer.Compare(decrypted, data))
                {
                    EncryptDecryptText.Text += "Decrypted does not match original!";
                    return;
                }
            }
            else
            {
                // CBC mode needs Initialization vector, here just random data.
                // IV property will be set on "Encrypted".
                if (algName.Contains("CBC"))
                {
                    SymmetricKeyAlgorithmProvider algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(algName);
                    iv = CryptographicBuffer.GenerateRandom(algorithm.BlockLength);
                }

                // Encrypt the data.
                try
                {
                    encrypted = CryptographicEngine.Encrypt(key, data, iv);
                }
                catch (ArgumentException ex)
                {
                    EncryptDecryptText.Text += ex.Message + "\n";
                    EncryptDecryptText.Text += "An invalid key size was selected for the given algorithm.\n";
                    return;
                }

                EncryptDecryptText.Text += "    Plain text: " + data.Length + " bytes\n";
                EncryptDecryptText.Text += "    Encrypted: " + encrypted.Length + " bytes\n";

                // Decrypt the data.
                decrypted = CryptographicEngine.Decrypt(key, encrypted, iv);

                if (!CryptographicBuffer.Compare(decrypted, data))
                {
                    EncryptDecryptText.Text += "Decrypted data does not match original!";
                    return;
                }
            }
        }
        
        private void RunDataProtection_Click(object sender, RoutedEventArgs e)
        {
            EncryptDecryptText.Text = "";

            String descriptor = tbDescriptor.Text;
            if ((bool)bFixedInput.IsChecked)
                SampleDataProtection(descriptor);

            if ((bool)bStreamInput.IsChecked)
                SampleDataProtectionStream(descriptor);
        }
        
        /// <summary>
        /// This utility function returns a nonce value for authenticated encryption modes.
        /// </summary>
        /// <returns></returns>
        static IBuffer GetNonce()
        {
            // NOTE: 
            // 
            // The security best practises require that the Encrypt operation
            // not be called more than once with the same nonce for the same key.
            // 
            // Nonce can be predictable, but must be unique per secure session.

            int carry = 1;

            for (int i = 0; i < NonceBytes.Length && carry == 1; i++)
            {
                if (NonceBytes[i] == 255)
                {
                    NonceBytes[i] = 0;
                }
                else
                {
                    NonceBytes[i]++;
                    carry = 0;
                }
            }

            return CryptographicBuffer.CreateFromByteArray(NonceBytes);
        }

        private IBuffer GenearetData()
        {
            IBuffer data;
            String cookie = "Data to encrypt ";

            if ((bool)bSymAlgs.IsChecked)
            {
                data = CryptographicBuffer.ConvertStringToBinary(cookie, BinaryStringEncoding.Utf8);
            }
            else
            {
                switch (AlgorithmNames.SelectedIndex)
                {
                    case 0:
                        data = CryptographicBuffer.ConvertStringToBinary(cookie, BinaryStringEncoding.Utf16LE);
                        break;

                    // OAEP Padding depends on key size, message length and hash block length
                    // 
                    // The maximum plaintext length is KeyLength - 2*HashBlock - 2
                    //
                    // OEAP padding supports an optional label with the length is restricted by plaintext/key/hash sizes.
                    // Here we just use a small label.
                    case 1:
                        data = CryptographicBuffer.GenerateRandom(1024 / 8 - 2 * 20 - 2);
                        break;
                    case 2:
                        data = CryptographicBuffer.GenerateRandom(1024 / 8 - 2 * (256 / 8) - 2);
                        break;
                    case 3:
                        data = CryptographicBuffer.GenerateRandom(2048 / 8 - 2 * (384 / 8) - 2);
                        break;
                    case 4:
                        data = CryptographicBuffer.GenerateRandom(2048 / 8 - 2 * (512 / 8) - 2);
                        break;
                    default:
                        EncryptDecryptText.Text += "An invalid algorithm was selected";
                        return null;
                }
            }

            return data;
        }

        private CryptographicKey GenerateAsymmetricKey()
        {
            String algName = AlgorithmNames.SelectionBoxItem.ToString();
            UInt32 keySize = UInt32.Parse(KeySizes.SelectionBoxItem.ToString());

            CryptographicKey keyPair;
            // Create an AsymmetricKeyAlgorithmProvider object for the algorithm specified on input.
            AsymmetricKeyAlgorithmProvider Algorithm = AsymmetricKeyAlgorithmProvider.OpenAlgorithm(algName);

            EncryptDecryptText.Text += "*** Sample Encryption Algorithm\n";
            EncryptDecryptText.Text += "    Algorithm Name: " + Algorithm.AlgorithmName + "\n";
            EncryptDecryptText.Text += "    Key Size: " + keySize + "\n";

            // Generate a key pair.
            try
            {
                keyPair = Algorithm.CreateKeyPair(keySize);
            }
            catch (ArgumentException ex)
            {
                EncryptDecryptText.Text += ex.Message + "\n";
                EncryptDecryptText.Text += "An invalid key size was specified for the given algorithm.";
                return null;
            }
            return keyPair;
        }

        private CryptographicKey GenerateSymmetricKey()
        {
            String algName = AlgorithmNames.SelectionBoxItem.ToString();
            UInt32 keySize = UInt32.Parse(KeySizes.SelectionBoxItem.ToString());

            CryptographicKey key;
            // Create an SymmetricKeyAlgorithmProvider object for the algorithm specified on input.
            SymmetricKeyAlgorithmProvider Algorithm = SymmetricKeyAlgorithmProvider.OpenAlgorithm(algName);

            EncryptDecryptText.Text += "*** Sample Encryption Algorithm\n";
            EncryptDecryptText.Text += "    Algorithm Name: " + Algorithm.AlgorithmName + "\n";
            EncryptDecryptText.Text += "    Key Size: " + keySize + "\n";
            EncryptDecryptText.Text += "    Block length: " + Algorithm.BlockLength + "\n";

            // Generate a symmetric key.
            IBuffer keymaterial = CryptographicBuffer.GenerateRandom((keySize + 7) / 8);
            try
            {
                key = Algorithm.CreateSymmetricKey(keymaterial);
            }
            catch (ArgumentException ex)
            {
                EncryptDecryptText.Text += ex.Message + "\n";
                EncryptDecryptText.Text += "An invalid key size was specified for the given algorithm.";
                return null;
            }
            return key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptor">The descriptor string used to protect the data</param>
        public async void SampleDataProtection(String descriptor)
        {
            EncryptDecryptText.Text += "*** Sample Data Protection for " + descriptor + " ***\n";

            DataProtectionProvider Provider = new DataProtectionProvider(descriptor);
            EncryptDecryptText.Text += "    DataProtectionProvider is Ready\n";

            // Create random data for protection
            IBuffer data = CryptographicBuffer.GenerateRandom(73);
            EncryptDecryptText.Text += "    Original Data: " + CryptographicBuffer.EncodeToHexString(data) + "\n";

            // Protect the random data
            IBuffer protectedData = await Provider.ProtectAsync(data);
            EncryptDecryptText.Text += "    Protected Data: " + CryptographicBuffer.EncodeToHexString(protectedData) + "\n";

            if (CryptographicBuffer.Compare(data, protectedData))
            {
                EncryptDecryptText.Text += "ProtectAsync returned unprotected data";
                return;
            }

            EncryptDecryptText.Text += "    ProtectAsync succeeded\n";

            // Unprotect
            DataProtectionProvider Provider2 = new DataProtectionProvider();
            IBuffer unprotectedData = await Provider2.UnprotectAsync(protectedData);

            if (!CryptographicBuffer.Compare(data, unprotectedData))
            {
                EncryptDecryptText.Text += "UnprotectAsync returned invalid data";
                return;
            }

            EncryptDecryptText.Text += "    Unprotected Data: " + CryptographicBuffer.EncodeToHexString(unprotectedData) + "\n";
            EncryptDecryptText.Text += "*** Done!\n";
        }

        public async void SampleDataProtectionStream(String descriptor)
        {
            EncryptDecryptText.Text += "*** Sample Stream Data Protection for " + descriptor + " ***\n";

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
                EncryptDecryptText.Text += "    Protected output was successfully flushed\n";


            //Verify the protected data does not match the original
            reader1 = new DataReader(originalData.GetInputStreamAt(0));
            reader2 = new DataReader(protectedData.GetInputStreamAt(0));

            await reader1.LoadAsync((uint)originalData.Size);
            await reader2.LoadAsync((uint)protectedData.Size);

            EncryptDecryptText.Text += "    Size of original stream:  " + originalData.Size + "\n";
            EncryptDecryptText.Text += "    Size of protected stream:  " + protectedData.Size + "\n";

            if (originalData.Size == protectedData.Size)
            {
                buff1 = reader1.ReadBuffer((uint)originalData.Size);
                buff2 = reader2.ReadBuffer((uint)protectedData.Size);
                if (CryptographicBuffer.Compare(buff1, buff2))
                {
                    EncryptDecryptText.Text += "ProtectStreamAsync returned unprotected data";
                    return;
                }
            }

            EncryptDecryptText.Text += "    Stream Compare completed.  Streams did not match.\n";

            source = protectedData.GetInputStreamAt(0);

            InMemoryRandomAccessStream unprotectedData = new InMemoryRandomAccessStream();
            dest = unprotectedData.GetOutputStreamAt(0);

            // Unprotect
            DataProtectionProvider Provider2 = new DataProtectionProvider();
            await Provider2.UnprotectStreamAsync(source, dest);

            if (await dest.FlushAsync())
                EncryptDecryptText.Text += "    Unprotected output was successfully flushed\n";

            //Verify the unprotected data does match the original
            reader1 = new DataReader(originalData.GetInputStreamAt(0));
            reader2 = new DataReader(unprotectedData.GetInputStreamAt(0));

            await reader1.LoadAsync((uint)originalData.Size);
            await reader2.LoadAsync((uint)unprotectedData.Size);

            EncryptDecryptText.Text += "    Size of original stream:  " + originalData.Size + "\n";
            EncryptDecryptText.Text += "    Size of unprotected stream:  " + unprotectedData.Size + "\n";

            buff1 = reader1.ReadBuffer((uint)originalData.Size);
            buff2 = reader2.ReadBuffer((uint)unprotectedData.Size);
            if (!CryptographicBuffer.Compare(buff1, buff2))
            {
                EncryptDecryptText.Text += "UnrotectStreamAsync did not return expected data";
                return;
            }

            EncryptDecryptText.Text += "*** Done!\n";
        }

        private void bSymAlgs_Checked(object sender, RoutedEventArgs e)
        {
            AlgorithmNames.Items.Clear();
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.AesCbc);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.AesCbcPkcs7);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.AesEcb);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.AesEcbPkcs7);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.DesCbc);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.DesCbcPkcs7);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.DesEcb);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.DesEcbPkcs7);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.Rc2Cbc);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.Rc2CbcPkcs7);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.Rc2Ecb);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.Rc2EcbPkcs7);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.Rc4);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.TripleDesCbc);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.TripleDesCbcPkcs7);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.TripleDesEcb);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.TripleDesEcbPkcs7);
            AlgorithmNames.SelectedIndex = 0;
        }

        private void bAsymAlgs_Checked(object sender, RoutedEventArgs e)
        {
            AlgorithmNames.Items.Clear();
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.RsaPkcs1);
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.RsaOaepSha1);
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.RsaOaepSha256);
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.RsaOaepSha384);
            AlgorithmNames.Items.Add(AsymmetricAlgorithmNames.RsaOaepSha512);
            AlgorithmNames.SelectedIndex = 0;
        }

        private void bAuthEncrypt_Checked(object sender, RoutedEventArgs e)
        {
            AlgorithmNames.Items.Clear();
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.AesCcm);
            AlgorithmNames.Items.Add(SymmetricAlgorithmNames.AesGcm);
            AlgorithmNames.SelectedIndex = 0;
        }

        private void bEncryption_Checked(object sender, RoutedEventArgs e)
        {
            spEncryption.Visibility = Windows.UI.Xaml.Visibility.Visible;
            spDataProtection.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void bDataProtection_Checked(object sender, RoutedEventArgs e)
        {
            spEncryption.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            spDataProtection.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }
    }
}
