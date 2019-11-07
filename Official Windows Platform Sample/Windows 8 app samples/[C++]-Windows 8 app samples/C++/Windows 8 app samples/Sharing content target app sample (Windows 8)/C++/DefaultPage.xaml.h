// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// DefaultPage.xaml.h
// Declaration of the DefaultPage class
//

#pragma once

#include "pch.h"
#include "DefaultPage.g.h"

namespace ShareTarget
{
    public ref class DefaultPage sealed
    {
    public:
        DefaultPage();

    private:
        void Footer_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
    };
}