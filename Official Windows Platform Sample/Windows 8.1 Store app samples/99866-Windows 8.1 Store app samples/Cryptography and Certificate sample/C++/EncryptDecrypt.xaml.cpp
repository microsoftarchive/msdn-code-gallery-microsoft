//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "EncryptDecrypt.xaml.h"

using namespace std;
using namespace SDKSample::CryptographyAndCertificate;
using namespace Platform;
using namespace Windows::Security::Cryptography;
using namespace Windows::Security::Cryptography::Core;
using namespace Windows::Security::Cryptography::DataProtection;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Concurrency;

EncryptDecrypt::EncryptDecrypt()
{
    InitializeComponent();

    NonceBytes = ref new Platform::Array<BYTE>(12);

    for (uint32 i = 0; i < NonceBytes->Length; i++)
        NonceBytes->set(i, 0);

    AlgorithmNames->SelectedIndex = 0;
    KeySizes->SelectedIndex = 0;
    bAsymAlgs->IsChecked = true;
    bEncryption->IsChecked = true;
    bFixedInput->IsChecked = true;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void EncryptDecrypt::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

/// <summary>
/// This is the click handler for the 'RunSample' button.  It is responsible for executing the sample code.
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void EncryptDecrypt::RunEncryption_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    rootPage = MainPage::Current;
    TextBlock^ encryptDecryptText = safe_cast<TextBlock^>(rootPage->FindName(L"EncryptDecryptText"));
    ComboBox^ algoNames = safe_cast<ComboBox^>(rootPage->FindName(L"AlgorithmNames"));
    RadioButton^ bSymAlgs = safe_cast<RadioButton^>(rootPage->FindName(L"bSymAlgs"));
    RadioButton^ bAuthEncrypt = safe_cast<RadioButton^>(rootPage->FindName(L"bAuthEncrypt"));

    encryptDecryptText->Text = "";

    IBuffer^ encrypted;
    IBuffer^ decrypted;
    IBuffer^ iv = nullptr;
    IBuffer^ data;
    IBuffer^ nonce;
    String^ algName = algoNames->SelectionBoxItem->ToString();

    CryptographicKey^ key = nullptr;
    if (bSymAlgs->IsChecked->Value || bAuthEncrypt->IsChecked->Value)
        key = GenerateSymmetricKey();
    else
        key = GenerateAsymmetricKey();

    {
        String^ cookie = "Data to encrypt ";

        if (bSymAlgs->IsChecked->Value)
        {
            data = CryptographicBuffer::ConvertStringToBinary(cookie, BinaryStringEncoding::Utf8);
        }
        else
        {
            switch (algoNames->SelectedIndex)
            {
                case 0:
                    data = CryptographicBuffer::ConvertStringToBinary(cookie, BinaryStringEncoding::Utf16LE);
                    break;

                // OAEP Padding depends on key size, message length and hash block length
                // 
                // The maximum plaintext length is KeyLength - 2*HashBlock - 2
                //
                // OEAP padding supports an optional label with the length is restricted by plaintext/key/hash sizes.
                // Here we just use a small label.
                case 1:
                    data = CryptographicBuffer::GenerateRandom(1024 / 8 - 2 * 20 - 2);
                    break;
                case 2:
                    data = CryptographicBuffer::GenerateRandom(1024 / 8 - 2 * (256 / 8) - 2);
                    break;
                case 3:
                    data = CryptographicBuffer::GenerateRandom(2048 / 8 - 2 * (384 / 8) - 2);
                    break;
                case 4:
                    data = CryptographicBuffer::GenerateRandom(2048 / 8 - 2 * (512 / 8) - 2);
                    break;
                default:
                    encryptDecryptText->Text += "An invalid algorithm was selected";
                    data = nullptr;
            }
        }
    }

    if (bAuthEncrypt->IsChecked->Value)
    {
        {
            // NOTE: 
            // 
            // The security best practises require that the Encrypt operation
            // not be called more than once with the same nonce for the same key.
            // 
            // Nonce can be predictable, but must be unique per secure session.

            int carry = 1;

            for (uint32 i = 0; i < NonceBytes->Length && carry == 1; i++)
            {
                if (NonceBytes->Data[i] == 255)
                {
                    NonceBytes->Data[i] = 0;
                }
                else
                {
                    NonceBytes->set(i, NonceBytes->Data[i] + 1);
                    carry = 0;
                }
            }

            nonce = CryptographicBuffer::CreateFromByteArray(NonceBytes);
        }

        EncryptedAndAuthenticatedData^ encryptedData;

        try
        {
            encryptedData = CryptographicEngine::EncryptAndAuthenticate(key, data, nonce, nullptr);
        }
        catch(Exception^ ex)
        {
            encryptDecryptText->Text += "Encryption Failed. Exception data:" + ex->Message;
            return;
        }

        encryptDecryptText->Text += "    Plain text: " + data->Length + " bytes\n";
        encryptDecryptText->Text += "    Encrypted: " + encryptedData->EncryptedData->Length + " bytes\n";
        encryptDecryptText->Text += "    AuthTag: " + encryptedData->AuthenticationTag->Length + " bytes\n";

        decrypted = CryptographicEngine::DecryptAndAuthenticate(key, encryptedData->EncryptedData, nonce, encryptedData->AuthenticationTag, nullptr);

        if (!CryptographicBuffer::Compare(decrypted, data))
        {
            encryptDecryptText->Text += "Decrypted does not match original!";
            return;
        }
    }
    else
    {
        // CBC mode needs Initialization vector, here just random data.
        // IV property will be set on "Encrypted".
        if (wcsstr(reinterpret_cast<const wchar_t*>(algName->Data()), L"CBC"))
        {
            SymmetricKeyAlgorithmProvider^ algorithm = SymmetricKeyAlgorithmProvider::OpenAlgorithm(algName);
            iv = CryptographicBuffer::GenerateRandom(algorithm->BlockLength);
        }

        // Encrypt the data.
        try
        {
            encrypted = CryptographicEngine::Encrypt(key, data, iv);
        }
        catch (Exception^ ex)
        {
            encryptDecryptText->Text += ex->Message + "\n";
            encryptDecryptText->Text += "An invalid key size was selected for the given algorithm.\n";
            return;
        }

        encryptDecryptText->Text += "    Plain text: " + data->Length + " bytes\n";
        encryptDecryptText->Text += "    Encrypted: " + encrypted->Length + " bytes\n";

        // Decrypt the data.
        decrypted = CryptographicEngine::Decrypt(key, encrypted, iv);

        if (!CryptographicBuffer::Compare(decrypted, data))
        {
            encryptDecryptText->Text += "Decrypted data does not match original!";
            return;
        }
    }
}
        

void EncryptDecrypt::RunDataProtection_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    rootPage = MainPage::Current;
    RadioButton^ bFixedInput = safe_cast<RadioButton^>(rootPage->FindName(L"bFixedInput"));
    RadioButton^ bStreamInput = safe_cast<RadioButton^>(rootPage->FindName(L"bStreamInput"));
    TextBox^ tbDescriptor = safe_cast<TextBox^>(rootPage->FindName(L"tbDescriptor"));
    TextBlock^ encryptDecryptText = safe_cast<TextBlock^>(rootPage->FindName(L"EncryptDecryptText"));


    encryptDecryptText->Text = "";

    String^ descriptor = tbDescriptor->Text;
    if (bFixedInput->IsChecked->Value)
        SampleDataProtection(descriptor);

    if (bStreamInput->IsChecked->Value)
        SampleDataProtectionStream(descriptor);
}

CryptographicKey^ EncryptDecrypt::GenerateAsymmetricKey()
{
    rootPage = MainPage::Current;
    ComboBox^ algoNames = safe_cast<ComboBox^>(rootPage->FindName(L"AlgorithmNames"));
    ComboBox^ keySizes = safe_cast<ComboBox^>(rootPage->FindName(L"KeySizes"));
    TextBlock^ encryptDecryptText = safe_cast<TextBlock^>(rootPage->FindName(L"EncryptDecryptText"));

    Platform::String^ algName = algoNames->SelectionBoxItem->ToString();
    uint32 keySize = _wtoi(keySizes->SelectionBoxItem->ToString()->Data());

    CryptographicKey^ keyPair;
    // Create an AsymmetricKeyAlgorithmProvider object for the algorithm specified on input.
    AsymmetricKeyAlgorithmProvider^ Algorithm = AsymmetricKeyAlgorithmProvider::OpenAlgorithm(algName);

    encryptDecryptText->Text += "*** Sample Encryption Algorithm\n";
    encryptDecryptText->Text += "    Algorithm Name: " + Algorithm->AlgorithmName + "\n";
    encryptDecryptText->Text += "    Key Size: " + keySize + "\n";

    // Generate a key pair.
    try
    {
        keyPair = Algorithm->CreateKeyPair(keySize);
    }
    catch (Exception^ ex)
    {
        encryptDecryptText->Text += ex->Message + "\n";
        encryptDecryptText->Text += "An invalid key size was specified for the given algorithm.";
        return nullptr;
    }
    return keyPair;
}

CryptographicKey^ EncryptDecrypt::GenerateSymmetricKey()
{
    rootPage = MainPage::Current;
    ComboBox^ algoNames = safe_cast<ComboBox^>(rootPage->FindName(L"AlgorithmNames"));
    ComboBox^ keySizes = safe_cast<ComboBox^>(rootPage->FindName(L"KeySizes"));
    TextBlock^ encryptDecryptText = safe_cast<TextBlock^>(rootPage->FindName(L"EncryptDecryptText"));

    Platform::String^ algName = algoNames->SelectionBoxItem->ToString();
    uint32 keySize = _wtoi(keySizes->SelectionBoxItem->ToString()->Data());

    CryptographicKey^ key;
    // Create an SymmetricKeyAlgorithmProvider object for the algorithm specified on input.
    SymmetricKeyAlgorithmProvider^ Algorithm = SymmetricKeyAlgorithmProvider::OpenAlgorithm(algName);

    encryptDecryptText->Text += "*** Sample Encryption Algorithm\n";
    encryptDecryptText->Text += "    Algorithm Name: " + Algorithm->AlgorithmName + "\n";
    encryptDecryptText->Text += "    Key Size: " + keySize + "\n";
    encryptDecryptText->Text += "    Block length: " + Algorithm->BlockLength + "\n";

    // Generate a symmetric key.
    IBuffer^ keymaterial = CryptographicBuffer::GenerateRandom((keySize + 7) / 8);
    try
    {
        key = Algorithm->CreateSymmetricKey(keymaterial);
    }
    catch (Exception^ ex)
    {
        encryptDecryptText->Text += ex->Message + "\n";
        encryptDecryptText->Text += "An invalid key size was specified for the given algorithm.";
        return nullptr;
    }
    return key;
}

/// <summary>
/// 
/// </summary>
/// <param name="descriptor">The descriptor string used to protect the data</param>
void EncryptDecrypt::SampleDataProtection(Platform::String^ descriptor)
{
    rootPage = MainPage::Current;
    TextBlock^ encryptDecryptText = safe_cast<TextBlock^>(rootPage->FindName(L"EncryptDecryptText"));

    encryptDecryptText->Text +=     L"*** Sample Data Protection for " + descriptor + L" ***\n";

    DataProtectionProvider Provider(descriptor);

    encryptDecryptText->Text += "    DataProtectionProvider is Ready\n";

    // Create random data for protection
    IBuffer^ data = CryptographicBuffer::GenerateRandom(73);
    encryptDecryptText->Text += "    Original Data: " + CryptographicBuffer::EncodeToHexString(data) + "\n";

    // Protect the random data
    create_task(Provider.ProtectAsync(data)).then([this, data, encryptDecryptText](IBuffer^ protectedData)
    {
        encryptDecryptText->Text += "    Protected Data: " + CryptographicBuffer::EncodeToHexString(protectedData) + "\n";

        if (CryptographicBuffer::Compare(data, protectedData))
        {
            encryptDecryptText->Text += "ProtectAsync returned unprotected data";
            return;
        }

        encryptDecryptText->Text += "    ProtectAsync succeeded\n";

        // Unprotect
        DataProtectionProvider^ Provider2 = ref new DataProtectionProvider();

        create_task(Provider2->UnprotectAsync(protectedData)).then([this, data, encryptDecryptText](IBuffer^ unprotectedData)
        {
            if (!CryptographicBuffer::Compare(data, unprotectedData))
            {
                encryptDecryptText->Text += "UnprotectAsync returned invalid data";
                return;
            }

            encryptDecryptText->Text += "    Unprotected Data: " + CryptographicBuffer::EncodeToHexString(unprotectedData) + "\n";
            encryptDecryptText->Text += "*** Done!\n";
        });
    });
}

void EncryptDecrypt::SampleDataProtectionStream(Platform::String^ descriptor)
{
    EncryptDecryptText->Text += "*** Sample Stream Data Protection for " + descriptor + " ***\n";

    IBuffer^ data = CryptographicBuffer::GenerateRandom(10000);
    DataProtectionProvider^ Provider = ref new DataProtectionProvider(descriptor);
    InMemoryRandomAccessStream^ originalData = ref new InMemoryRandomAccessStream();

    //Populate the new memory stream
    IOutputStream^ outputStream = originalData->GetOutputStreamAt(0);
    DataWriter^ writer = ref new DataWriter(outputStream);
    writer->WriteBuffer(data);

    create_task(writer->StoreAsync()).then([this, outputStream, originalData, Provider](unsigned int)
    {
        create_task(outputStream->FlushAsync()).then([this, originalData, Provider](bool)
        {
            //open new memory stream for read0
            IInputStream^ source = originalData->GetInputStreamAt(0);

            //Open the output memory stream
            InMemoryRandomAccessStream^ protectedData = ref new InMemoryRandomAccessStream();
            IOutputStream^ dest = protectedData->GetOutputStreamAt(0);

            // Protect
            create_task(Provider->ProtectStreamAsync(source, dest)).then([this, originalData, protectedData, source, dest](void)
            {
                //Flush the output
                create_task(dest->FlushAsync()).then([this, originalData, protectedData, source, dest](bool bSuccess)
                {
                    if (bSuccess)
                        EncryptDecryptText->Text += "    Protected output was successfully flushed\n";

                    DataReader^ reader1;
                    DataReader^ reader2;

                    //Verify the protected data does not match the original
                    reader1 = ref new DataReader(originalData->GetInputStreamAt(0));
                    reader2 = ref new DataReader(protectedData->GetInputStreamAt(0));

                    auto task1 = create_task(reader1->LoadAsync(safe_cast<unsigned int>(originalData->Size)));
                    auto task2 = create_task(reader2->LoadAsync(safe_cast<unsigned int>(protectedData->Size)));

                    std::vector<task<unsigned int>> tasks; 
                    tasks.push_back(task1);
                    tasks.push_back(task2);

                    when_all(tasks.begin(), tasks.end()).then([this, originalData, protectedData, reader1, reader2](std::vector<unsigned int,std::allocator<char32_t>>)
                    {
                        EncryptDecryptText->Text += "    Size of original stream:  " + originalData->Size + "\n";
                        EncryptDecryptText->Text += "    Size of protected stream:  " + protectedData->Size + "\n";

                        if (originalData->Size == protectedData->Size)
                        {
                            IBuffer^ buff1 = reader1->ReadBuffer(safe_cast<unsigned int>(originalData->Size));
                            IBuffer^ buff2 = reader2->ReadBuffer(safe_cast<unsigned int>(protectedData->Size));

                            if (CryptographicBuffer::Compare(buff1, buff2))
                            {
                                EncryptDecryptText->Text += "ProtectStreamAsync returned unprotected data";
                            }
                        }
                        else
                        {
                            EncryptDecryptText->Text += "    Stream Compare completed.  Streams did not match.\n";

                            IInputStream^ source = protectedData->GetInputStreamAt(0);

                            InMemoryRandomAccessStream^ unprotectedData = ref new InMemoryRandomAccessStream();
                            IOutputStream^ dest = unprotectedData->GetOutputStreamAt(0);

                            // Unprotect
                            DataProtectionProvider^ Provider2 = ref new DataProtectionProvider();
                                
                            create_task(Provider2->UnprotectStreamAsync(source, dest)).then([this, dest, originalData, unprotectedData](void)
                            {
                                create_task(dest->FlushAsync()).then([this, originalData, unprotectedData](bool bResult)
                                {
                                    if (bResult)
                                        EncryptDecryptText->Text += "    Unprotected output was successfully flushed\n";

                                    //Verify the unprotected data does match the original
                                    DataReader^ reader1 = ref new DataReader(originalData->GetInputStreamAt(0));
                                    DataReader^ reader2 = ref new DataReader(unprotectedData->GetInputStreamAt(0));

                                    auto task1 = create_task(reader1->LoadAsync(safe_cast<unsigned int>(originalData->Size)));
                                    auto task2 = create_task(reader2->LoadAsync(safe_cast<unsigned int>(unprotectedData->Size)));

                                    std::vector<task<unsigned int>> tasks; 
                                    tasks.push_back(task1);
                                    tasks.push_back(task2);

                                    when_all(tasks.begin(), tasks.end()).then([this, originalData, unprotectedData, reader1, reader2](std::vector<unsigned int,std::allocator<char32_t>>)
                                    {
                                        EncryptDecryptText->Text += "    Size of original stream:  " + originalData->Size + "\n";
                                        EncryptDecryptText->Text += "    Size of unprotected stream:  " + unprotectedData->Size + "\n";

                                        IBuffer^ buff1 = reader1->ReadBuffer(safe_cast<unsigned int>(originalData->Size));
                                        IBuffer^ buff2 = reader2->ReadBuffer(safe_cast<unsigned int>(unprotectedData->Size));
                                        if (!CryptographicBuffer::Compare(buff1, buff2))
                                        {
                                            EncryptDecryptText->Text += "UnrotectStreamAsync did not return expected data";
                                        }
                                        else
                                        {
                                            EncryptDecryptText->Text += "*** Done!\n";
                                        }
                                    });
                                });
                            });
                        }
                    });

                });
            });
        });
    });
    

}

void EncryptDecrypt::bSymAlgs_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    AlgorithmNames->Items->Clear();
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::AesCbc);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::AesCbcPkcs7);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::AesEcb);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::AesEcbPkcs7);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::DesCbc);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::DesCbcPkcs7);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::DesEcb);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::DesEcbPkcs7);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::Rc2Cbc);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::Rc2CbcPkcs7);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::Rc2Ecb);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::Rc2EcbPkcs7);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::Rc4);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::TripleDesCbc);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::TripleDesCbcPkcs7);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::TripleDesEcb);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::TripleDesEcbPkcs7);
    AlgorithmNames->SelectedIndex = 0;
}

void EncryptDecrypt::bAsymAlgs_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    AlgorithmNames->Items->Clear();
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::RsaPkcs1);
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::RsaOaepSha1);
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::RsaOaepSha256);
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::RsaOaepSha384);
    AlgorithmNames->Items->Append(AsymmetricAlgorithmNames::RsaOaepSha512);
    AlgorithmNames->SelectedIndex = 0;
}

void EncryptDecrypt::bAuthEncrypt_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    AlgorithmNames->Items->Clear();
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::AesCcm);
    AlgorithmNames->Items->Append(SymmetricAlgorithmNames::AesGcm);
    AlgorithmNames->SelectedIndex = 0;
}

void EncryptDecrypt::bEncryption_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    spEncryption->Visibility = Windows::UI::Xaml::Visibility::Visible;
    spDataProtection->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
}

void EncryptDecrypt::bDataProtection_Checked(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    spEncryption->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    spDataProtection->Visibility = Windows::UI::Xaml::Visibility::Visible;
}
