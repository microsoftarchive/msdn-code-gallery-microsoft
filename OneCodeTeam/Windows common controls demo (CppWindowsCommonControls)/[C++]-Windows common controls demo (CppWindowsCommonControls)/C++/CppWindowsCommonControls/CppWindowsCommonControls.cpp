/******************************** Module Header ********************************\
Module Name:  CppWindowsCommonControls.cpp
Project:      CppWindowsCommonControls
Copyright (c) Microsoft Corporation.

CppWindowsCommonControls contains simple examples of how to create common 
controls defined in comctl32.dll. The controls include Animation, ComboBoxEx, 
Updown, Header, MonthCal, DateTimePick, ListView, TreeView, Tab, Tooltip, IP 
Address, Statusbar, Progress Bar, Toolbar, Trackbar, and SysLink.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*******************************************************************************/

#pragma region Includes and Manifest Dependencies
#include <stdio.h>
#include <windows.h>
#include <windowsx.h>
#include "Resource.h"
#include <assert.h>

#include <commctrl.h>
#pragma comment(lib, "comctl32.lib")

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


HINSTANCE g_hInst;  // The handle to the instance of the current module


//
//   FUNCTION: OnClose(HWND)
//
//   PURPOSE: Process the WM_CLOSE message
//
void OnClose(HWND hWnd)
{
    EndDialog(hWnd, 0);
}


#pragma region Animation

// MSDN: Animation Control
// http://msdn.microsoft.com/en-us/library/bb761881.aspx

#define IDC_ANIMATION		990


//
//   FUNCTION: OnInitAnimationDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitAnimationDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register animation control class.
    INITCOMMONCONTROLSEX iccx = { sizeof(iccx) };
    iccx.dwICC = ICC_ANIMATE_CLASS;
    if (!InitCommonControlsEx(&iccx))
    {
        return FALSE;
    }

    // Create the animation control.
    RECT rc = { 20, 20, 280, 60 };
    HWND hAnimate = CreateWindowEx(0, ANIMATE_CLASS, 0, 
        ACS_TIMER | ACS_AUTOPLAY | ACS_TRANSPARENT | WS_CHILD | WS_VISIBLE, 
        rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, reinterpret_cast<HMENU>(IDC_ANIMATION), g_hInst, 0);
    if (hAnimate == NULL)
    {
        return FALSE;
    }

    // Open the AVI clip and display its first frame in the animation control.
    if (0 == SendMessage(hAnimate, ACM_OPEN, static_cast<WPARAM>(0), 
        reinterpret_cast<LPARAM>(MAKEINTRESOURCE(IDR_UPLOAD_AVI))))
    {
        return FALSE;
    }

    // Plays the AVI clip in the animation control.
    if (0 == SendMessage(hAnimate, ACM_PLAY, static_cast<WPARAM>(-1), 
        MAKELONG(/*from frame*/0, /*to frame*/-1)))
    {
        return FALSE;
    }

    return TRUE;
}


//
//  FUNCTION: AnimationDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the Animation control dialog.
//
INT_PTR CALLBACK AnimationDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitAnimationDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitAnimationDialog);

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region ComboBoxEx

// MSDN: ComboBoxEx Control Reference
// http://msdn.microsoft.com/en-us/library/bb775740.aspx

#define IDC_COMBOBOXEX		1990


//
//   FUNCTION: OnInitComboBoxExDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitComboBoxExDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register ComboBoxEx control class.
    INITCOMMONCONTROLSEX iccx = { sizeof(iccx) };
    iccx.dwICC = ICC_USEREX_CLASSES;
    if (!InitCommonControlsEx(&iccx))
    {
        return FALSE;
    }

    // Create the ComboBoxEx control.
    RECT rc = { 20, 20, 280, 100 };
    HWND hComboEx = CreateWindowEx(0, WC_COMBOBOXEX, 0, CBS_DROPDOWN | WS_CHILD | 
        WS_VISIBLE, rc.left, rc.top, rc.right, rc.bottom, hWnd, 
        reinterpret_cast<HMENU>(IDC_COMBOBOXEX), g_hInst, 0);
    if (hComboEx == NULL)
    {
        return FALSE;
    }

    // Create an image list to hold icons for use by the ComboBoxEx control.
    // The image list is destroyed in OnComboBoxExClose.
    PCWSTR lpszResID[] = { IDI_APPLICATION, IDI_INFORMATION, IDI_QUESTION };
    int nIconCount = ARRAYSIZE(lpszResID);
    HIMAGELIST hImageList = ImageList_Create(16, 16, ILC_MASK | ILC_COLOR32, 
        nIconCount, 0);
    if (hImageList == NULL)
    {
        return FALSE;
    }

    for (int i = 0; i < nIconCount; i++)
    {
        HANDLE hIcon = LoadImage(NULL, lpszResID[i], IMAGE_ICON, 0, 0, LR_SHARED);
        if (hIcon != NULL)
        {
            ImageList_AddIcon(hImageList, static_cast<HICON>(hIcon));
        }
    }

    // Associate the image list with the ComboBoxEx common control
    SendMessage(hComboEx, CBEM_SETIMAGELIST, 0, 
        reinterpret_cast<LPARAM>(hImageList));

    // Add nIconCount items to the ComboBoxEx common control
    wchar_t szText[200];
    for (int i = 0; i < nIconCount; i++)
    {
        COMBOBOXEXITEM cbei = {0};
        cbei.mask = CBEIF_IMAGE | CBEIF_TEXT | CBEIF_SELECTEDIMAGE;
        cbei.iItem = -1;
        swprintf_s(szText, 200, L"Item  %d", i);
        cbei.pszText = szText;
        cbei.iImage = i;
        cbei.iSelectedImage = i;
        SendMessage(hComboEx, CBEM_INSERTITEM, 0, reinterpret_cast<LPARAM>(&cbei));
    }

    // Store the image list handle as the user data of the window so that it
    // can be destroyed when the window is destroyed. (See OnComboBoxExClose)
    SetWindowLongPtr(hWnd, GWLP_USERDATA, reinterpret_cast<LONG_PTR>(hImageList));

    return TRUE;
}

//
//   FUNCTION: OnComboBoxExClose(HWND)
//
//   PURPOSE: Process the WM_CLOSE message
//
void OnComboBoxExClose(HWND hWnd)
{
    // Destroy the image list associated with the ComboBoxEx control
    ImageList_Destroy((HIMAGELIST)GetWindowLongPtr(hWnd, GWLP_USERDATA));

    DestroyWindow(hWnd);
}

//
//  FUNCTION: ComboBoxExDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the ComboBoxEx control dialog.
//
//
INT_PTR CALLBACK ComboBoxExDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitComboBoxExDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitComboBoxExDialog);

        // Handle the WM_CLOSE message in OnComboBoxExClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnComboBoxExClose);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region Updown
// MSDN: Up-Down Control
// http://msdn.microsoft.com/en-us/library/bb759880.aspx

#define IDC_EDIT		2990
#define IDC_UPDOWN		2991

//
//   FUNCTION: OnInitUpdownDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitUpdownDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register Updown control class
    INITCOMMONCONTROLSEX iccx;
    iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
    iccx.dwICC = ICC_UPDOWN_CLASS;
    if (!InitCommonControlsEx(&iccx))
        return FALSE;

    // Create an Edit control
    RECT rc = { 20, 20, 100, 24 };
    HWND hEdit = CreateWindowEx(WS_EX_CLIENTEDGE, L"EDIT", 0, 
        WS_CHILD | WS_VISIBLE, rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, (HMENU)IDC_EDIT, g_hInst, 0);

    // Create the ComboBoxEx control
    SetRect(&rc, 20, 60, 180, 20);
    HWND hUpdown = CreateWindowEx(0, UPDOWN_CLASS, 0, 
        UDS_ALIGNRIGHT | UDS_SETBUDDYINT | UDS_WRAP | WS_CHILD | WS_VISIBLE, 
        rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, (HMENU)IDC_UPDOWN, g_hInst, 0);

    // Explicitly attach the Updown control to its 'buddy' edit control
    SendMessage(hUpdown, UDM_SETBUDDY, (WPARAM)hEdit, 0);

    return TRUE;
}

//
//  FUNCTION: UpdownDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the Updown control dialog.
//
//
INT_PTR CALLBACK UpdownDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitUpdownDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitUpdownDialog);

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region Header
// MSDN: Header Control
// http://msdn.microsoft.com/en-us/library/bb775239.aspx

#define IDC_HEADER			3990

//
//   FUNCTION: OnHeaderSize(HWND, UINT, int, int)
//
//   PURPOSE: Process the WM_SIZE message
//
void OnHeaderSize(HWND hWnd, UINT state, int cx, int cy)
{
    // Adjust the position and the layout of the Header control
    RECT rc = { 0, 0, cx, cy };
    WINDOWPOS wp = { 0 };
    HDLAYOUT hdl = { &rc, &wp };

    // Get the header control handle which has been previously stored in the 
    // user data associated with the parent window.
    HWND hHeader = (HWND)GetWindowLongPtr(hWnd, GWLP_USERDATA);

    // hdl.wp retrieves information used to set the size and postion of the  
    // header control within the target rectangle of the parent window. 
    SendMessage(hHeader, HDM_LAYOUT, 0, (LPARAM)&hdl);

    // Set the size and position of the header control based on hdl.wp.
    SetWindowPos(hHeader, wp.hwndInsertAfter, 
        wp.x, wp.y, wp.cx, wp.cy + 8, wp.flags);
}

//
//   FUNCTION: OnInitHeaderDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitHeaderDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register Header control class
    INITCOMMONCONTROLSEX iccx;
    iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
    iccx.dwICC = ICC_WIN95_CLASSES;
    if (!InitCommonControlsEx(&iccx))
        return FALSE;

    // Create the Header control
    RECT rc = { 0, 0, 0, 0 };
    HWND hHeader = CreateWindowEx(0, WC_HEADER, 0, 
        HDS_BUTTONS | WS_CHILD | WS_VISIBLE, 
        rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, (HMENU)IDC_HEADER, g_hInst, 0);

    // Store the header control handle as the user data associated with the 
    // parent window so that it can be retrieved for later use.
    SetWindowLongPtr(hWnd, GWLP_USERDATA, (LONG_PTR)hHeader);

    // Resize the header control
    GetClientRect(hWnd, &rc);
    OnHeaderSize(hWnd, 0, rc.right, rc.bottom);

    // Set the font for the header common control
    SendMessage(hHeader, WM_SETFONT, (WPARAM)GetStockObject(DEFAULT_GUI_FONT), 0);

    // Add 4 Header items
    TCHAR szText[200];
    for (UINT i = 0; i < 4; i++)
    {
        HDITEM hdi = {0};
        hdi.mask = HDI_WIDTH | HDI_FORMAT | HDI_TEXT;
        hdi.cxy = rc.right / 4;
        hdi.fmt = HDF_CENTER;
        swprintf_s(szText, 200, L"Header  %d", i);
        hdi.pszText = szText;
        hdi.cchTextMax = 200;

        SendMessage(hHeader, HDM_INSERTITEM, i, (LPARAM)&hdi);
    }

    return TRUE;
}

//
//  FUNCTION: HeaderDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the Header control dialog.
//
//
INT_PTR CALLBACK HeaderDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitHeaderDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitHeaderDialog);

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

        // Handle the WM_SIZE message in OnHeaderSize
        HANDLE_MSG (hWnd, WM_SIZE, OnHeaderSize);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region MonthCal
// MSDN: Month Calendar Control Reference
// http://msdn.microsoft.com/en-us/library/bb760917.aspx

#define IDC_MONTHCAL		4990

//
//   FUNCTION: OnInitMonthCalDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitMonthCalDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register MonthCal control class
    INITCOMMONCONTROLSEX iccx;
    iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
    iccx.dwICC = ICC_DATE_CLASSES;
    if (!InitCommonControlsEx(&iccx))
        return FALSE;

    // Create the Month Calendar control
    RECT rc = { 20, 20, 280, 200 };
    HWND hMonthCal = CreateWindowEx(0, MONTHCAL_CLASS, 0, 
        WS_CHILD | WS_VISIBLE, rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, (HMENU)IDC_MONTHCAL, g_hInst, 0);

    return TRUE;
}

//
//  FUNCTION: MonthCalDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the MonthCal control dialog.
//
//
INT_PTR CALLBACK MonthCalDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitMonthCalDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitMonthCalDialog);

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region DateTimePick
// MSDN: Date and Time Picker
// http://msdn.microsoft.com/en-us/library/bb761727.aspx

#define IDC_DATETIMEPICK		5990

//
//   FUNCTION: OnInitDateTimePickDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitDateTimePickDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register DateTimePick control class
    INITCOMMONCONTROLSEX iccx;
    iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
    iccx.dwICC = ICC_DATE_CLASSES;
    if (!InitCommonControlsEx(&iccx))
        return FALSE;

    // Create the DateTimePick control
    RECT rc = { 20, 20, 150, 30 };
    HWND hDateTimePick = CreateWindowEx(0, DATETIMEPICK_CLASS, 0, 
        WS_CHILD | WS_VISIBLE, rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, (HMENU)IDC_DATETIMEPICK, g_hInst, 0);

    return TRUE;
}

//
//  FUNCTION: DateTimePickDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the DateTimePick control dialog.
//
//
INT_PTR CALLBACK DateTimePickDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitDateTimePickDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitDateTimePickDialog);

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region Listview
// MSDN: List View
// http://msdn.microsoft.com/en-us/library/bb774737.aspx

#define IDC_LISTVIEW		6990

struct LVHandles
{
    HWND hListview;
    HIMAGELIST hLargeIcons;
    HIMAGELIST hSmallIcons;
};

//
//   FUNCTION: OnListviewSize(HWND, UINT, int, int)
//
//   PURPOSE: Process the WM_SIZE message
//
void OnListviewSize(HWND hWnd, UINT state, int cx, int cy)
{
    // Get the pointer to listview information which was previously stored in 
    // the user data associated with the parent window.
    LVHandles* lvh = (LVHandles*)GetWindowLongPtr(hWnd, GWLP_USERDATA);

    // Resize the listview control to fill the parent window's client area
    MoveWindow(lvh->hListview, 0, 0, cx, cy, 1);

    // Arrange contents of listview along top of control
    SendMessage(lvh->hListview, LVM_ARRANGE, LVA_ALIGNTOP, 0);
}

//
//   FUNCTION: OnInitListviewDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitListviewDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register Listview control class
    INITCOMMONCONTROLSEX iccx;
    iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
    iccx.dwICC = ICC_LISTVIEW_CLASSES;
    if (!InitCommonControlsEx(&iccx))
        return FALSE;

    // Create storage for struct to contain information about the listview 
    // (window and image list handles).
    LVHandles* lvh = new LVHandles();

    // Store that pointer as the user data associated with the parent window 
    // so that it can be retrieved for later use. 
    SetWindowLongPtr(hWnd, GWLP_USERDATA, (LONG_PTR)lvh);

    // Create the Listview control
    RECT rc;
    GetClientRect(hWnd, &rc);
    lvh->hListview = CreateWindowEx(0, WC_LISTVIEW, 0, 
        LVS_ICON | WS_CHILD | WS_VISIBLE, 
        rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, (HMENU)IDC_LISTVIEW, g_hInst, 0);


    /////////////////////////////////////////////////////////////////////////
    // Set up and attach image lists to list view common control.
    // 

    // Create the image lists
    int lx = GetSystemMetrics(SM_CXICON);
    int ly = GetSystemMetrics(SM_CYICON);
    lvh->hLargeIcons = ImageList_Create(lx, ly, ILC_COLOR32 | ILC_MASK, 1, 1); 
    int sx = GetSystemMetrics(SM_CXSMICON);
    int sy = GetSystemMetrics(SM_CYSMICON);
    lvh->hSmallIcons = ImageList_Create(sx, sy, ILC_COLOR32 | ILC_MASK, 1, 1);

    // Add icons to image lists
    HICON hLargeIcon, hSmallIcon;
    for (int rid = IDI_ICON1; rid <= IDI_ICON10; rid++)
    {
        // Load and add the large icon
        hLargeIcon = (HICON)LoadImage(g_hInst, MAKEINTRESOURCE(rid), 
            IMAGE_ICON, lx, ly, 0);
        ImageList_AddIcon(lvh->hLargeIcons, hLargeIcon);
        DestroyIcon(hLargeIcon);

        // Load and add the small icon
        hSmallIcon = (HICON)LoadImage(g_hInst, MAKEINTRESOURCE(rid), 
            IMAGE_ICON, sx, sy, 0);
        ImageList_AddIcon(lvh->hSmallIcons, hSmallIcon);
        DestroyIcon(hSmallIcon);
    }

    // Attach image lists to list view common control
    ListView_SetImageList(lvh->hListview, lvh->hLargeIcons, LVSIL_NORMAL); 
    ListView_SetImageList(lvh->hListview, lvh->hSmallIcons, LVSIL_SMALL);


    /////////////////////////////////////////////////////////////////////////
    // Add items to the the list view common control.
    // 

    LVITEM lvi = {0};
    lvi.mask = LVIF_TEXT | LVIF_IMAGE;
    TCHAR szText[200];
    for (int i = 0; i < 10; i++)
    {
        lvi.iItem = i;
        swprintf_s(szText, 200, L"Item  %d", i);
        lvi.pszText = szText;
        lvi.iImage = i;

        SendMessage(lvh->hListview, LVM_INSERTITEM, 0, (LPARAM)&lvi);
    }

    return TRUE;
}

//
//   FUNCTION: OnListviewClose(HWND)
//
//   PURPOSE: Process the WM_CLOSE message
//
void OnListviewClose(HWND hWnd)
{
    // Free up resources

    // Get the information which was previously stored as the user data of 
    // the main window
    LVHandles* lvh = (LVHandles*)GetWindowLongPtr(hWnd, GWLP_USERDATA);

    // Destroy the image lists
    ImageList_Destroy(lvh->hLargeIcons);
    ImageList_Destroy(lvh->hSmallIcons);
    delete lvh;

    DestroyWindow(hWnd);
}

//
//  FUNCTION: ListviewDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the Listview control dialog.
//
//
INT_PTR CALLBACK ListviewDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitListviewDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitListviewDialog);

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnListviewClose);

        // Handle the WM_SIZE message in OnListviewSize
        HANDLE_MSG (hWnd, WM_SIZE, OnListviewSize);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region Treeview
// MSDN: Tree View
// http://msdn.microsoft.com/en-us/library/bb759988.aspx

#define IDC_TREEVIEW		7990

//
//   FUNCTION: OnTreeviewSize(HWND, UINT, int, int)
//
//   PURPOSE: Process the WM_SIZE message
//
void OnTreeviewSize(HWND hWnd, UINT state, int cx, int cy)
{
    // Get the treeview control handle which was previously stored in the 
    // user data associated with the parent window.
    HWND hTreeview = (HWND)GetWindowLongPtr(hWnd, GWLP_USERDATA);

    // Resize treeview control to fill client area of its parent window
    MoveWindow(hTreeview, 0, 0, cx, cy, TRUE);
}

HTREEITEM InsertTreeviewItem(const HWND hTreeview, const LPTSTR pszText, 
                             HTREEITEM htiParent)
{
    TVITEM tvi = {0};
    tvi.mask = TVIF_TEXT | TVIF_IMAGE | TVIF_SELECTEDIMAGE;
    tvi.pszText = pszText;
    tvi.cchTextMax = wcslen(pszText);
    tvi.iImage = 0;

    TVINSERTSTRUCT tvis = {0};
    tvi.iSelectedImage = 1;
    tvis.item = tvi; 
    tvis.hInsertAfter = 0;
    tvis.hParent = htiParent;

    return (HTREEITEM)SendMessage(hTreeview, TVM_INSERTITEM, 0, (LPARAM)&tvis);
}

//
//   FUNCTION: OnInitTreeviewDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitTreeviewDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register Treeview control class
    INITCOMMONCONTROLSEX iccx;
    iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
    iccx.dwICC = ICC_TREEVIEW_CLASSES;
    if (!InitCommonControlsEx(&iccx))
        return FALSE;

    // Create the Treeview control
    RECT rc;
    GetClientRect(hWnd, &rc);
    HWND hTreeview = CreateWindowEx(0, WC_TREEVIEW, 0, 
        TVS_HASLINES | TVS_LINESATROOT | TVS_HASBUTTONS | WS_CHILD | WS_VISIBLE, 
        rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, (HMENU)IDC_TREEVIEW, g_hInst, 0);

    // Store the Treeview control handle as the user data associated with the 
    // parent window so that it can be retrieved for later use.
    SetWindowLongPtr(hWnd, GWLP_USERDATA, (LONG_PTR)hTreeview);


    /////////////////////////////////////////////////////////////////////////
    // Set up and attach image lists to tree view common control.
    // 

    // Create an image list
    HIMAGELIST hImages = ImageList_Create(
        GetSystemMetrics(SM_CXSMICON),
        GetSystemMetrics(SM_CYSMICON), 
        ILC_COLOR32 | ILC_MASK, 1, 1);

    // Get an instance handle for a source of icon images
    HINSTANCE hLib = LoadLibrary(L"shell32.dll");
    if (hLib)
    {
        for (int i = 4; i < 6; i++)
        {
            // Because the icons are loaded from system resources (i.e. they are 
            // shared), it is not necessary to free resources with 'DestroyIcon'.
            HICON hIcon = (HICON)LoadImage(hLib, MAKEINTRESOURCE(i), IMAGE_ICON,
                0, 0, LR_SHARED);
            ImageList_AddIcon(hImages, hIcon); 
        }
        
        FreeLibrary(hLib);
        hLib = NULL;
    }

    // Attach image lists to tree view common control
    TreeView_SetImageList(hTreeview, hImages, TVSIL_NORMAL);


    /////////////////////////////////////////////////////////////////////////
    // Add items to the tree view common control.
    // 

    // Insert the first item at root level
    HTREEITEM hPrev = InsertTreeviewItem(hTreeview, L"First", TVI_ROOT);

    // Sub item of first item
    hPrev = InsertTreeviewItem(hTreeview, L"Level01", hPrev);

    // Sub item of 'level01' item
    hPrev = InsertTreeviewItem(hTreeview, L"Level02", hPrev);

    // Insert the second item at root level
    hPrev = InsertTreeviewItem(hTreeview, L"Second", TVI_ROOT);

    // Insert the third item at root level
    hPrev = InsertTreeviewItem(hTreeview, L"Third", TVI_ROOT);

    return TRUE;
}

//
//   FUNCTION: OnTreeviewClose(HWND)
//
//   PURPOSE: Process the WM_CLOSE message
//
void OnTreeviewClose(HWND hWnd)
{
    // Get the treeview control handle which was previously stored in the 
    // user data associated with the parent window.
    HWND hTreeview = (HWND)GetWindowLongPtr(hWnd, GWLP_USERDATA);

    // Free resources used by the treeview's image list
    HIMAGELIST hImages = TreeView_GetImageList(hTreeview, TVSIL_NORMAL);
    ImageList_Destroy(hImages);

    DestroyWindow(hWnd);
}

//
//  FUNCTION: TreeviewDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the Treeview control dialog.
//
//
INT_PTR CALLBACK TreeviewDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitTreeviewDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitTreeviewDialog);

        // Handle the WM_CLOSE message in OnTreeviewClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnTreeviewClose);

        // Handle the WM_SIZE message in OnTreeviewSize
        HANDLE_MSG (hWnd, WM_SIZE, OnTreeviewSize);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region TabControl
// MSDN: Tab
// http://msdn.microsoft.com/en-us/library/bb760548.aspx

#define IDC_TAB			8990

//
//   FUNCTION: OnTabSize(HWND, UINT, int, int)
//
//   PURPOSE: Process the WM_SIZE message
//
void OnTabSize(HWND hWnd, UINT state, int cx, int cy)
{
    // Get the Tab control handle which was previously stored in the 
    // user data associated with the parent window.
    HWND hTab = (HWND)GetWindowLongPtr(hWnd, GWLP_USERDATA);

    // Resize tab control to fill client area of its parent window
    MoveWindow(hTab, 2, 2, cx - 4, cy - 4, TRUE);
}

int InsertTabItem(HWND hTab, LPTSTR pszText, int iid)
{
    TCITEM ti = {0};
    ti.mask = TCIF_TEXT;
    ti.pszText = pszText;
    ti.cchTextMax = wcslen(pszText);

    return (int)SendMessage(hTab, TCM_INSERTITEM, iid, (LPARAM)&ti);
}

//
//   FUNCTION: OnInitTabControlDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitTabControlDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register Tab control class
    INITCOMMONCONTROLSEX iccx;
    iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
    iccx.dwICC = ICC_TAB_CLASSES;
    if (!InitCommonControlsEx(&iccx))
        return FALSE;

    // Create the Tab control 
    RECT rc;
    GetClientRect(hWnd, &rc);
    HWND hTab = CreateWindowEx(0, WC_TABCONTROL, 0, 
        TCS_FIXEDWIDTH | WS_CHILD | WS_VISIBLE, 
        rc.left + 2, rc.top + 2, rc.right - 4, rc.bottom - 4, 
        hWnd, (HMENU)IDC_TAB, g_hInst, 0);

    // Set the font of the tabs to a more typical system GUI font
    SendMessage(hTab, WM_SETFONT, (WPARAM)GetStockObject(DEFAULT_GUI_FONT), 0);

    // Store the Tab control handle as the user data associated with the 
    // parent window so that it can be retrieved for later use.
    SetWindowLongPtr(hWnd, GWLP_USERDATA, (LONG_PTR)hTab);


    /////////////////////////////////////////////////////////////////////////
    // Add items to the tab common control.
    // 

    InsertTabItem(hTab, L"First Page", 0);
    InsertTabItem(hTab, L"Second Page", 1);
    InsertTabItem(hTab, L"Third Page", 2);
    InsertTabItem(hTab, L"Fourth Page", 3);
    InsertTabItem(hTab, L"Fifth Page", 4);

    return TRUE;
}

//
//  FUNCTION: TabControlDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the TabControl control dialog.
//
//
INT_PTR CALLBACK TabControlDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitTabControlDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitTabControlDialog);

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

        // Handle the WM_SIZE message in OnTabSize
        HANDLE_MSG (hWnd, WM_SIZE, OnTabSize);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region Tooltip
// MSDN: ToolTip
// http://msdn.microsoft.com/en-us/library/bb760246.aspx

#define IDC_BUTTON1		9990

//
//   FUNCTION: OnInitTooltipDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitTooltipDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register Tooltip control class
    INITCOMMONCONTROLSEX iccx;
    iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
    iccx.dwICC = ICC_WIN95_CLASSES;
    if (!InitCommonControlsEx(&iccx))
        return FALSE;

    // Create a button control
    RECT rc = { 20, 20, 120, 40 };
    HWND hBtn = CreateWindowEx(0, L"BUTTON", L"Tooltip Target", 
        WS_CHILD | WS_VISIBLE, rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, (HMENU)IDC_BUTTON1, g_hInst, 0); 

    // Create a tooltip
    // A tooltip control should not have the WS_CHILD style, nor should it 
    // have an id, otherwise its behavior will be adversely affected.
    HWND hTooltip = CreateWindowEx(0, TOOLTIPS_CLASS, L"", TTS_ALWAYSTIP, 
        0, 0, 0, 0, hWnd, 0, g_hInst, 0);

    // Associate the tooltip with the button control
    TOOLINFO ti = {0};
    ti.cbSize = sizeof(ti);
    ti.uFlags = TTF_IDISHWND | TTF_SUBCLASS;
    ti.uId = (UINT_PTR)hBtn;
    ti.lpszText = L"This is a button.";
    ti.hwnd = hWnd;
    SendMessage(hTooltip, TTM_ADDTOOL, 0, (LPARAM)&ti);

    return TRUE;
}

//
//  FUNCTION: TooltipDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the Tooltip control dialog.
//
//
INT_PTR CALLBACK TooltipDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitTooltipDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitTooltipDialog);

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region IPAddress
// MSDN: IP Address Control
// http://msdn.microsoft.com/en-us/library/bb761374.aspx

#define IDC_IPADDRESS		10990

//
//   FUNCTION: OnInitIPAddressDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitIPAddressDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register IPAddress control class
    INITCOMMONCONTROLSEX iccx;
    iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
    iccx.dwICC = ICC_INTERNET_CLASSES;
    if (!InitCommonControlsEx(&iccx))
        return FALSE;

    // Create the IPAddress control
    RECT rc = { 20, 20, 180, 24 };
    HWND hIPAddress = CreateWindowEx(0, WC_IPADDRESS, 0, 
        WS_CHILD | WS_VISIBLE, rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, (HMENU)IDC_IPADDRESS, g_hInst, 0);

    return TRUE;
}

//
//  FUNCTION: IPAddressDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the IPAddress control dialog.
//
//
INT_PTR CALLBACK IPAddressDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitIPAddressDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitIPAddressDialog);

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region Statusbar
// MSDN: Status Bar
// http://msdn.microsoft.com/en-us/library/bb760726.aspx

#define IDC_STATUSBAR		11990

//
//   FUNCTION: OnStatusbarSize(HWND, UINT, int, int)
//
//   PURPOSE: Process the WM_SIZE message
//
void OnStatusbarSize(HWND hWnd, UINT state, int cx, int cy)
{
    // Get the Statusbar control handle which was previously stored in the 
    // user data associated with the parent window.
    HWND hStatusbar = (HWND)GetWindowLongPtr(hWnd, GWLP_USERDATA);

    // Partition the statusbar here to keep the ratio of the sizes of its 
    // parts constant. Each part is set by specifing the coordinates of the 
    // right edge of each part. -1 signifies the rightmost part of the parent.
    int nHalf = cx / 2;
    int nParts[] = { nHalf, nHalf + nHalf / 3, nHalf + nHalf * 2 / 3, -1 };
    SendMessage(hStatusbar, SB_SETPARTS, 4, (LPARAM)&nParts);

    // Resize statusbar so it's always same width as parent's client area
    SendMessage(hStatusbar, WM_SIZE, 0, 0);
}

//
//   FUNCTION: OnInitStatusbarDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitStatusbarDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register IPAddress control class
    INITCOMMONCONTROLSEX iccx;
    iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
    iccx.dwICC = ICC_BAR_CLASSES;
    if (!InitCommonControlsEx(&iccx))
        return FALSE;

    // Create the status bar control
    RECT rc = { 0, 0, 0, 0 };
    HWND hStatusbar = CreateWindowEx(0, STATUSCLASSNAME, 0, 
        SBARS_SIZEGRIP | WS_CHILD | WS_VISIBLE, 
        rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, (HMENU)IDC_STATUSBAR, g_hInst, 0);

    // Store the statusbar control handle as the user data associated with 
    // the parent window so that it can be retrieved for later use.
    SetWindowLongPtr(hWnd, GWLP_USERDATA, (LONG_PTR)hStatusbar);

    // Establish the number of partitions or 'parts' the status bar will 
    // have, their actual dimensions will be set in the parent window's 
    // WM_SIZE handler.
    GetClientRect(hWnd, &rc);
    int nHalf = rc.right / 2;
    int nParts[4] = { nHalf, nHalf + nHalf / 3, nHalf + nHalf * 2 / 3, -1 };
    SendMessage(hStatusbar, SB_SETPARTS, 4, (LPARAM)&nParts);

    // Put some texts into each part of the status bar and setup each part
    SendMessage(hStatusbar, SB_SETTEXT, 0, (LPARAM)L"Status Bar: Part 1");
    SendMessage(hStatusbar, SB_SETTEXT, 1| SBT_POPOUT, (LPARAM)L"Part 2");
    SendMessage(hStatusbar, SB_SETTEXT, 2| SBT_POPOUT, (LPARAM)L"Part 3");
    SendMessage(hStatusbar, SB_SETTEXT, 3| SBT_POPOUT, (LPARAM)L"Part 4");

    return TRUE;
}

//
//  FUNCTION: StatusbarDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the Statusbar control dialog.
//
//
INT_PTR CALLBACK StatusbarDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitStatusbarDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitStatusbarDialog);

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

        // Handle the WM_SIZE message in OnStatusbarSize
        HANDLE_MSG (hWnd, WM_SIZE, OnStatusbarSize);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region Progress
// MSDN: Progress Bar
// http://msdn.microsoft.com/en-us/library/bb760818.aspx

#define IDC_PROGRESSBAR		12990

//
//   FUNCTION: OnInitProgressDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitProgressDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register Progress control class
    INITCOMMONCONTROLSEX iccx;
    iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
    iccx.dwICC = ICC_PROGRESS_CLASS;
    if (!InitCommonControlsEx(&iccx))
        return FALSE;

    // Create the progress bar control
    RECT rc = { 20, 20, 250, 20 };
    HWND hProgress = CreateWindowEx(0, PROGRESS_CLASS, 0, 
        WS_CHILD | WS_VISIBLE, rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, (HMENU)IDC_PROGRESSBAR, g_hInst, 0);

    // Set the progress bar position to half-way
    SendMessage(hProgress, PBM_SETPOS, 50, 0);

    return TRUE;
}

//
//  FUNCTION: ProgressDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the Progress control dialog.
//
//
INT_PTR CALLBACK ProgressDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitProgressDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitProgressDialog);

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region Toolbar
// MSDN: Toolbar
// http://msdn.microsoft.com/en-us/library/bb760435.aspx

#define IDC_TOOLBAR			13990

//
//   FUNCTION: OnInitToolbarDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitToolbarDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register Toolbar control class
    INITCOMMONCONTROLSEX iccx;
    iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
    iccx.dwICC = ICC_BAR_CLASSES;
    if (!InitCommonControlsEx(&iccx))
        return FALSE;

    // Create the Toolbar control
    RECT rc = { 0, 0, 0, 0 };
    HWND hToolbar = CreateWindowEx(0, TOOLBARCLASSNAME, 0, 
        TBSTYLE_FLAT | CCS_ADJUSTABLE | CCS_NODIVIDER | WS_CHILD | WS_VISIBLE, 
        rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, (HMENU)IDC_TOOLBAR, g_hInst, 0);


    /////////////////////////////////////////////////////////////////////////
    // Setup and add buttons to Toolbar.
    // 

    // If an application uses the CreateWindowEx function to create the 
    // toolbar, the application must send this message to the toolbar before 
    // sending the TB_ADDBITMAP or TB_ADDBUTTONS message. The CreateToolbarEx 
    // function automatically sends TB_BUTTONSTRUCTSIZE, and the size of the 
    // TBBUTTON structure is a parameter of the function.
    SendMessage(hToolbar, TB_BUTTONSTRUCTSIZE, (WPARAM)sizeof(TBBUTTON), 0);

    // Add images

    TBADDBITMAP tbAddBmp = {0};
    tbAddBmp.hInst = HINST_COMMCTRL;
    tbAddBmp.nID = IDB_STD_SMALL_COLOR;

    SendMessage(hToolbar, TB_ADDBITMAP, 0, (WPARAM)&tbAddBmp);

    // Add buttons

    const int numButtons = 7;
    TBBUTTON tbButtons[numButtons] = 
    {
        { MAKELONG(STD_FILENEW, 0), NULL, TBSTATE_ENABLED, 
        BTNS_AUTOSIZE, {0}, 0, (INT_PTR)L"New" },
        { MAKELONG(STD_FILEOPEN, 0), NULL, TBSTATE_ENABLED, 
        BTNS_AUTOSIZE, {0}, 0, (INT_PTR)L"Open" },
        { MAKELONG(STD_FILESAVE, 0), NULL, 0, 
        BTNS_AUTOSIZE, {0}, 0, (INT_PTR)L"Save" }, 
        { MAKELONG(0, 0), NULL, 0, 
        TBSTYLE_SEP, {0}, 0, (INT_PTR)L"" }, // Separator
        { MAKELONG(STD_COPY, 0), NULL, TBSTATE_ENABLED, 
        BTNS_AUTOSIZE, {0}, 0, (INT_PTR)L"Copy" }, 
        { MAKELONG(STD_CUT, 0), NULL, TBSTATE_ENABLED, 
        BTNS_AUTOSIZE, {0}, 0, (INT_PTR)L"Cut" }, 
        { MAKELONG(STD_PASTE, 0), NULL, TBSTATE_ENABLED, 
        BTNS_AUTOSIZE, {0}, 0, (INT_PTR)L"Paste" }
    };

    SendMessage(hToolbar, TB_ADDBUTTONS, numButtons, (LPARAM)tbButtons);

    // Tell the toolbar to resize itself, and show it.
    SendMessage(hToolbar, TB_AUTOSIZE, 0, 0); 

    return TRUE;
}

//
//  FUNCTION: ToolbarDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the Toolbar control dialog.
//
//
INT_PTR CALLBACK ToolbarDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitToolbarDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitToolbarDialog);

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region Trackbar
// MSDN: Trackbar
// http://msdn.microsoft.com/en-us/library/bb760145.aspx

#define IDC_TRACKBAR			14990

//
//   FUNCTION: OnInitTrackbarDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitTrackbarDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register Trackbar control class
    INITCOMMONCONTROLSEX iccx;
    iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
    iccx.dwICC = ICC_WIN95_CLASSES; // Or ICC_PROGRESS_CLASS
    if (!InitCommonControlsEx(&iccx))
        return FALSE;

    // Create the Trackbar control
    RECT rc = { 20, 20, 250, 20 };
    HWND hTrackbar = CreateWindowEx(0, TRACKBAR_CLASS, 0, 
        TBS_AUTOTICKS | WS_CHILD | WS_VISIBLE, 
        rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, (HMENU)IDC_TRACKBAR, g_hInst, 0);

    // Set Trackbar range
    SendMessage(hTrackbar, TBM_SETRANGE, 0, MAKELONG(0, 20));

    return TRUE;
}

//
//  FUNCTION: TrackbarDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the Trackbar control dialog.
//
//
INT_PTR CALLBACK TrackbarDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitTrackbarDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitTrackbarDialog);

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region SysLink
// MSDN: SysLink
// http://msdn.microsoft.com/en-us/library/bb760704.aspx

#define IDC_SYSLINK			15990

//
//   FUNCTION: OnInitSysLinkDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitSysLinkDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register SysLink control class
    INITCOMMONCONTROLSEX iccx;
    iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
    iccx.dwICC = ICC_LINK_CLASS;
    if (!InitCommonControlsEx(&iccx))
        return FALSE;

    // Create the SysLink control
    // The SysLink control supports the anchor tag(<a>) along with the 
    // attributes HREF and ID. 
    RECT rc = { 20, 20, 500, 100 };
    HWND hLink = CreateWindowEx(0, WC_LINK, 
        L"All-In-One Code Framework\n" \
        L"<A HREF=\"http://cfx.codeplex.com\">Home</A> " \
        L"and <A ID=\"idBlog\">Blog</A>", 
        WS_VISIBLE | WS_CHILD | WS_TABSTOP, 
        rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, (HMENU)IDC_SYSLINK, g_hInst, NULL);

    return TRUE;
}

//
//   FUNCTION: OnSysLinkNotify(HWND, int, PNMLINK)
//
//   PURPOSE: Process the WM_NOTIFY message
//
LRESULT OnSysLinkNotify(HWND hWnd, int idCtrl, LPNMHDR pNMHdr)
{
    if (idCtrl != IDC_SYSLINK)	// Make sure that it is the SysLink control
        return 0;

    // The notifications associated with SysLink controls are NM_CLICK 
    // (syslink) and (for links that can be activated by the Enter key) 
    // NM_RETURN.
    switch (pNMHdr->code)
    {
    case NM_CLICK:
    case NM_RETURN:
        {
            PNMLINK pNMLink = (PNMLINK)pNMHdr;
            LITEM item = pNMLink->item;

            // Judging by the index of the link
            if (item.iLink == 0) // If it is the first link
            {
                ShellExecute(NULL, L"open", item.szUrl, NULL, NULL, SW_SHOW);
            }
            // Judging by the ID of the link
            else if (wcscmp(item.szID, L"idBlog") == 0)
            {
                MessageBox(hWnd, L"http://blogs.msdn.com/codefx", 
                    L"All-In-One Code Framework Blog", MB_OK);
            }
            break;
        }
    }
    return 0;
}

//
//  FUNCTION: SysLinkDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the SysLink control dialog.
//
//
INT_PTR CALLBACK SysLinkDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitSysLinkDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitSysLinkDialog);

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

        // Handle the WM_NOTIFY message in OnNotify
        HANDLE_MSG (hWnd, WM_NOTIFY, OnSysLinkNotify);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region Rebar
// MSDN: Rebar
// http://msdn.microsoft.com/en-us/library/bb774375.aspx

#define IDC_REBAR			16990

//
//   FUNCTION: OnInitRebarDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message
//
BOOL OnInitRebarDialog(HWND hWnd, HWND hWndFocus, LPARAM lParam)
{
    // Load and register Rebar control class
    INITCOMMONCONTROLSEX iccx;
    iccx.dwSize = sizeof(INITCOMMONCONTROLSEX);
    iccx.dwICC = ICC_COOL_CLASSES;
    if (!InitCommonControlsEx(&iccx))
        return FALSE;

    // Create the Rebar control
    RECT rc = { 0, 0, 0, 0 };
    HWND hRebar = CreateWindowEx(0, REBARCLASSNAME, L"", 
        WS_VISIBLE | WS_CHILD | RBS_AUTOSIZE, 
        rc.left, rc.top, rc.right, rc.bottom, 
        hWnd, (HMENU)IDC_REBAR, g_hInst, NULL);

    REBARINFO ri = {0};
    ri.cbSize = sizeof(REBARINFO);
    SendMessage(hRebar, RB_SETBARINFO, 0, (LPARAM)&ri);

    // Insert a image
    HICON hImg = (HICON)LoadImage(0, IDI_QUESTION, IMAGE_ICON, 0, 0, LR_SHARED);
    HWND hwndImg = CreateWindow(L"STATIC", NULL, 
        WS_CHILD | WS_VISIBLE | SS_ICON | SS_REALSIZEIMAGE | SS_NOTIFY,
        0, 0, 0, 0, hRebar, (HMENU)NULL, g_hInst,	NULL);

    // Set static control image
    SendMessage(hwndImg, STM_SETICON, (WPARAM)hImg, NULL);

    REBARBANDINFO rbBand;
    rbBand.cbSize = sizeof(REBARBANDINFO);
    rbBand.fMask = RBBIM_STYLE | RBBIM_CHILDSIZE | RBBIM_CHILD | RBBIM_SIZE;
    rbBand.fStyle = RBBS_CHILDEDGE | RBBS_NOGRIPPER;
    rbBand.hwndChild = hwndImg;
    rbBand.cxMinChild = 0;
    rbBand.cyMinChild = 20;
    rbBand.cx = 20;

    // Insert the img into the rebar
    SendMessage(hRebar, RB_INSERTBAND, (WPARAM)-1, (LPARAM)&rbBand);

    // Insert a blank band
    rbBand.cbSize = sizeof(REBARBANDINFO);
    rbBand.fMask =  RBBIM_STYLE | RBBIM_SIZE;
    rbBand.fStyle = RBBS_CHILDEDGE | RBBS_HIDETITLE | RBBS_NOGRIPPER;
    rbBand.cx = 1;

    // Insert the blank band into the rebar
    SendMessage(hRebar, RB_INSERTBAND, (WPARAM)-1, (LPARAM)&rbBand);

    return TRUE;
}

//
//  FUNCTION: RebarDlgProc(HWND, UINT, WPARAM, LPARAM)
//
//  PURPOSE:  Processes messages for the Rebar control dialog.
//
//
INT_PTR CALLBACK RebarDlgProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_INITDIALOG message in OnInitRebarDialog
        HANDLE_MSG (hWnd, WM_INITDIALOG, OnInitRebarDialog);

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

    default:
        return FALSE;	// Let system deal with msg
    }
    return 0;
}

#pragma endregion


#pragma region Main Window

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
void OnCommand(HWND hWnd, int id, HWND hWndCtl, UINT codeNotify)
{
    switch (id)
    {
    case IDC_BUTTON_ANIMATION:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_ANIMATIONDIALOG), 
                hWnd, AnimationDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDC_BUTTON_COMBOBOXEX:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_COMBOBOXEXDIALOG), 
                hWnd, ComboBoxExDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDC_BUTTON_DATETIMEPICK:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_DATETIMEPICKDIALOG), 
                hWnd, DateTimePickDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDC_BUTTON_HEADER:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_HEADERDIALOG), 
                hWnd, HeaderDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDC_BUTTON_IPADDRESS:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_IPADDRESSDIALOG), 
                hWnd, IPAddressDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDC_BUTTON_LISTVIEW:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_LISTVIEWDIALOG), 
                hWnd, ListviewDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDC_BUTTON_MONTHCAL:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_MONTHCALDIALOG), 
                hWnd, MonthCalDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDC_BUTTON_PROGRESS:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_PROGRESSDIALOG), 
                hWnd, ProgressDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDC_BUTTON_SYSLINK:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_SYSLINKDIALOG), 
                hWnd, SysLinkDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDC_BUTTON_STATUS:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_STATUSDIALOG), 
                hWnd, StatusbarDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDC_BUTTON_TABCONTROL:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_TABCONTROLDIALOG), 
                hWnd, TabControlDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDC_BUTTON_TOOLBAR:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_TOOLBARDIALOG), 
                hWnd, ToolbarDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDC_BUTTON_TOOLTIP:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_TOOLTIPDIALOG), 
                hWnd, TooltipDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDC_BUTTON_TRACKBAR:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_TRACKBARDIALOG), 
                hWnd, TrackbarDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDC_BUTTON_TREEVIEW:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_TREEVIEWDIALOG), 
                hWnd, TreeviewDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDC_BUTTON_UPDOWN:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_UPDOWNDIALOG), 
                hWnd, UpdownDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDC_BUTTON_REBAR:
        {
            HWND hDlg = CreateDialog(g_hInst, 
                MAKEINTRESOURCE(IDD_REBARDIALOG), 
                hWnd, RebarDlgProc);
            if (hDlg)
            {
                ShowWindow(hDlg, SW_SHOW);
            }
        }
        break;

    case IDOK:
    case IDCANCEL:
        EndDialog(hWnd, 0);
        break;
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

        // Handle the WM_CLOSE message in OnClose
        HANDLE_MSG (hWnd, WM_CLOSE, OnClose);

    default:
        return FALSE;
    }
    return 0;
}

#pragma endregion


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
    g_hInst = hInstance;
    return DialogBox(hInstance, MAKEINTRESOURCE(IDD_MAINDIALOG), NULL, DialogProc);
}