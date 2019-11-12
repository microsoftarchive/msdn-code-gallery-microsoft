# How to use DataAdapter to retrieve and update data
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- ADO.NET
- Data Access
- .NET Development
## Topics
- Update
- DataAdapter
## Updated
- 05/16/2013
## Description

<h1>How to use DataAdapter to retrieve and update the data (CSDataAdapterOperations)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">We can use DataAdapter to retrieve and update the data, and sometimes the features of DataAdapter make some specific operations easier. In this sample, we will demonstrate how to use DataAdapter to retrieve and update the data:</p>
<p class="MsoNormal" style="line-height:18.0pt"><span style="color:black">1. Retrieve Data</span><span style="color:black">
</span></p>
<p class="MsoNormal">a. Use <span class="MsoHyperlink"><span style=""><a href="http://msdn.microsoft.com/en-us/library/system.data.common.dataadapter.acceptchangesduringfill%28VS.110%29.aspx">DataAdapter.AcceptChangesDuringFill Property</a></span></span>
 to clone the data in database.</p>
<p class="MsoNormal">&nbsp;If the property is set as false, AcceptChanges is not called when filling the table, and the newly added rows are treated as inserted rows. So we can use these rows to insert the new rows into the database.</p>
<p class="MsoNormal" style="line-height:18.0pt"><span class="GramE"><span style="color:black">b</span></span><span style="color:black">. Use
<a href="http://msdn.microsoft.com/en-us/library/system.data.common.dataadapter.tablemappings(v=vs.110).aspx">
<span class="SpellE">DataAdapter.TableMappings</span> Property</a> to define the mapping between the source table and DataTable.</span><span style="font-size:12.0pt; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;; color:black">
</span></p>
<p class="MsoNormal" style="line-height:18.0pt"><span style="color:black">c. Use
</span><span style="color:black"><a href="http://msdn.microsoft.com/en-us/library/system.data.common.dataadapter.fillloadoption%28v=vs.110%29.aspx"><span style="">DataAdapter.FillLoadOption Property</span></a></span><span style="color:black"> to determine how
 the adapter fills the DataTable from the DbDataReader.</span><span style="color:black">
</span></p>
<p class="MsoNormal">When we create a DataTable, we can only writ the data from database to the current version or the original version by setting the property as the LoadOption.Upsert or the LoadOption.PreserveChanges.</p>
<p class="MsoNormal" style="line-height:18.0pt"><span style="color:black">2. Update table</span><span style="font-size:12.0pt; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;; color:black">
</span></p>
<p class="MsoNormal" style="line-height:18.0pt"><span style="color:black">Use </span>
<span style="color:black"><a href="http://msdn.microsoft.com/en-us/library/system.data.common.dbdataadapter.updatebatchsize.aspx"><span style="">DbDataAdapter.UpdateBatchSize Property</span></a></span><span style="color:black"> to perfom
</span><span style="color:black"><a href="http://msdn.microsoft.com/en-us/library/aadf8fk2%28VS.110%29.aspx"><span style="">batch operations</span></a></span><span style="color:black">.</span><span style="font-size:12.0pt; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;; color:black">
</span></p>
<h2>Building the Sample</h2>
<p class="MsoNormal">Before you run the sample, you need to finish the following steps:</p>
<p class="MsoNormal">Step1. Please choose one way of the following ways to build the database:</p>
<p class="MsoListParagraphCxSpFirst" style="margin-left:63.15pt; text-indent:-.25in">
<span style="font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Attach the database file <span class="SpellE">MySchool.mdf</span> under the folder _<span class="SpellE">External_Dependecies</span> to your SQL Server 2008 database instance.</p>
<p class="MsoListParagraphCxSpLast" style="margin-left:63.15pt; text-indent:-.25in">
<span style="font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Run the <span class="SpellE">MySchool.sql</span> script under the folder _<span class="SpellE">External_Dependecies</span> in your SQL Server 2008 database instance.</p>
<p class="MsoNormal">Step2. Modify the connection string in the Project Properties-&gt;Settings according-&gt; MySchoolConnectionString to your SQL Server 2008 database instance name.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Press F5 to run the sample, the following is the result.</p>
<p class="MsoNormal"><span style=""><img src="82207-image.png" alt="" width="637" height="362" align="middle">
</span></p>
<p class="MsoNormal">First, we retrieve the copy of the two tables from the database.</p>
<p class="MsoNormal"><span style=""><img src="82208-image.png" alt="" width="637" height="367" align="middle">
</span></p>
<p class="MsoNormal">Second, we change the values in the tables.</p>
<p class="MsoNormal"><span style=""><img src="82209-image.png" alt="" width="637" height="244" align="middle">
</span></p>
<p class="MsoNormal">And then roll back the changes of Department table and reset the values of the Course table.</p>
<p class="MsoNormal"><span style=""><img src="82210-image.png" alt="" width="502" height="198" align="middle">
</span></p>
<p class="MsoNormal">At last, we use Batch update to insert the rows into the database and we can find the new courses in database that are the original course's copiers.</p>
<h2>Using the Code</h2>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
1. Copy the Data from the database.</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
If set the AcceptChangesDuringFill as the false, AcceptChanges will not be called on a DataRow after it is added to the DataTable during any of the Fill operations. We can find the RowState of the rows in the tables are Added instead of Unchanged.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
private static void CopyData(DataSet dataSet,String connectionString,
&nbsp;&nbsp;&nbsp; SqlCommand selectCommand,DataTableMapping[] tableMappings)
{
&nbsp;&nbsp;&nbsp; using (SqlConnection connection = new SqlConnection(connectionString))
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; selectCommand.Connection = connection;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; connection.Open();


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; using (SqlDataAdapter adapter = new SqlDataAdapter(selectCommand))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; adapter.TableMappings.AddRange(tableMappings);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; adapter.AcceptChangesDuringFill = false;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; adapter.Fill(dataSet);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
2. Roll back the changes.</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
RejectChanges will roll back all changes that have been made to the table since it was loaded, or the last time AcceptChanges was called. If we want to copy all the data from the database, we may lose all the data after call the RejectChanges method. The
<span class="SpellE">ResetDataTable</span> method can just roll back one or more columns of data.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
private static void ResetDataTable(DataTable table, String connectionString,
&nbsp;&nbsp;&nbsp; SqlCommand selectCommand)
{
&nbsp;&nbsp;&nbsp; using (SqlConnection connection = new SqlConnection(connectionString))
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; selectCommand.Connection = connection;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; connection.Open();


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; using (SqlDataAdapter adapter = new SqlDataAdapter(selectCommand))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // The incoming values for this row will be written to the current version of each 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// column. The original version of each column's data will not be changed.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; adapter.FillLoadOption = LoadOption.Upsert;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; adapter.Fill(table);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
3. Use Batch update the table.</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
We set the <span class="SpellE">UpdateBatchSize</span> property to set the number of rows that are processed in each round-trip to the server. Set it to 1 disables batch updates, as rows are sent one at a time.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
private static void BatchInsertUpdate(DataTable table, String connectionString, 
&nbsp;&nbsp;&nbsp;&nbsp;SqlCommand insertCommand, Int32 batchSize)
{
&nbsp;&nbsp;&nbsp; using (SqlConnection connection = new SqlConnection(connectionString))
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; insertCommand.Connection = connection;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // When setting UpdateBatchSize to a value other than 1, all the commands 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// associated with the SqlDataAdapter have to have their UpdatedRowSource 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// property set to None or OutputParameters. An exception is thrown otherwise.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; insertCommand.UpdatedRowSource = UpdateRowSource.None;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; connection.Open();


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; using (SqlDataAdapter adapter = new SqlDataAdapter())
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; adapter.InsertCommand = insertCommand;
 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;adapter.UpdateBatchSize = batchSize;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; adapter.Update(table);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Console.WriteLine(&quot;Successfully to update the table.&quot;);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style=""><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/en-us/library/system.data.common.dataadapter.acceptchangesduringfill(v=vs.110).aspx"><span class="SpellE">DataAdapter.AcceptChangesDuringFill</span> Property</a>
</span></p>
<p class="MsoNormal" style=""><a href="http://msdn.microsoft.com/en-us/library/system.data.common.dataadapter.tablemappings(v=vs.110).aspx"><span class="SpellE">DataAdapter.TableMappings</span> Property</a><span class="MsoHyperlink">
</span></p>
<p class="MsoNormal" style=""><a href="http://msdn.microsoft.com/en-us/library/system.data.common.dataadapter.fillloadoption(v=vs.110).aspx"><span class="SpellE">DataAdapter.FillLoadOption</span> Property</a></p>
<p class="MsoNormal" style=""><a href="http://msdn.microsoft.com/en-us/library/aadf8fk2(v=vs.110).aspx">Performing Batch Operations Using
<span class="SpellE">DataAdapters</span></a></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
