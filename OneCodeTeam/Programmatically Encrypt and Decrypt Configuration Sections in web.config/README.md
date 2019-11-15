# Programmatically Encrypt and Decrypt Configuration Sections in web.config
## Requires
- Visual Studio 2013
## License
- Apache License, Version 2.0
## Technologies
- ASP.NET
- .NET
- Web App Development
## Topics
- Security
## Updated
- 09/22/2016
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode"><img src="https://aka.ms/onecodesampletopbanner1" alt=""></a><strong>&nbsp;</strong><em>&nbsp;</em></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:24pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><a name="_GoBack"></a><span style="font-weight:bold; font-size:14pt">How to u</span><span style="font-weight:bold; font-size:14pt">se RSA encryption algorithm API to encrypt and decrypt
</span><span style="font-weight:bold; font-size:14pt">c</span><span style="font-weight:bold; font-size:14pt">onfiguration section.</span><span style="font-weight:bold; font-size:14pt"> (</span><span style="font-weight:bold; font-size:14pt">CS\VBASPNETEncryptAndDecryptConfiguration</span><span style="font-weight:bold; font-size:14pt">)</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Introduction</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">This sample shows how to use RSA encryption algorithm API to encrypt and decrypt configuration section</span><span style="font-size:11pt">s</span><span style="font-size:11pt"> in order to protect sensitive
 information from interception or</span><span style="font-size:11pt"> being</span><span style="font-size:11pt"> hijack</span><span style="font-size:11pt">ed</span><span style="font-size:11pt"> in ASP.NET web application</span><span style="font-size:11pt">s</span><span style="font-size:11pt">.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Building the Sample</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>&nbsp;</span><span style="font-size:11pt">If your application hasn't web.config, please create one. And also specify some section</span><span style="font-size:11pt">s</span><span style="font-size:11pt"> such as appSetting,
 connectSetting in this web.config.</span><span style="font-size:11pt"> <br>
How to create Web.config in application: </span><span style="font-size:11pt"><br>
</span><a href="http://support.microsoft.com/kb/815179" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://support.microsoft.com/kb/815179</span></a><span style="font-size:11pt">
<br>
Working with Web.config Files: </span><span style="font-size:11pt"><br>
</span><a href="http://msdn.microsoft.com/en-us/library/ms460914.aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/ms460914.aspx</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Running the Sample</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:13pt"><span style="font-size:11pt">Step 1: Open the CSASPNETEncryptAndDecryptConfiguration.sln.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:13pt"><span style="font-size:11pt">&nbsp;</span><span style="font-size:11pt">Step 2: Press Ctrl&#43;F5</span><span style="font-size:11pt">.
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:13pt"><span style="font-size:11pt">&nbsp;</span><span style="font-size:11pt">Step 3: Choose a configuration section in
</span><span style="font-size:11pt">the </span><span style="font-size:11pt">dropdown list.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:13pt"><span style="font-size:11pt">&nbsp;</span><span style="font-size:11pt">Step</span><span style="font-size:11pt"> 4: Click</span><span style="font-size:11pt"> the</span><span style="font-size:11pt"> &quot;encrypt it&quot; button
</span><span style="font-size:11pt">as </span><span style="font-size:11pt">shown</span><span style="font-size:11pt"> below. If the encryption
</span><span style="font-size:11pt">is </span><span style="font-size:11pt">success</span><span style="font-size:11pt">ful</span><span style="font-size:11pt">, then open
</span><span style="font-size:11pt">the web.config file.</span><span style="font-size:11pt">
</span><span style="font-size:11pt">Y</span><span style="font-size:11pt">ou will observe</span><span style="font-size:11pt"> that</span><span style="font-size:11pt"> the specific section is encrypted and is replaced by some RSA data section.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt"><img src="119264-image.png" alt="" width="351" height="118" align="middle">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:13pt"><span style="font-size:11pt">Step 5: If you want to recover this section to plain text</span></span><span style="font-size:13pt"><span style="font-size:11pt">,</span><span style="font-size:11pt">
</span><span style="font-size:11pt">c</span><span style="font-size:11pt">lick</span><span style="font-size:11pt"> the</span><span style="font-size:11pt"> &quot;decrypt it&rdquo; button and check web.config again.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt"><img src="119265-image.png" alt="" width="399" height="146" align="middle">
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-weight:bold">Note</span><span>: If you are running this application
</span><span>in</span><span> the file system, </span><span>Visual Studio will display a dialog with the message &quot;The file has been modified outside the editor. Do you want to reload it?&quot; when you close the application.</span><span> Click yes and then view the
 web.config.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">Using the Code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">1. Get the </span><a name="OLE_LINK1"></a><a name="OLE_LINK2"></a><span style="font-size:11pt">dropdown list selected value
</span><span style="font-size:11pt">to assign which configuration section to encrypt or decrypt.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>&nbsp;</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>HTML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">html</span>

<pre class="html" id="codePreview">&lt;html&gt;
&lt;head&gt;
    &lt;meta name=&quot;viewport&quot; content=&quot;width=device-width&quot; /&gt;
    &lt;title&gt;Index&lt;/title&gt;
    &lt;script src=&quot;~/Scripts/jquery-2.1.1.min.js&quot;&gt;&lt;/script&gt;
    &lt;script type=&quot;text/javascript&quot;&gt;
    $(document).ready(function () {
        $(&quot;#SectionNames&quot;).append($('&lt;option&gt;', {
            value: &quot;connectionStrings&quot;,
            text: &quot;Connection Strings&quot;
            })
        );
        $(&quot;#SectionNames&quot;).append($('&lt;option&gt;', {
            value: &quot;appSettings&quot;,
            text: &quot;Application Settings&quot;
        })
        );
        $(&quot;#SectionNames&quot;).append($('&lt;option&gt;', {
            value: &quot;system.web/machineKey&quot;,
            text: &quot;Machine Key&quot;
        })
        );
        $(&quot;#SectionNames&quot;).append($('&lt;option&gt;', {
            value: &quot;system.web/sessionState&quot;,
            text: &quot;Session State&quot;
        })
        );
    });
    var EncryptConfig = function () {
        var url = &quot;/Home/EncryptConfig&quot;;
        var sectionName = $(&quot;#SectionNames option:selected&quot;).text();
        $.ajax({
            url: url,
            type: &quot;POST&quot;,
            data: &quot;sectionName=&quot; &#43; $(&quot;#SectionNames option:selected&quot;).val(),
            success:function()
            {
                $(&quot;#lbresult&quot;).text(&quot;encrypt successed, please check the configuration file.&quot;);
            },
            error: function () {
                $(&quot;#lbresult&quot;).text(&quot;encrypt failed.&quot;);
            }
        });
    }
    var DecryptConfig = function () {
        var url = &quot;/Home/DecryptConfig&quot;;
        var sectionName = $(&quot;#SectionNames option:selected&quot;).text();
        $.ajax({
            url: url,
            type: &quot;POST&quot;,
            data: &quot;sectionName=&quot; &#43; $(&quot;#SectionNames option:selected&quot;).val(),
            success: function () {
                $(&quot;#lbresult&quot;).text(&quot;decrypt successed, please check the configuration file.&quot;);
            },
            error: function () {
                $(&quot;#lbresult&quot;).text(&quot;decrypt failed.&quot;);
            }
        });
    }
    &lt;/script&gt;
&lt;/head&gt;
&lt;body&gt;
    <div> 
        <p>Choose a section:</p>
        &lt;select id=&quot;SectionNames&quot;&gt;&lt;/select&gt;
    </div>
    @*<br><br><br><br>*@
    <div>
        &lt;button id=&quot;btnEncrypt&quot; onclick=&quot;EncryptConfig()&quot;&gt;Encrypt it&lt;/button&gt;
        &lt;button id=&quot;btnDecrypt&quot; onclick=&quot;DecryptConfig()&quot;&gt;Decrypt it&lt;/button&gt;
    </div>
   <p id="lbresult">&nbsp;</p>
&lt;/body&gt;
&lt;/html&gt;
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">2. Open the web.config in this web application.
</span><span style="font-size:11pt"><br>
</span><span style="font-size:11pt">3. Find the specific section and use RSAProtectedConfigurationProvider to encrypt or decrypt it.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span><span class="hidden">vb</span>


<pre class="csharp" id="codePreview">[HttpPost]
public ActionResult EncryptConfig(string sectionName)
{
    if (string.IsNullOrEmpty(sectionName))
    {
        return null;
    }
    Configuration config = WebConfigurationManager.OpenWebConfiguration(Request.ApplicationPath);
    ConfigurationSection section = config.GetSection(sectionName);
    section.SectionInformation.ProtectSection(provider);
    config.Save();
    return Content(&quot;Success&quot;);
}
</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span style="font-size:11pt">4. If</span><span style="font-size:11pt"> this sample runs</span><span style="font-size:11pt"> success</span><span style="font-size:11pt">ful</span><span style="font-size:11pt">, this section will be
 encrypted by RSA and replaced by some RSA section in web.config.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:0pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:13pt"><span style="font-weight:bold; font-size:13pt">More Information</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt"><span>RSACryptoServiceProvider</span><span style="font-size:11pt">
<br>
</span><a href="http://msdn.microsoft.com/en-us/library/system.security.cryptography.rsacryptoserviceprovider(VS.80).aspx" style="text-decoration:none"><span style="color:#0000ff; text-decoration:underline">http://msdn.microsoft.com/en-us/library/system.security.cryptography.rsacryptoserviceprovider(VS.80).aspx</span></a><span style="font-size:11pt">
<br>
</span><span>CspParameters</span><span style="font-size:11pt"> <br>
</span><a href="http://msdn.microsoft.com/en-us/library/system.security.cryptography.cspparameters(VS.80).aspx" style="text-decoration:none"><span style="color:#0000ff; text-decoration:underline">http://msdn.microsoft.com/en-us/library/system.security.cryptography.cspparameters(VS.80).aspx</span></a><span style="font-size:11pt">
<br>
</span><span>ConfigurationSection</span><span style="font-size:11pt"> <br>
</span><a href="http://msdn.microsoft.com/en-us/library/system.configuration.configurationsection.aspx" style="text-decoration:none"><span style="color:#0000ff; text-decoration:underline">http://msdn.microsoft.com/en-us/library/system.configuration.configurationsection.aspx</span></a><span style="font-size:11pt">
<br>
</span><span>SectionInformation.ProtectSection </span><span style="font-size:11pt"><br>
</span><a href="http://msdn.microsoft.com/en-us/library/system.configuration.sectioninformation.protectsection.aspx" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">http://msdn.microsoft.com/en-us/library/system.configuration.sectioninformation.protectsection.aspx</span></a><span style="font-size:11pt">
<br>
</span><span style="font-size:11pt"><br>
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:10pt; font-size:10.0pt; line-height:27.6pt; direction:ltr; unicode-bidi:normal">
<span style="font-size:11pt">&nbsp;</span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
