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
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once

#include "pch.h"
#include "Scenario1_GridView.g.h"
#include "MainPage.xaml.h"
#include "SampleDataSource.h"

using namespace Windows::Foundation;
using namespace Windows::UI::Xaml::Controls;

namespace SDKSample
{
    namespace ListViewSimple
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHiddenAttribute]
        public ref class Scenario1 sealed
        {
        public:
            Scenario1();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
            MainPage^ rootPage;
            ListViewSampleDataSource::StoreData^ storeData;
            TypedEventHandler<ListViewBase^, ContainerContentChangingEventArgs^>^ _delegate;

            void ItemGridView_ContainerContentChanging(ListViewBase^ sender, Windows::UI::Xaml::Controls::ContainerContentChangingEventArgs^ e);

            // Managing delegate creation to ensure we instantiate a single instance for optimal performance
            property TypedEventHandler<ListViewBase^, ContainerContentChangingEventArgs^>^ ContainerContentChangingDelegate
            {
                TypedEventHandler<ListViewBase^, ContainerContentChangingEventArgs^>^ get()
                {
                    if (_delegate == nullptr)
                    {
                        _delegate = ref new TypedEventHandler<ListViewBase^, ContainerContentChangingEventArgs^>(
                            this,
                            &SDKSample::ListViewSimple::Scenario1::ItemGridView_ContainerContentChanging);
                    }
                    return _delegate;
                }
            }
        };
    }
}
