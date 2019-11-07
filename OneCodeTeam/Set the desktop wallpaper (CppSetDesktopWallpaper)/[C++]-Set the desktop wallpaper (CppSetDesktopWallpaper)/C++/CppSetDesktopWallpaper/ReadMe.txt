=============================================================================
        WIN32 APPLICATION : CppSetDesktopWallpaper Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

This code sample application allows you select an image, view a preview 
(resized smaller to fit if necessary), select a display style among Tile, 
Center, Stretch, Fit (Windows 7 and later) and Fill (Windows 7 and later), 
and set the image as the Desktop wallpaper. 


/////////////////////////////////////////////////////////////////////////////
Demo:

The following steps walk through a demonstration of the code sample.

Step1. After you successfully build the sample project in Visual Studio 2008, 
you will get an application: CppSetDesktopWallpaper.exe. 

Step2. Run the application on a Windows 7 system. The window consists of a 
button to browse to an image, a picture box to preview the selected image, a 
group of radio buttons to configure the style, and a button to set the 
desktop wallpaper as that image.

The Fit and Fill wallpaper styles are not supported before Windows 7, so if 
the current operating system prior to Windows 7, e.g. Windows XP, only the 
Tile, Center and Stretch styles are enabled. 

Step3. Click the Browse... button to select an image file. You will see a 
preview of the selected image in the picture box. 

Step4. Check the Title style, and click the Set Wallpaper button. The 
wallpaper is tiled across the screen. If you check the Stretch style, and 
click the Set Wallpaper button, the wallpaper is stretched vertically and 
horizontally to fit the screen. 

Step5. Close the application. 


/////////////////////////////////////////////////////////////////////////////
Implementation:

Step1. Create a new Visual C++ dialog-based Windows Application project named 
CppSetDesktopWallpaper. Add controls to the main dialog. The dialog consists 
of a button to browse to an image, a picture box to preview the selected 
image, a group of radio buttons to configure the style, and a button to set 
desktop wallpaper as that image.

Step2. Design the Wallpaper helper methods in Wallpaper.h/cpp. 

    HRESULT SetDesktopWallpaper(PWSTR pszFile, WallpaperStyle style);

This is the key method that sets the desktop wallpaper. The method body is 
composed of configuring the wallpaper style in the registry and setting the 
wallpaper with SystemParametersInfo.

  1) Configuring the wallpaper style in the registry
  
  Desktop wallpaper can use one of three (or five, if the operating system is 
  Windows 7 or later) different sizing styles for display. 
  
    Tile:       Wallpaper is tiled across the screen.
    Center:     Wallpaper is centered on the screen.
    Stretch:    Wallpaper is stretched vertically and horizontally to fit the 
                screen.
    Fit:        Wallpaper is resized to fit the screen while maintaining the 
                aspect ratio. (Windows 7 and later)
    Fill:       Wallpaper is resized and cropped to fill the screen while 
                maintaining the aspect ratio. (Windows 7 and later)

  In order to support these three styles in an object-oriented way, an 
  enumerated type was created.

    enum WallpaperStyle
    {
        Tile,
        Center,
        Stretch,
        Fit, 
        Fill
    };

  Two registry values are set in the Control Panel\Desktop key. Based on 
  which style is requested, numeric codes are set for the WallpaperStyle and 
  TileWallpaper values:

  http://msdn.microsoft.com/en-us/library/bb773190(VS.85).aspx#desktop

  TileWallpaper
    0: The wallpaper picture should not be tiled 
    1: The wallpaper picture should be tiled 

  WallpaperStyle
    0:  The image is centered if TileWallpaper=0 or tiled if TileWallpaper=1
    2:  The image is stretched to fill the screen
    6:  The image is resized to fit the screen while maintaining the aspect 
        ratio. (Windows 7 and later)
    10: The image is resized and cropped to fill the screen while maintaining 
        the aspect ratio. (Windows 7 and later)

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

  2) Setting the wallpaper with SystemParametersInfo
  
  At this point, the sizing options have been set, but the actual image path 
  still needs to be set. The SystemParametersInfo function exists in the 
  user32.dll to allow you to set or retrieve hardware and configuration 
  information from your system.  The function accepts four arguments. The 
  first indicates the operation to take place, the second two parameters 
  represent data to be set, dependant on requested operation, and the final 
  parameter allows you to specify how changes are saved and/or broadcasted. 
  
  The operation to be invoked is SPI_SETDESKWALLPAPER. It sets the desktop 
  wallpaper. The value of the pvParam parameter determines file path of the 
  new wallpaper. The file must be a bitmap (.bmp). On Windows Vista and later 
  pvParam can also specify a .jpg file.
  
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

  Setting pvParam to SETWALLPAPER_DEFAULT or null in the above call reverts 
  to the default wallpaper. 


/////////////////////////////////////////////////////////////////////////////
References:

MSDN: SystemParametersInfo Function
http://msdn.microsoft.com/en-us/library/ms724947(VS.85).aspx

MSDN: Theme File Format
http://msdn.microsoft.com/en-us/library/bb773190(VS.85).aspx#desktop

Setting Wallpaper
http://blogs.msdn.com/b/coding4fun/archive/2006/10/31/912569.aspx

How to load graphics files and display them in Visual C++
http://support.microsoft.com/kb/218972m


/////////////////////////////////////////////////////////////////////////////
