/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.cpp
* Project:      CppUniveralAppWebViewZoom.Windows
* Copyright (c) Microsoft Corporation.
*
* This code sample shows you how to zoom in/out the content of WebView in Universal apps.
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

using namespace CppUniveralAppWebViewZoom;

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
using namespace concurrency;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

MainPage::MainPage()
{
	InitializeComponent();
	Window::Current->SizeChanged += ref new WindowSizeChangedEventHandler(this, &MainPage::WindowSizeChanged);
}

void MainPage::WindowSizeChanged(Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e)
{
	(void)sender;	// Unused parameter

	if (e->Size.Width <= 500)
	{
		VisualStateManager::GoToState(this, "MinimalLayout", true);
	}
	else if (e->Size.Width < e->Size.Height)
	{
		VisualStateManager::GoToState(this, "PortraitLayout", true);
	}
	else
	{
		VisualStateManager::GoToState(this, "DefaultLayout", true);
	}
}

void CppUniveralAppWebViewZoom::MainPage::Slider_ValueChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e)
{
	if (MyWebView != nullptr)
	{
		Platform::Collections::Vector<String^>^ arguments = ref new Platform::Collections::Vector<String^>(1);

		String^ s1 = "ZoomFunction(";
		String^ s2 = String::Concat(s1, e->NewValue.ToString()) + ");";

		arguments->SetAt(0, s2);

		// Invoke the javascript function called 'ZoomFunction' that is loaded into the WebView.
		create_task(MyWebView->InvokeScriptAsync(L"eval", arguments)).then([this](String ^result)
		{
			
		})
			.then([](task<void> t)
		{
			try
			{
				t.get();
			}
			catch (Platform::Exception^ ex)
			{
				// An exception can be thrown if a webpage has not been loaded into the WebView or no javascript function named "ZoomFunction" is found in the webpage.
				Platform::Details::Console::WriteLine(ex->Message);
			}
		});
	}
}


void CppUniveralAppWebViewZoom::MainPage::Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	auto uri = ref new Uri((String^)((HyperlinkButton^)sender)->Tag);
	Windows::System::Launcher::LaunchUriAsync(uri);
}
