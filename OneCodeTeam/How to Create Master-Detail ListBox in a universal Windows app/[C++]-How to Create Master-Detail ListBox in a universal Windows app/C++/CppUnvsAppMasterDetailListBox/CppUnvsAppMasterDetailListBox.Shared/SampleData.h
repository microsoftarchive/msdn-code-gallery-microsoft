/****************************** Module Header ******************************\
* Module Name:  SampleData.h
* Project:      CppUnvsAppMasterDetailListBox.Shared
* Copyright (c) Microsoft Corporation.
*
* This sample demonstrates how to create master detail ListBox in Universal app
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
#include <pch.h>
namespace CppUnvsAppMasterDetailListBox
{
	namespace SampleData
	{
		[Windows::UI::Xaml::Data::Bindable]
		public ref class City sealed
		{
		public:
			property Platform::String^ Name;
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class Province sealed
		{
		public:

			Province()
			{
				Cities = ref new Platform::Collections::Vector<City^>();
			}

			property Platform::String^ Name;
			property Windows::Foundation::Collections::IVector<City^>^ Cities;
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class Country sealed
		{
		public:

			Country()
			{
				Provinces = ref new Platform::Collections::Vector<Province^>();
			}

			property Platform::String^ Name;
			property Windows::Foundation::Collections::IVector<Province^>^ Provinces;
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class Data sealed
		{
		public:
			Data()
			{
				Province^ provinceOne = ref new Province();
				provinceOne->Name = "Province1";

				Province^ provinceTwo = ref new Province();
				provinceTwo->Name = "Province2";

				Province^ provinceThree = ref new Province();
				provinceThree->Name = "Province3";

				Province^ provinceFour = ref new Province();
				provinceFour->Name = "Province4";

				for (int i = 1; i<4; i++)
				{
					City^ city = ref new City();
					city->Name = provinceOne->Name + "City" + i;
					provinceOne->Cities->Append(city);
				}

				for (int i = 1; i<4; i++)
				{
					City^ city = ref new City();
					city->Name = provinceTwo->Name + "City" + i;
					provinceTwo->Cities->Append(city);
				}


				for (int i = 1; i<4; i++)
				{
					City^ city = ref new City();
					city->Name = provinceThree->Name + "City" + i;
					provinceThree->Cities->Append(city);
				}

				for (int i = 1; i<4; i++)
				{
					City^ city = ref new City();
					city->Name = provinceFour->Name + "City" + i;
					provinceFour->Cities->Append(city);
				}

				Country^ countryOne = ref new Country();
				countryOne->Name = "Country1";
				countryOne->Provinces->Append(provinceOne);
				countryOne->Provinces->Append(provinceTwo);

				Country^ countryTwo = ref new Country();
				countryTwo->Name = "Country2";
				countryTwo->Provinces->Append(provinceThree);
				countryTwo->Provinces->Append(provinceFour);

				Countries = ref new Platform::Collections::Vector<Country^>();

				Countries->Append(countryOne);
				Countries->Append(countryTwo);
			}
			property Windows::Foundation::Collections::IVector<Country^>^ Countries;
		};
	}
}
