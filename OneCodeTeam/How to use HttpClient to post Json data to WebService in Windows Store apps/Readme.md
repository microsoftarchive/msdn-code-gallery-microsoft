# How to use HttpClient to post Json data to WebService in Windows Store apps
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Windows
- Windows 8
- Windows Store app Development
## Topics
- JSON
- HttpClient
## Updated
- 09/21/2016
## Description

<h1><em><img id="154982" src="154982-8171.onecodesampletopbanner.png" alt=""></em></h1>
<h1>Windows ストア アプリで HttpClient を使用して JSON データを WebService に送信する方法 (CSWindowsStoreAppHttpClientPostJson)</h1>
<h2>はじめに</h2>
<p>​サンプルでは、HttpClient および DataContractJsonSerializer&nbsp;クラスを使用して、JSON データを Web サービスに送信する方法を示します。これは、&nbsp;WinJS 領域で行うと簡単に実現できます。&nbsp;しかし、.NET アプリケーション用に HttpClient を使用してこれを行う方法を示す例はありません。</p>
<h2>サンプルのビルド</h2>
<p>1.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Visual Studio 2012 を起動し、[ファイル]、[開く]、[プロジェクト/ソリューション] の順に選択します。</p>
<p>2.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;サンプルをダウンロードしたディレクトリに移動します。サンプル用に名前を付けたディレクトリに移動し、Microsoft Visual Studio ソリューション (.sln) ファイルをダブルクリックします。</p>
<p>3.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;F7 キーを押すか、[ビルド]、[ソリューションのビルド] の順に選択して、サンプルをビルドします。</p>
<h2>サンプルの実行</h2>
<p>1.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&quot;JSONWCFService&quot; プロジェクトを右クリックし、[View in Browser (Internet Explorer)] をクリックして、JSON Web Service を最初に実行します。<img id="154983" src="154983-image.png" alt="" width="576" height="324"></p>
<p>4.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[スタート] ボタンをクリックし、Web サービスから結果を取得します。下の結果が表示されます。<img id="154984" src="154984-e1533caf-7040-45bb-8ce5-d56a65c05264image.png" alt=""></p>
<p><strong>&nbsp;</strong><em>&nbsp;</em></p>
<h2>コードの使用</h2>
<p>1. Visual Studio 2012 を使用して Widnows ストア アプリ プロジェクトを作成します。</p>
<p>2. WCF サービス アプリケーション プロジェクトをソリューション内に追加します</p>
<p>3. Web サービスを作成します。</p>
<p>4. Web サービスの構成ファイルを構成します。 </p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xml</span>
<pre class="hidden">&lt;system.serviceModel&gt;
    &lt;!--サービスを追加する--&gt;
  &lt;services&gt;
      &lt;service name=&quot;JSONWCFService.WCFService&quot; behaviorConfiguration=&quot;ServiceBehaviour&quot;&gt;
        &lt;endpoint name=&quot;JsonEndPoint&quot; contract=&quot;JSONWCFService.IWCFService&quot; binding=&quot;webHttpBinding&quot; behaviorConfiguration=&quot;jsonbehavior&quot;/&gt;
      &lt;/service&gt;
    &lt;/services&gt;
    
    &lt;behaviors&gt;
      &lt;serviceBehaviors&gt;
        &lt;behavior name=&quot;ServiceBehaviour&quot;&gt;
          &lt;!-- メタデータ情報の開示を避けるには、展開する前に下の値を false に設定する --&gt;
          &lt;serviceMetadata httpGetEnabled=&quot;true&quot; httpsGetEnabled=&quot;true&quot;/&gt;
          &lt;!-- デバッグのためエラー時に例外の詳細を取得するには、下の値を true に設定する 意図しない例外情報が開示されてしまうのを避けるには、配置前に false に設定する --&gt;
          &lt;serviceDebug includeExceptionDetailInFaults=&quot;false&quot;/&gt;
        &lt;/behavior&gt;
      &lt;/serviceBehaviors&gt;
      &lt;!--エンドポイントの動作を追加する--&gt;
      &lt;endpointBehaviors&gt;
        &lt;behavior name=&quot;jsonbehavior&quot;&gt;
          &lt;webHttp defaultBodyStyle=&quot;Wrapped&quot; defaultOutgoingResponseFormat=&quot;Json&quot;/&gt;
        &lt;/behavior&gt;
      &lt;/endpointBehaviors&gt;
    &lt;/behaviors&gt;
    
    &lt;protocolMapping&gt;
        &lt;add binding=&quot;basicHttpsBinding&quot; scheme=&quot;https&quot; /&gt;
    &lt;/protocolMapping&gt;    
    &lt;serviceHostingEnvironment aspNetCompatibilityEnabled=&quot;true&quot; multipleSiteBindingsEnabled=&quot;true&quot; /&gt;
  &lt;/system.serviceModel&gt;
</pre>
<div class="preview">
<pre class="xml"><span class="xml__tag_start">&lt;system</span>.serviceModel<span class="xml__tag_start">&gt;&nbsp;
</span><span class="xml__comment">&lt;!--サービスを追加する--&gt;</span><span class="xml__tag_start">&lt;services</span><span class="xml__tag_start">&gt;&nbsp;
</span><span class="xml__tag_start">&lt;service</span><span class="xml__attr_name">name</span>=<span class="xml__attr_value">&quot;JSONWCFService.WCFService&quot;</span><span class="xml__attr_name">behaviorConfiguration</span>=<span class="xml__attr_value">&quot;ServiceBehaviour&quot;</span><span class="xml__tag_start">&gt;&nbsp;
</span><span class="xml__tag_start">&lt;endpoint</span><span class="xml__attr_name">name</span>=<span class="xml__attr_value">&quot;JsonEndPoint&quot;</span><span class="xml__attr_name">contract</span>=<span class="xml__attr_value">&quot;JSONWCFService.IWCFService&quot;</span><span class="xml__attr_name">binding</span>=<span class="xml__attr_value">&quot;webHttpBinding&quot;</span><span class="xml__attr_name">behaviorConfiguration</span>=<span class="xml__attr_value">&quot;jsonbehavior&quot;</span><span class="xml__tag_start">/&gt;</span><span class="xml__tag_end">&lt;/service&gt;</span><span class="xml__tag_end">&lt;/services&gt;</span><span class="xml__tag_start">&lt;behaviors</span><span class="xml__tag_start">&gt;&nbsp;
</span><span class="xml__tag_start">&lt;serviceBehaviors</span><span class="xml__tag_start">&gt;&nbsp;
</span><span class="xml__tag_start">&lt;behavior</span><span class="xml__attr_name">name</span>=<span class="xml__attr_value">&quot;ServiceBehaviour&quot;</span><span class="xml__tag_start">&gt;&nbsp;
</span><span class="xml__comment">&lt;!--&nbsp;メタデータ情報の開示を避けるには、展開する前に下の値を&nbsp;false&nbsp;に設定する&nbsp;--&gt;</span><span class="xml__tag_start">&lt;serviceMetadata</span><span class="xml__attr_name">httpGetEnabled</span>=<span class="xml__attr_value">&quot;true&quot;</span><span class="xml__attr_name">httpsGetEnabled</span>=<span class="xml__attr_value">&quot;true&quot;</span><span class="xml__tag_start">/&gt;</span><span class="xml__comment">&lt;!--&nbsp;デバッグのためエラー時に例外の詳細を取得するには、下の値を&nbsp;true&nbsp;に設定する&nbsp;意図しない例外情報が開示されてしまうのを避けるには、配置前に&nbsp;false&nbsp;に設定する&nbsp;--&gt;</span><span class="xml__tag_start">&lt;serviceDebug</span><span class="xml__attr_name">includeExceptionDetailInFaults</span>=<span class="xml__attr_value">&quot;false&quot;</span><span class="xml__tag_start">/&gt;</span><span class="xml__tag_end">&lt;/behavior&gt;</span><span class="xml__tag_end">&lt;/serviceBehaviors&gt;</span><span class="xml__comment">&lt;!--エンドポイントの動作を追加する--&gt;</span><span class="xml__tag_start">&lt;endpointBehaviors</span><span class="xml__tag_start">&gt;&nbsp;
</span><span class="xml__tag_start">&lt;behavior</span><span class="xml__attr_name">name</span>=<span class="xml__attr_value">&quot;jsonbehavior&quot;</span><span class="xml__tag_start">&gt;&nbsp;
</span><span class="xml__tag_start">&lt;webHttp</span><span class="xml__attr_name">defaultBodyStyle</span>=<span class="xml__attr_value">&quot;Wrapped&quot;</span><span class="xml__attr_name">defaultOutgoingResponseFormat</span>=<span class="xml__attr_value">&quot;Json&quot;</span><span class="xml__tag_start">/&gt;</span><span class="xml__tag_end">&lt;/behavior&gt;</span><span class="xml__tag_end">&lt;/endpointBehaviors&gt;</span><span class="xml__tag_end">&lt;/behaviors&gt;</span><span class="xml__tag_start">&lt;protocolMapping</span><span class="xml__tag_start">&gt;&nbsp;
</span><span class="xml__tag_start">&lt;add</span><span class="xml__attr_name">binding</span>=<span class="xml__attr_value">&quot;basicHttpsBinding&quot;</span><span class="xml__attr_name">scheme</span>=<span class="xml__attr_value">&quot;https&quot;</span><span class="xml__tag_start">/&gt;</span><span class="xml__tag_end">&lt;/protocolMapping&gt;</span><span class="xml__tag_start">&lt;serviceHostingEnvironment</span><span class="xml__attr_name">aspNetCompatibilityEnabled</span>=<span class="xml__attr_value">&quot;true&quot;</span><span class="xml__attr_name">multipleSiteBindingsEnabled</span>=<span class="xml__attr_value">&quot;true&quot;</span><span class="xml__tag_start">/&gt;</span>&nbsp;
&nbsp;&nbsp;&lt;/system.serviceModel&gt;&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"><span>5.<span>&nbsp; </span><span class="SpellE">HttpClient</span> クラスを使用して
<span class="SpellE">json</span> データを Web サービスに送信し、Windows ストア アプリでコールバックの結果を取得します。</span></div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">/// &lt;summary&gt;
       /// Call WCF サービスを開始する
       /// &lt;/summary&gt;
       /// &lt;param name=&quot;sender&quot;&gt;&lt;/param&gt;
       /// &lt;param name=&quot;e&quot;&gt;&lt;/param&gt;
       private async void Start_Click(object sender, RoutedEventArgs e)
       {
           // Output テキスト ボックスのテキストをクリアする 
           this.OutputField.Text = string.Empty;
           this.StatusBlock.Text = string.Empty;


           this.StartButton.IsEnabled = false;
           httpClient = new HttpClient();
           try
           {
               string resourceAddress = &quot;http://localhost:44516/WCFService.svc/GetData&quot;;
               int age = Convert.ToInt32(this.Agetxt.Text);
               if (age &gt; 120 || age &lt; 0)
               {
                   throw new Exception(&quot;Age must be between 0 and 120&quot;);
               }
               Person p = new Person { Name = this.Nametxt.Text, Age = age };
               string postBody = JsonSerializer(p);
               httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(&quot;application/json&quot;));
               HttpResponseMessage wcfResponse = await httpClient.PostAsync(resourceAddress, new StringContent(postBody, Encoding.UTF8, &quot;application/json&quot;));
               await DisplayTextResult(wcfResponse, OutputField);
           }
           catch (HttpRequestException hre)
           {
               NotifyUser(&quot;Error:&quot; &#43; hre.Message);
           }
           catch (TaskCanceledException)
           {
               NotifyUser(&quot;Request canceled.&quot;);
           }
           catch (Exception ex)
           {
               NotifyUser(ex.Message);
           }
           finally
           {
               this.StartButton.IsEnabled = true;
               if (httpClient != null)
               {
                   httpClient.Dispose();
                   httpClient = null;
               }
           }
       }
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__com">///&nbsp;&lt;summary&gt;</span><span class="cs__com">///&nbsp;Call&nbsp;WCF&nbsp;サービスを開始する</span><span class="cs__com">///&nbsp;&lt;/summary&gt;</span><span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;sender&quot;&gt;&lt;/param&gt;</span><span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;e&quot;&gt;&lt;/param&gt;</span><span class="cs__keyword">private</span>&nbsp;async&nbsp;<span class="cs__keyword">void</span>&nbsp;Start_Click(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;RoutedEventArgs&nbsp;e)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Output&nbsp;テキスト&nbsp;ボックスのテキストをクリアする&nbsp;</span><span class="cs__keyword">this</span>.OutputField.Text&nbsp;=&nbsp;<span class="cs__keyword">string</span>.Empty;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.StatusBlock.Text&nbsp;=&nbsp;<span class="cs__keyword">string</span>.Empty;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.StartButton.IsEnabled&nbsp;=&nbsp;<span class="cs__keyword">false</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpClient&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;HttpClient();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;resourceAddress&nbsp;=&nbsp;<span class="cs__string">&quot;http://localhost:44516/WCFService.svc/GetData&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">int</span>&nbsp;age&nbsp;=&nbsp;Convert.ToInt32(<span class="cs__keyword">this</span>.Agetxt.Text);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(age&nbsp;&gt;&nbsp;<span class="cs__number">120</span>&nbsp;||&nbsp;age&nbsp;&lt;&nbsp;<span class="cs__number">0</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">throw</span><span class="cs__keyword">new</span>&nbsp;Exception(<span class="cs__string">&quot;Age&nbsp;must&nbsp;be&nbsp;between&nbsp;0&nbsp;and&nbsp;120&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Person&nbsp;p&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Person&nbsp;{&nbsp;Name&nbsp;=&nbsp;<span class="cs__keyword">this</span>.Nametxt.Text,&nbsp;Age&nbsp;=&nbsp;age&nbsp;};&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;postBody&nbsp;=&nbsp;JsonSerializer(p);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpClient.DefaultRequestHeaders.Accept.Add(<span class="cs__keyword">new</span>&nbsp;MediaTypeWithQualityHeaderValue(<span class="cs__string">&quot;application/json&quot;</span>));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;HttpResponseMessage&nbsp;wcfResponse&nbsp;=&nbsp;await&nbsp;httpClient.PostAsync(resourceAddress,&nbsp;<span class="cs__keyword">new</span>&nbsp;StringContent(postBody,&nbsp;Encoding.UTF8,&nbsp;<span class="cs__string">&quot;application/json&quot;</span>));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;await&nbsp;DisplayTextResult(wcfResponse,&nbsp;OutputField);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(HttpRequestException&nbsp;hre)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;NotifyUser(<span class="cs__string">&quot;Error:&quot;</span>&nbsp;&#43;&nbsp;hre.Message);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(TaskCanceledException)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;NotifyUser(<span class="cs__string">&quot;Request&nbsp;canceled.&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(Exception&nbsp;ex)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;NotifyUser(ex.Message);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">finally</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.StartButton.IsEnabled&nbsp;=&nbsp;<span class="cs__keyword">true</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(httpClient&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpClient.Dispose();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpClient&nbsp;=&nbsp;<span class="cs__keyword">null</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
</pre>
</div>
</div>
</div>
</div>
<p></p>
<h2>詳細</h2>
<p>HttpClient クラス</p>
<p><a href="http://msdn.microsoft.com/ja-jp/library/system.net.http.httpclient.aspx">http://msdn.microsoft.com/ja-jp/library/system.net.http.httpclient.aspx</a></p>
<p>DataContractJsonSerializer クラス</p>
<p><a href="http://msdn.microsoft.com/ja-jp/library/system.runtime.serialization.json.datacontractjsonserializer.aspx">http://msdn.microsoft.com/ja-jp/library/system.runtime.serialization.json.datacontractjsonserializer.aspx</a>
<img id="154985" src="154985-48062aed-b646-4f91-b294-871db0bb42b7image.png" alt="" width="341" height="57"><strong>&nbsp;</strong><em>&nbsp;</em></p>
<p></p>
<div class="endscriptcode"></div>
<p></p>
<p>&nbsp;</p>
<p><em><br>
</em></p>
