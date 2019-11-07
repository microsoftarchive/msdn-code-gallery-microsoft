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
// UpdateAsync.xaml.cpp
// Implementation of the UpdateAsync class
//

#include "pch.h"
#include "UpdateAsync.xaml.h"

using namespace SecondaryTiles;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::StartScreen;
using namespace Windows::Foundation;
using namespace Platform;
using namespace concurrency;

UpdateAsync::UpdateAsync()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void UpdateAsync::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::current;
    // Preserve the app bar
    appBar = rootPage->BottomAppBar;
    // Now null it so we don't show the appbar in this scenario
    rootPage->BottomAppBar = nullptr;
}

/// <summary>
/// Invoked when this page is about to be navigated out in a Frame
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void UpdateAsync::OnNavigatingFrom(NavigatingCancelEventArgs^ e)
{
    // Restore the app bar
    rootPage->BottomAppBar = appBar;
}

void SecondaryTiles::UpdateAsync::UpdateDefaultLogo_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        if (SecondaryTile::Exists(rootPage->tileId))
        {
            auto secondaryTile = ref new SecondaryTile(rootPage->tileId);
            // Add the properties we want to update (logo in this example)
            secondaryTile->Logo = ref new Uri("ms-appx:///Assets/squareTileLogoUpdate-sdk.png");
            // Now make the update request
            create_task(secondaryTile->UpdateAsync()).then([this](bool isUpdated)
            {
                if (isUpdated)
                {
                    rootPage->NotifyUser("Secondary tile logo updated.", NotifyType::StatusMessage);
                }
                else
                {
                    rootPage->NotifyUser("Secondary tile logo not updated.", NotifyType::ErrorMessage);
                }
            });
        }
        else
        {
            rootPage->NotifyUser("Please pin a secondary tile using scenario 1 before updating the Logo.", NotifyType::ErrorMessage);
        }
    }
}
