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
#include "S7_ListSmartCards.g.h"
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Media;

namespace SDKSample
{
    namespace SmartCardSample
    {
        [Windows::UI::Xaml::Data::Bindable]
        [Windows::Foundation::Metadata::WebHostHiddenAttribute]
        public ref class SmartCardItem sealed
        {
        public:
            SmartCardItem();

            property Platform::String^ ReaderName
            {
                Platform::String^  get()
                {
                    return readerName;
                }

                void set(Platform::String^  value)
                {
                    readerName = value;
                }
            }

            property Platform::String^ CardName
            {
                Platform::String^  get()
                {
                    return cardName;
                }

                void set(Platform::String^  value)
                {
                    cardName = value;
                }
            }
        private:
            Platform::String^ readerName;
            Platform::String^ cardName;
        };

        public ref class Scenario7 sealed
        {
        public:
            Scenario7();

        private:
            void ListSmartCards_Click(Platform::Object^ sender,
                                      Windows::UI::Xaml::RoutedEventArgs^ e);
            Platform::Collections::Vector<SmartCardItem^>^ smartCardItems;
        };


    }
}
