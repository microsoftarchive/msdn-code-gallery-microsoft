/********************************* Module Header **********************************\
* Module Name:	SalesInfo.cs
* Project:		CSEFDeepCloneObject
* Copyright (c) Microsoft Corporation.
* 
* This sample demonstrates how to implement deep clone/duplicate entity objects 
* using serialization and reflection.
* This module is used to show the detail information of the sales.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
* **********************************************************************************/

using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CSEFDeepCloneObject.Properties;

namespace CSEFDeepCloneObject
{
    public partial class SalesInfo : Form
    {
        #region Constructors

        public SalesInfo()
        {
            InitializeComponent();
        }

        #endregion

        #region Event

        private void SalesInfo_Load(object sender, EventArgs e)
        {
            IQueryable<int> empIds = from a in Config.Context.Employees select a.EmpId;
            cbxEmpId.DataSource = empIds;
            cbxYear.DataSource = Config.Years;
        }

        /// <summary>
        /// Save the sales information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Config.ValidateTextBox(this))
            {
                int empId = Convert.ToInt32(cbxEmpId.Text);
                BasicSalesInfo basicSalesInfo =
                    (from a in Config.Context.BasicSalesInfoes 
                     where a.EmpId == empId select a)
                    .FirstOrDefault();

                // If the basicSalesInfo has exist then add the sales 
                // to the total of the basicSalesInfo if not add a new 
                // record to the basicSalesInfo.
                if (basicSalesInfo == null)
                {
                    var bsInfo = new BasicSalesInfo 
                                     { 
                                       EmpId = empId, 
                                       Total = Convert.ToDouble(tbxSales.Text)
                                     };

                    Config.Context.AddToBasicSalesInfoes(bsInfo);
                    Config.Context.SaveChanges();

                    int bsInfoId = (from a in Config.Context.BasicSalesInfoes 
                                    select a.SalInfoId).Max();
                    AddDetailSalsInfo(bsInfoId);
                }
                else
                {
                    basicSalesInfo.Total += Convert.ToDouble(tbxSales.Text);
                    AddDetailSalsInfo(basicSalesInfo.SalInfoId);
                }

                Hide();
                Config.EmpListForm.RefreshData();
            }
        }

        /// <summary>
        /// Ensure that the sales must be decimal and can't be null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbxSales_Validating(object sender, CancelEventArgs e)
        {
            if (!Regex.IsMatch(tbxSales.Text, @"(\d+(\.\d+)?)?") 
                || tbxSales.Text.Trim().Equals(""))
            {
                e.Cancel = true;
                tbxSales.Select(0, tbxSales.Text.Length);
                errorProvider.SetError(tbxSales, Resources.SalesMsg);
            }
        }

        /// <summary>
        /// Clear the error provider's information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbxSales_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(tbxSales, "");
        }

        #endregion

        #region Private Function

        /// <summary>
        /// Save the detail sales information to database.
        /// </summary>
        /// <param name="bsInfoId">The basic sales id of the save item </param>
        private void AddDetailSalsInfo(int bsInfoId)
        {
            var dsInfo = new DetailSalesInfo
                             {
                                 BasicSalInfoId = bsInfoId, 
                                 Sale = Convert.ToDouble(tbxSales.Text), 
                                 Year = Convert.ToDateTime(
                                 string.Format("1/1/{0}", cbxYear.Text)
                                 )
                             };

            Config.Context.AddToDetailSalesInfoes(dsInfo);
            Config.Context.SaveChanges();
        }

        #endregion
    }
}