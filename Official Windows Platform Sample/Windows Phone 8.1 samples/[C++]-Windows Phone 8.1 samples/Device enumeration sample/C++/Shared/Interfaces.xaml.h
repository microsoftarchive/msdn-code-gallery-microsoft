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
// Interfaces.xaml.h
// Declaration of the Interfaces class
//

#pragma once

#include "pch.h"
#include "Interfaces.g.h"
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
        public ref class Interfaces sealed
        {
        public:
            Interfaces();		
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
    		void InterfaceClasses_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ eventArgs);
    		void EnumerateDeviceInterfaces(Object^ sender, RoutedEventArgs^ eventArgs);
            void DisplayDeviceInterface(DeviceInformation^ deviceInterface);
        };
    
    	[Windows::UI::Xaml::Data::Bindable]
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class InterfaceDisplayItem sealed
        {
            public:
                InterfaceDisplayItem(String^ id, String^ name, String^ isEnabled):
                    id(id), name(name), isEnabled(isEnabled), thumbnail(ref new BitmapImage), glyphThumbnail(ref new BitmapImage) {}
    
                property String^ Id { String^ get() { return id; }}
                property String^ Name { String^ get() { return name; }}
                property String^ IsEnabled { String^ get() { return isEnabled; }}
                property BitmapImage^ Thumbnail { BitmapImage^ get() { return thumbnail; }}
                property BitmapImage^ GlyphThumbnail { BitmapImage^ get() { return glyphThumbnail; }}
    
            private protected:
                String^ id;
                String^ name;
                String^ isEnabled;
                BitmapImage^ thumbnail;
                BitmapImage^ glyphThumbnail;
        };
    
    }
}
