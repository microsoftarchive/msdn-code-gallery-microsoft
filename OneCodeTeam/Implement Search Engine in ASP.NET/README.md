# Implement Search Engine in ASP.NET
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- ASP.NET
## Topics
- GridView
- Highlighting
## Updated
- 01/22/2013
## Description

<h1>A simple search engine implemented in ASP.NET (CSASPNETSearchEngine)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">This sample shows how to implement a simple search engine in an ASP.NET web site.<span style="">
</span></p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Step 1: Open the CSASPNETSearchEngine.sln.<br>
Step 2: Expand the CSASPNETSearchEngine web application and press Ctrl &#43; F5 to show the Default.aspx.</p>
<p class="MsoNormal"><span style=""><img src="75066-image.png" alt="" width="521" height="246" align="middle">
</span><br>
Step 3: <span style="">Type one or more keywords into the text box.</span> <span style="">
Click the submit button.</span></p>
<p class="MsoNormal"><span style=""><img src="75067-image.png" alt="" width="519" height="538" align="middle">
</span><br>
Step 4: Validation finished.</p>
<h2>Using the Code</h2>
<p class="MsoNormal">Step 1: Create the database.<span style=""><br>
</span><span style="">&nbsp;&nbsp; </span>a. Create a SQL Server database named &quot;MyDatabase.mdf&quot; within App_Data folder.<span style=""><br>
</span><span style="">&nbsp;&nbsp; </span>b. Create a Table named &quot;Articles&quot; in the database. The definition of the table as shown below:<span style="">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>SQL</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">mysql</span>

<pre id="codePreview" class="mysql">
ID&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; bigint (Primary Key)
Title&nbsp;&nbsp;&nbsp; nvarchar(50)
Content&nbsp; varchar(MAX)

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style="">&nbsp;&nbsp; </span>c. Input some sample records to the Table.<span style=""><br>
</span>Step 2: Data Access.<span style=""><br>
</span><span style="">&nbsp;&nbsp; </span>a. Create a class named &quot;Article&quot; represents a record.<span style=""><br>
</span><span style="">&nbsp;&nbsp; </span>b. Create a class named &quot;DataAccess&quot; to access database. This class contains public methods GetArticle(), GetAll() and Search(). Within Search() method,<span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>the key code is generating a complex Sql command which is used to search database.<span style="">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
// Generate a complex Sql command.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; StringBuilder sqlBuilder = new StringBuilder();
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; sqlBuilder.Append(&quot;select * from [Articles] where &quot;);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; foreach (string item in keywords)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; sqlBuilder.AppendFormat(&quot;([Title] like '%{0}%' or [Content] like '%{0}%') and &quot;, item);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;// Remove unnecessary string &quot; and &quot; at the end of the command.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; string sql = sqlBuilder.ToString(0, sqlBuilder.Length - 5);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step 3: Search Page.<span style=""><br>
</span><span style="">&nbsp;&nbsp; </span>The key controls of this page is the TextBox control which named &quot;txtKeyWords&quot;. It&#39;s used to input keywords. And the Repeater control is used to display
<span style="">&nbsp;</span>result.<span style="">&nbsp; While the following</span> JavaScript function is used to hightlight keywords<span style="">&nbsp;
</span>in the result.<span style=""> </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">js</span>

<pre id="codePreview" class="js">
for (var i = 0; i &lt; keywords.length; i&#43;&#43;)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; var a = new RegExp(keywords[i], &quot;igm&quot;);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; container.innerHTML = container.innerHTML.replace(a, &quot;<span style="background:#FF0">&quot; &#43; keywords[i] &#43; &quot;</span>&quot;);
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step 4: Detail Page.<span style=""><br>
</span><span style="">&nbsp;&nbsp; </span>This page receives a parameter from Query String named &quot;id&quot;, and then calls DataAccess class to retrieve an individual record from database to show in the page.<br>
Step <span style="">5</span>: Build the application and you can debug it.</p>
<h2>More Information</h2>
<p class="MsoNormal">SQL Server and ADO.NET<span style=""><br>
</span><a href="http://msdn.microsoft.com/en-us/library/kb9s9ks0.aspx">http://msdn.microsoft.com/en-us/library/kb9s9ks0.aspx</a><span style=""><br>
</span>Connecting to a Data Source (ADO.NET)<span style=""><br>
</span><a href="http://msdn.microsoft.com/en-us/library/32c5dh3b.aspx">http://msdn.microsoft.com/en-us/library/32c5dh3b.aspx</a><span style=""><br>
</span>LIKE (Transact-SQL)<span style=""><br>
</span><a href="http://msdn.microsoft.com/en-us/library/ms179859.aspx">http://msdn.microsoft.com/en-us/library/ms179859.aspx</a><span style=""><br>
</span>Repeater Web Server Control Overview<span style=""><br>
</span><a href="http://msdn.microsoft.com/en-us/library/x8f2zez5.aspx">http://msdn.microsoft.com/en-us/library/x8f2zez5.aspx</a><span style=""><br>
</span>How to: Pass Values Between ASP.NET Web Pages<span style=""><br>
</span><a href="http://msdn.microsoft.com/en-us/library/6c3yckfw.aspx">http://msdn.microsoft.com/en-us/library/6c3yckfw.aspx</a><span style="">
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
