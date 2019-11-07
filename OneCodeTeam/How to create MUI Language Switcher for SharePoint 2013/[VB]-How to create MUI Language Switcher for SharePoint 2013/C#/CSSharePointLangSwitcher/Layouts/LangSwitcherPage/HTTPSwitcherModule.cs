/****************************** Module Header ******************************\
* Module Name:    HTTPSwitcherModule.cs
* Project:        CSSharePointLangSwitcher
* Copyright (c) Microsoft Corporation
*
* This custom HttpModule is used to apply the new Culture.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Web;
using System.Web.UI;
using Microsoft.SharePoint;
using System.Threading;
using Microsoft.SharePoint.Utilities;

namespace CSSharePointLangSwitcher.LangSwitcherPage
{
    public class HTTPSwitcherModule : IHttpModule
    {
        /// <summary>
        /// You will need to configure this module in the web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: http://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
        }

        /// <summary>
        /// Init event
        /// </summary>
        /// <param name="context"></param>
        public void Init(HttpApplication context)
        {
            // Below is an example of how you can handle Request event and provide 
            // custom logging implementation for it
            context.PreRequestHandlerExecute += context_PreRequestHandlerExecute;
        }

        #endregion

        /// <summary>
        /// Assuming the selected language is stored in a cookie. Firstly, get the selected
        /// language from cookie. Then add the selected language to the request header. 
        /// Finally, use the selected language for the current culture.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void context_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            // Get current application.
            HttpApplication httpApp = sender as HttpApplication;

            // Get all HTTP-specific information about current HTTP request.
            HttpContext context = httpApp.Context;

            // Current language.
            string strLanguage = string.Empty;

            // The key of current selected language in the cookies.
            string strKeyName = "LangSwitcher_Setting";

            try
            {
                // Set the current language.
                if (httpApp.Request.Cookies[strKeyName] != null)
                {
                    strLanguage = httpApp.Request.Cookies[strKeyName].Value;
                }
                else
                {
                    strLanguage = "en-us";
                }
              
                var lang = context.Request.Headers["Accept-Language"];              
                if (lang != null)
                {
                    if (!lang.Contains(strLanguage))
                        context.Request.Headers["Accept-Language"] = strLanguage + "," + context.Request.Headers["Accept-Language"];

                    var culture = new System.Globalization.CultureInfo(strLanguage);

                    // Apply the culture.
                    SPUtility.SetThreadCulture(culture, culture);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}
