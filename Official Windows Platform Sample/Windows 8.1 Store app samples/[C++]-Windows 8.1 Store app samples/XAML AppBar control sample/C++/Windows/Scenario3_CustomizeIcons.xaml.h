//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario3.xaml.h
// Declaration of the Scenario3 class
//

#pragma once
#include "Scenario3_CustomizeIcons.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
    namespace AppBarControl
    {
        /// <summary>
        /// An empty page that can be used on its own or navigated to within a Frame.
        /// </summary>
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class Scenario3 sealed
        {
        public:
            Scenario3();

        private:
			MainPage^ rootPage;
			Windows::UI::Xaml::Controls::StackPanel^ leftPanel;
			Windows::UI::Xaml::Controls::StackPanel^ rightPanel;
			Platform::Collections::Vector<Windows::UI::Xaml::UIElement^>^ leftItems;
			Platform::Collections::Vector<Windows::UI::Xaml::UIElement^>^ rightItems;
			void CopyButtons(Windows::UI::Xaml::Controls::StackPanel^ panel, Platform::Collections::Vector<Windows::UI::Xaml::UIElement^>^ list);
			void RestoreButtons(Windows::UI::Xaml::Controls::StackPanel^ panel, Platform::Collections::Vector<Windows::UI::Xaml::UIElement^>^ list);

		protected:
			virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
			virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        };
    }
}
