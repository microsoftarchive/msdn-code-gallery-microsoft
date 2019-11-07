// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// TouchableTextBlock.xaml.h
// Declaration of the TouchableTextBlock class
//

#pragma once

#include "pch.h"
#include "TouchableTextBlock.g.h"

namespace SDKSample
{
    namespace TouchKeyboard
    {
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class TouchableTextBlock sealed
        {
        internal:
            virtual property Platform::String^ ContentText
            {
                Platform::String^ get() {return contentText;}
                void set(Platform::String^ text) {contentText = text;}
            }
        public:
            TouchableTextBlock();
    
        protected:
            virtual Windows::UI::Xaml::Automation::Peers::AutomationPeer^ OnCreateAutomationPeer() override;
            virtual void OnGotFocus(Windows::UI::Xaml::RoutedEventArgs^ e) override;
            virtual void OnLostFocus(Windows::UI::Xaml::RoutedEventArgs^ e) override;
            virtual void OnTapped(Windows::UI::Xaml::Input::TappedRoutedEventArgs^ e) override;
            virtual void OnKeyDown(Windows::UI::Xaml::Input::KeyRoutedEventArgs^ e) override;
    
        private:
            Platform::String^ contentText;
        };
    
        ref class TouchableTextBlockAutomationPeer;
    
        /// <summary>
        /// A minimal implementation of ITextRangeProvider, used by TouchableTextBlockAutomationPeer
        /// A real implementation is beyond the scope of this sample
        /// </summary>
    	[Windows::Foundation::Metadata::WebHostHidden]
        public ref class TouchableTextBlockRangeProvider sealed : public Windows::UI::Xaml::Automation::Provider::ITextRangeProvider
        {
        
        public:
            TouchableTextBlockRangeProvider(Platform::String^ text, TouchableTextBlockAutomationPeer^ peer);
    
    //ITextRangeProvider
            virtual void AddToSelection();
            virtual Windows::UI::Xaml::Automation::Provider::ITextRangeProvider^ Clone();
            virtual bool Compare(Windows::UI::Xaml::Automation::Provider::ITextRangeProvider^ textRangeProvider);
            virtual int CompareEndpoints(Windows::UI::Xaml::Automation::Text::TextPatternRangeEndpoint endpoint, 
                                         Windows::UI::Xaml::Automation::Provider::ITextRangeProvider^ textRangeProvider, 
                                         Windows::UI::Xaml::Automation::Text::TextPatternRangeEndpoint targetEndpoint);
            virtual void ExpandToEnclosingUnit(Windows::UI::Xaml::Automation::Text::TextUnit unit);
            virtual Windows::UI::Xaml::Automation::Provider::ITextRangeProvider^ FindAttribute(int attributeId, Platform::Object^ value, bool backward);
            virtual Windows::UI::Xaml::Automation::Provider::ITextRangeProvider^ FindText(Platform::String^ text, bool backward, bool ignoreCase);
            virtual Platform::Object^ GetAttributeValue(int attributeId);
            virtual void GetBoundingRectangles(Platform::Array<double>^* returnValue);
            virtual Platform::Array<Windows::UI::Xaml::Automation::Provider::IRawElementProviderSimple^>^ GetChildren();
            virtual Windows::UI::Xaml::Automation::Provider::IRawElementProviderSimple^ GetEnclosingElement();
            virtual Platform::String^ GetText(int maxLength);
            virtual int Move(Windows::UI::Xaml::Automation::Text::TextUnit unit, int count);
            virtual void MoveEndpointByRange(Windows::UI::Xaml::Automation::Text::TextPatternRangeEndpoint endpoint,
                                             Windows::UI::Xaml::Automation::Provider::ITextRangeProvider^ textRangeProvider, 
                                             Windows::UI::Xaml::Automation::Text::TextPatternRangeEndpoint targetEndpoint);
            virtual int MoveEndpointByUnit(Windows::UI::Xaml::Automation::Text::TextPatternRangeEndpoint endpoint, 
                                          Windows::UI::Xaml::Automation::Text::TextUnit unit, 
                                    int count);
            virtual void RemoveFromSelection();
            virtual void ScrollIntoView(bool alignToTop);
            virtual void Select();
    
        private:
           Platform::String^ _text;
           TouchableTextBlockAutomationPeer^ _peer;
        };
    }
}
