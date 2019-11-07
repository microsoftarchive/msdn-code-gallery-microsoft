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
// Scenario4.xaml.h
// Declaration of the Scenario4 class
//

#pragma once

#include "pch.h"
#include "Scenario4.g.h"
#include "MainPage.xaml.h"

namespace SDKSample
{
	namespace Accessibility
	{
		using namespace Windows::UI::Xaml::Data;
        using namespace Windows::UI::Xaml;
        using namespace Windows::UI::Xaml::Automation;

		/// <summary>
		/// An empty page that can be used on its own or navigated to within a Frame.
		/// </summary>
	
		[Windows::Foundation::Metadata::WebHostHiddenAttribute]
		public ref class MyList sealed : Windows::UI::Xaml::Controls::ListView
		{
			protected:
				virtual void PrepareContainerForItemOverride(Windows::UI::Xaml::DependencyObject^ element, Platform::Object^ item) override
				{
					Windows::UI::Xaml::Controls::ListView::PrepareContainerForItemOverride(element, item);
                    FrameworkElement^ source = safe_cast<FrameworkElement^>(element);
                    Binding^ idBinding = ref new Binding;
                    Binding^ nameBinding = ref new Binding;
                    idBinding->Path = ref new PropertyPath("AutomationId");
                    source->SetBinding(AutomationProperties::AutomationIdProperty, idBinding);
                    nameBinding->Path = ref new PropertyPath("AutomationName");
                    source->SetBinding(AutomationProperties::NameProperty, nameBinding);
				}
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class Person sealed 
		{
			private:
				Platform::String^ firstName;
				Platform::String^ lastName;
				Platform::String^ automationName;
				Platform::String^ automationId;
				int _age;

			public:
				Person()
				{
				}
				Person(Platform::String^ firstName, Platform::String^ lastName, Platform::String^ id, int age, Platform::String^ name)
				{
					FirstName = firstName;
					LastName = lastName;
					AutomationId = id;
					Age = age;
					AutomationName = name;
				}

	
				property Platform::String^ FirstName
				{
					Platform::String^ get()
					{
						return firstName;
					}
					void set(Platform::String^ value)
					{
						firstName = value;
					}
				}

				property Platform::String^ LastName
				{
					Platform::String^ get()
					{
						return lastName;
					}
					void set(Platform::String^ value)
					{
						lastName = value;
					}
				}

				property int Age
				{
					int get()
					{
						return _age;
					}
					void set(int value)
					{
						_age = value;
					}
				}

				property Platform::String^ AutomationName
				{
					Platform::String^ get()
					{
						return automationName;
					}
					void set(Platform::String^ value)
					{
						automationName = value;
					}
				}

				property Platform::String^ AutomationId
				{
					Platform::String^ get()
					{
						return automationId;
					}
					void set(Platform::String^ value)
					{
						automationId = value;
					}
				}
		};

		public ref class DataHelper sealed
		{
			public:	
				static Windows::Foundation::Collections::IVector<Person^>^ GeneratePersonNamesSource()
				{
					Platform::Collections::Vector<Person^>^ ds = ref new Platform::Collections::Vector<Person^>();
					ds->Append(ref new Person("George", "Washington", "ListItemId1", 25, "ListItemName1"));
					ds->Append(ref new Person("John", "Adams", "ListItemId2", 30, "ListItemName2"));
					ds->Append(ref new Person("Thomas", "Jefferson", "ListItemId3", 45, "ListItemName3"));
					ds->Append(ref new Person("James", "Madison", "ListItemId4", 55, "ListItemName4"));
					ds->Append(ref new Person("James", "Monroe", "ListItemId5", 30, "ListItemName5"));
					ds->Append(ref new Person("John", "Adams", "ListItemId6", 25, "ListItemName6"));
					ds->Append(ref new Person("Andrew", "Jackson", "ListItemId7", 55, "ListItemName7"));
					ds->Append(ref new Person("Martin", "Van Buren", "ListItemId8", 56, "ListItemName8"));
					ds->Append(ref new Person("William", "Harrison", "ListItemId9", 40, "ListItemName9"));
					ds->Append(ref new Person("John", "Tyler", "ListItemId10", 42, "ListItemName10"));
					ds->Append(ref new Person("James", "Polk", "ListItemId11", 60, "ListItemName11"));
					ds->Append(ref new Person("Zachary", "Taylor", "ListItemId12", 65, "ListItemName12"));
					ds->Append(ref new Person("Millard", "Fillmore", "ListItemId13", 25, "ListItemName13"));
					ds->Append(ref new Person("Franklin", "Pierce", "ListItemId14", 35, "ListItemName14"));
					ds->Append(ref new Person("James", "Buchanan", "ListItemId15", 43, "ListItemName15"));
					ds->Append(ref new Person("Abraham", "Lincoln", "ListItemId16", 23, "ListItemName16"));
					ds->Append(ref new Person("Andrew", "Johnson", "ListItemId17", 21, "ListItemName17"));
					ds->Append(ref new Person("Rutherford", "Hayes", "ListItemId18", 25, "ListItemName18"));
					ds->Append(ref new Person("James", "Garfield", "ListItemId19", 30, "ListItemName19"));
					ds->Append(ref new Person("Chester", "Arthur", "ListItemId20", 34, "ListItemName20"));
					ds->Append(ref new Person("Grover", "Cleveland", "ListItemId21", 55, "ListItemName21"));

					return ds;
				}
		};

		[Windows::Foundation::Metadata::WebHostHiddenAttribute]
		public ref class Scenario4 sealed
		{
		public:
			Scenario4();

		protected:
			virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
		private:
			MainPage^ rootPage;
		};
	}
}
