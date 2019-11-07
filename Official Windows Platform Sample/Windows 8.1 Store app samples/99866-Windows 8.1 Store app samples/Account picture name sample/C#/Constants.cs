//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using AccountPictureName;
using System;
using System.Collections.Generic;
using Windows.UI.ViewManagement;

namespace SDKTemplate
{
    public partial class MainPage : SDKTemplate.Common.LayoutAwarePage
    {

        public const string FEATURE_NAME = "Account Picture Name C#";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Title = "Get user's display name", ClassType = typeof(GetUserDisplayName) },
            new Scenario() { Title = "Get user's first and last name", ClassType = typeof(GetUserFirstLastName) },
            new Scenario() { Title = "Get user's Account Picture", ClassType = typeof(GetAccountPicture) },
            new Scenario() { Title = "Set user's Account Picture and Listen for changes", ClassType = typeof(SetAccountPictureAndListen) }
        };


        // Navigates to the Scenario "Set Account Picture and listen"
        public void NavigateToSetAccountPictureAndListen()
        {
            int index = -1;
            // Populate the ListBox with the list of scenarios as defined in Constants.cs.
            foreach (Scenario s in scenarios)
            {
                index++;
                if (s.ClassType == typeof(SetAccountPictureAndListen))
                {
                    break;
                }
            }
            SuspensionManager.SessionState["SelectedScenario"] = index;
            Scenarios.SelectedIndex = index;
            LoadScenario(scenarios[index].ClassType);
            InvalidateSize();
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
}
