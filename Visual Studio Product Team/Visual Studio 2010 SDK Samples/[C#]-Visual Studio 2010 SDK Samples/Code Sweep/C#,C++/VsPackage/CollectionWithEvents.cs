/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Samples.VisualStudio.CodeSweep.VSPackage
{
    class ItemEventArgs<T> : EventArgs
    {
        readonly T _item;

        public ItemEventArgs(T item)
        {
            _item = item;
        }

        T Item
        {
            get { return _item; }
        }
    }

    class CollectionWithEvents<T> : ICollection<T>
    {
        readonly List<T> _list = new List<T>();

        /// <summary>
        /// Fired once for each item that is added to the collection, after the item is added.
        /// </summary>
        public event EventHandler<ItemEventArgs<T>> ItemAdded;

        /// <summary>
        /// Fired once for each item that is removed from the collection, before the item is removed.
        /// </summary>
        public event EventHandler<ItemEventArgs<int>> ItemRemoving;

        /// <summary>
        /// Fired once for each action that removes one or more items from the collection, after the items have been removed.
        /// </summary>
        public event EventHandler ItemsRemoved;

        public void AddRange(IEnumerable<T> content)
        {
            foreach (T t in content)
            {
                Add(t);
            }
        }

        #region ICollection<T> Members

        public void Add(T item)
        {
            _list.Add(item);
            if (ItemAdded != null)
            {
                ItemAdded(this, new ItemEventArgs<T>(item));
            }
        }

        public void Clear()
        {
            if (ItemRemoving != null)
            {
                for (int i = 0; i < Count; ++i)
                {
                    ItemRemoving(this, new ItemEventArgs<int>(i));
                }
            }
            _list.Clear();
            if (ItemsRemoved != null)
            {
                ItemsRemoved(this, new EventArgs());
            }
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            if (ItemRemoving != null)
            {
                int index = _list.IndexOf(item);
                if (index >= 0)
                {
                    ItemRemoving(this, new ItemEventArgs<int>(index));
                }
            }
            bool result = _list.Remove(item);
            if (ItemsRemoved != null)
            {
                ItemsRemoved(this, new EventArgs());
            }
            return result;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion
    }
}
