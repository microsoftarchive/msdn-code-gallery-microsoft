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
// FailWithError.xaml.cpp
// Implementation of the FailWithError class
//

#include "pch.h"
#include "SharePage.h"
#include "SetErrorMessage.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample::ShareSource;
using namespace SDKSample::Common;

using namespace Platform;
using namespace Windows::ApplicationModel::DataTransfer;

SetErrorMessage::SetErrorMessage()
{
    InitializeComponent();
}

bool SetErrorMessage::GetShareContent(DataRequest^ request)
{
    auto errorMessage = CustomErrorText->Text;
    if (errorMessage == nullptr)
    {
        errorMessage = ref new String(L"Enter a failure display text and try again.");
    }
    request->FailWithDisplayText(errorMessage);
    return false;
}
