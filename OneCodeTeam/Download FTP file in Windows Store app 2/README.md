# Download FTP file in Windows Store app
## Requires
- Visual Studio 2012
## License
- MS-LPL
## Technologies
- Windows 8
## Topics
- FTP
## Updated
- 04/16/2013
## Description

<h1><span><a href="http://www.microsoft.com/click/services/Redirect2.ashx?CR_CC=200144425" target="_blank"><img id="79968" src="http://i1.code.msdn.s-msft.com/cswindowsstoreappadditem-a5d7fbcc/image/file/79968/1/dpe_w8_728x90_1022_v2.jpg" alt="" width="728" height="90" style="width:100%"></a></span></h1>
<h1><span>Download a file from FTP server in </span><span>a Windows Store</span><span> app (VBWindowsStoreAppFTPDownloader)
</span></h1>
<h2><span>Introduction </span></h2>
<p class="MsoNormal">This sample demonstrates how to download a file <span>from
</span><span>IIS</span><span> FTP</span> server in a <span>Windows Store</span><span>
</span>app. It also supplies functions to list the sub folders and files in the server.</p>
<p class="MsoNormal">To list the sub folders and files in the server, we can use a
<strong>WebRequest</strong> with the <strong>LIST</strong> method, and then parse the response.</p>
<p class="MsoNormal">To download a small file, we can use a <strong>WebRequest</strong> with the
<strong>RETR</strong> method. For large file, it is better to use <strong>BackgroundDownloader</strong>.</p>
<p class="MsoNormal">This app need following capabilities:</p>
<p class="MsoListParagraph"><span style="font-family:Wingdings"><span>&uuml;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;
</span></span></span>Internet (Client)</p>
<p class="MsoListParagraph"><span style="font-family:Wingdings"><span>&uuml;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;
</span></span></span>Private Networks (Client and Server)</p>
<p class="MsoListParagraph"><span style="font-family:Wingdings"><span>&uuml;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;
</span></span></span>Enterprise Authentication</p>
<p class="MsoNormal">Different FTP server may have different response to the LIST command, this app works for the IIS FTP Server, and it will use the default proxy setting to connect to FTP Server.</p>
<h2><span>Running the Sample </span></h2>
<p class="MsoListParagraph">1.<span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Open this sample in VS2012 on Win8 machine, and press F5 to run it.</p>
<p class="MsoListParagraph">2.<span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>After the app is launched, you will see following screen.</p>
<p class="MsoNormal" style="margin-left:22.5pt"><span><img src="73720-image.png" alt="" width="694" height="397" align="middle">
</span></p>
<p class="MsoListParagraph">&nbsp;</p>
<p class="MsoListParagraph">3.<span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>Type the server name, username and password, and then click the Connect button.</p>
<p class="MsoListParagraph" style="margin-left:.25in">&nbsp;<strong><span> <img src="73721-image.png" alt="" width="700" height="401" align="middle">
</span></strong></p>
<p class="MsoListParagraph">4.<span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Click a folder, and you will be navigated to the sub folder.</p>
<p class="MsoListParagraph"><span><img src="73722-image.png" alt="" width="694" height="397" align="middle">
</span></p>
<p class="MsoListParagraph">5.<span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</span>Select one or more items, the AppBar will be opened, and then you can click Download button to
 download the files.</p>
<p class="MsoListParagraph"><span><img src="73723-image.png" alt="" width="693" height="397" align="middle">
</span></p>
<p class="MsoListParagraph">6.<span>&nbsp;&nbsp;&nbsp; </span>When a file is downloaded, a notification will show.</p>
<p class="MsoListParagraph"><span><img src="73724-image.png" alt="" width="684" height="392" align="middle">
</span></p>
<p class="MsoListParagraph"><span>&nbsp;</span></p>
<h2><span>Using the Code </span></h2>
<p class="MsoListParagraph"><span><span>1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>List files and sub folder.</p>
<p class="MsoListParagraph" style="margin-left:1.0in"><span><span>a.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>When run the FTP LIST protocol method to get a detailed listing of the files on an FTP server, the server will response many records of information. Each record represents a file or a directory.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">Public Async Function ListFtpContentAsync(ByVal url As Uri, ByVal credential As ICredentials) As Task(Of IEnumerable(Of FTPFileSystem))
&nbsp;&nbsp;&nbsp; Dim currentUri As Uri = url


&nbsp;&nbsp;&nbsp; ' This request is FtpWebRequest in fact.
&nbsp;&nbsp;&nbsp; Dim request As WebRequest = WebRequest.Create(currentUri)
&nbsp;&nbsp;&nbsp; If credential IsNot Nothing Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; request.Credentials = credential
&nbsp;&nbsp;&nbsp; End If


&nbsp;&nbsp;&nbsp; ' Set the method to LIST.
&nbsp;&nbsp;&nbsp; request.Method = &quot;LIST&quot;


&nbsp;&nbsp;&nbsp; ' Get response.
&nbsp;&nbsp;&nbsp; Using response As WebResponse = Await request.GetResponseAsync()


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ' Get response stream.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Using responseStream As Stream = response.GetResponseStream()


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Using reader As New StreamReader(responseStream)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Dim subDirs As New List(Of FTP.FTPFileSystem)()


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Dim item As String = reader.ReadLine()


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' Find out the FTP Directory Listing Style from the recordString.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Dim style As FTP.FTPDirectoryListingStyle = FTP.FTPDirectoryListingStyle.MSDOS
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; If Not String.IsNullOrEmpty(item) Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; style = FTP.FTPFileSystem.GetDirectoryListingStyle(item)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; End If
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Do While Not String.IsNullOrEmpty(item)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; subDirs.Add(FTP.FTPFileSystem.ParseRecordString(currentUri, item, style))
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; item = reader.ReadLine()
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Loop


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Return subDirs
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; End Using
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; End Using
&nbsp;&nbsp;&nbsp; End Using


End Function

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style="margin-left:1.0in">&nbsp;</p>
<p class="MsoListParagraph" style="margin-left:1.0in"><span><span>b.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Parse record string.</p>
<p class="MsoListParagraph" style="margin-left:1.0in">Depended on the FTP Directory Listing Style of the server,the record is like</p>
<p class="MsoListParagraph" style="margin-left:1.5in"><span><span><span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>i.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>MSDOS</p>
<p class="MsoListParagraph" style="margin-left:1.5in">Directory:<span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>12-13-10<span>&nbsp; </span>12:41PM<span>&nbsp; </span>&lt;DIR&gt;<span>&nbsp;
</span>Folder A</p>
<p class="MsoListParagraph" style="margin-left:1.5in">File:<span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>12-13-10<span>&nbsp; </span>12:41PM<span>&nbsp; </span>[Size] File B<span>&nbsp;
</span></p>
<p class="MsoListParagraph" style="margin-left:1.5in">&nbsp;</p>
<p class="MsoListParagraph" style="margin-left:1.5in"><span><span><span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>ii.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>UNIX</p>
<p class="MsoListParagraph" style="margin-left:1.5in">Directory:<span>&nbsp;&nbsp;&nbsp;
</span>drwxrwxrwx 1 owner group 0 Dec 1 12:00 Folder A</p>
<p class="MsoListParagraph" style="margin-left:1.5in">File:<span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>-rwxrwxrwx 1 owner group [Size] Dec 1 12:00 File B</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">''' &lt;summary&gt;
''' Find out the FTP Directory Listing Style from the recordString.
''' &lt;/summary&gt;
Public Shared Function GetDirectoryListingStyle(ByVal recordString As String) As FTPDirectoryListingStyle
&nbsp;&nbsp;&nbsp; Dim regex As Regex = New <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Text.RegularExpressions.Regex.aspx" target="_blank" title="Auto generated link to System.Text.RegularExpressions.Regex">System.Text.RegularExpressions.Regex</a>(&quot;^[d-]([r-][w-][x-]){3}$&quot;)


&nbsp;&nbsp;&nbsp; Dim header As String = recordString.Substring(0, 10)


&nbsp;&nbsp;&nbsp; ' If the style is UNIX, then the header is like &quot;drwxrwxrwx&quot;.
&nbsp;&nbsp;&nbsp; If regex.IsMatch(header) Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Return FTPDirectoryListingStyle.UNIX
&nbsp;&nbsp;&nbsp; Else
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Return FTPDirectoryListingStyle.MSDOS
&nbsp;&nbsp;&nbsp; End If
End Function


''' &lt;summary&gt;
''' Get an FTPFileSystem from the recordString. 
''' &lt;/summary&gt;
Public Shared Function ParseRecordString(ByVal baseUrl As Uri, ByVal recordString As String, ByVal type As FTPDirectoryListingStyle) As FTPFileSystem
&nbsp;&nbsp;&nbsp; Dim fileSystem As FTPFileSystem = Nothing


&nbsp;&nbsp;&nbsp; If type = FTPDirectoryListingStyle.UNIX Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; fileSystem = ParseUNIXRecordString(recordString)
&nbsp;&nbsp;&nbsp; Else
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; fileSystem = ParseMSDOSRecordString(recordString)
&nbsp;&nbsp;&nbsp; End If


&nbsp;&nbsp;&nbsp; Dim encodedName As String = <a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Net.WebUtility.UrlEncode.aspx" target="_blank" title="Auto generated link to System.Net.WebUtility.UrlEncode">System.Net.WebUtility.UrlEncode</a>(fileSystem.Name)
&nbsp;&nbsp;&nbsp; encodedName = encodedName.Replace(&quot;&#43;&quot;, &quot;%20&quot;)
&nbsp;&nbsp;&nbsp; encodedName = encodedName.Replace(&quot;%2b&quot;, &quot;&#43;&quot;)


&nbsp;&nbsp;&nbsp; ' Add &quot;/&quot; to the url if it is a directory
&nbsp;&nbsp;&nbsp; fileSystem.Url = New Uri(baseUrl, encodedName &amp; (If(fileSystem.IsDirectory, &quot;/&quot;, String.Empty)))


&nbsp;&nbsp;&nbsp; Return fileSystem
End Function


''' &lt;summary&gt;
''' The recordString is like
''' Directory: drwxrwxrwx&nbsp;&nbsp; 1 owner&nbsp;&nbsp;&nbsp; group&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 0 Dec 13 11:25 Folder A
''' File:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; -rwxrwxrwx&nbsp;&nbsp; 1 owner&nbsp;&nbsp;&nbsp; group&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 1024 Dec 13 11:25 File B
''' NOTE: The date segment does not contains year.
''' &lt;/summary&gt;
Shared Function ParseUNIXRecordString(ByVal recordString As String) As FTPFileSystem
&nbsp;&nbsp;&nbsp; Dim fileSystem As New FTPFileSystem()


&nbsp;&nbsp;&nbsp; fileSystem.OriginalRecordString = recordString.Trim()
&nbsp;&nbsp;&nbsp; fileSystem.DirectoryListingStyle = FTPDirectoryListingStyle.UNIX


&nbsp;&nbsp;&nbsp; ' The segments is like &quot;drwxrwxrwx&quot;, &quot;&quot;,&nbsp; &quot;&quot;, &quot;1&quot;, &quot;owner&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, 
&nbsp;&nbsp;&nbsp;&nbsp;' &quot;group&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, 
&nbsp;&nbsp;&nbsp;&nbsp;' &quot;0&quot;, &quot;Dec&quot;, &quot;13&quot;, &quot;11:25&quot;, &quot;Folder&quot;, &quot;A&quot;.
&nbsp;&nbsp;&nbsp; Dim segments() As String = fileSystem.OriginalRecordString.Split(&quot; &quot;c)


&nbsp;&nbsp;&nbsp; Dim index As Integer = 0


&nbsp;&nbsp;&nbsp; ' The permission segment is like &quot;drwxrwxrwx&quot;.
&nbsp;&nbsp;&nbsp; Dim permissionsegment As String = segments(index)


&nbsp;&nbsp;&nbsp; ' If the property start with 'd', then it means a directory.
&nbsp;&nbsp;&nbsp; fileSystem.IsDirectory = permissionsegment.Chars(0) = &quot;d&quot;c


&nbsp;&nbsp;&nbsp; ' Skip the empty segments.
&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Do While segments(index) = String.Empty
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Loop


&nbsp;&nbsp;&nbsp; ' Skip the directories segment.


&nbsp;&nbsp;&nbsp; ' Skip the empty segments.
&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Do While segments(index) = String.Empty
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Loop


&nbsp;&nbsp;&nbsp; ' Skip the owner segment.


&nbsp;&nbsp;&nbsp; ' Skip the empty segments.
&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Do While segments(index) = String.Empty
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Loop


&nbsp;&nbsp;&nbsp; ' Skip the group segment.


&nbsp;&nbsp;&nbsp; ' Skip the empty segments.
&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Do While segments(index) = String.Empty
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Loop


&nbsp;&nbsp;&nbsp; ' If this fileSystem is a file, then the size is larger than 0. 
&nbsp;&nbsp;&nbsp;&nbsp;fileSystem.Size = Long.Parse(segments(index))


&nbsp;&nbsp;&nbsp; ' Skip the empty segments.
&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Do While segments(index) = String.Empty
&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;index &#43;= 1
&nbsp;&nbsp;&nbsp; Loop


&nbsp;&nbsp;&nbsp; ' The month segment.
&nbsp;&nbsp;&nbsp; Dim monthsegment As String = segments(index)


&nbsp;&nbsp;&nbsp; ' Skip the empty segments.
&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Do While segments(index) = String.Empty
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Loop


&nbsp;&nbsp;&nbsp; ' The day segment.
&nbsp;&nbsp;&nbsp; Dim daysegment As String = segments(index)


&nbsp;&nbsp;&nbsp; ' Skip the empty segments.
&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Do While segments(index) = String.Empty
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Loop


&nbsp;&nbsp;&nbsp; ' The time segment.
&nbsp;&nbsp;&nbsp; Dim timesegment As String = segments(index)


&nbsp;&nbsp;&nbsp; fileSystem.ModifiedTime = Date.Parse(String.Format(&quot;{0} {1} {2} &quot;, timesegment, monthsegment, daysegment))


&nbsp;&nbsp;&nbsp; ' Skip the empty segments.
&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Do While segments(index) = String.Empty
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Loop


&nbsp;&nbsp;&nbsp; ' Calculate the index of the file name part in the original string.
&nbsp;&nbsp;&nbsp; Dim filenameIndex As Integer = 0


&nbsp;&nbsp;&nbsp; For i As Integer = 0 To index - 1
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ' &quot;&quot; represents ' ' in the original string.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; If segments(i) = String.Empty Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; filenameIndex &#43;= 1
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Else
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; filenameIndex &#43;= segments(i).Length &#43; 1
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; End If
&nbsp;&nbsp;&nbsp; Next i
&nbsp;&nbsp;&nbsp; ' The file name may include many segments because the name can contain ' '.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
&nbsp;&nbsp;&nbsp;&nbsp;fileSystem.Name = fileSystem.OriginalRecordString.Substring(filenameIndex).Trim()


&nbsp;&nbsp;&nbsp; Return fileSystem
End Function


''' &lt;summary&gt;
''' 12-13-10&nbsp; 12:41PM&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &lt;DIR&gt;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Folder A
''' &lt;/summary&gt;
''' &lt;param name=&quot;recordString&quot;&gt;&lt;/param&gt;
''' &lt;returns&gt;&lt;/returns&gt;
Shared Function ParseMSDOSRecordString(ByVal recordString As String) As FTPFileSystem
&nbsp;&nbsp;&nbsp; Dim fileSystem As New FTPFileSystem()


&nbsp;&nbsp;&nbsp; fileSystem.OriginalRecordString = recordString.Trim()
&nbsp;&nbsp;&nbsp; fileSystem.DirectoryListingStyle = FTPDirectoryListingStyle.MSDOS


&nbsp;&nbsp;&nbsp; ' The segments is like &quot;12-13-10&quot;,&nbsp; &quot;&quot;, &quot;12:41PM&quot;, &quot;&quot;, &quot;&quot;,&quot;&quot;, &quot;&quot;,
&nbsp;&nbsp;&nbsp; ' &quot;&quot;, &quot;&quot;, &quot;&lt;DIR&gt;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;&quot;, &quot;Folder&quot;, &quot;A&quot;.
&nbsp;&nbsp;&nbsp; Dim segments() As String = fileSystem.OriginalRecordString.Split(&quot; &quot;c)


&nbsp;&nbsp;&nbsp; Dim index As Integer = 0


&nbsp;&nbsp;&nbsp; ' The date segment is like &quot;12-13-10&quot; instead of &quot;12-13-2010&quot; if Four-digit years
&nbsp;&nbsp;&nbsp; ' is not checked in IIS.
&nbsp;&nbsp;&nbsp; Dim dateSegment As String = segments(index)
&nbsp;&nbsp;&nbsp; Dim dateSegments() As String = dateSegment.Split(New Char() {&quot;-&quot;c}, StringSplitOptions.RemoveEmptyEntries)


&nbsp;&nbsp;&nbsp; Dim month As Integer = Integer.Parse(dateSegments(0))
&nbsp;&nbsp;&nbsp; Dim day As Integer = Integer.Parse(dateSegments(1))
&nbsp;&nbsp;&nbsp; Dim year As Integer = Integer.Parse(dateSegments(2))


&nbsp;&nbsp;&nbsp; ' If year &gt;=50 and year &lt;100, then&nbsp; it means the year 19**
&nbsp;&nbsp;&nbsp; If year &gt;= 50 AndAlso year &lt; 100 Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; year &#43;= 1900


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ' If year &lt;50, then it means the year 20**
&nbsp;&nbsp;&nbsp; ElseIf year &lt; 50 Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; year &#43;= 2000
&nbsp;&nbsp;&nbsp; End If


&nbsp;&nbsp;&nbsp; ' Skip the empty segments.
&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Do While segments(index) = String.Empty
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Loop


 &nbsp;&nbsp;&nbsp;' The time segment.
&nbsp;&nbsp;&nbsp; Dim timesegment As String = segments(index)


&nbsp;&nbsp;&nbsp; fileSystem.ModifiedTime = Date.Parse(String.Format(&quot;{0}-{1}-{2} {3}&quot;, year, month, day, timesegment))


&nbsp;&nbsp;&nbsp; ' Skip the empty segments.
&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Do While segments(index) = String.Empty
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Loop


&nbsp;&nbsp;&nbsp; ' The size or directory segment.
&nbsp;&nbsp;&nbsp; ' If this segment is &quot;&lt;DIR&gt;&quot;, then it means a directory, else it means the
&nbsp;&nbsp;&nbsp; ' file size.
&nbsp;&nbsp;&nbsp; Dim sizeOrDirSegment As String = segments(index)


&nbsp;&nbsp;&nbsp; fileSystem.IsDirectory = sizeOrDirSegment.Equals(&quot;&lt;DIR&gt;&quot;, StringComparison.OrdinalIgnoreCase)


&nbsp;&nbsp;&nbsp; ' If this fileSystem is a file, then the size is larger than 0. 
&nbsp;&nbsp;&nbsp;&nbsp;If Not fileSystem.IsDirectory Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; fileSystem.Size = Long.Parse(sizeOrDirSegment)
&nbsp;&nbsp;&nbsp; End If


&nbsp;&nbsp;&nbsp; ' Skip the empty segments.
&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Do While segments(index) = String.Empty
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; index &#43;= 1
&nbsp;&nbsp;&nbsp; Loop


&nbsp;&nbsp;&nbsp; ' Calculate the index of the file name part in the original string.
&nbsp;&nbsp;&nbsp; Dim filenameIndex As Integer = 0


&nbsp;&nbsp;&nbsp; For i As Integer = 0 To index - 1
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ' &quot;&quot; represents ' ' in the original string.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; If segments(i) = String.Empty Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; filenameIndex &#43;= 1
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Else
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; filenameIndex &#43;= segments(i).Length &#43; 1
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; End If
&nbsp;&nbsp;&nbsp; Next i
&nbsp;&nbsp;&nbsp; ' The file name may include many segments because the name can contain ' '.&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
&nbsp;&nbsp;&nbsp;&nbsp;fileSystem.Name = fileSystem.OriginalRecordString.Substring(filenameIndex).Trim()


&nbsp;&nbsp;&nbsp; Return fileSystem
End Function
&nbsp;&nbsp; 

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style="margin-left:1.0in">&nbsp;</p>
<p class="MsoListParagraph"><strong><span><span>2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span></strong>Download files<strong> </strong></p>
<p class="MsoListParagraph" style="margin-left:1.0in"><span><span>a.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>If the file is less than 1MB, download it using WebRequest.<span>
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">''' &lt;summary&gt;
''' Download a single file from FTP server using WebRequest.
''' &lt;/summary&gt;
Public Async Function DownloadFTPFileAsync(ByVal item As FTPFileSystem, ByVal targetFile As StorageFile, ByVal credential As ICredentials) As Task(Of DownloadCompletedEventArgs)


&nbsp;&nbsp;&nbsp; ' This request is FtpWebRequest in fact.
&nbsp;&nbsp;&nbsp; Dim request As WebRequest = WebRequest.Create(item.Url)


&nbsp;&nbsp;&nbsp; If credential IsNot Nothing Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; request.Credentials = credential
&nbsp;&nbsp;&nbsp; End If


&nbsp;&nbsp;&nbsp; ' Set the method to Download File
&nbsp;&nbsp;&nbsp; request.Method = &quot;RETR&quot;


&nbsp;&nbsp;&nbsp; ' Open the file for write.
&nbsp;&nbsp;&nbsp; Using fileStream As IRandomAccessStream = Await targetFile.OpenAsync(FileAccessMode.ReadWrite)


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ' Get response.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Using response As WebResponse = Await request.GetResponseAsync()


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ' Get response stream.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Using responseStream As Stream = response.GetResponseStream()
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Dim downloadBuffer(2047) As Byte
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Dim bytesSize As Integer = 0


&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' Download the file until the download is completed.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Do


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ' Read a buffer of data from the stream.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; bytesSize = responseStream.Read(downloadBuffer, 0, downloadBuffer.Length)
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;If bytesSize = 0 Then
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Exit Do
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; End If


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ' Write buffer to the file.
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Await fileStream.WriteAsync(downloadBuffer.AsBuffer())
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Loop
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; End Using
&nbsp;&nbsp;&nbsp; &nbsp;&nbsp;&nbsp;&nbsp;End Using
&nbsp;&nbsp;&nbsp; End Using


&nbsp;&nbsp;&nbsp; Return New DownloadCompletedEventArgs With {.RequestFile = item.Url, .LocalFile = targetFile}
End Function
&nbsp;&nbsp; 

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style="margin-left:1.0in"><span>&nbsp;</span></p>
<p class="MsoListParagraph" style="margin-left:1.0in"><span><span>b.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span>Download large file using BackgroundDownloader. </span>
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">' Create a BackgroundDownloader
Dim downloader As New BackgroundDownloader()
Dim download As DownloadOperation = downloader.CreateDownload(urlWithCredential, targetFile)
Await download.StartAsync().AsTask(progressCallback)





</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style="margin-left:1.0in"><span>&nbsp;</span></p>
<h2><span>More Information </span></h2>
<p class="MsoListParagraph"><span style="font-family:Symbol">&bull;</span><span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>BackgroundDownloader class (Windows)</p>
<p class="MsoListParagraph"><a href="http://msdn.microsoft.com/en-us/library/windows/apps/windows.networking.backgroundtransfer.backgrounddownloader(v=win.10).aspx">http://msdn.microsoft.com/en-us/library/windows/apps/windows.networking.backgroundtransfer.backgrounddownloader(v=win.10).aspx</a></p>
<p class="MsoListParagraph"><span style="font-family:Symbol">&bull;</span><span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>Background Transfer sample</p>
<p class="MsoListParagraph"><a href="http://code.msdn.microsoft.com/windowsapps/Background-Transfer-Sample-d7833f61">http://code.msdn.microsoft.com/windowsapps/Background-Transfer-Sample-d7833f61</a></p>
<p class="MsoListParagraph"><span style="font-family:Symbol">&bull;</span><span style="font-size:7.0pt; line-height:115%; font-family:&quot;Times New Roman&quot;,&quot;serif&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span>WebRequest Class</p>
<p class="MsoListParagraph"><a href="http://msdn.microsoft.com/en-us/library/5t9y35bd.aspx">http://msdn.microsoft.com/en-us/library/5t9y35bd.aspx</a></p>
<p class="MsoListParagraph">&nbsp;</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
