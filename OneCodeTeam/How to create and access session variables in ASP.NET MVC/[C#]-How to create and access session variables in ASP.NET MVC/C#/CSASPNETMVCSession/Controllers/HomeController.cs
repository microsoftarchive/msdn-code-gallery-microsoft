/****************************** Module Header ******************************\
 * Module Name:  HomeController.cs
 * Project:      CSASPNETMVCSession
 * Copyright (c) Microsoft Corporation.
 * 
 * This sample demonstrates how to use Session in ASPNET MVC. 
 * This class is the Controller for whole project.
 *
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CSASPNETMVCSession.Controllers
{
    public class HomeController : Controller
    {
        #region All Views' names
        enum AllViewsNames
        {
            RazorIndex,
            ASPXIndex
        }

        #endregion

        static AllViewsNames currentViewEnum = AllViewsNames.ASPXIndex;
        // Current view name;
        string strCurrentView = currentViewEnum == AllViewsNames.RazorIndex ? "Index" : "TestPage";

        public ActionResult Index()
        {
            return View(strCurrentView);
        }

        /// <summary>
        /// ActionResult for ordinary session(HttpContext).
        /// </summary>
        /// <param name="sessionValue"></param>
        /// <returns></returns>
        public ActionResult SaveSession(string sessionValue)
        {
            try
            {
                System.Web.HttpContext.Current.Session["sessionString"] = sessionValue;
                return RedirectToAction("LoadSession");
            }
            catch (InvalidOperationException)
            {
                return View(strCurrentView);
            }
        }

        /// <summary>
        /// Load session data and redirect to TestPage.
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadSession()
        {
            LoadSessionObject();
            return View(strCurrentView);
        }

        /// <summary>
        /// ActionResult for Extension method.
        /// </summary>
        /// <param name="sessionValue"></param>
        /// <returns></returns>
        public ActionResult SaveSessionByExtensions(string sessionValue)
        {
            try
            {
                Session.SetDataToSession<string>("key1", sessionValue);
                return RedirectToAction("LoadSession");
            }
            catch (InvalidOperationException)
            {
                return View(strCurrentView);
            }
        }

        /// <summary>
        /// Store the session value to ViewData.
        /// </summary>
        private void LoadSessionObject()
        {
            // Load session from HttpContext.
            ViewData["sessionString"] = System.Web.HttpContext.Current.Session["sessionString"] as String;

            // Load session by Extension method.
            string value = Session.GetDataFromSession<string>("key1");
            ViewData["sessionStringByExtensions"] = value;
        }
    }
}
