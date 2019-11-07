# LINQ - Partitioning Operators
## Requires
- Visual Studio 2010
## License
- Custom
## Technologies
- LINQ
## Topics
- Partitioning Operators
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
<td width="50px" align="left" valign="middle"><a href="http://archive.msdn.microsoft.com/vb2008samples/Release/ProjectReleases.aspx?ReleaseId=1426"><img title="101 Samples for Visual Basic 2005" src="-dd183105.download_45(en-us,msdn.10).jpg" alt="101 Samples for Visual Basic 2005" align="left"></a></td>
<td align="left" valign="middle"><span style="font-size:medium"><strong><a href="http://code.msdn.microsoft.com/101-LINQ-Samples-3fb9811b/viewsamplepack">Browse all 101 LINQ Samples</a>&nbsp;</strong></span></td>
</tr>
</tbody>
</table>
</div>
<hr>
</div>
<h1 id="Introduction">Introduction</h1>
<p>This sample shows different uses of Partitioning Operators:</p>
<ul class="bulletedlist">
<li><a href="#TakeSimple">Take - Simple</a> </li><li><a href="#TakeNested">Take - Nested</a> </li><li><a href="#SkipSimple">Skip - Simple</a> </li><li><a href="#SkipNested">Skip - Nested</a> </li><li><a href="#TakeWhileSimple">TakeWhile - Simple</a> </li><li><a title="New Link" href="#TakeWhileIndexed">TakeWhile - Indexed</a> </li><li><a href="#SkipWhileSimple">SkipWhile - Simple</a> </li><li><a href="#SkipWhileIndexed">SkipWhile - Indexed</a> </li></ul>
<h1><span>Building the Sample</span></h1>
<ol>
<li>Open the Program.cs </li><li>Comment or uncomment the desired samples </li><li>Press Ctrl &#43; F5 </li></ol>
<h1>Description</h1>
<div class="BostonPostCard">
<h2 id="TakeSimple">Take - Simple</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Take to get only the first 3 elements of the array.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq20()
 
{
 
    int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
 
 
 
    var first3Numbers = numbers.Take(3);
 
 
 
    Console.WriteLine(&quot;First 3 numbers:&quot;);
 
    foreach (var n in first3Numbers)
 
    {
 
        Console.WriteLine(n);
 
    }
 
}</pre>
<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq20()&nbsp;
&nbsp;&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;numbers&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">4</span>,&nbsp;<span class="js__num">1</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">9</span>,&nbsp;<span class="js__num">8</span>,&nbsp;<span class="js__num">6</span>,&nbsp;<span class="js__num">7</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">0</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;first3Numbers&nbsp;=&nbsp;numbers.Take(<span class="js__num">3</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;First&nbsp;3&nbsp;numbers:&quot;</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;n&nbsp;<span class="js__operator">in</span>&nbsp;first3Numbers)&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(n);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold">Result</span></div>
</div>
<div class="BostonPostCard">
<p style="padding-left:30px"><code>First 3 numbers: <br>
5 <br>
4 <br>
1</code></p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="TakeNested">Take - Nested</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Take to get the first 3 orders from customers in Washington.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq21()
 
{
 
    List&lt;Customer&gt; customers = GetCustomerList();
 
 
 
    var first3WAOrders = (
 
        from c in customers
 
        from o in c.Orders
 
        where c.Region == &quot;WA&quot;
 
        select new { c.CustomerID, o.OrderID, o.OrderDate })
 
        .Take(3);
 
 
 
    Console.WriteLine(&quot;First 3 orders in WA:&quot;);
 
    foreach (var order in first3WAOrders)
 
    {
 
        ObjectDumper.Write(order);
 
    }
 
}</pre>
<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq21()&nbsp;
&nbsp;&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Customer&gt;&nbsp;customers&nbsp;=&nbsp;GetCustomerList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;first3WAOrders&nbsp;=&nbsp;(&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;c&nbsp;<span class="js__operator">in</span>&nbsp;customers&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;o&nbsp;<span class="js__operator">in</span>&nbsp;c.Orders&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;where&nbsp;c.Region&nbsp;==&nbsp;<span class="js__string">&quot;WA&quot;</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__brace">{</span>&nbsp;c.CustomerID,&nbsp;o.OrderID,&nbsp;o.OrderDate&nbsp;<span class="js__brace">}</span>)&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.Take(<span class="js__num">3</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;First&nbsp;3&nbsp;orders&nbsp;in&nbsp;WA:&quot;</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;order&nbsp;<span class="js__operator">in</span>&nbsp;first3WAOrders)&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ObjectDumper.Write(order);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold">Result</span></div>
</div>
<div class="BostonPostCard">
<p style="padding-left:30px"><code>First 3 orders in WA: <br>
CustomerID=LAZYK OrderID=10482 OrderDate=3/21/1997 <br>
CustomerID=LAZYK OrderID=10545 OrderDate=5/22/1997 <br>
CustomerID=TRAIH OrderID=10574 OrderDate=6/19/1997</code></p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="SkipSimple">Skip - Simple</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Skip to get all but the first 4 elements of the array.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq22()
 
{
 
    int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
 
 
 
    var allButFirst4Numbers = numbers.Skip(4);
 
 
 
    Console.WriteLine(&quot;All but first 4 numbers:&quot;);
 
    foreach (var n in allButFirst4Numbers)
 
    {
 
        Console.WriteLine(n);
 
    }
 
}</pre>
<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq22()&nbsp;
&nbsp;&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;numbers&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">4</span>,&nbsp;<span class="js__num">1</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">9</span>,&nbsp;<span class="js__num">8</span>,&nbsp;<span class="js__num">6</span>,&nbsp;<span class="js__num">7</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">0</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;allButFirst4Numbers&nbsp;=&nbsp;numbers.Skip(<span class="js__num">4</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;All&nbsp;but&nbsp;first&nbsp;4&nbsp;numbers:&quot;</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;n&nbsp;<span class="js__operator">in</span>&nbsp;allButFirst4Numbers)&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(n);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold">Result</span></div>
</div>
<div class="BostonPostCard">
<p style="padding-left:30px"><code>All but first 4 numbers: <br>
9 <br>
8 <br>
6 <br>
7 <br>
2 <br>
0</code></p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="SkipNested">Skip - Nested</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Take to get all but the first 2 orders from customers in Washington.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq23()
 
{
 
    List&lt;Customer&gt; customers = GetCustomerList();
 
 
 
    var waOrders =
 
        from c in customers
 
        from o in c.Orders
 
        where c.Region == &quot;WA&quot;
 
        select new { c.CustomerID, o.OrderID, o.OrderDate };
 
 
 
    var allButFirst2Orders = waOrders.Skip(2);
 
 
 
    Console.WriteLine(&quot;All but first 2 orders in WA:&quot;);
 
    foreach (var order in allButFirst2Orders)
 
    {
 
        ObjectDumper.Write(order);
 
    }
 
}</pre>
<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq23()&nbsp;
&nbsp;&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Customer&gt;&nbsp;customers&nbsp;=&nbsp;GetCustomerList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;waOrders&nbsp;=&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;c&nbsp;<span class="js__operator">in</span>&nbsp;customers&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;o&nbsp;<span class="js__operator">in</span>&nbsp;c.Orders&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;where&nbsp;c.Region&nbsp;==&nbsp;<span class="js__string">&quot;WA&quot;</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__brace">{</span>&nbsp;c.CustomerID,&nbsp;o.OrderID,&nbsp;o.OrderDate&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;allButFirst2Orders&nbsp;=&nbsp;waOrders.Skip(<span class="js__num">2</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;All&nbsp;but&nbsp;first&nbsp;2&nbsp;orders&nbsp;in&nbsp;WA:&quot;</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;order&nbsp;<span class="js__operator">in</span>&nbsp;allButFirst2Orders)&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ObjectDumper.Write(order);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold">Result</span></div>
</div>
<div class="BostonPostCard">
<p style="padding-left:30px">All but first 2 orders in WA:<br>
CustomerID=TRAIH OrderID=10574 OrderDate=6/19/1997<br>
CustomerID=TRAIH OrderID=10577 OrderDate=6/23/1997<br>
CustomerID=TRAIH OrderID=10822 OrderDate=1/8/1998<br>
CustomerID=WHITC OrderID=10269 OrderDate=7/31/1996<br>
CustomerID=WHITC OrderID=10344 OrderDate=11/1/1996<br>
CustomerID=WHITC OrderID=10469 OrderDate=3/10/1997<br>
CustomerID=WHITC OrderID=10483 OrderDate=3/24/1997<br>
CustomerID=WHITC OrderID=10504 OrderDate=4/11/1997<br>
CustomerID=WHITC OrderID=10596 OrderDate=7/11/1997<br>
CustomerID=WHITC OrderID=10693 OrderDate=10/6/1997<br>
CustomerID=WHITC OrderID=10696 OrderDate=10/8/1997<br>
CustomerID=WHITC OrderID=10723 OrderDate=10/30/1997<br>
CustomerID=WHITC OrderID=10740 OrderDate=11/13/1997<br>
CustomerID=WHITC OrderID=10861 OrderDate=1/30/1998<br>
CustomerID=WHITC OrderID=10904 OrderDate=2/24/1998<br>
CustomerID=WHITC OrderID=11032 OrderDate=4/17/1998<br>
CustomerID=WHITC OrderID=11066 OrderDate=5/1/1998</p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="TakeWhileSimple">TakeWhile - Simple</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses TakeWhile to return elements starting from the beginning of the array until a number is hit that is not less than 6.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq24()
 
{
 
    int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
 
 
 
    var firstNumbersLessThan6 = numbers.TakeWhile(n =&gt; n &lt; 6);
 
 
 
    Console.WriteLine(&quot;First numbers less than 6:&quot;);
 
    foreach (var n in firstNumbersLessThan6)
 
    {
 
        Console.WriteLine(n);
 
    }
 
}</pre>
<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq24()&nbsp;
&nbsp;&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;numbers&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">4</span>,&nbsp;<span class="js__num">1</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">9</span>,&nbsp;<span class="js__num">8</span>,&nbsp;<span class="js__num">6</span>,&nbsp;<span class="js__num">7</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">0</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;firstNumbersLessThan6&nbsp;=&nbsp;numbers.TakeWhile(n&nbsp;=&gt;&nbsp;n&nbsp;&lt;&nbsp;<span class="js__num">6</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;First&nbsp;numbers&nbsp;less&nbsp;than&nbsp;6:&quot;</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;n&nbsp;<span class="js__operator">in</span>&nbsp;firstNumbersLessThan6)&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(n);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<p>&nbsp;</p>
</div>
<div class="BostonPostCard">
<h3>Result</h3>
<p style="padding-left:30px">First numbers less than 6:<br>
5<br>
4<br>
1<br>
3</p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="TakeWhileIndexed">TakeWhile - Indexed</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses TakeWhile to return elements starting from the beginning of the array until a number is hit that is less than its position in the array.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq25()
 
{
 
    int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
 
 
 
    var firstSmallNumbers = numbers.TakeWhile((n, index) =&gt; n &gt;= index);
 
 
 
    Console.WriteLine(&quot;First numbers not less than their position:&quot;);
 
    foreach (var n in firstSmallNumbers)
 
    {
 
        Console.WriteLine(n);
 
    }
 
}</pre>
<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq25()&nbsp;
&nbsp;&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;numbers&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">4</span>,&nbsp;<span class="js__num">1</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">9</span>,&nbsp;<span class="js__num">8</span>,&nbsp;<span class="js__num">6</span>,&nbsp;<span class="js__num">7</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">0</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;firstSmallNumbers&nbsp;=&nbsp;numbers.TakeWhile((n,&nbsp;index)&nbsp;=&gt;&nbsp;n&nbsp;&gt;=&nbsp;index);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;First&nbsp;numbers&nbsp;not&nbsp;less&nbsp;than&nbsp;their&nbsp;position:&quot;</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;n&nbsp;<span class="js__operator">in</span>&nbsp;firstSmallNumbers)&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(n);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold">Result</span></div>
</div>
<div class="BostonPostCard">
<p style="padding-left:30px"><code>First numbers not less than their position: <br>
5 <br>
4</code></p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="SkipWhileSimple">SkipWhile - Simple</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses SkipWhile to get the elements of the array starting from the first element divisible by 3.</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq26()
 
{
 
    int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
 
 
 
    var allButFirst3Numbers = numbers.SkipWhile(n =&gt; n % 3 != 0);
 
 
 
    Console.WriteLine(&quot;All elements starting from first element divisible by 3:&quot;);
 
    foreach (var n in allButFirst3Numbers)
 
    {
 
        Console.WriteLine(n);
 
    }
 
}</pre>
<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq26()&nbsp;
&nbsp;&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;numbers&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">4</span>,&nbsp;<span class="js__num">1</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">9</span>,&nbsp;<span class="js__num">8</span>,&nbsp;<span class="js__num">6</span>,&nbsp;<span class="js__num">7</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">0</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;allButFirst3Numbers&nbsp;=&nbsp;numbers.SkipWhile(n&nbsp;=&gt;&nbsp;n&nbsp;%&nbsp;<span class="js__num">3</span>&nbsp;!=&nbsp;<span class="js__num">0</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;All&nbsp;elements&nbsp;starting&nbsp;from&nbsp;first&nbsp;element&nbsp;divisible&nbsp;by&nbsp;3:&quot;</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;n&nbsp;<span class="js__operator">in</span>&nbsp;allButFirst3Numbers)&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(n);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span style="font-size:12px; font-weight:bold">Result</span></div>
</div>
<p style="padding-left:30px">All elements starting from first element divisible by 3:<br>
3<br>
9<br>
8<br>
6<br>
7<br>
2<br>
0</p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="SkipWhileIndexed">SkipWhile - Indexed</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses SkipWhile to get the elements of the array starting from the first element less than its position.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public void Linq27()
 
{
 
    int[] numbers = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
 
 
 
    var laterNumbers = numbers.SkipWhile((n, index) =&gt; n &gt;= index);
 
 
 
    Console.WriteLine(&quot;All elements starting from first element less than its position:&quot;);
 
    foreach (var n in laterNumbers)
 
    {
 
        Console.WriteLine(n);
 
    }
 
}</pre>
<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq27()&nbsp;
&nbsp;&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;numbers&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">4</span>,&nbsp;<span class="js__num">1</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">9</span>,&nbsp;<span class="js__num">8</span>,&nbsp;<span class="js__num">6</span>,&nbsp;<span class="js__num">7</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">0</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;laterNumbers&nbsp;=&nbsp;numbers.SkipWhile((n,&nbsp;index)&nbsp;=&gt;&nbsp;n&nbsp;&gt;=&nbsp;index);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;All&nbsp;elements&nbsp;starting&nbsp;from&nbsp;first&nbsp;element&nbsp;less&nbsp;than&nbsp;its&nbsp;position:&quot;</span>);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;n&nbsp;<span class="js__operator">in</span>&nbsp;laterNumbers)&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(n);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<h3>Result</h3>
<p style="padding-left:30px">All elements starting from first element less than its position:<br>
1<br>
3<br>
9<br>
8<br>
6<br>
7<br>
2<br>
0</p>
<p>&nbsp;</p>
<p><span style="font-size:20px; font-weight:bold">Source Code Files</span></p>
<ul>
<li><a class="browseFile" href="sourcecode?fileId=23919&pathId=901540695">Program.cs</a>
</li></ul>
<h1><strong>101 LINQ Samples</strong></h1>
<ul>
<li><a href="../LINQ-Restriction-Operators-b15d29ca">Restriction Operators</a> </li><li><a href="../LINQ-to-DataSets-09787825">Projection Operators</a> </li><li><a href="../LINQ-Partitioning-Operators-c68aaccc">Partitioning Operators</a> </li><li><a href="../SQL-Ordering-Operators-050af19e">Ordering Operators</a> </li><li><a href="../LINQ-to-DataSets-Grouping-c62703ea">Grouping Operators</a> </li><li><a href="../LINQ-Set-Operators-374f34fe">Set Operators</a> </li><li><a href="../LINQ-Conversion-Operators-e4e59714">Conversion Operators</a> </li><li><a href="../LINQ-Element-Operators-0f3f12ce">Element Operators</a> </li><li><a href="../LINQ-Element-Operators-0f3f12ce">Generation Operators</a> </li><li><a href="../LINQ-Quantifiers-f00e7e3e">Quantifiers</a> </li><li><a href="../LINQ-Aggregate-Operators-c51b3869">Aggregate Operators</a> </li><li><a href="../LINQ-Miscellaneous-6b72bb2a">Miscellaneous Operators</a> </li><li><a href="../LINQ-to-DataSets-Custom-41738490">Custom Sequence Operators</a> </li><li><a href="../LINQ-Query-Execution-ce0d3b95">Query Execution</a> </li><li><a href="../LINQ-Join-Operators-dabef4e9">Join Operators</a> </li></ul>
<h1>More Information</h1>
<p>For more information, see:</p>
<ul>
<li>Partitioning Data - <a href="http://msdn.microsoft.com/en-us/library/bb546164.aspx" target="_blank">
http://msdn.microsoft.com/en-us/library/bb546164.aspx</a> </li></ul>
