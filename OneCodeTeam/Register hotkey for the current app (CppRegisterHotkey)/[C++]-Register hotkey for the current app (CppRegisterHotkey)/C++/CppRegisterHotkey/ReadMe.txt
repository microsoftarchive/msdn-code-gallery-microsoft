=============================================================================
        WIN32 APPLICATION : CppRegisterHotkey Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

The sample demonstrates how to register and unregister a hotkey for the 
current application.


/////////////////////////////////////////////////////////////////////////////
Demo:

Step1. Build this project in VS2010. 

Step2. Run CppRegisterHotkey.exe.

Step3. Click the edit control in the window, and press Alt+Ctrl+T. You will 
       see "Alt,Control+T" displayed in the edit control, and the "Register" 
       button becomes enabled.

Step4. Click the "Register" button, then the edit control and the "Register" 
       button are disabled, and the "Unregister" button becomes enabled.

       If the hot key has already been registered, you will get an alert 
       "The hotkey is already in use.". You can try other hotkey e.g Alt+M.

Step5. Press Alt+Ctrl+T even when this application is not the active window. 
       The application will show up and be activated.


/////////////////////////////////////////////////////////////////////////////
Implementation:

A. Implementing the hotkey edit control.

The edit control traps the key-down messages, and captures the key 
combination. To trap the key-down messages, we subclass the edit control in 
OnInitDialog:

    // Subclass the hotkey edit control.
    HWND hHotKeyEdit = GetDlgItem(hWnd, IDC_EDIT_HOTKEY);
    UINT_PTR uIdSubclass = 0;
    if (!SetWindowSubclass(hHotKeyEdit, HotKeyEditProc, uIdSubclass, 0))
    {
        ReportError(L"SetWindowSubclass in OnInitDialog");
        return FALSE;
    }

We remove the window subclass before the edit control is destroyed by 
inserting a call to RemoveWindowSubclass inside the subclass procedure 
itself:

    LRESULT CALLBACK HotKeyEditProc(HWND hWnd, UINT message, WPARAM wParam, 
        LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData)
    {
        switch (message)
        {
        case WM_NCDESTROY:
            if (!RemoveWindowSubclass(hWnd, HotKeyEditProc, uIdSubclass))
            {
                ReportError(L"RemoveWindowSubclass in handling WM_NCDESTROY");
            }
            return DefSubclassProc(hWnd, message, wParam, lParam);
        ...
        }
    }

HotKeyEditProc is the new procedure that processes messages for the hotkey 
edit control. Every time a message is received by the new window procedure, a 
subclass ID and reference data are included.

HotKeyEditProc traps the WM_SYSKEYDOWN, WM_KEYDOWN, WM_SYSCHAR, WM_CHAR, 
WM_SYSKEYUP and WM_KEYUP messages and stops their default message handler 
by returning zero. The six messages are introduced in this MSDN article:
http://msdn.microsoft.com/en-us/library/gg153546.aspx.

    LRESULT CALLBACK HotKeyEditProc(HWND hWnd, UINT message, WPARAM wParam, 
        LPARAM lParam, UINT_PTR uIdSubclass, DWORD_PTR dwRefData)
    {
        switch (message)
        {
        case WM_SYSKEYDOWN:
        case WM_KEYDOWN:
            // Process the WM_KEYDOWN and WM_SYSKEYDOWN messages in 
            // OnHotKeyEditKeyDown. WM_SYSKEYDOWN is posted to the window with 
            // the keyboard focus when the user presses the F10 key (which 
            // activates the menu bar) or holds down the ALT key and then presses 
            // another key. It also occurs when no window currently has the 
            // keyboard focus.
            OnHotKeyEditKeyDown(hWnd, static_cast<UINT>(wParam), lParam);
            return 0;

        case WM_SYSCHAR:
        case WM_CHAR:
        case WM_SYSKEYUP:
        case WM_KEYUP:
            // Stop processing.
            return 0;

        ...
        }
    }

OnHotKeyEditKeyDown handles the WM_KEYDOWN and WM_SYSKEYDOWN messages posted 
to the edit control. It validates the hotkey combination. If no modifier key 
(Ctrl / Shift / Alt) is pressed, the function returns directly. If the key 
combination is valid (e.g. Ctrl, Alt+T), it displays the key combination in 
the edit control, and saves the key modifiers and the virtual-key code in 
the global variables: g_fsModifiers and g_vk.

---------------------------

B. Registering / unregistering the hotkey, and processing the WM_HOTKEY 
message.

The registration and unregistration of the hotkey are implemented in the 
OnCommand function. If the IDC_BUTTON_REGISTER button is clicked, it calls 
RegisterHotKey with the g_vk and g_fsModifiers values set previously to 
register the hotkey. 

The second parameter of RegisterHotKey is the hotkey ID. An application must 
specify an id value in the range 0x0000 through 0xBFFF (100 in this code 
sample). A shared DLL must specify a value in the range 0xC000 through 0xFFFF 
(the range returned by the GlobalAddAtom function). To avoid conflicts with 
hot-key identifiers defined by other shared DLLs, a DLL should use the 
GlobalAddAtom function to obtain the hot-key identifier.

RegisterHotKey fails if the keystrokes specified for the hot key have already 
been registered by another hot key. 

    case IDC_BUTTON_REGISTER:
        // Register the hotkey.
        if (RegisterHotKey(hWnd, g_idHotKey, g_fsModifiers, g_vk))
        {
            // Update the UI.
            Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_REGISTER), FALSE);
            Edit_Enable(GetDlgItem(hWnd, IDC_EDIT_HOTKEY), FALSE);
            Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_UNREGISTER), TRUE);

            g_fHotkeyRegistered = TRUE;
        }
        else
        {
            ReportError(L"RegisterHotKey", GetLastError());
        }
        break;

When a key is pressed, the system looks for a match against all hot keys. Upon 
finding a match, the system posts the WM_HOTKEY message to the message queue 
of the window with which the hot key is associated. If the hot key is not 
associated with a window, then the WM_HOTKEY message is posted to the thread 
associated with the hot key. In this code sample, when WM_HOTKEY is received, 
we activate the application main window and bring it into the front. 

    // Handle the WM_HOTKEY message in OnHotKey
    HANDLE_MSG (hWnd, WM_HOTKEY, OnHotKey);

    //
    //   FUNCTION: OnHotKey(HWND, int, UINT, UINT)
    //
    //   PURPOSE: Process the WM_HOTKEY message. The WM_HOTKEY message is posted 
    //   when the user presses a hot key registered by the RegisterHotKey 
    //   function. The message is placed at the top of the message queue 
    //   associated with the thread that registered the hot key. 
    //
    void OnHotKey(HWND hWnd, int idHotKey, UINT fuModifiers, UINT vk)
    {
        // If the window is minimized, restore it.
        if (IsIconic(hWnd))
        {
            ShowWindow(hWnd, SW_SHOWNORMAL);
        }

        // Bring the window into the front, and activate the window.
        SetForegroundWindow(hWnd);
    }

If the IDC_BUTTON_UNREGISTER button is clicked, the OnCommand function calls 
UnregisterHotKey to unregister the hotkey.  

    case IDC_BUTTON_UNREGISTER:
        // Unregister the hotkey.
        if (UnregisterHotKey(hWnd, g_idHotKey))
        {
            // Update the UI.
            Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_REGISTER), TRUE);
            Edit_Enable(GetDlgItem(hWnd, IDC_EDIT_HOTKEY), TRUE);
            Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_UNREGISTER), FALSE);

            g_fHotkeyRegistered = FALSE;
        }
        else
        {
            ReportError(L"UnregisterHotKey", GetLastError());
        }
        break;


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: RegisterHotKey Function
http://msdn.microsoft.com/en-us/library/ms646309.aspx

MSDN: UnregisterHotKey Function
http://msdn.microsoft.com/en-us/library/s646327.aspx


/////////////////////////////////////////////////////////////////////////////
