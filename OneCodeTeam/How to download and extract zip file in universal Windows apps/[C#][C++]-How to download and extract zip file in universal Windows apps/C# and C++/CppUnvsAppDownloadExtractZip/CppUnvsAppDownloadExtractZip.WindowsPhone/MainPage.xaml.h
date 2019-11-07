/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.h
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

#pragma once

#include "MainPage.g.h"

namespace CppUnvsAppDownloadExtractZip
{
	public enum class NotifyType
	{
		StatusMessage,
		ErrorMessage
	};

	class Helper
	{
	public:

		static Platform::String^ Trim(Platform::String^ s)
		{
			const WCHAR* first = s->Begin();
			const WCHAR* last = s->End();

			while (first != last && iswspace(*first))
			{
				++first;
			}

			while (first != last && iswspace(last[-1]))
			{
				--last;
			}

			return ref new Platform::String(first, static_cast<unsigned int>(last - first));
		}
	};
	public ref class MainPage sealed : IFolderPickerContinuable
	{
	public:
		MainPage();
		virtual void ContinueFolderPicker(FolderPickerContinuationEventArgs^ args);
	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
	
	private:
		Windows::Foundation::Uri^ m_source;
		Concurrency::cancellation_token_source* cancellationToken;
		std::map<int /*Platform::Guid*/, Windows::Networking::BackgroundTransfer::DownloadOperation^> activeDownloads;
		Platform::String^ pickedFolderPath;
		Platform::String^ localFileName;
		~MainPage();
		void StartDownload_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void CancelAll_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void ResumeAll_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void PauseAll_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

		void HandleDownloadAsync(Windows::Networking::BackgroundTransfer::DownloadOperation^ download, boolean start);
		void DownloadProgress(
			Windows::Foundation::IAsyncOperationWithProgress<Windows::Networking::BackgroundTransfer::DownloadOperation^, Windows::Networking::BackgroundTransfer::DownloadOperation^>^ operation,
			Windows::Networking::BackgroundTransfer::DownloadOperation^);

		void DiscoverActiveDownloads();
		void LogException(Platform::String^ title, Platform::Exception^ ex);
		void MarshalLog(Platform::String^ value);
		void Log(Platform::String^ message);
		void LogStatus(Platform::String^ message, NotifyType type);
		void StartDownload();
		void UnZipFileAsync(Windows::Storage::StorageFile^ zipFile, Windows::Storage::StorageFolder^ unzipFolder);
		bool TryGetUri(Platform::String^ uriString, Windows::Foundation::Uri^* uri);
		void NotifyUser(Platform::String^ strMessage, NotifyType type);
		void Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
	};
}
