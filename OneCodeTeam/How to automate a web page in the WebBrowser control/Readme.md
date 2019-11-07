# How to automate a web page in the WebBrowser control
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Internet Explorer
- Internet Explorer Development
## Topics
- Automation
- WebBrowser
## Updated
- 10/15/2013
## Description

<h1>How to Automate HTML Elements Loaded in <span class="SpellE">WebBrowser</span> Control (<span class="SpellE">CSWebBrowserAutomation</span>)</h1>
<h2>Introduction</h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">The sample demonstrates how to create a
<span class="SpellE">WebBrowser</span> which supplies following features </span>
</h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">1. Manipulate the html elements and login a website automatically.
</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">2. Block specified sites.
</span></h2>
<h2>Running the Sample</h2>
<p class="MsoNormal">Step1. Open the project in VS2012, replace the stored <span class="SpellE">
UserName</span> and Password in <span class="SpellE">StoredSites</span>\www.codeplex.com.xml with your username and password for http://www.codeplex.com first.
</p>
<p class="MsoNormal">Step2. Build this project in VS2012. </p>
<p class="MsoNormal">Step3. Run CSWebBrowserAutomation.exe. The button &quot;Auto Complete&quot; is disabled by default.
</p>
<p class="MsoNormal"><span style=""><img src="97989-image.png" alt="" width="576" height="297" align="middle">
</span></p>
<p class="MsoNormal">Step4. Type <a href="https://www.codeplex.com/site/login?RedirectUrl=https%3a%2f%2fwww.codeplex.com%2fsite%2fusers%2fupdate">
https://www.codeplex.com/site/login?RedirectUrl=https%3a%2f%2fwww.codeplex.com%2fsite%2fusers%2fupdate</a> in the
<span class="SpellE"><span class="GramE">Url</span></span> textbox and press GO.
</p>
<p class="MsoNormal">This <span class="SpellE"><span class="GramE">url</span></span> is the login page of www.codeplex.com. The
<span class="SpellE">RedirectUrl</span> means that the page will be redirected to the
<span class="SpellE"><span class="GramE">url</span></span> if you login the site successfully.
</p>
<p class="MsoNormal"><span style=""><img src="97990-image.png" alt="" width="576" height="297" align="middle">
</span></p>
<p class="MsoNormal">Step5. After the web page is loaded completed, the button &quot;Auto Complete&quot; is enabled.
<span class="SpellE">Clickthe</span> button and the web page will be <span class="SpellE">
be</span> redirected to https://www.codeplex.com/site/users/update. </p>
<p class="MsoNormal"><span style=""><img src="97991-image.png" alt="" width="576" height="443" align="middle">
</span></p>
<p class="MsoNormal">Step6. After the new web page is loaded, click the button &quot;Auto Complete&quot; again, and the &quot;New email address&quot; field in the web page will be filled.
</p>
<p class="MsoNormal"><span style=""><img src="97992-image.png" alt="" width="575" height="448" align="middle">
</span></p>
<p class="MsoNormal">Step7. Type http://<span class="GramE">www.contoso.com<span style="">&nbsp;
</span>in</span> the <span class="SpellE">urltext</span> box and press Go. You will view
<span class="GramE">a page that show</span> you a message &quot;Sorry, this site is blocked.&quot;</p>
<p class="MsoNormal"><span style=""><img src="97993-image.png" alt="" width="576" height="276" align="middle">
</span></p>
<h2>Using the Code</h2>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
1. Design a class <span class="SpellE">XMLSerialization</span> that can serialize an object to an XML file or
<span class="SpellE">deserialize</span> an XML file to an object. </p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
2. Design classes <span class="SpellE">HtmlCheckBox</span>, <span class="SpellE">
HtmlPassword</span>, <span class="SpellE">HtmlSubmit</span> and <span class="SpellE">
HtmlText</span> that represent the checkbox, password text box, submit button and normal text box. All the classes inherit
<span class="SpellE">HtmlInputElement</span> that represents an <span class="SpellE">
HtmlElement</span> with the tag &quot;input&quot;. The class <span class="SpellE">
HtmlInputElementFactory</span> is used to get an <span class="SpellE">HtmlInputElement</span> from an
<span class="SpellE">HtmlElement</span> in the web page. </p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
3. Design a class <span class="SpellE">StoredSite</span> that represents a site whose html elements are stored. A site is stored as an XML file under
<span class="SpellE">StoredSites</span> folder, and can be <span class="SpellE">
deserialized</span>. </p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">&nbsp;&nbsp; </span>This class also supplies a method <span class="SpellE">
FillWebPage</span> to complete the web page automatically. If a submit button could be found, then the button will also be clicked automatically.
</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
4. Design a class <span class="SpellE">BlockSites</span> which contains that blocked sites list. The file \Resource\BlockList.xml can be
<span class="SpellE">deserialized</span> to a <span class="SpellE">BlockSites</span> instance.
</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
5. Design a class <span class="SpellE">WebBrowserEx</span> that inherits class <span class="SpellE">
System.Windows.Forms.WebBrowser</span>. Override the <span class="SpellE">OnNavigating</span> method to check whether the
<span class="SpellE"><span class="GramE">url</span></span> is included in the block list. If so, navigate the build-in Block.htm.
</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">&nbsp;&nbsp; </span>Override the <span class="SpellE">OnDocumentCompleted</span> method to check whether the loaded page could be completed automatically. If the site and
<span class="SpellE"><span class="GramE">url</span></span> are stored, then the method AutoComplete can be used.</p>
<p class="MsoNormal" style=""><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/en-us/library/system.xml.serialization.xmlserializer.aspx"><span class="SpellE">XmlSerializer</span> Class</a>
</span></p>
<p class="MsoNormal" style=""><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/en-us/library/system.windows.forms.webbrowser.aspx"><span class="SpellE">WebBrowser</span> Class</a>
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
