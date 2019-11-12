// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "pch.h"
#include "Scenario8_CopyAFile.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace FileAccess
    {
        /// <summary>
        /// Copying a file.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario8 sealed
        {
        public:
            Scenario8();

        private:
            MainPage^ rootPage;

            void CopyFileButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
