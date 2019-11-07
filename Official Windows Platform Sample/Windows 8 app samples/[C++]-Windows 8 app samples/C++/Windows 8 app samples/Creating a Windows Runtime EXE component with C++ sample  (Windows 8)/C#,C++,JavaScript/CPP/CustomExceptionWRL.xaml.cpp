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
#include "CustomExceptionWRL.xaml.h"

using namespace WRLOutOfProcessWinRTComponent;
using namespace Microsoft::SDKSamples::Kitchen;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

CustomExceptionWRL::CustomExceptionWRL()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void CustomExceptionWRL::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void WRLOutOfProcessWinRTComponent::CustomExceptionWRL::CustomExceptionWRLRun(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Microsoft::WRL::ComPtr<ABI::Microsoft::SDKSamples::Kitchen::IOven> spMyOven;
    HRESULT hr = Windows::Foundation::ActivateInstance(Microsoft::WRL::Wrappers::HStringReference(RuntimeClass_Microsoft_SDKSamples_Kitchen_Oven).Get(), &spMyOven);
    if (SUCCEEDED(hr)) 
    {
        // Intentionally pass an invalid value
        hr = spMyOven->ConfigurePreheatTemperature((ABI::Microsoft::SDKSamples::Kitchen::OvenTemperature)5);
        if (hr == E_INVALIDARG)
        {
            CustomExceptionWRLOutput->Text += L"Error handled. Please attach a debugger and enable first chance native exceptions to view exception details.";
        }
    }
}
