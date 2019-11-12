/****************************** Module Header ******************************\
Module Name:  Wallpaper.h
Project:      CppSetDesktopWallpaper
Copyright (c) Microsoft Corporation.

The file declares the wallpaper helper functions.

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

#pragma once


//
//   FUNCTION: SupportJpgAsWallpaper()
//
//   PURPOSE: Determine if .jpg files are supported as wallpaper in the 
//   current operating system. The .jpg wallpapers are not supported before 
//   Windows Vista.
//
BOOL SupportJpgAsWallpaper();


//
//   FUNCTION: SupportFitFillWallpaperStyles()
//
//   PURPOSE: Determine if the fit and fill wallpaper styles are supported in 
//   the current operating system. The styles are not supported before 
//   Windows 7.
//
BOOL SupportFitFillWallpaperStyles();


enum WallpaperStyle
{
    Tile,
    Center,
    Stretch,
    Fit, 
    Fill
};


//
//   FUNCTION: SetDesktopWallpaper(PCWSTR, WallpaperStyle)
//
//   PURPOSE: Set the desktop wallpaper.
//
//   PARAMETERS: 
//   * pszFile - Path of the wallpaper
//   * style - Wallpaper style
//
HRESULT SetDesktopWallpaper(PWSTR pszFile, WallpaperStyle style);