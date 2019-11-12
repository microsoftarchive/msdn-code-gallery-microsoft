# LINQ - Join Operators
## Requires
- Visual Studio 2010
## License
- Custom
## Technologies
- LINQ
## Topics
- Join Operators
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
<h1 id="Introduction">Introduction</h1>
<p>This sample shows different uses of Join Operators:</p>
<ul class="bulletedlist">
<li><a title="New Link" href="#crossjoin">Cross Join</a> </li><li><a title="New Link" href="#groupjoin">Group Join</a> </li><li><a title="New Link" href="#crossgroup">Cross Join with Group Join</a> </li><li><a title="New Link" href="#leftouterjoin">Left Outer Join</a> </li></ul>
<h1><span>Building the Sample</span></h1>
<ol>
<li>Open the Program.cs </li><li>Comment or uncomment the desired samples </li><li>Press Ctrl &#43; F5 </li></ol>
<h1>Description</h1>
<h2 id="crossjoin">Cross Join</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample shows how to efficiently join elements of two sequences based on equality between key expressions over the two.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Linq102()&nbsp;
{&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>[]&nbsp;categories&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;<span class="cs__keyword">string</span>[]{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;Beverages&quot;</span>,&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;Condiments&quot;</span>,&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;Vegetables&quot;</span>,&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;Dairy&nbsp;Products&quot;</span>,&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;Seafood&quot;</span>&nbsp;};&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Product&gt;&nbsp;products&nbsp;=&nbsp;GetProductList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;q&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;c&nbsp;<span class="cs__keyword">in</span>&nbsp;categories&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;join&nbsp;p&nbsp;<span class="cs__keyword">in</span>&nbsp;products&nbsp;on&nbsp;c&nbsp;equals&nbsp;p.Category&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;<span class="cs__keyword">new</span>&nbsp;{&nbsp;Category&nbsp;=&nbsp;c,&nbsp;p.ProductName&nbsp;};&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">foreach</span>&nbsp;(var&nbsp;v&nbsp;<span class="cs__keyword">in</span>&nbsp;q)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(v.ProductName&nbsp;&#43;&nbsp;<span class="cs__string">&quot;:&nbsp;&quot;</span>&nbsp;&#43;&nbsp;v.Category);&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}</pre>
</div>
</div>
</div>
<h3><strong>Result</strong></h3>
<p style="padding-left:30px">Chai: Beverages<br>
Chang: Beverages<br>
Guaran&Atilde;&iexcl; Fant&Atilde;&iexcl;stica: Beverages<br>
Sasquatch Ale: Beverages<br>
Steeleye Stout: Beverages<br>
C&Atilde;&acute;te de Blaye: Beverages<br>
Chartreuse verte: Beverages<br>
Ipoh Coffee: Beverages<br>
Laughing Lumberjack Lager: Beverages<br>
Outback Lager: Beverages<br>
Rh&Atilde;&para;nbr&Atilde;&curren;u Klosterbier: Beverages<br>
Lakkalik&Atilde;&para;&Atilde;&para;ri: Beverages<br>
Aniseed Syrup: Condiments<br>
Chef Anton's Cajun Seasoning: Condiments<br>
Chef Anton's Gumbo Mix: Condiments<br>
Grandma's Boysenberry Spread: Condiments<br>
Northwoods Cranberry Sauce: Condiments<br>
Genen Shouyu: Condiments<br>
Gula Malacca: Condiments<br>
Sirop d'&Atilde;&copy;rable: Condiments<br>
Vegie-spread: Condiments<br>
Louisiana Fiery Hot Pepper Sauce: Condiments<br>
Louisiana Hot Spiced Okra: Condiments<br>
Original Frankfurter gr&Atilde;&frac14;ne So&Atilde;&Yuml;e: Condiments<br>
Queso Cabrales: Dairy Products<br>
Queso Manchego La Pastora: Dairy Products<br>
Gorgonzola Telino: Dairy Products<br>
Mascarpone Fabioli: Dairy Products<br>
Geitost: Dairy Products<br>
Raclette Courdavault: Dairy Products<br>
Camembert Pierrot: Dairy Products<br>
Gudbrandsdalsost: Dairy Products<br>
Flotemysost: Dairy Products<br>
Mozzarella di Giovanni: Dairy Products<br>
Ikura: Seafood<br>
Konbu: Seafood<br>
Carnarvon Tigers: Seafood<br>
Nord-Ost Matjeshering: Seafood<br>
Inlagd Sill: Seafood<br>
Gravad lax: Seafood<br>
Boston Crab Meat: Seafood<br>
Jack's New England Clam Chowder: Seafood<br>
Rogede sild: Seafood<br>
Spegesild: Seafood<br>
Escargots de Bourgogne: Seafood<br>
R&Atilde;&para;d Kaviar: Seafood</p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="groupjoin">Group Join</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>Using a group join you can get all the products that match a given category bundled as a sequence.</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq103()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;string[]&nbsp;categories&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;string[]<span class="js__brace">{</span>&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__string">&quot;Beverages&quot;</span>,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__string">&quot;Condiments&quot;</span>,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__string">&quot;Vegetables&quot;</span>,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__string">&quot;Dairy&nbsp;Products&quot;</span>,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__string">&quot;Seafood&quot;</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Product&gt;&nbsp;products&nbsp;=&nbsp;GetProductList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;q&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;c&nbsp;<span class="js__operator">in</span>&nbsp;categories&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;join&nbsp;p&nbsp;<span class="js__operator">in</span>&nbsp;products&nbsp;on&nbsp;c&nbsp;equals&nbsp;p.Category&nbsp;into&nbsp;ps&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__brace">{</span>&nbsp;Category&nbsp;=&nbsp;c,&nbsp;Products&nbsp;=&nbsp;ps&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;v&nbsp;<span class="js__operator">in</span>&nbsp;q)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(v.Category&nbsp;&#43;&nbsp;<span class="js__string">&quot;:&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;p&nbsp;<span class="js__operator">in</span>&nbsp;v.Products)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;&nbsp;&nbsp;&nbsp;&quot;</span>&nbsp;&#43;&nbsp;p.ProductName);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<h3><strong>Result</strong></h3>
<p style="padding-left:30px">Beverages:<br>
Chai<br>
Chang<br>
Guaran&Atilde;&iexcl; Fant&Atilde;&iexcl;stica<br>
Sasquatch Ale<br>
Steeleye Stout<br>
C&Atilde;&acute;te de Blaye<br>
Chartreuse verte<br>
Ipoh Coffee<br>
Laughing Lumberjack Lager<br>
Outback Lager<br>
Rh&Atilde;&para;nbr&Atilde;&curren;u Klosterbier<br>
Lakkalik&Atilde;&para;&Atilde;&para;ri<br>
Condiments:<br>
Aniseed Syrup<br>
Chef Anton's Cajun Seasoning<br>
Chef Anton's Gumbo Mix<br>
Grandma's Boysenberry Spread<br>
Northwoods Cranberry Sauce<br>
Genen Shouyu<br>
Gula Malacca<br>
Sirop d'&Atilde;&copy;rable<br>
Vegie-spread<br>
Louisiana Fiery Hot Pepper Sauce<br>
Louisiana Hot Spiced Okra<br>
Original Frankfurter gr&Atilde;&frac14;ne So&Atilde;&Yuml;e<br>
Vegetables:<br>
Dairy Products:<br>
Queso Cabrales<br>
Queso Manchego La Pastora<br>
Gorgonzola Telino<br>
Mascarpone Fabioli<br>
Geitost<br>
Raclette Courdavault<br>
Camembert Pierrot<br>
Gudbrandsdalsost<br>
Flotemysost<br>
Mozzarella di Giovanni<br>
Seafood:<br>
Ikura<br>
Konbu<br>
Carnarvon Tigers<br>
Nord-Ost Matjeshering<br>
Inlagd Sill<br>
Gravad lax<br>
Boston Crab Meat<br>
Jack's New England Clam Chowder<br>
Rogede sild<br>
Spegesild<br>
Escargots de Bourgogne<br>
R&Atilde;&para;d Kaviar</p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="crossgroup">Cross Join with Group Join</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>The group join operator is more general than join, as this slightly more verbose version of the cross join sample shows.</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq104()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;string[]&nbsp;categories&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;string[]<span class="js__brace">{</span>&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__string">&quot;Beverages&quot;</span>,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__string">&quot;Condiments&quot;</span>,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__string">&quot;Vegetables&quot;</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__string">&quot;Dairy&nbsp;Products&quot;</span>,&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__string">&quot;Seafood&quot;</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Product&gt;&nbsp;products&nbsp;=&nbsp;GetProductList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;q&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;c&nbsp;<span class="js__operator">in</span>&nbsp;categories&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;join&nbsp;p&nbsp;<span class="js__operator">in</span>&nbsp;products&nbsp;on&nbsp;c&nbsp;equals&nbsp;p.Category&nbsp;into&nbsp;ps&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;p&nbsp;<span class="js__operator">in</span>&nbsp;ps&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;<span class="js__operator">new</span>&nbsp;<span class="js__brace">{</span>&nbsp;Category&nbsp;=&nbsp;c,&nbsp;p.ProductName&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;v&nbsp;<span class="js__operator">in</span>&nbsp;q)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(v.ProductName&nbsp;&#43;&nbsp;<span class="js__string">&quot;:&nbsp;&quot;</span>&nbsp;&#43;&nbsp;v.Category);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<h3><strong>Result</strong></h3>
<p style="padding-left:30px">Chai: Beverages<br>
Chang: Beverages<br>
Guaran&Atilde;&iexcl; Fant&Atilde;&iexcl;stica: Beverages<br>
Sasquatch Ale: Beverages<br>
Steeleye Stout: Beverages<br>
C&Atilde;&acute;te de Blaye: Beverages<br>
Chartreuse verte: Beverages<br>
Ipoh Coffee: Beverages<br>
Laughing Lumberjack Lager: Beverages<br>
Outback Lager: Beverages<br>
Rh&Atilde;&para;nbr&Atilde;&curren;u Klosterbier: Beverages<br>
Lakkalik&Atilde;&para;&Atilde;&para;ri: Beverages<br>
Aniseed Syrup: Condiments<br>
Chef Anton's Cajun Seasoning: Condiments<br>
Chef Anton's Gumbo Mix: Condiments<br>
Grandma's Boysenberry Spread: Condiments<br>
Northwoods Cranberry Sauce: Condiments<br>
Genen Shouyu: Condiments<br>
Gula Malacca: Condiments<br>
Sirop d'&Atilde;&copy;rable: Condiments<br>
Vegie-spread: Condiments<br>
Louisiana Fiery Hot Pepper Sauce: Condiments<br>
Louisiana Hot Spiced Okra: Condiments<br>
Original Frankfurter gr&Atilde;&frac14;ne So&Atilde;&Yuml;e: Condiments<br>
Queso Cabrales: Dairy Products<br>
Queso Manchego La Pastora: Dairy Products<br>
Gorgonzola Telino: Dairy Products<br>
Mascarpone Fabioli: Dairy Products<br>
Geitost: Dairy Products<br>
Raclette Courdavault: Dairy Products<br>
Camembert Pierrot: Dairy Products<br>
Gudbrandsdalsost: Dairy Products<br>
Flotemysost: Dairy Products<br>
Mozzarella di Giovanni: Dairy Products<br>
Ikura: Seafood<br>
Konbu: Seafood<br>
Carnarvon Tigers: Seafood<br>
Nord-Ost Matjeshering: Seafood<br>
Inlagd Sill: Seafood<br>
Gravad lax: Seafood<br>
Boston Crab Meat: Seafood<br>
Jack's New England Clam Chowder: Seafood<br>
Rogede sild: Seafood<br>
Spegesild: Seafood<br>
Escargots de Bourgogne: Seafood<br>
R&Atilde;&para;d Kaviar: Seafood</p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="leftouterjoin">Left Outer Join</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>A so-called outer join can be expressed with a group join. A left outer joinis like a cross join, except that all the left hand side elements get included at least once, even if they don't match any right hand side elements. Note how Vegetablesshows up in
 the output even though it has no matching products.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;Linq105()&nbsp;&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>[]&nbsp;categories&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;<span class="cs__keyword">string</span>[]{&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;Beverages&quot;</span>,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;Condiments&quot;</span>,&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;Vegetables&quot;</span>,&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;Dairy&nbsp;Products&quot;</span>,&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__string">&quot;Seafood&quot;</span>&nbsp;};&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Product&gt;&nbsp;products&nbsp;=&nbsp;GetProductList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;q&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;c&nbsp;<span class="cs__keyword">in</span>&nbsp;categories&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;join&nbsp;p&nbsp;<span class="cs__keyword">in</span>&nbsp;products&nbsp;on&nbsp;c&nbsp;equals&nbsp;p.Category&nbsp;into&nbsp;ps&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;p&nbsp;<span class="cs__keyword">in</span>&nbsp;ps.DefaultIfEmpty()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;<span class="cs__keyword">new</span>&nbsp;{&nbsp;Category&nbsp;=&nbsp;c,&nbsp;ProductName&nbsp;=&nbsp;p&nbsp;==&nbsp;<span class="cs__keyword">null</span>&nbsp;?&nbsp;<span class="cs__string">&quot;(No&nbsp;products)&quot;</span>&nbsp;:&nbsp;p.ProductName&nbsp;};&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">foreach</span>&nbsp;(var&nbsp;v&nbsp;<span class="cs__keyword">in</span>&nbsp;q)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(v.ProductName&nbsp;&#43;&nbsp;<span class="cs__string">&quot;:&nbsp;&quot;</span>&nbsp;&#43;&nbsp;v.Category);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<h3><strong>Result</strong></h3>
<p style="padding-left:30px">Chai: Beverages<br>
Chang: Beverages<br>
Guaran&Atilde;&iexcl; Fant&Atilde;&iexcl;stica: Beverages<br>
Sasquatch Ale: Beverages<br>
Steeleye Stout: Beverages<br>
C&Atilde;&acute;te de Blaye: Beverages<br>
Chartreuse verte: Beverages<br>
Ipoh Coffee: Beverages<br>
Laughing Lumberjack Lager: Beverages<br>
Outback Lager: Beverages<br>
Rh&Atilde;&para;nbr&Atilde;&curren;u Klosterbier: Beverages<br>
Lakkalik&Atilde;&para;&Atilde;&para;ri: Beverages<br>
Aniseed Syrup: Condiments<br>
Chef Anton's Cajun Seasoning: Condiments<br>
Chef Anton's Gumbo Mix: Condiments<br>
Grandma's Boysenberry Spread: Condiments<br>
Northwoods Cranberry Sauce: Condiments<br>
Genen Shouyu: Condiments<br>
Gula Malacca: Condiments<br>
Sirop d'&Atilde;&copy;rable: Condiments<br>
Vegie-spread: Condiments<br>
Louisiana Fiery Hot Pepper Sauce: Condiments<br>
Louisiana Hot Spiced Okra: Condiments<br>
Original Frankfurter gr&Atilde;&frac14;ne So&Atilde;&Yuml;e: Condiments<br>
(No products): Vegetables<br>
Queso Cabrales: Dairy Products<br>
Queso Manchego La Pastora: Dairy Products<br>
Gorgonzola Telino: Dairy Products<br>
Mascarpone Fabioli: Dairy Products<br>
Geitost: Dairy Products<br>
Raclette Courdavault: Dairy Products<br>
Camembert Pierrot: Dairy Products<br>
Gudbrandsdalsost: Dairy Products<br>
Flotemysost: Dairy Products<br>
Mozzarella di Giovanni: Dairy Products<br>
Ikura: Seafood<br>
Konbu: Seafood<br>
Carnarvon Tigers: Seafood<br>
Nord-Ost Matjeshering: Seafood<br>
Inlagd Sill: Seafood<br>
Gravad lax: Seafood<br>
Boston Crab Meat: Seafood<br>
Jack's New England Clam Chowder: Seafood<br>
Rogede sild: Seafood<br>
Spegesild: Seafood<br>
Escargots de Bourgogne: Seafood<br>
R&Atilde;&para;d Kaviar: Seafood</p>
<p>&nbsp;</p>
<p><span style="font-size:20px; font-weight:bold">Source Code Files</span></p>
<ul>
<li><a class="browseFile" href="sourcecode?fileId=23900&pathId=1791866665">Program.cs</a>
</li></ul>
<h1><strong>101 LINQ Samples</strong></h1>
<ul>
<li><a href="../LINQ-Restriction-Operators-b15d29ca">Restriction Operators</a> </li><li><a href="../LINQ-to-DataSets-09787825">Projection Operators</a> </li><li><a href="../LINQ-Partitioning-Operators-c68aaccc">Partitioning Operators</a> </li><li><a href="../SQL-Ordering-Operators-050af19e">Ordering Operators</a> </li><li><a href="../LINQ-to-DataSets-Grouping-c62703ea">Grouping Operators</a> </li><li><a href="../LINQ-Set-Operators-374f34fe">Set Operators</a> </li><li><a href="../LINQ-Conversion-Operators-e4e59714">Conversion Operators</a> </li><li><a href="../LINQ-Element-Operators-0f3f12ce">Element Operators</a> </li><li><a href="../LINQ-Element-Operators-0f3f12ce">Generation Operators</a> </li><li><a href="../LINQ-Quantifiers-f00e7e3e">Quantifiers</a> </li><li><a href="../LINQ-Aggregate-Operators-c51b3869">Aggregate Operators</a> </li><li><a href="../LINQ-Miscellaneous-6b72bb2a">Miscellaneous Operators</a> </li><li><a href="../LINQ-to-DataSets-Custom-41738490">Custom Sequence Operators</a> </li><li><a href="../LINQ-Query-Execution-ce0d3b95">Query Execution</a> </li><li><a href="../LINQ-Join-Operators-dabef4e9">Join Operators</a> </li></ul>
<h1>More Information</h1>
<p>For more information, see:</p>
<ul>
<li>Join Operations - <a href="http://msdn.microsoft.com/en-us/library/bb397908.aspx" target="_blank">
http://msdn.microsoft.com/en-us/library/bb397908.aspx</a> </li></ul>
</div>
