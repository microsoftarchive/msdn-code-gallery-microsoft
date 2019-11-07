// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// App.xaml.cpp
// Implementation of the App.xaml class.
//

#include "pch.h"
#include "App.xaml.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::Foundation;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace CppSamplesUtils;
using namespace ShareTarget;
using namespace Windows::UI::Xaml::Interop;

App::App()
{
    InitializeComponent();
}

void App::OnLaunched(LaunchActivatedEventArgs^ pArgs)
{
    auto rootFrame = ref new Frame();
    TypeName pageType = { "ShareTarget.DefaultPage", TypeKind::Custom };
    rootFrame->Navigate(pageType);
    Window::Current->Content = rootFrame;
    Window::Current->Activate();
}

void App::OnShareTargetActivated(ShareTargetActivatedEventArgs^ args)
{
    auto rootFrame = ref new Frame();
    TypeName pageType = { "ShareTarget.MainPage", TypeKind::Custom };
    rootFrame->Navigate(pageType, args->ShareOperation);
    Window::Current->Content = rootFrame;
    Window::Current->Activate();
}