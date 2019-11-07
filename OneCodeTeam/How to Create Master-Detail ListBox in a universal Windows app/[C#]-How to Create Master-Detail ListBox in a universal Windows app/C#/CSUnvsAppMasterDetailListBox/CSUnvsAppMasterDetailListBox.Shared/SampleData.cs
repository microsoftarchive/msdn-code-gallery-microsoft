/****************************** Module Header ******************************\
 * Module Name:  SampleData.cs
 * Project:      CSUnvsAppMasterDetailListBox.Shared
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
using System;
using System.Collections.Generic;
using System.Text;

namespace CSUnvsAppMasterDetailListBox.SampleData
{
    public class City
    {
        public string Name { get; set; }
    }

    public class Province
    {
        public string Name { get; set; }
        public List<City> Cities { get; set; }

        public Province()
        {
            Cities = new List<City>();
        }
    }

    public class Country
    {
        public string Name { get; set; }
        public List<Province> Provinces { get; set; }

        public Country()
        {
            Provinces = new List<Province>();
        }
    }

    public class Data : List<Country>
    {
        public Data()
        {
            Province provinceOne = new Province();
            provinceOne.Name = "Province1";
            Province ProvinceTwo = new Province();
            ProvinceTwo.Name = "Province2";
            Province ProvinceThree = new Province();
            ProvinceThree.Name = "Province3";
            Province ProvinceFour = new Province();
            ProvinceFour.Name = "Province4";

            for (int i = 1; i < 4; i++)
            {
                City city = new City();
                city.Name = provinceOne.Name + " City" + i;
                provinceOne.Cities.Add(city);
            }

            for (int i = 1; i < 4; i++)
            {
                City city = new City();
                city.Name = ProvinceTwo.Name + " City" + i;
                ProvinceTwo.Cities.Add(city);
            }

            for (int i = 1; i < 4; i++)
            {
                City city = new City();
                city.Name = ProvinceThree.Name + " City" + i;
                ProvinceThree.Cities.Add(city);
            }

            for (int i = 1; i < 4; i++)
            {
                City city = new City();
                city.Name = ProvinceFour.Name + " City" + i;
                ProvinceFour.Cities.Add(city);
            }



            Country CountryOne = new Country();
            CountryOne.Name = "Country1";
            CountryOne.Provinces.Add(provinceOne);
            CountryOne.Provinces.Add(ProvinceTwo);
            Country CountryTwo = new Country();
            CountryTwo.Name = "Country2";
            CountryTwo.Provinces.Add(ProvinceThree);
            CountryTwo.Provinces.Add(ProvinceFour);


            this.Add(CountryOne);
            this.Add(CountryTwo);
        }
    }
}
