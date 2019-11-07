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
#include "Constants.h"

using namespace AccountPictureName;
using namespace Windows::UI::ViewManagement;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    // The format here is the following:
    // {"Description for the sample", "Fully quaified name for the class that implements the scenario" }
    { "Get User DisplayName", "AccountPictureName.GetUserDisplayName" },
    { "Get User First and Last Name", "AccountPictureName.GetUserFirstAndLastName" },
    { "Get Account Picture", "AccountPictureName.GetAccountPicture" },
    { "Set Account Picture", "AccountPictureName.SetAccountPicture" }
};

void MainPage::LoadSetAccountPictureScenario()
{
    LoadScenario("AccountPictureName.SetAccountPicture");
    Scenarios->SelectedIndex = 3;
    InvalidateSize();
}

bool MainPage::EnsureUnsnapped()
{
    // FilePicker APIs will not work if the application is in a snapped state. If an app wants to show a FilePicker while snapped,
    // it must attempt to unsnap first.
    bool unsnapped = ((ApplicationView::Value != ApplicationViewState::Snapped) || ApplicationView::TryUnsnap());
    if (!unsnapped)
    {
        NotifyUser("Cannot unsnap the sample application.", NotifyType::StatusMessage);
    }

    return unsnapped;
}