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
// ShareFiles.xaml.h
// Declaration of the ShareFiles class
//

#pragma once

#include "pch.h"
#include "SharePage.h"
#include "ShareFiles.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace ShareSource
    {
        public ref class ShareFiles sealed
        {
        public:
            ShareFiles();
    
        protected:
            virtual bool GetShareContent(Windows::ApplicationModel::DataTransfer::DataRequest^ request) override;
    
        private:
            MainPage^ rootPage;
            Windows::Foundation::Collections::IVectorView<Windows::Storage::StorageFile^>^ storageItems;
    
            void SelectFilesButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
