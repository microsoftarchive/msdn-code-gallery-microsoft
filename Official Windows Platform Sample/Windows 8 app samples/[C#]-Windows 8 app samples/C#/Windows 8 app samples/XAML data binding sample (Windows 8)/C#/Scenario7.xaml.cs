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
using System.Collections.ObjectModel;


namespace DataBinding
{
    public sealed partial class Scenario7 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;
        private ObservableCollection<Team> ocTeams;

        public Scenario7()
        {
            this.InitializeComponent();

            btnRemoveTeam.Click += BtnRemoveTeam_Click;
            Scenario7Reset(null, null);
        }

        private void Scenario7Reset(object sender, RoutedEventArgs e)
        {
            if(ocTeams!=null)
                ocTeams.CollectionChanged -= _ocTeams_CollectionChanged;
            
            ocTeams = new ObservableCollection<Team>(new Teams());
            ocTeams.CollectionChanged += _ocTeams_CollectionChanged;
                        
            teamsCVS.Source = ocTeams;
            
            tbCollectionChangeStatus.Text = String.Empty;
        }

        void BtnRemoveTeam_Click(object sender, RoutedEventArgs e)
        {
            if (ocTeams.Count > 0)
            {
                int index=0;
                if (lvTeams.SelectedItem != null)
                    index = lvTeams.SelectedIndex;
                ocTeams.RemoveAt(index);
            }
        }

        void _ocTeams_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            tbCollectionChangeStatus.Text = String.Format("Collection was changed. Count = {0}", ocTeams.Count);
        }
    }
}
