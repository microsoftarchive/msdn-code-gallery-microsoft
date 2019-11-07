
/****************************** Module Header ******************************\
*Module Name:  Home.aspx.cs
*Project:      CSAZureBulkImportExportExcelTableStorage
*Copyright (c) Microsoft Corporation.
* 
*The Azure Table storage service stores large amounts of structured data. 
*The service is a NoSQL datastore which accepts authenticated calls from inside and outside the Azure cloud
*You can use the Table service to store and query huge sets of structured
*
*This project  demonstrates How to bulk import/export data with Excel to/from Azure table storage.
*Users can bulk import data with Excel to Table storage 
*Users can bulk export data with Excel from Table storage 
* 
*This source is subject to the Microsoft Public License.
*See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
*All other rights reserved.
* 
*THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
*EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
*WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using Microsoft.Office.Interop.Excel;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CSAZureBulkImportExportExcelTableStorage
{
    public partial class Home : System.Web.UI.Page
    {
        string strAccount = "Storage Account";
        string strAccountKey = "Primary Access Key";

        CloudStorageAccount storageAccount;

        protected void Page_Load(object sender, EventArgs e)
        {
            string connectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", strAccount, strAccountKey);
            storageAccount = CloudStorageAccount.Parse(connectionString);
            if (!IsPostBack)
            {
                try
                {
                    GetAllTableName();
                }
                catch (Exception ex)
                {
                }
            }
        }

        /// <summary>
        /// Imports selected excel files to table storage
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Import_Click(object sender, EventArgs e)
        {
            FileInfo file = null;
            FileInfo copyfile = null;
            try
            {
                bool blnFlag = false;
                HttpFileCollection Files = Request.Files;
                for (int i = 0; i < Files.Count; i++)
                {
                    string strFileName = string.Empty;
                    string strFilePath = Files[i].FileName;
                    string[] aryFileName = strFilePath.Split('\\');
                    if (aryFileName.Length > 0)
                    {
                        strFileName = aryFileName[aryFileName.Length - 1];
                    }
                    if (!string.IsNullOrEmpty(strFileName))
                    {
                        string strCopyFilePath = Server.MapPath("DownLoad");
                        if (Directory.Exists(strCopyFilePath) == false)
                        {
                            Directory.CreateDirectory(strCopyFilePath);
                        }
                        ful_FileUpLoad.SaveAs(strCopyFilePath + "\\" + strFileName);
                        file = new FileInfo(strCopyFilePath + "\\" + strFileName);
                        copyfile = new FileInfo(strCopyFilePath + "\\" + "Copy" + strFileName);
                        if (copyfile.Exists)
                        {
                            copyfile.Delete();
                        }
                        file.CopyTo(strCopyFilePath + "\\" + "Copy" + strFileName);
                        string extension = file.Extension;
                        if (extension == ".xls" || extension == ".xlsx")
                        {
                            ReadExcelInfo(strCopyFilePath + "\\" + "Copy" + strFileName);
                        }
                        else
                        {
                            Response.Write("<script>alert('Selected file is not an excel file!');</script>");
                            file.Delete();
                            copyfile.Delete();
                            //Lists all tables of the specified storageAccount 
                            RefreshAllTableName();
                            return;
                        }
                        blnFlag = true;
                        file.Delete();
                        copyfile.Delete();
                    }
                }
                if (blnFlag)
                {
                    Response.Write("<script>alert('Successfully imported excel files！');</script>");
                }
                else
                {
                    Response.Write("<script>alert('Select the excel files you want to import!');</script>");
                }
            }
            catch (Exception ex)
            {
                if (file != null)
                {
                    file.Delete();
                }
                if (copyfile != null)
                {
                    copyfile.Delete();
                }
                string strError = "<script>alert('Importing failed! " + "Error message is \" " + ex.Message + " \" ');</script>";
                Response.Write(strError);
            }
            //Lists all tables of the specified storageAccount 
            RefreshAllTableName();
        }

        /// <summary>
        /// refresh all table of the specified storageAccount 
        /// </summary>
        private void RefreshAllTableName()
        {
            List<string> lstSelectedTableName = new List<string>();
            foreach (ListItem item in ckb_TableName.Items)
            {
                if (item.Selected)
                {
                    if (!lstSelectedTableName.Contains(item.Text))
                    {
                        lstSelectedTableName.Add(item.Text);
                    }
                }
            }
            //Lists all tables of the specified storageAccount 
            ViewState.Add("SelectedTableName", lstSelectedTableName);
            GetAllTableName();
        }


        /// <summary>
        /// Exports selected storage tables to excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_ExportData_Click(object sender, EventArgs e)
        {
            List<string> lstSelectedTableName = new List<string>();
            bool blnFlag = false;
            if (ckb_TableName.Items.Count <= 0)
            {
                return;
            }
            try
            {
                foreach (ListItem item in ckb_TableName.Items)
                {
                    if (item.Selected)
                    {
                        blnFlag = true;
                        ExportDataToExcel(item.Text);
                        if (!lstSelectedTableName.Contains(item.Text))
                        {
                            lstSelectedTableName.Add(item.Text);
                        }
                    }
                }
                if (blnFlag)
                {
                    Response.Write("<script>alert('Successfully exported data to excel！');</script>");
                }
                else
                {
                    Response.Write("<script>alert('Select the storage tables you want to export！');</script>");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Exporting failed！');</script>");
            }

            //Lists all tables of the specified storageAccount 
            ViewState.Add("SelectedTableName", lstSelectedTableName);
            GetAllTableName();
        }


        /// <summary>
        /// Reads content of excel files that are selected
        /// </summary>
        /// <param name="strFilePath"></param>
        private void ReadExcelInfo(string strFilePath)
        {
            OleDbConnection conn = null;
            try
            {
                string strConn = string.Empty;
                FileInfo file = new FileInfo(strFilePath);
                if (!file.Exists) { throw new Exception("file is not exists"); }
                string extension = file.Extension;
                switch (extension)
                {
                    case ".xls":
                        strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + strFilePath + ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'";
                        break;
                    case ".xlsx":
                        strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + strFilePath + ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'";
                        break;
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(strConn))
                {
                    conn = new OleDbConnection(strConn);
                    conn.Open();
                    System.Data.DataTable dtSheetInfo = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dtSheetInfo.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtSheetInfo.Rows.Count; i++)
                        {
                            if (dtSheetInfo.Rows[i]["TABLE_NAME"] != null)
                            {
                                string strSheetName = dtSheetInfo.Rows[i]["TABLE_NAME"].ToString();
                                string strSql = string.Format("SELECT * FROM [{0}]", strSheetName);
                                OleDbDataAdapter myCommand = new OleDbDataAdapter(strSql, strConn);
                                DataSet myDataSet = new DataSet();
                                try
                                {
                                    myCommand.Fill(myDataSet);
                                    if (myDataSet.Tables.Count > 0)
                                    {
                                        ImportDataToTable(myDataSet.Tables[0], strSheetName);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                        }
                    }
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }

                throw ex;
            }
        }

        /// <summary>
        /// Imports data of DataTable to table storage
        /// </summary>
        /// <param name="dtSheetInfo"></param>
        /// <param name="strSheetName"></param>
        private void ImportDataToTable(System.Data.DataTable dtSheetInfo, string strSheetName)
        {
            try
            {
                tbl_TableDetailList.Rows.Clear();
                for (int j = 0; j < dtSheetInfo.Rows.Count; j++)
                {
                    ExcelTableEntity entity = new ExcelTableEntity(strSheetName, DateTime.Now.ToLongTimeString());
                    for (int i = 0; i < dtSheetInfo.Columns.Count; i++)
                    {
                        string strCloName = dtSheetInfo.Columns[i].ColumnName;
                        if (!(dtSheetInfo.Rows[j][i] is DBNull) && (dtSheetInfo.Rows[j][i] != null))
                        {
                            string strValue = dtSheetInfo.Rows[j][i].ToString();
                            if (!CheckPropertyExist(strCloName, strValue, entity))
                            {
                                EntityProperty property = entity.ConvertToEntityProperty(strCloName, dtSheetInfo.Rows[j][i]);
                                if (!entity.properties.ContainsKey(strCloName))
                                {
                                    entity.properties.Add(strCloName, property);
                                }
                                else
                                {
                                    entity.properties[strCloName] = property;
                                }
                            }
                        }
                    }

                    var client = storageAccount.CreateCloudTableClient();
                    string strTableName = txt_TableName.Text;
                    if (!string.IsNullOrEmpty(strTableName))
                    {
                        CloudTable table = client.GetTableReference(strTableName);
                        table.CreateIfNotExists();
                        if (!CheckTableEntityDataExist(entity, table))
                        {
                            table.Execute(TableOperation.Insert(entity));
                        }
                        else
                        {
                            table.Execute(TableOperation.InsertOrMerge(entity));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// Exports  data of selected storage tables to excel
        /// </summary>
        /// <param name="strTableName"></param>
        private void ExportDataToExcel(string strTableName)
        {
            try
            {
                var client = storageAccount.CreateCloudTableClient();
                if (!string.IsNullOrEmpty(strTableName))
                {
                    CloudTable table = client.GetTableReference(strTableName);
                    string strPartitionKey = "";
                    string strFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.NotEqual, strPartitionKey);
                    TableQuery<ExcelTableEntity> query = new TableQuery<ExcelTableEntity>().Where(strFilter);

                    if (table.ExecuteQuery(query).Count() > 0)
                    {
                        System.Data.DataTable dtInfo = new System.Data.DataTable();
                        int i = 0;
                        foreach (object entity in table.ExecuteQuery(query))
                        {
                            if (i == 0)
                            {
                                SetColumnTitle(entity, dtInfo);
                            }
                            InsertEntityDataToTable(entity, dtInfo);
                            i++;
                        }
                        InsertTableDataToExcel(dtInfo, strTableName);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Inserts  data of DataTable to excel
        /// </summary>
        /// <param name="dtInfo"></param>
        /// <param name="strTableName"></param>
        private void InsertTableDataToExcel(System.Data.DataTable dtInfo, string strTableName)
        {
            try
            {
                Application app = new Application();
                Workbooks wbks = app.Workbooks;
                app.DisplayAlerts = false;
                app.AlertBeforeOverwriting = false;
                _Workbook _wbk = wbks.Add(true);
                Sheets sh = _wbk.Sheets;
                Worksheet _worksh = (Worksheet)sh.get_Item(1);
                string strPath = Server.MapPath("DownLoad");
                if (Directory.Exists(strPath) == false)
                {
                    Directory.CreateDirectory(strPath);
                }
                strPath = strPath + "\\" + strTableName + ".xlsx";
                int cnt = dtInfo.Rows.Count;
                int columncnt = dtInfo.Columns.Count;
                object[,] objData = new Object[cnt + 1, columncnt];
                for (int col = 0; col < columncnt; col++)
                {
                    objData[0, col] = dtInfo.Columns[col].ColumnName;
                }
                for (int row = 0; row < cnt; row++)
                {
                    System.Data.DataRow dr = dtInfo.Rows[row];
                    for (int j = 0; j < columncnt; j++)
                    {
                        objData[row + 1, j] = dr[j];
                    }
                }
                //********************* write to Excel******************
                Range oRange = (Range)_worksh.get_Range((Range)_worksh.Cells[1, 1], (Range)_worksh.Cells[cnt + 1, columncnt]);
                oRange.NumberFormat = "@";
                oRange.Value2 = objData;
                oRange.EntireColumn.AutoFit();
                _wbk.SaveCopyAs(strPath);
                app.Quit();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Sets title of column of DataTable using property of ExcelTableEntity
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dtEntityInfo"></param>
        private void SetColumnTitle(object obj, System.Data.DataTable dtEntityInfo)
        {
            try
            {
                //Lists all Properties of ExcelTableEntity
                Type entityType = typeof(ExcelTableEntity);
                PropertyInfo[] ProList = entityType.GetProperties();
                foreach (PropertyInfo Pro in ProList)
                {
                    if (Pro.PropertyType.Name.Contains("IDictionary"))
                    {
                        Dictionary<string, EntityProperty> dicEntity = (Dictionary<string, EntityProperty>)Pro.GetValue(obj, null);

                        foreach (string key in dicEntity.Keys)
                        {
                            DataColumn col = new DataColumn(key);
                            dtEntityInfo.Columns.Add(col);
                        }
                    }
                    else
                    {
                        DataColumn col = new DataColumn(Pro.Name);
                        dtEntityInfo.Columns.Add(col);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Inserts values of all ExcelTableEntity properties to DataTable
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dtEntityInfo"></param>
        private void InsertEntityDataToTable(object obj, System.Data.DataTable dtEntityInfo)
        {
            try
            {
                DataRow row = dtEntityInfo.Rows.Add();

                //Lists all Properties of ExcelTableEntity
                Type entityType = typeof(ExcelTableEntity);
                PropertyInfo[] ProList = entityType.GetProperties();
                foreach (PropertyInfo Pro in ProList)
                {
                    if (Pro.PropertyType.Name.Contains("IDictionary"))
                    {
                        Dictionary<string, EntityProperty> dicEntity = (Dictionary<string, EntityProperty>)Pro.GetValue(obj, null);

                        foreach (string key in dicEntity.Keys)
                        {
                            if (!dtEntityInfo.Columns.Contains(key))
                            {
                                DataColumn col = new DataColumn(key);
                                dtEntityInfo.Columns.Add(col);
                            }
                            row[key] = dicEntity[key].PropertyAsObject.ToString();
                        }
                    }
                    else
                    {
                        row[Pro.Name] = Pro.GetValue(obj, null).ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Checks the property is exist or not in ExcelTableEntity
        /// </summary>
        /// <param name="strProperName"></param>
        /// <param name="strValue"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool CheckPropertyExist(string strProperName, string strValue, ExcelTableEntity entity)
        {
            bool bln_Result = false;
            try
            {
                Type entityType = typeof(ExcelTableEntity);
                PropertyInfo[] ProList = entityType.GetProperties();
                for (int i = 0; i < ProList.Count(); i++)
                {
                    if (ProList[i].Name == strProperName)
                    {
                        if (ProList[i].PropertyType.Name == "DateTimeOffset")
                        {
                            DateTime dtime = Convert.ToDateTime(strValue);
                            dtime = DateTime.SpecifyKind(dtime, DateTimeKind.Utc);
                            DateTimeOffset utcTime2 = dtime;
                            ProList[i].SetValue(entity, utcTime2);
                        }
                        else
                        {
                            ProList[i].SetValue(entity, strValue);
                        }
                        bln_Result = true;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return bln_Result;
        }

        /// <summary>
        /// Checks ExcelTableEntity is exist or not in table storage
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="ctTable"></param>
        /// <returns></returns>
        private bool CheckTableEntityDataExist(ExcelTableEntity entity, CloudTable ctTable)
        {
            bool bln_Result = false;
            try
            {
                string strPartitionKey = entity.PartitionKey;
                string strRowKey = entity.RowKey;
                string strFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, strPartitionKey) + " and " + TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, strRowKey);
                TableQuery<ExcelTableEntity> query = new TableQuery<ExcelTableEntity>().Where(strFilter);

                if (ctTable.ExecuteQuery(query).Count() > 0)
                {
                    bln_Result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return bln_Result;
        }


        /// <summary>
        /// Lists all tables of the specified storageAccount 
        /// </summary>
        private void GetAllTableName()
        {
            try
            {
                CloudTableClient client = storageAccount.CreateCloudTableClient();
                ckb_TableName.Items.Clear();
                List<string> lstSelectedTableName = new List<string>();
                if (ViewState["SelectedTableName"] != null)
                {
                    lstSelectedTableName = (List<string>)ViewState["SelectedTableName"];
                }
                foreach (CloudTable table in client.ListTables())
                {
                    ListItem item = new ListItem();
                    item.Text = table.Name;
                    item.Value = table.Name;
                    ckb_TableName.Items.Add(item);

                    if (lstSelectedTableName.Contains(item.Text))
                    {
                        item.Selected = true;
                    }
                }
                ckb_TableName_SelectedIndexChanged(null, null);
            }
            catch (Exception ex)
            {
            }
        }

        protected void ckb_TableName_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!chk_ShowDetails.Checked)
                {
                    return;
                }
                ShowTableContent();

            }
            catch (Exception ex)
            {
            }
        }

        protected void chk_ShowDetails_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!chk_ShowDetails.Checked)
                {
                    return;
                }
                ShowTableContent();

            }
            catch (Exception ex)
            {
            }
        }


        /// <summary>
        ///Shows 10 records of each selected table under table storage
        /// </summary>
        private void ShowTableContent()
        {
            try
            {
                foreach (ListItem item in ckb_TableName.Items)
                {
                    if (item.Selected)
                    {
                        //Sets text content of cells using table name
                        TableRow tableTitleRow = new TableRow();
                        tableTitleRow.ID = "tblTitleRow_" + item.Text;
                        TableCell celltitle = new TableCell();
                        celltitle.Text = item.Text;
                        celltitle.HorizontalAlign = HorizontalAlign.Left;
                        celltitle.Style.Add("font-size", "16pt");
                        celltitle.Style.Add("Font-Bold", "true");
                        tableTitleRow.Cells.Add(celltitle);
                        tbl_TableDetailList.Rows.Add(tableTitleRow);

                        //Binds DataGrid with data of Querying table storage
                        TableCell cell = new TableCell();
                        DataGrid dgDynamicTableInfo = new DataGrid();
                        dgDynamicTableInfo.ID = "dg_" + item.Text;
                        var client = storageAccount.CreateCloudTableClient();
                        CloudTable table = client.GetTableReference(item.Text);
                        string strPartitionKey = "";
                        string strFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.NotEqual, strPartitionKey);
                        TableQuery<ExcelTableEntity> query = new TableQuery<ExcelTableEntity>().Where(strFilter);

                        TableRow tableDataRow = new TableRow();
                        tableDataRow.ID = "tblDataRow_" + item.Text;
                        if (table.ExecuteQuery(query).Count() > 0)
                        {
                            System.Data.DataTable dtInfo = new System.Data.DataTable();
                            int i = 0;
                            foreach (object entity in table.ExecuteQuery(query))
                            {
                                if (i == 0)
                                {
                                    SetColumnTitle(entity, dtInfo);
                                }
                                if (i <= 10)
                                {
                                    InsertEntityDataToTable(entity, dtInfo);
                                }
                                i++;
                            }
                            dgDynamicTableInfo.EnableViewState = true;
                            dgDynamicTableInfo.DataSource = dtInfo;
                            dgDynamicTableInfo.DataBind();
                            cell.Controls.Add(dgDynamicTableInfo);
                            tableDataRow.Cells.Add(cell);
                            tbl_TableDetailList.Rows.Add(tableDataRow);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}