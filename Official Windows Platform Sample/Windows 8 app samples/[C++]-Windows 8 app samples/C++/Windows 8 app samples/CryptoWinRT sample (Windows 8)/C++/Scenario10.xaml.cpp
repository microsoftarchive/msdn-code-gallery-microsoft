//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// Scenario10.xaml.cpp
// Implementation of the Scenario10 class
//

#include "pch.h"
#include "Scenario10.xaml.h"

using namespace SDKSample::CryptoWinRT;

using namespace concurrency;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Security::Cryptography;
using namespace Windows::Security::Cryptography::Core;
using namespace Windows::Security::Cryptography::DataProtection;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

Scenario10::Scenario10()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario10::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::CryptoWinRT::Scenario10::RunSample_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Scenario10Text->Text = "";
    String^ descriptor = tbDescriptor->Text;

    Scenario10Text->Text += "*** Sample Stream Data Protection for " + descriptor + " ***\n";

    IBuffer^ data = CryptographicBuffer::GenerateRandom(10000);

    DataProtectionProvider^ Provider = ref new DataProtectionProvider(descriptor);
    InMemoryRandomAccessStream^ originalData = ref new InMemoryRandomAccessStream();

    //Populate the new memory stream
    IOutputStream^ outputStream = originalData->GetOutputStreamAt(0);
    DataWriter^ writer = ref new DataWriter(outputStream);
    writer->WriteBuffer(data);

    task<unsigned int> (writer->StoreAsync()).then([=](task<unsigned int> resultTask)
    {
        Scenario10Text->Text += "    StoreAsync completed.\n";
        Scenario10Text->Text += "    Stored " + resultTask.get() + " bytes.\n";

        task<bool>(writer->FlushAsync()).then([=](bool isFlushCompleted)
        {
            if (!isFlushCompleted)
            {
                Scenario10Text->Text += "    FlushAsync failed!\n";
                return;
            }

            Scenario10Text->Text += "    FlushAsync completed.\n";

            //open new memory stream for read
            IInputStream^ source = originalData->GetInputStreamAt(0);

            //Open the output memory stream
            InMemoryRandomAccessStream^ protectedData = ref new InMemoryRandomAccessStream();
            IOutputStream^ dest = protectedData->GetOutputStreamAt(0);

            //The streams are ready for input and output.  ProtectStreamAsync can now be called
            create_task(Provider->ProtectStreamAsync(source,dest)).then([=]()
            {
                Scenario10Text->Text += "    ProtectStreamAsync completed.\n";

                task<bool>(dest->FlushAsync()).then([=](bool isFlush2Completed)
                {
                    if (!isFlush2Completed)
                    {
                        Scenario10Text->Text += "    FlushAsync failed!\n";
                        return;
                    }

                    Scenario10Text->Text += "    Protected output was successfully flushed.\n";

                    //Verify the protected data does not match the original
                    DataReader^ reader1 = ref new DataReader(originalData->GetInputStreamAt(0));
                    DataReader^ reader2 = ref new DataReader(protectedData->GetInputStreamAt(0));

                    task<unsigned int> loadSize1(reader1->LoadAsync((unsigned int)originalData->Size));
                    loadSize1.then([=](task<unsigned int> size1Task)
                    {
                        unsigned int size1 = size1Task.get();
                        task<unsigned int> loadSize2(reader2->LoadAsync((unsigned int)protectedData->Size));
                        loadSize2.then([=](task<unsigned int> size2Task)
                        {
                            unsigned int size2 = size2Task.get();
                            Scenario10Text->Text += "    Size of original stream:  " + originalData->Size + "\n";
                            Scenario10Text->Text += "    Size of protected stream:  " + protectedData->Size + "\n";

                            //Check that the data was actually protected.
                            if (originalData->Size == protectedData->Size)
                            {
                                IBuffer^ buff1 = reader1->ReadBuffer((unsigned int)originalData->Size);
                                IBuffer^ buff2 = reader2->ReadBuffer((unsigned int)protectedData->Size);
                                if (CryptographicBuffer::Compare(buff1, buff2))
                                {
                                    Scenario10Text->Text += "ERROR:  ProtectStreamAsync returned unprotected data!\n";
                                    return;
                                }
                            }

                            Scenario10Text->Text += "    Stream Compare completed.  Streams did not match.\n";

                            IInputStream^ source2 = protectedData->GetInputStreamAt(0);

                            InMemoryRandomAccessStream^ unprotectedData = ref new InMemoryRandomAccessStream();
                            IOutputStream^ dest2 = unprotectedData->GetOutputStreamAt(0);

                            // Unprotect
                            DataProtectionProvider^ Provider2 = ref new DataProtectionProvider();
                            create_task(Provider2->UnprotectStreamAsync(source2,dest2)).then([=]()
                            {
                                Scenario10Text->Text += "    UnprotectStreamAsync completed.\n";

                                task<bool>(dest2->FlushAsync()).then([=](bool isFlush3Completed)
                                {
                                    if (!isFlush3Completed)
                                    {
                                        Scenario10Text->Text += "    FlushAsync failed!\n";
                                        return;
                                    }
                                    Scenario10Text->Text += "    Unprotected output was successfully flushed\n";

                                    //Verify the unprotected data does match the original
                                    DataReader^ reader3 = ref new DataReader(originalData->GetInputStreamAt(0));
                                    DataReader^ reader4 = ref new DataReader(unprotectedData->GetInputStreamAt(0));

                                    task<unsigned int> loadSize3(reader3->LoadAsync((unsigned int)originalData->Size));
                                    loadSize3.then([=](task<unsigned int> size3Task)
                                    {
                                        unsigned int size3 = size3Task.get();
                                        task<unsigned int> loadSize4(reader4->LoadAsync((unsigned int)unprotectedData->Size));
                                        loadSize4.then([=](task<unsigned int> size4Task)
                                        {
                                            unsigned int size4 = size4Task.get();
                                            Scenario10Text->Text += "    Size of original stream:  " + originalData->Size + "\n";
                                            Scenario10Text->Text += "    Size of unprotected stream:  " + unprotectedData->Size + "\n";
                                            
                                            if(size3 != size4)
                                            {
                                                Scenario10Text->Text += "ERROR:  UnprotectStreamAsync did not return expected data!\n";
                                                return;
                                            }

                                            IBuffer^ buff3 = reader3->ReadBuffer((unsigned int)originalData->Size);
                                            IBuffer^ buff4 = reader4->ReadBuffer((unsigned int)unprotectedData->Size);

                                            if (!CryptographicBuffer::Compare(buff3, buff4))
                                            {
                                                Scenario10Text->Text += "ERROR:  UnprotectStreamAsync did not return expected data!\n";
                                                return;
                                            }

                                            Scenario10Text->Text += "    UnprotectStreamAsync completed successfully.\n";
                                            Scenario10Text->Text += "*** Done!\n";
                                        });
                                    });
                                });
                            });
                        });
                    });
                });
            });
        });
    });
}
