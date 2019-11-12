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
// Scenario4.xaml.h
// Declaration of the Scenario4 class
//

#pragma once

#include "pch.h"
#include "Scenario4.g.h"
#include "MainPage.xaml.h"
#include "SampleDataSource.h"

using namespace Windows::UI::Xaml::Controls;

namespace SDKSample
{
    namespace ListViewInteraction
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

            ListViewInteractionSampleDataSource::ToppingsData^ toppingsData;
            ListViewInteractionSampleDataSource::StoreData^ storeData;
            TypedEventHandler<ListViewBase^, ContainerContentChangingEventArgs^>^ _delegate;
            void GridView_ContainerContentChanging(ListViewBase^ sender, ContainerContentChangingEventArgs^ e);
            void CreateCustomCarton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            // Managing delegate creation to ensure we instantiate a single instance for optimal performance
            property TypedEventHandler<ListViewBase^, ContainerContentChangingEventArgs^>^ ContainerContentChangingDelegate
            {
                TypedEventHandler<ListViewBase^, ContainerContentChangingEventArgs^>^ get()
                {
                    if (_delegate == nullptr)
                    {
                        _delegate = ref new TypedEventHandler<ListViewBase^, ContainerContentChangingEventArgs^>(
                            this,
                            &SDKSample::ListViewInteraction::Scenario4::GridView_ContainerContentChanging);
                    }
                    return _delegate;
                }
            }
        };
    }
}
