/****************************** Module Header ******************************\
 * Module Name:  StoreData.cs
 * Project:      CSUWPAddToGroupedGridView
 * Copyright (c) Microsoft Corporation.
 * 
 * This is the sample data which bind to the GridView
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

namespace CSUWPAddToGroupedGridView.SampleData
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// The store data used to initially populate the grid.
    /// </summary>
    public class StoreData
    {
        /// <summary>
        /// The base URI for items originating in the folder the app is installed to. 
        /// </summary>
        public static readonly Uri BaseUri = new Uri("ms-appx:///");

        /// <summary>
        /// The collection storing the items internally. 
        /// </summary>
        private readonly ItemCollection _collection = new ItemCollection();

        /// <summary>
        /// Initializes a new instance of the <see cref="StoreData"/> class.
        /// </summary>
        public StoreData()
        {
            Item item = new Item
            {
                Title = "Banana Blast Frozen Yogurt",
                Category = "Low-fat frozen yogurt"
            };
            item.SetImage(BaseUri, "SampleData/Images/60Banana.png");
            Collection.Add(item);

            item = new Item
            {
                Title = "Lavish Lemon Ice",
                Category = "Sorbet"
            };
            item.SetImage(BaseUri, "SampleData/Images/60Lemon.png");
            Collection.Add(item);

            item = new Item
            {
                Title = "Marvelous Mint",
                Category = "Gelato"
            };
            item.SetImage(BaseUri, "SampleData/Images/60Mint.png");
            Collection.Add(item);

            item = new Item
            {
                Title = "Creamy Orange",
                Category = "Sorbet"
            };
            item.SetImage(BaseUri, "SampleData/Images/60Orange.png");
            Collection.Add(item);

            item = new Item
            {
                Title = "Succulent Strawberry",
                Category = "Sorbet"
            };
            item.SetImage(BaseUri, "SampleData/Images/60Strawberry.png");
            Collection.Add(item);

            item = new Item
            {
                Title = "Very Vanilla",
                Category = "Ice Cream"
            };
            item.SetImage(BaseUri, "SampleData/Images/60Vanilla.png");
            Collection.Add(item);

            item = new Item
            {
                Title = "Creamy Caramel Frozen Yogurt",
                Category = "Low-fat frozen yogurt"
            };
            item.SetImage(BaseUri, "SampleData/Images/60SauceCaramel.png");
            Collection.Add(item);

            item = new Item
            {
                Title = "Chocolate Lovers Frozen Yougurt",
                Category = "Low-fat frozen yogurt"
            };
            item.SetImage(BaseUri, "SampleData/Images/60SauceChocolate.png");
            Collection.Add(item);

            item = new Item
            {
                Title = "Roma Strawberry",
                Category = "Gelato"
            };
            item.SetImage(BaseUri, "SampleData/Images/60Strawberry.png");
            Collection.Add(item);

            item = new Item
            {
                Title = "Italian Rainbow",
                Category = "Gelato"
            };
            item.SetImage(BaseUri, "SampleData/Images/60SprinklesRainbow.png");
            Collection.Add(item);

            item = new Item
            {
                Title = "Strawberry",
                Category = "Ice Cream"
            };
            item.SetImage(BaseUri, "SampleData/Images/60Strawberry.png");
            Collection.Add(item);

            item = new Item
            {
                Title = "Strawberry Frozen Yogurt",
                Category = "Low-fat frozen yogurt"
            };
            item.SetImage(BaseUri, "SampleData/Images/60Strawberry.png");
            Collection.Add(item);

            item = new Item
            {
                Title = "Bongo Banana",
                Category = "Sorbet"
            };
            item.SetImage(BaseUri, "SampleData/Images/60Banana.png");
            Collection.Add(item);

            item = new Item
            {
                Title = "Firenze Vanilla",
                Category = "Gelato"
            };
            item.SetImage(BaseUri, "SampleData/Images/60Vanilla.png");
            Collection.Add(item);

            item = new Item
            {
                Title = "Choco-wocko",
                Category = "Sorbet"
            };
            item.SetImage(BaseUri, "SampleData/Images/60SauceChocolate.png");
            Collection.Add(item);

            item = new Item
            {
                Title = "Chocolate",
                Category = "Ice Cream"
            };
            item.SetImage(BaseUri, "SampleData/Images/60SauceChocolate.png");
            Collection.Add(item);
        }

        /// <summary>
        /// Gets the collection.
        /// </summary>
        public ItemCollection Collection
        {
            get
            {
                return _collection;
            }
        }

        /// <summary>
        /// The method returns the list of groups, each containing a key and a list of items. 
        /// The key of each group is the category of the item. 
        /// </summary>
        /// <returns>
        /// The List of groups of items. 
        /// </returns>
        internal ObservableCollection<GroupInfoCollection<Item>> GetGroupsByCategory()
        {
            ObservableCollection<GroupInfoCollection<Item>> groups = new ObservableCollection<GroupInfoCollection<Item>>();

            var query = from item in Collection
                        orderby item.Category
                        group item by item.Category into g
                        select new { GroupName = g.Key, Items = g };

            foreach (var g in query)
            {
                GroupInfoCollection<Item> info = new GroupInfoCollection<Item>
                                                 {
                                                     Key = g.GroupName
                                                 };
                foreach (Item item in g.Items)
                {
                    info.Add(item);                    
                }

                groups.Add(info);
            }

            return groups;
        }
    }
}
