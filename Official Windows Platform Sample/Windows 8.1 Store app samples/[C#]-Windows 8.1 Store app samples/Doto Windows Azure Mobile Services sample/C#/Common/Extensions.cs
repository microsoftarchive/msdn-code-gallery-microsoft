// Copyright (c) Microsoft Corporation. All rights reserved

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Doto
{
    /// <summary>
    /// Useful extension method for Doto sample
    /// </summary>
    public static class Extensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
