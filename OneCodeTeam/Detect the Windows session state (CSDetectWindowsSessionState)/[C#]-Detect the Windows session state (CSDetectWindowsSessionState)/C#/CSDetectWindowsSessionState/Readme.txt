================================================================================
       Windows APPLICATION: CSDetectWindowsSessionState Overview                        
===============================================================================
/////////////////////////////////////////////////////////////////////////////
Summary:
The sample demonstrates how to detect the Windows session state.

1 To detect whether the current session is locked, you can use the extern method 
  OpenInputDesktop in User32.dll. This method is used to open the desktop that 
  receives user input. If this operation failed, it means that the session is locked.

2 If a user locked / unlocked the current session, you can use the SessionSwitch event
  to monitor it. 

  Microsoft.Win32.SystemEvents contains a SessionSwitch event. This event occurs
  when the user session switches, e.g. when the session is locked / unlocked or 
  when a user has logged on to a session, and so on. If you register this event, 
  you can get the session state when the state changed.


Note:
1 If UAC popups a secure desktop, this method may also fail. There is no API for
  differentiating between Locked Desktop and UAC Secure Desktop.

2 The SessionSwitch event is fired only in the current session. If another user locks 
  or unlocks his session on the same machine, you won't see this event in your own 
  session.

////////////////////////////////////////////////////////////////////////////////
Demo:

Step1. Build this project in VS2010. 

Step2. Run CSDetectWindowsSessionState.exe.

Step3. Check the "Enable a timer to detect the session state every 5 seconds", and then
       you will see "Current State: <state> Time: <time>" on the top of the UI, 
	   and a new record in the list box every 5 seconds. 

Step4. Uncheck the "Enable a timer to detect the session state every 5 seconds". 

Step5. Press Win+L to lock the PC, then unlock it. You will see 2 new records in the
       list box. One is "<time> SessionLock <Occurred>" and another one is
	    "<time> SessionUnlock <Occurred>".


/////////////////////////////////////////////////////////////////////////////
Code Logic:

1. Design a class WindowsSession. When an instance of this class is initialized, register
   the SystemEvents.SessionSwitch event.
    
2. If the SystemEvents.SessionSwitch event occurs, then raise a StateChanged event.

3. The class WindowsSession also wraps 2 extern methods OpenInputDesktop and CloseDesktop.
   If the method OpenInputDesktop fails, the return value is IntPtr.Zero, which means that
   the current session is locked.
   
4. Design the UI in MainForm which contains a list box. 

   The MainForm will handle the StateChanged event of a WindowsSession instance. If the
   event occurred, then add a new record in the list box.  

   The MainForm also contains a timer that could detect the WindowsSessionState every 5
   seconds.

/////////////////////////////////////////////////////////////////////////////
References:

SystemEvents.SessionSwitch Event
http://msdn.microsoft.com/en-us/library/microsoft.win32.systemevents.sessionswitch.aspx

OpenInputDesktop Function
http://msdn.microsoft.com/en-us/library/ms684309(VS.85).aspx

CloseDesktop Function
http://msdn.microsoft.com/en-us/library/ms682024(VS.85).aspx

Desktop Security and Access Rights
http://msdn.microsoft.com/en-us/library/ms682575(VS.85).aspx

/////////////////////////////////////////////////////////////////////////////
