/****************************** Module Header ******************************\
* Module Name:  ItemsPage.xaml.h
* Project:      CppWindowsStoreAppFlightDataFilter
* Copyright (c) Microsoft Corporation.
*
* ItemsPage.
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

#include "ItemsPage.g.h"
#include "Common\NavigationHelper.h"
#include "ViewModel\MainViewModel.h"
#include "Converters\DoubleToMoneyString.h"
#include "ViewModel\ViewModelLocator.h"

using namespace CppWindowsStoreAppFlightDataFilter::Converters;
using namespace CppWindowsStoreAppFlightDataFilter::ViewModel;

namespace CppWindowsStoreAppFlightDataFilter
{
	/// <summary>
	/// A page that displays a collection of item previews.  In the Split Application this page
	/// is used to display and select one of the available groups.
	/// </summary>
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class ItemsPage sealed
	{
	public:
		ItemsPage();

		/// <summary>
		/// This can be changed to a strongly typed view model.
		/// </summary>
		property MainViewModel^ DefaultViewModel
		{
			MainViewModel^  get();
		}

		/// <summary>
		/// NavigationHelper is used on each page to aid in navigation and 
		/// process lifetime management
		/// </summary>
		property Common::NavigationHelper^ NavigationHelper
		{
			Common::NavigationHelper^ get();
		}

	protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
		virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

	private:
		void LoadState(Platform::Object^ sender, Common::LoadStateEventArgs^ e);
		MainViewModel^ _defaultViewModel;

		static Windows::UI::Xaml::DependencyProperty^ _defaultViewModelProperty;
		static Windows::UI::Xaml::DependencyProperty^ _navigationHelperProperty;
		void Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		void WindowSizeChanged(Platform::Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e);
	};
}
