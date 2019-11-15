# Register hotkey for the current app (CSRegisterHotkey)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Windows General
## Topics
- Hotkey
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>Windows APPLICATION: CSRegisterHotkey Overview </h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
The sample demonstrates how to register and unregister a hotkey for the <br>
current application.<br>
<br>
User32.dll contains 2 extern method RegisterHotKey and UnregisterHotKey to <br>
define or free a system-wide hot key. The method Application.AddMessageFilter <br>
is used to add a message filter to monitor Windows messages as they are <br>
routed to their destinations. Before a message is dispatched, the method <br>
PreFilterMessage could handle it. <br>
<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
Step1. Build this project in VS2010. <br>
<br>
Step2. Run CSRegisterHotkey.exe.<br>
<br>
Step3. Click the textbox in the form, and press Alt&#43;Ctrl&#43;T. You will see <br>
&nbsp; &nbsp; &nbsp; &quot;Alt,Control&#43;T&quot; in the textbox, and the &nbsp;&quot;Register&quot; button is enabled.<br>
<br>
Step4. Click the &quot;Register&quot; button, then the textbox and the &quot;Register&quot;
<br>
&nbsp; &nbsp; &nbsp; button will be disabled, the &quot;Unregister&quot; button will be enabled.<br>
<br>
&nbsp; &nbsp; &nbsp; If the hot key has already been registered, you will get an alert
<br>
&nbsp; &nbsp; &nbsp; &quot;The hotkey is already in use.&quot;. You can try other hotkey like Alt&#43;M.<br>
<br>
Step5. Press Alt&#43;Ctrl&#43;T even when this application is not the active window. <br>
&nbsp; &nbsp; &nbsp; The application will show up and be activated.<br>
<br>
</p>
<h3>Implementation:</h3>
<p style="font-family:Courier New"><br>
1. Design a class HotKeyRegister that wraps 2 extern methods RegisterHotKey <br>
&nbsp; and UnregisterHotKey of User32.dll. This class also supplies a static <br>
&nbsp; method GetModifiers to get the modifiers and key from the KeyData property
<br>
&nbsp; of KeyEventArgs.<br>
<br>
&nbsp; When creating a new instance of this class with the parameters handle, id,
<br>
&nbsp; modifiers and key, the constructor will call the method RegisterHotKey to <br>
&nbsp; register the specified hotkey.<br>
<br>
2. The enum KeyModifiers contains the supported modifiers, like CTRL, ALT and <br>
&nbsp; SHIFT. The WinKey is also supported by the RegisterHotKey method, but <br>
&nbsp; keyboard shortcuts that involve the WINDOWS key are reserved for use by <br>
&nbsp; the operating system.<br>
<br>
3. Design the UI in MainFrom which contains a textbox, 2 buttons and some <br>
&nbsp; labels. <br>
<br>
&nbsp; The MainFrom will handle the KeyDown event of the textbox and check <br>
&nbsp; whether the pressed keys are valid, the keys that must be pressed in <br>
&nbsp; combination with the key Ctrl, Shift or Alt, like Ctrl&#43;Alt&#43;T. <br>
<br>
&nbsp; It will also handle the Click event of the buttons to define or free a <br>
&nbsp; system-wide hotkey. When the form is closed, it will dispose the <br>
&nbsp; HotKeyRegister instance.<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
MSDN: RegisterHotKey Function<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms646309.aspx">http://msdn.microsoft.com/en-us/library/ms646309.aspx</a><br>
<br>
MSDN: UnregisterHotKey Function<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/s646327.aspx">http://msdn.microsoft.com/en-us/library/s646327.aspx</a><br>
<br>
MSDN: Application.AddMessageFilter Method <br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.windows.forms.application.addmessagefilter.aspx">http://msdn.microsoft.com/en-us/library/system.windows.forms.application.addmessagefilter.aspx</a><br>
<br>
IMessageFilter Interface<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/system.windows.forms.imessagefilter.aspx">http://msdn.microsoft.com/en-us/library/system.windows.forms.imessagefilter.aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
