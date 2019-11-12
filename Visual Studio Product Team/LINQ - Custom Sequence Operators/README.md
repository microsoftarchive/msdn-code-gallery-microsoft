# LINQ - Custom Sequence Operators
## Requires
- Visual Studio 2010
## License
- Custom
## Technologies
- LINQ
## Topics
- Custom Sequence Operators
## Updated
- 08/12/2011
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
<td width="50px" align="left" valign="middle"><a href="http://archive.msdn.microsoft.com/vb2008samples/Release/ProjectReleases.aspx?ReleaseId=1426"><img title="101 Samples for Visual Basic 2005" src="-dd183105.download_45(en-us,msdn.10).jpg" alt="101 Samples for Visual Basic 2005" align="left"></a></td>
<td align="left" valign="middle"><span style="font-size:medium"><strong><a href="http://code.msdn.microsoft.com/101-LINQ-Samples-3fb9811b/viewsamplepack">Browse all 101 LINQ Samples</a>&nbsp;</strong></span></td>
</tr>
</tbody>
</table>
</div>
<hr>
</div>
<h1 id="Introduction">Introduction</h1>
<p>This sample shows different uses of Custom Sequence Operators:</p>
<ul>
<li><a title="This sample calculates the dot product of two integer vectors." href="#Combine">Combine</a>
</li></ul>
<h1><span>Building the Sample</span></h1>
<ol>
<li>Open the Program.cs </li><li>Comment or uncomment the desired samples </li><li>Press Ctrl &#43; F5 </li></ol>
<h1>Description</h1>
<h2 id="Combine">Combine</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample calculates the dot product of two integer vectors. It uses a user-created sequence operator, Combine, to calculate the dot product, passing it a lambda function to multiply two arrays, element by element, and sum the result.</p>
<p><span style="font-family:monospace"></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;static&nbsp;class&nbsp;CustomSequenceOperators&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;public&nbsp;static&nbsp;IEnumerable&nbsp;Combine(<span class="js__operator">this</span>&nbsp;IEnumerable&nbsp;first,&nbsp;IEnumerable&nbsp;second,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Func&nbsp;func)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;using&nbsp;(IEnumerator&nbsp;e1&nbsp;=&nbsp;first.GetEnumerator(),&nbsp;e2&nbsp;=&nbsp;second.GetEnumerator())&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">while</span>&nbsp;(e1.MoveNext()&nbsp;&amp;&amp;&nbsp;e2.MoveNext())&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;yield&nbsp;<span class="js__statement">return</span>&nbsp;func(e1.Current,&nbsp;e2.Current);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
<span class="js__brace">}</span>&nbsp;
&nbsp;
public&nbsp;<span class="js__operator">void</span>&nbsp;Linq98()&nbsp;<span class="js__brace">{</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;vectorA&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">0</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">4</span>,&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">6</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;vectorB&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">1</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">7</span>,&nbsp;<span class="js__num">8</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int&nbsp;dotProduct&nbsp;=&nbsp;vectorA.Combine(vectorB,&nbsp;(a,&nbsp;b)&nbsp;=&gt;&nbsp;a&nbsp;*&nbsp;b).Sum();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;Dot&nbsp;product:&nbsp;{0}&quot;</span>,&nbsp;dotProduct);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<h3><strong>Result</strong></h3>
<p style="padding-left:30px">Dot product: 109</p>
<p>&nbsp;</p>
<p><span style="font-size:20px; font-weight:bold">Source Code Files</span></p>
<ul>
<li><a class="browseFile" href="sourcecode?fileId=24036&pathId=1429239949">Program.cs</a>
</li></ul>
<h1><strong>101 LINQ Samples</strong></h1>
<ul>
<li><a href="../LINQ-Restriction-Operators-b15d29ca">Restriction Operators</a> </li><li><a href="../LINQ-to-DataSets-09787825">Projection Operators</a> </li><li><a href="../LINQ-Partitioning-Operators-c68aaccc">Partitioning Operators</a> </li><li><a href="../SQL-Ordering-Operators-050af19e">Ordering Operators</a> </li><li><a href="../LINQ-to-DataSets-Grouping-c62703ea">Grouping Operators</a> </li><li><a href="../LINQ-Set-Operators-374f34fe">Set Operators</a> </li><li><a href="../LINQ-Conversion-Operators-e4e59714">Conversion Operators</a> </li><li><a href="../LINQ-Element-Operators-0f3f12ce">Element Operators</a> </li><li><a href="../LINQ-Element-Operators-0f3f12ce">Generation Operators</a> </li><li><a href="../LINQ-Quantifiers-f00e7e3e">Quantifiers</a> </li><li><a href="../LINQ-Aggregate-Operators-c51b3869">Aggregate Operators</a> </li><li><a href="../LINQ-Miscellaneous-6b72bb2a">Miscellaneous Operators</a> </li><li><a href="../LINQ-to-DataSets-Custom-41738490">Custom Sequence Operators</a> </li><li><a href="../LINQ-Query-Execution-ce0d3b95">Query Execution</a> </li><li><a href="../LINQ-Join-Operators-dabef4e9">Join Operators</a> </li></ul>
<h1>More Information</h1>
<p>For more information, see:</p>
<ul>
<li>LINQ - <a href="http://msdn.microsoft.com/en-us/netframework/aa904594" target="_blank">
http://msdn.microsoft.com/en-us/netframework/aa904594</a> </li><li>LINQ to DataSet &ndash; Data Binding - <a href="http://blogs.msdn.com/b/erickt/archive/2007/08/24/linq-to-dataset-data-binding.aspx" target="_blank">
http://blogs.msdn.com/b/erickt/archive/2007/08/24/linq-to-dataset-data-binding.aspx</a>
</li></ul>
