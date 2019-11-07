# Show GIF animation in WPF (CSWPFAnimatedGIF)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- WPF
## Topics
- Animation
- GIF
## Updated
- 08/08/2011
## Description

<p style="font-family:Courier New"></p>
<h2>WPF APPLICATION : CSWPFAnimatedGIF Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
The code sample demonstrates how to implement an animated GIF image in WPF.<br>
<br>
Note:<br>
<br>
GIF images are deprecated in WPF due to the imperfect scaling ability.<br>
<br>
In WPF MediaElement could show animated GIF images with some less desirable limitations:<br>
1. The Source property must be set with absolute path;<br>
2. Play-back functionality is not supported by default;<br>
3. Transparent pixels display as black.<br>
<br>
This sample is intended to show how to implement a simple and efficient<br>
animated GIF image control in case this might be a better alternative in some situations.<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
Step1. Build the sample project in Visual Studio 2010.<br>
<br>
Step2. Click &quot;Start&quot; button and play the animated GIF image; <br>
Click &quot;Stop&quot; button to stop the GIF image.<br>
</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
1. Use ImageAnimator to animates a GIF image that has time-based frames.<br>
<br>
2. Render every frame of GIF image in another thread using InvalidateVisual method.<br>
<br>
3. Derive from the Image control, in order to make it easy to use as Image control.<br>
<br>
4. Dispose the dump resources to ensure the efficency.<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.windows.controls.mediaelement.aspx">http://msdn.microsoft.com/en-us/library/system.windows.controls.mediaelement.aspx</a><br>
<a target="_blank" href="http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/93d50a97-0d8d-4b18-992e-cd3200693337">http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/93d50a97-0d8d-4b18-992e-cd3200693337</a><br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.drawing.imageanimator.aspx">http://msdn.microsoft.com/en-us/library/system.drawing.imageanimator.aspx</a><br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
