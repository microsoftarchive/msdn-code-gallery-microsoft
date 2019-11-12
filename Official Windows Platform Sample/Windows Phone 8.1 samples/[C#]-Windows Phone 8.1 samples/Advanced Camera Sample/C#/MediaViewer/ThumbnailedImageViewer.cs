// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="ThumbnailedImageViewer.cs">
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
    using System.Windows.Data;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// Knowns how to display an IThumbnailedImage, picking the thumbnail or 
    /// full resolution image to display based on the container size.  The
    /// IThumbnailedImage to display should be assigned to the DataContext
    /// property.
    /// </summary>
    /// <remarks>
    /// This code is taken from the Code Samples for Windows Phone (http://code.msdn.microsoft.com/wpapps).
    /// </remarks>
    public class ThumbnailedImageViewer : Control
    {
        private enum ImageBindingState { ScreenSizeThumbnail, FullSizePhoto }
        private ImageBindingState _imageBindingState = ImageBindingState.ScreenSizeThumbnail;
        private Image _image = null;
        private FrameworkElement _placeholder;

        private BitmapImage _thumbnailBitmapImage;
        private ImageSource _thumbnailImageSource;
        private BitmapImage _fullResolutionBitmapImage;
        private ImageSource _fullResolutionImageSource;

        public ThumbnailedImageViewer()
        {
            this.DefaultStyleKey = typeof(ThumbnailedImageViewer);

            // Register for DataContext change notifications
            DependencyProperty dataContextDependencyProperty = System.Windows.DependencyProperty.RegisterAttached("DataContextProperty", typeof(object), typeof(FrameworkElement), new System.Windows.PropertyMetadata(OnDataContextPropertyChanged));
            this.SetBinding(dataContextDependencyProperty, new Binding());
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.DataContext is IThumbnailedImage)
            {
                if ((this._imageBindingState == ImageBindingState.ScreenSizeThumbnail) &&
                    (this._image.Visibility == System.Windows.Visibility.Visible) &&      // make sure the image is loaded before measuring its size
                    (this.CurrentImageSizeIsTooSmall(availableSize)))
                {
                    this.BeginLoadingFullResolutionImage();
                }
            }

            return base.MeasureOverride(availableSize);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this._image = this.GetTemplateChild("Image") as Image;
            this._placeholder = this.GetTemplateChild("Placeholder") as FrameworkElement;

            this.ShowPlaceholder();
        }

        private static void OnDataContextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ThumbnailedImageViewer mediaItemViewer = (ThumbnailedImageViewer)d;

            IThumbnailedImage newPhoto = e.NewValue as IThumbnailedImage;

            mediaItemViewer.ClearImageSources();

            if (e.NewValue != null)
            {
                mediaItemViewer.ShowPlaceholder();
                mediaItemViewer.BeginLoadingThumbnail();
            }
            else
            {
                mediaItemViewer._image.Source = null;
            }
        }

        private void OnThumbnailOpened(object sender, EventArgs e)
        {
            this._image.Source = null;
            this._image.Source = this._thumbnailImageSource;

            this.ClearImageSources();

            this.HidePlaceholder();
            this.InvalidateMeasure();
        }

        private void OnFullSizeImageOpened(object sender, EventArgs e)
        {
            this._image.Source = null;
            this._image.Source = this._fullResolutionImageSource;

            this.ClearImageSources();

            this.HidePlaceholder();
            this.InvalidateMeasure();
        }

        private void ClearImageSources()
        {
            if (this._thumbnailBitmapImage != null)
            {
                this._thumbnailBitmapImage.ImageOpened -= this.OnThumbnailOpened;
            }
            this._thumbnailBitmapImage = null;

            if (this._fullResolutionBitmapImage != null)
            {
                this._fullResolutionBitmapImage.ImageOpened -= this.OnFullSizeImageOpened;
                this._fullResolutionBitmapImage.ImageOpened -= this.OnFullSizeImageOpened;
            }
            this._fullResolutionBitmapImage = null;

            this._thumbnailImageSource = null;
            this._fullResolutionImageSource = null;
        }

        private bool CurrentImageSizeIsTooSmall(Size availableSize)
        {
            BitmapImage source = this._image.Source as BitmapImage;

            if (source == null)
            {
                return true;
            }

            bool toReturn = ((source.PixelWidth < availableSize.Width) && (source.PixelHeight < availableSize.Height));

            if (toReturn)
            {
                //Tracing.Trace("MediaItemViewer.CurrentImageSizeIsTooSmall() - switching from thumbnail to full res photo because the thumbnail is too small (" + source.PixelWidth + ", " + source.PixelHeight + ") for the available size (" + availableSize + ")");
            }

            return toReturn;
        }

        private void BeginLoadingThumbnail()
        {
            if (this.DataContext is IThumbnailedImage == false)
            {
                return;
            }

            //Tracing.Trace("MediaItemViewer.BeginLoadingThumbnail()");

            if (this._thumbnailBitmapImage != null)
            {
                this._thumbnailBitmapImage.ImageOpened -= this.OnThumbnailOpened;
            }
            this._thumbnailBitmapImage = null;

            this._thumbnailBitmapImage = new BitmapImage();
            this._thumbnailBitmapImage.ImageOpened += this.OnThumbnailOpened;
            this._thumbnailBitmapImage.CreateOptions = BitmapCreateOptions.BackgroundCreation;
            this._thumbnailBitmapImage.SetSource(((IThumbnailedImage)this.DataContext).GetThumbnailImage());
            this._thumbnailImageSource = this._thumbnailBitmapImage;

            this._imageBindingState = ImageBindingState.ScreenSizeThumbnail;
        }

        private void BeginLoadingFullResolutionImage()
        {
            if (this.DataContext is IThumbnailedImage == false)
            {
                return;
            }
            
            //Tracing.Trace("MediaItemViewer.BeginLoadingFullResolutionImage()");

            if (this._fullResolutionBitmapImage != null)
            {
                this._fullResolutionBitmapImage.ImageOpened -= this.OnFullSizeImageOpened;
            }
            this._fullResolutionBitmapImage = null;

            this._fullResolutionBitmapImage = new BitmapImage();
            this._fullResolutionBitmapImage.ImageOpened += this.OnFullSizeImageOpened;
            this._fullResolutionBitmapImage.CreateOptions = BitmapCreateOptions.BackgroundCreation;
            this._fullResolutionBitmapImage.SetSource(((IThumbnailedImage)this.DataContext).GetImage());
            this._fullResolutionImageSource = this._fullResolutionBitmapImage;

            this._imageBindingState = ImageBindingState.FullSizePhoto;
        }

        private void ShowPlaceholder()
        {
            if (this._placeholder != null)
            {
                this._placeholder.Visibility = System.Windows.Visibility.Visible;
            }
            if (this._image != null)
            {
                this._image.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void HidePlaceholder()
        {
            if (this._image != null)
            {
                this._image.Visibility = System.Windows.Visibility.Visible;
            }
            if (this._placeholder != null)
            {
                this._placeholder.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
