/*********************************** Module Header ***********************************\
Module Name:  MainForm.cs
Project:      CSSetDesktopWallpaper
Copyright (c) Microsoft Corporation.

This code sample application allows you select an image, view a preview (resized 
smaller to fit if necessary), select a display style among Tile, Center, Stretch, Fit 
(Windows 7 and later) and Fill (Windows 7 and later), and set the image as the Desktop 
wallpaper. 

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*************************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CSSetDesktopWallpaper.Properties;


namespace CSSetDesktopWallpaper
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private string wallpaperFileName = null;


        private void MainForm_Load(object sender, EventArgs e)
        {
            // If the current operating system is not Windows 7 or later, disable the 
            // Fit and Fill wallpaper styles.
            if (!Wallpaper.SupportFitFillWallpaperStyles)
            {
                this.radFit.Enabled = false;
                this.radFill.Enabled = false;
            }

            this.toolTip.SetToolTip(this.radTile, Resources.TileStyleTooltip);
            this.toolTip.SetToolTip(this.radCenter, Resources.CenterStyleTooltip);
            this.toolTip.SetToolTip(this.radStretch, Resources.StretchStyleTooltip);
            this.toolTip.SetToolTip(this.radFit, Resources.FitStyleTooltip);
            this.toolTip.SetToolTip(this.radFill, Resources.FillStyleTooltip);
        }


        private void btnBrowseWallpaper_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == wallpaperOpenFileDialog.ShowDialog())
            {
                this.wallpaperFileName = wallpaperOpenFileDialog.FileName;
                this.tbWallpaperFileName.Text = this.wallpaperFileName;

                // Preview the image in a picture box.
                Image wallpaper = Image.FromFile(this.wallpaperFileName);

                if (wallpaper.Width < this.pctPreview.Width &&
                    wallpaper.Height < this.pctPreview.Height)
                {
                    this.pctPreview.Image = wallpaper;
                }
                else
                {
                    float wallpaperRatio = (float)wallpaper.Width / (float)wallpaper.Height;
                    float pctPreviewRatio = (float)pctPreview.Width / (float)pctPreview.Height;

                    if (wallpaperRatio >= pctPreviewRatio)
                    {
                        this.pctPreview.Image = wallpaper.GetThumbnailImage(
                            this.pctPreview.Width,
                            (int)(this.pctPreview.Width / wallpaperRatio),
                            null, IntPtr.Zero);
                    }
                    else
                    {
                        this.pctPreview.Image = wallpaper.GetThumbnailImage(
                            (int)(this.pctPreview.Height * wallpaperRatio),
                            this.pctPreview.Height,
                            null, IntPtr.Zero);
                    }
                }
            }
        }


        private void btnSetWallpaper_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.wallpaperFileName))
            {
                Wallpaper.SetDesktopWallpaper(
                    this.wallpaperFileName, 
                    this.SelectedWallpaperStyle);
            }
        }


        private WallpaperStyle SelectedWallpaperStyle
        {
            get
            {
                if (this.radTile.Checked)
                {
                    return WallpaperStyle.Tile;
                }
                else if (this.radCenter.Checked)
                {
                    return WallpaperStyle.Center;
                }
                else if (this.radStretch.Checked)
                {
                    return WallpaperStyle.Stretch;
                }
                else if (this.radFit.Checked)
                {
                    return WallpaperStyle.Fit;
                }
                else
                {
                    return WallpaperStyle.Fill;
                }
            }
        }
    }
}