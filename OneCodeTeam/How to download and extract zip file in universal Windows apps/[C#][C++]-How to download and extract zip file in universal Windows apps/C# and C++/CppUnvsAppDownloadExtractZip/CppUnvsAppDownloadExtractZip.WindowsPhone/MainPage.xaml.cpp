/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.cpp
* Project:      CppUnvsAppDownloadExtractZip.WindowsPhone
* Copyright (c) Microsoft Corporation.
*
* This code sample shows how to download and extract zip file in universal Windows app.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include "pch.h"
#include "MainPage.xaml.h"

using namespace CppUnvsAppDownloadExtractZip;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Concurrency;
using namespace Windows::Networking::BackgroundTransfer;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace Windows::Web;
using namespace Windows::UI::Core;
using namespace ZipHelperWinRT;

MainPage::MainPage()
{
	InitializeComponent();

	pickedFolderPath = nullptr;
	localFileName = nullptr;
	cancellationToken = new cancellation_token_source();
}

MainPage::~MainPage()
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
void MainPage::OnNavigatedTo(NavigationEventArgs^ e)
{
	DiscoverActiveDownloads();
}

// Enumerate the downloads that were going on in the background while the app was closed.
void MainPage::DiscoverActiveDownloads()
{
	create_task(BackgroundDownloader::GetCurrentDownloadsAsync()).then([this](IVectorView<DownloadOperation^>^ downloads)
	{
		Log("Loading background downloads: " + downloads->Size);

		for (unsigned int i = 0; i < downloads->Size; i++)
		{
			DownloadOperation^ download = downloads->GetAt(i);
			Log("Discovered background download: " + download->Guid + ", Status: " + download->Progress.Status.ToString());

			// Attach progress and completion handlers.
			HandleDownloadAsync(download, false);
		}
	}).then([this](task<void> previousTask)
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

// This function handles downloading opertion asynchronously. Once the downloading is finished, 
// it will launch another task to unzip the zip file.
void MainPage::HandleDownloadAsync(DownloadOperation^ download, boolean start)
{
	IAsyncOperationWithProgress<DownloadOperation^, DownloadOperation^>^ async;
	activeDownloads[download->Guid.GetHashCode()] = download;

	MarshalLog("Running: " + download->Guid);

	if (start)
	{
		async = download->StartAsync();
	}
	else
	{
		async = download->AttachAsync();
	}

	async->Progress = ref new AsyncOperationProgressHandler<DownloadOperation^, DownloadOperation^>(this, &MainPage::DownloadProgress);

	create_task(async, cancellationToken->get_token()).then([this](DownloadOperation^ download)
	{
		ResponseInformation^ response = download->GetResponseInformation();
		MarshalLog("Completed: " + download->Guid + ", Status Code: " + response->StatusCode);

		std::wstring wsUnzipFolder(localFileName->Data());

		size_t lastDotIndex = wsUnzipFolder.find_last_of('.');

		String^ unzipFolderName = ref new String(wsUnzipFolder.substr(0, lastDotIndex).data());

		create_task(StorageFolder::GetFolderFromPathAsync(pickedFolderPath)).then([=](StorageFolder^ pickedFolder)
		{
			create_task(pickedFolder->GetFileAsync(localFileName)).then([=](StorageFile^ localFile)
			{
				create_task(pickedFolder->CreateFolderAsync(unzipFolderName,
					CreationCollisionOption::GenerateUniqueName)).then([=](StorageFolder^ unzipFolder)
				{
					UnZipFileAsync(localFile, unzipFolder);
				}).then([this](task<void> previousTask)
				{
					try
					{
						previousTask.get();
					}
					catch (Exception^ ex)
					{
						MarshalLog("Failed to unzip file.." + ex->Message);
					}
				});
			});

		});

	}).then([this, download](task<void> previousTask)
	{
		try
		{
			previousTask.get();
		}
		catch (Exception^ ex)
		{
			MarshalLog("Handle download" + ex->Message);
		}
		catch (const task_canceled&)
		{
			MarshalLog("Canceled: " + download->Guid);
		}

		// It is important to note, that this method executes on UI thread. This guarantees that
		// access to the activeDownloads is synchronized.
		activeDownloads.erase(download->Guid.GetHashCode());
	});

}

void MainPage::DownloadProgress(IAsyncOperationWithProgress<DownloadOperation^, DownloadOperation^>^ operation, DownloadOperation^ download)
{
	MarshalLog("Progress: " + download->Guid + ", Status: " + download->Progress.Status.ToString());

	UINT64 percent = 0;
	if (download->Progress.TotalBytesToReceive > 0)
	{
		percent = download->Progress.BytesReceived * 100 / download->Progress.TotalBytesToReceive;
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

void MainPage::StartDownload()
{
	// The URI is validated by calling TryGetUri() that will return 'false' for strings that are not valid URIs.
	// Note that when enabling the text box users may provide URIs to machines on the intrAnet that require the
	// "Home or Work Networking" capability.
	
	if (!TryGetUri(ZipFileUrlTextBox->Text, &m_source))
	{
		return;
	}

	FolderPicker^ folderPicker = ref new FolderPicker();
	folderPicker->SuggestedStartLocation = PickerLocationId::Downloads;
	folderPicker->FileTypeFilter->Append(".zip");
	folderPicker->PickFolderAndContinue();
	
}

void MainPage::ContinueFolderPicker(FolderPickerContinuationEventArgs^ args)
{
	BackgroundTransferPriority priority = BackgroundTransferPriority::Default;
	boolean requestUnconstrainedDownload = false;

	StorageFolder ^ destinationFolder = args->Folder;
	if (destinationFolder != nullptr)
	{
		pickedFolderPath = destinationFolder->Path;

		AccessCache::StorageApplicationPermissions::FutureAccessList->AddOrReplace("PickedFolderToken", destinationFolder);
		Log("Picked folder: " + destinationFolder->Name);

		std::wstring wsFileName(Helper::Trim(FileNameField->Text)->Data());

		size_t lastDotIndex = wsFileName.find_last_of('.');

		if (_wcsicmp(wsFileName.substr(lastDotIndex + 1).data(), L"zip") != 0)
		{
			LogStatus("Invalid file type. Please make sure the file type is zip.", NotifyType::ErrorMessage);
			return;
		}

		String^ fileName = ref new String(wsFileName.data());

		create_task(destinationFolder->CreateFileAsync(fileName, CreationCollisionOption::GenerateUniqueName))
			.then([=](StorageFile^ localFile)
		{
			localFileName = localFile->Name;

			BackgroundDownloader^ downloader = ref new BackgroundDownloader();
			DownloadOperation^ download = downloader->CreateDownload(m_source, localFile);

			Log("Downloading " + m_source->AbsoluteUri + " to " + localFile->Name + " with " +
				((priority == BackgroundTransferPriority::Default) ? "default" : "high") +
				" priority, " + download->Guid);

			download->Priority = priority;

			if (!requestUnconstrainedDownload)
			{
				// Attach progress and completion handlers.
				HandleDownloadAsync(download, true);

			}
		}).then([this](task<void> previousTask)
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
	else
	{
		Log("Operation cancelled");
		return;
	}
}

void MainPage::StartDownload_Click(Object^ sender, RoutedEventArgs^ e)
{
	StartDownload();
}

void MainPage::PauseAll_Click(Object^ sender, RoutedEventArgs^ e)
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

void MainPage::ResumeAll_Click(Object^ sender, RoutedEventArgs^ e)
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

void MainPage::CancelAll_Click(Object^ sender, RoutedEventArgs^ e)
{
	Log("Cancelling Downloads: " + activeDownloads.size());

	cancellationToken->cancel();
	cancellationToken = new cancellation_token_source();
	activeDownloads.clear();
}

void MainPage::LogException(String^ title, Exception^ ex)
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
void MainPage::MarshalLog(String^ value)
{
	Dispatcher->RunAsync(CoreDispatcherPriority::Normal, ref new DispatchedHandler([this, value]()
	{
		Log(value);
	}));
}

void MainPage::Log(String^ message)
{
	outputField->Text += message + "\r\n";
}

void MainPage::LogStatus(String^ message, NotifyType type)
{
	NotifyUser(message, type);
	outputField->Text += message + "\r\n";
}

void MainPage::UnZipFileAsync(Windows::Storage::StorageFile^ zipFile, Windows::Storage::StorageFolder^ unzipFolder)
{
	try
	{
		LogStatus("Unziping file: " + zipFile->DisplayName + "...", NotifyType::StatusMessage);
		create_task(ZipHelper::UnZipFileAsync(zipFile, unzipFolder)).then([this, zipFile]()
		{
			LogStatus("Unzip file '" + zipFile->DisplayName + "' successfully!", NotifyType::StatusMessage);
		});

	}
	catch (Exception^ ex)
	{
		LogStatus("Failed to unzip file ..." + ex->Message, NotifyType::ErrorMessage);
	}
}

bool MainPage::TryGetUri(Platform::String^ uriString, Windows::Foundation::Uri^* uri)
{
	// Create a Uri instance and catch exceptions related to invalid input. This method returns 'true'
	// if the Uri instance was successfully created and 'false' otherwise.
	try
	{
		*uri = ref new Windows::Foundation::Uri(Helper::Trim(uriString));
		return true;
	}
	catch (Platform::NullReferenceException^ exception)
	{
		NotifyUser("Error: URI must not be null or empty.", NotifyType::ErrorMessage);
	}
	catch (Platform::InvalidArgumentException^ exception)
	{
		NotifyUser("Error: Invalid URI", NotifyType::ErrorMessage);
	}

	return false;
}

void MainPage::NotifyUser(String^ strMessage, NotifyType type)
{
	switch (type)
	{
	case NotifyType::StatusMessage:
		// Use the status message style.
		statusText->Style = safe_cast<Windows::UI::Xaml::Style^>(this->Resources->Lookup("StatusStyle"));
		break;
	case NotifyType::ErrorMessage:
		// Use the error message style.
		statusText->Style = safe_cast<Windows::UI::Xaml::Style^>(this->Resources->Lookup("ErrorStyle"));
		break;
	default:
		break;
	}
	statusText->Text = strMessage;

	// Collapsed the statusText if it has no text to conserve real estate.
	if (statusText->Text != "")
	{
		statusText->Visibility = Windows::UI::Xaml::Visibility::Visible;
	}
	else
	{
		statusText->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	}
}

void MainPage::Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	auto uri = ref new Uri((String^)((HyperlinkButton^)sender)->Tag);
	Windows::System::Launcher::LaunchUriAsync(uri);
}