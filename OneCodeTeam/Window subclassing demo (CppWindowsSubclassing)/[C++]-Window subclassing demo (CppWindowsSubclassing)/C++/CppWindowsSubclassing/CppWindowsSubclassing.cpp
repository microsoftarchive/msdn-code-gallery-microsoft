/****************************** Module Header ******************************\
Module Name:  CppWindowsSubclassing.cpp
Project:      CppWindowsSubclassing
Copyright (c) Microsoft Corporation.

If a control or a window does almost everything you want, but you need a few 
more features, you can change or add features to the original control by 
subclassing it. A subclass can have all the features of an existing class as 
well as any additional features you want to give it.

Two subclassing rules apply to subclassing in Win32.

1. Subclassing is allowed only within a process. An application cannot 
subclass a window or class that belongs to another process.

2. The subclassing process may not use the original window procedure address 
directly.

There are two approaches to window subclassing.

1. Subclassing Controls Prior to ComCtl32.dll version 6

The first is usable by most windows operating systems (Windows 2000, XP and 
later). You can put a control in a subclass and store user data within a 
control. You do this when you use versions of ComCtl32.dll prior to version 6 
which ships with Microsoft Windows XP. There are some disadvantages in 
creating subclasses with earlier versions of ComCtl32.dll.

a) The window procedure can only be replaced once.
b) It is difficult to remove a subclass after it is created.
c) Associating private data with a window is inefficient.
d) To call the next procedure in a subclass chain, you cannot cast the old 
window procedure and call it, you must call it using CallWindowProc.

To make a new control it is best to start with one of the Windows common 
controls and extend it to fit a particular need. To extend a control, create 
a control and replace its existing window procedure with a new one. The new 
procedure intercepts the control's messages and either acts on them or passes 
them to the original procedure for default processing. Use the SetWindowLong 
or SetWindowLongPtr function to replace the WNDPROC of the control.

2. Subclassing Controls Using ComCtl32.dll version 6

The second is only usable with a minimum operating system of Windows XP since 
it relies on ComCtl32.dll version 6. ComCtl32.dll version 6 supplied with 
Windows XP contains four functions that make creating subclasses easier and 
eliminate the disadvantages previously discussed. The new functions 
encapsulate the management involved with multiple sets of reference data, 
therefore developers can focus on programming features and not on managing 
subclasses. The subclassing functions are: 

SetWindowSubclass
GetWindowSubclass
RemoveWindowSubclass
DefSubclassProc

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes and Manifest Dependencies
#include <stdio.h>
#include <Windows.h>
#include <windowsx.h>
#include <strsafe.h>
#include "resource.h"

#include <Commctrl.h>
#pragma comment(lib, "Comctl32.lib")

// Enable Visual Style
#if defined _M_IX86
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='x86' publicKeyToken='6595b64144ccf1df' language='*'\"")
#elif defined _M_IA64
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='ia64' publicKeyToken='6595b64144ccf1df' language='*'\"")
#elif defined _M_X64
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='amd64' publicKeyToken='6595b64144ccf1df' language='*'\"")
#else
#pragma comment(linker,"/manifestdependency:\"type='win32' name='Microsoft.Windows.Common-Controls' version='6.0.0.0' processorArchitecture='*' publicKeyToken='6595b64144ccf1df' language='*'\"")
#endif
#pragma endregion


//
//   FUNCTION: ReportError(LPWSTR, DWORD)
//
//   PURPOSE: Display an error dialog for the failure of a certain function.
//
//   PARAMETERS:
//   * pszFunction - the name of the function that failed.
//   * dwError - the Win32 error code. 
//
void ReportError(LPCWSTR pszFunction, DWORD dwError = NO_ERROR)
{
    wchar_t szMessage[200];
    if (dwError == NO_ERROR)
    {
        StringCchPrintf(szMessage, ARRAYSIZE(szMessage), 
            L"%s failed", pszFunction);
    }
    else 
    {
        StringCchPrintf(szMessage, ARRAYSIZE(szMessage), 
            L"%s failed w/err 0x%08lx", pszFunction, dwError);
    }

    MessageBox(NULL, szMessage, L"Error", MB_ICONERROR);
}


#pragma region Subclassing Controls Prior to ComCtl32.dll version 6

// You can put a control in a subclass and store user data within a control. 
// You do this when you use versions of ComCtl32.dll prior to version 6 which 
// ships with Microsoft Windows XP. There are some disadvantages in creating 
// subclasses with earlier versions of ComCtl32.dll:
// 
//     1. The window procedure can only be replaced once.
//     2. It is difficult to remove a subclass after it is created.
//     3. Associating private data with a window is inefficient.
//     4. To call the next procedure in a subclass chain, you cannot cast the 
//        old window procedure and call it, you must call it by using /
//        CallWindowProc.
// 
// To make a new control it is best to start with one of the Windows common 
// controls and extend it to fit a particular need. To extend a control, 
// create a control and replace its existing window procedure with a new one. 
// The new procedure intercepts the control's messages and either acts on them 
// or passes them to the original procedure for default processing. Use the 
// SetWindowLong or SetWindowLongPtr function to replace the WNDPROC of the 
// control. 


//
//  FUNCTION: NewBtnProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  The new procedure that processes messages for the button control.
//
LRESULT CALLBACK NewBtnProc(HWND hButton, UINT message, WPARAM wParam, LPARAM lParam)
{
    // Retrieve the previously stored original button window procedure.
    WNDPROC OldBtnProc = reinterpret_cast<WNDPROC>(
        GetWindowLongPtr(hButton, GWLP_USERDATA));

    switch (message)
    {
    case WM_RBUTTONDOWN:

        // Mouse button right-click event handler
        MessageBox(hButton, L"NewBtnProc / WM_RBUTTONDOWN", 
            L"Subclass Example", MB_OK);

        // Stop the default message handler
        return TRUE;
        // [-or-]
        // Call the default message handler
        //return CallWindowProc(OldBtnProc, hButton, message, wParam, lParam);

    case WM_PAINT: // OwnerDraw
        // http://msdn.microsoft.com/en-us/library/dd145137.aspx
        {
            PAINTSTRUCT ps;
            HDC hDC = BeginPaint(hButton, &ps);

            // Do painting here
            FillRect(hDC, &ps.rcPaint, (HBRUSH)(COLOR_WINDOW+1));
            TextOut(hDC, 0, 0, L"Rich-click me!", 11);

            EndPaint(hButton, &ps);
        }
        return TRUE;

    case WM_NCDESTROY:

        // You must remove your window subclass before the window being 
        // subclassed is destroyed. 
        if (0 == SetWindowLongPtr(hButton, GWLP_WNDPROC, 
            reinterpret_cast<LONG_PTR>(OldBtnProc)))
        {
            ReportError(L"SetWindowLongPtr in handling WM_NCDESTROY",
                GetLastError());
        }
        return 0;

    default:
        // Call the default system handler for the control (button).
        return CallWindowProc(OldBtnProc, hButton, message, wParam, lParam);
    }
}

//
//   FUNCTION: OnSubclass(HWND)
//
//   PURPOSE: Subclass the button control. 
//
void OnSubclass(HWND hWnd)
{
    // Get the handle of the control to be subclassed, and subclass it by 
    // using SetWindowLongPtr with GWLP_WNDPROC or using the SubclassWindow 
    // macro defined in windowsx.h.
    HWND hButton = GetDlgItem(hWnd, IDC_BUTTON);
    WNDPROC OldBtnProc = reinterpret_cast<WNDPROC>(SetWindowLongPtr(
        hButton, GWLP_WNDPROC, reinterpret_cast<LONG_PTR>(NewBtnProc)));
    if (OldBtnProc == NULL)
    {
        ReportError(L"SetWindowLongPtr in OnSubclass", GetLastError());
        return;
    }

    // Store the original, default window procedure of the button as the 
    // button control's user data.
    SetWindowLongPtr(hButton, GWLP_USERDATA, reinterpret_cast<LONG_PTR>(OldBtnProc));

    // Invalidate the button control so that WM_PAINT is sent to it and the 
    // new paint of the button can be shown immediately.
    RECT rc;
    if (GetClientRect(hButton, &rc))
    {
        InvalidateRect(hButton, &rc, TRUE);
    }

    // Update the UI.
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SUBCLASS), FALSE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_UNSUBCLASS), TRUE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SAFESUBCLASS), FALSE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SAFEUNSUBCLASS), FALSE);
}

//
//   FUNCTION: OnUnsubclass(HWND)
//
//   PURPOSE: Unsubclass the button control. 
//   Do not assume that subclasses are added and removed in a purely static-
//   like manner. If you want to unsubclass and find that you are not the 
//   window procedure at the top of the chain you cannot safely unsubclass.
//   You will have to leave your subclass attached until it becomes safe to 
//   unsubclass. Until that time, you just have to keep passing the messages 
//   through to the previous procedure. 
//
void OnUnsubclass(HWND hWnd)
{
    // Get the handle of the control that was subclassed, and unsubclass it 
    // by retrieving the previously stored original button window procedure 
    // and replacing the current handler with the old one.
    HWND hButton = GetDlgItem(hWnd, IDC_BUTTON);
    LONG_PTR OldBtnProc = GetWindowLongPtr(hButton, GWLP_USERDATA);
    if (0 == SetWindowLongPtr(hButton, GWLP_WNDPROC, OldBtnProc))
    {
        ReportError(L"SetWindowLongPtr in OnUnsubclass", GetLastError());
    }

    // Invalidate the button control so that WM_PAINT is sent to it and the 
    // new paint of the button can be shown immediately.
    RECT rc;
    if (GetClientRect(hButton, &rc))
    {
        InvalidateRect(hButton, &rc, TRUE);
    }

    // Update the UI.
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SUBCLASS), TRUE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_UNSUBCLASS), FALSE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SAFESUBCLASS), TRUE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SAFEUNSUBCLASS), FALSE);
}

#pragma endregion


#pragma region Subclassing Controls Using ComCtl32.dll version 6

// ComCtl32.dll version 6 supplied with Windows XP contains four functions 
// that make creating subclasses easier and eliminate the disadvantages 
// previously discussed. The new functions encapsulate the management 
// involved with multiple sets of reference data, therefore the developer can 
// focus on programming features and not on managing subclasses. The 
// subclassing functions are: 
// 
//     SetWindowSubclass
//     GetWindowSubclass
//     RemoveWindowSubclass
//     DefSubclassProc


//
//  FUNCTION: NewSafeBtnProc(HWND, UINT, WPARAM, LPARAM, UINT_PTR, DWORD_PTR)
//
//  PURPOSE:  The new procedure that processes messages for the button control. 
//  Every time a message is received by the new window procedure, a subclass 
//  ID and reference data are included.
//
LRESULT CALLBACK NewSafeBtnProc(HWND hButton, 
                                UINT message, 
                                WPARAM wParam, 
                                LPARAM lParam, 
                                UINT_PTR uIdSubclass, 
                                DWORD_PTR dwRefData)
{
    switch (message)
    {
    case WM_RBUTTONDOWN:

        // Mouse button right-click event handler
        MessageBox(hButton, L"NewSafeBtnProc / WM_RBUTTONDOWN", 
            L"Subclass Example", MB_OK);

        // Stop the default message handler.
        return TRUE;
        // [-or-]
        // Call the default message handler.
        //return DefSubclassProc(hButton, message, wParam, lParam);

    case WM_PAINT: // OwnerDraw
        // http://msdn.microsoft.com/en-us/library/dd145137.aspx
        {
            PAINTSTRUCT ps;
            HDC hDC = BeginPaint(hButton, &ps);

            // Do painting here
            FillRect(hDC, &ps.rcPaint, reinterpret_cast<HBRUSH>(COLOR_WINDOW+1));
            TextOut(hDC, 0, 0, L"Rich-click me!", 11);

            EndPaint(hButton, &ps);
        }
        return TRUE;

    case WM_NCDESTROY:

        // You must remove your window subclass before the window being 
        // subclassed is destroyed. This is typically done either by removing 
        // the subclass once your temporary need has passed, or if you are 
        // installing a permanent subclass, by inserting a call to 
        // RemoveWindowSubclass inside the subclass procedure itself:

        if (!RemoveWindowSubclass(hButton, NewSafeBtnProc, uIdSubclass))
        {
            ReportError(L"RemoveWindowSubclass in handling WM_NCDESTROY");
        }

        return DefSubclassProc(hButton, message, wParam, lParam);

    default:
        return DefSubclassProc(hButton, message, wParam, lParam);
    }
}

//
//   FUNCTION: OnSafeSubclass(HWND)
//
//   PURPOSE: Safely subclass the button control. 
//
void OnSafeSubclass(HWND hWnd)
{
    // Get the handle of the control to be subclassed, and subclass it.
    HWND hButton = GetDlgItem(hWnd, IDC_BUTTON);
    UINT_PTR uIdSubclass = 0;
    if (!SetWindowSubclass(hButton, NewSafeBtnProc, uIdSubclass, 0))
    {
        ReportError(L"SetWindowSubclass in OnSafeSubclass");
        return;
    }

    // Invalidate the button control so that WM_PAINT is sent to it and the 
    // new paint of the button can be shown immediately
    RECT rc;
    if (GetClientRect(hButton, &rc))
    {
        InvalidateRect(hButton, &rc, TRUE);
    }

    // Update the UI.
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SUBCLASS), FALSE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_UNSUBCLASS), FALSE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SAFESUBCLASS), FALSE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SAFEUNSUBCLASS), TRUE);
}

//
//   FUNCTION: OnSafeUnsubclass(HWND)
//
//   PURPOSE: Safely unsubclass the button control. 
//
void OnSafeUnsubclass(HWND hWnd)
{
    // Get the handle of the control that was subclassed, and unsubclass it.
    HWND hButton = GetDlgItem(hWnd, IDC_BUTTON);
    UINT_PTR uIdSubclass = 0;
    if (!RemoveWindowSubclass(hButton, NewSafeBtnProc, uIdSubclass))
    {
        ReportError(L"RemoveWindowSubclass in OnSafeUnsubclass");
        return;
    }

    // Invalidate the button control so that WM_PAINT is sent to it and the 
    // new paint of the button can be shown immediately.
    RECT rc;
    if (GetClientRect(hButton, &rc))
    {
        InvalidateRect(hButton, &rc, TRUE);
    }

    // Update the UI.
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SUBCLASS), TRUE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_UNSUBCLASS), FALSE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SAFESUBCLASS), TRUE);
    Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_SAFEUNSUBCLASS), FALSE);
}

#pragma endregion


// 
//   FUNCTION: OnInitDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message. 
//
BOOL OnInitDialog(HWND hWnd, HWND hwndFocus, LPARAM lParam)
{
    return TRUE;
}


//
//   FUNCTION: OnCommand(HWND, int, HWND, UINT)
//
//   PURPOSE: Process the WM_COMMAND message
//
void OnCommand(HWND hWnd, int id, HWND hwndCtl, UINT codeNotify)
{
    switch (id)
    {
    case IDC_BUTTON_SUBCLASS:
        OnSubclass(hWnd);
        break;

    case IDC_BUTTON_UNSUBCLASS:
        OnUnsubclass(hWnd);
        break;

    case IDC_BUTTON_SAFESUBCLASS:
        OnSafeSubclass(hWnd);
        break;

    case IDC_BUTTON_SAFEUNSUBCLASS:
        OnSafeUnsubclass(hWnd);
        break;

    case IDCANCEL:
        EndDialog(hWnd, 0);
        break;
    }
}


//
//   FUNCTION: OnClose(HWND)
//
//   PURPOSE: Process the WM_CLOSE message
//
void OnClose(HWND hWnd)
{
    EndDialog(hWnd, 0);
}


//
//  FUNCTION: DialogProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the main dialog.
//
INT_PTR CALLBACK DialogProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitDialog);

        // Handle the WM_COMMAND message in OnCommand
        HANDLE_MSG (hWnd, WM_COMMAND, OnCommand);

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

    default:
        return FALSE;
    }
    return 0;
}


//
//  FUNCTION: wWinMain(HINSTANCE, HINSTANCE, LPWSTR, int)
//
//  PURPOSE:  The entry point of the application.
//
int APIENTRY wWinMain(HINSTANCE hInstance,
                      HINSTANCE hPrevInstance,
                      LPWSTR    lpCmdLine,
                      int       nCmdShow)
{
    return DialogBox(hInstance, MAKEINTRESOURCE(IDD_MAINDIALOG), NULL, DialogProc);
}