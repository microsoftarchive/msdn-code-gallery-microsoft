//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

//
// Scenario3.xaml.h
// Declaration of the Scenario3 class
//

#pragma once

#include "pch.h"
#include "Scenario3.g.h"
#include "MainPage.xaml.h"
#include "SampleDataSource.h"

namespace SDKSample
{
	namespace FlipViewCPP
	{
		/// <summary>
		/// An empty page that can be used on its own or navigated to within a Frame.
		/// </summary>
		[Windows::Foundation::Metadata::WebHostHiddenAttribute]
		public ref class Scenario3 sealed
		{
		public:
			Scenario3();

		internal:
			static Scenario3^ Current;

		protected:
			virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
		private:
			MainPage^ rootPage;
			void TOCButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
		};

	
		[Windows::Foundation::Metadata::WebHostHiddenAttribute]
		public ref class ItemSelector sealed :  Windows::UI::Xaml::Controls::DataTemplateSelector
		{
		protected:
			virtual Windows::UI::Xaml::DataTemplate^ SelectTemplateCore(Platform::Object^  item, Windows::UI::Xaml::DependencyObject^ container) override
			{
				using namespace Windows::UI::Xaml;
				using namespace FlipViewData;
				
				DataTemplate^ itemTemplate = dynamic_cast<DataTemplate^>(Scenario3::Current->Resources->Lookup("ImageTemplate"));
				DataTemplate^ tocTemplate =  dynamic_cast<DataTemplate^>(Scenario3::Current->Resources->Lookup("TOCTemplate"));
			

				SampleDataItem^ dataItem = dynamic_cast<SampleDataItem^>(item); 

				if (dataItem != nullptr)
				{
					return itemTemplate;
				}
				else
				{
					return tocTemplate;
				}
			}
		};
	
	}
}
