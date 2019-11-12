// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="MediaViewer.cs">
//   Copyright (c) 2012 - 2014 Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   Use of this sample source code is subject to the terms of the Microsoft license
//   agreement under which you licensed this sample source code and is provided AS-IS.
//   If you did not accept the terms of the license agreement, you are not authorized
//   to use this sample source code. For the terms of the license, please see the
//   license agreement between you and Microsoft.<br/><br/>
//   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604.
// </summary>
// ----------------------------------------------------------------------------

namespace Microsoft.Phone.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;

    public enum DisplayedElementType { None, Header, Item, Footer }
    public enum InitiallyDisplayedElementType { First, Last }

    /// <summary>
    /// Displays a virtualized list of Items, allowing the user to swipe
    /// from item to item.  An optional header and optional footer may
    /// also be displayed.  The user can also pinch zoom into and back
    /// out of items.
    /// </summary>
    /// <remarks>
    /// This code is taken from the Code Samples for Windows Phone (http://code.msdn.microsoft.com/wpapps).
    /// </remarks>
    [TemplatePart(Name = "MediaStrip", Type = typeof(Canvas))]
    [TemplatePart(Name = "MediaStripCompositeTransform", Type = typeof(CompositeTransform))]
    public class MediaViewer : Control
    {
        /// <summary>
        /// The number of items to load at a time.  Must be odd.
        /// </summary>
        private const int _virtualizedItemPoolSize = 3;
        
        /// <summary>
        /// The amount of space between items.  For full-screen items this will only be visible when scrolling from
        /// one item to another.
        /// </summary>
        private const double _itemGutter = 18;

        /// <summary>
        /// How many pixels past the beginning or end of the list the user will be allowed to drag.
        /// Note that during a drag past the beginning or end of the list the user will see the "squish" animation.
        /// </summary>
        private const double _maxDraggingSquishDistance = 150;

        /// <summary>
        /// How much to squish the UI if you drag maxDraggingSquishDistance past the beginning or end of the UI.
        /// </summary>
        private const double _minDraggingSquishScale = 0.90;

        /// <summary>
        /// How long the unsquish animation should take
        /// </summary>
        private const int _unsquishAnimationMilliseconds = 100;

        /// <summary>
        /// How long should a pause in dragging be before it resets the inertia calculation?  This is in milliseconds.
        /// </summary>
        private const double _dragStagnationTimeThreshold = 300;

        /// <summary>
        /// Tolerance in pixels for considering a drag stopped
        /// </summary>
        private const double _dragStagnationDistanceThreshold = 15;

        // These constants define how fast the inertia animation will run after a flick.  The actual flick speed 
        // (in pixels / ms) is mapped in the range _flickMinInputVelocity, _flickMaxInputVelocity, and an inertia
        // animation duration is then calculated as the same proportion of _flickMinOutputMilliseconds, 
        // _flickMaxOutputMilliseconds.
        private const double _flickMinInputVelocity = 0;
        private const double _flickMaxInputVelocity = 5;
        private const double _flickMinOutputMilliseconds = 100;
        private const double _flickMaxOutputMilliseconds = 800;
        
        private enum MediaViewerState 
        { 
            Uninitialized, 
            Initialized, 
            InertiaAnimating, 
            Dragging, 
            DraggingAndSquishing, 
            Pinching, 
            UnsquishAnimating 
        }

        private CompositeTransform _mediaStripCompositeTransform;
        private MediaViewerState _state = MediaViewerState.Uninitialized;
        private List<VirtualizedItem> _virtualizedItemPool;
        private Canvas _mediaStrip;
        private Size? _size = null;
        private DragState _dragState = new DragState(_maxDraggingSquishDistance);
        private double ScrollOffset
        {
            get
            {
                return this._mediaStripCompositeTransform.TranslateX;
            }
            set
            {
                this._mediaStripCompositeTransform.TranslateX = value;
            }
        }
        private Storyboard _dragInertiaAnimation;
        private Storyboard _unsquishAnimation;
        private DoubleAnimation _dragInertiaAnimationTranslation;
        private DoubleAnimation _unsquishAnimationTranslation;
        private VirtualizedItem _displayedVirtualizedItem;
        private FrameworkElement _headerTemplateInstance;
        private FrameworkElement _footerTemplateInstance;
        private FrameworkElement _headerOnMediaStrip;
        private FrameworkElement _footerOnMediaStrip;

        /// <summary>
        /// The index of the displayed element.  An element can be the header, an item, or the footer.
        /// If there is no header, footer, or items, this value is null;
        /// </summary>
        private int? _displayedElementIndex;

        #region Public Properties

        public static readonly DependencyProperty ItemTemplateProperty = DependencyProperty.Register(
            "ItemTemplate", 
            typeof(DataTemplate), 
            typeof(MediaViewer), 
            new PropertyMetadata(null, OnItemTemplatePropertyChanged));

        /// <summary>
        /// The DataTemplate used to represent each virtualized item.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(ItemTemplateProperty);
            }
            set
            {
                this.SetValue(ItemTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items", 
            typeof(ObservableCollection<object>), 
            typeof(MediaViewer),
            new PropertyMetadata(OnItemsPropertyChanged));

        /// <summary>
        /// The collection of items to display in a virtualized fashion.
        /// </summary>
        public ObservableCollection<object> Items
        {
            get { return (ObservableCollection<object>)this.GetValue(ItemsProperty); }
            set { this.SetValue(ItemsProperty, value); }
        }

        public static readonly DependencyProperty DisplayedElementProperty = DependencyProperty.Register(
            "DisplayedElement",
            typeof(DisplayedElementType),
            typeof(MediaViewer),
            new PropertyMetadata(DisplayedElementType.None));

        /// <summary>
        /// Indicates which type of element is displayed (e.g. Header, Item, Footer).
        /// </summary>
        public DisplayedElementType DisplayedElement
        {
            get { return (DisplayedElementType)this.GetValue(DisplayedElementProperty); }
            private set { this.SetValue(DisplayedElementProperty, value); }
        }
        
        public static readonly DependencyProperty DisplayedItemIndexProperty = DependencyProperty.Register(
            "DisplayedItemIndex",
            typeof(int),
            typeof(MediaViewer),
            new PropertyMetadata(null));

        /// <summary>
        /// Index of the currently displayed Item, if any.
        /// </summary>
        public int DisplayedItemIndex
        {
            get { return (int)this.GetValue(DisplayedItemIndexProperty); }
            private set { this.SetValue(DisplayedItemIndexProperty, value); }
        }

        public static readonly DependencyProperty InitiallyDisplayedElementProperty = DependencyProperty.Register(
            "InitiallyDisplayedElement",
            typeof(InitiallyDisplayedElementType),
            typeof(MediaViewer),
            new PropertyMetadata(InitiallyDisplayedElementType.First, OnInitiallyDisplayedElementPropertyChanged));

        /// <summary>
        /// Indicates which element should be displayed initially - the first or last one.
        /// </summary>
        public InitiallyDisplayedElementType InitiallyDisplayedElement
        {
            get { return (InitiallyDisplayedElementType)this.GetValue(InitiallyDisplayedElementProperty); }
            set { this.SetValue(InitiallyDisplayedElementProperty, value); }
        }

        public static readonly DependencyProperty HeaderVisibilityProperty = DependencyProperty.Register(
            "HeaderVisibility", 
            typeof(Visibility), 
            typeof(MediaViewer), 
            new PropertyMetadata(Visibility.Collapsed, OnHeaderVisibilityPropertyChanged));

        /// <summary>
        /// Shows or hides the optional header.
        /// </summary>
        public Visibility HeaderVisibility
        {
            get { return (Visibility)this.GetValue(HeaderVisibilityProperty); }
            set { this.SetValue(HeaderVisibilityProperty, value); }
        }

        public static readonly DependencyProperty FooterVisibilityProperty = DependencyProperty.Register(
            "FooterVisibility", 
            typeof(Visibility), 
            typeof(MediaViewer), 
            new PropertyMetadata(Visibility.Collapsed, OnFooterVisibilityPropertyChanged));

        /// <summary>
        /// Shows or hides the optional footer.
        /// </summary>
        public Visibility FooterVisibility
        {
            get { return (Visibility)this.GetValue(FooterVisibilityProperty); }
            set { this.SetValue(FooterVisibilityProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(
            "Header",
            typeof(FrameworkElement),
            typeof(MediaViewer),
            new PropertyMetadata(null, OnHeaderPropertyChanged));

        /// <summary>
        /// The root FrameworkElement of the Header.
        /// </summary>
        public FrameworkElement Header
        {
            get { return (FrameworkElement)this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty FooterProperty = DependencyProperty.Register(
            "Footer",
            typeof(FrameworkElement),
            typeof(MediaViewer),
            new PropertyMetadata(null, OnFooterPropertyChanged));

        /// <summary>
        /// The root FrameworkElement of the Footer.
        /// </summary>
        public FrameworkElement Footer
        {
            get { return (FrameworkElement)this.GetValue(FooterProperty); }
            set { this.SetValue(FooterProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty = DependencyProperty.Register(
            "HeaderTemplate", 
            typeof(DataTemplate), 
            typeof(MediaViewer), 
            new PropertyMetadata(new PropertyChangedCallback(HeaderTemplateChangedEventHandler)));

        /// <summary>
        /// The DataTemplate used to render the header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(HeaderTemplateProperty);
            }
            set
            {
                this.SetValue(HeaderTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty FooterTemplateProperty = DependencyProperty.Register(
            "FooterTemplate", 
            typeof(DataTemplate), 
            typeof(MediaViewer), 
            new PropertyMetadata(new PropertyChangedCallback(FooterTemplateChangedEventHandler)));

        /// <summary>
        /// The DataTemplate used to render the footer.
        /// </summary>
        public DataTemplate FooterTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(FooterTemplateProperty);
            }
            set
            {
                this.SetValue(FooterTemplateProperty, value);
            }
        }

        public static readonly DependencyProperty DragEnabledProperty = DependencyProperty.Register(
            "DragEnabled",
            typeof(bool),
            typeof(MediaViewer),
            new PropertyMetadata(true));

        /// <summary>
        /// Enables or disables dragging by the user.
        /// </summary>
        public bool DragEnabled
        {
            get { return (bool)this.GetValue(DragEnabledProperty); }
            set { this.SetValue(DragEnabledProperty, value); }
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Raised when the header is displayed.
        /// </summary>
        public event EventHandler HeaderDisplayed;
        /// <summary>
        /// Raised when an item is displayed.
        /// </summary>
        public event EventHandler<ItemDisplayedEventArgs> ItemDisplayed;
        /// <summary>
        /// Raised when the footer is displayed.
        /// </summary>
        public event EventHandler FooterDisplayed;
        /// <summary>
        /// Raised when the currently displayed item is zoomed in.
        /// </summary>
        public event EventHandler ItemZoomed;
        /// <summary>
        /// Raised when the currently displayed item is zoomed back out to nuetral.
        /// </summary>
        public event EventHandler ItemUnzoomed;

        #endregion

        public MediaViewer()
        {
            System.Diagnostics.Debug.Assert(_virtualizedItemPoolSize % 2 == 1);

            this.DefaultStyleKey = typeof(MediaViewer);
            this.SizeChanged += this.OnMediaViewerSizeChanged;
        }

        #region Property Changed Event Handlers

        private static void OnHeaderVisibilityPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            MediaViewer mediaViewer = (MediaViewer)dependencyObject;
            mediaViewer.InitializeOrReset();
        }

        private static void OnFooterVisibilityPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            MediaViewer mediaViewer = (MediaViewer)dependencyObject;
            mediaViewer.InitializeOrReset();
        }

        private static void OnItemsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            MediaViewer thisMediaViewer = (MediaViewer)dependencyObject;

            ObservableCollection<object> oldItems = eventArgs.OldValue as ObservableCollection<object>;
            ObservableCollection<object> newItems = eventArgs.NewValue as ObservableCollection<object>;

            if (oldItems != null)
            {
                oldItems.CollectionChanged -= thisMediaViewer.OnItemsCollectionChanged;
            }

            if (newItems != null)
            {
                newItems.CollectionChanged += thisMediaViewer.OnItemsCollectionChanged;
            }

            thisMediaViewer.InitializeOrReset();
        }

        private void OnItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int numberAdded = e.NewItems.Count;
 
                        // Adjust the media strip for its new length
                        //
                        this.ResetMediaStripGeometry();

                        // We always need to update the footer's location if items are added
                        //
                        this.PlaceFooter();

                        // Update the RepresentingItemIndex for each VirtualizedItem that represents an item
                        // that follows where the new items were added
                        //
                        foreach (VirtualizedItem virtualizedItem in this._virtualizedItemPool)
                        {
                            if ((virtualizedItem.RepresentingItemIndex != null) &&
                                (virtualizedItem.RepresentingItemIndex >= e.NewStartingIndex))
                            {
                                virtualizedItem.RepresentingItemIndex += numberAdded;
                            }
                        }

                        if (this._displayedElementIndex == null)
                        {
                            this.JumpToElement(0);
                        }
                        else
                        {
                            if (this._displayedElementIndex.Value >= e.NewStartingIndex)
                            {
                                // Jump to the element index that now represents what we were already viewing
                                //
                                this.JumpToElement(this._displayedElementIndex.Value + numberAdded);
                            }
                        }
                    } break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        int numberRemoved = e.OldItems.Count;

                        // Adjust the media strip for its new length
                        //
                        this.ResetMediaStripGeometry();

                        // We always need to update the footer's location if items are removed
                        //
                        this.PlaceFooter();

                        foreach (VirtualizedItem virtualizedItem in this._virtualizedItemPool)
                        {
                            if ((virtualizedItem.RepresentingItemIndex != null) &&
                                (virtualizedItem.RepresentingItemIndex >= e.OldStartingIndex) &&
                                (virtualizedItem.RepresentingItemIndex < e.OldStartingIndex + numberRemoved))
                            {
                                // This VirtualizedItem represented an item that was removed, disassociate it
                                //
                                virtualizedItem.RepresentingItemIndex = null;
                                virtualizedItem.DataContext = null;
                            }

                            if ((virtualizedItem.RepresentingItemIndex != null) &&
                                (virtualizedItem.RepresentingItemIndex > e.OldStartingIndex))
                            {
                                // This VirtualizedItem represents an item whose index was changed by this removal
                                //
                                virtualizedItem.RepresentingItemIndex -= numberRemoved;
                            }
                        }

                        if (this._displayedElementIndex != null)
                        {
                            int newElementCount = (int)this.GetElementCount();

                            if (newElementCount == 0)
                            {
                                this.UpdateDisplayedElement(null);
                            }
                            else
                            {
                                // Calculate new element index to display
                                //
                                int newElementIndex = this._displayedElementIndex.Value;
                                int elementIndexOfDeletedRange = this.HeaderVisibility == Visibility.Visible ? e.OldStartingIndex + 1 : e.OldStartingIndex;
                                if (this._displayedElementIndex.Value > e.OldStartingIndex)
                                {
                                    int change = -1 * Math.Min(numberRemoved, (int)this._displayedElementIndex.Value - elementIndexOfDeletedRange);
                                    newElementIndex += change;
                                }

                                // Ensure it's still a valid index
                                //
                                if (newElementIndex >= newElementCount)
                                {
                                    newElementIndex = newElementCount - 1;
                                }

                                this.JumpToElement(newElementIndex);
                            }
                        }
                    } break;
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    {
                        // In these cases, do a full reset
                        //
                        this.InitializeOrReset();
                    } break;
            }
        }

        private static void OnInitiallyDisplayedElementPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            MediaViewer thisMediaViewer = (MediaViewer)dependencyObject;

            if ((DisplayedElementType)eventArgs.NewValue == DisplayedElementType.None)
            {
                throw new ArgumentException("InitiallyDisplayedElement cannot be set to DisplayedElementType.None");
            }
        }

        private static void HeaderTemplateChangedEventHandler(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            MediaViewer mediaViewer = (MediaViewer)dependencyObject;
            DataTemplate newTemplate = (DataTemplate)eventArgs.NewValue;

            if (newTemplate != null)
            {
                mediaViewer._headerTemplateInstance = newTemplate.LoadContent() as FrameworkElement;
            }
            else
            {
                mediaViewer._headerTemplateInstance = null;
            }

            if (mediaViewer._state == MediaViewerState.Initialized)
            {
                mediaViewer.PlaceHeader();
            }
        }

        private static void FooterTemplateChangedEventHandler(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            MediaViewer mediaViewer = (MediaViewer)dependencyObject;
            DataTemplate newTemplate = (DataTemplate)eventArgs.NewValue;

            if (newTemplate != null)
            {
                mediaViewer._footerTemplateInstance = newTemplate.LoadContent() as FrameworkElement;
            }
            else
            {
                mediaViewer._footerTemplateInstance = null;
            }

            if (mediaViewer._state == MediaViewerState.Initialized)
            {
                mediaViewer.PlaceFooter();
            }
        }

        private static void OnHeaderPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            MediaViewer mediaViewer = (MediaViewer)dependencyObject;

            if (mediaViewer._state == MediaViewerState.Initialized)
            {
                mediaViewer.PlaceHeader();
            }
        }

        private static void OnFooterPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            MediaViewer mediaViewer = (MediaViewer)dependencyObject;

            if (mediaViewer._state == MediaViewerState.Initialized)
            {
                mediaViewer.PlaceFooter();
            }
        }

        private static void OnItemTemplatePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs eventArgs)
        {
            MediaViewer mediaViewer = (MediaViewer)dependencyObject;
            mediaViewer.InitializeVirtualizationIfReady();
        }

        #endregion

        #region Virtualization

        /// <summary>
        /// Initializes virtualization if all of the prerequisites are available, so it's safe to call once each is set.
        /// </summary>
        private void InitializeVirtualizationIfReady()
        {
            if ((this._mediaStrip == null) ||
                (this._size == null))
            {
                // We don't have everything we need to initialize, wait for the next call
                return;
            }

            //
            // Initialize the virtualized item pool
            //
            this._virtualizedItemPool = new List<VirtualizedItem>();
            this._mediaStrip.Children.Clear();
            for (int index = 0; index < _virtualizedItemPoolSize; index++)
            {
                VirtualizedItem virtualizedItem = new VirtualizedItem(new Size(this._size.Value.Width, this._size.Value.Height));
                virtualizedItem.DataTemplate = this.ItemTemplate;
                virtualizedItem.ItemZoomed += this.OnItemZoomed;
                virtualizedItem.ItemUnzoomed += this.OnItemUnzoomed;
                this._mediaStrip.Children.Add(virtualizedItem.RootFrameworkElement);
                this._virtualizedItemPool.Add(virtualizedItem);
            }

            if (this._state == MediaViewerState.Uninitialized)
            {
                this._state = MediaViewerState.Initialized;
            }

            this.ResetDisplayedElement();

            this.ResetItemLayout();    
        }

        private void ResetDisplayedElement()
        {
            if ((this.HeaderVisibility == Visibility.Collapsed) &&
                (this.FooterVisibility == Visibility.Collapsed) &&
                ((this.Items == null) || (this.Items.Count == 0)))
            {
                this.ScrollOffset = 0;
                this.UpdateDisplayedElement(null);
            }
            else
            {
                if (this.InitiallyDisplayedElement == InitiallyDisplayedElementType.First)
                {
                    this.JumpToFirstElement();
                }
                else
                {
                    this.JumpToLastElement();
                }
            }
        }

        private void UpdateDisplayedElementPropertiesBasedOnIndex()
        {
            if (this._displayedElementIndex == null)
            {
                this.DisplayedElement = DisplayedElementType.None;
                this.DisplayedItemIndex = -1;
            }
            else
            {
                if ((this._displayedElementIndex == 0) && 
                    (this.HeaderVisibility == Visibility.Visible))
                {
                    this.DisplayedElement = DisplayedElementType.Header;
                    this.DisplayedItemIndex = -1;

                    var handler = this.HeaderDisplayed;
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }
                else if ((this._displayedElementIndex == this.GetElementCount() - 1) &&
                         (this.FooterVisibility == Visibility.Visible))
                {
                    this.DisplayedElement = DisplayedElementType.Footer;
                    this.DisplayedItemIndex = -1;

                    var handler = this.FooterDisplayed;
                    if (handler != null)
                    {
                        handler(this, EventArgs.Empty);
                    }
                }
                else
                {
                    int index = this._displayedElementIndex.Value;
                    if (this.HeaderVisibility == Visibility.Visible)
                    {
                        index--;
                    }

                    System.Diagnostics.Debug.Assert(index < this.Items.Count);

                    this.DisplayedElement = DisplayedElementType.Item;
                    this.DisplayedItemIndex = (int)index;

                    var handler = this.ItemDisplayed;
                    if (handler != null)
                    {
                        handler(this, new ItemDisplayedEventArgs(index));
                    }
                }
            }
        }

        private void PlaceHeader()
        {
            FrameworkElement effectiveHeader = null;

            if (this.Header is FrameworkElement)
            {
                effectiveHeader = this.Header;
            }
            else
            {
                effectiveHeader = this._headerTemplateInstance;
                if ((effectiveHeader != null) &&
                    (this.Header != null))
                {
                    effectiveHeader.DataContext = this.Header;
                }
            }

            // If we currently have a different header on the MediaStrip, remove it
            //
            if ((this._headerOnMediaStrip != null) &&
                (this._headerOnMediaStrip != effectiveHeader))
            {
                this._mediaStrip.Children.Remove(this._headerOnMediaStrip);
                this._headerOnMediaStrip = null;
            }

            if (this.HeaderVisibility == Visibility.Visible)
            {
                if (effectiveHeader != null)
                {
                    effectiveHeader.SetValue(Canvas.TopProperty, 0.0);
                    effectiveHeader.SetValue(Canvas.LeftProperty, 0.0);

                    effectiveHeader.Height = this._size.Value.Height;
                    effectiveHeader.Width = this._size.Value.Width;

                    if (this._headerOnMediaStrip != effectiveHeader)
                    {
                        this._mediaStrip.Children.Add(effectiveHeader);
                        this._headerOnMediaStrip = effectiveHeader;
                    }
                }
            }
            else
            {
                if (this._headerOnMediaStrip != null)
                {
                    this._mediaStrip.Children.Remove(this._headerOnMediaStrip);
                    this._headerOnMediaStrip = null;
                }
            }
        }

        private void PlaceFooter()
        {
            FrameworkElement effectiveFooter = null;

            if (this.Footer is FrameworkElement)
            {
                effectiveFooter = this.Footer;
            }
            else
            {
                effectiveFooter = this._footerTemplateInstance;
                if ((effectiveFooter != null) &&
                    (this.Footer != null))
                {
                    effectiveFooter.DataContext = this.Footer;
                }
            }

            // If we currently have a different Footer on the MediaStrip, remove it
            //
            if ((this._footerOnMediaStrip != null) &&
                (this._footerOnMediaStrip != effectiveFooter))
            {
                this._mediaStrip.Children.Remove(this._footerOnMediaStrip);
                this._footerOnMediaStrip = null;
            }

            if (this.FooterVisibility == Visibility.Visible)
            {
                if (effectiveFooter != null)
                {
                    effectiveFooter.SetValue(Canvas.TopProperty, 0.0);
                    effectiveFooter.SetValue(Canvas.LeftProperty, this._mediaStrip.Width - this._size.Value.Width);

                    effectiveFooter.Height = this._size.Value.Height;
                    effectiveFooter.Width = this._size.Value.Width;

                    if (this._footerOnMediaStrip != effectiveFooter)
                    {
                        this._mediaStrip.Children.Add(effectiveFooter);
                        this._footerOnMediaStrip = effectiveFooter;
                    }
                }
            }
            else
            {
                if (this._footerOnMediaStrip != null)
                {
                    this._mediaStrip.Children.Remove(this._footerOnMediaStrip);
                    this._footerOnMediaStrip = null;
                }
            }
        }

        private void ResetItemLayout()
        {
            if (this._state == MediaViewerState.Uninitialized)
            {
                // Not ready to do this yet
                return;
            }

            this.ResetMediaStripGeometry();

            this.PlaceHeader();
            this.PlaceFooter();
            this.UpdateVirtualizedItemPositions();
        }

        private void UpdateVirtualizedItemSizes()
        {
            foreach (VirtualizedItem virtualizedItem in this._virtualizedItemPool)
            {
                virtualizedItem.Size = this._size.Value;
            }
        }
        
        private void UpdateVirtualizedItemPositions()
        {
            int? itemIndexToCenterOn = null;

            switch (this.DisplayedElement)
            {
                case DisplayedElementType.Header:
                    {
                        if ((this.Items != null) &&
                            (this.Items.Count > 0))
                        {
                            itemIndexToCenterOn = 0;
                        }
                        else
                        {
                            itemIndexToCenterOn = null;
                        }
                    } break;
                case DisplayedElementType.Item:
                    {
                        itemIndexToCenterOn = this.DisplayedItemIndex;
                    } break;
                case DisplayedElementType.Footer:
                    {
                        if ((this.Items != null) &&
                            (this.Items.Count > 0))
                        {
                            itemIndexToCenterOn = this.Items.Count - 1;
                        }
                        else
                        {
                            itemIndexToCenterOn = null;
                        }
                    } break;
            }

            if (itemIndexToCenterOn == null)
            {
                // There are valid cases where there are no items to virtualize in.  
                return;
            }

            // Calculate the range of indexes we want the virtualized items to represent
            int itemsToEitherSide = _virtualizedItemPoolSize / 2;
            int firstIndex = Math.Max(0, itemIndexToCenterOn.Value - itemsToEitherSide);
            int lastIndex = Math.Min(this.Items.Count - 1, itemIndexToCenterOn.Value + itemsToEitherSide);

            for (int index = firstIndex; index <= lastIndex; index++)
            {
                bool isAlreadyVirtualizedIn = false;
                double correctPosition = this.CalculateItemOffset(index);
                VirtualizedItem repurposeCandidate = null;
                
                // Check to see if this item index is already virtualized in
                foreach (VirtualizedItem virtualizedItem in this._virtualizedItemPool)
                {
                    if ((this.DisplayedItemIndex != -1) &&
                        (virtualizedItem.RepresentingItemIndex == this.DisplayedItemIndex))
                    {
                        this._displayedVirtualizedItem = virtualizedItem;
                    }

                    if (virtualizedItem.RepresentingItemIndex == index)
                    {
                        isAlreadyVirtualizedIn = true;

                        // Put it in the correct position if it isn't already there
                        if ((double)virtualizedItem.RootFrameworkElement.GetValue(Canvas.LeftProperty) != correctPosition)
                        {
                            virtualizedItem.RootFrameworkElement.SetValue(Canvas.LeftProperty, correctPosition);
                        }

                        break;
                    }
                    else
                    {
                        if ((repurposeCandidate == null) ||
                            (virtualizedItem.RepresentingItemIndex == null))
                        {
                            repurposeCandidate = virtualizedItem;
                        }
                        else if ((repurposeCandidate != null) &&
                                 (repurposeCandidate.RepresentingItemIndex != null))
                        {
                            // Look for the VirtualizedItem that is furthest from our itemIndexToCenterOn

                            int existingDistance = Math.Abs((int)repurposeCandidate.RepresentingItemIndex.Value - (int)itemIndexToCenterOn);
                            int thisDistance = Math.Abs((int)virtualizedItem.RepresentingItemIndex.Value - (int)itemIndexToCenterOn);

                            if (thisDistance > existingDistance)
                            {
                                repurposeCandidate = virtualizedItem;
                            }
                        }
                    }
                }

                if (!isAlreadyVirtualizedIn)
                {
                    // Repurpose the repurposeCandidate to represent this item
                    repurposeCandidate.DataContext = this.Items[(int)index];
                    repurposeCandidate.RepresentingItemIndex = index;
                    repurposeCandidate.RootFrameworkElement.SetValue(Canvas.LeftProperty, correctPosition);
                    repurposeCandidate.RootFrameworkElement.Visibility = System.Windows.Visibility.Visible;


                    if ((this.DisplayedItemIndex != -1) &&
                        (repurposeCandidate.RepresentingItemIndex == this.DisplayedItemIndex))
                    {
                        this._displayedVirtualizedItem = repurposeCandidate;
                    }
                }
            }
        }

        private void ResetMediaStripGeometry()
        {
            this._mediaStrip.Height = this._size.Value.Height;
            
            int elementCount = this.GetElementCount();
            this._mediaStrip.Width = (this._size.Value.Width * elementCount) + (_itemGutter * (elementCount - 1));

            this._dragState.MinDraggingBoundary = _maxDraggingSquishDistance;
            this._dragState.MaxDraggingBoundary = -1 * (this._mediaStrip.Width - this._size.Value.Width + _maxDraggingSquishDistance);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Displays the header without animating to it.
        /// </summary>
        public bool JumpToHeader()
        {
            if (this.HeaderVisibility == Visibility.Collapsed)
            {
                return false;
            }

            this.JumpToFirstElement();
            return true;
        }

        /// <summary>
        /// Displays the footer without animating to it.
        /// </summary>
        public bool JumpToFooter()
        {
            if (this.FooterVisibility == Visibility.Collapsed)
            {
                return false;
            }

            this.JumpToLastElement();
            return true;
        }

        /// <summary>
        /// Displays an item without animating to it.
        /// </summary>
        public bool JumpToItem(int itemIndex)
        {
            if ((this.Items == null) ||
                (itemIndex >= this.Items.Count) ||
                (itemIndex < 0))
            {
                return false;
            }

            this.JumpToElement(this.HeaderVisibility == Visibility.Visible ? itemIndex + 1 : itemIndex);
            return true;
        }

        /// <summary>
        /// Animates to the element to the left of the currently displayed element.
        /// </summary>
        /// <returns></returns>
        public bool ScrollLeftOneElement()
        {
            if ((this._displayedElementIndex == null) ||  
                (this._displayedElementIndex == 0))
            {
                return false;
            }

            this.AnimateToElement(this._displayedElementIndex.Value - 1, new TimeSpan(0, 0, 0, 0, (int)_flickMaxOutputMilliseconds));

            return true;
        }

        /// <summary>
        /// Finds an object in the header by name
        /// </summary>
        public object FindNameInHeader(string name)
        {
            if (this._headerOnMediaStrip == null)
            {
                return null;
            }
            return this._headerOnMediaStrip.FindName(name);
        }

        /// <summary>
        /// Finds an object in the footer by name
        /// </summary>
        public object FindNameInFooter(string name)
        {
            if (this._footerOnMediaStrip == null)
            {
                return null;
            }
            return this._footerOnMediaStrip.FindName(name);
        }

        #endregion

        #region Control events

        private void OnMediaViewerSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this._size = e.NewSize;

            if (this._state == MediaViewerState.Uninitialized)
            {
                this.InitializeVirtualizationIfReady();
            }
            else
            {
                this.UpdateVirtualizedItemSizes();
                this.ResetItemLayout();
                if (this._displayedElementIndex != null)
                {
                    this.ScrollOffset = -1 * this.CalculateElementOffset(this._displayedElementIndex.Value);
                }
                else
                {
                    this.ScrollOffset = 0;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._mediaStrip = (Canvas)this.GetTemplateChild("MediaStrip");
            this._mediaStripCompositeTransform = (CompositeTransform)this.GetTemplateChild("MediaStripCompositeTransform");

            this.InitializeVirtualizationIfReady();
        }

        #endregion

        #region User Interaction Events

        protected override void OnDoubleTap(GestureEventArgs e)
        {
            base.OnDoubleTap(e);
            if (this.DisplayedElement == DisplayedElementType.Item)
            {
                this._displayedVirtualizedItem.DoubleTapped();
            }
        }

        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            base.OnManipulationStarted(e);

            // If we were in the middle of an inertia animation, end it now and jump to its final position
            //
            if (this._state == MediaViewerState.InertiaAnimating)
            {
                this.CompleteDragInertiaAnimation();
            }
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            base.OnManipulationDelta(e);

            if (e.PinchManipulation == null)
            {
                if (!this.IsZoomedInToItem() &&
                    (this._state == MediaViewerState.Initialized) &&
                    (this.DragEnabled) &&
                    (this.GetElementCount() > 0))
                {
                    this._state = MediaViewerState.Dragging;
                    this.DragStartedEventHandler();
                }

                if ((this._state == MediaViewerState.Dragging) ||
                    (this._state == MediaViewerState.DraggingAndSquishing))
                {
                    this.DragDeltaEventHandler(e.DeltaManipulation.Translation.X, e.DeltaManipulation.Translation.Y);
                }
            }
        }

        protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e)
        {
            base.OnManipulationCompleted(e);

            if ((this._state == MediaViewerState.Dragging) ||
                (this._state == MediaViewerState.DraggingAndSquishing))
            {
                // NOTE: If the drag was fast enough that we didn't get any drag deltas reported,
                //       then these totals are the same as the first and only delta, so we will
                //       process them accordingly
                //
                if (this._dragState.GotDragDelta == false)
                {
                    this.ProcessDragDelta(e.TotalManipulation.Translation.X, e.TotalManipulation.Translation.Y);
                }

                this.DragCompletedEventHandler();
            }
        }

        private void OnItemZoomed(int? representingItemIndex)
        {
            if ((this._displayedElementIndex != null) &&
                (this._displayedElementIndex == representingItemIndex))
            {
                var handler = this.ItemZoomed;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }

        private void OnItemUnzoomed(int? representingItemIndex)
        {
            if ((this._displayedElementIndex != null) &&
                (this._displayedElementIndex == representingItemIndex))
            {
                var handler = this.ItemUnzoomed;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }
        }
        
        #endregion

        #region Drag handling
        
        private void DragStartedEventHandler()
        {
            //Tracing.Trace("DragStartedEventHandler() MediaStripOffset = " + offsetHelper.MediaStripOffset);
            int elementCount = this.GetElementCount();

            this._state = MediaViewerState.Dragging;
            this._dragState.LastDragUpdateTime = DateTime.Now;
            this._dragState.DragStartingMediaStripOffset = this.ScrollOffset;
            this._dragState.NetDragDistanceSincleLastDragStagnation = 0.0;
            this._dragState.IsDraggingFirstElement = this._displayedElementIndex == 0;
            this._dragState.IsDraggingLastElement = this._displayedElementIndex == elementCount - 1;
            this._dragState.GotDragDelta = false;
        }

        private void DragDeltaEventHandler(double horizontalChange, double verticalChange)
        {
            this._dragState.GotDragDelta = true;

            this.ProcessDragDelta(horizontalChange, verticalChange);
        }

        private void DragCompletedEventHandler()
        {
            switch (this._state)
            {
                case MediaViewerState.Dragging:
                    {
                        this.StartDragInertiaAnimation();
                    } break;
                case MediaViewerState.DraggingAndSquishing:
                    {
                        this.StartUndoSquishAnimation();
                    } break;
                default:
                    {
                        // Ignore
                    } break;
            }
        }

        private void ProcessDragDelta(double horizontalChange, double verticalChange)
        {
            // Do time calculations necessary to determine if the drag has stagnated or not.
            // This is important for inertia calculations.
            //
            DateTime currentTime = DateTime.Now;
            double millisecondsSinceLastDragUpdate = ((TimeSpan)(currentTime - this._dragState.LastDragUpdateTime)).TotalMilliseconds;
            this._dragState.LastDragUpdateTime = currentTime;
            if (millisecondsSinceLastDragUpdate > _dragStagnationTimeThreshold)
            {
                this._dragState.NetDragDistanceSincleLastDragStagnation = 0.0;
            }

            // Calculate new translation value
            //
            double newTranslation = 0;
            this._dragState.LastDragDistanceDelta = horizontalChange;
            newTranslation = this.ScrollOffset + horizontalChange;

            // Update NetDragDistanceSincleLastDragStagnation for stagnation detection
            //
            this._dragState.NetDragDistanceSincleLastDragStagnation += horizontalChange;

            // Enforce dragging limits
            //
            newTranslation = Math.Min(newTranslation, this._dragState.MinDraggingBoundary);
            newTranslation = Math.Max(newTranslation, this._dragState.MaxDraggingBoundary);

            // Possibly do squish animation if we're dragging the first or last element
            //
            if ((this._dragState.IsDraggingFirstElement) || (this._dragState.IsDraggingLastElement))
            {
                this.HandleSquishingWhileDragging(newTranslation);
            }

            // Apply the new translation
            //
            this.ScrollOffset = newTranslation;
        }

        private void ConstructDragInertiaAnimation(double animationEndingValue, TimeSpan animationDuration)
        {
            this._dragInertiaAnimation = new Storyboard();

            this._dragInertiaAnimationTranslation = new DoubleAnimation();
            Storyboard.SetTarget(this._dragInertiaAnimationTranslation, this._mediaStripCompositeTransform);
            Storyboard.SetTargetProperty(this._dragInertiaAnimationTranslation, new PropertyPath(CompositeTransform.TranslateXProperty));

            QuadraticEase easingFunction = new QuadraticEase();
            easingFunction.EasingMode = EasingMode.EaseOut;

            this._dragInertiaAnimationTranslation.From = this.ScrollOffset;
            this._dragInertiaAnimationTranslation.To = animationEndingValue;
            this._dragInertiaAnimationTranslation.Duration = animationDuration;
            this._dragInertiaAnimationTranslation.EasingFunction = easingFunction;

            this._dragInertiaAnimation.Children.Add(this._dragInertiaAnimationTranslation);
            this._dragInertiaAnimation.Completed += this.DragInertiaAnimationComplete;
            this._dragInertiaAnimation.FillBehavior = FillBehavior.HoldEnd;
        }

        private int CalculateDragInertiaAnimationEndingValue()
        {
            bool userStoppedDrag = (Math.Abs(this._dragState.NetDragDistanceSincleLastDragStagnation) <= _dragStagnationDistanceThreshold);
            int elementIndexDelta = 0;

            if (userStoppedDrag == false)
            {
                elementIndexDelta = -1 * Math.Sign(this._dragState.LastDragDistanceDelta);

                // Ensure we don't try to drag beyond either end of the list of elements
                //
                if ((this._displayedElementIndex == 0) &&
                    (elementIndexDelta == -1))
                {
                    elementIndexDelta = 0;
                }
                else if ((this._displayedElementIndex == this.GetElementCount() - 1) &&
                         (elementIndexDelta == 1))
                {
                    elementIndexDelta = 0;
                }
            }

            return elementIndexDelta;
        }

        private TimeSpan CalculateDragInertiaAnimationDuration(TimeSpan lastDragTimeDelta)
        {
            double actualVelocity = Math.Abs(this._dragState.LastDragDistanceDelta / lastDragTimeDelta.TotalMilliseconds);
            actualVelocity = Math.Min(_flickMaxInputVelocity, actualVelocity);
            actualVelocity = Math.Max(_flickMinInputVelocity, actualVelocity);
            double velocityPercentage = (actualVelocity - _flickMinInputVelocity) / (_flickMaxInputVelocity - _flickMinInputVelocity);

            int milliSeconds = (int)((_flickMaxOutputMilliseconds - _flickMinOutputMilliseconds) * (1 - velocityPercentage) + _flickMinOutputMilliseconds);

            milliSeconds = Math.Min((int)_flickMaxOutputMilliseconds, milliSeconds);
            milliSeconds = Math.Max((int)_flickMinOutputMilliseconds, milliSeconds);

            return new TimeSpan(0, 0, 0, 0, milliSeconds);
        }

        private void StartDragInertiaAnimation()
        {
            TimeSpan lastDragTimeDelta = DateTime.Now - this._dragState.LastDragUpdateTime;

            // Build animation to finish the drag
            //
            int elementIndexDelta = this.CalculateDragInertiaAnimationEndingValue();
            TimeSpan animationDuration = this.CalculateDragInertiaAnimationDuration(lastDragTimeDelta);

            this.AnimateToElement(this._displayedElementIndex.Value + elementIndexDelta, animationDuration);

            this._state = MediaViewerState.InertiaAnimating;
        }

        private void AnimateToElement(int elementIndex, TimeSpan animationDuration)
        {
            double animationEndingValue = -1 * this.CalculateElementOffset(elementIndex);

            this.ConstructDragInertiaAnimation(animationEndingValue, animationDuration);
            this._state = MediaViewerState.InertiaAnimating;
            this._dragInertiaAnimation.Begin();

            this._dragState.NewDisplayedElementIndex = elementIndex;
        }

        private void DragInertiaAnimationComplete(object sender, EventArgs e)
        {
            this.CompleteDragInertiaAnimation();
        }

        private void CompleteDragInertiaAnimation()
        {
            if (this._dragInertiaAnimation != null)
            {
                if (this._state == MediaViewerState.InertiaAnimating)
                {
                    this._state = MediaViewerState.Initialized;
                }

                this.ScrollOffset = this._dragInertiaAnimationTranslation.To.Value;

                this._dragInertiaAnimation.Stop();
                this._dragInertiaAnimation = null;
                this._dragInertiaAnimationTranslation = null;

                this.UpdateDisplayedElement(this._dragState.NewDisplayedElementIndex);
            }
        }

        private void HandleSquishingWhileDragging(double newTranslation)
        {
            double translationOfLastItem = -1 * this.CalculateElementOffset(this.GetElementCount() - 1);
            double squishDistance = 0;

            if (newTranslation > 0)
            {
                // We're squishing the first item
                //
                squishDistance = newTranslation;
                this._dragState.UnsquishTranslationAnimationTarget = 0;
                this._mediaStrip.RenderTransformOrigin = new Point(0, 0);
            }
            else if (newTranslation < translationOfLastItem)
            {
                // We're squishing the last item
                //
                squishDistance = translationOfLastItem - newTranslation;
                this._dragState.UnsquishTranslationAnimationTarget = translationOfLastItem;
                this._mediaStrip.RenderTransformOrigin = new Point(1, 0);
            }

            double squishScale = 1.0 - (squishDistance / _maxDraggingSquishDistance) * (1 - _minDraggingSquishScale);

            // Apply the squish
            //
            this._mediaStripCompositeTransform.ScaleX = squishScale;

            // Update our state
            //
            this._state = squishScale == 1.0 ? MediaViewerState.Dragging : MediaViewerState.DraggingAndSquishing;
        }

        private void StartUndoSquishAnimation()
        {
            // Build animation to undo squish
            //
            this._unsquishAnimation = new Storyboard();
            DoubleAnimation scaleAnimation = new DoubleAnimation();
            this._unsquishAnimationTranslation = new DoubleAnimation();
            Storyboard.SetTarget(scaleAnimation, this._mediaStripCompositeTransform);
            Storyboard.SetTarget(this._unsquishAnimationTranslation, this._mediaStripCompositeTransform);

            Storyboard.SetTargetProperty(scaleAnimation, new PropertyPath(CompositeTransform.ScaleXProperty));
            Storyboard.SetTargetProperty(this._unsquishAnimationTranslation, new PropertyPath(CompositeTransform.TranslateXProperty));
            scaleAnimation.From = this._mediaStripCompositeTransform.ScaleX;
            this._unsquishAnimationTranslation.From = this._mediaStripCompositeTransform.TranslateX;

            scaleAnimation.To = 1.0;
            this._unsquishAnimationTranslation.To = this._dragState.UnsquishTranslationAnimationTarget;
            scaleAnimation.Duration = new TimeSpan(0, 0, 0, 0, _unsquishAnimationMilliseconds);
            this._unsquishAnimationTranslation.Duration = scaleAnimation.Duration;

            this._unsquishAnimation.Children.Add(scaleAnimation);
            this._unsquishAnimation.Children.Add(this._unsquishAnimationTranslation);
            this._unsquishAnimation.FillBehavior = FillBehavior.Stop;
            this._unsquishAnimation.Completed += this.UnsquishAnimationComplete;
            this._state = MediaViewerState.UnsquishAnimating;
            this._unsquishAnimation.Begin();

            // Go ahead and set the values we're animating to their final values so when the storyboard ends, these will take effect
            //
            this._mediaStripCompositeTransform.ScaleX = scaleAnimation.To.Value;
            this._mediaStripCompositeTransform.TranslateX = this._unsquishAnimationTranslation.To.Value;
        }

        private void UnsquishAnimationComplete(object sender, EventArgs e)
        {
            if (this._state == MediaViewerState.UnsquishAnimating)
            {
                this._state = MediaViewerState.Initialized;
            }

            this.ScrollOffset = this._unsquishAnimationTranslation.To.Value;

            this._unsquishAnimation.Stop();
            this._unsquishAnimation = null;
            this._unsquishAnimationTranslation = null;
        }

        #endregion

        #region Helper methods

        private int GetElementCount()
        {
            int elementCount = 0;
            if (this.HeaderVisibility == Visibility.Visible)
            {
                elementCount++;
            }
            if (this.Items != null)
            {
                elementCount += this.Items.Count;
            }
            if (this.FooterVisibility == Visibility.Visible)
            {
                elementCount++;
            }

            return elementCount;
        }

        private void UpdateDisplayedElement(int? newElementIndex)
        {
            this._displayedElementIndex = newElementIndex;
            this.UpdateDisplayedElementPropertiesBasedOnIndex();
            this.UpdateVirtualizedItemPositions();
        }

        private double RoundOffsetDownToElementStart(double offset)
        {
            int index = (int)(offset / (this._size.Value.Width + _itemGutter));

            return index * (this._size.Value.Width + _itemGutter);
        }

        private double CalculateElementOffset(int elementIndex)
        {
            return elementIndex * (this._size.Value.Width + _itemGutter);
        }

        private double CalculateItemOffset(int itemIndex)
        {
            double position = 0;

            if (this.HeaderVisibility == Visibility.Visible)
            {
                position += this._size.Value.Width + _itemGutter;
            }

            position += itemIndex * (this._size.Value.Width + _itemGutter);

            return position;
        }

        private bool IsZoomedInToItem()
        {
            return ((this._displayedVirtualizedItem != null) &&
                    (this._displayedVirtualizedItem.IsZoomedIn));
        }

        private void JumpToFirstElement()
        {
            this.JumpToElement(0);
        }

        private void JumpToLastElement()
        {
            int elementCount = this.GetElementCount();
            if (elementCount > 0)
            {
                this.JumpToElement(elementCount - 1);
            }
        }

        private void JumpToElement(int elementIndex)
        {
            // If we are zoomed into an item, unzoom it before jumping
            //
            if ((this.DisplayedElement == DisplayedElementType.Item) &&
                (this._displayedVirtualizedItem.IsZoomedIn))
            {
                this._displayedVirtualizedItem.ZoomAllTheWayOut();
            }

            this.ScrollOffset = -1 * this.CalculateElementOffset(elementIndex);
            this.UpdateDisplayedElement(elementIndex);
        }

        private void InitializeOrReset()
        {
            if (this._state == MediaViewerState.Uninitialized)
            {
                this.InitializeVirtualizationIfReady();
            }
            else
            {
                // Clear out all of the VirtualizedItem assignments
                //
                foreach (VirtualizedItem virtualizedItem in this._virtualizedItemPool)
                {
                    virtualizedItem.DataContext = null;
                    virtualizedItem.RepresentingItemIndex = null;
                }

                this.ResetDisplayedElement();
                this.ResetItemLayout();
            }
        }

        #endregion
    }
}
