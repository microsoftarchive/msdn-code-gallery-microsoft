/****************************** Module Header ******************************\
* Module Name: MainPage.xaml.cpp
* Project:     CppUploadImage
* Copyright (c) Microsoft Corporation.
*
* An application that selects a local image and uploads onto the app
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
#include <ppltasks.h>

using namespace CppUploadImage;

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
using namespace Windows::Storage::Pickers;
using namespace concurrency;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Storage::Streams;

MainPage::MainPage()
{
	InitializeComponent();
}


void CppUploadImage::MainPage::UploadButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	FileOpenPicker^ picker = ref new FileOpenPicker();
	picker->ViewMode = PickerViewMode::Thumbnail;
	picker->SuggestedStartLocation = PickerLocationId::PicturesLibrary;
	picker->FileTypeFilter->Append(".jpg");
	picker->FileTypeFilter->Append(".jpeg");
	picker->FileTypeFilter->Append(".png");

	create_task(picker->PickSingleFileAsync()).then([this](Windows::Storage::StorageFile^ file) {
		if (file) {
			// Application now has read/write access to the picked file
			imagePath->Text = file->Path;

			// Open a stream for the selected file.
			create_task(file->OpenAsync(Windows::Storage::FileAccessMode::Read)).then([this](IRandomAccessStream^ filestream) {
				// Create BitmapImage of file and set the image source
				BitmapImage^ bitmapImage = ref new BitmapImage();
				bitmapImage->SetSource(filestream);
				uploadedImage->Source = bitmapImage;
			});

		}
	});
}
