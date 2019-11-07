//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

#pragma once

#include "MainPage.g.h"
#include "ProductItem.h"

namespace Simple3DGameXaml
{
    ref class App;

    public enum class GameInfoOverlayCommand
    {
        None,
        TapToContinue,
        PleaseWait,
        PlayAgain,
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class MainPage sealed
    {
    public:
        MainPage();

        void SetApp(App^ app);
        void SetGameLoading();
        void SetGameStats(int maxLevel, int hitCount, int shotCount);
        void SetGameOver(bool win, int maxLevel, int hitCount, int shotCount, int highScore);
        void SetLevelStart(int level, Platform::String^ objective, float timeLimit, float bonusTime);
        void SetPause(int level, int hitCount, int shotCount, float timeRemaining);
        void SetSnapped();
        void HideSnapped();
        void SetAction(GameInfoOverlayCommand action);
        void HideGameInfoOverlay();
        void ShowGameInfoOverlay();
        void LicenseChanged(
            Windows::ApplicationModel::Store::ListingInformation^ listing,
            Windows::ApplicationModel::Store::LicenseInformation^ license
            );
        void SetProductItems(
            Windows::ApplicationModel::Store::ListingInformation^ listing,
            Windows::ApplicationModel::Store::LicenseInformation^ license
            );
        void OnWindowSizeChanged();

        Windows::UI::Xaml::Controls::SwapChainBackgroundPanel^ GetSwapChainBackgroundPanel() { return DXSwapChainPanel; };

    protected:
        void OnPlayButtonClicked(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ args);
        void OnResetButtonClicked(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ args);
        void OnBuyAppButtonTapped(Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ args);
        void OnBuySelectorClicked(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ args);
        void OnChangeBackgroundButtonClicked(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ args);
        void OnResetLicenseButtonClicked(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ args);
        void OnGameInfoOverlayTapped(Object^ sender, Windows::UI::Xaml::Input::TappedRoutedEventArgs^ args);
        void OnAppBarOpened(Object^ sender, Object^ args);
        void OnStoreReturnClicked(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ args);
        void OnLoadStoreClicked(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ args);

        void OptionalTrialUpgrade();
        void ShowStoreFlyout();

    private:
        App^ m_app;
        Windows::ApplicationModel::Store::LicenseInformation^ m_licenseInformation;
        Windows::ApplicationModel::Store::ListingInformation^ m_listingInformation;
        bool m_possiblePurchaseUpgrade;
    };
}
