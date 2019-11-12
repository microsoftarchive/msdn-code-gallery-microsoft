# How to display ProgressRing over WebView in universal Windows apps
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
- WebView
- ProgressRing
- universal app
## Updated
- 12/15/2014
## Description

<h1>How to display ProgressRing over WebView in universal Windows apps</h1>
<h2>Introduction</h2>
<p>This sample demonstrates how to display ProgressRing over WebView. As the WebView in Windows 8.1/Windows Phone 8.1 first supports overlay control, so we upgraded this sample. Now this sample just overlays the ProgressRing onto a WebView instead of the tricky
 way which uses WebView brush.</p>
<p>See <a href="http://blogs.windows.com/buildingapps/2013/07/17/whats-new-in-webview-in-windows-8-1/">
What&rsquo;s new in WebView in Windows 8.1</a></p>
<p>This sample targets both Windows 8.1 and Windows Phone 8.1.</p>
<h2>Running the Sample</h2>
<ol>
<li>Press F5 to run it. </li></ol>
<p>&nbsp; &nbsp; &nbsp;2. &nbsp;After the sample is launched, the screen will display as below in Windows 8.1.</p>
<p><img id="131151" src="131151-dsf.png" alt="" width="600" height="338" style="border:1px solid black"></p>
<p>The Windows Phone 8.1 will display as below:</p>
<p><img id="131152" src="131152-er'te'r't.png" alt="" width="355" height="592" style="display:block; margin-left:auto; margin-right:auto"></p>
<p>&nbsp; &nbsp; &nbsp;3. &nbsp;Please enter a valid url and click the &lsquo;Load&rsquo; button to load the page, then enter another url and click the &lsquo;Load&rsquo; button again. You will see a ProgressRing over the first page before the second page is
 loading.</p>
<p><img id="131154" src="131154-121212121.png" alt="" width="600" height="338" style="border:1px solid black"></p>
<p><img id="131155" src="131155-er'te'r't.png" alt="" width="355" height="592" style="display:block; margin-left:auto; margin-right:auto"></p>
<p><strong>PS:</strong> When the page is loading, a JavaScript exception may be thrown. This is not caused by the sample but by the web page&mdash;many web pages have JavaScript problems, even some famous websites. This issue only occurs during debugging. You
 can get rid of those annoying JavaScript exceptions in the way mentioned in this blog:</p>
<p><a href="http://blogs.msdn.com/b/wsdevsol/archive/2012/10/18/nine-things-you-need-to-know-about-webview.aspx#AN10">http://blogs.msdn.com/b/wsdevsol/archive/2012/10/18/nine-things-you-need-to-know-about-webview.aspx#AN10</a></p>
<p>&nbsp;</p>
<h2>Using the Code</h2>
<p>The code below shows the XAML UI.</p>
<p>&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XAML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xaml</span>

<div class="preview">
<pre class="xaml"><span class="xaml__tag_start">&lt;Grid</span><span class="xaml__tag_start">&gt;&nbsp;</span><span class="xaml__tag_start">&lt;WebView</span><span class="xaml__attr_name">Name</span>=<span class="xaml__attr_value">&quot;DisplayWebView&quot;</span><span class="xaml__attr_name">NavigationCompleted</span>=<span class="xaml__attr_value">&quot;DisplayWebView_NavigationCompleted&quot;</span><span class="xaml__attr_name">VerticalAlignment</span>=<span class="xaml__attr_value">&quot;Stretch&quot;</span><span class="xaml__attr_name">HorizontalAlignment</span>=<span class="xaml__attr_value">&quot;Stretch&quot;</span><span class="xaml__tag_start">/&gt;</span><span class="xaml__tag_start">&lt;ProgressRing</span><span class="xaml__attr_name">Name</span>=<span class="xaml__attr_value">&quot;LoadingProcessProgressRing&quot;</span><span class="xaml__attr_name">Width</span>=<span class="xaml__attr_value">&quot;50&quot;</span><span class="xaml__attr_name">Height</span>=<span class="xaml__attr_value">&quot;50&quot;</span><span class="xaml__attr_name">Background</span>=<span class="xaml__attr_value">&quot;Transparent&quot;</span><span class="xaml__attr_name">HorizontalAlignment</span>=<span class="xaml__attr_value">&quot;Center&quot;</span><span class="xaml__attr_name">VerticalAlignment</span>=<span class="xaml__attr_value">&quot;Center&quot;</span><span class="xaml__tag_start">/&gt;</span><span class="xaml__tag_end">&lt;/Grid&gt;</span></pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;The Code below shows HTML UI:</div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">html</span>

<div class="preview">
<pre class="html"><span class="html__tag_start">&lt;div</span><span class="html__attr_name">class</span>=<span class="html__attr_value">&quot;output&quot;</span><span class="html__tag_start">&gt;&nbsp;</span><span class="html__tag_start">&lt;x</span>-ms-webview&nbsp;<span class="html__attr_name">id</span>=<span class="html__attr_value">&quot;webview&quot;</span><span class="html__tag_start">&gt;</span>&lt;/x-ms-webview&gt;&nbsp;
<span class="html__tag_start">&lt;label</span><span class="html__attr_name">id</span>=<span class="html__attr_value">&quot;progressRingContainer&quot;</span><span class="html__tag_start">&gt;&nbsp;</span><span class="html__tag_start">&lt;progress</span><span class="html__attr_name">id</span>=<span class="html__attr_value">&quot;loadingProcessProgressRing&quot;</span><span class="html__attr_name">class</span>=<span class="html__attr_value">&quot;win-ring&quot;</span><span class="html__tag_start">&gt;&nbsp;
</span><span class="html__tag_end">&lt;/progress&gt;</span><span class="html__tag_end">&lt;/label&gt;</span><span class="html__tag_end">&lt;/div&gt;</span></pre>
</div>
</div>
</div>
</div>
<div class="endscriptcode">The code below shows Load button click event handler:</div>
<div class="endscriptcode">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>C&#43;&#43;</span><span>JavaScript</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">cplusplus</span><span class="hidden">js</span>



<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span><span class="cs__keyword">void</span>&nbsp;LoadButton_Click(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;RoutedEventArgs&nbsp;e)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Uri&nbsp;uri&nbsp;=&nbsp;ValidateAndGetUri(UrlTextBox.Text);&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(uri&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;LoadingProcessProgressRing.IsActive&nbsp;=&nbsp;<span class="cs__keyword">true</span>;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;LoadingProcessProgressRing.Visibility&nbsp;=&nbsp;Windows.UI.Xaml.Visibility.Visible;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;LoadButton.IsEnabled&nbsp;=&nbsp;<span class="cs__keyword">false</span>;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DisplayWebView.Navigate(uri);&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(Exception&nbsp;ex)&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;NotifyUser(ex.ToString());&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;&nbsp;
</pre>
</div>
</div>
</div>
</div>
<p>&nbsp;</p>
<h2>More Information</h2>
<p>WebView class</p>
<p><a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.webview.aspx">http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.webview.aspx</a></p>
<p>ProgressRing class</p>
<p><a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.progressring.aspx">http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.progressring.aspx</a></p>
<p>What&rsquo;s new in WebView in Windows 8.1</p>
<p><a href="http://blogs.windows.com/buildingapps/2013/07/17/whats-new-in-webview-in-windows-8-1/">http://blogs.windows.com/buildingapps/2013/07/17/whats-new-in-webview-in-windows-8-1/</a></p>
<p>&nbsp;</p>
<div class="endscriptcode"></div>
