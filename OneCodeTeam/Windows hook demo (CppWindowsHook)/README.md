# Windows hook demo (CppWindowsHook)
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
<h2>WIN32 APPLICATION : CppWindowsHook Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Use:</h3>
<p style="font-family:Courier New"><br>
A hook is a point in the system message-handling mechanism where an application <br>
can install a subroutine to monitor the message traffic in the system and <br>
process certain types of messages before they reach the target window procedure.<br>
There are two types of Hooks - Thread specific hooks and global hooks. A <br>
thread specific hook is associated with particular thread only. While a global <br>
hook is associated with all threads in the same desktop as the calling thread.<br>
You must place a global hook procedure in a DLL(CppHookDll) separate from the <br>
application installing the hook procedure. <br>
<br>
</p>
<h3>Project Relation:</h3>
<p style="font-family:Courier New"><br>
CppWindowsHook -&gt; CppHookDll<br>
CppHookDll posts a private message(WM_KEYSTROKE or WM_KEYINPUT) to <br>
CppWindowsHook's main window(CppWindowsHookDlg).WM_KEYINPUT is sent when a <br>
real key is stroked.<br>
<br>
</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
1. Install a thread or global hook.<br>
<br>
2. The hook procedure post a private message(WM_KEYSTROKE or WM_KEYINPUT) to the<br>
&nbsp; main Window(CppWindowsHookDlg). And WM_KEYINPUT is sent when a real key is
<br>
&nbsp; stroked if you setup a WH_KEYBOARD_LL hook.<br>
<br>
3. Log the information when main windows receive the private message.<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
MSDN: About Hooks<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms644959(VS.85).aspx">http://msdn.microsoft.com/en-us/library/ms644959(VS.85).aspx</a><br>
<br>
MSDN: Using Hooks <br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms644960(VS.85).aspx">http://msdn.microsoft.com/en-us/library/ms644960(VS.85).aspx</a><br>
<br>
Hooking the Keyboard <br>
<a target="_blank" href="http://www.codeguru.com/cpp/w-p/system/keyboard/article.php/c5699">http://www.codeguru.com/cpp/w-p/system/keyboard/article.php/c5699</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
