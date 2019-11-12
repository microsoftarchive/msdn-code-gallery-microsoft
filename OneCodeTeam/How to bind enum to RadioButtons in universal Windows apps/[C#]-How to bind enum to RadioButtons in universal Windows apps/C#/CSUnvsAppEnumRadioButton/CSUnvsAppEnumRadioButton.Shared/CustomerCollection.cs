/****************************** Module Header ******************************\
 * Module Name:  CustomerCollection.cs
 * Project:      CSUnvsAppEnumRadioButton
 * Copyright (c) Microsoft Corporation.
 * 
 * This is the demo data
 *  
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Collections.ObjectModel;

namespace CSUnvsAppEnumRadioButton
{
    public class CustomerCollection
    {
        public static ObservableCollection<Customer> Customers = new ObservableCollection<Customer>();
        static CustomerCollection()
        {            
            Customers.Add(new Customer() { Name = "Allen",   Age = 25, Sex = true,  FavouriteSport =   Sport.Basketball });
            Customers.Add(new Customer() { Name = "Carter",  Age = 26, Sex = true,  FavouriteSport =  Sport.Basketball });
            Customers.Add(new Customer() { Name = "Rose",    Age = 30, Sex = true,  FavouriteSport =    Sport.Swimming   });
            Customers.Add(new Customer() { Name = "Dove",    Age = 33, Sex = true,  FavouriteSport =    Sport.Football   });
            Customers.Add(new Customer() { Name = "Mary",    Age = 30, Sex = false, FavouriteSport =   Sport.Swimming   });
            Customers.Add(new Customer() { Name = "William", Age = 42, Sex = true,  FavouriteSport = Sport.Basketball });
            Customers.Add(new Customer() { Name = "Daisy",   Age = 16, Sex = false, FavouriteSport =  Sport.Swimming   });
            Customers.Add(new Customer() { Name = "Elena",   Age = 17, Sex = false, FavouriteSport =  Sport.Football   });
            Customers.Add(new Customer() { Name = "Tracy",   Age = 35, Sex = false, FavouriteSport =  Sport.Basketball });
            Customers.Add(new Customer() { Name = "Alex",    Age = 23, Sex = true,  FavouriteSport =    Sport.Basketball });
            Customers.Add(new Customer() { Name = "Mike",    Age = 50, Sex = true,  FavouriteSport =    Sport.Football   });
            Customers.Add(new Customer() { Name = "Lisa",    Age = 23, Sex = false, FavouriteSport =   Sport.Basketball });
            Customers.Add(new Customer() { Name = "Andrew",  Age = 19, Sex = true,  FavouriteSport =  Sport.Football   });
            Customers.Add(new Customer() { Name = "Steve",   Age = 39, Sex = true,  FavouriteSport =   Sport.Swimming   });
            Customers.Add(new Customer() { Name = "Jim",     Age = 14, Sex = true,  FavouriteSport =     Sport.Basketball });
        }
    }
}
