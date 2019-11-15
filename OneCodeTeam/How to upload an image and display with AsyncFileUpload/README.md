# How to upload an image and display with AsyncFileUpload
## Requires
- Visual Studio 2012
## License
- MIT
## Technologies
- ASP.NET
- .NET
- Web App Development
## Topics
- Upload
## Updated
- 09/21/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src="https://aka.ms/onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em></em></div>
<p class="MsoNormal" style="text-align:center"><span class="info-text"><strong><span style="font-size:10.0pt; line-height:106%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black">How to use
<span class="SpellE">AsyncFileUpload</span> in ASP.NET to upload and display an image.</span></strong></span><span class="info-text"><strong><span style="line-height:106%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black">
</span></strong></span></p>
<p class="MsoNormal"><span class="info-text"><span style="font-size:10.0pt; line-height:106%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black">&nbsp;</span></span></p>
<p class="MsoNormal"><span class="info-text"><strong><span style="font-size:10.0pt; line-height:106%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black">Requirement:
</span></strong></span><span class="info-text"><span style="font-size:10.0pt; line-height:106%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black">T</span></span><span style="font-size:10.0pt; line-height:106%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black">o
 upload an image and display with <span class="SpellE">AsyncFileUpload</span> in ASP.NET project<span class="info-text">
</span></span></p>
<p class="MsoNormal"><strong>Technology</strong>: C#, ASP.NET, Visual Studio 2012, AJAX</p>
<p class="MsoNormal">The sample demonstrates <span class="info-text"><span style="font-size:10.0pt; line-height:106%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black">how to use
<span class="SpellE">AsyncFileUpload</span> in ASP.NET to upload and display an image.
</span></span></p>
<p class="MsoNormal"><span class="info-text"><span style="font-size:10.0pt; line-height:106%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black">&nbsp;</span></span></p>
<p class="MsoNormal" style="line-height:105%"><strong>To Run the sample</strong>:</p>
<p class="MsoNormalCxSpMiddle" style="margin-left:.25in; line-height:105%">&nbsp;</p>
<p class="MsoNormal" style="line-height:105%"><strong>Code Used: </strong></p>
<p class="MsoNormal" style="line-height:105%">Upload and Display code</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<strong></strong><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">protected</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">void</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"> AsyncFileUpload1_<span class="GramE">UploadedComplete(</span></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">object</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
 sender, <span class="SpellE">AjaxControlToolkit.<span style="color:#2b91af">AsyncFileUploadEventArgs</span></span> e)
</span></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">{
</span></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">string</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
<span class="SpellE">strPath</span> = <span class="SpellE">MapPath</span>(</span><span style="font-size:9.5pt; font-family:Consolas; color:#a31515; background:white">&quot;~/upload/&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">)
 &#43; </span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:#2b91af; background:white">Path</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">.GetFileName</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">(<span class="SpellE"><span class="GramE">e.FileName</span></span>);
</span></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">AsyncFileUpload1.SaveAs(<span class="SpellE">strPath</span>);
</span></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"><span class="SpellE">displayImage.ImageUrl</span> =
<span class="SpellE">strPath</span>; </span></p>
<p class="MsoNormal" style="line-height:105%"><span style="font-size:9.5pt; line-height:105%; font-family:Consolas; color:black; background:white">}</span><span style="font-size:9.5pt; line-height:105%; font-family:Consolas; color:black">
</span></p>
<p class="MsoNormal" style="margin-left:.5in"><span style="font-size:12.0pt; line-height:106%">&nbsp;</span></p>
<p class="MsoNormal" style="margin-left:.5in"><span style="font-size:12.0pt; line-height:106%">Need to add the following in the client side JavaScript.
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:.0001pt; margin-left:.5in; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">&lt;</span><span style="font-size:9.5pt; font-family:Consolas; color:maroon; background:white">script</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span><span style="font-size:9.5pt; font-family:Consolas; color:red; background:white">type</span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">=&quot;text/<span class="SpellE">javascript</span>&quot;&gt;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:.0001pt; margin-left:.5in; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"></span><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">function</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
<span class="SpellE"><span class="GramE">uploadComplete</span></span><span class="GramE">(</span>sender,
<span class="SpellE">args</span>) { </span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:.0001pt; margin-left:.5in; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white"></span><span class="SpellE"><span style="font-size:9.5pt; font-family:Consolas; color:blue; background:white">var</span></span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
<span class="SpellE">imageName</span> = <span class="SpellE">args.get_<span class="GramE">fileName</span></span><span class="GramE">(</span>);
</span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:.0001pt; margin-left:.5in; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">$get(</span><span style="font-size:9.5pt; font-family:Consolas; color:#a31515; background:white">&quot;<span class="SpellE">displayImage</span>&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">).<span class="SpellE">src</span>
 = </span><span class="GramE"><span style="font-size:9.5pt; font-family:Consolas; color:#a31515; background:white">&quot;./</span></span><span style="font-size:9.5pt; font-family:Consolas; color:#a31515; background:white">upload/&quot;</span><span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">
 &#43; <span class="SpellE">imageName</span>; </span></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:.0001pt; margin-left:.5in; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:black; background:white">}
</span></p>
<p class="MsoNormal" style="margin-left:.5in"><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:black; background:white"></span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:blue; background:white">&lt;/</span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:maroon; background:white">script</span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:blue; background:white">&gt;</span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:blue">
</span></p>
<p class="MsoNormal"><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:blue">&nbsp;</span></p>
<p class="MsoNormal" style="line-height:105%">Control to Use <span class="SpellE">
AsyncFileUpload</span>:</p>
<p class="MsoNormal" style="text-indent:.5in"><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:blue; background:white">&lt;</span><span class="SpellE"><span class="GramE"><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:maroon; background:white">ajaxToolkit</span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:blue; background:white">:</span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:maroon; background:white">AsyncFileUpload</span></span></span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:black; background:white">
</span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:red; background:white">ID</span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:blue; background:white">=&quot;AsyncFileUpload1&quot;</span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:black; background:white">
</span><span class="SpellE"><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:red; background:white">runat</span></span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:blue; background:white">=&quot;server&quot;</span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:black; background:white">
</span><span class="SpellE"><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:red; background:white">OnClientUploadComplete</span></span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:blue; background:white">=&quot;<span class="SpellE">uploadComplete</span>&quot;</span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:black; background:white">
</span><span class="SpellE"><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:red; background:white">OnUploadedComplete</span></span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:blue; background:white">=&quot;AsyncFileUpload1_UploadedComplete&quot;</span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:black; background:white">
</span><span style="font-size:9.5pt; line-height:106%; font-family:Consolas; color:blue; background:white">/&gt;</span><span style="font-size:12.0pt; line-height:106%">
</span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
