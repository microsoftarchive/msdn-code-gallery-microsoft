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
// PinTileAlternateVisualElementsAsync.xaml.cpp
// Implementation of the PinTileAlternateVisualElementsAsync class
//

#include "pch.h"
#include "Scenario10_PinTileAlternateVisualElementsAsync.xaml.h"

using namespace SDKSample;

using namespace Windows::System::Threading;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::StartScreen;
using namespace Windows::Foundation;
using namespace Windows::Globalization;
using namespace Windows::Globalization::DateTimeFormatting;
using namespace Platform;
using namespace concurrency;

PinTileAlternateVisualElementsAsync::PinTileAlternateVisualElementsAsync()
{
    InitializeComponent();
}

/// <summary>
/// Invoked when this page is about to be displayed in a Frame.
/// </summary>
/// <param name="e">Event data that describes how this page was reached.  The Parameter
/// property is typically used to configure the page.</param>
void PinTileAlternateVisualElementsAsync::OnNavigatedTo(NavigationEventArgs^ e)
{
    // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    // as NotifyUser()
    rootPage = MainPage::Current;
}

void SDKSample::PinTileAlternateVisualElementsAsync::PinButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    Button^ button = safe_cast<Button^>(sender);
    if (button != nullptr)
    {
        if (button->Name == "PinAsyncButton")
        {
            pinAsync = true;
        }
        else
        {
            pinAsync = false;
        }

        // Prepare package images for use as the Tile Logo and small logo in the tile to be pinned.
        // The path provided is a relative path to the package
        auto square70x70Logo = ref new Uri("ms-appx:///Assets/square70x70Tile-sdk.png");
        auto square150x150Logo = ref new Uri("ms-appx:///Assets/square150x150Tile-sdk.png");
        auto wide310x150Logo = ref new Uri("ms-appx:///Assets/wide310x150Tile-sdk.png");
        auto square310x310Logo = ref new Uri("ms-appx:///Assets/square310x310Tile-sdk.png");
        auto square30x30Logo = ref new Uri("ms-appx:///Assets/square30x30Tile-sdk.png");

        Calendar^ cal = ref new Calendar();
        auto longtime = ref new DateTimeFormatter("longtime");
        DateTime time = cal->GetDateTime();

        // During creation of the secondary tile, an application may set additional arguments on the tile that will be passed in during activation.
        // These arguments should be meaningful to the application. In this sample, we will just pass a text as an argument.
        String^ tileActivationArguments = rootPage->tileId + " WasPinnedAt=" + longtime->Format(time);

        // Create a Secondary tile with all the required arguments and set the preferred size to be the medium tile size.
        auto secondaryTile = ref new SecondaryTile(rootPage->tileId,
            "Title text shown on the tile",
            tileActivationArguments,
            square150x150Logo,
            TileSize::Square150x150);

        // Only support of the small and medium tile sizes is mandatory.
        // To have the larger tile sizes available the assets must be provided.
        secondaryTile->VisualElements->Wide310x150Logo = wide310x150Logo;
        secondaryTile->VisualElements->Square310x310Logo = square310x310Logo;

        // If the asset for the small tile size is not provided, it will be created by scaling down the medium tile size asset.
        // Thus, providing the asset for the small tile size is not mandatory, though is recommended to avoid scaling artifacts and can be overridden as shown below. 
        // Note that the asset for the small tile size must be explicitly provided if alternates for the small tile size are also explicitly provided.
        secondaryTile->VisualElements->Square70x70Logo = square70x70Logo;

        // Like the background color, the square30x30 logo is inherited from the parent application tile by default-> 
        // Let's override it, just to see how that's done.
        secondaryTile->VisualElements->Square30x30Logo = square30x30Logo;

        // The display of the secondary tile name can be controlled for each tile size.
        // The default is false.
        secondaryTile->VisualElements->ShowNameOnSquare150x150Logo = false;
        secondaryTile->VisualElements->ShowNameOnWide310x150Logo = true;
        secondaryTile->VisualElements->ShowNameOnSquare310x310Logo = true;

        // Add the handler for the VisualElemets request.
        // This is needed to add alternate tile options for a user to choose from for the supported tile sizes.
        secondaryTile->VisualElementsRequested += 
            ref new TypedEventHandler<Windows::UI::StartScreen::SecondaryTile^, 
            Windows::UI::StartScreen::VisualElementsRequestedEventArgs^>(this, &PinTileAlternateVisualElementsAsync::VisualElementsRequestedHandler);

        // The tile background color is inherited from the parent unless a separate value is specified.
        // This operation will over-ride the default.
        secondaryTile->VisualElements->ForegroundText = ForegroundText::Dark;

        // OK, the tile is created and we can now attempt to pin the tile.
        // Note that the status message is updated when the async operation to pin the tile completes.
        auto const rect = rootPage->GetElementRect(safe_cast<FrameworkElement^>(sender));
        create_task(secondaryTile->RequestCreateForSelectionAsync(rect, Windows::UI::Popups::Placement::Below)).then([this](bool isCreated)
        {
            if (isCreated)
            {
                rootPage->NotifyUser("Secondary tile successfully pinned.", NotifyType::StatusMessage);
            }
            else
            {
                rootPage->NotifyUser("Secondary tile not pinned.", NotifyType::ErrorMessage);
            }
        });
    }
}

void PinTileAlternateVisualElementsAsync::DelayHandler(Windows::System::Threading::ThreadPoolTimer^ sender)
{
    SetAlternativeElements();

    // Once the assets have been assigned, this signals the Pin to Start Flyout to show the tiles.
    deferral->Complete();
    sender->Cancel();
}

void PinTileAlternateVisualElementsAsync::VisualElementsRequestedHandler(Windows::UI::StartScreen::SecondaryTile^ tile, Windows::UI::StartScreen::VisualElementsRequestedEventArgs^ args)
{
    // Request the deferral object if additional time is needed to acquire or process the assets.
    // The Pin to Start Flyout will display a progress control during this time.
    deferral = args->Request->GetDeferral();

    // This delay is to simulate doing an async operation or any processing that will 
    // take a noticeable amount of time to complete.
    visualElementArgs = args;
    TimeSpan delay;

    // 3 seconds.
    delay.Duration = 3 * 10000000L;
    Windows::System::Threading::ThreadPoolTimer::CreateTimer(ref new TimerElapsedHandler(this, &PinTileAlternateVisualElementsAsync::DelayHandler), delay);
}

// Alternate tile options for the supported tile sizes are set in this helper function
void PinTileAlternateVisualElementsAsync::SetAlternativeElements()
{
    // Prepare the images for the alternate tile options.
    auto alternate1Square70x70Logo = ref new Uri("ms-appx:///Assets/alternate1Square70x70Tile-sdk.png");
    auto alternate1Square150x150Logo = ref new Uri("ms-appx:///Assets/alternate1Square150x150Tile-sdk.png");
    auto alternate1Wide310x150Logo = ref new Uri("ms-appx:///Assets/alternate1Wide310x150Tile-sdk.png");
    auto alternate1Square310x310Logo = ref new Uri("ms-appx:///Assets/alternate1Square310x310Tile-sdk.png");

    auto alternate2Square70x70Logo = ref new Uri("ms-appx:///Assets/alternate2Square70x70Tile-sdk.png");
    auto alternate2Square150x150Logo = ref new Uri("ms-appx:///Assets/alternate2Square150x150Tile-sdk.png");
    auto alternate2Wide310x150Logo = ref new Uri("ms-appx:///Assets/alternate2Wide310x150Tile-sdk.png");
    auto alternate2Square310x310Logo = ref new Uri("ms-appx:///Assets/alternate2Square310x310Tile-sdk.png");

    auto alternate3Square70x70Logo = ref new Uri("ms-appx:///Assets/alternate3Square70x70Tile-sdk.png");
    auto alternate3Square150x150Logo = ref new Uri("ms-appx:///Assets/alternate3Square150x150Tile-sdk.png");
    auto alternate3Wide310x150Logo = ref new Uri("ms-appx:///Assets/alternate3Wide310x150Tile-sdk.png");
    auto alternate3Square310x310Logo = ref new Uri("ms-appx:///Assets/alternate3Square310x310Tile-sdk.png");

    Calendar^ cal = ref new Calendar();

    // This check verifies that we are still within the deadline to complete the request.
    // Deadline is when the fly out will time out and stop showing the progress control if the deferral has not completed by then.
    if (cal->GetDateTime().UniversalTime < visualElementArgs->Request->Deadline.UniversalTime)
    {
        visualElementArgs->Request->AlternateVisualElements->GetAt(0)->Square70x70Logo = alternate1Square70x70Logo;
        visualElementArgs->Request->AlternateVisualElements->GetAt(0)->Square150x150Logo = alternate1Square150x150Logo;
        visualElementArgs->Request->AlternateVisualElements->GetAt(0)->Wide310x150Logo = alternate1Wide310x150Logo;
        visualElementArgs->Request->AlternateVisualElements->GetAt(0)->Square310x310Logo = alternate1Square310x310Logo;

        visualElementArgs->Request->AlternateVisualElements->GetAt(0)->BackgroundColor = Windows::UI::Colors::Green;
        visualElementArgs->Request->AlternateVisualElements->GetAt(0)->ForegroundText = Windows::UI::StartScreen::ForegroundText::Dark;
        visualElementArgs->Request->AlternateVisualElements->GetAt(0)->ShowNameOnSquare310x310Logo = true;
    }

    // If there is still time left, continue assigning assets.
    if (cal->GetDateTime().UniversalTime < visualElementArgs->Request->Deadline.UniversalTime)
    {
        visualElementArgs->Request->AlternateVisualElements->GetAt(1)->Square70x70Logo = alternate2Square70x70Logo;
        visualElementArgs->Request->AlternateVisualElements->GetAt(1)->Square150x150Logo = alternate2Square150x150Logo;
        visualElementArgs->Request->AlternateVisualElements->GetAt(1)->Wide310x150Logo = alternate2Wide310x150Logo;
        visualElementArgs->Request->AlternateVisualElements->GetAt(1)->Square310x310Logo = alternate2Square310x310Logo;

        visualElementArgs->Request->AlternateVisualElements->GetAt(1)->BackgroundColor = Windows::UI::Colors::DarkViolet;
        visualElementArgs->Request->AlternateVisualElements->GetAt(1)->ForegroundText = Windows::UI::StartScreen::ForegroundText::Dark;
        visualElementArgs->Request->AlternateVisualElements->GetAt(1)->ShowNameOnWide310x150Logo = true;
    }

    if (cal->GetDateTime().UniversalTime < visualElementArgs->Request->Deadline.UniversalTime)
    {
        visualElementArgs->Request->AlternateVisualElements->GetAt(2)->Square70x70Logo = alternate3Square70x70Logo;
        visualElementArgs->Request->AlternateVisualElements->GetAt(2)->Square150x150Logo = alternate3Square150x150Logo;
        visualElementArgs->Request->AlternateVisualElements->GetAt(2)->Wide310x150Logo = alternate3Wide310x150Logo;
        visualElementArgs->Request->AlternateVisualElements->GetAt(2)->Square310x310Logo = alternate3Square310x310Logo;

        visualElementArgs->Request->AlternateVisualElements->GetAt(2)->BackgroundColor = Windows::UI::Colors::Red;
        visualElementArgs->Request->AlternateVisualElements->GetAt(2)->ForegroundText = Windows::UI::StartScreen::ForegroundText::Light;
        visualElementArgs->Request->AlternateVisualElements->GetAt(2)->ShowNameOnSquare150x150Logo = true;
    }
}