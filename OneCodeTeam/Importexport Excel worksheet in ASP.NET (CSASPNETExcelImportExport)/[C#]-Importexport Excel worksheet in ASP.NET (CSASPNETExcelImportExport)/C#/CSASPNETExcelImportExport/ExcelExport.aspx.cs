/****************************** Module Header ******************************\
* Module Name:  ExcelExport.aspx.cs
* Project:      CSASPNETExcelImportExport
* Copyright (c) Microsoft Corporation.
* 
* This page retrieves data from SQL Server using a DataTable and then exports
* the DataTable to an Excel 2003/2007 spreadsheet on server disk. 
* 
* There are two options on the page:
* 
* One is for choosing output Excel * Version and the other is for choosing 
* whether to provide the file download link.
*  
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* History:
* 11/24/2009 11:16 AM Jian Kang Created
\***************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Configuration;
#endregion Using directives

namespace CSASPNETExcelImportExport
{
    public partial class ExcelExport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        // Retrieve data from SQL Server table
        protected DataTable RetrieveData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLServer2005DBConnectionString"].ToString()))
            {
                // Initialize a SqlDataAdapter object.
                SqlDataAdapter da = new SqlDataAdapter("select LastName,FirstName,PersonCategory from Person", conn);

                // Fill the DataTable with data from SQL Server table.
                da.Fill(dt);
            }

            return dt;
        }

        // Export data to an Excel spreadsheet via ADO.NET
        protected void ExportToExcel(string strConn, DataTable dtSQL)
        {
            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                // Create a new sheet in the Excel spreadsheet.
                OleDbCommand cmd = new OleDbCommand("create table Person(LastName varchar(50), FirstName varchar(50),PersonCategory varchar(50))", conn);

                // Open the connection.
                conn.Open();

                // Execute the OleDbCommand.
                cmd.ExecuteNonQuery();

                cmd.CommandText = "INSERT INTO Person (LastName, FirstName,PersonCategory) values (?,?,?)";

                // Add the parameters.
                cmd.Parameters.Add("LastName", OleDbType.VarChar, 50, "LastName");
                cmd.Parameters.Add("FirstName", OleDbType.VarChar, 50, "FirstName");
                cmd.Parameters.Add("PersonCategory", OleDbType.VarChar, 50, "PersonCategory");

                // Initialize an OleDBDataAdapter object.
                OleDbDataAdapter da = new OleDbDataAdapter("select * from Person", conn);

                // Set the InsertCommand of OleDbDataAdapter, 
                // which is used to insert data.
                da.InsertCommand = cmd;

                // Changes the Rowstate()of each DataRow to Added,
                // so that OleDbDataAdapter will insert the rows.
                foreach (DataRow dr in dtSQL.Rows)
                {
                    dr.SetAdded();
                }

                // Insert the data into the Excel spreadsheet.
                da.Update(dtSQL);

            }
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string strDownloadFileName = "";
            string strExcelConn = "";

            if (rblExtension.SelectedValue == "2003")
            {
                // Excel 97-2003
                strDownloadFileName = "~/DownloadFiles/" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
                strExcelConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath(strDownloadFileName) + ";Extended Properties='Excel 8.0;HDR=Yes'";
            }
            else
            {
                // Excel 2007
                strDownloadFileName = "~/DownloadFiles/" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                strExcelConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath(strDownloadFileName) + ";Extended Properties='Excel 12.0 Xml;HDR=Yes'";
            }

            // Retrieve data from SQL Server table.
            DataTable dtSQL = RetrieveData();

            // Export data to an Excel spreadsheet.
            ExportToExcel(strExcelConn, dtSQL);

            if (rblDownload.SelectedValue == "Yes")
            {
                hlDownload.Visible = true;

                // Display the download link.
                hlDownload.Text = "Click here to download file.";
                hlDownload.NavigateUrl = strDownloadFileName;
            }
            else
            {
                // Hide the download link.
                hlDownload.Visible = false;
            }
        }
    }
}
