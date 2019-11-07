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
// MainPage.xaml.h
// Declaration of the MainPage.xaml class.
//

#pragma once

#include "pch.h"
#include "MainPage.g.h"
#include "Constants.h"
using namespace Platform;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Interop;

namespace ControlChannelTrigger
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
        void NotifyUser(String^ strMessage, NotifyType type);

        property Windows::UI::Xaml::Controls::Frame^ ScenariosFrame
        {
            Windows::UI::Xaml::Controls::Frame^ get() { return _scenariosFrame; }
            void set(Windows::UI::Xaml::Controls::Frame^ value) { _scenariosFrame = value; }
        }

        property Windows::UI::Xaml::Controls::Frame^ InputFrame
        {
            Windows::UI::Xaml::Controls::Frame^ get() { return _inputFrame; }
            void set(Windows::UI::Xaml::Controls::Frame^ value) { _inputFrame = value; }
        }

        property Windows::UI::Xaml::Controls::Frame^ OutputFrame
        {
            Windows::UI::Xaml::Controls::Frame^ get() { return _outputFrame; }
            void set(Windows::UI::Xaml::Controls::Frame^ value) { _outputFrame = value; }
        }

        property String^ RootNamespace
        {
            String^ get() { return _rootNamespace; }
            void set(String^ value) { _rootNamespace = value; }
        }

        void MainPage_Loaded(Object^ sender, RoutedEventArgs^ e);
        void CheckResolutionAndViewState();
        void DisplayProperties_LogicalDpiChanged(Object^ sender);
        void Page_SizeChanged(Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ args);
        void DoNavigation(TypeName inPageType, Windows::UI::Xaml::Controls::Frame^ inFrame, TypeName outPageType, Windows::UI::Xaml::Controls::Frame^ outFrame);
        event Windows::Foundation::EventHandler<Platform::Object^>^ InputFrameLoaded;
        event Windows::Foundation::EventHandler<Platform::Object^>^ OutputFrameLoaded;

    protected:

    private:
        void Footer_Click(Object^ sender, RoutedEventArgs^ e);
        ~MainPage();

        Windows::UI::Xaml::Controls::Frame^ _scenariosFrame;
        Windows::UI::Xaml::Controls::Frame^ _inputFrame;
        Windows::UI::Xaml::Controls::Frame^ _outputFrame;
        String^ _rootNamespace;
        void SetFeatureName(String^ str);
    };
}
