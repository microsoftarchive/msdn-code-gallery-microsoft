# Import/export Excel worksheet in ASP.NET (CSASPNETExcelImportExport)
## Requires
- Visual Studio 2008
## License
- MS-LPL
## Technologies
- ASP.NET
- Office
## Topics
- Office
- Excel
## Updated
- 02/03/2012
## Description

<p style="font-family:Courier New">&nbsp;</p>
<h2>ASP.NET APPLICATION: CSASPNETExcelImportExport Project Overview</h2>
<p style="font-family:Courier New">&nbsp;</p>
<h3>Use:</h3>
<p style="font-family:Courier New"><br>
This CSASPNETExcelImportExport sample introduces how to import data from an <br>
Excel spreadsheet to SQL Server and how to generate an Excel spreadsheet <br>
with data from SQL Server.<br>
<br>
This project includes two pages: Default and Image.<br>
<br>
ExcelExport page retrieves data from SQL Server using a DataTable and then <br>
exports the DataTable to an Excel 2003/2007 spreadsheet on server disk. <br>
<br>
ExcelImport page fills a DataTable with data from an Excel 2003/2007 <br>
spreadsheet using a DataTable, and then import the DataTable to SQL Server <br>
via SqlBulkCopy, which efficiently bulk loads a SQL Server table.<br>
<br>
This sample uses the SQLServer2005DB sample database. <br>
<br>
</p>
<h3>Creation:</h3>
<p style="font-family:Courier New"><br>
Step1. Create a C# ASP.NET Web Application named CSASPNETExcelImportExport in <br>
Visual Studio 2008 / Visual Web Developer.<br>
<br>
Step2. Create a new Web page named ExcelImport. Drag a FileUpload,<br>
a RadioButtonList, a Button and a Label into the ExcelImport page. <br>
<br>
After that, rename the controls as follows:<br>
<br>
FileUpload1 &nbsp; &nbsp; &nbsp; &nbsp; -&gt; &nbsp; &nbsp;fupExcel <br>
RadioButtonList1 &nbsp; &nbsp;-&gt; &nbsp; &nbsp;rblArchive<br>
Button1 &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; -&gt; &nbsp; &nbsp;btnImport<br>
Label1 &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;-&gt; &nbsp; &nbsp;lblMessages<br>
<br>
Step3. Add two items in rblArchive:<br>
<br>
&lt;asp:RadioButtonList ID=&quot;rblArchive&quot; runat=&quot;server&quot;&gt;<br>
&nbsp; &nbsp;&lt;asp:ListItem Value=&quot;Yes&quot;&gt;Yes&lt;/asp:ListItem&gt;<br>
&nbsp; &nbsp;&lt;asp:ListItem Selected=&quot;True&quot; Value=&quot;No&quot;&gt;No&lt;/asp:ListItem&gt;<br>
&lt;/asp:RadioButtonList&gt; &nbsp; &nbsp;<br>
<br>
Step4. Copy the following methods from the sample, and paste them in the <br>
code-behind of Default page:<br>
<br>
GetRowCounts &nbsp; &nbsp; &nbsp; &nbsp;-- &nbsp; Get the row counts in SQL Server table<br>
RetrieveData &nbsp; &nbsp; &nbsp; &nbsp;-- &nbsp; Retrieve data from the Excel spreadsheet.<br>
SqlBulkCopyImport &nbsp; -- &nbsp; Import the data from DataTable to SQL Server via<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; SqlBulkCopy<br>
<br>
Step5. Double-click on btnImport to generate the Button_Click event handlers.<br>
After that, fill the generated methods with the sample code.<br>
<br>
Step6. Create a new Web page named ExcelExport. Drag two RadioButtonList, <br>
a Button and a HyperLinkinto the ExcelExport page. After that, rename <br>
the controls as follows:<br>
<br>
RadioButtonList1 &nbsp; &nbsp;-&gt; &nbsp; &nbsp;rblExtension<br>
RadioButtonList1 &nbsp; &nbsp;-&gt; &nbsp; &nbsp;rblDownload<br>
Button1 &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; -&gt; &nbsp; &nbsp;btnExport<br>
HyperLink1 &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;-&gt; &nbsp; &nbsp;hlDownload<br>
<br>
Step7. Add two items in rblExtension:<br>
<br>
&lt;asp:RadioButtonList ID=&quot;rblExtension&quot; runat=&quot;server&quot;&gt;<br>
&nbsp; &nbsp;&lt;asp:ListItem Selected=&quot;True&quot; Value=&quot;2003&quot;&gt;Excel 97-2003&lt;/asp:ListItem&gt;<br>
&nbsp; &nbsp;&lt;asp:ListItem Value=&quot;2007&quot;&gt;Excel 2007&lt;/asp:ListItem&gt;<br>
&lt;/asp:RadioButtonList&gt; &nbsp;<br>
<br>
Step7. Add two items in rblDownload:<br>
<br>
&lt;asp:RadioButtonList ID=&quot;rblDownload&quot; runat=&quot;server&quot;&gt;<br>
&nbsp; &nbsp;&lt;asp:ListItem Selected=&quot;True&quot; Value=&quot;Yes&quot;&gt;Yes&lt;/asp:ListItem&gt;<br>
&nbsp; &nbsp;&lt;asp:ListItem Value=&quot;No&quot;&gt;No&lt;/asp:ListItem&gt;<br>
&lt;/asp:RadioButtonList&gt;<br>
<br>
Step8. Copy the following methods from the sample, and paste them in the <br>
code-behind of Default page:<br>
<br>
RetrieveData &nbsp; &nbsp;-- &nbsp; Retrieve data from SQL Server table<br>
ExportToExcel &nbsp; -- &nbsp; Export data to an Excel spreadsheet via ADO.NET<br>
<br>
Step9. Double-click on btnExport to generate the Button_Click event handlers.<br>
After that, fill the generated methods with the sample code.<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
SqlBulkCopy Class<br>
<a href="http://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlbulkcopy.aspx" target="_blank">http://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlbulkcopy.aspx</a><br>
<br>
DataRow.SetAdded Method .<br>
<a href="http://msdn.microsoft.com/en-us/library/system.data.datarow.setadded.aspx" target="_blank">http://msdn.microsoft.com/en-us/library/system.data.datarow.setadded.aspx</a><br>
<br>
<a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Data.OleDb.aspx" target="_blank" title="Auto generated link to System.Data.OleDb">System.Data.OleDb</a> Namespace<br>
<a href="http://msdn.microsoft.com/en-us/library/system.data.oledb.aspx" target="_blank">http://msdn.microsoft.com/en-us/library/system.data.oledb.aspx</a><br>
<br>
How to query and display excel data by using ASP.NET, ADO.NET, and Visual C# .NET<br>
<a href="http://support.microsoft.com/kb/306572/" target="_blank">http://support.microsoft.com/kb/306572/</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
