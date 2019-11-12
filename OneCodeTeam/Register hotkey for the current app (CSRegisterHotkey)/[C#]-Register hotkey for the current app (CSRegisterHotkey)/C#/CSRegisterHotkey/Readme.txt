=============================================================================
       Windows APPLICATION: CSRegisterHotkey Overview                        
=============================================================================
/////////////////////////////////////////////////////////////////////////////
Summary:

The sample demonstrates how to register and unregister a hotkey for the 
current application.

User32.dll contains 2 extern method RegisterHotKey and UnregisterHotKey to 
define or free a system-wide hot key. The method Application.AddMessageFilter 
is used to add a message filter to monitor Windows messages as they are 
routed to their destinations. Before a message is dispatched, the method 
PreFilterMessage could handle it. 


/////////////////////////////////////////////////////////////////////////////
Demo:

Step1. Build this project in VS2010. 

Step2. Run CSRegisterHotkey.exe.

Step3. Click the textbox in the form, and press Alt+Ctrl+T. You will see 
       "Alt,Control+T" in the textbox, and the  "Register" button is enabled.

Step4. Click the "Register" button, then the textbox and the "Register" 
       button will be disabled, the "Unregister" button will be enabled.

       If the hot key has already been registered, you will get an alert 
       "The hotkey is already in use.". You can try other hotkey like Alt+M.

Step5. Press Alt+Ctrl+T even when this application is not the active window. 
       The application will show up and be activated.


/////////////////////////////////////////////////////////////////////////////
Implementation:

1. Design a class HotKeyRegister that wraps 2 extern methods RegisterHotKey 
   and UnregisterHotKey of User32.dll. This class also supplies a static 
   method GetModifiers to get the modifiers and key from the KeyData property 
   of KeyEventArgs.

   When creating a new instance of this class with the parameters handle, id, 
   modifiers and key, the constructor will call the method RegisterHotKey to 
   register the specified hotkey.

2. The enum KeyModifiers contains the supported modifiers, like CTRL, ALT and 
   SHIFT. The WinKey is also supported by the RegisterHotKey method, but 
   keyboard shortcuts that involve the WINDOWS key are reserved for use by 
   the operating system.

3. Design the UI in MainFrom which contains a textbox, 2 buttons and some 
   labels. 

   The MainFrom will handle the KeyDown event of the textbox and check 
   whether the pressed keys are valid, the keys that must be pressed in 
   combination with the key Ctrl, Shift or Alt, like Ctrl+Alt+T. 

   It will also handle the Click event of the buttons to define or free a 
   system-wide hotkey. When the form is closed, it will dispose the 
   HotKeyRegister instance.


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: RegisterHotKey Function
http://msdn.microsoft.com/en-us/library/ms646309.aspx

MSDN: UnregisterHotKey Function
http://msdn.microsoft.com/en-us/library/s646327.aspx

MSDN: Application.AddMessageFilter Method 
http://msdn.microsoft.com/en-us/library/system.windows.forms.application.addmessagefilter.aspx

IMessageFilter Interface
http://msdn.microsoft.com/en-us/library/system.windows.forms.imessagefilter.aspx


/////////////////////////////////////////////////////////////////////////////
