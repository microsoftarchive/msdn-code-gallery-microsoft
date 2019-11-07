/****************************** Module Header ******************************\
Module Name:  Wallpaper.cpp
Project:      CppSetDesktopWallpaper
Copyright (c) Microsoft Corporation.

The file defines the wallpaper helper functions.

    BOOL SupportJpgAsWallpaper();
    BOOL SupportFitFillWallpaperStyles();
    HRESULT SetDesktopWallpaper(PWSTR pszFile, WallpaperStyle style);

SetDesktopWallpaper is the key function that sets the desktop wallpaper. The 
function body is composed of configuring the wallpaper style in the registry 
and setting the wallpaper with SystemParametersInfo.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#include <stdio.h>
#include <windows.h>
#include "Wallpaper.h"


//
//   FUNCTION: SupportJpgAsWallpaper()
//
//   PURPOSE: Determine if .jpg files are supported as wallpaper in the 
//   current operating system. The .jpg wallpapers are not supported before 
//   Windows Vista.
//
BOOL SupportJpgAsWallpaper()
{
    OSVERSIONINFOEX osVersionInfoToCompare = { sizeof(OSVERSIONINFOEX) };
    osVersionInfoToCompare.dwMajorVersion = 6;
    osVersionInfoToCompare.dwMinorVersion = 0;

    ULONGLONG comparisonInfo = 0;
    BYTE conditionMask = VER_GREATER_EQUAL;
    VER_SET_CONDITION(comparisonInfo, VER_MAJORVERSION, conditionMask);
    VER_SET_CONDITION(comparisonInfo, VER_MINORVERSION, conditionMask);

    return VerifyVersionInfo(&osVersionInfoToCompare, 
        VER_MAJORVERSION | VER_MINORVERSION, comparisonInfo);
}


//
//   FUNCTION: SupportFitFillWallpaperStyles()
//
//   PURPOSE: Determine if the fit and fill wallpaper styles are supported in 
//   the current operating system. The styles are not supported before 
//   Windows 7.
//
BOOL SupportFitFillWallpaperStyles()
{
    OSVERSIONINFOEX osVersionInfoToCompare = { sizeof(OSVERSIONINFOEX) };
    osVersionInfoToCompare.dwMajorVersion = 6;
    osVersionInfoToCompare.dwMinorVersion = 1;

    ULONGLONG comparisonInfo = 0;
    BYTE conditionMask = VER_GREATER_EQUAL;
    VER_SET_CONDITION(comparisonInfo, VER_MAJORVERSION, conditionMask);
    VER_SET_CONDITION(comparisonInfo, VER_MINORVERSION, conditionMask);

    return VerifyVersionInfo(&osVersionInfoToCompare, 
        VER_MAJORVERSION | VER_MINORVERSION, comparisonInfo);
}


//
//   FUNCTION: SetDesktopWallpaper(PCWSTR, WallpaperStyle)
//
//   PURPOSE: Set the desktop wallpaper.
//
//   PARAMETERS: 
//   * pszFile - Path of the wallpaper
//   * style - Wallpaper style
//
HRESULT SetDesktopWallpaper(PWSTR pszFile, WallpaperStyle style)
{
    HRESULT hr = S_OK;

    // Set the wallpaper style and tile. 
    // Two registry values are set in the Control Panel\Desktop key.
    // TileWallpaper
    //  0: The wallpaper picture should not be tiled 
    //  1: The wallpaper picture should be tiled 
    // WallpaperStyle
    //  0:  The image is centered if TileWallpaper=0 or tiled if TileWallpaper=1
    //  2:  The image is stretched to fill the screen
    //  6:  The image is resized to fit the screen while maintaining the aspect 
    //      ratio. (Windows 7 and later)
    //  10: The image is resized and cropped to fill the screen while 
    //      maintaining the aspect ratio. (Windows 7 and later)

    // Open the HKCU\Control Panel\Desktop registry key.
    HKEY hKey = NULL;
    hr = HRESULT_FROM_WIN32(RegOpenKeyEx(HKEY_CURRENT_USER, 
        L"Control Panel\\Desktop", 0, KEY_READ | KEY_WRITE, &hKey));
    if (SUCCEEDED(hr))
    {
        PWSTR pszWallpaperStyle;
        PWSTR pszTileWallpaper;

        switch (style)
        {
        case Tile:
            pszWallpaperStyle = L"0";
            pszTileWallpaper = L"1";
            break;

        case Center:
            pszWallpaperStyle = L"0";
            pszTileWallpaper = L"0";
            break;

        case Stretch:
            pszWallpaperStyle = L"2";
            pszTileWallpaper = L"0";
            break;

        case Fit: // (Windows 7 and later)
            pszWallpaperStyle = L"6";
            pszTileWallpaper = L"0";
            break;

        case Fill: // (Windows 7 and later)
            pszWallpaperStyle = L"10";
            pszTileWallpaper = L"0";
            break;
        }

        // Set the WallpaperStyle and TileWallpaper registry values.
        DWORD cbData = lstrlen(pszWallpaperStyle) * sizeof(*pszWallpaperStyle);
        hr = HRESULT_FROM_WIN32(RegSetValueEx(hKey, L"WallpaperStyle", 0, REG_SZ, 
            reinterpret_cast<const BYTE *>(pszWallpaperStyle), cbData));
        if (SUCCEEDED(hr))
        {
            cbData = lstrlen(pszTileWallpaper) * sizeof(*pszTileWallpaper);
            hr = HRESULT_FROM_WIN32(RegSetValueEx(hKey, L"TileWallpaper", 0, REG_SZ, 
                reinterpret_cast<const BYTE *>(pszTileWallpaper), cbData));
        }

        RegCloseKey(hKey);
    }

    // Set the desktop wallpapaer by calling the Win32 API SystemParametersInfo 
    // with the SPI_SETDESKWALLPAPER desktop parameter. The changes should 
    // persist, and also be immediately visible.
    if (SUCCEEDED(hr))
    {
        if (!SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, 
            static_cast<PVOID>(pszFile), 
            SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE))
        {
            hr = HRESULT_FROM_WIN32(GetLastError());
        }
    }

    return hr;
}