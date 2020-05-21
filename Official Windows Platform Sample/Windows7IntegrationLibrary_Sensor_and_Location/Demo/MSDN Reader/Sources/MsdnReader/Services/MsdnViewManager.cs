// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SceReader.View;
using Microsoft.SceReader.Data;
using Microsoft.SceReader;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.SceReader.Controls;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.SubscriptionCenter.Sync;

namespace MsdnReader
{
    public class MsdnViewManager : ViewManager
    {
        #region Methods

        public MsdnViewManager() : base()
        {
            InitializeMsdnCommands();

            PropertyChanged += OnPropertyChanged;

            // no need for read state persistence for design data feeds
            // only enable read state persistence for real feed sources
            bool enableReadStatePersistence = true;
#if DEBUG
            enableReadStatePersistence = !SceReaderSettings.UseDesignFeedInDebug;
#endif
            if (enableReadStatePersistence)
            {
                _readStatePersistence = new ReadStatePersistence(SceReaderSettings.LocalCacheFolder, SyncItemCache.DefaultFeedDatabaseName, ServiceProvider.Logger, this);
            }
        }
 
        /// <summary>
        /// Opens image viewer control
        /// </summary>
        /// <param name="control"></param>
        public void OpenImageViewerWindow(StoryImageControl control)
        {
            if (control != null)
            {
                OpenImageViewerWindow(control.ImageReference, control.Story);
            }
        }

        /// <summary>
        /// Static handler for image hyperlink's request navigate event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnImageHyperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // Always set handled to true because hyperlinks to which this event is attached are not real weblinks and require special handling.
            // Don't let this go up to the navigation service
            e.Handled = true;
            MsdnStoryImageHyperlink hyperlink = sender as MsdnStoryImageHyperlink;
            if (hyperlink != null)
            {
                if (hyperlink.IsImageReferenceLink)
                {
                    // Link to an image reference, open view
                    OpenImageViewerWindow(hyperlink.ImageReference, hyperlink.Story);
                }
                else
                {
                    // Not an image reference, this is a web link. Open web figure viewer window
                    OpenWebFigureViewerWindow(hyperlink.NavigateUri, hyperlink.Story);
                }
            }
        }

        /// <summary>
        /// Loads persisted Read state for stories
        /// </summary>
        public void LoadStoryReadState()
        {
            if (_readStatePersistence != null)
            {
                _readStatePersistence.Load();
            }
        }

        /// <summary>
        /// Persists Read state for stories
        /// </summary>
        public void SaveStoryReadState(bool runInBackground)
        {
            if (_readStatePersistence != null)
            {
                _readStatePersistence.Save(runInBackground);
                _readStatePersistence.Close();
                _readStatePersistence = null;
            }
        }

        /// <summary>
        /// When the current image viewer window closes, reset this value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnImageViewerWindowClosed(object sender, EventArgs e)
        {
            _imageViewerWindow.Closed -= OnImageViewerWindowClosed;
            _imageViewerWindow = null;
        }

        /// <summary>
        /// When the current web figure viewer window closes, reset this value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWebFigureViewerWindowClosed(object sender, EventArgs e)
        {
            _webFigureViewerWindow.Closed -= OnImageViewerWindowClosed;
            _webFigureViewerWindow = null;
        }

        /// <summary>
        /// Event handler for property changed event
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">Event Args describing property changed</param>
        private void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentVisual")
            {
                // If CurrentVisual changed, focus needs to be set depending on what the current visual is
                OnCurrentVisualChanged();
            }
            else if (e.PropertyName == "CurrentNavigator")
            {
                // When the CurrentNavigator changes, if image viewers are open and a Story is the current content,
                // image viewers need to be updated with the Story's images
                OnCurrentNavigatorChanged();
            }
        }

        /// <summary>
        /// On CurrentVisual changed, if CurrentNavigator is not null, navigation took place, and the CurrentVisual is the
        /// resulting content root. Focus on specific UI items may now be set if necessary
        /// </summary>
        /// <remarks> 
        /// This method checks that CurrentNavigator is not null, but we cannot implement this logic by listening for CurrentNavigator
        /// changed (as image viewer update logic is implemented) because visuals may not be ready to take focus
        /// </remarks>
        private void OnCurrentVisualChanged()
        {
            if (CurrentNavigator != null)
            {
                FocusCurrentVisual();
            }
        }

        /// <summary>
        /// When the current navigator changed, update the image viewer visuals if a new Story was navigated to
        /// </summary>
        private void OnCurrentNavigatorChanged()
        {
            if (CurrentNavigator is StoryNavigator)
            {
                UpdateImageViewers();
            }
        }

        /// <summary>
        /// On current visual changed, change focus as desired
        /// </summary>
        private void FocusCurrentVisual()
        {
            System.Windows.UIElement element = CurrentVisual as System.Windows.UIElement;
            if (element == null)
            {
                return;
            }

            bool focusElement;
            if (CurrentNavigator is SearchNavigator && CurrentVisual is SearchViewControl)
            {
                // Search view control implements custom focus, don't override it 
                focusElement = false;
            }
            else
            {
                // For anything else, focus current visual
                focusElement = true;
            }

            if (focusElement)
            {
                if (!element.Focus())
                {
                    element.MoveFocus(
                        new System.Windows.Input.TraversalRequest(
                            System.Windows.Input.FocusNavigationDirection.First
                        )
                    );
                }
            }
        }

        /// <summary>
        /// Update image viewers, if open with images for the current story
        /// </summary>
        private void UpdateImageViewers()
        {
            // If current navigator is a story navigator, hook the story's ImageReferenceCollection to the window
            // if there is an instance of the image viewer window active
            StoryNavigator storyNavigator = CurrentNavigator as StoryNavigator;
            if (storyNavigator != null)
            {
                Story story = storyNavigator.Content as Story;
                if (story != null)
                {
                    if (_imageViewerWindow != null)
                    {
                        _imageViewerWindow.Story = story;
                        if (story.ImageReferenceCollection != null && story.ImageReferenceCollection.Count > 0)
                        {
                            _imageViewerWindow.ImageReference = story.ImageReferenceCollection[0];
                        }
                        else
                        {
                            _imageViewerWindow.ImageReference = null;
                        }
                    }

                    if (_webFigureViewerWindow != null)
                    {
                        _webFigureViewerWindow.Story = story;
                        _webFigureViewerWindow.Source = MsdnMagazineDocumentToFlowDocumentConverter.GetMagazineArticleWebFigureUri(story);
                    }
                }
            }
        }

        /// <summary>
        /// Opens the image viewer window control
        /// </summary>
        /// <param name="imageReference"></param>
        /// <param name="story"></param>
        private void OpenImageViewerWindow(ImageReference imageReference, Story story)
        {
            // If there is no instance of image viewer currently open, create one
            if (_imageViewerWindow == null)
            {
                _imageViewerWindow = new ImageViewerWindow();
                _imageViewerWindow.Owner = Application.Current.MainWindow;
                _imageViewerWindow.Closed += OnImageViewerWindowClosed;
            }

            // Configure window to control's Story and ImageReference
            _imageViewerWindow.ImageReference = imageReference;
            _imageViewerWindow.Story = story;
            _imageViewerWindow.Show();
        }

        /// <summary>
        /// Opens the web figure viewer window control
        /// </summary>
        /// <param name="imageReference"></param>
        /// <param name="story"></param>
        private void OpenWebFigureViewerWindow(Uri source, Story story)
        {
            // If there is no instance of image viewer currently open, create one
            if (_webFigureViewerWindow == null)
            {
                _webFigureViewerWindow = new WebFigureViewerWindow();
                _webFigureViewerWindow.Owner = Application.Current.MainWindow;
                _webFigureViewerWindow.Closed += OnWebFigureViewerWindowClosed;
            }

            // Configure window to control's Story and ImageReference
            _webFigureViewerWindow.Source = source;
            _webFigureViewerWindow.Story = story;
            _webFigureViewerWindow.Show();
        }

        /// <summary>
        /// Initializes Msdn commands
        /// </summary>
        private void InitializeMsdnCommands()
        {
            _msdnCommands = new MsdnCommands(this);
        }

        /// <summary>
        /// Populates items for the NavPanel, excluding ReadingList which is included by default
        /// </summary>
        /// <returns></returns>
        protected override NavigationItemCollection  GetValidNavigationItems()
        {
            List<NavigationItem> navigationItems = new List<NavigationItem>();

            // Get Navigators from MasterNavigator and create navigation item list
            ReadOnlyCollection<Navigator> navigators = MasterNavigator.GetTopLevelNavigators();
            foreach (Navigator navigator in navigators)
            {
                string name = String.Empty;
                if (navigator is ItemNavigator && !(navigator is ReadingListNavigator))
                {
                    Item item = (navigator as ItemNavigator).Content as Item;
                    if (item != null)
                    {
                        name = item.Title;
                        NavigationItem navigationItem = new NavigationItem(name, navigator);
                        navigationItems.Add(navigationItem);
                    }
                }
            }

            return new NavigationItemCollection(navigationItems);
        
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Custom commands for MsdnReader
        /// </summary>
        public MsdnCommands MsdnCommands
        {
            get { return _msdnCommands; }
        }

        #endregion

        #region Fields
        
        private ImageViewerWindow _imageViewerWindow;
        private WebFigureViewerWindow _webFigureViewerWindow;
        private MsdnCommands _msdnCommands;
        private ReadStatePersistence _readStatePersistence;

        #endregion
    }
}