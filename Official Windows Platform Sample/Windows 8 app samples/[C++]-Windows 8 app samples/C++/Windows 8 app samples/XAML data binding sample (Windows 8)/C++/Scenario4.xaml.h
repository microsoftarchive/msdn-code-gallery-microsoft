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
// Scenario4.xaml.h
// Declaration of the Scenario4 class
//

#pragma once

#include "pch.h"
#include "Scenario4.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace DataBinding
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class Scenario4 sealed
        {
        public:
            Scenario4();
    
        private:
            MainPage^ rootPage;
            void Scenario4Reset(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            
        };
    }
}
