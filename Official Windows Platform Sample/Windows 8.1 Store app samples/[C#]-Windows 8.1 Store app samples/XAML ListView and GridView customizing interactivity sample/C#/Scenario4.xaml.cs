//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Expression.Blend.SampleData.SampleDataSource;
using SDKTemplate;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ListViewInteraction
{
    public sealed partial class Scenario4 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // sample data - See SampleData Folder
        ToppingsData toppingsData = null;
        StoreData storeData = null;

        public Scenario4()
        {
            this.InitializeComponent();

            // initializing sample data
            toppingsData = new ToppingsData();
            storeData = new StoreData();
            // setting GridView data sources to sample data
            FlavorGrid.ItemsSource = storeData.Collection;
            FixinsGrid.ItemsSource = toppingsData.Collection;
        }

        private void CreateCustomCarton_Click(object sender, RoutedEventArgs e)
        {
            rootPage.NotifyUser("", NotifyType.StatusMessage);
            if (FlavorGrid.SelectedItems.Count > 0)
            {
                CustomCarton.Text = "Custom Carton: ";
                char[] charsToTrim = { ',', ' ' };
                CustomCarton.Text += ((Item)FlavorGrid.SelectedItem).Title;
                if (FixinsGrid.SelectedItems.Count > 0)
                {
                    CustomCarton.Text += " with ";
                    foreach (Item topping in FixinsGrid.SelectedItems)
                    {
                        CustomCarton.Text += topping.Title + ", ";
                    }
                    CustomCarton.Text = CustomCarton.Text.TrimEnd(charsToTrim);
                    CustomCarton.Text += " toppings";
                }
            }
            else
            {
                rootPage.NotifyUser("Please select at least a flavor...", NotifyType.ErrorMessage);
            }
        }


        #region Data Visualization
        /// <summary>
        /// We will visualize the data item in asynchronously in multiple phases for improved panning user experience 
        /// of large lists.  In this sample scneario, we will visualize different parts of the data item
        /// in the following order:
        /// 
        ///     1) Placeholders (visualized synchronously - Phase 0)
        ///     2) Tilte (visualized asynchronously - Phase 1)
        ///     3) Category and Image (visualized asynchronously - Phase 2)
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void GridView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            ItemViewer iv = args.ItemContainer.ContentTemplateRoot as ItemViewer;

            if (args.InRecycleQueue == true)
            {
                iv.ClearData();
            }
            else if (args.Phase == 0)
            {
                iv.ShowPlaceholder(args.Item as Item);

                // Register for async callback to visualize Title asynchronously
                args.RegisterUpdateCallback(ContainerContentChangingDelegate);
            }
            else if (args.Phase == 1)
            {
                iv.ShowTitle();
                args.RegisterUpdateCallback(ContainerContentChangingDelegate);
            }
            else if (args.Phase == 2)
            {
                iv.ShowCategory();
                iv.ShowImage();
            }

            // For imporved performance, set Handled to true since app is visualizing the data item
            args.Handled = true;
        }

        /// <summary>
        /// Managing delegate creation to ensure we instantiate a single instance for 
        /// optimal performance. 
        /// </summary>
        private TypedEventHandler<ListViewBase, ContainerContentChangingEventArgs> ContainerContentChangingDelegate
        {
            get
            {
                if (_delegate == null)
                {
                    _delegate = new TypedEventHandler<ListViewBase, ContainerContentChangingEventArgs>(GridView_ContainerContentChanging);
                }
                return _delegate;
            }
        }
        private TypedEventHandler<ListViewBase, ContainerContentChangingEventArgs> _delegate;

        #endregion //Data Visualization
    }
}
