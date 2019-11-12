# Extended WebBrowser control supporting tabs
## Requires
- Visual Studio 2012
## License
- Apache License, Version 2.0
## Technologies
- Internet Explorer
- Internet Explorer Development
## Topics
- WebBrowser
## Updated
- 07/28/2013
## Description

<h1>How to Create a Tabbed <span class="SpellE">WebBrowser</span> (<span class="SpellE">CSTabbedWebBrowser</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The sample demonstrates how to create a tabbed <span class="SpellE">
WebBrowser</span>. </p>
<p class="MsoNormal">The &quot;Open in new Tab&quot; context command is disabled in
<span class="SpellE">WebBorwser</span> by default, you can add a value *.exe=1 (* means the process name) to the key HKCU\Software\Microsoft\Internet Explorer\Main\<span class="SpellE">FeatureControl</span>\FEATURE_TABBED_BROWSING.
</p>
<p class="MsoNormal">This menu will only take effect after the application is restarted. See http://msdn.microsoft.com/en-us/library/<span class="GramE">ms537636(</span>VS.85).aspx
</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">Press F5 to run the sample, the following is the result.</p>
<p class="MsoNormal">Step1. Type http://1code.codeplex.com/ in the <span class="SpellE">
<span class="GramE">Url</span></span>, and press Enter. </p>
<p class="MsoNormal"><span style=""><img src="93202-image.png" alt="" width="576" height="370" align="middle">
</span></p>
<p class="MsoNormal">Step2. Right click the &quot;Downloads&quot; in the header of the page, and then click &quot;Open in new tab&quot;. This application will open the link in a new tab.
</p>
<p class="MsoNormal"><span style=""><img src="93203-image.png" alt="" width="933" height="436" align="middle">
</span></p>
<p class="MsoNormal">If the &quot;Open in new tab&quot; is disabled, check &quot;Enable Tab&quot; and restart the application.
</p>
<p class="MsoNormal"><span style=""><img src="93204-image.png" alt="" width="931" height="414" align="middle">
</span></p>
<h2>Using the Code</h2>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
1. Design a class <span class="SpellE">WebBrowserEx</span> that inherits <span class="GramE">
class <span style="">&nbsp;</span><span class="SpellE"><a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Windows.Forms.WebBrowser.aspx" target="_blank" title="Auto generated link to System.Windows.Forms.WebBrowser">System.Windows.Forms.WebBrowser</a></span></span>. This class can handle NewWindow3 event.
</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">&nbsp;&nbsp; </span>The interface DWebBrowserEvents2 designates an event sink interface that an application must implement to receive event notifications from a
<span class="SpellE">WebBrowser</span> control or from the Windows Internet Explorer application. The event notifications include NewWindow3 event that will be used in this application.
</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
2. Design a class <span class="SpellE">WebBrowserTabPage</span> that inherits the
<span class="SpellE">the</span> <span class="SpellE"><a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Windows.Forms.TabPage.aspx" target="_blank" title="Auto generated link to System.Windows.Forms.TabPage">System.Windows.Forms.TabPage</a></span> class and contains a
<span class="SpellE">WebBrowserEx</span> property. An instance of this class could be
<span class="GramE">add</span> to a tab control directly. </p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span></p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
3. Create a <span class="SpellE">UserControl</span> that contains a <span class="SpellE">
<a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Windows.Forms.TabControl.aspx" target="_blank" title="Auto generated link to System.Windows.Forms.TabControl">System.Windows.Forms.TabControl</a></span> instance. This <span class="SpellE">UserControl</span> supplies the method to create/close the
<span class="SpellE">WebBrowserTabPage</span> in the <span class="SpellE">TabControl</span>. It also supplies a Property
<span class="SpellE">IsTabEnabled</span> to get or set whether the &quot;Open in new Tab&quot; context menu in
<span class="SpellE">WebBrowser</span> is enabled. </p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
4. In the <span class="SpellE">MainForm</span>, it supplies controls to enable/disable tab, create/close the tab, and make the
<span class="SpellE">WebBrowser</span> <span class="SpellE">GoBack</span>, <span class="SpellE">
GoForward</span> or Refresh.</p>
<p class="MsoNormal" style="margin-bottom:0in; margin-bottom:.0001pt; line-height:normal; text-autospace:none">
</p>
<p class="MsoNormal" style=""><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/en-us/library/aa768283(VS.85).aspx">DWebBrowserEvents2 interface</a>
</span></p>
<p class="MsoNormal" style=""><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/en-us/library/ms537636(VS.85).aspx">Tabbed Browsing for Developers</a>
</span></p>
<p class="MsoNormal" style=""><span class="MsoHyperlink"><a href="http://msdn.microsoft.com/en-us/library/system.windows.forms.webbrowser.createsink.aspx"><span class="SpellE">WebBrowser.CreateSink</span> Method</a>
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
