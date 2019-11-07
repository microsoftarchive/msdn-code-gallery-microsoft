# How to add a hint text to a WPF Textbox
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- WPF
- .NET Framework
- Windows Desktop App Development
## Topics
- TextBox
- hint text
## Updated
- 09/22/2016
## Description

<h2><img id="154119" alt="" src="154119-8171.onecodesampletopbanner.png" width="696" height="58"></h2>
<h2>本專案說明如何將提示文字加入至 WPF 文字方塊<strong>&nbsp;</strong><em>&nbsp;</em></h2>
<h2>簡介</h2>
<p>本專案說明如何將提示文字加入至 WPF 文字方塊，以便作為控制項的正式附註。</p>
<p>由於很多開發人員在 MSDN 論壇中提出此問題，因此我們建立了程式碼範例來解決常見程式設計案例。</p>
<p>1. 文字方塊提示？</p>
<p><a href="http://social.msdn.microsoft.com/Forums/en-US/e6c6fea0-faba-440a-9e21-2e7baee52df0/textbox-hints?forum=wpf">http://social.msdn.microsoft.com/Forums/en-US/e6c6fea0-faba-440a-9e21-2e7baee52df0/textbox-hints?forum=wpf</a></p>
<p>&nbsp;</p>
<p>2. 文字方塊顯示提示資訊</p>
<p><a href="http://social.msdn.microsoft.com/Forums/en-US/37abab21-ac9f-4aa2-a073-0e363576ad5d/textbox-display-hint-info?forum=silverlightcontrols">http://social.msdn.microsoft.com/Forums/en-US/37abab21-ac9f-4aa2-a073-0e363576ad5d/textbox-display-hint-info?forum=silverlightcontrols</a></p>
<p>&nbsp;</p>
<p>3. 如何將提示文字加入至 WPF 文字方塊？</p>
<p><a href="http://stackoverflow.com/questions/7425618/how-can-i-add-a-hint-text-to-wpf-textbox?lq=1">http://stackoverflow.com/questions/7425618/how-can-i-add-a-hint-text-to-wpf-textbox?lq=1</a></p>
<p>&nbsp;</p>
<h2>建置專案</h2>
<p><strong>建立 WPF 應用程式 (CSAddHintText2Textbox)</strong></p>
<ol>
<li>在 Visual Studio 2012 中建立 WPF 應用程式 </li><li>實作一個可實作 IMultiValueConverter 的轉換器類別 </li></ol>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)

        {

            // 非空&#20540;的測試

            if (values[0] is bool &amp;&amp; values[1] is bool)

            {

                bool hasText = !(bool)values[0];

                bool hasFocus = (bool)values[1];

 

                if (hasFocus || hasText)

                    return Visibility.Collapsed;

            }

 

            return Visibility.Visible;

         }</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">object</span>&nbsp;Convert(<span class="cs__keyword">object</span>[]&nbsp;values,&nbsp;Type&nbsp;targetType,&nbsp;<span class="cs__keyword">object</span>&nbsp;parameter,&nbsp;System.Globalization.CultureInfo&nbsp;culture)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;非空&#20540;的測試</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(values[<span class="cs__number">0</span>]&nbsp;<span class="cs__keyword">is</span>&nbsp;<span class="cs__keyword">bool</span>&nbsp;&amp;&amp;&nbsp;values[<span class="cs__number">1</span>]&nbsp;<span class="cs__keyword">is</span>&nbsp;<span class="cs__keyword">bool</span>)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">bool</span>&nbsp;hasText&nbsp;=&nbsp;!(<span class="cs__keyword">bool</span>)values[<span class="cs__number">0</span>];&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">bool</span>&nbsp;hasFocus&nbsp;=&nbsp;(<span class="cs__keyword">bool</span>)values[<span class="cs__number">1</span>];&nbsp;
&nbsp;
&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(hasFocus&nbsp;||&nbsp;hasText)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;Visibility.Collapsed;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;Visibility.Visible;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}</pre>
</div>
</div>
</div>
<p style="padding-left:30px">3. &nbsp;XAML 檔案將會確保 UI 操作</p>
<p style="padding-left:30px">&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XAML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xaml</span>
<pre class="hidden">&lt;Window.Resources&gt;

        &lt;SolidColorBrush x:Key=&quot;brushWatermarkBackground&quot; Color=&quot;White&quot; /&gt;

        &lt;SolidColorBrush x:Key=&quot;brushWatermarkForeground&quot; Color=&quot;LightSteelBlue&quot; /&gt;

        &lt;SolidColorBrush x:Key=&quot;brushWatermarkBorder&quot; Color=&quot;Indigo&quot; /&gt;

 

        &lt;BooleanToVisibilityConverter x:Key=&quot;BooleanToVisibilityConverter&quot; /&gt;

        &lt;local:TextInputToVisibilityConverter x:Key=&quot;TextInputToVisibilityConverter&quot; /&gt;

 

        &lt;Style x:Key=&quot;EntryFieldStyle&quot; TargetType=&quot;Grid&quot;&gt;

            &lt;Setter Property=&quot;HorizontalAlignment&quot; Value=&quot;Stretch&quot; /&gt;

            &lt;Setter Property=&quot;VerticalAlignment&quot; Value=&quot;Center&quot; /&gt;

            &lt;Setter Property=&quot;Margin&quot; Value=&quot;20,0&quot;/&gt;

        &lt;/Style&gt;

    &lt;/Window.Resources&gt;

&lt;Grid Grid.Row=&quot;0&quot; Background=&quot;{StaticResource brushWatermarkBackground}&quot; Style=&quot;{StaticResource EntryFieldStyle}&quot; &gt;

            &lt;TextBlock Margin=&quot;5,2&quot; Text=&quot;This prompt dissappears as you type...&quot; Foreground=&quot;{StaticResource brushWatermarkForeground}&quot;

                       Visibility=&quot;{Binding ElementName=txtUserEntry, Path=Text.IsEmpty, Converter={StaticResource BooleanToVisibilityConverter}}&quot; /&gt;

            &lt;TextBox Name=&quot;txtUserEntry&quot; Background=&quot;Transparent&quot; BorderBrush=&quot;{StaticResource brushWatermarkBorder}&quot; /&gt;

        &lt;/Grid&gt;

 

        &lt;Grid Grid.Row=&quot;1&quot; Background=&quot;{StaticResource brushWatermarkBackground}&quot; Style=&quot;{StaticResource EntryFieldStyle}&quot; &gt;

            &lt;TextBlock Margin=&quot;5,2&quot; Text=&quot;This dissappears as the control gets focus...&quot; Foreground=&quot;{StaticResource brushWatermarkForeground}&quot; &gt;

                &lt;TextBlock.Visibility&gt;

                    &lt;MultiBinding Converter=&quot;{StaticResource TextInputToVisibilityConverter}&quot;&gt;

                        &lt;Binding ElementName=&quot;txtUserEntry2&quot; Path=&quot;Text.IsEmpty&quot; /&gt;

                        &lt;Binding ElementName=&quot;txtUserEntry2&quot; Path=&quot;IsFocused&quot; /&gt;

                    &lt;/MultiBinding&gt;

                &lt;/TextBlock.Visibility&gt;

            &lt;/TextBlock&gt;

            &lt;TextBox Name=&quot;txtUserEntry2&quot; Background=&quot;Transparent&quot; BorderBrush=&quot;{StaticResource brushWatermarkBorder}&quot; /&gt;

        &lt;/Grid&gt;</pre>
<div class="preview">
<pre class="xaml"><span class="xaml__tag_start">&lt;Window</span>.Resources<span class="xaml__tag_start">&gt;&nbsp;
</span><span class="xaml__tag_start">&lt;SolidColorBrush</span>&nbsp;x:<span class="xaml__attr_name">Key</span>=<span class="xaml__attr_value">&quot;brushWatermarkBackground&quot;</span><span class="xaml__attr_name">Color</span>=<span class="xaml__attr_value">&quot;White&quot;</span><span class="xaml__tag_start">/&gt;</span><span class="xaml__tag_start">&lt;SolidColorBrush</span>&nbsp;x:<span class="xaml__attr_name">Key</span>=<span class="xaml__attr_value">&quot;brushWatermarkForeground&quot;</span><span class="xaml__attr_name">Color</span>=<span class="xaml__attr_value">&quot;LightSteelBlue&quot;</span><span class="xaml__tag_start">/&gt;</span><span class="xaml__tag_start">&lt;SolidColorBrush</span>&nbsp;x:<span class="xaml__attr_name">Key</span>=<span class="xaml__attr_value">&quot;brushWatermarkBorder&quot;</span><span class="xaml__attr_name">Color</span>=<span class="xaml__attr_value">&quot;Indigo&quot;</span><span class="xaml__tag_start">/&gt;</span><span class="xaml__tag_start">&lt;BooleanToVisibilityConverter</span>&nbsp;x:<span class="xaml__attr_name">Key</span>=<span class="xaml__attr_value">&quot;BooleanToVisibilityConverter&quot;</span><span class="xaml__tag_start">/&gt;</span><span class="xaml__tag_start">&lt;local</span>:TextInputToVisibilityConverter&nbsp;x:<span class="xaml__attr_name">Key</span>=<span class="xaml__attr_value">&quot;TextInputToVisibilityConverter&quot;</span><span class="xaml__tag_start">/&gt;</span><span class="xaml__tag_start">&lt;Style</span>&nbsp;x:<span class="xaml__attr_name">Key</span>=<span class="xaml__attr_value">&quot;EntryFieldStyle&quot;</span><span class="xaml__attr_name">TargetType</span>=<span class="xaml__attr_value">&quot;Grid&quot;</span><span class="xaml__tag_start">&gt;</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;<span class="css__element">Setter</span><span class="css__element">Property</span>=&quot;<span class="css__element">HorizontalAlignment</span>&quot;&nbsp;<span class="css__element">Value</span>=&quot;<span class="css__element">Stretch</span>&quot;&nbsp;/&gt;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;<span class="css__element">Setter</span><span class="css__element">Property</span>=&quot;<span class="css__element">VerticalAlignment</span>&quot;&nbsp;<span class="css__element">Value</span>=&quot;<span class="css__element">Center</span>&quot;&nbsp;/&gt;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;<span class="css__element">Setter</span><span class="css__element">Property</span>=&quot;<span class="css__element">Margin</span>&quot;&nbsp;<span class="css__element">Value</span>=&quot;<span class="css__element">20</span>,<span class="css__element">0</span>&quot;/&gt;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_end">&lt;/Style&gt;</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&lt;/Window.Resources&gt;&nbsp;
&nbsp;
<span class="xaml__tag_start">&lt;Grid</span>&nbsp;Grid.<span class="xaml__attr_name">Row</span>=<span class="xaml__attr_value">&quot;0&quot;</span><span class="xaml__attr_name">Background</span>=<span class="xaml__attr_value">&quot;{StaticResource&nbsp;brushWatermarkBackground}&quot;</span><span class="xaml__attr_name">Style</span>=<span class="xaml__attr_value">&quot;{StaticResource&nbsp;EntryFieldStyle}&quot;</span><span class="xaml__tag_start">&gt;&nbsp;
</span><span class="xaml__tag_start">&lt;TextBlock</span><span class="xaml__attr_name">Margin</span>=<span class="xaml__attr_value">&quot;5,2&quot;</span><span class="xaml__attr_name">Text</span>=<span class="xaml__attr_value">&quot;This&nbsp;prompt&nbsp;dissappears&nbsp;as&nbsp;you&nbsp;type...&quot;</span><span class="xaml__attr_name">Foreground</span>=<span class="xaml__attr_value">&quot;{StaticResource&nbsp;brushWatermarkForeground}&quot;</span><span class="xaml__attr_name">Visibility</span>=<span class="xaml__attr_value">&quot;{Binding&nbsp;ElementName=txtUserEntry,&nbsp;Path=Text.IsEmpty,&nbsp;Converter={StaticResource&nbsp;BooleanToVisibilityConverter}}&quot;</span><span class="xaml__tag_start">/&gt;</span><span class="xaml__tag_start">&lt;TextBox</span><span class="xaml__attr_name">Name</span>=<span class="xaml__attr_value">&quot;txtUserEntry&quot;</span><span class="xaml__attr_name">Background</span>=<span class="xaml__attr_value">&quot;Transparent&quot;</span><span class="xaml__attr_name">BorderBrush</span>=<span class="xaml__attr_value">&quot;{StaticResource&nbsp;brushWatermarkBorder}&quot;</span><span class="xaml__tag_start">/&gt;</span><span class="xaml__tag_end">&lt;/Grid&gt;</span><span class="xaml__tag_start">&lt;Grid</span>&nbsp;Grid.<span class="xaml__attr_name">Row</span>=<span class="xaml__attr_value">&quot;1&quot;</span><span class="xaml__attr_name">Background</span>=<span class="xaml__attr_value">&quot;{StaticResource&nbsp;brushWatermarkBackground}&quot;</span><span class="xaml__attr_name">Style</span>=<span class="xaml__attr_value">&quot;{StaticResource&nbsp;EntryFieldStyle}&quot;</span><span class="xaml__tag_start">&gt;&nbsp;
</span><span class="xaml__tag_start">&lt;TextBlock</span><span class="xaml__attr_name">Margin</span>=<span class="xaml__attr_value">&quot;5,2&quot;</span><span class="xaml__attr_name">Text</span>=<span class="xaml__attr_value">&quot;This&nbsp;dissappears&nbsp;as&nbsp;the&nbsp;control&nbsp;gets&nbsp;focus...&quot;</span><span class="xaml__attr_name">Foreground</span>=<span class="xaml__attr_value">&quot;{StaticResource&nbsp;brushWatermarkForeground}&quot;</span><span class="xaml__tag_start">&gt;&nbsp;
</span><span class="xaml__tag_start">&lt;TextBlock</span>.Visibility<span class="xaml__tag_start">&gt;&nbsp;
</span><span class="xaml__tag_start">&lt;MultiBinding</span><span class="xaml__attr_name">Converter</span>=<span class="xaml__attr_value">&quot;{StaticResource&nbsp;TextInputToVisibilityConverter}&quot;</span><span class="xaml__tag_start">&gt;&nbsp;
</span><span class="xaml__tag_start">&lt;Binding</span><span class="xaml__attr_name">ElementName</span>=<span class="xaml__attr_value">&quot;txtUserEntry2&quot;</span><span class="xaml__attr_name">Path</span>=<span class="xaml__attr_value">&quot;Text.IsEmpty&quot;</span><span class="xaml__tag_start">/&gt;</span><span class="xaml__tag_start">&lt;Binding</span><span class="xaml__attr_name">ElementName</span>=<span class="xaml__attr_value">&quot;txtUserEntry2&quot;</span><span class="xaml__attr_name">Path</span>=<span class="xaml__attr_value">&quot;IsFocused&quot;</span><span class="xaml__tag_start">/&gt;</span><span class="xaml__tag_end">&lt;/MultiBinding&gt;</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&lt;/TextBlock.Visibility&gt;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_end">&lt;/TextBlock&gt;</span><span class="xaml__tag_start">&lt;TextBox</span><span class="xaml__attr_name">Name</span>=<span class="xaml__attr_value">&quot;txtUserEntry2&quot;</span><span class="xaml__attr_name">Background</span>=<span class="xaml__attr_value">&quot;Transparent&quot;</span><span class="xaml__attr_name">BorderBrush</span>=<span class="xaml__attr_value">&quot;{StaticResource&nbsp;brushWatermarkBorder}&quot;</span><span class="xaml__tag_start">/&gt;</span><span class="xaml__tag_end">&lt;/Grid&gt;</span></pre>
</div>
</div>
</div>
<p>&nbsp;</p>
<h2>執行範例</h2>
<p><span>1. 這個範例有兩個文字方塊</span></p>
<p><span>a.&nbsp;提示會在輸入第 1 個</span><span>文字方塊時消失</span></p>
<p><span>b.&nbsp;提示會在選取第 2 個</span><span>文字方塊時消失</span></p>
<p><span>2. 由其操作共用範例資料：</span></p>
<p><span><img id="154118" alt="" src="154118-image001.png" width="527" height="346"><br>
</span></p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and
 reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</span></p>
<p><span style="color:#ffffff"><strong>&nbsp;</strong><em>&nbsp;</em></span></p>
<p><span style="color:#ffffff">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies, and
 reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</span></p>
<p><span style="color:#ffffff"><strong>&nbsp;</strong><em>&nbsp;</em></span></p>
<p style="padding-left:30px">&nbsp;</p>
<h1><em><br>
</em></h1>
