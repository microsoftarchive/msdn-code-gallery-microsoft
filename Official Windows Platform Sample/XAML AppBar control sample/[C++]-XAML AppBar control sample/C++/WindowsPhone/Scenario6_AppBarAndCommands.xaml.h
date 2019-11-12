//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario6.xaml.h
// Declaration of the Scenario6 class
//

#pragma once
#include "Scenario6_AppBarAndCommands.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace AppBarControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class Scenario6 sealed
        {
        public:
            Scenario6();
		protected:
			virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
			virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

        private:
			MainPage^ rootPage;
			Platform::Collections::Vector<Windows::UI::Xaml::Controls::ICommandBarElement^>^ commands;
			void BottomAppBar_Opened(Platform::Object^ sender, Platform::Object^ e);
			void BottomAppBar_Closed(Platform::Object^ sender,Platform::Object^ e);

			void ShowCommands_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void HideCommands_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void HideAppBarButtons();
			void ShowAppBarButtons();

			Windows::Foundation::EventRegistrationToken openedToken;
			Windows::Foundation::EventRegistrationToken closedToken;
		};
    }
}
