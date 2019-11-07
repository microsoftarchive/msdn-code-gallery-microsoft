/****************************** Module Header ******************************\
Module Name:  CppReceiveWM_COPYDATA.cpp
Project:      CppReceiveWM_COPYDATA
Copyright (c) Microsoft Corporation.

Inter-process Communication (IPC) based on the Windows message WM_COPYDATA is 
a mechanism for exchanging data among Windows applications in the local 
machine. The receiving application must be a Windows application. The data 
being passed must not contain pointers or other references to objects not 
accessible to the application receiving the data. While WM_COPYDATA is being 
sent, the referenced data must not be changed by another thread of the 
sending process. The receiving application should consider the data read-only. 
If the receiving application must access the data after SendMessage returns, 
it needs to copy the data into a local buffer.

This code sample demonstrates receiving a custom data structure (MY_STRUCT) 
from the sending application (CppSendWM_COPYDATA) by handling WM_COPYDATA 
messages.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes and Manifest Dependencies
#include <stdio.h>
#include <windows.h>
#include <windowsx.h>
#include "Resource.h"

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


// The struct that carries the data.
struct MY_STRUCT
{
    int Number;

    wchar_t Message[256];
};


//
//   FUNCTION: OnCopyData(HWND, HWND, PCOPYDATASTRUCT)
//
//   PURPOSE: Process the WM_COPYDATA message
//
//   PARAMETERS:
//   * hWnd - Handle to the current window.
//   * hwndFrom - Handle to the window passing the data. 
//   * pcds - Pointer to a COPYDATASTRUCT structure that contains the data 
//     that was passed. 
//
BOOL OnCopyData(HWND hWnd, HWND hwndFrom, PCOPYDATASTRUCT pcds)
{
    MY_STRUCT myStruct;

    // If the size matches
    if (pcds->cbData == sizeof(myStruct))
    {
        // The receiving application should consider the data (pcds->lpData) 
        // read-only. The pcds parameter is valid only during the processing 
        // of the message. The receiving application should not free the 
        // memory referenced by pcds. If the receiving application must 
        // access the data after SendMessage returns, it must copy the data 
        // into a local buffer. 
        memcpy_s(&myStruct, sizeof(myStruct), pcds->lpData, pcds->cbData);

        // Display the MY_STRUCT value in the window.
        SetDlgItemInt(hWnd, IDC_NUMBER_STATIC, myStruct.Number, TRUE);
        SetDlgItemText(hWnd, IDC_MESSAGE_STATIC, myStruct.Message);
    }

    return TRUE;
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
        // Handle the WM_COPYDATA message in OnCopyData
        HANDLE_MSG (hWnd, WM_COPYDATA, OnCopyData);

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