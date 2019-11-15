# C# app uses ADO to access database (CSUseADO)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- ADO
## Topics
- Interop
- Data Platform
## Updated
- 03/04/2012
## Description

<h1>C# app uses ADO to access database (<span class="SpellE">CSUseADO</span>) <span style="">
</span></h1>
<h2>Introduction<span style=""> </span></h2>
<p class="MsoNormal"><span style="">The <span class="SpellE">CSUseADO</span> sample demonstrates the Microsoft ADO technology to access databases using Visual C#. It shows the basic structure of connecting to a data source, issuing SQL commands, using Recordset
 object and performing the cleanup. </span></p>
<h2><span style="">Building the sample </span></h2>
<p class="MsoNormal"><span style="">Attach the <b style="">_<span class="SpellE">External_Dependencies</span>\SQLServer2005DB.mdf</b> to your SQL Server (2005 or later versions).
</span></p>
<h2>Running the Sample</h2>
<p class="MsoNormal"><span style=""><img src="53754-image.png" alt="" width="826" height="366" align="middle">
</span></p>
<h2>Using the Code</h2>
<p class="MsoListParagraph" style=""><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Connect to the data source<span style="">.</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
////////////////////////////////////////////////////////////////////////////////
// Connect to the data source.
// 


Console.WriteLine(&quot;Connecting to the database ...&quot;);


// Get the connection string 
string connStr = string.Format(&quot;Provider=SQLOLEDB;Data Source={0};Initial Catalog={1};Integrated Security=SSPI&quot;,
    &quot;.\\sqlexpress&quot;, &quot;SQLServer2005DB&quot;);


// Open the connection
conn = new ADODB.Connection();
conn.Open(connStr, null, null, 0);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Build and execute an ADO command. </p>
<p class="MsoListParagraphCxSpLast"><span style="">T</span>he command can be a SQL statement (SELECT/UPDATE/INSERT/DELETE), or a stored<span style="">
</span>procedure call. <span style=""></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
////////////////////////////////////////////////////////////////////////////////
// Build and Execute an ADO Command.
// It can be a SQL statement (SELECT/UPDATE/INSERT/DELETE), or a stored 
// procedure call. Here is the sample of an INSERT command.
// 


Console.WriteLine(&quot;Inserting a record to the CountryRegion table...&quot;);


// 1. Create a command object
ADODB.Command cmdInsert = new ADODB.Command();


// 2. Assign the connection to the command
cmdInsert.ActiveConnection = conn;


// 3. Set the command text
// SQL statement or the name of the stored procedure 
cmdInsert.CommandText = &quot;INSERT INTO CountryRegion(CountryRegionCode, Name, ModifiedDate)&quot;
    &#43; &quot; VALUES (?, ?, ?)&quot;;


// 4. Set the command type
// ADODB.CommandTypeEnum.adCmdText for oridinary SQL statements; 
// ADODB.CommandTypeEnum.adCmdStoredProc for stored procedures.
cmdInsert.CommandType = ADODB.CommandTypeEnum.adCmdText;


// 5. Append the parameters


// Append the parameter for CountryRegionCode (nvarchar(20)
ADODB.Parameter paramLN = cmdInsert.CreateParameter(
    &quot;CountryRegionCode&quot;,                        // Parameter name
    ADODB.DataTypeEnum.adVarChar,               // Parameter type (nvarchar(20))
    ADODB.ParameterDirectionEnum.adParamInput,  // Parameter direction
    20,                                         // Max size of value in bytes
    &quot;ZZ&quot;&#43;DateTime.Now.Millisecond);             // Parameter value
cmdInsert.Parameters.Append(paramLN);


// Append the parameter for FirstName (nvarchar(200))
ADODB.Parameter paramFN = cmdInsert.CreateParameter(
    &quot;FirstName&quot;,                                // Parameter name
    ADODB.DataTypeEnum.adVarChar,               // Parameter type (nvarchar(200))
    ADODB.ParameterDirectionEnum.adParamInput,  // Parameter direction
    200,                                        // Max size of value in bytes
    &quot;Test Region Name&quot;);                        // Parameter value
cmdInsert.Parameters.Append(paramFN);


// Append the parameter for EnrollmentDate (datetime)
ADODB.Parameter paramED = cmdInsert.CreateParameter(
    &quot;ModifiedDate&quot;,                             // Parameter name
    ADODB.DataTypeEnum.adDate,                  // Parameter type (datetime)
    ADODB.ParameterDirectionEnum.adParamInput,  // Parameter direction
    -1,                                         // Max size (ignored for datetime)
    DateTime.Now);                              // Parameter value
cmdInsert.Parameters.Append(paramED);


          
// 6. Execute the command
object nRecordsAffected = Type.Missing;
object oParams = Type.Missing;
cmdInsert.Execute(out nRecordsAffected, ref oParams,
    (int)ADODB.ExecuteOptionEnum.adExecuteNoRecords);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpMiddle"><span style=""></span></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Use the Recordset object. </p>
<p class="MsoListParagraphCxSpLast">Recordset represents the entire set of records from a base table or the results of an executed command. It facilitates the enumeration, insertion, update, deletion of records. At any time, the Recordset object refers to
 only a single record within the set as the current record.<span style=""> </span>
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
Console.WriteLine(&quot;Enumerating the records in the CountryRegion table&quot;);


// 1. Create a Recordset object
rs = new ADODB.Recordset();


// 2. Open the Recordset object
string strSelectCmd = &quot;SELECT * FROM CountryRegion&quot;; // WHERE ...
rs.Open(strSelectCmd,                                // SQL statement / table,view name / 
                                                     // stored procedure call / file name
    conn,                                            // Connection / connection string
    ADODB.CursorTypeEnum.adOpenForwardOnly,          // Cursor type. (forward-only cursor)
    ADODB.LockTypeEnum.adLockOptimistic,             // Lock type. (locking records only 
                                                     // when you call the Update method.
    (int)ADODB.CommandTypeEnum.adCmdText);             // Evaluate the first parameter as
                                                     // a SQL command or stored procedure.


// 3. Enumerate the records by moving the cursor forward


// Move to the first record in the Recordset
rs.MoveFirst(); 
while (!rs.EOF)
{
    // When dumping a SQL-Nullable field in the table, need to test it for DBNull.Value.
    string code = (rs.Fields[&quot;CountryRegionCode&quot;].Value == DBNull.Value) ?
        &quot;(DBNull)&quot; : rs.Fields[&quot;CountryRegionCode&quot;].Value.ToString();


    string name = (rs.Fields[&quot;Name&quot;].Value == DBNull.Value) ?
        &quot;(DBNull)&quot; : rs.Fields[&quot;Name&quot;].Value.ToString();


    DateTime modifiedDate = (rs.Fields[&quot;ModifiedDate&quot;].Value == DBNull.Value) ?
        DateTime.MinValue : (DateTime)rs.Fields[&quot;ModifiedDate&quot;].Value;


    Console.WriteLine(&quot; {2} \t{0}\t{1}&quot;, code, name, modifiedDate.ToString(&quot;yyyy-MM-dd&quot;));


    // Move to the next record
    rs.MoveNext();   
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpLast" style=""><span style=""><span style="">4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Clean up objects before exit. </p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
////////////////////////////////////////////////////////////////////////////////
// Clean up objects before exit.
// 


Console.WriteLine(&quot;Closing the connections ...&quot;);


// Close the record set if it is open
if (rs != null && rs.State == (int)ADODB.ObjectStateEnum.adStateOpen)
    rs.Close();


// Close the connection to the database if it is open
if (conn != null && conn.State == (int)ADODB.ObjectStateEnum.adStateOpen)
    conn.Close();

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph"></p>
<h2>More Information<span style=""> </span></h2>
<p class="MsoNormal"><span style=""><a href="http://support.microsoft.com/kb/308611">HOW TO: Open ADO Connection and
<span class="SpellE">RecordSet</span> Objects in Visual C# .NET</a> </span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
