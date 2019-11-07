# LINQ - Conversion Operators
## Requires
- Visual Studio 2010
## License
- Custom
## Technologies
- LINQ
## Topics
- Conversion Operators
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
<td width="50px" align="left" valign="middle"><a href="http://archive.msdn.microsoft.com/vb2008samples/Release/ProjectReleases.aspx?ReleaseId=1426"><img title="101 LINQ Samples" src="-dd183105.download_45(en-us,msdn.10).jpg" alt="101 LINQ Samples" align="left"></a></td>
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
<p>This sample shows different uses of Conversion Operators:</p>
<ul>
<li><a title="This sample uses ToArray to immediately evaluate a sequence into an array." href="#ToArray">ToArray</a>
</li><li><a title="This sample uses ToList to immediately evaluate a sequence into a List&lt;T&gt;." href="#ToList">ToList</a>
</li><li><a title="This sample uses ToDictionary to immediately evaluate a sequence and a related key expression into a dictionary." href="#ToDictionary">ToDictionary</a>
</li><li><a title="This sample uses OfType to return only the elements of the array that are of type double." href="#OfType">OfType</a>
</li></ul>
<h1><span>Building the Sample</span></h1>
<ol>
<li>Open the Program.cs </li><li>Comment or uncomment the desired samples </li><li>Press Ctrl &#43; F5 </li></ol>
<h1>Description</h1>
<h2 id="ToArray">ToArray</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses ToArray to immediately evaluate a sequence into an array.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq54()
{
    double[] doubles = { 1.7, 2.3, 1.9, 4.1, 2.9 };
 
    var sortedDoubles =
        from d in doubles
        orderby d descending
        select d;
    var doublesArray = sortedDoubles.ToArray();
 
    Console.WriteLine(&quot;Every other double from highest to lowest:&quot;);
    for (int d = 0; d &lt; doublesArray.Length; d &#43;= 2)
    {
        Console.WriteLine(doublesArray[d]);
    }
}</pre>
<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq54()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;double[]&nbsp;doubles&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">1.7</span>,&nbsp;<span class="js__num">2.3</span>,&nbsp;<span class="js__num">1.9</span>,&nbsp;<span class="js__num">4.1</span>,&nbsp;<span class="js__num">2.9</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;sortedDoubles&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;d&nbsp;<span class="js__operator">in</span>&nbsp;doubles&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;orderby&nbsp;d&nbsp;descending&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;d;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;doublesArray&nbsp;=&nbsp;sortedDoubles.ToArray();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;Every&nbsp;other&nbsp;double&nbsp;from&nbsp;highest&nbsp;to&nbsp;lowest:&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">for</span>&nbsp;(int&nbsp;d&nbsp;=&nbsp;<span class="js__num">0</span>;&nbsp;d&nbsp;&lt;&nbsp;doublesArray.Length;&nbsp;d&nbsp;&#43;=&nbsp;<span class="js__num">2</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(doublesArray[d]);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<p><strong>Result</strong></p>
<p style="padding-left:30px">Every other double from highest to lowest:<br>
4.1<br>
2.3<br>
1.7</p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="ToList">ToList</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses ToList to immediately evaluate a sequence into a List&lt;T&gt;.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq55()
{
    string[] words = { &quot;cherry&quot;, &quot;apple&quot;, &quot;blueberry&quot; };
 
    var sortedWords =
        from w in words
        orderby w
        select w;
    var wordList = sortedWords.ToList();
 
    Console.WriteLine(&quot;The sorted word list:&quot;);
    foreach (var w in wordList)
    {
        Console.WriteLine(w);
    }
}</pre>
<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq55()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;string[]&nbsp;words&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__string">&quot;cherry&quot;</span>,&nbsp;<span class="js__string">&quot;apple&quot;</span>,&nbsp;<span class="js__string">&quot;blueberry&quot;</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;sortedWords&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;w&nbsp;<span class="js__operator">in</span>&nbsp;words&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;orderby&nbsp;w&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;w;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;wordList&nbsp;=&nbsp;sortedWords.ToList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;The&nbsp;sorted&nbsp;word&nbsp;list:&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;w&nbsp;<span class="js__operator">in</span>&nbsp;wordList)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(w);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<p><strong>Result</strong></p>
<p style="padding-left:30px">The sorted word list:<br>
apple<br>
blueberry<br>
cherry</p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="ToDictionary">ToDictionary</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses ToDictionary to immediately evaluate a sequence and a related key expression into a dictionary.</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq56()
{
    var scoreRecords = new[] { new {Name = &quot;Alice&quot;, Score = 50},
                                new {Name = &quot;Bob&quot;  , Score = 40},
                                new {Name = &quot;Cathy&quot;, Score = 45}
                            };
 
    var scoreRecordsDict = scoreRecords.ToDictionary(sr =&gt; sr.Name);
 
    Console.WriteLine(&quot;Bob's score: {0}&quot;, scoreRecordsDict[&quot;Bob&quot;]);
}</pre>
<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq56()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;scoreRecords&nbsp;=&nbsp;<span class="js__operator">new</span>[]&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__brace">{</span>Name&nbsp;=&nbsp;<span class="js__string">&quot;Alice&quot;</span>,&nbsp;Score&nbsp;=&nbsp;<span class="js__num">50</span><span class="js__brace">}</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__brace">{</span>Name&nbsp;=&nbsp;<span class="js__string">&quot;Bob&quot;</span>&nbsp;&nbsp;,&nbsp;Score&nbsp;=&nbsp;<span class="js__num">40</span><span class="js__brace">}</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__brace">{</span>Name&nbsp;=&nbsp;<span class="js__string">&quot;Cathy&quot;</span>,&nbsp;Score&nbsp;=&nbsp;<span class="js__num">45</span><span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;scoreRecordsDict&nbsp;=&nbsp;scoreRecords.ToDictionary(sr&nbsp;=&gt;&nbsp;sr.Name);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;Bob's&nbsp;score:&nbsp;{0}&quot;</span>,&nbsp;scoreRecordsDict[<span class="js__string">&quot;Bob&quot;</span>]);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><strong>Result</strong></div>
<p style="padding-left:30px">Bob's score: { Name = Bob, Score = 40 }</p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="OfType">OfType</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses OfType to return only the elements of the array that are of type double.</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq57()
{
    object[] numbers = { null, 1.0, &quot;two&quot;, 3, &quot;four&quot;, 5, &quot;six&quot;, 7.0 };
 
    var doubles = numbers.OfType&lt;double&gt;();
 
    Console.WriteLine(&quot;Numbers stored as doubles:&quot;);
    foreach (var d in doubles)
    {
        Console.WriteLine(d);
    }
}</pre>
<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq57()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;object[]&nbsp;numbers&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;null,&nbsp;<span class="js__num">1.0</span>,&nbsp;<span class="js__string">&quot;two&quot;</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__string">&quot;four&quot;</span>,&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__string">&quot;six&quot;</span>,&nbsp;<span class="js__num">7.0</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;doubles&nbsp;=&nbsp;numbers.OfType&lt;double&gt;();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;Numbers&nbsp;stored&nbsp;as&nbsp;doubles:&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;d&nbsp;<span class="js__operator">in</span>&nbsp;doubles)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(d);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><strong>Result</strong></div>
<p style="padding-left:30px">Numbers stored as doubles:<br>
1<br>
7</p>
<p>&nbsp;</p>
<p><span style="font-size:20px; font-weight:bold">Source Code Files</span></p>
<ul>
<li><a class="browseFile" href="sourcecode?fileId=23883&pathId=1743770264">Program.cs</a>
</li></ul>
<h1><strong>101 LINQ Samples</strong></h1>
<ul>
<li><a href="../LINQ-Restriction-Operators-b15d29ca">Restriction Operators</a> </li><li><a href="../LINQ-to-DataSets-09787825">Projection Operators</a> </li><li><a href="../LINQ-Partitioning-Operators-c68aaccc">Partitioning Operators</a> </li><li><a href="../SQL-Ordering-Operators-050af19e">Ordering Operators</a> </li><li><a href="../LINQ-to-DataSets-Grouping-c62703ea">Grouping Operators</a> </li><li><a href="../LINQ-Set-Operators-374f34fe">Set Operators</a> </li><li><a href="../LINQ-Conversion-Operators-e4e59714">Conversion Operators</a> </li><li><a href="../LINQ-Element-Operators-0f3f12ce">Element Operators</a> </li><li><a href="../LINQ-Element-Operators-0f3f12ce">Generation Operators</a> </li><li><a href="../LINQ-Quantifiers-f00e7e3e">Quantifiers</a> </li><li><a href="../LINQ-Aggregate-Operators-c51b3869">Aggregate Operators</a> </li><li><a href="../LINQ-Miscellaneous-6b72bb2a">Miscellaneous Operators</a> </li><li><a href="../LINQ-to-DataSets-Custom-41738490">Custom Sequence Operators</a> </li><li><a href="../LINQ-Query-Execution-ce0d3b95">Query Execution</a> </li><li><a href="../LINQ-Join-Operators-dabef4e9">Join Operators</a> </li></ul>
<h1>More Information</h1>
<p>For more information, see:</p>
<ul>
<li>Conversion Operators (C# Programming Guide) - <a href="http://msdn.microsoft.com/en-us/library/09479473(VS.80).aspx" target="_blank">
http://msdn.microsoft.com/en-us/library/09479473(VS.80).aspx</a> </li><li>Conversion Operators - <a href="http://msdn.microsoft.com/en-us/library/ms229033.aspx" target="_blank">
http://msdn.microsoft.com/en-us/library/ms229033.aspx</a> </li></ul>
</div>
</td>
</tr>
</tbody>
</table>
