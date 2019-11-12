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
// Pointer.xaml.cpp
// Implementation of the Pointer class
//

#include "pch.h"
#include "Pointer.xaml.h"

using namespace SDKSample::DeviceCaps;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace Windows::Devices::Input;

Pointer::Pointer()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Pointer::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

Platform::String^ Pointer::PointerType(PointerDevice^ pointerDevice)
{
    if (pointerDevice->PointerDeviceType == PointerDeviceType::Mouse)
    {
        return "mouse";
    }
    else if (pointerDevice->PointerDeviceType == PointerDeviceType::Pen)
    {
        return "pen";
    }
    else if (pointerDevice->PointerDeviceType == PointerDeviceType::Touch)
    {
        return "touch";
    }

	return "undefined";
}

void SDKSample::DeviceCaps::Pointer::PointerGetSettings_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
		Windows::Foundation::Collections::IVectorView<PointerDevice^>^ PointerDeviceList = PointerDevice::GetPointerDevices();
        Platform::String^ Buffer;

        for (unsigned i = 0; i < PointerDeviceList->Size; i++) {
            Platform::String^ displayIndex = (i + 1).ToString();
            Buffer += "Pointer device " + displayIndex + ":\n";
            Buffer += "This pointer device type is " + PointerType(PointerDeviceList->GetAt(i)) + "\n";
            Buffer += "This pointer device is " + (PointerDeviceList->GetAt(i)->IsIntegrated ? "not " : "") + "external\n";
            Buffer += "This pointer device has a maximum of " + PointerDeviceList->GetAt(i)->MaxContacts.ToString() + " contacts\n";
            Buffer += "The physical device rect is " +
                PointerDeviceList->GetAt(i)->PhysicalDeviceRect.X.ToString() + ", " +
                PointerDeviceList->GetAt(i)->PhysicalDeviceRect.Y.ToString() + ", " +
                PointerDeviceList->GetAt(i)->PhysicalDeviceRect.Width.ToString() + ", " +
                PointerDeviceList->GetAt(i)->PhysicalDeviceRect.Height.ToString() + "\n";
            Buffer += "The screen rect is " +
                PointerDeviceList->GetAt(i)->ScreenRect.X.ToString() + ", " +
                PointerDeviceList->GetAt(i)->ScreenRect.Y.ToString() + ", " +
                PointerDeviceList->GetAt(i)->ScreenRect.Width.ToString() + ", " +
                PointerDeviceList->GetAt(i)->ScreenRect.Height.ToString() + "\n\n";
        }
        
        PointerOutputTextBlock->Text = Buffer;
    }
}
