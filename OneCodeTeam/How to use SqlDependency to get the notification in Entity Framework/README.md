# How to use SqlDependency to get the notification in Entity Framework
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- ADO.NET
- Data Access
- Entity Framework
- .NET Development
## Topics
- Entity Frameowork
- Sqldependency
- automatically update
## Updated
- 08/17/2016
## Description

<h1><em><img id="154893" src="154893-8171.onecodesampletopbanner.png" alt=""></em></h1>
<h1>如何在 Entity Framework 中透過 Sqldependency 自動更新資料 (CSEFAutoUpdate)</h1>
<h2>簡介</h2>
<p>我們可以在資料庫中使用 Sqldependency 在變更資料時取得通知，但 EF 沒有相同的功能。在此範例中，我們會示範如何在 Entity Framework 中透過 Sqldependency 自動更新。</p>
<p>在此範例中，我們會示範兩種使用 SqlDependency 取得變更通知以自動更新資料的方法：</p>
<p>1. 立即取得變更通知。</p>
<p>2. 定期取得變更通知。</p>
<h2>建置範例</h2>
<p>在執行範例前，您必須先完成下列步驟：</p>
<p>步驟 1.在 App.config-&gt; &lt;configuration&gt;-&gt; &lt;connectionStrings&gt; 中，將連接字串修改為您的 SQL Server 2008 資料庫執行個體。</p>
<h2>執行範例</h2>
<p>請按下 F5 以執行範例，結果如下所示。<em><br>
</em></p>
<h1><img id="154894" src="154894-image.png" alt=""></h1>
<p><strong>1. 立即更新 </strong></p>
<p>首先，輸入價&#26684;範圍以取得產品，或直接按一下 [<strong><em>Get Data</em></strong>] 按鈕以取得所有產品。 <strong>
&nbsp;</strong><em>&nbsp;</em></p>
<p><img id="154895" src="154895-82b91a16-c418-4a57-af98-2bf62e4feb6aimage.png" alt=""></p>
<p>第二，您可以按一下 DataGridView 中的儲存&#26684;以選取產品或在 [<strong><em>Product Id</em></strong>] 中輸入識別碼，然後輸入新價&#26684;。按一下 [<strong><em>Update</em></strong>] 按鈕後，系統便會更新 DataGridView 中的&#20540;。<strong>&nbsp;</strong><em></em></p>
<p><img id="154896" src="154896-d3753704-82e0-47b2-a1b3-c6d0bd8b2f66image.png" alt=""></p>
<p>最後，您可以按一下 [<strong><em>Stop</em></strong>] 按鈕以停止更新。</p>
<p><strong>2. 定期更新 </strong></p>
<p>首先，輸入價&#26684;範圍以取得產品，並輸入秒數以設定更新週期。<strong></strong><em></em></p>
<p><img id="154897" src="154897-05c139dc-87f7-454a-b6ad-37540692fdaeimage.png" alt=""></p>
<p>然後您可以輸入新價&#26684;並更新。週期結束時便會更新該&#20540;<strong>。&nbsp;</strong><em>&nbsp;</em></p>
<p><img id="154898" src="154898-9d9d014f-6bd6-4951-950d-63e183093159image.png" alt=""></p>
<h2>使用程式碼</h2>
<p>1. <strong>取得 ObjectQuery</strong></p>
<p>我們需要連接字串、命令字串和參數才能建立 SqlDependency，因此我們必須取得 ObjectQuery。如果我們使用 DbQuery 進行查詢，則會先將 DbQuery 轉換為 ObjectQuery。<strong></strong><em></em></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;ObjectQuery&nbsp;GetObjectQuery&lt;TEntity&gt;(DbContext&nbsp;context,&nbsp;IQueryable&nbsp;query)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;where&nbsp;TEntity&nbsp;:&nbsp;<span class="cs__keyword">class</span>&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(query&nbsp;<span class="cs__keyword">is</span>&nbsp;ObjectQuery)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;query&nbsp;<span class="cs__keyword">as</span>&nbsp;ObjectQuery;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(context&nbsp;==&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">throw</span>&nbsp;<span class="cs__keyword">new</span>&nbsp;ArgumentException(<span class="cs__string">&quot;Paramter&nbsp;cannot&nbsp;be&nbsp;null&quot;</span>,&nbsp;<span class="cs__string">&quot;context&quot;</span>);&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;使用&nbsp;DbContext&nbsp;建立&nbsp;ObjectContext</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ObjectContext&nbsp;objectContext&nbsp;=&nbsp;((IObjectContextAdapter)context).ObjectContext;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;使用&nbsp;DbSet&nbsp;建立&nbsp;ObjectSet&nbsp;並取得適合的提供者。</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;IQueryable&nbsp;iqueryable&nbsp;=&nbsp;objectContext.CreateObjectSet&lt;TEntity&gt;()&nbsp;<span class="cs__keyword">as</span>&nbsp;IQueryable;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;IQueryProvider&nbsp;provider&nbsp;=&nbsp;iqueryable.Provider;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;使用提供者和運算式建立&nbsp;ObjectQuery。</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;provider.CreateQuery(query.Expression)&nbsp;<span class="cs__keyword">as</span>&nbsp;ObjectQuery;&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<p>2. <strong>立即更新</strong></p>
<p>在啟動 SqlDependency 前，停止所有 SqlDependency。<strong></strong><em></em></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;BeginSqlDependency()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;SqlDependency.Stop(QueryExtension.GetConnectionString(oquery));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;SqlDependency.Start(QueryExtension.GetConnectionString(oquery));&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;RegisterSqlDependency();&nbsp;
}&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"></div>
<p>然後登錄 SqlDependency。</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;RegisterSqlDependency()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(command&nbsp;==&nbsp;<span class="cs__keyword">null</span>&nbsp;||&nbsp;connection&nbsp;==&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">throw</span>&nbsp;<span class="cs__keyword">new</span>&nbsp;ArgumentException(<span class="cs__string">&quot;command&nbsp;and&nbsp;connection&nbsp;cannot&nbsp;be&nbsp;null&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;確定命令物件沒有</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;與其相關的&nbsp;notification&nbsp;物件。</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;command.Notification&nbsp;=&nbsp;<span class="cs__keyword">null</span>;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;建立&nbsp;SqlDependency&nbsp;物件並將其繫結至&nbsp;command&nbsp;物件。</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;dependency&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlDependency(command);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;dependency.OnChange&nbsp;&#43;=&nbsp;<span class="cs__keyword">new</span>&nbsp;OnChangeEventHandler(DependencyOnChange);&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;登錄&nbsp;SqlDependency&nbsp;後必須執行&nbsp;SqlCommand，否則我們無法</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;取得通知。</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;RegisterSqlCommand();&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">變更資料時，將引發事件處理常式。<strong></strong><em></em></div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;DependencyOnChange(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;SqlNotificationEventArgs&nbsp;e)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;移動原始&nbsp;SqlDependency&nbsp;事件處理常式。</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;SqlDependency&nbsp;dependency&nbsp;=(SqlDependency)sender;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;dependency.OnChange&nbsp;-=&nbsp;DependencyOnChange;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(OnChanged&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OnChanged(<span class="cs__keyword">this</span>,<span class="cs__keyword">null</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;我們會重新登錄&nbsp;SqlDependency。</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;RegisterSqlDependency();&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<p>3. <strong>定期更新 </strong></p>
<p>登錄 SqlDependency 時，我們會建立 Threading.Timer 並設定委派、狀態、延遲時間、週期，然後執行該項目。<strong></strong><em></em></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;RegisterSqlDependency()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(connection&nbsp;==&nbsp;<span class="cs__keyword">null</span>&nbsp;||&nbsp;command&nbsp;==&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">throw</span>&nbsp;<span class="cs__keyword">new</span>&nbsp;ArgumentException(<span class="cs__string">&quot;command&nbsp;and&nbsp;connection&nbsp;cannot&nbsp;be&nbsp;null&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;確定命令物件沒有</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;與其相關的&nbsp;notification&nbsp;物件。</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;command.Notification&nbsp;=&nbsp;<span class="cs__keyword">null</span>;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;建立&nbsp;SqlDependency&nbsp;物件並將其繫結</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;至&nbsp;command&nbsp;物件。</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;dependency&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlDependency(command);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__string">&quot;Id&nbsp;of&nbsp;sqldependency:{0}&quot;</span>,&nbsp;dependency.Id);&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;RegisterSqlCommand();&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;timer&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Timer(CheckChange,&nbsp;<span class="cs__keyword">null</span>,&nbsp;<span class="cs__number">0</span>,&nbsp;interval);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;timer.Change(<span class="cs__number">0</span>,&nbsp;interval);&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<p>&nbsp;然後週期結束時，將引發委派。<strong></strong><em></em></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span><span class="cs__keyword">void</span>&nbsp;CheckChange(<span class="cs__keyword">object</span>&nbsp;state)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(dependency!=<span class="cs__keyword">null</span>&amp;&amp;dependency.HasChanges)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(OnChanged&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;OnChanged(<span class="cs__keyword">this</span>,&nbsp;<span class="cs__keyword">null</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<p><a title="自動產生 <a class="libraryLink" href="https://msdn.microsoft.com/zh-TW/library/System.Data.SqlClient.SqlDependency.aspx" target="_blank" title="Auto generated link to System.Data.SqlClient.SqlDependency">System.Data.SqlClient.SqlDependency</a> 連結" href="http://msdn.microsoft.com/query/dev11.query?appId=Dev11IDEF1&l=EN-US&k=k(&lt;a class=">System.Data.SqlClient.SqlDependency</a>);k(TargetFrameworkMoniker-.NETFramework,Version%3Dv4.5);k(DevLang-csharp)&amp;rd=true&quot;&gt;SqlDependency
 類別 (機器翻譯)</p>
<p><a href="http://msdn.microsoft.com/zh-tw/library/a52dhwx7.aspx">Windows 應用程式中的 SqlDependency (ADO.NET)</a></p>
<p><a href="http://msdn.microsoft.com/zh-tw/library/System.Threading.Timer.aspx">Timer 類別</a> (機器翻譯)<strong></strong><em></em></p>
<div class="endscriptcode"></div>
<p><img id="154899" src="154899-5218c418-5817-4d69-8983-6f1a197e7492image.png" alt=""><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and
 reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</span></p>
<p><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p><em><br>
</em></p>
<p><em>&nbsp;</em><strong></strong><em></em></p>
