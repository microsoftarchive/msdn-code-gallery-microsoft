# How to retrieve the Identity value after inserting a data using ADO.NET
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- ADO.NET
- Data Access
- .NET Development
## Topics
- Identity
- auto number
## Updated
- 03/24/2014
## Description

<h1>How to Retrieve the Identity Values in ADO.NET</h1>
<h2>Introduction</h2>
<p>We often set a column as identity column when the values in the column must be unique. And sometimes we need the identity values of new data. In this application, we will demonstrate how to retrieve the identity values:</p>
<p>1. Create a stored procedure to insert data and return identity values;</p>
<p>2. Execute a command to insert the new data and display the result;</p>
<p>3. Use SqlDataAdapter to insert new data and display the result.</p>
<h2>Building the Sample</h2>
<p>Before you run the sample, you need to finish the following steps:</p>
<p>Step1. Please choose one of the following ways to build the database:</p>
<ul>
<li>Attach the database file MySchool.mdf under the folder _External_Dependecies to your SQL Server (2008 or later version) database instance.
</li><li>Run the MySchool.sql script under the folder _External_Dependecies in your SQL Server (2008 or later version) database instance.
</li></ul>
<p>Step2. Modify the connection string in the Project Properties-&gt;Settings according-&gt; MySchoolConnectionString to your SQL Server (2008 or later version) database instance name.</p>
<h2>Running the Sample</h2>
<p>Press F5 to run the sample.</p>
<p>We use the stored procedure to insert a new row and retrieve the identity values.</p>
<p><img id="111053" src="111053-1.jpg" alt="" width="631" height="30"></p>
<p>We can also use the data adapter to update the new row and the identity value automatically.</p>
<p><img id="111054" src="111054-2.jpg" alt="" width="643" height="143"></p>
<p>For the Jet 4.0 database, we will use the single statement, event handler and merge method to retrieve the new identity.</p>
<p>This is how it looks like before merging:</p>
<p><img id="111055" src="111055-3.jpg" alt="" width="640" height="128"></p>
<p>This is how it looks like after merging:</p>
<p><img id="111056" src="111056-4.jpg" alt="" width="643" height="134"></p>
<h2>Using the Code</h2>
<p>1. Stored Procedure</p>
<p>We use the stored procedure to insert rows and return the identity values.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>SQL</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">mysql</span>

<div class="preview">
<pre class="mysql"><span class="sql__keyword">CREATE</span>&nbsp;<span class="sql__keyword">PROCEDURE</span>&nbsp;[<span class="sql__id">dbo</span>].[<span class="sql__id">InsertPerson</span>]&nbsp;&nbsp;
&nbsp;<span class="sql__com">--&nbsp;Add&nbsp;the&nbsp;parameters&nbsp;for&nbsp;the&nbsp;stored&nbsp;procedure&nbsp;here</span>&nbsp;
&nbsp;<span class="sql__keyword">@</span><span class="sql__variable">FirstName</span>&nbsp;<span class="sql__keyword">nvarchar</span>(<span class="sql__number">50</span>),<span class="sql__keyword">@</span><span class="sql__variable">LastName</span>&nbsp;<span class="sql__keyword">nvarchar</span>(<span class="sql__number">50</span>),&nbsp;
&nbsp;<span class="sql__keyword">@</span><span class="sql__variable">PersonID</span>&nbsp;<span class="sql__keyword">int</span>&nbsp;<span class="sql__id">output</span>&nbsp;
<span class="sql__keyword">AS</span>&nbsp;
<span class="sql__keyword">BEGIN</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="sql__keyword">insert</span>&nbsp;[<span class="sql__id">dbo</span>].[<span class="sql__id">Person</span>](<span class="sql__id">LastName</span>,<span class="sql__id">FirstName</span>)&nbsp;<span class="sql__keyword">Values</span>(<span class="sql__keyword">@</span><span class="sql__variable">LastName</span>,<span class="sql__keyword">@</span><span class="sql__variable">FirstName</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="sql__keyword">set</span>&nbsp;<span class="sql__keyword">@</span><span class="sql__variable">PersonID</span>=<span class="sql__id">SCOPE_IDENTITY</span>()&nbsp;
<span class="sql__keyword">END</span>&nbsp;</pre>
</div>
</div>
</div>
<p>2. Use the Stored Procedure to Insert Rows</p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<div class="preview">
<pre class="csharp">String&nbsp;commandText&nbsp;=&nbsp;<span class="cs__string">&quot;dbo.InsertPerson&quot;</span>;&nbsp;
<span class="cs__keyword">using</span>&nbsp;(SqlConnection&nbsp;conn&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlConnection(connectionString))&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">using</span>&nbsp;(SqlCommand&nbsp;cmd&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlCommand(commandText,&nbsp;conn))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cmd.CommandType&nbsp;=&nbsp;CommandType.StoredProcedure;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cmd.Parameters.Add(<span class="cs__keyword">new</span>&nbsp;SqlParameter(<span class="cs__string">&quot;@FirstName&quot;</span>,&nbsp;firstName));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cmd.Parameters.Add(<span class="cs__keyword">new</span>&nbsp;SqlParameter(<span class="cs__string">&quot;@LastName&quot;</span>,&nbsp;lastName));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SqlParameter&nbsp;personId&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlParameter(<span class="cs__string">&quot;@PersonID&quot;</span>,&nbsp;SqlDbType.Int);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;personId.Direction&nbsp;=&nbsp;ParameterDirection.Output;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cmd.Parameters.Add(personId);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;conn.Open();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cmd.ExecuteNonQuery();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__string">&quot;Person&nbsp;Id&nbsp;of&nbsp;new&nbsp;person:{0}&quot;</span>,&nbsp;personId.Value);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;</pre>
</div>
</div>
</div>
<p></p>
<div class="endscriptcode">
<div class="endscriptcode"></div>
</div>
<p>3. Use DataAdater to Insert Rows</p>
<p>We can use data adapter to insert rows and update the identity values automatically.</p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<div class="preview">
<pre class="csharp">String&nbsp;commandText&nbsp;=&nbsp;<span class="cs__string">&quot;dbo.InsertPerson&quot;</span>;&nbsp;
<span class="cs__keyword">using</span>&nbsp;(SqlConnection&nbsp;conn&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlConnection(connectionString))&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;SqlDataAdapter&nbsp;mySchool&nbsp;=&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlDataAdapter(<span class="cs__string">&quot;Select&nbsp;PersonID,FirstName,LastName&nbsp;from&nbsp;[dbo].[Person]&quot;</span>,&nbsp;conn);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;mySchool.InsertCommand&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlCommand(commandText,&nbsp;conn);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;mySchool.InsertCommand.CommandType&nbsp;=&nbsp;CommandType.StoredProcedure;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;mySchool.InsertCommand.Parameters.Add(&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlParameter(<span class="cs__string">&quot;@FirstName&quot;</span>,&nbsp;SqlDbType.NVarChar,&nbsp;<span class="cs__number">50</span>,&nbsp;<span class="cs__string">&quot;FirstName&quot;</span>));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;mySchool.InsertCommand.Parameters.Add(&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlParameter(<span class="cs__string">&quot;@LastName&quot;</span>,&nbsp;SqlDbType.NVarChar,&nbsp;<span class="cs__number">50</span>,&nbsp;<span class="cs__string">&quot;LastName&quot;</span>));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;SqlParameter&nbsp;personId&nbsp;=&nbsp;mySchool.InsertCommand.Parameters.Add(&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlParameter(<span class="cs__string">&quot;@PersonID&quot;</span>,&nbsp;SqlDbType.Int,&nbsp;<span class="cs__number">0</span>,&nbsp;<span class="cs__string">&quot;PersonID&quot;</span>));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;personId.Direction&nbsp;=&nbsp;ParameterDirection.Output;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;DataTable&nbsp;persons&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DataTable();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;mySchool.Fill(persons);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;DataRow&nbsp;newPerson&nbsp;=&nbsp;persons.NewRow();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;newPerson[<span class="cs__string">&quot;FirstName&quot;</span>]&nbsp;=&nbsp;firstName;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;newPerson[<span class="cs__string">&quot;LastName&quot;</span>]&nbsp;=&nbsp;lastName;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;persons.Rows.Add(newPerson);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;mySchool.Update(persons);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__string">&quot;Show&nbsp;all&nbsp;persons:&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ShowDataTable(persons,&nbsp;<span class="cs__number">14</span>);&nbsp;
}&nbsp;</pre>
</div>
</div>
</div>
<p></p>
<div class="endscriptcode">
<div class="endscriptcode"></div>
</div>
<p>4. Retrieve the Identity Value from the Jet 4.0 Database</p>
<p>a. Set the Insert Command</p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<div class="preview">
<pre class="csharp">mySchool.InsertCommand&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;OleDbCommand(commandText,&nbsp;conn);&nbsp;
mySchool.InsertCommand.CommandType&nbsp;=&nbsp;CommandType.Text;&nbsp;
mySchool.InsertCommand.Parameters.Add(&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">new</span>&nbsp;OleDbParameter(<span class="cs__string">&quot;@FirstName&quot;</span>,&nbsp;OleDbType.VarChar,&nbsp;<span class="cs__number">50</span>,&nbsp;<span class="cs__string">&quot;FirstName&quot;</span>));&nbsp;
mySchool.InsertCommand.Parameters.Add(&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">new</span>&nbsp;OleDbParameter(<span class="cs__string">&quot;@LastName&quot;</span>,&nbsp;OleDbType.VarChar,&nbsp;<span class="cs__number">50</span>,&nbsp;<span class="cs__string">&quot;LastName&quot;</span>));&nbsp;
mySchool.InsertCommand.UpdatedRowSource&nbsp;=&nbsp;UpdateRowSource.Both;&nbsp;&nbsp;</pre>
</div>
</div>
</div>
<p></p>
<div class="endscriptcode">
<div class="endscriptcode"></div>
</div>
<p>b. Create the Table</p>
<p>We will create the table Persons so that we can define some properties of the table.</p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;DataTable&nbsp;CreatePersonsTable()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;DataTable&nbsp;persons&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DataTable();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;DataColumn&nbsp;personId&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DataColumn();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;personId.DataType&nbsp;=&nbsp;Type.GetType(<span class="cs__string">&quot;System.Int32&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;personId.ColumnName&nbsp;=&nbsp;<span class="cs__string">&quot;PersonID&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;personId.AutoIncrement&nbsp;=&nbsp;<span class="cs__keyword">true</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;personId.AutoIncrementSeed&nbsp;=&nbsp;<span class="cs__number">0</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;personId.AutoIncrementStep&nbsp;=&nbsp;-<span class="cs__number">1</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;persons.Columns.Add(personId);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;DataColumn&nbsp;firstName&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DataColumn();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;firstName.DataType&nbsp;=&nbsp;Type.GetType(<span class="cs__string">&quot;System.String&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;firstName.ColumnName&nbsp;=&nbsp;<span class="cs__string">&quot;FirstName&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;persons.Columns.Add(firstName);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;DataColumn&nbsp;lastName&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DataColumn();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;lastName.DataType&nbsp;=&nbsp;Type.GetType(<span class="cs__string">&quot;System.String&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;lastName.ColumnName&nbsp;=&nbsp;<span class="cs__string">&quot;LastName&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;persons.Columns.Add(lastName);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;DataColumn[]&nbsp;pkey&nbsp;=&nbsp;{&nbsp;personId&nbsp;};&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;persons.PrimaryKey&nbsp;=&nbsp;pkey;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;persons;&nbsp;
}&nbsp;</pre>
</div>
</div>
</div>
<p></p>
<div class="endscriptcode">
<div class="endscriptcode"></div>
</div>
<p>c. Using the Event Handler</p>
<p>In this event handler, we will retrieve the identity value and keep the original value.</p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<div class="preview">
<pre class="csharp"><span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;OnRowUpdated(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;OleDbRowUpdatedEventArgs&nbsp;e)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(e.StatementType&nbsp;==&nbsp;StatementType.Insert)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Retrieve&nbsp;the&nbsp;identity&nbsp;value</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OleDbCommand&nbsp;cmdNewId&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;OleDbCommand(<span class="cs__string">&quot;Select&nbsp;@@IDENTITY&quot;</span>,&nbsp;e.Command.Connection);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;e.Row[<span class="cs__string">&quot;PersonID&quot;</span>]&nbsp;=&nbsp;(Int32)cmdNewId.ExecuteScalar();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;After&nbsp;the&nbsp;status&nbsp;is&nbsp;changed,&nbsp;the&nbsp;original&nbsp;values&nbsp;in&nbsp;the&nbsp;row&nbsp;are&nbsp;preserved.&nbsp;And&nbsp;the&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Merge&nbsp;method&nbsp;will&nbsp;be&nbsp;called&nbsp;to&nbsp;merge&nbsp;the&nbsp;new&nbsp;identity&nbsp;value&nbsp;into&nbsp;the&nbsp;original&nbsp;DataTable.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;e.Status&nbsp;=&nbsp;UpdateStatus.SkipCurrentRow;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;</pre>
</div>
</div>
</div>
<p></p>
<div class="endscriptcode"></div>
<p>d. Using Merge Method</p>
<p>We use Merge method to update the identity value.</p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<div class="preview">
<pre class="csharp">persons.Merge(dataChanges);&nbsp;
persons.AcceptChanges();&nbsp;</pre>
</div>
</div>
</div>
<p></p>
<div class="endscriptcode"></div>
<h2>More Information</h2>
<p><a href="http://msdn.microsoft.com/en-us/library/ks9f57t0(VS.110).aspx">Retrieving Identity or Autonumber Values</a></p>
