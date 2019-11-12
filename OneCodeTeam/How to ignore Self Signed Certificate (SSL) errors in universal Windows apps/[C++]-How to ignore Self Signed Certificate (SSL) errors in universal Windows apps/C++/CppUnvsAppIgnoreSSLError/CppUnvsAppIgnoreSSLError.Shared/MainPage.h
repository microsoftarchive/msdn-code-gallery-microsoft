/****************************** Module Header ******************************\
* Module Name:  MainPage.h
* Project:      CppUnvsAppIgnoreSSLError
* Copyright (c) Microsoft Corporation.
*
* This sample demonstrates how to ignore SSL errors in universal Windows apps.
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

namespace CppUnvsAppIgnoreSSLError
{
	partial ref class MainPage
	{
	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

		virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

#if WINAPI_FAMILY==WINAPI_FAMILY_PC_APP
		void Page_SizeChanged(Platform::Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);
#endif
	public:
		void Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

	private:
		Windows::Web::Http::HttpClient^ m_httpClient;
		Windows::Web::Http::Filters::HttpBaseProtocolFilter^ m_httpBaseProtocolFilter;
		void Button_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void btnIgnore_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
	};
}
