# Set the desktop wallpaper (CSSetDesktopWallpaper)
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
<h2>APPLICATION : CSSetDesktopWallpaper Project Overview</h2>
<p style="font-family:Courier New"></p>
<h3>Summary: </h3>
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
you will get an application: CSSetDesktopWallpaper.exe. <br>
<br>
Step2. Run the application on a Windows 7 system. The form consists of a <br>
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
Step1. Create a new Visual C# Windows Forms project named <br>
CSSetDesktopWallpaper. Add controls to the main form. The form consists of a <br>
button to browse to an image, a picture box to preview the selected image, a <br>
group of radio buttons to configure the style, and a button to set the <br>
desktop wallpaper as that image.<br>
<br>
Step2. Design the Wallpaper helper methods in Wallpaper.cs. <br>
<br>
&nbsp; &nbsp;Wallpaper.SetDesktopWallpaper(string path, WallpaperStyle style)<br>
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
&nbsp; &nbsp;public enum WallpaperStyle<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;Tile,<br>
&nbsp; &nbsp; &nbsp; &nbsp;Center,<br>
&nbsp; &nbsp; &nbsp; &nbsp;Stretch,<br>
&nbsp; &nbsp; &nbsp; &nbsp;Fit, <br>
&nbsp; &nbsp; &nbsp; &nbsp;Fill<br>
&nbsp; &nbsp;}<br>
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
&nbsp; &nbsp;RegistryKey key = Registry.CurrentUser.OpenSubKey(@&quot;Control Panel\Desktop&quot;, true);<br>
<br>
&nbsp; &nbsp;switch (style)<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;case WallpaperStyle.Tile:<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;key.SetValue(@&quot;WallpaperStyle&quot;, &quot;0&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;key.SetValue(@&quot;TileWallpaper&quot;, &quot;1&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;break;<br>
&nbsp; &nbsp; &nbsp; &nbsp;case WallpaperStyle.Center:<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;key.SetValue(@&quot;WallpaperStyle&quot;, &quot;0&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;key.SetValue(@&quot;TileWallpaper&quot;, &quot;0&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;break;<br>
&nbsp; &nbsp; &nbsp; &nbsp;case WallpaperStyle.Stretch:<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;key.SetValue(@&quot;WallpaperStyle&quot;, &quot;2&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;key.SetValue(@&quot;TileWallpaper&quot;, &quot;0&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;break;<br>
&nbsp; &nbsp; &nbsp; &nbsp;case WallpaperStyle.Fit: // (Windows 7 and later)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;key.SetValue(@&quot;WallpaperStyle&quot;, &quot;6&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;key.SetValue(@&quot;TileWallpaper&quot;, &quot;0&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;break;<br>
&nbsp; &nbsp; &nbsp; &nbsp;case WallpaperStyle.Fill: // (Windows 7 and later)<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;key.SetValue(@&quot;WallpaperStyle&quot;, &quot;10&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;key.SetValue(@&quot;TileWallpaper&quot;, &quot;0&quot;);<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;break;<br>
&nbsp; &nbsp;}<br>
<br>
&nbsp; &nbsp;key.Close();<br>
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
<br>
&nbsp; &nbsp;[DllImport(&quot;user32.dll&quot;, CharSet = CharSet.Unicode, SetLastError = true)]<br>
&nbsp; &nbsp;[return: MarshalAs(UnmanagedType.Bool)]<br>
&nbsp; &nbsp;private static extern bool SystemParametersInfo(uint uiAction, uint uiParam,<br>
&nbsp; &nbsp; &nbsp; &nbsp;string pvParam, uint fWinIni);<br>
<br>
&nbsp;In addition to importing the function, you will need to define some <br>
&nbsp;constants for use with it. The first constant represents the wallpaper <br>
&nbsp;operation to take place in this sample, to be used in the first argument. <br>
&nbsp;The other two constants will be combined together for the final argument. <br>
<br>
&nbsp; &nbsp;private const uint SPI_SETDESKWALLPAPER = 20;<br>
&nbsp; &nbsp;private const uint SPIF_UPDATEINIFILE = 0x01;<br>
&nbsp; &nbsp;private const uint SPIF_SENDWININICHANGE = 0x02;<br>
<br>
&nbsp;The operation to be invoked is SPI_SETDESKWALLPAPER. It sets the desktop <br>
&nbsp;wallpaper. The value of the pvParam parameter determines file path of the <br>
&nbsp;new wallpaper. The file must be a bitmap (.bmp). On Windows Vista and later
<br>
&nbsp;pvParam can also specify a .jpg file. If the specified image file is <br>
&nbsp;neither .bmp nor .jpg, or if the image is a .jpg file but the operating <br>
&nbsp;system is Windows Server 2003 or Windows XP/2000 that does not support .jpg
<br>
&nbsp;as the desktop wallpaper, we convert the image file to .bmp and save it to <br>
&nbsp;the %appdata%\Microsoft\Windows\Themes folder.<br>
&nbsp;<br>
&nbsp; &nbsp;string ext = Path.GetExtension(path);<br>
&nbsp; &nbsp;if ((!ext.Equals(&quot;.bmp&quot;, StringComparison.OrdinalIgnoreCase) &&<br>
&nbsp; &nbsp; &nbsp; &nbsp;!ext.Equals(&quot;.jpg&quot;, StringComparison.OrdinalIgnoreCase))<br>
&nbsp; &nbsp; &nbsp; &nbsp;||<br>
&nbsp; &nbsp; &nbsp; &nbsp;(ext.Equals(&quot;.jpg&quot;, StringComparison.OrdinalIgnoreCase) &&<br>
&nbsp; &nbsp; &nbsp; &nbsp;!SupportJpgAsWallpaper))<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;using (Image image = Image.FromFile(path))<br>
&nbsp; &nbsp; &nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;path = String.Format(@&quot;{0}\Microsoft\Windows\Themes\{1}.bmp&quot;,<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;Path.GetFileNameWithoutExtension(path));<br>
&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp;image.Save(path, ImageFormat.Bmp);<br>
&nbsp; &nbsp; &nbsp; &nbsp;}<br>
&nbsp; &nbsp;}<br>
&nbsp; &nbsp;<br>
&nbsp; &nbsp;// Set the desktop wallpapaer by calling the Win32 API SystemParametersInfo
<br>
&nbsp; &nbsp;// with the SPI_SETDESKWALLPAPER desktop parameter. The changes should
<br>
&nbsp; &nbsp;// persist, and also be immediately visible.<br>
&nbsp; &nbsp;if (!SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path,<br>
&nbsp; &nbsp; &nbsp; &nbsp;SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE))<br>
&nbsp; &nbsp;{<br>
&nbsp; &nbsp; &nbsp; &nbsp;throw new Win32Exception();<br>
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
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo">
</a></div>
