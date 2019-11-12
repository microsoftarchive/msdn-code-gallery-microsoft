//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBinding;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace DataBinding
{
    // This Data Source is used in Scenarios 4, 5 and 6.
    public class Team //Has a custom string indexer
    {
        Dictionary<string, object> _propBag;
        public Team()
        {
            _propBag = new Dictionary<string, object>();
        }

        public string Name { get; set; }
        public string City { get; set; }
        public SolidColorBrush Color { get; set; }

        // this is how you can create a custom indexer in c#
        public object this[string indexer]
        {
            get
            {
                return _propBag[indexer];
            }
            set
            {
                _propBag[indexer] = value;
            }
        }

        public void Insert(string key, object value)
        {
            _propBag.Add(key, value);
        }

    }

    // This class is used to demonstrate grouping.
    public class Teams : List<Team>
    {
        public Teams()
        {
            Add(new Team() { Name = "The Reds", City = "Liverpool", Color = new SolidColorBrush(Colors.Green) });
            Add(new Team() { Name = "The Red Devils", City = "Manchester", Color = new SolidColorBrush(Colors.Yellow) });
            Add(new Team() { Name = "The Blues", City = "London", Color = new SolidColorBrush(Colors.Orange) });
            Team _t = new Team() { Name = "The Gunners", City = "London", Color = new SolidColorBrush(Colors.Red) };
            _t["Gaffer"] = "le Professeur";
            _t["Skipper"] = "Mr Gooner";

            Add(_t);
        }

    }
}

