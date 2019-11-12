/****************************** Module Header ******************************\
* Module Name: CallWeatherRibbon.cs
* Project:     CSExcelCallWebService
* Copyright(c) Microsoft Corporation.
* 
* The class is the  custom ribbon of excel.
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


using System.Windows.Forms;
using Microsoft.Office.Tools.Ribbon;

namespace CSExcelCallWebService
{
    public partial class CallWeatherRibbon
    {
        private void CallWeatherRibbon_Load(object sender, RibbonUIEventArgs e)
        {

        }

        /// <summary>
        /// Get Weather method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGetWeather_Click(object sender, RibbonControlEventArgs e)
        {
            if (citybox.Text.Trim().Equals(string.Empty) || countrybox.Text.Trim().Equals(string.Empty))
            {
                MessageBox.Show("Please input the city or country name firstly.");
                return;
            }

            // Call web service to get Weather
            Globals.Sheet1.DisplayWebServiceResult(citybox.Text.Trim(), countrybox.Text.Trim());
        }
    }
}
