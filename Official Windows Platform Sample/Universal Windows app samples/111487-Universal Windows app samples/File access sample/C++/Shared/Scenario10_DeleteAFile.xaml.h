// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "pch.h"
#include "Scenario10_DeleteAFile.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace FileAccess
    {
        /// <summary>
        /// Deleting a file.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario10 sealed
        {
        public:
            Scenario10();

        private:
            MainPage^ rootPage;

            void DeleteFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
