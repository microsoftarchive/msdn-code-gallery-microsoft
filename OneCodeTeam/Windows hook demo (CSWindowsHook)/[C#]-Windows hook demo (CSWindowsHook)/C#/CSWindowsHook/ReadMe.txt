============================================================================
    WINDOWS APPLICATION : CSWindowsHook Project Overview
============================================================================

/////////////////////////////////////////////////////////////////////////////
Use:

This example demonstrates how to set a hook that is specific to a thread as 
well as the global hook by using the low-level mouse and keyboard hooks in 
.NET. You can use hooks to monitor certain types of events. You can associate 
these events with a specific thread or with all the threads in the same 
desktop as the calling thread.


/////////////////////////////////////////////////////////////////////////////
Creation:

Step1. Create a new Windows Form application. Import the 
System.Runtime.InteropServices namespace.

Step2. Define the callback functions that will be invoked when system or  
thread receive mouse and keyboard actions.

Step3. Set/remove these hooks when the button is clicked.

	// Set mouse hook
	MouseHookProcedure = new HookProc(this.MouseHookProc);

	mouseHook = NativeMethods.SetWindowsHookEx(HookTypes.WH_MOUSE,
		MouseHookProcedure,
		(IntPtr)0,
		AppDomain.GetCurrentThreadId());
    
	// Remoce mouse hook           
	bool ret = NativeMethods.UnhookWindowsHookEx(mouseHook);

Step4. Remove all hooks when closing the main form.


/////////////////////////////////////////////////////////////////////////////
References:

How to set a Windows hook in Visual C# .NET
http://support.microsoft.com/kb/318804

Windows Hooks in the .NET Framework
http://msdn.microsoft.com/en-us/magazine/cc188966.aspx

Processing Global Mouse and Keyboard Hooks in C#
http://www.codeproject.com/KB/cs/globalhook.aspx

Global System Hooks in .NET
http://www.codeproject.com/KB/system/globalsystemhook.aspx


/////////////////////////////////////////////////////////////////////////////