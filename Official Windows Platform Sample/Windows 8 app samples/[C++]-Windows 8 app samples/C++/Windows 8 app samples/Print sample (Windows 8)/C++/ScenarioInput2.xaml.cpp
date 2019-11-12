// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

//
// Scenario2Input.xaml.cpp
// Implementation of the Scenario2Input class.
//

#include "pch.h"
#include "ScenarioInput2.xaml.h"
#include "ScenarioOutput2.xaml.h"
#include "MainPage.xaml.h"

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

ScenarioInput2::ScenarioInput2()
{
    InitializeComponent();
}

/// <summary>
/// button click handlers
/// </summary>
void ScenarioInput2::InvokePrintButtonClick(Object^ sender, RoutedEventArgs^ e)
{
    // Don't act when in snapped mode
    if (ApplicationView::Value != ApplicationViewState::Snapped)
    {
        Windows::Graphics::Printing::PrintManager::ShowPrintUIAsync();
    }
}

/// <summary>
/// Provide print content for scenario 2 first page
/// </summary>
void ScenarioInput2::PreparetPrintContent()
{
    if (FirstPage == nullptr)
    {
        FirstPage = ref new ScenarioOutput2();
        StackPanel^ header = safe_cast<StackPanel^>(FirstPage->FindName("header"));
        header->Visibility = Windows::UI::Xaml::Visibility::Visible;
    }

    // Add the (newley created) page to the printing root which is part of the visual tree and force it to go
    // through layout so that the linked containers correctly distribute the content inside them.
    PrintingRoot->Children->Append(FirstPage);
    PrintingRoot->InvalidateMeasure();
    PrintingRoot->UpdateLayout();
}

void ScenarioInput2::OnNavigatedTo(NavigationEventArgs^ e)
{
    BasePrintPage::OnNavigatedTo(e);

    RegisterForPrinting();
}

void ScenarioInput2::OnNavigatedFrom(NavigationEventArgs^ e)
{
    UnregisterForPrinting() ;
}