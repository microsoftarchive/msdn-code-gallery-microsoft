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
// Scenario2.xaml.h
// Declaration of the Scenario2 class
//

#pragma once

#include "pch.h"
#include "Scenario2.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace Scaling
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario2 sealed
        {
        public:
            Scenario2();
    
        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
    
        private:
            // Helpers to convert between points and pixels.
            double PtFromPx(double pixel);
            double PxFromPt(double pt);

            Platform::String^ StringFromDouble(double);
            void SetOverrideRectSize(double sizeInPhysicalPx, double scaleFactor);
            void SetOverrideTextFont(double size, Windows::UI::Xaml::Media::FontFamily^ fontFamily);
            void OutputSettings(double scaleFactor, Windows::UI::Xaml::FrameworkElement^ rectangle, Windows::UI::Xaml::Controls::TextBlock^ relativePxText,
                                Windows::UI::Xaml::Controls::TextBlock^ physicalPxText, Windows::UI::Xaml::Controls::TextBlock^ fontTextBlock);
            void ResetOutput();
            void DisplayProperties_LogicalDpiChanged(Platform::Object^ sender);

            Windows::UI::Xaml::Media::FontFamily^ defaultFontFamily;
            Windows::UI::Xaml::Media::FontFamily^ overrideFontFamily;
        };
    }
}
