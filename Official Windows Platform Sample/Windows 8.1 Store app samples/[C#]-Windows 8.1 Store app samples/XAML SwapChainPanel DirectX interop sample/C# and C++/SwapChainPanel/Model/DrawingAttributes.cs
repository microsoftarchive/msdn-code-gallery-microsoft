//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
//
//*********************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SDKTemplate.Common;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace SwapChainPanel.Model
{
    /// <summary>
    /// A simple data model that tracks the drawing parameters for an inking surface.
    /// </summary>
    class DrawingAttributes : BindableBase
    {
        public DrawingAttributes() : this(Colors.Black, new Size(4, 4), false)
        {
        }

        public DrawingAttributes(Color brushColor, Size brushSize, bool brushFitsToCurve, [Optional] IEnumerable<Color> activePaletteColors)
        {
            _brushColor = brushColor;
            _brushSize = brushSize;
            _brushFitsToCurve = brushFitsToCurve;

            _activePaletteColors = new PaletteColorCollection(activePaletteColors);
            _brushSizes = new BrushSizesCollection();
        }

        private bool _brushIsEraser = false;
        /// <summary>
        /// Whether the brush is in erase mode.
        /// </summary>
        public bool BrushIsEraser
        {
            get { return _brushIsEraser; }
            set { SetProperty(ref _brushIsEraser, value); }
        }

        private Color _brushColor = Colors.Black;
        /// <summary>
        /// Color of the current drawing brush.
        /// </summary>
        public Color BrushColor
        {
            get { return _brushColor; }
            set { SetProperty(ref _brushColor, value); }
        }

        private bool _brushFitsToCurve = false;
        /// <summary>
        /// Whether the current drawing brush smooths strokes.
        /// </summary>
        public bool BrushFitsToCurve
        {
            get { return _brushFitsToCurve; }
            set { SetProperty(ref _brushFitsToCurve, value); }
        }

        private Size _brushSize;
        /// <summary>
        /// Size of the current drawing brush.
        /// </summary>
        public Size BrushSize
        {
            get { return _brushSize; }
            set { SetProperty(ref _brushSize, value); }
        }

        private PaletteColorCollection _activePaletteColors = null;
        /// <summary>
        /// The list of colors that are active on a palette control.
        /// </summary>
        public PaletteColorCollection ActivePaletteColors
        {
            get { return _activePaletteColors; }
        }

        private static AllColorsCollection _allColors = new AllColorsCollection();
        /// <summary>
        /// All colors available in a palette control.
        /// </summary>
        public static AllColorsCollection AllColors
        {
            get { return _allColors; }
        }

        private BrushSizesCollection _brushSizes = null;
        /// <summary>
        /// All supported sizes for a drawing brush.
        /// </summary>
        public BrushSizesCollection BrushSizes
        {
            get { return _brushSizes; }
        }
    }

    /// <summary>
    /// A collection of supported drawing brush sizes.
    /// </summary>
    public class BrushSizesCollection : IReadOnlyCollection<Size>
    {
        private static readonly Size[] Sizes = { new Size(2, 2), new Size(4, 4), new Size(10, 10), new Size(20, 20), new Size(40, 40) };
        
        public BrushSizesCollection()
        {
        }

        public int Count
        {
            get { return Sizes.Length; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Sizes.GetEnumerator();
        }

        IEnumerator<Size> IEnumerable<Size>.GetEnumerator()
        {
            return Sizes.Cast<Size>().GetEnumerator();
        }
    }

    /// <summary>
    /// A collection of all available colors to be used in a palette control.
    /// </summary>
    public class AllColorsCollection : IReadOnlyCollection<Color>
    {
        private static readonly Color[] AllColors = {
            Colors.White, Colors.Black, Colors.DarkGray, Colors.LightGray,
            Colors.DarkRed, Colors.Red, Colors.Orange, Colors.Salmon,
            Colors.Yellow, Colors.Bisque, Colors.Goldenrod, Colors.YellowGreen,
            Colors.ForestGreen, Colors.Green, Colors.LawnGreen, Colors.SeaGreen,
            Colors.Navy, Colors.Blue, Colors.CornflowerBlue, Colors.SkyBlue,
            Colors.Indigo, Colors.Purple, Colors.Plum, Colors.MediumPurple
            };

        public AllColorsCollection()
        {
            
        }

        public int Count
        {
            get { return AllColors.Length; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return AllColors.GetEnumerator();
        }

        IEnumerator<Color> IEnumerable<Color>.GetEnumerator()
        {
            return AllColors.Cast<Color>().GetEnumerator();
        }
    }

    /// <summary>
    /// A collection of active colors being used in a palette control.
    /// </summary>
    public class PaletteColorCollection : IList<Color>, IList, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private const int MaxCount = 4;
        private static readonly Color[] DefaultColors = { Colors.White, Colors.Black, Colors.Red, Colors.Blue };
        private Queue<Color> _colors;
        
        public PaletteColorCollection([Optional] IEnumerable<Color> colors)
        {
            _colors = new Queue<Color>(colors != null ? colors : DefaultColors);
            RemoveExtraneousItems();
        }

        private void RemoveExtraneousItems()
        {
            while (_colors.Count > MaxCount)
            {
                _colors.Dequeue();
            }
        }

        private void RaiseCollectionReset()
        {
            var eventHandler = this.CollectionChanged;
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        #region IList<Color>
        public void Add(Color item)
        {
            if (!_colors.Contains(item))
            {
                _colors.Enqueue(item);
                RemoveExtraneousItems();
                RaiseCollectionReset();
            }
        }

        public void Clear()
        {
            _colors = new Queue<Color>(DefaultColors);
            RaiseCollectionReset();
        }

        public bool Contains(Color item)
        {
            return _colors.Contains(item);
        }

        public void CopyTo(Color[] array, int arrayIndex)
        {
            _colors.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _colors.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Color item)
        {
            return false;
        }

        public IEnumerator<Color> GetEnumerator()
        {
            return _colors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _colors.GetEnumerator();
        }

        public int IndexOf(Color item)
        {
            var enumerator = _colors.GetEnumerator();
            int i = 0;
            while (enumerator.MoveNext())
            {
                if (enumerator.Current == item)
                {
                    return i;
                }
                i++;
            }

            return -1;
        }

        #region Not Implemented

        public void Insert(int index, Color item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public Color this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #endregion

        #region IList
        int IList.Add(object value)
        {
            if (value is Color)
            {
                this.Add((Color)value);
                return this.Count - 1;
            }
            
            return -1;            
        }

        void IList.Clear()
        {
            this.Clear();
        }

        bool IList.Contains(object value)
        {
            return value is Color ? this.Contains((Color)value) : false;
        }

        bool IList.IsFixedSize
        {
            get { return false; }
        }

        bool IList.IsReadOnly
        {
            get { return false; }
        }

        int ICollection.Count
        {
            get { return this.Count; }
        }

        object IList.this[int index]
        {
            get
            {
                return _colors.ElementAt<Color>(index);
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        int IList.IndexOf(object value)
        {
            return (value is Color) ? IndexOf((Color)value) : -1;
        }

        #region Not implemented

        void IList.Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        void IList.Remove(object value)
        {
            throw new NotImplementedException();
        }

        void IList.RemoveAt(int index)
        {
            throw new NotImplementedException();
        }        

        void ICollection.CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }


        bool ICollection.IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        object ICollection.SyncRoot
        {
            get { throw new NotImplementedException(); }
        }
        #endregion

        #endregion

    }
}
