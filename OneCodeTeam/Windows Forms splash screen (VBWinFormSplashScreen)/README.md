# Windows Forms splash screen (VBWinFormSplashScreen)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Windows Forms
## Topics
- Controls
- Splash Screen
## Updated
- 07/22/2012
## Description

<h1><span style="font-family:新宋体">WINDOWS FORMS APPLICATION</span> (<span style="font-family:新宋体">VBWinFormSplashScreen</span>)</h1>
<h2>Introduction</h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">The Splash Screen sample demonstrates how to achieve splash screen effect in Windows Forms application.</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">
</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">The Splash Screen sample demonstrates how to achieve splash screen effect in Windows Forms application.</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">
</span></h2>
<p class="MsoNormal"><span style="">A splash screen is an image that appears while a game or program is loading. It may also be used to describe an introduction page on a website. Splash screens cover the entire screen or simply a rectangle near the center
 of the screen. The splash screens of operating systems and some applications that expect to be run full-screen usually cover the entire screen.
</span></p>
<p class="MsoNormal"><span lang="EN" style="">Splash screens are typically used by particularly large applications to notify the user that the program is in the process of loading. They provide feedback that a lengthy process is underway. Occasionally, a
 progress bar within the splash screen indicates the loading progress. A splash screen disappears when the application's main window appears.
</span></p>
<p class="MsoNormal"><span lang="EN" style="">Splash screens typically serve to enhance the look and feel of an application or web
<span class="GramE">site,</span> hence they are often visually appealing. They may also have animations, graphics, and sound</span><span style="">
</span></p>
<h2>Running the Sample</h2>
<p class="MsoListParagraph" style="text-indent:5.0pt"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">The Splash Screen fades in and then fades out.
</span></p>
<p class="MsoNormal"><span style=""><img src="61551-image.png" alt="" width="466" height="106" align="middle">
</span><span style=""></span></p>
<p class="MsoListParagraph" style="text-indent:5.0pt"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">The main form shows out. </span></p>
<p class="MsoNormal"><span style=""><img src="61552-image.png" alt="" width="720" height="604" align="middle">
</span><span style=""></span></p>
<h2>Using the Code</h2>
<p class="MsoNormal"><span style="">&nbsp;&nbsp; </span>1. Create a new form named &quot;SplashScreen1&quot;</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp; </span>2. Write a method named &quot;Splash1Setting&quot;, in the method, set the &quot;FormBorderStyle&quot;, &quot;BackgroundImage&quot; and &quot;StartPosition&quot; properties as follows:<span style="">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
   Private Sub Splash1Setting()
       Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
       Me.StartPosition = FormStartPosition.CenterScreen
       Me.BackgroundImage = VBWinFormSplashScreen.My.Resources.SplashImage
   End Sub

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style="">&nbsp;&nbsp;&nbsp; </span></p>
<p class="MsoNormal"><span style="">&nbsp;</span><span style="">&nbsp;&nbsp;&nbsp;&nbsp;
</span>In the New() method, call the Splash1Setting() method.<span style=""> </span>
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
Public Sub New()
    MyBase.New()
    InitializeComponent()


    Splash1Setting()
End Sub

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp; </span>3. Right-click the project and choose the &quot;Properties&quot; option, in the &quot;application<span class="GramE">&quot;<span style="">&nbsp;
</span>tab</span>, select the &quot;Splash Screen&quot; as &quot;SplashScreen1&quot;.</p>
<p class="MsoNormal"><span style="">&nbsp;&nbsp; </span>4. Built the project.<span style="">
</span></p>
<h2>More Information<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">
</span></h2>
<h2><span class="SpellE"><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">WindowsFormsApplicationBase.SplashScreen</span></span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">
 Property. </span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal"><a href="http://msdn.microsoft.com/en-us/library/microsoft.visualbasic.applicationservices.windowsformsapplicationbase.splashscreen.aspx">http://msdn.microsoft.com/en-us/library/microsoft.visualbasic.applicationservices.windowsformsapplicationbase.splashscreen.aspx</a>
</span></h2>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
