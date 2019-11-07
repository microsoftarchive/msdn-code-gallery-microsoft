/****************************** Module Header ******************************\
Module Name:  CppSendWM_COPYDATA.cpp
Project:      CppSendWM_COPYDATA
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

This code sample demonstrates sending a custom data structure (MY_STRUCT) to 
the receiving Windows application (CppReceiveWM_COPYDATA) by using 
SendMessage(WM_COPYDATA). If the data structure fails to be passed, the 
application displays the error code for diagnostics. A typical error code is 
0x5 (Access is denied) caused by User Interface Privilege Isolation (UIPI). 
UIPI prevents processes from sending selected window messages and other USER 
APIs to processes running with higher integrity. When the receiving 
application (CppReceiveWM_COPYDATA) runs at an integrity level higher than 
this sending application, you will see the "SendMessage(WM_COPYDATA) failed 
w/err 0x00000005" error message.

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
//   FUNCTION: ReportError(LPWSTR, DWORD)
//
//   PURPOSE: Display an error dialog for the failure of a certain function.
//
//   PARAMETERS:
//   * pszFunction - the name of the function that failed.
//   * dwError - the Win32 error code. Its default value is the calling 
//   thread's last-error code value.
//
//   NOTE: The failing function must be immediately followed by the call of 
//   ReportError if you do not explicitly specify the dwError parameter of 
//   ReportError. This is to ensure that the calling thread's last-error code 
//   value is not overwritten by any calls of API between the failing 
//   function and ReportError.
//
void ReportError(LPCWSTR pszFunction, DWORD dwError = GetLastError())
{
    wchar_t szMessage[200];
    if (-1 != swprintf_s(szMessage, ARRAYSIZE(szMessage), 
        L"%s failed w/err 0x%08lx", pszFunction, dwError))
    {
        MessageBox(NULL, szMessage, L"Error", MB_ICONERROR);
    }
}


//
//   FUNCTION: OnCommand(HWND, int, HWND, UINT)
//
//   PURPOSE: Process the WM_COMMAND message
//
void OnCommand(HWND hWnd, int id, HWND hwndCtl, UINT codeNotify)
{
    if (id == IDC_SENDMSG_BUTTON)
    {
        // Find the target window handle.
        HWND hTargetWnd = FindWindow(NULL, L"CppReceiveWM_COPYDATA");
        if (hTargetWnd == NULL)
        {
            MessageBox(hWnd, L"Unable to find the \"CppReceiveWM_COPYDATA\" window", 
                L"Error", MB_ICONERROR);
            return;
        }

        // Prepare the COPYDATASTRUCT struct with the data to be sent.
        MY_STRUCT myStruct;

        BOOL fTranslated = FALSE;
        myStruct.Number = GetDlgItemInt(hWnd, IDC_NUMBER_EDIT, &fTranslated, TRUE);
        if (!fTranslated)
        {
            MessageBox(hWnd, L"Invalid value of Number!", L"Error", MB_ICONERROR);
            return;
        }

        GetDlgItemText(hWnd, IDC_MESSAGE_EDIT, myStruct.Message, 
            ARRAYSIZE(myStruct.Message));

        COPYDATASTRUCT cds;
        cds.cbData = sizeof(myStruct);
        cds.lpData = &myStruct;

        // Send the COPYDATASTRUCT struct through the WM_COPYDATA message to 
        // the receiving window. (The application must use SendMessage, 
        // instead of PostMessage to send WM_COPYDATA because the receiving 
        // application must accept while it is guaranteed to be valid.)
        SendMessage(hTargetWnd, WM_COPYDATA, reinterpret_cast<WPARAM>(hWnd), 
            reinterpret_cast<LPARAM>(&cds));

        DWORD dwError = GetLastError();
        if (dwError != NO_ERROR)
        {
            ReportError(L"SendMessage(WM_COPYDATA)", dwError);
        }
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