/****************************** Module Header ******************************\
Module Name:  CppSetDesktopWallpaper.cpp
Project:      CppSetDesktopWallpaper
Copyright (c) Microsoft Corporation.

This code sample application allows you select an image, view a preview 
(resized smaller to fit if necessary), select a display style among Tile, 
Center, Stretch, Fit (Windows 7 and later) and Fill (Windows 7 and later), 
and set the image as the Desktop wallpaper. 

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

#include <strsafe.h>
#include <shlobj.h>
#include <shlwapi.h>

#include <OleCtl.h>
#include <Commctrl.h>
#pragma comment(lib, "Comctl32.lib")

#include "Wallpaper.h"

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
//   * hr - the HRESULT value. 
//
void ReportError(LPCWSTR pszFunction, HRESULT hr = E_FAIL)
{
    wchar_t szMessage[200];
    if (hr == E_FAIL)
    {
        if (SUCCEEDED(StringCchPrintf(szMessage, ARRAYSIZE(szMessage), 
            L"%s failed", pszFunction)))
        {
            MessageBox(NULL, szMessage, L"Error", MB_ICONERROR);
        }
    }
    else
    {
        if (SUCCEEDED(StringCchPrintf(szMessage, ARRAYSIZE(szMessage), 
            L"%s failed w/hr 0x%08lx", pszFunction, hr)))
        {
            MessageBox(NULL, szMessage, L"Error", MB_ICONERROR);
        }
    }
}


wchar_t g_szWallpaperFileName[MAX_PATH] = {};
LPPICTURE g_pWallpaper = NULL;


//
//   FUNCTION: PctPreviewProc(HWND, UINT, WPARAM, LPARAM, UINT_PTR, DWORD_PTR)
//
//   PURPOSE:  The new procedure that processes messages for the picture 
//   preview control. Every time a message is received by the new window 
//   procedure, a subclass ID and reference data are included.
//
LRESULT CALLBACK PctPreviewProc(HWND hWnd, UINT message, WPARAM wParam, 
                                LPARAM lParam, UINT_PTR uIdSubclass, 
                                DWORD_PTR dwRefData)
{
    switch (message)
    {
    case WM_PAINT: // Owner-draw
        {
            PAINTSTRUCT ps;
            HDC hDC = BeginPaint(hWnd, &ps);

            // Do painting here...
            if (g_pWallpaper)
            {
                // Get the width and height of the picture.
                LONG hmWidth;
                LONG hmHeight;
                g_pWallpaper->get_Width(&hmWidth);
                g_pWallpaper->get_Height(&hmHeight);

                // Convert himetric to pixels.
                const int HIMETRIC_INCH = 2540;
                LONG nWidth = MulDiv(hmWidth, GetDeviceCaps(hDC, LOGPIXELSX), 
                    HIMETRIC_INCH);
                LONG nHeight = MulDiv(hmHeight, GetDeviceCaps(hDC, LOGPIXELSY), 
                    HIMETRIC_INCH);

                // Get the rect to display the image preview.
                RECT rc;
                GetClientRect(hWnd, &rc);
                if (nWidth < rc.right && nHeight < rc.bottom)
                {
                    rc.right = nWidth;
                    rc.bottom = nHeight;
                }
                else
                {
                    float wallpaperRatio = (float)nWidth / (float)nHeight;
                    float pctPreviewRatio = (float)rc.right / (float)rc.bottom;

                    if (wallpaperRatio >= pctPreviewRatio)
                    {
                        rc.bottom = (LONG)(rc.right / wallpaperRatio);
                    }
                    else
                    {
                        rc.right = (LONG)(rc.bottom * wallpaperRatio);
                    }
                }

                // Display the picture using IPicture::Render.
                g_pWallpaper->Render(hDC, 0, 0, rc.right, rc.bottom, 
                    0, hmHeight, hmWidth, -hmHeight, &rc);
            }

            EndPaint(hWnd, &ps);
        }
        return 0;

    default:
        return DefSubclassProc(hWnd, message, wParam, lParam);
    }
}


// 
//   FUNCTION: OnInitDialog(HWND, HWND, LPARAM)
//
//   PURPOSE: Process the WM_INITDIALOG message. 
//
BOOL OnInitDialog(HWND hWnd, HWND hwndFocus, LPARAM lParam)
{
    // If the current operating system is not Windows 7 or later, disable the 
    // Fit and Fill wallpaper styles.
    if (!SupportFitFillWallpaperStyles())
    {
        // Disable the Fit and Fill wallpaper styles.
        Button_Enable(GetDlgItem(hWnd, IDC_RADIO_FIT), FALSE);
        Button_Enable(GetDlgItem(hWnd, IDC_RADIO_FILL), FALSE);
    }

    Button_SetCheck(GetDlgItem(hWnd, IDC_RADIO_STRETCH), TRUE);

    // Subclass the picture preview control.
    HWND hPctPreview = GetDlgItem(hWnd, IDC_STATIC_PREVIEW);
    UINT_PTR uIdSubclass = 0;
    if (!SetWindowSubclass(hPctPreview, PctPreviewProc, uIdSubclass, 0))
    {
        ReportError(L"SetWindowSubclass");
        return FALSE;
    }

    return TRUE;
}


HRESULT LoadPicture(PCWSTR pszFile, LPPICTURE *ppPicture)
{
    HRESULT hr = S_OK;
    HANDLE hFile = INVALID_HANDLE_VALUE;
    HGLOBAL hGlobal = NULL;
    PVOID pData = NULL;
    LPSTREAM pstm = NULL;

    // Open the file.
    hFile = CreateFile(pszFile, GENERIC_READ, 0, NULL, OPEN_EXISTING, 0, NULL);
    if (hFile == INVALID_HANDLE_VALUE)
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
        goto Error;
    }

    // Get the file size.
    DWORD dwFileSize = GetFileSize(hFile, NULL);
    if (dwFileSize == INVALID_FILE_SIZE)
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
        goto Error;
    }

    // Allocate memory based on the file size using GlobalAlloc.
    hGlobal = GlobalAlloc(GMEM_MOVEABLE, dwFileSize);
    if (hGlobal == NULL)
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
        goto Error;
    }

    // Lock the global memory object and return a pointer to the block.
    pData = GlobalLock(hGlobal);
    if (pData == NULL)
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
        goto Error;
    }

    // Read the file and store the data in the global memory.
    DWORD dwBytesRead = 0;
    if (!ReadFile(hFile, pData, dwFileSize, &dwBytesRead, NULL))
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
        goto Error;
    }

    // Create IStream* from the global memory.
    hr = CreateStreamOnHGlobal(hGlobal, TRUE, &pstm);
    if (FAILED(hr))
    {
        goto Error;
    }

    // Create IPicture from the image file.
    hr = OleLoadPicture(pstm, dwFileSize, FALSE, IID_PPV_ARGS(ppPicture));

Error:
    // Clean up the resources in a centralized place.
    if (hFile != INVALID_HANDLE_VALUE)
    {
        CloseHandle(hFile);
        hFile = INVALID_HANDLE_VALUE;
    }
    if (hGlobal != NULL)
    {
        if (pData != NULL)
        {
            GlobalUnlock(hGlobal);
            pData = NULL;
        }
        GlobalFree(hGlobal);
        hGlobal = NULL;
    }
    if (pstm != NULL)
    {
        pstm->Release();
        pstm = NULL;
    }

    return hr;
}


// Prior to Windows Vista, only .bmp files are supported as wallpaper.
const COMDLG_FILTERSPEC c_rgOldFileTypes[] =
{
    { L"Bitmap Files",     L"*.bmp" }
};

const COMDLG_FILTERSPEC c_rgNewFileTypes[] = 
{
    { L"All Supported Files",   L"*.bmp;*.jpg" },
    { L"Bitmap Files",          L"*.bmp" },
    { L"Jpg Files",             L"*.jpg" }
};


//
//   FUNCTION: OnBrowseWallpaperButtonClick(HWND)
//
//   PURPOSE: The function is invoked when the "Browse..." button is clicked.
//
void OnBrowseWallpaperButtonClick(HWND hWnd)
{
    HRESULT hr = CoInitializeEx(NULL, COINIT_APARTMENTTHREADED);
    if (FAILED(hr))
    {
        ReportError(L"CoInitializeEx", hr);
        return;
    }

    // If running under Vista or later operating system, use the new common 
    // item dialog.
    OSVERSIONINFO vi = { sizeof(vi) };
    GetVersionEx(&vi);
    if (vi.dwMajorVersion >= 6)
    {
        // Create and show a common open file dialog and allow users to select 
        // an image file.
        IFileDialog *pfd = NULL;
        hr = CoCreateInstance(CLSID_FileOpenDialog, NULL, CLSCTX_INPROC_SERVER, 
            IID_PPV_ARGS(&pfd));
        if (SUCCEEDED(hr))
        {
            // Set the title of the dialog.
            hr = pfd->SetTitle(L"Select Wallpaper");

            // Specify file filters for the file dialog.
            if (SUCCEEDED(hr))
            {
                if (SupportJpgAsWallpaper())
                {
                    hr = pfd->SetFileTypes(ARRAYSIZE(c_rgNewFileTypes), 
                        c_rgNewFileTypes);
                }
                else
                {
                    hr = pfd->SetFileTypes(ARRAYSIZE(c_rgOldFileTypes), 
                        c_rgOldFileTypes);
                }

                if (SUCCEEDED(hr))
                {
                    // Set the selected file type index.
                    hr = pfd->SetFileTypeIndex(1);
                }
            }

            // Show the open file dialog.
            if (SUCCEEDED(hr))
            {
                hr = pfd->Show(hWnd);
                if (SUCCEEDED(hr))
                {
                    // Get the result of the open file dialog.
                    IShellItem *psiResult = NULL;
                    hr = pfd->GetResult(&psiResult);
                    if (SUCCEEDED(hr))
                    {
                        PWSTR pszFile = NULL;
                        hr = psiResult->GetDisplayName(SIGDN_FILESYSPATH, &pszFile);
                        if (SUCCEEDED(hr))
                        {
                            hr = StringCchCopy(g_szWallpaperFileName, 
                                ARRAYSIZE(g_szWallpaperFileName), 
                                pszFile);
                            CoTaskMemFree(pszFile);
                        }
                        psiResult->Release();
                    }
                }
            }
            pfd->Release();
        }
    }
    else
    {
        // Before Windows Vista, the common item dialogs are not supported. 
        // Use the GetOpenFileName function from the Common Dialog Box Library.
        wchar_t szFile[MAX_PATH];
        OPENFILENAME ofn = { sizeof(ofn) };
        ofn.hwndOwner = hWnd;
        ofn.lpstrFile = szFile;
        ofn.lpstrFile[0] = L'\0';
        ofn.nMaxFile = ARRAYSIZE(szFile);
        if (SupportJpgAsWallpaper())
        {
            ofn.lpstrFilter = 
                L"All Supported Files (*.bmp;*.jpg)\0*.bmp;*.jpg\0" \
                L"Bitmap Files (*.bmp)\0*.bmp\0" \
                L"Jpg Files (*.jpg)\0*.jpg\0";
        }
        else
        {
            ofn.lpstrFilter = L"Bitmap Files (*.bmp)\0*.bmp\0";
        }
        ofn.nFilterIndex = 1;
        ofn.lpstrFileTitle = NULL;
        ofn.nMaxFileTitle = 0;
        ofn.lpstrInitialDir = NULL;
        ofn.lpstrTitle = L"Select Wallpaper";
        ofn.Flags = OFN_PATHMUSTEXIST | OFN_FILEMUSTEXIST;
        
        // Display the Open dialog box. 
        if (GetOpenFileName(&ofn))
        {
            hr = StringCchCopy(g_szWallpaperFileName, 
                ARRAYSIZE(g_szWallpaperFileName), szFile);
        }
        else
        {
            // The user pressed the Cancel button.
            hr = HRESULT_FROM_WIN32(ERROR_CANCELLED);
        }
    }

    // Fill out the wallpaper file name; load the wallpaper.
    if (SUCCEEDED(hr))
    {
        HWND hEditWallpaper = GetDlgItem(hWnd, IDC_EDIT_WALLPAPER);
        Edit_SetText(hEditWallpaper, g_szWallpaperFileName);

        // Unload the original picture if any.
        if (g_pWallpaper)
        {
            g_pWallpaper->Release();
            g_pWallpaper = NULL;
        }

        // Load the new picture.
        hr = LoadPicture(g_szWallpaperFileName, &g_pWallpaper);

    }

    if (SUCCEEDED(hr))
    {
        InvalidateRect(hWnd, NULL, TRUE);
    }
    else
    {
        if (hr != HRESULT_FROM_WIN32(ERROR_CANCELLED))
        {
            ReportError(L"OnBrowseWallpaperButtonClick", hr);
        }
    }

    CoUninitialize();
}


//
//   FUNCTION: GetSelectedWallpaperStyle(HWND)
//
//   PURPOSE: Get the selected wallpaper style.
//
WallpaperStyle GetSelectedWallpaperStyle(HWND hWnd)
{
    WallpaperStyle style = Stretch;
    if (BST_CHECKED == Button_GetCheck(GetDlgItem(hWnd, IDC_RADIO_TILE)))
    {
        style = Tile;
    }
    else if (BST_CHECKED == Button_GetCheck(GetDlgItem(hWnd, IDC_RADIO_CENTER)))
    {
        style = Center;
    }
    else if (BST_CHECKED == Button_GetCheck(GetDlgItem(hWnd, IDC_RADIO_STRETCH)))
    {
        style = Stretch;
    }
    else if (BST_CHECKED == Button_GetCheck(GetDlgItem(hWnd, IDC_RADIO_FIT)))
    {
        style = Fit;
    }
    else if (BST_CHECKED == Button_GetCheck(GetDlgItem(hWnd, IDC_RADIO_FILL)))
    {
        style = Fill;
    }
    return style;
}


//
//   FUNCTION: OnSetWallpaperButtonClick(HWND)
//
//   PURPOSE: The function is invoked when the "Set Wallpaper" button is 
//   clicked.
//
void OnSetWallpaperButtonClick(HWND hWnd)
{
    if (wcslen(g_szWallpaperFileName) > 0)
    {
        // Get the selected wallpaper style.
        WallpaperStyle style = GetSelectedWallpaperStyle(hWnd);
        HRESULT hr = SetDesktopWallpaper(g_szWallpaperFileName, style);
        if (FAILED(hr))
        {
            ReportError(L"SetDesktopWallpaper", hr);
        }
    }
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
    case IDC_BUTTON_BROWSE:
        OnBrowseWallpaperButtonClick(hWnd);
        break;

    case IDC_BUTTON_SETWALLPAPER:
        OnSetWallpaperButtonClick(hWnd);
        break;

    case IDOK:
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