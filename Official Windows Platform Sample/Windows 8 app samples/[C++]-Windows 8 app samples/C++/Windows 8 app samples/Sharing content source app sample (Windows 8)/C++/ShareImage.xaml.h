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
// ShareImage.xaml.h
// Declaration of the ShareImage class
//

#pragma once

#include "pch.h"
#include "SharePage.h"
#include "ShareImage.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace ShareSource
    {
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class ShareImage sealed
        {
        public:
            ShareImage();
    
        protected:
            virtual bool GetShareContent(Windows::ApplicationModel::DataTransfer::DataRequest^ request) override;
    
        private:
            MainPage^ rootPage;
            Windows::Storage::StorageFile^ imageFile;
    
            void SelectImageButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
