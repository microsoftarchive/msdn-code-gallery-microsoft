# How to create an Azure SQL Database programmatically with Visual Studio 2015
## Requires
- Visual Studio 2015
## License
- Apache License, Version 2.0
## Technologies
- Azure
- Cloud
- Azure SQL Database
- Azure Data Services
## Topics
- Azure
## Updated
- 06/22/2017
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src=":-onecodesampletopbanner1" alt=""></a><strong></strong><em></em></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:24pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="color:#2e74b5">How to create a Microsoft Azure SQL Database programmatically</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">&nbsp;</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="color:#2e74b5">Build the sample</span></span></p>
<ol>
<li><span style="font-weight:bold; font-style:italic; font-size:11pt">&nbsp;</span><span style="font-size:11pt">Start Microsoft Visual Studio 2015 and select
</span><span style="font-weight:bold; font-size:11pt">File &gt; Open &gt; Project/Solution.</span>
</li><li><span style="font-size:11pt; line-height:27.6pt; text-indent:18pt">Go to the directory to which the sample was unzipped. Then go to the subdirectory named for the sample and double-click the Visual Studio 2015 Solution (.sln) file</span>
</li><li><span style="font-size:11pt; line-height:27.6pt; text-indent:18pt">&nbsp;Create and configure Active Directory</span>
</li></ol>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:18pt">
<span style="font-size:11pt"><span style="font-size:11pt">a.</span><span style="font-weight:bold; font-style:italic; font-size:11pt">
</span><span style="font-size:11pt">Go to </span><span style="font-weight:bold; font-size:11pt">Microsoft Azure</span><span style="font-size:11pt"> (portal.azure.com)</span></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:18pt">
<span style="font-size:11pt"><span style="font-size:11pt">b.</span><span style="font-weight:bold; font-style:italic; font-size:11pt">
</span><span style="font-size:11pt">Select </span><span style="font-weight:bold; font-size:11pt">Active Directory &gt; Applications &gt; Add</span></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:18pt">
<span style="font-size:11pt"><span style="font-size:11pt">c.</span><span style="font-weight:bold; font-style:italic; font-size:11pt">
</span><span style="font-size:11pt">Enter a </span><span style="font-weight:bold; font-size:11pt">Name</span><span style="font-size:11pt"> and choose
</span><span style="font-weight:bold; font-size:11pt">Native Client Application</span></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:18pt">
<span style="font-size:11pt"><span style="font-size:11pt">d.</span><span style="font-weight:bold; font-style:italic; font-size:11pt">
</span><span style="font-size:11pt">Enter a </span><span style="font-weight:bold; font-size:11pt">Redirect URI
</span><span style="font-size:11pt">(does not need to be an actual endpoint, just a valid URI)</span></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:18pt">
<span style="font-size:11pt"><span style="font-size:11pt">e.</span><span style="font-weight:bold; font-style:italic; font-size:11pt">
</span><span style="font-size:11pt">After AD is created, click </span><span style="font-weight:bold; font-size:11pt">Properties</span></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:18pt">
<span style="font-size:11pt"></span><span style="font-size:11pt">i.</span><span style="font-weight:bold; font-style:italic; font-size:11pt">
</span><span style="font-size:11pt">Take note of the </span><span style="font-weight:bold; font-size:11pt">Application ID</span><span style="font-size:11pt">(as clientID in the code) on this page for the next step</span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:18pt">
<span style="font-size:11pt"><span style="font-size:11pt">f.</span><span style="font-weight:bold; font-style:italic; font-size:11pt">
</span><span style="font-size:11pt">Click <strong>Required permissions</strong></span><span style="font-size:11pt">.</span></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:18pt">
<span style="font-size:11pt"><span style="font-size:11pt">g.</span><span style="font-weight:bold; font-style:italic; font-size:11pt">
</span><span style="font-size:11pt">Click&nbsp;</span><span style="font-weight:bold; font-size:11pt">Add</span><span style="font-size:11pt">&nbsp;button and select
<strong>Windows&nbsp;</strong></span><span style="font-weight:bold; font-size:11pt">Azure Service Management API</span><span style="font-size:11pt"> from the table and complete the wizard.</span></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:18pt">
<span style="font-size:11pt"><span style="font-size:11pt">h.</span><span style="font-weight:bold; font-style:italic; font-size:11pt">
</span><span style="font-size:11pt">Grant permission to access this API by selecting
</span><span style="font-weight:bold; font-size:11pt">Access Azure Service Management as organization users (preview)</span></span></p>
<p><span style="font-size:11pt; line-height:27.6pt; text-indent:18pt">&nbsp; &nbsp; 4. Add your variables to the program</span></p>
<ul>
<li><span style="font-size:11pt">azureSubscriptionId &ndash; From portal.azure.com in
</span><span style="font-weight:bold; font-size:11pt">Subscriptions</span> </li><li><span style="font-size:11pt">location &ndash; See </span><a href="https://azure.microsoft.com/en-us/pricing/details/sql-database/" style="text-decoration:none"><span style="color:#0563c1; font-size:11pt; text-decoration:underline">https://azure.microsoft.com/en-
 us/pricing/details/sql-database/</span></a><span style="font-size:11pt"> for more information</span>
</li><li><span style="font-size:11pt">edition &ndash; See </span><a href="https://azure.microsoft.com/en-us/pricing/details/sql-database/" style="text-decoration:none"><span style="color:#0563c1; font-size:11pt; text-decoration:underline">https://azure.microsoft.com/en-us/pricing/details/sql-database/</span></a><span style="font-size:11pt">
 for more information</span> </li><li><span style="font-size:11pt">requestedServiceObjectName &ndash; See </span><a href="https://azure.microsoft.com/en-us/pricing/details/sql-database/" style="text-decoration:none"><span style="color:#0563c1; font-size:11pt; text-decoration:underline">https://azure.microsoft.com/en-us/pricing/details/sql-database/</span></a><span style="font-size:11pt">
 for more information</span> </li><li><span style="font-size:11pt">resourceGroupName &ndash; From portal.azure.com in
</span><span style="font-weight:bold; font-size:11pt">Resource Groups</span><span style="font-size:11pt">, create new or select existing Resource group</span>
</li><li><span style="font-size:11pt">serverName &ndash; From portal.azure.com in </span>
<span style="font-weight:bold; font-size:11pt">Resource Groups</span><span style="font-size:11pt">, go into selected Resource group and
</span><span style="font-weight:bold; font-size:11pt">&#43;Add</span><span style="font-size:11pt"> new or select existing SQL server</span>
</li><li><span style="font-size:11pt; line-height:27.6pt; text-indent:18pt">databaseName &ndash; Name of your database</span>
</li><li><span style="font-size:11pt; line-height:27.6pt; text-indent:18pt">domainName &ndash; Go to portal.azure.com and hover over your name in the upper right corner and the Domain will appear in the pop-up window</span>
</li><li><span style="font-size:11pt; line-height:27.6pt; text-indent:18pt">clientId &ndash; From Step 3</span>
</li><li><span style="font-size:11pt; line-height:27.6pt; text-indent:18pt">clientAppUri &ndash; The Redirect URI from Step 3&nbsp;</span>
</li></ul>
<p><span style="font-size:11pt; line-height:27.6pt; text-indent:18pt">&nbsp; &nbsp; 5. Build the sample. Build &gt; Build Solution</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="color:#2e74b5">Run the sample</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">To debug the sample and then run it, press F5 or use
</span><span style="font-weight:bold; font-size:11pt">Debug &gt; Start Debugging</span><span style="font-size:11pt">. To run the sample without debugging, press Ctrl&#43;F5 or use
</span><span style="font-weight:bold; font-size:11pt">Debug &gt; Start Without Debugging</span><span style="font-size:11pt">.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
