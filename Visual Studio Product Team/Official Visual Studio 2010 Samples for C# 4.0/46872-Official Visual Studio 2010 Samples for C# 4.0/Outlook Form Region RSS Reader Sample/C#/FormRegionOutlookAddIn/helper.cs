// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace FormRegionOutlookAddIn
{
    static class Helper
    {
        // Return the url of the 'View article' link that appears in the headline of the RSS item. 
        public static string ParseUrl(Outlook.PostItem item)
        {
            const string lookUpText = "HYPERLINK";
            const string articleStr = "View article";
            string body = item.Body;

            int index = body.IndexOf(lookUpText, 0, body.Length);
            int end = 0;
            // Look through body for 'HYPERLINKS' and narrow down to 'View article...' link.
            while (true)
            {
                end = body.IndexOf(articleStr, index, body.Length - index);
                int nextIndex = body.IndexOf(lookUpText, index + 1, body.Length - (index + 1));

                if (nextIndex > index && nextIndex < end)
                {
                    index = nextIndex;
                }
                else
                    break;
            }
            // Get the Link to the article.
            string url = body.Substring(index + lookUpText.Length + 1, end - index - (lookUpText.Length + 1));

            url = url.Trim('"');

            return url;
        }
    }
}
