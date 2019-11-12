# Windows hook demo (CSWindowsHook)
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- Windows General
## Topics
- Windows Hook
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>WINDOWS APPLICATION : CSWindowsHook Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Use:</h3>
<p style="font-family:Courier New"><br>
This example demonstrates how to set a hook that is specific to a thread as <br>
well as the global hook by using the low-level mouse and keyboard hooks in <br>
.NET. You can use hooks to monitor certain types of events. You can associate <br>
these events with a specific thread or with all the threads in the same <br>
desktop as the calling thread.<br>
<br>
</p>
<h3>Creation:</h3>
<p style="font-family:Courier New"><br>
Step1. Create a new Windows Form application. Import the <br>
<a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/System.Runtime.InteropServices.aspx" target="_blank" title="Auto generated link to System.Runtime.InteropServices">System.Runtime.InteropServices</a> namespace.<br>
<br>
Step2. Define the callback functions that will be invoked when system or &nbsp;<br>
thread receive mouse and keyboard actions.<br>
<br>
Step3. Set/remove these hooks when the button is clicked.<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;// Set mouse hook<br>
&nbsp;&nbsp;&nbsp;&nbsp;MouseHookProcedure = new HookProc(this.MouseHookProc);<br>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;mouseHook = NativeMethods.SetWindowsHookEx(HookTypes.WH_MOUSE,<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;MouseHookProcedure,<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;(IntPtr)0,<br>
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;AppDomain.GetCurrentThreadId());<br>
&nbsp; &nbsp;<br>
&nbsp;&nbsp;&nbsp;&nbsp;// Remoce mouse hook &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; <br>
&nbsp;&nbsp;&nbsp;&nbsp;bool ret = NativeMethods.UnhookWindowsHookEx(mouseHook);<br>
<br>
Step4. Remove all hooks when closing the main form.<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
How to set a Windows hook in Visual C# .NET<br>
<a target="_blank" href="http://support.microsoft.com/kb/318804">http://support.microsoft.com/kb/318804</a><br>
<br>
Windows Hooks in the .NET Framework<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/magazine/cc188966.aspx">http://msdn.microsoft.com/en-us/magazine/cc188966.aspx</a><br>
<br>
Processing Global Mouse and Keyboard Hooks in C#<br>
<a target="_blank" href="http://www.codeproject.com/KB/cs/globalhook.aspx">http://www.codeproject.com/KB/cs/globalhook.aspx</a><br>
<br>
Global System Hooks in .NET<br>
<a target="_blank" href="http://www.codeproject.com/KB/system/globalsystemhook.aspx">http://www.codeproject.com/KB/system/globalsystemhook.aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
