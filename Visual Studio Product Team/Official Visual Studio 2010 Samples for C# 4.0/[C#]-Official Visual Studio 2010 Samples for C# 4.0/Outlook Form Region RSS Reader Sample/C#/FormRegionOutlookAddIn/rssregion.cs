// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Office = Microsoft.Office.Core;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace FormRegionOutlookAddIn
{
    partial class RssRegion
    {
        #region Form Region Factory

        [Microsoft.Office.Tools.Outlook.FormRegionMessageClass(Microsoft.Office.Tools.Outlook.FormRegionMessageClassAttribute.PostRss)]
        [Microsoft.Office.Tools.Outlook.FormRegionName("FormRegionOutlookAddIn.RssRegion")]
        public partial class RssRegionFactory
        {
            // Occurs before the form region is initialized.
            // To prevent the form region from appearing, set e.Cancel to true.
            // Use e.OutlookItem to get a reference to the current Outlook item.
            private void RssRegionFactory_FormRegionInitializing(object sender, Microsoft.Office.Tools.Outlook.FormRegionInitializingEventArgs e)
            {
            }
        }

        #endregion

        // Occurs before the form region is displayed.
        // Use this.OutlookItem to get a reference to the current Outlook item.
        // Use this.OutlookFormRegion to get a reference to the form region.
        private void RssRegion_FormRegionShowing(object sender, System.EventArgs e)
        {
            this.RssRegionSplitContainer.Panel2Collapsed = true;

            Outlook.PostItem rssItem = (Outlook.PostItem)this.OutlookItem;

            this.webBrowserRss.Navigate(Helper.ParseUrl(rssItem));
        }

        // Occurs when the form region is closed.
        // Use this.OutlookItem to get a reference to the current Outlook item.
        // Use this.OutlookFormRegion to get a reference to the form region.
        private void RssRegion_FormRegionClosed(object sender, System.EventArgs e)
        {
        }

        // Clicking on 'SearchSimilarTopicsbutton' will open 'webBrowserSearch' in a Separate Pane.
        private void searchSimilarTopicsButton_Click(object sender, EventArgs e)
        {
            Outlook.PostItem rssItem = (Outlook.PostItem)this.OutlookItem;

            this.searchSimilarTopicsButton.Visible = false;

            this.RssRegionSplitContainer.Panel2Collapsed = false;

            // Search for the matching titles by placing title in "".
            this.webBrowserSearch.Navigate(string.Concat("http://www.bing.com/search?q=\"", rssItem.Subject, "\""));

            this.RssRegionSplitContainer.SplitterDistance = (this.OutlookFormRegion.Inspector.Width / 2);

            this.searchWindowProgressBar.Visible = true;
        }

        // To navigate back to Rss Article with in 'View Article' Pane.
        private void viewRssBackButton_Click(object sender, EventArgs e)
        {
            this.webBrowserRss.GoBack();
        }

        // Event to set 'ViewRssProgressBar' Properties.
        private void webBrowserRss_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            this.viewRssProgressBar.Visible = true;

            this.viewRssProgressBar.Value = 0;
        }

        private void webBrowserRss_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            // 'viewRssProgressBar.Visible' is set in Navigating and DocumentCompleted events
            // in order to keep it updated correctly,Coordinate these three events.

            if (this.viewRssProgressBar.Visible)
            {
                // -1 Indicates download is completed.
                if (e.CurrentProgress == -1)
                {
                    this.viewRssProgressBar.Value = 100;
                }
                else
                {
                    this.viewRssProgressBar.Value = (int)((100 * e.CurrentProgress) / e.MaximumProgress);
                }
            }

        }

        // Event to set 'ViewRssProgressBar' Properties.
        private void webBrowserRss_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.viewRssProgressBar.Visible = false;

            this.viewRssProgressBar.Value = 0;
        }

        // Hide 'Search Results' and expand 'View Article' pane.
        private void hideSearchResultsButton_Click(object sender, EventArgs e)
        {
            this.RssRegionSplitContainer.Panel2Collapsed = true;

            this.searchSimilarTopicsButton.Visible = true;
        }

        // To navigate back to Search Results.
        private void searchResultsBackButton_Click(object sender, EventArgs e)
        {
            this.webBrowserSearch.GoBack();
        }

        // Event to set 'Search ProgressBar' Properties.
        private void webBrowserSearch_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            this.searchWindowProgressBar.Visible = true;

            this.searchWindowProgressBar.Value = 0;
        }

        private void webBrowserSearch_ProgressChanged(object sender, WebBrowserProgressChangedEventArgs e)
        {
            // 'searchWindowProgressBar.Visible' is set in Navigating and DocumentCompleted events
            // in order to keep it updated correctly, we Coordinate these three events.

            if (this.searchWindowProgressBar.Visible)
            {
                // -1 Indicates download is completed.
                if (e.CurrentProgress == -1)
                {
                    this.searchWindowProgressBar.Value = 100;
                }
                else
                {
                    this.searchWindowProgressBar.Value = (int)((100 * e.CurrentProgress) / e.MaximumProgress);
                }
            }
        }

        // Event to set ProgressBar Properties.
        private void webBrowserSearch_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            this.searchWindowProgressBar.Visible = false;

            this.searchWindowProgressBar.Value = 0;
        }
    }
}
