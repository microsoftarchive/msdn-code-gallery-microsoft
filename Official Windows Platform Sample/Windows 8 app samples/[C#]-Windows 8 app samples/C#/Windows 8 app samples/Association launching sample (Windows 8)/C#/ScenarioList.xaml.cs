// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AssociationLaunching;

namespace AssociationLaunching
{
    public sealed partial class ScenarioList : Page
    {
        // A pointer back to the main page which is used to gain access to the input and output frames and their content.
        rootPage rootPage = null;

        public ScenarioList()
        {
            InitializeComponent();
        }

        #region Template-Related Code - Do not remove
        void Scenarios_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Scenarios.SelectedItem != null)
            {
                rootPage.NotifyUser("", NotifyType.StatusMessage);

                ListBoxItem selectedListBoxItem = Scenarios.SelectedItem as ListBoxItem;
                SuspensionManager.SessionState["SelectedScenario"] = selectedListBoxItem.Name;

                if (rootPage.InputFrame != null && rootPage.OutputFrame != null)
                {
                    // Load the input and output pages for the current scenario into their respective frames.

                    rootPage.DoNavigation(Type.GetType(typeof(ScenarioList).Namespace + "." + selectedListBoxItem.Name + "Input"), rootPage.InputFrame);
                    rootPage.DoNavigation(Type.GetType(typeof(ScenarioList).Namespace + "." + selectedListBoxItem.Name + "Output"), rootPage.OutputFrame);
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = e.Parameter as rootPage;
            Scenarios.SelectionChanged += new SelectionChangedEventHandler(Scenarios_SelectionChanged);

            // Starting scenario is the first or based upon a previous selection.
            ListBoxItem startingScenario = null;

            if (rootPage.FileEvent != null)
            {
                // Set the file launch scenario.
                startingScenario = this.FindName("ReceiveFile") as ListBoxItem;
            }
            else if (rootPage.ProtocolEvent != null)
            {
                // Set the protocol launch scenario.
                startingScenario = this.FindName("ReceiveUri") as ListBoxItem;
            }
            else if (SuspensionManager.SessionState.ContainsKey("SelectedScenario"))
            {
                String selectedScenarioName = SuspensionManager.SessionState["SelectedScenario"] as string;
                startingScenario = this.FindName(selectedScenarioName) as ListBoxItem;
            }

            Scenarios.SelectedItem = startingScenario != null ? startingScenario : LaunchFile;
        }
        #endregion
    }
}