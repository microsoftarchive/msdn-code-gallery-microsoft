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
// App.xaml.cpp
// Implementation of the App.xaml class.
//

#include "pch.h"
#include "App.xaml.h"
#include "MainPage.xaml.h"

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace CppSamplesUtils;
using namespace ProximityCPP;
using namespace Windows::UI::Xaml::Interop;

App::App()
{
    InitializeComponent();
    this->Suspending += ref new SuspendingEventHandler(this, &ProximityCPP::App::OnSuspending);
}

void App::OnSuspending(Platform::Object^ sender, Windows::ApplicationModel::SuspendingEventArgs^ pArgs)
{
    auto deferral = pArgs->SuspendingOperation->GetDeferral();
    SuspensionManager::StartSaveTask().then([=]() 
    {
        deferral->Complete();
    });
}

void App::OnLaunched(Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ pArgs)
{
    if (pArgs->PreviousExecutionState == ApplicationExecutionState::Terminated)
    {
        SuspensionManager::StartRestoreTask().then([=](concurrency::task<void> restoreTask)
        {
            try
            {
                restoreTask.get();
            }
            catch(Platform::Exception^)
            {
                // If restore fails, the app should proceed as though there was no restored state.
            }

            SetupWindowing(pArgs);
        });
    }
    else
    {
        SetupWindowing(pArgs);
    }
}

void App::SetupWindowing(Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ pArgs)
{
    TypeName pageType = { "ProximityCPP.MainPage", TypeKind::Custom };
    if (Window::Current->Content == nullptr)
    {
        auto rootFrame = ref new Frame();
        
        rootFrame->Navigate(pageType, pArgs);
        Window::Current->Content = rootFrame;
    }
    else
    {
        // The app might be executing another scenario
        Frame^ rootFrame = safe_cast<Frame^>(Window::Current->Content);
        rootFrame->Navigate(pageType, pArgs);
    }
    Window::Current->Activate();
}

