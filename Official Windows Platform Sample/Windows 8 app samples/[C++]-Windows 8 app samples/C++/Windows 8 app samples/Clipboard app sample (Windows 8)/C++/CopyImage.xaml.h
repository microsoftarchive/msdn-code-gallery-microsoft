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
// CopyImage.xaml.h
// Declaration of the CopyImage class
//

#pragma once

#include "pch.h"
#include "CopyImage.g.h"
#include "MainPage.xaml.h"

namespace Clipboard
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class CopyImage sealed
    {
    public:
        CopyImage();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        MainPage^ rootPage;

        // CopyImage click events
        void CopyButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void PasteButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        void CopyWithDelayedRenderingButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

        void CopyBitmap(bool useDelayedRendering);
        void OnDeferredImageRequestedHandler(Windows::ApplicationModel::DataTransfer::DataProviderRequest^ request);

        Windows::Storage::Streams::RandomAccessStreamReference^ imgStreamRef;
    };
}
