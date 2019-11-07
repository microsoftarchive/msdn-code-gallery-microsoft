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
// Scenario8.xaml.cpp
// Implementation of the Scenario8 class
//

#include "pch.h"
#include "Scenario8_CaptureBitmap.xaml.h"
#include "S8_Datasource.h"
#include <Robuffer.h>

using namespace SDKSample::WebViewControl;

using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Graphics::Imaging;
using namespace Windows::Storage::Streams;
using namespace concurrency;

Scenario8::Scenario8()
{
    InitializeComponent();
}

// Invoked when this page is about to be displayed in a Frame.
void Scenario8::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
	bookmarks = ref new BookmarkCollection();
	bookmarkList->ItemsSource = bookmarks->items();
	webView8->Navigate(ref new Uri(L"http://www.bing.com"));
}


// Click handler for 'Show Me' button
void Scenario8::bookmarkBtn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	//TypeName scenarioType = {PageWithAppBar::typeid->FullName, TypeKind::Custom};
	//rootPage->Frame->Navigate(scenarioType);
	WebView^ wv = webView8;

	InMemoryRandomAccessStream^ ms = ref new InMemoryRandomAccessStream();
	create_task(webView8->CapturePreviewToStreamAsync(ms)).then([this, ms]()
	{
		//Create a small thumbnail
		int longlength = 180, width = 0, height = 0;
		double srcwidth = webView8->ActualWidth, srcheight = webView8->ActualHeight;
		double factor = srcwidth / srcheight;

		if (factor < 1)
		{
			height = longlength;
			width = (int) (longlength * factor);
		}
		else
		{
			width = longlength;
			height = (int) (longlength / factor);
		}

		create_task(resize(width, height, ms)).then([this](BitmapSource^ smallimg)
		{
			BookmarkItem^ item = ref new BookmarkItem(webView8->DocumentTitle, smallimg, webView8->Source);
			bookmarks->Add(item);
		});
	});
}

task<WriteableBitmap^> Scenario8::resize(int width, int height, Windows::Storage::Streams::IRandomAccessStream^ source)
{
	WriteableBitmap^ smallimg = ref new WriteableBitmap(width, height);
	BitmapTransform^ transform = ref new BitmapTransform();
	transform->ScaledHeight = (unsigned int) height;
	transform->ScaledWidth = (unsigned int) width;
	return create_task(BitmapDecoder::CreateAsync(source)).then([this, transform](BitmapDecoder^ decoder)
	{
		// transform.InterpolationMode = BitmapInterpolationMode.NearestNeighbor;
		return create_task(decoder->GetPixelDataAsync(
			BitmapPixelFormat::Bgra8,
			BitmapAlphaMode::Straight,
			transform,
			ExifOrientationMode::RespectExifOrientation,
			ColorManagementMode::DoNotColorManage)
			);
	}).then([this,smallimg](PixelDataProvider^ pixelData)
	{
		Platform::Array<byte>^ srcPixels = pixelData->DetachPixelData();
		
		//copy from srcPixels to smallimg->pixelBuffer;
		auto bufferInspectable = reinterpret_cast<IInspectable*>(smallimg->PixelBuffer);
		IBufferByteAccess* bufferInternal;
		bufferInspectable->QueryInterface(IID_PPV_ARGS(&bufferInternal));
		bufferInspectable->Release();
		byte* destPixels;
		bufferInternal->Buffer(&destPixels);

		memcpy(destPixels, srcPixels->Data, srcPixels->Length);

		return smallimg;
	});
}

void Scenario8::bookmarkList_ItemClick(Platform::Object^ sender, ItemClickEventArgs^ e)
{
	BookmarkItem^ b = (BookmarkItem ^) e->ClickedItem;
	webView8->Navigate(b->PageUrl);
}

