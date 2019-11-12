// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "ExtendedSplash.xaml.h"
#include "MainPage.xaml.h"

using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Navigation;

using namespace SDKSample;
using namespace SDKSample::Common;
using namespace SDKSample::SplashScreen;

using namespace concurrency;
using namespace Platform;

ExtendedSplash::ExtendedSplash(Windows::ApplicationModel::Activation::SplashScreen^ splashscreen, bool loadState) : splash(splashscreen), dismissed(false)
{
    InitializeComponent();
    LearnMoreButton->Click += ref new RoutedEventHandler(this, &ExtendedSplash::LearnMoreButton_Click);
    // Listen for window resize events to reposition the extended splash screen image accordingly.
    // This is important to ensure that the extended splash screen is formatted properly in response to snapping, unsnapping, rotation, etc...
    Window::Current->SizeChanged += ref new WindowSizeChangedEventHandler(this, &ExtendedSplash::ExtendedSplash_OnResize);

    if (splash != nullptr)
    {
        // Register an event handler to be executed when the splash screen has been dismissed.
        splash->Dismissed += ref new TypedEventHandler<Windows::ApplicationModel::Activation::SplashScreen^, Object^>(this, &ExtendedSplash::DismissedEventHandler);

        // Retrieve the window coordinates of the splash screen image.
        splashImageRect = splash->ImageLocation;
        PositionImage();
    }

    // Create a Frame to act as the navigation context 
    rootFrame = ref new Frame();
    SuspensionManager::RegisterFrame(rootFrame, "appFrame");

    // Normally you should start the time consuming task asynchronously here
    // and dissmiss the extended splash screen in the completion handler for that task
    // In this sample we dismiss the extended splash screen in the handler for 
    // "Learn More" button for demonstration.

    // Here Restore the saved session state if necessary
    if (loadState)
        SuspensionManager::RestoreAsync();
}

// Position the extended splash screen image in the same location as the system splash screen image.
void ExtendedSplash::PositionImage()
{
    Canvas::SetTop(extendedSplashImage, splashImageRect.Y);
    Canvas::SetLeft(extendedSplashImage, splashImageRect.X);
    extendedSplashImage->Height = splashImageRect.Height;
    extendedSplashImage->Width = splashImageRect.Width;
}

void ExtendedSplash::ExtendedSplash_OnResize(Object^ sender, WindowSizeChangedEventArgs^ e)
{
    // Safely update the extended splash screen image coordinates. This function will be fired in response to snapping, unsnapping, rotation, etc...
    if (splash != nullptr)
    {
        // Update the coordinates of the splash screen image.
        splashImageRect = splash->ImageLocation;
        PositionImage();
    }
}

void ExtendedSplash::LearnMoreButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    if (rootFrame->Content == nullptr)
    {
        // Navigate to mainpage    
        if (!rootFrame->Navigate(TypeName(MainPage::typeid)))
        {
            throw ref new FailureException("Failed to create main page");
        }
    }
    // Set extended splash parameters on the main page
    ((MainPage^)rootFrame->Content)->SetExtendedSplashInfo(splashImageRect, dismissed);

    // Place the frame in the current Window 
    Window::Current->Content = rootFrame;
}

// Include code to be executed when the system has transitioned from the splash screen to the extended splash screen (application's first view).
void ExtendedSplash::DismissedEventHandler(Windows::ApplicationModel::Activation::SplashScreen^ sender, Object^ e)
{
    dismissed = true;

    // Navigate away from the app's extended splash screen after completing setup operations here...
    // This sample navigates away from the extended splash screen when the "Learn More" button is clicked.
}
