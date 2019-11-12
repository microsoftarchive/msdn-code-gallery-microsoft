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
// Scenario1.xaml.cpp
// Implementation of the Scenario1 class
//

#include "pch.h"
#include "Scenario1.xaml.h"
#include "CompressionUtils.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::ViewManagement;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage::Streams;
using namespace Windows::Storage::Compression;

using namespace SDKSample::Compression;

using namespace concurrency;

Scenario1::Scenario1()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario1::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void ::SDKSample::Compression::Scenario1::DoScenario(CompressAlgorithm Algorithm)
{
    Progress->Text = "";

    // This scenario uses File Picker which doesn't work in snapped mode - try unsnap first
    // and fail gracefully if we can't
    if ((ApplicationView::Value == ApplicationViewState::Snapped) &&
        !ApplicationView::TryUnsnap())
    {
        rootPage->NotifyUser("Sample doesn't work in snapped mode", NotifyType::ErrorMessage);
        return;
    }

    rootPage->NotifyUser("Working...", NotifyType::StatusMessage);

    auto context = std::make_shared<ScenarioContext>();
    context->raStream = ref new InMemoryRandomAccessStream();

    auto picker = ref new Pickers::FileOpenPicker();
    picker->FileTypeFilter->Append("*");

    // First pick a test file and open it for reading
    create_task(picker->PickSingleFileAsync()).then([=](StorageFile^ OriginalFile)
    {
        if (!OriginalFile)
        {
            throw std::runtime_error("No file has been selected");
        }

        Progress->Text += "File \"" + OriginalFile->Name + "\" has been picked\n";

        return OriginalFile->OpenAsync(FileAccessMode::Read);
    })

    // Then read the whole file into memory buffer
    .then([=](IRandomAccessStream^ OriginalStream)
    {
        return ReadStreamTask(OriginalStream, context->originalData).GetTask();
    })

    // Then compress data into in-memory stream
    .then([=](size_t BytesRead)
    {
        Progress->Text += BytesRead + " bytes have been read from disk\n";

        auto memoryOutputStream = context->raStream->GetOutputStreamAt(0);

        // Use default constructor if you don't need any specific algorithm and/or block size, use
        // "extended" constructor otherwise. This samples shows both versions for demonstration
        // purposes only.
        if (Algorithm == CompressAlgorithm::InvalidAlgorithm)
        {
            context->compressor = ref new Compressor(memoryOutputStream);
        }
        else
        {
            context->compressor = ref new Compressor(memoryOutputStream,
                                                        Algorithm,
                                                        0           // use default block size
                                                        );
        }

        Progress->Text += "Compressor object has been created\n";

        auto dataWriter = ref new DataWriter(context->compressor);
        context->writer = dataWriter;
        auto data = ref new Platform::Array<byte>(context->originalData.data(), (unsigned int)context->originalData.size());
        dataWriter->WriteBytes(data);

        return dataWriter->StoreAsync();
    })

    // Then finalize compression stream
    .then([=](unsigned int BytesWritten)
    {
        Progress->Text += BytesWritten + " bytes have been compressed\n";

        return context->compressor->FinishAsync();
    })

    // Then decompress in-memory stream into vector
    .then([=](bool)
    {
        Progress->Text += "Compressed into " + context->raStream->Size + " bytes\n";

        auto decompressor = ref new Decompressor(context->raStream->GetInputStreamAt(0));

        Progress->Text += "Decompressor object has been created\n";

        return ReadStreamTask(decompressor, context->decompressedData).GetTask();
    })

    // Then verify if any data has been lost in action
    .then([=](size_t BytesRead)
    {
        Progress->Text += BytesRead + " bytes have been decompressed\n";

        if (context->decompressedData.size() != BytesRead)
        {
            throw std::runtime_error("Decompressed data size doesn't match number of bytes read from in-memory stream");
        }

        if (context->originalData.size() != context->decompressedData.size())
        {
            throw std::runtime_error("Decompressed data size doesn't match original one");
        }

        if (!std::equal(context->originalData.begin(), context->originalData.end(), context->decompressedData.begin()))
        {
            throw std::runtime_error("Decompressed data doesn't match original one");
        }

        Progress->Text += "Decompressed data matches original\n";
    })

    // Final task based continuation is used to handle exceptions in the chain above
    .then([=](task<void> FinalContinuation)
    {
        try
        {
            // Transport all exceptions to this thread. This task is guaranteed to be completed by now.
            FinalContinuation.get();

            Progress->Text += "All done\n";
            rootPage->NotifyUser("Finished", NotifyType::StatusMessage);
        }
        catch (Platform::Exception ^e)
        {
            rootPage->NotifyUser(e->Message, NotifyType::ErrorMessage);
        }
        catch (const std::exception &e)
        {
            std::wstringstream wss;
            wss << e.what();
            rootPage->NotifyUser(ref new Platform::String(wss.str().c_str()), NotifyType::ErrorMessage);
        }
    });
}

void ::SDKSample::Compression::Scenario1::Default_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);

    if (b != nullptr)
    {
        DoScenario(CompressAlgorithm::InvalidAlgorithm);
    }
}

void ::SDKSample::Compression::Scenario1::Xpress_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);

    if (b != nullptr)
    {
        DoScenario(CompressAlgorithm::Xpress);
    }
}

void ::SDKSample::Compression::Scenario1::XpressHuff_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);

    if (b != nullptr)
    {
        DoScenario(CompressAlgorithm::XpressHuff);
    }
}

void ::SDKSample::Compression::Scenario1::Mszip_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);

    if (b != nullptr)
    {
        DoScenario(CompressAlgorithm::Mszip);
    }
}

void ::SDKSample::Compression::Scenario1::Lzms_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);

    if (b != nullptr)
    {
        DoScenario(CompressAlgorithm::Lzms);
    }
}
