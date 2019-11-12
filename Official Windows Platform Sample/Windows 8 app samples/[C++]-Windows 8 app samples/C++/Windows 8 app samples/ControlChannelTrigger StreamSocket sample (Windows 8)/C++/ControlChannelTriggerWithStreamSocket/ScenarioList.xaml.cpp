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
// ScenarioList.xaml.cpp
// Implementation of the ScenarioList class
//

#include "pch.h"
#include "ScenarioList.xaml.h"

using namespace ControlChannelTrigger;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation;
using namespace CppSamplesUtils;

ScenarioList::ScenarioList()
{
    InitializeComponent();
    rootPage=nullptr;
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void ScenarioList::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage= dynamic_cast<MainPage^>(e->Parameter);
    Scenarios->SelectionChanged += ref new SelectionChangedEventHandler(this, &ScenarioList::Scenarios_SelectionChanged);
    ListBoxItem^ startingScenario = nullptr;
    
    auto ps = SuspensionManager::SessionState();
    if (ps->HasKey("SelectedScenario"))
    {
        PropertyValue^ pv;
        String^ item = dynamic_cast<String^>(ps->Lookup("SelectedScenario"));
        startingScenario = dynamic_cast<ListBoxItem^>(this->FindName(item));
    }

    Scenarios->SelectedItem = startingScenario != nullptr ? startingScenario : Scenario1;
}


void ScenarioList::Scenarios_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
     if(Scenarios->SelectedItem != nullptr)
    {
        rootPage->NotifyUser("", NotifyType::StatusMessage);

        ListBoxItem^ selectedListBoxItem = dynamic_cast<ListBoxItem^>(Scenarios->SelectedItem);
        SuspensionManager::SessionState()->Insert("SelectedScenario", selectedListBoxItem->Name);

        if (rootPage->InputFrame != nullptr && rootPage->OutputFrame != nullptr)
        {
             //Load the input and output pages for the current scenario into their respective frames
            TypeName inputPage = { "ControlChannelTrigger" + "." + "ScenarioInput" + ((Scenarios->SelectedIndex + 1).ToString()), TypeKind::Custom };
            TypeName outputPage = { "ControlChannelTrigger" + "." + "ScenarioOutput" + ((Scenarios->SelectedIndex + 1).ToString()), TypeKind::Custom };
            rootPage->DoNavigation(inputPage, rootPage->InputFrame, outputPage, rootPage->OutputFrame);
        }
    }
}
