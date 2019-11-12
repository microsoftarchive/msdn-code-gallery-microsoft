using SDKTemplate;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace AppBarControl
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GridViewPage : Page
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // This collection will contain our flavors of ice cream
        private ObservableCollection<IceCream> Flavors;

        private Random random = null;

        public GridViewPage()
        {
            this.InitializeComponent();

            Flavors = new ObservableCollection<IceCream>();
            random = new Random();
            IceCreamList.SelectionChanged += IceCreamList_SelectionChanged;
        }

        void IceCreamList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridView gv = sender as GridView;
            if (gv != null)
            {
                if (gv.SelectedItem != null)
                {
                    // We have selected items so show the AppBar and make it sticky
                    BottomAppBar.IsSticky = true;
                    BottomAppBar.IsOpen = true;
                }
                else
                {
                    // No selections so hide the AppBar and don't make it sticky any longer
                    BottomAppBar.IsSticky = false;
                    BottomAppBar.IsOpen = false;
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Populate our collection of ice cream flavors
            for (int i = 0; i < 50; i++)
            {
                Flavors.Add(GenerateItem());
            }

            IceCreamList.ItemsSource = Flavors;
        }

        /// <summary>
        /// This method will generate random ice cream flavors
        /// </summary>
        /// <returns></returns>
        private IceCream GenerateItem()
        {
            
            int type = (int)Math.Floor((double)random.Next(1, 6));

            switch (type)
            {
                case 1:
                    return new IceCream { Name = "Banana Blast", Type = "Low-fat Frozen Yogurt", Image = "Assets/60Banana.png" };
                case 2:
                    return new IceCream { Name = "Lavish Lemon Ice", Type = "Sorbet", Image = "Assets/60Lemon.png" };
                case 3:
                    return new IceCream { Name = "Marvelous Mint", Type = "Gelato", Image = "Assets/60Mint.png" };
                case 4:
                    return new IceCream { Name = "Creamy Orange", Type = "Sorbet", Image = "Assets/60Orange.png" };
                case 5:
                    return new IceCream { Name = "Very Vanilla", Type = "Ice Cream", Image = "Assets/60Vanilla.png" };
                default:
                    return new IceCream { Name = "Succulent Strawberry", Type = "Sorbet", Image = "Assets/60Strawberry.png" };
            }
        }

        /// <summary>
        /// This is the click handler for the 'Select All' button.  When clicked we want to select all flavors in our list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            IceCreamList.SelectAll();
        }

        /// <summary>
        /// This is the click handler for the 'Clear' button.  When clicked we want to clear all selected items in our list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            IceCreamList.SelectedIndex = -1;
            
        }

        /// <summary>
        /// This is the click handler for the 'Delete' button.  When clicked we want to delete all selected items in our list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            List<IceCream> items = new List<IceCream>();
            foreach (IceCream item in IceCreamList.SelectedItems)
            {
                items.Add(item);
                //Flavors.Remove(item);
            }
            foreach(IceCream item in items)
            {
                Flavors.Remove(item);
            }
        }

        /// <summary>
        /// This is the click handler for the 'Back' button.  When clicked we want to go back to the main sample page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            rootPage.Frame.GoBack();
        }

        /// <summary>
        /// This is the click handler for our 'Add' button.  It doesn't really do much of anything interesting :)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void Add_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog d = new MessageDialog("XAML AppBar Control Sample");
            d.Content = "Add button pressed";
            await d.ShowAsync();
        }
    }
}
