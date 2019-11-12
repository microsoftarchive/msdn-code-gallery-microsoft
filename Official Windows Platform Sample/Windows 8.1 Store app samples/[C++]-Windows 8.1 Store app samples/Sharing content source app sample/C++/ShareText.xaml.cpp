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
// ShareText.xaml.cpp
// Implementation of the ShareText class
//

#include "pch.h"
#include "ShareText.xaml.h"

using namespace SDKSample::ShareSource;
using namespace SDKSample::Common;

using namespace Platform;
using namespace Windows::ApplicationModel::DataTransfer;

ShareText::ShareText()
{
    InitializeComponent();
}

bool ShareText::GetShareContent(DataRequest^ request)
{
    bool succeeded = false;

    auto dataPackageText = TextToShare->Text;
    if (dataPackageText != nullptr)
    {
        auto requestData = request->Data;
        requestData->Properties->Title = TitleInputBox->Text;

        // The description is optional.
        auto dataPackageDescription = DescriptionInputBox->Text;
        if (dataPackageDescription != nullptr)
        {
            requestData->Properties->Description = dataPackageDescription;
        }
        requestData->SetText(dataPackageText);
        succeeded = true;
    }
    else
    {
        request->FailWithDisplayText("Enter the text you would like to share and try again.");
    }
    return succeeded;
}
