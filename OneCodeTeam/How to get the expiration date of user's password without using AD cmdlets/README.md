# How to get the expiration date of user's password without using AD cmdlets
## Requires
- Visual Studio 2012
## License
- MIT
## Technologies
- Active Directory
- User Accounts
## Topics
- AD
## Updated
- 09/21/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src=":-onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em>&nbsp;</em><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"></a></div>
<div></div>
<div><strong style="text-align:center">How to get the expiration date of user's password in VB.NET application - without using AD cmdlets</strong></div>
<p class="MsoNormal"><strong>&nbsp;</strong></p>
<p class="MsoNormal"><strong>Requirement:</strong> How to get the expiration date of user's password in VB.NET application - without using AD cmdlets.</p>
<p class="MsoNormal"><strong>Technology: </strong>VB, VB.NET, Visual Studio 2012</p>
<p class="MsoNormal">The sample demonstrates how to<span class="info-text"><span style="font-size:10.0pt; line-height:107%; font-family:&quot;Segoe UI&quot;,sans-serif; color:black">
</span></span>get the expiration date of user's password in VB.NET application - without using AD cmdlets.</p>
<p class="MsoNormal">&nbsp;</p>
<p class="MsoNormal" style="line-height:105%"><strong>To Run the sample</strong>:</p>
<ol>
<li>Open project VBPasswordExpiry2012.sln in Visual Studio 2012. </li><li>Run the application. </li><li>Enter a valid domain name </li><li>Enter a valid username </li><li>The password validity days are displayed for that user name </li></ol>
<p class="MsoNormal" style="line-height:105%">&nbsp;</p>
<p class="MsoNormal" style="line-height:105%"><strong>Code Used: </strong></p>
<p class="MsoNormal" style="line-height:105%">To get the Expiration date of password.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
&nbsp;</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Private</span>&nbsp;<span class="visualBasic__keyword">Function</span>&nbsp;GetTimeRemainingUntilPasswordExpiration(<span class="visualBasic__keyword">ByVal</span>&nbsp;domain&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>,&nbsp;<span class="visualBasic__keyword">ByVal</span>&nbsp;userName&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>)&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;TimeSpan&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Using</span>&nbsp;userEntry&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;<a class="libraryLink" href="https://msdn.microsoft.com/en-US/library/System.DirectoryServices.DirectoryEntry.aspx" target="_blank" title="Auto generated link to System.DirectoryServices.DirectoryEntry">System.DirectoryServices.DirectoryEntry</a>(<span class="visualBasic__keyword">String</span>.Format(<span class="visualBasic__string">&quot;WinNT://{0}/{1},user&quot;</span>,&nbsp;domain,&nbsp;userName))&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;maxPasswordAge&nbsp;=&nbsp;<span class="visualBasic__keyword">CInt</span>(userEntry.Properties.Cast(<span class="visualBasic__keyword">Of</span>&nbsp;<a class="libraryLink" href="https://msdn.microsoft.com/en-US/library/System.DirectoryServices.PropertyValueCollection.aspx" target="_blank" title="Auto generated link to System.DirectoryServices.PropertyValueCollection">System.DirectoryServices.PropertyValueCollection</a>)().First(<span class="visualBasic__keyword">Function</span>(p)&nbsp;p.PropertyName&nbsp;=&nbsp;<span class="visualBasic__string">&quot;MaxPasswordAge&quot;</span>).Value)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;passwordAge&nbsp;=&nbsp;<span class="visualBasic__keyword">CInt</span>(userEntry.Properties.Cast(<span class="visualBasic__keyword">Of</span>&nbsp;<a class="libraryLink" href="https://msdn.microsoft.com/en-US/library/System.DirectoryServices.PropertyValueCollection.aspx" target="_blank" title="Auto generated link to System.DirectoryServices.PropertyValueCollection">System.DirectoryServices.PropertyValueCollection</a>)().First(<span class="visualBasic__keyword">Function</span>(p)&nbsp;p.PropertyName&nbsp;=&nbsp;<span class="visualBasic__string">&quot;PasswordAge&quot;</span>).Value)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Return</span>&nbsp;TimeSpan.FromSeconds(maxPasswordAge)&nbsp;-&nbsp;TimeSpan.FromSeconds(passwordAge)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Using</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Function</span>&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p>&nbsp;</p>
<p class="MsoNormal">&nbsp;</p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
