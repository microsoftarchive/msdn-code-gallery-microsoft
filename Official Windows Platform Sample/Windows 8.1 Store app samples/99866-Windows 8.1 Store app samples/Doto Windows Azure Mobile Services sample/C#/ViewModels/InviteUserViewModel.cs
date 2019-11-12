// Copyright (c) Microsoft Corporation. All rights reserved

using Doto.Model;
using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace Doto
{
    /// <summary>
    /// ViewModel that implements the logic behind the Invite Users dialog
    /// </summary>
    public class InviteUserViewModel : ViewModel
    {
        private int profileCountPerPage = 3;
        private long totalCount = 0;
        private int currentPage = 0;
        private IMobileServiceTableQuery<Profile> query;
        private IMobileServiceTable<Profile> profileTable;
        private MainViewModel parent;

        public InviteUserViewModel(MainViewModel mainViewModelParent, IMobileServiceTable<Profile> mobileServiceProfileTable)
        {
            profileTable = mobileServiceProfileTable;
            parent = mainViewModelParent;

            SearchCommand = new DelegateCommand(() =>
            {
                currentPage = 0;
                if (String.IsNullOrWhiteSpace(SearchText))
                {
                    query = profileTable.Take(profileCountPerPage).IncludeTotalCount();
                }
                else
                {
                    query = profileTable.Take(profileCountPerPage).IncludeTotalCount().Where
                        (p => p.Name.IndexOf(SearchText) >= 0);
                }
                RefreshQueryAsync();
            });

            NextCommand = new DelegateCommand(() =>
            {
                currentPage++;
                RefreshQueryAsync();
            }, false);

            PrevCommand = new DelegateCommand(() =>
            {
                currentPage--;
                RefreshQueryAsync();
            }, false);
        }

        private async void RefreshQueryAsync()
        {
            // Temporarily disable the next/prev commands until this query finishes
            NextCommand.IsEnabled = false;
            PrevCommand.IsEnabled = false;
            List<Profile> profiles = await query.Skip(currentPage * profileCountPerPage).ToListAsync();
            totalCount = ((ITotalCountProvider)profiles).TotalCount;
            Users.Clear();
            Users.AddRange(profiles);
            EnableDisablePaging();
        }

        private void EnableDisablePaging()
        {
            NextCommand.IsEnabled = ((currentPage + 1) * profileCountPerPage) < totalCount;
            PrevCommand.IsEnabled = currentPage > 0;
            ShowPageControls = NextCommand.IsEnabled || PrevCommand.IsEnabled;
        }

        public async void InviteUserAsync(Profile profile)
        {
            try
            {
                await parent.InviteUserAsync(profile);
            }
            catch (MobileServiceInvalidOperationException exc)
            {
                // The server validates invitations, handles expected responses
                // and displays a friendly message to the user
                if (exc.Response.StatusCode == HttpStatusCode.BadRequest)
                {
                    parent.ShowError(exc.Message);
                }
                else
                {
                    parent.ShowError("An unexpected error occured. Please try again later.");
                }
            }
        }

        private string searchText;

        public string SearchText
        {
            get { return searchText; }
            set { SetValue(ref searchText, value); }
        }

        private readonly ObservableCollection<Profile> users = new ObservableCollection<Profile>();

        public ObservableCollection<Profile> Users
        { 
            get { return users; }
        }

        private bool showPageControls;

        public bool ShowPageControls
        {
            get { return showPageControls; }
            set { SetValue(ref showPageControls, value); }
        }

        public DelegateCommand SearchCommand { get; private set; }
        public DelegateCommand NextCommand { get; private set; }
        public DelegateCommand PrevCommand { get; private set; }
    }
}
