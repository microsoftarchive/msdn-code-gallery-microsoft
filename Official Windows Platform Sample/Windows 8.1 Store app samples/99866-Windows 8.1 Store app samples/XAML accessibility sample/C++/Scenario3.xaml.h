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
// Scenario3.xaml.h
// Declaration of the Scenario3 class
//

#pragma once

#include "pch.h"
#include "Scenario3.g.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Automation;
using namespace Windows::UI::Xaml::Automation::Peers;
using namespace Windows::UI::Xaml::Automation::Provider;
using namespace Windows::UI::Xaml::Media;

namespace SDKSample
{
    namespace Accessibility
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHiddenAttribute]
        public ref class Scenario3 sealed
        {
        public:
            Scenario3();

        protected:
            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        private:
            MainPage^ rootPage;
        };    

        ref class MediaElementContainer;
        
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class MediaContainerAP sealed :  Windows::UI::Xaml::Automation::Peers::FrameworkElementAutomationPeer
                                                    ,Windows::UI::Xaml::Automation::Provider::IRangeValueProvider
                                                    ,Windows::UI::Xaml::Automation::Provider::IToggleProvider
        {
            MediaElement ^_mediaElement;
            FrameworkElement ^_labeledBy;

        public: 
                MediaContainerAP(MediaElementContainer^ owner, MediaElement^ mediaElement);

        protected: 
            virtual Object^ GetPatternCore(PatternInterface patternInterface) override
            {
                if (patternInterface == PatternInterface::RangeValue)
                {
                    return this;
                }
                else if (patternInterface == PatternInterface::Toggle)
                {
                    return this;
                }
                return nullptr;
            }

        protected:
            virtual  AutomationControlType GetAutomationControlTypeCore() override
            {
                return  AutomationControlType::Group;
            }

        protected:
            virtual Platform::String^ GetLocalizedControlTypeCore() override
            {
                return "Video";
            }
            
        protected:
            virtual Platform::String^ GetClassNameCore() override
            {
                return "MediaElementContainer";
            }

        public: 
            virtual property bool IsReadOnly
            {
                bool get()
                { 
                    return false; 
                }
            }

        public: 
            virtual property double LargeChange
            {
                double get() 
                { 
                    Windows::UI::Xaml::Duration md;
                    md = _mediaElement->NaturalDuration;
                    Windows::Foundation::TimeSpan t =_mediaElement->NaturalDuration.TimeSpan;
                    // Windows::Foundation::TimeSpan's Duration Property is in 100 ns units (= 10 ^ -7 seconds)
                    // This, divided by 10 (since the C# sample does total Duration in seconds / 10) makes the conversion factor 0.00000001.
                    // this is both for parity with the C# sample as well as to make the 
                    // Accessibility client see a more human friendly value / range.
                    return (static_cast<double> (t.Duration) * 0.00000001);                    
                }
            }

        public:  
            virtual property double Maximum
            {
                double get()
                { 
                    Windows::Foundation::TimeSpan t =_mediaElement->NaturalDuration.TimeSpan;
                    return (static_cast<double> (t.Duration) * 0.0000001);
                }
            }

        public: 
            virtual property double Minimum
            {
                double get() { return 0; }
            }

        public:
            virtual void SetValue(double value)
            {
                if (value > Maximum || value < 0)
                {
                    throw ref new Platform::InvalidArgumentException;
                }
                // Here, we divide by 10 ^ -7 to convert from seconds to 100s of ns.
                // this is both for parity with the C# sample as well as to make the 
                // Accessibility client see a more human friendly value / range.
                Windows::Foundation::TimeSpan t = { (__int64)(value / 0.0000001)};
                _mediaElement->Position = t;
            }

        public: 
            virtual property double SmallChange
            {
                double get() 
                { 
                    Windows::UI::Xaml::Duration md;
                    md = _mediaElement->NaturalDuration;
                    Windows::Foundation::TimeSpan t =_mediaElement->NaturalDuration.TimeSpan;
                    // Windows::Foundation::TimeSpan's Duration Property is in 100 ns units (= 10 ^ -7 seconds)
                    // This, divided by 10 (since the C# sample does total Duration in seconds / 50) makes the conversion factor 0.000000002.
                    return (static_cast<double> (t.Duration) * 0.000000002);                
                }
            }

        public: 
            virtual property double Value
            {
                double get() 
                { 
                    return (static_cast<double>(_mediaElement->Position.Duration) * 0.0000001);
                }
            }

        public: 
            virtual void Toggle()
            {
                if (_mediaElement->CurrentState == MediaElementState::Playing)
                {
                    if (_mediaElement->CanPause)
                    {
                        _mediaElement->Pause();
                    }
                    else
                    {
                        _mediaElement->Stop();
                    }
                }
                else if (_mediaElement->CurrentState == MediaElementState::Paused || _mediaElement->CurrentState == MediaElementState::Stopped)
                {
                    _mediaElement->Play();
                }
            }

            public:
                virtual property Windows::UI::Xaml::Automation::ToggleState ToggleState
            {
                Windows::UI::Xaml::Automation::ToggleState get()
                {
                    if (_mediaElement->CurrentState == MediaElementState::Playing)
                    {
                        return Windows::UI::Xaml::Automation::ToggleState::On;
                    }
                    else if (_mediaElement->CurrentState == MediaElementState::Stopped || _mediaElement->CurrentState == MediaElementState::Paused)
                    {
                        return Windows::UI::Xaml::Automation::ToggleState::Off;
                    }
                    else
                    {
                        return Windows::UI::Xaml::Automation::ToggleState::Indeterminate;
                    }
                }
            }
        };

        [Windows::Foundation::Metadata::WebHostHiddenAttribute]
        public ref class MediaElementContainer sealed : Windows::UI::Xaml::Controls::ContentControl
        {
            //Platform::String^ mediaUri;
            Windows::UI::Xaml::Controls::MediaElement^ mediaElement; 

        public:
            MediaElementContainer(Windows::UI::Xaml::Controls::Panel^ parent)
            {
                parent->Children->Append(this);
                mediaElement = ref new Windows::UI::Xaml::Controls::MediaElement();
                mediaElement->Width = 200;
                this->Content = mediaElement;
                Windows::Foundation::Uri^ uri = ref new Windows::Foundation::Uri("ms-appx:///Media/video.wmv");        
                mediaElement->Source = uri;
                this->IsTabStop = true;
                AutomationProperties::SetAutomationId(mediaElement, "MediaElement1");
                AutomationProperties::SetName(mediaElement, "MediaElement");
            }
        protected:
            virtual AutomationPeer^ OnCreateAutomationPeer() override
            {
                return ref new MediaContainerAP(this, mediaElement);
            }
        };
    }
}
