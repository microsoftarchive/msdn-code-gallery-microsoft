/****************************** Module Header ******************************\
* Module Name:  Default.aspx.cs
* Project:      CSASPNETGroupedGridView
* Copyright (c) Microsoft Corporation
*
* The code sample shows how to group cells in GridView with the same value.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace CSASPNETGroupedGridView
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindSortedTestData(generalGridView);
                BindSortedTestData(groupedGridView);
            }
        }


        /// <summary>
        /// Bind sorted test data to the given GridView control.
        /// </summary>
        /// <param name="gridView">the GridView control</param>
        private void BindSortedTestData(GridView gridView)
        {
            const string TestDataViewStateId = "TestData";
            DataTable dt = ViewState[TestDataViewStateId] as DataTable;

            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add("Product Name", typeof(string));
                dt.Columns.Add("Category", typeof(int));
                dt.Columns.Add("Weight", typeof(double));
                Random r = new Random(DateTime.Now.Millisecond);

                for (int i = 1; i <= 50; i++)
                {
                    // Adding ProductId, Category, and random price.
                    dt.Rows.Add(
                        "Product" + r.Next(1, 5), 
                        r.Next(1, 5), 
                        Math.Round(r.NextDouble() * 100 + 50, 2)
                        );
                }

                ViewState[TestDataViewStateId] = dt;
            }

            // Sort by ProductName and Category
            dt.DefaultView.Sort = "Product Name,Category";
            
            gridView.DataSource = dt;
            gridView.DataBind();
        }


        protected void generalGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            generalGridView.PageIndex = e.NewPageIndex;
        }

        protected void generalGridView_PageIndexChanged(object sender, EventArgs e)
        {
            BindSortedTestData(generalGridView);
        }

        protected void groupedGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            groupedGridView.PageIndex = e.NewPageIndex;
        }

        protected void groupedGridView_PageIndexChanged(object sender, EventArgs e)
        {
            BindSortedTestData(groupedGridView);
        }
    }
}