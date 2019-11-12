=============================================================================
          APPLICATION : CSSetDesktopWallpaper Project Overview
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
you will get an application: CSSetDesktopWallpaper.exe. 

Step2. Run the application on a Windows 7 system. The form consists of a 
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

Step1. Create a new Visual C# Windows Forms project named 
CSSetDesktopWallpaper. Add controls to the main form. The form consists of a 
button to browse to an image, a picture box to preview the selected image, a 
group of radio buttons to configure the style, and a button to set the 
desktop wallpaper as that image.

Step2. Design the Wallpaper helper methods in Wallpaper.cs. 

    Wallpaper.SetDesktopWallpaper(string path, WallpaperStyle style)

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

    public enum WallpaperStyle
    {
        Tile,
        Center,
        Stretch,
        Fit, 
        Fill
    }

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

    RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);

    switch (style)
    {
        case WallpaperStyle.Tile:
            key.SetValue(@"WallpaperStyle", "0");
            key.SetValue(@"TileWallpaper", "1");
            break;
        case WallpaperStyle.Center:
            key.SetValue(@"WallpaperStyle", "0");
            key.SetValue(@"TileWallpaper", "0");
            break;
        case WallpaperStyle.Stretch:
            key.SetValue(@"WallpaperStyle", "2");
            key.SetValue(@"TileWallpaper", "0");
            break;
        case WallpaperStyle.Fit: // (Windows 7 and later)
            key.SetValue(@"WallpaperStyle", "6");
            key.SetValue(@"TileWallpaper", "0");
            break;
        case WallpaperStyle.Fill: // (Windows 7 and later)
            key.SetValue(@"WallpaperStyle", "10");
            key.SetValue(@"TileWallpaper", "0");
            break;
    }

    key.Close();

  2) Setting the wallpaper with SystemParametersInfo
  
  At this point, the sizing options have been set, but the actual image path 
  still needs to be set. The SystemParametersInfo function exists in the 
  user32.dll to allow you to set or retrieve hardware and configuration 
  information from your system.  The function accepts four arguments. The 
  first indicates the operation to take place, the second two parameters 
  represent data to be set, dependant on requested operation, and the final 
  parameter allows you to specify how changes are saved and/or broadcasted. 

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SystemParametersInfo(uint uiAction, uint uiParam,
        string pvParam, uint fWinIni);

  In addition to importing the function, you will need to define some 
  constants for use with it. The first constant represents the wallpaper 
  operation to take place in this sample, to be used in the first argument. 
  The other two constants will be combined together for the final argument. 

    private const uint SPI_SETDESKWALLPAPER = 20;
    private const uint SPIF_UPDATEINIFILE = 0x01;
    private const uint SPIF_SENDWININICHANGE = 0x02;

  The operation to be invoked is SPI_SETDESKWALLPAPER. It sets the desktop 
  wallpaper. The value of the pvParam parameter determines file path of the 
  new wallpaper. The file must be a bitmap (.bmp). On Windows Vista and later 
  pvParam can also specify a .jpg file. If the specified image file is 
  neither .bmp nor .jpg, or if the image is a .jpg file but the operating 
  system is Windows Server 2003 or Windows XP/2000 that does not support .jpg 
  as the desktop wallpaper, we convert the image file to .bmp and save it to 
  the %appdata%\Microsoft\Windows\Themes folder.
  
    string ext = Path.GetExtension(path);
    if ((!ext.Equals(".bmp", StringComparison.OrdinalIgnoreCase) &&
        !ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase))
        ||
        (ext.Equals(".jpg", StringComparison.OrdinalIgnoreCase) &&
        !SupportJpgAsWallpaper))
    {
        using (Image image = Image.FromFile(path))
        {
            path = String.Format(@"{0}\Microsoft\Windows\Themes\{1}.bmp",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Path.GetFileNameWithoutExtension(path));
            image.Save(path, ImageFormat.Bmp);
        }
    }
    
    // Set the desktop wallpapaer by calling the Win32 API SystemParametersInfo 
    // with the SPI_SETDESKWALLPAPER desktop parameter. The changes should 
    // persist, and also be immediately visible.
    if (!SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path,
        SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE))
    {
        throw new Win32Exception();
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


/////////////////////////////////////////////////////////////////////////////
