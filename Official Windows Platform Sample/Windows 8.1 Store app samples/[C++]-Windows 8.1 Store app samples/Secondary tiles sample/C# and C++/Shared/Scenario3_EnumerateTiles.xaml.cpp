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
// EnumerateTiles.xaml.cpp
// Implementation of the EnumerateTiles class
//

#include "pch.h"
#include "Scenario3_EnumerateTiles.xaml.h"
#include <MainPage.xaml.h>

using namespace SDKSample;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::StartScreen;
using namespace Platform;
using namespace concurrency;
using namespace Windows::Foundation::Collections;

EnumerateTiles::EnumerateTiles()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void EnumerateTiles::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::EnumerateTiles::EnumerateSecondaryTiles_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        create_task(SecondaryTile::FindAllAsync("SecondaryTiles.App")).then([this](IVectorView<SecondaryTile^>^ tileList)
        {
            if (tileList->Size > 0)
            {
                Platform::String^ outputText;
                std::for_each(begin(tileList), end(tileList), [this, &outputText](SecondaryTile^ tile)
                {
                    outputText += "Tile Id: " + tile->TileId + ", Tile display name: " + tile->DisplayName + "\n";
                });
                rootPage->NotifyUser(outputText, NotifyType::StatusMessage);
            }
            else
            {
                rootPage->NotifyUser("No secondary tiles are available for this appId.", NotifyType::ErrorMessage);
            }
        });
    }
}
