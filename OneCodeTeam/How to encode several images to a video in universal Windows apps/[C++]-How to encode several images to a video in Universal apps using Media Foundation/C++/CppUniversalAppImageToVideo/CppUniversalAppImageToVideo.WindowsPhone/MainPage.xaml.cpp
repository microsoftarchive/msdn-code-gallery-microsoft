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
#include "ImagesPage.xaml.h"
#include "ppl.h"
using namespace CppUniversalAppImageToVideo;
using namespace CppUniversalAppImageToVideo::Common;
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


using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::Storage;
using namespace Windows::Storage::Pickers;
using namespace concurrency;
using namespace Windows::Graphics::Imaging;
using namespace Windows::UI::Popups;
using namespace EncodeImage;


MainPage::MainPage() : m_videoWidth(640), m_videoHeight(480)
{
	InitializeComponent();
	/*Window::Current->SizeChanged += */
	SetValue(_defaultViewModelProperty, ref new Platform::Collections::Map<String^, Object^>(std::less<String^>()));
	auto navigationHelper = ref new Common::NavigationHelper(this);
	SetValue(_navigationHelperProperty, navigationHelper);
	navigationHelper->LoadState += ref new Common::LoadStateEventHandler(this, &MainPage::LoadState);

	m_files = ref new Platform::Collections::Vector<StorageFile^>;
	
}
DependencyProperty^ MainPage::_defaultViewModelProperty =
DependencyProperty::Register("DefaultViewModel",
TypeName(IObservableMap<String^, Object^>::typeid), TypeName(MainPage::typeid), nullptr);

/// <summary>
/// Used as a trivial view model.
/// </summary>
IObservableMap<String^, Object^>^ MainPage::DefaultViewModel::get()
{
	return safe_cast<IObservableMap<String^, Object^>^>(GetValue(_defaultViewModelProperty));
}

DependencyProperty^ MainPage::_navigationHelperProperty =
DependencyProperty::Register("NavigationHelper",
TypeName(Common::NavigationHelper::typeid), TypeName(MainPage::typeid), nullptr);

/// <summary>
/// Gets an implementation of <see cref="NavigationHelper"/> designed to be
/// used as a trivial view model.
/// </summary>
NavigationHelper^ MainPage::NavigationHelper::get()
{
	return safe_cast<Common::NavigationHelper^>(GetValue(_navigationHelperProperty));
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void MainPage::OnNavigatedTo(NavigationEventArgs^ e)
{
	NavigationHelper->OnNavigatedTo(e);
	
}
/// <summary>
/// Populates the page with content passed during navigation.  Any saved state is also
/// provided when recreating a page from a prior session.
/// </summary>
/// <param name="sender">
/// The source of the event; typically <see cref="NavigationHelper"/>
/// </param>
/// <see cref="Frame::Navigate(Type, Object)"/> when this page was initially requested and
/// a dictionary of state preserved by this page during an earlier
/// session.  The state will be null the first time a page is visited.</param>
void MainPage::LoadState(Object^ sender, LoadStateEventArgs^ e)
{
	if (e->NavigationParameter != nullptr)
	{
		Platform::Collections::Vector<StorageFile^>^ files =
			(Platform::Collections::Vector<StorageFile^>^)e->NavigationParameter;
		m_files = files;
		statusText->Text = "Images opened.";
	}
}


void CppUniversalAppImageToVideo::MainPage::ContinueFileSavePicker(FileSavePickerContinuationEventArgs^ args)
{
	auto file = args->File;
	if (file != nullptr)
	{
		// Prevent updates to the remote version of the file until we finish making changes and call CompleteUpdatesAsync.
		CachedFileManager::DeferUpdates(file);

		create_task(file->OpenAsync(FileAccessMode::ReadWrite)).then([=](Streams::IRandomAccessStream^ stream){
			m_picture = ref new PictureWriter(stream, m_videoWidth, m_videoHeight);
		}).then([=](){
			ProcessVideoRing->IsActive = true;
			VideoElement->AreTransportControlsEnabled = false;
			statusText->Text = "Encoding...";
			ImageBtn->IsEnabled = false;
			EncodeBtn->IsEnabled = false;

			static int imageWidth, imageHeight, width, height;
			static int videoHeight = m_videoHeight;
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
				return file->OpenAsync(FileAccessMode::Read);
			}).then([=](Streams::IRandomAccessStream^ stream){
				VideoElement->SetSource(stream, nullptr);
				ProcessVideoRing->IsActive = false;
				VideoElement->AreTransportControlsEnabled = true;
				ImageBtn->IsEnabled = true;
				EncodeBtn->IsEnabled = true;
				statusText->Text = "The image files are encoded successfully.";
				m_files->Clear();
			});
		});
	
	}
}

void CppUniversalAppImageToVideo::MainPage::ImageBtn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	statusText->Text = "";
	FileOpenPicker^ picker = ref new FileOpenPicker;
	picker->SuggestedStartLocation = PickerLocationId::PicturesLibrary;
	picker->ViewMode = PickerViewMode::Thumbnail;
	picker->FileTypeFilter->Append(".jpg");
	picker->FileTypeFilter->Append(".png");
	picker->FileTypeFilter->Append(".bmp");
	picker->PickMultipleFilesAndContinue();	
}

/// <summary>
/// Handle the returned files from file picker
/// This method is triggered by ContinuationManager based on ActivationKind
/// </summary>
/// <param name="args">File open picker continuation activation argment. It cantains the list of files user selected with file open picker </param>
void CppUniversalAppImageToVideo::MainPage::ContinueFileOpenPicker(FileOpenPickerContinuationEventArgs^ args)
{
	if (args->Files->Size > 0)
	{
		for (StorageFile^ file : args->Files)
		{
			m_files->Append(file);
		}

		this->Frame->Navigate(TypeName(ImagesPage::typeid), m_files);			
	}
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

	FileSavePicker^ savePicker = ref new FileSavePicker;
	savePicker->SuggestedStartLocation = PickerLocationId::VideosLibrary;
	auto mp4Extensions = ref new Platform::Collections::Vector<Platform::String^>();
	mp4Extensions->Append(".mp4");
	savePicker->FileTypeChoices->Insert("MP4 file", mp4Extensions);
	savePicker->DefaultFileExtension = ".mp4";
	savePicker->SuggestedFileName = "output";
	savePicker->SuggestedStartLocation = PickerLocationId::VideosLibrary;
	MessageDialog^ dialog = ref new MessageDialog("Create new video first.");
	dialog->Commands->Append(ref new UICommand("Ok"));
	create_task(dialog->ShowAsync()).then([=](IUICommand^ command){
		savePicker->PickSaveFileAndContinue();
	});
	
}


void CppUniversalAppImageToVideo::MainPage::Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Windows::System::Launcher::LaunchUriAsync(ref new Uri((safe_cast<HyperlinkButton^>(sender))->Tag->ToString()));
}


void CppUniversalAppImageToVideo::MainPage::Page_SizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e)
{
	if (e->NewSize.Height < e->NewSize.Width)
	{
		VisualStateManager::GoToState(this, "PortraitlLayout", true);
	}
	else
	{
		VisualStateManager::GoToState(this, "DefaultLayout", true);
	}
}
