// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace WalletQuickstart
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "Wallet Quickstart";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title="launch Wallet", ClassType=typeof(Scenario1)},
            new Scenario() { Title="add an item", ClassType=typeof(Scenario2)},
            new Scenario() { Title="update an item", ClassType=typeof(Scenario3)},
            new Scenario() { Title="add a transaction", ClassType=typeof(Scenario4)},
            new Scenario() { Title="add a custom verb", ClassType=typeof(Scenario5)},
            new Scenario() { Title="set item relevant date and location", ClassType=typeof(Scenario6)},
            new Scenario() { Title="delete an item", ClassType=typeof(Scenario7)},
            new Scenario() { Title="import a wallet item package", ClassType=typeof(Scenario8)}
            
        };
    }

    public class Scenario
    {
        public string Title { get; set; }

        public Type ClassType { get; set; }
    }
}
