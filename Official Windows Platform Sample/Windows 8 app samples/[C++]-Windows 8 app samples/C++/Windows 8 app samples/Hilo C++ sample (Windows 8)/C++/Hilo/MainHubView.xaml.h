// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved
#pragma once

#include "pch.h"
#include "VariableGridView.h" // Required by generated header
#include "MainHubView.g.h"

namespace Hilo
{
    // See http://go.microsoft.com/fwlink/?LinkId=267278 for info on how Hilo creates pages and navigates to pages.

    // See http://go.microsoft.com/fwlink/?LinkId=267276 for info on how view classes interact 
    // with corresponding view model classes that encapsulate the app’s state, actions, and operations.

    // See http://go.microsoft.com/fwlink/?LinkId=267279 for info on how Hilo pages use XAML controls.

    // The MainHubView class implements the main hub page.
    public ref class MainHubView sealed
    {
    public:
        MainHubView();

    private:
        void OnPhotoItemClicked(Platform::Object^ sender, Windows::UI::Xaml::Controls::ItemClickEventArgs^ e);
    };
}
