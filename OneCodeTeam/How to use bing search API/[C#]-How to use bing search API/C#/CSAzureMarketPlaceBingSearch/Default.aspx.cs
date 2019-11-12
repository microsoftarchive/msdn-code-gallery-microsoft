/****************************** Module Header ******************************\
* Module Name:	Default.aspx.cs
* Project:		CSAzureMarketPlaceBingSearch
* Copyright (c) Microsoft Corporation.
* 
* 
* Default page.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;

namespace CSAzureMarketPlaceBingSearch
{
    public partial class Default : System.Web.UI.Page
    {
        // Create a Bing container.
        private const string rootUrl = "https://api.datamarket.azure.com/Bing/Search";
        //TODO:Change this account key to yours.
        //Example:
        //AgiyQkKH0B/1OTwW/zXu3hGNc2mU2OGintltk1IqajY=
        private const string AccountKey = "[Account key]";
      
        string market = "en-us";
        protected void Page_Load(object sender, EventArgs e)
        {
           
        }

        /// <summary>
        /// Search for web only.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnWebSearch_Click(object sender, EventArgs e)
        {
            Repeater rptResult=new Repeater();
           
            // This is the query expression.
            string query = tbQueryString.Text;
            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));

            // Configure bingContainer to use your credentials.
            bingContainer.Credentials = new NetworkCredential(AccountKey, AccountKey);

            // Build the query, limiting to 10 results.
            var webQuery =bingContainer.Web(query, null, null, market, null, null, null, null);
            webQuery = webQuery.AddQueryOption("$top", 10);

            // Run the query and display the results.
            var webResults = webQuery.Execute();
            Label lblResults = new Label();
            StringBuilder searchResult = new StringBuilder();

            foreach (Bing.WebResult wResult in webResults)
	        {
                searchResult.Append(string.Format("<a href={2}>{0}</a><br /> {1}<br /> {2}<br /><br />",
                    wResult.Title,
                    wResult.Url,
                    wResult.Description));

	        }
            lblResults.Text = searchResult.ToString();
            Panel1.Controls.Add(lblResults);
           
        }

        /// <summary>
        /// Search for image only.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnImageSearch_Click(object sender, EventArgs e)
        {
            Repeater rptResult = new Repeater();
            string query = tbQueryString.Text;

            // Create a Bing container.
            string rootUrl = "https://api.datamarket.azure.com/Bing/Search";
            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));

            // Configure bingContainer to use your credentials.
            bingContainer.Credentials = new NetworkCredential(AccountKey, AccountKey);

            // Build the query, limiting to 10 results.
            var imageQuery =
                bingContainer.Image(query, null, market, null, null, null, null);
            imageQuery = imageQuery.AddQueryOption("$top", 50);

            // Run the query and display the results.
            var imageResults = imageQuery.Execute();
            StringBuilder searchResult = new StringBuilder();
            Label lblResults = new Label();

            foreach (Bing.ImageResult iResult in imageResults)
            {
                searchResult.Append(string.Format("Image Title: <a href={1}>{0}</a><br />Image Url: {1}<br /><br />",
                    iResult.Title,
                    iResult.MediaUrl));
            }
            lblResults.Text = searchResult.ToString();
            Panel1.Controls.Add(lblResults);
        }

        /// <summary>
        /// Search for video only.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnVideosSearch_Click(object sender, EventArgs e)
        {
            Repeater rptResult = new Repeater();
            string query = tbQueryString.Text;

            // Create a Bing container.
            string rootUrl = "https://api.datamarket.azure.com/Bing/Search";
            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));

            // Configure bingContainer to use your credentials.
            bingContainer.Credentials = new NetworkCredential(AccountKey, AccountKey);

            // Build the query, limiting to 10 results.
            var mediaQuery =
                bingContainer.Video(query, null, market, null, null, null, null, null);
            mediaQuery = mediaQuery.AddQueryOption("$top", 50);

            // Run the query and display the results.
            var mediaResults = mediaQuery.Execute();
            Label lblResults = new Label();
            StringBuilder searchResult = new StringBuilder();

            foreach (Bing.VideoResult vResult in mediaResults)
            {
                searchResult.Append(string.Format("Video Tile: <a href={1}>{0}</a><br />Video URL: {1}<br />",
                    vResult.Title,
                    vResult.MediaUrl));
            }
               lblResults.Text=searchResult.ToString();
               Panel1.Controls.Add(lblResults);
        }

        /// <summary>
        /// Search for news only.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnNewsSearch_Click(object sender, EventArgs e)
        {
            Repeater rptResult = new Repeater();

            string query = tbQueryString.Text;
            // Create a Bing container.
            string rootUrl = "https://api.datamarket.azure.com/Bing/Search";
            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));

            // Get news for science and technology.
            string newsCat = "rt_ScienceAndTechnology";

            // Configure bingContainer to use your credentials.
            bingContainer.Credentials = new NetworkCredential(AccountKey, AccountKey);

            // Build the query, limiting to 10 results.
            var newsQuery =
                bingContainer.News(query, null, market, null, null, null, null, newsCat, null);
            newsQuery = newsQuery.AddQueryOption("$top", 10);

            // Run the query and display the results.
            var newsResults = newsQuery.Execute();

            StringBuilder searchResult = new StringBuilder();
            Label lblResults = new Label();

            foreach (Bing.NewsResult nResult in newsResults)
            {
                searchResult.Append(string.Format("<a href={0}>{1}</a><br /> {2}<br /> {3}&nbsp;{4}<br /><br />",
                nResult.Url,
                nResult.Title,
                nResult.Description,
                nResult.Source,
                nResult.Date));
            }
            lblResults.Text = searchResult.ToString();

            Panel1.Controls.Add(lblResults);
        }

        /// <summary>
        /// Search with spelling suggestion.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnSpellingSuggestionSearch_Click(object sender, EventArgs e)
        {
            string query = tbQueryString.Text;

            // Create a Bing container.
            string rootUrl = "https://api.datamarket.azure.com/Bing/Search";
            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));

            // Configure bingContainer to use your credentials.
            bingContainer.Credentials = new NetworkCredential(AccountKey, AccountKey);

            // Build the query.
            var spellQuery =
                bingContainer.SpellingSuggestions(query, null, market, null, null, null);

            // Run the query and display the results.
            var spellResults = spellQuery.Execute();

            List<Bing.SpellResult> spellResultList = new List<Bing.SpellResult>();

            foreach (var result in spellResults)
            {
                spellResultList.Add(result);
            }

            Label lblResults = new Label();
            if (spellResultList.Count>0)
            {
                lblResults.Text = string.Format(
                "Spelling suggestion is <strong>{0}</strong>",
                spellResultList[0].Value);
            }
            else
            {
                lblResults.Text = "No spelling suggestion. Type some typo key words for suggestion for example \"xbx gamess\"";
            }
            Panel1.Controls.Add(lblResults);

        }

        /// <summary>
        /// Related search.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnRelatedSearch_Click(object sender, EventArgs e)
        {
            string query = tbQueryString.Text;

            // Create a Bing container.
            string rootUrl = "https://api.datamarket.azure.com/Bing/Search";
            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));

            // Configure bingContainer to use your credentials.
            bingContainer.Credentials = new NetworkCredential(AccountKey, AccountKey);

            // Build the query, limiting to 10 results.
            var relatedQuery =
                bingContainer.RelatedSearch(query, null, market, null, null, null);
            relatedQuery = relatedQuery.AddQueryOption("$top", 10);

            // Run the query and display the results.
            var relatedResults = relatedQuery.Execute();

            List<Bing.RelatedSearchResult> relatedSearchResultList = new List<Bing.RelatedSearchResult>();
            Label lblResults = new Label();
            StringBuilder searchResults=new StringBuilder();
            foreach (Bing.RelatedSearchResult rResult in relatedResults)
            {
               searchResults.Append(string.Format("<a href={1}>{0}</a><br /> {1}<br />",
                   rResult.Title,
                   rResult.BingUrl));
            }
                lblResults.Text=searchResults.ToString();
                Panel1.Controls.Add(lblResults);
            }

        /// <summary>
        /// Composite search.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCompositeSearch_Click(object sender, EventArgs e)
        {
            string query = tbQueryString.Text;
            // Create a Bing container.
            string rootUrl = "https://api.datamarket.azure.com/Bing/Search";
            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));

            // The composite operations to use.
            string operations = "web+news";

            // Configure bingContainer to use your credentials.
            bingContainer.Credentials = new NetworkCredential(AccountKey, AccountKey);

            // Build the query, limiting to 5 results (per service operation).
            var compositeQuery =
                bingContainer.Composite(operations, query, null, null, market,
                                        null, null, null, null, null,
                                        null, null, null, null, null);
            compositeQuery = compositeQuery.AddQueryOption("$top", 5);

            // Run the query and display the results.
            var compositeResults = compositeQuery.Execute();

            StringBuilder searchResults = new StringBuilder();

            foreach (var cResult in compositeResults)
            {
                searchResults.Append("<h3>Web Result</h3>");

                // Display web results.
                foreach (var result in cResult.Web)
                {
                   searchResults.Append(string.Format("<a href={2}>{0}</a><br /> {1}<br /> {2}<br /><br />",
                       result.Title,result.Url,result.Description));
                }

                searchResults.Append("<h3>News Result</h3>");

                // Display news results.
                foreach (var result in cResult.News)
                {
                    searchResults.Append(string.Format("<a href={0}>{1}</a><br /> {2}<br /> {3}&nbsp;{4}<br /><br />",
                        result.Url, result.Title, result.Description, result.Source, result.Date));
                }
            }

            Label lblResults = new Label();

            lblResults.Text = searchResults.ToString();
            Panel1.Controls.Add(lblResults);

        }

    }
}