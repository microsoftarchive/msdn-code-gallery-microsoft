/****************************** Module Header ******************************\
* Module Name: PersonService.cs
* Project:     CSASPNETWebGrid
* Copyright (c) Microsoft Corporation.
* 
* This sample will show how to add checkbox to MVC web grid.
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
using CSASPNETWebGrid.Models;

namespace CSASPNETWebGrid
{
    public class PersonService
    {
        public IList<Person> GetPersons()
        {
            return _persons;
        }
        static IList<Person> _persons = new List<Person>();
        static PersonService()
        {
            Random ran=new Random(100);
            for (int i = 5000; i < 5020; i++)
                _persons.Add(new Person { ID = i, Name = "Person" + i, Address = "Street, " + i, Email = "a" + i + "@microsoft.com" });
        }

    }
}