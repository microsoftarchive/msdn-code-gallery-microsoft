// Copyright (c) Microsoft. All rights reserved.

#pragma once

#include "pch.h"
#include "Scenario11_TryToGetAFileWithoutGettingAnError.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace FileAccess
    {
        /// <summary>
        /// Attempting to get a file with no error on failure.
        /// </summary>
        [Windows::Foundation::Metadata::WebHostHidden]
        public ref class Scenario11 sealed
        {
        public:
            Scenario11();

        private:
            MainPage^ rootPage;
        };
    }
}
