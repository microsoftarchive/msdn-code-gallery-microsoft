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
// Scenario2.xaml.cpp
// Implementation of the Scenario2 class
//

#include "pch.h"
#include "Scenario2.xaml.h"
#include <ppltasks.h>
#include <stdio.h>
#include <stdlib.h>

using namespace SDKSample::Images;

using namespace std;
using namespace concurrency;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage::Pickers;
using namespace Windows::Storage;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml::Media::Imaging;

Scenario2::Scenario2()
{
    InitializeComponent();
	Scenario2Button1->Click += ref new RoutedEventHandler(this, &Scenario2::Scenario2Button1_Click);
	Scenario2DecodePixelHeight->Text = "100";
	Scenario2DecodePixelWidth->Text = "100";
}

// Invoked when this page is about to be displayed in a Frame.

void Scenario2::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::Images::Scenario2::Scenario2Button1_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	
	// Try to parse an integer from the given text. If invalid, default to 100px
	auto str = Scenario2DecodePixelHeight->Text;
	decodePixelHeight = stoi(str->Data());

	if (decodePixelHeight == 0)
	{
	    Scenario2DecodePixelHeight->Text = "100";
	    decodePixelHeight = 100;
	}
	
	// Try to parse an integer from the given text. If invalid, default to 100px
	auto str2 = Scenario2DecodePixelWidth->Text;
	decodePixelWidth = stoi(str2->Data());

	if (decodePixelWidth == 0)
	{
	    Scenario2DecodePixelWidth->Text = "100";
	    decodePixelWidth = 100;
	}
	
	//  Initialize file picker
	auto open = ref new FileOpenPicker();
	open->SuggestedStartLocation = PickerLocationId::PicturesLibrary;
	open->ViewMode = PickerViewMode::Thumbnail;
	
	// Filter to include a sample subset of file types
	open->FileTypeFilter->Clear();
	open->FileTypeFilter->Append(".bmp");
	open->FileTypeFilter->Append(".png");
	open->FileTypeFilter->Append(".jpeg");
	open->FileTypeFilter->Append(".jpg");
	
	
	// Open a stream for the selected file
	create_task(open->PickSingleFileAsync()).then([this](StorageFile^ file)
	{
	    if (file)
	    {
			// Ensure the stream is disposed once the image is loaded
			create_task(file->OpenAsync(Windows::Storage::FileAccessMode::Read)).then([this](IRandomAccessStream^ fileStream)
			{
			   // Set the image source to the selected bitmap
			   auto bitmapImage = ref new BitmapImage();
			   bitmapImage->DecodePixelHeight = decodePixelHeight;
			   bitmapImage->DecodePixelWidth = decodePixelWidth;
			   
			   bitmapImage->SetSource(fileStream);
			   Scenario2Image->Source = bitmapImage;
			});

	    }
	});
}