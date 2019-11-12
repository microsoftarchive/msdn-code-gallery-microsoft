# How to zoom in/out the content in WebView in universal Windows apps
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Windows
- Windows Phone
- Windows 8
- Windows Store app Development
- Windows Phone Development
## Topics
- WebView
- universal app
## Updated
- 09/22/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"></a><a href="http://blogs.msdn.com/b/onecode"><img src=":-onecodesampletopbanner1" alt=""></a><strong></strong><em></em>
</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:24pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">How to zoom in/out the content in WebView in universal Windows apps</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Introduction</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">This code sample will show you how to zoom in/out the content in WebView in universal Windows apps.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">To resize the content of the WebView, we can manipulate the content by using JS/CSS that are fully supported by WebView.</span></span></p>
<h2 style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Video</span></span></h2>
<p><span style="font-size:11pt"><span style="font-size:11pt"><a href="http://channel9.msdn.com/Blogs/OneCode/How-to-zoom-inout-the-content-in-WebView-in-Univeral-Windows-apps" target="_blank"><img id="133570" src="133570-how%20to%20zoom%20in%20or%20out%20the%20content%20in%20webview%20in%20univeral%20apps%20%20%20channel%209.png" alt="" width="640" height="360" style="border:1px solid black"></a><br>
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Building the Sample</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Just build the solution in Visual Studio 2013.
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Note: to build and run Universal apps, you have to apply Visual Studio 2013 Update 2.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Running the Sample</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Make sure you've built the solution successfully. Then you can run the app.
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">If you launch the Windows Phone app, it will look like this:
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt"><img src="119270-image.png" alt="" width="287" height="527" align="middle">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">If you launch the Windows Store app, it will look like this:</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt"><img src="119271-image.png" alt="" width="687" height="426" align="middle">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Using the Code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Step1. Create a Universal app in Visual Studio.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Step2. Design the UI for both Windows and Windows Phone apps. The following code snippet only shows the page body code. For detailed information, please look into the MainPage.xaml file.</span><span>
</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XAML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xaml</span>

<pre class="xaml" id="codePreview">&lt;!-- page body --&gt;
&lt;Grid  Grid.Row=&quot;0&quot;&gt;
    &lt;Grid.RowDefinitions&gt;
        &lt;RowDefinition Height=&quot;auto&quot;&gt;&lt;/RowDefinition&gt;
        &lt;RowDefinition Height=&quot;*&quot;&gt;&lt;/RowDefinition&gt;
    &lt;/Grid.RowDefinitions&gt;
    &lt;StackPanel&gt;
        &lt;Slider Margin=&quot;10&quot; Minimum=&quot;1&quot; Maximum=&quot;200&quot; Value=&quot;100&quot; Width =&quot;300&quot; ValueChanged=&quot;Slider_ValueChanged&quot; SnapsTo=&quot;Ticks&quot; TickFrequency=&quot;1&quot; /&gt;
    &lt;/StackPanel&gt;
    &lt;WebView Grid.Row=&quot;1&quot; x:Name=&quot;MyWebView&quot; Source=&quot;ms-appx-web:///iframe.html&quot; Margin=&quot;0,0,0,10&quot; /&gt;
&lt;/Grid&gt;
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>&nbsp;</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Step3. Create an html file named &quot;iframe.html&quot;, in which we embed an
</span><span style="font-weight:bold">iframe </span><span style="font-size:11pt">element which hosts a web site source. The JavaScript function &quot;</span><span style="font-weight:bold">ZoomFunction</span><span style="font-size:11pt">&quot; is used to zoom in/out the
 web page.</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">html</span>

<pre class="html" id="codePreview">&lt;!DOCTYPE html&gt;
&lt;html lang=&quot;en&quot; xmlns=&quot;http://www.w3.org/1999/xhtml&quot;&gt;
&lt;head&gt;
    &lt;meta charset=&quot;utf-8&quot; /&gt;
    &lt;title&gt;&lt;/title&gt;
    &lt;script type=&quot;text/javascript&quot;&gt;
        var OriginalWidth = 0;
        var OriginalHeight = 0;
        function ZoomFunction(Percentage) {
            var mybody = document.getElementById(&quot;mybody&quot;);
            var myframe = document.getElementById(&quot;myiframe&quot;);
           
            if (OriginalWidth == 0 &amp;&amp; OriginalHeight == 0) {
                OriginalWidth = myframe.style.width.replace(&quot;px&quot;, &quot;&quot;);
                OriginalHeight = myframe.style.height.replace(&quot;px&quot;, &quot;&quot;);
            }
            
            if (Percentage &lt; 100) {
                mybody.style.overflow = &quot;hidden&quot;;
                myframe.style.overflowY = &quot;auto&quot;;
                NewWidth = (100 * OriginalWidth) / Percentage;
                NewHeight = (100 * OriginalHeight) / Percentage;
            }
            else if (Percentage == 100) {
                mybody.style.overflow = &quot;hidden&quot;;
                myframe.style.overflowY = &quot;auto&quot;;
                NewWidth = OriginalWidth;
                NewHeight = OriginalHeight;
            }
            else {
                mybody.style.overflow = &quot;auto&quot;;
                myframe.style.overflowY = &quot;hidden&quot;;
                NewWidth = OriginalWidth * (Percentage / 100);
                NewHeight = OriginalHeight * (Percentage / 10);
            }
            myframe.style.zoom = Percentage &#43; &quot;%&quot;;
            myframe.style.width = NewWidth &#43; &quot;px&quot;;
            myframe.style.height = NewHeight &#43; &quot;px&quot;;
        }
    &lt;/script&gt;
&lt;/head&gt;
&lt;body id=&quot;mybody&quot; style=&quot;margin:0px;padding:0px; overflow:hidden;&quot;&gt;
    &lt;iframe id=&quot;myiframe&quot; src=&quot;http://www.example.com/&quot; style=&quot;width:1200px; height:800px; overflow-X:auto; overflow-y:auto;&quot;&gt;&lt;/iframe&gt;
&lt;/body&gt;
&lt;/html&gt;
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>&nbsp;</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>Step4. In the code-behind file of MainPage.xaml, add the following code snippet to make sure
</span><a name="_GoBack"></a><span>the WebView control will call the </span><span style="font-weight:bold">ZoomFunction</span><span>.</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">cplusplus</span>


<pre class="csharp" id="codePreview">private async void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
{
    if (MyWebView != null)
        await MyWebView.InvokeScriptAsync(&quot;eval&quot;, new string[] { &quot;ZoomFunction(&quot; &#43; e.NewValue.ToString() &#43; &quot;);&quot; });
}
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>&nbsp;</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">More Information</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Build universal Windows apps that target Windows and Windows Phone</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="http://msdn.microsoft.com/en-us/library/windows/apps/dn609832.aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/windows/apps/dn609832.aspx</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Using Visual Studio to build Universal XAML Apps</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="http://blogs.msdn.com/b/visualstudio/archive/2014/04/14/using-visual-studio-to-build-universal-xaml-apps.aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://blogs.msdn.com/b/visualstudio/archive/2014/04/14/using-visual-studio-to-build-universal-xaml-apps.aspx</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">WebView class</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.webview.aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.webview.aspx</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">WebView Magic Tricks: Zoom Levels</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="http://blogs.msdn.com/b/wsdevsol/archive/2013/05/28/webview-magic-tricks-zoom-levels.aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://blogs.msdn.com/b/wsdevsol/archive/2013/05/28/webview-magic-tricks-zoom-levels.aspx</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
