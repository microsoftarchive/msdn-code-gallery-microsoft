# Windows Form with Aero glass style (VBWinFormExAeroToClient)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Windows Forms
## Topics
- Aero
## Updated
- 02/03/2012
## Description

<p style="font-family:Courier New">&nbsp;</p>
<h2>WINDOWS FORMS APPLICATION : VBWinFormExAeroToClient Project Overview</h2>
<p style="font-family:Courier New">&nbsp;</p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
VBWinFormExAeroToClient example demonstrates how to create a WinFrom application with<br>
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
enabled.</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
<br>
Step 1. Build this project in VS2010. <br>
<br>
Step 2. Run VBWinFormExAeroToClient.exe<br>
<br>
Step 3. You will see the main form. It will show whether the aero is enabled/supported.<br>
<br>
Step 4. If the Aero is enabled, set the parameters and click the &quot;Apply&quot; button to show a
<br>
&nbsp; &nbsp; &nbsp; &nbsp;Demo Form with the Aero effect. <br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;You can also change the parameters and click the &quot;Apply&quot; button to change the Aero<br>
&nbsp; &nbsp; &nbsp; &nbsp;effect of the Demo Form.</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
A. Determine whether Aero effect is enabled.<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;If Environment.OSVersion.Version.Major &gt;= 6 Then<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' Make sure the Glass is enabled by the user.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;NativeMethods.DwmIsCompositionEnabled(isDWMEnable)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;End If<br>
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
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;NativeMethods.DwmExtendFrameIntoClientArea(Me.Handle, glassMargins)<br>
&nbsp; &nbsp; &nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' Make the region in the margins transparent. <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;_marginRegion = New Region(Me.ClientRectangle)<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' If the glassMargins contains a negative value, or the values are not valid,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' then make the whole form transparent.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;If Me.GlassMargins.IsNegativeOrOverride(Me.ClientSize) Then<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;e.Graphics.FillRegion(transparentBrush, _marginRegion)<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' By default, exlucde the region of the client area.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Else<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;_marginRegion.Exclude(New Rectangle(<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Me.GlassMargins.cxLeftWidth,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Me.GlassMargins.cyTopHeight,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Me.ClientSize.Width - Me.GlassMargins.cxLeftWidth - Me.GlassMargins.cxRightWidth,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Me.ClientSize.Height - Me.GlassMargins.cyTopHeight - Me.GlassMargins.cyBottomHeight))<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;e.Graphics.FillRegion(transparentBrush, _marginRegion)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp;End If<br>
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
&nbsp; &nbsp; &nbsp;Dim bbh As New NativeMethods.DWM_BLURBEHIND()<br>
&nbsp; &nbsp; &nbsp;bbh.dwFlags = NativeMethods.DWM_BLURBEHIND.DWM_BB_ENABLE<br>
&nbsp; &nbsp; &nbsp;bbh.fEnable = True<br>
&nbsp; &nbsp; &nbsp;bbh.hRegionBlur = Me.BlurRegion.GetHrgn(graphics)<br>
&nbsp; &nbsp; &nbsp;NativeMethods.DwmEnableBlurBehindWindow(Me.Handle, bbh)<br>
<br>
D. The GlassForm supplies following properties to set the Aero effect.<br>
&nbsp; &nbsp; &nbsp; EnableExtendFrame<br>
&nbsp; &nbsp; &nbsp; GlassMargins<br>
&nbsp; &nbsp; &nbsp; EnableBlurBehindWindow <br>
&nbsp; &nbsp; &nbsp; BlurRegion<br>
<br>
E. The MainForm sets the properties of a GlassForm instance to demo the Aero effect.</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
DwmEnableBlurBehindWindow Function<br>
<a href="http://msdn.microsoft.com/en-us/library/aa969508(VS.85).aspx" target="_blank">http://msdn.microsoft.com/en-us/library/aa969508(VS.85).aspx</a><br>
<br>
DwmExtendFrameIntoClientArea Function<br>
<a href="http://msdn.microsoft.com/en-us/library/aa969512(VS.85).aspx" target="_blank">http://msdn.microsoft.com/en-us/library/aa969512(VS.85).aspx</a><br>
<br>
DwmIsCompositionEnabled Function<br>
<a href="http://msdn.microsoft.com/en-us/library/aa969518(VS.85).aspx" target="_blank">http://msdn.microsoft.com/en-us/library/aa969518(VS.85).aspx</a><br>
<br>
Windows Vista for Developers &ndash; Part 3 &ndash; The Desktop Window Manager<br>
<a href="http://weblogs.asp.net/kennykerr/archive/2006/08/10/Windows-Vista-for-Developers-_1320_-Part-3-_1320_-The-Desktop-Window-Manager.aspx" target="_blank">http://weblogs.asp.net/kennykerr/archive/2006/08/10/Windows-Vista-for-Developers-_1320_-Part-3-_1320_-The-Desktop-Window-Manager.aspx</a><br>
<br>
Create Special Effects With The Desktop Window Manager<br>
<a href="http://msdn.microsoft.com/en-us/magazine/cc163435.aspx" target="_blank">http://msdn.microsoft.com/en-us/magazine/cc163435.aspx</a><br>
<br>
DWM Functions<br>
<a href="http://msdn.microsoft.com/en-us/library/aa969527(v=VS.85).aspx" target="_blank">http://msdn.microsoft.com/en-us/library/aa969527(v=VS.85).aspx</a></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
