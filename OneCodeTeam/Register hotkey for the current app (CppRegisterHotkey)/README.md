# Register hotkey for the current app (CppRegisterHotkey)
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
<h2>WIN32 APPLICATION : CppRegisterHotkey Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
The sample demonstrates how to register and unregister a hotkey for the <br>
current application.<br>
<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
Step1. Build this project in VS2010. <br>
<br>
Step2. Run CppRegisterHotkey.exe.<br>
<br>
Step3. Click the edit control in the window, and press Alt&#43;Ctrl&#43;T. You will <br>
&nbsp; &nbsp; &nbsp; see &quot;Alt,Control&#43;T&quot; displayed in the edit control, and the &quot;Register&quot;
<br>
&nbsp; &nbsp; &nbsp; button becomes enabled.<br>
<br>
Step4. Click the &quot;Register&quot; button, then the edit control and the &quot;Register&quot;
<br>
&nbsp; &nbsp; &nbsp; button are disabled, and the &quot;Unregister&quot; button becomes enabled.<br>
<br>
&nbsp; &nbsp; &nbsp; If the hot key has already been registered, you will get an alert
<br>
&nbsp; &nbsp; &nbsp; &quot;The hotkey is already in use.&quot;. You can try other hotkey e.g Alt&#43;M.<br>
<br>
Step5. Press Alt&#43;Ctrl&#43;T even when this application is not the active window. <br>
&nbsp; &nbsp; &nbsp; The application will show up and be activated.<br>
<br>
</p>
<h3>Implementation:</h3>
<p style="font-family:Courier New"><br>
A. Implementing the hotkey edit control.<br>
<br>
The edit control traps the key-down messages, and captures the key <br>
combination. To trap the key-down messages, we subclass the edit control in <br>
OnInitDialog:<br>
<br>
&nbsp; &nbsp;// Subclass the hotkey edit control.<br>
&nbsp; &nbsp;HWND hHotKeyEdit = GetDlgItem(hWnd, IDC_EDIT_HOTKEY);<br>
&nbsp; &nbsp;UINT_PTR uIdSubclass = 0;<br>
&nbsp; &nbsp;if (!SetWindowSubclass(hHotKeyEdit, HotKeyEditProc, uIdSubclass, 0))<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;ReportError(L&quot;SetWindowSubclass in OnInitDialog&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp;return FALSE;<br>
&nbsp; &nbsp;}<br>
<br>
We remove the window subclass before the edit control is destroyed by <br>
inserting a call to RemoveWindowSubclass inside the subclass procedure <br>
itself:<br>
<br>
&nbsp; &nbsp;LRESULT CALLBACK HotKeyEditProc(HWND hWnd, UINT message, WPARAM wParam,
<br>
&nbsp; &nbsp; &nbsp; &nbsp;LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;switch (message)<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;case WM_NCDESTROY:<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;if (!RemoveWindowSubclass(hWnd, HotKeyEditProc, uIdSubclass))<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;ReportError(L&quot;RemoveWindowSubclass in handling WM_NCDESTROY&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;return DefSubclassProc(hWnd, message, wParam, lParam);<br>
&nbsp; &nbsp; &nbsp; &nbsp;...<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp;}<br>
<br>
HotKeyEditProc is the new procedure that processes messages for the hotkey <br>
edit control. Every time a message is received by the new window procedure, a <br>
subclass ID and reference data are included.<br>
<br>
HotKeyEditProc traps the WM_SYSKEYDOWN, WM_KEYDOWN, WM_SYSCHAR, WM_CHAR, <br>
WM_SYSKEYUP and WM_KEYUP messages and stops their default message handler <br>
by returning zero. The six messages are introduced in this MSDN article:<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/gg153546.aspx.">http://msdn.microsoft.com/en-us/library/gg153546.aspx.</a><br>
<br>
&nbsp; &nbsp;LRESULT CALLBACK HotKeyEditProc(HWND hWnd, UINT message, WPARAM wParam,
<br>
&nbsp; &nbsp; &nbsp; &nbsp;LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;switch (message)<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;case WM_SYSKEYDOWN:<br>
&nbsp; &nbsp; &nbsp; &nbsp;case WM_KEYDOWN:<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Process the WM_KEYDOWN and WM_SYSKEYDOWN messages in
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// OnHotKeyEditKeyDown. WM_SYSKEYDOWN is posted to the window with
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// the keyboard focus when the user presses the F10 key (which
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// activates the menu bar) or holds down the ALT key and then presses
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// another key. It also occurs when no window currently has the
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// keyboard focus.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;OnHotKeyEditKeyDown(hWnd, static_cast&lt;UINT&gt;(wParam), lParam);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;return 0;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;case WM_SYSCHAR:<br>
&nbsp; &nbsp; &nbsp; &nbsp;case WM_CHAR:<br>
&nbsp; &nbsp; &nbsp; &nbsp;case WM_SYSKEYUP:<br>
&nbsp; &nbsp; &nbsp; &nbsp;case WM_KEYUP:<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Stop processing.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;return 0;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;...<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp;}<br>
<br>
OnHotKeyEditKeyDown handles the WM_KEYDOWN and WM_SYSKEYDOWN messages posted <br>
to the edit control. It validates the hotkey combination. If no modifier key <br>
(Ctrl / Shift / Alt) is pressed, the function returns directly. If the key <br>
combination is valid (e.g. Ctrl, Alt&#43;T), it displays the key combination in <br>
the edit control, and saves the key modifiers and the virtual-key code in <br>
the global variables: g_fsModifiers and g_vk.<br>
<br>
---------------------------<br>
<br>
B. Registering / unregistering the hotkey, and processing the WM_HOTKEY <br>
message.<br>
<br>
The registration and unregistration of the hotkey are implemented in the <br>
OnCommand function. If the IDC_BUTTON_REGISTER button is clicked, it calls <br>
RegisterHotKey with the g_vk and g_fsModifiers values set previously to <br>
register the hotkey. <br>
<br>
The second parameter of RegisterHotKey is the hotkey ID. An application must <br>
specify an id value in the range 0x0000 through 0xBFFF (100 in this code <br>
sample). A shared DLL must specify a value in the range 0xC000 through 0xFFFF <br>
(the range returned by the GlobalAddAtom function). To avoid conflicts with <br>
hot-key identifiers defined by other shared DLLs, a DLL should use the <br>
GlobalAddAtom function to obtain the hot-key identifier.<br>
<br>
RegisterHotKey fails if the keystrokes specified for the hot key have already <br>
been registered by another hot key. <br>
<br>
&nbsp; &nbsp;case IDC_BUTTON_REGISTER:<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Register the hotkey.<br>
&nbsp; &nbsp; &nbsp; &nbsp;if (RegisterHotKey(hWnd, g_idHotKey, g_fsModifiers, g_vk))<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Update the UI.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_REGISTER), FALSE);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Edit_Enable(GetDlgItem(hWnd, IDC_EDIT_HOTKEY), FALSE);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_UNREGISTER), TRUE);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;g_fHotkeyRegistered = TRUE;<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;else<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;ReportError(L&quot;RegisterHotKey&quot;, GetLastError());<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;break;<br>
<br>
When a key is pressed, the system looks for a match against all hot keys. Upon <br>
finding a match, the system posts the WM_HOTKEY message to the message queue <br>
of the window with which the hot key is associated. If the hot key is not <br>
associated with a window, then the WM_HOTKEY message is posted to the thread <br>
associated with the hot key. In this code sample, when WM_HOTKEY is received, <br>
we activate the application main window and bring it into the front. <br>
<br>
&nbsp; &nbsp;// Handle the WM_HOTKEY message in OnHotKey<br>
&nbsp; &nbsp;HANDLE_MSG (hWnd, WM_HOTKEY, OnHotKey);<br>
<br>
&nbsp; &nbsp;//<br>
&nbsp; &nbsp;// &nbsp; FUNCTION: OnHotKey(HWND, int, UINT, UINT)<br>
&nbsp; &nbsp;//<br>
&nbsp; &nbsp;// &nbsp; PURPOSE: Process the WM_HOTKEY message. The WM_HOTKEY message is posted
<br>
&nbsp; &nbsp;// &nbsp; when the user presses a hot key registered by the RegisterHotKey
<br>
&nbsp; &nbsp;// &nbsp; function. The message is placed at the top of the message queue
<br>
&nbsp; &nbsp;// &nbsp; associated with the thread that registered the hot key. <br>
&nbsp; &nbsp;//<br>
&nbsp; &nbsp;void OnHotKey(HWND hWnd, int idHotKey, UINT fuModifiers, UINT vk)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;// If the window is minimized, restore it.<br>
&nbsp; &nbsp; &nbsp; &nbsp;if (IsIconic(hWnd))<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;ShowWindow(hWnd, SW_SHOWNORMAL);<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Bring the window into the front, and activate the window.<br>
&nbsp; &nbsp; &nbsp; &nbsp;SetForegroundWindow(hWnd);<br>
&nbsp; &nbsp;}<br>
<br>
If the IDC_BUTTON_UNREGISTER button is clicked, the OnCommand function calls <br>
UnregisterHotKey to unregister the hotkey. &nbsp;<br>
<br>
&nbsp; &nbsp;case IDC_BUTTON_UNREGISTER:<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Unregister the hotkey.<br>
&nbsp; &nbsp; &nbsp; &nbsp;if (UnregisterHotKey(hWnd, g_idHotKey))<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;// Update the UI.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_REGISTER), TRUE);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Edit_Enable(GetDlgItem(hWnd, IDC_EDIT_HOTKEY), TRUE);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_UNREGISTER), FALSE);<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;g_fHotkeyRegistered = FALSE;<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;else<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;ReportError(L&quot;UnregisterHotKey&quot;, GetLastError());<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp; &nbsp; &nbsp;break;<br>
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
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
