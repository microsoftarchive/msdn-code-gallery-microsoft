//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#include "pch.h"
#include "App.xaml.h"
#include "MainPage.xaml.h"
#include "ProductItem.h"

using namespace Simple3DGameXaml;

using namespace Microsoft::WRL;
using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::Graphics::Display;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::ApplicationModel::Store;
using namespace concurrency;

//----------------------------------------------------------------------

MainPage::MainPage() :
    m_possiblePurchaseUpgrade(false)
{
    InitializeComponent();

#ifdef USE_STORE_SIMULATOR
    ResetLicense->Visibility = ::Visibility::Visible;
#endif
}

//----------------------------------------------------------------------

void MainPage::SetApp(App^ app)
{
    m_app = app;
}

//----------------------------------------------------------------------

void MainPage::HideGameInfoOverlay()
{
    VisualStateManager::GoToState(this, "NormalState", true);

    StoreFlyout->IsOpen = false;
    StoreFlyout->Visibility = ::Visibility::Collapsed;
    GameAppBar->IsOpen = false;
}

//----------------------------------------------------------------------

void MainPage::ShowGameInfoOverlay()
{
    VisualStateManager::GoToState(this, "GameInfoOverlayState", true);
}

//----------------------------------------------------------------------

void MainPage::SetAction(GameInfoOverlayCommand action)
{
    // Enable only one of the four possible commands at the bottom of the
    // Game Info Overlay.

    PlayAgain->Visibility = ::Visibility::Collapsed;
    PleaseWait->Visibility = ::Visibility::Collapsed;
    TapToContinue->Visibility = ::Visibility::Collapsed;

    switch (action)
    {
    case GameInfoOverlayCommand::PlayAgain:
        PlayAgain->Visibility = ::Visibility::Visible;
        break;
    case GameInfoOverlayCommand::PleaseWait:
        PleaseWait->Visibility = ::Visibility::Visible;
        break;
    case GameInfoOverlayCommand::TapToContinue:
        TapToContinue->Visibility = ::Visibility::Visible;
        break;
    case GameInfoOverlayCommand::None:
        break;
    }
}

//----------------------------------------------------------------------

void MainPage::SetGameLoading()
{
    GameInfoOverlayTitle->Text = "Loading Resources";

    Loading->Visibility = ::Visibility::Visible;
    Stats->Visibility = ::Visibility::Collapsed;
    LevelStart->Visibility = ::Visibility::Collapsed;
    PauseData->Visibility = ::Visibility::Collapsed;
    LoadingProgress->IsActive = true;
}

//----------------------------------------------------------------------

void MainPage::SetGameStats(
    int maxLevel,
    int hitCount,
    int shotCount
    )
{
    GameInfoOverlayTitle->Text = "Game Statistics";
    m_possiblePurchaseUpgrade = true;
    OptionalTrialUpgrade();

    Loading->Visibility = ::Visibility::Collapsed;
    Stats->Visibility = ::Visibility::Visible;
    LevelStart->Visibility = ::Visibility::Collapsed;
    PauseData->Visibility = ::Visibility::Collapsed;

    static const int bufferLength = 20;
    static char16 wsbuffer[bufferLength];

    int length = swprintf_s(wsbuffer, bufferLength, L"%d", maxLevel);
    LevelsCompleted->Text = ref new Platform::String(wsbuffer, length);

    length = swprintf_s(wsbuffer, bufferLength, L"%d", hitCount);
    TotalPoints->Text = ref new Platform::String(wsbuffer, length);

    length = swprintf_s(wsbuffer, bufferLength, L"%d", shotCount);
    TotalShots->Text = ref new Platform::String(wsbuffer, length);

    // High Score is not used for showing Game Statistics
    HighScoreTitle->Visibility = ::Visibility::Collapsed;
    HighScoreData->Visibility  = ::Visibility::Collapsed;
}

//----------------------------------------------------------------------

void MainPage::SetGameOver(
    bool win,
    int maxLevel,
    int hitCount,
    int shotCount,
    int highScore
    )
{
    if (win)
    {
        GameInfoOverlayTitle->Text = "You Won!";
        m_possiblePurchaseUpgrade = true;
        OptionalTrialUpgrade();
    }
    else
    {
        GameInfoOverlayTitle->Text = "Game Over";
        m_possiblePurchaseUpgrade = false;
        PurchaseUpgrade->Visibility = ::Visibility::Collapsed;
    }
    Loading->Visibility = ::Visibility::Collapsed;
    Stats->Visibility = ::Visibility::Visible;
    LevelStart->Visibility = ::Visibility::Collapsed;
    PauseData->Visibility = ::Visibility::Collapsed;

    static const int bufferLength = 20;
    static char16 wsbuffer[bufferLength];

    int length = swprintf_s(wsbuffer, bufferLength, L"%d", maxLevel);
    LevelsCompleted->Text = ref new Platform::String(wsbuffer, length);

    length = swprintf_s(wsbuffer, bufferLength, L"%d", hitCount);
    TotalPoints->Text = ref new Platform::String(wsbuffer, length);

    length = swprintf_s(wsbuffer, bufferLength, L"%d", shotCount);
    TotalShots->Text = ref new Platform::String(wsbuffer, length);

    // Show High Score
    HighScoreTitle->Visibility = ::Visibility::Visible;
    HighScoreData->Visibility  = ::Visibility::Visible;
    length = swprintf_s(wsbuffer, bufferLength, L"%d", highScore);
    HighScore->Text = ref new Platform::String(wsbuffer, length);
}

//----------------------------------------------------------------------

void MainPage::SetLevelStart(
    int level,
    Platform::String^ objective,
    float timeLimit,
    float bonusTime
    )
{
    static const int bufferLength = 20;
    static char16 wsbuffer[bufferLength];

    int length = swprintf_s(wsbuffer, bufferLength, L"Level %d", level);
    GameInfoOverlayTitle->Text = ref new Platform::String(wsbuffer, length);

    Loading->Visibility = ::Visibility::Collapsed;
    Stats->Visibility = ::Visibility::Collapsed;
    LevelStart->Visibility = ::Visibility::Visible;
    PauseData->Visibility = ::Visibility::Collapsed;

    Objective->Text = objective;

    length = swprintf_s(wsbuffer, bufferLength, L"%6.1f sec", timeLimit);
    TimeLimit->Text = ref new Platform::String(wsbuffer, length);

    if (bonusTime > 0.0)
    {
        BonusTimeTitle->Visibility = ::Visibility::Visible;
        BonusTimeData->Visibility  = ::Visibility::Visible;
        length = swprintf_s(wsbuffer, bufferLength, L"%6.1f sec", bonusTime);
        BonusTime->Text = ref new Platform::String(wsbuffer, length);
    }
    else
    {
        BonusTimeTitle->Visibility = ::Visibility::Collapsed;
        BonusTimeData->Visibility  = ::Visibility::Collapsed;
    }
}

//----------------------------------------------------------------------

void MainPage::SetPause(int level, int hitCount, int shotCount, float timeRemaining)
{
    GameInfoOverlayTitle->Text = "Paused";
    Loading->Visibility = ::Visibility::Collapsed;
    Stats->Visibility = ::Visibility::Collapsed;
    LevelStart->Visibility = ::Visibility::Collapsed;
    PauseData->Visibility = ::Visibility::Visible;

    static const int bufferLength = 20;
    static char16 wsbuffer[bufferLength];

    int length = swprintf_s(wsbuffer, bufferLength, L"%d", level);
    PauseLevel->Text = ref new Platform::String(wsbuffer, length);

    length = swprintf_s(wsbuffer, bufferLength, L"%d", hitCount);
    PauseHits->Text = ref new Platform::String(wsbuffer, length);

    length = swprintf_s(wsbuffer, bufferLength, L"%d", shotCount);
    PauseShots->Text = ref new Platform::String(wsbuffer, length);

    length = swprintf_s(wsbuffer, bufferLength, L"%6.1f sec", timeRemaining);
    PauseTimeRemaining->Text = ref new Platform::String(wsbuffer, length);
}

//----------------------------------------------------------------------

void MainPage::SetSnapped()
{
    VisualStateManager::GoToState(this, "SnappedState", true);
}

//----------------------------------------------------------------------

void MainPage::HideSnapped()
{
    VisualStateManager::GoToState(this, "UnsnappedState", true);
}

//----------------------------------------------------------------------

void MainPage::OnGameInfoOverlayTapped(Object^ /* sender */, TappedRoutedEventArgs^ /* args */)
{
    m_app->PressComplete();
}

//----------------------------------------------------------------------

void MainPage::OnPlayButtonClicked(Object^ /* sender */, RoutedEventArgs^ /* args */)
{
    m_app->PressComplete();
}

//----------------------------------------------------------------------

void MainPage::OnResetButtonClicked(Object^ /* sender */, RoutedEventArgs^ /* args */)
{
    m_app->ResetGame();
    GameAppBar->IsOpen = false;
}

//----------------------------------------------------------------------

void MainPage::LicenseChanged(
    ListingInformation^ listing,
    LicenseInformation^ license
    )
{
    m_listingInformation = listing;
    m_licenseInformation = license;

    // This function may be called from a different thread.
    // All XAML updates need to occur on the UI thread so dispatch to ensure this is true.
    Dispatcher->RunAsync(
        CoreDispatcherPriority::Normal,
        ref new DispatchedHandler([this]()
        {
            if (m_licenseInformation->IsActive)
            {
                if (!m_licenseInformation->IsTrial)
                {
                    PurchaseUpgrade->Visibility = ::Visibility::Collapsed;
                }
            }
            else
            {
                ChangeBackground->Visibility = ::Visibility::Collapsed;
            }

            if (m_licenseInformation->IsActive && m_licenseInformation->IsTrial)
            {
                if (m_listingInformation != nullptr)
                {
                    PurchaseMessage->Text =
                        "You are running a trial version. Purchase the full version for: " + m_listingInformation->FormattedPrice;
                }
                else
                {
                    PurchaseMessage->Text =
                        "You are running a trial version. Purchase the full version.";
                }
                if (m_possiblePurchaseUpgrade)
                {
                    PurchaseUpgrade->Visibility = ::Visibility::Visible;
                }
            }

            if (m_licenseInformation != nullptr)
            {
                auto items = dynamic_cast<Platform::Collections::Vector<Platform::Object^>^>(ProductListView->ItemsSource);
                for (uint32 i = 0; i < items->Size; i++)
                {
                    dynamic_cast<ProductItem^>(items->GetAt(i))->UpdateContent(m_licenseInformation);
                }
            }
            if (m_listingInformation != nullptr)
            {
                auto items = dynamic_cast<Platform::Collections::Vector<Platform::Object^>^>(ProductListView->ItemsSource);
                for (uint32 i = 0; i < items->Size; i++)
                {
                    dynamic_cast<ProductItem^>(items->GetAt(i))->UpdateContent(m_listingInformation);
                }
            }
        })
        );
}

//----------------------------------------------------------------------

void MainPage::OnBuyAppButtonTapped(Object^ sender, TappedRoutedEventArgs^ args)
{
    args->Handled = true;
    OnBuySelectorClicked(sender, args);
}

//----------------------------------------------------------------------

void MainPage::OnBuySelectorClicked(Object^ sender, RoutedEventArgs^ /* args */)
{
    Platform::String^ tag = dynamic_cast<Platform::String^>(dynamic_cast<Button^>(sender)->CommandParameter);
    StoreUnavailable->Visibility = ::Visibility::Collapsed;

    if (tag == "MainApp")
    {
        if ((m_licenseInformation != nullptr) && m_licenseInformation->IsActive)
        {
            if (m_licenseInformation->IsTrial)
            {
#ifdef USE_STORE_SIMULATOR
                task<Platform::String^> purchaseTask(CurrentAppSimulator::RequestAppPurchaseAsync(false));
#else
                task<Platform::String^> purchaseTask(CurrentApp::RequestAppPurchaseAsync(false));
#endif
                purchaseTask.then([this](task<Platform::String^> previousTask)
                {
                    try
                    {
                        // Try getting all exceptions from the continuation chain above this point
                        previousTask.get();
                    }
                    catch (Platform::Exception^ exception)
                    {
                        if (exception->HResult == E_FAIL)
                        {
                            StoreUnavailable->Visibility = ::Visibility::Visible;
                        }
                    }
                });
            }
        }
    }
    else
    {
        if ((m_licenseInformation != nullptr) && m_licenseInformation->IsActive && !m_licenseInformation->IsTrial)
        {
            if (!m_licenseInformation->ProductLicenses->Lookup(tag)->IsActive)
            {
#ifdef USE_STORE_SIMULATOR
                task<Platform::String^> purchaseTask(CurrentAppSimulator::RequestProductPurchaseAsync(tag, false));
#else
                task<Platform::String^> purchaseTask(CurrentApp::RequestProductPurchaseAsync(tag, false));
#endif
                purchaseTask.then([=](task<Platform::String^> previousTask)
                {
                    try
                    {
                        // Try getting all exceptions from the continuation chain above this point
                        previousTask.get();
                    }
                    catch (Platform::Exception^ exception)
                    {
                        if (exception->HResult == E_FAIL)
                        {
                            StoreUnavailable->Visibility = ::Visibility::Visible;
                        }
                    }
                });
            }
        }
    }
}

//----------------------------------------------------------------------

void MainPage::OnChangeBackgroundButtonClicked(Object^ /* sender */, RoutedEventArgs^ /* args */)
{
    if ((m_licenseInformation != nullptr) && m_licenseInformation->IsActive)
    {
        if (m_licenseInformation->IsTrial ||
            (!m_licenseInformation->ProductLicenses->Lookup("NightBackground")->IsActive  &&
            !m_licenseInformation->ProductLicenses->Lookup("DayBackground")->IsActive))
        {
            ShowStoreFlyout();
        }
        else
        {
            m_app->CycleBackground();
        }
    }
}

//----------------------------------------------------------------------

void MainPage::OnResetLicenseButtonClicked(Object^ /* sender */, RoutedEventArgs^ /* args */)
{
#ifdef USE_STORE_SIMULATOR
    m_app->ResetLicense();
#endif
    m_app->SetBackground(0);
}

//----------------------------------------------------------------------

void MainPage::OptionalTrialUpgrade()
{
    PurchaseUpgrade->Visibility = ::Visibility::Collapsed;

    if (m_licenseInformation != nullptr)
    {
        if (m_licenseInformation->IsActive && m_licenseInformation->IsTrial)
        {
            if (m_listingInformation != nullptr)
            {
                PurchaseMessage->Text =
                    "You are running a trial version. Purchase the full version for: " + m_listingInformation->FormattedPrice;
            }
            else
            {
                PurchaseMessage->Text =
                    "You are running a trial version. Purchase the full version.";
            }
            PurchaseUpgrade->Visibility = ::Visibility::Visible;
        }
    }
}

//----------------------------------------------------------------------

void MainPage::OnStoreReturnClicked(Object^ /* sender */, RoutedEventArgs^ /* args */)
{
    StoreFlyout->IsOpen = false;
    StoreFlyout->Visibility = ::Visibility::Collapsed;
}

//----------------------------------------------------------------------

void MainPage::OnLoadStoreClicked(Object^ /* sender */, RoutedEventArgs^ /* args */)
{
    m_app->PauseRequested();
    ShowStoreFlyout();
}

//----------------------------------------------------------------------

void MainPage::SetProductItems(
    ListingInformation^ listing,
    LicenseInformation^ license
    )
{
    auto items = ref new Platform::Collections::Vector<Platform::Object^>();
    items->Append(ref new ProductItem(listing, license, "MainApp", true));
    items->Append(ref new ProductItem(listing, license, "AutoFire", false));
    items->Append(ref new ProductItem(listing, license, "NightBackground", false));
    items->Append(ref new ProductItem(listing, license, "DayBackground", false));
    ProductListView->ItemsSource = items;
    StoreUnavailable->Visibility = ::Visibility::Collapsed;
}

//----------------------------------------------------------------------

void MainPage::OnWindowSizeChanged()
{
    StoreGrid->Height = Window::Current->Bounds.Height;
    StoreFlyout->HorizontalOffset = Window::Current->Bounds.Width - StoreGrid->Width;
}

//----------------------------------------------------------------------

void MainPage::OnAppBarOpened(Object^ /* sender */, Object^ /* args */)
{
    m_app->PauseRequested();
}

//----------------------------------------------------------------------

void MainPage::ShowStoreFlyout()
{
    StoreGrid->Height = Window::Current->Bounds.Height;
    StoreUnavailable->Visibility = ::Visibility::Collapsed;
    StoreFlyout->HorizontalOffset = Window::Current->Bounds.Width - StoreGrid->Width;
    StoreFlyout->IsOpen = true;
    StoreFlyout->Visibility = ::Visibility::Visible;
    GameAppBar->IsOpen = false;
}

//----------------------------------------------------------------------
