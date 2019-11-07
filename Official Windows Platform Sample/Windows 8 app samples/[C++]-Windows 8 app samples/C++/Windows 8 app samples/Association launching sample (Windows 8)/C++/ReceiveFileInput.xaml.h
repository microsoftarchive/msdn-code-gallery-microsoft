// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// ReceiveFileInput.xaml.h
// Declaration of the ReceiveFileInput class.
//

#pragma once

#include "pch.h"
#include "ReceiveFileInput.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;

namespace AssociationLaunching
{
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class ReceiveFileInput sealed
    {
    public:
        ReceiveFileInput();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        ~ReceiveFileInput();
        MainPage^ rootPage;
    };
}
