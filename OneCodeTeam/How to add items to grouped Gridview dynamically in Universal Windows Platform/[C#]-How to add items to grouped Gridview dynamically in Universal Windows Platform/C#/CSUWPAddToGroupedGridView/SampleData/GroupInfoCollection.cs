/****************************** Module Header ******************************\
 * Module Name:  GroupInfoCollection.cs
 * Project:      CSUWPAddToGroupedGridView
 * Copyright (c) Microsoft Corporation.
 * 
 * This is the grouped collection.
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
using System.Collections.ObjectModel;

namespace CSUWPAddToGroupedGridView.SampleData
{

    /// <summary>
    /// The group info list.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the items in the list. 
    /// </typeparam>
    public class GroupInfoCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Gets or sets the key of the group.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets the enumerator, enumerating the list of items.
        /// </summary>
        /// <returns>
        /// Returns the enumerator, enumerating the list of items. 
        /// </returns>
        public new IEnumerator<T> GetEnumerator()
        {
            return base.GetEnumerator();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return Key;
        }
    }
}
