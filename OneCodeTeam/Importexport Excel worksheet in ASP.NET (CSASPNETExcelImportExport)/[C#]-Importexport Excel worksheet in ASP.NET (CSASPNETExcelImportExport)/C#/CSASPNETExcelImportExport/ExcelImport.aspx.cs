/****************************** Module Header ******************************\
* Module Name:  ExcelImport.aspx.cs
* Project:      CSASPNETExcelImportExport
* Copyright (c) Microsoft Corporation.
* 
* This page fills a DataTable with data from an Excel 2003/2007 spreadsheet 
* using a DataTable, and then import the DataTable to SQL Server via 
* SqlBulkCopy, which efficiently bulk loads a SQL Server table.
* 
* There is one option on the page, which is for choosing whether to remove 
* the uploaded Excel spreadsheet from server after importing data.
*  
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* History:
* 11/24/2009 13:20 AM Jian Kang Created
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
using System.IO;
using System.Configuration;
#endregion Using directives

namespace CSASPNETExcelImportExport
{
    public partial class ExcelImport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        // Get the row counts in SQL Server table.
        protected int GetRowCounts()
        {
            int iRowCount = 0;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLServer2005DBConnectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("select count(*) from Person", conn);
                conn.Open();

                // Execute the SqlCommand and get the row counts.
                iRowCount = (int)cmd.ExecuteScalar();
            }

            return iRowCount;
        }

        // Retrieve data from the Excel spreadsheet.
        protected DataTable RetrieveData(string strConn)
        {
            DataTable dtExcel = new DataTable();

            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                // Initialize an OleDbDataAdapter object.
                OleDbDataAdapter da = new OleDbDataAdapter("select * from Person", conn);

                // Fill the DataTable with data from the Excel spreadsheet.
                da.Fill(dtExcel);
            }

            return dtExcel;
        }

        // Import the data from DataTable to SQL Server via SqlBulkCopy
        protected void SqlBulkCopyImport(DataTable dtExcel)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLServer2005DBConnectionString"].ToString()))
            {
                // Open the connection.
                conn.Open();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(conn))
                {
                    // Specify the destination table name.
                    bulkCopy.DestinationTableName = "dbo.Person";

                    foreach (DataColumn dc in dtExcel.Columns)
                    {
                        // Because the number of the test Excel columns is not 
                        // equal to the number of table columns, we need to map 
                        // columns.
                        bulkCopy.ColumnMappings.Add(dc.ColumnName, dc.ColumnName);
                    }

                    // Write from the source to the destination.
                    bulkCopy.WriteToServer(dtExcel);
                }
            }
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            // Before attempting to import the file, verify
            // that the FileUpload control contains a file.
            if (fupExcel.HasFile)
            {
                // Get the name of the Excel spreadsheet to upload.
                string strFileName = Server.HtmlEncode(fupExcel.FileName);

                // Get the extension of the Excel spreadsheet.
                string strExtension = Path.GetExtension(strFileName);

                // Validate the file extension.
                if (strExtension != ".xls" && strExtension != ".xlsx")
                {
                    Response.Write("<script>alert('Please select a Excel spreadsheet to import!');</script>");
                    return;
                }

                // Generate the file name to save.
                string strUploadFileName = "~/UploadFiles/" + DateTime.Now.ToString("yyyyMMddHHmmss") + strExtension;

                // Save the Excel spreadsheet on server.
                fupExcel.SaveAs(Server.MapPath(strUploadFileName));

                // Generate the connection string for Excel file.
                string strExcelConn = "";

                // There is no column name In a Excel spreadsheet. 
                // So we specify "HDR=YES" in the connection string to use 
                // the values in the first row as column names. 
                if (strExtension == ".xls")
                {
                    // Excel 97-2003
                    strExcelConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Server.MapPath(strUploadFileName) + ";Extended Properties='Excel 8.0;HDR=YES;'";
                }
                else
                {
                    // Excel 2007
                    strExcelConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Server.MapPath(strUploadFileName) + ";Extended Properties='Excel 12.0 Xml;HDR=YES;'";
                }

                DataTable dtExcel = RetrieveData(strExcelConn);

                // Get the row counts before importing.
                int iStartCount = GetRowCounts();

                // Import the data.
                SqlBulkCopyImport(dtExcel);

                // Get the row counts after importing.
                int iEndCount = GetRowCounts();

                // Display the number of imported rows. 
                lblMessages.Text = Convert.ToString(iEndCount - iStartCount) + " rows were imported into Person table";

                if (rblArchive.SelectedValue == "No")
                {
                    // Remove the uploaded Excel spreadsheet from server.
                    File.Delete(Server.MapPath(strUploadFileName));
                }
            }
        }
    }
}
