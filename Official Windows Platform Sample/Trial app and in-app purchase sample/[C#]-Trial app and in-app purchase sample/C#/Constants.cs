//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System.Collections.Generic;
using System;
using Store;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        // Change the string below to reflect the name of your sample.
        // This is used on the main page as the title of the sample.
        public const string FEATURE_NAME = "In-app purchase and trial management sample";

        // Change the array below to reflect the name of your scenarios.
        // This will be used to populate the list of scenarios on the main page with
        // which the user will choose the specific scenario that they are interested in.
        // These should be in the form: "Navigating to a web page".
        // The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Trial-mode", ClassType = typeof(TrialMode) },
            new Scenario() { Title = "In-app purchase", ClassType = typeof(InAppPurchase) },
            new Scenario() { Title = "Expiring product", ClassType = typeof(ExpiringProduct) },
            new Scenario() { Title = "Consumable product", ClassType = typeof(InAppPurchaseConsumables) },
            new Scenario() { Title = "Advanced consumable product", ClassType = typeof(InAppPurchaseConsumablesAdvanced) },
            new Scenario() { Title = "Large catalog product", ClassType = typeof(InAppPurchaseLargeCatalog) },
            new Scenario() { Title = "App Listing URI", ClassType = typeof(AppListingUri) },
            new Scenario() { Title = "Receipt", ClassType = typeof(Receipt) }
        };
    }

    public class Scenario
    {
        public string Title { get; set; }

        public Type ClassType { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
