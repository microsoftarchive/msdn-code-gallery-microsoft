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
// Scenario9.xaml.cpp
// Implementation of the Scenario9 class
//

#include "pch.h"
#include "Scenario9.xaml.h"
using namespace SDKSample::ApplicationResources;
using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::ApplicationModel::Resources;
using namespace Windows::ApplicationModel::Resources::Core;


Scenario9::Scenario9()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void Scenario9::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
    Scenario9TextBlock->Text = "";
}

/// <summary>
/// Invoked when the user has selected a different scenario and navigation away from
/// this page is about to occur.
/// </summary>
/// <param name="e">Event data that describes the navigation that has unloaded this
/// page.</param>
void Scenario9::OnNavigatedFrom(NavigationEventArgs^ e)
{
	// Clearing qualifier value overrides set on the default context for the current
	// view so that an override set here doesn't impact other scenarios. See comments
	// below for additional information.
	ResourceContext::GetForCurrentView()->Reset();
}

void SDKSample::ApplicationResources::Scenario9::Scenario9Button_Show_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ b = safe_cast<Button^>(sender);
    if (b != nullptr)
    {
		// A langauge override will be set on the default context for the current view.
		// When navigating between different scenarios in this sample, the content for each
		// scenario is loaded into a host page that remains constant. The view (meaning, the 
		// CoreWindow) remains the same, and so it is the same default context that's in use. 
		// Thus, an override set here can impact behavior in other scenarios.
		//
		// However, the description for the scenario may give the impression that a value 
		// being set is local to this scenario. Also, when entering this scenario, the combo 
		// box always gets set to the first item, which can add to the confusion. To avoid 
		// confusion when using this sample, the override that gets set here will be cleared 
		// when the user navigates away from this scenario (in the OnNavigatedFrom event 
		// handler). In a real app, whether and when to clear an override will depend on
		// the desired behaviour and the design of the app.

		auto selectedLanguage = Scenario9ComboBox->SelectedValue;
        if (selectedLanguage != nullptr)
        {
            auto context = ResourceContext::GetForCurrentView();
            context->QualifierValues->Insert("language", selectedLanguage->ToString());

            auto resourceStringMap = ResourceManager::Current->MainResourceMap->GetSubtree("Resources");
            auto resourceCandidate = resourceStringMap->GetValue("string1", context);
            if (resourceCandidate)
            {
                Scenario9TextBlock->Text = resourceCandidate->ValueAsString;
            }
        }
    }
}

