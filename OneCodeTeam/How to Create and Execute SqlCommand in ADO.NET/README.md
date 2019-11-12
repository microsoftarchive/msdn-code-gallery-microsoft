# How to Create and Execute SqlCommand in ADO.NET
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- ADO.NET
- Data Access
- .NET Development
## Topics
- Stored Procedures
- sql commond
## Updated
- 04/09/2014
## Description

<h1>How to Create and Execute SqlCommand in ADO.NET</h1>
<h2>Introduction</h2>
<p>We can create and execute different types of SqlCommand. In this applilcation, we will demonstrate how to create and execute SqlCommand:</p>
<p>1. Create different types of SqlCommand;</p>
<p>2. Execute SqlCommand in different ways;</p>
<p>3. Display the result.</p>
<h2>Building the Sample</h2>
<p>Before you run the sample, you need to finish the following steps:</p>
<p>Step1. Please choose one way of the following ways to build the database:</p>
<ul>
<li>Attach the database file MySchool.mdf under the folder _External_Dependecies to your SQL Server (2008 or later version) database instance.
</li><li>Run the MySchool.sql script under the folder _External_Dependecies in your SQL Server (2008 or later version) database instance.
</li></ul>
<p>Step2. Modify the connection string in the Project Properties-&gt;Settings according-&gt; MySchoolConnectionString to your SQL Server (2008 or later version) database instance name.</p>
<h2>Running the Sample</h2>
<p>Press F5 to run the sample.</p>
<p>In this application, we can get the statistical value , the details of department or update the data.</p>
<h1><img id="112533" src="112533-1.png" alt="" width="643" height="237"></h1>
<h2>Using the Code</h2>
<p>1. ExecuteNonQuery method</p>
<p>If you need modify the data (Add, Delete, Update), you can use the method to complete it.</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span><span class="cs__keyword">static</span>&nbsp;Int32&nbsp;ExecuteNonQuery(String&nbsp;connectionString,&nbsp;String&nbsp;commandText,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;CommandType&nbsp;commandType,&nbsp;<span class="cs__keyword">params</span>&nbsp;SqlParameter[]&nbsp;parameters)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(SqlConnection&nbsp;conn&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlConnection(connectionString))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(SqlCommand&nbsp;cmd&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlCommand(commandText,&nbsp;conn))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cmd.CommandType&nbsp;=&nbsp;commandType;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cmd.Parameters.AddRange(parameters);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;conn.Open();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;cmd.ExecuteNonQuery();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p>2. ExecuteScalar method</p>
<p>If you only need one value (first column and first row), you can use the method to get the value, such as the statistical value.</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span><span class="cs__keyword">static</span>&nbsp;Object&nbsp;ExecuteScalar(String&nbsp;connectionString,&nbsp;String&nbsp;commandText,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;CommandType&nbsp;commandType,&nbsp;<span class="cs__keyword">params</span>&nbsp;SqlParameter[]&nbsp;parameters)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(SqlConnection&nbsp;conn&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlConnection(connectionString))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(SqlCommand&nbsp;cmd&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlCommand(commandText,&nbsp;conn))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cmd.CommandType&nbsp;=&nbsp;commandType;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cmd.Parameters.AddRange(parameters);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;conn.Open();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;cmd.ExecuteScalar();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p>3. ExecuteReader method</p>
<p>When you need the details of the data, you can use this method to return the information.</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span><span class="cs__keyword">static</span>&nbsp;SqlDataReader&nbsp;ExecuteReader(String&nbsp;connectionString,&nbsp;String&nbsp;commandText,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;CommandType&nbsp;commandType,&nbsp;<span class="cs__keyword">params</span>&nbsp;SqlParameter[]&nbsp;parameters)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;SqlConnection&nbsp;conn&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlConnection(connectionString);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(SqlCommand&nbsp;cmd&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlCommand(commandText,&nbsp;conn))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cmd.CommandType&nbsp;=&nbsp;commandType;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cmd.Parameters.AddRange(parameters);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;conn.Open();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;When&nbsp;using&nbsp;CommandBehavior.CloseConnection,&nbsp;the&nbsp;connection&nbsp;will&nbsp;be&nbsp;closed&nbsp;when&nbsp;the&nbsp;</span><span class="cs__com">//&nbsp;IDataReader&nbsp;is&nbsp;closed.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SqlDataReader&nbsp;reader&nbsp;=&nbsp;cmd.ExecuteReader(CommandBehavior.CloseConnection);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;reader;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p>4. Parameter</p>
<p>You can add several parameters to the command and set the properties of parameter, such as the value, the type and the direction. If the direction is set as Output, you can get the value of parameter after executing the command.</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<div class="preview">
<pre class="csharp"><span class="cs__com">//&nbsp;Specify&nbsp;the&nbsp;year&nbsp;of&nbsp;StartDate</span>&nbsp;
SqlParameter&nbsp;parameterYear&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlParameter(<span class="cs__string">&quot;@Year&quot;</span>,&nbsp;SqlDbType.Int);&nbsp;
parameterYear.Value&nbsp;=&nbsp;year;&nbsp;
SqlParameter&nbsp;parameterBudget&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlParameter(<span class="cs__string">&quot;@BudgetSum&quot;</span>,&nbsp;SqlDbType.Money);&nbsp;
parameterBudget.Direction&nbsp;=&nbsp;ParameterDirection.Output;&nbsp;
</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<p>5. Command Type.</p>
<p>There're three command types: StoredProcedure, Text (Default), TableDirect. The TableDirect type is only for OLE DB.&nbsp;</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<div class="preview">
<pre class="csharp"><span class="cs__keyword">using</span>&nbsp;(SqlDataReader&nbsp;reader&nbsp;=&nbsp;SqlHelper.ExecuteReader(connectionString,&nbsp;commandText,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;CommandType.StoredProcedure,&nbsp;parameterYear,&nbsp;parameterBudget))&nbsp;
</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<h2>More Information</h2>
<p><a href="http://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlcommand.sqlcommand(VS.110).aspx">SqlCommand Constructor</a></p>
<p><a href="http://msdn.microsoft.com/en-us/library/fs6bx44a(l=en-us,v=VS.110).aspx">SqlCommand.CommandType Property</a></p>
