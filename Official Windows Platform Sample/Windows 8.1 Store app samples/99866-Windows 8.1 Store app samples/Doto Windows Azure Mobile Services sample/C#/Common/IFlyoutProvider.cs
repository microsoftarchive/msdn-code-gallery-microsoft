// Copyright (c) Microsoft Corporation. All rights reserved

using System.Threading.Tasks;

namespace Doto
{
    /// <summary>
    /// Since the ViewModels frequently need to interact with the View programmatically
    /// (not through binding) to display flyouts, we use this interface
    /// to abstract the ViewModel from the specific implementation in the View
    /// </summary>
    public interface IFlyoutProvider
    {
        Task ShowDialogAsync(string title, string message, params SimpleCommand[] commands);

        void ShowInviteUserFlyout(InviteUserViewModel viewModel);
        void ShowViewInvitesFlyout(ViewInvitesViewModel viewInvitesViewModel);
        void ShowAddItemFlyout();
        void ShowNewListFlyout();
        void ShowNewUserNewListFlyout();
        void ShowChooseListFlyout();
        void ShowLeaveListConfirmationFlyout();
        void ShowRegisterValidationFlyout();
        void ShowDescriptionRequiredFlyout();
        void ShowNameRequiredFlyout();
        void HideNewListFlyout();
        void HideViewInvitesFlyout();
        void HideChooseListFlyout();
        void HideLeaveListConfirmationFlyout();
    }
}
