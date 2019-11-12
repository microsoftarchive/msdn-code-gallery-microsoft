// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// MainPage.xaml.h
// Declaration of the MainPage.xaml class.
//

#pragma once

#include "pch.h"
#include "MainPage.g.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation;
using namespace Windows::System;
using namespace Platform;
using namespace Windows::UI::Xaml::Interop;

namespace PrintSample
{
    public enum class NotifyType
    {
        StatusMessage,
        ErrorMessage
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class MainPage sealed
    {
    public:
        MainPage();

        property Windows::UI::Xaml::Controls::Frame^ ScenariosFrame
        {
            Windows::UI::Xaml::Controls::Frame^ get() { return _scenariosFrame; }
        }
        property Windows::UI::Xaml::Controls::Frame^ InputFrame
        {
            Windows::UI::Xaml::Controls::Frame^ get() { return _inputFrame; }
        }
        property Windows::UI::Xaml::Controls::Frame^ OutputFrame
        {
            Windows::UI::Xaml::Controls::Frame^ get() { return _outputFrame; }
        }

        event Windows::Foundation::EventHandler<Platform::Object^>^ InputFrameLoaded;
        event Windows::Foundation::EventHandler<Platform::Object^>^ OutputFrameLoaded;

        void NotifyUser(String^ strMessage, NotifyType type);
        void DoNavigation(TypeName pageType, Windows::UI::Xaml::Controls::Frame^ frame);
        String^ ConvertViewState(ApplicationViewState state);
        String^ ConvertResolution(ResolutionScale scale);

    private:
        ~MainPage();
        Windows::UI::Xaml::Controls::Frame^ _scenariosFrame;
        Windows::UI::Xaml::Controls::Frame^ _inputFrame;
        Windows::UI::Xaml::Controls::Frame^ _outputFrame;
        void Footer_Click(Object^ sender, RoutedEventArgs^ e);
        Windows::Foundation::EventRegistrationToken _displayHandlerToken;
        Windows::Foundation::EventRegistrationToken _sizeChangedHandlerToken;
        Windows::Foundation::EventRegistrationToken _pageLoadedHandlerToken;
        Windows::Foundation::EventRegistrationToken _logicalDpiChangedToken;
        Windows::Foundation::EventRegistrationToken _inputFrameLoadedToken;
        Windows::Foundation::EventRegistrationToken _outputFrameLoadedToken;
        void Page_SizeChanged(Windows::UI::Core::CoreWindow^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ args);
        void DisplayProperties_LogicalDpiChanged(Object^ sender);
        void Page_Loaded(Object^ sender, RoutedEventArgs^ e);
        void CheckResolutionAndViewState();
        void SetFeatureName(Platform::String^ strFeature);
        void inputFrameNavigated(Object^ sender, NavigationEventArgs^ e);
        void outputFrameNavigated(Object^ sender, NavigationEventArgs^ e);
    };
}
