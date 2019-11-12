# Detect the Windows session state (CSDetectWindowsSessionState)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Windows General
## Topics
- Windows Session
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>Windows APPLICATION: CSDetectWindowsSessionState Overview </h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New">The sample demonstrates how to detect the Windows session state.<br>
<br>
1 To detect whether the current session is locked, you can use the extern method <br>
&nbsp;OpenInputDesktop in User32.dll. This method is used to open the desktop that
<br>
&nbsp;receives user input. If this operation failed, it means that the session is locked.<br>
<br>
2 If a user locked / unlocked the current session, you can use the SessionSwitch event<br>
&nbsp;to monitor it. <br>
<br>
&nbsp;<a class="libraryLink" href="http://msdn.microsoft.com/en-US/library/Microsoft.Win32.SystemEvents.aspx" target="_blank" title="Auto generated link to Microsoft.Win32.SystemEvents">Microsoft.Win32.SystemEvents</a> contains a SessionSwitch event. This event occurs<br>
&nbsp;when the user session switches, e.g. when the session is locked / unlocked or
<br>
&nbsp;when a user has logged on to a session, and so on. If you register this event,
<br>
&nbsp;you can get the session state when the state changed.<br>
<br>
<br>
Note:<br>
1 If UAC popups a secure desktop, this method may also fail. There is no API for<br>
&nbsp;differentiating between Locked Desktop and UAC Secure Desktop.<br>
<br>
2 The SessionSwitch event is fired only in the current session. If another user locks
<br>
&nbsp;or unlocks his session on the same machine, you won't see this event in your own
<br>
&nbsp;session.<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
Step1. Build this project in VS2010. <br>
<br>
Step2. Run CSDetectWindowsSessionState.exe.<br>
<br>
Step3. Check the &quot;Enable a timer to detect the session state every 5 seconds&quot;, and then<br>
&nbsp; &nbsp; &nbsp; you will see &quot;Current State: &lt;state&gt; Time: &lt;time&gt;&quot; on the top of the UI,
<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; and a new record in the list box every 5 seconds.
<br>
<br>
Step4. Uncheck the &quot;Enable a timer to detect the session state every 5 seconds&quot;.
<br>
<br>
Step5. Press Win&#43;L to lock the PC, then unlock it. You will see 2 new records in the<br>
&nbsp; &nbsp; &nbsp; list box. One is &quot;&lt;time&gt; SessionLock &lt;Occurred&gt;&quot; and another one is<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp;&quot;&lt;time&gt; SessionUnlock &lt;Occurred&gt;&quot;.<br>
<br>
</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
1. Design a class WindowsSession. When an instance of this class is initialized, register<br>
&nbsp; the SystemEvents.SessionSwitch event.<br>
&nbsp; &nbsp;<br>
2. If the SystemEvents.SessionSwitch event occurs, then raise a StateChanged event.<br>
<br>
3. The class WindowsSession also wraps 2 extern methods OpenInputDesktop and CloseDesktop.<br>
&nbsp; If the method OpenInputDesktop fails, the return value is IntPtr.Zero, which means that<br>
&nbsp; the current session is locked.<br>
&nbsp; <br>
4. Design the UI in MainForm which contains a list box. <br>
<br>
&nbsp; The MainForm will handle the StateChanged event of a WindowsSession instance. If the<br>
&nbsp; event occurred, then add a new record in the list box. &nbsp;<br>
<br>
&nbsp; The MainForm also contains a timer that could detect the WindowsSessionState every 5<br>
&nbsp; seconds.<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
SystemEvents.SessionSwitch Event<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/microsoft.win32.systemevents.sessionswitch.aspx">http://msdn.microsoft.com/en-us/library/microsoft.win32.systemevents.sessionswitch.aspx</a><br>
<br>
OpenInputDesktop Function<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms684309(VS.85).aspx">http://msdn.microsoft.com/en-us/library/ms684309(VS.85).aspx</a><br>
<br>
CloseDesktop Function<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms682024(VS.85).aspx">http://msdn.microsoft.com/en-us/library/ms682024(VS.85).aspx</a><br>
<br>
Desktop Security and Access Rights<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms682575(VS.85).aspx">http://msdn.microsoft.com/en-us/library/ms682575(VS.85).aspx</a><br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
