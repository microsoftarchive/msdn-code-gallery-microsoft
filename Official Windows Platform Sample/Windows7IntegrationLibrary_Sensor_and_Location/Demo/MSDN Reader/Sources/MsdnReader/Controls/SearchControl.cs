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
    using System.Windows.Controls;
    using System.Windows;
    using Microsoft.SceReader;
    using Microsoft.SceReader.View;
    using System.Windows.Input;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for Search control in main application chrome
    /// </summary>
    [TemplatePart(Name = "PART_SearchTextBox", Type = typeof(TextBox))]
    public class SearchControl : Control
    {
        /// <summary>
        /// OnApplyTemplate override searches for TextBox in control's template
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (this.Template != null)
            {
                this.searchTextBox = this.Template.FindName("PART_SearchTextBox", this) as TextBox;
            }
        }

        /// <summary>
        /// Override for KeyDown event
        /// </summary>
        /// <param name="e">EventArgs describing the event</param>
        protected override void OnPreviewKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (!e.Handled)
            {
                if (e.KeyboardDevice.Modifiers == ModifierKeys.None)
                {
                    switch (e.Key)
                    {
                        case Key.Down:
                            if (this.searchTextBox != null && e.OriginalSource == this.searchTextBox)
                            {
                                // On down key, move focus to SearchViewControl, if possible
                                if (MoveFocusToSearchView())
                                {
                                    e.Handled = true;
                                }
                            }

                            break;
                        default:
                            break;
                    }
                }
            }

            if (!e.Handled)
            {
                base.OnPreviewKeyDown(e);
            }
        }

        /// <summary>
        /// Moves focus to the SearchViewControl
        /// </summary>
        /// <returns>True if focus move succeeded</returns>
        /// <remarks>The SearchViewControl is the application's default data template for viewing search results. This control has custom logic
        /// to move focus to search results. Search control in MainWindow UI interacts with this control to focus on search results 
        /// when the Down key is hit. If SearchViewControl is not used as the search result data template, this code path will need to
        /// be changed</remarks>
        private bool MoveFocusToSearchView()
        {
            bool focusMoved = false;
            if (ServiceProvider.ViewManager.CurrentNavigator is SearchNavigator)
            {
                SearchViewControl searchViewControl = ServiceProvider.ViewManager.CurrentVisual as SearchViewControl;
                if (searchViewControl != null)
                {
                    searchViewControl.MoveFocusToSearchResults();
                    focusMoved = true;
                }
            }
            else if (ServiceProvider.ViewManager.CurrentVisual != null)
            {
                UIElement element = ServiceProvider.ViewManager.CurrentVisual as UIElement;
                if (element != null)
                {
                    // Otherwise, focus on current visual, whatever it is
                    element.Focus();
                    focusMoved = true;
                }
            }
            return focusMoved;
        }

        /// <summary>
        /// Moves focus to the search TextBox
        /// </summary>
        /// <returns>
        /// True if the search text box could be successfully focused
        /// </returns>
        public bool MoveFocusToSearch()
        {
            if (this.searchTextBox != null)
            {
                this.searchTextBox.Focus();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Search text box that should be specified in this control's template
        /// </summary>
        private TextBox searchTextBox;
    }
}