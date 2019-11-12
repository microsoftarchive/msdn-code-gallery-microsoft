/****************************** Module Header ******************************\
* Module Name: Sheet1.cs
* Project:     CSExcelCallWebService
* Copyright(c) Microsoft Corporation.
* 
* This Class calls web service to get information about the weather.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/



using System;
using System.Linq;
using System.Xml.Linq;
using CSExcelCallWebService.WeatherWebService;
using Microsoft.Office.Tools.Excel;
using Excel = Microsoft.Office.Interop.Excel;

namespace CSExcelCallWebService
{
    public partial class Sheet1
    {

        private void Sheet1_Startup(object sender, System.EventArgs e)
        {
            // Add a new Name Range control to cell A1-A8
            // Bind data to the NameRange control 
            this.Controls.AddNamedRange(this.Range["A1", "A8"], "Data");
        }

        private void Sheet1_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(this.Sheet1_Startup);
            this.Shutdown += new System.EventHandler(this.Sheet1_Shutdown);

        }

        #endregion

        /// <summary>
        ///  Call Web service and display the results to the NameRange control
        /// </summary>
        /// <param name="city">Search City</param>
        /// <param name="country">Search Country</param>
        public void DisplayWebServiceResult(string city, string country)
        {
            // Get Name Range and Clear current display
            NamedRange range = (NamedRange)this.Controls["Data"];
            range.Clear();

            // Initialize the value of x 
            int x = 0;

            try
            {
                // Initialize a new instance of Service Client 
                using (GlobalWeatherSoapClient weatherclien = new GlobalWeatherSoapClient())
                {
                    // Call Web service method to Get Weather Data
                    string xmlweatherresult = weatherclien.GetWeather(city, country);

                    // Load an XElement from a string that contains XML data
                    var xmldata = XElement.Parse(xmlweatherresult);

                    // Query the Name and value of Weather
                    var query = from weather in xmldata.Elements()
                                select new
                                {
                                    weather.Name,
                                    weather.Value
                                };

                    if (query.Count() > 0)
                    {
                        foreach (var item in query)
                        {
                            // Use RefersToR1C1 property to change the range that a NameRange control refers to
                            range.RefersToR1C1 = String.Format("=R1C1:R{0}C2", query.Count());

                            // Update data  in range.
                            // Excel uses 1 as the base for index.
                            ((Excel.Range)range.Cells[x + 1, 1]).Value2 = item.Name.ToString();
                            ((Excel.Range)range.Cells[x + 1, 2]).Value2 = item.Value.ToString();
                            x++;
                            if (x == query.Count() - 1)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
                this.Range["A1"].Value2 = "Input City or Country is error, Please check them again";

                // -16776961 is represent for red
                this.Range["A1"].Font.Color = -16776961;
            }
        }
    }
}
