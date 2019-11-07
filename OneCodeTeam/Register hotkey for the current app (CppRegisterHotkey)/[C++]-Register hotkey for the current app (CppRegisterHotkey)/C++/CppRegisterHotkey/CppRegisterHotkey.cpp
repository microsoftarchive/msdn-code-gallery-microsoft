/****************************** Module Header ******************************\
Module Name:  CppRegisterHotkey.cpp
Project:      CppRegisterHotkey
Copyright (c) Microsoft Corporation.

The sample demonstrates how to register and unregister a hotkey for the 
current application.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
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

#include <CommCtrl.h>
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
	else if (dwError == ERROR_HOTKEY_ALREADY_REGISTERED)
	{
		StringCchPrintf(szMessage, ARRAYSIZE(szMessage), 
			L"The hotkey is in use");
	}
	else 
	{
		StringCchPrintf(szMessage, ARRAYSIZE(szMessage), 
			L"%s failed w/err 0x%08lx", pszFunction, dwError);
	}

	MessageBox(NULL, szMessage, L"Error", MB_ICONERROR);
}


UINT g_vk = 0;
UINT g_fsModifiers = 0;
BOOL g_fHotkeyRegistered = FALSE;
const int g_idHotKey = 100;


#pragma region Hotkey Edit Control

// 
//   FUNCTION: OnHotKeyEditKeyDown(HWND, UINT, LPARAM)
//
//   PURPOSE:  The function handles the WM_KEYDOWN message of the 
//   IDC_EDIT_HOTKEY edit control. It checks and displays the hotkey 
//   combination that user inputs.
//
void OnHotKeyEditKeyDown(HWND hWnd, UINT vk, LPARAM lParam)
{
	// Check whether the modifier keys are pressed.
	UINT fsModifiers = 0;
	wchar_t szResult[250] = {};

	if (GetKeyState(VK_MENU /*Alt*/) & 0x8000)
	{
		fsModifiers |= MOD_ALT;
		StringCchCat(szResult, ARRAYSIZE(szResult), L"Alt");
	}
	if (GetKeyState(VK_CONTROL) & 0x8000)
	{
		StringCchCat(szResult, ARRAYSIZE(szResult), 
			(fsModifiers != 0) ? L", Control" : L"Control");
		fsModifiers |= MOD_CONTROL;
	}
	if (GetKeyState(VK_SHIFT) & 0x8000)
	{
		StringCchCat(szResult, ARRAYSIZE(szResult), 
			(fsModifiers != 0) ? L", Shift" : L"Shift");
		fsModifiers |= MOD_SHIFT;
	}

	// If no modifier key was pressed, return directly.
	if (fsModifiers == 0)
	{
		return;
	}

	// Check the virtual-key code. If the pressed key is Alt (Menu) or Ctrl 
	// or Shift, ignore the input and return directly.
	if (vk == 0 || vk == VK_MENU || vk == VK_CONTROL || vk == VK_SHIFT)
	{
		return;
	}

	// Get the name of the virtual-key.
	wchar_t szTemp[100] = {};
	wchar_t szKeyName[100] = {};
	int cchKeyName = GetKeyNameText(lParam, szTemp, ARRAYSIZE(szTemp));
	if (cchKeyName == 0)
	{
		ReportError(L"GetKeyNameText in OnHotKeyEditKeyDown", GetLastError());
		return;
	}
	StringCchCopyN(szKeyName, ARRAYSIZE(szKeyName), szTemp, cchKeyName);
	StringCchCat(szResult, ARRAYSIZE(szResult), L"+");
	StringCchCat(szResult, ARRAYSIZE(szResult), szKeyName);

	// Update the UI and store the value of vk and fsModifiers.
	Edit_SetText(hWnd, szResult);
	g_vk = vk;
	g_fsModifiers = fsModifiers;
	Button_Enable(GetDlgItem(GetParent(hWnd), IDC_BUTTON_REGISTER), TRUE);
}


// 
//   FUNCTION: HotKeyEditProc(HWND, UINT, WPARAM, LPARAM, UINT_PTR, DWORD_PTR)
//
//   PURPOSE:  The new procedure that processes messages for the hotkey edit 
//   control. Every time a message is received by the new window procedure, a 
//   subclass ID and reference data are included.
//
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

	case WM_NCDESTROY:

		// You must remove your window subclass before the window being 
		// subclassed is destroyed. This is typically done either by removing 
		// the subclass once your temporary need has passed, or if you are 
		// installing a permanent subclass, by inserting a call to 
		// RemoveWindowSubclass inside the subclass procedure itself:

		if (!RemoveWindowSubclass(hWnd, HotKeyEditProc, uIdSubclass))
		{
			ReportError(L"RemoveWindowSubclass in handling WM_NCDESTROY");
		}

		return DefSubclassProc(hWnd, message, wParam, lParam);

	default:
		return DefSubclassProc(hWnd, message, wParam, lParam);
	}
}

#pragma endregion


// 
//   FUNCTION: OnInitDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message. 
//
BOOL OnInitDialog(HWND hWnd, HWND hwndFocus, LPARAM lParam)
{
	// Subclass the hotkey edit control.
	HWND hHotKeyEdit = GetDlgItem(hWnd, IDC_EDIT_HOTKEY);
	UINT_PTR uIdSubclass = 0;
	if (!SetWindowSubclass(hHotKeyEdit, HotKeyEditProc, uIdSubclass, 0))
	{
		ReportError(L"SetWindowSubclass in OnInitDialog");
		return FALSE;
	}

	// Disable the Register and Unregister buttons.
	Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_REGISTER), FALSE);
	Button_Enable(GetDlgItem(hWnd, IDC_BUTTON_UNREGISTER), FALSE);

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
	}
}


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
//   FUNCTION: OnNCDestroy(HWND)
//
//   PURPOSE: Process the WM_NCDESTROY message
//
void OnNCDestroy(HWND hWnd)
{
	if (g_fHotkeyRegistered)
	{
		// Unregister the hotkey.
		if (UnregisterHotKey(hWnd, g_idHotKey))
		{
			g_fHotkeyRegistered = FALSE;
		}
		else
		{
			ReportError(L"UnregisterHotKey", GetLastError());
		}
	}
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

		// Handle the WM_HOTKEY message in OnHotKey
		HANDLE_MSG (hWnd, WM_HOTKEY, OnHotKey);

		// Handle the WM_CLOSE message in OnClose
		HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

		// Handle the WM_NCDESTROY message in OnNCDestroy
		HANDLE_MSG (hWnd, WM_NCDESTROY, OnNCDestroy);

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