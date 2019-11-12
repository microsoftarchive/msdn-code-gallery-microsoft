# Search and Highlight in TextBlock (CSWPFSearchAndHighlightTextBlockControl)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- WPF
## Topics
- Controls
- TextBlock
## Updated
- 12/28/2011
## Description

<h1>Search and Highlight Keywords in TextBlock</h1>
<h2>Summary</h2>
<p>The WPF code sample demonstrates how to search and highlight keywords in a TextBlock control.</p>
<p><img src="48099-cswpfsearchandhighlighttextblockcontrol.png" alt="" width="522" height="354"></p>
<p>The sample includes a custom user control &quot;SearchableTextControl&quot; and its search method is used to demonstrate how to highlight keywords using <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Windows.Documents.Run.aspx" target="_blank" title="Auto generated link to System.Windows.Documents.Run">System.Windows.Documents.Run</a> and System.Windows.Documents.Incline.</p>
<h2><span>Demo</span></h2>
<p>Step1. Build this project in VS2010.</p>
<p>Step2. Run CSWPFSearchAndHighlightTextBlockControl.exe.</p>
<p>Step3. Input the source string after the &quot;Source Text&quot; TextBlock.</p>
<p>Step4. Type the keyword which you want to search in the source string after the &quot;Search&quot; TextBlock.</p>
<p>Step5. You can change the Background/Foreground colors by selecting the specific color&nbsp;in drop-list of combobox.</p>
<p><span style="font-size:medium; font-weight:bold">Code Logic</span></p>
<p>Step 1. Create a Visual C# WPF Application in Visual Studio 2010 and name it&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &quot;CSWPFSearchAndHighlightTextBlockControl&quot;.</p>
<p>Step 2. Create a class &quot;SearchableTextControl&quot;. Make sure it inherits from the &quot;Control&quot; class.</p>
<p>Step 3. The &quot;SearchableTextControl&quot; class uses DependencyProperty.Register to implement a render event bound to custom user control such as:</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;SearchText&nbsp;sandbox&nbsp;which&nbsp;is&nbsp;used&nbsp;to&nbsp;get&nbsp;or&nbsp;set&nbsp;the&nbsp;value&nbsp;from&nbsp;a&nbsp;dependency&nbsp;property,</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;if&nbsp;it&nbsp;gets&nbsp;a&nbsp;value,it&nbsp;should&nbsp;be&nbsp;forced&nbsp;to&nbsp;bind&nbsp;to&nbsp;a&nbsp;string&nbsp;type.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">string</span>&nbsp;SearchText&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;(<span class="cs__keyword">string</span>)GetValue(SearchTextProperty);&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">set</span>&nbsp;{&nbsp;SetValue(SearchTextProperty,&nbsp;<span class="cs__keyword">value</span>);&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;Real&nbsp;implementation&nbsp;about&nbsp;SearchTextProperty&nbsp;which&nbsp;registers&nbsp;a&nbsp;dependency&nbsp;property&nbsp;with&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;the&nbsp;specified&nbsp;property&nbsp;name,&nbsp;property&nbsp;type,&nbsp;owner&nbsp;type,&nbsp;and&nbsp;property&nbsp;metadata.&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">readonly</span>&nbsp;DependencyProperty&nbsp;SearchTextProperty&nbsp;=&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DependencyProperty.Register(<span class="cs__string">&quot;SearchText&quot;</span>,&nbsp;<span class="cs__keyword">typeof</span>(<span class="cs__keyword">string</span>),&nbsp;<span class="cs__keyword">typeof</span>(SearchableTextControl),&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">new</span>&nbsp;UIPropertyMetadata(<span class="cs__keyword">string</span>.Empty,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;UpdateControlCallBack));&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;Create&nbsp;a&nbsp;call&nbsp;back&nbsp;function&nbsp;which&nbsp;is&nbsp;used&nbsp;to&nbsp;invalidate&nbsp;the&nbsp;rendering&nbsp;of&nbsp;the&nbsp;element,&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;and&nbsp;force&nbsp;a&nbsp;complete&nbsp;new&nbsp;layout&nbsp;pass.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;One&nbsp;such&nbsp;advanced&nbsp;scenario&nbsp;is&nbsp;if&nbsp;you&nbsp;are&nbsp;creating&nbsp;a&nbsp;PropertyChangedCallback&nbsp;for&nbsp;a&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;dependency&nbsp;property&nbsp;that&nbsp;is&nbsp;not&nbsp;&nbsp;on&nbsp;a&nbsp;Freezable&nbsp;or&nbsp;FrameworkElement&nbsp;derived&nbsp;class&nbsp;that&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;still&nbsp;influences&nbsp;the&nbsp;layout&nbsp;when&nbsp;it&nbsp;changes.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;UpdateControlCallBack(DependencyObject&nbsp;d,&nbsp;DependencyPropertyChangedEventArgs&nbsp;e)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;SearchableTextControl&nbsp;obj&nbsp;=&nbsp;d&nbsp;<span class="cs__keyword">as</span>&nbsp;SearchableTextControl;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;obj.InvalidateVisual();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}</pre>
</div>
</div>
</div>
<p><span>Step 4. In SearchableTextControl class, there is a override method &quot;OnRender&quot;, this method uses&nbsp;String.IndexOf and String.Substring methods to match the target string, and we associate&nbsp;those methods with TextBlock.Inlines.Add method in this
 sample. And in order to implement&nbsp;the behavior which looks like capture of regular expression with several times.</span></p>
<p><span>Step 5. The method &quot;GenerateRun&quot; which in class SearchableTextControl is used to alternate the&nbsp;colors of all matched strings. And it add some features by adding the FontStyle property and&nbsp;the FontWeight property.<br>
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="js">private&nbsp;Run&nbsp;GenerateRun(string&nbsp;searchedString,&nbsp;bool&nbsp;isHighlight)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">if</span>&nbsp;(!string.IsNullOrEmpty(searchedString))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Run&nbsp;run&nbsp;=&nbsp;<span class="js__operator">new</span>&nbsp;Run(searchedString)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">{</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Background&nbsp;=&nbsp;isHighlight&nbsp;?&nbsp;<span class="js__operator">this</span>.HighlightBackground&nbsp;:&nbsp;<span class="js__operator">this</span>.Background,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Foreground&nbsp;=&nbsp;isHighlight&nbsp;?&nbsp;<span class="js__operator">this</span>.HighlightForeground&nbsp;:&nbsp;<span class="js__operator">this</span>.Foreground,&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;Set&nbsp;the&nbsp;source&nbsp;text&nbsp;with&nbsp;the&nbsp;style&nbsp;which&nbsp;is&nbsp;Italic.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FontStyle&nbsp;=&nbsp;isHighlight&nbsp;?&nbsp;FontStyles.Italic&nbsp;:&nbsp;FontStyles.Normal,&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__sl_comment">//&nbsp;Set&nbsp;the&nbsp;source&nbsp;text&nbsp;with&nbsp;the&nbsp;style&nbsp;which&nbsp;is&nbsp;Bold.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;FontWeight&nbsp;=&nbsp;isHighlight&nbsp;?&nbsp;FontWeights.Bold&nbsp;:&nbsp;FontWeights.Normal,&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">return</span>&nbsp;run;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__statement">return</span>&nbsp;null;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="js__brace">}</span></pre>
</div>
</div>
</div>
<h2>References</h2>
<p>Run Class <br>
<a href="http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k(<a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/SYSTEM.WINDOWS.DOCUMENTS.RUN.aspx" target="_blank" title="Auto generated link to SYSTEM.WINDOWS.DOCUMENTS.RUN">SYSTEM.WINDOWS.DOCUMENTS.RUN</a>);k(RUN);k(DevLang-CSHARP)&rd=true">http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&amp;l=EN-US&amp;k=k(<a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/SYSTEM.WINDOWS.DOCUMENTS.RUN.aspx" target="_blank" title="Auto generated link to SYSTEM.WINDOWS.DOCUMENTS.RUN">SYSTEM.WINDOWS.DOCUMENTS.RUN</a>);k(RUN);k(DevLang-CSHARP)&amp;rd=true</a></p>
<p>DependencyProperty.OverrideMetadata Method<br>
<a href="http://msdn.microsoft.com/en-us/library/system.windows.dependencyproperty.overridemetadata.aspx">http://msdn.microsoft.com/en-us/library/system.windows.dependencyproperty.overridemetadata.aspx</a></p>
<p>DependencyProperty.Register Method <br>
<a href="http://msdn.microsoft.com/en-us/library/system.windows.dependencyproperty.register.aspx">http://msdn.microsoft.com/en-us/library/system.windows.dependencyproperty.register.aspx</a></p>
<p>FrameworkTemplate.FindName Method <br>
<a href="http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&l=EN-US&k=k(<a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/SYSTEM.WINDOWS.FRAMEWORKTEMPLATE.FINDNAME.aspx" target="_blank" title="Auto generated link to SYSTEM.WINDOWS.FRAMEWORKTEMPLATE.FINDNAME">SYSTEM.WINDOWS.FRAMEWORKTEMPLATE.FINDNAME</a>);k(TargetFrameworkMoniker-%22.NETFRAMEWORK%2cVERSION%3dV4.0%22);k(DevLang-CSHARP)&rd=true">http://msdn.microsoft.com/query/dev10.query?appId=Dev10IDEF1&amp;l=EN-US&amp;k=k(<a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/SYSTEM.WINDOWS.FRAMEWORKTEMPLATE.FINDNAME.aspx" target="_blank" title="Auto generated link to SYSTEM.WINDOWS.FRAMEWORKTEMPLATE.FINDNAME">SYSTEM.WINDOWS.FRAMEWORKTEMPLATE.FINDNAME</a>);k(TargetFrameworkMoniker-%22.NETFRAMEWORK%2cVERSION%3dV4.0%22);k(DevLang-CSHARP)&amp;rd=true</a></p>
<p>UIElement.InvalidateVisual Method <br>
<a href="http://msdn.microsoft.com/en-us/library/system.windows.uielement.invalidatevisual.aspx">http://msdn.microsoft.com/en-us/library/system.windows.uielement.invalidatevisual.aspx</a></p>
<p>&nbsp;</p>
<p>&nbsp;</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt=""></a></div>
<p>&nbsp;</p>
