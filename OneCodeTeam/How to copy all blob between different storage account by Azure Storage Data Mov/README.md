# How to copy all blob between different storage account by Azure Storage Data Mov
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
- azure storage
- Azure Storage Data Movement Library
## Updated
- 09/22/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src="https://aka.ms/onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em></em></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:24pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><span>How</span><span style="font-weight:bold; font-size:14pt"> to copy all blob between different storage account
</span><span>by</span><span style="font-weight:bold; font-size:14pt"> </span><span style="font-weight:bold; font-size:14pt">Azure Storage Data Movement Library</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Introduction</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Sometime, we need move all blob in a container to different blob for backup, or just moving resources to different account.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">This example is to help you move all blob resource in a container to different storage account container.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Prerequisites</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-weight:bold; font-style:italic">&bull;&nbsp;</span><span style="font-weight:bold; font-style:italic">Storage Account</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">To transfer data between storage account, we need create source and target storage account.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="https://azure.microsoft.com/en-us/documentation/articles/storage-create-storage-account/" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">https://azure.microsoft.com/en-us/documentation/articles/storage-create-storage-account/</span></a></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-weight:bold; font-style:italic">&bull;&nbsp;</span><span style="font-weight:bold; font-style:italic">Create storage connection string</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">The storage connection string is used to connect storage from solution.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="https://azure.microsoft.com/en-us/documentation/articles/storage-configure-connection-string/" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">https://azure.microsoft.com/en-us/documentation/articles/storage-configure-connection-string/</span></a></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-weight:bold; font-style:italic">&bull;&nbsp;</span><span style="font-weight:bold; font-style:italic">Create source storage container</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">The container that need to be transfer. All blob in this container will be copied.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a href="https://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-blobs/#create-a-container" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">https://azure.microsoft.com/en-us/documentation/articles/storage-dotnet-how-to-use-blobs/#create-a-container</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Building the Sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-weight:bold; font-style:italic">&bull;&nbsp;</span><span style="font-weight:bold; font-style:italic">Open</span><span style="font-weight:bold; font-style:italic">
</span><span style="font-weight:bold; font-style:italic">Sol</span><span style="font-weight:bold; font-style:italic">ution in Visual Studio 2015</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Please download the sample code, and open it in Visual Studio</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:11pt"><span style="font-weight:bold; font-style:italic">&bull;&nbsp;</span><span style="font-weight:bold; font-style:italic">Configure app.config
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">Open app.config and change yellow mark with your own settings.</span></span></p>
<p style="font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-autospace:none; margin:0pt">
<span style="font-size:11pt"><span style="color:#0000ff; font-size:9.5pt; background:white">&lt;</span><span style="color:#a31515; font-size:9.5pt; background:white">appSettings</span><span style="color:#0000ff; font-size:9.5pt; background:white">&gt;</span></span></p>
<p style="font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-autospace:none; margin:0pt">
<span style="font-size:11pt"><span style="color:#0000ff; font-size:9.5pt; background:white">&lt;</span><span style="color:#a31515; font-size:9.5pt; background:white">add</span><span style="color:#0000ff; font-size:9.5pt; background:white">
</span><span style="color:#ff0000; font-size:9.5pt; background:white">key</span><span style="color:#0000ff; font-size:9.5pt; background:white">=</span><span style="color:#000000; font-size:9.5pt; background:white">&quot;</span><span style="color:#0000ff; font-size:9.5pt; background:white">sourceStorageConnectionString</span><span style="color:#000000; font-size:9.5pt; background:white">&quot;</span><span style="color:#0000ff; font-size:9.5pt; background:white">
</span><span style="color:#ff0000; font-size:9.5pt; background:white">value</span><span style="color:#0000ff; font-size:9.5pt; background:white">=</span><span style="color:#000000; font-size:9.5pt; background:white">&quot;</span><span style="color:#0000ff; font-size:9.5pt; background:yellow">connectiongString</span><span style="color:#000000; font-size:9.5pt; background:white">&quot;</span><span style="color:#0000ff; font-size:9.5pt; background:white">
 /&gt;</span></span></p>
<p style="font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-autospace:none; margin:0pt">
<span style="font-size:11pt"><span style="color:#0000ff; font-size:9.5pt; background:white">&lt;</span><span style="color:#a31515; font-size:9.5pt; background:white">add</span><span style="color:#0000ff; font-size:9.5pt; background:white">
</span><span style="color:#ff0000; font-size:9.5pt; background:white">key</span><span style="color:#0000ff; font-size:9.5pt; background:white">=</span><span style="color:#000000; font-size:9.5pt; background:white">&quot;</span><span style="color:#0000ff; font-size:9.5pt; background:white">targetStorageConnectionString</span><span style="color:#000000; font-size:9.5pt; background:white">&quot;</span><span style="color:#0000ff; font-size:9.5pt; background:white">
</span><span style="color:#ff0000; font-size:9.5pt; background:white">value</span><span style="color:#0000ff; font-size:9.5pt; background:white">=</span><span style="color:#000000; font-size:9.5pt; background:white">&quot;</span><span style="color:#0000ff; font-size:9.5pt; background:yellow">connectiongString</span><span style="color:#000000; font-size:9.5pt; background:white">&quot;</span><span style="color:#0000ff; font-size:9.5pt; background:white">
 /&gt;</span></span></p>
<p style="font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-autospace:none; margin:0pt">
<span style="font-size:11pt"><span style="color:#0000ff; font-size:9.5pt; background:white">&lt;</span><span style="color:#a31515; font-size:9.5pt; background:white">add</span><span style="color:#0000ff; font-size:9.5pt; background:white">
</span><span style="color:#ff0000; font-size:9.5pt; background:white">key</span><span style="color:#0000ff; font-size:9.5pt; background:white">=</span><span style="color:#000000; font-size:9.5pt; background:white">&quot;</span><span style="color:#0000ff; font-size:9.5pt; background:white">sourceContainer</span><span style="color:#000000; font-size:9.5pt; background:white">&quot;</span><span style="color:#0000ff; font-size:9.5pt; background:white">
</span><span style="color:#ff0000; font-size:9.5pt; background:white">value</span><span style="color:#0000ff; font-size:9.5pt; background:white">=</span><span style="color:#000000; font-size:9.5pt; background:white">&quot;</span><span style="color:#0000ff; font-size:9.5pt; background:yellow">name</span><span style="color:#000000; font-size:9.5pt; background:white">&quot;</span><span style="color:#0000ff; font-size:9.5pt; background:white">
 /&gt;</span></span></p>
<p style="font-size:10.0pt; line-height:24pt; direction:ltr; unicode-bidi:normal; text-autospace:none; margin:0pt">
<span style="font-size:11pt"><span style="color:#0000ff; font-size:9.5pt; background:white">&lt;</span><span style="color:#a31515; font-size:9.5pt; background:white">add</span><span style="color:#0000ff; font-size:9.5pt; background:white">
</span><span style="color:#ff0000; font-size:9.5pt; background:white">key</span><span style="color:#0000ff; font-size:9.5pt; background:white">=</span><span style="color:#000000; font-size:9.5pt; background:white">&quot;</span><span style="color:#0000ff; font-size:9.5pt; background:white">targetContainer</span><span style="color:#000000; font-size:9.5pt; background:white">&quot;</span><span style="color:#0000ff; font-size:9.5pt; background:white">
</span><span style="color:#ff0000; font-size:9.5pt; background:white">value</span><span style="color:#0000ff; font-size:9.5pt; background:white">=</span><span style="color:#000000; font-size:9.5pt; background:white">&quot;</span><span style="color:#0000ff; font-size:9.5pt; background:yellow">name</span><span style="color:#000000; font-size:9.5pt; background:white">&quot;</span><span style="color:#0000ff; font-size:9.5pt; background:white">
 /&gt;</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="color:#0000ff; font-size:9.5pt; background:white">&lt;/</span><span style="color:#a31515; font-size:9.5pt; background:white">appSettings</span><span style="color:#0000ff; font-size:9.5pt; background:white">&gt;</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:11pt">The source
</span><span style="font-weight:bold; font-size:11pt">and target connection string is used to connect storage. The source container name is the container you need to copy.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">You need provide a target container name, if this container is not existing, will automatically create a new one.</span><span style="font-size:11pt">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><a name="_GoBack"></a></span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
