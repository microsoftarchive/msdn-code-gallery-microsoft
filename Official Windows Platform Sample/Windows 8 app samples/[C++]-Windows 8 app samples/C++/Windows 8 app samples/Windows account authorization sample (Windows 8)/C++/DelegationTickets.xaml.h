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
// DelegationTickets.xaml.h
// Declaration of the DelegationTickets class
//

#pragma once

#include "pch.h"
#include "DelegationTickets.g.h"
#include "MainPage.xaml.h"
#include "HttpRequest.h"

using namespace Windows::Foundation;

namespace SDKSample
{
    namespace MicrosoftAccount
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
        [ Windows::Foundation::Metadata::WebHostHidden ]
        public ref class DelegationTickets sealed
        {
        public:
            DelegationTickets();

            property Platform::String^ AccessTicket
            {
                Platform::String^ get()
                { 
                    return accessTicket;
                }

                void set(Platform::String^ value)
                {
                    accessTicket = value;
                }
            }

            property bool NeedsToGetTicket
            {
                bool get()
                { 
                    return accessTicket == "";
                }
            }

            property bool CanSignOut
            {
                bool get()
                { 
                    return authenticator->CanSignOut; 
                }
            }

        protected:

            virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:

            Windows::Security::Authentication::OnlineId::OnlineIdAuthenticator^ authenticator;
            MainPage^ rootPage;
            Platform::String^ accessTicket;
            Web::HttpRequest httpRequest;
            concurrency::cancellation_token_source cancellationTokenSource;

            void SignInButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void SignOutButton_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
