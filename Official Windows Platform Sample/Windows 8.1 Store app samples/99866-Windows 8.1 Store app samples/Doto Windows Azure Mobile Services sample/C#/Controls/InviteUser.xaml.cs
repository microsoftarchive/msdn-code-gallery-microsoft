// Copyright (c) Microsoft Corporation. All rights reserved

using Doto.Model;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Doto.Controls
{
    public sealed partial class InviteUser : UserControl
    {
        private static FrameworkElement placementSource;

        public InviteUser()
        {
            this.InitializeComponent();
        }

        public static FrameworkElement PlacementSource
        {
            get { return placementSource; }
            set { placementSource = value; }
        }

        private void UsersList_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var source = e.OriginalSource as FrameworkElement;
            if (source == null) return; 
            
            Profile user = source.DataContext as Profile;
            if (user != null)
            {
                // Tapped an item
                ((InviteUserViewModel)DataContext).InviteUserAsync(user);
                ShowInviteSentConfirmationFlyout();
            }
        }

        public void ShowInviteSentConfirmationFlyout()
        {
            Flyout flyout = new Flyout(); 
            flyout.Content = new ContentPresenter
            {
                ContentTemplate = (DataTemplate)App.Current.Resources["InviteConfirmationTemplate"]
            };
            flyout.Placement = FlyoutPlacementMode.Bottom;
            flyout.ShowAt(PlacementSource);
        }
    }
}
