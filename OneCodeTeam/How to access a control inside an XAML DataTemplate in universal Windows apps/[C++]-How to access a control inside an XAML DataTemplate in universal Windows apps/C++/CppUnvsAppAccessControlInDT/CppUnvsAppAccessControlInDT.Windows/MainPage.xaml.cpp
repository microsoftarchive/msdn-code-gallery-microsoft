/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.cpp
* Project:        CppUnvsAppAccessControlInDT.Windows
* Copyright (c) Microsoft Corporation.
*
* This sample demonstrates how to access control inside DataTemplate in universal Windows apps.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include "pch.h"
#include "MainPage.xaml.h"

using namespace CppUnvsAppAccessControlInDT;

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


MainPage::MainPage()
{
	InitializeComponent();

	m_renderedGrids = ref new Platform::Collections::Vector < Grid^ > ;

	auto videos = ref new Platform::Collections::Vector < VideoInfo^ > ;

	videos->Append(ref new VideoInfo("How to pick and manipulate a 3D object in universal Windows game apps", 
		"http://video.ch9.ms/ch9/b994/95d5d113-f8d0-4489-91f1-6943eeabb994/OnecodeGridview_mid.mp4"));
	videos->Append(ref new VideoInfo("How to add wartermark text or image to a bitmap in Windows Store app",
		"http://video.ch9.ms/ch9/024c/988b0a77-acbf-4b8c-abc9-c460079c024c/AddWatermarkToBitmap_mid.mp4"));
	videos->Append(ref new VideoInfo("How to Add an Item Dynamically to a Grouped GridView in a Windows Store app",
		"http://video.ch9.ms/ch9/b994/95d5d113-f8d0-4489-91f1-6943eeabb994/OnecodeGridview_mid.mp4"));

	gridView->ItemsSource = videos;
}


void MainPage::Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	Windows::System::Launcher::LaunchUriAsync(ref new Uri(((HyperlinkButton^)sender)->Tag->ToString()));
}


void MainPage::Grid_Loaded(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	// Add the Grids inside DataTemplate into a List.
	m_renderedGrids->Append((Grid^)sender);
}


void MainPage::Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	try
	{
		int index = _wtoi(tbVideoIndex->Text->Data()) - 1;
		if (index > -1)
		{
			Grid^ currentGrid = m_renderedGrids->GetAt(index);

			MediaElement^ myVideo = safe_cast<MediaElement^>(GetChildByName(currentGrid, "myVideo"));

			myVideo->Play();
		}		
	}
	catch (Exception^ exception)
	{
		statusText->Text = exception->Message;
	}
}

FrameworkElement^ MainPage::GetChildByName(DependencyObject^ parent, String^ name)
{	
	int i = 0;
	for (; i < VisualTreeHelper::GetChildrenCount(parent); ++i)
	{
		auto child = VisualTreeHelper::GetChild(parent, i);

		if (child != nullptr)
		{
			if (safe_cast<FrameworkElement^>(child)->Name == name)
			{
				return safe_cast<FrameworkElement^>(child);
			}
			/*else
			{
				FrameworkElement^ control = GetChildByName(child, name);
				if (control != nullptr)
					return control;
			}*/
		}
		
	}
	return nullptr;
}

void MainPage::Page_SizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e)
{
	if (e->NewSize.Width < 800)
	{
		VisualStateManager::GoToState(this, "MinimalLayout", true);
	}
	else if (e->NewSize.Width < e->NewSize.Height)
	{
		VisualStateManager::GoToState(this, "PortraitLayout", true);
	}
	else
	{
		VisualStateManager::GoToState(this, "DefaultLayout", true);
	}
}
