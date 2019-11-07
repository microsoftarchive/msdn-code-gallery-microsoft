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
// ShareHtml.xaml.cpp
// Implementation of the ShareHtml class
//

#include "pch.h"
#include "ShareHtml.xaml.h"

using namespace SDKSample::ShareSource;
using namespace SDKSample::Common;

using namespace Platform;
using namespace Windows::ApplicationModel::DataTransfer;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

ShareHtml::ShareHtml()
{
    InitializeComponent();
    ShareWebView->Navigate(ref new Uri("http://msdn.microsoft.com"));
}

void ShareHtml::ShareWebView_LoadCompleted(Object^ sender, NavigationEventArgs^ e)
{
    ShareWebView->Visibility = Windows::UI::Xaml::Visibility::Visible;
    BlockingRect->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
    LoadingProgressRing->IsActive = false;
}

bool ShareHtml::GetShareContent(DataRequest^ request)
{
    bool succeeded = false;

    // Get the user's selection from the WebView.
    auto requestData = ShareWebView->DataTransferPackage;
    auto dataPackageView = requestData->GetView();

    if ((dataPackageView != nullptr) && (dataPackageView->AvailableFormats->Size > 0))
    {
        requestData->Properties->Title = "A web snippet for you";
        requestData->Properties->Description = "HTML selection from a WebView control"; // The description is optional.
        request->Data = requestData;
        succeeded = true;
    }
    else
    {
        request->FailWithDisplayText("Make a selection in the WebView control and try again.");
    }
    return succeeded;
}