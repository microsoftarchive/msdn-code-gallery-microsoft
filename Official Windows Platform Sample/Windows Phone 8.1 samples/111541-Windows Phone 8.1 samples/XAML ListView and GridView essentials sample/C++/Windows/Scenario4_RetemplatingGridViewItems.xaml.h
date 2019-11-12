//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario4.xaml.h
// Declaration of the Scenario4 class
//

#pragma once

#include "pch.h"
#include "Scenario4_RetemplatingGridViewItems.g.h"
#include "MainPage.xaml.h"
#include "SampleDataSource.h"

namespace SDKSample
{
    namespace ListViewSimple
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
		[Windows::Foundation::Metadata::WebHostHiddenAttribute]
		public ref class Scenario4 sealed
        {
        public:
            Scenario4();

		protected:
			virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
			MainPage^ rootPage;
			ListViewSampleDataSource::StoreData^ storeData;
        };
    }
}
