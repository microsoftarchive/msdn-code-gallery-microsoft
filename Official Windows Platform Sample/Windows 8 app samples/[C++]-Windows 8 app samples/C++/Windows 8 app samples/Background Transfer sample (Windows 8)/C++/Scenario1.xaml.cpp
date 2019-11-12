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

using namespace SDKSample;
using namespace SDKSample::BackgroundTransfer;

using namespace Concurrency;
using namespace Platform;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Networking::BackgroundTransfer;
using namespace Windows::Web;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage;

Scenario1::Scenario1()
{
    InitializeComponent();
    cancellationToken = new cancellation_token_source();
}

Scenario1::~Scenario1()
{
    if (cancellationToken != nullptr)
    {
        delete cancellationToken;
    }
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
    DiscoverActiveDownloads();
}

// Enumerate the downloads that were going on in the background while the app was closed.
void Scenario1::DiscoverActiveDownloads()
{
    task<IVectorView<DownloadOperation^>^>(BackgroundDownloader::GetCurrentDownloadsAsync()).then([this] (IVectorView<DownloadOperation^>^ downloads)
    {
        Log("Loading background downloads: " + downloads->Size);

        for (unsigned int i = 0; i < downloads->Size; i++)
        {
            DownloadOperation^ download = downloads->GetAt(i);
            Log("Discovered background download: " + download->Guid + ", Status: " + download->Progress.Status.ToString());

            // Attach progress and completion handlers.
            HandleDownloadAsync(download, false);
        }
    }).then([this] (task<void> previousTask)
    {
        try
        {
            previousTask.get();
        }
        catch (Exception^ ex)
        {
            LogException("Discovery error", ex);
        }
    });
}

void Scenario1::HandleDownloadAsync(DownloadOperation^ download, boolean start)
{
    IAsyncOperationWithProgress<DownloadOperation^, DownloadOperation^>^ async;
    activeDownloads[download->Guid.GetHashCode()] = download;

    LogStatus("Running: " + download->Guid, NotifyType::StatusMessage);

    if (start)
    {
        async = download->StartAsync();
    }
    else
    {
        async = download->AttachAsync();
    }

    async->Progress = ref new AsyncOperationProgressHandler<DownloadOperation^, DownloadOperation^>(this, &Scenario1::DownloadProgress);
    task<DownloadOperation^>(async, cancellationToken->get_token()).then([this] (DownloadOperation^ download)
    {
        ResponseInformation^ response = download->GetResponseInformation();
        LogStatus("Completed: " + download->Guid + ", Status Code: " + response->StatusCode, NotifyType::StatusMessage);
    }).then([this, download] (task<void> previousTask)
    {
        try
        {
            previousTask.get();
        }
        catch (Exception^ ex)
        {
            LogException("Handle download", ex);
        }

        // It is important to note, that this method executes on UI thread. This guarantees that
        // access to the activeDownloads is synchronized.
        activeDownloads.erase(download->Guid.GetHashCode());
    });
}

void Scenario1::DownloadProgress(IAsyncOperationWithProgress<DownloadOperation^, DownloadOperation^>^ operation, DownloadOperation^ download)
{
    MarshalLog("Progress: " + download->Guid + ", Status: " + download->Progress.Status.ToString());

    double percent = 0;
    if (download->Progress.TotalBytesToReceive > 0)
    {
        double percent = (double)(download->Progress.BytesReceived / download->Progress.TotalBytesToReceive) * 100;
    }

    MarshalLog(" - Transfered bytes: " + download->Progress.BytesReceived + " of " +
        download->Progress.TotalBytesToReceive + ", " + percent + "%");

    if (download->Progress.HasRestarted)
    {
        MarshalLog(" - Download restarted");
    }

    if (download->Progress.HasResponseChanged)
    {
        // We've received new response headers from the server.
        MarshalLog(" - Response updated; Header count: " + download->GetResponseInformation()->Headers->Size);

        // If you want to stream the response data this is a good time to start.
        // download->GetResultStreamAt(0);
    }
}

void Scenario1::StartDownload_Click(Object^ sender, RoutedEventArgs^ e)
{
    // By default 'serverAddressField' is disabled and URI validation is not required. When enabling the text box
    // validating the URI is required since it was received from an untrusted source (user input).
    // The URI is validated by calling TryGetUri() that will return 'false' for strings that are not valid URIs.
    // Note that when enabling the text box users may provide URIs to machines on the intrAnet that require the
    // "Home or Work Networking" capability.
    Uri^ source;
    if (!rootPage->TryGetUri(serverAddressField->Text, &source))
    {
        return;
    }

    String^ destination = StringTrimmer::Trim(fileNameField->Text);

    if (destination == "")
    {
        rootPage->NotifyUser("A local file name is required.", NotifyType::ErrorMessage);
        return;
    }

    task<StorageFile^>(KnownFolders::PicturesLibrary->CreateFileAsync(destination, CreationCollisionOption::GenerateUniqueName)).then([this, source] (StorageFile^ destinationFile)
    {
        BackgroundDownloader^ downloader = ref new BackgroundDownloader();
        DownloadOperation^ download = downloader->CreateDownload(source, destinationFile);

        Log("Downloading " + source->AbsoluteUri + " to " + destinationFile->Name + ", " + download->Guid);

        // Attach progress and completion handlers.
        HandleDownloadAsync(download, true);
    }).then([this] (task<void> previousTask)
    {
        try
        {
            previousTask.get();
        }
        catch (Exception^ ex)
        {
            LogException("Error", ex);
        }
    });
}

void Scenario1::PauseAll_Click(Object^ sender, RoutedEventArgs^ e)
{
    Log("Downloads: " + activeDownloads.size());
    for (auto iterator = activeDownloads.begin(); iterator != activeDownloads.end(); iterator++)
    {
        DownloadOperation^ download = iterator->second;
        if (download->Progress.Status == BackgroundTransferStatus::Running)
        {
            download->Pause();
            Log("Paused: " + download->Guid);
        }
        else
        {
            Log("Skipped: " + download->Guid + ", Status: " + download->Progress.Status.ToString());
        }
    }
}

void Scenario1::ResumeAll_Click(Object^ sender, RoutedEventArgs^ e)
{
    Log("Downloads: " + activeDownloads.size());
    for (auto iterator = activeDownloads.begin(); iterator != activeDownloads.end(); iterator++)
    {
        DownloadOperation^ download = iterator->second;
        if (download->Progress.Status == BackgroundTransferStatus::PausedByApplication)
        {
            download->Resume();
            Log("Resumed: " + download->Guid);
        }
        else
        {
            Log("Skipped: " + download->Guid + ", Status: " + download->Progress.Status.ToString());
        }
    }
}

void Scenario1::CancelAll_Click(Object^ sender, RoutedEventArgs^ e)
{
    Log("Cancelling Downloads: " + activeDownloads.size());

    cancellationToken->cancel();
    cancellationToken = new cancellation_token_source();
    activeDownloads.clear();
}

void Scenario1::LogException(String^ title, Exception^ ex)
{
    WebErrorStatus error = BackgroundTransferError::GetStatus(ex->HResult);
    if (error == WebErrorStatus::Unknown)
    {
        LogStatus(title + ": " + ex, NotifyType::ErrorMessage);
    }
    else
    {
        LogStatus(title + ": " + error.ToString(), NotifyType::ErrorMessage);
    }
}

// When operations happen on a background thread we have to marshal UI updates back to the UI thread.
void Scenario1::MarshalLog(String^ value)
{
    Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, value] ()
    {
        Log(value);
    }));
}

void Scenario1::Log(String^ message)
{
    outputField->Text += message + "\r\n";
}

void Scenario1::LogStatus(String^ message, NotifyType type)
{
    rootPage->NotifyUser(message, type);
    outputField->Text += message + "\r\n";
}
