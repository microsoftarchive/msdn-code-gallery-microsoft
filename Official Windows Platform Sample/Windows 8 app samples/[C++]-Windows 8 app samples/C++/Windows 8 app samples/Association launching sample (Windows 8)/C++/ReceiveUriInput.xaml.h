// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// ReceiveUriInput.xaml.h
// Declaration of the ReceiveUriInput class.
//

#pragma once

#include "pch.h"
#include "ReceiveUriInput.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;

namespace AssociationLaunching
{
	[Windows::Foundation::Metadata::WebHostHidden]
    public ref class ReceiveUriInput sealed
    {
    public:
        ReceiveUriInput();

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    private:
        ~ReceiveUriInput();
        MainPage^ rootPage;
    };
}