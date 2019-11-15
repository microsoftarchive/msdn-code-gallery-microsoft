# Windows Form with Aero glass style (CSWinFormExAeroToClient)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Windows Forms
## Topics
- Aero
## Updated
- 08/08/2011
## Description

<p style="font-family:Courier New"></p>
<h2>WINDOWS FORMS APPLICATION : CSWinFormExAeroToClient Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
CSWinFormExAeroToClient example demonstrates how to create a WinFrom application with<br>
Aero effect.<br>
<br>
There are 2 approaches to achieve this effect.<br>
<br>
1. The frame of a Form has the Aero effect, and then we can extend the Frame to client<br>
&nbsp; area using the DwmExtendFrameIntoClientArea method.<br>
<br>
2. Set the client area of a Form to transparent, and enable the blur effect on this window<br>
&nbsp; using DwmEnableBlurBehindWindow method.<br>
<br>
NOTE:<br>
This appication must run on Windows Vista or later version, and the DWM composition is
<br>
enabled.<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
<br>
Step 1. Build this project in VS2010. <br>
<br>
Step 2. Run CSWinFormExAeroToClient.exe<br>
<br>
Step 3. You will see the main form. It will show whether the aero is enabled/supported.<br>
<br>
Step 4. If the Aero is enabled, set the parameters and click the &quot;Apply&quot; button to show a
<br>
&nbsp; &nbsp; &nbsp; &nbsp;Demo Form with the Aero effect. <br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;You can also change the parameters and click the &quot;Apply&quot; button to change the Aero<br>
&nbsp; &nbsp; &nbsp; &nbsp;effect of the Demo Form.<br>
</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
A. Determine whether Aero effect is enabled.<br>
<br>
&nbsp; &nbsp; if (System.Environment.OSVersion.Version.Major &gt;= 6)<br>
&nbsp; &nbsp; {<br>
&nbsp; &nbsp; &nbsp; &nbsp; NativeMethods.DwmIsCompositionEnabled(out isDWMEnable);<br>
&nbsp; &nbsp; }<br>
<br>
B. Extend the frame to achieve the Aero effect.<br>
&nbsp; <br>
&nbsp; 1. Extend the frame using the DwmExtendFrameIntoClientArea method. The margins parameter<br>
&nbsp; &nbsp; &nbsp;of this method defines the margins of the form, in other words, the width and height<br>
&nbsp; &nbsp; &nbsp;of the frame.<br>
&nbsp; &nbsp; &nbsp; <br>
&nbsp; 2. Make the region in the margins transparent. <br>
&nbsp; <br>
&nbsp; Because the region belongs to the frame now, so it will have the Aero effect.<br>
&nbsp; &nbsp; &nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;NativeMethods.DwmExtendFrameIntoClientArea(this.Handle, ref glassMargins);<br>
&nbsp; &nbsp; &nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;marginRegion = new Region(this.ClientRectangle);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (this.GlassMargins.IsNegativeOrOverride(this.ClientSize))<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;e.Graphics.FillRegion(transparentBrush, marginRegion);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;else<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;marginRegion.Exclude(new Rectangle(<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;this.GlassMargins.cxLeftWidth,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;this.GlassMargins.cyTopHeight,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;this.ClientSize.Width - this.GlassMargins.cxLeftWidth - this.GlassMargins.cxRightWidth,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;this.ClientSize.Height - this.GlassMargins.cyTopHeight - this.GlassMargins.cyBottomHeight));<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;e.Graphics.FillRegion(transparentBrush, marginRegion);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
<br>
C. Enable the blur effect on the form to achieve the Aero effect.<br>
<br>
&nbsp; 1. Make the region transparent.<br>
&nbsp; <br>
&nbsp; 2. Enable the blur effect on the form using the DwmEnableBlurBehindWindow method.<br>
<br>
&nbsp; Then the region will have a Aero effect.<br>
&nbsp; <br>
&nbsp; &nbsp; &nbsp;NativeMethods.DWM_BLURBEHIND bbh = new NativeMethods.DWM_BLURBEHIND();<br>
&nbsp; &nbsp; &nbsp;bbh.dwFlags = NativeMethods.DWM_BLURBEHIND.DWM_BB_ENABLE;<br>
&nbsp; &nbsp; &nbsp;bbh.fEnable = true;<br>
&nbsp; &nbsp; &nbsp;bbh.hRegionBlur = this.BlurRegion.GetHrgn(graphics);<br>
&nbsp; &nbsp; &nbsp;NativeMethods.DwmEnableBlurBehindWindow(this.Handle, bbh);<br>
<br>
D. The GlassForm supplies following properties to set the Aero effect.<br>
&nbsp; &nbsp; &nbsp; EnableExtendFrame<br>
&nbsp; &nbsp; &nbsp; GlassMargins<br>
&nbsp; &nbsp; &nbsp; EnableBlurBehindWindow <br>
&nbsp; &nbsp; &nbsp; BlurRegion<br>
<br>
E. The MainForm sets the properties of a GlassForm instance to demo the Aero effect.
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
DwmEnableBlurBehindWindow Function<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/aa969508(VS.85).aspx">http://msdn.microsoft.com/en-us/library/aa969508(VS.85).aspx</a><br>
<br>
DwmExtendFrameIntoClientArea Function<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/aa969512(VS.85).aspx">http://msdn.microsoft.com/en-us/library/aa969512(VS.85).aspx</a><br>
<br>
DwmIsCompositionEnabled Function<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/aa969518(VS.85).aspx">http://msdn.microsoft.com/en-us/library/aa969518(VS.85).aspx</a><br>
<br>
Windows Vista for Developers – Part 3 – The Desktop Window Manager<br>
<a target="_blank" href="http://weblogs.asp.net/kennykerr/archive/2006/08/10/Windows-Vista-for-Developers-_1320_-Part-3-_1320_-The-Desktop-Window-Manager.aspx">http://weblogs.asp.net/kennykerr/archive/2006/08/10/Windows-Vista-for-Developers-_1320_-Part-3-_1320_-The-Desktop-Window-Manager.aspx</a><br>
<br>
Create Special Effects With The Desktop Window Manager<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/magazine/cc163435.aspx">http://msdn.microsoft.com/en-us/magazine/cc163435.aspx</a><br>
<br>
DWM Functions<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/aa969527(v=VS.85).aspx">http://msdn.microsoft.com/en-us/library/aa969527(v=VS.85).aspx</a><br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
