# How to retrieve the contents of the content control in Word using Open XML SDK
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Office
- Office Development
## Topics
- OpenXml
- retrieve the contents
- Content control
## Updated
- 12/11/2015
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="http://bit.ly/onecodesampletopbanner" alt="">
</a></div>
<h2><span style="font-size:14.0pt; line-height:115%">The project illustrates How to retrieve the contents of the content control using Open XML SDK
</span></h2>
<h2>Introduction</h2>
<p class="MsoNormal">Some customers want to get the values in content control using Open XML way, but there is no sample in MSDN. So if we have this sample, the sample will be helpful for customers which have the requirement.</p>
<p class="MsoNormal"><strong>Customer Evidence: </strong></p>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,sans-serif; font-weight:normal"><a href="http://social.msdn.microsoft.com/Forums/en-US/oxmlsdk/thread/ddeed0c3-d532-49c4-b9b1-c193fe94d2a4">http://social.msdn.microsoft.com/Forums/en-US/oxmlsdk/thread/ddeed0c3-d532-49c4-b9b1-c193fe94d2a4</a>
</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,sans-serif; font-weight:normal"><a href="http://social.msdn.microsoft.com/Forums/en-US/oxmlsdk/thread/449013e0-f885-489f-ba45-0bb080e7ad90/">http://social.msdn.microsoft.com/Forums/en-US/oxmlsdk/thread/449013e0-f885-489f-ba45-0bb080e7ad90/</a>
</span></h2>
<h2>Building the Project</h2>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
Open the project (ReadContentCtrlFromWordDoc.csproj) in the Visual Studio 2010 and build it.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp"><span class="cs__keyword">class</span>&nbsp;Program&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">static</span><span class="cs__keyword">void</span>&nbsp;Main(<span class="cs__keyword">string</span>[]&nbsp;args)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;appPath&nbsp;=&nbsp;Path.GetDirectoryName(<a class="libraryLink" href="https://msdn.microsoft.com/en-US/library/System.Reflection.Assembly.GetExecutingAssembly.aspx" target="_blank" title="Auto generated link to System.Reflection.Assembly.GetExecutingAssembly">System.Reflection.Assembly.GetExecutingAssembly</a>().Location);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;containingDocumentPath&nbsp;=&nbsp;appPath&nbsp;&#43;&nbsp;<span class="cs__string">&quot;\\ContainingDocument.docx&quot;</span>;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WordprocessingDocument&nbsp;document&nbsp;=&nbsp;WordprocessingDocument.Open(containingDocumentPath,&nbsp;<span class="cs__keyword">true</span>);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MainDocumentPart&nbsp;mainDocumentPart&nbsp;=&nbsp;document.MainDocumentPart;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;locate&nbsp;content&nbsp;control&nbsp;collection</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;sdtblocks&nbsp;=&nbsp;mainDocumentPart.Document.Descendants&lt;SdtBlock&gt;();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;contentCtrlData&nbsp;=&nbsp;<span class="cs__keyword">string</span>.Empty;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Iterate&nbsp;through&nbsp;the&nbsp;content&nbsp;control&nbsp;blocks&nbsp;and&nbsp;append&nbsp;data&nbsp;to&nbsp;a&nbsp;string&nbsp;object</span><span class="cs__keyword">foreach</span>&nbsp;(var&nbsp;sdtb&nbsp;<span class="cs__keyword">in</span>&nbsp;sdtblocks)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;contentCtrlData&nbsp;&#43;=&nbsp;sdtb.SdtContentBlock.InnerText;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Display&nbsp;the&nbsp;content&nbsp;control&nbsp;data&nbsp;to&nbsp;the&nbsp;user</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(contentCtrlData);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.ReadKey();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>(Exception&nbsp;ex)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Console.WriteLine(ex.Message);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
</pre>
</div>
</div>
</div>
<p></p>
<h2>Running the Sample</h2>
<ol>
<li>This sample reads the contents of content control from a word document by using OpenXML SDK and displays it into the console window.
</li><li>Copy &ldquo;ContainingDocument.docx&rdquo; from the project directory to the executable file location.
</li><li>Run the executable ReadContentCtrlFromWordDoc.exe and this will read the contents of content control and displays it into the console window.
</li><li>Open the document &ldquo;ContainingDocument.docx&rdquo; and verify its contents.
</li></ol>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
