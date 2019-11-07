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
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Contacts.Provider;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "Contact Picker Sample";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Pick a single contact",  ClassType = typeof(ContactPicker.ScenarioSingle) },
            new Scenario() { Title = "Pick multiple contacts", ClassType = typeof(ContactPicker.ScenarioMultiple) },
        };

        internal bool EnsureUnsnapped()
        {
            // The ContactPicker APIs will not work if the application is in the snapped state. If
            // wants to show the ContactPicker while snapped, it must attempt to unsnap first.
            bool unsnapped = (ApplicationView.Value != ApplicationViewState.Snapped) || ApplicationView.TryUnsnap();
            if (!unsnapped)
            {
                NotifyUser("Cannot unsnap the sample.", NotifyType.StatusMessage);
            }
            return unsnapped;
        }
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

    public partial class App : Application
    {
        protected override void OnActivated(Windows.ApplicationModel.Activation.IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.ContactPicker)
            {
                var page = new MainPagePicker();
                page.Activate((ContactPickerActivatedEventArgs)args);
            }
            else
            {
                base.OnActivated(args);
            }
        }
    }

    public partial class MainPagePicker : SDKTemplate.Common.LayoutAwarePage
    {
        public const string FEATURE_NAME = "Contact Picker Sample";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Select contact(s)", ClassType = typeof(ContactPicker.ContactPickerPage) },
        };

        internal ContactPickerUI contactPickerUI = null;

        public void Activate(ContactPickerActivatedEventArgs args)
        {
            // cache ContactPickerUI
            contactPickerUI = args.ContactPickerUI;
            Window.Current.Content = this;
            this.OnNavigatedTo(null);
            Window.Current.Activate();
        }
    }
}
