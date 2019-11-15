# LINQ - Query Execution
## Requires
- Visual Studio 2010
## License
- Custom
## Technologies
- LINQ
## Topics
- Query Execution
## Updated
- 08/11/2011
## Description

<table border="0" cellspacing="2" cellpadding="1" width="100%">
<tbody>
<tr align="left" valign="top">
<td align="left" valign="middle" style="background-color:#c0c0c0"><span style="color:#ffffff; font-size:x-large">&nbsp;Part of the 101 LINQ SAMPLES</span></td>
</tr>
</tbody>
</table>
<div class="RoundedBox">
<div class="boxheader">
<div class="BostonPostCard"></div>
</div>
<div class="boxheader"><span style="font-size:medium; background-color:#ffffff; color:#333333">Learn how to use LINQ in your applications with these code samples, covering the entire range of LINQ functionality and demonstrating LINQ with SQL, DataSets, and
 XML.</span></div>
<div class="boxcontent">
<table border="0" cellspacing="2" cellpadding="1" width="100%">
<tbody>
<tr align="left" valign="top">
<td width="50px" align="left" valign="middle"><a href="http://archive.msdn.microsoft.com/vb2008samples/Release/ProjectReleases.aspx?ReleaseId=1426"><img title="101 Samples for Visual Basic 2005" src="http://i.msdn.microsoft.com/dd183105.download_45(en-us,MSDN.10).jpg" alt="101 Samples for Visual Basic 2005" align="left"></a></td>
<td align="left" valign="middle"><span style="font-size:medium"><strong><a href="http://code.msdn.microsoft.com/101-LINQ-Samples-3fb9811b/viewsamplepack">Browse all 101 LINQ Samples</a>&nbsp;</strong></span></td>
</tr>
</tbody>
</table>
</div>
<hr>
</div>
<table class="multicol">
<tbody>
<tr>
<td class="innercol" valign="top">
<div class="maincolumn">
<div class="BostonPostCard"></div>
<h1 id="Introduction">Introduction</h1>
<p>This sample shows different uses of Query Execution:</p>
<ul class="bulletedlist">
<li><a title="The following sample shows how query execution is deferred until the query is enumerated at a foreach statement." href="#DeferredExecution">Deferred Execution</a>
</li><li><a title="The following sample shows how queries can be executed immediately with operators such as ToList()." href="#ImmediateExecution">Immediate Execution</a>
</li><li><a title="The following sample shows how, because of deferred execution, queries can be used again after data changes and will then operate on the new data." href="#QueryReuse">Query Reuse</a>
</li></ul>
<h1><span>Building the Sample</span></h1>
<ol>
<li>Open the Program.cs </li><li>Comment or uncomment the desired samples </li><li>Press Ctrl &#43; F5 </li></ol>
<h1>Description</h1>
<div class="fullwidth">
<div class="BostonPostCard">
<h2 id="DeferredExecution">Deferred Execution</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>The following sample shows how query execution is deferred until the query is enumerated at a foreach statement.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq99()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;Sequence&nbsp;operators&nbsp;form&nbsp;first-class&nbsp;queries&nbsp;that</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;are&nbsp;not&nbsp;executed&nbsp;until&nbsp;you&nbsp;enumerate&nbsp;over&nbsp;them.</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;numbers&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;int[]&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">4</span>,&nbsp;<span class="js__num">1</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">9</span>,&nbsp;<span class="js__num">8</span>,&nbsp;<span class="js__num">6</span>,&nbsp;<span class="js__num">7</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">0</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int&nbsp;i&nbsp;=&nbsp;<span class="js__num">0</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;q&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;n&nbsp;<span class="js__operator">in</span>&nbsp;numbers&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;&#43;&#43;i;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;Note,&nbsp;the&nbsp;local&nbsp;variable&nbsp;'i'&nbsp;is&nbsp;not&nbsp;incremented</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;until&nbsp;each&nbsp;element&nbsp;is&nbsp;evaluated&nbsp;(as&nbsp;a&nbsp;side-effect):</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;v&nbsp;<span class="js__operator">in</span>&nbsp;q)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;v&nbsp;=&nbsp;{0},&nbsp;i&nbsp;=&nbsp;{1}&quot;</span>,&nbsp;v,&nbsp;i);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
</div>
<div class="BostonPostCard">
<p style="padding-left:30px">v = 1, i = 1<br>
v = 2, i = 2<br>
v = 3, i = 3<br>
v = 4, i = 4<br>
v = 5, i = 5<br>
v = 6, i = 6<br>
v = 7, i = 7<br>
v = 8, i = 8<br>
v = 9, i = 9<br>
v = 10, i = 10</p>
<p style="padding-left:30px">&nbsp;</p>
<h2>Immediate Execution</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>The following sample shows how queries can be executed immediately with operators such as ToList().</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq100()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;Methods&nbsp;like&nbsp;ToList()&nbsp;cause&nbsp;the&nbsp;query&nbsp;to&nbsp;be</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;executed&nbsp;immediately,&nbsp;caching&nbsp;the&nbsp;results.</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;numbers&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;int[]&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">4</span>,&nbsp;<span class="js__num">1</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">9</span>,&nbsp;<span class="js__num">8</span>,&nbsp;<span class="js__num">6</span>,&nbsp;<span class="js__num">7</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">0</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int&nbsp;i&nbsp;=&nbsp;<span class="js__num">0</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;q&nbsp;=&nbsp;(&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;n&nbsp;<span class="js__operator">in</span>&nbsp;numbers&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;&#43;&#43;i)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.ToList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;The&nbsp;local&nbsp;variable&nbsp;i&nbsp;has&nbsp;already&nbsp;been&nbsp;fully</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;incremented&nbsp;before&nbsp;we&nbsp;iterate&nbsp;the&nbsp;results:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;v&nbsp;<span class="js__operator">in</span>&nbsp;q)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;v&nbsp;=&nbsp;{0},&nbsp;i&nbsp;=&nbsp;{1}&quot;</span>,&nbsp;v,&nbsp;i);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
<span class="js__brace">}</span>&nbsp;</pre>
</div>
</div>
</div>
</div>
<div class="BostonPostCard">
<h3><strong>Result</strong></h3>
<p style="padding-left:30px">v = 1, i = 10<br>
v = 2, i = 10<br>
v = 3, i = 10<br>
v = 4, i = 10<br>
v = 5, i = 10<br>
v = 6, i = 10<br>
v = 7, i = 10<br>
v = 8, i = 10<br>
v = 9, i = 10<br>
v = 10, i = 10</p>
<p>&nbsp;</p>
<h2 id="QueryReuse">Query Reuse</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>The following sample shows how, because of deferred execution, queries can be used again after data changes and will then operate on the new data.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq101()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;Deferred&nbsp;execution&nbsp;lets&nbsp;us&nbsp;define&nbsp;a&nbsp;query&nbsp;once</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;and&nbsp;then&nbsp;reuse&nbsp;it&nbsp;later&nbsp;after&nbsp;data&nbsp;changes.</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;numbers&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;int[]&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">4</span>,&nbsp;<span class="js__num">1</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">9</span>,&nbsp;<span class="js__num">8</span>,&nbsp;<span class="js__num">6</span>,&nbsp;<span class="js__num">7</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">0</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;lowNumbers&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;n&nbsp;<span class="js__operator">in</span>&nbsp;numbers&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;where&nbsp;n&nbsp;&lt;=&nbsp;<span class="js__num">3</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;n;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;First&nbsp;run&nbsp;numbers&nbsp;&lt;=&nbsp;3:&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(int&nbsp;n&nbsp;<span class="js__operator">in</span>&nbsp;lowNumbers)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(n);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">for</span>&nbsp;(int&nbsp;i&nbsp;=&nbsp;<span class="js__num">0</span>;&nbsp;i&nbsp;&lt;&nbsp;<span class="js__num">10</span>;&nbsp;i&#43;&#43;)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;numbers[i]&nbsp;=&nbsp;-numbers[i];&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;During&nbsp;this&nbsp;second&nbsp;run,&nbsp;the&nbsp;same&nbsp;query&nbsp;object,</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;lowNumbers,&nbsp;will&nbsp;be&nbsp;iterating&nbsp;over&nbsp;the&nbsp;new&nbsp;state</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;of&nbsp;numbers[],&nbsp;producing&nbsp;different&nbsp;results:</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;Second&nbsp;run&nbsp;numbers&nbsp;&lt;=&nbsp;3:&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(int&nbsp;n&nbsp;<span class="js__operator">in</span>&nbsp;lowNumbers)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(n);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
<span class="js__brace">}</span>&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
</div>
<p style="padding-left:30px">First run numbers &lt;= 3:<br>
1<br>
3<br>
2<br>
0<br>
Second run numbers &lt;= 3:<br>
-5<br>
-4<br>
-1<br>
-3<br>
-9<br>
-8<br>
-6<br>
-7<br>
-2<br>
0</p>
<div></div>
</div>
<p>&nbsp;</p>
<p><span style="font-size:20px; font-weight:bold">Source Code Files</span></p>
<ul>
<li><a class="browseFile" href="sourcecode?fileId=23934&pathId=1281268086">Program.cs</a>
</li></ul>
<h1><strong>101 LINQ Samples</strong></h1>
<ul>
<li><a href="../LINQ-Restriction-Operators-b15d29ca">Restriction Operators</a> </li><li><a href="../LINQ-to-DataSets-09787825">Projection Operators</a> </li><li><a href="../LINQ-Partitioning-Operators-c68aaccc">Partitioning Operators</a> </li><li><a href="../SQL-Ordering-Operators-050af19e">Ordering Operators</a> </li><li><a href="../LINQ-to-DataSets-Grouping-c62703ea">Grouping Operators</a> </li><li><a href="../LINQ-Set-Operators-374f34fe">Set Operators</a> </li><li><a href="../LINQ-Conversion-Operators-e4e59714">Conversion Operators</a> </li><li><a href="../LINQ-Element-Operators-0f3f12ce">Element Operators</a> </li><li><a href="../LINQ-Element-Operators-0f3f12ce">Generation Operators</a> </li><li><a href="../LINQ-Quantifiers-f00e7e3e">Quantifiers</a> </li><li><a href="../LINQ-Aggregate-Operators-c51b3869">Aggregate Operators</a> </li><li><a href="../LINQ-Miscellaneous-6b72bb2a">Miscellaneous Operators</a> </li><li><a href="../LINQ-to-DataSets-Custom-41738490">Custom Sequence Operators</a> </li><li><a href="../LINQ-Query-Execution-ce0d3b95">Query Execution</a> </li><li><a href="../LINQ-Join-Operators-dabef4e9">Join Operators</a> </li></ul>
<h1>More Information</h1>
<p>For more information, see:</p>
<ul>
<li>Query Execution - <a href="http://msdn.microsoft.com/en-us/library/bb738633.aspx" target="_blank">
http://msdn.microsoft.com/en-us/library/bb738633.aspx</a> </li></ul>
</div>
</td>
</tr>
</tbody>
</table>
