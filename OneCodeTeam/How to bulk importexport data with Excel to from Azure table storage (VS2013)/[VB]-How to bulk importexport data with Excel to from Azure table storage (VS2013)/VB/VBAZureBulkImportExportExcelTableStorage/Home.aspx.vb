
'/****************************** Module Header ******************************\
'Module Name:  Home.aspx.cs
'Project:      VBAZureBulkImportExportExcelTableStorage
'Copyright (c) Microsoft Corporation.
' 
'The Azure Table storage service stores large amounts of structured data. 
'The service is a NoSQL datastore which accepts authenticated calls from inside and outside the Azure cloud
'You can use the Table service to store and query huge sets of structured
'
'This project  demonstrates How to bulk import/export data with Excel to/from Azure table storage.
'Users can bulk import data with Excel to Table storage 
'Users can bulk export data with Excel from Table storage 
' 
'This source is subject to the Microsoft Public License.
'See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
'All other rights reserved.
' 
'THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\***************************************************************************/


Imports Microsoft.WindowsAzure.Storage
Imports Microsoft.WindowsAzure.Storage.Table
Imports System.IO
Imports Microsoft.Office.Interop.Excel
Imports TableEntityHelper
Imports System.Reflection
Imports System.Data.OleDb

Public Class Home
    Inherits System.Web.UI.Page

    Private strAccount As String = "Storage Account"
    Private strAccountKey As String = "Primary Access Key"

    Private storageAccount As CloudStorageAccount

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Dim connectionString As String = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", strAccount, strAccountKey)
        storageAccount = CloudStorageAccount.Parse(connectionString)
        If Not IsPostBack Then
            Try
                GetAllTableName()
            Catch ex As Exception
            End Try
        End If
    End Sub

    ''' <summary>
    ''' Imports selected excel files to table storage
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btn_Import_Click(ByVal sender As Object, ByVal e As EventArgs)

        Dim file As FileInfo = Nothing
        Dim copyfile As FileInfo = Nothing
        Try
            Dim blnFlag As Boolean = False
            Dim Files As HttpFileCollection = Request.Files
            For i As Integer = 0 To Files.Count - 1
                Dim strFileName As String = String.Empty
                Dim strFilePath As String = Files(i).FileName
                Dim aryFileName() As String = strFilePath.Split("\"c)
                If aryFileName.Length > 0 Then
                    strFileName = aryFileName(aryFileName.Length - 1)
                End If

                If Not String.IsNullOrEmpty(strFileName) Then
                    Dim strCopyFilePath As String = Server.MapPath("DownLoad")
                    If Directory.Exists(strCopyFilePath) = False Then
                        Directory.CreateDirectory(strCopyFilePath)
                    End If
                    ful_FileUpLoad.SaveAs(strCopyFilePath & "\" & strFileName)
                    file = New FileInfo(strCopyFilePath & "\" & strFileName)
                    copyfile = New FileInfo(strCopyFilePath & "\" & "Copy" & strFileName)
                    If copyfile.Exists Then
                        copyfile.Delete()
                    End If
                    file.CopyTo(strCopyFilePath & "\" & "Copy" & strFileName)
                    Dim extension As String = file.Extension
                    If extension = ".xls" OrElse extension = ".xlsx" Then
                        ReadExcelInfo(strCopyFilePath & "\" & "Copy" & strFileName)
                    Else
                        Response.Write("<script>alert('Selected file is not an excel file!');</script>")
                        file.Delete()
                        copyfile.Delete()
                        'Lists all tables of the specified storageAccount 
                        RefreshAllTableName()
                        Return
                    End If
                    blnFlag = True
                    file.Delete()
                    copyfile.Delete()

                End If
            Next i

            If blnFlag Then
                Response.Write("<script>alert('Successfully imported excel files!');</script>")
            Else
                Response.Write("<script>alert('Select the excel files you want to import!');</script>")
            End If

        Catch ex As Exception
            If file IsNot Nothing Then
                file.Delete()
            End If
            If copyfile IsNot Nothing Then
                copyfile.Delete()
            End If
            Dim strError As String = "<script>alert('Importing failed! " & "Error message is "" " & ex.Message & " "" ');</script>"
            Response.Write(strError)
        End Try
        'Lists all tables of the specified storageAccount 
        RefreshAllTableName()
    End Sub

    ''' <summary>
    ''' refresh all table of the specified storageAccount 
    ''' </summary>
    Private Sub RefreshAllTableName()
        Dim lstSelectedTableName As New List(Of String)()
        For Each item As ListItem In ckb_TableName.Items
            If item.Selected Then
                If Not lstSelectedTableName.Contains(item.Text) Then
                    lstSelectedTableName.Add(item.Text)
                End If
            End If
        Next item
        'Lists all tables of the specified storageAccount 
        ViewState.Add("SelectedTableName", lstSelectedTableName)
        GetAllTableName()
    End Sub

    ''' <summary>
    ''' Exports selected storage tables to excel
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Protected Sub btn_ExportData_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim blnFlag As Boolean = False
        Dim lstSelectedTableName As New List(Of String)()
        Try
            For Each item As ListItem In ckb_TableName.Items
                If item.Selected Then
                    blnFlag = True
                    ExportDataToExcel(item.Text)
                    If Not lstSelectedTableName.Contains(item.Text) Then
                        lstSelectedTableName.Add(item.Text)
                    End If
                End If
            Next item
            If blnFlag Then
                Response.Write("<script>alert('Successfully exported data to excel!');</script>")
            Else
                Response.Write("<script>alert('Select the storage tables you want to export！');</script>")
            End If
        Catch ex As Exception
            Response.Write("<script>alert('Exporting failed！');</script>")
        End Try
        'Lists all tables of the specified storageAccount 
        ViewState.Add("SelectedTableName", lstSelectedTableName)
        GetAllTableName()
    End Sub

    ''' <summary>
    ''' Reads content of excel files that are selected
    ''' </summary>
    ''' <param name="strFilePath"></param>
    Private Sub ReadExcelInfo(ByVal strFilePath As String)
        Dim conn As System.Data.OleDb.OleDbConnection = Nothing

        Try
            Dim strConn As String = String.Empty
            Dim file As New FileInfo(strFilePath)
            If Not file.Exists Then
                Throw New Exception("file is not exists")
            End If
            Dim extension As String = file.Extension
            Select Case extension
                Case ".xls"
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" & strFilePath & ";Extended Properties='Excel 8.0;HDR=Yes;IMEX=1;'"
                Case ".xlsx"
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" & strFilePath & ";Extended Properties='Excel 12.0;HDR=Yes;IMEX=1;'"
                Case Else
            End Select

            If Not String.IsNullOrEmpty(strConn) Then
                conn = New System.Data.OleDb.OleDbConnection(strConn)
                conn.Open()
                Dim dtSheetInfo As System.Data.DataTable = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, Nothing)
                If dtSheetInfo.Rows.Count > 0 Then
                    For i As Integer = 0 To dtSheetInfo.Rows.Count - 1
                        If dtSheetInfo.Rows(i)("TABLE_NAME") IsNot Nothing Then
                            Dim strSheetName As String = dtSheetInfo.Rows(i)("TABLE_NAME").ToString()
                            Dim strSql As String = String.Format("SELECT * FROM [{0}]", strSheetName)
                            Dim myCommand As New OleDbDataAdapter(strSql, strConn)
                            Dim myDataSet As New DataSet()
                            Try
                                myCommand.Fill(myDataSet)
                                If myDataSet.Tables.Count > 0 Then
                                    ImportDataToTable(myDataSet.Tables(0), strSheetName)
                                End If
                            Catch ex As Exception
                                Throw ex
                            End Try
                        End If
                    Next i
                End If
                conn.Close()
                conn.Dispose()
            End If
        Catch ex As Exception
            If conn IsNot Nothing Then
                conn.Close()
                conn.Dispose()
            End If

            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Imports data of DataTable to table storage
    ''' </summary>
    ''' <param name="dtSheetInfo"></param>
    ''' <param name="strSheetName"></param>
    Private Sub ImportDataToTable(ByVal dtSheetInfo As System.Data.DataTable, ByVal strSheetName As String)
        Try

            For j As Integer = 0 To dtSheetInfo.Rows.Count - 1
                Dim entity As New ExcelTableEntity(strSheetName, Date.Now.ToLongTimeString())
                For i As Integer = 0 To dtSheetInfo.Columns.Count - 1
                    Dim strCloName As String = dtSheetInfo.Columns(i).ColumnName
                    If Not (TypeOf dtSheetInfo.Rows(j)(i) Is DBNull) AndAlso (dtSheetInfo.Rows(j)(i) IsNot Nothing) Then
                        Dim strValue As String = dtSheetInfo.Rows(j)(i).ToString()
                        If Not CheckPropertyExist(strCloName, strValue, entity) Then
                            Dim [property] As EntityProperty = entity.ConvertToEntityProperty(strCloName, dtSheetInfo.Rows(j)(i))
                            If Not entity.properties.ContainsKey(strCloName) Then
                                entity.properties.Add(strCloName, [property])
                            Else
                                entity.properties(strCloName) = [property]
                            End If
                        End If
                    End If
                Next i

                Dim client = storageAccount.CreateCloudTableClient()
                Dim strTableName As String = txt_TableName.Text
                If Not String.IsNullOrEmpty(strTableName) Then
                    Dim table As CloudTable = client.GetTableReference(strTableName)
                    table.CreateIfNotExists()
                    If Not CheckTableEntityDataExist(entity, table) Then
                        table.Execute(TableOperation.Insert(entity))
                    Else
                        table.Execute(TableOperation.InsertOrMerge(entity))
                    End If
                End If
            Next j
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Exports  data of selected storage tables to excel
    ''' </summary>
    ''' <param name="strTableName"></param>
    Private Sub ExportDataToExcel(ByVal strTableName As String)
        Try
            Dim client = storageAccount.CreateCloudTableClient()
            If Not String.IsNullOrEmpty(strTableName) Then
                Dim table As CloudTable = client.GetTableReference(strTableName)
                Dim strPartitionKey As String = ""
                Dim strFilter As String = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.NotEqual, strPartitionKey)
                Dim query As New TableQuery(Of ExcelTableEntity)()
                query.Where(strFilter)

                If table.ExecuteQuery(query).Count() > 0 Then
                    Dim dtInfo As New System.Data.DataTable()
                    Dim i As Integer = 0
                    For Each entity As Object In table.ExecuteQuery(query)
                        If i = 0 Then
                            SetColumnTitle(entity, dtInfo)
                        End If
                        InsertEntityDataToTable(entity, dtInfo)
                        i += 1
                    Next entity
                    InsertTableDataToExcel(dtInfo, strTableName)
                End If
            End If
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Inserts  data of DataTable to excel
    ''' </summary>
    ''' <param name="dtInfo"></param>
    ''' <param name="strTableName"></param>
    Private Sub InsertTableDataToExcel(ByVal dtInfo As System.Data.DataTable, ByVal strTableName As String)
        Try
            Dim app As New Application()
            Dim wbks As Workbooks = app.Workbooks
            app.DisplayAlerts = False
            app.AlertBeforeOverwriting = False
            Dim _wbk As _Workbook = wbks.Add(True)
            Dim sh As Sheets = _wbk.Sheets
            Dim _worksh As Worksheet = CType(sh.Item(1), Worksheet)

            Dim strPath As String = Server.MapPath("DownLoad")
            If Directory.Exists(strPath) = False Then
                Directory.CreateDirectory(strPath)
            End If
            strPath = strPath & "\" & strTableName & ".xlsx"
            Dim cnt As Integer = dtInfo.Rows.Count
            Dim columncnt As Integer = dtInfo.Columns.Count
            Dim objData(cnt, columncnt - 1) As Object
            For col As Integer = 0 To columncnt - 1
                objData(0, col) = dtInfo.Columns(col).ColumnName
            Next col
            For row As Integer = 0 To cnt - 1
                Dim dr As System.Data.DataRow = dtInfo.Rows(row)
                For j As Integer = 0 To columncnt - 1
                    objData(row + 1, j) = dr(j)
                Next j
            Next row

            '********************* write to Excel******************
            Dim oRange As Range
            Dim c1 As Range = _worksh.Cells(1, 1)
            Dim c2 As Range = _worksh.Cells(cnt + 1, columncnt)
            oRange = _worksh.Range(c1, c2)
            oRange.NumberFormat = "@"
            oRange.Value2 = objData
            oRange.EntireColumn.AutoFit()
            _wbk.SaveCopyAs(strPath)
            app.Quit()
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    '''  Sets title of column of DataTable using property of ExcelTableEntity
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <param name="dtEntityInfo"></param>
    Private Sub SetColumnTitle(ByVal obj As Object, ByVal dtEntityInfo As System.Data.DataTable)
        Try
            'Lists all Properties of ExcelTableEntity
            Dim entityType As Type = GetType(ExcelTableEntity)
            Dim ProList() As PropertyInfo = entityType.GetProperties()
            For Each Pro As PropertyInfo In ProList
                If Pro.PropertyType.Name.Contains("IDictionary") Then
                    Dim dicEntity As Dictionary(Of String, EntityProperty) = CType(Pro.GetValue(obj, Nothing), Dictionary(Of String, EntityProperty))

                    For Each key As String In dicEntity.Keys
                        Dim col As New DataColumn(key)
                        dtEntityInfo.Columns.Add(col)
                    Next key
                Else
                    Dim col As New DataColumn(Pro.Name)
                    dtEntityInfo.Columns.Add(col)
                End If
            Next Pro
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Inserts values of all ExcelTableEntity properties to DataTable
    ''' </summary>
    ''' <param name="obj"></param>
    ''' <param name="dtEntityInfo"></param>
    Private Sub InsertEntityDataToTable(ByVal obj As Object, ByVal dtEntityInfo As System.Data.DataTable)
        Try
            Dim row As DataRow = dtEntityInfo.Rows.Add()

            'Lists all Properties of ExcelTableEntity
            Dim entityType As Type = GetType(ExcelTableEntity)
            Dim ProList() As PropertyInfo = entityType.GetProperties()
            For Each Pro As PropertyInfo In ProList
                If Pro.PropertyType.Name.Contains("IDictionary") Then
                    Dim dicEntity As Dictionary(Of String, EntityProperty) = CType(Pro.GetValue(obj, Nothing), Dictionary(Of String, EntityProperty))

                    For Each key As String In dicEntity.Keys
                        If Not dtEntityInfo.Columns.Contains(key) Then
                            Dim col As New DataColumn(key)
                            dtEntityInfo.Columns.Add(col)

                        End If
                        row(key) = dicEntity(key).PropertyAsObject.ToString()
                    Next key
                Else
                    row(Pro.Name) = Pro.GetValue(obj, Nothing).ToString()
                End If
            Next Pro
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

    ''' <summary>
    ''' Checks the property is exist or not in ExcelTableEntity
    ''' </summary>
    ''' <param name="strProperName"></param>
    ''' <param name="strValue"></param>
    ''' <param name="entity"></param>
    ''' <returns></returns>
    Private Function CheckPropertyExist(ByVal strProperName As String, ByVal strValue As String, ByVal entity As ExcelTableEntity) As Boolean
        Dim bln_Result As Boolean = False
        Try
            'Lists all Properties of ExcelTableEntity
            Dim entityType As Type = GetType(ExcelTableEntity)
            Dim ProList() As PropertyInfo = entityType.GetProperties()
            For i As Integer = 0 To ProList.Count() - 1
                If ProList(i).Name = strProperName Then
                    If ProList(i).PropertyType.Name = "DateTimeOffset" Then
                        Dim dtime As Date = Convert.ToDateTime(strValue)
                        dtime = Date.SpecifyKind(dtime, DateTimeKind.Utc)
                        Dim utcTime2 As DateTimeOffset = dtime
                        ProList(i).SetValue(entity, utcTime2)
                    Else
                        ProList(i).SetValue(entity, strValue)
                    End If
                    bln_Result = True
                    Exit For
                End If
            Next i
        Catch ex As Exception
            Throw ex
        End Try
        Return bln_Result
    End Function

    ''' <summary>
    ''' Checks ExcelTableEntity is exist or not in table storage
    ''' </summary>
    ''' <param name="entity"></param>
    ''' <param name="ctTable"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckTableEntityDataExist(ByVal entity As ExcelTableEntity, ByVal ctTable As CloudTable) As Boolean
        Dim bln_Result As Boolean = False
        Try
            Dim strPartitionKey As String = entity.PartitionKey
            Dim strRowKey As String = entity.RowKey
            Dim strFilter As String = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, strPartitionKey) & " and " & TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, strRowKey)
            Dim query As New TableQuery(Of ExcelTableEntity)()
            query.Where(strFilter)

            If ctTable.ExecuteQuery(query).Count() > 0 Then
                bln_Result = True
            End If
        Catch ex As Exception
            Throw ex
        End Try
        Return bln_Result
    End Function

    ''' <summary>
    ''' Lists all tables of the specified storageAccount 
    ''' </summary>
    Private Sub GetAllTableName()
        Try
            Dim client As CloudTableClient = storageAccount.CreateCloudTableClient()
            ckb_TableName.Items.Clear()
            Dim lstSelectedTableName As New List(Of String)()
            If ViewState("SelectedTableName") IsNot Nothing Then
                lstSelectedTableName = CType(ViewState("SelectedTableName"), List(Of String))
            End If
            For Each table As CloudTable In client.ListTables()
                Dim item As New ListItem()
                item.Text = table.Name
                item.Value = table.Name
                ckb_TableName.Items.Add(item)

                If lstSelectedTableName.Contains(item.Text) Then
                    item.Selected = True
                End If
            Next table
            ckb_TableName_SelectedIndexChanged(Nothing, Nothing)
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub ckb_TableName_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
        Try
            If Not chk_ShowDetails.Checked Then
                Return
            End If
            ShowTableContent()

        Catch ex As Exception
        End Try
    End Sub

    Protected Sub chk_ShowDetails_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs)
        Try
            If Not chk_ShowDetails.Checked Then
                Return
            End If
            ShowTableContent()

        Catch ex As Exception
        End Try
    End Sub

    ''' <summary>
    ''' Shows 10 records of each selected table under table storage
    ''' </summary>
    Private Sub ShowTableContent()
        Try
            tbl_TableDetailList.Rows.Clear()
            For Each item As ListItem In ckb_TableName.Items
                If item.Selected Then
                    Dim tableTitleRow As New TableRow()
                    tableTitleRow.ID = "tblTitleRow_" & item.Text
                    Dim celltitle As New TableCell()
                    celltitle.Text = item.Text
                    celltitle.HorizontalAlign = HorizontalAlign.Left
                    celltitle.Style.Add("font-size", "16pt")
                    celltitle.Style.Add("Font-Bold", "true")
                    tableTitleRow.Cells.Add(celltitle)
                    tbl_TableDetailList.Rows.Add(tableTitleRow)

                    Dim cell As New TableCell()
                    Dim dgDynamicTableInfo As New DataGrid()
                    dgDynamicTableInfo.ID = "dg_" & item.Text
                    Dim client = storageAccount.CreateCloudTableClient()
                    Dim table As CloudTable = client.GetTableReference(item.Text)

                    Dim strPartitionKey As String = ""
                    Dim strFilter As String = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.NotEqual, strPartitionKey)
                    Dim query As New TableQuery(Of ExcelTableEntity)()
                    query.Where(strFilter)

                    Dim tableDataRow As New TableRow()
                    tableDataRow.ID = "tblDataRow_" & item.Text
                    If table.ExecuteQuery(query).Count() > 0 Then
                        Dim dtInfo As New System.Data.DataTable()
                        Dim i As Integer = 0
                        For Each entity As Object In table.ExecuteQuery(query)
                            If i = 0 Then
                                SetColumnTitle(entity, dtInfo)
                            End If
                            If i <= 10 Then
                                InsertEntityDataToTable(entity, dtInfo)
                            End If
                            i += 1
                        Next entity
                        dgDynamicTableInfo.EnableViewState = True
                        dgDynamicTableInfo.DataSource = dtInfo
                        dgDynamicTableInfo.DataBind()
                        cell.Controls.Add(dgDynamicTableInfo)
                        tableDataRow.Cells.Add(cell)
                        tbl_TableDetailList.Rows.Add(tableDataRow)
                    End If
                End If
            Next item
        Catch ex As Exception
            Throw ex
        End Try
    End Sub

End Class