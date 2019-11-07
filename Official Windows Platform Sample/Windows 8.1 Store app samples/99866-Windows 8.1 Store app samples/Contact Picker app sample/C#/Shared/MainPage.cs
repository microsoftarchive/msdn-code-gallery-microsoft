// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace SDKTemplate
{
    /// <summary>
    /// MainPage feature name and scenarios.
    /// </summary>
    public partial class MainPage : Page
    {
        /// <summary>
        /// Feature name.
        /// </summary>
        public const string FEATURE_NAME = "Contact Picker Sample";

        /// <summary>
        /// List of scenarios.
        /// </summary>
        private List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Pick a single contact",  ClassType = typeof(ContactPicker.Scenario1_PickContact) },
            new Scenario() { Title = "Pick multiple contacts", ClassType = typeof(ContactPicker.Scenario2_PickContacts) },
        };
    }
}
