/****************************** Module Header ******************************\
* Module Name:  DataInMemory.aspx.cs
* Project:      CSASPNETGridView
* Copyright (c) Microsoft Corporation.
* 
* The DataInMemory sample describes how to populate ASP.NET GridView 
* control with simple DataTable and how to implement Insert, Edit, Update, 
* Delete, Paging and Sorting functions in ASP.NET GridView control. The 
* DataTable is stored in ViewState to persist data across postbacks. 
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
#endregion Using directives


namespace CSASPNETGridView
{
    public partial class DataInMemory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // The Page is accessed for the first time.
            if (!IsPostBack)
            {
                // Initialize the DataTable and store it in ViewState.
                InitializeDataSource();

                // Enable the GridView paging option and specify the page size.
                gvPerson.AllowPaging = true;
                gvPerson.PageSize = 5;

                // Enable the GridView sorting option.
                gvPerson.AllowSorting = true;

                // Initialize the sorting expression.
                ViewState["SortExpression"] = "PersonID ASC";

                // Populate the GridView.
                BindGridView();
            }
        }

        // Initialize the DataTable.
        private void InitializeDataSource()
        {
            // Create a DataTable object named dtPerson.
            DataTable dtPerson = new DataTable();

            // Add four columns to the DataTable.
            dtPerson.Columns.Add("PersonID");
            dtPerson.Columns.Add("LastName");
            dtPerson.Columns.Add("FirstName");

            // Specify PersonID column as an auto increment column
            // and set the starting value and increment.
            dtPerson.Columns["PersonID"].AutoIncrement = true;
            dtPerson.Columns["PersonID"].AutoIncrementSeed = 1;
            dtPerson.Columns["PersonID"].AutoIncrementStep = 1;

            // Set PersonID column as the primary key.
            DataColumn[] dcKeys = new DataColumn[1];
            dcKeys[0] = dtPerson.Columns["PersonID"];
            dtPerson.PrimaryKey = dcKeys;

            // Add new rows into the DataTable.
            dtPerson.Rows.Add(null, "Davolio", "Nancy");
            dtPerson.Rows.Add(null, "Fuller", "Andrew");
            dtPerson.Rows.Add(null, "Leverling", "Janet");
            dtPerson.Rows.Add(null, "Dodsworth", "Anne");
            dtPerson.Rows.Add(null, "Buchanan", "Steven");
            dtPerson.Rows.Add(null, "Suyama", "Michael");
            dtPerson.Rows.Add(null, "Callahan", "Laura");

            // Store the DataTable in ViewState. 
            ViewState["dtPerson"] = dtPerson;
        }

        private void BindGridView()
        {
            if (ViewState["dtPerson"] != null)
            {
                // Get the DataTable from ViewState.
                DataTable dtPerson = (DataTable)ViewState["dtPerson"];

                // Convert the DataTable to DataView.
                DataView dvPerson = new DataView(dtPerson);

                // Set the sort column and sort order.
                dvPerson.Sort = ViewState["SortExpression"].ToString();

                // Bind the GridView control.
                gvPerson.DataSource = dvPerson;
                gvPerson.DataBind();
            }
        }

        // GridView.RowDataBound Event
        protected void gvPerson_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Make sure the current GridViewRow is a data row.
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Make sure the current GridViewRow is either 
                // in the normal state or an alternate row.
                if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate)
                {
                    // Add client-side confirmation when deleting.
                    ((LinkButton)e.Row.Cells[1].Controls[0]).Attributes["onclick"] = "if(!confirm('Are you certain you want to delete this person ?')) return false;";
                }
            }
        }

        // GridView.PageIndexChanging Event
        protected void gvPerson_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            // Set the index of the new display page.  
            gvPerson.PageIndex = e.NewPageIndex;

            // Rebind the GridView control to 
            // show data in the new page.
            BindGridView();
        }

        // GridView.RowEditing Event
        protected void gvPerson_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // Make the GridView control into edit mode 
            // for the selected row. 
            gvPerson.EditIndex = e.NewEditIndex;

            // Rebind the GridView control to show data in edit mode.
            BindGridView();

            // Hide the Add button.
            lbtnAdd.Visible = false;
        }

        // GridView.RowCancelingEdit Event
        protected void gvPerson_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // Exit edit mode.
            gvPerson.EditIndex = -1;

            // Rebind the GridView control to show data in view mode.
            BindGridView();

            // Show the Add button.
            lbtnAdd.Visible = true;
        }

        // GridView.RowUpdating Event
        protected void gvPerson_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            if (ViewState["dtPerson"] != null)
            {
                // Get the DataTable from ViewState.
                DataTable dtPerson = (DataTable)ViewState["dtPerson"];

                // Get the PersonID of the selected row.
                string strPersonID = gvPerson.Rows[e.RowIndex].Cells[2].Text;

                // Find the row in DateTable.
                DataRow drPerson = dtPerson.Rows.Find(strPersonID);

                // Retrieve edited values and updating respective items.
                drPerson["LastName"] = ((TextBox)gvPerson.Rows[e.RowIndex].FindControl("TextBox1")).Text;
                drPerson["FirstName"] = ((TextBox)gvPerson.Rows[e.RowIndex].FindControl("TextBox2")).Text;

                // Exit edit mode.
                gvPerson.EditIndex = -1;

                // Rebind the GridView control to show data after updating.
                BindGridView();

                // Show the Add button.
                lbtnAdd.Visible = true;
            }
        }

        // GridView.RowDeleting Event
        protected void gvPerson_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            if (ViewState["dtPerson"] != null)
            {
                // Get the DataTable from ViewState.
                DataTable dtPerson = (DataTable)ViewState["dtPerson"];

                // Get the PersonID of the selected row.
                string strPersonID = gvPerson.Rows[e.RowIndex].Cells[2].Text;

                // Find the row in DateTable.
                DataRow drPerson = dtPerson.Rows.Find(strPersonID);

                // Remove the row from the DataTable.
                dtPerson.Rows.Remove(drPerson);

                // Rebind the GridView control to show data after deleting.
                BindGridView();
            }
        }

        // GridView.Sorting Event
        protected void gvPerson_Sorting(object sender, GridViewSortEventArgs e)
        {
            string[] strSortExpression = ViewState["SortExpression"].ToString().Split(' ');

            // If the sorting column is the same as the previous one, 
            // then change the sort order.
            if (strSortExpression[0] == e.SortExpression)
            {
                if (strSortExpression[1] == "ASC")
                {
                    ViewState["SortExpression"] = e.SortExpression + " " + "DESC";
                }
                else
                {
                    ViewState["SortExpression"] = e.SortExpression + " " + "ASC";
                }
            }
            // If sorting column is another column, 
            // then specify the sort order to "Ascending".
            else
            {
                ViewState["SortExpression"] = e.SortExpression + " " + "ASC";
            }

            // Rebind the GridView control to show sorted data.
            BindGridView();
        }

        protected void lbtnAdd_Click(object sender, EventArgs e)
        {
            // Hide the Add button and showing Add panel.
            lbtnAdd.Visible = false;
            pnlAdd.Visible = true;
        }

        protected void lbtnSubmit_Click(object sender, EventArgs e)
        {
            if (ViewState["dtPerson"] != null)
            {
                // Get the DataTable from ViewState and inserting new data to it.
                DataTable dtPerson = (DataTable)ViewState["dtPerson"];

                // Add the new row.
                dtPerson.Rows.Add(null, tbLastName.Text, tbFirstName.Text);

                // Rebind the GridView control to show inserted data.
                BindGridView();
            }

            // Empty the TextBox controls.
            tbLastName.Text = "";
            tbFirstName.Text = "";

            // Show the Add button and hiding the Add panel.
            lbtnAdd.Visible = true;
            pnlAdd.Visible = false;
        }

        protected void lbtnCancel_Click(object sender, EventArgs e)
        {
            // Empty the TextBox controls.
            tbLastName.Text = "";
            tbFirstName.Text = "";

            // Show the Add button and hiding the Add panel.
            lbtnAdd.Visible = true;
            pnlAdd.Visible = false;
        }

    }
}
