// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.SceReader.Data;
using Microsoft.SceReader;
using Microsoft.SceReader.Data.Feed;
using System.Windows.Threading;

namespace MsdnReader
{
    /// <summary>
    /// Interaction logic for ImageViewerWindow.xaml
    /// </summary>

    public partial class ImageViewerWindow : ViewerWindowBase
    {
        #region Methods

        public ImageViewerWindow()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ComponentCommands.ScrollPageUp, OnScrollPageUpCommand));
            CommandBindings.Add(new CommandBinding(ComponentCommands.ScrollPageDown, OnScrollPageDownCommand));
            CommandBindings.Add(new CommandBinding(NextImage, OnNextImageCommand, OnNextImageCommandCanExecute));
            CommandBindings.Add(new CommandBinding(PreviousImage, OnPreviousImageCommand, OnPreviousImageCommandCanExecute));
            InputBindings.Add(new InputBinding(ComponentCommands.ScrollPageDown, new KeyGesture(Key.Space)));
            InputBindings.Add(new InputBinding(NextImage, new KeyGesture(Key.Right)));
            InputBindings.Add(new InputBinding(PreviousImage, new KeyGesture(Key.Left)));

        }

        /// <summary>
        /// Go to the next image
        /// </summary>
        public void GoToNextImage()
        {
            if (this.CanGoToNextImage)
            {
                if (this.Story != null && this.ImageReference != null)
                {
                    // Go to the next image in the story
                    int newIndex = _index + 1;
                    if (Story.ImageReferenceCollection != null && newIndex >= 0 && newIndex < Story.ImageReferenceCollection.Count)
                    {
                        this.ImageReference = Story.ImageReferenceCollection[newIndex];
                    }
                }
            }
        }

        /// <summary>
        /// Go to the previous image
        /// </summary>
        public void GoToPreviousImage()
        {
            if (this.CanGoToPreviousImage)
            {
                if (this.Story != null && this.ImageReference != null)
                {
                    // Go to the next image in the story
                    int newIndex = _index - 1;
                    if (Story.ImageReferenceCollection != null &&  newIndex >= 0 && newIndex < Story.ImageReferenceCollection.Count)
                    {
                        this.ImageReference = Story.ImageReferenceCollection[newIndex];
                    }
                }
            }
        }

        /// <summary>
        /// Mouse wheel changes zoom
        /// </summary>
        /// <param name="e">Event data</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (e.Delta != 0)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    if (e.Delta > 0)
                    {
                        IncreaseZoom();
                    }
                    else
                    {
                        DecreaseZoom();
                    }
                }
            }
        }

        /// <summary>
        /// Mouse down over title grid - fakes Drag since window has no style
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnMouseDownOverTitleGrid(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// <summary>
        /// KeyDown handler for ImageViewerWindow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.Key == Key.OemPlus && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) > 0))
                {
                    IncreaseZoom();
                }
                else if (e.Key == Key.OemMinus && ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) > 0))
                {
                    DecreaseZoom();
                }
            }
        }

        /// <summary>
        /// Decreases zoom
        /// </summary>
        private void DecreaseZoom()
        {
            if (Slider.DecreaseLarge.CanExecute(null, this.zoomSlider))
            {
                Slider.DecreaseLarge.Execute(null, this.zoomSlider);
            }
        }

        /// <summary>
        /// Increases zoom
        /// </summary>
        private void IncreaseZoom()
        {
            if (Slider.IncreaseLarge.CanExecute(null, this.zoomSlider))
            {
                Slider.IncreaseLarge.Execute(null, this.zoomSlider);
            }
        }

        /// <summary>
        /// Invoked when ImageReference property is changed.
        /// </summary>
        private static void OnImageReferenceChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            ((ImageViewerWindow)element).InvalidateContent();
        }

        /// <summary>
        /// Invoked when Story property is changed.
        /// </summary>
        private static void OnStoryChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            ((ImageViewerWindow)element).InvalidateContent();
        }

        /// <summary>
        /// Event handler for Unloaded event
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">Event data</param>
        protected override void OnUnloaded(RoutedEventArgs e)
        {
            ServiceProvider.DataManager.GetImageSourceCompleted -= OnGetImageSourceCompleted;
            base.OnUnloaded(e);
        }

        /// <summary>
        /// Event handler for Loaded event
        /// </summary>
        /// <param name="e">Event data</param>
        protected override void OnLoaded(RoutedEventArgs e)
        {
            ServiceProvider.DataManager.GetImageSourceCompleted += OnGetImageSourceCompleted;
            this.DataContext = this;
            this.closeButton.Focus();
            base.OnLoaded(e);
        }

        /// <summary>
        /// Called when image download has been completed (successfully or not).
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">Provides detailed information about download completion.</param>
        private void OnGetImageSourceCompleted(object sender, GetImageSourceCompletedEventArgs e)
        {
            _isImageDownloadInProgress = false;
            if (Object.ReferenceEquals(e.UserState, this))
            {
                if (e.Error == null && !e.Cancelled)
                {
                    SetValue(ImageSourcePropertyKey, e.ImageSource);
                }
                else
                {
                    SetValue(ImageSourcePropertyKey, null);
                }
            }
        }

        /// <summary>
        /// Schedule content update operation
        /// </summary>
        private void InvalidateContent()
        {
            if (!_isContentUpdatePending)
            {
                // Cancel any pending image downloads.
                if (_isImageDownloadInProgress)
                {
                    ServiceProvider.DataManager.CancelAsync(this);
                }

                // Schedule a background task to asynchronously update the content.
                _isContentUpdatePending = true;

                // Don't allow movement to next/previous image
                SetValue(CanGoToNextImagePropertyKey, false);
                SetValue(CanGoToPreviousImagePropertyKey, false);

                Dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(UpdateContent), null);
            }
        }

        /// <summary>
        /// Called when content is changed and control is ready for content update.
        /// </summary>
        /// <remarks>
        /// Starts asynchronous image download if image reference changed
        /// </remarks>
        private void OnUpdateContent()
        {
            UpdateSelectedImage();

            // If image data is provided, start asynchronous download of the image, if it's been matched to story
            ImageReference imageReference = ImageReference;
            if (imageReference != null && Story != null && Story.ImageReferenceCollection != null &&
                imageReference.ImageDataCollection.Count > 0 && _index >= 0 && _index < Story.ImageReferenceCollection.Count)
            {
                _isImageDownloadInProgress = true;
                ServiceProvider.DataManager.GetImageSourceAsync(imageReference.LargestImageData.Source, Story, this);
            }
            else
            {
                // No mage source available, set ImageSource to null
                SetValue(ImageSourcePropertyKey, null);
            }
        }

        /// <summary>
        /// Update selected index with index for selected image
        /// </summary>
        private void UpdateSelectedImage()
        {
            _index = -1;
            bool canGoToNextImage = false;
            bool canGoToPreviousImage = false;
            if (ImageReference != null && Story != null)
            {
                if (Story.ImageReferenceCollection != null &&  Story.ImageReferenceCollection.Contains(ImageReference))
                {
                    _index = Story.ImageReferenceCollection.IndexOf(ImageReference);
                    canGoToNextImage = (_index < (Story.ImageReferenceCollection.Count - 1)) ? true : false;
                    canGoToPreviousImage = (_index > 0) ? true : false;
                }
            }
            SetValue(CanGoToNextImagePropertyKey, canGoToNextImage);
            SetValue(CanGoToPreviousImagePropertyKey, canGoToPreviousImage);
            UpdateNavButtonVisibility();
        }

        /// <summary>
        /// Update visiblity of nav buttons when content is invalidated
        /// </summary>
        private void UpdateNavButtonVisibility()
        {
            if (!this.CanGoToNextImage)
            {
                this.nextButton.Visibility = Visibility.Hidden;
                this.nextButton.IsEnabled = false;
                this.nextLabel.Visibility = Visibility.Hidden;
                this.divider.Visibility = Visibility.Hidden;
            }
            else
            {
                this.nextButton.Visibility = Visibility.Visible;
                this.nextButton.IsEnabled = true;
                this.nextLabel.Visibility = Visibility.Visible;
            }

            if (!this.CanGoToPreviousImage)
            {
                this.previousButton.Visibility = Visibility.Hidden;
                this.previousButton.IsEnabled = false;
                this.previousLabel.Visibility = Visibility.Hidden;
                this.divider.Visibility = Visibility.Hidden;
            }
            else
            {
                this.previousButton.Visibility = Visibility.Visible;
                this.previousButton.IsEnabled = true;
                this.previousLabel.Visibility = Visibility.Visible;
            }

            if (this.CanGoToPreviousImage && this.CanGoToNextImage)
            {
                this.divider.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Content update callback
        /// </summary>
        private object UpdateContent(object arg)
        {
            _isContentUpdatePending = false;
            OnUpdateContent();
            return null;
        }

        /// <summary>
        /// Command handler for next image command
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">EventArgs describing the event</param>
        private static void OnNextImageCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ImageViewerWindow window = sender as ImageViewerWindow;
            if (window != null)
            {
                if (window.CanGoToNextImage)
                {
                    window.GoToNextImage();
                }
            }
        }

        /// <summary>
        /// Command handler for previous image command
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">EventArgs describing the event</param>
        private static void OnPreviousImageCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ImageViewerWindow window = sender as ImageViewerWindow;
            if (window != null)
            {
                if (window.CanGoToPreviousImage)
                {
                    window.GoToPreviousImage();
                }
            }
        }

        /// <summary>
        /// Command handler for next image command can execute
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">EventArgs describing the event</param>
        private static void OnNextImageCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ImageViewerWindow window = sender as ImageViewerWindow;
            if (window != null)
            {
                if (window.CanGoToNextImage)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
        }

        /// <summary>
        /// Command handler for previous image command can execute
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">EventArgs describing the event</param>
        private static void OnPreviousImageCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            ImageViewerWindow window = sender as ImageViewerWindow;
            if (window != null)
            {
                if (window.CanGoToPreviousImage)
                {
                    e.CanExecute = true;
                }
                else
                {
                    e.CanExecute = false;
                }
            }
        }

        /// <summary>
        /// Command handler for Restore command
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        protected override void OnRestore(ExecutedRoutedEventArgs e)
        {
            base.OnRestore(e);
            this.maximizeButton.Visibility = Visibility.Visible;
            this.restoreButton.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Command handler for Maximize command
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        protected override void OnMaximize(ExecutedRoutedEventArgs e)
        {
            base.OnMaximize(e);
            this.maximizeButton.Visibility = Visibility.Collapsed;
            this.restoreButton.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Command handler for Scroll page up command
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">EventArgs describing the event</param>
        private void OnScrollPageUpCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ImageViewerWindow window = sender as ImageViewerWindow;
            if (window != null)
            {
                if (window.CanGoToPreviousImage)
                {
                    window.GoToPreviousImage();
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Command handler for Scroll page down command
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">EventArgs describing the event</param>
        private void OnScrollPageDownCommand(object sender, ExecutedRoutedEventArgs e)
        {
            ImageViewerWindow window = sender as ImageViewerWindow;
            if (window != null)
            {
                if (window.CanGoToNextImage)
                {
                    window.GoToNextImage();
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handler for Slider's zoom changed event, applies zoom to image
        /// </summary>
        /// <param name="sender">Event source</param>
        /// <param name="args">Event data</param>
        private void OnZoomChanged(object sender, RoutedPropertyChangedEventArgs<double> args)
        {
            double scaledNewValue = args.NewValue / 100;
            double scaledOldValue = args.OldValue / 100;
            double centerX = this.imageScrollViewer.HorizontalOffset + this.imageScrollViewer.ViewportWidth / 2.0;
            double centerY = this.imageScrollViewer.VerticalOffset + this.imageScrollViewer.ViewportHeight / 2.0;

            double offsetX = 0.0;
            double offsetY = 0.0;
            if (scaledOldValue != 0)
            {
                offsetX = centerX * (scaledNewValue / scaledOldValue) - this.imageScrollViewer.ViewportWidth / 2.0;
                offsetY = centerY * (scaledNewValue / scaledOldValue) - this.imageScrollViewer.ViewportHeight / 2.0;
            }

            this.image.LayoutTransform = new ScaleTransform(scaledNewValue, scaledNewValue);

            this.imageScrollViewer.ScrollToHorizontalOffset(offsetX);
            this.imageScrollViewer.ScrollToVerticalOffset(offsetY);
        }

        #endregion

        #region Properties

        /// <summary>
        /// DependencyPropertyKey for <see cref="ImageSource"/> property.
        /// </summary>
        private static readonly DependencyPropertyKey ImageSourcePropertyKey =
                DependencyProperty.RegisterReadOnly(
                        "ImageSource",
                        typeof(ImageSource),
                        typeof(ImageViewerWindow),
                        new FrameworkPropertyMetadata(null));

        /// <summary>
        /// DependencyProperty for <see cref="ImageSource"/> property.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
                ImageSourcePropertyKey.DependencyProperty;

        /// <summary>
        /// The ImageSource representing the image data.
        /// </summary>
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="ImageReference" /> property.
        /// </summary>
        public static readonly DependencyProperty ImageReferenceProperty =
                DependencyProperty.Register(
                        "ImageReference",
                        typeof(ImageReference),
                        typeof(ImageViewerWindow),
                        new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnImageReferenceChanged)));

        /// <summary>
        /// The ImageReference object used to generate the content of this control.
        /// </summary>
        public ImageReference ImageReference
        {
            get { return (ImageReference)GetValue(ImageReferenceProperty); }
            set { SetValue(ImageReferenceProperty, value); }
        }

        /// <summary>
        /// DependencyProperty for <see cref="Story" /> property.
        /// </summary>
        public static readonly DependencyProperty StoryProperty =
                DependencyProperty.Register(
                        "Story",
                        typeof(Story),
                        typeof(ImageViewerWindow),
                        new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnStoryChanged)));

        /// <summary>
        /// The Story object associated with ImageReference.
        /// </summary>
        public Story Story
        {
            get { return (Story)GetValue(StoryProperty); }
            set { SetValue(StoryProperty, value); }
        }

        /// <summary>
        /// DependencyPropertyKey for <see cref="CanGoToNextImage"/> property.
        /// </summary>
        private static readonly DependencyPropertyKey CanGoToNextImagePropertyKey =
                DependencyProperty.RegisterReadOnly(
                        "CanGoToNextImage",
                        typeof(bool),
                        typeof(ImageViewerWindow),
                        new FrameworkPropertyMetadata(false));

        /// <summary>
        /// DependencyProperty for <see cref="CanGoToNextImage"/> property.
        /// </summary>
        public static readonly DependencyProperty CanGoToNextImageProperty =
                CanGoToNextImagePropertyKey.DependencyProperty;

        /// <summary>
        /// True if the control can go to the next image
        /// </summary>
        public bool CanGoToNextImage
        {
            get { return (bool)GetValue(CanGoToNextImageProperty); }
        }

        /// <summary>
        /// DependencyPropertyKey for <see cref="CanGoToPreviousImage"/> property.
        /// </summary>
        private static readonly DependencyPropertyKey CanGoToPreviousImagePropertyKey =
                DependencyProperty.RegisterReadOnly(
                        "CanGoToPreviousImage",
                        typeof(bool),
                        typeof(ImageViewerWindow),
                        new FrameworkPropertyMetadata(false));

        /// <summary>
        /// DependencyProperty for <see cref="CanGoToPreviousImage"/> property.
        /// </summary>
        public static readonly DependencyProperty CanGoToPreviousImageProperty =
                CanGoToPreviousImagePropertyKey.DependencyProperty;

        /// <summary>
        /// True if the control can go to the previous image
        /// </summary>
        public bool CanGoToPreviousImage
        {
            get { return (bool)GetValue(CanGoToPreviousImageProperty); }
        }

        /// <summary>
        /// NextImage command
        /// </summary>
        public static RoutedCommand NextImage
        {
            get { return _nextImage; }
        }

        /// <summary>
        /// PreviousImage command
        /// </summary>
        public static RoutedCommand PreviousImage
        {
            get { return _previousImage; }
        }

        #endregion

        #region Fields

        private int _index = -1;
        private static RoutedCommand _nextImage = new RoutedCommand("NextImage", typeof(ImageViewerWindow));
        private static RoutedCommand _previousImage = new RoutedCommand("PreviousImage", typeof(ImageViewerWindow));
        private bool _isContentUpdatePending;
        private bool _isImageDownloadInProgress;

        #endregion
    }
}