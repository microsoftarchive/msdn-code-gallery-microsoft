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
// ScenarioMultiple.xaml.cpp
// Implementation of the ScenarioMultiple class
//
#include "pch.h"
#include "ScenarioMultiple.xaml.h"

using namespace ContactPicker;
using namespace concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media::Imaging;

ScenarioMultiple::ScenarioMultiple()
{
    InitializeComponent();
    PickContactsButton->Click += ref new RoutedEventHandler(this, &ScenarioMultiple::PickContactsButton_Click);
}

void ScenarioMultiple::PickContactsButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    MainPage^ page = MainPage::Current;

    if (page->EnsureUnsnapped())
    {
        auto contactPicker = ref new Contacts::ContactPicker();
        contactPicker->CommitButtonText = "Select";

        create_task(contactPicker->PickMultipleContactsAsync()).then([this](IVectorView<Contacts::ContactInformation^>^ contacts)
        {
            if (contacts->Size > 0)
            {
                String^ output = "Selected contacts:\n";
                std::for_each(begin(contacts), end(contacts), [&output](Contacts::ContactInformation^ contact)
                {
                    output += contact->Name + "\n";
                });
                OutputText->Text = output;
            }
            else
            {
                OutputText->Text = "No contacts were selected";
            }
        });
    }
}

