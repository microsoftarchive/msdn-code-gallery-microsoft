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
// ScenarioSingle.xaml.cpp
// Implementation of the ScenarioSingle class
//

#include "pch.h"
#include "ScenarioSingle.xaml.h"

using namespace ContactPicker;
using namespace concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::Foundation::Collections;
using namespace Windows::Storage::Streams;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Media::Imaging;

void AppendContactFieldValues(TextBlock^ header, TextBlock^ content, IVectorView<Contacts::ContactField^>^ fields);

ScenarioSingle::ScenarioSingle()
{
    InitializeComponent();
    PickAContactButton->Click += ref new RoutedEventHandler(this, &ScenarioSingle::PickAContactButton_Click);
}

void ScenarioSingle::PickAContactButton_Click(Object^ sender, RoutedEventArgs^ e)
{
    MainPage^ page = MainPage::Current;

    if (page->EnsureUnsnapped())
    {
        auto contactPicker = ref new Contacts::ContactPicker();
        contactPicker->CommitButtonText = "Select";

        create_task(contactPicker->PickSingleContactAsync()).then([this](Contacts::ContactInformation^ contact)
        {
            if (contact != nullptr)
            {
                OutputFields->Visibility = Windows::UI::Xaml::Visibility::Visible;
                OutputEmpty->Visibility = Windows::UI::Xaml::Visibility::Collapsed;

                OutputName->Text = contact->Name;
                AppendContactFieldValues(OutputEmailHeader, OutputEmails, contact->Emails);
                AppendContactFieldValues(OutputPhoneNumberHeader, OutputPhoneNumbers, contact->PhoneNumbers);

                create_task(contact->GetThumbnailAsync()).then([this](IRandomAccessStreamWithContentType^ stream)
                {
                    if (stream != nullptr && stream->Size > 0)
                    {
                        BitmapImage^ bitmap = ref new BitmapImage();
                        bitmap->SetSource(stream);
                        OutputThumbnail->Source = bitmap;
                    }
                    else
                    {
                        OutputThumbnail->Source = nullptr;
                    }
                });
            }
            else
            {
                OutputEmpty->Visibility = Windows::UI::Xaml::Visibility::Visible;
                OutputFields->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
                OutputThumbnail->Source = nullptr;
            }
        });
    }
}

void AppendContactFieldValues(TextBlock^ header, TextBlock^ content, IVectorView<Contacts::ContactField^>^ fields)
{
    if (fields->Size > 0)
    {
        String^ output = "";
        std::for_each(begin(fields), end(fields), [&output](Contacts::ContactField^ field)
        {
            output += field->Value + "\n";
        });

        header->Visibility = Visibility::Visible;
        content->Visibility = Visibility::Visible;
        content->Text = output;
    }
    else
    {
        header->Visibility = Visibility::Collapsed;
        content->Visibility = Visibility::Collapsed;
    }
}
