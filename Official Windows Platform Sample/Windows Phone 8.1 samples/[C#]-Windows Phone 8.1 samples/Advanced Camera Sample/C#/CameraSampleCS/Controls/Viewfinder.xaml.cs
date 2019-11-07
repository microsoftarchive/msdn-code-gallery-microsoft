// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="Viewfinder.xaml.cs">
//   Copyright (c) 2014 Microsoft Corporation. All rights reserved.
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

namespace CameraSampleCS.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using CameraSampleCS.Helpers;
    using Microsoft.Devices;
    using Microsoft.Phone.Controls;

    /// <summary>
    /// This component displays camera video preview and handles user actions.
    /// </summary>
    public partial class Viewfinder
    {
        #region Fields

        /// <summary>
        /// The <see cref="PreviewHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviewHeightProperty = DependencyProperty.Register("PreviewHeight", typeof(double), typeof(Viewfinder), new PropertyMetadata(800.0));

        /// <summary>
        /// The <see cref="PreviewWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviewWidthProperty = DependencyProperty.Register("PreviewWidth", typeof(double), typeof(Viewfinder), new PropertyMetadata(480.0));

        /// <summary>
        /// The <see cref="TapBoundary"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TapBoundaryProperty = DependencyProperty.Register("TapBoundary", typeof(int), typeof(Viewfinder), new PropertyMetadata(25));

        /// <summary>
        /// The <see cref="PreviewBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviewBrushProperty = DependencyProperty.Register("PreviewBrush", typeof(Brush), typeof(Viewfinder), null);

        /// <summary>
        /// The <see cref="ReviewImageBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ReviewImageBrushProperty = DependencyProperty.Register("ReviewImageBrush", typeof(Brush), typeof(Viewfinder), null);

        /// <summary>
        /// Current viewfinder orientation.
        /// </summary>
        private PageOrientation orientation = PageOrientation.None;

        /// <summary>
        /// Current camera type the live preview is displayed for.
        /// </summary>
        private CameraType cameraType = CameraType.Primary;

        #endregion // Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Viewfinder"/> class.
        /// </summary>
        public Viewfinder()
        {
            this.InitializeComponent();
            this.DataContext = this;
        }

        #endregion // Constructor

        #region Events

        /// <summary>
        /// Occurs when user taps the preview.
        /// </summary>
        public event EventHandler<PreviewTapLocationEventArgs> PreviewTap;

        /// <summary>
        /// Occurs when the left arrow button is clicked.
        /// </summary>
        public event EventHandler LeftArrowButtonTap;

        /// <summary>
        /// Occurs when review slide animation finishes.
        /// </summary>
        public event EventHandler<EventArgs> SlideReviewComplete;

        #endregion // Events

        #region Properties

        /// <summary>
        /// Gets or sets the height of the preview canvas.
        /// </summary>
        public double PreviewHeight
        {
            get
            {
                return (double)this.GetValue(Viewfinder.PreviewHeightProperty);
            }

            set
            {
                this.SetValue(Viewfinder.PreviewHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the width of the preview canvas.
        /// </summary>
        public double PreviewWidth
        {
            get
            {
                
                return (double)this.GetValue(Viewfinder.PreviewWidthProperty);
            }

            set
            {
                this.SetValue(Viewfinder.PreviewWidthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the preview tap boundary.
        /// </summary>
        /// <remarks>
        /// Tap boundary is a distance to the preview border, where all user actions are ignored.
        /// </remarks>
        public int TapBoundary
        {
            get
            {
                return (int)this.GetValue(Viewfinder.TapBoundaryProperty);
            }

            set
            {
                this.SetValue(Viewfinder.TapBoundaryProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the preview brush.
        /// </summary>
        public Brush PreviewBrush
        {
            get
            {
                return (Brush)this.GetValue(Viewfinder.PreviewBrushProperty);
            }

            set
            {
                this.SetValue(Viewfinder.PreviewBrushProperty, value);
                this.ApplyOrientation();
            }
        }

        /// <summary>
        /// Gets or sets the review image.
        /// </summary>
        public Brush ReviewImageBrush
        {
            get
            {
                return (Brush)this.GetValue(Viewfinder.ReviewImageBrushProperty);
            }

            set
            {
                this.SetValue(Viewfinder.ReviewImageBrushProperty, value);
                this.ApplyOrientation();
            }
        }

        /// <summary>
        /// Gets or sets the type of the camera this viewfinder is showing preview for.
        /// </summary>
        /// <remarks>
        /// The preview will be flipped for the front-facing camera.
        /// </remarks>
        public CameraType CameraType
        {
            get
            {
                return this.cameraType;
            }

            set
            {
                if (this.cameraType == value)
                {
                    return;
                }

                this.cameraType = value;
                this.ApplyOrientation();
            }
        }

        /// <summary>
        /// Gets or sets the viewfinder orientation.
        /// </summary>
        public PageOrientation Orientation
        {
            get
            {
                return this.orientation;
            }

            set
            {
                if (this.orientation == value)
                {
                    return;
                }

                if (OrientationHelper.IsPortrait(value) != OrientationHelper.IsPortrait(this.orientation))
                {
                    double temp        = this.PreviewHeight;
                    this.PreviewHeight = this.PreviewWidth;
                    this.PreviewWidth  = temp;
                }

                this.orientation = value;

                if (OrientationHelper.IsPortrait(this.orientation))
                {
                    this.RotateArrowButtonToLandscape.Stop();
                    this.RotateArrowButtonToPortrait.Begin();
                }
                else
                {
                    this.RotateArrowButtonToPortrait.Stop();
                    this.RotateArrowButtonToLandscape.Begin();
                }

                this.ApplyOrientation();
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the left arrow button.
        /// </summary>
        public Visibility LeftArrowButtonVisibility
        {
            get
            {
                return this.LeftArrowButton.IsHitTestVisible ? Visibility.Visible : Visibility.Collapsed;
            }

            set
            {
                bool shouldBeVisible = value == Visibility.Visible;

                // We rely on the IsHitTestVisible property, because Visibility is set
                // at the beginning or end of an animation.
                if (this.LeftArrowButton.IsHitTestVisible == shouldBeVisible)
                {
                    return;
                }

                if (shouldBeVisible)
                {
                    this.LeftArrowButton.IsHitTestVisible = true;
                    this.HideArrowButton.Stop();
                    this.ShowArrowButton.Begin();
                }
                else
                {
                    this.LeftArrowButton.IsHitTestVisible = false;
                    this.ShowArrowButton.Stop();
                    this.HideArrowButton.Begin();
                }
            }
        }

        #endregion Properties

        #region Public methods

        /// <summary>
        /// Shows the automatic focus brackets at the center of the viewfinder.
        /// </summary>
        public void FlashAutoFocusBrackets()
        {
            this.DismissPointFocusBrackets();

            this.AutoFocusInProgress.Begin();
        }

        /// <summary>
        /// Shows the touch focus brackets at the <paramref name="focusPoint"/> specified.
        /// </summary>
        /// <param name="focusPoint">Relative point to flash brackets at.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="focusPoint"/> does not contain relative coordinates.</exception>
        public void FlashPointFocusBrackets(Point focusPoint)
        {
            if (focusPoint.X < 0.0 || focusPoint.X > 1.0 || focusPoint.Y < 0.0 || focusPoint.Y > 1.0)
            {
                throw new ArgumentOutOfRangeException("focusPoint", focusPoint, "Point does not contain relative coordinates.");
            }

            this.DismissAutoFocusBrackets();

            // Move focus brackets taking the relative focus point into account.
            CompositeTransform pointFocusBracketsTransform = (CompositeTransform)this.PointFocusBrackets.RenderTransform;
            pointFocusBracketsTransform.TranslateX = this.LivePreview.ActualWidth * focusPoint.X  - this.LivePreview.ActualWidth / 2;
            pointFocusBracketsTransform.TranslateY = this.LivePreview.ActualHeight * focusPoint.Y - this.LivePreview.ActualHeight / 2;

            if (this.cameraType == CameraType.FrontFacing)
            {
                pointFocusBracketsTransform.TranslateX = -pointFocusBracketsTransform.TranslateX;
            }

            this.PointFocusInProgress.Begin();
        }

        /// <summary>
        /// Stops the automatic focus brackets animation, keeping them visible.
        /// </summary>
        public void LockAutoFocusBrackets()
        {
            this.DismissPointFocusBrackets();

            this.AutoFocusInProgress.Stop();
            this.AutoFocusLocked.Begin();
        }

        /// <summary>
        /// Stops the touch focus brackets animation, keeping them visible.
        /// </summary>
        public void LockTouchFocusBrackets()
        {
            this.DismissAutoFocusBrackets();

            this.PointFocusInProgress.Stop();
            this.PointFocusLocked.Begin();
        }

        /// <summary>
        /// Hides all focus brackets.
        /// </summary>
        public void DismissFocusBrackets()
        {
            this.DismissAutoFocusBrackets();
            this.DismissPointFocusBrackets();
        }

        /// <summary>
        /// Slides the preview image to the border of the screen.
        /// </summary>
        public void SlideReviewImage()
        {
            this.ReviewImageSlideOff.Begin();
        }

        /// <summary>
        /// Shows the messgae for a short time.
        /// </summary>
        /// <param name="message">Message to show.</param>
        /// <exception cref="ArgumentNullException"><paramref name="message"/> is <see langword="null"/> or empty.</exception>
        public void ShowStatusMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("message");
            }

            this.StatusText.Text = message;
            this.DisplayStatus.Begin();
        }

        /// <summary>
        /// Gets the image containing the snapshot of the preview.
        /// </summary>
        /// <returns>Image snapshot.</returns>
        /// <remarks>
        /// Some applications that capture video may want to shown the latest captured
        /// frame sliding left at the end of the capture.<br/>
        /// The current <c>WMC</c> API doesn't provide an easy way of getting a video
        /// preview buffer, so this method is just a simple workaround.<br/>
        /// After the <see cref="WriteableBitmap"/> is acquired, you can easily convert it
        /// to the <see cref="ImageBrush"/> by setting its <see cref="ImageBrush.ImageSource"/>
        /// property.
        /// </remarks>
        public WriteableBitmap GetPreviewSnapshot()
        {
            return new WriteableBitmap(this.LivePreview, new CompositeTransform());
        }

        #endregion // Public methods

        #region Private methods

        #region Orientation

        /// <summary>
        /// Adjusts position of the UI elements in accordance with the current <see cref="Orientation"/>.
        /// </summary>
        private void ApplyOrientation()
        {
            this.UpdatePreviewPosition();
            this.UpdateStatusTextPosition();
            this.UpdateLeftArrowPosition();
        }

        /// <summary>
        /// Updates the preview position and rotation.
        /// </summary>
        private void UpdatePreviewPosition()
        {
            // Apply a mirror effect for front facing camera.
            double scaleX = this.cameraType == CameraType.FrontFacing ? -1.0 : 1.0;

            if (this.PreviewBrush != null)
            {
                int angle = OrientationHelper.GetRotationAngle(this.orientation);
                this.PreviewBrush.RelativeTransform = new CompositeTransform { CenterY = 0.5, CenterX = 0.5, Rotation = angle, ScaleX = scaleX };
            }

            // Update the slide off animation with the correct starting and ending points.
            this.ReviewImageSlideOffTranslateStart.Value = this.LivePreviewTransform.TranslateX;
            this.ReviewImageSlideOffTranslateEnd.Value   = this.ReviewImageSlideOffTranslateStart.Value - this.ActualWidth;
        }

        /// <summary>
        /// Updates the left arrow button position.
        /// </summary>
        private void UpdateLeftArrowPosition()
        {
            double xDelta = 0;
            double yDelta = 0;

            // Adjustments for proper rotation.
            // NOTE: Left arrow button Width should be used, because the ActualWidth is 0, when it's collapsed.
            switch (this.Orientation)
            {
                case PageOrientation.None:
                case PageOrientation.Portrait:
                case PageOrientation.PortraitUp:
                case PageOrientation.PortraitDown:
                case PageOrientation.Landscape:
                case PageOrientation.LandscapeLeft:
                    xDelta = (-this.ActualWidth + this.LeftArrowButton.Width) / 2 - this.LeftArrowButton.Margin.Left;
                    yDelta = (-this.ActualHeight + this.LeftArrowButton.Height) / 2 - this.LeftArrowButton.Margin.Top;
                    break;
                case PageOrientation.LandscapeRight:
                    xDelta = (this.ActualWidth - this.LeftArrowButton.Width) / 2 + this.LeftArrowButton.Margin.Right;
                    yDelta = (-this.ActualHeight + this.LeftArrowButton.Height) / 2 - this.LeftArrowButton.Margin.Top;
                    break;
            }

            ((CompositeTransform)this.LeftArrowButton.RenderTransform).TranslateX = xDelta;
            ((CompositeTransform)this.LeftArrowButton.RenderTransform).TranslateY = yDelta;
        }

        /// <summary>
        /// Updates the status text block position.
        /// </summary>
        private void UpdateStatusTextPosition()
        {
            double xDelta = (this.ActualWidth - this.PreviewWidth) / 2;
            double yDelta = -(this.ActualHeight - this.PreviewHeight) / 2;

            switch (this.Orientation)
            {
                case PageOrientation.None:
                case PageOrientation.Portrait:
                case PageOrientation.PortraitUp:
                case PageOrientation.PortraitDown:
                    yDelta = Math.Min(yDelta, -Constants.ApplicationBarHeight);
                    break;
                case PageOrientation.Landscape:
                case PageOrientation.LandscapeLeft:
                    xDelta = Math.Max(xDelta, 0);
                    break;
                case PageOrientation.LandscapeRight:
                    xDelta = Math.Max(xDelta, Constants.ApplicationBarHeight);
                    break;
            }

            ((TranslateTransform)this.StatusText.RenderTransform).X = xDelta;
            ((TranslateTransform)this.StatusText.RenderTransform).Y = yDelta;
        }

        #endregion // Orientation

        #region Helpers

        /// <summary>
        /// Hides the automatic focus brackets.
        /// </summary>
        private void DismissAutoFocusBrackets()
        {
            this.AutoFocusInProgress.Stop();
            this.AutoFocusLocked.Stop();

            this.AutoFocusBrackets.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Hides the point focus brackets.
        /// </summary>
        private void DismissPointFocusBrackets()
        {
            this.PointFocusInProgress.Stop();
            this.PointFocusLocked.Stop();

            this.PointFocusBrackets.Visibility = Visibility.Collapsed;
        }

        #endregion // Helpers

        #region Event handlers

        /// <summary>
        /// <see cref="FrameworkElement.Loaded"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            this.ApplyOrientation();
        }

        /// <summary>
        /// <see cref="UIElement.Tap"/> event handler.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="GestureEventArgs"/> instance containing the event data.</param>
        private void OnPreviewTap(object sender, GestureEventArgs e)
        {
            Point pointOnPreview = e.GetPosition(this.LivePreview);

            // Don't notify about touches made close to the edges.
            if (pointOnPreview.X < this.TapBoundary || pointOnPreview.X > this.LivePreview.Width - this.TapBoundary
                || pointOnPreview.Y < this.TapBoundary || pointOnPreview.Y > this.PreviewHeight - this.TapBoundary)
            {
                return;
            }

            Point positionPercentage = new Point(pointOnPreview.X / this.LivePreview.ActualWidth, pointOnPreview.Y / this.LivePreview.ActualHeight);

            // We flip preview's X-axis for the front-facing camera.
            if (this.cameraType == CameraType.FrontFacing)
            {
                positionPercentage.X = 1.0 - positionPercentage.X;
            }

            EventHandler<PreviewTapLocationEventArgs> handler = this.PreviewTap;
            if (handler != null)
            {
                handler.Invoke(this, new PreviewTapLocationEventArgs(positionPercentage));
            }
        }

        /// <summary>
        /// <see cref="ButtonBase.Click"/> event handler for the <see cref="LeftArrowButton"/>.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="GestureEventArgs"/> instance containing the event data.</param>
        private void OnLeftArrowButtonTap(object sender, GestureEventArgs e)
        {
            // Ensure this doesn't bubble down to the viewfinder and trigger a capture.
            e.Handled = true;

            EventHandler handler = this.LeftArrowButtonTap;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// <see cref="Timeline.Completed"/> event handler for the <see cref="ReviewImageSlideOff"/> object.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnReviewImageSlideOffCompleted(object sender, EventArgs e)
        {
            EventHandler<EventArgs> handler = this.SlideReviewComplete;
            if (handler != null)
            {
                handler.Invoke(this, e);
            }
        }

        #endregion // Event handlers

        #endregion // Private methods
    }
}
