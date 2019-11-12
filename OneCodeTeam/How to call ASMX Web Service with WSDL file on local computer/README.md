# How to call ASMX Web Service with WSDL file on local computer
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- WCF
- Web Services
- .NET Framework
## Topics
- WSDL
## Updated
- 09/21/2016
## Description

<h2>はじめに</h2>
<p>このプロジェクトでは、ローカル コンピューター上で WSDL ファイルを使用して ASMX Web サービスを呼び出す方法を示します。</p>
<p>多くの開発者が MSDN フォーラムでこの点に関して質問するため、マイクロソフトは、よく質問されるプログラミングのシナリオに対処するためのコード サンプルを作成しました。</p>
<p><a href="http://forums.asp.net/p/1739187/4685087.aspx/1?Re&#43;how&#43;to&#43;delpoy&#43;this&#43;web&#43;service">http://forums.asp.net/p/1739187/4685087.aspx/1?Re&#43;how&#43;to&#43;delpoy&#43;this&#43;web&#43;service</a></p>
<p><a href="http://forums.asp.net/p/1729861/4641556.aspx/1?Re&#43;Webservice&#43;for&#43;oracle&#43;database&#43;lookup">http://forums.asp.net/p/1729861/4641556.aspx/1?Re&#43;Webservice&#43;for&#43;oracle&#43;database&#43;lookup</a></p>
<p>&nbsp;</p>
<h2>プロジェクトのビルド</h2>
<p>&nbsp;</p>
<p><strong>A.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </strong><strong>コンソール アプリケーションの作成 (CScallASMXlocalWsdl)</strong></p>
<ol>
<li>Visual Studio 2012 でコンソール アプリケーションを作成します </li><li>プロジェクトでソリューション エクスプローラーを開きます </li><li>右クリックして、[Web 参照の追加] を選択します </li><li>[Web 参照の追加] ポップ ウィンドウが表示されます </li><li>[URL] ボックスで、Web サイト アドレスを入力するのではなく、wsdl ファイルの正確な位置を入力し、また必ず拡張子も入力します。(例: D:\OneCode\CScallASMXlocalWsdl\Metadata\SampleWebService.wsdl)
</li><li>[移動] をクリックし、すべて問題なく実行されれば、Visual Studio では wsdl と内部の処理が認識されます。 </li><li>Web 参照の名前を変更することもできます。[参照の追加] ボタンをクリックします。 </li><li>[ソリューション エクスプローラー] ウィンドウに戻ると、[WebReference] ノードの下に wsdl ファイルが表示されていることが確認されます。これで、他の参照ファイルと同じ方法で利用できるようになります。
</li><li>次のコード スニペットを使用して Web メソッドを呼び出します。 </li><li>この例では、パラメーターを変更してアプリケーションを何回もテストすることを検討します。 </li></ol>
<p>&nbsp;</p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">using</span>&nbsp;(var&nbsp;webService&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;WebReference.WebService1())&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;result&nbsp;=&nbsp;webService.GetData(<span class="cs__number">10</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(result);&nbsp;
}</pre>
</div>
</div>
</div>
<p></p>
<h2>サンプルの実行</h2>
<p>&nbsp;</p>
<ol>
<li>ターゲット メソッドは GetData です。 </li><li>テスト メソッド </li></ol>
<ul>
<li>GetData() のパラメーターを変更する <strong>&nbsp;</strong><em>&nbsp;</em> </li></ul>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp">var&nbsp;result&nbsp;=&nbsp;webService.GetData(<span class="cs__number">10</span>);</pre>
</div>
</div>
</div>
<p></p>
<p>&nbsp;</p>
<ul>
<li>アプリケーションを実行する </li></ul>
<p>&nbsp;</p>
<p>操作によるサンプル データの共有:</p>
<p><img id="154380" alt="" src="154380-1.png" width="296" height="165"></p>
<p>&nbsp;</p>
<p><img id="154382" alt="" src="154382-image.png"></p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and
 reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.<span>Microsoft All-In-One
 Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and reduce developers' efforts in solving typical programming
 tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks, and allow developers to download them with a short sample
 publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</span><strong>&nbsp;</strong><em>&nbsp;</em></span></p>
<p><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p>&nbsp;</p>
<p>&nbsp;</p>
