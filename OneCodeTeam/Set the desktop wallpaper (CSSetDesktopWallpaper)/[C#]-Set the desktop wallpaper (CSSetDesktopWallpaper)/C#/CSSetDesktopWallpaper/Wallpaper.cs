/*********************************** Module Header ***********************************\
Module Name:  Wallpaper.cs
Project:      CSSetDesktopWallpaper
Copyright (c) Microsoft Corporation.

The file defines a wallpaper helper class.

    Wallpaper.SetDesktopWallpaper(string path, WallpaperStyle style)

This is the key method that sets the desktop wallpaper. The method body is composed of 
configuring the wallpaper style in the registry and setting the wallpaper with 
SystemParametersInfo.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*************************************************************************************/

using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Microsoft.Win32;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;


namespace CSSetDesktopWallpaper
{
    public static class Wallpaper
    {
        /// <summary>
        /// Determine if .jpg files are supported as wallpaper in the current 
        /// operating system. The .jpg wallpapers are not supported before 
        /// Windows Vista.
        /// </summary>
        public static bool SupportJpgAsWallpaper
        {
            get
            {
                return (Environment.OSVersion.Version >= new Version(6, 0));
            }
        }


        /// <summary>
        /// Determine if the fit and fill wallpaper styles are supported in 
        /// the current operating system. The styles are not supported before 
        /// Windows 7.
        /// </summary>
        public static bool SupportFitFillWallpaperStyles
        {
            get
            {
                return (Environment.OSVersion.Version >= new Version(6, 1));
            }
        }


        /// <summary>
        /// Set the desktop wallpaper.
        /// </summary>
        /// <param name="path">Path of the wallpaper</param>
        /// <param name="style">Wallpaper style</param>
        public static void SetDesktopWallpaper(string path, WallpaperStyle style)
        {
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

            // If the specified image file is neither .bmp nor .jpg, - or -
            // if the image is a .jpg file but the operating system is Windows Server 
            // 2003 or Windows XP/2000 that does not support .jpg as the desktop 
            // wallpaper, convert the image file to .bmp and save it to the 
            // %appdata%\Microsoft\Windows\Themes folder.
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
        }


        private const uint SPI_SETDESKWALLPAPER = 20;
        private const uint SPIF_UPDATEINIFILE = 0x01;
        private const uint SPIF_SENDWININICHANGE = 0x02;

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam,
            string pvParam, uint fWinIni);
    }


    public enum WallpaperStyle
    {
        Tile,
        Center,
        Stretch,
        Fit,
        Fill
    }
}