========================================================================
    WIN32 APPLICATION : CppWindowsHook Project Overview
========================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

A hook is a point in the system message-handling mechanism where an application 
can install a subroutine to monitor the message traffic in the system and 
process certain types of messages before they reach the target window procedure.
There are two types of Hooks - Thread specific hooks and global hooks. A 
thread specific hook is associated with particular thread only. While a global 
hook is associated with all threads in the same desktop as the calling thread.
You must place a global hook procedure in a DLL(CppHookDll) separate from the 
application installing the hook procedure. 


/////////////////////////////////////////////////////////////////////////////
Project Relation:

CppWindowsHook -> CppHookDll
CppHookDll posts a private message(WM_KEYSTROKE or WM_KEYINPUT) to 
CppWindowsHook's main window(CppWindowsHookDlg).WM_KEYINPUT is sent when a 
real key is stroked.


/////////////////////////////////////////////////////////////////////////////
Code Logic:

1. Install a thread or global hook.

2. The hook procedure post a private message(WM_KEYSTROKE or WM_KEYINPUT) to the
   main Window(CppWindowsHookDlg). And WM_KEYINPUT is sent when a real key is 
   stroked if you setup a WH_KEYBOARD_LL hook.

3. Log the information when main windows receive the private message.


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: About Hooks
http://msdn.microsoft.com/en-us/library/ms644959(VS.85).aspx

MSDN: Using Hooks 
http://msdn.microsoft.com/en-us/library/ms644960(VS.85).aspx

Hooking the Keyboard 
http://www.codeguru.com/cpp/w-p/system/keyboard/article.php/c5699


/////////////////////////////////////////////////////////////////////////////