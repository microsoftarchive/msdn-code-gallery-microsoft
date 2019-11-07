# LINQ - Element Operators
## Requires
- Visual Studio 2010
## License
- Custom
## Technologies
- LINQ
## Topics
- Element Operators
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
<td width="50px" align="left" valign="middle"><a href="http://archive.msdn.microsoft.com/vb2008samples/Release/ProjectReleases.aspx?ReleaseId=1426"><img title="101 LINQ Samples" src="-dd183105.download_45(en-us,msdn.10).jpg" alt="101 LINQ Samples" align="left"></a></td>
<td align="left" valign="middle"><span style="font-size:medium"><strong><a href="http://code.msdn.microsoft.com/101-LINQ-Samples-3fb9811b/viewsamplepack">Browse all 101 LINQ Samples</a>&nbsp;</strong></span></td>
</tr>
</tbody>
</table>
</div>
<hr>
</div>
<h1 id="Introduction">Introduction</h1>
<p>&nbsp;This sample shows different uses of Element Operators:</p>
<ul class="bulletedlist">
<li><a href="#FirstSimple">First - Simple</a> </li><li><a href="#FirstCondition">First - Condition</a> </li><li><a href="#FirstOrDefaultSimple">FirstOrDefault - Simple</a> </li><li><a href="#FirstOrDefaultCondition">FirstOrDefault - Condition</a> </li><li><a href="#ElementAt">ElementAt</a> </li></ul>
<h1><span>Building the Sample</span></h1>
<ol>
<li>Open the Program.cs </li><li>Comment or uncomment the desired samples </li><li>Press Ctrl &#43; F5 </li></ol>
<h1>Description</h1>
<h2 id="FirstSimple">First - Simple</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses First to return the first matching element as a Product, instead of as a sequence containing a Product.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq58()
{
    List&lt;Product&gt; products = GetProductList();

    Product product12 = (
        from p in products
        where p.ProductID == 12
        select p)
        .First();
 
    ObjectDumper.Write(product12);
}</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Linq58()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Product&gt;&nbsp;products&nbsp;=&nbsp;GetProductList();&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Product&nbsp;product12&nbsp;=&nbsp;(&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;p&nbsp;<span class="cs__keyword">in</span>&nbsp;products&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;where&nbsp;p.ProductID&nbsp;==&nbsp;<span class="cs__number">12</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;p)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.First();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ObjectDumper.Write(product12);&nbsp;
}</pre>
</div>
</div>
</div>
<h3><strong>Result</strong></h3>
<p style="padding-left:30px">ProductID=12 ProductName=Queso Manchego La Pastora Category=Dairy Products UnitPrice=38.0000 UnitsInStock=86</p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="FirstCondition">First - Condition</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses First to find the first element in the array that starts with 'o'.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq59()
{
    string[] strings = { &quot;zero&quot;, &quot;one&quot;, &quot;two&quot;, &quot;three&quot;, &quot;four&quot;, &quot;five&quot;, &quot;six&quot;, &quot;seven&quot;, &quot;eight&quot;, &quot;nine&quot; };
 
    string startsWithO = strings.First(s =&gt; s[0] == 'o');
 
    Console.WriteLine(&quot;A string starting with 'o': {0}&quot;, startsWithO);
}</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Linq59()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>[]&nbsp;strings&nbsp;=&nbsp;{&nbsp;<span class="cs__string">&quot;zero&quot;</span>,&nbsp;<span class="cs__string">&quot;one&quot;</span>,&nbsp;<span class="cs__string">&quot;two&quot;</span>,&nbsp;<span class="cs__string">&quot;three&quot;</span>,&nbsp;<span class="cs__string">&quot;four&quot;</span>,&nbsp;<span class="cs__string">&quot;five&quot;</span>,&nbsp;<span class="cs__string">&quot;six&quot;</span>,&nbsp;<span class="cs__string">&quot;seven&quot;</span>,&nbsp;<span class="cs__string">&quot;eight&quot;</span>,&nbsp;<span class="cs__string">&quot;nine&quot;</span>&nbsp;};&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;startsWithO&nbsp;=&nbsp;strings.First(s&nbsp;=&gt;&nbsp;s[<span class="cs__number">0</span>]&nbsp;==&nbsp;<span class="cs__string">'o'</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__string">&quot;A&nbsp;string&nbsp;starting&nbsp;with&nbsp;'o':&nbsp;{0}&quot;</span>,&nbsp;startsWithO);&nbsp;
}</pre>
</div>
</div>
</div>
<h3><strong>Result</strong></h3>
<p style="padding-left:30px">A string starting with 'o': one</p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="FirstOrDefaultSimple">FirstOrDefault - Simple</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses FirstOrDefault to try to return the first element of the sequence, unless there are no elements, in which case the default value for that type is returned.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq61()
 
{
    int[] numbers = { };
 
    int firstNumOrDefault = numbers.FirstOrDefault();
 
    Console.WriteLine(firstNumOrDefault);
}</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Linq61()&nbsp;
&nbsp;&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">int</span>[]&nbsp;numbers&nbsp;=&nbsp;{&nbsp;};&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">int</span>&nbsp;firstNumOrDefault&nbsp;=&nbsp;numbers.FirstOrDefault();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(firstNumOrDefault);&nbsp;
}</pre>
</div>
</div>
</div>
<h3><strong>Result</strong></h3>
<p style="padding-left:30px">0</p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="FirstOrDefaultCondition">FirstOrDefault - Condition</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses FirstOrDefault to return the first product whose ProductID is 789 as a single Product object, unless there is no match, in which case null is returned.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq62()
{
    List&lt;Product&gt; products = GetProductList();
 
    Product product789 = products.FirstOrDefault(p =&gt; p.ProductID == 789);

    Console.WriteLine(&quot;Product 789 exists: {0}&quot;, product789 != null);
}</pre>
<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq62()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Product&gt;&nbsp;products&nbsp;=&nbsp;GetProductList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Product&nbsp;product789&nbsp;=&nbsp;products.FirstOrDefault(p&nbsp;=&gt;&nbsp;p.ProductID&nbsp;==&nbsp;<span class="js__num">789</span>);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;Product&nbsp;789&nbsp;exists:&nbsp;{0}&quot;</span>,&nbsp;product789&nbsp;!=&nbsp;null);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<h3><strong>Result</strong></h3>
<p style="padding-left:30px">Product 789 exists: False</p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="ElementAt">ElementAt</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses ElementAt to retrieve the second number greater than 5 from an array.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq64()
{
    int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
 
    int fourthLowNum = (
        from n in numbers
        where n &gt; 5
        select n)
        .ElementAt(1);  // second number is index 1 because sequences use 0-based indexing

    Console.WriteLine(&quot;Second number &gt; 5: {0}&quot;, fourthLowNum);
}</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Linq64()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">int</span>[]&nbsp;numbers&nbsp;=&nbsp;{&nbsp;<span class="cs__number">5</span>,&nbsp;<span class="cs__number">4</span>,&nbsp;<span class="cs__number">1</span>,&nbsp;<span class="cs__number">3</span>,&nbsp;<span class="cs__number">9</span>,&nbsp;<span class="cs__number">8</span>,&nbsp;<span class="cs__number">6</span>,&nbsp;<span class="cs__number">7</span>,&nbsp;<span class="cs__number">2</span>,&nbsp;<span class="cs__number">0</span>&nbsp;};&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">int</span>&nbsp;fourthLowNum&nbsp;=&nbsp;(&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;n&nbsp;<span class="cs__keyword">in</span>&nbsp;numbers&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;where&nbsp;n&nbsp;&gt;&nbsp;<span class="cs__number">5</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;n)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.ElementAt(<span class="cs__number">1</span>);&nbsp;&nbsp;<span class="cs__com">//&nbsp;second&nbsp;number&nbsp;is&nbsp;index&nbsp;1&nbsp;because&nbsp;sequences&nbsp;use&nbsp;0-based&nbsp;indexing</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__string">&quot;Second&nbsp;number&nbsp;&gt;&nbsp;5:&nbsp;{0}&quot;</span>,&nbsp;fourthLowNum);&nbsp;
}</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<p style="padding-left:30px">Second number &gt; 5: 8</p>
<p>&nbsp;</p>
<p><span style="font-size:20px; font-weight:bold">Source Code Files</span></p>
<ul>
<li><a class="browseFile" href="sourcecode?fileId=23887&pathId=1332778713">Program.cs</a>
</li></ul>
<h1><strong>101 LINQ Samples</strong></h1>
<ul>
<li><a href="../LINQ-Restriction-Operators-b15d29ca">Restriction Operators</a> </li><li><a href="../LINQ-to-DataSets-09787825">Projection Operators</a> </li><li><a href="../LINQ-Partitioning-Operators-c68aaccc">Partitioning Operators</a> </li><li><a href="../SQL-Ordering-Operators-050af19e">Ordering Operators</a> </li><li><a href="../LINQ-to-DataSets-Grouping-c62703ea">Grouping Operators</a> </li><li><a href="../LINQ-Set-Operators-374f34fe">Set Operators</a> </li><li><a href="../LINQ-Conversion-Operators-e4e59714">Conversion Operators</a> </li><li><a href="../LINQ-Element-Operators-0f3f12ce">Element Operators</a> </li><li><a href="../LINQ-Element-Operators-0f3f12ce">Generation Operators</a> </li><li><a href="../LINQ-Quantifiers-f00e7e3e">Quantifiers</a> </li><li><a href="../LINQ-Aggregate-Operators-c51b3869">Aggregate Operators</a> </li><li><a href="../LINQ-Miscellaneous-6b72bb2a">Miscellaneous Operators</a> </li><li><a href="../LINQ-to-DataSets-Custom-41738490">Custom Sequence Operators</a> </li><li><a href="../LINQ-Query-Execution-ce0d3b95">Query Execution</a> </li><li><a href="../LINQ-Join-Operators-dabef4e9">Join Operators</a> </li></ul>
<h1>More Information</h1>
<p>For more information, see:</p>
<ul>
<li>Element Operators Query Expression Syntax Examples (LINQ to DataSet) - <a href="http://technet.microsoft.com/en-us/library/bb399382(VS.90).aspx" target="_blank">
http://technet.microsoft.com/en-us/library/bb399382(VS.90).aspx</a> </li></ul>
