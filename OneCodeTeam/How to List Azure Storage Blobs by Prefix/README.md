# How to List Azure Storage Blobs by Prefix
## Requires
- Visual Studio 2015
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
- Cloud
- Azure Data Services
## Updated
- 11/10/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner1" alt="">
</a></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span style="font-weight:bold; font-size:14pt">List the blobs in a container with the specified prefix</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Introduction</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:12pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="color:#333333; font-size:11pt">&nbsp;</span><span style="color:#333333; font-size:12pt"><br>
</span><span style="color:#333333; font-size:11pt">This is an example of how to list the blobs in a container whose names begin with the specified prefix. The ListBlobs method lists blobs and virtual directories in a container, optionally in segments of a specified
 or default size. You can optionally specify a blob prefix to list the blobs whose names&nbsp;begin with the same string. If you use a delimiter character in your blob names to create a virtual directory structure, the blob prefix&nbsp;will include all or part
 of the virtual directory structure (but not the container name). You can list blobs hierarchically in a manner similar to traversing a file system, or in a flat listing, where all blobs matching the specified prefix are returned by the listing operation. You
 can specify additional details to return with the listing, including copy properties, metadata, snapshots, and uncommitted blobs.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Prerequisites</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt">&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:12pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="font-weight:bold; font-style:italic; color:#333333; font-size:12pt">1. Azure Account</span><span style="color:#333333">&nbsp;</span><span style="color:#333333">
<br>
You need an Azure account. You can&nbsp;</span><a href="https://azure.microsoft.com/pricing/free-trial/?WT.mc_id=A261C142F" style="text-decoration:none"><span style="color:#4078c0; text-decoration:underline">open a free Azure account</span></a><span style="color:#333333">&nbsp;or&nbsp;</span><a href="https://azure.microsoft.com/pricing/member-offers/msdn-benefits-details/?WT.mc_id=A261C142F" style="text-decoration:none"><span style="color:#4078c0; text-decoration:underline">Activate
 Visual Studio subscriber benefits</span></a><span style="color:#333333">.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:12pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="font-weight:bold; font-style:italic; color:#333333; font-size:12pt">2. Visual Studio 2015</span><span style="color:#333333; font-size:12pt">&nbsp;</span><span style="color:#333333">
<br>
</span><a href="https://www.visualstudio.com/downloads/" style="text-decoration:none"><span style="color:#4078c0; text-decoration:underline">https://www.visualstudio.com/downloads/</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:12pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="font-weight:bold; font-style:italic; color:#333333; font-size:12pt">3. Azure SDK</span><span style="color:#333333">&nbsp;</span><span style="color:#333333">
<br>
The tutorial is written for Visual Studio 2015 with the&nbsp;</span><a href="https://azure.microsoft.com/en-us/documentation/articles/dotnet-sdk/" style="text-decoration:none"><span style="color:#4078c0; text-decoration:underline">Azure SDK for .NET 2.9</span></a><span style="color:#333333">&nbsp;or
 later.&nbsp;</span><a href="http://go.microsoft.com/fwlink/?linkid=518003" style="text-decoration:none"><span style="color:#4078c0; text-decoration:underline">Download the latest Azure SDK for Visual Studio 2015</span></a><span style="color:#333333">. The
 SDK installs Visual Studio 2015 if you don't already have it.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:12pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Run the sample</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:12pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:12pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="color:#333333">Before you build the solution, please do some modification as below:</span><span style="color:#333333">
<br>
Please note that the delete operation cannot be undone. So make sure you have </span>
<span style="color:#333333">confirmed the code logic first.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:12pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="color:#333333">1. Enter your storage account name, account key and container name as below lines.</span><span style="color:#333333">
<br>
</span><span style="color:#4078c0"><img src="163323-image.png" alt="" width="881" height="143" align="middle">
</span><span style="color:#333333"><br>
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:12pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="color:#333333">2. Please specify your prefix if needed to meet your demands, and enable line 46 when you need to remove those blobs.</span><span style="color:#333333">
<br>
</span><span style="color:#4078c0"><img src="163324-image.png" alt="" width="706" height="183" align="middle">
</span><span style="color:#333333"><br>
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:12pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="color:#333333">3. Add a threshold parameter for method ListBlobsFromContainer(). (please check the&nbsp;source code for details)</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; background-color:#f7f7f7; direction:ltr; unicode-bidi:normal">
<span><span style="color:#a71d5d">var</span><span style="color:#333333"> rsltList = ListBlobsFromContainer(container, prefix,
</span><span style="color:#0086b3">5000</span><span style="color:#333333">);</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; background-color:#f7f7f7; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; background-color:#f7f7f7; direction:ltr; unicode-bidi:normal">
<span><span style="color:#a71d5d">private</span><span style="color:#333333"> </span>
<span style="color:#a71d5d">static</span><span style="color:#333333"> List&lt;Uri&gt; ListBlobsFromContainer(CloudBlobContainer container,
</span><span style="color:#a71d5d">string</span><span style="color:#333333"> prefix,
</span><span style="color:#a71d5d">int</span><span style="color:#333333"> threshold)</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; background-color:#f7f7f7; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; background-color:#f7f7f7; direction:ltr; unicode-bidi:normal">
<span><span style="color:#a71d5d">if</span><span style="color:#333333"> (lstBlobUri.Count &gt; threshold)</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; background-color:#f7f7f7; direction:ltr; unicode-bidi:normal">
<span><span style="color:#333333">{</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; background-color:#f7f7f7; direction:ltr; unicode-bidi:normal">
<span><span style="color:#a71d5d">break</span><span style="color:#333333">;</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; background-color:#f7f7f7; direction:ltr; unicode-bidi:normal">
<span><span style="color:#333333">}</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><a name="_GoBack"></a></span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
