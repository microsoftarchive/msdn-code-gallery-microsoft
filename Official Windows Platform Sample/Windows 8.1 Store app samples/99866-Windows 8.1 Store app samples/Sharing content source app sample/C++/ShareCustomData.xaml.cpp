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
// ShareCustomData.xaml.cpp
// Implementation of the ShareCustomData class
//

#include "pch.h"
#include "ShareCustomData.xaml.h"

using namespace SDKSample::ShareSource;
using namespace SDKSample::Common;

using namespace Platform;
using namespace Windows::ApplicationModel::DataTransfer;

ShareCustomData::ShareCustomData()
{
    InitializeComponent();
    CustomDataTextBox->Text = CUSTOM_DATA_CONTENT;
}

bool ShareCustomData::GetShareContent(DataRequest^ request)
{
    bool succeeded = false;

    auto dataPackageFormat = DataFormatInputBox->Text;
    if (dataPackageFormat != nullptr)
    {
        auto dataPackageText = CustomDataTextBox->Text;
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
            requestData->SetData(dataPackageFormat, dataPackageText);
            succeeded = true;
        }
        else
        {
            request->FailWithDisplayText("Enter the custom data you would like to share and try again.");
        }
    }
    else
    {
        request->FailWithDisplayText("Enter a custom data format and try again.");
    }
    return succeeded;
}
