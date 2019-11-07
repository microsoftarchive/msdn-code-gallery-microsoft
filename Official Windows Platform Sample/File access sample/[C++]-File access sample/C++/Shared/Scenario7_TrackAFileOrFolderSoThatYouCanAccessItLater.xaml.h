// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "pch.h"
#include "Scenario7_TrackAFileOrFolderSoThatYouCanAccessItLater.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace FileAccess
    {
        /// <summary>
        /// Persisting access to a storage item for future use.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario7 sealed
        {
        public:
            Scenario7();

        private:
            MainPage^ rootPage;

            void AddToListButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ShowListButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void OpenFromListButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
