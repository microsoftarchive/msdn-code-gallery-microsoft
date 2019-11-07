# How to store temp files in Azure applications
## Requires
- 
## License
- Apache License, Version 2.0
## Technologies
- Azure
- Storage
- Cloud
- Azure Data Services
## Topics
- Azure
- Storage
- temp
## Updated
- 09/22/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src=":-onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em></em></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:24pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">How to store temp files in Azure applications</span></span></p>
<h2 style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:17.3333px">Download this sample</span></h2>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">To download this sample, click
<a href="https://github.com/Azure-Samples/storage-blob-dotnet-store-temp-files/archive/master.zip">
here</a>.</span></span></p>
<h2><span style="font-size:large">Introduction</span></h2>
<p><span style="font-size:small">This sample demonstrates how to store temp file in Azure applications.</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">We have two solutions</span><span>：</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span style="font-size:11pt">First one is traditional way to call &lsquo;</span><span style="font-size:11pt">Path.GetTempPath</span><span style="font-size:11pt">()&rsquo; to store temp file in all windows
 platform</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span style="font-size:11pt">The other is azure specific way to use blob object in Azure to simulate temp file.
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Building the Sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span style="font-size:11pt">Double click CSAzureTempFiles.sln file to open this sample solution by using Microsoft Visual Studio 2012 or the later version(s).</span><a name="_GoBack"></a></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span style="font-size:11pt">Restore nugget packages in the solution</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt"><img src="156688-image.png" alt="" width="575" height="443" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span style="font-size:11pt">Copy account name, access key and address of your azure storage account.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt"><img src="156689-image.png" alt="" width="511" height="242" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt"><img src="156690-image.png" alt="" width="519" height="235" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span>&bull;&nbsp;</span><span style="font-size:11pt">Configure necessary parameters about account name, key and address in the solution in file Controllers\</span><span style="font-size:11pt">AzureBlobController.cs</span></span></p>
<p style="margin-left:18pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:18pt">
<span style="font-size:11pt"><span style="font-size:11pt"><img src="156691-image.png" alt="" width="534" height="94" align="middle">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Running the Sample</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">You can upload the sample to your Azure storage, or run at your local IIS, or just debug it in your Visual Studio.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Using the Code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Solution1:</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">&nbsp;</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">private void SaveTempFile(HttpPostedFileBase file) 
{ 
	//get the uploaded file name 
	string fileName = file.FileName; 

	//get temp directory path 
	string tempPath = Path.GetTempPath(); 

	//init the file path 
	string filePath = tempPath &#43; fileName; 

	//if the path is exists,delete old file 
	if (System.IO.File.Exists(filePath)) 
	{ 
		System.IO.File.Delete(filePath); 
	} 

	//and then save new file 
	file.SaveAs(filePath); 
} </pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;SaveTempFile(HttpPostedFileBase&nbsp;file)&nbsp;&nbsp;
{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//get&nbsp;the&nbsp;uploaded&nbsp;file&nbsp;name&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;fileName&nbsp;=&nbsp;file.FileName;&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//get&nbsp;temp&nbsp;directory&nbsp;path&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;tempPath&nbsp;=&nbsp;Path.GetTempPath();&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//init&nbsp;the&nbsp;file&nbsp;path&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;filePath&nbsp;=&nbsp;tempPath&nbsp;&#43;&nbsp;fileName;&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//if&nbsp;the&nbsp;path&nbsp;is&nbsp;exists,delete&nbsp;old&nbsp;file&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(System.IO.File.Exists(filePath))&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;System.IO.File.Delete(filePath);&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//and&nbsp;then&nbsp;save&nbsp;new&nbsp;file&nbsp;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;file.SaveAs(filePath);&nbsp;&nbsp;
}&nbsp;</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span style="font-size:11pt; line-height:27.6pt">Solution 2:</span></div>
<div class="endscriptcode"><span style="font-size:11pt; line-height:27.6pt">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">private async Task SaveTempFile(string fileName, long contentLenght, Stream inputStream)
{
	try
	{
		//firstly, we need check the container if exists or not. And if not, we need to create one.
		await container.CreateIfNotExistsAsync();

		//init a blobReference
		CloudBlockBlob tempFileBlob = container.GetBlockBlobReference(fileName);

		//if the blobReference is exists, delete the old blob
		tempFileBlob.DeleteIfExists();

		//check the count of blob if over limit or not, if yes, clear them.
		await CleanStorageIfReachLimit(contentLenght);

		//and upload the new file in this
		tempFileBlob.UploadFromStream(inputStream);
	}
	catch (Exception ex)
	{
		if (ex.InnerException != null)
		{
			throw ex.InnerException;
		}
		else
		{
			throw ex;
		}
	}
}

//check the count of blob if over limit or not, if yes, clear them.
private async Task CleanStorageIfReachLimit(long newFileLength)
{
	List&lt;CloudBlob&gt; blobs = container.ListBlobs()
		.OfType&lt;CloudBlob&gt;()
		.OrderBy(m =&gt; m.Properties.LastModified)
		.ToList();

	//get total size of all blobs.
	long totalSize = blobs.Sum(m =&gt; m.Properties.Length);

	//calculate out the real limit size of before upload
	long realLimetSize = TotalLimitSizeOfTempFiles - newFileLength;

	//delete all,when the free size is enough, break this loop,and stop delete blob anymore
	foreach (CloudBlob item in blobs)
	{
		if (totalSize &lt;= realLimetSize)
		{
			break;
		}

		await item.DeleteIfExistsAsync();
		totalSize -= item.Properties.Length;
	}
}</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;async&nbsp;Task&nbsp;SaveTempFile(<span class="cs__keyword">string</span>&nbsp;fileName,&nbsp;<span class="cs__keyword">long</span>&nbsp;contentLenght,&nbsp;Stream&nbsp;inputStream)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//firstly,&nbsp;we&nbsp;need&nbsp;check&nbsp;the&nbsp;container&nbsp;if&nbsp;exists&nbsp;or&nbsp;not.&nbsp;And&nbsp;if&nbsp;not,&nbsp;we&nbsp;need&nbsp;to&nbsp;create&nbsp;one.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;await&nbsp;container.CreateIfNotExistsAsync();&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//init&nbsp;a&nbsp;blobReference</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;CloudBlockBlob&nbsp;tempFileBlob&nbsp;=&nbsp;container.GetBlockBlobReference(fileName);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//if&nbsp;the&nbsp;blobReference&nbsp;is&nbsp;exists,&nbsp;delete&nbsp;the&nbsp;old&nbsp;blob</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;tempFileBlob.DeleteIfExists();&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//check&nbsp;the&nbsp;count&nbsp;of&nbsp;blob&nbsp;if&nbsp;over&nbsp;limit&nbsp;or&nbsp;not,&nbsp;if&nbsp;yes,&nbsp;clear&nbsp;them.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;await&nbsp;CleanStorageIfReachLimit(contentLenght);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//and&nbsp;upload&nbsp;the&nbsp;new&nbsp;file&nbsp;in&nbsp;this</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;tempFileBlob.UploadFromStream(inputStream);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">catch</span>&nbsp;(Exception&nbsp;ex)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(ex.InnerException&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">throw</span>&nbsp;ex.InnerException;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">throw</span>&nbsp;ex;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
&nbsp;
<span class="cs__com">//check&nbsp;the&nbsp;count&nbsp;of&nbsp;blob&nbsp;if&nbsp;over&nbsp;limit&nbsp;or&nbsp;not,&nbsp;if&nbsp;yes,&nbsp;clear&nbsp;them.</span>&nbsp;
<span class="cs__keyword">private</span>&nbsp;async&nbsp;Task&nbsp;CleanStorageIfReachLimit(<span class="cs__keyword">long</span>&nbsp;newFileLength)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;CloudBlob&gt;&nbsp;blobs&nbsp;=&nbsp;container.ListBlobs()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.OfType&lt;CloudBlob&gt;()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.OrderBy(m&nbsp;=&gt;&nbsp;m.Properties.LastModified)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;.ToList();&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//get&nbsp;total&nbsp;size&nbsp;of&nbsp;all&nbsp;blobs.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">long</span>&nbsp;totalSize&nbsp;=&nbsp;blobs.Sum(m&nbsp;=&gt;&nbsp;m.Properties.Length);&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//calculate&nbsp;out&nbsp;the&nbsp;real&nbsp;limit&nbsp;size&nbsp;of&nbsp;before&nbsp;upload</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">long</span>&nbsp;realLimetSize&nbsp;=&nbsp;TotalLimitSizeOfTempFiles&nbsp;-&nbsp;newFileLength;&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//delete&nbsp;all,when&nbsp;the&nbsp;free&nbsp;size&nbsp;is&nbsp;enough,&nbsp;break&nbsp;this&nbsp;loop,and&nbsp;stop&nbsp;delete&nbsp;blob&nbsp;anymore</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">foreach</span>&nbsp;(CloudBlob&nbsp;item&nbsp;<span class="cs__keyword">in</span>&nbsp;blobs)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(totalSize&nbsp;&lt;=&nbsp;realLimetSize)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">break</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;await&nbsp;item.DeleteIfExistsAsync();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;totalSize&nbsp;-=&nbsp;item.Properties.Length;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;<span style="font-size:13pt; font-weight:bold; line-height:27.6pt">More Information and Resources</span></div>
</span></div>
<p>&nbsp;</p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:11pt">Microsoft Azure storage documents:
</span><a href="https://azure.microsoft.com/en-us/documentation/services/storage" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">https://azure.microsoft.com/en-us/documentation/services/storage</span></a><span style="font-size:11pt">
</span></span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
