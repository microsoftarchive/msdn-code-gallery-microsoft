# How to implement resume download in ASP.NET
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
- .NET
- Web App Development
## Topics
- resume download
## Updated
- 04/05/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner" alt="">
</a></div>
<h2><span style="font-size:14.0pt; line-height:115%">ASP.NET resume download sample (CSASPNETResumeDownload2012)
</span></h2>
<h2>Introduction</h2>
<p class="MsoNormal"><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,sans-serif">The sample CSASPNETResumeDownload demonstrates how to implement resume download feature in ASP.NET. As we know, due to network interruptions,</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,sans-serif">
</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,sans-serif">Downloading file may meet a problem when the size of file is
<span class="SpellE">large<span style="font-family:SimSun">.</span><span>A</span>t</span> this time we need to support resume download if the connection is broken.</span><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,sans-serif"> In this sample, we
 need two classes: <strong>HttpRequest</strong> and <strong>HttpResponse</strong>.<span>&nbsp;
</span><strong>HttpRequest</strong> is used to get the downloaded partial file's length from the Range header and the other one
<strong>HttpResponse </strong>is for setting the start position of the reading file. And then read and send the rest of the file to client. You can find the answers for all the following questions in the code sample:
</span></p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>How to get and process the HTTP Web request by custom <span class="SpellE">
HttpHandler</span>?<span> </span></p>
<p class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>H<span style="font-family:&quot;Cambria&quot;,serif">ow to get the </span>
HTTP <span style="font-family:&quot;Cambria&quot;,serif">response header informa</span>tion?<span>
</span></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>How to send the file to client <span>by </span>using HttpResponse<span> class</span>?<span>
</span></p>
<h2>Running the Sample</h2>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span><span>1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Open the CSASPNETResumeDownload .sln. Expand the CSASPNETResumeDownload web application. Double click the Web.config file and find the &quot;appSettings&quot; node, then modify the value of the &quot;FilePath&quot; key with the physical path of the download
 file.</p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span><span>2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Right click the ResumeDownloadPage.htm, select and click the &quot;Set
<span class="GramE">As</span> Start Page&quot; item then press &quot;F5&quot; to start debug<span>ging</span> the project.</p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,sans-serif"><img src="150478-image.png" alt="" width="531" height="343" align="middle">
</span></p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span><span>3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>We will see a Button control named &quot;Download&quot; on the page. Please click it.</p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span><span>4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>After completing Step 3 you will see the IE download file dialog on the bottom of the browser.</p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,sans-serif"><img src="150479-image.png" alt="" width="591" height="340" align="middle">
</span></p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span><span>5.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Please click &quot;Save&quot; button and select a path to save the file, then IE download file dialog will show like below.<span>
</span></p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,sans-serif"><img src="150480-image.png" alt="" width="591" height="348" align="middle">
</span></p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span><span>6.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>You can click the &quot;Pause&quot; button then the download operation will in<span>terrupt.
</span></p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,sans-serif"><img src="150481-image.png" alt="" width="595" height="348" align="middle">
</span></p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span><span>7.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Click the &quot;Resume&quot; button to restart the download operation.</p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,sans-serif"><img src="150482-image.png" alt="" width="592" height="348" align="middle">
</span></p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span><span>8.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span>&nbsp;</span>When the download is finished, the dialog will show like below.</p>
<p class="MsoNormal" style="margin-bottom:10.0pt; line-height:115%"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,sans-serif"><img src="150483-image.png" alt="" width="595" height="350" align="middle">
</span></p>
<h2>Using the Code</h2>
<p class="MsoNormal"><span style="font-size:11.0pt; font-family:&quot;Calibri&quot;,sans-serif">The code sample provides the following reusable functions:</span></p>
<h3>1. How to get and process the HTTP Web request by custom HttpHandler?</h3>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">- C# code snippet -
public class DownloadHttpHandler : IHttpHandler
{
    public void ProcessRequest(HttpContext context)
    {
        string filePath = ConfigurationManager.AppSettings[&quot;FilePath&quot;];
        Downloader.DownloadFile(context, filePath);
    }
    public bool IsReusable
    {
        get { return false; }
    }
- end -
</pre>
<div class="preview">
<pre class="csharp">-&nbsp;C#&nbsp;code&nbsp;snippet&nbsp;-&nbsp;
<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">class</span>&nbsp;DownloadHttpHandler&nbsp;:&nbsp;IHttpHandler&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;ProcessRequest(HttpContext&nbsp;context)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;filePath&nbsp;=&nbsp;ConfigurationManager.AppSettings[<span class="cs__string">&quot;FilePath&quot;</span>];&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Downloader.DownloadFile(context,&nbsp;filePath);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">public</span>&nbsp;<span class="cs__keyword">bool</span>&nbsp;IsReusable&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">get</span>&nbsp;{&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__keyword">false</span>;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
-&nbsp;end&nbsp;-&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;
<h3>2. How to get the HTTP response header information?&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;</h3>
</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">- C# code snippet -
/// &lt;summary&gt;
/// Get the response header by the http request.
/// &lt;/summary&gt;
/// &lt;param name=&quot;httpRequest&quot;&gt;&lt;/param&gt;
/// &lt;param name=&quot;fileInfo&quot;&gt;&lt;/param&gt;
/// &lt;returns&gt;&lt;/returns&gt;
private static HttpResponseHeader GetResponseHeader(HttpRequest httpRequest, FileInfo fileInfo)
{
    if (httpRequest == null)
    {
        return null;
    }
    if (fileInfo == null)
    {
        return null;
    }
    long startPosition = 0;
    string contentRange = &quot;&quot;;
    string fileName = fileInfo.Name;
    long fileLength = fileInfo.Length;
    string lastUpdateTimeStr = fileInfo.LastWriteTimeUtc.ToString();
    string eTag = HttpUtility.UrlEncode(fileName, Encoding.UTF8) &#43; &quot; &quot; &#43; lastUpdateTimeStr;
    string contentDisposition = &quot;attachment;filename=&quot; &#43; HttpUtility.UrlEncode(fileName, Encoding.UTF8).Replace(&quot;&#43;&quot;, &quot;%20&quot;);
    if (httpRequest.Headers[&quot;Range&quot;] != null)
    {
        string[] range = httpRequest.Headers[&quot;Range&quot;].Split(new char[] { '=', '-' });
        startPosition = Convert.ToInt64(range[1]);
        if (startPosition &lt; 0 || startPosition &gt;= fileLength)
        {
            return null;
        }
    }
    if (httpRequest.Headers[&quot;If-Range&quot;] != null)
    {
        if (httpRequest.Headers[&quot;If-Range&quot;].Replace(&quot;\&quot;&quot;, &quot;&quot;) != eTag)
        {
            startPosition = 0;
        }
    }
    string contentLength = (fileLength - startPosition).ToString();
    if (startPosition &gt; 0)
    {
        contentRange = string.Format(&quot; bytes {0}-{1}/{2}&quot;, startPosition, fileLength - 1, fileLength);
    }
    HttpResponseHeader responseHeader = new HttpResponseHeader();
    responseHeader.AcceptRanges = &quot;bytes&quot;;
    responseHeader.Connection = &quot;Keep-Alive&quot;;
    responseHeader.ContentDisposition = contentDisposition;
    responseHeader.ContentEncoding = Encoding.UTF8;
    responseHeader.ContentLength = contentLength;
    responseHeader.ContentRange = contentRange;
    responseHeader.ContentType = &quot;application/octet-stream&quot;;
    responseHeader.Etag = eTag;
    responseHeader.LastModified = lastUpdateTimeStr;
    return responseHeader;
}
- end -
</pre>
<div class="preview">
<pre class="csharp">-&nbsp;C#&nbsp;code&nbsp;snippet&nbsp;-&nbsp;
<span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;Get&nbsp;the&nbsp;response&nbsp;header&nbsp;by&nbsp;the&nbsp;http&nbsp;request.</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;httpRequest&quot;&gt;&lt;/param&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;fileInfo&quot;&gt;&lt;/param&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;returns&gt;&lt;/returns&gt;</span>&nbsp;
<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;HttpResponseHeader&nbsp;GetResponseHeader(HttpRequest&nbsp;httpRequest,&nbsp;FileInfo&nbsp;fileInfo)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(httpRequest&nbsp;==&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__keyword">null</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(fileInfo&nbsp;==&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__keyword">null</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">long</span>&nbsp;startPosition&nbsp;=&nbsp;<span class="cs__number">0</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;contentRange&nbsp;=&nbsp;<span class="cs__string">&quot;&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;fileName&nbsp;=&nbsp;fileInfo.Name;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">long</span>&nbsp;fileLength&nbsp;=&nbsp;fileInfo.Length;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;lastUpdateTimeStr&nbsp;=&nbsp;fileInfo.LastWriteTimeUtc.ToString();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;eTag&nbsp;=&nbsp;HttpUtility.UrlEncode(fileName,&nbsp;Encoding.UTF8)&nbsp;&#43;&nbsp;<span class="cs__string">&quot;&nbsp;&quot;</span>&nbsp;&#43;&nbsp;lastUpdateTimeStr;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;contentDisposition&nbsp;=&nbsp;<span class="cs__string">&quot;attachment;filename=&quot;</span>&nbsp;&#43;&nbsp;HttpUtility.UrlEncode(fileName,&nbsp;Encoding.UTF8).Replace(<span class="cs__string">&quot;&#43;&quot;</span>,&nbsp;<span class="cs__string">&quot;%20&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(httpRequest.Headers[<span class="cs__string">&quot;Range&quot;</span>]&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>[]&nbsp;range&nbsp;=&nbsp;httpRequest.Headers[<span class="cs__string">&quot;Range&quot;</span>].Split(<span class="cs__keyword">new</span>&nbsp;<span class="cs__keyword">char</span>[]&nbsp;{&nbsp;<span class="cs__string">'='</span>,&nbsp;<span class="cs__string">'-'</span>&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;startPosition&nbsp;=&nbsp;Convert.ToInt64(range[<span class="cs__number">1</span>]);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(startPosition&nbsp;&lt;&nbsp;<span class="cs__number">0</span>&nbsp;||&nbsp;startPosition&nbsp;&gt;=&nbsp;fileLength)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;<span class="cs__keyword">null</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(httpRequest.Headers[<span class="cs__string">&quot;If-Range&quot;</span>]&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(httpRequest.Headers[<span class="cs__string">&quot;If-Range&quot;</span>].Replace(<span class="cs__string">&quot;\&quot;&quot;</span>,&nbsp;<span class="cs__string">&quot;&quot;</span>)&nbsp;!=&nbsp;eTag)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;startPosition&nbsp;=&nbsp;<span class="cs__number">0</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>&nbsp;contentLength&nbsp;=&nbsp;(fileLength&nbsp;-&nbsp;startPosition).ToString();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(startPosition&nbsp;&gt;&nbsp;<span class="cs__number">0</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;contentRange&nbsp;=&nbsp;<span class="cs__keyword">string</span>.Format(<span class="cs__string">&quot;&nbsp;bytes&nbsp;{0}-{1}/{2}&quot;</span>,&nbsp;startPosition,&nbsp;fileLength&nbsp;-&nbsp;<span class="cs__number">1</span>,&nbsp;fileLength);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;HttpResponseHeader&nbsp;responseHeader&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;HttpResponseHeader();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;responseHeader.AcceptRanges&nbsp;=&nbsp;<span class="cs__string">&quot;bytes&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;responseHeader.Connection&nbsp;=&nbsp;<span class="cs__string">&quot;Keep-Alive&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;responseHeader.ContentDisposition&nbsp;=&nbsp;contentDisposition;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;responseHeader.ContentEncoding&nbsp;=&nbsp;Encoding.UTF8;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;responseHeader.ContentLength&nbsp;=&nbsp;contentLength;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;responseHeader.ContentRange&nbsp;=&nbsp;contentRange;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;responseHeader.ContentType&nbsp;=&nbsp;<span class="cs__string">&quot;application/octet-stream&quot;</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;responseHeader.Etag&nbsp;=&nbsp;eTag;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;responseHeader.LastModified&nbsp;=&nbsp;lastUpdateTimeStr;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>&nbsp;responseHeader;&nbsp;
}&nbsp;
-&nbsp;end&nbsp;-&nbsp;</pre>
</div>
</div>
</div>
<h3>3. How to send the file to client by using HttpResponse class?</h3>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">- C# code snippet -
/// &lt;summary&gt;
/// Send the download file to the client.
/// &lt;/summary&gt;
/// &lt;param name=&quot;httpResponse&quot;&gt;&lt;/param&gt;
/// &lt;param name=&quot;responseHeader&quot;&gt;&lt;/param&gt;
/// &lt;param name=&quot;fileStream&quot;&gt;&lt;/param&gt;
private static void SendDownloadFile(HttpResponse httpResponse, HttpResponseHeader responseHeader, Stream fileStream)
{
    if (httpResponse == null || responseHeader == null)
    {
        return;
    }
    if (!string.IsNullOrEmpty(responseHeader.ContentRange))
    {
        httpResponse.StatusCode = 206;
        // Set the start position of the reading files.
        string[] range = responseHeader.ContentRange.Split(new char[] { ' ','=', '-' });
        fileStream.Position = Convert.ToInt64(range[2]);
    }
    httpResponse.Clear();
    httpResponse.Buffer = false;
    httpResponse.AppendHeader(&quot;Accept-Ranges&quot;, responseHeader.AcceptRanges);
    httpResponse.AppendHeader(&quot;Connection&quot;, responseHeader.Connection);
    httpResponse.AppendHeader(&quot;Content-Disposition&quot;, responseHeader.ContentDisposition);
    httpResponse.ContentEncoding = responseHeader.ContentEncoding;
    httpResponse.AppendHeader(&quot;Content-Length&quot;, responseHeader.ContentLength);
    if (!string.IsNullOrEmpty(responseHeader.ContentRange))
    {
        httpResponse.AppendHeader(&quot;Content-Range&quot;, responseHeader.ContentRange);
    }
    httpResponse.ContentType = responseHeader.ContentType;
    httpResponse.AppendHeader(&quot;Etag&quot;, &quot;\&quot;&quot; &#43; responseHeader.Etag &#43; &quot;\&quot;&quot;);
    httpResponse.AppendHeader(&quot;Last-Modified&quot;, responseHeader.LastModified);
    Byte[] buffer = new Byte[10240];
    long fileLength = Convert.ToInt64(responseHeader.ContentLength);
    // Send file to client.
    while (fileLength &gt; 0)
    {
        if (httpResponse.IsClientConnected)
        {
            int length = fileStream.Read(buffer, 0, 10240);
            httpResponse.OutputStream.Write(buffer, 0, length);
            httpResponse.Flush();
            fileLength = fileLength - length;
        }
        else
        {
            fileLength = -1;
        }
    }
}
    }
- end -
</pre>
<div class="preview">
<pre class="csharp">-&nbsp;C#&nbsp;code&nbsp;snippet&nbsp;-&nbsp;
<span class="cs__com">///&nbsp;&lt;summary&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;Send&nbsp;the&nbsp;download&nbsp;file&nbsp;to&nbsp;the&nbsp;client.</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;/summary&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;httpResponse&quot;&gt;&lt;/param&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;responseHeader&quot;&gt;&lt;/param&gt;</span>&nbsp;
<span class="cs__com">///&nbsp;&lt;param&nbsp;name=&quot;fileStream&quot;&gt;&lt;/param&gt;</span>&nbsp;
<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">static</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;SendDownloadFile(HttpResponse&nbsp;httpResponse,&nbsp;HttpResponseHeader&nbsp;responseHeader,&nbsp;Stream&nbsp;fileStream)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(httpResponse&nbsp;==&nbsp;<span class="cs__keyword">null</span>&nbsp;||&nbsp;responseHeader&nbsp;==&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">return</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(!<span class="cs__keyword">string</span>.IsNullOrEmpty(responseHeader.ContentRange))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpResponse.StatusCode&nbsp;=&nbsp;<span class="cs__number">206</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Set&nbsp;the&nbsp;start&nbsp;position&nbsp;of&nbsp;the&nbsp;reading&nbsp;files.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">string</span>[]&nbsp;range&nbsp;=&nbsp;responseHeader.ContentRange.Split(<span class="cs__keyword">new</span>&nbsp;<span class="cs__keyword">char</span>[]&nbsp;{&nbsp;<span class="cs__string">'&nbsp;'</span>,<span class="cs__string">'='</span>,&nbsp;<span class="cs__string">'-'</span>&nbsp;});&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;fileStream.Position&nbsp;=&nbsp;Convert.ToInt64(range[<span class="cs__number">2</span>]);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;httpResponse.Clear();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;httpResponse.Buffer&nbsp;=&nbsp;<span class="cs__keyword">false</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;httpResponse.AppendHeader(<span class="cs__string">&quot;Accept-Ranges&quot;</span>,&nbsp;responseHeader.AcceptRanges);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;httpResponse.AppendHeader(<span class="cs__string">&quot;Connection&quot;</span>,&nbsp;responseHeader.Connection);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;httpResponse.AppendHeader(<span class="cs__string">&quot;Content-Disposition&quot;</span>,&nbsp;responseHeader.ContentDisposition);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;httpResponse.ContentEncoding&nbsp;=&nbsp;responseHeader.ContentEncoding;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;httpResponse.AppendHeader(<span class="cs__string">&quot;Content-Length&quot;</span>,&nbsp;responseHeader.ContentLength);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(!<span class="cs__keyword">string</span>.IsNullOrEmpty(responseHeader.ContentRange))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpResponse.AppendHeader(<span class="cs__string">&quot;Content-Range&quot;</span>,&nbsp;responseHeader.ContentRange);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;httpResponse.ContentType&nbsp;=&nbsp;responseHeader.ContentType;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;httpResponse.AppendHeader(<span class="cs__string">&quot;Etag&quot;</span>,&nbsp;<span class="cs__string">&quot;\&quot;&quot;</span>&nbsp;&#43;&nbsp;responseHeader.Etag&nbsp;&#43;&nbsp;<span class="cs__string">&quot;\&quot;&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;httpResponse.AppendHeader(<span class="cs__string">&quot;Last-Modified&quot;</span>,&nbsp;responseHeader.LastModified);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Byte[]&nbsp;buffer&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;Byte[<span class="cs__number">10240</span>];&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">long</span>&nbsp;fileLength&nbsp;=&nbsp;Convert.ToInt64(responseHeader.ContentLength);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__com">//&nbsp;Send&nbsp;file&nbsp;to&nbsp;client.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">while</span>&nbsp;(fileLength&nbsp;&gt;&nbsp;<span class="cs__number">0</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(httpResponse.IsClientConnected)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">int</span>&nbsp;length&nbsp;=&nbsp;fileStream.Read(buffer,&nbsp;<span class="cs__number">0</span>,&nbsp;<span class="cs__number">10240</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpResponse.OutputStream.Write(buffer,&nbsp;<span class="cs__number">0</span>,&nbsp;length);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;httpResponse.Flush();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;fileLength&nbsp;=&nbsp;fileLength&nbsp;-&nbsp;length;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">else</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;fileLength&nbsp;=&nbsp;-<span class="cs__number">1</span>;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
-&nbsp;end&nbsp;-&nbsp;
</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2>More Information</h2>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/system.web.httprequest.aspx">MSDN: HttpRequest</a></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/system.web.httpresponse.aspx">MSDN: HttpResponse</a></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
