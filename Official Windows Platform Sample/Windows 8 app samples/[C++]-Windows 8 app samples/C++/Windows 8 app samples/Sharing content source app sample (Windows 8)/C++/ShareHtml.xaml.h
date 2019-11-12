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
// ShareHtml.xaml.h
// Declaration of the ShareHtml class
//

#pragma once

#include "pch.h"
#include "SharePage.h"
#include "ShareHtml.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace ShareSource
    {
        public ref class ShareHtml sealed
        {
        public:
            ShareHtml();

        protected:
            virtual bool GetShareContent(Windows::ApplicationModel::DataTransfer::DataRequest^ request) override;

        private:
            void ShareWebView_LoadCompleted(Platform::Object^ sender, Windows::UI::Xaml::Navigation::NavigationEventArgs^ e);
        };
    }
}