// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="VirtualizedItem.cs">
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
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Represents a virtualized item in a MediaViewer.  It knows which
    /// index it represents in the MediaViewer.Items collection, it holds
    /// the FrameworkElement reference for the ItemTemplate instance, and
    /// it handles zooming the item.
    /// </summary>
    /// <remarks>
    /// This code is taken from the Code Samples for Windows Phone (http://code.msdn.microsoft.com/wpapps).
    /// </remarks>
    internal class VirtualizedItem
    {
        private const double maxTextureSideLength = 2048;
      
        private FrameworkElement _zoomableContent;
        private ScaleTransform _contentTransform;
        private Point _lastOrigin;

        /// <summary>
        /// Stores the current zoom ratio, where 1.0 is zoomed all the way out to fit within the control.
        /// NOTE: this is not updated during a pinch operation, but serves as the "settled" zoom ratio
        /// that is only updated after a pinch is complete.
        /// </summary>
        private double _currentZoomRatio = 1.0;
        /// <summary>
        /// The temporary zoom ratio used during a pinch operation.
        /// </summary>
        private double _zoomRatioDuringPinch;
        
        private FrameworkElement _rootFrameworkElement;
        private DataTemplate _dataTemplate;
        private Size _size;
        private ViewportControl _viewport;
        private Canvas _canvas;
        /// <summary>
        /// Since we set a bounding box on the content and let it fill that box as much as possible, the actual size of the content is likely smaller.
        /// This stores the actual size of the content that has been rendered.
        /// </summary>
        private Size _actualContentSize = new Size(0,0);

        private bool _pinchInProgress;
        private bool _centerAtNextOpportunity = false;
        private Point _pinchMidpointInControlCoordinates;
        private Point _pinchMidpointInPercentOfContent;

        /// <summary>
        /// The index in the MediaViewer.Items collection that this instance
        /// represents
        /// </summary>
        public int? RepresentingItemIndex { get; set; }

        /// <summary>
        /// Raised when an item is zoomed in.
        /// </summary>
        public event Action<int?> ItemZoomed;
        /// <summary>
        /// Raised when an item is zoomed out to its original size.
        /// </summary>
        public event Action<int?> ItemUnzoomed;

        /// <summary>
        /// Creates a VirtualizedItem of a particular presentation Size.
        /// </summary>
        /// <param name="size">Initial Size for the item.</param>
        public VirtualizedItem(Size size)
        {
            this._size = size;
        }

        /// <summary>
        /// The DataTemplate that should be instantiated to represent an item.
        /// </summary>
        public DataTemplate DataTemplate
        {
            get
            {
                return this._dataTemplate;
            }
            set
            {
                this._dataTemplate = value;

                this._rootFrameworkElement = (FrameworkElement)this._dataTemplate.LoadContent();
                this._rootFrameworkElement.Visibility = Visibility.Collapsed;
                this._rootFrameworkElement.Height = this._size.Height;
                this._rootFrameworkElement.Width = this._size.Width;

                this._zoomableContent = (FrameworkElement)this._rootFrameworkElement.FindName("ZoomableContent");
                this._contentTransform = (ScaleTransform)this._rootFrameworkElement.FindName("ZoomableContentTransform");
                this._viewport = (ViewportControl)this._rootFrameworkElement.FindName("Viewport");
                this._canvas = (Canvas)this._rootFrameworkElement.FindName("Canvas");

                this._zoomableContent.ManipulationStarted += this.OnManipulationStarted;
                this._zoomableContent.ManipulationDelta += this.OnManipulationDelta;
                this._zoomableContent.ManipulationCompleted += this.OnManipulationCompleted;
                this._zoomableContent.LayoutUpdated += this.OnZoomableContentLayoutUpdated;
            }
        }

        /// <summary>
        /// The root FrameworkElement of the item.  Exposed so the MediaViewer
        /// can position it on the parent Canvas.
        /// </summary>
        public FrameworkElement RootFrameworkElement
        {
            get
            {
                return this._rootFrameworkElement;
            }
        }

        /// <summary>
        /// The Item to be virtualized is assigned to this property.
        /// </summary>
        public object DataContext
        {
            get
            {
                return this._rootFrameworkElement.DataContext;
            }
            set
            {
                this._rootFrameworkElement.DataContext = value;
                this.ZoomAllTheWayOut();
            }
        }

        /// <summary>
        /// The render Size available for this virtualized item.
        /// </summary>
        public Size Size
        {
            get
            {
                return this._size;
            }
            set
            {
                this._size = value;
                this._rootFrameworkElement.Height = this._size.Height;
                this._rootFrameworkElement.Width = this._size.Width;
                this.ZoomAllTheWayOut();
            }
        }

        /// <summary>
        /// Returns true if the item is zoomed in.
        /// </summary>
        public bool IsZoomedIn
        {
            get
            {
                return ((this._currentZoomRatio > 1.0) ||
                        (this._pinchInProgress));
            }
        }

        /// <summary>
        /// Processes a double tap performed on the virtualized item.
        /// </summary>
        public void DoubleTapped()
        {
            if (this.IsZoomedIn)
            {
                this.ZoomAllTheWayOut();
            }
            else
            {
                this.ZoomInToDefaultLevel();
            }
        }

        /// <summary>
        /// Zooms out to the original Size.
        /// </summary>
        public void ZoomAllTheWayOut()
        {
            this.ItemUnzoomed(this.RepresentingItemIndex);
            this._currentZoomRatio = 1.0;
            this._centerAtNextOpportunity = true;
            this.ResizeContent();
        }

        private void ZoomInToDefaultLevel()
        {
            this.ItemZoomed(this.RepresentingItemIndex);
            this._currentZoomRatio = 2.0;
            this._centerAtNextOpportunity = true;
            this.ResizeContent();
        }

        private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            // Clear the pinchInProgress flag since we don't know yet if this is a pinch or not
            //
            this._pinchInProgress = false;
        }

        private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (e.PinchManipulation != null)
            {
                e.Handled = true;

                // If this is the first delta of this pinch, calculate the starting position of the pinch
                //
                if (!this._pinchInProgress)
                {
                    this._pinchInProgress = true;
                    this.ItemZoomed(this.RepresentingItemIndex);

                    Point center = e.PinchManipulation.Original.Center;
                    this._pinchMidpointInPercentOfContent = new Point(center.X / this._actualContentSize.Width, center.Y / this._actualContentSize.Height);

                    var xform = this._zoomableContent.TransformToVisual(this._rootFrameworkElement);
                    this._pinchMidpointInControlCoordinates = xform.Transform(center);

                    this._lastOrigin = new Point(-1, -1);
                }

                this._zoomRatioDuringPinch = this.ClampZoomRatioToMinMax(this._currentZoomRatio * e.PinchManipulation.CumulativeScale);
                this.ScaleContentDuringPinch();
            }
            else
            {
                if (this._pinchInProgress)
                {
                    // This happens when pinching then lifting a finger - let's commit the pinch as if it were the end of the manipulation
                    //
                    this.FinishPinch();
                }
            }
        }

        private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (this._pinchInProgress)
            {
                e.Handled = true;

                this.FinishPinch();
            }
        }

        private void FinishPinch()
        {
            this._pinchInProgress = false;
            this._currentZoomRatio = this._zoomRatioDuringPinch;

            if (this._currentZoomRatio == 1.0)
            {
                this.ItemUnzoomed(this.RepresentingItemIndex);
            }
            this.ResizeContent();
        }

        /// <summary>
        /// Resizes the content itself into the "settled" zoom ratio (currentZoomRatio).  
        /// This is not used during a pinch operation.
        /// </summary>
        private void ResizeContent()
        {
            double newMaxWidth = Math.Round(this._size.Width * this._currentZoomRatio);
            double newMaxHeight = Math.Round(this._size.Height * this._currentZoomRatio);

            this._canvas.Width = newMaxWidth;
            this._canvas.Height = newMaxHeight;

            // Give the content a bounding box to fit within.
            //
            this._zoomableContent.MaxWidth = newMaxWidth;
            this._zoomableContent.MaxHeight = newMaxHeight;

            // Undo any scaling
            //
            this._contentTransform.ScaleX = 1.0;
            this._contentTransform.ScaleY = 1.0;
        }

        /// <summary>
        /// Used during a pinch operation to scale the content for a fast pinch effect.  This does not actually change
        /// the layout of the content, so this can be done very quickly.  When the pinch completes, ResizeContent() will
        /// update the content's layout with the final zoom ratio.
        /// </summary>
        private void ScaleContentDuringPinch()
        {
            double newMaxWidth = Math.Round(this._size.Width * this._zoomRatioDuringPinch);
            double newMaxHeight = Math.Round(this._size.Height * this._zoomRatioDuringPinch);

            double scaler = newMaxWidth / this._zoomableContent.MaxWidth;

            double newWidth = this._actualContentSize.Width * scaler;
            double newHeight = this._actualContentSize.Height * scaler;

            this._contentTransform.ScaleX = scaler;
            this._contentTransform.ScaleY = scaler;

            this._viewport.Bounds = new Rect(0, 0, newWidth, newHeight);

            // Pan the Viewport such that the point on the content where the pinch started doesn't move on the screen
            //
            Point pinchMidpointOnScaledContent = new Point(
                newWidth * this._pinchMidpointInPercentOfContent.X, 
                newHeight * this._pinchMidpointInPercentOfContent.Y);

            Point origin = new Point(
                (int)(pinchMidpointOnScaledContent.X - this._pinchMidpointInControlCoordinates.X),
                (int)(pinchMidpointOnScaledContent.Y - this._pinchMidpointInControlCoordinates.Y));

            if (origin != this._lastOrigin)
            {
                this._viewport.SetViewportOrigin(origin);
                this._lastOrigin = origin;
            }
        }

        /// <summary>
        /// When the ZoomableContent calculates a new DesiredSize, we need to update our Viewport bounds
        /// and possibly center the content now that the DesiredSize is known.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnZoomableContentLayoutUpdated(object sender, EventArgs e)
        {
            if ((this._zoomableContent.DesiredSize.Height != this._actualContentSize.Height) ||
                (this._zoomableContent.DesiredSize.Width != this._actualContentSize.Width))
            {
                // If the desired size has changed, record the new desired size and update the viewport bounds
                //
                this._actualContentSize = this._zoomableContent.DesiredSize;
                this._viewport.Bounds = new Rect(0, 0, this._actualContentSize.Width, this._actualContentSize.Height);

                // Center it if requested
                //
                if (this._centerAtNextOpportunity)
                {
                    this._viewport.SetViewportOrigin(
                        new Point(
                            Math.Round((this._actualContentSize.Width - this._size.Width) / 2),
                            Math.Round((this._actualContentSize.Height - this._size.Height) / 2)
                            ));
                    this._centerAtNextOpportunity = false;
                }
            }
        }

        private double ClampZoomRatioToMinMax(double ratio)
        {
            // Ensure that we never zoom in such that the content has a side > 2048 pixels (the texture size limit)
            //
            double maxZoomRatio = Math.Min(
                maxTextureSideLength / this._size.Width,
                maxTextureSideLength / this._size.Height);

            return Math.Min(maxZoomRatio, Math.Max(1.0, ratio));
        }
    }
}
