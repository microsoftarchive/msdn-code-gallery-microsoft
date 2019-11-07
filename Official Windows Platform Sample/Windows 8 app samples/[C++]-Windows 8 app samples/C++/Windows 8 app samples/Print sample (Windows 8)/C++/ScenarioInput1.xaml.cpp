// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario1Input.xaml.cpp
// Implementation of the Scenario1Input class
//

#include "pch.h"
#include "ScenarioOutput1.xaml.h"
#include "ScenarioInput1.xaml.h"

using namespace PrintSample;

using namespace Windows::Foundation::Collections;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;
using namespace Windows::Graphics::Printing;
using namespace Windows::UI;
using namespace Windows::UI::Text;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Documents;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;
using namespace Windows::UI::Xaml::Navigation;

ScenarioInput1::ScenarioInput1()
{
    InitializeComponent();
}

/// <summary>
/// Provide print content for scenario 1 first page
/// </summary>
void ScenarioInput1::PreparetPrintContent()
{
    if (FirstPage == nullptr)
    {
        FirstPage = ref new ScenarioOutput1();
        StackPanel^ header = safe_cast<StackPanel^>(FirstPage->FindName("header"));
        header->Visibility = Windows::UI::Xaml::Visibility::Visible;
    }

    // Add the (newley created) page to the printing root which is part of the visual tree and force it to go
    // through layout so that the linked containers correctly distribute the content inside them.
    PrintingRoot->Children->Append(FirstPage);
    PrintingRoot->InvalidateMeasure();
    PrintingRoot->UpdateLayout();
}

/// <summary>
/// Register for print button handler
/// </summary>
void ScenarioInput1::RegisterForPrintingButtonClick(Object^ sender, RoutedEventArgs^ e)
{
    Button^ clickedButton = safe_cast<Button^>(sender);

    // check to see if the application is registered for printing
    if (RegisteredForPrinting)
    {
        // unregister for printing 
        UnregisterForPrinting();

        // change the text on the button
        clickedButton->Content = "Register";

        // tell the user that the print contract has been unregistered
        RootPage->NotifyUser("Print contract unregistered.", NotifyType::StatusMessage);
    }
    else
    {
        // register for printing
        RegisterForPrinting();

        // change the text on the button
        clickedButton->Content = "Unregister";

        // tell the user that the print contract has been registered
        RootPage->NotifyUser("Print contract registered, use the Charms Bar to print.", NotifyType::StatusMessage);
    }

    // toggel the value of isRegisteredForPrinting
    RegisteredForPrinting = !RegisteredForPrinting;
}