//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SDKTemplate;
using System;
using Expression.Blend.SampleData.SampleDataSource;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace ListViewDnD
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>



    public sealed partial class Scenario5 : SDKTemplate.Common.LayoutAwarePage
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        // holds the sample data - See SampleData folder
        StoreData storeData = null;
        StoreData storeData2 = null;

        int currDropIndex = -1;

        SolidColorBrush blueBorder = new SolidColorBrush(Colors.Blue);
        Thickness thickBorder = new Thickness(2);
        Thickness noBorder = new Thickness(0);

        public Scenario5()
        {
            this.InitializeComponent();

            // create a new instance of store data
            storeData = new StoreData();
            // set the source of the GridView to be the sample data
            ItemListView.ItemsSource = storeData.Collection;

            storeData2 = new StoreData();
            ItemListView2.ItemsSource = storeData2.Collection;
        }

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
        void ItemListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
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
                    _delegate = new TypedEventHandler<ListViewBase, ContainerContentChangingEventArgs>(ItemListView_ContainerContentChanging);
                }
                return _delegate;
            }
        }
        private TypedEventHandler<ListViewBase, ContainerContentChangingEventArgs> _delegate;

        private void ItemListView_DragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            e.Data.SetData("data", (e.Items[0] as Item).Title);
        }

        private async void ItemListView2_Drop(object sender, DragEventArgs e)
        {
            string data = await e.Data.GetView().GetTextAsync("data");

            //Find the position where item will be dropped in the listview
            Point pos = e.GetPosition(ItemListView2.ItemsPanelRoot);

            //Get the size of one of the list items
            ListViewItem lvi = (ListViewItem)ItemListView2.ContainerFromIndex(0);
            double itemHeight = lvi.ActualHeight + lvi.Margin.Top + lvi.Margin.Bottom;

            //Determine the index of the item from the item position (assumed all items are the same size)
            int index = Math.Min(ItemListView2.Items.Count - 1, (int)(pos.Y / itemHeight));

            rootPage.NotifyUser("You dropped \'" + data + "\' from the first list onto item \'" + (index + 1) + "\' of the second list", NotifyType.StatusMessage);

            //Remove the border from the item
            ListViewItem lvidropOld = (ListViewItem)ItemListView2.ContainerFromIndex(currDropIndex);
            lvidropOld.BorderThickness = noBorder;

        }

        void dropList_DragOver(object sender, DragEventArgs e)
        {
            var pos = e.GetPosition(ItemListView2.ItemsPanelRoot);
            ListViewItem lvi = (ListViewItem)ItemListView2.ContainerFromIndex(0);
            double itemHeight = lvi.ActualHeight + lvi.Margin.Top + lvi.Margin.Bottom;

            int index = Math.Min(ItemListView2.Items.Count - 1, (int)(pos.Y / itemHeight));
            ListViewItem lvidrop = (ListViewItem)ItemListView2.ContainerFromIndex(index);

            if (index != currDropIndex)
            {
                if (currDropIndex >= 0)
                {
                    //remove the border
                    ListViewItem lvidropOld = (ListViewItem)ItemListView2.ContainerFromIndex(currDropIndex);
                    lvidropOld.BorderThickness = noBorder;
                }
                lvidrop.BorderBrush = blueBorder;
                lvidrop.BorderThickness = thickBorder;
                currDropIndex = index;
            }
        }

        void dropList_DragLeave(object sender, DragEventArgs e)
        {
            ListViewItem lvidropOld = (ListViewItem)ItemListView2.ContainerFromIndex(currDropIndex);
            lvidropOld.BorderThickness = noBorder;
            currDropIndex = -1;
        }

    }
}
