# How to create DataTable manually with specific schema definitions
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- ADO.NET
- Data Access
- .NET Development
## Topics
- Create DataTable
- schema definition
## Updated
- 08/17/2016
## Description

<h1>How to Create DataTable Manually with Specific Schema Definitions (CSDataTableCreateManually)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">Sometimes we may need to create the DataTable manually and some specific schema definitions, such as Foreign Key constraints, expression columns and so on.</p>
<p class="MsoNormal">In this sample, we will demonstrate how to create the DataTable manually with specific schema definitions:</p>
<p class="MsoNormal">1. Create multiple DataTable and define the initial columns.</p>
<p class="MsoNormal">2. Create the constraints on the tables.</p>
<p class="MsoNormal">3. Insert the values and show the tables.</p>
<p class="MsoNormal">4. Create the expression columns and show the tables.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Press F5 to run the sample, the following is the result.</p>
<p class="MsoNormal"><span><img src="91869-image.png" alt="" width="645" height="87" align="middle">
</span></p>
<p class="MsoNormal">First, create OrderTable and OrderDetailTable, and then define the initial columns.</p>
<p class="MsoNormal">Second, create the relation and constraint on the tables. If inserting a row in OrderDetailTable with the wrong OrderId, we will get the error message.</p>
<h2><span><img src="91870-image.png" alt="" width="644" height="375" align="middle">
</span></h2>
<p class="MsoNormal">Third, insert the rows into the tables and show the tables. The initial Order table has two columns, OrderId and OrderDate.</p>
<p class="MsoNormal"><span><img src="91871-image.png" alt="" width="637" height="124" align="middle">
</span></p>
<p class="MsoNormal">At last, create the expression columns and show the result. Now the Order table has five columns, and the new three column are the expression columns.</p>
<h2>Using the Code</h2>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
1. Create tables and define columns.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
a. Define one column once.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">DataTable orderTable = new DataTable(&quot;Order&quot;);


DataColumn colId = new DataColumn(&quot;OrderId&quot;, typeof(String));
orderTable.Columns.Add(colId);


DataColumn colDate = new DataColumn(&quot;OrderDate&quot;, typeof(DateTime));
orderTable.Columns.Add(colDate);

</pre>
<pre id="codePreview" class="csharp">DataTable orderTable = new DataTable(&quot;Order&quot;);


DataColumn colId = new DataColumn(&quot;OrderId&quot;, typeof(String));
orderTable.Columns.Add(colId);


DataColumn colDate = new DataColumn(&quot;OrderDate&quot;, typeof(DateTime));
orderTable.Columns.Add(colDate);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
b. Define all columns once.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">DataTable orderDetailTable = new DataTable(&quot;OrderDetail&quot;);


DataColumn[] cols ={
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new DataColumn(&quot;OrderDetailId&quot;,typeof(Int32)),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;new DataColumn(&quot;OrderId&quot;,typeof(String)),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new DataColumn(&quot;Product&quot;,typeof(String)),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new DataColumn(&quot;UnitPrice&quot;,typeof(Decimal)),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new DataColumn(&quot;OrderQty&quot;,typeof(Int32)),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;new DataColumn(&quot;LineTotal&quot;,typeof(Decimal),&quot;UnitPrice*OrderQty&quot;)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; };
orderDetailTable.Columns.AddRange(cols);

</pre>
<pre id="codePreview" class="csharp">DataTable orderDetailTable = new DataTable(&quot;OrderDetail&quot;);


DataColumn[] cols ={
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new DataColumn(&quot;OrderDetailId&quot;,typeof(Int32)),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;new DataColumn(&quot;OrderId&quot;,typeof(String)),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new DataColumn(&quot;Product&quot;,typeof(String)),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new DataColumn(&quot;UnitPrice&quot;,typeof(Decimal)),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new DataColumn(&quot;OrderQty&quot;,typeof(Int32)),
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;new DataColumn(&quot;LineTotal&quot;,typeof(Decimal),&quot;UnitPrice*OrderQty&quot;)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; };
orderDetailTable.Columns.AddRange(cols);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
2. Create the relation and constraint on the tables.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
Create the foreign key relation and constraint between the Order table and OrderDetail table.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">salesSet.Relations.Add(&quot;OrderOrderDetail&quot;,
&nbsp;&nbsp;&nbsp; orderTable.Columns[&quot;OrderId&quot;], orderDetailTable.Columns[&quot;OrderId&quot;], true);

</pre>
<pre id="codePreview" class="csharp">salesSet.Relations.Add(&quot;OrderOrderDetail&quot;,
&nbsp;&nbsp;&nbsp; orderTable.Columns[&quot;OrderId&quot;], orderDetailTable.Columns[&quot;OrderId&quot;], true);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
3. Insert the rows into the table.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
a. Insert one row once.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">DataRow row1 = orderTable.NewRow();
row1[&quot;OrderId&quot;] = &quot;O0001&quot;;
row1[&quot;OrderDate&quot;] = new DateTime(2013, 3, 1);
orderTable.Rows.Add(row1);


DataRow row2 = orderTable.NewRow();
row2[&quot;OrderId&quot;] = &quot;O0002&quot;;
row2[&quot;OrderDate&quot;] = new DateTime(2013, 3, 12);
orderTable.Rows.Add(row2);


DataRow row3 = orderTable.NewRow();
row3[&quot;OrderId&quot;] = &quot;O0003&quot;;
row3[&quot;OrderDate&quot;] = new DateTime(2013, 3, 20);
orderTable.Rows.Add(row3);

</pre>
<pre id="codePreview" class="csharp">DataRow row1 = orderTable.NewRow();
row1[&quot;OrderId&quot;] = &quot;O0001&quot;;
row1[&quot;OrderDate&quot;] = new DateTime(2013, 3, 1);
orderTable.Rows.Add(row1);


DataRow row2 = orderTable.NewRow();
row2[&quot;OrderId&quot;] = &quot;O0002&quot;;
row2[&quot;OrderDate&quot;] = new DateTime(2013, 3, 12);
orderTable.Rows.Add(row2);


DataRow row3 = orderTable.NewRow();
row3[&quot;OrderId&quot;] = &quot;O0003&quot;;
row3[&quot;OrderDate&quot;] = new DateTime(2013, 3, 20);
orderTable.Rows.Add(row3);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
b. Insert all rows once.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">Object[] rows = {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new Object[]{1,&quot;O0001&quot;,&quot;Mountain Bike&quot;,1419.5,36},
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new Object[]{2,&quot;O0001&quot;,&quot;Road Bike&quot;,1233.6,16},
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new Object[]{3,&quot;O0001&quot;,&quot;Touring Bike&quot;,1653.3,32},
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new Object[]{4,&quot;O0002&quot;,&quot;Mountain Bike&quot;,1419.5,24},
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new Object[]{5,&quot;O0002&quot;,&quot;Road Bike&quot;,1233.6,12},
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new Object[]{6,&quot;O0003&quot;,&quot;Mountain Bike&quot;,1419.5,48},
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new Object[]{7,&quot;O0003&quot;,&quot;Touring Bike&quot;,1653.3,8},
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;};


foreach (Object[] row in rows)
{
&nbsp;&nbsp;&nbsp; orderDetailTable.Rows.Add(row);
}

</pre>
<pre id="codePreview" class="csharp">Object[] rows = {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new Object[]{1,&quot;O0001&quot;,&quot;Mountain Bike&quot;,1419.5,36},
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new Object[]{2,&quot;O0001&quot;,&quot;Road Bike&quot;,1233.6,16},
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new Object[]{3,&quot;O0001&quot;,&quot;Touring Bike&quot;,1653.3,32},
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new Object[]{4,&quot;O0002&quot;,&quot;Mountain Bike&quot;,1419.5,24},
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new Object[]{5,&quot;O0002&quot;,&quot;Road Bike&quot;,1233.6,12},
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new Object[]{6,&quot;O0003&quot;,&quot;Mountain Bike&quot;,1419.5,48},
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; new Object[]{7,&quot;O0003&quot;,&quot;Touring Bike&quot;,1653.3,8},
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;};


foreach (Object[] row in rows)
{
&nbsp;&nbsp;&nbsp; orderDetailTable.Rows.Add(row);
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
4. Create the expression columns.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
a. Use the Aggregate-Sum on the child table column to get the result.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">DataColumn colSub = new DataColumn(&quot;SubTotal&quot;, typeof(Decimal), &quot;Sum(Child.LineTotal)&quot;);
orderTable.Columns.Add(colSub);

</pre>
<pre id="codePreview" class="csharp">DataColumn colSub = new DataColumn(&quot;SubTotal&quot;, typeof(Decimal), &quot;Sum(Child.LineTotal)&quot;);
orderTable.Columns.Add(colSub);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
b. Compute the tax by referencing the SubTotal expression column.<span style="font-size:9.5pt; font-family:Consolas; color:green">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">DataColumn colTax = new DataColumn(&quot;Tax&quot;, typeof(Decimal), &quot;SubTotal*0.1&quot;);
orderTable.Columns.Add(colTax);

</pre>
<pre id="codePreview" class="csharp">DataColumn colTax = new DataColumn(&quot;Tax&quot;, typeof(Decimal), &quot;SubTotal*0.1&quot;);
orderTable.Columns.Add(colTax);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
c. If the OrderId is 'Total', compute the due on all orders; or compute the due on this order.<span style="font-size:9.5pt; font-family:Consolas; color:green">
</span></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">DataColumn colTotal = new DataColumn(&quot;TotalDue&quot;, typeof(Decimal), 
&nbsp;&nbsp;&nbsp;&nbsp;&quot;IIF(OrderId='Total',Sum(SubTotal)&#43;Sum(Tax),SubTotal&#43;Tax)&quot;);
orderTable.Columns.Add(colTotal);


DataRow row = orderTable.NewRow();
row[&quot;OrderId&quot;] = &quot;Total&quot;;
orderTable.Rows.Add(row);

</pre>
<pre id="codePreview" class="csharp">DataColumn colTotal = new DataColumn(&quot;TotalDue&quot;, typeof(Decimal), 
&nbsp;&nbsp;&nbsp;&nbsp;&quot;IIF(OrderId='Total',Sum(SubTotal)&#43;Sum(Tax),SubTotal&#43;Tax)&quot;);
orderTable.Columns.Add(colTotal);


DataRow row = orderTable.NewRow();
row[&quot;OrderId&quot;] = &quot;Total&quot;;
orderTable.Rows.Add(row);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2>More Information</h2>
<p class="MsoNormal"><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/en-us/library/system.data.datacolumn.expression(v=vs.110).aspx"><span class="SpellE">DataColumn.Expression</span> Property
</a></span></p>
<p class="MsoNormal"><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/en-us/library/hfx3s9wd(v=vs.110).aspx">Adding Columns to a
<span class="SpellE">DataTable</span> (ADO.NET) </a></span></p>
<p class="MsoNormal"><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/en-us/library/6zd7cwzh(v=vs.110).aspx">Creating a
<span class="SpellE">DataTable</span> (ADO.NET) </a></span></p>
<p class="MsoNormal"><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/en-us/library/zwxk25bd(v=vs.110).aspx">Creating Expression Columns (ADO.NET)</a>
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
