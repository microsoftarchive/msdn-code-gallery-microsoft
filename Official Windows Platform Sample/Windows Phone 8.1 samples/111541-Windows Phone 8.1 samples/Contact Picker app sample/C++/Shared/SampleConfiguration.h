//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

#pragma once

#include <collection.h>

namespace SDKSample
{
	/// <summary>
	/// Feature scenario.
	/// </summary>
	public value struct Scenario
	{
		Platform::String^ Title;
		Platform::String^ ClassName;
	};

	/// <summary>
	/// Sample contact.
	/// </summary>
	public value struct SampleContact
	{
		Platform::String^ DisplayName;
		Platform::String^ FirstName;
		Platform::String^ LastName;
		Platform::String^ HomeEmail;
		Platform::String^ WorkEmail;
		Platform::String^ HomePhone;
		Platform::String^ WorkPhone;
		Platform::String^ MobilePhone;
		Platform::String^ Id;
	};

	/// <summary>
	/// Contact creator.
	/// </summary>
	public ref class ContactCreator sealed
	{
	public:		
		static Platform::Array<SampleContact>^ CreateSampleContacts();
	};

	/// <summary>
	/// MainPage feature name and scenarios.
	/// </summary>
	partial ref class MainPage
	{
	public:
		static property Platform::String^ FEATURE_NAME
		{
			Platform::String^ get()
			{
				return ref new Platform::String(L"Contact Picker Sample");
			}
		}

		static property Platform::Array<Scenario>^ scenarios
		{
			Platform::Array<Scenario>^ get()
			{
				return scenariosInner;
			}
		}

	private:
		static Platform::Array<Scenario>^ scenariosInner;
	};
}
