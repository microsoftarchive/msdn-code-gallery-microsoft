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
    public sealed partial class GlobalPage : Page
    {
        MainPage rootPage = null;

        public GlobalPage()
        {
            this.InitializeComponent();
            Back.Click += Back_Click;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = e.Parameter as MainPage;

            Frame1.Navigate(typeof(Page1), this);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (Frame1.CanGoBack)
            {
                Frame1.GoBack();
            }
            else
            {
                rootPage.Frame.GoBack();
            }
        }
    }
}
