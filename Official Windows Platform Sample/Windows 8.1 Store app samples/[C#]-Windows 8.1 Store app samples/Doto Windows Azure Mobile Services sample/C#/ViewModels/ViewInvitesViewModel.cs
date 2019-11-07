// Copyright (c) Microsoft Corporation. All rights reserved

using Doto.Model;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Doto
{
    /// <summary>
    /// ViewModel that implements the logic for the View Invites dialog
    /// </summary>
    public class ViewInvitesViewModel : ViewModel
    {
        private IMobileServiceTable<Invite> invitesTable;
        private MainViewModel parent;
        private IFlyoutProvider flyoutProvider;

        public ViewInvitesViewModel(MainViewModel mainViewModelParent, IMobileServiceTable<Invite> mobileServiceInvitesTable)
        {
            flyoutProvider = mainViewModelParent.FlyoutProvider;
            parent = mainViewModelParent;
            invitesTable = mobileServiceInvitesTable;
            Invites = parent.Invites;

            AcceptCommand = new DelegateCommand<Invite>(async invite =>
            {
                invite.IsApproved = true;
                await ProcessInvite(invite);
                parent.LoadListsAsync();
            });

            RejectCommand = new DelegateCommand<Invite>(async invite =>
            {
                await ProcessInvite(invite);
            });
        }

        private async Task ProcessInvite(Invite invite)
        {
            await invitesTable.UpdateAsync(invite);
            Invites.Remove(invite);
            if (Invites.Count > 0)
            {
                parent.IsPendingInvitesButtonVisible = true;
                parent.ViewInvitesCommand.IsEnabled = true;
            }
            else
            {
                flyoutProvider.HideViewInvitesFlyout();
                parent.IsPendingInvitesButtonVisible = false;
                parent.ViewInvitesCommand.IsEnabled = false;
            }
        }

        public ObservableCollection<Invite> Invites { get; private set; }

        public DelegateCommand<Invite> AcceptCommand { get; private set; }
        public DelegateCommand<Invite> RejectCommand { get; private set; }
    }
}
