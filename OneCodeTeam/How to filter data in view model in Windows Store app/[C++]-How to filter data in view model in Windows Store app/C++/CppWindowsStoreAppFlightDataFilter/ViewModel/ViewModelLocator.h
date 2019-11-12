/****************************** Module Header ******************************\
* Module Name:  FlightDataItem.h
* Project:      CppWindowsStoreAppFlightDataFilter
* Copyright (c) Microsoft Corporation.
*
* ViewModelLocator.
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
#include "MainViewModel.h"
using namespace CppWindowsStoreAppFlightDataFilter::ViewModel;

namespace CppWindowsStoreAppFlightDataFilter
{
	namespace ViewModel
	{
		[Windows::UI::Xaml::Data::Bindable]
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class ViewModelLocator sealed
		{
		public:
			//ViewModelLocator();

			property MainViewModel^ Main
			{
				MainViewModel^ get()
				{
					if (nullptr == _main)
					{
						_main = ref new MainViewModel();
					}
					return _main;
				}
			}


		private:
			MainViewModel^ _main;

		};
	}
}