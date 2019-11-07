//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// Scenario1.xaml.h
// Declaration of the Scenario1 class
//

#pragma once
#include "Scenario1.g.h"

namespace SDKSample
{
    namespace MultipleViews
    {
        [Windows::UI::Xaml::Data::Bindable]
        public ref class SizePreferenceString sealed
        {
        public: 
            SizePreferenceString(Windows::UI::ViewManagement::ViewSizePreference preference, Platform::String^ title);

            property Platform::String^ Title
            {
                Platform::String^ get();
            };

            property Windows::UI::ViewManagement::ViewSizePreference Preference
            {
                Windows::UI::ViewManagement::ViewSizePreference get();
            };
        private:
            Platform::String^ title;
            Windows::UI::ViewManagement::ViewSizePreference preference;
            
        };

        public ref class Scenario1 sealed
        {
        public:
            Scenario1();

        private:
            void CreateView_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void ShowAsStandalone_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            Platform::Collections::Vector<SizePreferenceString^>^ GenerateSizePreferenceBinding();

            MainPage^ rootPage;
        };

     }
}
