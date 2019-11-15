# LINQ - Aggregate Operators
## Requires
- Visual Studio 2010
## License
- Custom
## Technologies
- LINQ
## Topics
- Aggregate Operators
## Updated
- 08/24/2011
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
<h1 id="Introduction">Introduction</h1>
<p>This sample shows different uses of Aggregate Operators:</p>
<ul>
<li><a title="This sample uses Count to get the number of unique factors of 300." href="#CountSimple">Count - Simple</a>
</li><li><a title="This sample uses Count to get the number of odd ints in the array." href="#CountConditional">Count - Conditional</a>
</li><li><a title="This sample uses Count to return a list of customers and how many orders each has." href="#CountNested">Count - Nested</a>
</li><li><a title="This sample uses Count to return a list of categories and how many products each has." href="#CountGrouped">Count - Grouped</a>
</li><li><a title="This sample uses Sum to get the total of the numbers in an array." href="#SumSimple">Sum - Simple</a>
</li><li><a title="This sample uses Sum to get the total number of characters of all words in the array." href="#SumProjection">Sum - Projection</a>
</li><li><a title="This sample uses Sum to get the total units in stock for each product category." href="#SumGrouped">Sum - Grouped</a>
</li><li><a title="This sample uses Min to get the lowest number in an array." href="#MinSimple">Min - Simple</a>
</li><li><a title="This sample uses Min to get the length of the shortest word in an array." href="#MinProjection">Min - Projection</a>
</li><li><a title="This sample uses Min to get the cheapest price among each category's products." href="#MinGrouped">Min - Grouped</a>
</li><li><a title="This sample uses Min to get the products with the cheapest price in each category." href="#MinElements">Min - Elements</a>
</li><li><a title="This sample uses Max to get the highest number in an array." href="#MaxSimple">Max - Simple</a>
</li><li><a title="This sample uses Max to get the length of the longest word in an array." href="#MaxProjection">Max - Projection</a>
</li><li><a title="This sample uses Max to get the most expensive price among each category's products." href="#MaxGrouped">Max - Grouped</a>
</li><li><a title="This sample uses Max to get the products with the most expensive price in each category." href="#MaxElements">Max - Elements</a>
</li><li><a title="This sample uses Average to get the average of all numbers in an array." href="#AverageSimple">Average - Simple</a>
</li><li><a title="This sample uses Average to get the average length of the words in the array." href="#AverageProjection">Average - Projection</a>
</li><li><a title="This sample uses Average to get the average price of each category's products." href="#AverageGrouped">Average - Grouped</a>
</li><li><a title="This sample uses Aggregate to create a running product on the array that calculates the total product of all elements." href="#AggregateSimple">Aggregate - Simple</a>
</li><li><a title="This sample uses Aggregate to create a running account balance that subtracts each withdrawal from the initial balance of 100, as long as the balance never drops below 0." href="#AggregateSeed">Aggregate - Seed</a>
</li></ul>
<h1><span>Building the Sample</span></h1>
<ol>
<li>Open the Program.cs </li><li>Comment or uncomment the desired samples </li><li>Press Ctrl &#43; F5 </li></ol>
<h1>Description</h1>
<div class="BostonPostCard">
<h2 id="CountSimple">Count - Simple <span style="font-size:x-small">&nbsp;</span></h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Count to get the number of unique factors of 300.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq73()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;factorsOf300&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">5</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int&nbsp;uniqueFactors&nbsp;=&nbsp;factorsOf300.Distinct().Count();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;There&nbsp;are&nbsp;{0}&nbsp;unique&nbsp;factors&nbsp;of&nbsp;300.&quot;</span>,&nbsp;uniqueFactors);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<p style="padding-left:30px"><code>There are 3 unique factors of 300.</code></p>
<h2 id="CountConditonal">Count - Conditional</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Count to get the number of odd ints in the array.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq74()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;numbers&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">4</span>,&nbsp;<span class="js__num">1</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">9</span>,&nbsp;<span class="js__num">8</span>,&nbsp;<span class="js__num">6</span>,&nbsp;<span class="js__num">7</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">0</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int&nbsp;oddNumbers&nbsp;=&nbsp;numbers.Count(n&nbsp;=&gt;&nbsp;n&nbsp;%&nbsp;<span class="js__num">2</span>&nbsp;==&nbsp;<span class="js__num">1</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;There&nbsp;are&nbsp;{0}&nbsp;odd&nbsp;numbers&nbsp;in&nbsp;the&nbsp;list.&quot;</span>,&nbsp;oddNumbers);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<p><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></p>
<p style="padding-left:30px"><code>There are 5 odd numbers in the list.</code></p>
<h2 id="CountNested">Count - Nested</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Count to return a list of customers and how many orders each has.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq76()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Customer&gt;&nbsp;customers&nbsp;=&nbsp;GetCustomerList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;orderCounts&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;c&nbsp;<span class="js__operator">in</span>&nbsp;customers&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__brace">{</span>&nbsp;c.CustomerID,&nbsp;OrderCount&nbsp;=&nbsp;c.Orders.Count()&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ObjectDumper.Write(orderCounts);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<table border="0" cellspacing="2" cellpadding="1" width="2106" height="1230">
<tbody>
<tr align="left" valign="top">
<td width="18%" style="padding-left:30px">CustomerID=ALFKI <br>
CustomerID=ANATR <br>
CustomerID=ANTON <br>
CustomerID=AROUT <br>
CustomerID=BERGS <br>
CustomerID=BLAUS <br>
CustomerID=BLONP <br>
CustomerID=BOLID <br>
CustomerID=BONAP <br>
CustomerID=BOTTM <br>
CustomerID=BSBEV <br>
CustomerID=CACTU <br>
CustomerID=CENTC <br>
CustomerID=CHOPS <br>
CustomerID=COMMI <br>
CustomerID=CONSH <br>
CustomerID=DRACD <br>
CustomerID=DUMON <br>
CustomerID=EASTC <br>
CustomerID=ERNSH <br>
CustomerID=FAMIA <br>
CustomerID=FISSA <br>
CustomerID=FOLIG <br>
CustomerID=FOLKO <br>
CustomerID=FRANK <br>
CustomerID=FRANR <br>
CustomerID=FRANS <br>
CustomerID=FURIB <br>
CustomerID=GALED <br>
CustomerID=GODOS <br>
CustomerID=GOURL <br>
CustomerID=GREAL <br>
CustomerID=GROSR <br>
CustomerID=HANAR <br>
CustomerID=HILAA <br>
CustomerID=HUNGC <br>
CustomerID=HUNGO <br>
CustomerID=ISLAT <br>
CustomerID=KOENE <br>
CustomerID=LACOR <br>
CustomerID=LAMAI <br>
CustomerID=LAUGB <br>
CustomerID=LAZYK <br>
CustomerID=LEHMS <br>
CustomerID=LETSS <br>
CustomerID=LILAS <br>
CustomerID=LINOD <br>
CustomerID=LONEP <br>
CustomerID=MAGAA <br>
CustomerID=MAISD <br>
CustomerID=MEREP <br>
CustomerID=MORGK <br>
CustomerID=NORTS <br>
CustomerID=OCEAN <br>
CustomerID=OLDWO <br>
CustomerID=OTTIK <br>
CustomerID=PARIS <br>
CustomerID=PERIC <br>
CustomerID=PICCO <br>
CustomerID=PRINI <br>
CustomerID=QUEDE <br>
CustomerID=QUEEN <br>
CustomerID=QUICK <br>
CustomerID=RANCH <br>
CustomerID=RATTC <br>
CustomerID=REGGC <br>
CustomerID=RICAR <br>
CustomerID=RICSU <br>
CustomerID=ROMEY <br>
CustomerID=SANTG <br>
CustomerID=SAVEA <br>
CustomerID=SEVES <br>
CustomerID=SIMOB <br>
CustomerID=SPECD <br>
CustomerID=SPLIR <br>
CustomerID=SUPRD <br>
CustomerID=THEBI <br>
CustomerID=THECR <br>
CustomerID=TOMSP <br>
CustomerID=TORTU <br>
CustomerID=TRADH <br>
CustomerID=TRAIH <br>
CustomerID=VAFFE <br>
CustomerID=VICTE <br>
CustomerID=VINET <br>
CustomerID=WANDK <br>
CustomerID=WARTH <br>
CustomerID=WELLI <br>
CustomerID=WHITC <br>
CustomerID=WILMK <br>
CustomerID=WOLZA</td>
<td>OrderCount=6 <br>
OrderCount=4 <br>
OrderCount=7 <br>
OrderCount=13 <br>
OrderCount=18 <br>
OrderCount=7 <br>
OrderCount=11 <br>
OrderCount=3 <br>
OrderCount=17 <br>
OrderCount=14 <br>
OrderCount=10 <br>
OrderCount=6 <br>
OrderCount=1 <br>
OrderCount=8 <br>
OrderCount=5 <br>
OrderCount=3 <br>
OrderCount=6 <br>
OrderCount=4 <br>
OrderCount=8 <br>
OrderCount=30 <br>
OrderCount=7 <br>
OrderCount=0 <br>
OrderCount=5 <br>
OrderCount=19 <br>
OrderCount=15 <br>
OrderCount=3 <br>
OrderCount=6 <br>
OrderCount=8 <br>
OrderCount=5 <br>
OrderCount=10 <br>
OrderCount=9 <br>
OrderCount=11 <br>
OrderCount=2 <br>
OrderCount=14 <br>
OrderCount=18 <br>
OrderCount=5 <br>
OrderCount=19 <br>
OrderCount=10 <br>
OrderCount=14 <br>
OrderCount=4 <br>
OrderCount=14 <br>
OrderCount=3 <br>
OrderCount=2 <br>
OrderCount=15 <br>
OrderCount=4 <br>
OrderCount=14 <br>
OrderCount=12 <br>
OrderCount=8 <br>
OrderCount=10 <br>
OrderCount=7 <br>
OrderCount=13 <br>
OrderCount=5 <br>
OrderCount=3 <br>
OrderCount=5 <br>
OrderCount=10 <br>
OrderCount=9 <br>
OrderCount=0 <br>
OrderCount=6 <br>
OrderCount=10 <br>
OrderCount=6 <br>
OrderCount=9 <br>
OrderCount=13 <br>
OrderCount=28 <br>
OrderCount=5 <br>
OrderCount=18 <br>
OrderCount=12 <br>
OrderCount=11 <br>
OrderCount=10 <br>
OrderCount=5 <br>
OrderCount=6 <br>
OrderCount=31 <br>
OrderCount=9 <br>
OrderCount=7 <br>
OrderCount=4 <br>
OrderCount=9 <br>
OrderCount=12 <br>
OrderCount=4 <br>
OrderCount=3 <br>
OrderCount=5 <br>
OrderCount=10 <br>
OrderCount=7 <br>
OrderCount=3 <br>
OrderCount=11 <br>
OrderCount=10 <br>
OrderCount=4 <br>
OrderCount=10 <br>
OrderCount=15 <br>
OrderCount=9 <br>
OrderCount=14 <br>
OrderCount=8 <br>
OrderCount=7</td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<h2 id="CountGrouped">Count - Grouped</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Count to return a list of categories and how many products each has.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq77()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Product&gt;&nbsp;products&nbsp;=&nbsp;GetProductList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;categoryCounts&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;p&nbsp;<span class="js__operator">in</span>&nbsp;products&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;group&nbsp;p&nbsp;by&nbsp;p.Category&nbsp;into&nbsp;g&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__brace">{</span>&nbsp;Category&nbsp;=&nbsp;g.Key,&nbsp;ProductCount&nbsp;=&nbsp;g.Count()&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ObjectDumper.Write(categoryCounts&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<table border="0" cellspacing="2" cellpadding="1" width="100%" style="padding-left:30px">
<tbody>
<tr align="left" valign="top">
<td width="18%" style="padding-left:30px">Category=Beverages <br>
Category=Condiments <br>
Category=Produce <br>
Category=Meat/Poultry <br>
Category=Seafood <br>
Category=Dairy Products <br>
Category=Confections <br>
Category=Grains/Cereals</td>
<td>ProductCount=12 <br>
ProductCount=12 <br>
ProductCount=5 <br>
ProductCount=6 <br>
ProductCount=12 <br>
ProductCount=10 <br>
ProductCount=13 <br>
ProductCount=7</td>
</tr>
</tbody>
</table>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="SumSimple">Sum - Simple</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Sum to get the total of the numbers in an array.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq78()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;numbers&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">4</span>,&nbsp;<span class="js__num">1</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">9</span>,&nbsp;<span class="js__num">8</span>,&nbsp;<span class="js__num">6</span>,&nbsp;<span class="js__num">7</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">0</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;double&nbsp;numSum&nbsp;=&nbsp;numbers.Sum();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;The&nbsp;sum&nbsp;of&nbsp;the&nbsp;numbers&nbsp;is&nbsp;{0}.&quot;</span>,&nbsp;numSum);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<p style="padding-left:30px"><code>The sum of the numbers is 45.</code></p>
<h2 id="SumProjection">Sum - Projection</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Sum to get the total number of characters of all words in the array.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq79()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;string[]&nbsp;words&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__string">&quot;cherry&quot;</span>,&nbsp;<span class="js__string">&quot;apple&quot;</span>,&nbsp;<span class="js__string">&quot;blueberry&quot;</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;double&nbsp;totalChars&nbsp;=&nbsp;words.Sum(w&nbsp;=&gt;&nbsp;w.Length);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;There&nbsp;are&nbsp;a&nbsp;total&nbsp;of&nbsp;{0}&nbsp;characters&nbsp;in&nbsp;these&nbsp;words.&quot;</span>,&nbsp;totalChars);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<p style="padding-left:30px"><code>There are a total of 20 characters in these words.</code></p>
<h2 id="SumGrouped">Sum - Grouped</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Sum to get the total units in stock for each product category.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq80()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Product&gt;&nbsp;products&nbsp;=&nbsp;GetProductList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;categories&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;p&nbsp;<span class="js__operator">in</span>&nbsp;products&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;group&nbsp;p&nbsp;by&nbsp;p.Category&nbsp;into&nbsp;g&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__brace">{</span>&nbsp;Category&nbsp;=&nbsp;g.Key,&nbsp;TotalUnitsInStock&nbsp;=&nbsp;g.Sum(p&nbsp;=&gt;&nbsp;p.UnitsInStock)&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ObjectDumper.Write(categories);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<table border="0" cellspacing="2" cellpadding="1" width="100%">
<tbody>
<tr align="left" valign="top">
<td width="18%" style="padding-left:30px">Category=Beverages <br>
Category=Condiments <br>
Category=Produce <br>
Category=Meat/Poultry <br>
Category=Seafood <br>
Category=Dairy Products <br>
Category=Confections <br>
Category=Grains/Cereals</td>
<td style="padding-left:30px">TotalUnitsInStock=559 <br>
TotalUnitsInStock=507 <br>
TotalUnitsInStock=100 <br>
TotalUnitsInStock=165 <br>
TotalUnitsInStock=701 <br>
TotalUnitsInStock=393 <br>
TotalUnitsInStock=386 <br>
TotalUnitsInStock=308</td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<h2 id="MinSimple">Min - Simple</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Min to get the lowest number in an array.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq81()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;numbers&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">4</span>,&nbsp;<span class="js__num">1</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">9</span>,&nbsp;<span class="js__num">8</span>,&nbsp;<span class="js__num">6</span>,&nbsp;<span class="js__num">7</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">0</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int&nbsp;minNum&nbsp;=&nbsp;numbers.Min();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;The&nbsp;minimum&nbsp;number&nbsp;is&nbsp;{0}.&quot;</span>,&nbsp;minNum);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<p style="padding-left:30px"><code>The minimum number is 0.</code></p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="MinProjection">Min - Projection</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Min to get the length of the shortest word in an array.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq82()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;string[]&nbsp;words&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__string">&quot;cherry&quot;</span>,&nbsp;<span class="js__string">&quot;apple&quot;</span>,&nbsp;<span class="js__string">&quot;blueberry&quot;</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int&nbsp;shortestWord&nbsp;=&nbsp;words.Min(w&nbsp;=&gt;&nbsp;w.Length);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;The&nbsp;shortest&nbsp;word&nbsp;is&nbsp;{0}&nbsp;characters&nbsp;long.&quot;</span>,&nbsp;shortestWord);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<p style="padding-left:30px"><code>The shortest word is 5 characters long.</code></p>
<h2 id="MinGrouped">Min - Grouped</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Min to get the cheapest price among each category's products.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq83()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Product&gt;&nbsp;products&nbsp;=&nbsp;GetProductList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;categories&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;p&nbsp;<span class="js__operator">in</span>&nbsp;products&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;group&nbsp;p&nbsp;by&nbsp;p.Category&nbsp;into&nbsp;g&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__brace">{</span>&nbsp;Category&nbsp;=&nbsp;g.Key,&nbsp;CheapestPrice&nbsp;=&nbsp;g.Min(p&nbsp;=&gt;&nbsp;p.UnitPrice)&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ObjectDumper.Write(categories);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<table border="0" cellspacing="2" cellpadding="1" width="2107" height="113">
<tbody>
<tr align="left" valign="top">
<td width="18%" style="padding-left:30px">Category=Beverages<br>
Category=Condiments<br>
Category=Produce<br>
Category=Meat/Poultry<br>
Category=Seafood<br>
Category=Dairy Products<br>
Category=Confections<br>
Category=Grains/Cereals</td>
<td style="padding-left:30px">CheapestPrice=4.5000<br>
CheapestPrice=10.0000<br>
CheapestPrice=10.0000<br>
CheapestPrice=7.4500<br>
CheapestPrice=6.0000<br>
CheapestPrice=2.5000<br>
CheapestPrice=9.2000<br>
CheapestPrice=7.0000</td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<h2 id="MinElements">Min - Elements</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Min to get the products with the cheapest price in each category.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq84()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Product&gt;&nbsp;products&nbsp;=&nbsp;GetProductList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;categories&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;p&nbsp;<span class="js__operator">in</span>&nbsp;products&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;group&nbsp;p&nbsp;by&nbsp;p.Category&nbsp;into&nbsp;g&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;let&nbsp;minPrice&nbsp;=&nbsp;g.Min(p&nbsp;=&gt;&nbsp;p.UnitPrice)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__brace">{</span>&nbsp;Category&nbsp;=&nbsp;g.Key,&nbsp;CheapestProducts&nbsp;=&nbsp;g.Where(p&nbsp;=&gt;&nbsp;p.UnitPrice&nbsp;==&nbsp;minPrice)&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ObjectDumper.Write(categories,&nbsp;<span class="js__num">1</span>);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<p style="padding-left:30px">Category=Beverages&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;CheapestProducts=...
<br>
&nbsp;&nbsp;CheapestProducts: ProductID=24&nbsp;&nbsp;ProductName=Guaran&Atilde;&iexcl; Fant&Atilde;&iexcl;stica&nbsp;&nbsp;Category=Beverages&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitPrice=4.5000&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitsInStock=20
<br>
Category=Condiments&nbsp;&nbsp;&nbsp;&nbsp;CheapestProducts=... <br>
&nbsp;&nbsp;CheapestProducts: ProductID=3&nbsp;&nbsp;ProductName=Aniseed Syrup&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Category=Condiments&nbsp;&nbsp;&nbsp;&nbsp;UnitPrice=10.0000&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitsInStock=13
<br>
Category=Produce&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;CheapestProducts=...
<br>
&nbsp;&nbsp;CheapestProducts: ProductID=74&nbsp;&nbsp;ProductName=Longlife Tofu&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Category=Produce&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitPrice=10.0000&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitsInStock=4
<br>
Category=Meat/Poultry&nbsp;&nbsp;CheapestProducts=... <br>
&nbsp;&nbsp;CheapestProducts: ProductID=54&nbsp;&nbsp;ProductName=Tourti&Atilde;&uml;re&nbsp;&nbsp;Category=Meat/Poultry&nbsp;&nbsp;UnitPrice=7.4500&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitsInStock=21
<br>
Category=Seafood&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;CheapestProducts=...
<br>
&nbsp;&nbsp;CheapestProducts: ProductID=13&nbsp;&nbsp;ProductName=Konbu&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Category=Seafood&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitPrice=6.0000&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitsInStock=24
<br>
Category=Dairy Products&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;CheapestProducts=...
<br>
&nbsp;&nbsp;CheapestProducts: ProductID=33&nbsp;&nbsp;ProductName=Geitost&nbsp;&nbsp;&nbsp;&nbsp;Category=Dairy Products&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitPrice=2.5000&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitsInStock=112
<br>
Category=Confections&nbsp;&nbsp;&nbsp;&nbsp;CheapestProducts=... <br>
&nbsp;&nbsp;CheapestProducts: ProductID=19&nbsp;&nbsp;ProductName=Teatime Chocolate Biscuits&nbsp;&nbsp;Category=Confections&nbsp;&nbsp;&nbsp;&nbsp;UnitPrice=9.2000&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitsInStock=25
<br>
Category=Grains/Cereals&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;CheapestProducts=...
<br>
&nbsp;&nbsp;CheapestProducts: ProductID=52&nbsp;&nbsp;ProductName=Filo Mix&nbsp;&nbsp;&nbsp;&nbsp;Category=Grains/Cereals&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitPrice=7.0000&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitsInStock=38</p>
<h2 id="MaxSimple">Max - Simple</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Max to get the highest number in an array.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq85()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;numbers&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">4</span>,&nbsp;<span class="js__num">1</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">9</span>,&nbsp;<span class="js__num">8</span>,&nbsp;<span class="js__num">6</span>,&nbsp;<span class="js__num">7</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">0</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int&nbsp;maxNum&nbsp;=&nbsp;numbers.Max();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;The&nbsp;maximum&nbsp;number&nbsp;is&nbsp;{0}.&quot;</span>,&nbsp;maxNum);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<p style="padding-left:30px"><code>The maximum number is 9.</code></p>
<h2 id="MaxProjection">Max - Projection</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Max to get the length of the longest word in an array.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq86()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;string[]&nbsp;words&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__string">&quot;cherry&quot;</span>,&nbsp;<span class="js__string">&quot;apple&quot;</span>,&nbsp;<span class="js__string">&quot;blueberry&quot;</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int&nbsp;longestLength&nbsp;=&nbsp;words.Max(w&nbsp;=&gt;&nbsp;w.Length);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;The&nbsp;longest&nbsp;word&nbsp;is&nbsp;{0}&nbsp;characters&nbsp;long.&quot;</span>,&nbsp;longestLength);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<p style="padding-left:30px"><code>The longest word is 9 characters long.</code></p>
<h2 id="MaxGrouped">Max - Grouped</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Max to get the most expensive price among each category's products.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq87()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Product&gt;&nbsp;products&nbsp;=&nbsp;GetProductList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;categories&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;p&nbsp;<span class="js__operator">in</span>&nbsp;products&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;group&nbsp;p&nbsp;by&nbsp;p.Category&nbsp;into&nbsp;g&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__brace">{</span>&nbsp;Category&nbsp;=&nbsp;g.Key,&nbsp;MostExpensivePrice&nbsp;=&nbsp;g.Max(p&nbsp;=&gt;&nbsp;p.UnitPrice)&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ObjectDumper.Write(categories);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<table border="0" cellspacing="2" cellpadding="1" width="100%" style="padding-left:30px">
<tbody>
<tr align="left" valign="top">
<td width="18%">Category=Beverages <br>
Category=Condiments <br>
Category=Produce <br>
Category=Meat/Poultry <br>
Category=Seafood <br>
Category=Dairy Products <br>
Category=Confections <br>
Category=Grains/Cereals</td>
<td>MostExpensivePrice=263.5000 <br>
MostExpensivePrice=43.9000 <br>
MostExpensivePrice=53.0000 <br>
MostExpensivePrice=123.7900 <br>
MostExpensivePrice=62.5000 <br>
MostExpensivePrice=55.0000 <br>
MostExpensivePrice=81.0000 <br>
MostExpensivePrice=38.0000</td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<h2 id="MaxElements">Max - Elements</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Max to get the products with the most expensive price in each category.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq88()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Product&gt;&nbsp;products&nbsp;=&nbsp;GetProductList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;categories&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;p&nbsp;<span class="js__operator">in</span>&nbsp;products&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;group&nbsp;p&nbsp;by&nbsp;p.Category&nbsp;into&nbsp;g&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;let&nbsp;maxPrice&nbsp;=&nbsp;g.Max(p&nbsp;=&gt;&nbsp;p.UnitPrice)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__brace">{</span>&nbsp;Category&nbsp;=&nbsp;g.Key,&nbsp;MostExpensiveProducts&nbsp;=&nbsp;g.Where(p&nbsp;=&gt;&nbsp;p.UnitPrice&nbsp;==&nbsp;maxPrice)&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ObjectDumper.Write(categories,&nbsp;<span class="js__num">1</span>);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<p style="padding-left:30px">Category=Beverages&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MostExpensiveProducts=...
<br>
&nbsp;&nbsp;MostExpensiveProducts: ProductID=38&nbsp;&nbsp;&nbsp;&nbsp;ProductName=C&Atilde;&acute;te de Blaye&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Category=Beverages&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitPrice=263.5000&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitsInStock=17
<br>
Category=Condiments&nbsp;&nbsp;&nbsp;&nbsp;MostExpensiveProducts=... <br>
&nbsp;&nbsp;MostExpensiveProducts: ProductID=63&nbsp;&nbsp;&nbsp;&nbsp;ProductName=Vegie-spread&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Category=Condiments&nbsp;&nbsp;&nbsp;&nbsp;UnitPrice=43.9000&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitsInStock=24
<br>
Category=Produce&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MostExpensiveProducts=...
<br>
&nbsp;&nbsp;MostExpensiveProducts: ProductID=51&nbsp;&nbsp;&nbsp;&nbsp;ProductName=Manjimup Dried Apples&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Category=Produce&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitPrice=53.0000&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitsInStock=20
<br>
Category=Meat/Poultry&nbsp;&nbsp;MostExpensiveProducts=... <br>
&nbsp;&nbsp;MostExpensiveProducts: ProductID=29&nbsp;&nbsp;&nbsp;&nbsp;ProductName=Th&Atilde;&frac14;ringer Rostbratwurst&nbsp;&nbsp;&nbsp;&nbsp;Category=Meat/Poultry&nbsp;&nbsp;UnitPrice=123.7900&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitsInStock=0
<br>
Category=Seafood&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MostExpensiveProducts=...
<br>
&nbsp;&nbsp;MostExpensiveProducts: ProductID=18&nbsp;&nbsp;&nbsp;&nbsp;ProductName=Carnarvon Tigers&nbsp;&nbsp;&nbsp;&nbsp;Category=Seafood&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitPrice=62.5000&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitsInStock=42
<br>
Category=Dairy Products&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MostExpensiveProducts=...
<br>
&nbsp;&nbsp;MostExpensiveProducts: ProductID=59&nbsp;&nbsp;&nbsp;&nbsp;ProductName=Raclette Courdavault&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Category=Dairy Products&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitPrice=55.0000&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitsInStock=79
<br>
Category=Confections&nbsp;&nbsp;&nbsp;&nbsp;MostExpensiveProducts=... <br>
&nbsp;&nbsp;MostExpensiveProducts: ProductID=20&nbsp;&nbsp;&nbsp;&nbsp;ProductName=Sir Rodney's Marmalade&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Category=Confections&nbsp;&nbsp;&nbsp;&nbsp;UnitPrice=81.0000&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitsInStock=40
<br>
Category=Grains/Cereals&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MostExpensiveProducts=...
<br>
&nbsp;&nbsp;MostExpensiveProducts: ProductID=56&nbsp;&nbsp;&nbsp;&nbsp;ProductName=Gnocchi di nonna Alice&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Category=Grains/Cereals&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitPrice=38.0000&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UnitsInStock=21</p>
<h2 id="AverageSimple">Average - Simple</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Average to get the average of all numbers in an array.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq89()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;numbers&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">4</span>,&nbsp;<span class="js__num">1</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">9</span>,&nbsp;<span class="js__num">8</span>,&nbsp;<span class="js__num">6</span>,&nbsp;<span class="js__num">7</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">0</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;double&nbsp;averageNum&nbsp;=&nbsp;numbers.Average();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;The&nbsp;average&nbsp;number&nbsp;is&nbsp;{0}.&quot;</span>,&nbsp;averageNum);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<p style="padding-left:30px"><code>The average number is 4.5.</code></p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="AverageProjection">Average - Projection</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Average to get the average length of the words in the array.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq90()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;string[]&nbsp;words&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__string">&quot;cherry&quot;</span>,&nbsp;<span class="js__string">&quot;apple&quot;</span>,&nbsp;<span class="js__string">&quot;blueberry&quot;</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;double&nbsp;averageLength&nbsp;=&nbsp;words.Average(w&nbsp;=&gt;&nbsp;w.Length);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;The&nbsp;average&nbsp;word&nbsp;length&nbsp;is&nbsp;{0}&nbsp;characters.&quot;</span>,&nbsp;averageLength);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<p style="padding-left:30px"><code>The average word length is 6.66666666666667 characters.</code></p>
<h2 id="AverageGrouped">Average - Grouped</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Average to get the average price of each category's products.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq91()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Product&gt;&nbsp;products&nbsp;=&nbsp;GetProductList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;categories&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;p&nbsp;<span class="js__operator">in</span>&nbsp;products&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;group&nbsp;p&nbsp;by&nbsp;p.Category&nbsp;into&nbsp;g&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__brace">{</span>&nbsp;Category&nbsp;=&nbsp;g.Key,&nbsp;AveragePrice&nbsp;=&nbsp;g.Average(p&nbsp;=&gt;&nbsp;p.UnitPrice)&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ObjectDumper.Write(categories);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<h3 class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></h3>
<table border="0" cellspacing="2" cellpadding="1" width="100%">
<tbody>
<tr align="left" valign="top">
<td width="18%" style="padding-left:30px">Category=Beverages <br>
Category=Condiments <br>
Category=Produce <br>
Category=Meat/Poultry <br>
Category=Seafood <br>
Category=Dairy Products <br>
Category=Confections <br>
Category=Grains/Cereals</td>
<td style="padding-left:30px">AveragePrice=37.979166666666666666666666667 <br>
AveragePrice=23.0625 <br>
AveragePrice=32.3700 <br>
AveragePrice=54.006666666666666666666666667 <br>
AveragePrice=20.6825 <br>
AveragePrice=28.7300 <br>
AveragePrice=25.1600 <br>
AveragePrice=20.2500</td>
</tr>
</tbody>
</table>
<p>&nbsp;</p>
<h2 id="AggregateSimple">Aggregate - Simple</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Aggregate to create a running product on the array that calculates the total product of all elements.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq92()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;double[]&nbsp;doubles&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">1.7</span>,&nbsp;<span class="js__num">2.3</span>,&nbsp;<span class="js__num">1.9</span>,&nbsp;<span class="js__num">4.1</span>,&nbsp;<span class="js__num">2.9</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;double&nbsp;product&nbsp;=&nbsp;doubles.Aggregate((runningProduct,&nbsp;nextFactor)&nbsp;=&gt;&nbsp;runningProduct&nbsp;*&nbsp;nextFactor);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;Total&nbsp;product&nbsp;of&nbsp;all&nbsp;numbers:&nbsp;{0}&quot;</span>,&nbsp;product);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<h3 class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></h3>
<p style="padding-left:30px"><code>Total product of all numbers: 88.33081</code></p>
<h2 id="AverageSeed">Aggregate - Seed</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Aggregate to create a running account balance that subtracts each withdrawal from the initial balance of 100, as long as the balance never drops below 0.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq93()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;double&nbsp;startBalance&nbsp;=&nbsp;<span class="js__num">100.0</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;attemptedWithdrawals&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">20</span>,&nbsp;<span class="js__num">10</span>,&nbsp;<span class="js__num">40</span>,&nbsp;<span class="js__num">50</span>,&nbsp;<span class="js__num">10</span>,&nbsp;<span class="js__num">70</span>,&nbsp;<span class="js__num">30</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;double&nbsp;endBalance&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;attemptedWithdrawals.Aggregate(startBalance,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(balance,&nbsp;nextWithdrawal)&nbsp;=&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;((nextWithdrawal&nbsp;&lt;=&nbsp;balance)&nbsp;?&nbsp;(balance&nbsp;-&nbsp;nextWithdrawal)&nbsp;:&nbsp;balance));&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;Ending&nbsp;balance:&nbsp;{0}&quot;</span>,&nbsp;endBalance);&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<h3 class="endscriptcode"><strong>Result</strong></h3>
<p style="padding-left:30px"><code>Ending balance: 20</code></p>
</div>
<p>&nbsp;</p>
<h1 id="SourceCodeFiles"><span>Source Code Files</span></h1>
<ul>
<li><a class="browseFile" href="sourcecode?fileId=23874&pathId=1037848421">Program.cs</a>
</li></ul>
<h1><strong>101 LINQ Samples</strong></h1>
<ul>
<li><a href="../LINQ-Restriction-Operators-b15d29ca">Restriction Operators</a> </li><li><a href="../LINQ-to-DataSets-09787825">Projection Operators</a> </li><li><a href="../LINQ-Partitioning-Operators-c68aaccc">Partitioning Operators</a> </li><li><a href="../SQL-Ordering-Operators-050af19e">Ordering Operators</a> </li><li><a href="../LINQ-to-DataSets-Grouping-c62703ea">Grouping Operators</a> </li><li><a href="../LINQ-Set-Operators-374f34fe">Set Operators</a> </li><li><a href="../LINQ-Conversion-Operators-e4e59714">Conversion Operators</a> </li><li><a href="../LINQ-Element-Operators-0f3f12ce">Element Operators</a> </li><li><a href="../LINQ-Element-Operators-0f3f12ce">Generation Operators</a> </li><li><a href="../LINQ-Quantifiers-f00e7e3e">Quantifiers</a> </li><li><a href="../LINQ-Aggregate-Operators-c51b3869">Aggregate Operators</a> </li><li><a href="../LINQ-Miscellaneous-6b72bb2a">Miscellaneous Operators</a> </li><li><a href="../LINQ-to-DataSets-Custom-41738490">Custom Sequence Operators</a> </li><li><a href="../LINQ-Query-Execution-ce0d3b95">Query Execution</a> </li><li><a href="../LINQ-Join-Operators-dabef4e9">Join Operators</a> </li></ul>
<h1>More Information</h1>
<p>For more information, see:</p>
<ul>
<li>
<div class="title">User-defined Aggregates and Operators - <a href="http://technet.microsoft.com/en-us/library/ee842720.aspx" target="_blank">
http://technet.microsoft.com/en-us/library/ee842720.aspx</a></div>
</li></ul>
