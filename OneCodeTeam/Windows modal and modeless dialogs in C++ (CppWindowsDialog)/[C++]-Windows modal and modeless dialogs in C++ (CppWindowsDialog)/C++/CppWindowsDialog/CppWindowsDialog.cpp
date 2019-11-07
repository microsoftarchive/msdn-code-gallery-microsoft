/****************************** Module Header ******************************\
* Module Name:  CppWindowsDialog.cpp
* Project:      CppWindowsDialog
* Copyright (c) Microsoft Corporation.
* 
* The CppWindowsDialog example demonstrates the skeleton of registered and 
* created window, based on a dialog resource defined in a resource script. It
* also shows the steps to show a modal or a modeless dialog.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes
#include "stdafx.h"
#include "CppWindowsDialog.h"
#include <windowsx.h>
#pragma endregion


#define MAX_LOADSTRING 100

// Global Variables:
HINSTANCE hInst;								// Current instance
TCHAR szTitle[MAX_LOADSTRING];					// The title bar text
TCHAR szWindowClass[MAX_LOADSTRING];			// The main window class name

// Forward declarations of functions included in this code module:
ATOM				MyRegisterClass(HINSTANCE hInstance);
BOOL				InitInstance(HINSTANCE, int);
LRESULT CALLBACK	WndProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	ModalDlgProc(HWND, UINT, WPARAM, LPARAM);
INT_PTR CALLBACK	ModelessDlgProc(HWND, UINT, WPARAM, LPARAM);


int APIENTRY _tWinMain(HINSTANCE hInstance,
                       HINSTANCE hPrevInstance,
                       LPTSTR    lpCmdLine,
                       int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    // TODO: Place code here.
    MSG msg;

    // Initialize global strings
    LoadString(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
    LoadString(hInstance, IDC_CPPWINDOWSDIALOG, szWindowClass, MAX_LOADSTRING);
    MyRegisterClass(hInstance);

    // Perform application initialization:
    if (!InitInstance (hInstance, nCmdShow))
    {
        return FALSE;
    }

    // Main message loop:
    while (GetMessage(&msg, NULL, 0, 0))
    {
        TranslateMessage(&msg);
        DispatchMessage(&msg);
    }

    return (int) msg.wParam;
}


//
//  FUNCTION: MyRegisterClass()
//
//  PURPOSE: Registers the window class.
//
//  COMMENTS:
//
//    This function and its usage are only necessary if you want this code
//    to be compatible with Win32 systems prior to the 'RegisterClassEx'
//    function that was added to Windows 95. It is important to call this function
//    so that the application will get 'well formed' small icons associated
//    with it.
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
    WNDCLASSEX wcex = { sizeof(wcex) };

    wcex.style          = CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc    = WndProc;
    wcex.cbClsExtra     = 0;
    wcex.cbWndExtra     = DLGWINDOWEXTRA;
    wcex.hInstance      = hInstance;
    wcex.hIcon          = LoadIcon(hInstance, MAKEINTRESOURCE(IDI_CPPWINDOWSDIALOG));
    wcex.hCursor        = LoadCursor(NULL, IDC_ARROW);
    wcex.hbrBackground  = (HBRUSH)(COLOR_BTNFACE+1);
    wcex.lpszMenuName   = 0;
    wcex.lpszClassName  = szWindowClass;
    wcex.hIconSm        = LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

    return RegisterClassEx(&wcex);
}

//
//   FUNCTION: InitInstance(HINSTANCE, int)
//
//   PURPOSE: Saves instance handle and creates main window
//
//   COMMENTS:
//   In this function, we save the instance handle in a global variable and 
//   create and display the main program window.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
    HWND hWnd;

    hInst = hInstance; // Store instance handle in our global variable

    hWnd = CreateDialog(hInst, MAKEINTRESOURCE(IDD_MAINDIALOG), 0, 0);
    if (!hWnd)
    {
        return FALSE;
    }

    ShowWindow(hWnd, nCmdShow);
    UpdateWindow(hWnd);

    return TRUE;
}

//
//   FUNCTION: OnCreate(HWND, LPCREATESTRUCT)
//
//   PURPOSE: Process the WM_CREATE message
//
BOOL OnCreate(HWND hWnd, LPCREATESTRUCT lpCreateStruct)
{
    return TRUE;
}

//
//   FUNCTION: OnCommand(HWND, int, HWND, UINT)
//
//   PURPOSE: Process the WM_COMMAND message
//
void OnCommand(HWND hWnd, int id, HWND hWndCtl, UINT codeNotify)
{
    switch (id)
    {
    case IDC_SHOWMODAL_BN:

        // 
        // Create a Modal Dialog Box.
        // 

        // You create a modal dialog box by using the DialogBox function. You 
        // must specify the identifier or name of a dialog box template 
        // resource and a pointer to the dialog box procedure. The DialogBox 
        // function loads the template, displays the dialog box, and  
        // processes all user input until the user closes the dialog box.
        if (DialogBox(hInst, MAKEINTRESOURCE(IDD_MODALDIALOG), hWnd, 
            ModalDlgProc) == IDOK)
        {
            // Complete the command;
            MessageBox(NULL, _T("OK"), _T("ModalDialog Result"), MB_OK);
        }
        else
        {
            // Cancel the command;
            MessageBox(NULL, _T("Cancel"), _T("ModalDialog Result"), MB_OK);
        }
        break;

    case IDC_SHOWMODELESS_BN:

        // 
        // Create a Modeless Dialog Box.
        // 

        // You create a modeless dialog box by using the CreateDialog 
        // function, specifying the identifier or name of a dialog box 
        // template resource and a pointer to the dialog box procedure. 
        // CreateDialog loads the template, creates the dialog box, and 
        // optionally displays it. Your application is responsible for 
        // retrieving and dispatching user input messages to the dialog box 
        // procedure.
        {
            HWND hDlg = CreateDialog(hInst, 
                MAKEINTRESOURCE(IDD_MODELESSDIALOG), 
                hWnd, ModelessDlgProc);
            if (hDlg)
                ShowWindow(hDlg, SW_SHOWNORMAL);
        }
        break;

    case IDOK:
        PostQuitMessage(0);
        break;
    }
}

//
//   FUNCTION: OnDestroy(HWND)
//
//   PURPOSE: Process the WM_DESTROY message
//
void OnDestroy(HWND hWnd)
{
    PostQuitMessage(0);
}

//
//  FUNCTION: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the main window.
//
//  WM_CREATE	- a window is being created
//  WM_COMMAND	- process the application commands
//  WM_DESTROY	- post a quit message and return
//
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_CREATE message in OnCreate
        // Because it is a window based on a dialog resource but NOT a dialog 
        // box it receives a WM_CREATE message and NOT a WM_INITDIALOG message.
        HANDLE_MSG (hWnd, WM_CREATE, OnCreate);

        // Handle the WM_COMMAND message in OnCommand
        HANDLE_MSG (hWnd, WM_COMMAND, OnCommand);

        // Handle the WM_DESTROY message in OnDestroy
        HANDLE_MSG (hWnd, WM_DESTROY, OnDestroy);

    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
}


//
//  FUNCTION: ModalDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the modal dialog.
//
//
INT_PTR CALLBACK ModalDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
    case WM_COMMAND:
        switch (LOWORD(wParam))
        {
        case IDOK:
        case IDCANCEL:
            return EndDialog(hWnd, wParam);
        }

    case WM_CLOSE:
        return EndDialog(hWnd, 0);

    default:
        return FALSE;	// Let system deal with the message
    }
}


//
//  FUNCTION: ModalessDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the modaless dialog.
//
//
INT_PTR CALLBACK ModelessDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
    case WM_COMMAND:
        switch (LOWORD(wParam))
        {
        case IDOK:
        case IDCANCEL:
            return DestroyWindow(hWnd);
        }

    case WM_CLOSE:
        return DestroyWindow(hWnd);

    default:
        return FALSE;	// Let system deal with the message
    }
}