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
// MainPage.xaml.h
// Declaration of the MainPage.xaml class.
//

#pragma once

#include "pch.h"
#include "MainPage.g.h"
#include "Common\LayoutAwarePage.h" // Required by generated header
#include "Constants.h"

namespace SecondaryTiles
{
    public enum class NotifyType
    {
        StatusMessage,
        ErrorMessage
    };

    public ref class MainPageSizeChangedEventArgs sealed
    {
    public:
        property Windows::UI::ViewManagement::ApplicationViewState ViewState
        {
            Windows::UI::ViewManagement::ApplicationViewState get()
            {
                return viewState;
            }

            void set(Windows::UI::ViewManagement::ApplicationViewState value)
            {
                viewState = value;
            }
        }

    private:
        Windows::UI::ViewManagement::ApplicationViewState viewState;
    };

    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class MainPage sealed
    {
    public:
        MainPage();

        property bool AutoSizeInputSectionWhenSnapped
        {
            bool get()
            {
                return autoSizeInputSectionWhenSnapped;
            }

            void set(bool value)
            {
                autoSizeInputSectionWhenSnapped = value;
            }
        }

        property Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ LaunchArgs;

        void NotifyUser(Platform::String^ strMessage, NotifyType type);
        void LoadScenario(Platform::String^ scenarioName);
        event Windows::Foundation::EventHandler<Platform::Object^>^ ScenarioLoaded;
        event Windows::Foundation::EventHandler<MainPageSizeChangedEventArgs^>^ MainPageResized;
        void NavigateToLaunchedFromSecondaryTile();

    private:
         ~MainPage();
       void SetSecondaryTiles(Platform::String^ strFeature);
        void PopulateScenarios();
        void InvalidateSize();
        void InvalidateViewState();

        Platform::Collections::Vector<Object^>^ scenarioList;
        Windows::UI::Xaml::Controls::Frame^ hiddenFrame;
        void Footer_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        bool autoSizeInputSectionWhenSnapped;

    protected:
        virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        void MainPage_SizeChanged(Object^ sender, Windows::UI::Xaml::SizeChangedEventArgs^ e);
        void Scenarios_SelectionChanged(Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);
        void MainPage_Loaded(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

    internal:
        static MainPage^ current;
    };
}
