# Windows Azure HelloWorld Sample
## Requires
- Visual Studio 2010
## License
- Custom
## Technologies
- ASP.NET
- Microsoft Azure
## Topics
- Getting Started
## Updated
- 08/10/2012
## Description

<p><span style="font-size:x-small"><strong>Before you install and use&nbsp;Windows Azure HelloWorld Sample you must:</strong></span></p>
<ol>
<li><span style="font-size:x-small"><strong>Review the&nbsp;Windows Azure&nbsp;HelloWorld Sample&nbsp;license terms by clicking&nbsp;the Custom link above.</strong>
</span></li><li><span style="font-size:x-small"><strong>Print and retain a copy of the license terms for your records.</strong>
</span></li></ol>
<p><span style="font-size:x-small"><strong>By downloading and using&nbsp;the&nbsp;Windows Azure HelloWorld Sample,&nbsp;you agree to such license terms.&nbsp; If you do not accept them, do not use the software.</strong></span></p>
<h1><span style="font-size:large">Introduction</span></h1>
<div><span style="font-family:verdana,geneva; font-size:x-small"><span style="line-height:150%">The HelloWorld sample demonstrates how to create a simple ASP.NET web role that can be deployed to Windows Azure. The Diagnostics module is imported into the sample
 and is ready to collect the default diagnostic data. The default data that is collected is the Windows Azure logs and Windows Azure Diagnostic infrastructure logs. For more information, see&nbsp;<a href="http://msdn.microsoft.com/en-us/library/gg433048.aspx">Collecting
 Logging Data by Using Windows Azure Diagnostics</a>.</span></span></div>
<div><span style="font-family:verdana,geneva; font-size:x-small"><span style="line-height:150%">For more information about getting started, see
<a href="http://www.microsoft.com/windowsazure/getstarted/default.aspx">Windows Azure</a>.</span></span></div>
<h1><span style="font-size:large">Prerequisites</span></h1>
<div><span style="font-family:verdana,geneva; font-size:x-small"><span style="line-height:150%">You must install the Windows Azure Software Development Kit (SDK) 1.3 or later to run the HelloWorld sample. You can get the latest version at
<a href="http://www.microsoft.com/windowsazure/windowsazuresdk&#43;tools/">Windows Azure SDK Downloads</a>.
<div><span style="font-family:verdana,geneva; font-size:x-small"><span style="line-height:150%">The HelloWorld sample is configured by default to use the Windows Azure Storage Emulator, which is included in the SDK. The following XML code is included in the
 ServiceDefintion.csdef file:</span></span></div>
</span></span></div>
<div><span style="font-family:verdana,geneva; font-size:small"><span style="line-height:150%">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xml</span>

<div class="preview">
<pre class="js">&lt;ConfigurationSettings&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&lt;Setting&nbsp;name=<span class="js__string">&quot;Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString&quot;</span>&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;value=<span class="js__string">&quot;UseDevelopmentStorage=true&quot;</span>&nbsp;/&gt;&nbsp;
&lt;/ConfigurationSettings&gt;&nbsp;
&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode"><span style="line-height:150%; font-family:verdana,geneva; font-size:small"><span style="font-size:x-small">If you want to use Windows Azure storage, you must use the following value:</span>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xml</span>

<div class="preview">
<pre class="js">&lt;ConfigurationSettings&gt;&nbsp;
&nbsp;&nbsp;&nbsp;&lt;Setting&nbsp;name=<span class="js__string">&quot;Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString&quot;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;value=<span class="js__string">&quot;DefaultEndpointsProtocol=https;AccountName=AccountName;AccountKey=AccountKey&quot;</span>&nbsp;/&gt;&nbsp;
&lt;/ConfigurationSettings&gt;&nbsp;
&nbsp;
&nbsp;
</pre>
</div>
</div>
</div>
</span></div>
<span style="line-height:150%; font-family:verdana,geneva; font-size:small">
<div class="endscriptcode"><span style="line-height:150%; font-family:verdana,geneva; font-size:x-small">You must replace the
<span class="Placeholder">AccountName </span>and <span class="Placeholder">AccountKey
</span>with the values from your storage account. For more information about using storage, see
<a href="http://www.microsoft.com/windowsazure/storage/default.aspx">Windows Azure Storage</a>.</span></div>
</span></span></span></div>
<div>&nbsp;</div>
<h1><span style="font-size:large">Building and Running the Sample</span></h1>
<ol>
<li><span style="font-family:verdana,geneva; font-size:x-small"><span style="line-height:150%">Open Visual Studio 2010 as an administrator.</span></span>
</li><li><span style="font-family:verdana,geneva; font-size:x-small"><span style="line-height:150%"><span style="line-height:150%">Browse to the folder where you extracted the samples, then browse to the HelloWorld folder.</span></span></span>
</li><li><span style="font-family:verdana,geneva; font-size:x-small"><span style="line-height:150%"><span style="line-height:150%">Open HelloWorld.sln.</span></span></span>
</li><li><span style="font-family:verdana,geneva; font-size:x-small"><span style="line-height:150%"><span style="line-height:150%"><span style="line-height:150%">Press F6 to build the application from Visual Studio.
<span style="line-height:150%">&nbsp;</span></span></span></span></span> </li><li><span style="font-family:verdana,geneva; font-size:x-small"><span style="line-height:150%"><span style="line-height:150%"><span style="line-height:150%"><span style="line-height:150%">Press F5 to debug the application. When you debug or run the application
 from Visual<br>
Studio, the following actions are performed:</span></span></span></span></span>
<ol>
<li><span style="font-family:verdana,geneva; font-size:x-small"><span style="line-height:150%"><span style="line-height:150%"><span style="line-height:150%"><span style="line-height:150%">The application is packaged.</span></span></span></span></span>
</li><li><span style="font-family:verdana,geneva; font-size:x-small"><span style="line-height:150%"><span style="line-height:150%"><span style="line-height:150%"><span style="line-height:150%">The Windows Azure Compute Emulator is started.</span></span></span></span></span>
</li><li><span style="font-family:verdana,geneva; font-size:x-small"><span style="line-height:150%"><span style="line-height:150%"><span style="line-height:150%"><span style="line-height:150%">T<span style="line-height:150%">he application package is
<span style="line-height:150%">deployed to the Compute Emulator</span></span></span></span></span></span></span>
</li><li><span style="font-family:verdana,geneva; font-size:x-small"><span style="line-height:150%"><span style="line-height:150%"><span style="line-height:150%"><span style="line-height:150%"><span style="line-height:150%"><span style="line-height:150%"><span style="line-height:150%">The
 browser displays the default web page defined by the web role.</span></span></span></span></span></span></span></span>
</li></ol>
</li></ol>
