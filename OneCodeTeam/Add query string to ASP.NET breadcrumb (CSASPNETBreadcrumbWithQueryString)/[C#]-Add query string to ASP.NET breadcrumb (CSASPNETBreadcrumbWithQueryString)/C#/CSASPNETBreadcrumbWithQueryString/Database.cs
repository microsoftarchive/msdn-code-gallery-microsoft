/****************************** Module Header ******************************\
* Module Name:    Database.cs
* Project:        CSASPNETBreadcrumbWithQueryString
* Copyright (c) Microsoft Corporation
*
* This is a very simple in-code database for demo purpose. It is not the point 
* of this code sample project.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System.Collections.Generic;

namespace CSASPNETBreadcrumbWithQueryString
{
    /// <summary>
    /// This is a very simple in-code database for demo purpose.
    /// </summary>
    public static class Database
    {
        public static List<string> Categories { get; set; }
        public static List<KeyValuePair<string, string>> Items { get; set; }

        static Database()
        {
            Categories = new List<string>() { "Category1", "Category2", "Category3" };
            Items = new List<KeyValuePair<string, string>>();
            Items.Add(new KeyValuePair<string, string>("Category1", "Item1"));
            Items.Add(new KeyValuePair<string, string>("Category1", "Item2"));
            Items.Add(new KeyValuePair<string, string>("Category1", "Item3"));
            Items.Add(new KeyValuePair<string, string>("Category2", "Item4"));
            Items.Add(new KeyValuePair<string, string>("Category2", "Item5"));
            Items.Add(new KeyValuePair<string, string>("Category2", "Item6"));
            Items.Add(new KeyValuePair<string, string>("Category3", "Item7"));
            Items.Add(new KeyValuePair<string, string>("Category3", "Item8"));
            Items.Add(new KeyValuePair<string, string>("Category3", "Item9"));
        }
        public static string GetCategoryByItem(string item)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Value == item)
                {
                    return Items[i].Key;
                }
            }
            return string.Empty;
        }
        public static List<string> GetItemsByCategory(string category)
        {
            List<string> list = new List<string>();
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i].Key == category)
                {
                    list.Add(Items[i].Value);
                }
            }
            return list;
        }
    }
}