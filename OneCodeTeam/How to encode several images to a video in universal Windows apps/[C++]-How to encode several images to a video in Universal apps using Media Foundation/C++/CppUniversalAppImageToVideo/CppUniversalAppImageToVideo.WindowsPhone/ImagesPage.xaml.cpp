/****************************** Module Header ******************************\
* Module Name:  ImagesPage.xaml.cpp
* Project:      CppUniversalAppImageToVideo
* Copyright (c) Microsoft Corporation.
*
* This page is used to display selected images.
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
#include "ImagesPage.xaml.h"
#include "MainPage.xaml.h"

using namespace CppUniversalAppImageToVideo;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage;
using namespace concurrency;
using namespace Windows::UI::Xaml::Media::Imaging;
// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

ImagesPage::ImagesPage()
{
	InitializeComponent();
	SetValue(_defaultViewModelProperty, ref new Platform::Collections::Map<String^, Object^>(std::less<String^>()));
	auto navigationHelper = ref new Common::NavigationHelper(this);
	SetValue(_navigationHelperProperty, navigationHelper);
	navigationHelper->LoadState += ref new Common::LoadStateEventHandler(this, &ImagesPage::LoadState);
	navigationHelper->SaveState += ref new Common::SaveStateEventHandler(this, &ImagesPage::SaveState);

	m_files = ref new Platform::Collections::Vector<StorageFile^>;
	m_images = ref new Platform::Collections::Vector < Windows::UI::Xaml::Controls::Image^ >;
	ImageGV->DataContext = m_images;
}

DependencyProperty^ ImagesPage::_defaultViewModelProperty =
DependencyProperty::Register("DefaultViewModel",
TypeName(IObservableMap<String^, Object^>::typeid), TypeName(ImagesPage::typeid), nullptr);

/// <summary>
/// Used as a trivial view model.
/// </summary>
IObservableMap<String^, Object^>^ ImagesPage::DefaultViewModel::get()
{
	return safe_cast<IObservableMap<String^, Object^>^>(GetValue(_defaultViewModelProperty));
}

DependencyProperty^ ImagesPage::_navigationHelperProperty =
DependencyProperty::Register("NavigationHelper",
TypeName(Common::NavigationHelper::typeid), TypeName(ImagesPage::typeid), nullptr);

/// <summary>
/// Gets an implementation of <see cref="NavigationHelper"/> designed to be
/// used as a trivial view model.
/// </summary>
Common::NavigationHelper^ ImagesPage::NavigationHelper::get()
{
	return safe_cast<Common::NavigationHelper^>(GetValue(_navigationHelperProperty));
}

#pragma region Navigation support

/// The methods provided in this section are simply used to allow
/// NavigationHelper to respond to the page's navigation methods.
/// 
/// Page specific logic should be placed in event handlers for the  
/// <see cref="NavigationHelper::LoadState"/>
/// and <see cref="NavigationHelper::SaveState"/>.
/// The navigation parameter is available in the LoadState method 
/// in addition to page state preserved during an earlier session.

void ImagesPage::OnNavigatedTo(NavigationEventArgs^ e)
{
	NavigationHelper->OnNavigatedTo(e);
}

void ImagesPage::OnNavigatedFrom(NavigationEventArgs^ e)
{
	NavigationHelper->OnNavigatedFrom(e);
}

#pragma endregion

/// <summary>
/// Populates the page with content passed during navigation. Any saved state is also
/// provided when recreating a page from a prior session.
/// </summary>
/// <param name="sender">
/// The source of the event; typically <see cref="NavigationHelper"/>
/// </param>
/// <param name="e">Event data that provides both the navigation parameter passed to
/// <see cref="Frame::Navigate(Type, Object)"/> when this page was initially requested and
/// a dictionary of state preserved by this page during an earlier
/// session. The state will be null the first time a page is visited.</param>
void ImagesPage::LoadState(Object^ sender, Common::LoadStateEventArgs^ e)
{
	m_files = (Platform::Collections::Vector<Windows::Storage::StorageFile^>^)e->NavigationParameter;
	auto images = std::make_shared<Platform::Collections::Vector<Windows::UI::Xaml::Controls::Image^>^>(m_images);
	for (StorageFile^ file : m_files)
	{
		create_task(file->OpenAsync(FileAccessMode::Read)).then([=](Streams::IRandomAccessStream^ stream){
			auto bitmapImage = ref new BitmapImage();
			bitmapImage->SetSource(stream);
			Image^ xamlImage = ref new Image;
			xamlImage->Source = bitmapImage;
			(*images)->Append(xamlImage);
		}).then([=](task<void> t){
			try
			{
				t.get();
			}
			catch (InvalidArgumentException^ e)
			{				
				m_images->Clear();				
			}
		});
	}
	
	
}

/// <summary>
/// Preserves state associated with this page in case the application is suspended or the
/// page is discarded from the navigation cache.  Values must conform to the serialization
/// requirements of <see cref="SuspensionManager::SessionState"/>.
/// </summary>
/// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
/// <param name="e">Event data that provides an empty dictionary to be populated with
/// serializable state.</param>
void ImagesPage::SaveState(Object^ sender, Common::SaveStateEventArgs^ e){
	(void) sender;	// Unused parameter
	(void) e; // Unused parameter
}


void CppUniversalAppImageToVideo::ImagesPage::ImageGV_ItemClick(Platform::Object^ sender, Windows::UI::Xaml::Controls::ItemClickEventArgs^ e)
{
	unsigned int index = 0;
	m_images->IndexOf((Image^)(e->ClickedItem), &index);
	m_images->RemoveAt(index);
	m_files->RemoveAt(index);
}


void CppUniversalAppImageToVideo::ImagesPage::OkBtn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	this->Frame->Navigate(TypeName(MainPage::typeid), m_files);
}
