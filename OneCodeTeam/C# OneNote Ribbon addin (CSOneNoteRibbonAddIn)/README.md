# C# OneNote Ribbon addin (CSOneNoteRibbonAddIn)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Office
## Topics
- VSTO
- Ribbon
- OneNote
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>APPLICATION : CSOneNoteRibbonAddIn Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary: </h3>
<p style="font-family:Courier New"><br>
The code sample demonstrates a OneNote 2010 COM add-in that implements<br>
IDTExtensibility2. <br>
The add-in also supports customizing the Ribbon by implementing the<br>
IRibbonExtensibility interface.<br>
In addition, the sample also demonstrates the usage of the<br>
OneNote 2010 Object Model.<br>
<br>
CSOneNoteRibbonAddIn: The project which generates CSOneNoteRibbonAddIn.dll<br>
for project CSOneNoteRibbonAddInSetup.<br>
<br>
CSOneNoteRibbonAddInSetup: Setup project which generates setup.exe and<br>
CSOneNoteRibbonAddInSetup.msi for OneNote 2010.<br>
</p>
<h3>Prerequisite:</h3>
<p style="font-family:Courier New"><br>
You must run this code sample on a computer that has Microsoft OneNote 2010 <br>
installed.<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
The following steps walk through a demonstration of the CSOneNoteRibbonAddIn<br>
sample.<br>
<br>
Step1. Open the solution file CSOneNoteRibbonAddIn.sln as Administrator;<br>
<br>
Step2. Build CSOneNoteRibbonAddIn first, then build setup project<br>
CSOneNoteRibbonAddInSetup in Visual Studio 2010, then you will get a <br>
bootstrapper setup.exe and the application CSOneNoteRibbonAddInSetup.msi;<br>
<br>
Step3.Install setup.exe;<br>
<br>
Step4. Open OneNote 2010 and you will see three MesseageBoxs:<br>
MessageBox.Show(&quot;CSOneNoteRibbonAddIn OnConnection&quot;),<br>
MessageBox.Show(&quot;CSOneNoteRibbonAddIn OnAddInsUpdate&quot;),<br>
MessageBox.Show(&quot;CSOneNoteRibbonAddIn OnStartupComplete&quot;);<br>
<br>
Step5. Click Review Tab and you will see Statistics group which contains a<br>
button ShowForm that the add-in added to the Ribbon. Click the ShowForm <br>
button, a window form will pop up and you can click the button on the form<br>
to get the title of the current page;<br>
<br>
Step6. When closing OneNote 2010, you will see two MessageBoxs:<br>
MessageBox.Show(&quot;CSOneNoteRibbonAddIn OnBeginShutdown&quot;),<br>
MessageBox.Show(&quot;CSOneNoteRibbonAddIn OnDisconnection&quot;)<br>
</p>
<h3>Creation:</h3>
<p style="font-family:Courier New"><br>
Step1. Create a Shared Add-in Extensibility,and the shared Add-in Wizard is <br>
as follows:<br>
&nbsp;&nbsp;&nbsp;&nbsp;Open the Visual Studio 2010 as Administrator;<br>
&nbsp;&nbsp;&nbsp;&nbsp;Create an Shared Add-in (Other Project Types-&gt;Extensibility)
<br>
&nbsp;&nbsp;&nbsp;&nbsp;using Visual C#; <br>
&nbsp;&nbsp;&nbsp;&nbsp;choose Microsoft Access&nbsp;&nbsp;&nbsp;&nbsp;(since there doesn't exist Microsoft OneNote<br>
&nbsp;&nbsp;&nbsp;&nbsp;option to choose, you can choose Microsoft Access first, but remeber
<br>
&nbsp;&nbsp;&nbsp;&nbsp;to modify Setup project registry HKCU to be OneNote);<br>
&nbsp;&nbsp;&nbsp;&nbsp;fill name and description of the Add-in;<br>
&nbsp;&nbsp;&nbsp;&nbsp;select the two checkboxes in Choose Add-in Options.<br>
<br>
Step2. Modify the CSOneNoteRibbonAddInSetup Registry <br>
(right click Project-&gt;View-&gt;Registry) <br>
[HKEY_CLASSES_ROOT\AppID\{Your GUID}]<br>
&quot;DllSurrogate&quot;=&quot;&quot;<br>
[HKEY_CLASSES_ROOT\CLSID\{Your GUID}]<br>
&quot;AppID&quot;=&quot;{Your GUID}&quot;<br>
<br>
[HKEY_CURRENT_USER\Software\Microsoft\Office\OneNote\AddIns\<br>
CSOneNoteRibbonAddIn.Connect]<br>
&quot;LoadBehavior&quot;=dword:00000003<br>
&quot;FriendlyName&quot;=&quot;CSOneNoteRibbionAddIn&quot;<br>
&quot;Description&quot;=&quot;OneNote2010 Ribbon AddIn Sample&quot;<br>
<br>
[HKEY_LOCAL_MACHINE\SOFTWARE\Classes\AppID\{Your GUID}]<br>
&quot;DllSurrogate&quot;=&quot;&quot;<br>
[HKEY_LOCAL_MACHINE\SOFTWARE\Classes\CLSID\{Your GUID}]<br>
&quot;AppID&quot;=&quot;{Your GUID}&quot;<br>
<br>
Step3. Add customUI.xml and showform.png resource files into<br>
CSOneNoteRibbonAddIn project.<br>
<br>
Step4. Make Connect class inherent IRibbonExtensibility and implement the method<br>
GetCustomUI.<br>
&nbsp; &nbsp; &nbsp; <br>
&nbsp;&nbsp;&nbsp;&nbsp;/// &lt;summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &nbsp; &nbsp; Loads the XML markup from an XML customization file
<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &nbsp; &nbsp; that customizes the Ribbon user interface.<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;/summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;param name=&quot;RibbonID&quot;&gt;The ID for the RibbonX UI&lt;/param&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;returns&gt;string&lt;/returns&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;public string GetCustomUI(string RibbonID)<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;return Properties.Resources.customUI;<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
Step5. Implement the methods OnGetImage and ShowForm according to the customUI.xml<br>
content.<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &nbsp; &nbsp; Implements the OnGetImage method in customUI.xml<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;/summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;param name=&quot;imageName&quot;&gt;the image name in customUI.xml&lt;/param&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;returns&gt;memory stream contains image&lt;/returns&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;public IStream OnGetImage(string imageName)<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;MemoryStream stream = new MemoryStream();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (imageName == &quot;showform.png&quot;)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Resources.ShowForm.Save(stream, ImageFormat.Png);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;return new ReadOnlyIStreamWrapper(stream);<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &nbsp; &nbsp; show Windows Form method<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;/summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;param name=&quot;control&quot;&gt;Represents the object passed into every<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// Ribbon user interface (UI) control's callback procedure.&lt;/param&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;public void ShowForm(IRibbonControl control)<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;OneNote.Window context = control.Context as OneNote.Window;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;CWin32WindowWrapper owner =<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;new CWin32WindowWrapper((IntPtr)context.WindowHandle);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;TestForm form = new TestForm(applicationObject as OneNote.Application);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;form.ShowDialog(owner);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;form.Dispose();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;form = null;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;context = null;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;owner = null; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;GC.Collect();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;GC.WaitForPendingFinalizers();<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;GC.Collect();<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
Step6. Add ReadOnlyIStreamWrapper class and CWin32WindowWrapper class into<br>
CSOneNoteRibbonAddIn project and add Windows Form for testing to open.<br>
<br>
Step7. Add the follwing methods in the TestForm which using OneNote 2010 Object Model<br>
to show the title of the current page:<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;/// &lt;summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// Get the title of the page<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;/summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;returns&gt;string&lt;/returns&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;private string GetPageTitle()<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;string pageXmlOut = GetActivePageContent(); &nbsp; &nbsp; &nbsp; &nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;var doc = XDocument.Parse(pageXmlOut);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;string pageTitle = &quot;&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;pageTitle = doc.Descendants().FirstOrDefault().Attribute(&quot;ID&quot;).NextAttribute.Value;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;return pageTitle;<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// Get active page content and output the xml string<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;/summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;returns&gt;string&lt;/returns&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;private string GetActivePageContent()<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;string activeObjectID = this.GetActiveObjectID(_ObjectType.Page);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;string pageXmlOut = &quot;&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;oneNoteApp.GetPageContent(activeObjectID,out pageXmlOut);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;return pageXmlOut;<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// Get ID of current page <br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;/summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;param name=&quot;obj&quot;&gt;_Object Type&lt;/param&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;returns&gt;current page Id&lt;/returns&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;private string GetActiveObjectID(_ObjectType obj)<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;string currentPageId = &quot;&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;uint count = oneNoteApp.Windows.Count;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;foreach (OneNote.Window window in oneNoteApp.Windows)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (window.Active)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;switch (obj)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;case _ObjectType.Notebook:<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;currentPageId = window.CurrentNotebookId;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;break;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;case _ObjectType.Section:<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;currentPageId = window.CurrentSectionId;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;break;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;case _ObjectType.SectionGroup:<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;currentPageId = window.CurrentSectionGroupId;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;break;
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;currentPageId = window.CurrentPageId;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;return currentPageId;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// Nested types<br>
&nbsp; &nbsp; &nbsp; &nbsp;/// &lt;/summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;private enum _ObjectType<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Notebook,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Section,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;SectionGroup,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Page,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;SelectedPages,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;PageObject<br>
&nbsp; &nbsp; &nbsp; &nbsp;} <br>
<br>
Step8. Register the output assembly as COM component.<br>
<br>
To do this, click Project-&gt;Project Properties... button. And in the project<br>
properties page, navigate to Build tab and check the box &quot;Register for COM<br>
interop&quot;.<br>
<br>
Step8. Build your CSOneNoteRibbonAddIn project, <br>
and then build CSOneNoteRibbonAddInSetup project to generate setup.exe and<br>
CSOneNoteRibbonAddInSetup.msi.<br>
<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
MSDN: Creating OneNote 2010 Extensions with the OneNote Object Model<br>
<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/magazine/ff796230.aspx">http://msdn.microsoft.com/en-us/magazine/ff796230.aspx</a><br>
<br>
Jeff Cardon's Blog<br>
<a target="_blank" href="http://blogs.msdn.com/b/onenotetips/">http://blogs.msdn.com/b/onenotetips/</a><br>
<br>
</p>
<h3></h3>
<p style="font-family:Courier New"></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
