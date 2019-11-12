// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// ScenarioInput1.xaml.h
// Declaration of the ScenarioInput1 class.
//

#pragma once

#include "pch.h"
#include "ScenarioInput1.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Printing;

namespace PrintSample
{
    /// <summary>
    /// Basic scenario for modern printing (how to register)
    /// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class ScenarioInput1 sealed
    {
    public:
        ScenarioInput1();

    protected:
        virtual void PreparetPrintContent() override;

    private:
        /// <summary>
        /// Register for print button handler
        /// </summary>
        void RegisterForPrintingButtonClick(Object^ sender, RoutedEventArgs^ e);
    };
}
