//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

//
// S2-IndirectContentUri.xaml.cpp
// Implementation of the IndirectContentUriScenario class
//

#include "pch.h"
#include "S2-IndirectContentUri.xaml.h"
#include "MainPage.xaml.h"

using namespace SDKSample;
using namespace SDKSample::SDKTemplate;

using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;

IndirectContentUriScenario::IndirectContentUriScenario()
{
    InitializeComponent();
    UpdateIndirectUriBlock();
}

void SDKTemplate::IndirectContentUriScenario::SetIndirectContentUri_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    MainPage::Current->NotifyUser("", NotifyType::StatusMessage);

    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        Platform::String ^uriString = IndirectContentUriBox->Text;
        if (uriString != "")
        {
            Windows::Foundation::Uri ^uri;

            try
            {
                uri = ref new Windows::Foundation::Uri(IndirectContentUriBox->Text);
            }
            catch (...)
            {
                MainPage::Current->NotifyUser("A URI must be provided that has the form scheme://address", NotifyType::ErrorMessage);
            }

            if (uri)
            {
                Windows::Networking::BackgroundTransfer::ContentPrefetcher::IndirectContentUri = uri;
                UpdateIndirectUriBlock();
                IndirectContentUriBox->Text = "";
            }
        }
    }
}

void SDKTemplate::IndirectContentUriScenario::ClearIndirectContentUri_Click(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
    MainPage::Current->NotifyUser("", NotifyType::StatusMessage);

    Button^ b = dynamic_cast<Button^>(sender);
    if (b != nullptr)
    {
        Windows::Networking::BackgroundTransfer::ContentPrefetcher::IndirectContentUri = nullptr;
        UpdateIndirectUriBlock();
    }
}

void SDKTemplate::IndirectContentUriScenario::UpdateIndirectUriBlock()
{
    MainPage::Current->NotifyUser("", NotifyType::StatusMessage);

    Windows::Foundation::Uri ^uri = Windows::Networking::BackgroundTransfer::ContentPrefetcher::IndirectContentUri;
    if (uri != nullptr)
    {
        IndirectContentUriBlock->Text = "The indirect content URI is " + uri->AbsoluteUri;
    }
    else
    {
        IndirectContentUriBlock->Text = "There is no indirect content URI set";
    }
}