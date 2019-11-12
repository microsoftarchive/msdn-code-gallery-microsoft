// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
//
// NonTouchableTextBlock.xaml.h
// Declaration of the NonTouchableTextBlock class
//

#pragma once

#include "pch.h"
#include "NonTouchableTextBlock.g.h"

namespace SDKSample
{
    namespace TouchKeyboard
    {
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class NonTouchableTextBlock sealed
        {
        public:
            NonTouchableTextBlock();

        protected:
            virtual Windows::UI::Xaml::Automation::Peers::AutomationPeer^ OnCreateAutomationPeer() override;
            virtual void OnGotFocus(Windows::UI::Xaml::RoutedEventArgs^ e) override;
            virtual void OnLostFocus(Windows::UI::Xaml::RoutedEventArgs^ e) override;
            virtual void OnTapped(Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e) override;
            virtual void OnKeyDown(Windows::UI::Xaml::Input::KeyRoutedEventArgs^ e) override;
        };
    }
}
