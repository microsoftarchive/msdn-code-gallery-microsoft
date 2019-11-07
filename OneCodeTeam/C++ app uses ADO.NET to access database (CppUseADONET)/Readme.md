# C++ app uses ADO.NET to access database (CppUseADONET)
## Requires
- Visual Studio 2008
## License
- MS-LPL
## Technologies
- ADO.NET
## Topics
- Interop
- Data Platform
## Updated
- 03/01/2012
## Description

<h1><span style="font-family:������">CONSOLE APPLICATION</span> (<span style="font-family:������">CppUseADONET</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The CppUseADONET example demonstrates the Microsoft ADO.NET technology to
</p>
<p class="MsoNormal">access databases using Visual C&#43;&#43; in both managed code and unmanaged code.
</p>
<p class="MsoNormal">It shows the basic structure of connecting to a data source, issuing SQL
</p>
<p class="MsoNormal">commands, using DataSet object and performing the cleanup.<span style="">&nbsp;
</span></p>
<h2>Running the Sample</h2>
<p class="MsoNormal"><span style=""><img src="53121-image.png" alt="" width="576" height="526" align="middle">
</span><span style=""></span></p>
<h2><span style="">Using the code </span></h2>
<p class="MsoNormal">1. Enable the support of CLR in Project Properties / Configuration
<span style="">&nbsp;</span>Properties / General / Set Common Language Runtime Support as &quot;Common
<span style="">&nbsp;&nbsp;&nbsp;</span>Language Runtime Support (/clr)&quot;. </p>
<p class="MsoNormal">2. Connect to data source.<span style="">&nbsp; </span>(System::Data::SqlClient::SqlConnection-&gt;Open)<span style="">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>
<pre class="hidden">
void CreateConnection()
{
    con = gcnew SqlConnection(conStr);
    con-&gt;Open();
}

</pre>
<pre id="codePreview" class="cplusplus">
void CreateConnection()
{
    con = gcnew SqlConnection(conStr);
    con-&gt;Open();
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">3. Build and Execute an ADO.NET Command. </p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp; </span>(System::Data::SqlCommand-&gt;ExecuteNonQuery) It can be a SQL statement (SELECT/UPDATE/INSERT/DELETE), or a stored procedure call.<span style="">&nbsp;&nbsp;
</span><span style=""></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>
<pre class="hidden">
void AddRow(wchar_t *lastName, wchar_t *firstName, SAFEARRAY *image)
    {
        // 1. Inialize the SqlCommand object.
        cmd = gcnew SqlCommand();


        // 2. Assign the connection to the SqlCommand.
        cmd-&gt;Connection = con;


        // 3. Set the SQL command text.
        // SQL statement or the name of the stored procedure.
        cmd-&gt;CommandText = &quot;INSERT INTO Person(LastName, FirstName, &quot; &#43; 
            &quot;HireDate, EnrollmentDate, Picture) VALUES (@LastName, &quot; &#43; 
            &quot;@FirstName, @HireDate, @EnrollmentDate, @Picture)&quot;;


        // 4. Set the command type.
        // CommandType::Text for ordinary SQL statements;
        // CommandType::StoredProcedure for stored procedures.
        cmd-&gt;CommandType = CommandType::Text;


        // 5. Append the parameters.
        // DBNull::Value for SQL-Nullable fields.
        cmd-&gt;Parameters-&gt;Add(&quot;@LastName&quot;, SqlDbType::NVarChar, 50)-&gt;Value = 
            Marshal::PtrToStringUni((IntPtr)lastName);
        cmd-&gt;Parameters-&gt;Add(&quot;@FirstName&quot;, SqlDbType::NVarChar, 50)-&gt;Value = 
            Marshal::PtrToStringUni((IntPtr)firstName);
        cmd-&gt;Parameters-&gt;Add(&quot;@HireDate&quot;, SqlDbType::DateTime)-&gt;Value = 
            DBNull::Value;
        cmd-&gt;Parameters-&gt;Add(&quot;@EnrollmentDate&quot;, SqlDbType::DateTime)-&gt;Value
            = DateTime::Now;
        if (image == NULL)
        {
            cmd-&gt;Parameters-&gt;Add(&quot;@Picture&quot;, SqlDbType::Image)-&gt;Value = 
                DBNull::Value;
        }
        else
        {
            // Convert the SAFEARRAY to an array of bytes.
            int len = image-&gt;rgsabound[0].cElements;
            array&lt;byte&gt; ^arr = gcnew array&lt;byte&gt;(len);
            int *pData;
            SafeArrayAccessData(image, (void **)&pData);
            Marshal::Copy(IntPtr(pData), arr, 0, len);
            SafeArrayUnaccessData(image);
            cmd-&gt;Parameters-&gt;Add(&quot;@Picture&quot;, SqlDbType::Image)-&gt;Value = arr;
        }


        // 6. Execute the command.
        cmd-&gt;ExecuteNonQuery();
    }

</pre>
<pre id="codePreview" class="cplusplus">
void AddRow(wchar_t *lastName, wchar_t *firstName, SAFEARRAY *image)
    {
        // 1. Inialize the SqlCommand object.
        cmd = gcnew SqlCommand();


        // 2. Assign the connection to the SqlCommand.
        cmd-&gt;Connection = con;


        // 3. Set the SQL command text.
        // SQL statement or the name of the stored procedure.
        cmd-&gt;CommandText = &quot;INSERT INTO Person(LastName, FirstName, &quot; &#43; 
            &quot;HireDate, EnrollmentDate, Picture) VALUES (@LastName, &quot; &#43; 
            &quot;@FirstName, @HireDate, @EnrollmentDate, @Picture)&quot;;


        // 4. Set the command type.
        // CommandType::Text for ordinary SQL statements;
        // CommandType::StoredProcedure for stored procedures.
        cmd-&gt;CommandType = CommandType::Text;


        // 5. Append the parameters.
        // DBNull::Value for SQL-Nullable fields.
        cmd-&gt;Parameters-&gt;Add(&quot;@LastName&quot;, SqlDbType::NVarChar, 50)-&gt;Value = 
            Marshal::PtrToStringUni((IntPtr)lastName);
        cmd-&gt;Parameters-&gt;Add(&quot;@FirstName&quot;, SqlDbType::NVarChar, 50)-&gt;Value = 
            Marshal::PtrToStringUni((IntPtr)firstName);
        cmd-&gt;Parameters-&gt;Add(&quot;@HireDate&quot;, SqlDbType::DateTime)-&gt;Value = 
            DBNull::Value;
        cmd-&gt;Parameters-&gt;Add(&quot;@EnrollmentDate&quot;, SqlDbType::DateTime)-&gt;Value
            = DateTime::Now;
        if (image == NULL)
        {
            cmd-&gt;Parameters-&gt;Add(&quot;@Picture&quot;, SqlDbType::Image)-&gt;Value = 
                DBNull::Value;
        }
        else
        {
            // Convert the SAFEARRAY to an array of bytes.
            int len = image-&gt;rgsabound[0].cElements;
            array&lt;byte&gt; ^arr = gcnew array&lt;byte&gt;(len);
            int *pData;
            SafeArrayAccessData(image, (void **)&pData);
            Marshal::Copy(IntPtr(pData), arr, 0, len);
            SafeArrayUnaccessData(image);
            cmd-&gt;Parameters-&gt;Add(&quot;@Picture&quot;, SqlDbType::Image)-&gt;Value = arr;
        }


        // 6. Execute the command.
        cmd-&gt;ExecuteNonQuery();
    }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">4. Use the DataSet Object.<span style="">&nbsp; </span>(System::Data::SqlClient::SqlDataAdapter-&gt;Fill)
</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp; </span>The DataSet, which is an in-memory cache of data retrieved from a data
<span style="">&nbsp;&nbsp;</span>source, is a major component of the ADO.NET architecture.<span style="">&nbsp;
</span>The DataSet consists of a collection of DataTable objects that you can relate to each
<span style="">&nbsp;</span>other with DataRelation objects.<span style=""> </span>
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>
<pre class="hidden">
void FillDataSet(wchar_t *command)
    {
        // 1. Inialize the DataSet object.
        ds = gcnew DataSet();


        // 2. Create a SELECT SQL command.
        String ^ strSelectCmd = Marshal::PtrToStringUni((IntPtr)command);
        
        // 3. Inialize the SqlDataAdapter object.
        // SqlDataAdapter represents a set of data commands and a 
        // database connection that are used to fill the DataSet and 
        // update a SQL Server database. 
        da = gcnew SqlDataAdapter(strSelectCmd, con);


        // 4. Fill the DataSet object.
        // Fill the DataTable in DataSet with the rows selected by the SQL 
        // command.
        da-&gt;Fill(ds);
    }

</pre>
<pre id="codePreview" class="cplusplus">
void FillDataSet(wchar_t *command)
    {
        // 1. Inialize the DataSet object.
        ds = gcnew DataSet();


        // 2. Create a SELECT SQL command.
        String ^ strSelectCmd = Marshal::PtrToStringUni((IntPtr)command);
        
        // 3. Inialize the SqlDataAdapter object.
        // SqlDataAdapter represents a set of data commands and a 
        // database connection that are used to fill the DataSet and 
        // update a SQL Server database. 
        da = gcnew SqlDataAdapter(strSelectCmd, con);


        // 4. Fill the DataSet object.
        // Fill the DataTable in DataSet with the rows selected by the SQL 
        // command.
        da-&gt;Fill(ds);
    }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">5. Clean up objects before exit.<span style="">&nbsp; </span>
</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp; </span>(System::Data::SqlClient::SqlConnection-&gt;Close)<span style="">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>
<pre class="hidden">
void CloseConnection()
{
    con-&gt;Close();
}

</pre>
<pre id="codePreview" class="cplusplus">
void CloseConnection()
{
    con-&gt;Close();
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2>More Information </h2>
<p class="MsoListParagraphCxSpFirst" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/e80y5yhx.aspx">ADO.NET Introduction</a>
</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/9ctka9db.aspx">Data Access Using ADO.NET in C&#43;&#43;</a>
</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/ms235266.aspx">How to: Marshal a VARIANT for ADO.NET</a>
</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/ms235208.aspx">How to: Marshal Unicode Strings for ADO.NET</a>
</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/0adb9zxe.aspx">C/C&#43;&#43; Preprocessor Reference (managed, unmanaged)</a>
</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/k8d11d4s.aspx">/clr (Comman Language Runtime Compilation)</a>
</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/system.data.dataset.aspx">MSDN: DataSet</a>
</p>
<p class="MsoListParagraphCxSpLast" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqldataadapter.aspx">MSDN: SqlDataAdapter</a>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
