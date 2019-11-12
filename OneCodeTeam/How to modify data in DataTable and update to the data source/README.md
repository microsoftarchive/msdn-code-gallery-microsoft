# How to modify data in DataTable and update to the data source
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- ADO.NET
- Data Access
- .NET Development
## Topics
- DataTable Edit
- constraints
- deletion
## Updated
- 09/21/2016
## Description

<h1><em><img id="154759" src="154759-8171.onecodesampletopbanner.png" alt=""></em></h1>
<h1>如何修改 DataTable 数据和更新 (CSDataTableModify)</h1>
<h2>简介</h2>
<p>DataTable 数据的修改方式有多种。在该应用程序中，我们将演示如何使用其他方法来修改 DataTable 数据和更新数据源。</p>
<p>1. 使用 SqlDataAdapter 填充 DataTable。</p>
<p>2. 在 DataTable 中设置&nbsp;DataTable 约束。</p>
<p>4. 使用 DataTable 编辑内容来修改数据。</p>
<p>3. 使用 DataRow.Delete 方法和 DataRowCollection.Remove 方法删除行，然后进行比较。</p>
<p>5. 使用 SqlDataAdapter 更新数据源。</p>
<h2>生成示例</h2>
<p>运行该示例之前，您需要完成以下步骤：</p>
<p>步骤 1.请选择以下一种方式来构建数据库：</p>
<ul>
<li>将文件夹_External_Dependecies 下的数据库文件 MySchool.mdf 附加到 SQL Server 2008 数据库实例中。 </li><li>在 SQL Server 2008 数据库实例中，运行文件夹_External_Dependecies 下的 MySchool.sql 脚本。 </li></ul>
<p>步骤 2.将&ldquo;项目属性 -&gt; 相应设置 -&gt;MySchoolConnectionString &rdquo;中的连接字符串修改为 SQL Server 2008 数据库实例名称。</p>
<h2>运行示例</h2>
<p>按 F5 运行示例，结果如下所示。</p>
<p>首先，获取数据库数据。</p>
<p><img id="154769" src="154769-image.png" alt=""></p>
<p>然后，使用 DataTable 编辑内容来修改数据。</p>
<p>a. 更改Credits 列中的两个&#20540;，其中一个&#20540;更改会生成以下显示内容，并取消编辑内容。</p>
<p><img id="154770" src="154770-fd7d1aa3-ccc4-4890-afff-c03f75c69697image.png" alt="" width="392" height="44"></p>
<p>b. 更改Credits 的前两个&#20540;，但只有第二个&#20540;发生变化。<strong>&nbsp;</strong><em>&nbsp;</em></p>
<p><img id="154764" src="154764-9d02f6f0-79d8-462b-9fda-335f4106b1c0image.png" alt=""><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p>接着，从 DataTable 中删除并移除行。</p>
<p>a. 由于我们设置了外键约束，因此在父表中的行遭到删除后，子表中的相关行也会随之删除。<strong>&nbsp;</strong><em></em></p>
<p><img id="154765" src="154765-c1eeacee-75db-4e62-8cf5-c73f0e001ba1image.png" alt=""><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p>然后，可以更新 Delete 操作。现在，已删除的行已从 DataTable 和数据库中移除。<strong></strong><em></em><img id="154768" src="154768-1c9c9667-7c43-4ed0-8682-f33f042901bdimage.png" alt="" width="642" height="136"></p>
<p>b. 我们还可以移除 DataTable 中的行，此时行不存在于表&#26684;中。</p>
<p><img id="154774" src="154774-61196964-a807-46c6-a3af-973363cac941image.png" alt="" width="641" height="93"></p>
<p>不过，更新 Delete 操作后，我们还会发现行存在于数据库中。此Remove 操作只会移除 DataTable 中的行，不会改变数据库中的&#20540;。<strong></strong><em></em></p>
<p><img id="154771" src="154771-153a02cc-7b72-410d-9ac3-5b6ffc1ec323image.png" alt=""></p>
<h2>使用代码</h2>
<p>1. 使用 SqlDataAdapter 获取数据。</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span><span class="cs__keyword">static</span><span class="cs__keyword">void</span>&nbsp;GetDataTables(String&nbsp;connectionString,String&nbsp;selectString,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;DataSet&nbsp;dataSet,<span class="cs__keyword">params</span>&nbsp;DataTable[]&nbsp;tables)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(SqlDataAdapter&nbsp;adapter&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlDataAdapter())&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;adapter.SelectCommand&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlCommand(selectString);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;adapter.SelectCommand.Connection&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlConnection(connectionString);&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;adapter.Fill(<span class="cs__number">0</span>,&nbsp;<span class="cs__number">0</span>,tables);&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">foreach</span>&nbsp;(DataTable&nbsp;table&nbsp;<span class="cs__keyword">in</span>&nbsp;dataSet.Tables)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__string">&quot;Data&nbsp;in&nbsp;{0}:&quot;</span>,table.TableName);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ShowDataTable(table);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;2. 使用 DataTable 编辑内容来修改数据。<strong></strong><em></em></div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp">row.BeginEdit();&nbsp;
row[<span class="cs__string">&quot;Credits&quot;</span>]&nbsp;=&nbsp;credits;&nbsp;
row.EndEdit();&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;在表&#26684;中的&#20540;发生变化后，将会触发以下方法。如果Credits 的新&#20540;为负&#20540;，则我们会拒绝修改。<strong></strong><em></em></div>
<div class="endscriptcode"></div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;OnColumnChanged(Object&nbsp;sender,&nbsp;DataColumnChangeEventArgs&nbsp;args)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Int32&nbsp;credits&nbsp;=&nbsp;<span class="cs__number">0</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;如果&nbsp;Credits&nbsp;发生变化且&#20540;为负&#20540;，则我们会取消编辑内容。</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;((args.Column.ColumnName&nbsp;==&nbsp;<span class="cs__string">&quot;Credits&quot;</span>)&amp;&amp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(!Int32.TryParse(args.ProposedValue.ToString(),<span class="cs__keyword">out</span>&nbsp;credits)||credits&lt;<span class="cs__number">0</span>))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__string">&quot;The&nbsp;value&nbsp;of&nbsp;Credits&nbsp;is&nbsp;invalid.&nbsp;Edit&nbsp;canceled.&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;args.Row.CancelEdit();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
</div>
<p>3. 从 DataTable 中删除并移除行。</p>
<p>a.删除行</p>
<p>创建外键约束，并将DeleteRule 设置为 Cascade.</p>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp">ForeignKeyConstraint&nbsp;courseDepartFK&nbsp;=&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">new</span>&nbsp;ForeignKeyConstraint(<span class="cs__string">&quot;CourseDepartFK&quot;</span>,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;department.Columns[<span class="cs__string">&quot;DepartmentID&quot;</span>],&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;course.Columns[<span class="cs__string">&quot;DepartmentID&quot;</span>]);&nbsp;
courseDepartFK.DeleteRule&nbsp;=&nbsp;Rule.Cascade;&nbsp;
courseDepartFK.UpdateRule&nbsp;=&nbsp;Rule.Cascade;&nbsp;
courseDepartFK.AcceptRejectRule&nbsp;=&nbsp;AcceptRejectRule.None;&nbsp;
course.Constraints.Add(courseDepartFK);&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;从 department 中删除一行，Course 表&#26684;中的相关行也会遭到删除。然后，使用 Deleted 操作更新Course 表&#26684;，数据库中的行也会遭到删除。<strong></strong><em></em></div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp">department.Rows[<span class="cs__number">0</span>].Delete();</pre>
</div>
</div>
</div>
</div>
</div>
<p>b. 移除行</p>
<p>从 course 表&#26684;中移除一行。然后，使用 Deleted 操作更新Course 表&#26684;，我们会发现行仍存在于数据库中。Remove 操作只会移除 DataTable 中的行，不会改变数据库中的&#20540;。<strong></strong><em></em></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp">course.Rows.RemoveAt(<span class="cs__number">0</span>);</pre>
</div>
</div>
</div>
<p><a href="http://msdn.microsoft.com/zh-cn/library/ch2aw0w6(VS.110).aspx">DataTable 编辑内容</a></p>
<p><a href="http://msdn.microsoft.com/zh-cn/library/st1t2c35(VS.110).aspx">DataTable 约束</a></p>
<p><a href="http://msdn.microsoft.com/zh-cn/library/03c7a3zb(VS.110).aspx">DataRow 删除</a></p>
<p><a href="http://msdn.microsoft.com/zh-cn/library/system.data.datarowcollection.remove(VS.110).aspx">DataRowCollection.Remove 方法</a></p>
<p><a href="http://msdn.microsoft.com/zh-cn/library/system.data.datarowcollection.indexof.aspx">DataRowCollection.IndexOf 方法</a></p>
<p><a href="http://msdn.microsoft.com/zh-cn/library/at8a576f(VS.100).aspx">DbDataAdapter.Update 方法 (DataSet)</a><strong></strong><em></em></p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p><img id="154773" src="154773-a2aa0602-8919-4a19-9dcf-bdbc504c20dfimage.png" alt=""></p>
<p><em>&nbsp;</em><strong>&nbsp;</strong><em>&nbsp;</em><span style="text-decoration:underline">&nbsp;</span><span style="text-decoration:line-through">&nbsp;</span></p>
