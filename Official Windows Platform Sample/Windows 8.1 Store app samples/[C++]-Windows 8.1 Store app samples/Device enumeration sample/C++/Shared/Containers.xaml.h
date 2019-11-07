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
// Containers.xaml.h
// Declaration of the Containers class
//

#pragma once

#include "pch.h"
#include "Containers.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::ViewManagement;
using namespace Platform;
using namespace concurrency;
using namespace Windows::Devices::Enumeration;
using namespace Windows::Devices::Enumeration::Pnp;

namespace SDKSample
{
    namespace DeviceEnumeration
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class Containers sealed
        {
        public:
            Containers();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
    		void EnumerateDeviceContainers(Object^ sender, RoutedEventArgs^ eventArgs);
    		void DisplayDeviceContainer(PnpObject^ container);
        };
    
    	[Windows::UI::Xaml::Data::Bindable]
        public ref class ContainerDisplayItem sealed
        {
            public:
                ContainerDisplayItem(String^ id, String^ name, String^ properties):
                    id(id), name(name), properties(properties) {}
    
                property String^ Id { String^ get() { return id; }}
                property String^ Name { String^ get() { return name; }}
                property String^ Properties { String^ get() { return properties; }}
    
            private protected:
                String^ id;
                String^ name;
                String^ properties;
        };
    }
}
