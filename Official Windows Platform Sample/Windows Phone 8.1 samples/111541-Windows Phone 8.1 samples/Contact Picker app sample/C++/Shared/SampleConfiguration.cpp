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
#include "MainPage.xaml.h"
#include "SampleConfiguration.h"

using namespace SDKSample;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::ApplicationModel::Contacts;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;

/// <summary>
/// Sets the name of the scenarios.
/// </summary>
Platform::Array<Scenario>^ MainPage::scenariosInner = ref new Platform::Array<Scenario>
{
	{ "Pick a single contact", "SDKSample.Scenario1_PickContact" },
    { "Pick multiple contacts", "SDKSample.Scenario2_PickContacts" },
};

/// <summary>
/// Sample set of contacts to pick from.
/// </summary>
Platform::Array<SampleContact>^ ContactCreator::CreateSampleContacts()
{
	Platform::Array<SampleContact>^ contacts = ref new Platform::Array<SampleContact>
	{
		{
			/*DisplayName*/ "David Jaffe",
			/*FirstName*/ "David",
			/*LastName*/ "Jaffe",
			/*HomeEmail*/ "david@contoso.com",
			/*WorkEmail*/ "david@cpandl.com",
			/*HomePhone*/ "248-555-0150",
			/*WorkPhone*/ "",
			/*MobilePhone*/ "",
			/*Id*/ "761cb6fb-0270-451e-8725-bb575eeb24d5"
		},

		{
			/*DisplayName*/ "Kim Abercrombie",
			/*FirstName*/ "Kim",
			/*LastName*/ "Abercrombie",
			/*HomeEmail*/ "kim@contoso.com",
			/*WorkEmail*/ "kim@adatum.com",
			/*HomePhone*/ "444 555-0001",
			/*WorkPhone*/ "245 555-0123",
			/*MobilePhone*/ "921 555-0187",
			/*Id*/ "49b0652e-8f39-48c5-853b-e5e94e6b8a11"
		},

		{
			/*DisplayName*/ "Jeff Phillips",
			/*FirstName*/ "Jeff",
			/*LastName*/ "Phillips",
			/*HomeEmail*/ "jeff@contoso.com",
			/*WorkEmail*/ "jeff@fabrikam.com",
			/*HomePhone*/ "987-555-0199",
			/*WorkPhone*/ "",
			/*MobilePhone*/ "543-555-0111",
			/*Id*/ "864abfb4-8998-4355-8236-8d69e47ec832"
		},

		{
			/*DisplayName*/ "Arlene Huff",
			/*FirstName*/ "Arlene",
			/*LastName*/ "Huff",
			/*HomeEmail*/ "arlene@contoso.com",
			/*WorkEmail*/ "",
			/*HomePhone*/ "",
			/*WorkPhone*/ "",
			/*MobilePhone*/ "234-555-0156",
			/*Id*/ "27347af8-0e92-45b8-b14c-dd70fcd3b4a6"
		},

		{
			/*DisplayName*/ "Miles Reid",
			/*FirstName*/ "Miles",
			/*LastName*/ "Reid",
			/*HomeEmail*/ "miles@contoso.com",
			/*WorkEmail*/ "miles@proseware.com",
			/*HomePhone*/ "",
			/*WorkPhone*/ "",
			/*MobilePhone*/ "",
			/*Id*/ "e3d24a99-0e29-41af-9add-18f5e3cfc518"
		}
	};

	return contacts;
}