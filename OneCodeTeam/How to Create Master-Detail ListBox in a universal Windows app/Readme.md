# How to Create Master-Detail ListBox in a universal Windows app
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Windows
- Windows Phone
- Windows 8
- Windows Phone 8
- Windows Store app Development
- Windows Phone Development
- Windows 8.1
- Windows Phone 8.1
## Topics
- DataBinding
- ListBox
- windows8
- universal app
## Updated
- 08/21/2016
## Description

<h1>マスター/詳細リスト ボックス (JSWindowsStoreAppMasterDetailListBox) の作成方法</h1>
<h2>はじめに</h2>
<p>このサンプルでは、CollectionViewSource を使用して Windows ストア アプリでマスター/詳細リスト ボックスを作成する方法を示します。</p>
<h2>サンプルのビルド</h2>
<p>1.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Visual Studio 2012 を起動し、[ファイル]、[開く]、[プロジェクト/ソリューション] の順に選択します。</p>
<p>2.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; サンプルをダウンロードしたディレクトリに移動します。サンプル用に名前を付けたディレクトリに移動し、Microsoft Visual Studio ソリューション (.sln) ファイルをダブルクリックします。</p>
<p>3.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; F7 キーを押すか、[ビルド]、[ソリューションのビルド] の順に選択して、サンプルをビルドします。</p>
<h2>サンプルの実行</h2>
<p>1.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; F5 キーを押して実行します。</p>
<p>2.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; サンプルを起動した後で、次の画面が表示されます。</p>
<p><img id="155086" src="155086-image.png" alt=""></p>
<p>3.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 各リスト ボックスで選択を変更することができます。詳細リスト ボックスの項目は、マスター リスト ボックスで選択した項目を基にして変更されます。</p>
<p>&nbsp;</p>
<h2>コードの使用</h2>
<p>以下のコードは、UI をデザインする方法を示しています。 </p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">编辑脚本</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">html</span>
<pre class="hidden">&lt;label&gt;Countries:&lt;/label&gt;

                        
                        &lt;select id=&quot;countryselect&quot; size=&quot;2&quot;&gt;
                            &lt;option value=&quot;1&quot; selected=&quot;selected&quot;&gt;Country1&lt;/option&gt;
                            &lt;option value=&quot;2&quot;&gt;Country2&lt;/option&gt;
                        &lt;/select&gt;
                

                
                        &lt;label id=&quot;selectedContry&quot;&gt;Country1&lt;/label&gt;

                        
                        &lt;select id=&quot;provinceselect&quot; size=&quot;2&quot;&gt;
                            &lt;option value=&quot;1&quot; selected=&quot;selected&quot;&gt;Province1&lt;/option&gt;
                            &lt;option value=&quot;2&quot;&gt;Province2&lt;/option&gt;
                        &lt;/select&gt;
                

                
                            
                    &lt;label id=&quot;selectedProvince&quot;&gt;Province1&lt;/label&gt;

                            
                    &lt;select id=&quot;cityselect&quot; size=&quot;3&quot;&gt;
                        &lt;option value=&quot;1&quot; selected=&quot;selected&quot;&gt;Province1 City1&lt;/option&gt;
                        &lt;option value=&quot;2&quot;&gt;Province1 City2&lt;/option&gt;
                        &lt;option value=&quot;3&quot;&gt;Province1 City3&lt;/option&gt;                        
                    &lt;/select&gt;
                

            




 

 

onactivated イベント ハンドラーを使用してイベント ハンドラーを登録します。以下のコードは、Select コントロールを使用して change イベント ハンドラーを登録する方法を示しています。
app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO:このアプリケーションは新しく起動された。
                // ここでアプリケーションを初期化する
            } else {
                // TODO:このアプリケーションは中断状態から再びアクティブ化された。
                // ここでアプリケーションの状態を復元する
            }
            args.setPromise(WinJS.UI.processAll().then(function completed() {
                // 選択を取得する
                var countryselect = document.getElementById(&quot;countryselect&quot;);
                var provinceselect = document.getElementById(&quot;provinceselect&quot;);
                var cityselect = document.getElementById(&quot;cityselect&quot;);


                // イベント ハンドラーを登録する
                countryselect.addEventListener(&quot;change&quot;, countryChanged, false);
                provinceselect.addEventListener(&quot;change&quot;, provinceChanged, false);
                cityselect.addEventListener(&quot;change&quot;,cityChanged,false);
            }));
        }
    };


app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO:このアプリケーションは新しく起動された。
                // ここでアプリケーションを初期化する
            } else {
                // TODO:このアプリケーションは中断状態から再びアクティブ化された。
                // ここでアプリケーションの状態を復元する
            }
            args.setPromise(WinJS.UI.processAll().then(function completed() {
                // 選択を取得する
                var countryselect = document.getElementById(&quot;countryselect&quot;);
                var provinceselect = document.getElementById(&quot;provinceselect&quot;);
                var cityselect = document.getElementById(&quot;cityselect&quot;);


                // イベント ハンドラーを登録する
                countryselect.addEventListener(&quot;change&quot;, countryChanged, false);
                provinceselect.addEventListener(&quot;change&quot;, provinceChanged, false);
                cityselect.addEventListener(&quot;change&quot;,cityChanged,false);
            }));
        }
    };


</pre>
<div class="preview">
<pre class="js">&lt;label&gt;Countries:&lt;/label&gt;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;select&nbsp;id=<span class="js__string">&quot;countryselect&quot;</span>&nbsp;size=<span class="js__string">&quot;2&quot;</span>&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;option&nbsp;value=<span class="js__string">&quot;1&quot;</span>&nbsp;selected=<span class="js__string">&quot;selected&quot;</span>&gt;Country1&lt;/option&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;option&nbsp;value=<span class="js__string">&quot;2&quot;</span>&gt;Country2&lt;/option&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/select&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;label&nbsp;id=<span class="js__string">&quot;selectedContry&quot;</span>&gt;Country1&lt;/label&gt;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;select&nbsp;id=<span class="js__string">&quot;provinceselect&quot;</span>&nbsp;size=<span class="js__string">&quot;2&quot;</span>&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;option&nbsp;value=<span class="js__string">&quot;1&quot;</span>&nbsp;selected=<span class="js__string">&quot;selected&quot;</span>&gt;Province1&lt;/option&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;option&nbsp;value=<span class="js__string">&quot;2&quot;</span>&gt;Province2&lt;/option&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/select&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;label&nbsp;id=<span class="js__string">&quot;selectedProvince&quot;</span>&gt;Province1&lt;/label&gt;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;select&nbsp;id=<span class="js__string">&quot;cityselect&quot;</span>&nbsp;size=<span class="js__string">&quot;3&quot;</span>&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;option&nbsp;value=<span class="js__string">&quot;1&quot;</span>&nbsp;selected=<span class="js__string">&quot;selected&quot;</span>&gt;Province1&nbsp;City1&lt;/option&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;option&nbsp;value=<span class="js__string">&quot;2&quot;</span>&gt;Province1&nbsp;City2&lt;/option&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;option&nbsp;value=<span class="js__string">&quot;3&quot;</span>&gt;Province1&nbsp;City3&lt;/option&gt;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/select&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;
&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;
&nbsp;
onactivated&nbsp;イベント&nbsp;ハンドラーを使用してイベント&nbsp;ハンドラーを登録します。以下のコードは、Select&nbsp;コントロールを使用して&nbsp;change&nbsp;イベント&nbsp;ハンドラーを登録する方法を示しています。&nbsp;
app.onactivated&nbsp;=&nbsp;<span class="js__operator">function</span>&nbsp;(args)&nbsp;<span class="js__brace">{</span><span class="js__statement">if</span>&nbsp;(args.detail.kind&nbsp;===&nbsp;activation.ActivationKind.launch)&nbsp;<span class="js__brace">{</span><span class="js__statement">if</span>&nbsp;(args.detail.previousExecutionState&nbsp;!==&nbsp;activation.ApplicationExecutionState.terminated)&nbsp;<span class="js__brace">{</span><span class="js__sl_comment">//&nbsp;TODO:このアプリケーションは新しく起動された。</span><span class="js__sl_comment">//&nbsp;ここでアプリケーションを初期化する</span><span class="js__brace">}</span><span class="js__statement">else</span><span class="js__brace">{</span><span class="js__sl_comment">//&nbsp;TODO:このアプリケーションは中断状態から再びアクティブ化された。</span><span class="js__sl_comment">//&nbsp;ここでアプリケーションの状態を復元する</span><span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;args.setPromise(WinJS.UI.processAll().then(<span class="js__operator">function</span>&nbsp;completed()&nbsp;<span class="js__brace">{</span><span class="js__sl_comment">//&nbsp;選択を取得する</span><span class="js__statement">var</span>&nbsp;countryselect&nbsp;=&nbsp;document.getElementById(<span class="js__string">&quot;countryselect&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;provinceselect&nbsp;=&nbsp;document.getElementById(<span class="js__string">&quot;provinceselect&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;cityselect&nbsp;=&nbsp;document.getElementById(<span class="js__string">&quot;cityselect&quot;</span>);&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;イベント&nbsp;ハンドラーを登録する</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;countryselect.addEventListener(<span class="js__string">&quot;change&quot;</span>,&nbsp;countryChanged,&nbsp;false);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;provinceselect.addEventListener(<span class="js__string">&quot;change&quot;</span>,&nbsp;provinceChanged,&nbsp;false);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cityselect.addEventListener(<span class="js__string">&quot;change&quot;</span>,cityChanged,false);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span><span class="js__brace">}</span>;&nbsp;
&nbsp;
&nbsp;
app.onactivated&nbsp;=&nbsp;<span class="js__operator">function</span>&nbsp;(args)&nbsp;<span class="js__brace">{</span><span class="js__statement">if</span>&nbsp;(args.detail.kind&nbsp;===&nbsp;activation.ActivationKind.launch)&nbsp;<span class="js__brace">{</span><span class="js__statement">if</span>&nbsp;(args.detail.previousExecutionState&nbsp;!==&nbsp;activation.ApplicationExecutionState.terminated)&nbsp;<span class="js__brace">{</span><span class="js__sl_comment">//&nbsp;TODO:このアプリケーションは新しく起動された。</span><span class="js__sl_comment">//&nbsp;ここでアプリケーションを初期化する</span><span class="js__brace">}</span><span class="js__statement">else</span><span class="js__brace">{</span><span class="js__sl_comment">//&nbsp;TODO:このアプリケーションは中断状態から再びアクティブ化された。</span><span class="js__sl_comment">//&nbsp;ここでアプリケーションの状態を復元する</span><span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;args.setPromise(WinJS.UI.processAll().then(<span class="js__operator">function</span>&nbsp;completed()&nbsp;<span class="js__brace">{</span><span class="js__sl_comment">//&nbsp;選択を取得する</span><span class="js__statement">var</span>&nbsp;countryselect&nbsp;=&nbsp;document.getElementById(<span class="js__string">&quot;countryselect&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;provinceselect&nbsp;=&nbsp;document.getElementById(<span class="js__string">&quot;provinceselect&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;cityselect&nbsp;=&nbsp;document.getElementById(<span class="js__string">&quot;cityselect&quot;</span>);&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;イベント&nbsp;ハンドラーを登録する</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;countryselect.addEventListener(<span class="js__string">&quot;change&quot;</span>,&nbsp;countryChanged,&nbsp;false);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;provinceselect.addEventListener(<span class="js__string">&quot;change&quot;</span>,&nbsp;provinceChanged,&nbsp;false);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cityselect.addEventListener(<span class="js__string">&quot;change&quot;</span>,cityChanged,false);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span><span class="js__brace">}</span>;&nbsp;
&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;コントロールが変更されるときに select コントロールの値を変更します。以下のコードは、最後の手順で登録するイベント ハンドラーを実装する方法を示しています。</div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">编辑脚本</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">js</span>
<pre class="hidden">function countryChanged(eventInfo) {


       objCountrySelect = document.getElementById(&quot;countryselect&quot;);
       objProvinceSelect = document.getElementById(&quot;provinceselect&quot;);


       var selectedCountry = objCountrySelect.options[objCountrySelect.selectedIndex].value;
       var selectedText = objCountrySelect.options[objCountrySelect.selectedIndex].text;


    
       // 選択した国を表示する
       document.getElementById(&quot;country&quot;).innerText = selectedText;
       document.getElementById(&quot;selectedContry&quot;).innerText = selectedText;


       if (selectedCountry == 1) {
           objProvinceSelect.innerHTML = &quot;&lt;select&gt;&lt;option&gt;Province1&lt;/option&gt;&lt;option&gt;Province2&lt;/option&gt;&lt;/select&gt;&quot;;
           document.getElementById(&quot;province&quot;).innerText = objProvinceSelect.options[0].text;
           document.getElementById(&quot;selectedProvince&quot;).innerText = objProvinceSelect.options[0].text;


           // 最初の項目を選択する
           objProvinceSelect.options[0].selected = &quot;selected&quot;;
           document.getElementById(&quot;city&quot;).innerText = &quot;Province1 City1&quot;;
           fillCitySelect(objProvinceSelect.options[0].text);
       
       }
       else {
           objProvinceSelect.innerHTML = &quot;&lt;select&gt;&lt;option&gt;Province3&lt;/option&gt;&lt;option&gt;Province4&lt;/option&gt;&lt;/select&gt;&quot;;
           document.getElementById(&quot;province&quot;).innerText = objProvinceSelect.options[0].text;
           document.getElementById(&quot;selectedProvince&quot;).innerText = objProvinceSelect.options[0].text;


           // 最初の項目を選択する
           objProvinceSelect.options[0].selected = &quot;selected&quot;;
           document.getElementById(&quot;city&quot;).innerText = &quot;Province3 City1&quot;;
           fillCitySelect(objProvinceSelect.options[0].text);
       }
   }


   function provinceChanged(eventInfo) {
       objProvinceSelect = document.getElementById(&quot;provinceselect&quot;);
       var selectedProviceText = objProvinceSelect.options[objProvinceSelect.selectedIndex].text;


       // 選択した国を表示する
       document.getElementById(&quot;province&quot;).innerText = selectedProviceText
       document.getElementById(&quot;selectedProvince&quot;).innerText = selectedProviceText;




       // 都市の選択を入力する
       fillCitySelect(selectedProviceText);
       document.getElementById(&quot;city&quot;).innerText = selectedProviceText &#43; &quot; City1&quot;;
   }


   function cityChanged(eventInfo) {
       objCitySelect = document.getElementById(&quot;cityselect&quot;);
       var selectedCityText = objCitySelect.options[objCitySelect.selectedIndex].text;


       // 選択した国を表示する
       document.getElementById(&quot;city&quot;).innerText = selectedCityText;
   }


   // 都市の選択を入力する
   function fillCitySelect(province) {      
       objCitySelect = document.getElementById(&quot;cityselect&quot;);
       objCitySelect.innerHTML = &quot;&lt;select&gt;&lt;option&gt;&quot; &#43; province &#43; &quot; City1&quot; &#43; &quot;&lt;/option&gt;&lt;option&gt;&quot; &#43; province &#43; &quot; City2&quot; &#43; &quot;&lt;/option&gt;&lt;option&gt;&quot; &#43; province &#43; &quot; City3&quot; &#43; &quot;&lt;/option&gt;&lt;/select&gt;&quot;;


       objCitySelect.options[0].selected = &quot;selected&quot;;
   }

</pre>
<div class="preview">
<pre class="js"><span class="js__operator">function</span>&nbsp;countryChanged(eventInfo)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;objCountrySelect&nbsp;=&nbsp;document.getElementById(<span class="js__string">&quot;countryselect&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;objProvinceSelect&nbsp;=&nbsp;document.getElementById(<span class="js__string">&quot;provinceselect&quot;</span>);&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;selectedCountry&nbsp;=&nbsp;objCountrySelect.options[objCountrySelect.selectedIndex].value;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;selectedText&nbsp;=&nbsp;objCountrySelect.options[objCountrySelect.selectedIndex].text;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;選択した国を表示する</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(<span class="js__string">&quot;country&quot;</span>).innerText&nbsp;=&nbsp;selectedText;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(<span class="js__string">&quot;selectedContry&quot;</span>).innerText&nbsp;=&nbsp;selectedText;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">if</span>&nbsp;(selectedCountry&nbsp;==&nbsp;<span class="js__num">1</span>)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;objProvinceSelect.innerHTML&nbsp;=&nbsp;<span class="js__string">&quot;&lt;select&gt;&lt;option&gt;Province1&lt;/option&gt;&lt;option&gt;Province2&lt;/option&gt;&lt;/select&gt;&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(<span class="js__string">&quot;province&quot;</span>).innerText&nbsp;=&nbsp;objProvinceSelect.options[<span class="js__num">0</span>].text;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(<span class="js__string">&quot;selectedProvince&quot;</span>).innerText&nbsp;=&nbsp;objProvinceSelect.options[<span class="js__num">0</span>].text;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;最初の項目を選択する</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;objProvinceSelect.options[<span class="js__num">0</span>].selected&nbsp;=&nbsp;<span class="js__string">&quot;selected&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(<span class="js__string">&quot;city&quot;</span>).innerText&nbsp;=&nbsp;<span class="js__string">&quot;Province1&nbsp;City1&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;fillCitySelect(objProvinceSelect.options[<span class="js__num">0</span>].text);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span><span class="js__statement">else</span><span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;objProvinceSelect.innerHTML&nbsp;=&nbsp;<span class="js__string">&quot;&lt;select&gt;&lt;option&gt;Province3&lt;/option&gt;&lt;option&gt;Province4&lt;/option&gt;&lt;/select&gt;&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(<span class="js__string">&quot;province&quot;</span>).innerText&nbsp;=&nbsp;objProvinceSelect.options[<span class="js__num">0</span>].text;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(<span class="js__string">&quot;selectedProvince&quot;</span>).innerText&nbsp;=&nbsp;objProvinceSelect.options[<span class="js__num">0</span>].text;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;最初の項目を選択する</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;objProvinceSelect.options[<span class="js__num">0</span>].selected&nbsp;=&nbsp;<span class="js__string">&quot;selected&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(<span class="js__string">&quot;city&quot;</span>).innerText&nbsp;=&nbsp;<span class="js__string">&quot;Province3&nbsp;City1&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;fillCitySelect(objProvinceSelect.options[<span class="js__num">0</span>].text);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span><span class="js__brace">}</span><span class="js__operator">function</span>&nbsp;provinceChanged(eventInfo)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;objProvinceSelect&nbsp;=&nbsp;document.getElementById(<span class="js__string">&quot;provinceselect&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;selectedProviceText&nbsp;=&nbsp;objProvinceSelect.options[objProvinceSelect.selectedIndex].text;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;選択した国を表示する</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(<span class="js__string">&quot;province&quot;</span>).innerText&nbsp;=&nbsp;selectedProviceText&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(<span class="js__string">&quot;selectedProvince&quot;</span>).innerText&nbsp;=&nbsp;selectedProviceText;&nbsp;
&nbsp;
&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;都市の選択を入力する</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;fillCitySelect(selectedProviceText);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(<span class="js__string">&quot;city&quot;</span>).innerText&nbsp;=&nbsp;selectedProviceText&nbsp;&#43;&nbsp;<span class="js__string">&quot;&nbsp;City1&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span><span class="js__operator">function</span>&nbsp;cityChanged(eventInfo)&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;objCitySelect&nbsp;=&nbsp;document.getElementById(<span class="js__string">&quot;cityselect&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">var</span>&nbsp;selectedCityText&nbsp;=&nbsp;objCitySelect.options[objCitySelect.selectedIndex].text;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;選択した国を表示する</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;document.getElementById(<span class="js__string">&quot;city&quot;</span>).innerText&nbsp;=&nbsp;selectedCityText;&nbsp;
&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span><span class="js__sl_comment">//&nbsp;都市の選択を入力する</span><span class="js__operator">function</span>&nbsp;fillCitySelect(province)&nbsp;<span class="js__brace">{</span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;objCitySelect&nbsp;=&nbsp;document.getElementById(<span class="js__string">&quot;cityselect&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;objCitySelect.innerHTML&nbsp;=&nbsp;<span class="js__string">&quot;&lt;select&gt;&lt;option&gt;&quot;</span>&nbsp;&#43;&nbsp;province&nbsp;&#43;&nbsp;<span class="js__string">&quot;&nbsp;City1&quot;</span>&nbsp;&#43;&nbsp;<span class="js__string">&quot;&lt;/option&gt;&lt;option&gt;&quot;</span>&nbsp;&#43;&nbsp;province&nbsp;&#43;&nbsp;<span class="js__string">&quot;&nbsp;City2&quot;</span>&nbsp;&#43;&nbsp;<span class="js__string">&quot;&lt;/option&gt;&lt;option&gt;&quot;</span>&nbsp;&#43;&nbsp;province&nbsp;&#43;&nbsp;<span class="js__string">&quot;&nbsp;City3&quot;</span>&nbsp;&#43;&nbsp;<span class="js__string">&quot;&lt;/option&gt;&lt;/select&gt;&quot;</span>;&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;objCitySelect.options[<span class="js__num">0</span>].selected&nbsp;=&nbsp;<span class="js__string">&quot;selected&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span></pre>
</div>
</div>
</div>
</div>
<p></p>
<h2>詳細情報</h2>
<p>JavaScript を使った初めての Windows ストア アプリの作成</p>
<p><a href="http://msdn.microsoft.com/ja-jp/library/windows/apps/br211385.aspx">http://msdn.microsoft.com/ja-jp/library/windows/apps/br211385.aspx</a></p>
<p>レイアウトと方向 (JavaScript と HTML を使った Windows ストア アプリ)</p>
<p><a href="http://msdn.microsoft.com/ja-jp/library/windows/apps/jj841108.aspx">http://msdn.microsoft.com/ja-jp/library/windows/apps/jj841108.aspx</a></p>
<p>アプリのライフサイクルと状態の管理 (JavaScript と HTML を使った Windows ストア アプリ)</p>
<p><a href="http://msdn.microsoft.com/ja-jp/library/windows/apps/hh986966.aspx">http://msdn.microsoft.com/ja-jp/library/windows/apps/hh986966.aspx</a>
<strong>&nbsp;</strong><em>&nbsp;</em></p>
<p></p>
<div class="endscriptcode"></div>
<p></p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and
 reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</span></p>
<p><strong>&nbsp;</strong><em></em></p>
<p><img id="155087" src="155087-a20d39fc-847b-4f6c-a5ee-5b3dad80666dimage.png" alt=""></p>
