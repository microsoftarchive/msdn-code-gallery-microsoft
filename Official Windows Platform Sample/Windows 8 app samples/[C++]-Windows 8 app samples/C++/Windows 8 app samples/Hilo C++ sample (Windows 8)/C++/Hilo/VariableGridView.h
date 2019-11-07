// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

namespace Hilo
{
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class VariableGridView sealed : public Windows::UI::Xaml::Controls::GridView
    {
    protected:
        virtual void PrepareContainerForItemOverride(Windows::UI::Xaml::DependencyObject^ element, Platform::Object^ item) override;
    };
}