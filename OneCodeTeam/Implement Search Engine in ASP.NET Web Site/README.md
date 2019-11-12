# Implement Search Engine in ASP.NET Web Site
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- Search
## Updated
- 02/05/2017
## Description

<p>==============================================================================<br>
ASP.NET APPLICATION : CSASPNETSearchEngine Project Overview<br>
==============================================================================<br>
<br>
//////////////////////////////////////////////////////////////////////////////<br>
Summary:<br>
<br>
This sample shows how to implement a simple search engine in an ASP.NET web site.<br>
<br>
//////////////////////////////////////////////////////////////////////////////<br>
Demo the Sample:<br>
<br>
Open Default.aspx page, input one or more keywords into the text box. <br>
Click the submit button.<br>
<br>
//////////////////////////////////////////////////////////////////////////////<br>
Code Logical:<br>
<br>
1. Create the database.<br>
&nbsp; a. Create a SQL Server database named &quot;MyDatabase.mdf&quot; within App_Data folder.<br>
&nbsp; b. Create a Table named &quot;Articles&quot; in the database.<br>
<br>
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>SQL</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">mysql</span>

<div class="preview">
<pre class="mysql">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="sql__id">ID</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="sql__keyword">bigint</span>&nbsp;(<span class="sql__keyword">Primary</span>&nbsp;<span class="sql__keyword">Key</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="sql__id">Title</span>&nbsp;&nbsp;&nbsp;&nbsp;<span class="sql__keyword">nvarchar</span>(<span class="sql__number">50</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="sql__id">Content</span>&nbsp;&nbsp;<span class="sql__keyword">varchar</span>(<span class="sql__id">MAX</span>)</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp; <br>
&nbsp; c. Input some sample records to the Table.<br>
<br>
2. Data Access.<br>
&nbsp; a. Create a class named &quot;Article&quot; represents a record.<br>
&nbsp; b. Create a class named &quot;DataAccess&quot; to access database. This class contains
<br>
&nbsp; &nbsp; &nbsp;public methods GetArticle(), GetAll() and Search(). Within Search() method,<br>
&nbsp; &nbsp; &nbsp;the key code is generating a complex Sql command which is used to search database.<br>
<br>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Generate&nbsp;a&nbsp;complex&nbsp;Sql&nbsp;command.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;StringBuilder&nbsp;builder&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;StringBuilder();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;builder.Append(<span class="cs__string">&quot;select&nbsp;*&nbsp;from&nbsp;[Articles]&nbsp;where&nbsp;&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">foreach</span>&nbsp;(<span class="cs__keyword">string</span>&nbsp;item&nbsp;<span class="cs__keyword">in</span>&nbsp;keywords)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;builder.AppendFormat(<span class="cs__string">&quot;([Title]&nbsp;like&nbsp;'%{0}%'&nbsp;or&nbsp;[Content]&nbsp;like&nbsp;'%{0}%')&nbsp;and&nbsp;&quot;</span>,&nbsp;item);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Remove&nbsp;unnecessary&nbsp;string&nbsp;&quot;&nbsp;and&nbsp;&quot;&nbsp;at&nbsp;the&nbsp;end&nbsp;of&nbsp;the&nbsp;command.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;sql&nbsp;=&nbsp;builder.ToString(<span class="cs__number">0</span>,&nbsp;builder.Length&nbsp;-&nbsp;<span class="cs__number">5</span>);</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<br>
3. Search Page.<br>
&nbsp; The key controls of this page is TextBox control named &quot;txtKeyWords&quot; which
<br>
&nbsp; is used to input keywords, and Repeater control which is used to display<br>
&nbsp; result.<br>
&nbsp; And there is a JavaScript function which is used to hightlight keywords<br>
&nbsp; in the result.<br>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">js</span>

<div class="preview">
<pre class="js">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">for</span>&nbsp;(<span class="js__statement">var</span>&nbsp;i&nbsp;=&nbsp;<span class="js__num">0</span>;&nbsp;i&nbsp;&lt;&nbsp;keywords.length;&nbsp;i&#43;&#43;)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;a&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__object">RegExp</span>(keywords[i],&nbsp;<span class="js__string">&quot;igm&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;container.innerHTML&nbsp;=&nbsp;container.innerHTML.replace(a,&nbsp;<span class="js__string">&quot;$0&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<br>
<br>
4. The Detail Page.<br>
&nbsp; This page receives a parameter from Query String named &quot;id&quot;, and then call
<br>
&nbsp; DataAccess class to retrieve a individual record from database to show in the page.<br>
&nbsp; <br>
<br>
//////////////////////////////////////////////////////////////////////////////<br>
References:<br>
<br>
SQL Server and ADO.NET<br>
http://msdn.microsoft.com/en-us/library/kb9s9ks0.aspx<br>
<br>
Connecting to a Data Source (ADO.NET)<br>
http://msdn.microsoft.com/en-us/library/32c5dh3b.aspx<br>
<br>
LIKE (Transact-SQL)<br>
http://msdn.microsoft.com/en-us/library/ms179859.aspx<br>
<br>
Repeater Web Server Control Overview<br>
http://msdn.microsoft.com/en-us/library/x8f2zez5.aspx<br>
<br>
How to: Pass Values Between ASP.NET Web Pages<br>
http://msdn.microsoft.com/en-us/library/6c3yckfw.aspx</div>
<p>&nbsp;</p>
