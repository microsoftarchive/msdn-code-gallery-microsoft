using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public sealed partial class Page1 : Page
    {
        GlobalPage rootPage = null;
        public Page1()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = e.Parameter as GlobalPage;
        }

        private void PageTwo_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Page2), rootPage);
        }
    }
}
