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
// ShareLink.xaml.h
// Declaration of the ShareLink class
//

#pragma once

#include "pch.h"
#include "SharePage.h"
#include "ShareLink.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace ShareSource
    {
        public ref class ShareLink sealed
        {
        public:
            ShareLink();
    
        protected:
            virtual bool GetShareContent(Windows::ApplicationModel::DataTransfer::DataRequest^ request) override;
    
        private:
            MainPage^ rootPage;
            Windows::Foundation::Uri^ ValidateAndGetUri(Platform::String^ uriString);
        };
    }
}
