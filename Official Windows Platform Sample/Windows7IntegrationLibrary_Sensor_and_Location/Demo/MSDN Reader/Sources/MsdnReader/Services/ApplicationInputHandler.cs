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

namespace MsdnReader
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Input;
    using Microsoft.SceReader;
    using Microsoft.SceReader.View;
    using System.Windows;
    using Microsoft.SceReader.Controls;
    using Microsoft.SceReader.Data;

    /// <summary>
    /// Global input  handler for the application. Global input handler receives the OnKeyDown event from the main page.
    /// If no control or other UI has handled the event on the route, global application key handling can take over.
    /// It can then take make "application-wide" decisions about what action take based on
    /// application state that individual controls may not have, or if users do not want to customize controls to handle key input.
    /// </summary>
    /// <remarks>
    /// If users wish to pre-empt key handling by individual controls, they may add logic in the PreviewKeyDown. 
    /// method for this class
    /// </remarks>
    public class ApplicationInputHandler
    {
        /// <summary>
        /// Enum describing navigation actions that can be taken by the application
        /// </summary>
        public enum ApplicationNavigationAction
        {
            /// <summary>
            /// Navigate out of a SectionFrontControl or other section display
            /// </summary>
            NavigateFromSection,


            /// <summary>
            /// Navigate out of a StoryViewer or other story display
            /// </summary>
            NavigateFromStory,

            /// <summary>
            /// No navigation action taken
            /// </summary>
            None
        }

        /// <summary>
        /// Application-level handler for preview key down - should be called at the start of the tunneling route, before
        /// any controls receive this notification. This event should be handled in preview only for StoryViewer because we wish
        /// to override it's behavior but in general it is better to let the event bubble up the event route and let controls on the route
        /// handle as required, unless some behavior is explicitly not desired.
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        public void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (!e.Handled)
            {
                // Attempt navigation only if no modifiers, default navigation does not take place with modifiers
                if (e.KeyboardDevice.Modifiers == ModifierKeys.None)
                {
                    // StoryViewer subclasses FlowDocumentPageViewer which will handle left, right and home keys as previous/next page/first page commands
                    // To prevent this and navigate to next/previous story per application requirements, these keys are handled in preview
                    if (e.OriginalSource is StoryViewer)
                    {
                        switch (e.Key)
                        {
                            case Key.Left:
                                NavigateLeft(e);
                                break;
                            case Key.Right:
                                NavigateRight(e);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Application-level navigation handler for key down - checks if key event impacts navigation and takes
        /// necessary action
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        public void OnKeyDown(KeyEventArgs e)
        {
            if (!e.Handled)
            {
                // Key input is handled differently depending on the modifier keys in use
                // Check modifier keys first, then proceed to default actions
                if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                {
                    switch (e.Key)
                    {
                        case Key.R:
                            // On Ctrl + R, navigate to reading list 
                            NavigateToReadingList(e);
                            break;
                        case Key.M:
                            // On Ctrl + M, application will toggle a Story's read/unread state if a Story is available, but the source of the Story is not
                            // a StoryViewer
                            ToggleStoryReadState(e);
                            break;
                        case Key.S:
                            // On Ctrl + S, try to save story to desktop
                            SaveStoryToDesktop(e);
                            break;
                        case Key.P:
                            // On Ctrl + P, try to print a story
                            PrintStory(e);
                            break;
                        case Key.E:
                            // On Ctrl + E, email the story
                            EmailStory(e);
                            break;
                        default:
                            break;
                    }
                } // Alt, Shift, etc may be handled similarly
                else if (e.KeyboardDevice.Modifiers == ModifierKeys.None)
                {
                    // Shortcut keys without modifiers generally initiate navigation, handled here
                    switch (e.Key)
                    {
                        case Key.Left:
                        case Key.J:
                            NavigateLeft(e);
                            break;
                        case Key.Right:
                        case Key.K:
                            NavigateRight(e);
                            break;
                        case Key.Up:
                        case Key.I:
                        case Key.PageUp:
                            NavigateUp(e);
                            break;
                        case Key.Down:
                        case Key.Space:
                        case Key.PageDown:
                            NavigateDown(e);
                            break;
                        case Key.Home:
                            // On Home, navigate to first section. StoryViewer's parent class, FlowDocumentPageViewer, internally handles
                            // this key to go to the first page. This is desirable if stories are long, so this shortcut works
                            // only frmo non-story content. If users require that Home go to the top section on stories also, 
                            // they can add this logic in the PreviewKeyDown method
                            NavigateHome(e);
                            break;
                        case Key.End:
                            NavigateEnd(e);
                            break;
                        case Key.Enter:
                            NavigateToParentSectionOnStory(e);
                            break;
                        case Key.R:
                            // On R, application will try to add a story to the reading list if the current keyboard focus state allows this
                            AddStoryToReadingList(e);
                            break;
                        case Key.A:
                            AnnotateStory(e);
                            break;
                        case Key.C:
                            // On C, application will try to copy a story summary to the clipboard if the current keyboard focus state allows this
                            CopyStorySummary(e);
                            break;
                        case Key.M:
                            // Without Control modifier, M navigates down
                            NavigateDown(e);
                            break;
                        case Key.B:
                            // On B, try to open a Story in the browser depending on current focus state
                            OpenStoryInBrowser(e);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Application-wide handler for MouseWheel event
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        public void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                // StoryViewer sends MouseWheel events to NextPage/PreviousPage commands, which also handle story navigation by default
                // MouseWheel events do need to be handled for section display controls to navigate sections
                if (ServiceProvider.ViewManager.CurrentNavigator is SectionNavigator)
                {
                    NavigateSectionOnMouseWheel(e);
                }
            }
        }

        /// <summary>
        /// Navigates to next/previous section on mouse wheel input
        /// </summary>
        /// <param name="e">EventArgs describing mouse wheel event</param>
        private void NavigateSectionOnMouseWheel(MouseWheelEventArgs e)
        {
            if (!e.Handled)
            {
                if (DoubleUtil.LessThan(e.Delta, 0))
                {
                    GoToNextSection();
                }
                else if (DoubleUtil.GreaterThan(e.Delta, 0))
                {
                    GoToPreviousSection();
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Navigate to end (last section) on key event
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void NavigateEnd(KeyEventArgs e)
        {
            if (ServiceProvider.ViewManager.NavigationCommands.NavigateToLastSectionCommand.CanExecute(null))
            {
                ServiceProvider.ViewManager.NavigationCommands.NavigateToLastSectionCommand.Execute(null);
            }
            e.Handled = true;
        }

        /// <summary>
        /// Navigate to home (first section) on key event
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void NavigateHome(KeyEventArgs e)
        {
            if (ServiceProvider.ViewManager.NavigationCommands.NavigateToFirstSectionCommand.CanExecute(null))
            {
                ServiceProvider.ViewManager.NavigationCommands.NavigateToFirstSectionCommand.Execute(null);
            }
            e.Handled = true;
        }

        /// <summary>
        /// Navigate to a Story's parent section on key input
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void NavigateToParentSectionOnStory(KeyEventArgs e)
        {
            // Check if story is currently displayed
            StoryNavigator storyNavigator = ServiceProvider.ViewManager.CurrentNavigator as StoryNavigator;
            if (storyNavigator != null)
            {
                if (storyNavigator.GetParent() == null)
                {
                    // Story has no parent, navigation will not succeed. Navigate to home section instead
                    if (ServiceProvider.ViewManager.NavigationCommands.NavigateToFirstSectionCommand.CanExecute(null))
                    {
                        ServiceProvider.ViewManager.NavigationCommands.NavigateToFirstSectionCommand.Execute(null);
                    }
                }
                else if (ServiceProvider.ViewManager.NavigationCommands.NavigateToParentSectionCommand.CanExecute(null))
                {
                    ServiceProvider.ViewManager.NavigationCommands.NavigateToParentSectionCommand.Execute(null);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Navigates to previous (left) item or directional focus object if possible on key input
        /// </summary>
        /// <param name="e">EventArgs describing key input</param>
        private void NavigateLeft(KeyEventArgs e)
        {
            ApplicationNavigationAction action = GetApplicationNavigationActionForKeyEvent(e);
            if (action != ApplicationNavigationAction.None)
            {
                switch (action)
                {
                    case ApplicationNavigationAction.NavigateFromSection:
                    case ApplicationNavigationAction.NavigateFromStory:
                        GoToPreviousStory();
                        e.Handled = true;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Navigates to next (right) item or directional focus object if possible on key input
        /// </summary>
        /// <param name="e">EventArgs describing key input</param>
        private void NavigateRight(KeyEventArgs e)
        {
            ApplicationNavigationAction action = GetApplicationNavigationActionForKeyEvent(e);
            if (action != ApplicationNavigationAction.None)
            {
                switch (action)
                {
                    case ApplicationNavigationAction.NavigateFromSection:
                    case ApplicationNavigationAction.NavigateFromStory:
                        GoToNextStory();
                        e.Handled = true;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Navigates to previous (up) item or directional focus object if possible on key input
        /// </summary>
        /// <param name="e">EventArgs describing key input</param>
        private void NavigateUp(KeyEventArgs e)
        {
            ApplicationNavigationAction action = GetApplicationNavigationActionForKeyEvent(e);
            if (action != ApplicationNavigationAction.None)
            {
                switch (action)
                {
                    case ApplicationNavigationAction.NavigateFromSection:
                        GoToPreviousSection();
                        e.Handled = true;
                        break;
                    case ApplicationNavigationAction.NavigateFromStory:
                        // Execute PreviousPage command for StoryViewer, which is configured to go to next story if next page cannot execute
                        StoryViewer storyViewer = Keyboard.FocusedElement as StoryViewer;
                        if (storyViewer != null)
                        {
                            if (System.Windows.Input.NavigationCommands.PreviousPage.CanExecute(null, storyViewer))
                            {
                                System.Windows.Input.NavigationCommands.PreviousPage.Execute(null, storyViewer);    
                            }
                            e.Handled = true;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Navigates to next (down) item or directional focus object if possible on key input
        /// </summary>
        /// <param name="e">EventArgs describing key input</param>
        private void NavigateDown(KeyEventArgs e)
        {
            ApplicationNavigationAction action = GetApplicationNavigationActionForKeyEvent(e);
            if (action != ApplicationNavigationAction.None)
            {
                switch (action)
                {
                    case ApplicationNavigationAction.NavigateFromSection:
                        GoToNextSection();
                        e.Handled = true;
                        break;
                    case ApplicationNavigationAction.NavigateFromStory:
                        // Execute NextPage command for StoryViewer, which is configured to go to next story if next page cannot execute
                        StoryViewer storyViewer = Keyboard.FocusedElement as StoryViewer;
                        if (storyViewer != null)
                        {
                            if (System.Windows.Input.NavigationCommands.NextPage.CanExecute(null, storyViewer))
                            {
                                System.Windows.Input.NavigationCommands.NextPage.Execute(null, storyViewer);
                            }
                            e.Handled = true;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// Navigates to reading list
        /// </summary>
        /// <param name="e">EventArgs describing key input</param>
        private void NavigateToReadingList(KeyEventArgs e)
        {
            Navigator readingListNavigator = ServiceProvider.ViewManager.MasterNavigator.GetReadingListNavigator();
            if (ServiceProvider.ViewManager.NavigationCommands.NavigateToSectionCommand.CanExecute(readingListNavigator))
            {
                ServiceProvider.ViewManager.NavigationCommands.NavigateToSectionCommand.Execute(readingListNavigator);
                e.Handled = true;
            }
        }

        /// <summary>
        /// Gets the appropriate navigation action for key input
        /// </summary>
        /// <returns>Navigation action to perform for the key input</returns>
        private ApplicationNavigationAction GetApplicationNavigationActionForKeyEvent(KeyEventArgs e)
        {
            ApplicationNavigationAction action = ApplicationNavigationAction.None;
            switch (e.Key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                case Key.I:
                case Key.J:
                case Key.K:
                case Key.M:
                case Key.Space:
                case Key.PageDown:
                case Key.PageUp:
                    if (ServiceProvider.ViewManager.CurrentNavigator is SectionNavigator)
                    {
                        if (ServiceProvider.ViewManager.CurrentNavigator is ReadingListNavigator ||
                            ServiceProvider.ViewManager.CurrentNavigator is SearchNavigator)
                        {
                            // For reading list and search UI, the main content visual is not focusable, and won't have the focus
                            // Since this UI is application-based (unlike SectionFrontControl) the application can choose to handle 
                            // navigation keys regardless of what the event source is. For these sections, the application does not
                            // handle up or down keys, since they are not connected to other sections and up and down should
                            // navigate reading list items/ search results. But left and right keys navigate stories
                            if (e.Key == Key.Left || e.Key == Key.Right)
                            {
                                action = ApplicationNavigationAction.NavigateFromSection;
                            }
                        }
                        else
                        {
                            // Sections don't have a fixed template, may be different for search, reading list, etc
                            // In general, check that source is the current content visual, section fronts are a special case because they steal
                            // focus from the current content visual
                            // Application authors may change this code to recognize their custom controls for focus manipulation
                            UIElement source = e.OriginalSource as UIElement;
                            UIElement currentContentVisual = ServiceProvider.ViewManager.CurrentVisual as UIElement;
                            if (currentContentVisual != null && source != null)
                            {
                                if (source == currentContentVisual || source is SectionFrontControl)
                                {
                                    action = ApplicationNavigationAction.NavigateFromSection;
                                }
                            }
                        }
                    }
                    else if (ServiceProvider.ViewManager.CurrentNavigator is StoryNavigator)
                    {
                        // Check that a StoryViewer is focused, since annotation layer may be topmost in stories if user is making notes
                        // In such a case navigation should not take place on key input
                        if (e.OriginalSource is StoryViewer)
                        {
                            action = ApplicationNavigationAction.NavigateFromStory;
                        }
                    }
                    break;
                default:
                    break;
            }
            return action;
        }

        /// <summary>
        /// Logic to navigate to the previous section on receiving key input
        /// </summary>
        private void GoToPreviousSection()
        {
            if (ServiceProvider.ViewManager.NavigationCommands.PreviousSectionCommand.CanExecute(null))
            {
                ServiceProvider.ViewManager.NavigationCommands.PreviousSectionCommand.Execute(null);
            }
        }

        /// <summary>
        /// Logic to navigate to the next section on receiving key input
        /// </summary>
        private void GoToNextSection()
        {
            if (ServiceProvider.ViewManager.NavigationCommands.NextSectionCommand.CanExecute(null))
            {
                ServiceProvider.ViewManager.NavigationCommands.NextSectionCommand.Execute(null);
            }
        }

        /// <summary>
        /// Logic to navigate to the previous story on receiving key input
        /// </summary>
        private void GoToPreviousStory()
        {
            if (ServiceProvider.ViewManager.NavigationCommands.PreviousStoryCommand.CanExecute(null))
            {
                ServiceProvider.ViewManager.NavigationCommands.PreviousStoryCommand.Execute(null);
            }
        }

        /// <summary>
        /// Logic to navigate to the next story on receiving key input
        /// </summary>
        private void GoToNextStory()
        {
            if (ServiceProvider.ViewManager.NavigationCommands.NextStoryCommand.CanExecute(null))
            {
                ServiceProvider.ViewManager.NavigationCommands.NextStoryCommand.Execute(null);
            }
        }


        /// <summary>
        /// Add to reading list on R key press, if possible
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void AddStoryToReadingList(KeyEventArgs e)
        {
            object parameter = GetStoryParameter(e.OriginalSource, false);
            if (parameter != null)
            {
                if (ServiceProvider.ViewManager.PersistenceCommands.AddStoryToReadingListCommand.CanExecute(parameter))
                {
                    ServiceProvider.ViewManager.PersistenceCommands.AddStoryToReadingListCommand.Execute(parameter);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Copy story summary on C key press, if possible
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void CopyStorySummary(KeyEventArgs e)
        {
            object parameter = GetStoryParameter(e.OriginalSource, false);
            if (parameter != null)
            {
                if (ServiceProvider.ViewManager.PersistenceCommands.CopyStoryHtmlEmailSummaryCommand.CanExecute(parameter))
                {
                    ServiceProvider.ViewManager.PersistenceCommands.CopyStoryHtmlEmailSummaryCommand.Execute(parameter);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Open story in Browser on B key press
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void OpenStoryInBrowser(KeyEventArgs e)
        {
            Story story = GetStoryParameter(e.OriginalSource, false) as Story;
            if (story != null)
            {
                Uri uri = null;
                if (Uri.TryCreate(story.WebLink, UriKind.Absolute, out uri))
                {
                    Microsoft.SceReader.FullTrust.ShellExecuteMethods.OpenNewBrowserWindow(uri);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Create text annotation in story on A key press
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void AnnotateStory(KeyEventArgs e)
        {
            // Annotation only enabled within viewer
            StoryViewer storyViewer = GetStoryParameter(e.OriginalSource, true) as StoryViewer;
            if (storyViewer != null)
            {
                if (System.Windows.Annotations.AnnotationService.CreateTextStickyNoteCommand.CanExecute(null, storyViewer))
                {
                    System.Windows.Annotations.AnnotationService.CreateTextStickyNoteCommand.Execute(null, storyViewer);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Save to desktop on Ctrl + S key press, if possible
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void SaveStoryToDesktop(KeyEventArgs e)
        {
            object parameter = GetStoryParameter(e.OriginalSource, true);
            if (parameter != null)
            {
                if (ServiceProvider.ViewManager.PersistenceCommands.SaveStoryToDesktopCommand.CanExecute(parameter))
                {
                    ServiceProvider.ViewManager.PersistenceCommands.SaveStoryToDesktopCommand.Execute(parameter);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Print story on Ctrl + P key press, if possible
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void PrintStory(KeyEventArgs e)
        {
            object parameter = GetStoryParameter(e.OriginalSource, false);
            if (parameter != null)
            {
                if (ServiceProvider.ViewManager.PersistenceCommands.PrintStoryCommand.CanExecute(parameter))
                {
                    ServiceProvider.ViewManager.PersistenceCommands.PrintStoryCommand.Execute(parameter);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Toggle Story read state, if possible on Ctrl + M
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void ToggleStoryReadState(KeyEventArgs e)
        {
            // When getting parameter, get StoryViewer as parameter if there is one
            // Toggling Read state is not permitted from within a StoryViewer since the story is being read at that point
            Story story = GetStoryParameter(e.OriginalSource, true) as Story;
            if (story != null)
            {
                story.Read = !story.Read;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Email story on Ctrl + E key press, if possible
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        private void EmailStory(KeyEventArgs e)
        {
            object parameter = GetStoryParameter(e.OriginalSource, true);
            if (parameter != null)
            {
                if (ServiceProvider.ViewManager.PersistenceCommands.EmailStoryCommand.CanExecute(parameter))
                {
                    ServiceProvider.ViewManager.PersistenceCommands.EmailStoryCommand.Execute(parameter);
                }
                e.Handled = true;
            }
        }

        /// <summary>
        /// Extracts Story info from event source for a key press even
        /// </summary>
        /// <param name="p">Source of the key event</param>
        /// <param name="viewerNeeded">If this param is true, the method should return an StoryViewer that contains the Story, if possible.
        /// If false, the method should return a Story even if its viewer is available
        /// </param>
        /// <returns>Story parameter to add to reading list, if possible </returns>
        private object GetStoryParameter(object p, bool viewerNeeded)
        {
            object parameter = null;
            StoryViewer storyViewer = p as StoryViewer;
            if (storyViewer != null)
            {
                if (viewerNeeded)
                {
                    parameter = storyViewer;
                }
                else
                {
                    parameter = storyViewer.Story;
                }
            }
            else
            {
                parameter = GetStoryFromCommandSource(p);
            }
            return parameter;
        }

        /// <summary>
        /// On SectionFronts, objects causing navigation to stories are command sources for navigation commands. These objects
        /// may also allow addition to reading list and other operations on the Story. This method extracts Story paramter info 
        /// from such an object. 
        /// </summary>
        /// <param name="p">Event source that may provide Story info</param>
        /// <returns>Story from the command info for this ICommandSource, if it exists</returns>
        /// <remarks>
        /// This code extracts the Story from a command source based on an understanding of where
        /// commands are attached in application UI. It will NOT work if commands in UI are changed. Application authors should
        /// use this as a sample and change as necessary
        /// </remarks>
        private object GetStoryFromCommandSource(object p)
        {
            ICommandSource source = p as ICommandSource;
            Story parameter = null;
            if (source != null)
            {
                // This code relies on the fact that a) buttons on section fronts are command sources for story navigation and 
                // b) Knowledge of the specific command attached to them and how to extract parameter information for it
                // If application developers change this logic in their styles, this will need to be updated
                NavigateToStoryRelativeCommand command = source.Command as NavigateToStoryRelativeCommand;
                if (command != null)
                {
                    parameter = source.CommandParameter as Story;
                }
            }
            return parameter;
        }
    }
}