// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// ScenarioOutput5.xaml.h
// Declaration of the ScenarioOutput5 class
//

#pragma once

#include "pch.h"
#include "ScenarioOutput5.g.h"
#include "MainPage.xaml.h"

namespace PrintSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioOutput5 sealed
    {
    public:
        ScenarioOutput5();

        property Platform::String^ SelectedText
        {
            Platform::String^ get()
            {
                return textContent->SelectedText;
            }
        }
    private:
        ~ScenarioOutput5();
    };
}
