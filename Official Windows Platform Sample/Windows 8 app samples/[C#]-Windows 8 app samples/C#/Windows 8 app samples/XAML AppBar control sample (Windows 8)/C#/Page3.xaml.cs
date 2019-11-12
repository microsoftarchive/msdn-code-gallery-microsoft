using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SDKTemplate;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace AppBarControl
{
    public sealed partial class Page3 : Page
    {
        MainPage rootPage = null;

        public Page3()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = e.Parameter as MainPage;
            BottomAppBar.IsOpen = true;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            rootPage.Frame.GoBack();
        }

        private void RemoveSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Save != null)
            {
                LeftPanel.Children.Remove(Save);
            }
        }

        private void AddFavoriteButton_Click(object sender, RoutedEventArgs e)
        {
            Button favButton = new Button();
            favButton.Style = App.Current.Resources["FavoriteAppBarButtonStyle"] as Style;
            LeftPanel.Children.Insert(2, favButton);
        }
    }
}
