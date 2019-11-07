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
// Accessibility.xaml.cpp
// Implementation of the Accessibility class
//

#include "pch.h"
#include "Accessibility.xaml.h"

using namespace SDKSample::HighContrast;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;

using namespace Windows::UI::ViewManagement;

Accessibility::Accessibility()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Accessibility::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::HighContrast::Accessibility::AccessibilityGetSettings_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
        AccessibilitySettings^ pAccessibilitySettings = ref new AccessibilitySettings();
        Platform::String^ Buffer;
        
        Buffer = "High Contrast " + (pAccessibilitySettings->HighContrast ? "true" : "false") + "\n";
        Buffer += "High Contrast Scheme " + (pAccessibilitySettings->HighContrast ? pAccessibilitySettings->HighContrastScheme : "undefined") + "\n";

        AccessibilityOutputTextBlock->Text = Buffer;
    }
}
