//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S2_AddRoutePolicy.xaml.h
// Declaration of the S2_AddRoutePolicy class
//
using namespace Windows::System;
using namespace Platform;
using namespace Windows::Networking;
#pragma once
#include "S2_AddRoutePolicy.g.h"

namespace SDKSample
{
    namespace ConnectivityManager
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        public ref class S2_AddRoutePolicy sealed
        {
        public:
            S2_AddRoutePolicy();

        private:
            void AddRoutePolicy_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void RemoveRoutePolicy_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);

            // Display Methods
            void DisplayError(String^ message);
            void DisplayWarning(String^ message);
            void DisplayInfo(String^ message);

            DomainNameType ParseDomainNameType(String^ input);
        };
    }
}
