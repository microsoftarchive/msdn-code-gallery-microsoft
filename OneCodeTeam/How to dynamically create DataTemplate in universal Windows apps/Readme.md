# How to dynamically create DataTemplate in universal Windows apps
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Windows
- Windows 8
- Windows Phone 8
- Windows Store app
- Windows Store app Development
- Windows Phone Development
- Windows 8.1
- Windows Phone 8.1
## Topics
- DataTemplate
- universal app
## Updated
- 09/22/2016
## Description

<h1>
<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src=":-onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em>&nbsp;</em></div>
</h1>
<h1>How to create DataTemplate dynamically in universal Windows apps</h1>
<h2>Introduction</h2>
<p>This sample is upgraded from the Windows Store sample:</p>
<p><a href="https://code.msdn.microsoft.com/How-to-dynamically-875a548e">https://code.msdn.microsoft.com/How-to-dynamically-875a548e</a> (C#)</p>
<p><a href="https://code.msdn.microsoft.com/How-to-dynamically-1fc09032">https://code.msdn.microsoft.com/How-to-dynamically-1fc09032</a> (VB)</p>
<p>We demonstrate how to create DataTemplate dynamically in universal Windows apps. The GridView&rsquo;s ItemTemplate is created in code behind.</p>
<h2>Building the Sample</h2>
<p>1.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Start Visual Studio 2013 and select File &gt; Open &gt; Project/Solution.</p>
<p>2.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Go to the directory in which you download the sample. Go to the directory named for the sample, and double-click the Microsoft Visual Studio Solution (.sln) file.</p>
<p>3.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Press F7 or use Build &gt; Build Solution to build the sample.</p>
<h2>Running the Sample</h2>
<ol>
<li>Press F5 to debug the app, this screen will be displayed. The GridView&rsquo;s ItemTemplate is created in code behind.
</li></ol>
<p>&nbsp;<img id="132329" src="132329-23424324324.png" alt="" width="640" height="360" style="border:1px solid black"></p>
<p>&nbsp;</p>
<h2>Using the Code</h2>
<p>The code below shows how to create DataTemplate in code-behind and assign it to GridView control.</p>
<pre><div class="scriptcode"><div class="pluginEditHolder" pluginCommand="mceScriptCode"><div class="title"><span>C#</span><span>C&#43;&#43;</span></div><div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div><span class="hidden">csharp</span><span class="hidden">cplusplus</span><pre class="hidden">StringBuilder sb = new StringBuilder();
 
sb.Append(&quot;&lt;DataTemplate xmlns=\&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation\&quot;&gt;&quot;);
sb.Append(&quot;&lt;Grid Width=\&quot;200\&quot; Height=\&quot;100\&quot;&gt;&quot;);
sb.Append(&quot;&lt;StackPanel&gt;&quot;);
sb.Append(&quot;&lt;StackPanel Orientation=\&quot;Horizontal\&quot; Margin=\&quot;3,3,0,3\&quot;&gt;&lt;TextBlock Text=\&quot;Name:\&quot; Style=\&quot;{StaticResource AppBodyTextStyle}\&quot; Margin=\&quot;0,0,5,0\&quot;/&gt;&lt;TextBlock Text=\&quot;{Binding Name}\&quot; Style=\&quot;{StaticResource AppBodyTextStyle}\&quot;/&gt;&lt;/StackPanel&gt;&quot;);
sb.Append(&quot;&lt;StackPanel Orientation=\&quot;Horizontal\&quot; Margin=\&quot;3,3,0,3\&quot;&gt;&lt;TextBlock Text=\&quot;Price:\&quot; Style=\&quot;{StaticResource AppBodyTextStyle}\&quot; Margin=\&quot;0,0,5,0\&quot;/&gt;&lt;TextBlock Text=\&quot;{Binding Price}\&quot; Style=\&quot;{StaticResource AppBodyTextStyle}\&quot;/&gt;&lt;/StackPanel&gt;&quot;);
sb.Append(&quot;&lt;StackPanel Orientation=\&quot;Horizontal\&quot; Margin=\&quot;3,3,0,3\&quot;&gt;&lt;TextBlock Text=\&quot;Author:\&quot; Style=\&quot;{StaticResource AppBodyTextStyle}\&quot; Margin=\&quot;0,0,5,0\&quot;/&gt;&lt;TextBlock Text=\&quot;{Binding Author}\&quot; Style=\&quot;{StaticResource AppBodyTextStyle}\&quot;/&gt;&lt;/StackPanel&gt;&quot;);
sb.Append(&quot;&lt;/StackPanel&gt;&quot;);
sb.Append(&quot;&lt;/Grid&gt;&quot;);
sb.Append(&quot;&lt;/DataTemplate&gt;&quot;);
 
DataTemplate datatemplate = (DataTemplate)XamlReader.Load(sb.ToString());
BookGridView.ItemTemplate = datatemplate;
BookListView.ItemTemplate = datatemplate;</pre>
<pre class="hidden">Platform::String^ str = &quot;&lt;DataTemplate xmlns=\&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation\&quot;&gt;&quot;;
        str = String::Concat(str, &quot;&lt;Grid Width=\&quot;200\&quot; Height=\&quot;100\&quot;&gt;&quot;);
        str = String::Concat(str, &quot;&lt;StackPanel&gt;&quot;);
        str = String::Concat(str, &quot;&lt;StackPanel Orientation=\&quot;Horizontal\&quot; Margin=\&quot;10,3,0,3\&quot;&gt;&lt;TextBlock Text=\&quot;Name:\&quot; Style=\&quot;{StaticResource AppBodyTextStyle}\&quot; Margin=\&quot;0,0,5,0\&quot;/&gt;\
                                                  &lt;TextBlock Text=\&quot;{Binding Name}\&quot; Style=\&quot;{StaticResource AppBodyTextStyle}\&quot;/&gt;&lt;/StackPanel&gt;&quot;);
        str = String::Concat(str, &quot;&lt;StackPanel Orientation=\&quot;Horizontal\&quot; Margin=\&quot;10,3,0,3\&quot;&gt;&lt;TextBlock Text=\&quot;Price:\&quot; Style=\&quot;{StaticResource AppBodyTextStyle}\&quot; Margin=\&quot;0,0,5,0\&quot;/&gt;\
                                                  &lt;TextBlock Text=\&quot;{Binding Price}\&quot; Style=\&quot;{StaticResource AppBodyTextStyle}\&quot;/&gt;&lt;/StackPanel&gt;&quot;);
        str = String::Concat(str, &quot;&lt;StackPanel Orientation=\&quot;Horizontal\&quot; Margin=\&quot;10,3,0,3\&quot;&gt;&lt;TextBlock Text=\&quot;Author:\&quot; Style=\&quot;{StaticResource AppBodyTextStyle}\&quot; Margin=\&quot;0,0,5,0\&quot;/&gt;\
                                                  &lt;TextBlock Text=\&quot;{Binding Author}\&quot; Style=\&quot;{StaticResource AppBodyTextStyle}\&quot;/&gt;&lt;/StackPanel&gt;&quot;);
        str = String::Concat(str, &quot;&lt;/StackPanel&gt;&quot;);
        str = String::Concat(str, &quot;&lt;/Grid&gt;&quot;);
        str = String::Concat(str, &quot;&lt;/DataTemplate&gt;&quot;);
        
        DataTemplate^ datatemplate = (DataTemplate^)Windows::UI::Xaml::Markup::XamlReader::Load(str);
        BookGridView-&gt;ItemTemplate = datatemplate;</pre>
<div class="preview">
<pre class="csharp">StringBuilder&nbsp;sb&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;StringBuilder();&nbsp;
&nbsp;&nbsp;
sb.Append(<span class="cs__string">&quot;&lt;DataTemplate&nbsp;xmlns=\&quot;http://schemas.microsoft.com/winfx/2006/xaml/presentation\&quot;&gt;&quot;</span>);&nbsp;
sb.Append(<span class="cs__string">&quot;&lt;Grid&nbsp;Width=\&quot;200\&quot;&nbsp;Height=\&quot;100\&quot;&gt;&quot;</span>);&nbsp;
sb.Append(<span class="cs__string">&quot;&lt;StackPanel&gt;&quot;</span>);&nbsp;
sb.Append(<span class="cs__string">&quot;&lt;StackPanel&nbsp;Orientation=\&quot;Horizontal\&quot;&nbsp;Margin=\&quot;3,3,0,3\&quot;&gt;&lt;TextBlock&nbsp;Text=\&quot;Name:\&quot;&nbsp;Style=\&quot;{StaticResource&nbsp;AppBodyTextStyle}\&quot;&nbsp;Margin=\&quot;0,0,5,0\&quot;/&gt;&lt;TextBlock&nbsp;Text=\&quot;{Binding&nbsp;Name}\&quot;&nbsp;Style=\&quot;{StaticResource&nbsp;AppBodyTextStyle}\&quot;/&gt;&lt;/StackPanel&gt;&quot;</span>);&nbsp;
sb.Append(<span class="cs__string">&quot;&lt;StackPanel&nbsp;Orientation=\&quot;Horizontal\&quot;&nbsp;Margin=\&quot;3,3,0,3\&quot;&gt;&lt;TextBlock&nbsp;Text=\&quot;Price:\&quot;&nbsp;Style=\&quot;{StaticResource&nbsp;AppBodyTextStyle}\&quot;&nbsp;Margin=\&quot;0,0,5,0\&quot;/&gt;&lt;TextBlock&nbsp;Text=\&quot;{Binding&nbsp;Price}\&quot;&nbsp;Style=\&quot;{StaticResource&nbsp;AppBodyTextStyle}\&quot;/&gt;&lt;/StackPanel&gt;&quot;</span>);&nbsp;
sb.Append(<span class="cs__string">&quot;&lt;StackPanel&nbsp;Orientation=\&quot;Horizontal\&quot;&nbsp;Margin=\&quot;3,3,0,3\&quot;&gt;&lt;TextBlock&nbsp;Text=\&quot;Author:\&quot;&nbsp;Style=\&quot;{StaticResource&nbsp;AppBodyTextStyle}\&quot;&nbsp;Margin=\&quot;0,0,5,0\&quot;/&gt;&lt;TextBlock&nbsp;Text=\&quot;{Binding&nbsp;Author}\&quot;&nbsp;Style=\&quot;{StaticResource&nbsp;AppBodyTextStyle}\&quot;/&gt;&lt;/StackPanel&gt;&quot;</span>);&nbsp;
sb.Append(<span class="cs__string">&quot;&lt;/StackPanel&gt;&quot;</span>);&nbsp;
sb.Append(<span class="cs__string">&quot;&lt;/Grid&gt;&quot;</span>);&nbsp;
sb.Append(<span class="cs__string">&quot;&lt;/DataTemplate&gt;&quot;</span>);&nbsp;
&nbsp;&nbsp;
DataTemplate&nbsp;datatemplate&nbsp;=&nbsp;(DataTemplate)XamlReader.Load(sb.ToString());&nbsp;
BookGridView.ItemTemplate&nbsp;=&nbsp;datatemplate;&nbsp;
BookListView.ItemTemplate&nbsp;=&nbsp;datatemplate;</pre>
</div>
</div>
</div>
<div class="endscriptcode"></div>
</pre>
<h2>More Information</h2>
<p>XamlReader class</p>
<p><a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.markup.xamlreader">http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.markup.xamlreader</a></p>
<p>&nbsp;</p>
<p>XamlReader.Load method</p>
<p><a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.markup.xamlreader.load">http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.markup.xamlreader.load</a></p>
<p>&nbsp;</p>
<p>DataTemplate class</p>
<p><a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.datatemplate">http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.datatemplate</a></p>
<p>&nbsp;</p>
<p>GridView class</p>
<p><a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.gridview.aspx">http://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.controls.gridview.aspx</a></p>
