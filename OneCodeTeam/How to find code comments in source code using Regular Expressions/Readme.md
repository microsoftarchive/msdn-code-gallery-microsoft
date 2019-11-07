# How to find code comments in source code using Regular Expressions
## Requires
- Visual Studio 2013
## License
- MIT
## Technologies
- .NET
## Topics
- Regular Expressions
- code snippets
- code comments
## Updated
- 09/22/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src=":-onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em>&nbsp;</em></div>
<h1>The project illustrates how <span class="info-text">to find code comments in source code using Regular Expressions
</span></h1>
<h2>Introduction</h2>
<div class="MsoNormal">The project illustrates how <span class="info-text">to find source code comments in source code using Regular Expressions.</span></div>
<div class="MsoNormal">Lots of developers ask about this in the MSDN forums, so we created the code sample to address the frequently asked programming scenario.</div>
<div class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span><span>1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Regular expression library</div>
<div class="MsoListParagraphCxSpMiddle"><a href="http://www.regexlib.com/Search.aspx?k=vb.net&c=-1&m=-1&ps=20">http://www.regexlib.com/Search.aspx?k=vb.net&amp;c=-1&amp;m=-1&amp;ps=20</a></div>
<div class="MsoListParagraphCxSpMiddle"><a href="http://www.regexlib.com/Search.aspx?k=c%23&c=-1&m=-1&ps=20">http://www.regexlib.com/Search.aspx?k=c%23&amp;c=-1&amp;m=-1&amp;ps=20</a></div>
<div class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span><span>2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>General samples and help regarding regex usage</div>
<div class="MsoListParagraphCxSpMiddle"><a href="http://ostermiller.org/findcomment.html">http://ostermiller.org/findcomment.html</a></div>
<div class="MsoListParagraphCxSpMiddle"><a href="http://www.regular-expressions.info/tutorial.html">http://www.regular-expressions.info/tutorial.html</a></div>
<div class="MsoListParagraphCxSpMiddle"><a href="http://www.regexbuddy.com/use.html">http://www.regexbuddy.com/use.html</a></div>
<div class="MsoListParagraphCxSpMiddle"><a href="http://stackoverflow.com/questions/5378662/regex-to-find-comment-in-c-sharp-source-file">http://stackoverflow.com/questions/5378662/regex-to-find-comment-in-c-sharp-source-file</a></div>
<div class="MsoListParagraphCxSpLast">&nbsp;</div>
<h2>Building the Project</h2>
<div class="MsoNormal">&nbsp;</div>
<div class="MsoNormal"><strong><span>&nbsp;</span>Creating a c# console application
</strong></div>
<div class="MsoListParagraphCxSpFirst"><span><span>1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>In the Visual Studio 2013, create a console application</div>
<div class="MsoListParagraphCxSpMiddle"><span><span>2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Using the regex pattern:</div>
<div class="MsoListParagraphCxSpMiddle">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; C# (/\*([^*]<span class="GramE">|[</span>\r\n]|(\*&#43;([^*/]|[\r\n])))*\*&#43;/)|(//.*)</div>
<div class="MsoListParagraphCxSpMiddle">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; VB.NET (/\*([^*]<span class="GramE">|[</span>\r\n]|(\*&#43;([^*/]|[\r\n])))*\*&#43;/)|(//.*)</div>
<div class="MsoListParagraphCxSpMiddle">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; C&#43;&#43; (/\*([^*]<span class="GramE">|[</span>\r\n]|(\*&#43;([^*/]|[\r\n])))*\*&#43;/)|(//.*)<span style="font-size:10.0pt; line-height:115%; font-family:&quot;Courier New&quot;">
</span></div>
<div class="MsoListParagraphCxSpMiddle"><span><span>3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>In this sample, I have tried to read the source code contents from a file.</div>
<div class="MsoListParagraphCxSpLast"><span><span>4.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>In nutshell, read a file line by line and find source code comments if any.</div>
<div class="MsoListParagraph" style="text-align:justify; text-indent:-.25in"><span style="font-size:10.0pt; line-height:115%; font-family:&quot;Courier New&quot;; color:black"><span>-<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;
</span></span></span></div>
<div class="MsoNormal">
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span><span>Visual Basic</span><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">cplusplus</span><span class="hidden">vb</span><span class="hidden">csharp</span>
<pre class="hidden">string pattern(&quot;(/\\*([^*]|[\r\n]|(\\*&#43;([^*/]|[\r\n])))*\\*&#43;/)|(//.*)&quot;);
	regex r(pattern, regex_constants::egrep);
	for (sregex_iterator it(str.begin(), str.end(), r), end;
		it != end;
		&#43;&#43;it)
	{
		cout &lt;&lt; it-&gt;str() &lt;&lt; endl;
	}</pre>
<pre class="hidden">Dim file As String = &quot;../../SourceCode.vb&quot;
        Dim pattern As String = &quot;REM((\t| ).*$|$)|^\'[^\r\n]&#43;$|''[^\r\n]&#43;$&quot;

        ' local variable to get hold of single or multi-line code comments
        Dim sLine As String = String.Empty
        Dim mLine As String = String.Empty

        ' Read the file stream
        Dim stream As New FileStream(file, FileMode.Open, FileAccess.Read)
        Dim reader As New StreamReader(stream)

        While reader.Peek() &lt;&gt; -1
            ' Read a line of the stream reader
            sLine = reader.ReadLine()

            ' Trim the space in the start of the line
            sLine = If((sLine IsNot Nothing), sLine.Trim(), sLine)

            Dim res = Regex.Matches(sLine, pattern)
            If res.Count &gt; 0 Then
                Console.WriteLine(sLine)
            End If
        End While</pre>
<pre class="hidden"> while (reader.Peek() != -1)
            {
                // Read a line of the stream reader
                sLine = reader.ReadLine();

                // Trim the space in the start of the line
                sLine = (sLine != null) ? sLine.Trim() : sLine;

                if (sLine.StartsWith(@&quot;/*&quot;) &amp;&amp; !sLine.EndsWith(@&quot;*/&quot;))
                {
                    mLine = sLine &#43; Environment.NewLine;
                    continue;
                }
                if (mLine.StartsWith(@&quot;/*&quot;))
                {
                    if (sLine.EndsWith(@&quot;*/&quot;))
                    {
                        mLine &#43;= sLine;
                    }
                    else
                    {
                        mLine &#43;= sLine &#43; Environment.NewLine;
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(mLine))
                {
                    var res = Regex.Matches(mLine, pattern);
                    if(res.Count &gt; 0)
                        Console.WriteLine(mLine);
                    mLine = string.Empty;
                }
                else
                {
                    var res = Regex.Matches(sLine, pattern);
                    if(res.Count &gt; 0)
                        Console.WriteLine(sLine);
                }
}</pre>
<div class="preview">
<pre class="cplusplus"><span class="cpp__datatype">string</span>&nbsp;pattern(<span class="cpp__string">&quot;(/\\*([^*]|[\r\n]|(\\*&#43;([^*/]|[\r\n])))*\\*&#43;/)|(//.*)&quot;</span>);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;regex&nbsp;r(pattern,&nbsp;regex_constants::egrep);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cpp__keyword">for</span>&nbsp;(sregex_iterator&nbsp;it(str.begin(),&nbsp;str.end(),&nbsp;r),&nbsp;end;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;it&nbsp;!=&nbsp;end;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&#43;&#43;it)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;cout&nbsp;&lt;&lt;&nbsp;it-&gt;str()&nbsp;&lt;&lt;&nbsp;endl;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}</pre>
</div>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
</div>
<h2>Running the Sample</h2>
<div class="MsoNormal"><span><span>&nbsp;</span></span>&nbsp;</div>
<div class="MsoListParagraphCxSpFirst" style="margin-left:.75in"></div>
<div class="MsoListParagraphCxSpMiddle" style="text-indent:-.25in"><span><span>2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Sharing the sample data by its expression matches:</div>
<div class="MsoListParagraphCxSpMiddle">&nbsp;</div>
<div class="MsoListParagraphCxSpMiddle"><span><img src="154509-image.png" alt="" width="576" height="166" align="middle">
</span></div>
<div class="MsoListParagraphCxSpMiddle">&nbsp;</div>
<div class="MsoListParagraphCxSpLast">&nbsp;</div>
<div class="MsoNormal"></div>
<div class="MsoNormal">&nbsp;</div>
<div style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</div>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
