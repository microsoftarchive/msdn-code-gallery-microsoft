# How to Query in a DataSet with Multiple DataTables
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- ADO.NET
- Data Access
- .NET Development
## Topics
- query dataset
- expression
## Updated
- 09/21/2016
## Description

<h1><em><img id="154785" src="154785-8171.onecodesampletopbanner.png" alt=""></em></h1>
<p><span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">複数の DataTable を使用して DataSet をクエリする方法</span><span style="font-weight:bold; font-size:14pt">&nbsp;</span><span style="font-weight:bold; font-size:14pt">&nbsp;</span><span style="font-weight:bold; font-size:14pt">&nbsp;</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">はじめに</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">データソースからデータを取得するには通常の SQL クエリを使用できますが、DataSet 内では SQL クエリを使用できません。</span><span style="font-size:11pt">このサンプルでは、DataSet 内で式を使用してクエリを行なう方法について説明します。</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">このサンプルでは、DataSet 内で式を使用してクエリを行なう方法について説明します。</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">1. 2 つの DataTable を持つ DataSet を作成します。</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">2. この 2 つのテーブル間に制約を作成します。</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">3. </span><a href="http://msdn.microsoft.com/ja-jp/library/way3dy9w.aspx" style="text-decoration:none"><span>DataTable.Select</span><span> メソッド</span></a><span style="font-size:11pt">を使用してテーブルから行を取得します。</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">4. </span><a href="http://msdn.microsoft.com/ja-jp/library/system.data.datatable.compute.aspx" style="text-decoration:none"><span>DataTable.Compute</span><span> メソッド</span></a><span style="font-size:11pt">を使用して、指定した行を計算します。</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">5. 前述のメソッドの中で</span><a href="http://msdn.microsoft.com/ja-jp/library/system.data.datacolumn.expression.aspx" style="text-decoration:none"><span>式</span></a><span style="font-size:11pt">を使用してクエリを行ないます。</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">サンプルの実行</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">F5 キーを押してサンプルを実行します。</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">アプリケーションによって、まず、データを&#26684;納する 3 つのテーブルが表示されます。</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">SalesPerson</span><span style="font-size:11pt"> テーブル:</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<img id="154786" src="154786-image.png" alt="" width="642" height="57"></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Order テーブル:</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<img id="154787" src="154787-0a30ee57-4646-4fa7-a661-48d53d652e2dimage.png" alt="" width="646" height="147"></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">OrderDetail</span><span style="font-size:11pt"> テーブル:</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<img id="154788" src="154788-7c23f753-0483-4f3a-86e3-9ae12cbb3d31image.png" alt="" width="643" height="207"></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span><span>次に、SalesPerson テーブルの複数の Territory を基にして Order テーブルから行を選択した後、合計結果を計算します。</span></span>
<strong></strong><em></em></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<img id="154789" src="154789-2d072d4e-a20d-4eca-9f03-c078511949fdimage.png" alt=""></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span><span>最後に、OrderDetail テーブルから行を選択し、Bike の売り上げ情報をすべて取得します。</span></span> <strong>
</strong><em></em></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<img id="154790" src="154790-7c23f753-0483-4f3a-86e3-9ae12cbb3d31image.png" alt=""></p>
<p><strong></strong><em></em></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">6 番目の行は Helmet 製品で、今は表示できません。</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">コードの使用</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">1. テーブル間に制約を定義します。</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>


<div class="preview">
<pre class="vb">salesSet.Relations.Add(<span class="visualBasic__string">&quot;OrderOrderDetail&quot;</span>,&nbsp;orderTable.Columns(<span class="visualBasic__string">&quot;OrderId&quot;</span>),&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;orderDetailTable.Columns(<span class="visualBasic__string">&quot;OrderId&quot;</span>),&nbsp;<span class="visualBasic__keyword">True</span>)&nbsp;
salesSet.Relations.Add(<span class="visualBasic__string">&quot;SalesPersonOrder&quot;</span>,&nbsp;salesPersonTable.Columns(<span class="visualBasic__string">&quot;PersonId&quot;</span>),&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;orderTable.Columns(<span class="visualBasic__string">&quot;SalesPerson&quot;</span>),&nbsp;<span class="visualBasic__keyword">True</span>)</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span style="font-size:11pt"><span style="font-size:11pt">2. 式列を作成します。</span></span></div>
<p></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">2 つ以上の列で演算を実行する必要がある場合は、DataTable.Compute メソッドを使用するのではなく、</span><a href="http://msdn.microsoft.com/ja-jp/library/system.data.datacolumn.aspx"><span style="font-size:11pt">DataColumn</span></a><span style="font-size:11pt">
 を作成し、その Expression プロパティに該当する式を設定してください。</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>


<div class="preview">
<pre class="vb"><span class="visualBasic__com">'&nbsp;子テーブルの列で集計&nbsp;Sum&nbsp;を使用して結果を取得します。</span>&nbsp;
<span class="visualBasic__keyword">Dim</span>&nbsp;colSub&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;DataColumn(<span class="visualBasic__string">&quot;SubTotal&quot;</span>,&nbsp;<span class="visualBasic__keyword">GetType</span>(<span class="visualBasic__keyword">Decimal</span>),&nbsp;<span class="visualBasic__string">&quot;Sum(Child.LineTotal)&quot;</span>)&nbsp;
orderTable.Columns.Add(colSub)&nbsp;
<span class="visualBasic__com">'&nbsp;SubTotal&nbsp;式列を参照して税を計算します。</span>&nbsp;
<span class="visualBasic__keyword">Dim</span>&nbsp;colTax&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;DataColumn(<span class="visualBasic__string">&quot;Tax&quot;</span>,&nbsp;<span class="visualBasic__keyword">GetType</span>(<span class="visualBasic__keyword">Decimal</span>),&nbsp;<span class="visualBasic__string">&quot;SubTotal*0.1&quot;</span>)&nbsp;
orderTable.Columns.Add(colTax)&nbsp;
<span class="visualBasic__com">'&nbsp;OrderId&nbsp;が&nbsp;&quot;Total&quot;&nbsp;の場合はすべての注文の支払い金額を計算し、それ以外の場合はこの注文の金額を計算します。</span>&nbsp;
<span class="visualBasic__keyword">Dim</span>&nbsp;colTotal&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;DataColumn(<span class="visualBasic__string">&quot;TotalDue&quot;</span>,&nbsp;<span class="visualBasic__keyword">GetType</span>(<span class="visualBasic__keyword">Decimal</span>),&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__string">&quot;IIF(OrderId='Total',Sum(SubTotal)&#43;Sum(Tax),SubTotal&#43;Tax)&quot;</span>)&nbsp;
orderTable.Columns.Add(colTotal)&nbsp;
<span class="visualBasic__keyword">Dim</span>&nbsp;totalRow&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;DataRow&nbsp;=&nbsp;orderTable.NewRow()&nbsp;
totalRow(<span class="visualBasic__string">&quot;OrderId&quot;</span>)&nbsp;=&nbsp;<span class="visualBasic__string">&quot;Total&quot;</span>&nbsp;
orderTable.Rows.Add(totalRow)</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span style="font-size:11pt"><span style="font-size:11pt">3. Select メソッドと Compute メソッドを使用します。</span></span></div>
<p></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">SalesPerson テーブルの Territory を基にして、Select メソッドを使用して行を取得します。</span><span style="font-size:11pt">また、Compute メソッドを使用して合計結果を取得します。</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>


<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Dim</span>&nbsp;territories()&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;=&nbsp;{<span class="visualBasic__string">&quot;Europe&quot;</span>,&nbsp;<span class="visualBasic__string">&quot;North&nbsp;America&quot;</span>}&nbsp;
Console.WriteLine(<span class="visualBasic__string">&quot;Following&nbsp;is&nbsp;the&nbsp;sales&nbsp;information&nbsp;for&nbsp;every&nbsp;territories.&quot;</span>)&nbsp;
<span class="visualBasic__keyword">For</span>&nbsp;<span class="visualBasic__keyword">Each</span>&nbsp;territory&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;<span class="visualBasic__keyword">In</span>&nbsp;territories&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;expression&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>&nbsp;=&nbsp;<span class="visualBasic__keyword">String</span>.Format(<span class="visualBasic__string">&quot;Parent.Territory='{0}'&quot;</span>,&nbsp;territory)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;total&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Object</span>&nbsp;=&nbsp;orderTable.Compute(<span class="visualBasic__string">&quot;Sum(TotalDue)&quot;</span>,&nbsp;expression)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="visualBasic__string">&quot;Sales&nbsp;information&nbsp;in&nbsp;{0}(Total:{1:C}):&quot;</span>,&nbsp;territory,&nbsp;total)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;territoryRows()&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;DataRow&nbsp;=&nbsp;orderTable.<span class="visualBasic__keyword">Select</span>(expression)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ShowRows(territoryRows)&nbsp;
<span class="visualBasic__keyword">Next</span>&nbsp;territory&nbsp;
</pre>
</div>
</div>
</div>
<p></p>
<div class="endscriptcode">&nbsp;<span style="font-size:11pt"><span style="font-size:11pt">4. Select メソッドと Like を使用します。</span></span>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Select メソッドと Like を使用してすべての Bike の売り上げ情報を取得します。</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span><span class="hidden">csharp</span>


<div class="preview">
<pre class="vb">Console.WriteLine(<span class="visualBasic__string">&quot;Following&nbsp;is&nbsp;the&nbsp;sales&nbsp;information&nbsp;for&nbsp;all&nbsp;the&nbsp;bikes.&quot;</span>)&nbsp;
<span class="visualBasic__keyword">Dim</span>&nbsp;bikeRows()&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;DataRow&nbsp;=&nbsp;orderDetailTable.<span class="visualBasic__keyword">Select</span>(<span class="visualBasic__string">&quot;Product&nbsp;like&nbsp;'*Bike'&quot;</span>)&nbsp;
ShowRows(bikeRows)&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">詳細</span></span></div>
<p></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="http://msdn.microsoft.com/ja-jp/library/way3dy9w.aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">DataTable.Select</span><span style="color:#0563c1; text-decoration:underline"> メソッド
 (String, String)</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="http://msdn.microsoft.com/ja-jp/library/system.data.datatable.compute.aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">DataTable.Compute</span><span style="color:#0563c1; text-decoration:underline">
 メソッド</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="http://msdn.microsoft.com/ja-jp/library/system.data.datacolumn.expression.aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">DataColumn.Expression</span><span style="color:#0563c1; text-decoration:underline">
 プロパティ</span></a></span> <strong></strong><em></em></p>
</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<strong></strong><img id="154791" src="154791-b08b4c3c-eea7-41ca-bd6a-b65c9a20da04image.png" alt=""></p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and
 reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</span></p>
<p><strong></strong><em></em></p>
