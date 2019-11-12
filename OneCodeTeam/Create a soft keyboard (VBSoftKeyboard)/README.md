# Create a soft keyboard (VBSoftKeyboard)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Windows General
## Topics
- Virtual Keyboard
- Soft keyboard
- On-screen keyboard
## Updated
- 06/19/2011
## Description

<p style="font-family:Courier New">&nbsp;</p>
<h2>Windows APPLICATION: VBSoftKeyboard Overview</h2>
<p style="font-family:Courier New">&nbsp;</p>
<h3>Summary:</h3>
<p style="font-family:Courier New">The sample demonstrates how to create a soft keyboard. It has the following
<br>
features<br>
<br>
1. It will not get focus when a key button clicked.<br>
<br>
2. If the user presses the left mouse button within its nonclient area(such as the<br>
&nbsp; title bar), it will be activated. When the left mouse button is released, it will<br>
&nbsp; activate the previous foreground Window.<br>
<br>
3 When user clicks a charactor on it, like &quot;A&quot; or &quot;1&quot;, it will send the key to <br>
&nbsp;the active application.<br>
<br>
4 It supports special keys, like &quot;WinKey&quot; &quot;Delete&quot;.<br>
<br>
5 It supports the combination of keys, like &quot;Ctrl&#43;C&quot;.<br>
<br>
NOTE: Ctrl&#43;Alt&#43;Del is not supported as it will cause security issue.</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
Step1. Build this project in VS2010. <br>
<br>
Step2. Open NotePad.exe and then run VBSoftKeyboard.exe. Make sure NotePad.exe <br>
&nbsp; &nbsp; &nbsp; is the active application.<br>
<br>
<br>
Demo a normal key button.<br>
<br>
Step3. Click the &quot;a&quot; key on the keyboard, you will see that a letter &quot;a&quot; appears in the
<br>
&nbsp; &nbsp; &nbsp; NotePad.exe.<br>
<br>
<br>
Demo a lock key button.<br>
<br>
Step4. Click the &quot;Caps&quot; key, you will see the background of the key is changed to white.<br>
&nbsp; &nbsp; &nbsp; And the text of all the letter keys will be changed to UpperCase.
<br>
<br>
Step5. Click the &quot;a&quot; key on the keyboard, you will see that a letter &quot;A&quot; appears in the
<br>
&nbsp; &nbsp; &nbsp; NotePad.exe.<br>
<br>
Step6. Click the &quot;Caps&quot; key, you will see the background of the key is changed to black again.<br>
&nbsp; &nbsp; &nbsp; And the text of all the letter keys will be changed to LowerCase.
<br>
<br>
<br>
Demo the Win key.<br>
<br>
Step7. Click the &quot;Win&quot; key, you will see the background of the key is changed to white.<br>
<br>
Step8. Click the &quot;Win&quot; key again, you will see the background of the key is changed to black again.<br>
&nbsp; &nbsp; &nbsp; And the Start menu is opened.<br>
<br>
<br>
Demo shift key.<br>
<br>
Step9. Click the left &quot;Shift&quot; key, you will see the background of this key and the right<br>
&nbsp; &nbsp; &nbsp; &quot;Shift&quot; key are changed to white. And the keys that support Shift will display the shift<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; text. For example, the key &quot;=&quot; will display as &quot;&#43;&quot;.<br>
<br>
Step10. Click the key &quot;&#43;&quot; on the keyboard, you will see that a letter &quot;&#43;&quot; appears in the
<br>
&nbsp; &nbsp; &nbsp; &nbsp;NotePad.exe. <br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; <br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp;The background of the left and right &quot;Shift&quot; keys will be changed to black, and the keys<br>
&nbsp;&nbsp;&nbsp;&nbsp; &nbsp; &nbsp;that support Shift will display the normal text.
<br>
<br>
<br>
Demo the combination of keys.<br>
<br>
Step11. Click the left &quot;Ctrl&quot; key, you will see the background of this key and the right<br>
&nbsp; &nbsp; &nbsp; &nbsp;&quot;Ctrl&quot; key are changed to white.<br>
<br>
Step12. Click the &quot;s&quot; key, you will see that NotePad.exe will show a Save File dialog.</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
1. Design a NoActivateWindow class to be inherited by the main KeyBoardForm.<br>
<br>
&nbsp; The NoActivateWindow class represents a form that will not be activated until the user<br>
&nbsp; presses the left mouse button within its nonclient area(such as the title bar, menu bar,
<br>
&nbsp; or window frame). When the left mouse button is released, this window will activate the<br>
&nbsp; previous foreground Window.<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' &lt;summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' Set the form style to WS_EX_NOACTIVATE so that it will not get focus.
<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' &lt;/summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;Protected Overrides ReadOnly Property CreateParams() As CreateParams<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;&lt;PermissionSetAttribute(SecurityAction.LinkDemand, Name:=&quot;FullTrust&quot;)&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Get<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Dim cp As CreateParams = MyBase.CreateParams<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;cp.ExStyle = cp.ExStyle Or CInt(Fix(WS_EX_NOACTIVATE))<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Return cp<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;End Get<br>
&nbsp; &nbsp; &nbsp; &nbsp;End Property<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' &lt;summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' Process Windows messages.<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' <br>
&nbsp; &nbsp; &nbsp; &nbsp;''' When the user presses the left mouse button while the cursor is within the<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' nonclient area of this window, the it will store the handle of previous
<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' foreground Window, and then activate itself.<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' <br>
&nbsp; &nbsp; &nbsp; &nbsp;''' When the cursor is moved within the nonclient area of the window, which means<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' that the left mouse button is released, this window will activate the previous
<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' foreground Window.<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' &lt;/summary&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;''' &lt;param name=&quot;m&quot;&gt;&lt;/param&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;&lt;PermissionSetAttribute(SecurityAction.LinkDemand, Name:=&quot;FullTrust&quot;)&gt;<br>
&nbsp; &nbsp; &nbsp; &nbsp;Protected Overrides Sub WndProc(ByRef m As Message)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Select Case m.Msg<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Case WM_NCLBUTTONDOWN<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' Get the current foreground window.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Dim foregroundWindow = UnsafeNativeMethods.GetForegroundWindow()<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' If this window is not the current foreground window, then activate<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' itself.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;If foregroundWindow = Me.Handle Then<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;UnsafeNativeMethods.SetForegroundWindow(Me.Handle)<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' Store the handle of previous foreground window.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;If foregroundWindow = IntPtr.Zero Then<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;previousForegroundWindow = foregroundWindow<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;End If<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;End If<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Case WM_NCMOUSEMOVE<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' Determine whether previous window still exist. If yes, then
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' activate it.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' Note: There is a scenario that the previous window is closed, but
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' &nbsp; &nbsp; &nbsp; the same handle is assgined to a new window.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;If UnsafeNativeMethods.IsWindow(previousForegroundWindow) Then<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;UnsafeNativeMethods.SetForegroundWindow(previousForegroundWindow)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;End If<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Case Else<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;End Select<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;MyBase.WndProc(m)<br>
&nbsp; &nbsp; &nbsp; &nbsp;End Sub<br>
<br>
2. The KeyboardInput class wraps the SendInput method in User32.dll and supplies SendKey method to
<br>
&nbsp; simulate a normal key press event. It also supports the combination of keys, like &quot;Ctrl&#43;C&quot;.<br>
<br>
&nbsp; There are 3 scenarios:<br>
&nbsp; 2.1. A single key is pressed, such as &quot;A&quot;.<br>
&nbsp; &nbsp; &nbsp; &nbsp;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Dim inputs = New NativeMethods.INPUT(0) {}<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;inputs(0).type = NativeMethods.INPUT_KEYBOARD<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;inputs(0).inputUnion.ki.wVk = CShort(Fix(key))<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;UnsafeNativeMethods.SendInput(1, inputs, Marshal.SizeOf(inputs(0)))<br>
<br>
&nbsp; 2.2. A key with modifier keys is pressed, such as &quot;Ctrl&#43;A&quot;.<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' To simulate this scenario, the inputs contains the toggling
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' modifier keys, pressing the key and releasing modifier keys events.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;'<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' For example, to simulate Ctrl&#43;C, we have to send 3 inputs:<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' 1. Ctrl is pressed.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' 2. C is pressed. <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' 3. Ctrl is released.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Dim inputs = New NativeMethods.INPUT(modifierKeys.Count() * 2) {}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Dim i As Integer = 0<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' Simulate toggling the modifier keys.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;For Each modifierKey In modifierKeys<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;inputs(i).type = NativeMethods.INPUT_KEYBOARD<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;inputs(i).inputUnion.ki.wVk = CShort(Fix(modifierKey))<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;i &#43;= 1<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Next modifierKey<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' Simulate pressing the key.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;inputs(i).type = NativeMethods.INPUT_KEYBOARD<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;inputs(i).inputUnion.ki.wVk = CShort(Fix(key))<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;i &#43;= 1<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;For Each modifierKey In modifierKeys<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;inputs(i).type = NativeMethods.INPUT_KEYBOARD<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;inputs(i).inputUnion.ki.wVk = CShort(Fix(modifierKey))<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' 0x0002 means that the key-up event.
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;inputs(i).inputUnion.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;i &#43;= 1<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Next modifierKey<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;UnsafeNativeMethods.SendInput(CUInt(inputs.Length), inputs, Marshal.SizeOf(inputs(0)))<br>
<br>
&nbsp; 2.3. A key that could be toggled is pressed, such as Caps Lock, Num Lock or<br>
&nbsp; &nbsp; &nbsp; &nbsp;Scroll Lock. <br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Dim inputs = New NativeMethods.INPUT(1) {}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' Press the key.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;inputs(0).type = NativeMethods.INPUT_KEYBOARD<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;inputs(0).inputUnion.ki.wVk = CShort(Fix(key))<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;' Release the key.<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;inputs(1).type = NativeMethods.INPUT_KEYBOARD<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;inputs(1).inputUnion.ki.wVk = CShort(Fix(key))<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;inputs(1).inputUnion.ki.dwFlags = NativeMethods.KEYEVENTF_KEYUP<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;UnsafeNativeMethods.SendInput(2, inputs, Marshal.SizeOf(inputs(0)))<br>
<br>
&nbsp; &nbsp; &nbsp; <br>
3. The KeyBoardForm class is the main form of the keyboard. When the form is <br>
&nbsp; being loaded, it will load the KeysMapping.xml to initialize the keyboard buttons.
<br>
<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
SendInput Function<br>
<a href="http://msdn.microsoft.com/en-us/library/ms646310(VS.85).aspx" target="_blank">http://msdn.microsoft.com/en-us/library/ms646310(VS.85).aspx</a><br>
<br>
GetKeyState Function<br>
<a href="http://msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx" target="_blank">http://msdn.microsoft.com/en-us/library/ms646301(VS.85).aspx</a><br>
<br>
Control.CreateParams Property<br>
<a href="http://msdn.microsoft.com/en-us/library/system.windows.forms.control.createparams.aspx" target="_blank">http://msdn.microsoft.com/en-us/library/system.windows.forms.control.createparams.aspx</a><br>
<br>
GetForegroundWindow Function<br>
<a href="http://msdn.microsoft.com/en-us/library/ms633505(VS.85).aspx" target="_blank">http://msdn.microsoft.com/en-us/library/ms633505(VS.85).aspx</a><br>
<br>
SetForegroundWindow Function<br>
<a href="http://msdn.microsoft.com/en-us/library/ms633539(VS.85).aspx" target="_blank">http://msdn.microsoft.com/en-us/library/ms633539(VS.85).aspx</a></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
