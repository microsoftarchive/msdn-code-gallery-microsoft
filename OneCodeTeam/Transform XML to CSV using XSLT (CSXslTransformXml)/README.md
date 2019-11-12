# Transform XML to CSV using XSLT (CSXslTransformXml)
## Requires
- Visual Studio 2008
## License
- MS-LPL
## Technologies
- XML
## Topics
- XSL
## Updated
- 07/22/2012
## Description

<h1>CONSOLE APPLICATION <span style="">(</span> CSXslTransformXml)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">This sample project shows how to use XslCompiledTransform to transform an XML data file to .csv file using an XSLT style sheet.<span style="">&nbsp;
</span><span style=""></span></p>
<p class="MsoNormal"><span style="">The <span class="SpellE">XslCompiledTransform</span> class is an XSLT processor that supports the XSLT 1.0 syntax. It is a new implementation and includes performance gains when compared to the obsolete
<span class="SpellE">XslTransform</span> class. The structure of the <span class="SpellE">
XslCompiledTransform</span> class is very similar to the <span class="SpellE">XslTransform</span> class. The Load method loads and compiles the style sheet, while the Transform method executes the XSLT transform.
</span></p>
<p class="MsoNormal"><span style="">Support for the XSLT <span class="GramE">
document(</span>) function and embedded script blocks are disabled by default. These features can be enabled by creating an
<span class="SpellE">XsltSettings</span> object and passing it to the Load method.
</span></p>
<p class="MsoNormal"><span style="">For more information, see Using the <span class="SpellE">
XslCompiledTransform</span> <span class="GramE">Class(</span><a href="http://msdn.microsoft.com/en-us/library/0610k0w4.aspx">http://msdn.microsoft.com/en-us/library/0610k0w4.aspx</a>) and Migrating From the
<span class="SpellE">XslTransform</span> Class(<a href="http://msdn.microsoft.com/en-us/library/66f54faw.aspx">http://msdn.microsoft.com/en-us/library/66f54faw.aspx</a>).
</span></p>
<h2>Using the Code</h2>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">xml</span>

<pre id="codePreview" class="xml">
&lt;?xml version=&quot;1.0&quot; encoding=&quot;utf-8&quot;?&gt;
&lt;xsl:stylesheet version=&quot;1.0&quot; xmlns:xsl=&quot;http://www.w3.org/1999/XSL/Transform&quot;
    xmlns:msxsl=&quot;urn:schemas-microsoft-com:xslt&quot; exclude-result-prefixes=&quot;msxsl&quot;
&gt;

    &lt;xsl:output method=&quot;text&quot;/&gt;


    &lt;xsl:template match=&quot;catalog&quot;&gt;
      &lt;xsl:apply-templates select =&quot;book&quot;/&gt;
    &lt;/xsl:template&gt;


    &lt;xsl:template match=&quot;book&quot;&gt;
      &lt;xsl:text&gt;&#34;&lt;/xsl:text&gt;
      &lt;xsl:value-of select =&quot;@id&quot;/&gt;
      &lt;xsl:text&gt;&#34;&lt;/xsl:text&gt;
      &lt;xsl:value-of select =&quot;','&quot;/&gt;
      &lt;xsl:for-each select =&quot;*&quot;&gt;
        &lt;xsl:text&gt;&#34;&lt;/xsl:text&gt;
        &lt;xsl:value-of select =&quot;.&quot;/&gt;
        &lt;xsl:text&gt;&#34;&lt;/xsl:text&gt;
        &lt;xsl:if test =&quot;position() != last()&quot;&gt;
          &lt;xsl:value-of select =&quot;','&quot;/&gt;
        &lt;/xsl:if&gt;
      &lt;/xsl:for-each&gt;
      &lt;xsl:text&gt;&#xD;&#xa;&lt;/xsl:text&gt;
    &lt;/xsl:template&gt;
&lt;/xsl:stylesheet&gt;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style="">Note: </span></p>
<p class="MsoNormal" style="margin-left:18.0pt">&amp;#34 means the quote mark. Use quote mark above each value to avoid internal<span style="">
</span>comma's <span class="SpellE">effect.Use</span> @ to select the attribute instead of child node.<span style="">
</span></p>
<p class="MsoNormal" style="margin-left:18.0pt"><span style="">Use <span class="SpellE">
xsl<span class="GramE">:for</span>-each</span> to loop all child nodes. &lt;<span class="SpellE">xsl<span class="GramE">:value</span>-of</span> select=&quot;.&quot;&gt;
<span class="GramE">means</span> to select the node's content. </span></p>
<p class="MsoNormal" style="margin-left:18.0pt"><span style="">&amp;#<span class="SpellE">xD</span><span class="GramE">;&amp;</span>#<span class="SpellE">xa</span>; refers to the new line(\r\n). At end of row, we should start a new line with a new book.
</span></p>
<h2>More Information</h2>
<p class="MsoNormal" style="margin-top:0cm; margin-right:0cm; margin-bottom:0cm; margin-left:18.0pt; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:新宋体">XslCompiledTransform document. </span></p>
<p class="MsoNormal" style="margin-top:0cm; margin-right:0cm; margin-bottom:0cm; margin-left:18.0pt; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:新宋体"><a href="http://msdn.microsoft.com/en-us/library/system.xml.xsl.xslcompiledtransform.aspx">http://msdn.microsoft.com/en-us/library/system.xml.xsl.xslcompiledtransform.aspx</a>
</span></p>
<p class="MsoNormal" style="margin-top:0cm; margin-right:0cm; margin-bottom:0cm; margin-left:18.0pt; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:新宋体">XSLT Reference. </span></p>
<p class="MsoNormal" style="margin-top:0cm; margin-right:0cm; margin-bottom:0cm; margin-left:18.0pt; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-family:新宋体"><a href="http://msdn.microsoft.com/en-us/library/ms256069.aspx">http://msdn.microsoft.com/en-us/library/ms256069.aspx</a>
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
