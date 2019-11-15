# Send emails using the SendGrid service in Azure
## Requires
- Visual Studio 2015
## License
- Apache License, Version 2.0
## Technologies
- Azure
- Cloud
## Topics
- Azure
- Email
- SendGrid
## Updated
- 09/22/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src="https://aka.ms/onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em>&nbsp;</em></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:24pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">How to send emails using Azure</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Introduction</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">A console application that uses the
</span><a href="http://sendgrid.com" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">Sen</span><span style="color:#0563c1; text-decoration:underline">d</span><span style="color:#0563c1; text-decoration:underline">Grid</span></a><span style="font-size:11pt">
 email service to send messages to </span><span style="font-size:11pt">multiple recipients</span><span style="font-size:11pt">.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Azure customers can unlock 25,000 free emails each month. These free monthly emails will give you access to advanced reporting and analytics and all APIs. See
</span><a href="https://sendgrid.com/features" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">SendGrid Features</span></a><span style="font-size:11pt"> page.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Building the Sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:25.9pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-size:11pt">1. Start Microsoft Visual Studio 2015 and select
</span><span style="font-weight:bold">File &gt; Open &gt; Project/Solution.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:25.9pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-size:11pt">2. Go to the directory to which the sample was unzipped. Then go to the subdirectory named for the sample and double-click the Visual Studio 2015 Solution (.</span><span style="font-size:11pt">sln</span><span style="font-size:11pt">)
 file</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-size:11pt">3. Create a </span><span style="font-size:11pt">SendGrid</span><span style="font-size:11pt"> account</span></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-size:11pt">a. Log in to the </span>
<a href="http://portal.azure.com/" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">Microsoft Az</span><span style="color:#0563c1; text-decoration:underline">u</span><span style="color:#0563c1; text-decoration:underline">re
 Portal</span></a></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-size:11pt">b. Go to </span><span style="font-weight:bold">New</span><span style="font-size:11pt"> and type &ldquo;</span><span style="font-size:11pt">SendGrid</span><span style="font-size:11pt">&rdquo; into the
</span><span style="font-size:11pt">search bar</span></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-size:11pt">c. Select </span><span style="font-weight:bold">SendGrid</span><span style="font-weight:bold"> Email Delivery</span><span style="font-size:11pt"> and click
</span><span style="font-weight:bold">Creat</span><span style="font-weight:bold">e</span></span></p>
<p><img class="WACImage SCX123973873" src="https://word-edit.officeapps.live.com/we/ResReader.ashx?v=00000000-0000-0000-0000-000000000014&n=E2o2.img&rndm=fddb42f2-2c60-42a7-99ec-124486a08043&WOPIsrc=https%3A%2F%2Fmicrosoft%2Esharepoint%2Ecom%2Fteams%2Fonecode%2F%5Fvti%5Fbin%2Fwopi%2Eashx%2Ffiles%2Fbbb42001db0a4b35ae9ba851797a4f08&access_token=eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImlZR1ZYcU1YbDZuSkI5MldBdW9jN1V3RHI0WSJ9%2EeyJhdWQiOiJ3b3BpL21pY3Jvc29mdC5zaGFyZXBvaW50LmNvbUA3MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDciLCJpc3MiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDBAOTAxNDAxMjItODUxNi0xMWUxLThlZmYtNDkzMDQ5MjQwMTliIiwibmJmIjoiMTQ1NjgyMDI3NiIsImV4cCI6IjE0NTY4NTYyNzYiLCJuYW1laWQiOiIwIy5mfG1lbWJlcnNoaXB8c2hlcnJ5d0BtaWNyb3NvZnQuY29tIiwibmlpIjoibWljcm9zb2Z0LnNoYXJlcG9pbnQiLCJpc3VzZXIiOiJ0cnVlIiwiY2FjaGVrZXkiOiIwaC5mfG1lbWJlcnNoaXB8MTAwMzAwMDA5MTQxYzY5NUBsaXZlLmNvbSIsImlzbG9vcGJhY2siOiJUcnVlIiwiYXBwY3R4IjoiYmJiNDIwMDFkYjBhNGIzNWFlOWJhODUxNzk3YTRmMDg7c1ZtbkxOWURWd2dubCtuUXVkQXBEeVdvMkdjPTtEZWZhdWx0OzsxQjAzQzQzMTI2NztUcnVlOyJ9%2EFd50QaTfqRKYozjy4KAhlUQRktCfat9sJC0tjQbukCLTM4CYwv9cAfojE%2DibBhnqe2dQyDV%2DTW7P60CFctxBW8AOOhFB6UrTevWbHCy2QRgshHB1hGv6b4a3pKN66ASaNhAXzJLZgPdRHR1S3sWo5GqK%5FvbP5DgL4w%5Ff94hUX3R9TOSV8SqFLcWzIFc8DsDZltuWm45H5jzZWyPMtD0mSXfWWus0JAcnwtv1QGj2CX0lrOKb4j3D4INr2XhdcaS1HPtBAXeEMCJ6zcVA7obRYC6Xzd1BnLDwS7RwjaG2CbNFmu2xdPNnIuvlSwAuy7NjsmxnjiL0uRDi81CEUsViLA&access_token_ttl=1456856276632&usid=8c853755-4d2d-483d-bc67-f928947234bd&build=16.0.6721.2200&waccluster=SG1B" alt="Image" style="display:block; margin-left:auto; margin-right:auto"></p>
<ol class="NumberListStyle2">
</ol>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-size:11pt">&nbsp;</span></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>d.&nbsp;</span><span style="font-size:11pt">Choose a name and password for your account and select an appropriate</span><span style="font-size:11pt"> pricing tier
</span><span style="font-size:11pt">(</span><span style="font-size:11pt">Azure customers can unlock 25,000 free emails each month</span><span style="font-size:11pt">)</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">4. &nbsp;<span style="font-size:11pt">Add your properties to the program</span></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span style="font-size:11pt">fromProperty</span><span style="font-size:11pt"> &ndash; email address you&rsquo;d like to seen as</span></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span style="font-size:11pt">recipientsProperty</span><span style="font-size:11pt"> &ndash; the email addresses you&rsquo;d like to receive the message</span></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span style="font-size:11pt">subjectProperty</span><span style="font-size:11pt"> &ndash; subject of message</span></span></p>
<p style="margin-left:72pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span style="font-size:11pt">HTMLContentProperty</span><span style="font-size:11pt"> &ndash; the message in HTML format</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt">5. &nbsp;<span style="font-size:11pt">Enter </span><span style="font-size:11pt">SendGrid</span><span style="font-size:11pt"> credentials to the program</span><span style="font-size:11pt">.
</span><span style="font-size:11pt">The username and password can be found from the Azure Portal in
</span><span style="font-weight:bold">Keys</span><span style="font-size:11pt">.</span><span style="font-size:11pt">&nbsp;</span></span></p>
<p class="Paragraph SCX189880547" style="display:inline!important"><span class="TextRun SCX189880547"><span class="NormalTextRun SCX189880547">&nbsp;</span></span><span class="WACImageContainer BlobObject SCX189880547">&nbsp;</span></p>
<div class="WACAltTextDescribedBy SCX189880547" id="x_{e8682b6e-16fc-4eba-8440-b3d172342eae}{187}">
</div>
<p style="text-align:center"><img class="WACImage SCX189880547" src="https://word-edit.officeapps.live.com/we/ResReader.ashx?v=00000000-0000-0000-0000-000000000014&n=E2o3.img&rndm=b4439033-8a80-477b-af93-6ce1520c84fd&WOPIsrc=https%3A%2F%2Fmicrosoft%2Esharepoint%2Ecom%2Fteams%2Fonecode%2F%5Fvti%5Fbin%2Fwopi%2Eashx%2Ffiles%2Fbbb42001db0a4b35ae9ba851797a4f08&access_token=eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImlZR1ZYcU1YbDZuSkI5MldBdW9jN1V3RHI0WSJ9%2EeyJhdWQiOiJ3b3BpL21pY3Jvc29mdC5zaGFyZXBvaW50LmNvbUA3MmY5ODhiZi04NmYxLTQxYWYtOTFhYi0yZDdjZDAxMWRiNDciLCJpc3MiOiIwMDAwMDAwMy0wMDAwLTBmZjEtY2UwMC0wMDAwMDAwMDAwMDBAOTAxNDAxMjItODUxNi0xMWUxLThlZmYtNDkzMDQ5MjQwMTliIiwibmJmIjoiMTQ1NjgyMDI3NiIsImV4cCI6IjE0NTY4NTYyNzYiLCJuYW1laWQiOiIwIy5mfG1lbWJlcnNoaXB8c2hlcnJ5d0BtaWNyb3NvZnQuY29tIiwibmlpIjoibWljcm9zb2Z0LnNoYXJlcG9pbnQiLCJpc3VzZXIiOiJ0cnVlIiwiY2FjaGVrZXkiOiIwaC5mfG1lbWJlcnNoaXB8MTAwMzAwMDA5MTQxYzY5NUBsaXZlLmNvbSIsImlzbG9vcGJhY2siOiJUcnVlIiwiYXBwY3R4IjoiYmJiNDIwMDFkYjBhNGIzNWFlOWJhODUxNzk3YTRmMDg7c1ZtbkxOWURWd2dubCtuUXVkQXBEeVdvMkdjPTtEZWZhdWx0OzsxQjAzQzQzMTI2NztUcnVlOyJ9%2EFd50QaTfqRKYozjy4KAhlUQRktCfat9sJC0tjQbukCLTM4CYwv9cAfojE%2DibBhnqe2dQyDV%2DTW7P60CFctxBW8AOOhFB6UrTevWbHCy2QRgshHB1hGv6b4a3pKN66ASaNhAXzJLZgPdRHR1S3sWo5GqK%5FvbP5DgL4w%5Ff94hUX3R9TOSV8SqFLcWzIFc8DsDZltuWm45H5jzZWyPMtD0mSXfWWus0JAcnwtv1QGj2CX0lrOKb4j3D4INr2XhdcaS1HPtBAXeEMCJ6zcVA7obRYC6Xzd1BnLDwS7RwjaG2CbNFmu2xdPNnIuvlSwAuy7NjsmxnjiL0uRDi81CEUsViLA&access_token_ttl=1456856276632&usid=8c853755-4d2d-483d-bc67-f928947234bd&build=16.0.6721.2200&waccluster=SG1B" alt="Image"></p>
<ol class="NumberListStyle1">
</ol>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Running the Sample</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">To debug the sample and then run it, press F5 or use
</span><span style="font-weight:bold">Debug &gt; Start Debugging</span><span style="font-size:11pt">. To run the sample without debugging, press Ctrl&#43;F5 or use
</span><span style="font-weight:bold">Debug &gt; Start Without Debugging</span><span style="font-size:11pt">.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Using the Code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Add the following code namespace declarations to the top of any C# file in which you want to programmatically access the
</span><span style="font-size:11pt">SendGrid</span><span style="font-size:11pt"> email service.
</span><span style="font-weight:bold; font-size:11pt"><a class="libraryLink" href="https://msdn.microsoft.com/en-US/library/System.Net.aspx" target="_blank" title="Auto generated link to System.Net">System.Net</a></span><span style="font-size:11pt"> and
</span><span style="font-weight:bold; font-size:11pt"><a class="libraryLink" href="https://msdn.microsoft.com/en-US/library/System.Net.Mail.aspx" target="_blank" title="Auto generated link to System.Net.Mail">System.Net.Mail</a></span><span style="font-size:11pt"> are .NET Framework namespaces that are included because they include types you will commonly use with the
</span><span style="font-size:11pt">SendGrid</span><span style="font-size:11pt"> APIs.</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre class="csharp" id="codePreview">using System;
using <a class="libraryLink" href="https://msdn.microsoft.com/en-US/library/System.Net.aspx" target="_blank" title="Auto generated link to System.Net">System.Net</a>;
using <a class="libraryLink" href="https://msdn.microsoft.com/en-US/library/System.Net.Mail.aspx" target="_blank" title="Auto generated link to System.Net.Mail">System.Net.Mail</a>;
using SendGrid;
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">The </span><span style="font-size:11pt">code provided demonstrates how to create a simple message and add properties to it. Further properties can be added to the code and message to include attachments,
 footers, click tracking etc. </span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre class="csharp" id="codePreview">//Message properties, add your values below
string fromProperty = &quot;noreply@example.com&quot;;
            
List&lt;String&gt; recipientsProperty = new List&lt;String&gt;
{
    @&quot;John Smith &lt;john@example.com&gt;&quot;,
    @&quot;Jane Smith &lt;jane@example.com&gt;&quot;
};
string subjectProperty = &quot;Title of email&quot;;
string HTMLContentProperty = &quot;<p>Your email message in HTML format</p>&quot;;
//SendGrid credentials
string username = &quot;Username&quot;;
string password = &quot;Password&quot;;
//The email object
var message = new SendGridMessage();
message.From = new MailAddress(fromProperty);
message.AddTo(recipientsProperty);
message.Subject = subjectProperty;
//Add the HTML and Text bodies
message.Html = HTMLContentProperty;
var credentials = new NetworkCredential(username, password);
var transportWeb = new Web(credentials);
transportWeb.DeliverAsync(message).Wait();
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">More Information</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
&nbsp;</p>
<ul>
<li><span style="font-size:11pt">SendGrid</span><span style="font-size:11pt"> C# library repo:
</span><a href="https://github.com/sendgrid/sendgrid-csharp" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">sendgrid-csharp</span></a>
</li><li><span style="font-size:11pt">SendGrid</span><span style="font-size:11pt"> API
</span><span style="font-size:11pt">documentation: </span><a href="https://sendgrid.com/docs" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">https://sendgrid.com/docs</span></a>
</li><li><span style="font-size:11pt">More on using </span><span style="font-size:11pt">SendGrid</span><span style="font-size:11pt"> with Azure:
</span><a href="https://azure.microsoft.com/en-us/documentation/articles/sendgrid-dotnet-how-to-send-email/" style="text-decoration:none"><span style="color:#0563c1; font-size:11pt; text-decoration:underline">https://azure.microsoft.com/en-us/documentation/articles/sendgrid-dotnet-how-to-send-email/</span></a>
</li></ul>
<p>&nbsp;</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
