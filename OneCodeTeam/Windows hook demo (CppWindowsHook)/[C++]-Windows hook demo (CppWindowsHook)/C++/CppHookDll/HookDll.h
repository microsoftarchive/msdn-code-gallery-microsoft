/****************************** Module Header ******************************\
* Module Name:	HookDll.h
* Project:		CppHookDll
* Copyright (c) Microsoft Corporation.
* 
* This sample exports hook procedure that is used by CppWindowsHook.

* More information exporting data, methods and classes, please see the 
* CppDllExport.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* History:
* * 5/01/2009 11:04 PM RongChun Zhang Created
\***************************************************************************/


#ifdef HOOKDLL_EXPORTS
#define HOOKDLL_API __declspec(dllexport)
#else
#define HOOKDLL_API __declspec(dllimport)
#endif

// Define a private messages. This message is sent when key is down/up
#define WM_KEYSTROKE (WM_USER + 101)
#define WM_KEYINPUT  (WM_USER + 102)

//exported functions.
BOOL HOOKDLL_API WINAPI SetKeyboardHook(BOOL bInstall, 
										DWORD dwThreadId = 0, 
										HWND hWndCaller = NULL);

BOOL HOOKDLL_API WINAPI SetLowKeyboardHook(BOOL bInstall, 
										DWORD dwThreadId = 0, 
										HWND hWndCaller = NULL);