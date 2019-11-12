/****************************** Module Header ******************************\
 * Module Name:  ItemCollection.cs
 * Project:      CSUWPAddToGroupedGridView
 * Copyright (c) Microsoft Corporation.
 * 
 * This is the collection which stores item objects.
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

using System.Collections.Generic;

namespace CSUWPAddToGroupedGridView.SampleData
{

    /// <summary>
    /// The item collection.
    /// Workaround: data binding works best with an enumeration of objects that does not implement IList
    /// </summary>
    public class ItemCollection : IEnumerable<Item>
    {
        /// <summary>
        /// The collection storing the items internally. 
        /// </summary>
        private readonly System.Collections.ObjectModel.ObservableCollection<Item> _itemCollection = new System.Collections.ObjectModel.ObservableCollection<Item>();

        /// <summary>
        /// Gets an enumerator, enumerating the items in the collection.
        /// </summary>
        /// <returns>
        /// Returns an <see cref="IEnumerator"/>, enumerating the items in the collection.
        /// </returns>
        public IEnumerator<Item> GetEnumerator()
        {
            return _itemCollection.GetEnumerator();
        }

        /// <summary>
        /// Gets an un-typed enumerator, enumerating the items in the collection.
        /// </summary>
        /// <returns>
        /// Returns an un-typed <see cref="IEnumerator"/>, enumerating the items in the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the collection. 
        /// </summary>
        /// <param name="item">
        /// The item to be added to the collection.
        /// </param>
        public void Add(Item item)
        {
            _itemCollection.Add(item);
        }
    }
}
