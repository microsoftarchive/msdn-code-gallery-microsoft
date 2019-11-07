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
// ContactPickerPage.xaml.h
// Declaration of the ContactPickerPage class
//

#pragma once

#include "pch.h"
#include "ContactPickerPage.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
	/// <summary>
	/// ContactPickerPage class to select one or more contacts.
	/// </summary>
    [Windows::Foundation::Metadata::WebHostHidden]
    public ref class ContactPickerPage sealed
    {
    public:
        ContactPickerPage();

    protected:
		virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
        virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;

    internal:
        Windows::ApplicationModel::Contacts::Provider::ContactPickerUI^ contactPickerUI;
		virtual void Activate(Windows::ApplicationModel::Activation::ContactPickerActivatedEventArgs^ e);

    private:
		void AddSampleContact(SDKSample::SampleContact sampleContact);
        SDKSample::SampleContact ResolveSampleContact(Object^ listBoxItem);

        void OnContactRemoved(Windows::ApplicationModel::Contacts::Provider::ContactPickerUI^ sender, Windows::ApplicationModel::Contacts::Provider::ContactRemovedEventArgs^ e);
        void ContactList_SelectionChanged(Platform::Object^ sender, Windows::UI::Xaml::Controls::SelectionChangedEventArgs^ e);

        Windows::Foundation::EventRegistrationToken token;
    };
}
