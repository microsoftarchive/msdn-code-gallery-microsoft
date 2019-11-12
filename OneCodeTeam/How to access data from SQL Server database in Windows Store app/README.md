# How to access data from SQL Server database in Windows Store app
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Windows
- Windows 8
- App-Entwicklung für Windows
## Topics
- WinJS
- access data from SQL server
- Zugriff auf Daten aus SQL Server
## Updated
- 10/31/2016
## Description

<h1><em><img id="154741" src="154741-8171.onecodesampletopbanner.png" alt=""></em></h1>
<h1>如何從 Windows 市集應用程式中的 SQL Server 資料庫存取資料 (JSWindowsStoreAppAccessSQLServer)</h1>
<h2>簡介</h2>
<p>​此範例示範如何從 Windows 市集應用程式中的 SQL Server 資料庫存取資料。我們無法直接從 Windows 市集應用程式存取 SQL Server 資料庫。我們必須建立服務層才能存取資料庫。</p>
<h2>建置範例</h2>
<p>1.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;啟動 Visual Studio 2013 並選取 [檔案] &gt; [開啟] &gt; [專案/方案]。</p>
<p>2.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;移至您下載此範例的目錄。移至該目錄並為範例命名，然後按兩下 Microsoft Visual Studio 方案 (.sln) 檔案。</p>
<p>3.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;按下 F7 或使用 [建置] &gt; [建置方案] 以建置範例。</p>
<h2>執行範例 <img id="154742" src="154742-image.png" alt=""></h2>
<p><span>按一下 ['<span class="SpellE">GetData</span>' ] 按鈕從 Web 服務取得結果，您會看到下列結果。</span><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p><img id="154743" src="154743-41360e4b-1e59-4478-a02c-247c15ab4e48image.png" alt=""></p>
<h2>使用程式碼</h2>
<p>1. 使用 Visual Studio 2013 建立 Widnows 市集應用程式專案。</p>
<p>2. 將 WCF 服務應用程式專案新增至解決方案</p>
<p>3. 透過下列程式碼片段建立 WCF 服務。 </p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp">[ServiceContract]&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span><span class="cs__keyword">interface</span>&nbsp;IService&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;查詢資料</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[OperationContract]&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DataSet&nbsp;querySql();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
<span class="cs__keyword">public</span><span class="cs__keyword">class</span>&nbsp;Service&nbsp;:&nbsp;IService&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;summary&gt;</span><span class="cs__com">///&nbsp;查詢&nbsp;TestTable&nbsp;中的資料</span><span class="cs__com">///&nbsp;&lt;/summary&gt;</span><span class="cs__com">///&nbsp;&lt;returns&gt;&lt;/returns&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SqlConnection&nbsp;sqlCon&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlConnection(<span class="cs__string">&quot;Data&nbsp;Source=(local);Initial&nbsp;Catalog=Test;Integrated&nbsp;Security&nbsp;=SSPI;&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;DataSet&nbsp;querySql()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;sqlCon.Open();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;strSql&nbsp;=&nbsp;<span class="cs__string">&quot;select&nbsp;Title,&nbsp;Text&nbsp;from&nbsp;TestTable&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DataSet&nbsp;ds&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;DataSet();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SqlDataAdapter&nbsp;sqlDa&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;SqlDataAdapter(strSql,sqlCon);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;sqlDa.Fill(ds);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;ds;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">finally</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;sqlCon.Close();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"><span>4. 插入下列 JavaScript 程式碼片段以處理 [&quot;<span class="SpellE">GetData</span>&quot; ] 按鈕的 Click 事件。</span></div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">js</span>

<div class="preview">
<pre class="js"><span class="js__sl_comment">//&nbsp;[GetData]&nbsp;按鈕的事件處理常式</span><span class="js__operator">function</span>&nbsp;getDatabuttonclick()&nbsp;
&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span><span class="js__sl_comment">//&nbsp;清除錯誤訊息</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(<span class="js__string">'error'</span>).innerText&nbsp;=&nbsp;<span class="js__string">&quot;&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(<span class="js__string">'getdatabtn'</span>).style.setAttribute(<span class="js__string">&quot;disabled&quot;</span>,&nbsp;<span class="js__string">&quot;disabled&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;baseURI&nbsp;=&nbsp;<span class="js__string">&quot;http://localhost:42920/Service.svc/querySql&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;xmlDoc;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WinJS.xhr(<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;type:<span class="js__string">&quot;get&quot;</span>,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;url:&nbsp;baseURI&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>).then(<span class="js__operator">function</span>&nbsp;(response)&nbsp;<span class="js__brace">{</span><span class="js__statement">if</span>&nbsp;(<span class="js__function">eval</span>(<span class="js__string">'('</span>&nbsp;&#43;&nbsp;response.responseText&nbsp;&#43;&nbsp;<span class="js__string">')'</span>).queryParam&nbsp;==&nbsp;true)&nbsp;<span class="js__brace">{</span><span class="js__statement">var</span>&nbsp;items&nbsp;=&nbsp;[];&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;resulttxt&nbsp;=&nbsp;<span class="js__function">eval</span>(<span class="js__string">'('</span>&nbsp;&#43;&nbsp;response.responseText&nbsp;&#43;&nbsp;<span class="js__string">')'</span>).querySqlResult;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">if</span>&nbsp;(window.DOMParser)&nbsp;<span class="js__brace">{</span><span class="js__statement">var</span>&nbsp;parser&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;DOMParser();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;xmlDoc&nbsp;=&nbsp;parser.parseFromString(resulttxt,&nbsp;<span class="js__string">&quot;text/xml&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span><span class="js__statement">else</span><span class="js__brace">{</span><span class="js__sl_comment">//&nbsp;Internet&nbsp;Explorer</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;xmlDoc&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;ActiveXObject(<span class="js__string">&quot;Microsoft.XMLDOM&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;xmlDoc.async&nbsp;=&nbsp;false;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;xmlDoc.loadXML(resulttxt);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span><span class="js__statement">var</span>&nbsp;nodes&nbsp;=&nbsp;xmlDoc.querySelectorAll(<span class="js__string">&quot;Table&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">for</span>&nbsp;(<span class="js__statement">var</span>&nbsp;i&nbsp;=&nbsp;<span class="js__num">0</span>;&nbsp;i&nbsp;&lt;&nbsp;nodes.length;&nbsp;i&#43;&#43;)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span><span class="js__statement">var</span>&nbsp;item&nbsp;=<span class="js__operator">new</span><span class="js__object">Object</span>();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;item.Title=nodes[i].childNodes[<span class="js__num">0</span>].textContent;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;item.Text&nbsp;=nodes[i].childNodes[<span class="js__num">1</span>].textContent;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;items.push(item);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span><span class="js__statement">var</span>&nbsp;list&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;WinJS.Binding.List(items);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(<span class="js__string">'listView'</span>).winControl.itemDataSource&nbsp;=&nbsp;list.dataSource;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(<span class="js__string">'getdatabtn'</span>).removeAttribute(<span class="js__string">&quot;disabled&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span><span class="js__statement">else</span><span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;writeError(<span class="js__string">&quot;Error&nbsp;occurs.&nbsp;Please&nbsp;make&nbsp;sure&nbsp;the&nbsp;database&nbsp;has&nbsp;been&nbsp;attached&nbsp;to&nbsp;SQL&nbsp;Server!&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span><span class="js__brace">}</span>);&nbsp;
&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span></pre>
</div>
</div>
</div>
</div>
<p></p>
<h2>其他相關資訊</h2>
<p>&nbsp;parseFromString 方法：</p>
<p><a href="http://msdn.microsoft.com/zh-tw/library/windows/apps/hh770806.aspx">http://msdn.microsoft.com/zh-tw/library/windows/apps/hh770806.aspx</a> (英文)</p>
<p>WinJS.UI.ListView 物件：</p>
<p><a href="http://msdn.microsoft.com/zh-tw/library/windows/apps/br211837.aspx#properties">http://msdn.microsoft.com/zh-tw/library/windows/apps/br211837.aspx#properties</a> (英文)<strong>&nbsp;</strong><em>&nbsp;</em></p>
<p></p>
<div class="endscriptcode"><img id="154744" src="154744-534781ff-b8c9-4f5a-85ef-4e8cc09ddc5bimage.png" alt=""></div>
<div class="endscriptcode"></div>
<p></p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and
 reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</span></p>
<p><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p><em><br>
</em></p>
