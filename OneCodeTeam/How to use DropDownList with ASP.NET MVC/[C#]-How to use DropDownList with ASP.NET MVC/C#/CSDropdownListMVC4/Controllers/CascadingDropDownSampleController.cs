/*********************************** Module Header ***********************************\
Module Name:  CascadingDropDownSampleController.cs
Project:      CSDropdownListMVC4
Copyright (c) Microsoft Corporation.

ASP.NET MVC 4 is a framework for building scalable, standards-based web applications using well-established design patterns and the power of ASP.NET and the .NET Framework. 
This article and the attached code samples demonstrate demonstrates how to use cascading dropdown list with ASP.NET MVC 4. 

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*************************************************************************************/

#region "Directives"

using CSDropdownListMVC4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

#endregion

namespace CSDropdownListMVC4.Controllers
{
    /// <summary>
    /// Controller class for sample
    /// </summary>
    public class CascadingDropDownSampleController : Controller
    {
        #region "Public Actions"

        /// <summary>
        /// Default Action for the web-page handling HTTP GET requests
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            IDictionary<string, string> makes = GetSampleMakes();
            CascadingDropDownSampleModel viewModel = new CascadingDropDownSampleModel()
            {
                Makes = makes
            };

            return View(viewModel);
        }

        /// <summary>
        /// AJAX Action to send sample Models in JSON format based on the selected make
        /// </summary>
        /// <param name="selectedMake"></param>
        /// <returns></returns>
        public ActionResult GetSampleModels(string selectedMake)
        {
            IDictionary<string, string> models = GetSampleModelsFromSelectedMake(selectedMake);
            return Json(models);
        }

        /// <summary>
        /// AJAX Action to send sample colors in JSON format based on the selected model
        /// </summary>
        /// <param name="selectedModel"></param>
        /// <returns></returns>
        public ActionResult GetSampleColors(string selectedModel)
        {
            IDictionary<string, string> colors = GetSampleColorsFromSelectedModel(selectedModel);
            return Json(colors);
        }

        #endregion

        #region "Private Methods"

        /// <summary>
        /// Method to generate sample makes
        /// </summary>
        /// <returns></returns>
        private IDictionary<string, string> GetSampleMakes()
        {
            IDictionary<string, string> makes = new Dictionary<string, string>();

            makes.Add("1", "Acura");
            makes.Add("2", "Audi");
            makes.Add("3", "BMW");

            return makes;
        }

        /// <summary>
        /// Method to generate sample models based on selected make
        /// </summary>
        /// <param name="selectedMake"></param>
        /// <returns></returns>
        private IDictionary<string, string> GetSampleModelsFromSelectedMake(string selectedMake)
        {
            IDictionary<string, string> models = new Dictionary<string, string>();

            switch (selectedMake)
            {
                case "1":
                    models.Add("1", "Integra");
                    models.Add("2", "RL");
                    models.Add("3", "TL");
                    break;
                case "2":
                    models.Add("4", "A4");
                    models.Add("5", "S4");
                    models.Add("6", "A6");
                    break;
                case "3":
                    models.Add("7", "3 series");
                    models.Add("8", "5 series");
                    models.Add("9", "7 series");
                    break;
                default:
                    throw new NotImplementedException();

            }

            return models;
        }

        /// <summary>
        /// Method to generate sample colors based on selected model
        /// </summary>
        /// <param name="selectedModel"></param>
        /// <returns></returns>
        private IDictionary<string, string> GetSampleColorsFromSelectedModel(string selectedModel)
        {
            IDictionary<string, string> colors = new Dictionary<string, string>();

            switch (selectedModel)
            {
                case "1":
                    colors.Add("1", "Green");
                    colors.Add("2", "Sea Green");
                    colors.Add("3", "Pale Green");
                    break;
                case "2":
                    colors.Add("4", "Red");
                    colors.Add("5", "Bright Red");
                    break;
                case "3":
                    colors.Add("6", "Teal");
                    colors.Add("7", "Dark Teal");
                    break;
                case "4":
                    colors.Add("8", "Azure");
                    colors.Add("9", "Light Azure");
                    colors.Add("10", "Dark Azure");
                    break;
                case "5":
                    colors.Add("11", "Silver");
                    colors.Add("12", "Metallic");
                    break;
                case "6":
                    colors.Add("13", "Cyan");
                    break;
                case "7":
                    colors.Add("14", "Blue");
                    colors.Add("15", "Sky Blue");
                    colors.Add("16", "Racing Blue");
                    break;
                case "8":
                    colors.Add("17", "Yellow");
                    colors.Add("18", "Red");
                    break;
                case "9":
                    colors.Add("17", "Brown");
                    break;
                default:
                    throw new NotImplementedException();

            }

            return colors;
        }

        #endregion

    }
}
