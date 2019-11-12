# How to store the images in Microsoft Azure SQL Database
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- Microsoft Azure
- Microsoft Azure SQL Database
## Topics
- SQL Azure
- Image
- BLOB
## Updated
- 04/12/2015
## Description

<h1>在 Windows Azure SQL Server 中存储图像 (CSSQLAzureStoreImages)</h1>
<h2>简介</h2>
<p>此示例演示如何在 Windows Azure SQL Server 中存储图像。</p>
<p>有时开发人员需要在 Windows Azure 中存储文件。在此示例中，我们将介绍两种方法来实现此功能：</p>
<p>1. 在 SQL Azure 中存储图像数据。搜索和管理图像很简单。</p>
<p>2. 将图像存储在 Blob 中，将 Blob 的 Uri 存储在 SQL Azure 中。Blob 的空间更便宜。如果我们可以将图像存储在 Blob 中，并将图像信息存储在 SQL Azure 中，管理图像也很简单。</p>
<h2>生成示例</h2>
<p>将示例部署到云之前，您需要完成以下步骤：</p>
<p>步骤 1.根据您的服务器名、用户名和密码修改 Web.config 文件中的连接字符串</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xml</span>

<div class="preview">
<pre class="xml"><span class="xml__tag_start">&lt;add</span>&nbsp;<span class="xml__attr_name">name</span>=<span class="xml__attr_value">&quot;ImagesContext&quot;</span>&nbsp;connectionString=&quot;Server=tcp:&lt;servername<span class="xml__tag_start">&gt;.</span>database.windows.net,1433;Database=ImagesDb;User&nbsp;ID=<span class="xml__tag_start">&lt;username</span><span class="xml__tag_start">&gt;@</span><span class="xml__tag_start">&lt;servername</span><span class="xml__tag_start">&gt;;</span>Password=<span class="xml__tag_start">&lt;password</span><span class="xml__tag_start">&gt;;</span>Trusted_Connection=False;Encrypt=True;MultipleActiveResultSets=True;&quot;&nbsp;providerName=&quot;System.Data.SqlClient&quot;<span class="xml__tag_end">/&gt;</span>&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">&nbsp;</p>
<p class="MsoNormal"><span>步骤 2.根据您存储的用户帐户和密钥修改 StorageConnectionString 和 Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString 的&#20540;。您可以参考</span><a href="http://code.msdn.microsoft.com/How-to-store-the-in-SQL-6c6a46b5/http://code.msdn.microsoft.com/How-to-store-the-in-SQL-6c6a46b5/https://www.windowsazure.com/en-us/develop/net/how-to-guides/blob-storage/#header-4">此内容</a><span>。</span></p>
<p class="MsoNormal"><span><img src="96015-image.png" alt="" width="690" height="135" align="middle">
</span></p>
<h2>运行示例</h2>
<p>将此示例部署到云之后，您可以访问网络。</p>
<p>1.&nbsp;&nbsp;选择用于存储图像的位置。</p>
<p>2. 选择要上载的图像。</p>
<p>3. 您可以删除和复制图像</p>
<p class="MsoNormal"><span><img src="96016-image.png" alt="" width="1111" height="716" align="middle">
</span></p>
<h2>使用代码</h2>
<p><strong>1. 将图像存储在 SQL Azure 中。</strong></p>
<p>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<strong>a. 模型类</strong></p>
<p>我们使用以下类构建存储图像信息和图像数据的两个表。</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">public class ImageInSQLAzure
{
&nbsp;&nbsp;&nbsp; [Key]
&nbsp;&nbsp;&nbsp; public Int32 ImageId { get; set; }
&nbsp;&nbsp;&nbsp; public String FileName { get; set; }
&nbsp;&nbsp;&nbsp; public String ImageName { get; set; }
&nbsp;&nbsp;&nbsp; public String Description { get; set; }
}


public class ImagesTable
{
&nbsp;&nbsp;&nbsp; [Key]
&nbsp;&nbsp;&nbsp; public Int32 Id { get; set; }


&nbsp;&nbsp;&nbsp; [Column(TypeName = &quot;image&quot;)]
&nbsp;&nbsp;&nbsp; public byte[] ImageData { get; set; }


&nbsp;&nbsp;&nbsp; public Int32 ImageId { get; set; }
&nbsp;&nbsp;&nbsp; [ForeignKey(&quot;ImageId&quot;)]
&nbsp;&nbsp;&nbsp; public ImageInSQLAzure ImageInfo { get; set; }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;
<p><strong>&nbsp; &nbsp;&nbsp;&nbsp;b. 显示库</strong></p>
<p>我们将图像信息绑定到 ListView。</p>
</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">this.images.DataSource = imagesDb.SQLAzureImages.ToList();

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;<span>然后我们将图像的 Uri 设置为 GetImage.ashx 和 ImageId。</span></div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">img.ImageUrl = &quot;GetImage.ashx?ImageId=&quot; &#43; image.ImageId;
</pre>
</div>
</div>
<p class="MsoNormal"><span>在 GetImage.ashx 中，我们将根据 SQL Azure 中的 ImageId 获取图像，并返回给客户端。</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">context.Response.ContentType = &quot;image/jpeg&quot;;


Int32 imageId = Int32.Parse(context.Request.QueryString[&quot;ImageId&quot;]);
ImagesTable image = (from i in imagesDb.ImagesTable
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; where i.ImageId == imageId
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; select i).FirstOrDefault();
if (image != null)
{
&nbsp;&nbsp;&nbsp; context.Response.BinaryWrite(image.ImageData);
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;
<p>现在，我们可以在浏览器中查看图像库。</p>
<p><strong>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;c. 上载图像</strong></p>
<p>您还可以将图像上载至 SQL Azure。我们只需将图像信息和数据存储到类，并保存更改。现在，图像即位于 SQL Azure 的数据库中。</p>
</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">ImageInSQLAzure newImageInfo = new ImageInSQLAzure();
ImagesTable newImage = new ImagesTable();


newImageInfo.FileName = fileName;
newImageInfo.ImageName = string.IsNullOrEmpty(name) ? &quot;unknown&quot; : name;
newImageInfo.Description = string.IsNullOrEmpty(description) ? &quot;unknown&quot; : description;


newImage.ImageInfo = newImageInfo;
newImage.ImageData = data;


imagesDb.SQLAzureImages.Add(newImageInfo);
imagesDb.ImagesTable.Add(newImage);
imagesDb.SaveChanges();

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;
<p><strong>&nbsp; &nbsp;&nbsp;d. 删除图像</strong></p>
<p>我们首先根据 ImageId 获取图像，然后将其删除。</p>
</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">ImagesTable deletedImage = (from i in imagesDb.ImagesTable
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; where i.ImageId == imageId
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; select i).FirstOrDefault();


if (deletedImage != null)
{
&nbsp;&nbsp;&nbsp; ImageInSQLAzure deletedImageInfo = deletedImage.ImageInfo;


&nbsp;&nbsp;&nbsp; imagesDb.SQLAzureImages.Remove(deletedImageInfo);
&nbsp;&nbsp;&nbsp; imagesDb.ImagesTable.Remove(deletedImage);
&nbsp;&nbsp;&nbsp; imagesDb.SaveChanges();
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;
<div class="endscriptcode">&nbsp;</div>
<p><strong>e. 复制图像</strong></p>
<p>我们首先根据 ImageId 获取源图像，然后再创建新图像。保存更改后，新的图像现在即位于数据库中。</p>
</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">ImagesTable copiedImage = (from i in imagesDb.ImagesTable
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; where i.ImageId == imageId
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; select i).FirstOrDefault();


if (copiedImage != null)
{
&nbsp;&nbsp;&nbsp; ImageInSQLAzure copiedImageInfo = copiedImage.ImageInfo;
&nbsp;&nbsp;&nbsp; ImagesTable newImage = new ImagesTable();
&nbsp;&nbsp;&nbsp; ImageInSQLAzure newImageInfo = new ImageInSQLAzure();


&nbsp;&nbsp;&nbsp; // Copy the info of image.
&nbsp;&nbsp;&nbsp; newImageInfo.FileName = copiedImageInfo.FileName;
&nbsp;&nbsp;&nbsp; newImageInfo.ImageName = &quot;Copy of \&quot;&quot; &#43; copiedImageInfo.ImageName &#43; &quot;\&quot;&quot;;
&nbsp;&nbsp;&nbsp; newImageInfo.Description = copiedImageInfo.Description;


&nbsp;&nbsp;&nbsp; newImage.ImageData = copiedImage.ImageData;
&nbsp;&nbsp;&nbsp; newImage.ImageInfo = newImageInfo;


&nbsp;&nbsp;&nbsp; imagesDb.SQLAzureImages.Add(newImageInfo);
&nbsp;&nbsp;&nbsp; imagesDb.ImagesTable.Add(newImage);
&nbsp;&nbsp;&nbsp; imagesDb.SaveChanges();


&nbsp;&nbsp;&nbsp; this.RefreshGallery();
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;
<p><strong>2. 将图像存储在 Blob 中</strong></p>
<p><strong>&nbsp;&nbsp;&nbsp;&nbsp;a. 模型类</strong></p>
<p>我们使用以下类构建一个表来存储图像信息和 Blob 的 Uri。</p>
</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">public class ImageInBlob
{
&nbsp;&nbsp;&nbsp; [Key]
&nbsp;&nbsp;&nbsp; public Int32 ImageId { get; set; }
&nbsp;&nbsp;&nbsp; public String FileName { get; set; }
&nbsp;&nbsp;&nbsp; public String ImageName { get; set; }
&nbsp;&nbsp;&nbsp; public String Description { get; set; }
&nbsp;&nbsp;&nbsp; public String BlobUri { get; set; }
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;
<p><strong>&nbsp; &nbsp;&nbsp;b. 显示库</strong></p>
<p>我们将图像信息和 Blob 的 Uri 绑定到 ListView。</p>
</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">this.images.DataSource = imagesDb.BlobImages.ToList();

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;
<p><strong>&nbsp;&nbsp;&nbsp;c. 上载图像</strong></p>
<p>我们首先创建 blob，并将图像数据保存到该 blob。然后我们将图像信息和 blob 的 Uri 保存到数据库。</p>
</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">name = string.IsNullOrEmpty(name) ? &quot;unknown&quot; : name;


var blob = this.GetContainer().GetBlobReference(name);


blob.Properties.ContentType = contentType;


ImageInBlob newImage = new ImageInBlob();
newImage.FileName = fileName;
newImage.ImageName = name;
newImage.Description = string.IsNullOrEmpty(description) ? &quot;unknown&quot; : description;


blob.UploadByteArray(data);
newImage.BlobUri = blob.Uri.ToString();


imagesDb.BlobImages.Add(newImage);
imagesDb.SaveChanges();

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p><strong>&nbsp;&nbsp;&nbsp;d. 删除图像</strong></p>
<p>我们首先根据 ImageId 获取 ImageInBlob，然后根据 ImageInBlob.Uri 获取 blob。删除 blob 后，我们将从数据库中删除图像信息。</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<div class="preview">
<pre class="csharp">ImageInBlob&nbsp;deletedImage&nbsp;=&nbsp;(from&nbsp;i&nbsp;<span class="cs__keyword">in</span>&nbsp;imagesDb.BlobImages&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;where&nbsp;i.ImageId&nbsp;==&nbsp;imageId&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;select&nbsp;i).FirstOrDefault();&nbsp;
<span class="cs__keyword">if</span>&nbsp;(deletedImage&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;按&nbsp;uri&nbsp;删除&nbsp;blob。</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;var&nbsp;blob&nbsp;=&nbsp;<span class="cs__keyword">this</span>.GetContainer().GetBlobReference(deletedImage.BlobUri);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;blob.DeleteIfExists();&nbsp;
&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;imagesDb.BlobImages.Remove(deletedImage);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;imagesDb.SaveChanges();&nbsp;
}&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;
<p><strong>e. 复制图像</strong></p>
<p>我们首先根据 ImageId 获取源图像，然后根据 Uri 获取源 blob。创建 blob 的副本后，我们还会在数据库中插入一份信息。</p>
</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">ImageInBlob copiedImage = (from i in imagesDb.BlobImages
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; where i.ImageId == imageId
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; select i).FirstOrDefault();
if (copiedImage != null)
{
&nbsp;&nbsp;&nbsp; var srcBlob = this.GetContainer().GetBlobReference(copiedImage.BlobUri);


&nbsp;&nbsp;&nbsp; String newImageName = &quot;Copy of \&quot;&quot; &#43; copiedImage.ImageName &#43; &quot;\&quot;&quot;;
&nbsp;&nbsp;&nbsp; var newBlob = this.GetContainer().GetBlobReference(Guid.NewGuid().ToString());


&nbsp;&nbsp;&nbsp; // Copy content from source blob
&nbsp;&nbsp; &nbsp;newBlob.CopyFromBlob(srcBlob);


&nbsp;&nbsp;&nbsp; // Copy the info of image.
&nbsp;&nbsp;&nbsp; ImageInBlob newImage = new ImageInBlob();
&nbsp;&nbsp;&nbsp; newImage.FileName = copiedImage.FileName;
&nbsp;&nbsp;&nbsp; newImage.ImageName = newImageName;
&nbsp;&nbsp;&nbsp; newImage.Description = copiedImage.Description;
&nbsp;&nbsp;&nbsp; newImage.BlobUri = newBlob.Uri.ToString();


&nbsp;&nbsp;&nbsp; imagesDb.BlobImages.Add(newImage);
&nbsp;&nbsp;&nbsp; imagesDb.SaveChanges();

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2>更多信息</h2>
<p><a href="http://code.msdn.microsoft.com/How-to-store-the-in-SQL-6c6a46b5/https://www.windowsazure.com/en-us/develop/net/how-to-guides/blob-storage">如何在 .NET 中使用 Windows Azure Blob 存储服务</a></p>
<p><a href="http://www.windowsazure.com/en-us/develop/net/tutorials/cloud-service-with-sql-database/">向 Windows Azure 云服务和 SQL 数据库部署 ASP.NET Web 应用程序</a></p>
<h2></h2>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
