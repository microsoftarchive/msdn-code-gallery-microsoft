/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.cpp
* Project:      CppUniversalAppImageToVideo
* Copyright (c) Microsoft Corporation.
*
* This sample shows how to encode several images to a video using Media Foundation.
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
using namespace CppUniversalAppImageToVideo;

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

using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace concurrency;
using namespace Windows::Graphics::Imaging;
using namespace Windows::UI::Popups;
using namespace EncodeImage;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

MainPage::MainPage() : m_videoWidth(640), m_videoHeight(480)
{
	InitializeComponent();

	m_images = ref new Platform::Collections::Vector < Windows::UI::Xaml::Controls::Image^ >;
	m_files = ref new Platform::Collections::Vector < StorageFile^ > ;
	ImageGV->DataContext = m_images;
}

void CppUniversalAppImageToVideo::MainPage::ImageBtn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	statusText->Text = "";
	if (m_images->Size != 0)
	{
		m_images->Clear();
	}
	if (m_files->Size != 0)
	{
		m_files->Clear();
	}
	// Open images.
	FileOpenPicker^ picker = ref new FileOpenPicker;
	picker->SuggestedStartLocation = PickerLocationId::PicturesLibrary;
	picker->ViewMode = PickerViewMode::Thumbnail;
	picker->FileTypeFilter->Append(".jpg");
	picker->FileTypeFilter->Append(".png");
	picker->FileTypeFilter->Append(".bmp");
	create_task(picker->PickMultipleFilesAsync()).then([=](IVectorView<StorageFile^>^ files){
		if (files->Size == 0)
		{
			cancel_current_task();
		}
		
		auto images = std::make_shared<Platform::Collections::Vector<Windows::UI::Xaml::Controls::Image^>^>(m_images);
		for (StorageFile^ file : files)
		{			
			create_task(file->OpenAsync(FileAccessMode::Read)).then([=](Streams::IRandomAccessStream^ stream){
				
				auto bitmapImage = ref new BitmapImage();
				bitmapImage->SetSource(stream);
				Image^ xamlImage = ref new Image;
				xamlImage->Source = bitmapImage;
				m_images->Append(xamlImage);
			}).then([=](){
				m_files->Append(file);
			}, task_continuation_context::use_arbitrary()).then([=](task<void> t){
				try
				{
					t.get();
				}
				catch (InvalidArgumentException^ e)
				{
					statusText->Text = "Some errors occur when openning, please try again";
					m_images->Clear();
					m_files->Clear();
				}
			});
		}
	});
}

void CppUniversalAppImageToVideo::MainPage::ImageGV_ItemClick(Platform::Object^ sender, Windows::UI::Xaml::Controls::ItemClickEventArgs^ e)
{
	unsigned int index = 0;
	m_images->IndexOf((Image^)(e->ClickedItem), &index);
	m_images->RemoveAt(index);
	m_files->RemoveAt(index);
}

void CppUniversalAppImageToVideo::MainPage::EncodeBtn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if (m_files->Size == 0)
	{
		statusText->Text = "You must select one image at least.";
		return;
	}
	// Create the video via file picker.
	statusText->Text = "";

	FileSavePicker^ picker = ref new FileSavePicker;
	picker->SuggestedStartLocation = PickerLocationId::VideosLibrary;
	auto mp4Extensions = ref new Platform::Collections::Vector<Platform::String^>();
	mp4Extensions->Append(".mp4");
	picker->FileTypeChoices->Insert("MP4 file", mp4Extensions);
	picker->DefaultFileExtension = ".mp4";
	picker->SuggestedFileName = "output";
	picker->SuggestedStartLocation = PickerLocationId::VideosLibrary;

	
	create_task( picker->PickSaveFileAsync())
	.then([=](StorageFile^ file){
		if (nullptr == file)
		{
			cancel_current_task();
		}
		m_videoFile = file;
		return file->OpenAsync(FileAccessMode::ReadWrite);
	}).then([=](Streams::IRandomAccessStream^ stream){		
		m_picture = ref new PictureWriter(stream, m_videoWidth, m_videoHeight);
	}).then([this](){

		// Add frames to the video.		
		ProcessVideoRing->IsActive = true;
		statusText->Text = "Encoding...";
		static int imageWidth, imageHeight, width, height;
		
		create_task([=](){
			for (StorageFile^ file : m_files)
			{
				// We set 10 FPS default in the PictureWriter, so we add 10 same frames with each image.
				for (int i = 0; i < 10; ++i)
				{
					create_task(file->Properties->GetImagePropertiesAsync()).then([&](FileProperties::ImageProperties^ properties){
						imageWidth = properties->Width;
						imageHeight = properties->Height;
						return file->OpenAsync(FileAccessMode::Read);
					}).then([=](Streams::IRandomAccessStream^ stream){
						return BitmapDecoder::CreateAsync(stream);
					}).then([&](BitmapDecoder^ decoder){
						float scaleOfWidth = static_cast<float>(m_videoWidth) / imageWidth;
						float scaleOfHeight = static_cast<float>(m_videoHeight) / imageHeight;
						float scale = scaleOfHeight > scaleOfWidth ?
						scaleOfWidth : scaleOfHeight;
						width = static_cast<int>(imageWidth * scale);
						height = static_cast<int>(imageHeight * scale);
						
						BitmapTransform^ transform = ref new BitmapTransform;
						transform->ScaledWidth = width;
						transform->ScaledHeight = height;
						return decoder->GetPixelDataAsync(BitmapPixelFormat::Bgra8,
							BitmapAlphaMode::Straight,
							transform,
							ExifOrientationMode::RespectExifOrientation,
							ColorManagementMode::ColorManageToSRgb);
					}).then([&](PixelDataProvider^ provider){
						m_picture->AddFrame(provider->DetachPixelData(), width, height);
					}).wait();
				}				
			}
		}).then([=](){
			m_picture->Finalize();
			m_picture = nullptr;			
		}).then([=](){
			return m_videoFile->OpenAsync(FileAccessMode::Read);
		}).then([=](Streams::IRandomAccessStream^ stream){
			VideoElement->SetSource(stream, nullptr);
			
			ProcessVideoRing->IsActive = false;
			statusText->Text = "The image files are encoded successfully.";			
		});
	});	
}

void CppUniversalAppImageToVideo::MainPage::Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Windows::System::Launcher::LaunchUriAsync(ref new Uri((safe_cast<HyperlinkButton^>(sender))->Tag->ToString()));
}


void CppUniversalAppImageToVideo::MainPage::Page_SizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e)
{
	if (e->NewSize.Width < 800)
	{
		VisualStateManager::GoToState(this, "MinimalLayout", true);
	}
	else
	{
		VisualStateManager::GoToState(this, "DefaultLayout", true);
	}
}






