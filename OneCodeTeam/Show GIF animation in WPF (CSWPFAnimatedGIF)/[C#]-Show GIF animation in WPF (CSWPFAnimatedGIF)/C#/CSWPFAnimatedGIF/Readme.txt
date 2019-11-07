===============================================================================
       WPF APPLICATION : CSWPFAnimatedGIF Overview
===============================================================================

///////////////////////////////////////////////////////////////////////////////
Summary:

The code sample demonstrates how to implement an animated GIF image in WPF.

Note:

GIF images are deprecated in WPF due to the imperfect scaling ability.

In WPF MediaElement could show animated GIF images with some less desirable limitations:
1. The Source property must be set with absolute path;
2. Play-back functionality is not supported by default;
3. Transparent pixels display as black.

This sample is intended to show how to implement a simple and efficient
animated GIF image control in case this might be a better alternative in some situations.

///////////////////////////////////////////////////////////////////////////////
Demo:

Step1. Build the sample project in Visual Studio 2010.

Step2. Click "Start" button and play the animated GIF image; 
Click "Stop" button to stop the GIF image.

///////////////////////////////////////////////////////////////////////////////
Code Logic:

1. Use ImageAnimator to animates a GIF image that has time-based frames.

2. Render every frame of GIF image in another thread using InvalidateVisual method.

3. Derive from the Image control, in order to make it easy to use as Image control.

4. Dispose the dump resources to ensure the efficency.


///////////////////////////////////////////////////////////////////////////////
References:

http://msdn.microsoft.com/en-us/library/system.windows.controls.mediaelement.aspx
http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/93d50a97-0d8d-4b18-992e-cd3200693337
http://msdn.microsoft.com/en-us/library/system.drawing.imageanimator.aspx

///////////////////////////////////////////////////////////////////////////////
