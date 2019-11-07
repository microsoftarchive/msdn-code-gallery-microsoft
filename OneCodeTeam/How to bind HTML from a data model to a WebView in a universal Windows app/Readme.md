# How to bind HTML from a data model to a WebView in a universal Windows app
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Windows
- Windows 8
- Windows Phone 8
- Windows Store app Development
- Windows Phone Development
- Windows 8.1
- Windows Phone 8.1
## Topics
- WebView
- universal app
## Updated
- 11/02/2014
## Description

<h1><span lang="EN"><span class="TextRun SCX96579621"><span class="NormalTextRun SCX96579621">How to bind HTML from a data model to a&nbsp;</span><span class="SpellingError SCX96579621">WebView</span></span><span class="TextRun SCX96579621">&nbsp;in&nbsp;</span><span class="TextRun SCX96579621">a
 universal Windows app</span></span></h1>
<h2><span>Introduction </span></h2>
<p class="MsoNormal">​It's been said that if you aren't data binding you aren't really using Xaml, and a frequent topic on the forums is how to bind HTML from a data model to a WebView.</p>
<p class="MsoNormal">The problem: we bind data to properties, but the WebView doesn't have a property to bind to. An app sets the WebView's HTML by calling the NavigateToString method.</p>
<p class="MsoNormal">The solution: create our own property to bind to.</p>
<p class="MsoNormal">NOTE: This code sample is based on following blog:</p>
<p class="MsoNormal"><span lang="EN"><a href="http://blogs.msdn.com/b/wsdevsol/archive/2013/09/26/binding-html-to-a-webview-with-attached-properties.aspx">http://blogs.msdn.com/b/wsdevsol/archive/2013/09/26/binding-html-to-a-webview-with-attached-properties.aspx</a>
</span></p>
<h2><span>Running the Sample </span></h2>
<p class="MsoNormal"><span><span class="TextRun SCX26119909">Build the sample in Visual Studio 201</span><span class="TextRun SCX26119909">3</span><span class="TextRun SCX26119909">, and then run it.</span><span class="EOP SCX26119909">&nbsp;</span></span></p>
<h2><span>Using the Code </span></h2>
<p class="MsoNormal"><span lang="EN">Windows.UI.Xaml has the concept of attached properties which let us create a stand-alone property which can &quot;attach&quot; to any object, such as our WebView. We can create an attached property &quot;HTML&quot; that when changed calls
 NavigateToString. We can then bind it to HTML data in our data model. First create a new class to hold the attached property, then create the attached property inside it.
</span></p>
<p class="MsoNormal"><span lang="EN">Visual Studio includes a snippet to help with this: type &quot;propa&quot;&lt;tab&gt;&lt;tab&gt; and the basic skeleton for an attached property will be inserted. Change the return type to &quot;string&quot; and the name to something appropriate.
 I named my property &quot;HTML&quot;. The snippet will take care of synchronizing your change to the whole property. Set the ownerType in the RegisterAttached call to the name of your class.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">class MyExtensions 
{ 
&nbsp;&nbsp;&nbsp;&nbsp;public static string GetHTML(DependencyObject obj) 
&nbsp;&nbsp;&nbsp;&nbsp;{ 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return (string)obj.GetValue(HTMLProperty); 
&nbsp;&nbsp;&nbsp;&nbsp;} 


&nbsp;&nbsp;&nbsp; public static void SetHTML(DependencyObject obj, string value) 
&nbsp;&nbsp;&nbsp;&nbsp;{ 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;obj.SetValue(HTMLProperty, value); 
&nbsp;&nbsp;&nbsp;&nbsp;} 


&nbsp;&nbsp;&nbsp; // Using a DependencyProperty as the backing store for HTML.&nbsp; This enables animation, styling, binding, etc... 
&nbsp;&nbsp;&nbsp;&nbsp;public static readonly DependencyProperty HTMLProperty = 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DependencyProperty.RegisterAttached(&quot;HTML&quot;, typeof(string), typeof(MyExtensions), new PropertyMetadata(0));&nbsp;&nbsp;&nbsp; 
}

</pre>
<pre id="codePreview" class="csharp">class MyExtensions 
{ 
&nbsp;&nbsp;&nbsp;&nbsp;public static string GetHTML(DependencyObject obj) 
&nbsp;&nbsp;&nbsp;&nbsp;{ 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;return (string)obj.GetValue(HTMLProperty); 
&nbsp;&nbsp;&nbsp;&nbsp;} 


&nbsp;&nbsp;&nbsp; public static void SetHTML(DependencyObject obj, string value) 
&nbsp;&nbsp;&nbsp;&nbsp;{ 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;obj.SetValue(HTMLProperty, value); 
&nbsp;&nbsp;&nbsp;&nbsp;} 


&nbsp;&nbsp;&nbsp; // Using a DependencyProperty as the backing store for HTML.&nbsp; This enables animation, styling, binding, etc... 
&nbsp;&nbsp;&nbsp;&nbsp;public static readonly DependencyProperty HTMLProperty = 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DependencyProperty.RegisterAttached(&quot;HTML&quot;, typeof(string), typeof(MyExtensions), new PropertyMetadata(0));&nbsp;&nbsp;&nbsp; 
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span lang="EN">At this point we have a property which can be set to a string, but it isn't connected to the WebView's document. To do that we need to add a change handler to the property's PropertyMetaData:
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public static readonly DependencyProperty HTMLProperty = 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DependencyProperty.RegisterAttached(&quot;HTML&quot;, typeof(string), typeof(MyExtensions), new PropertyMetadata(&quot;&quot;,new PropertyChangedCallback(OnHTMLChanged)));


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; private static void OnHTMLChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; WebView wv = d as WebView; 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;if (wv != null) 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{ 
wv.NavigateToString((string)e.NewValue); 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;} 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}

</pre>
<pre id="codePreview" class="csharp">public static readonly DependencyProperty HTMLProperty = 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DependencyProperty.RegisterAttached(&quot;HTML&quot;, typeof(string), typeof(MyExtensions), new PropertyMetadata(&quot;&quot;,new PropertyChangedCallback(OnHTMLChanged)));


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; private static void OnHTMLChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; WebView wv = d as WebView; 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;if (wv != null) 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{ 
wv.NavigateToString((string)e.NewValue); 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;} 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span lang="EN">Now all we need to do is to reference the attached property in our Xaml to bind the WebView to HTML from an &quot;HTMLText&quot; property in our data model:
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XAML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xaml</span>
<pre class="hidden">&lt;WebView local:MyExtensions.HTML=&quot;{Binding HTMLText}&quot;&gt;&lt;/WebView&gt;

</pre>
<pre id="codePreview" class="xaml">&lt;WebView local:MyExtensions.HTML=&quot;{Binding HTMLText}&quot;&gt;&lt;/WebView&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span lang="EN">The same technique can be used to enable binding RTF into a
<span class="SpellE">RichEditBox</span> and for other controls which ones sets by method rather than properties.
</span></p>
<h2><span>More Information </span></h2>
<p class="MsoNormal"><span lang="EN">For more on Dependency Properties and Attached Properties see:<br>
<a title="Dependency properties overview" href="http://msdn.microsoft.com/en-us/library/windows/apps/hh700353.aspx">Dependency properties overview</a><br>
<a title="Attached properties overview" href="http://msdn.microsoft.com/en-us/library/windows/apps/hh758282.aspx">Attached properties overview<br>
</a><a title="Custom attached properties" href="http://msdn.microsoft.com/en-us/library/windows/apps/hh965327.aspx">Custom attached properties</a></span><span>
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
