# How to Get Screen Resolution in Win10 UWP App
## Requires
- Visual Studio 2015
## License
- Apache License, Version 2.0
## Technologies
- Universal Windows App Development
- Universal Windows Platform
- UWP
## Topics
- Universal Windows App Development
- screen resolution
## Updated
- 11/14/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src="https://aka.ms/onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em>&nbsp;</em></div>
<p style="font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; margin:0pt">
<span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">How to Get Screen Resolution in Win10 UWP App</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Introduction</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">The sample demonstrates how to get screen resolution in Win10 UWP app.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Building the Sample</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>This sample should be run in Microsoft Visual Studio 2015 version
</span><span>and</span><span> Windows 10. Before you start, make sure you </span>
<span>have installed </span><span style="font-size:11pt">Visual Studio 2015 </span>
<span>in</span><span> </span><span>Windows 10</span><span>.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>Start Microsoft Visual Studio and select </span>
<span style="font-weight:bold">File &gt; Open &gt; Project/Solution.</span><span>
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Go to the directory to which the sample was unzipped. Then go to the subdirectory named for the sample and double-click the Visual
 Studio 2015 Solution (.</span><span>sln</span><span>) file. </span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>S</span><span>elect</span><span>
</span><span style="font-weight:bold">Build &gt; Build Solution</span><span style="font-weight:bold">
</span><span style="font-weight:bold">to build the sample.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Running the Sample</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-weight:bold">Desktop </span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>In the target device menu on the Standard toolbar, make sure that
</span><span style="font-weight:bold">Local Machine</span><span> is selected. Do one of the following:
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Click the
</span><span style="font-weight:bold">Start Debugging</span><span> button on the toolbar</span><span>.</span><span>
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Click</span><span>
</span><span style="font-weight:bold">Start Debugging</span><span> </span><span>in the
</span><span style="font-weight:bold">Debug </span><span>menu.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Press F5</span><span>.</span><span>
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>&nbsp;</span><span>The app window </span><span>will be</span><span> open</span><span>ed.</span><span>
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt"><img src="156789-image.png" alt="" width="575" height="453" align="middle">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-weight:bold">Mobile </span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>In the target device menu on the Standard toolbar, select one of the Windows 10 mobile emulators and do one of the following:
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Click the
</span><span style="font-weight:bold">Start Debugging</span><span> button on the toolbar</span><span>.</span><span>
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>C</span><span>lick
</span><span style="font-weight:bold">Start Debugging</span><span> </span><span>in the
</span><span style="font-weight:bold">Debug </span><span>menu.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Press F5</span><span>.</span><span>
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>The app </span><span>will be</span><span> </span>
<span>open</span><span>ed</span><span> in the emulator</span><span>.</span><span>
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt"><img src="156790-image.png" alt="" width="291" height="519" align="middle">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span><span style="font-size:13pt; font-weight:bold; line-height:27.6pt">Using the Code</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span><span style="font-size:11pt; line-height:27.6pt">MainPage.xaml</span><span style="font-size:11pt; line-height:27.6pt">:</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>&nbsp;</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XAML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xaml</span>

<div class="preview">
<pre class="xaml"><span class="xaml__tag_start">&lt;Grid</span>&nbsp;<span class="xaml__attr_name">Background</span>=<span class="xaml__attr_value">&quot;{ThemeResource&nbsp;ApplicationPageBackgroundThemeBrush}&quot;</span><span class="xaml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_start">&lt;Button</span>&nbsp;x:<span class="xaml__attr_name">Name</span>=<span class="xaml__attr_value">&quot;btnShow&quot;</span>&nbsp;<span class="xaml__attr_name">Content</span>=<span class="xaml__attr_value">&quot;Show&quot;</span>&nbsp;<span class="xaml__attr_name">HorizontalAlignment</span>=<span class="xaml__attr_value">&quot;Left&quot;</span>&nbsp;<span class="xaml__attr_name">Margin</span>=<span class="xaml__attr_value">&quot;10,50,0,0&quot;</span>&nbsp;<span class="xaml__attr_name">VerticalAlignment</span>=<span class="xaml__attr_value">&quot;Top&quot;</span>&nbsp;<span class="xaml__attr_name">Click</span>=<span class="xaml__attr_value">&quot;btnShow_Click&quot;</span><span class="xaml__tag_start">/&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xaml__tag_start">&lt;TextBlock</span>&nbsp;x:<span class="xaml__attr_name">Name</span>=<span class="xaml__attr_value">&quot;txtInfo&quot;</span>&nbsp;<span class="xaml__attr_name">HorizontalAlignment</span>=<span class="xaml__attr_value">&quot;Left&quot;</span>&nbsp;<span class="xaml__attr_name">Margin</span>=<span class="xaml__attr_value">&quot;10,87,0,0&quot;</span>&nbsp;<span class="xaml__attr_name">TextWrapping</span>=<span class="xaml__attr_value">&quot;Wrap&quot;</span>&nbsp;<span class="xaml__attr_name">Text</span>=<span class="xaml__attr_value">&quot;Screen&nbsp;Resolution&quot;</span>&nbsp;<span class="xaml__attr_name">VerticalAlignment</span>=<span class="xaml__attr_value">&quot;Top&quot;</span>&nbsp;<span class="xaml__attr_name">Height</span>=<span class="xaml__attr_value">&quot;221&quot;</span>&nbsp;<span class="xaml__attr_name">Width</span>=<span class="xaml__attr_value">&quot;340&quot;</span><span class="xaml__tag_start">/&gt;</span>&nbsp;
<span class="xaml__tag_end">&lt;/Grid&gt;</span>&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="font-size:11pt; line-height:27.6pt">MainPage.xaml.cs</span></div>
<p>&nbsp;</p>
<p style="font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-autospace:none; margin:0pt">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;btnShow&nbsp;click&nbsp;event</span>&nbsp;
<span class="cs__com">///&nbsp;Get&nbsp;the&nbsp;screen&nbsp;resolution&nbsp;and&nbsp;show&nbsp;the&nbsp;information&nbsp;in&nbsp;the&nbsp;textblock.</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;sender&quot;&gt;&lt;/param&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;e&quot;&gt;&lt;/param&gt;</span>&nbsp;
<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;btnShow_Click(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;RoutedEventArgs&nbsp;e)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Clean&nbsp;the&nbsp;TextBlock</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.txtInfo.Text&nbsp;=&nbsp;<span class="cs__keyword">string</span>.Empty;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Add&nbsp;the&nbsp;screen&nbsp;resolution&nbsp;information&nbsp;to&nbsp;the&nbsp;textblock.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(App.ScreenResolutionSize&nbsp;!=&nbsp;Size.Empty)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;fullSize&nbsp;=&nbsp;App.ScreenResolutionSize;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.txtInfo.Text&nbsp;=&nbsp;<span class="cs__keyword">string</span>.Format(<span class="cs__string">&quot;The&nbsp;screen&nbsp;resolution&nbsp;is:&nbsp;{0}x{1}&quot;</span>,&nbsp;fullSize.Width,&nbsp;fullSize.Height);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;windowSize&nbsp;=&nbsp;ScreenResolutionHelper.GetScreenResolutionInfo();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//Add&nbsp;the&nbsp;application&nbsp;window's&nbsp;resolution&nbsp;information&nbsp;to&nbsp;the&nbsp;textblock.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(windowSize&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">this</span>.txtInfo.Text&nbsp;&#43;=&nbsp;(<span class="cs__keyword">string</span>.IsNullOrEmpty(<span class="cs__keyword">this</span>.txtInfo.Text)&nbsp;?&nbsp;<span class="cs__keyword">string</span>.Empty&nbsp;:&nbsp;Environment.NewLine)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&#43;&nbsp;<span class="cs__keyword">string</span>.Format(<span class="cs__string">&quot;The&nbsp;application&nbsp;window's&nbsp;resolution&nbsp;is:&nbsp;{0}x{1}&quot;</span>,&nbsp;windowSize.Width,&nbsp;windowSize.Height);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
&nbsp;
<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">class</span>&nbsp;ScreenResolutionHelper&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;Get&nbsp;screen&nbsp;resolution.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;If&nbsp;you&nbsp;want&nbsp;to&nbsp;get&nbsp;the&nbsp;resolution&nbsp;on&nbsp;every&nbsp;page&nbsp;in&nbsp;your&nbsp;solution,&nbsp;you&nbsp;need&nbsp;to&nbsp;call&nbsp;this&nbsp;method&nbsp;from&nbsp;app.xaml.cs&nbsp;and&nbsp;save&nbsp;the&nbsp;data&nbsp;as&nbsp;a&nbsp;global&nbsp;variable.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;If&nbsp;you&nbsp;have&nbsp;more&nbsp;than&nbsp;one&nbsp;computer&nbsp;monitor,&nbsp;you&nbsp;can&nbsp;only&nbsp;get&nbsp;the&nbsp;main&nbsp;monitor's&nbsp;screen&nbsp;resolution.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;returns&gt;&lt;/returns&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;Size&nbsp;GetScreenResolutionInfo()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;applicationView&nbsp;=&nbsp;ApplicationView.GetForCurrentView();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;displayInformation&nbsp;=&nbsp;DisplayInformation.GetForCurrentView();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;bounds&nbsp;=&nbsp;applicationView.VisibleBounds;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;scale&nbsp;=&nbsp;displayInformation.RawPixelsPerViewPixel;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;size&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Size(bounds.Width&nbsp;*&nbsp;scale,&nbsp;bounds.Height&nbsp;*&nbsp;scale);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;size;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span style="font-size:9.5pt; line-height:27.6pt">App.xaml.cs</span></div>
<p>&nbsp;</p>
<p style="font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-autospace:none; margin:0pt">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;strore&nbsp;the&nbsp;screen&nbsp;resolution&nbsp;size.</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;Size&nbsp;ScreenResolutionSize;&nbsp;
&nbsp;
<span class="cs__keyword">protected</span>&nbsp;<span class="cs__keyword">override</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;OnLaunched(LaunchActivatedEventArgs&nbsp;e)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&hellip;&hellip;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Window.Current.Activate();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ScreenResolutionSize&nbsp;=&nbsp;ScreenResolutionHelper.GetScreenResolutionInfo();&nbsp;
}</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span style="font-size:13pt; font-weight:bold; line-height:27.6pt">More Information</span></div>
<p>&nbsp;</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt">&nbsp;</span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:11pt">ApplicationView</span><span style="font-size:11pt"> class reference:
</span><span style="color:#0563c1; text-decoration:underline"><a href="https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.viewmanagement.applicationview.aspx" style="text-decoration:none">https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.viewmanagement.applicationview.aspx</a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="color:#0563c1; text-decoration:underline"><a href="https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.viewmanagement.applicationview.aspx" style="text-decoration:none"></a></span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:11pt; line-height:27.6pt; text-indent:-18pt">DisplayInformation</span><span style="font-size:11pt; line-height:27.6pt; text-indent:-18pt">
 class reference: </span><a href="https://msdn.microsoft.com/en-us/library/windows/apps/windows.graphics.display.displayinformation.aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">https://msdn.microsoft.com/en-us/library/windows/apps/windows.graphics.display.displayinformation.aspx</span></a></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a name="_GoBack"></a></span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
