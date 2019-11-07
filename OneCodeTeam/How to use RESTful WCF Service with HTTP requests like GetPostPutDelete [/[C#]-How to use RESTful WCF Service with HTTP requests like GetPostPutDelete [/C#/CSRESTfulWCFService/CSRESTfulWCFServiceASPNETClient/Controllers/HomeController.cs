/****************************** Module Header ******************************\
Module Name:  HomeController.cs
Project:      CSRESTfulWCFService
Copyright (c) Microsoft Corporation.
	 
HomeController class to Create/Delete/Update/GetAll users
	 
This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/licenses.aspx.
All other rights reserved.
	 
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CSRESTfulWCFServiceASPNETClient.Models;

namespace CSRESTfulWCFServiceASPNETClient.Controllers
{
    /// <summary>
    /// HomeController class
    /// </summary>
    [HandleError]
    public class HomeController : Controller
    {
        #region Fields

        private const string WCFURL = "http://localhost:50500/UserService.svc";
        private RESTContext<User> profile;

        #endregion

        #region Properties

        /// <summary>
        /// Profile property to create RESTContext object
        /// </summary>
        public RESTContext<User> Profile
        {
            get
            {
                if (profile == null)
                {
                    profile = new RESTContext<User>(WCFURL);
                }

                return profile;
            }
        }

        #endregion

        #region Methods: Action

        /// <summary>
        /// Home page: Index
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            List<User> lUser = Profile.GetAll();
            ShowMessage(Profile.StrMessage);

            return View(lUser);
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            return View(Profile.GetAll().Single(u => u.Id == id));
        }

        /// <summary>
        /// Edit
        /// </summary>
        /// <param name="user">User object</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                Profile.Update(user);
                ShowMessage(Profile.StrMessage);

                return RedirectToAction("Index");
            }

            return View(user);
        }

        /// <summary>
        /// Delete
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            Profile.Delete<int>(id);
            ShowMessage(Profile.StrMessage);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create
        /// </summary>
        /// <param name="user">USer object</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                List<User> lUser = Profile.GetAll();
                ShowMessage(Profile.StrMessage);

                if (lUser != null)
                {
                    user.Id = lUser.Count == 0 ? 1 : lUser.Max(u => u.Id) + 1;
                    Profile.Create(user);

                    return RedirectToAction("Index");
                }
            }

            return View(user);
        }

        /// <summary>
        /// Show error message
        /// </summary>
        /// <param name="strMessage">message</param>
        private void ShowMessage(string strMessage)
        {
            TempData["error"] = strMessage;
        }

        #endregion
    }
}
