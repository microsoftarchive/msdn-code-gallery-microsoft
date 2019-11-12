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
// UnpinTile.xaml.cpp
// Implementation of the UnpinTile class
//

#include "pch.h"
#include "Scenario2_UnpinTile.xaml.h"

using namespace SDKSample;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::StartScreen;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Foundation;
using namespace Platform;
using namespace concurrency;


UnpinTile::UnpinTile()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void UnpinTile::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::UnpinTile::UnpinSecondaryTile_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        if (SecondaryTile::Exists(rootPage->tileId))
        {
            // First prepare the tile to be unpinned
            auto secondaryTile = ref new SecondaryTile(rootPage->tileId);

            // Now make the delete request
            auto const rect = rootPage->GetElementRect(safe_cast<FrameworkElement^>(sender));
            create_task(secondaryTile->RequestDeleteForSelectionAsync(rect, Windows::UI::Popups::Placement::Below)).then([this](bool isUnPinned)
            {
                if (isUnPinned)
                {
                    rootPage->NotifyUser("Secondary tile successfully unpinned.", NotifyType::StatusMessage);
                }
                else
                {
                    rootPage->NotifyUser("Secondary tile not unpinned.", NotifyType::ErrorMessage);
                }
            });
        }
        else
        {
            rootPage->NotifyUser(rootPage->tileId + " not currently pinned.", NotifyType::ErrorMessage);
        }
    }
}
