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
    public sealed partial class Page2 : Page
    {
        GlobalPage rootPage = null;
        Button starButton = null;
        StackPanel rightPanel = null;

        public Page2()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            rootPage = e.Parameter as GlobalPage;

            // While our AppBar has global commands for navigation, we can demonstrate
            // how we can add contextual AppBar command buttons that only pertain to 
            // particular pages.  

            // In this case, whenever we navigate to this page, we want to add a new command button
            // to our AppBar

            // We want to add command buttons to the right side StackPanel within the AppBar.
            rightPanel = rootPage.FindName("RightCommands") as StackPanel;
            if (rightPanel != null)
            {
                // Create the button to add
                starButton = new Button();

                // Hook up the custom button style so that it looks like an AppBar button
                starButton.Style = App.Current.Resources.MergedDictionaries[0]["StarAppBarButtonStyle"] as Style;

                // Set up the Click handler for the new button
                starButton.Click += new RoutedEventHandler(starButton_Click);

                // Add the button to the AppBar
                rightPanel.Children.Add(starButton);
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (rightPanel != null)
            {
                // Unhook the Click event handler for the button
                starButton.Click -= new RoutedEventHandler(starButton_Click);

                // Remove the button from the AppBar
                rightPanel.Children.Remove(starButton);
            }
        }

        // This is the handler for our ApplicationBar button that is only available on this page
        async void starButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("You're a Superstar!");
            await dialog.ShowAsync();
        }

    }
}
