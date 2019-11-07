// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SDKTemplateCS;
using Windows.Security.Credentials;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Web;
using Windows.Web.AtomPub;
using Windows.Web.Syndication;

namespace AtomPub
{
    class CommonData
    {
        // The default values for the WordPress site.
        private static string baseUri = "http://<YourWordPressSite>.wordpress.com/";
        private static string user = "";
        private static string password = "";

        // The default Service Document and Edit 'URIs' for WordPress.
        private const string editUri = "./wp-app.php/posts";
        private const string serviceDocUri = "./wp-app.php/service";
        private const string feedUri = "./?feed=atom";

        static public void Restore(Page inputFrame)
        {
            // Set authentication fields.
            (inputFrame.FindName("ServiceAddressField") as TextBox).Text = baseUri;
            (inputFrame.FindName("UserNameField") as TextBox).Text = user;
            (inputFrame.FindName("PasswordField") as PasswordBox).Password = password;
        }

        static public void Save(Page inputFrame)
        {
            // Keep values of authentication fields.
            baseUri = (inputFrame.FindName("ServiceAddressField") as TextBox).Text;
            user = (inputFrame.FindName("UserNameField") as TextBox).Text;
            password = (inputFrame.FindName("PasswordField") as PasswordBox).Password;
        }

        static public AtomPubClient GetClient()
        {
            AtomPubClient client;

            client = new AtomPubClient();
            client.BypassCacheOnRetrieve = true;

            if (!String.IsNullOrEmpty(user) && !String.IsNullOrEmpty(password))
            {
                client.ServerCredential = new PasswordCredential()
                {
                    UserName = user,
                    Password = password
                };
            }
            else 
            {
                client.ServerCredential = null;
            }

            return client;
        }

        static public bool HandleException(Exception exception, TextBox outputField, MainPage rootPage)
        {
            SyndicationErrorStatus status = SyndicationError.GetStatus(exception.HResult);
            if (status != SyndicationErrorStatus.Unknown)
            {
                outputField.Text += "The response content is not valid. " +
                    "Please make sure to use a URI that points to an Atom feed.\r\n";
            }
            else
            {
                WebErrorStatus webError = WebError.GetStatus(exception.HResult);

                if (webError == WebErrorStatus.Unauthorized)
                {
                    outputField.Text += "Incorrect username or password.\r\n";
                }
                else if (webError == WebErrorStatus.Unknown)
                {
                    // Neither a syndication nor a web error.
                    return false;
                }
            }

            rootPage.NotifyUser(exception.Message, NotifyType.ErrorMessage);

            return true;
        }

        static public string EditUri
        {
            get
            {
                return editUri;
            }
        }

        static public string ServiceDocUri
        {
            get
            {
                return serviceDocUri;
            }
        }

        static public string FeedUri
        {
            get
            {
                return feedUri;
            }
        }
    }

    class SyndicationItemIterator
    {
        private SyndicationFeed feed;
        private int index;

        public SyndicationItemIterator()
        {
            this.feed = null;
            this.index = 0;
        }

        public void AttachFeed(SyndicationFeed feed)
        {
            this.feed = feed;
            this.index = 0;
        }

        public void MoveNext()
        {
            if (feed != null && index < feed.Items.Count - 1)
            {
                index++;
            }
        }

        public void MovePrevious()
        {
            if (feed != null && index > 0)
            {
                index--;
            }
        }

        public bool HasElements()
        {
            return feed != null && feed.Items.Count > 0;
        }

        public string GetTitle()
        {
            // Nothing to return yet.
            if (!HasElements())
            {
                return "(no title)";
            }

            if (feed.Items[index].Title != null)
            {
                return WebUtility.HtmlDecode(feed.Items[index].Title.Text);
            }

            return "(no title)";
        }

        public string GetContent()
        {
            // Nothing to return yet.
            if (!HasElements())
            {
                return "(no value)";
            }

            if ((feed.Items[index].Content != null) && (feed.Items[index].Content.Text != null))
            {
                return feed.Items[index].Content.Text;
            }

            return "(no value)";
        }

        public string GetIndexDescription()
        {
            // Nothing to return yet.
            if (!HasElements())
            {
                return "0 of 0";
            }

            return String.Format("{0} of {1}", index + 1, feed.Items.Count);
        }

        public Uri GetEditUri()
        {
            // Nothing to return yet.
            if (!HasElements())
            {
                return null;
            }

            return feed.Items[index].EditUri;
        }

        public SyndicationItem GetSyndicationItem()
        {
            // Nothing to return yet.
            if (!HasElements())
            {
                return null;
            }

            return feed.Items[index];
        }
    }
}
