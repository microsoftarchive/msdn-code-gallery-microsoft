/****************************** Module Header ******************************\
* Module Name:    MainPage.xaml.h
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

#pragma once

#include "MainPage.g.h"

namespace CppUnvsAppAccessControlInDT
{
	[Windows::UI::Xaml::Data::Bindable]
	public ref class VideoInfo sealed
	{
	public:
		VideoInfo(Platform::String^ name, Platform::String^ link)
		{
			Name = name;
			Link = link;
		}
		property Platform::String^ Name;
		property Platform::String^ Link;
	};

	public ref class MainPage sealed
	{
	public:
		MainPage();

	private:
		Platform::Collections::Vector<Windows::UI::Xaml::Controls::Grid^>^ m_renderedGrids;
		Windows::UI::Xaml::FrameworkElement^ GetChildByName(Windows::UI::Xaml::DependencyObject^ parent, Platform::String^ name);

		void Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void Grid_Loaded(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void Page_SizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);
	};
}
