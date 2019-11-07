//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;

namespace GesturesApp.Controls
{

    /// <summary>
    /// Basic class for each gesture page, defines the properties that are used by the UI.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public class GesturePageBase : Windows.UI.Xaml.Controls.Page
    {
        /// <summary>
        /// Creates a button that when clicked displays a flyout with the specified <paramref name="links"/>.
        /// Clicking on a link opens a web browser on that page.
        /// </summary>
        /// <param name="links"></param>
        /// <returns></returns>
        internal static Windows.UI.Xaml.Controls.Button CreateLinksAppBarButton(Dictionary<string, Uri> links)
        {
            var popup = new Windows.UI.Popups.PopupMenu();
            Windows.UI.Popups.UICommandInvokedHandler popupHandler = async (Windows.UI.Popups.IUICommand command) =>
            {
                await Windows.System.Launcher.LaunchUriAsync(links[command.Label]);
            };

            foreach (var item in links)
            {
                popup.Commands.Add(new Windows.UI.Popups.UICommand(item.Key, popupHandler));
            }

            var button = new Windows.UI.Xaml.Controls.Button
            {
                Style = App.Current.Resources["LinksAppBarButtonStyle"] as Windows.UI.Xaml.Style
            };
            button.Click += async (object sender, Windows.UI.Xaml.RoutedEventArgs e) =>
            {
                var btnSender = sender as Windows.UI.Xaml.Controls.Button;
                var transform = btnSender.TransformToVisual(Windows.UI.Xaml.Window.Current.Content) as Windows.UI.Xaml.Media.MatrixTransform;
                var point = transform.TransformPoint(new Windows.Foundation.Point());             

                await popup.ShowAsync(point);
            };

            return button;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// This <see cref="Windows.UI.Xaml.Controls.Page"/> does not have direct access to the <see cref="Windows.UI.Xaml.Controls.AppBar"/>
        /// because it is used in this app as the base class for <see cref="SemanticFlipView"/> items. Therefore the page is displayed inside
        /// the <see cref="SemanticZoomPage"/> and has to use that page's AppBar.
        /// </remarks>
        /// <seealso cref="SemanticZoomPage"/>
        public GesturePageBase(String uniqueId, String title, String description, String similarTo, String imagePath)
        {
            // Create GesturePageInfo
            // NOTE: we need a separate class to avoid issues when using this control as data item for the SemanticZoom
            this._appPageInfo = new GesturePageInfo(uniqueId, title, description, similarTo, imagePath, this);
            
            // The content of the global app bar in this app is a grid that contains
            // two panels for contextual and non-contextual items respectively.
            var appBar = (Windows.UI.Xaml.Controls.AppBar)GesturesApp.Pages.SemanticZoomPage.Current.FindName("globalAppBar");
            var grid = appBar.Content as Windows.UI.Xaml.Controls.Grid;
            this._contextualItemsPanel = grid.Children[0] as Windows.UI.Xaml.Controls.StackPanel;
            this._nonContextualItemsPanel = grid.Children[1] as Windows.UI.Xaml.Controls.StackPanel;
            
            // Create data structures for links and non contextual items, they will be populated by extending classes
            this._nonContextualItems = new List<Windows.UI.Xaml.Controls.Button>();
            this._links = new Dictionary<string, Uri>();

            this._isSelected = false;
        }

        protected IGesturePageInfo _appPageInfo;
        public IGesturePageInfo AppPageInfo
        {
            get { return this._appPageInfo; }
        }

        protected readonly Dictionary<string, Uri> _links;
        public IEnumerable<KeyValuePair<string, Uri>> Links
        {
            get { return this._links; }
        }

        // The content of the global app bar in this app is a grid that contains
        // two panels for contextual and non-contextual items respectively.
        // We store references to these panels for use by this class and its extensions.
        protected readonly Windows.UI.Xaml.Controls.StackPanel _contextualItemsPanel;
        protected readonly Windows.UI.Xaml.Controls.StackPanel _nonContextualItemsPanel;

        // Non contextual app bar items for this page, used by Selected to configure the app bar
        protected readonly List<Windows.UI.Xaml.Controls.Button> _nonContextualItems;

        // Whether this is the selected page in the FlipView
        private bool _isSelected;
        internal bool Selected
        {
            get { return this._isSelected; }
            set
            {
                if (this._isSelected != value)
                {
                    if (value)
                    {
                        this.OnSelected();
                    }
                    else
                    {
                        this.OnUnselected();
                    }

                    this._isSelected = value;
                }
            }
        }

        // Called when this becomes the selected page in the FlipView
        protected virtual void OnSelected()
        {
            // Populate the panel for non contextual items
            foreach (var item in this._nonContextualItems)
            {
                this._nonContextualItemsPanel.Children.Add(item);
            }
        }

        // Called when this is no longer the selected page in the FlipView
        protected virtual void OnUnselected()
        {
            // Remove items from the panel for non contextual items
            foreach (var item in this._nonContextualItems)
            {
                this._nonContextualItemsPanel.Children.Remove(item);
            }
        }
    }

    sealed class GesturePageInfo : IGesturePageInfo
    {
        public GesturePageInfo(String uniqueId, String title, String description, String similarTo, String imagePath, GesturesApp.Controls.GesturePageBase playArea)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._description = description;
            this._similarTo = similarTo;
            this._imagePath = imagePath;
            this._playArea = playArea;
        }

        private String _uniqueId;
        public String UniqueId
        {
            get { return this._uniqueId; }
        }

        private String _title;
        public String Title
        {
            get { return this._title; }
        }

        private String _description;
        public String Description
        {
            get { return this._description; }
        }

        private String _similarTo;
        public String SimilarTo
        {
            get { return this._similarTo; }
        }

        private String _imagePath;
        public String ImagePath
        {
            get { return this._imagePath; }
        }

        private Windows.UI.Xaml.Media.ImageSource _image = null;
        public Windows.UI.Xaml.Media.ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null && this._imagePath != String.Empty)
                {
                    this._image = new Windows.UI.Xaml.Media.Imaging.BitmapImage(
                        new Uri("ms-appx:///" + this._imagePath));
                }
                return this._image;
            }
        }

        private GesturesApp.Controls.GesturePageBase _playArea;
        public GesturesApp.Controls.GesturePageBase PlayArea
        {
            get { return this._playArea; }
        }
    }
}
