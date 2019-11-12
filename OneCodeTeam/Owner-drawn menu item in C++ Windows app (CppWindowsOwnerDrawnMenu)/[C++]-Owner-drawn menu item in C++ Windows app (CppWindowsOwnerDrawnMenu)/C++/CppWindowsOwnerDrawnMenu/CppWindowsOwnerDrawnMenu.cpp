/****************************** Module Header ******************************\
* Module Name:  CppWindowsOwnerDrawnMenu.cpp
* Project:      CppWindowsOwnerDrawnMenu
* Copyright (c) Microsoft Corporation.
* 
* If you need complete control over the appearance of a menu item, you can 
* use an owner-drawn menu item in your application. This VC++ code sample 
* demonstrates creating owner-drawn menu items. The example contains a 
* Character menu whose items display regular, bold, italic, and underline 
* texts in custom foreground, background and highlight colors.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma region Includes
#include <windows.h>
#include "resource.h"
#include <windowsx.h>
#pragma endregion


#define MAX_LOADSTRING 100

// Global Variables:
HINSTANCE hInst;								// Current instance
TCHAR szTitle[MAX_LOADSTRING];					// The title bar text
TCHAR szWindowClass[MAX_LOADSTRING];			// The main window class name

// Forward declarations of functions included in this code module:
ATOM                MyRegisterClass(HINSTANCE hInstance);
BOOL                InitInstance(HINSTANCE, int);
LRESULT CALLBACK    WndProc(HWND, UINT, WPARAM, LPARAM);
void                OnCommand(HWND, int, HWND, UINT);
INT_PTR CALLBACK    About(HWND, UINT, WPARAM, LPARAM);
HFONT               CreateMenuItemFont(UINT);
BOOL                OnCreate(HWND, LPCREATESTRUCT);
void                OnMeasureItem(HWND, MEASUREITEMSTRUCT *);
void                OnDrawItem(HWND, const DRAWITEMSTRUCT *);
void                OnDestroy(HWND);


int APIENTRY wWinMain(HINSTANCE hInstance,
                      HINSTANCE hPrevInstance,
                      LPTSTR    lpCmdLine,
                      int       nCmdShow)
{
    UNREFERENCED_PARAMETER(hPrevInstance);
    UNREFERENCED_PARAMETER(lpCmdLine);

    MSG msg;
    HACCEL hAccelTable;

    // Initialize global strings
    LoadString(hInstance, IDS_APP_TITLE, szTitle, MAX_LOADSTRING);
    LoadString(hInstance, IDC_CPPWINDOWSOWNERDRAWNMENU, szWindowClass, 
        MAX_LOADSTRING);
    MyRegisterClass(hInstance);

    // Perform application initialization:
    if (!InitInstance (hInstance, nCmdShow))
    {
        return FALSE;
    }

    hAccelTable = LoadAccelerators(hInstance, MAKEINTRESOURCE(
        IDC_CPPWINDOWSOWNERDRAWNMENU));

    // Main message loop:
    while (GetMessage(&msg, NULL, 0, 0))
    {
        if (!TranslateAccelerator(msg.hwnd, hAccelTable, &msg))
        {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
    }

    return (int) msg.wParam;
}


//
//   FUNCTION: MyRegisterClass()
//
//   PURPOSE: Registers the window class. This function and its usage are only 
//   necessary if you want this code to be compatible with Win32 systems prior 
//   to the 'RegisterClassEx' function that was added to Windows 95. It is 
//   important to call this function so that the application will get 'well 
//   formed' small icons associated with it.
//
ATOM MyRegisterClass(HINSTANCE hInstance)
{
    WNDCLASSEX wcex;

    wcex.cbSize = sizeof(WNDCLASSEX);

    wcex.style			= CS_HREDRAW | CS_VREDRAW;
    wcex.lpfnWndProc	= WndProc;
    wcex.cbClsExtra		= 0;
    wcex.cbWndExtra		= 0;
    wcex.hInstance		= hInstance;
    wcex.hIcon			= LoadIcon(hInstance, MAKEINTRESOURCE(IDI_CPPWINDOWSOWNERDRAWNMENU));
    wcex.hCursor		= LoadCursor(NULL, IDC_ARROW);
    wcex.hbrBackground	= (HBRUSH)(COLOR_WINDOW+1);
    wcex.lpszMenuName	= MAKEINTRESOURCE(IDC_CPPWINDOWSOWNERDRAWNMENU);
    wcex.lpszClassName	= szWindowClass;
    wcex.hIconSm		= LoadIcon(wcex.hInstance, MAKEINTRESOURCE(IDI_SMALL));

    return RegisterClassEx(&wcex);
}

//
//   FUNCTION: InitInstance(HINSTANCE, int)
//
//   PURPOSE: Saves instance handle in a global variable and creates and 
//   displays the main program window.
//
BOOL InitInstance(HINSTANCE hInstance, int nCmdShow)
{
    HWND hWnd;

    hInst = hInstance; // Store instance handle in our global variable

    hWnd = CreateWindow(szWindowClass, szTitle, WS_OVERLAPPEDWINDOW,
        CW_USEDEFAULT, 0, CW_USEDEFAULT, 0, NULL, NULL, hInstance, NULL);

    if (!hWnd)
    {
        return FALSE;
    }

    ShowWindow(hWnd, nCmdShow);
    UpdateWindow(hWnd);

    return TRUE;
}


//
//   FUNCTION: WndProc(HWND, UINT, WPARAM, LPARAM)
//
//   PURPOSE:  Processes messages for the main window.
//
//   WM_CREATE       - process the window creation event
//   WM_COMMAND	     - process the application menu commands
//   WM_DESTROY	     - post a quit message and return
//   WM_MEASUREITEM  - process the creation event of owner-drawn menu items
//   WM_DRAWITEM     - process the draw event of owner-draw menu items
//
LRESULT CALLBACK WndProc(HWND hWnd, UINT message, WPARAM wParam, LPARAM lParam)
{
    switch (message)
    {
        // Handle the WM_CREATE message in OnCreate
        HANDLE_MSG (hWnd, WM_CREATE, OnCreate);

        // Handle the WM_COMMAND message in OnCommand
        HANDLE_MSG (hWnd, WM_COMMAND, OnCommand);

        // Handle the WM_DESTROY message in OnDestroy
        HANDLE_MSG (hWnd, WM_DESTROY, OnDestroy);

        // Handle the WM_MEASUREITEM message in OnMeasureItem
        HANDLE_MSG (hWnd, WM_MEASUREITEM, OnMeasureItem);

        // Handle the WM_DRAWITEM message in OnDrawItem
        HANDLE_MSG (hWnd, WM_DRAWITEM, OnDrawItem);

    default:
        return DefWindowProc(hWnd, message, wParam, lParam);
    }
    return 0;
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
    case IDM_ABOUT:
        DialogBox(hInst, MAKEINTRESOURCE(IDD_ABOUTBOX), hWnd, About);
        break;

    case IDM_EXIT:
        DestroyWindow(hWnd);
        break;
    }
}


// Message handler for about box.
INT_PTR CALLBACK About(HWND hDlg, UINT message, WPARAM wParam, LPARAM lParam)
{
    UNREFERENCED_PARAMETER(lParam);
    switch (message)
    {
    case WM_INITDIALOG:
        return (INT_PTR)TRUE;

    case WM_COMMAND:
        if (LOWORD(wParam) == IDOK || LOWORD(wParam) == IDCANCEL)
        {
            EndDialog(hDlg, LOWORD(wParam));
            return (INT_PTR)TRUE;
        }
        break;
    }
    return (INT_PTR)FALSE;
}


#pragma region Owner-Drawn Menu Item

// Sructure associated with Character menu items
typedef struct tagCHARMENUITEM
{
    // Font of text on the menu item.
    HFONT hFont;

    // The length of the string pointed to by szItemText.
    int cchItemText;

    // A pointer to a buffer that specifies the text string. The string does 
    // not need to be null-terminated, because the c parameter specifies the 
    // length of the string.
    wchar_t szItemText[1];

} CHARMENUITEM , *PCHARMENUITEM;


#define MENUITEM_TEXTCOLOR      RGB(255,0,0) // or GetSysColor(COLOR_MENUTEXT)
#define MENUITEM_BACKCOLOR      RGB(0,255,0) // or GetSysColor(COLOR_MENU)
#define HIGHLIGHT_TEXTCOLOR     GetSysColor(COLOR_HIGHLIGHTTEXT)
#define HIGHLIGHT_BACKCOLOR     GetSysColor(COLOR_HIGHLIGHT)


//
//   FUNCTION: CreateMenuItemFont(UINT)
//
//   PURPOSE: Create a logical font for a menu item according to its ID. 
//
//      Menu Item ID   -   Font
//      IDM_REGULAR    -   A regular font
//      IDM_BOLD       -   A bold font
//      IDM_ITALIC     -   A italic font
//      IDM_UNDERLINE  -   An underline font
//
//   PARAMETERS: 
//   * uID - menu item ID.
//
//   RETURN VALUE: If the function succeeds, the return value is a handle 
//   to a logical font. If the function fails, the return value is NULL.
//
HFONT CreateMenuItemFont(UINT uID) 
{
    LOGFONT lf = { sizeof(lf) };
    wcscpy_s(lf.lfFaceName, ARRAYSIZE(lf.lfFaceName), L"Times New Roman");
    lf.lfHeight = 20;
    switch (uID)
    {
    case IDM_BOLD: 
        lf.lfWeight = FW_HEAVY;
        break; 
    case IDM_ITALIC: 
        lf.lfItalic = TRUE;
        break; 
    case IDM_UNDERLINE: 
        lf.lfUnderline = TRUE;
        break;
    }
    return CreateFontIndirect(&lf); 
}


//
//   FUNCTION: OnCreate(HWND, LPCREATESTRUCT)
//
//   PURPOSE: Process the WM_CREATE message. Because a menu template cannot 
//   specify owner-drawn items, the menu initially contains four text menu 
//   items with the following strings: "Regular," "Bold," "Italic," and 
//   "Underline." This function changes these to owner-drawn items, and 
//   attaches to each menu item a CHARMENUITEM structure that will be used 
//   when the menu item is created and drawn.
//
BOOL OnCreate(HWND hWnd, LPCREATESTRUCT lpCreateStruct)
{
    BOOL fSucceeded = TRUE;
    HMENU hMenu = GetMenu(hWnd);
    MENUITEMINFO mii = { sizeof(mii) };
    UINT uID = 0;
    PCHARMENUITEM pcmi = NULL;

    // Modify each menu item. Assume that the IDs IDM_REGULAR through 
    // IDM_UNDERLINE are consecutive numbers.
    for (uID = IDM_REGULAR; uID <= IDM_UNDERLINE; uID++) 
    {
        // To retrieve a menu item of type MFT_STRING, first find the length 
        // of the string by setting the dwTypeData member of MENUITEMINFO to 
        // NULL and then calling GetMenuItemInfo. The value of cch is the 
        // length of the menu item text.
        mii.fMask = MIIM_STRING;
        mii.dwTypeData = NULL;
        if (!GetMenuItemInfo(hMenu, uID, FALSE, &mii))
        {
            fSucceeded = FALSE;
            break;
        }

        // Then allocate a buffer of this size.
        pcmi = (PCHARMENUITEM)LocalAlloc(LPTR, 
            sizeof(*pcmi) + mii.cch * sizeof(*pcmi->szItemText));
        if (NULL == pcmi)
        {
            fSucceeded = FALSE;
            break;
        }

        // Place the pointer to the buffer in dwTypeData, increment cch, and 
        // call GetMenuItemInfo once again to fill the buffer with the string. 
        pcmi->cchItemText = mii.cch;
        mii.dwTypeData = pcmi->szItemText;
        mii.cch++;
        if (!GetMenuItemInfo(hMenu, uID, FALSE, &mii))
        {
            fSucceeded = FALSE;
            break;
        }

        // Create the font used to draw the item. 
        pcmi->hFont = CreateMenuItemFont(uID);
        if (NULL == pcmi->hFont)
        {
            fSucceeded = FALSE;
            break;
        }

        // Change the item to an owner-drawn item, and save the 
        // address of the item structure as item data. 
        mii.fMask = MIIM_FTYPE | MIIM_DATA; 
        mii.fType = MFT_OWNERDRAW;
        mii.dwItemData = (ULONG_PTR)pcmi;
        if (!SetMenuItemInfo(hMenu, uID, FALSE, &mii))
        {
            fSucceeded = FALSE;
            break;
        }
    }

    if (!fSucceeded)
    {
        // Clean up the allocated resource when applicable.
        if (pcmi)
        {
            LocalFree(pcmi);
            pcmi = NULL;
        }

        // Display a message box to report the error.
        MessageBox(hWnd, L"An error occurred when the application initializes" \
            L"the owner-drawn menu items. The application will shut down.", 
            L"Error", MB_ICONERROR);
    }

    return fSucceeded;
}


//
//   FUNCTION: OnMeasureItem(HWND, MEASUREITEMSTRUCT *)
//
//   PURPOSE: Process the WM_MEASUREITEM message. A WM_MEASUREITEM message is 
//   sent for each owner-drawn menu item the first time it is displayed. The 
//   application processes this message by selecting the font for the menu 
//   item into a device context and then determining the space required to 
//   display the menu item text in that font. The font and menu item text are 
//   both specified by the menu item's CHARMENUITEM structure (the structure 
//   defined by the application). 
//
void OnMeasureItem(HWND hWnd, MEASUREITEMSTRUCT *lpMeasureItem)
{
    // Retrieve the menu item's CHARMENUITEM structure.
    PCHARMENUITEM pcmi = (PCHARMENUITEM)lpMeasureItem->itemData;
    if (pcmi)
    {
        // Retrieve a device context for the main window. 
        HDC hdc = GetDC(hWnd); 

        // Select the font associated with the item into the main window's 
        // device context.
        HFONT hFontOld = (HFONT)SelectObject(hdc, pcmi->hFont); 

        // Retrieve the width and height of the item's string, and then copy 
        // the width and height into the MEASUREITEMSTRUCT structure's 
        // itemWidth and itemHeight members.
        SIZE size;
        GetTextExtentPoint32(hdc, pcmi->szItemText, pcmi->cchItemText, &size); 
        lpMeasureItem->itemWidth = size.cx;
        lpMeasureItem->itemHeight = size.cy;

        // Restore the original font and release the device context.
        SelectObject(hdc, hFontOld);
        ReleaseDC(hWnd, hdc);
    }
}


//
//   FUNCTION: OnDrawItem(HWND, const DRAWITEMSTRUCT *)
//
//   PURPOSE: Process the WM_DRAWITEM message by displaying the menu item 
//   text in the appropriate font. The font and menu item text are both 
//   specified by the menu item's CHARMENUITEM structure. The application 
//   selects text and background colors appropriate to the menu item's state.
//
void OnDrawItem(HWND hWnd, const DRAWITEMSTRUCT *lpDrawItem)
{
    // Retrieve the menu item's CHARMENUITEM structure.
    PCHARMENUITEM pcmi = (PCHARMENUITEM)lpDrawItem->itemData; 
    if (pcmi)
    {
        COLORREF clrPrevText, clrPrevBkgnd; 
        HFONT hFontPrev; 
        int x, y;

        // Set the appropriate foreground and background colors.
        if (lpDrawItem->itemState & ODS_SELECTED)
        {
            clrPrevText = SetTextColor(lpDrawItem->hDC, HIGHLIGHT_TEXTCOLOR);
            clrPrevBkgnd = SetBkColor(lpDrawItem->hDC, HIGHLIGHT_BACKCOLOR); 
        }
        else
        {
            clrPrevText = SetTextColor(lpDrawItem->hDC, MENUITEM_TEXTCOLOR); 
            clrPrevBkgnd = SetBkColor(lpDrawItem->hDC, MENUITEM_BACKCOLOR); 
        }

        // Determine where to draw and leave space for a check mark. 
        x = lpDrawItem->rcItem.left;
        y = lpDrawItem->rcItem.top;
        x += GetSystemMetrics(SM_CXMENUCHECK); 

        // Select the font and draw the text. 
        hFontPrev = (HFONT)SelectObject(lpDrawItem->hDC, pcmi->hFont); 
        ExtTextOut(lpDrawItem->hDC, x, y, ETO_OPAQUE, &lpDrawItem->rcItem, 
            pcmi->szItemText, pcmi->cchItemText, NULL); 

        // Restore the original font and colors. 
        SelectObject(lpDrawItem->hDC, hFontPrev);
        SetTextColor(lpDrawItem->hDC, clrPrevText);
        SetBkColor(lpDrawItem->hDC, clrPrevBkgnd);
    }
}


//
//   FUNCTION: OnDestroy(HWND)
//
//   PURPOSE: Process the WM_DESTROY message to destroy fonts and free memory. 
//   The application deletes the font and frees the application-defined 
//   CHARMENUITEM structure for each menu item.
//
void OnDestroy(HWND hWnd)
{
    HMENU hMenu = GetMenu(hWnd); 
    MENUITEMINFO mii = { sizeof(mii) };
    UINT uID = 0;
    PCHARMENUITEM pcmi = NULL;

    // Free resources associated with each menu item. 
    for (uID = IDM_REGULAR; uID <= IDM_UNDERLINE; uID++) 
    { 
        // Get the item data. 
        mii.fMask = MIIM_DATA; 
        if (GetMenuItemInfo(hMenu, uID, FALSE, &mii))
        {
            pcmi = (PCHARMENUITEM)mii.dwItemData; 

            // Destroy the font and free the item structure. 
            DeleteObject(pcmi->hFont);
            LocalFree(pcmi); 
        }
    }

    PostQuitMessage(0);
}

#pragma endregion