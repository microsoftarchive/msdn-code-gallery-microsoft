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
// FailWithError.xaml.h
// Declaration of the FailWithError class
//

#pragma once

#include "pch.h"
#include "SharePage.h"
#include "SetErrorMessage.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace ShareSource
    {
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class SetErrorMessage sealed
        {
        public:
            SetErrorMessage();
    
        protected:
            virtual bool GetShareContent(Windows::ApplicationModel::DataTransfer::DataRequest^ request) override;
        };
    }
}
