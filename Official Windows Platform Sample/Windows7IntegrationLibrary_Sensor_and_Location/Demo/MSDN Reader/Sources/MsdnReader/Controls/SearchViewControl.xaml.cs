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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Automation.Peers;
using Microsoft.SceReader;

namespace MsdnReader
{
    /// <summary>
    /// Interaction logic for SearchViewControl.xaml
    /// </summary>
    public partial class SearchViewControl : System.Windows.Controls.UserControl
    {
        /// <summary>
        /// Constructor - Initializes element
        /// </summary>
        public SearchViewControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Moves focus to search results
        /// </summary>
        public void MoveFocusToSearchResults()
        {
            // Focus on results view
            if (this.listView.Visibility == Visibility.Visible && this.listView.IsEnabled)
            {
                this.listView.Focus();
                this.listView.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        /// <summary>
        /// Returns special automation peer for this element
        /// </summary>
        /// <returns></returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SearchViewControlAutomationPeer(this);
        }

        /// <summary>
        /// AutomationPeer for SearchViewControl for UIAutomation accessibility
        /// </summary>
        private class SearchViewControlAutomationPeer : FrameworkElementAutomationPeer
        {
            public SearchViewControlAutomationPeer(SearchViewControl owner):
                base(owner)
            {
            }

            protected override AutomationControlType GetAutomationControlTypeCore()
            {
                return AutomationControlType.Pane;
            }

            protected override string GetClassNameCore()
            {
                return "SearchViewControl";
            }
        }

    }
}