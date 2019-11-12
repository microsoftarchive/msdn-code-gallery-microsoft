# How to add items to grouped Gridview dynamically in Universal Windows Platform
## Requires
- Visual Studio 2015
## License
- Apache License, Version 2.0
## Technologies
- Windows
- Windows Phone
- Windows Phone App Development
- Windows 10
- Universal Windows App Development
## Topics
- GridView
## Updated
- 09/22/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src=":-onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em>&nbsp;</em></div>
<h1><span lang="EN-US">How to add items to grouped <span class="SpellE">GridView</span> in Universal Windows Platform.
</span></h1>
<h2><span lang="EN-US">Introduction</span></h2>
<p class="MsoNormal" style="margin-bottom:.75pt"><span lang="EN-US" style="color:black">This sample demonstrates how to add an item dynamically to a grouped
<span class="SpellE">GridView</span> in Universal Windows Platform. </span><span lang="EN-US" style="font-size:12.0pt; line-height:115%; color:black">&nbsp;</span></p>
<h2><span lang="EN-US">Running the Sample</span></h2>
<p class="MsoListParagraphCxSpFirst" style="text-indent:5.0pt"><span lang="EN-US"><span>1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span lang="EN-US">Open this sample in Visual Studio 2015 on a Windows 10 machine, and press F5 to run it.</span></p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:5.0pt"><span lang="EN-US"><span>2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span lang="EN-US">After the app is launched, you will see the screen below.</span></p>
<p class="MsoListParagraphCxSpMiddle"><span lang="EN-US"><img src="139076-image.png" alt="" width="640" height="500" align="middle" style="vertical-align:middle">
</span><span lang="EN-US" style="font-size:12.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,serif; color:black">&nbsp;</span></p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:5.0pt"><span lang="EN-US" style="font-size:9.5pt; line-height:115%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black"><span>3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span lang="EN-US">Select the picture and a category from the
<span class="SpellE">ComboBox</span> controls and input the name of item. After clicking the &quot;Add Item&quot; button you will see the item appear in the list</span><span lang="EN-US" style="font-size:9.5pt; line-height:115%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black">.
</span></p>
<p class="MsoListParagraphCxSpLast"><span lang="EN-US"><img src="139077-image.png" alt="" width="650" height="500" align="middle" style="vertical-align:middle">
</span><span lang="EN-US" style="font-size:9.5pt; line-height:115%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black">&nbsp;</span></p>
<p class="MsoNormal"><span lang="EN-US">&nbsp;</span></p>
<h2><span lang="EN-US">Using the Code</span></h2>
<p class="MsoNormal"><span lang="EN-US">In <span class="SpellE">MainPage.xaml</span>, we bind
<span class="SpellE">GridView's</span> <span class="SpellE">itemsSource</span> to
<span class="SpellE">collectionViewSource</span> with &quot;<span class="SpellE">TwoWay</span>&quot; mode.</span></p>
<p class="MsoNormal"><span lang="EN-US">Set source in the code file:</span></p>
<p class="MsoNormal">&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">sealed</span>&nbsp;partial&nbsp;<span class="cs__keyword">class</span>&nbsp;MainPage&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;The&nbsp;data&nbsp;source&nbsp;for&nbsp;the&nbsp;grouped&nbsp;grid&nbsp;view.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;ObservableCollection&lt;GroupInfoCollection&lt;Item&gt;&gt;&nbsp;_source;&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;Initializes&nbsp;a&nbsp;new&nbsp;instance&nbsp;of&nbsp;the&nbsp;&lt;see&nbsp;cref=&quot;MainPage&quot;/&gt;&nbsp;class.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;MainPage()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;InitializeComponent();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_source&nbsp;=&nbsp;(<span class="cs__keyword">new</span>&nbsp;StoreData()).GetGroupsByCategory();&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;collectionViewSource.Source&nbsp;=&nbsp;_source;&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p>&nbsp;</p>
<p class="MsoNormal"><span lang="EN-US">The click event handler of the &quot;add item&quot; button:
</span></p>
<p class="MsoNormal">&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;btnAddItemClick(<span class="cs__keyword">object</span>&nbsp;sender,&nbsp;Windows.UI.Xaml.RoutedEventArgs&nbsp;e)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;path&nbsp;=&nbsp;<span class="cs__keyword">string</span>.Format(CultureInfo.InvariantCulture,&nbsp;<span class="cs__string">&quot;SampleData/Images/60{0}.png&quot;</span>,&nbsp;pictureComboBox.SelectedItem);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Item&nbsp;item&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Item&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Title&nbsp;=&nbsp;titleTextBox.Text,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Category&nbsp;=&nbsp;(<span class="cs__keyword">string</span>)groupComboBox.SelectedItem&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;};&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;item.SetImage(StoreData.BaseUri,&nbsp;path);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;GroupInfoCollection&lt;Item&gt;&nbsp;group&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;_source.Single(groupInfoList&nbsp;=&gt;&nbsp;groupInfoList.Key&nbsp;==&nbsp;item.Category);&nbsp;
&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;group.Add(item);&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p>&nbsp;</p>
<h2><span lang="EN-US">More Information</span></h2>
<p class="MsoNormal"><span lang="EN-US"><a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.groupstyle">GroupStyle Class</a></span></p>
<p class="MsoNormal">&nbsp;</p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
