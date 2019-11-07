/****************************** Module Header ******************************\
* Module Name:    Default.aspx.cs
* Project:        CSASPNETPagingGridViewMaintainCheckBox
* Copyright (c) Microsoft Corporation
*
* This demo is mainly showing you how to make the CheckBox in the each row of 
* GridView "keep its state" when paging.
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
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CSASPNETPagingGridViewMaintainCheckBox
{
    public partial class Default : System.Web.UI.Page
    {
        // A list to store check state of CheckBox.
        List<bool> isChecked = null;

        /// <summary>
        /// Initializing to bind with the generated data table.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                MyDataBind();
            }
        }

        /// <summary>
        /// Create a dynamic datatable and store it into ViewState 
        /// for further use.
        /// </summary>
        private void MyDataBind()
        {
            if (ViewState["dt"] == null)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Name", typeof(string));
                isChecked = new List<bool>();

                for (int i = 1; i < 41; ++i)
                {
                    dt.Rows.Add(i, "Name" + i);
                    isChecked.Add(false);
                }
                ViewState["dt"] = dt;
                ViewState["CheckList"] = isChecked;
            }

            gvData.DataSource = ViewState["dt"] as DataTable;
            gvData.DataBind();
            isChecked = ViewState["CheckList"] as List<bool>;
        }

        /// <summary>
        /// Change the current GridView's PageIndex
        /// </summary>
        protected void gvData_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvData.PageIndex = e.NewPageIndex;
        }

        /// <summary>
        /// After changing the current GridView's PageIndex, rebind to the GridView.
        /// </summary>
        protected void gvData_PageIndexChanged(object sender, EventArgs e)
        {
            MyDataBind();
            gvData.SelectedIndex = -1;
        }

        /// <summary>
        /// When choosing a CheckBox button, get the selected row's primary key's value.      
        /// </summary> 
        protected void chbChoice_CheckedChanged(object sender, EventArgs e)
        {
            GridViewRow gr = ((Control)sender).NamingContainer as GridViewRow;  
            isChecked = ViewState["CheckList"] as List<bool>;
            isChecked[gr.RowIndex + gvData.PageIndex * gvData.PageSize] = true;
            ViewState["CheckList"] = isChecked;
        }


        /// <summary>
        /// According to the List<Bool> that been saved in ViewState to set the state of current row's CheckBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void gvData_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            isChecked = ViewState["CheckList"] as List<bool>;                 

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chk = (CheckBox)e.Row.FindControl("chbChoice");
                chk.Checked = isChecked[e.Row.RowIndex + gvData.PageIndex * gvData.PageSize];
            }
        }
    }
}