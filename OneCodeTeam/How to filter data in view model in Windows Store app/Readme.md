# How to filter data in view model in Windows Store app
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Windows
- Windows 8
- Windows 8.1
## Topics
- Filter data
## Updated
- 09/21/2016
## Description

<h1><em><img id="154723" src="154723-8171.onecodesampletopbanner.png" alt=""></em></h1>
<h1>Windows Store アプリのビュー モデルでデータをフィルター処理する方法 (CSWindowsStoreAppFlightDataFilter)</h1>
<h2>はじめに</h2>
<p>大量のデータが画面に表示されるアプリケーションを作成したことがある、または作成している最中でしょうか。ある時点で、ユーザーがデータをフィルター処理できるようにしたいと思いますが、それを実現する最善の方法が分からないことに気付きます。以下に示すサンプル アプリケーションでは、サンプル データとしてフライト情報を使用し、航空運賃によるフィルターを追加します。このサンプルで示されるメソドロジは、複数のフィルター条件、およびリアル タイムのライブ データまたは静的データ ソースが存在する規模の大きいアプリケーションに適用できます。</p>
<p>メモ: このサンプルは、次のブログに基づいています。&nbsp;&nbsp;</p>
<p><a href="http://blogs.msdn.com/b/wsdevsol/archive/2013/11/14/filtering-data-in-your-view-model.aspx">http://blogs.msdn.com/b/wsdevsol/archive/2013/11/14/filtering-data-in-your-view-model.aspx</a></p>
<h2>サンプルの実行</h2>
<p>Visual Studio 2013 でこのサンプルをビルドし、実行します。アプリは次のようになります。<img id="154724" src="154724-image.png" alt="" width="576" height="359"></p>
<h2>コードの使用</h2>
<p>ItemsPage.xaml は、すべてのフライト情報を表示するアプリケーションのメイン ビューになります。MainViewModel.cs は、ItemsPage.xaml がバインドされるプロパティおよびメソッドを公開する ViewModel になります。&nbsp;フライト情報は、ItemsSource プロパティが MainViewModel の FilteredFlights プロパティにバインドされる GridView に表示されます。</p>
<p>以下の手順には、MainViewModel の FilteredFlights プロパティの公開、そのプロパティの GridView へのバインド、価&#26684;でフィルター処理するためのコントロールの追加、そして最後に現在の価&#26684;フィルターに基づいて FilteredFlights プロパティを最新の状態に維持する関数の追加を実行する方法が示されています。</p>
<p>手順 1: フィルター対象データのプロパティを作成します</p>
<p>MainViewModel には、FlightDataItem の ObservableCollection になる Flights プロパティがあります。&nbsp; このプロパティは、データ ソース (FlightData.json) のすべてのフライトで初期化されます。&nbsp; フィルター処理をサポートするため、FilteredFlights という名前のプロパティをビュー モデルに追加します。これを行なうには、MainViewModel.cs ファイルを開き、Flights プロパティの後に次のコードを追加します。<strong>&nbsp;</strong><em>&nbsp;</em></p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">private ObservableCollection&lt;FlightDataItem&gt; _filteredFlights;
  
 public ObservableCollection&lt;FlightDataItem&gt; FilteredFlights
 {
     get { return _filteredFlights; }
     set { _filteredFlights = value; NotifyPropertyChanged(&quot;FilteredFlights&quot;); }
 }
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;ObservableCollection&lt;FlightDataItem&gt;&nbsp;_filteredFlights;&nbsp;
&nbsp;&nbsp;&nbsp;
&nbsp;<span class="cs__keyword">public</span>&nbsp;ObservableCollection&lt;FlightDataItem&gt;&nbsp;FilteredFlights&nbsp;
&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;_filteredFlights;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">set</span>&nbsp;{&nbsp;_filteredFlights&nbsp;=&nbsp;<span class="cs__keyword">value</span>;&nbsp;NotifyPropertyChanged(<span class="cs__string">&quot;FilteredFlights&quot;</span>);&nbsp;}&nbsp;
&nbsp;}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span lang="EN">FilteredFlights を初期化して、すべてのフライトを含めるため、</span></div>
<div class="endscriptcode"><span lang="EN">MainViewModel コンストラクターに次のコード行を追加します。</span><strong>&nbsp;</strong><em>&nbsp;</em></div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">FilteredFlights = Flights;
</pre>
<div class="preview">
<pre class="csharp">FilteredFlights&nbsp;=&nbsp;Flights;&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span lang="EN">手順 2: GridView をフィルター対象データのプロパティにバインドします</span><strong>&nbsp;</strong><em></em></div>
<div class="endscriptcode"></div>
<span lang="EN"><span class="SpellE">MainViewModel</span> には FilteredFlights プロパティがあります。このプロパティを GridView にバインドします。ItemsPage.xaml ファイルを XAML ビューで開き、GridView の ItemsSource プロパティを変更して FilteredFlights にバインドします。</span><strong></strong><em></em>
<p></p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">&lt;GridView
        x:Name=&quot;itemGridView&quot;
        AutomationProperties.AutomationId=&quot;ItemsGridView&quot;
        AutomationProperties.Name=&quot;Items&quot;
        TabIndex=&quot;1&quot;
        ItemsSource=&quot;{Binding FilteredFlights}&quot;
        SelectionMode=&quot;None&quot;
        IsSwipeEnabled=&quot;false&quot;
        IsItemClickEnabled=&quot;True&quot; Grid.Column=&quot;1&quot; Margin=&quot;20,20,0,0&quot;&gt;

</pre>
<div class="preview">
<pre class="csharp">&lt;GridView&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;x:Name=<span class="cs__string">&quot;itemGridView&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AutomationProperties.AutomationId=<span class="cs__string">&quot;ItemsGridView&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AutomationProperties.Name=<span class="cs__string">&quot;Items&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;TabIndex=<span class="cs__string">&quot;1&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;ItemsSource=<span class="cs__string">&quot;{Binding&nbsp;FilteredFlights}&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SelectionMode=<span class="cs__string">&quot;None&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;IsSwipeEnabled=<span class="cs__string">&quot;false&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;IsItemClickEnabled=<span class="cs__string">&quot;True&quot;</span>&nbsp;Grid.Column=<span class="cs__string">&quot;1&quot;</span>&nbsp;Margin=<span class="cs__string">&quot;20,20,0,0&quot;</span>&gt;&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<p></p>
<p>これで追加機能をビューに追加するための設定が整ったので、データをフィルター処理することができます。</p>
<p>手順 3: データをフィルター処理するためのコントロールを追加します</p>
<p>続けて、ItemsPage.xaml ファイルで価&#26684;フィルターにスライダー コントロールを備えることができるようにレイアウトを変更します。最初に、列定義を追加することによって、メイン レイアウト Grid に 2 つの列を追加します。<strong></strong><em></em></p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XAML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xaml</span>
<pre class="hidden">&lt;!--
このグリッドは、以下の 2 つの行を定義するページのルート パネルとして機能する。
* Row 0 には、[戻る] ボタンとページ タイトルが含まれる
* Row 1 には、残りのページ レイアウトが含まれる
--&gt;
&lt;Grid Background=&quot;{ThemeResource ApplicationPageBackgroundThemeBrush}&quot;&gt;
    &lt;Grid.ColumnDefinitions&gt;
        &lt;ColumnDefinition Width=&quot;344*&quot;/&gt;
        &lt;ColumnDefinition Width=&quot;1023*&quot;/&gt;
    &lt;/Grid.ColumnDefinitions&gt;
</pre>
<div class="preview">
<pre class="xaml"><span class="xaml__comment">&lt;!--&nbsp;
このグリッドは、以下の&nbsp;2&nbsp;つの行を定義するページのルート&nbsp;パネルとして機能する。&nbsp;
*&nbsp;Row&nbsp;0&nbsp;には、[戻る]&nbsp;ボタンとページ&nbsp;タイトルが含まれる&nbsp;
*&nbsp;Row&nbsp;1&nbsp;には、残りのページ&nbsp;レイアウトが含まれる&nbsp;
--&gt;</span><span class="xaml__tag_start">&lt;Grid</span><span class="xaml__attr_name">Background</span>=<span class="xaml__attr_value">&quot;{ThemeResource&nbsp;ApplicationPageBackgroundThemeBrush}&quot;</span><span class="xaml__tag_start">&gt;&nbsp;
</span><span class="xaml__tag_start">&lt;Grid</span>.ColumnDefinitions<span class="xaml__tag_start">&gt;&nbsp;
</span><span class="xaml__tag_start">&lt;ColumnDefinition</span><span class="xaml__attr_name">Width</span>=<span class="xaml__attr_value">&quot;344*&quot;</span><span class="xaml__tag_start">/&gt;</span><span class="xaml__tag_start">&lt;ColumnDefinition</span><span class="xaml__attr_name">Width</span>=<span class="xaml__attr_value">&quot;1023*&quot;</span><span class="xaml__tag_start">/&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&lt;/Grid.ColumnDefinitions&gt;&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"><span lang="EN">GridView の後に価&#26684;フィルターのレイアウト要素およびコントロールを追加します。スライダー コントロールの Value プロパティは、ビュー モデルにまだ追加していない SelectedPrice プロパティにバインドされます。これは、次のセクションで行ないます。また、バインディング Mode は TwoWay になります。
</span></div>
<div class="endscriptcode"><span lang="EN"><br>
</span></div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XAML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xaml</span>
<pre class="hidden">&lt;StackPanel Grid.Row=&quot;1&quot;&gt;
    &lt;TextBlock Text=&quot;Price Filter&quot; FontSize=&quot;48&quot; Margin=&quot;10&quot; /&gt;
    &lt;Slider Height=&quot;50&quot; Margin=&quot;10&quot; Minimum=&quot;100.00&quot; Maximum=&quot;2265.00&quot; Value=&quot;{Binding Path=SelectedPrice, Mode=TwoWay}&quot; /&gt;
&lt;/StackPanel&gt;

</pre>
<div class="preview">
<pre class="xaml"><span class="xaml__tag_start">&lt;StackPanel</span>&nbsp;Grid.<span class="xaml__attr_name">Row</span>=<span class="xaml__attr_value">&quot;1&quot;</span><span class="xaml__tag_start">&gt;&nbsp;
</span><span class="xaml__tag_start">&lt;TextBlock</span><span class="xaml__attr_name">Text</span>=<span class="xaml__attr_value">&quot;Price&nbsp;Filter&quot;</span><span class="xaml__attr_name">FontSize</span>=<span class="xaml__attr_value">&quot;48&quot;</span><span class="xaml__attr_name">Margin</span>=<span class="xaml__attr_value">&quot;10&quot;</span><span class="xaml__tag_start">/&gt;</span><span class="xaml__tag_start">&lt;Slider</span><span class="xaml__attr_name">Height</span>=<span class="xaml__attr_value">&quot;50&quot;</span><span class="xaml__attr_name">Margin</span>=<span class="xaml__attr_value">&quot;10&quot;</span><span class="xaml__attr_name">Minimum</span>=<span class="xaml__attr_value">&quot;100.00&quot;</span><span class="xaml__attr_name">Maximum</span>=<span class="xaml__attr_value">&quot;2265.00&quot;</span><span class="xaml__attr_name">Value</span>=<span class="xaml__attr_value">&quot;{Binding&nbsp;Path=SelectedPrice,&nbsp;Mode=TwoWay}&quot;</span><span class="xaml__tag_start">/&gt;</span><span class="xaml__tag_end">&lt;/StackPanel&gt;</span></pre>
</div>
</div>
</div>
<p></p>
<p>これで、データのフィルター処理のビューで必要とされる変更は終わりです。アプリケーションを実行すると、スライダーを動かすことはできますが、データはまだ変更されません。この記事の最終セクションで、これを実行するためのコードを追加します。</p>
<p>手順 4: MainViewModel にすべてをまとめます</p>
<p>最終セクションでは、データをフィルター処理するための RefreshFilteredData という名前の関数を追加します。また、SelectedPrice プロパティを MainViewModel.cs ファイルに追加します。RefreshFilteredData 関数は、SelectedPrice より低い価&#26684;のすべてのアイテムを Flights コレクションから選択します。<strong></strong><em></em></p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">private void RefreshFilteredData()
 {
     var fr = from fobjs in Flights
             where fobjs.Price &lt; SelectedPrice
             select fobjs;
  
     // これはビューを更新する量を制限する
     if (FilteredFlights.Count == fr.Count())
         return;
  
     FilteredFlights = new ObservableCollection&lt;FlightDataItem&gt;(fr);
 }
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;RefreshFilteredData()&nbsp;
&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;fr&nbsp;=&nbsp;from&nbsp;fobjs&nbsp;<span class="cs__keyword">in</span>&nbsp;Flights&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;where&nbsp;fobjs.Price&nbsp;&lt;&nbsp;SelectedPrice&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;fobjs;&nbsp;
&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;これはビューを更新する量を制限する</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(FilteredFlights.Count&nbsp;==&nbsp;fr.Count())&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>;&nbsp;
&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FilteredFlights&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;ObservableCollection&lt;FlightDataItem&gt;(fr);&nbsp;
&nbsp;}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<span lang="EN">以下のコードは SelectedPrice プロパティの定義です。このプロパティが変更されると (TwoWay バインディングのために価&#26684;スライダーが移動する場合)、RefreshFilteredData 関数を呼び出し、MainViewModel の FilteredFlights プロパティを更新します。</span><strong></strong><em></em>
<p></p>
<p></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">private double _selectedPrice;
  
 public double SelectedPrice
 {
     get { return _selectedPrice; }
     set { _selectedPrice = value; NotifyPropertyChanged(&quot;SelectedPrice&quot;); RefreshFilteredData(); }
 }
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">double</span>&nbsp;_selectedPrice;&nbsp;
&nbsp;&nbsp;&nbsp;
&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">double</span>&nbsp;SelectedPrice&nbsp;
&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;_selectedPrice;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">set</span>&nbsp;{&nbsp;_selectedPrice&nbsp;=&nbsp;<span class="cs__keyword">value</span>;&nbsp;NotifyPropertyChanged(<span class="cs__string">&quot;SelectedPrice&quot;</span>);&nbsp;RefreshFilteredData();&nbsp;}&nbsp;
&nbsp;}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span lang="EN">最後に、LoadFlightData() 関数の終わりに以下のコードを追加して、SelectedPrice プロパティを初期化する必要があります。</span><strong></strong><em></em></div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">SelectedPrice = maxPrice;</pre>
<div class="preview">
<pre class="csharp">SelectedPrice&nbsp;=&nbsp;maxPrice;</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span lang="EN">これで、データのフィルター処理をサポートするための MainViewModel の更新に必要な処理は終わりです。この時点で、アプリケーションを実行してスライダー コントロールを左に移動すると、選択した価&#26684;範囲にある価&#26684;のみを表示するようにフィルター処理されたフライト情報が表示されます。<span>
</span></span><strong></strong><em></em></div>
</div>
<img id="154725" src="154725-db60ff8c-e1f3-4f08-a0e9-e63f0e5817f0image.png" alt="">
<p></p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<p><em><br>
</em></p>
