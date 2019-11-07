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
using namespace DeviceAppForPrinters;
using namespace Windows::UI::Xaml::Interop;

App::App()
{
    InitializeComponent();
    this->Suspending += ref new SuspendingEventHandler(this, &DeviceAppForPrinters::App::OnSuspending);
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
    if (Window::Current->Content == nullptr)
    {
        rootFrame = ref new Frame();
        TypeName pageType = { "DeviceAppForPrinters.MainPage", TypeKind::Custom };
        rootFrame->Navigate(pageType);
        Window::Current->Content = rootFrame;
    }
    MainPage^ p = safe_cast<MainPage^>(rootFrame->Content);
    p->LaunchArgs = pArgs;
    Window::Current->Activate();

    try
    {
        // Check if this is launched from toast or tile
        Platform::String^ arguments = pArgs->Arguments;
        if (arguments != "")
        {
            p->NotifyUser("Activated through Notification", NotifyType::StatusMessage);
            p->Notification = arguments;
        }
        else
        {
            p->NotifyUser("Activated through Tile", NotifyType::StatusMessage);
        }

        // Load the scenario
        p->LoadScenario(L"DeviceAppForPrinters.InkLevel");
    }
    catch (Platform::Exception^ exception)
    {
        p->NotifyUser(exception->Message, NotifyType::ErrorMessage);
    }
}

void App::OnActivated(Windows::ApplicationModel::Activation::IActivatedEventArgs^ pArgs)
{
    if (pArgs->Kind == Windows::ApplicationModel::Activation::ActivationKind::PrintTaskSettings)
    {
        if (Window::Current->Content == nullptr)
        {
            rootFrame = ref new Frame();
            TypeName pageType = { "DeviceAppForPrinters.MainPage", TypeKind::Custom };
            rootFrame->Navigate(pageType);
            Window::Current->Content = rootFrame;
        }
        MainPage^ p = safe_cast<MainPage^>(rootFrame->Content);
        Window::Current->Activate();
        try
        {
            p->NotifyUser("Activated through PrintTaskSettings contract", NotifyType::StatusMessage);
            PrintTaskSettingsActivatedEventArgs^ ptsArgs = safe_cast<PrintTaskSettingsActivatedEventArgs^>(pArgs);
            Windows::Devices::Printers::Extensions::PrintTaskConfiguration^ configuration = ptsArgs->Configuration;
            p->AddMessage("Got PrintTaskConfiguration");    
            p->Configuration = configuration;
            p->LoadScenario(L"DeviceAppForPrinters.Preferences");
        }
        catch (Platform::Exception^ exception)
        {
            p->NotifyUser(exception->Message, NotifyType::ErrorMessage);
        }
    }
}
