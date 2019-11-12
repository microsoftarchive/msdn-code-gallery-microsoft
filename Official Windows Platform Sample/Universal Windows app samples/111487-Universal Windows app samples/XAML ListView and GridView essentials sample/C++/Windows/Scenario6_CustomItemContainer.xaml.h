//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario6.xaml.h
// Declaration of the Scenario6 class
//

#pragma once

#include "pch.h"
#include "Scenario6_CustomItemContainer.g.h"
#include "MainPage.xaml.h"
#include "SampleDataSource.h"
#include "CustomGridViewItemPresenter.h"

namespace SDKSample
{
    namespace ListViewSimple
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
		[Windows::Foundation::Metadata::WebHostHiddenAttribute]
        public ref class Scenario6 sealed
        {
        public:
            Scenario6();

		protected:
			virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

		private:
			MainPage^ rootPage;
			ListViewSampleDataSource::StoreData^ storeData;
        };
    }
}
