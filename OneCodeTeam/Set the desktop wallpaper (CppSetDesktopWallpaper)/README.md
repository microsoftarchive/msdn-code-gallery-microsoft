# Set the desktop wallpaper (CppSetDesktopWallpaper)
## Requires
- Visual Studio 2008
## License
- Apache License, Version 2.0
## Technologies
- Windows General
## Topics
- Wallpaper
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>WIN32 APPLICATION : CppSetDesktopWallpaper Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
This code sample application allows you select an image, view a preview <br>
(resized smaller to fit if necessary), select a display style among Tile, <br>
Center, Stretch, Fit (Windows 7 and later) and Fill (Windows 7 and later), <br>
and set the image as the Desktop wallpaper. <br>
<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
The following steps walk through a demonstration of the code sample.<br>
<br>
Step1. After you successfully build the sample project in Visual Studio 2008, <br>
you will get an application: CppSetDesktopWallpaper.exe. <br>
<br>
Step2. Run the application on a Windows 7 system. The window consists of a <br>
button to browse to an image, a picture box to preview the selected image, a <br>
group of radio buttons to configure the style, and a button to set the <br>
desktop wallpaper as that image.<br>
<br>
The Fit and Fill wallpaper styles are not supported before Windows 7, so if <br>
the current operating system prior to Windows 7, e.g. Windows XP, only the <br>
Tile, Center and Stretch styles are enabled. <br>
<br>
Step3. Click the Browse... button to select an image file. You will see a <br>
preview of the selected image in the picture box. <br>
<br>
Step4. Check the Title style, and click the Set Wallpaper button. The <br>
wallpaper is tiled across the screen. If you check the Stretch style, and <br>
click the Set Wallpaper button, the wallpaper is stretched vertically and <br>
horizontally to fit the screen. <br>
<br>
Step5. Close the application. <br>
<br>
</p>
<h3>Implementation:</h3>
<p style="font-family:Courier New"><br>
Step1. Create a new Visual C&#43;&#43; dialog-based Windows Application project named <br>
CppSetDesktopWallpaper. Add controls to the main dialog. The dialog consists <br>
of a button to browse to an image, a picture box to preview the selected <br>
image, a group of radio buttons to configure the style, and a button to set <br>
desktop wallpaper as that image.<br>
<br>
Step2. Design the Wallpaper helper methods in Wallpaper.h/cpp. <br>
<br>
&nbsp; &nbsp;HRESULT SetDesktopWallpaper(PWSTR pszFile, WallpaperStyle style);<br>
<br>
This is the key method that sets the desktop wallpaper. The method body is <br>
composed of configuring the wallpaper style in the registry and setting the <br>
wallpaper with SystemParametersInfo.<br>
<br>
&nbsp;1) Configuring the wallpaper style in the registry<br>
&nbsp;<br>
&nbsp;Desktop wallpaper can use one of three (or five, if the operating system is
<br>
&nbsp;Windows 7 or later) different sizing styles for display. <br>
&nbsp;<br>
&nbsp; &nbsp;Tile: &nbsp; &nbsp; &nbsp; Wallpaper is tiled across the screen.<br>
&nbsp; &nbsp;Center: &nbsp; &nbsp; Wallpaper is centered on the screen.<br>
&nbsp; &nbsp;Stretch: &nbsp; &nbsp;Wallpaper is stretched vertically and horizontally to fit the
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;screen.<br>
&nbsp; &nbsp;Fit: &nbsp; &nbsp; &nbsp; &nbsp;Wallpaper is resized to fit the screen while maintaining the
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;aspect ratio. (Windows 7 and later)<br>
&nbsp; &nbsp;Fill: &nbsp; &nbsp; &nbsp; Wallpaper is resized and cropped to fill the screen while
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;maintaining the aspect ratio. (Windows 7 and later)<br>
<br>
&nbsp;In order to support these three styles in an object-oriented way, an <br>
&nbsp;enumerated type was created.<br>
<br>
&nbsp; &nbsp;enum WallpaperStyle<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;Tile,<br>
&nbsp; &nbsp; &nbsp; &nbsp;Center,<br>
&nbsp; &nbsp; &nbsp; &nbsp;Stretch,<br>
&nbsp; &nbsp; &nbsp; &nbsp;Fit, <br>
&nbsp; &nbsp; &nbsp; &nbsp;Fill<br>
&nbsp; &nbsp;};<br>
<br>
&nbsp;Two registry values are set in the Control Panel\Desktop key. Based on <br>
&nbsp;which style is requested, numeric codes are set for the WallpaperStyle and <br>
&nbsp;TileWallpaper values:<br>
<br>
&nbsp;<a target="_blank" href="&lt;a target=" href="http://msdn.microsoft.com/en-us/library/bb773190(VS.85).aspx#desktop">http://msdn.microsoft.com/en-us/library/bb773190(VS.85).aspx#desktop</a>'&gt;<a target="_blank" href="http://msdn.microsoft.com/en-us/library/bb773190(VS.85).aspx#desktop">http://msdn.microsoft.com/en-us/library/bb773190(VS.85).aspx#desktop</a><br>
<br>
&nbsp;TileWallpaper<br>
&nbsp; &nbsp;0: The wallpaper picture should not be tiled <br>
&nbsp; &nbsp;1: The wallpaper picture should be tiled <br>
<br>
&nbsp;WallpaperStyle<br>
&nbsp; &nbsp;0: &nbsp;The image is centered if TileWallpaper=0 or tiled if TileWallpaper=1<br>
&nbsp; &nbsp;2: &nbsp;The image is stretched to fill the screen<br>
&nbsp; &nbsp;6: &nbsp;The image is resized to fit the screen while maintaining the aspect
<br>
&nbsp; &nbsp; &nbsp; &nbsp;ratio. (Windows 7 and later)<br>
&nbsp; &nbsp;10: The image is resized and cropped to fill the screen while maintaining
<br>
&nbsp; &nbsp; &nbsp; &nbsp;the aspect ratio. (Windows 7 and later)<br>
<br>
&nbsp; &nbsp;// Open the HKCU\Control Panel\Desktop registry key.<br>
&nbsp; &nbsp;HKEY hKey = NULL;<br>
&nbsp; &nbsp;hr = HRESULT_FROM_WIN32(RegOpenKeyEx(HKEY_CURRENT_USER, <br>
&nbsp; &nbsp; &nbsp; &nbsp;L&quot;Control Panel\\Desktop&quot;, 0, KEY_READ | KEY_WRITE, &hKey));<br>
&nbsp; &nbsp;if (SUCCEEDED(hr))<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;PWSTR pszWallpaperStyle;<br>
&nbsp; &nbsp; &nbsp; &nbsp;PWSTR pszTileWallpaper;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;switch (style)<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;case Tile:<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;pszWallpaperStyle = L&quot;0&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;pszTileWallpaper = L&quot;1&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;break;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;case Center:<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;pszWallpaperStyle = L&quot;0&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;pszTileWallpaper = L&quot;0&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;break;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;case Stretch:<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;pszWallpaperStyle = L&quot;2&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;pszTileWallpaper = L&quot;0&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;break;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;case Fit: // (Windows 7 and later)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;pszWallpaperStyle = L&quot;6&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;pszTileWallpaper = L&quot;0&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;break;<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;case Fill: // (Windows 7 and later)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;pszWallpaperStyle = L&quot;10&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;pszTileWallpaper = L&quot;0&quot;;<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;break;<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;// Set the WallpaperStyle and TileWallpaper registry values.<br>
&nbsp; &nbsp; &nbsp; &nbsp;DWORD cbData = lstrlen(pszWallpaperStyle) * sizeof(*pszWallpaperStyle);<br>
&nbsp; &nbsp; &nbsp; &nbsp;hr = HRESULT_FROM_WIN32(RegSetValueEx(hKey, L&quot;WallpaperStyle&quot;, 0, REG_SZ,
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;reinterpret_cast&lt;const BYTE *&gt;(pszWallpaperStyle), cbData));<br>
&nbsp; &nbsp; &nbsp; &nbsp;if (SUCCEEDED(hr))<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;cbData = lstrlen(pszTileWallpaper) * sizeof(*pszTileWallpaper);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;hr = HRESULT_FROM_WIN32(RegSetValueEx(hKey, L&quot;TileWallpaper&quot;, 0, REG_SZ,
<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;reinterpret_cast&lt;const BYTE *&gt;(pszTileWallpaper), cbData));<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp; &nbsp; &nbsp;RegCloseKey(hKey);<br>
&nbsp; &nbsp;}<br>
<br>
&nbsp;2) Setting the wallpaper with SystemParametersInfo<br>
&nbsp;<br>
&nbsp;At this point, the sizing options have been set, but the actual image path <br>
&nbsp;still needs to be set. The SystemParametersInfo function exists in the <br>
&nbsp;user32.dll to allow you to set or retrieve hardware and configuration <br>
&nbsp;information from your system. &nbsp;The function accepts four arguments. The
<br>
&nbsp;first indicates the operation to take place, the second two parameters <br>
&nbsp;represent data to be set, dependant on requested operation, and the final <br>
&nbsp;parameter allows you to specify how changes are saved and/or broadcasted. <br>
&nbsp;<br>
&nbsp;The operation to be invoked is SPI_SETDESKWALLPAPER. It sets the desktop <br>
&nbsp;wallpaper. The value of the pvParam parameter determines file path of the <br>
&nbsp;new wallpaper. The file must be a bitmap (.bmp). On Windows Vista and later
<br>
&nbsp;pvParam can also specify a .jpg file.<br>
&nbsp;<br>
&nbsp; &nbsp;// Set the desktop wallpapaer by calling the Win32 API SystemParametersInfo
<br>
&nbsp; &nbsp;// with the SPI_SETDESKWALLPAPER desktop parameter. The changes should
<br>
&nbsp; &nbsp;// persist, and also be immediately visible.<br>
&nbsp; &nbsp;if (SUCCEEDED(hr))<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;if (!SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;static_cast&lt;PVOID&gt;(pszFile), <br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE))<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;hr = HRESULT_FROM_WIN32(GetLastError());<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp;}<br>
<br>
&nbsp;Setting pvParam to SETWALLPAPER_DEFAULT or null in the above call reverts <br>
&nbsp;to the default wallpaper. <br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
MSDN: SystemParametersInfo Function<br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/ms724947(VS.85).aspx">http://msdn.microsoft.com/en-us/library/ms724947(VS.85).aspx</a><br>
<br>
MSDN: Theme File Format<br>
<a target="_blank" href="&lt;a target=" href="http://msdn.microsoft.com/en-us/library/bb773190(VS.85).aspx#desktop">http://msdn.microsoft.com/en-us/library/bb773190(VS.85).aspx#desktop</a>'&gt;<a target="_blank" href="http://msdn.microsoft.com/en-us/library/bb773190(VS.85).aspx#desktop">http://msdn.microsoft.com/en-us/library/bb773190(VS.85).aspx#desktop</a><br>
<br>
Setting Wallpaper<br>
<a target="_blank" href="http://blogs.msdn.com/b/coding4fun/archive/2006/10/31/912569.aspx">http://blogs.msdn.com/b/coding4fun/archive/2006/10/31/912569.aspx</a><br>
<br>
How to load graphics files and display them in Visual C&#43;&#43;<br>
<a target="_blank" href="http://support.microsoft.com/kb/218972m">http://support.microsoft.com/kb/218972m</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
