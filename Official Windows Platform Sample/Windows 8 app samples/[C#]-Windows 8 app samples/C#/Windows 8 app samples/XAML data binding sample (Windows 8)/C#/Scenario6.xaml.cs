//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using System.Linq;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace DataBinding
{
    public sealed partial class Scenario6 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        public Scenario6()
        {
            this.InitializeComponent();
            Scenario6Reset(null, null);
        }

        private void Scenario6Reset(object sender, RoutedEventArgs e)
        {
            Teams teams = new Teams();
            var result = from t in teams
                         group t by t.City into g
                         orderby g.Key
                         select new { Key = g.Key, Items = g }; ;
            
            groupInfoCVS.Source = result;
        }
    }
}
