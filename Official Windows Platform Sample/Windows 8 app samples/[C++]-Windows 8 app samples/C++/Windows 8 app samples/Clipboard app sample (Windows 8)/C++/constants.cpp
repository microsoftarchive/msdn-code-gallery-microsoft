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

using namespace Clipboard;
using namespace Windows::UI::ViewManagement;

Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
    { "Copy and paste text", "Clipboard.CopyText" },
    { "Copy and paste an image", "Clipboard.CopyImage" },
    { "Copy and paste files", "Clipboard.CopyFiles" },
    { "Other Clipboard operations", "Clipboard.Others" }
};

bool MainPage::EnsureUnsnapped()
{
    // FilePicker APIs will not work if the application is in a snapped state.
    // If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
    bool unsnapped = ((ApplicationView::Value != ApplicationViewState::Snapped) || ApplicationView::TryUnsnap());
    if (!unsnapped)
    {
        NotifyUser("Cannot unsnap the sample application", NotifyType::StatusMessage);
    }

    return unsnapped;
}
