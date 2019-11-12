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
// Msappdata.xaml.h
// Declaration of the Msappdata class
//

#pragma once

#include "pch.h"
#include "Msappdata.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace ApplicationDataSample
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class Msappdata sealed
        {
        public:
            Msappdata();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
            virtual void OnNavigatingFrom(Windows::UI::Xaml::Navigation::NavigatingCancelEventArgs^ e) override;

        private:
            Concurrency::task<void> CopyFile(Windows::Foundation::Uri^ sourceUri, Windows::Storage::StorageFolder^ destFolder, Platform::String^ destFilename);
            MainPage^ rootPage;
        };
    }
}
