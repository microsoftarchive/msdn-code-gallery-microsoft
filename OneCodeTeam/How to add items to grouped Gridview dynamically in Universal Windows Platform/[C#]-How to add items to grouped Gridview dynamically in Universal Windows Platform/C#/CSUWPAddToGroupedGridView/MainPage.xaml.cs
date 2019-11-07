/****************************** Module Header ******************************\
 * Module Name:  MainPage.xaml.cs
 * Project:      CSUWPAddToGroupedGridView
 * Copyright (c) Microsoft Corporation.
 * 
 * This sample demonstrate how to add item to grouped GridView in UWP.
 *  
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using CSUWPAddToGroupedGridView.SampleData;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;


namespace CSUWPAddToGroupedGridView
{
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// The data source for the grouped grid view.
        /// </summary>
        private readonly ObservableCollection<GroupInfoCollection<Item>> _source;
        /// <summary>
        /// The variable is used to share information between the handler of the 
        /// click handler of the add item button and the layout updated
        /// handler of the grid-view. As both event handlers are executed 
        /// potentially concurrently, the access must be interlocked. 
        /// </summary>
        private int _itemAdded;

        public MainPage()
        {
            InitializeComponent();

            _source = (new StoreData()).GetGroupsByCategory();

            CollectionViewSource.Source = _source;

            ObservableCollection<string> pictureOptions = new ObservableCollection<string>
                                                              {
                                                                  "Banana",
                                                                  "Lemon",
                                                                  "Mint",
                                                                  "Orange",
                                                                  "SauceCaramel",
                                                                  "SauceChocolate",
                                                                  "SauceStrawberry",
                                                                  "SprinklesChocolate",
                                                                  "SprinklesRainbow",
                                                                  "SprinklesVanilla",
                                                                  "Strawberry",
                                                                  "Vanilla"
                                                              };
            PictureComboBox.ItemsSource = pictureOptions;
            PictureComboBox.SelectedIndex = 0;

            ObservableCollection<string> groupOptions = new ObservableCollection<string>();
            foreach (GroupInfoCollection<Item> groupInfoList in _source)
            {
                groupOptions.Add(groupInfoList.Key);
            }

            GroupComboBox.ItemsSource = groupOptions;
            GroupComboBox.SelectedIndex = 0;
        }

        #region Common methods

        /// <summary>
        /// The the event handler for the click event of the link in the footer. 
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private async void FooterClick(object sender, RoutedEventArgs e)
        {
            HyperlinkButton hyperlinkButton = sender as HyperlinkButton;
            if (hyperlinkButton != null)
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri(hyperlinkButton.Tag.ToString()));
            }
        }

        #endregion
        /// <summary>
        /// The event handler for the click event of the AddItem button. 
        /// The method creates a new object of type <see cref="Item"/>.
        /// From the observable collection containing the groups, the collection 
        /// for the selected group is selected. The Single-query will always succeed, 
        /// because the drop-down contains exactly the set of groups that is present 
        /// in _source. Then the new item is added to the collection. As both 
        /// collections are observable and are connected to the controls using data
        /// binding, the list will automatically update. 
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void AddItemClick(object sender, RoutedEventArgs e)
        {
            string path = string.Format(
                CultureInfo.InvariantCulture,
                "SampleData/Images/60{0}.png",
                PictureComboBox.SelectedItem);

            Item item = new Item
            {
                Title = TitleTextBox.Text,
                Category = (string)GroupComboBox.SelectedItem
            };
            item.SetImage(StoreData.BaseUri, path);

            GroupInfoCollection<Item> group =
                _source.Single(groupInfoList => groupInfoList.Key == item.Category);

            group.Add(item);
            Interlocked.Increment(ref _itemAdded);
        }

        /// <summary>
        /// If the selection in the combo-box for the group-selection changes, 
        /// the grouped grid view scrolls the selected group into view. This is 
        /// especially useful in narrow views. 
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        private void GroupComboBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string key = GroupComboBox.SelectedItem as string;
            GroupInfoCollection<Item> group = _source.Single(groupInfoList => groupInfoList.Key == key);
            ItemsByCategory.ScrollIntoView(group);
        }

        /// <summary>
        /// If the data structure bound to the grid-view changes and causes the layout to update
        /// (i.e. if an item was added), it is scrolled to the category in the combo box. 
        /// We can't call the scroll directly after adding the item (in AddItemClick), because
        /// we have to wait until the dependency property signals the control that it changed and 
        /// the control changed its layout. If a new column is added by adding the new item, the control 
        /// wouldn't scroll that new column into view otherwise. 
        /// </summary>
        /// <param name="sender">The sender</param>
        /// <param name="e">The event arguments</param>
        private void ItemsByCategoryLayoutUpdated(object sender, object e)
        {
            int needsToScroll = Interlocked.Exchange(ref _itemAdded, 0);

            if (needsToScroll != 0)
            {
                string key = GroupComboBox.SelectedItem as string;
                GroupInfoCollection<Item> group = _source.Single(groupInfoList => groupInfoList.Key == key);
                ItemsByCategory.ScrollIntoView(group);
            }
        }
    }
}

