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
#include "MainPage.xaml.h"
#include "MainPagePicker.xaml.h"
#include "Constants.h"

using namespace ContactPicker;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::Contacts::Provider;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    { "Pick a single contact", "ContactPicker.ScenarioSingle" },
    { "Pick multiple contacts", "ContactPicker.ScenarioMultiple" },
};

bool MainPage::EnsureUnsnapped()
{
    // The ContactPicker APIs will not work if the application is in the snapped state. If
    // wants to show the ContactPicker while snapped, it must attempt to unsnap first.
    bool unsnapped = ((ApplicationView::Value != ApplicationViewState::Snapped) || ApplicationView::TryUnsnap());
    if (!unsnapped)
    {
        NotifyUser("Cannot unsnap the sample application", NotifyType::StatusMessage);
    }

    return unsnapped;
}

Platform::Array<Scenario>^ MainPagePicker::scenariosInner = ref new Platform::Array<Scenario>
{
    { "Selection contact(s)", "ContactPicker.ContactPickerPage" }
};

void MainPagePicker::Activate(ContactPickerActivatedEventArgs^ args)
{
    // cache ContactPickerUI
    contactPickerUI = args->ContactPickerUI;
    Window::Current->Content = this;
    this->OnNavigatedTo(nullptr);
    Window::Current->Activate();
}
