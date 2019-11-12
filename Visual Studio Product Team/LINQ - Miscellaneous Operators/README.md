# LINQ - Miscellaneous Operators
## Requires
- Visual Studio 2010
## License
- Custom
## Technologies
- LINQ
## Topics
- Miscellaneous Operators
## Updated
- 08/16/2011
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
<td width="50px" align="left" valign="middle"><a href="http://archive.msdn.microsoft.com/vb2008samples/Release/ProjectReleases.aspx?ReleaseId=1426"><img title="101 Samples for Visual Basic 2005" src="-dd183105.download_45(en-us,msdn.10).jpg" alt="101 101 LINQ Samples" align="left"></a></td>
<td align="left" valign="middle"><span style="font-size:medium"><strong><a href="http://code.msdn.microsoft.com/101-LINQ-Samples-3fb9811b/viewsamplepack">Browse all 101 LINQ Samples</a>&nbsp;</strong></span></td>
</tr>
</tbody>
</table>
</div>
<hr>
</div>
<h1 id="Introduction">Introduction</h1>
<p>This sample shows different uses of Miscellaneous Operators:</p>
<ul class="bulletedlist">
<li><a href="#Concat1">Concat - 1</a> </li><li><a href="#Concat2">Concat - 2</a> </li><li><a href="#EqualAll1">EqualAll - 1</a> </li><li><a href="#EqualAll2">EqualAll - 2</a> </li></ul>
<p>&nbsp;</p>
<h1><span>Building the Sample</span></h1>
<ol>
<li>Open the Program.cs </li><li>Comment or uncomment the desired samples </li><li>Press Ctrl &#43; F5 </li></ol>
<h1>Description</h1>
<h2 id="Concat1">Concat - 1</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses Concat to create one sequence that contains each array's values, one after the other.</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq94()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;numbersA&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">0</span>,&nbsp;<span class="js__num">2</span>,&nbsp;<span class="js__num">4</span>,&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">6</span>,&nbsp;<span class="js__num">8</span>,&nbsp;<span class="js__num">9</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;int[]&nbsp;numbersB&nbsp;=&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__num">1</span>,&nbsp;<span class="js__num">3</span>,&nbsp;<span class="js__num">5</span>,&nbsp;<span class="js__num">7</span>,&nbsp;<span class="js__num">8</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;allNumbers&nbsp;=&nbsp;numbersA.Concat(numbersB);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;All&nbsp;numbers&nbsp;from&nbsp;both&nbsp;arrays:&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;n&nbsp;<span class="js__operator">in</span>&nbsp;allNumbers)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(n);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<p><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></p>
<div class="BostonPostCard">
<p style="padding-left:30px"><code>All numbers from both arrays:&nbsp;<br>
0&nbsp;<br>
2&nbsp;<br>
4&nbsp;<br>
5&nbsp;<br>
6&nbsp;<br>
8&nbsp;<br>
9&nbsp;<br>
1&nbsp;<br>
3&nbsp;<br>
5&nbsp;<br>
7&nbsp;<br>
8</code></p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="Concat2">Concat - 2</h2>
<span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span>
<p>This sample uses Concat to create one sequence that contains the names of all customers and products, including any duplicates.</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq95()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Customer&gt;&nbsp;customers&nbsp;=&nbsp;GetCustomerList();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;Product&gt;&nbsp;products&nbsp;=&nbsp;GetProductList();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;customerNames&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;c&nbsp;<span class="js__operator">in</span>&nbsp;customers&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;c.CompanyName;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;productNames&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;from&nbsp;p&nbsp;<span class="js__operator">in</span>&nbsp;products&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;p.ProductName;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;allNames&nbsp;=&nbsp;customerNames.Concat(productNames);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;Customer&nbsp;and&nbsp;product&nbsp;names:&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;foreach&nbsp;(<span class="js__statement">var</span>&nbsp;n&nbsp;<span class="js__operator">in</span>&nbsp;allNames)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(n);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
<span class="js__brace">}</span>&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:12px; font-weight:bold"><strong>Result</strong></span></div>
<p>&nbsp;</p>
</div>
<div class="BostonPostCard">
<p style="padding-left:30px"><code>Customer and product names:<br>
Alfreds Futterkiste<br>
Ana Trujillo Emparedados y helados<br>
Antonio Moreno Taquer&Atilde;&shy;a<br>
Around the Horn<br>
Berglunds snabbk&Atilde;&para;p<br>
Blauer See Delikatessen<br>
Blondel p&Atilde;&uml;re et fils<br>
B&Atilde;&sup3;lido Comidas preparadas<br>
Bon app'<br>
Bottom-Dollar Markets<br>
B's Beverages<br>
Cactus Comidas para llevar<br>
Centro comercial Moctezuma<br>
Chop-suey Chinese<br>
Com&Atilde;&copy;rcio Mineiro<br>
Consolidated Holdings<br>
Drachenblut Delikatessen<br>
Du monde entier<br>
Eastern Connection<br>
Ernst Handel<br>
Familia Arquibaldo<br>
FISSA Fabrica Inter. Salchichas S.A.<br>
Folies gourmandes<br>
Folk och f&Atilde;&curren; HB<br>
Frankenversand<br>
France restauration<br>
Franchi S.p.A.<br>
Furia Bacalhau e Frutos do Mar<br>
Galer&Atilde;&shy;a del gastr&Atilde;&sup3;nomo<br>
Godos Cocina T&Atilde;&shy;pica<br>
Gourmet Lanchonetes<br>
Great Lakes Food Market<br>
GROSELLA-Restaurante<br>
Hanari Carnes<br>
HILARI&Atilde;&ldquo;N-Abastos<br>
Hungry Coyote Import Store<br>
Hungry Owl All-Night Grocers<br>
Island Trading<br>
K&Atilde;&para;niglich Essen<br>
La corne d'abondance<br>
La maison d'Asie<br>
Laughing Bacchus Wine Cellars<br>
Lazy K Kountry Store<br>
Lehmanns Marktstand<br>
Let's Stop N Shop<br>
LILA-Supermercado<br>
LINO-Delicateses<br>
Lonesome Pine Restaurant<br>
Magazzini Alimentari Riuniti<br>
Maison Dewey M&Atilde;&uml;re Paillarde<br>
Morgenstern Gesundkost<br>
North/South<br>
Oc&Atilde;&copy;ano Atl&Atilde;&iexcl;ntico Ltda.<br>
Old World Delicatessen<br>
Ottilies K&Atilde;&curren;seladen<br>
Paris sp&Atilde;&copy;cialit&Atilde;&copy;s<br>
Pericles Comidas cl&Atilde;&iexcl;sicas<br>
Piccolo und mehr<br>
Princesa Isabel Vinhos<br>
Que Del&Atilde;&shy;cia<br>
Queen Cozinha<br>
QUICK-Stop<br>
Rancho grande<br>
Rattlesnake Canyon Grocery<br>
Reggiani Caseifici<br>
Ricardo Adocicados<br>
Richter Supermarkt<br>
Romero y tomillo Sant&Atilde;&copy; Gourmet<br>
Save-a-lot Markets<br>
Seven Seas Imports<br>
Simons bistro<br>
Sp&Atilde;&copy;cialit&Atilde;&copy;s du monde<br>
Split Rail Beer &amp; Ale<br>
Supr&Atilde;&ordf;mes d&Atilde;&copy;lices<br>
The Big Cheese<br>
The Cracker Box<br>
Toms Spezialit&Atilde;&curren;ten<br>
Tortuga Restaurante<br>
Tradi&Atilde;&sect;&Atilde;&pound;o Hipermercados<br>
Trail's Head Gourmet Provisioners<br>
Vaffeljernet<br>
Victuailles en stock<br>
Vins et alcools Chevalier<br>
Die Wandernde Kuh<br>
Wartian Herkku<br>
Wellington Importadora<br>
White Clover Markets<br>
Wilman Kala<br>
Wolski Zajazd<br>
Chai<br>
Chang<br>
Aniseed Syrup<br>
Chef Anton's Cajun Seasoning<br>
Chef Anton's Gumbo Mix<br>
Grandma's Boysenberry Spread<br>
Uncle Bob's Organic Dried Pears<br>
Northwoods Cranberry Sauce<br>
Mishi Kobe Niku<br>
Ikura<br>
Queso Cabrales<br>
Queso Manchego La Pastora<br>
Konbu<br>
Tofu<br>
Genen Shouyu<br>
Pavlova<br>
Alice Mutton<br>
Carnarvon Tigers<br>
Teatime Chocolate Biscuits<br>
Sir Rodney's Marmalade&nbsp;<br>
Sir Rodney's Scones&nbsp;<br>
Gustaf's Kn&Atilde;&curren;ckebr&Atilde;&para;d&nbsp;<br>
Tunnbr&Atilde;&para;d&nbsp;<br>
Guaran&Atilde;&iexcl; Fant&Atilde;&iexcl;stica&nbsp;<br>
NuNuCa Nu&Atilde;&Yuml;-Nougat-Creme&nbsp;<br>
Gumb&Atilde;&curren;r Gummib&Atilde;&curren;rchen&nbsp;<br>
Schoggi Schokolade&nbsp;<br>
R&Atilde;&para;ssle Sauerkraut&nbsp;<br>
Th&Atilde;&frac14;ringer Rostbratwurst&nbsp;<br>
Nord-Ost Matjeshering&nbsp;<br>
Gorgonzola Telino&nbsp;<br>
Mascarpone Fabioli&nbsp;<br>
Geitost&nbsp;<br>
Sasquatch Ale&nbsp;<br>
Steeleye Stout&nbsp;<br>
Inlagd Sill&nbsp;<br>
Gravad lax&nbsp;<br>
C&Atilde;&acute;te de Blaye&nbsp;<br>
Chartreuse verte&nbsp;<br>
Boston Crab Meat&nbsp;<br>
Jack's New England Clam Chowder&nbsp;<br>
Singaporean Hokkien Fried Mee&nbsp;<br>
Ipoh Coffee&nbsp;<br>
Gula Malacca&nbsp;<br>
Rogede sild&nbsp;<br>
Spegesild&nbsp;<br>
Zaanse koeken&nbsp;<br>
Chocolade&nbsp;<br>
Maxilaku&nbsp;<br>
Valkoinen suklaa&nbsp;<br>
Manjimup Dried Apples&nbsp;<br>
Filo Mix&nbsp;<br>
Perth Pasties&nbsp;<br>
Tourti&Atilde;&uml;re&nbsp;<br>
P&Atilde;&cent;t&Atilde;&copy; chinois&nbsp;<br>
Gnocchi di nonna Alice&nbsp;<br>
Ravioli Angelo&nbsp;<br>
Escargots de Bourgogne&nbsp;<br>
Raclette Courdavault&nbsp;<br>
Camembert Pierrot&nbsp;<br>
Sirop d'&Atilde;&copy;rable&nbsp;<br>
Tarte au sucre&nbsp;<br>
Vegie-spread&nbsp;<br>
Wimmers gute Semmelkn&Atilde;&para;del&nbsp;<br>
Louisiana Fiery Hot Pepper Sauce&nbsp;<br>
Louisiana Hot Spiced Okra&nbsp;<br>
Laughing Lumberjack Lager&nbsp;<br>
Scottish Longbreads&nbsp;<br>
Gudbrandsdalsost&nbsp;<br>
Outback Lager&nbsp;<br>
Flotemysost&nbsp;<br>
Mozzarella di Giovanni&nbsp;<br>
R&Atilde;&para;d Kaviar&nbsp;<br>
Longlife Tofu&nbsp;<br>
Rh&Atilde;&para;nbr&Atilde;&curren;u Klosterbier&nbsp;<br>
Lakkalik&Atilde;&para;&Atilde;&para;ri&nbsp;<br>
Original Frankfurter gr&Atilde;&frac14;ne So&Atilde;&Yuml;e</code></p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="EqualAll1">EqualAll - 1</h2>
<span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span>
<p>This sample uses EqualAll to see if two sequences match on all elements in the same order.</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq96()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;wordsA&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;string[]&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__string">&quot;cherry&quot;</span>,&nbsp;<span class="js__string">&quot;apple&quot;</span>,&nbsp;<span class="js__string">&quot;blueberry&quot;</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;wordsB&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;string[]&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__string">&quot;cherry&quot;</span>,&nbsp;<span class="js__string">&quot;apple&quot;</span>,&nbsp;<span class="js__string">&quot;blueberry&quot;</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;bool&nbsp;match&nbsp;=&nbsp;wordsA.SequenceEqual(wordsB);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;The&nbsp;sequences&nbsp;match:&nbsp;{0}&quot;</span>,&nbsp;match);&nbsp;
<span class="js__brace">}</span>&nbsp;
</pre>
</div>
</div>
</div>
<h3 class="endscriptcode"><strong>Result</strong></h3>
<p>&nbsp;</p>
</div>
<p style="padding-left:30px"><code>The sequences match: True</code></p>
<p style="padding-left:30px">&nbsp;</p>
<h2 id="EqualAll2">EqualAll - 2</h2>
<p><span style="font-size:x-small">(<a href="#Introduction">back to top</a>)</span></p>
<p>This sample uses EqualAll to see if two sequences match on all elements in the same order.</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">public&nbsp;<span class="js__operator">void</span>&nbsp;Linq97()&nbsp;
<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;wordsA&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;string[]&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__string">&quot;cherry&quot;</span>,&nbsp;<span class="js__string">&quot;apple&quot;</span>,&nbsp;<span class="js__string">&quot;blueberry&quot;</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;wordsB&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;string[]&nbsp;<span class="js__brace">{</span>&nbsp;<span class="js__string">&quot;apple&quot;</span>,&nbsp;<span class="js__string">&quot;blueberry&quot;</span>,&nbsp;<span class="js__string">&quot;cherry&quot;</span>&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;bool&nbsp;match&nbsp;=&nbsp;wordsA.SequenceEqual(wordsB);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="js__string">&quot;The&nbsp;sequences&nbsp;match:&nbsp;{0}&quot;</span>,&nbsp;match);&nbsp;
<span class="js__brace">}</span>&nbsp;</pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<h3><strong>Result</strong></h3>
<p style="padding-left:30px"><code>The sequences match: False</code></p>
<p>&nbsp;</p>
<p><span style="font-size:20px; font-weight:bold">Source Code Files</span></p>
<ul>
<li><a class="browseFile" href="sourcecode?fileId=23903&pathId=1275732313">Program.cs</a>
</li></ul>
<h1><strong>101 LINQ Samples</strong></h1>
<ul>
<li><a href="../LINQ-Restriction-Operators-b15d29ca">Restriction Operators</a> </li><li><a href="../LINQ-to-DataSets-09787825">Projection Operators</a> </li><li><a href="../LINQ-Partitioning-Operators-c68aaccc">Partitioning Operators</a> </li><li><a href="../SQL-Ordering-Operators-050af19e">Ordering Operators</a> </li><li><a href="../LINQ-to-DataSets-Grouping-c62703ea">Grouping Operators</a> </li><li><a href="../LINQ-Set-Operators-374f34fe">Set Operators</a> </li><li><a href="../LINQ-Conversion-Operators-e4e59714">Conversion Operators</a> </li><li><a href="../LINQ-Element-Operators-0f3f12ce">Element Operators</a> </li><li><a href="../LINQ-Element-Operators-0f3f12ce">Generation Operators</a> </li><li><a href="../LINQ-Quantifiers-f00e7e3e">Quantifiers</a> </li><li><a href="../LINQ-Aggregate-Operators-c51b3869">Aggregate Operators</a> </li><li><a href="../LINQ-Miscellaneous-6b72bb2a">Miscellaneous Operators</a> </li><li><a href="../LINQ-to-DataSets-Custom-41738490">Custom Sequence Operators</a> </li><li><a href="../LINQ-Query-Execution-ce0d3b95">Query Execution</a> </li><li><a href="../LINQ-Join-Operators-dabef4e9">Join Operators</a> </li></ul>
<h1>More Information</h1>
<p>For more information, see:</p>
<ul>
<li>Basic LINQ Query Operations (C#) - <a href="http://msdn.microsoft.com/en-us/library/bb397927(v=VS.100).aspx" target="_blank">
http://msdn.microsoft.com/en-us/library/bb397927(v=VS.100).aspx</a> </li></ul>
