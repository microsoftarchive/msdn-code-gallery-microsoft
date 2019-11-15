# Master-detail databinding in WPF (CSWPFMasterDetailBinding)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- WPF
## Topics
- Data Binding
## Updated
- 03/12/2017
## Description

<h1><span>Master-detail <span class="SpellE">databinding</span> in WPF (<span class="SpellE">CSWPFMasterDetailBinding</span>)
</span></h1>
<h2>Introduction</h2>
<p class="MsoNormal"><br>
<span>This example demonstrates how to do master/detail data binding in WPF. In this sample, two
<span class="SpellE">ListView</span> are used, one is master <span class="SpellE">
ListView</span> and another one is Detail <span class="SpellE">ListView</span>. The corresponding
<span class="GramE">class are</span> Customer and Order. The Customer class has a property of Orders which is an order list, bind a customer list to the master
<span class="SpellE">ListView</span>, and the Detail <span class="SpellE">ListView</span> bind to the order list of the selected item of master
<span class="SpellE">ListView</span> which is an instance of Customer class. So that when you select one customer in the master
<span class="SpellE">ListView</span>, the Detail <span class="SpellE">ListView</span> will show you the order list of that customer.</span></p>
<h2>Running the Sample</h2>
<p class="MsoNormal">&nbsp;</p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span><span>1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Build the sample project in Visual Studio.</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span><span>2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Start <span class="GramE">Without</span> Debugging, and the
<span class="SpellE">mian</span> window of the project will show.</p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span><span>3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>The window contains two <span class="SpellE">listview</span>, the first one shows the customer list, and the second shows the order lists of the selected customer.</p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span><span>4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Select one item of the customer list, the second <span class="SpellE">
listview</span> shows the order list of the selected customer.</p>
<p class="MsoNormal">You will see following result.</p>
<p class="MsoNormal"><span><img src="54093-image.png" alt="" width="658" height="408" align="middle">
</span><span>&nbsp;</span></p>
<h2>Using the Code<span> </span></h2>
<p class="MsoListParagraph" style="text-indent:-.25in"><span><span>1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span>Create Order class and Customer class, which <span class="GramE">
has</span> a master/detail relationship. Each customer has a customer ID and a name<span class="GramE">,<span>
</span>each</span> order also has a customer ID, so that we can get which customer the order belongs to. And in Customer,
<span class="GramE">there s<span> </span>a</span> </span><span class="SpellE"><span style="color:black">ObservableCollection</span></span><span> property Orders , we can get all the orders belong to the customer from this property.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">class</span>&nbsp;Customer&nbsp;:&nbsp;INotifyPropertyChanged&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;_id;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;_name;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;ObservableCollection&lt;Order&gt;&nbsp;_orders&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;ObservableCollection&lt;Order&gt;();&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;ID&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;_id;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">set</span>&nbsp;{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_id&nbsp;=&nbsp;<span class="cs__keyword">value</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OnPropertyChanged(<span class="cs__string">&quot;ID&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;Name&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;_name;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">set</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_name&nbsp;=&nbsp;<span class="cs__keyword">value</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OnPropertyChanged(<span class="cs__string">&quot;Name&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;ObservableCollection&lt;Order&gt;&nbsp;Orders&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;_orders;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}<span class="cs__preproc">&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;#region&nbsp;INotifyPropertyChanged&nbsp;Members</span>&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">event</span>&nbsp;PropertyChangedEventHandler&nbsp;PropertyChanged;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;OnPropertyChanged(<span class="cs__keyword">string</span>&nbsp;name)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(PropertyChanged&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;PropertyChanged(<span class="cs__keyword">this</span>,&nbsp;<span class="cs__keyword">new</span>&nbsp;PropertyChangedEventArgs(name));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}<span class="cs__preproc">&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>&nbsp;
}&nbsp;
&nbsp;
&nbsp;
<span class="cs__keyword">class</span>&nbsp;Order&nbsp;:&nbsp;INotifyPropertyChanged&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;_id;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;DateTime&nbsp;_date;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;_shipCity;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">int</span>&nbsp;ID&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;_id;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">set</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_id&nbsp;=&nbsp;<span class="cs__keyword">value</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OnPropertyChanged(<span class="cs__string">&quot;ID&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;DateTime&nbsp;Date&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;_date;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">set</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_date&nbsp;=&nbsp;<span class="cs__keyword">value</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OnPropertyChanged(<span class="cs__string">&quot;Date&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;ShipCity&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;_shipCity;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">set</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_shipCity&nbsp;=&nbsp;<span class="cs__keyword">value</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OnPropertyChanged(<span class="cs__string">&quot;ShipCity&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}<span class="cs__preproc">&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;#region&nbsp;INotifyPropertyChanged&nbsp;Members</span>&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">event</span>&nbsp;PropertyChangedEventHandler&nbsp;PropertyChanged;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;OnPropertyChanged(<span class="cs__keyword">string</span>&nbsp;name)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(PropertyChanged&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;PropertyChanged(<span class="cs__keyword">this</span>,&nbsp;<span class="cs__keyword">new</span>&nbsp;PropertyChangedEventArgs(name));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}<span class="cs__preproc">&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;#endregion</span>&nbsp;
}&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span>&nbsp;</span></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span><span>2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span>Create a <span class="SpellE">CustomerList</span> class, in its constructor add some customers, and add some orders for each customer, this class is used to provide a list of customers for binding.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">class</span>&nbsp;CustomerList&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;ObservableCollection&lt;Customer&gt;&nbsp;_customers;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;CustomerList()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_customers&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;ObservableCollection&lt;Customer&gt;();&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Insert&nbsp;customer&nbsp;and&nbsp;corresponding&nbsp;order&nbsp;information&nbsp;into</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Customer&nbsp;c&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Customer()&nbsp;{&nbsp;ID&nbsp;=&nbsp;<span class="cs__number">1</span>,&nbsp;Name&nbsp;=&nbsp;<span class="cs__string">&quot;Customer1&quot;</span>&nbsp;};&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;c.Orders.Add(<span class="cs__keyword">new</span>&nbsp;Order()&nbsp;{&nbsp;ID&nbsp;=&nbsp;<span class="cs__number">1</span>,&nbsp;Date&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DateTime(<span class="cs__number">2009</span>,&nbsp;<span class="cs__number">1</span>,&nbsp;<span class="cs__number">1</span>),&nbsp;ShipCity&nbsp;=&nbsp;<span class="cs__string">&quot;Shanghai&quot;</span>&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;c.Orders.Add(<span class="cs__keyword">new</span>&nbsp;Order()&nbsp;{&nbsp;ID&nbsp;=&nbsp;<span class="cs__number">1</span>,&nbsp;Date&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DateTime(<span class="cs__number">2009</span>,&nbsp;<span class="cs__number">2</span>,&nbsp;<span class="cs__number">1</span>),&nbsp;ShipCity&nbsp;=&nbsp;<span class="cs__string">&quot;Beijing&quot;</span>&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;c.Orders.Add(<span class="cs__keyword">new</span>&nbsp;Order()&nbsp;{&nbsp;ID&nbsp;=&nbsp;<span class="cs__number">1</span>,&nbsp;Date&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DateTime(<span class="cs__number">2009</span>,&nbsp;<span class="cs__number">11</span>,&nbsp;<span class="cs__number">10</span>),&nbsp;ShipCity&nbsp;=&nbsp;<span class="cs__string">&quot;Guangzhou&quot;</span>&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_customers.Add(c);&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;c&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Customer()&nbsp;{&nbsp;ID&nbsp;=&nbsp;<span class="cs__number">2</span>,&nbsp;Name&nbsp;=&nbsp;<span class="cs__string">&quot;Customer2&quot;</span>&nbsp;};&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;c.Orders.Add(<span class="cs__keyword">new</span>&nbsp;Order()&nbsp;{&nbsp;ID&nbsp;=&nbsp;<span class="cs__number">1</span>,&nbsp;Date&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DateTime(<span class="cs__number">2009</span>,&nbsp;<span class="cs__number">1</span>,&nbsp;<span class="cs__number">1</span>),&nbsp;ShipCity&nbsp;=&nbsp;<span class="cs__string">&quot;New&nbsp;York&quot;</span>&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;c.Orders.Add(<span class="cs__keyword">new</span>&nbsp;Order()&nbsp;{&nbsp;ID&nbsp;=&nbsp;<span class="cs__number">1</span>,&nbsp;Date&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DateTime(<span class="cs__number">2009</span>,&nbsp;<span class="cs__number">2</span>,&nbsp;<span class="cs__number">1</span>),&nbsp;ShipCity&nbsp;=&nbsp;<span class="cs__string">&quot;Seattle&quot;</span>&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_customers.Add(c);&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;c&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Customer()&nbsp;{&nbsp;ID&nbsp;=&nbsp;<span class="cs__number">3</span>,&nbsp;Name&nbsp;=&nbsp;<span class="cs__string">&quot;Customer3&quot;</span>&nbsp;};&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;c.Orders.Add(<span class="cs__keyword">new</span>&nbsp;Order()&nbsp;{&nbsp;ID&nbsp;=&nbsp;<span class="cs__number">1</span>,&nbsp;Date&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DateTime(<span class="cs__number">2009</span>,&nbsp;<span class="cs__number">1</span>,&nbsp;<span class="cs__number">1</span>),&nbsp;ShipCity&nbsp;=&nbsp;<span class="cs__string">&quot;Xiamen&quot;</span>&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;c.Orders.Add(<span class="cs__keyword">new</span>&nbsp;Order()&nbsp;{&nbsp;ID&nbsp;=&nbsp;<span class="cs__number">1</span>,&nbsp;Date&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DateTime(<span class="cs__number">2009</span>,&nbsp;<span class="cs__number">2</span>,&nbsp;<span class="cs__number">1</span>),&nbsp;ShipCity&nbsp;=&nbsp;<span class="cs__string">&quot;Shenzhen&quot;</span>&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;c.Orders.Add(<span class="cs__keyword">new</span>&nbsp;Order()&nbsp;{&nbsp;ID&nbsp;=&nbsp;<span class="cs__number">1</span>,&nbsp;Date&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DateTime(<span class="cs__number">2009</span>,&nbsp;<span class="cs__number">11</span>,&nbsp;<span class="cs__number">10</span>),&nbsp;ShipCity&nbsp;=&nbsp;<span class="cs__string">&quot;Tianjin&quot;</span>&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;c.Orders.Add(<span class="cs__keyword">new</span>&nbsp;Order()&nbsp;{&nbsp;ID&nbsp;=&nbsp;<span class="cs__number">1</span>,&nbsp;Date&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DateTime(<span class="cs__number">2009</span>,&nbsp;<span class="cs__number">11</span>,&nbsp;<span class="cs__number">10</span>),&nbsp;ShipCity&nbsp;=&nbsp;<span class="cs__string">&quot;Wuhan&quot;</span>&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;c.Orders.Add(<span class="cs__keyword">new</span>&nbsp;Order()&nbsp;{&nbsp;ID&nbsp;=&nbsp;<span class="cs__number">1</span>,&nbsp;Date&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DateTime(<span class="cs__number">2009</span>,&nbsp;<span class="cs__number">11</span>,&nbsp;<span class="cs__number">10</span>),&nbsp;ShipCity&nbsp;=&nbsp;<span class="cs__string">&quot;Jinan&quot;</span>&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_customers.Add(c);&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;c&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Customer()&nbsp;{&nbsp;ID&nbsp;=&nbsp;<span class="cs__number">4</span>,&nbsp;Name&nbsp;=&nbsp;<span class="cs__string">&quot;Customer4&quot;</span>&nbsp;};&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;c.Orders.Add(<span class="cs__keyword">new</span>&nbsp;Order()&nbsp;{&nbsp;ID&nbsp;=&nbsp;<span class="cs__number">1</span>,&nbsp;Date&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DateTime(<span class="cs__number">2009</span>,&nbsp;<span class="cs__number">1</span>,&nbsp;<span class="cs__number">1</span>),&nbsp;ShipCity&nbsp;=&nbsp;<span class="cs__string">&quot;Lanzhou&quot;</span>&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_customers.Add(c);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;ObservableCollection&lt;Customer&gt;&nbsp;Customers&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;_customers;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span>&nbsp;</span></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span><span>3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span>In <span class="SpellE">MainWindow.xaml</span> define the
<span class="SpellE">CustomerList</span> object in the window resources, so that we can use it as a static resource for binding.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XAML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xaml</span>

<div class="preview">
<pre class="xaml"><span class="xaml__tag_start">&lt;Window</span>.Resources<span class="xaml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__comment">&lt;!--&nbsp;Data&nbsp;Source&nbsp;For&nbsp;Binding--&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_start">&lt;local</span>:CustomerList&nbsp;x:<span class="xaml__attr_name">Key</span>=<span class="xaml__attr_value">&quot;CustomerList&quot;</span><span class="xaml__tag_start">/&gt;</span>&nbsp;
&lt;/Window.Resources&gt;&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span>&nbsp;</span></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span><span>4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span>Bind the <span class="SpellE">listViewCustomers s</span>
<span class="SpellE">ItemsSource</span> to the Customers property of <span class="SpellE">
CustomerList</span> class. </span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XAML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xaml</span>

<div class="preview">
<pre class="xaml"><span class="xaml__comment">&lt;!--&nbsp;This&nbsp;ListView&nbsp;displays&nbsp;the&nbsp;all&nbsp;the&nbsp;customer&nbsp;information&nbsp;--&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_start">&lt;ListView</span>&nbsp;Grid.<span class="xaml__attr_name">Row</span>=<span class="xaml__attr_value">&quot;1&quot;</span>&nbsp;<span class="xaml__attr_name">Name</span>=<span class="xaml__attr_value">&quot;listViewCustomers&quot;</span>&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__attr_name">ItemsSource</span>=<span class="xaml__attr_value">&quot;{Binding&nbsp;Source={StaticResource&nbsp;CustomerList},&nbsp;Path=Customers}&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__attr_name">SelectedIndex</span>=<span class="xaml__attr_value">&quot;0&quot;</span><span class="xaml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_start">&lt;ListView</span>.View<span class="xaml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_start">&lt;GridView</span><span class="xaml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_start">&lt;GridViewColumn</span>&nbsp;<span class="xaml__attr_name">Header</span>=<span class="xaml__attr_value">&quot;ID&quot;</span>&nbsp;<span class="xaml__attr_name">DisplayMemberBinding</span>=<span class="xaml__attr_value">&quot;{Binding&nbsp;ID}&quot;</span>&nbsp;<span class="xaml__attr_name">Width</span>=<span class="xaml__attr_value">&quot;50&quot;</span><span class="xaml__tag_start">/&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_start">&lt;GridViewColumn</span>&nbsp;<span class="xaml__attr_name">Header</span>=<span class="xaml__attr_value">&quot;Name&quot;</span>&nbsp;<span class="xaml__attr_name">DisplayMemberBinding</span>=<span class="xaml__attr_value">&quot;{Binding&nbsp;Name}&quot;</span>&nbsp;<span class="xaml__attr_name">Width</span>=<span class="xaml__attr_value">&quot;100&quot;</span><span class="xaml__tag_start">/&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_end">&lt;/GridView&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/ListView.View&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_end">&lt;/ListView&gt;</span>&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span>&nbsp;</span></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span><span>5.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span>Bind the <span class="SpellE">listViewOrders s</span>
<span class="SpellE">ItemsSource</span> to the Orders property of the selected customer object.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XAML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xaml</span>

<div class="preview">
<pre class="xaml"><span class="xaml__comment">&lt;!--&nbsp;This&nbsp;ListView&nbsp;displayd&nbsp;the&nbsp;corresponding&nbsp;order&nbsp;information&nbsp;for&nbsp;the&nbsp;selected&nbsp;customer&nbsp;in&nbsp;the&nbsp;customers&nbsp;ListView&nbsp;--&gt;</span>&nbsp;
<span class="xaml__comment">&lt;!--&nbsp;Put&nbsp;attention&nbsp;to&nbsp;the&nbsp;ItemSource&nbsp;property,&nbsp;its&nbsp;the&nbsp;key&nbsp;point&nbsp;of&nbsp;this&nbsp;kind&nbsp;of&nbsp;master/detail&nbsp;data&nbsp;binding&nbsp;--&gt;</span>&nbsp;
<span class="xaml__tag_start">&lt;ListView</span>&nbsp;Grid.<span class="xaml__attr_name">Row</span>=<span class="xaml__attr_value">&quot;3&quot;</span>&nbsp;<span class="xaml__attr_name">Name</span>=<span class="xaml__attr_value">&quot;listViewOrders&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__attr_name">ItemsSource</span>=<span class="xaml__attr_value">&quot;{Binding&nbsp;ElementName=listViewCustomers,&nbsp;Path=SelectedItem.Orders}&quot;</span><span class="xaml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_start">&lt;ListView</span>.View<span class="xaml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_start">&lt;GridView</span><span class="xaml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_start">&lt;GridViewColumn</span>&nbsp;<span class="xaml__attr_name">Header</span>=<span class="xaml__attr_value">&quot;ID&quot;</span>&nbsp;<span class="xaml__attr_name">DisplayMemberBinding</span>=<span class="xaml__attr_value">&quot;{Binding&nbsp;ID}&quot;</span>&nbsp;<span class="xaml__attr_name">Width</span>=<span class="xaml__attr_value">&quot;50&quot;</span><span class="xaml__tag_start">/&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_start">&lt;GridViewColumn</span>&nbsp;<span class="xaml__attr_name">Header</span>=<span class="xaml__attr_value">&quot;Date&quot;</span>&nbsp;<span class="xaml__attr_name">DisplayMemberBinding</span>=<span class="xaml__attr_value">&quot;{Binding&nbsp;Date}&quot;</span>&nbsp;<span class="xaml__attr_name">Width</span>=<span class="xaml__attr_value">&quot;auto&quot;</span><span class="xaml__tag_start">/&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_start">&lt;GridViewColumn</span>&nbsp;<span class="xaml__attr_name">Header</span>=<span class="xaml__attr_value">&quot;ShipCity&quot;</span>&nbsp;<span class="xaml__attr_name">DisplayMemberBinding</span>=<span class="xaml__attr_value">&quot;{Binding&nbsp;ShipCity}&quot;</span>&nbsp;<span class="xaml__attr_name">Width</span>=<span class="xaml__attr_value">&quot;auto&quot;</span><span class="xaml__tag_start">/&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_end">&lt;/GridView&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&lt;/ListView.View&gt;&nbsp;
<span class="xaml__tag_end">&lt;/ListView&gt;</span>&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
