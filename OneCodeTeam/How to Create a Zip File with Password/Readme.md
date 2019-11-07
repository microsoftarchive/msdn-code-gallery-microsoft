# How to Create a Zip File with Password
## Requires
- Visual Studio 2010
## License
- MIT
## Technologies
- .NET
## Topics
- password
- zip
## Updated
- 04/19/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner" alt="">
</a></div>
<p class="MsoNormal" style="text-align:center"><strong>Encrypt and compress(ZIP) a text file.
</strong></p>
<p class="MsoNormal" style="text-align:center"><strong>&nbsp;</strong></p>
<p class="MsoNormal"><strong>Requirement</strong>: Encrypt a text file and then compress it by adding it to a ZIP file.</p>
<p class="MsoNormal"><strong>Technology:</strong> Windows Forms, Visual Studio 2010, VB.NET</p>
<p class="MsoNormal">The sample demonstrates how to encrypt the contents of a text file and then compress it.</p>
<p class="MsoNormal" style="line-height:106%"><strong><span>To Run the sample</span></strong><span>:
</span></p>
<ol>
<li><span style="text-indent:-0.25in">Open project 2010VBEncryptCompress.sln in Visual Studio 2010.</span>
</li><li><span style="text-indent:-0.25in">Run the application</span> </li><li><span style="text-indent:-0.25in">Click Browse and select a text file.</span>
</li><li><span style="text-indent:-0.25in">Click compress. The ZIP file will be created in the same location as the original text file.</span>
</li></ol>
<p class="MsoNormal"><strong>&nbsp;</strong></p>
<p class="MsoNormal"><strong>Code Used: </strong></p>
<p class="MsoListParagraph" style="text-indent:-.25in"><span><span>&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
</span></span>On Compress Button click event</p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:.0001pt; margin-left:.25in; line-height:normal; text-autospace:none">
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>
<pre class="hidden">Try
                ' Reading all the data from the source file.
                originalData = File.ReadAllText(tbFilePath.Text)
                ' Creating a new instance of the AesCryptoServiceProvider class.
                ' This generates a new key and initialization vector (IV).
                Dim myAes As AesCryptoServiceProvider = New AesCryptoServiceProvider
                ' Encrypt the string to an array of bytes.
                encryptedData = EncryptStringToBytes_Aes(originalData, myAes.Key, myAes.IV)
                sourceFileDirectory = Path.GetDirectoryName(tbFilePath.Text)
                sourceFileName = Path.GetFileNameWithoutExtension(tbFilePath.Text)

                File.WriteAllText((sourceFileDirectory &#43; (&quot;\&quot; &#43; (sourceFileName &#43; &quot;_encrypted.txt&quot;))), Convert.ToBase64String(encryptedData))

                If File.Exists((sourceFileDirectory &#43; &quot;\Output.zip&quot;)) Then
                    File.Delete((sourceFileDirectory &#43; &quot;\Output.zip&quot;))
                End If

                Dim zipPackage As Package = Package.Open(((sourceFileDirectory &#43; &quot;\Output.zip&quot;)), IO.FileMode.OpenOrCreate, IO.FileAccess.ReadWrite)

                encryptedFileName = Path.GetFileName((sourceFileDirectory &#43; (&quot;\&quot; _
                                &#43; (sourceFileName &#43; &quot;_encrypted.txt&quot;))))
                Dim zipPartUri As Uri = PackUriHelper.CreatePartUri(New Uri(encryptedFileName, UriKind.Relative))
                Dim zipPackagePart As PackagePart = zipPackage.CreatePart(zipPartUri, &quot;&quot;, CompressionOption.Normal)
                Dim sourceFileStream As FileStream = New FileStream((sourceFileDirectory &#43; (&quot;\&quot; _
                                &#43; (sourceFileName &#43; &quot;_encrypted.txt&quot;))), FileMode.Open, FileAccess.Read)
                
                Dim destinationFileStream As Stream = zipPackagePart.GetStream

                'Dim contentType As String = Net.Mime.MediaTypeNames.Application.Zip
                Dim zipContent As Byte() = File.ReadAllBytes((sourceFileDirectory &#43; (&quot;\&quot; &#43; (sourceFileName &#43; &quot;_encrypted.txt&quot;))))
                zipPackagePart.GetStream().Write(zipContent, 0, zipContent.Length)

                zipPackage.Close()
                sourceFileStream.Close()

                MessageBox.Show((sourceFileName &#43; &quot;.txt is encrypted and zipped successfully.&quot;), &quot;Encrypt Compress&quot;, MessageBoxButtons.OK, MessageBoxIcon.Information)
                File.Delete((sourceFileDirectory &#43; (&quot;\&quot; &#43; (sourceFileName &#43; &quot;_encrypted.txt&quot;))))
                Process.Start(sourceFileDirectory)

            Catch exception As Exception
                MessageBox.Show(exception.Message, &quot;Encrypt Compress&quot;, MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try</pre>
<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Try</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Reading&nbsp;all&nbsp;the&nbsp;data&nbsp;from&nbsp;the&nbsp;source&nbsp;file.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;originalData&nbsp;=&nbsp;File.ReadAllText(tbFilePath.Text)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Creating&nbsp;a&nbsp;new&nbsp;instance&nbsp;of&nbsp;the&nbsp;AesCryptoServiceProvider&nbsp;class.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;This&nbsp;generates&nbsp;a&nbsp;new&nbsp;key&nbsp;and&nbsp;initialization&nbsp;vector&nbsp;(IV).</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;myAes&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;AesCryptoServiceProvider&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;AesCryptoServiceProvider&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Encrypt&nbsp;the&nbsp;string&nbsp;to&nbsp;an&nbsp;array&nbsp;of&nbsp;bytes.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;encryptedData&nbsp;=&nbsp;EncryptStringToBytes_Aes(originalData,&nbsp;myAes.Key,&nbsp;myAes.IV)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;sourceFileDirectory&nbsp;=&nbsp;Path.GetDirectoryName(tbFilePath.Text)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;sourceFileName&nbsp;=&nbsp;Path.GetFileNameWithoutExtension(tbFilePath.Text)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;File.WriteAllText((sourceFileDirectory&nbsp;&#43;&nbsp;(<span class="visualBasic__string">&quot;\&quot;&nbsp;&#43;&nbsp;(sourceFileName&nbsp;&#43;&nbsp;&quot;</span>_encrypted.txt&quot;))),&nbsp;Convert.ToBase64String(encryptedData))&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;File.Exists((sourceFileDirectory&nbsp;&#43;&nbsp;<span class="visualBasic__string">&quot;\Output.zip&quot;</span>))&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;File.Delete((sourceFileDirectory&nbsp;&#43;&nbsp;<span class="visualBasic__string">&quot;\Output.zip&quot;</span>))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;zipPackage&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Package&nbsp;=&nbsp;Package.Open(((sourceFileDirectory&nbsp;&#43;&nbsp;<span class="visualBasic__string">&quot;\Output.zip&quot;</span>)),&nbsp;IO.FileMode.OpenOrCreate,&nbsp;IO.FileAccess.ReadWrite)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;encryptedFileName&nbsp;=&nbsp;Path.GetFileName((sourceFileDirectory&nbsp;&#43;&nbsp;(<span class="visualBasic__string">&quot;\&quot;</span>&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&#43;&nbsp;(sourceFileName&nbsp;&#43;&nbsp;<span class="visualBasic__string">&quot;_encrypted.txt&quot;</span>))))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;zipPartUri&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Uri&nbsp;=&nbsp;PackUriHelper.CreatePartUri(<span class="visualBasic__keyword">New</span>&nbsp;Uri(encryptedFileName,&nbsp;UriKind.Relative))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;zipPackagePart&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;PackagePart&nbsp;=&nbsp;zipPackage.CreatePart(zipPartUri,&nbsp;<span class="visualBasic__string">&quot;&quot;</span>,&nbsp;CompressionOption.Normal)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;sourceFileStream&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;FileStream&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;FileStream((sourceFileDirectory&nbsp;&#43;&nbsp;(<span class="visualBasic__string">&quot;\&quot;</span>&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&#43;&nbsp;(sourceFileName&nbsp;&#43;&nbsp;<span class="visualBasic__string">&quot;_encrypted.txt&quot;</span>))),&nbsp;FileMode.Open,&nbsp;FileAccess.Read)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;destinationFileStream&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Stream&nbsp;=&nbsp;zipPackagePart.GetStream&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'Dim&nbsp;contentType&nbsp;As&nbsp;String&nbsp;=&nbsp;Net.Mime.MediaTypeNames.Application.Zip</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;zipContent&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Byte</span>()&nbsp;=&nbsp;File.ReadAllBytes((sourceFileDirectory&nbsp;&#43;&nbsp;(<span class="visualBasic__string">&quot;\&quot;&nbsp;&#43;&nbsp;(sourceFileName&nbsp;&#43;&nbsp;&quot;</span>_encrypted.txt&quot;))))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;zipPackagePart.GetStream().Write(zipContent,&nbsp;<span class="visualBasic__number">0</span>,&nbsp;zipContent.Length)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;zipPackage.Close()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;sourceFileStream.Close()&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MessageBox.Show((sourceFileName&nbsp;&#43;&nbsp;<span class="visualBasic__string">&quot;.txt&nbsp;is&nbsp;encrypted&nbsp;and&nbsp;zipped&nbsp;successfully.&quot;</span>),&nbsp;<span class="visualBasic__string">&quot;Encrypt&nbsp;Compress&quot;</span>,&nbsp;MessageBoxButtons.OK,&nbsp;MessageBoxIcon.Information)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;File.Delete((sourceFileDirectory&nbsp;&#43;&nbsp;(<span class="visualBasic__string">&quot;\&quot;&nbsp;&#43;&nbsp;(sourceFileName&nbsp;&#43;&nbsp;&quot;</span>_encrypted.txt&quot;))))&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Process.Start(sourceFileDirectory)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Catch</span>&nbsp;exception&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;Exception&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MessageBox.Show(exception.Message,&nbsp;<span class="visualBasic__string">&quot;Encrypt&nbsp;Compress&quot;</span>,&nbsp;MessageBoxButtons.OK,&nbsp;MessageBoxIcon.<span class="visualBasic__keyword">Error</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Try</span></pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p></p>
<p class="MsoNormal" style="margin-top:0in; margin-right:0in; margin-bottom:.0001pt; margin-left:.25in; line-height:normal; text-autospace:none">
Encrypt method.</p>
<p class="MsoListParagraph"></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>Visual Basic</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">vb</span>
<pre class="hidden">Private Shared Function EncryptStringToBytes_Aes(ByVal plainText As String, ByVal key() As Byte, ByVal IV() As Byte) As Byte()
        ' Checking the arguments.
        If (plainText.Length = 0) Then
            Throw New ArgumentNullException(&quot;Source file size is ZERO.&quot;)
        End If

        If ((key Is Nothing) _
                    OrElse (key.Length = 0)) Then
            Throw New ArgumentNullException(&quot;Symmetric key is null.&quot;)
        End If

        If ((IV Is Nothing) _
                    OrElse (IV.Length = 0)) Then
            Throw New ArgumentNullException(&quot;Initilization Vector is null.&quot;)
        End If

        Dim encrypted() As Byte

        Dim aesAlg As AesCryptoServiceProvider = New AesCryptoServiceProvider
        aesAlg.Key = key
        aesAlg.IV = IV

        Dim encryptor As ICryptoTransform = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV)

        Dim msEncrypt As MemoryStream = New MemoryStream
        Dim csEncrypt As CryptoStream = New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)
        Dim swEncrypt As StreamWriter = New StreamWriter(csEncrypt)
        ' Write all data to the stream.
        swEncrypt.Write(plainText)
        encrypted = msEncrypt.ToArray

        Return encrypted
    End Function</pre>
<div class="preview">
<pre class="vb"><span class="visualBasic__keyword">Private</span>&nbsp;<span class="visualBasic__keyword">Shared</span>&nbsp;<span class="visualBasic__keyword">Function</span>&nbsp;EncryptStringToBytes_Aes(<span class="visualBasic__keyword">ByVal</span>&nbsp;plainText&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">String</span>,&nbsp;<span class="visualBasic__keyword">ByVal</span>&nbsp;key()&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Byte</span>,&nbsp;<span class="visualBasic__keyword">ByVal</span>&nbsp;IV()&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Byte</span>)&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Byte</span>()&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Checking&nbsp;the&nbsp;arguments.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;(plainText.Length&nbsp;=&nbsp;<span class="visualBasic__number">0</span>)&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Throw</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;ArgumentNullException(<span class="visualBasic__string">&quot;Source&nbsp;file&nbsp;size&nbsp;is&nbsp;ZERO.&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;((key&nbsp;<span class="visualBasic__keyword">Is</span>&nbsp;<span class="visualBasic__keyword">Nothing</span>)&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">OrElse</span>&nbsp;(key.Length&nbsp;=&nbsp;<span class="visualBasic__number">0</span>))&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Throw</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;ArgumentNullException(<span class="visualBasic__string">&quot;Symmetric&nbsp;key&nbsp;is&nbsp;null.&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;((IV&nbsp;<span class="visualBasic__keyword">Is</span>&nbsp;<span class="visualBasic__keyword">Nothing</span>)&nbsp;_&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">OrElse</span>&nbsp;(IV.Length&nbsp;=&nbsp;<span class="visualBasic__number">0</span>))&nbsp;<span class="visualBasic__keyword">Then</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Throw</span>&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;ArgumentNullException(<span class="visualBasic__string">&quot;Initilization&nbsp;Vector&nbsp;is&nbsp;null.&quot;</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">If</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;encrypted()&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;<span class="visualBasic__keyword">Byte</span>&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;aesAlg&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;AesCryptoServiceProvider&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;AesCryptoServiceProvider&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;aesAlg.Key&nbsp;=&nbsp;key&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;aesAlg.IV&nbsp;=&nbsp;IV&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;encryptor&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;ICryptoTransform&nbsp;=&nbsp;aesAlg.CreateEncryptor(aesAlg.Key,&nbsp;aesAlg.IV)&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;msEncrypt&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;MemoryStream&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;MemoryStream&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;csEncrypt&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;CryptoStream&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;CryptoStream(msEncrypt,&nbsp;encryptor,&nbsp;CryptoStreamMode.Write)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Dim</span>&nbsp;swEncrypt&nbsp;<span class="visualBasic__keyword">As</span>&nbsp;StreamWriter&nbsp;=&nbsp;<span class="visualBasic__keyword">New</span>&nbsp;StreamWriter(csEncrypt)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__com">'&nbsp;Write&nbsp;all&nbsp;data&nbsp;to&nbsp;the&nbsp;stream.</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;swEncrypt.Write(plainText)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;encrypted&nbsp;=&nbsp;msEncrypt.ToArray&nbsp;
&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">Return</span>&nbsp;encrypted&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="visualBasic__keyword">End</span>&nbsp;<span class="visualBasic__keyword">Function</span></pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p></p>
<p class="MsoListParagraph">&nbsp;</p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
