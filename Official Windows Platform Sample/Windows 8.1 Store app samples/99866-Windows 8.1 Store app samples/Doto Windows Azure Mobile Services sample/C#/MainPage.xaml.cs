// Copyright (c) Microsoft Corporation. All rights reserved

using Doto.Controls;
using Doto.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Doto
{
    /// <summary>
    /// The main page, where it all happens inside Doto
    /// </summary>
    public sealed partial class MainPage : Page, IFlyoutProvider
    {
        private MainViewModel viewModel;
        private SynchronizationContext synchronizationContext;

        public MainPage()
        {
            InitializeComponent();
            synchronizationContext = SynchronizationContext.Current;
            viewModel = new MainViewModel(this, synchronizationContext);
            DataContext = viewModel;
            viewModel.Initialize();

            SettingsPane.GetForCurrentView().CommandsRequested += MainPage_CommandsRequested;

            viewModel.PropertyChanged += (sender, e) =>
            {
                if (String.Equals(e.PropertyName, "ViewMode"))
                {
                    SwitchViewMode(viewModel.ViewMode);
                }
            };

            listView.SelectionChanged += (sender, e) =>
            {
                IList<object> selectedItems = (sender as ListView).SelectedItems;
                viewModel.SetSelectedItems(selectedItems.Select(i => (Item)i).ToList());
            };

            InviteUser.PlacementSource = panelUserName;
        }

        /// <summary>
        /// Called when the size of the app changes, and monitors for snapping. When the
        /// app is snapped - a number of features are removed to keep the app functional and
        /// usable
        /// </summary>
        void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ApplicationView view = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView();
            buttonAddItem.Visibility = view.IsFullScreen ? Visibility.Visible : Visibility.Collapsed;
            buttonInviteUser.Visibility = view.IsFullScreen ? Visibility.Visible : Visibility.Collapsed;
            buttonLeaveList.Visibility = view.IsFullScreen ? Visibility.Visible : Visibility.Collapsed;
            buttonNewList.Visibility = view.IsFullScreen ? Visibility.Visible : Visibility.Collapsed;
            userPanel.Visibility = view.IsFullScreen ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Invoked when the user accesses the Settings panel in the app.
        /// </summary>
        void MainPage_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var commands = viewModel.GetSettingsCommands();

            foreach (var command in commands)
            {
                args.Request.ApplicationCommands.Add(new SettingsCommand(command, command.Text, new UICommandInvokedHandler(x =>
                {
                    command.Action();
                })));
            }
        }

        /// <summary>
        /// Flips the visibility of some items in the view based on the ViewModel's state
        /// </summary>
        private void SwitchViewMode(ViewMode state)
        {
            listView.Visibility = state == ViewMode.ListSelected ? Visibility.Visible : Visibility.Collapsed;
            noListView.Visibility = state == ViewMode.NoLists ? Visibility.Visible : Visibility.Collapsed;
        }

        public async Task ShowDialogAsync(string title, string message, params SimpleCommand[] commands)
        {
            var dialog = new MessageDialog(message, title);
            foreach (var command in commands)
            {
                dialog.Commands.Add(new UICommand(command.Text, x =>
                {
                    command.Action();
                }));
            }
            await dialog.ShowAsync();
        }

        public void HideFlyout(Button attachedFlyoutButton)
        {
            FlyoutBase attachedFlyout = attachedFlyoutButton.Flyout;
            if (attachedFlyout != null)
            {
                attachedFlyout.Hide();
            }
        }

        public void HideChooseListFlyout()
        {
            HideFlyout(buttonChooseList);
        }

        public void HideNewListFlyout()
        {
            // This flyout could be attached to either the CanvasNewList button or buttoNewList
            HideFlyout(buttonNewList);
            HideFlyout(buttonCanvasNewList);
        }

        public void HideViewInvitesFlyout()
        {
            buttonPendingInvites.Flyout.Hide();
        }

        public void HideLeaveListConfirmationFlyout()
        {
            buttonLeaveList.Flyout.Hide();
        }

        public void ShowDotoFlyout(FrameworkElement frameworkElement, FlyoutPlacementMode flyoutPlacement, 
            string contentTemplate = "", UserControl flyoutControl = null)
        {
            Flyout flyout = new Flyout();
            if (flyoutControl != null)
            {
                flyout.Content = flyoutControl;
            }
            else
            {
                flyout.Content = new ContentPresenter
                {
                    Content = viewModel,
                    ContentTemplate = (DataTemplate)App.Current.Resources[contentTemplate]
                };
            }
            
            flyout.Placement = flyoutPlacement;
            if (frameworkElement is Button)
            {
                ((Button)frameworkElement).Flyout = flyout;
            }
            flyout.ShowAt(frameworkElement);
        }

        public void ShowInviteUserFlyout(InviteUserViewModel viewModel)
        {
            var control = new InviteUser();
            control.DataContext = viewModel;
            ShowDotoFlyout(buttonInviteUser, FlyoutPlacementMode.Top, String.Empty, control);
        }

        public void ShowViewInvitesFlyout(ViewInvitesViewModel viewModel)
        {
            var control = new ViewInvites();
            control.DataContext = viewModel;
            ShowDotoFlyout(buttonPendingInvites, FlyoutPlacementMode.Top, String.Empty, control);
        }

        public void ShowNewListFlyout()
        {
            ShowNewListFlyoutCommon(buttonNewList);
        }

        public void ShowNewUserNewListFlyout()
        {
            ShowNewListFlyoutCommon(buttonCanvasNewList);
        }

        public void ShowNewListFlyoutCommon(Button flyoutPlacementButton)
        {
            ShowDotoFlyout(flyoutPlacementButton, FlyoutPlacementMode.Bottom, "NewListTemplate");
        }

        public void ShowAddItemFlyout()
        {
            ShowDotoFlyout(buttonAddItem, FlyoutPlacementMode.Top, "AddItemTemplate");
        }

        public void ShowChooseListFlyout()
        {
            ShowDotoFlyout(buttonChooseList, FlyoutPlacementMode.Bottom, "ChooseListTemplate");
        }

        public void ShowLeaveListConfirmationFlyout()
        {
            ShowDotoFlyout(buttonLeaveList, FlyoutPlacementMode.Top, "ConfirmLeaveListTemplate");
        }

        public void ShowDescriptionRequiredFlyout()
        {
            ShowDotoFlyout(buttonAddItem, FlyoutPlacementMode.Bottom, "DescriptionRequiredTemplate");
        }

        public void ShowNameRequiredFlyout()
        {
            ShowDotoFlyout(buttonNewList, FlyoutPlacementMode.Bottom, "NameRequiredTemplate");
        }

        public void ShowRegisterValidationFlyout()
        {
            ShowDotoFlyout(registrationView, FlyoutPlacementMode.Left, "RegisterValidationTemplate");
        }
    }
}
