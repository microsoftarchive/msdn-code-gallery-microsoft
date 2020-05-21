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
using System.Windows.Input;
using Microsoft.SceReader;
using Microsoft.SceReader.Controls;
using Microsoft.SceReader.View;
using System.Windows.Navigation;

namespace MsdnReader
{

    public enum ViewingMode
    {
        FullScreenNavUI,
        FullScreenNoNavUI,
        NormalScreenNoNavUI,
        NormalScreenNavUI
    }

    public partial class MainWindow : NavigationWindow
    {
        public MainWindow()
        {
#if DEBUG
            this.InputBindings.Add(new KeyBinding(DebugCommands.GarbageCollectCommand, new KeyGesture(Key.G, ModifierKeys.Control)));
#endif
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(System.Windows.Input.NavigationCommands.BrowseBack, new ExecutedRoutedEventHandler(OnNavigationCommandExecuted), new CanExecuteRoutedEventHandler(OnNavigationCommandCanExecute)));
            CommandBindings.Add(new CommandBinding(System.Windows.Input.NavigationCommands.BrowseForward, new ExecutedRoutedEventHandler(OnNavigationCommandExecuted), new CanExecuteRoutedEventHandler(OnNavigationCommandCanExecute)));
            CommandBindings.Add(new CommandBinding(System.Windows.Input.ComponentCommands.ScrollPageUp, new ExecutedRoutedEventHandler(OnNavigationCommandExecuted), new CanExecuteRoutedEventHandler(OnNavigationCommandCanExecute)));
            CommandBindings.Add(new CommandBinding(System.Windows.Input.ComponentCommands.ScrollPageDown, new ExecutedRoutedEventHandler(OnNavigationCommandExecuted), new CanExecuteRoutedEventHandler(OnNavigationCommandCanExecute)));

            // If journal navigation on input is disabled, all navigation keys, such as Backspace, etc. will be consumed as next/previous navigation.
            // Application authors may want to preserve some keys for journal navigation even though by and large input gestures are disabled for
            // this feature. In the handler for BrowseBack/Forward commands it is not possible to tell whether the command came from a 
            // key gesture or not, or which key caused it. To override specific keys, MainWindow exposes a navigationKeyOverrideCommand member,
            // and binds certain keys to it. Presently this is done for the Backspace key
            if (!MsdnReaderSettings.EnableJournalNavigationOnInput)
            {
                CommandBindings.Add(new CommandBinding(this.backNavigationKeyOverrideCommand, new ExecutedRoutedEventHandler(OnJournalBackNavigationKeyOverride)));
                InputBindings.Add(new InputBinding(this.backNavigationKeyOverrideCommand, new KeyGesture(Key.Back)));
            } 
        }

        /// <summary>
        /// Override for PreviewKeyDown. Currently this method implements no custom logic for MainPage but indicates
        /// where global application preview key handling (for application input handler) should be invoked
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            // First, call application-level preview
            MsdnServiceProvider.ApplicationInputHandler.OnPreviewKeyDown(e);

            // Next, custom logic for MainPage
            if (!e.Handled)
            {
            }

            // Finally, call base if not handled
            if (!e.Handled)
            {
                base.OnPreviewKeyDown(e);
            }
        }

        /// <summary>
        /// Override for MouseWheel event. In MainPage this handler does no custom operations, it invokes the MouseWheel handler for
        /// the application
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            // Custom logic for MainPage's MouseWheel should be put before calling global handler

            // Next, call global handler
            if (!e.Handled)
            {
                MsdnServiceProvider.ApplicationInputHandler.OnMouseWheel(e);
            }

            // Finally, call base if not handled
            if (!e.Handled)
            {
                base.OnMouseWheel(e);
            }
        }

        /// <summary>
        /// OnInitialized override loads saved bounds if saving is enabled
        /// </summary>
        /// <param name="e"></param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            // Restore bounds from settings if enabled
            if (MsdnReaderSettings.SaveMainWindowBounds && MsdnReaderSettings.MainWindowBounds != Rect.Empty)
            {
                this.Left = MsdnReaderSettings.MainWindowBounds.Left;
                this.Top = MsdnReaderSettings.MainWindowBounds.Top;
                this.Width = MsdnReaderSettings.MainWindowBounds.Width;
                this.Height = MsdnReaderSettings.MainWindowBounds.Height;
                this.WindowState = MsdnReaderSettings.MainWindowState;
            }
        }

        /// <summary>
        /// OnClosing override stores window bounds to settings, if enabled
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            base.OnClosing(e);
            if (MsdnReaderSettings.SaveMainWindowBounds)
            {
                MsdnReaderSettings.MainWindowBounds = this.RestoreBounds;
                MsdnReaderSettings.MainWindowState = this.WindowState;
            }
            else
            {
                MsdnReaderSettings.MainWindowBounds = Rect.Empty;
                MsdnReaderSettings.MainWindowState = WindowState.Normal;
            }
        }

        /// <summary>
        /// Override for KeyDown event. Application authors can put app-specific global key handling here
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Custom key handling for main page
            if (!e.Handled)
            {
                if (e.KeyboardDevice.Modifiers == ModifierKeys.None)
                {
                    switch (e.Key)
                    {
                        case Key.Oem2:
                            OnOem2KeyPress(e);
                            break;
                        case Key.F9:
                            OnF9KeyPress(e);
                            break;
                        case Key.Escape:
                            OnEscapeKeyPress(e);
                            break;
                        case Key.F11:
                            OnF11KeyPress(e);
                            break;
                        case Key.F12:
                            OnF12KeyPress(e);
                            break;
                        default:
                            break;
                    }
                }
            }

            // Next, call application-wide input handler
            if (!e.Handled)
            {
                MsdnServiceProvider.ApplicationInputHandler.OnKeyDown(e);
            }

            // Finally, call base if not handled
            if (!e.Handled)
            {
                base.OnKeyDown(e);
            }
        }

        /// <summary>
        /// On Oem2 press, move focus to Search text box. If focus move was successful, set handled to true to prevent further handling
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void OnOem2KeyPress(KeyEventArgs e)
        {
            if (MoveFocusToSearch())
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// On F12 key press, switch navigation UI visibility
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void OnF12KeyPress(KeyEventArgs e)
        {
            // Update viewing mode based on navigation UI visibility
            UpdateViewingModeNavUI();
            e.Handled = true;
        }

        /// <summary>
        /// On F11 key press, switch full screen mode
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void OnF11KeyPress(KeyEventArgs e)
        {
            // Update viewing mode based on full screen setting
            UpdateViewingModeFullScreen();
            e.Handled = true;
        }

        /// <summary>
        /// On escape, go back to normal screen and navigation ui
        /// </summary>
        /// <param name="e">Event args describing the event</param>
        private void OnEscapeKeyPress(KeyEventArgs e)
        {
            // Restore viewing mode
            RestoreViewingMode();
            e.Handled = true;
        }

        /// <summary>
        /// Cycle viewing mode on F9 key press
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void OnF9KeyPress(KeyEventArgs e)
        {
            CycleViewingMode();
            e.Handled = true;
        }

        /// <summary>
        /// Turn off full screen, make navigation UI visibile
        /// </summary>
        private void RestoreViewingMode()
        {
            SwitchFullScreenMode(false);
            SwitchNavigationUIVisibility(true);
            this.viewingMode = ViewingMode.NormalScreenNoNavUI;
        }

        /// <summary>
        /// Cycles the viewing mode to the next value
        /// </summary>
        private void CycleViewingMode()
        {
            ViewingMode nextViewingMode = GetNextViewingMode(this.viewingMode);
            switch (nextViewingMode)
            {
                case ViewingMode.FullScreenNavUI:
                    SwitchFullScreenMode(true);
                    SwitchNavigationUIVisibility(true);
                    break;
                case ViewingMode.FullScreenNoNavUI:
                    SwitchFullScreenMode(true);
                    SwitchNavigationUIVisibility(false);
                    break;
                case ViewingMode.NormalScreenNoNavUI:
                    SwitchFullScreenMode(false);
                    SwitchNavigationUIVisibility(false);
                    break;
                case ViewingMode.NormalScreenNavUI:
                    SwitchFullScreenMode(false);
                    SwitchNavigationUIVisibility(true);
                    break;
                default:
                    break;
            }
            this.viewingMode = nextViewingMode;
        }

        /// <summary>
        /// Switches viewing mode based on navigation UI visiblity, and toggles the visibilty of navigation UI
        /// </summary>
        private void UpdateViewingModeNavUI()
        {
            switch (this.viewingMode)
            {
                case ViewingMode.FullScreenNavUI:
                    SwitchNavigationUIVisibility(false);
                    this.viewingMode = ViewingMode.FullScreenNoNavUI;
                    break;
                case ViewingMode.NormalScreenNavUI:
                    SwitchNavigationUIVisibility(false);
                    this.viewingMode = ViewingMode.NormalScreenNoNavUI;
                    break;
                case ViewingMode.FullScreenNoNavUI:
                    SwitchNavigationUIVisibility(true);
                    this.viewingMode = ViewingMode.FullScreenNavUI;
                    break;
                case ViewingMode.NormalScreenNoNavUI:
                    SwitchNavigationUIVisibility(true);
                    this.viewingMode = ViewingMode.NormalScreenNavUI;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Switches viewing mode based on full screen setting, and toggles the visibilty of navigation UI
        /// </summary>
        private void UpdateViewingModeFullScreen()
        {
            switch (this.viewingMode)
            {
                case ViewingMode.FullScreenNavUI:
                    SwitchFullScreenMode(false);
                    this.viewingMode = ViewingMode.NormalScreenNavUI;
                    break;
                case ViewingMode.NormalScreenNavUI:
                    SwitchFullScreenMode(true);
                    this.viewingMode = ViewingMode.FullScreenNavUI;
                    break;
                case ViewingMode.FullScreenNoNavUI:
                    SwitchFullScreenMode(false);
                    this.viewingMode = ViewingMode.NormalScreenNoNavUI;
                    break;
                case ViewingMode.NormalScreenNoNavUI:
                    SwitchFullScreenMode(true);
                    this.viewingMode = ViewingMode.FullScreenNoNavUI;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Get the next viewing mode for the given value
        /// </summary>
        /// <param name="viewingMode">Current viewing mode</param>
        /// <returns>Next viewing mode</returns>
        private ViewingMode GetNextViewingMode(ViewingMode viewingMode)
        {
            ViewingMode nextViewingMode = ViewingMode.NormalScreenNoNavUI;
            switch (viewingMode)
            {
                case ViewingMode.FullScreenNavUI:
                    nextViewingMode = ViewingMode.FullScreenNoNavUI;
                    break;
                case ViewingMode.FullScreenNoNavUI:
                    nextViewingMode = ViewingMode.NormalScreenNoNavUI;
                    break;
                case ViewingMode.NormalScreenNoNavUI:
                    nextViewingMode = ViewingMode.NormalScreenNavUI;
                    break;
                case ViewingMode.NormalScreenNavUI:
                    nextViewingMode = ViewingMode.FullScreenNavUI;
                    break;
                default:
                    break;
            }
            return nextViewingMode;
        }

        /// <summary>
        /// True if navigation UI is visible, helper that ORs two viewing mode values
        /// </summary>
        private bool NavigationUIVisible
        {
            get { return (this.viewingMode == ViewingMode.FullScreenNavUI || this.viewingMode == ViewingMode.NormalScreenNavUI); }
        }

        /// <summary>
        /// True if full screen mode is on, helper that ORs two viewing mode values
        /// </summary>
        private bool FullScreenOn
        {
            get { return (this.viewingMode == ViewingMode.FullScreenNavUI || this.viewingMode == ViewingMode.FullScreenNoNavUI); }
        }

        /// <summary>
        /// Moves the current focus to the SearchControl for this page
        /// </summary>
        /// <returns>True if focus move succeeded</returns>
        private bool MoveFocusToSearch()
        {
            // Check that we're in a mode that allows search visibility
            if (this.SearchControl != null && this.NavigationUIVisible)
            {
                return this.SearchControl.MoveFocusToSearch();
            }
            return false;
        }

        /// <summary>
        /// Switches full screen mode on or off
        /// </summary>
        /// <param name="on">If true, full screen mode is on, otherwise, it's turned off</param>
        private void SwitchFullScreenMode(bool fullScreen)
        {
            // If viewing mode is already in a full screen state, not changes are necessary
            if (fullScreen && !this.FullScreenOn)
            {
                Application.Current.MainWindow.Topmost = true;
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
                windowStyle = Application.Current.MainWindow.WindowStyle;
                Application.Current.MainWindow.WindowStyle = WindowStyle.None;
                this.resizeMode = Application.Current.MainWindow.ResizeMode;
                Application.Current.MainWindow.ResizeMode = ResizeMode.NoResize;
            }
            else if (!fullScreen && this.FullScreenOn)
            {
                Application.Current.MainWindow.Topmost = false;
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                Application.Current.MainWindow.WindowStyle = windowStyle;
                Application.Current.MainWindow.ResizeMode = this.resizeMode;
            }
        }

        /// <summary>
        /// Switches navigation UI visibility on or off
        /// </summary>
        /// <param name="on">If true, navigation UI is visible, if false, visibility is collapsed</param>
        private void SwitchNavigationUIVisibility(bool visible)
        {
            if (visible && !this.NavigationUIVisible)
            {
                SetNavigationUIVisibility(Visibility.Visible);
            }
            else if (!visible && this.NavigationUIVisible)
            {
                SetNavigationUIVisibility(Visibility.Collapsed);
            }
        }

        /// <summary>
        /// Sets visiblity of UI considered navigation UI - nav panel, section header and search 
        /// </summary>
        /// <param name="visibility">Specified visibility value</param>
        private void SetNavigationUIVisibility(Visibility visibility)
        {
            this.SyncSearchStackPanel.Visibility = visibility;
            this.SectionLabel.Visibility = visibility;
            this.NavigationBorder.Visibility = visibility;
        }

        /// <summary>
        /// Non-static handler for BrowseForward command
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void OnBrowseForward(ExecutedRoutedEventArgs e)
        {
            if (MsdnReaderSettings.EnableJournalNavigationOnInput)
            {
                if (this.CanGoForward)
                {
                    this.GoForward();
                }
            }
            else
            {
                // Check original source, if journal navigation on input is disabled, only navigate through journal if source is a navigation button
                // on main page
                if (e.OriginalSource == this.BrowseForwardButton)
                {
                    if (this.CanGoForward)
                    {
                        this.GoForward();
                    }
                }
                else
                {
                    if (ServiceProvider.ViewManager.NavigationCommands.NextStoryCommand.CanExecute(null))
                    {
                        ServiceProvider.ViewManager.NavigationCommands.NextStoryCommand.Execute(null);
                    }
                }
            }
        }

        /// <summary>
        /// Handler for scroll page up command
        /// </summary>
        /// <param name="e">Event args describing the executed event</param>
        private void OnScrollPageUp(ExecutedRoutedEventArgs e)
        {
            // On scroll commands, there is no need to check for journal navigation since these commands do not initiate journal navigation in any case

            // Scroll page commands directionally specify the way the content moves on a scrolling surface. So a scroll up is actually a
            // page/section/story down and vice vera
            if (ServiceProvider.ViewManager.CurrentNavigator is StoryNavigator)
            {
                StoryViewer storyViewer = ServiceProvider.ViewManager.CurrentVisual as StoryViewer;

                // Translate to story viewer's previous page command
                if (storyViewer != null)
                {
                    System.Windows.Input.NavigationCommands.PreviousPage.Execute(null, storyViewer);
                }
            }
            else if (ServiceProvider.ViewManager.CurrentNavigator is SectionNavigator)
            {
                // On a section, try previous section command
                ServiceProvider.ViewManager.NavigationCommands.PreviousSectionCommand.Execute(null);
            }
        }

        /// <summary>
        /// Handler for scroll page down command
        /// </summary>
        /// <param name="e">Event args describing the executed event</param>
        private void OnScrollPageDown(ExecutedRoutedEventArgs e)
        {
            // On scroll commands, there is no need to check for journal navigation since these commands do not initiate journal navigation in any case

            // Scroll page commands directionally specify the way the content moves on a scrolling surface. So a scroll up is actually a
            // page/section/story down and vice vera
            if (ServiceProvider.ViewManager.CurrentNavigator is StoryNavigator)
            {
                StoryViewer storyViewer = ServiceProvider.ViewManager.CurrentVisual as StoryViewer;

                // Translate to story viewer's previous page command
                if (storyViewer != null)
                {
                    System.Windows.Input.NavigationCommands.NextPage.Execute(null, storyViewer);
                }
            }
            else if (ServiceProvider.ViewManager.CurrentNavigator is SectionNavigator)
            {
                // On a section, try previous section command
                ServiceProvider.ViewManager.NavigationCommands.NextSectionCommand.Execute(null);
            }
        }

        /// <summary>
        /// Command handler for BrowseBack
        /// </summary>
        /// <param name="sender">Source of the command</param>
        /// <param name="e">EventArgs describing the event</param>
        private void OnBrowseBack(ExecutedRoutedEventArgs e)
        {
            if (MsdnReaderSettings.EnableJournalNavigationOnInput)
            {
                if (this.CanGoBack)
                {
                    this.GoBack();
                }
            }
            else
            {
                // Check original source, if journal navigation on input is disabled, only navigate through journal if source is a navigation button
                // on main page
                if (e.OriginalSource == this.BrowseBackButton)
                {
                    if (this.CanGoBack)
                    {
                        this.GoBack();
                    }
                }
                else
                {
                    if (ServiceProvider.ViewManager.NavigationCommands.PreviousStoryCommand.CanExecute(null))
                    {
                        ServiceProvider.ViewManager.NavigationCommands.PreviousStoryCommand.Execute(null);
                    }
                }
            }
        }


        /// <summary>
        /// Command handler for BrowseForward CanExecute
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void OnBrowseForwardCanExecute(CanExecuteRoutedEventArgs e)
        {
            if (MsdnReaderSettings.EnableJournalNavigationOnInput)
            {
                e.CanExecute = this.CanGoForward;
            }
            else
            {
                // Check original source, if journal navigation on input is disabled, only navigate through journal if source is a navigation button
                // on main page
                if (e.OriginalSource == this.BrowseForwardButton)
                {
                    e.CanExecute = this.CanGoForward;
                }
                else
                {
                    e.CanExecute = ServiceProvider.ViewManager.NavigationCommands.NextStoryCommand.CanExecute(null);
                }
            }
        }

        /// <summary>
        /// Command handler for BrowseBack CanExecute
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void OnBrowseBackCanExecute(CanExecuteRoutedEventArgs e)
        {
            if (MsdnReaderSettings.EnableJournalNavigationOnInput)
            {
                e.CanExecute = this.CanGoBack;
            }
            else
            {
                // Check original source, if journal navigation on input is disabled, only navigate through journal if source is a navigation button
                // on main page
                if (e.OriginalSource == this.BrowseBackButton)
                {
                    e.CanExecute = this.CanGoBack;
                }
                else
                {
                    e.CanExecute = ServiceProvider.ViewManager.NavigationCommands.PreviousStoryCommand.CanExecute(null);
                }
            }
        }

        /// <summary>
        /// Event handler for scroll page up executed event
        /// </summary>
        /// <param name="e">Event args describing the event></param>
        private void OnScrollPageUpCanExecute(CanExecuteRoutedEventArgs e)
        {
            // On scroll commands, there is no need to check for journal navigation since these commands do not initiate journal navigation in any case

            // Scroll page commands directionally specify the way the content moves on a scrolling surface. So a scroll up is actually a
            // page/section/story down and vice vera
            if (ServiceProvider.ViewManager.CurrentNavigator is StoryNavigator)
            {
                StoryViewer storyViewer = ServiceProvider.ViewManager.CurrentVisual as StoryViewer;

                // Translate to story viewer's next page command
                if (storyViewer != null)
                {
                    e.CanExecute = System.Windows.Input.NavigationCommands.PreviousPage.CanExecute(null, storyViewer);
                }
            }
            else if (ServiceProvider.ViewManager.CurrentNavigator is SectionNavigator)
            {
                // On a section, try previous section command
                e.CanExecute = ServiceProvider.ViewManager.NavigationCommands.PreviousSectionCommand.CanExecute(null);
            }
        }

        /// <summary>
        /// Event handler for scroll page down executed event
        /// </summary>
        /// <param name="e">Event args describing the event></param>
        private void OnScrollPageDownCanExecute(CanExecuteRoutedEventArgs e)
        {
            // On scroll commands, there is no need to check for journal navigation since these commands do not initiate journal navigation in any case

            // Scroll page commands directionally specify the way the content moves on a scrolling surface. So a scroll up is actually a
            // page/section/story down and vice vera
            if (ServiceProvider.ViewManager.CurrentNavigator is StoryNavigator)
            {
                StoryViewer storyViewer = ServiceProvider.ViewManager.CurrentVisual as StoryViewer;
    
                // Translate to story viewer's previous page command
                if (storyViewer != null)
                {
                    e.CanExecute = System.Windows.Input.NavigationCommands.NextPage.CanExecute(null, storyViewer);
                }
            }
            else if (ServiceProvider.ViewManager.CurrentNavigator is SectionNavigator)
            {
                // On a section, try next section command
                e.CanExecute = ServiceProvider.ViewManager.NavigationCommands.NextSectionCommand.CanExecute(null);
            }
        }

        /// <summary>
        /// Command handler for BrowseBack CanExecute
        /// </summary>
        /// <param name="sender">Source of the command</param>
        /// <param name="e">EventArgs describing the event</param>
        private static void OnNavigationCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            MainWindow mainWindow = sender as MainWindow;
            if (mainWindow != null && !e.Handled)
            {
                if (e.Command == System.Windows.Input.NavigationCommands.BrowseBack)
                { 
                    mainWindow.OnBrowseBack(e);
                }
                else if (e.Command == System.Windows.Input.NavigationCommands.BrowseForward)
                {
                    mainWindow.OnBrowseForward(e);
                }
                else if (e.Command == System.Windows.Input.ComponentCommands.ScrollPageUp)
                {
                    mainWindow.OnScrollPageUp(e);
                }
                else if (e.Command == System.Windows.Input.ComponentCommands.ScrollPageDown)
                {
                    mainWindow.OnScrollPageDown(e);
                }
            }
        }

        /// <summary>
        /// Command handler for CanExecute for RoutedUI Commands interpreted as causing navigation - browse back, browse forward, scroll page up, scroll page down
        /// </summary>
        /// <param name="sender">Source of the command</param>
        /// <param name="e">EventArgs describing the event</param>
        private static void OnNavigationCommandCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            MainWindow mainWindow = sender as MainWindow;
            if (mainWindow != null && !e.Handled)
            {
                if (e.Command == System.Windows.Input.NavigationCommands.BrowseBack)
                {
                    mainWindow.OnBrowseBackCanExecute(e);
                }
                else if (e.Command == System.Windows.Input.NavigationCommands.BrowseForward)
                {
                    mainWindow.OnBrowseForwardCanExecute(e);
                }
                else if (e.Command == System.Windows.Input.ComponentCommands.ScrollPageUp)
                {
                    mainWindow.OnScrollPageUpCanExecute(e);
                }
                else if (e.Command == System.Windows.Input.ComponentCommands.ScrollPageDown)
                {
                    mainWindow.OnScrollPageDownCanExecute(e);
                }
            }
        }

         /// <summary>
        /// Command handler for back navigation key override
        /// </summary>
        /// <param name="sender">Source of the command</param>
        /// <param name="e">EventArgs describing the event</param>
        private static void OnJournalBackNavigationKeyOverride(object sender, ExecutedRoutedEventArgs e)
        {
            MainWindow mainWindow = sender as MainWindow;
            if (mainWindow != null && !e.Handled)
            {
                mainWindow.OnJournalBackNavigationKeyOverride(e);
            }
        }

        /// <summary>
        /// Handler for back navigation key override
        /// </summary>
        /// <param name="e">Event Args describing the event</param>
        private void OnJournalBackNavigationKeyOverride(ExecutedRoutedEventArgs e)
        {
            // On back navigation override, go back regardless of whether journal navigation on input is enabled or not
            if (this.CanGoBack)
            {
                this.GoBack();
            }
        }

        /// <summary>
        /// Custom command to allow overrides of navigation keys if journal navigation on input is disabled but application
        /// authors still want to enable journal navigation on certain keys. This is presently only applied to browse back
        /// command on backspace, but similar logic could be added for forward journal navigation keys
        /// </summary>
        private RoutedCommand backNavigationKeyOverrideCommand = new RoutedCommand();

        /// <summary>
        /// Saved window style if window style was changed to None for full screen mode
        /// </summary>
        private WindowStyle windowStyle = WindowStyle.None;

        /// <summary>
        /// Saved state for resize mode
        /// </summary>
        private ResizeMode resizeMode = ResizeMode.NoResize;

        /// <summary>
        /// Viewing mode for the next view
        /// </summary>
        private ViewingMode viewingMode = ViewingMode.NormalScreenNavUI;
    }
}