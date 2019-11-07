========================================================================
    ASP.NET APPLICATION: CSASPNETExcelImportExport Project Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

This CSASPNETExcelImportExport sample introduces how to import data from an 
Excel spreadsheet to SQL Server and how to generate an Excel spreadsheet 
with data from SQL Server.

This project includes two pages: Default and Image.

ExcelExport page retrieves data from SQL Server using a DataTable and then 
exports the DataTable to an Excel 2003/2007 spreadsheet on server disk. 
 
ExcelImport page fills a DataTable with data from an Excel 2003/2007 
spreadsheet using a DataTable, and then import the DataTable to SQL Server 
via SqlBulkCopy, which efficiently bulk loads a SQL Server table.

This sample uses the SQLServer2005DB sample database. 


/////////////////////////////////////////////////////////////////////////////
Creation:

Step1. Create a C# ASP.NET Web Application named CSASPNETExcelImportExport in 
Visual Studio 2008 / Visual Web Developer.

Step2. Create a new Web page named ExcelImport. Drag a FileUpload,
a RadioButtonList, a Button and a Label into the ExcelImport page. 

After that, rename the controls as follows:

FileUpload1         ->    fupExcel 
RadioButtonList1    ->    rblArchive
Button1             ->    btnImport
Label1              ->    lblMessages

Step3. Add two items in rblArchive:

<asp:RadioButtonList ID="rblArchive" runat="server">
    <asp:ListItem Value="Yes">Yes</asp:ListItem>
    <asp:ListItem Selected="True" Value="No">No</asp:ListItem>
</asp:RadioButtonList>    

Step4. Copy the following methods from the sample, and paste them in the 
code-behind of Default page:
 
GetRowCounts        --   Get the row counts in SQL Server table
RetrieveData        --   Retrieve data from the Excel spreadsheet.
SqlBulkCopyImport   --   Import the data from DataTable to SQL Server via
						 SqlBulkCopy

Step5. Double-click on btnImport to generate the Button_Click event handlers.
After that, fill the generated methods with the sample code.

Step6. Create a new Web page named ExcelExport. Drag two RadioButtonList, 
a Button and a HyperLinkinto the ExcelExport page. After that, rename 
the controls as follows:

RadioButtonList1    ->    rblExtension
RadioButtonList1    ->    rblDownload
Button1             ->    btnExport
HyperLink1          ->    hlDownload

Step7. Add two items in rblExtension:

<asp:RadioButtonList ID="rblExtension" runat="server">
    <asp:ListItem Selected="True" Value="2003">Excel 97-2003</asp:ListItem>
    <asp:ListItem Value="2007">Excel 2007</asp:ListItem>
</asp:RadioButtonList>  

Step7. Add two items in rblDownload:

<asp:RadioButtonList ID="rblDownload" runat="server">
    <asp:ListItem Selected="True" Value="Yes">Yes</asp:ListItem>
    <asp:ListItem Value="No">No</asp:ListItem>
</asp:RadioButtonList>

Step8. Copy the following methods from the sample, and paste them in the 
code-behind of Default page:

RetrieveData    --   Retrieve data from SQL Server table
ExportToExcel   --   Export data to an Excel spreadsheet via ADO.NET

Step9. Double-click on btnExport to generate the Button_Click event handlers.
After that, fill the generated methods with the sample code.


/////////////////////////////////////////////////////////////////////////////
References:

SqlBulkCopy Class
http://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlbulkcopy.aspx

DataRow.SetAdded Method .
http://msdn.microsoft.com/en-us/library/system.data.datarow.setadded.aspx

System.Data.OleDb Namespace
http://msdn.microsoft.com/en-us/library/system.data.oledb.aspx

How to query and display excel data by using ASP.NET, ADO.NET, and Visual C# .NET
http://support.microsoft.com/kb/306572/


/////////////////////////////////////////////////////////////////////////////
