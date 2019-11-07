# How to get WCF service contract programmatically from WSDL of the service
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- WCF
- .NET Framework
- Services
## Topics
- WCF
## Updated
- 09/21/2016
## Description

<h1><em><img id="154431" src="154431-8171.onecodesampletopbanner.png" alt=""></em></h1>
<h1>このプロジェクトは、サービスの WSDL から WCF サービス契約をプログラムにより取得する方法を示します</h1>
<h2>はじめに</h2>
<p>このプロジェクトは、サービスの WSDL を読み取ることで WCF サービス契約 (インターフェイス) をプログラムにより取得する方法を示します。</p>
<p>多くの開発者が MSDN フォーラムでこの点に関して質問するため、マイクロソフトは、よく質問されるプログラミングのシナリオに対処するためのコード サンプルを作成しました。</p>
<h2>サンプルの実行</h2>
<p>コードを実行すると、サービスの WSDL のアドレスを入力するよう求めるメッセージが表示されます。アドレスを入力すると、サービス契約名が出力されます。&nbsp;<img id="154432" src="154432-image001.png" alt="" width="677" height="343"></p>
<h2>コードの使用</h2>
<p>手順 1: Visual Studio でコンソール アプリケーションを作成します。</p>
<p>手順 2. <a title="System.Web.Services.Description への自動生成されたリンク" class="libraryLink" href="https://code.msdn.microsoft.com/The-illustrates-how-to-get-8adc4264/https://code.msdn.microsoft.com/The-illustrates-how-to-get-8adc4264/https://msdn.microsoft.com/en-US/library/System.Web.Services.Description.aspx">
System.Web.Services.Description</a> 名前空間を使用していることを確認します。</p>
<p>手順 3: このプロジェクトのこの場所では、WebRequest および HttpWebResponse オブジェクトを使用してサービス WSDL への要求を作成します。
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">//XML リーダーまたはストリーム リーダーを介して WSDL を読み取り、ノード ポートの種類を特定する

System.Web.Services.Description.ServiceDescription wsdl = new System.Web.Services.Description.ServiceDescription();
            wsdl = ServiceDescription.Read(reader);

            foreach (PortType pt in wsdl.PortTypes)
            {
                Console.WriteLine(&quot;ServiceContract : {0}&quot;, pt.Name);
                Console.ReadLine();
            }
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__com">//XML&nbsp;リーダーまたはストリーム&nbsp;リーダーを介して&nbsp;WSDL&nbsp;を読み取り、ノード&nbsp;ポートの種類を特定する</span>&nbsp;
&nbsp;
System.Web.Services.Description.ServiceDescription&nbsp;wsdl&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;System.Web.Services.Description.ServiceDescription();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;wsdl&nbsp;=&nbsp;ServiceDescription.Read(reader);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">foreach</span>&nbsp;(PortType&nbsp;pt&nbsp;<span class="cs__keyword">in</span>&nbsp;wsdl.PortTypes)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(<span class="cs__string">&quot;ServiceContract&nbsp;:&nbsp;{0}&quot;</span>,&nbsp;pt.Name);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.ReadLine();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
</pre>
</div>
</div>
</div>
<p></p>
<h2>詳細</h2>
<p><a title="System.Web.Services.Description への自動生成されたリンク" class="libraryLink" href="https://code.msdn.microsoft.com/The-illustrates-how-to-get-8adc4264/https://code.msdn.microsoft.com/The-illustrates-how-to-get-8adc4264/https://msdn.microsoft.com/en-US/library/System.Web.Services.Description.aspx">System.Web.Services.Description</a></p>
<p><a href="http://msdn.microsoft.com/en-us/library/system.web.services.description(v=vs.110).aspx">http://msdn.microsoft.com/en-us/library/system.web.services.description(v=vs.110).aspx</a></p>
<p>WebRequest クラス</p>
<p><a href="http://msdn.microsoft.com/en-us/library/system.net.webrequest(v=vs.110).aspx">http://msdn.microsoft.com/en-us/library/system.net.webrequest(v=vs.110).aspx</a></p>
<p>HttpWebResponse クラス</p>
<p><a href="http://msdn.microsoft.com/en-us/library/system.net.httpwebresponse(v=vs.110).aspx">http://msdn.microsoft.com/en-us/library/system.net.httpwebresponse(v=vs.110).aspx</a></p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and
 reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</span></p>
<p><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p><img id="154433" src="154433-image.png" alt=""></p>
<p><em><br>
</em></p>
