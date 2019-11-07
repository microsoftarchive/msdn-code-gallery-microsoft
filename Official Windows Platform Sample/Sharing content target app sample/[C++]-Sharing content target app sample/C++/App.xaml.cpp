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
#include "DefaultPage.xaml.h"

using namespace SDKSample::ShareTarget;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::Foundation;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;

App::App()
{
    InitializeComponent();
}

void App::OnLaunched(LaunchActivatedEventArgs^ pArgs)
{
    auto rootFrame = ref new Frame();
    rootFrame->Navigate(TypeName(DefaultPage::typeid));
    Window::Current->Content = rootFrame;
    Window::Current->Activate();
}

void App::OnShareTargetActivated(ShareTargetActivatedEventArgs^ args)
{
    auto rootFrame = ref new Frame();
    rootFrame->Navigate(TypeName(MainPage::typeid), args->ShareOperation);
    Window::Current->Content = rootFrame;
    Window::Current->Activate();
}