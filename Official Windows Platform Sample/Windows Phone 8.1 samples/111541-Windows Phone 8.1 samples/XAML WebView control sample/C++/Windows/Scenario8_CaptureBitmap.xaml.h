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
// Scenario8.xaml.h
// Declaration of the Scenario8 class
//

#pragma once

#include "pch.h"
#include "Scenario8_CaptureBitmap.g.h"
#include "MainPage.xaml.h"
#include "S8_Datasource.h"

namespace SDKSample
{
    namespace WebViewControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario8 sealed
        {
        public:
            Scenario8();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
			BookmarkCollection^ bookmarks;
			void bookmarkBtn_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			concurrency::task<Windows::UI::Xaml::Media::Imaging::WriteableBitmap^> resize(int width, int height, Windows::Storage::Streams::IRandomAccessStream^ source);
			void bookmarkList_ItemClick(Platform::Object^ sender, Windows::UI::Xaml::Controls::ItemClickEventArgs^ e);
    	};
    }
}
