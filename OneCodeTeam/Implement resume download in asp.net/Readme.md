# Implement resume download in asp.net
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
## Topics
- resume download
## Updated
- 06/14/2013
## Description

<h2><span style="font-size:14.0pt; line-height:115%">ASP.NET resume download sample (CSASPNETResumeDownload)
</span></h2>
<h2>Introduction</h2>
<p class="MsoNormal"><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">The sample CSASPNETResumeDownload demonstrates how to implement resume download feature in ASP.NET. As we know, due to network interruptions,</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">
</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">Downloading file may meet a problem when the size of file is
<span class="SpellE">large<span style="font-family:SimSun">.</span><span style="">A</span>t</span> this time we need to support resume download if the connection is broken.</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"> In this
 sample, we need two classes: <b style="">HttpRequest</b> and <b style="">HttpResponse</b>.<span style="">&nbsp;
</span><b style="">HttpRequest</b> is used to get the downloaded partial file's length from the Range header and the other one
<b style="">HttpResponse </b>is for setting the start position of the reading file. And then read and send the rest of the file to client. You can find the answers for all the following questions in the code sample:
</span></p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:5.0pt"><span style="font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>How to get and process the HTTP Web request by custom <span class="SpellE">
HttpHandler</span>?<span style=""> </span></p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:5.0pt"><span style="font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>H<span style="font-family:&quot;Cambria&quot;,&quot;serif&quot;">ow to get the </span>
HTTP <span style="font-family:&quot;Cambria&quot;,&quot;serif&quot;">response header informa</span>tion?<span style="">
</span></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:5.0pt"><span style="font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>How to send the file to client <span style="">by </span>using HttpResponse<span style=""> class</span>?<span style="">
</span></p>
<h2>Running the Sample</h2>
<p class="MsoListParagraphCxSpFirst" style="text-indent:5.0pt"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Open the <span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">
CSASPNETResumeDownload </span>.sln. Expand the <span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">
CSASPNETResumeDownload </span>web application. Double click the Web.config file and find the &quot;appSettings&quot; node, then modify the value of the &quot;FilePath&quot; key with the physical path of the download file.</p>
<p class="MsoListParagraphCxSpLast" style="text-indent:5.0pt"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Right click the ResumeDownloadPage.htm, select and click the &quot;Set
<span class="GramE">As</span> Start Page&quot; item then press &quot;F5&quot; to start debug<span style="">ging</span> the project.</p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="84805-image.png" alt="" width="416" height="303" align="middle">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:5.0pt"><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>We will see a Button control named &quot;Download&quot; on the page. Please click it.
<span style=""></span></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:5.0pt"><span style=""><span style="">4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>After completing Step 3 you will see the IE download file dialog on the bottom of the browser.
<span style=""></span></p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="84806-image.png" alt="" width="591" height="340" align="middle">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoListParagraph" style="text-indent:5.0pt"><span style=""><span style="">5.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Please click &quot;Save&quot; button and select a path to save the file, then IE download file dialog will show like below.<span style="">
</span></p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="84807-image.png" alt="" width="591" height="348" align="middle">
</span></p>
<p class="MsoListParagraph" style="text-indent:5.0pt"><span style=""><span style="">6.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>You can click the &quot;Pause&quot; button then the download operation will in<span style="">terrupt.
</span></p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="84808-image.png" alt="" width="595" height="348" align="middle">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoListParagraph" style="text-indent:5.0pt"><span style=""><span style="">7.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Click the &quot;Resume&quot; button to restart the download operation.</p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="84809-image.png" alt="" width="592" height="348" align="middle">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<p class="MsoListParagraph" style="text-indent:5.0pt"><span style=""><span style="">8.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">&nbsp;</span>When the download is finished, the dialog will show like below.</p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"><img src="84810-image.png" alt="" width="595" height="350" align="middle">
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;"></span></p>
<h2>Using the Code</h2>
<p class="MsoNormal"><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;">The code sample provides the following reusable functions:
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
public class DownloadHttpHandler : IHttpHandler
{
&nbsp;&nbsp;&nbsp; public void ProcessRequest(HttpContext context)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; string filePath = ConfigurationManager.AppSettings[&quot;FilePath&quot;];
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Downloader.DownloadFile(context, filePath);
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; public bool IsReusable
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; get { return false; }
&nbsp;&nbsp;&nbsp; }

</pre>
<pre id="codePreview" class="csharp">
public class DownloadHttpHandler : IHttpHandler
{
&nbsp;&nbsp;&nbsp; public void ProcessRequest(HttpContext context)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; string filePath = ConfigurationManager.AppSettings[&quot;FilePath&quot;];
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Downloader.DownloadFile(context, filePath);
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; public bool IsReusable
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; get { return false; }
&nbsp;&nbsp;&nbsp; }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
/// &lt;summary&gt;
/// Get the response header by the http request.
/// &lt;/summary&gt;
/// &lt;param name=&quot;httpRequest&quot;&gt;&lt;/param&gt;
/// &lt;param name=&quot;fileInfo&quot;&gt;&lt;/param&gt;
/// &lt;returns&gt;&lt;/returns&gt;
private static HttpResponseHeader GetResponseHeader(HttpRequest httpRequest, FileInfo fileInfo)
{
&nbsp;&nbsp;&nbsp; if (httpRequest == null)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return null;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; if (fileInfo == null)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return null;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; long startPosition = 0;
&nbsp;&nbsp;&nbsp; string contentRange = &quot;&quot;;


&nbsp;&nbsp;&nbsp; string fileName = fileInfo.Name;
&nbsp;&nbsp;&nbsp; long fileLength = fileInfo.Length;
&nbsp;&nbsp;&nbsp; string lastUpdateTimeStr = fileInfo.LastWriteTimeUtc.ToString();


&nbsp;&nbsp;&nbsp; string eTag = HttpUtility.UrlEncode(fileName, Encoding.UTF8) &#43; &quot; &quot; &#43; lastUpdateTimeStr;
&nbsp;&nbsp;&nbsp; string contentDisposition = &quot;attachment;filename=&quot; &#43; HttpUtility.UrlEncode(fileName, Encoding.UTF8).Replace(&quot;&#43;&quot;, &quot;%20&quot;);


&nbsp;&nbsp;&nbsp; if (httpRequest.Headers[&quot;Range&quot;] != null)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; string[] range = httpRequest.Headers[&quot;Range&quot;].Split(new char[] { '=', '-' });
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; startPosition = Convert.ToInt64(range[1]);
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;if (startPosition &lt; 0 || startPosition &gt;= fileLength)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return null;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; if (httpRequest.Headers[&quot;If-Range&quot;] != null)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (httpRequest.Headers[&quot;If-Range&quot;].Replace(&quot;\&quot;&quot;, &quot;&quot;) != eTag)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;startPosition = 0;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; string contentLength = (fileLength - startPosition).ToString();


&nbsp;&nbsp;&nbsp; if (startPosition &gt; 0)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contentRange = string.Format(&quot; bytes {0}-{1}/{2}&quot;, startPosition, fileLength - 1, fileLength);
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; HttpResponseHeader responseHeader = new HttpResponseHeader();


&nbsp;&nbsp;&nbsp; responseHeader.AcceptRanges = &quot;bytes&quot;;
&nbsp;&nbsp;&nbsp; responseHeader.Connection = &quot;Keep-Alive&quot;;
&nbsp;&nbsp;&nbsp; responseHeader.ContentDisposition = contentDisposition;
&nbsp;&nbsp;&nbsp; responseHeader.ContentEncoding = Encoding.UTF8;
&nbsp;&nbsp;&nbsp; responseHeader.ContentLength = contentLength;
&nbsp;&nbsp;&nbsp; responseHeader.ContentRange = contentRange;
&nbsp;&nbsp;&nbsp; responseHeader.ContentType = &quot;application/octet-stream&quot;;
&nbsp;&nbsp;&nbsp; responseHeader.Etag = eTag;
&nbsp;&nbsp;&nbsp; responseHeader.LastModified = lastUpdateTimeStr;


&nbsp;&nbsp;&nbsp; return responseHeader;
}

</pre>
<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
/// Get the response header by the http request.
/// &lt;/summary&gt;
/// &lt;param name=&quot;httpRequest&quot;&gt;&lt;/param&gt;
/// &lt;param name=&quot;fileInfo&quot;&gt;&lt;/param&gt;
/// &lt;returns&gt;&lt;/returns&gt;
private static HttpResponseHeader GetResponseHeader(HttpRequest httpRequest, FileInfo fileInfo)
{
&nbsp;&nbsp;&nbsp; if (httpRequest == null)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return null;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; if (fileInfo == null)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return null;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; long startPosition = 0;
&nbsp;&nbsp;&nbsp; string contentRange = &quot;&quot;;


&nbsp;&nbsp;&nbsp; string fileName = fileInfo.Name;
&nbsp;&nbsp;&nbsp; long fileLength = fileInfo.Length;
&nbsp;&nbsp;&nbsp; string lastUpdateTimeStr = fileInfo.LastWriteTimeUtc.ToString();


&nbsp;&nbsp;&nbsp; string eTag = HttpUtility.UrlEncode(fileName, Encoding.UTF8) &#43; &quot; &quot; &#43; lastUpdateTimeStr;
&nbsp;&nbsp;&nbsp; string contentDisposition = &quot;attachment;filename=&quot; &#43; HttpUtility.UrlEncode(fileName, Encoding.UTF8).Replace(&quot;&#43;&quot;, &quot;%20&quot;);


&nbsp;&nbsp;&nbsp; if (httpRequest.Headers[&quot;Range&quot;] != null)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; string[] range = httpRequest.Headers[&quot;Range&quot;].Split(new char[] { '=', '-' });
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; startPosition = Convert.ToInt64(range[1]);
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;if (startPosition &lt; 0 || startPosition &gt;= fileLength)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return null;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; if (httpRequest.Headers[&quot;If-Range&quot;] != null)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (httpRequest.Headers[&quot;If-Range&quot;].Replace(&quot;\&quot;&quot;, &quot;&quot;) != eTag)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;startPosition = 0;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; string contentLength = (fileLength - startPosition).ToString();


&nbsp;&nbsp;&nbsp; if (startPosition &gt; 0)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contentRange = string.Format(&quot; bytes {0}-{1}/{2}&quot;, startPosition, fileLength - 1, fileLength);
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; HttpResponseHeader responseHeader = new HttpResponseHeader();


&nbsp;&nbsp;&nbsp; responseHeader.AcceptRanges = &quot;bytes&quot;;
&nbsp;&nbsp;&nbsp; responseHeader.Connection = &quot;Keep-Alive&quot;;
&nbsp;&nbsp;&nbsp; responseHeader.ContentDisposition = contentDisposition;
&nbsp;&nbsp;&nbsp; responseHeader.ContentEncoding = Encoding.UTF8;
&nbsp;&nbsp;&nbsp; responseHeader.ContentLength = contentLength;
&nbsp;&nbsp;&nbsp; responseHeader.ContentRange = contentRange;
&nbsp;&nbsp;&nbsp; responseHeader.ContentType = &quot;application/octet-stream&quot;;
&nbsp;&nbsp;&nbsp; responseHeader.Etag = eTag;
&nbsp;&nbsp;&nbsp; responseHeader.LastModified = lastUpdateTimeStr;


&nbsp;&nbsp;&nbsp; return responseHeader;
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
/// &lt;summary&gt;
/// Send the download file to the client.
/// &lt;/summary&gt;
/// &lt;param name=&quot;httpResponse&quot;&gt;&lt;/param&gt;
/// &lt;param name=&quot;responseHeader&quot;&gt;&lt;/param&gt;
/// &lt;param name=&quot;fileStream&quot;&gt;&lt;/param&gt;
private static void SendDownloadFile(HttpResponse httpResponse, HttpResponseHeader responseHeader, Stream fileStream)
{
&nbsp;&nbsp;&nbsp; if (httpResponse == null || responseHeader == null)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; if (!string.IsNullOrEmpty(responseHeader.ContentRange))
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; httpResponse.StatusCode = 206;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Set the start position of the reading files.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; string[] range = responseHeader.ContentRange.Split(new char[] { ' ','=', '-' });
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; fileStream.Position = Convert.ToInt64(range[2]);
&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; httpResponse.Clear();
&nbsp;&nbsp;&nbsp; httpResponse.Buffer = false;
&nbsp;&nbsp;&nbsp; httpResponse.AppendHeader(&quot;Accept-Ranges&quot;, responseHeader.AcceptRanges);
&nbsp;&nbsp;&nbsp; httpResponse.AppendHeader(&quot;Connection&quot;, responseHeader.Connection);
&nbsp;&nbsp;&nbsp; httpResponse.AppendHeader(&quot;Content-Disposition&quot;, responseHeader.ContentDisposition);
&nbsp;&nbsp;&nbsp; httpResponse.ContentEncoding = responseHeader.ContentEncoding;
&nbsp;&nbsp;&nbsp; httpResponse.AppendHeader(&quot;Content-Length&quot;, responseHeader.ContentLength);
&nbsp;&nbsp;&nbsp; if (!string.IsNullOrEmpty(responseHeader.ContentRange))
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; httpResponse.AppendHeader(&quot;Content-Range&quot;, responseHeader.ContentRange);
&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; httpResponse.ContentType = responseHeader.ContentType;
&nbsp;&nbsp;&nbsp; httpResponse.AppendHeader(&quot;Etag&quot;, &quot;\&quot;&quot; &#43; responseHeader.Etag &#43; &quot;\&quot;&quot;);
&nbsp;&nbsp;&nbsp; httpResponse.AppendHeader(&quot;Last-Modified&quot;, responseHeader.LastModified);


&nbsp;&nbsp;&nbsp; Byte[] buffer = new Byte[10240];
&nbsp;&nbsp;&nbsp; long fileLength = Convert.ToInt64(responseHeader.ContentLength);


&nbsp;&nbsp;&nbsp; // Send file to client.
&nbsp;&nbsp;&nbsp; while (fileLength &gt; 0)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (httpResponse.IsClientConnected)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; int length = fileStream.Read(buffer, 0, 10240);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; httpResponse.OutputStream.Write(buffer, 0, length);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; httpResponse.Flush();


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; fileLength = fileLength - length;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; else
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; fileLength = -1;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }
}
&nbsp;&nbsp;&nbsp; }


</pre>
<pre id="codePreview" class="csharp">
/// &lt;summary&gt;
/// Send the download file to the client.
/// &lt;/summary&gt;
/// &lt;param name=&quot;httpResponse&quot;&gt;&lt;/param&gt;
/// &lt;param name=&quot;responseHeader&quot;&gt;&lt;/param&gt;
/// &lt;param name=&quot;fileStream&quot;&gt;&lt;/param&gt;
private static void SendDownloadFile(HttpResponse httpResponse, HttpResponseHeader responseHeader, Stream fileStream)
{
&nbsp;&nbsp;&nbsp; if (httpResponse == null || responseHeader == null)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; return;
&nbsp;&nbsp;&nbsp; }


&nbsp;&nbsp;&nbsp; if (!string.IsNullOrEmpty(responseHeader.ContentRange))
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; httpResponse.StatusCode = 206;


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; // Set the start position of the reading files.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; string[] range = responseHeader.ContentRange.Split(new char[] { ' ','=', '-' });
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; fileStream.Position = Convert.ToInt64(range[2]);
&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; httpResponse.Clear();
&nbsp;&nbsp;&nbsp; httpResponse.Buffer = false;
&nbsp;&nbsp;&nbsp; httpResponse.AppendHeader(&quot;Accept-Ranges&quot;, responseHeader.AcceptRanges);
&nbsp;&nbsp;&nbsp; httpResponse.AppendHeader(&quot;Connection&quot;, responseHeader.Connection);
&nbsp;&nbsp;&nbsp; httpResponse.AppendHeader(&quot;Content-Disposition&quot;, responseHeader.ContentDisposition);
&nbsp;&nbsp;&nbsp; httpResponse.ContentEncoding = responseHeader.ContentEncoding;
&nbsp;&nbsp;&nbsp; httpResponse.AppendHeader(&quot;Content-Length&quot;, responseHeader.ContentLength);
&nbsp;&nbsp;&nbsp; if (!string.IsNullOrEmpty(responseHeader.ContentRange))
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; httpResponse.AppendHeader(&quot;Content-Range&quot;, responseHeader.ContentRange);
&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; httpResponse.ContentType = responseHeader.ContentType;
&nbsp;&nbsp;&nbsp; httpResponse.AppendHeader(&quot;Etag&quot;, &quot;\&quot;&quot; &#43; responseHeader.Etag &#43; &quot;\&quot;&quot;);
&nbsp;&nbsp;&nbsp; httpResponse.AppendHeader(&quot;Last-Modified&quot;, responseHeader.LastModified);


&nbsp;&nbsp;&nbsp; Byte[] buffer = new Byte[10240];
&nbsp;&nbsp;&nbsp; long fileLength = Convert.ToInt64(responseHeader.ContentLength);


&nbsp;&nbsp;&nbsp; // Send file to client.
&nbsp;&nbsp;&nbsp; while (fileLength &gt; 0)
&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; if (httpResponse.IsClientConnected)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; int length = fileStream.Read(buffer, 0, 10240);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; httpResponse.OutputStream.Write(buffer, 0, length);


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; httpResponse.Flush();


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; fileLength = fileLength - length;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; else
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; {
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; fileLength = -1;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; }
&nbsp;&nbsp;&nbsp; }
}
&nbsp;&nbsp;&nbsp; }


</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2>More Information</h2>
<p class="MsoListParagraphCxSpFirst" style="text-indent:5.0pt"><span style="font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/system.web.httprequest.aspx">MSDN: HttpRequest</a></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:5.0pt"><span style="font-family:Symbol"><span style="">&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/system.web.httpresponse.aspx">MSDN: HttpResponse</a></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
