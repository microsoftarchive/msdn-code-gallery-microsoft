/****************************** Module Header ******************************\
* Module Name:  MainPage.xaml.h
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
#pragma once

#include "MainPage.g.h"

namespace CppUniveralAppWebViewZoom
{
	
	public ref class MainPage sealed
	{
	public:
		MainPage();

	private:
		void Slider_ValueChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::Primitives::RangeBaseValueChangedEventArgs^ e);
		void Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void WindowSizeChanged(Platform::Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e);
	};
}
