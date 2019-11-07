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
// Scenario6.xaml.h
// Declaration of the Scenario6 class
//

#pragma once

#include "pch.h"
#include "Scenario6.g.h"
#include "MainPage.xaml.h"

using namespace Platform;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Text ;
using namespace std ;

namespace SDKSample
{
    namespace TextEditing
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
    
        private:
    		// button click handlers
    		void BoldButtonClick(Object^ sender, RoutedEventArgs^ e) ;
    		void ItalicButtonClick(Object^ sender, RoutedEventArgs^ e);
    		void FontColorButtonClick(Object^ sender, RoutedEventArgs^ e);
    		void ColorButtonClick(Object^ sender, RoutedEventArgs^ e);
    
    		// focus change handlers
    		void FontColorButtonLostFocus(Object^ sender, RoutedEventArgs^ e); 
    		void FindBoxLostFocus(Object^ sender, RoutedEventArgs^ e);
    		void FindBoxGotFocus(Object^ sender, RoutedEventArgs^ e); 
    
    		// text changed handlers
    		void FindBoxTextChanged(Object^ sender, Windows::UI::Xaml::Controls::TextChangedEventArgs^ e);
    
    		// helper functions
    		void ClearAllHighlightedWords() ;
    		void FindAndHighlightText(String^ textToFind) ;
    		void LoadContentAsync();
    
            MainPage^ rootPage;
    		vector<ITextRange^> m_highlightedWords ;
            void Default_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
            void Other_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
        };
    }
}
