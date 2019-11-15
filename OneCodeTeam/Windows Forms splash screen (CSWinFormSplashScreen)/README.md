# Windows Forms splash screen (CSWinFormSplashScreen)
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
- 03/04/2012
## Description

<h1><span style="font-family:������">WINDOWS FORMS APPLICATION</span> (<span style="font-family:������">CSWinFormSplashScreen</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">The Splash Screen sample demonstrates how to achieve splash screen effect in Windows Forms application.<span style="">
</span></p>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">Solution 1: Customized SplashScreen with &quot;fade in&quot; and &quot;fade out&quot; effect.
</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">Solution 2: Using VB.NET Framework without &quot;fade in&quot; and &quot;fade out&quot; effect.</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">
</span></h2>
<h2>Running the Sample</h2>
<p class="MsoListParagraph" style=""><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">The Splash Screen fades in and then fades out.
</span></p>
<p class="MsoNormal"><span style=""><img src="53736-image.png" alt="" width="233" height="319" align="middle">
</span><span style=""></span></p>
<p class="MsoListParagraph" style=""><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">The main form shows out. </span></p>
<p class="MsoNormal"><span style=""><img src="53737-image.png" alt="" width="576" height="483" align="middle">
</span><span style=""></span></p>
<h2>Using the Code</h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">Solution 1: Customized SplashScreen with &quot;fade in&quot; and &quot;fade out&quot; effect.
</span></h2>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
        private void ExtraFormSettings()
      {
          this.FormBorderStyle = FormBorderStyle.None;
          this.Opacity = 0.5;
          this.BackgroundImage = CSWinFormSplashScreen.Properties.Resources.SplashImage;
      }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
        void t_Tick(object sender, EventArgs e)
       {
           // Fade in by increasing the opacity of the splash to 1.0
           if (fadeIn)
           {
               if (this.Opacity &lt; 1.0)
               {
                   this.Opacity &#43;= 0.02;
               }
               // After fadeIn complete, begin fadeOut
               else
               {
                   fadeIn = false;
                   fadeOut = true;
               }
           }
           else if (fadeOut) // Fade out by increasing the opacity of the splash to 1.0
           {
               if (this.Opacity &gt; 0)
               {
                   this.Opacity -= 0.02;
               }
               else
               {
                   fadeOut = false;
               }
           }


           // After fadeIn and fadeOut complete, stop the timer and close this splash.
           if (!(fadeIn || fadeOut))
           {
               t.Stop();
               this.Close();
           }
       }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal"><span style="">&nbsp;&nbsp;
</span><span style="">&nbsp;</span>4. Start the main form.<span style="">&nbsp;&nbsp;
</span></span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">Solution 2: Using VB.NET Framework without &quot;fade in&quot; and &quot;fade out&quot; effect.
</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal"><span style="">&nbsp;&nbsp;
</span>1. Add reference to &quot;Microsoft.VisualBasic&quot;. </span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal"><span style="">&nbsp;&nbsp;
</span>2. Create a class inheriting from WindowsFormsApplicationBase.</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">
</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal"><span style="">&nbsp;&nbsp;
</span>3. Override OnCreateSplashScreen method, and assign the SplashScreen property to an instance of the Splash form instance.
</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal"><span style="">&nbsp;&nbsp;
</span>4. Override OnCreateMainForm method, and assign the MainForm property to an instance of the main form instance.(We can keep the Splash screen for a while before set the MainForm property).</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">
</span></h2>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
  class SplashScreenUsingVBFramework : WindowsFormsApplicationBase
  {
      protected override void OnCreateSplashScreen()
      {
          base.OnCreateSplashScreen();
          // You can replace the Splash2 screen to yours.
          this.SplashScreen = new CSWinFormSplashScreen.SplashScreen2();
      }
      protected override void OnCreateMainForm()
      {
          base.OnCreateMainForm();
          //Here you can specify the MainForm you want to start.
          this.MainForm = new MainForm();
      }
  }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal"><span style="">&nbsp;&nbsp;
</span>5. In the static Main method, we can use the class deriving from WindowsFormsApplicationBase to run the application.
</span><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal"></span></h2>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>

<pre id="codePreview" class="csharp">
new SplashScreenUsingVBFramework().Run(args);

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<h2>More Information<span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">
</span></h2>
<h2><span style="font-size:11.0pt; line-height:115%; font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal"><a href="http://msdn.microsoft.com/en-us/library/microsoft.visualbasic.applicationservices.windowsformsapplicationbase.splashscreen.aspx">WindowsFormsApplicationBase.SplashScreen
 Property</a> </span></h2>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
