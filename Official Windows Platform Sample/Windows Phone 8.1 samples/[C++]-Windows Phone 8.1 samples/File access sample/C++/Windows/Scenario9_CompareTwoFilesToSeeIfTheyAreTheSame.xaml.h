// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "pch.h"
#include "Scenario9_CompareTwoFilesToSeeIfTheyAreTheSame.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace FileAccess
    {
        /// <summary>
        /// Comparing two files to see if they are the same file.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario9 sealed
        {
        public:
            Scenario9();

        private:
            MainPage^ rootPage;

            void CompareFilesButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
