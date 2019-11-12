//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#include "pch.h"
#include <ppltasks.h>
#include <ppl.h>  
#include "PhoneConfiguration.h"

using namespace concurrency;
using namespace Platform;
using namespace Windows::Phone::PersonalInformation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace SDKSample;

/// <summary>
/// Creates a contact store and add contacts.
/// </summary>
void PhoneConfiguration::CreateContactStore()
{
	create_task(ContactStore::CreateOrOpenAsync(ContactStoreSystemAccessMode::ReadWrite,
		ContactStoreApplicationAccessMode::ReadOnly)).then([](ContactStore^ contactStore)
	{
		for (SampleContact sampleContact : ContactCreator::CreateSampleContacts())
		{
			StoredContact^ contact = ref new StoredContact(contactStore);
			create_task(contact->GetPropertiesAsync()).then([sampleContact](IMap <String^, Object^>^ props)
			{
				if (!(sampleContact.FirstName->IsEmpty()))
				{
					props->Insert(KnownContactProperties::GivenName, sampleContact.FirstName);
				}
				
				if (!sampleContact.HomeEmail->IsEmpty())
				{
					props->Insert(KnownContactProperties::Email, sampleContact.HomeEmail);
				}

				if (!sampleContact.LastName->IsEmpty())
				{
					props->Insert(KnownContactProperties::FamilyName, sampleContact.LastName);
				}

				if (!sampleContact.HomeEmail->IsEmpty())
				{
					props->Insert(KnownContactProperties::Email, sampleContact.HomeEmail);
				}

				if (!sampleContact.WorkEmail->IsEmpty())
				{
					props->Insert(KnownContactProperties::WorkEmail, sampleContact.WorkEmail);
				}

				if (!sampleContact.HomePhone->IsEmpty())
				{
					props->Insert(KnownContactProperties::Telephone, sampleContact.HomePhone);
				}

				if (!sampleContact.WorkPhone->IsEmpty())
				{
				    props->Insert(KnownContactProperties::CompanyTelephone, sampleContact.WorkPhone);
				}

				if (!sampleContact.MobilePhone->IsEmpty())
				{
					props->Insert(KnownContactProperties::MobileTelephone, sampleContact.MobilePhone);
				}
			}).then([contact]()
			{
				contact->SaveAsync();
			});
		}
	});
}
