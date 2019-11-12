//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include "CustomException.xaml.h"

using namespace ProxyStubsForWinRTComponents;
using namespace Microsoft::SDKSamples::Kitchen;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

CustomException::CustomException()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void CustomException::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void ProxyStubsForWinRTComponents::CustomException::CustomExceptionRun(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    // Component Creation
    Oven^ myOven = ref new Oven();

    try 
    {
        // Intentionally pass an invalid value
        myOven->ConfigurePreheatTemperature(static_cast<Microsoft::SDKSamples::Kitchen::OvenTemperature>(5));
    }
    catch (InvalidArgumentException^ e)
    {
        CustomExceptionOutput->Text += L"Exception caught. Please attach a debugger and enable first chance native exceptions to view exception details.";
    }
}
