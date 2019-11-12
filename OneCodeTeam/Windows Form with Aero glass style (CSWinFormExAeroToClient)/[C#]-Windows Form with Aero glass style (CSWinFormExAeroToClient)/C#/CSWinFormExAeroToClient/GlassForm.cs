/************************************* Module Header ***********************************\
 * Module Name:	            GlassForm.cs
 * Project:		            CSWinFormExAeroToClient
 * Copyright (c)             Microsoft Corporation.
 * 
 * This class is a base class that is used to create a glass style win form on Vista or 
 * Windows7.
 * 
 * CSWinFormExAeroToClient example demonstrates how to use DwmExtendFrameIntoClientArea to
 * extend the Windows Vista glass frame into the client area of a Windows Form application.
 * 
 * There are 2 approaches to achieve this effect.
 * 
 * A. Extend the frame to achieve the Aero effect.  
 *    1. Extend the frame using the DwmExtendFrameIntoClientArea method. The margins parameter
 *       of this method defines the margins of the form, in other words, the width and height
 *       of the frame.       
 *    2. Make the region in the margins transparent.     
 *    Because the region belongs to the frame now, so it will have the Aero effect.
 * 
 * B. Enable the blur effect on the form to achieve the Aero effect.
 *    1. Make the region transparent.
 *    2. Enable the blur effect on the form using the DwmEnableBlurBehindWindow method.
 *    Then the region will have a Aero effect.
 * 
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************************/

using System;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;

namespace CSWinFormExAeroToClient
{
    [PermissionSetAttribute(SecurityAction.LinkDemand, Name = "FullTrust")]
    [PermissionSetAttribute(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public class GlassForm : Form
    {

        // Inform all top-level windows that Desktop Window Manager (DWM) 
        // composition has been enabled or disabled.
        const int WM_DWMCOMPOSITIONCHANGED = 0x031E;

        // Sent to a window in order to determine what part of the window 
        // corresponds to a particular screen coordinate.
        const int WM_NCHITTEST = 0x84;

        // In a client area.
        const int HTCLIENT = 0x01;

        // A less frequently used Color that is used as the TransparencyKey.  
        static Color transparentColor = Color.DarkTurquoise;

        /// <summary>
        /// Specify whether extending the frame is enabled.
        /// </summary>
        public bool ExtendFrameEnabled { get; set; }

        /// <summary>
        /// Specify whether the blur effect is enabled.
        /// </summary>
        public bool BlurBehindWindowEnabled { get; set; }

        private Region marginRegion = null;

        /// <summary>
        /// Set the frame border. 
        /// </summary>
        public NativeMethods.MARGINS GlassMargins { get; set; }

        Region blurRegion = null;

        /// <summary>
        /// The region that the blur effect will be applied.
        /// </summary>
        public Region BlurRegion
        {
            get
            {
                return blurRegion; ;
            }
            set
            {
                if (blurRegion != null)
                {
                    blurRegion.Dispose();
                }
                blurRegion = value;
            }
        }

        public event EventHandler DWMCompositionChanged;

        /// <summary>
        /// Set the TransparencyKey.
        /// </summary>
        public GlassForm()
        {
            this.TransparencyKey = transparentColor;
        }

        /// <summary>
        /// When the size of this form changes, redraw the form.
        /// </summary>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.Invalidate();
        }

        /// <summary>
        /// When the form is painted, set the region where the glass effect 
        /// is applied.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if ((!ExtendFrameEnabled && !BlurBehindWindowEnabled)
                || !IsAeroGlassStyleSupported())
            {
                return;
            }

            using (Brush transparentBrush = new SolidBrush(transparentColor))
            {

                // Extend the frame.
                if (ExtendFrameEnabled)
                {
                    var glassMargins = this.GlassMargins;

                    // Extend the frame.
                    NativeMethods.DwmExtendFrameIntoClientArea(this.Handle,
                        ref glassMargins);

                    // Make the region in the margins transparent. 
                    marginRegion = new Region(this.ClientRectangle);

                    // If the glassMargins contains a negative value, or the values are not valid,
                    // then make the whole form transparent.
                    if (this.GlassMargins.IsNegativeOrOverride(this.ClientSize))
                    {
                        e.Graphics.FillRegion(transparentBrush, marginRegion);
                    }
                    
                    // By default, exclude the region of the client area.
                    else
                    {                      
                        marginRegion.Exclude(new Rectangle(
                            this.GlassMargins.cxLeftWidth,
                            this.GlassMargins.cyTopHeight,
                            this.ClientSize.Width - this.GlassMargins.cxLeftWidth - this.GlassMargins.cxRightWidth,
                            this.ClientSize.Height - this.GlassMargins.cyTopHeight - this.GlassMargins.cyBottomHeight));
                        e.Graphics.FillRegion(transparentBrush, marginRegion);
                    }
                }

                // Reset the frame to the default state.
                else
                {
                    var glassMargins = new NativeMethods.MARGINS(-1);
                    NativeMethods.DwmExtendFrameIntoClientArea(this.Handle,
                       ref glassMargins);
                }

                // Enable the blur effect on the form.
                if (BlurBehindWindowEnabled)
                {
                    ResetDwmBlurBehind(true, e.Graphics);

                    if (this.BlurRegion != null)
                    {
                        e.Graphics.FillRegion(transparentBrush, BlurRegion);
                    }
                    else
                    {
                        e.Graphics.FillRectangle(transparentBrush, this.ClientRectangle);
                    }
                }
                else
                {
                    ResetDwmBlurBehind(false, null);
                }
            }
        }

        /// <summary>
        /// Enable or disable the blur effect on the form.
        /// </summary>
        private void ResetDwmBlurBehind(bool enable, Graphics graphics)
        {
            try
            {
                NativeMethods.DWM_BLURBEHIND bbh = new NativeMethods.DWM_BLURBEHIND();

                if (enable)
                {
                    bbh.dwFlags = NativeMethods.DWM_BLURBEHIND.DWM_BB_ENABLE;
                    bbh.fEnable = true;

                    if (this.BlurRegion != null)
                    {
                        bbh.hRegionBlur = this.BlurRegion.GetHrgn(graphics);
                    }
                    else
                    {
                        // Apply the blur glass effect to the entire window.
                        bbh.hRegionBlur = IntPtr.Zero;
                    }
                }
                else
                {
                    bbh.dwFlags = NativeMethods.DWM_BLURBEHIND.DWM_BB_ENABLE |
                        NativeMethods.DWM_BLURBEHIND.DWM_BB_BLURREGION;
                    // Turn off the glass effect.
                    bbh.fEnable = false;
                    // Apply the blur glass effect to the entire window.
                    bbh.hRegionBlur = IntPtr.Zero;
                }
                NativeMethods.DwmEnableBlurBehindWindow(this.Handle, bbh);
            }
            catch { }
        }

        /// <summary>
        /// Make sure the current computer is able to display the glass style windows.
        /// </summary>
        /// <returns>
        /// The flag that specify whether DWM composition is enabled or not.
        /// </returns>
        public static bool IsAeroGlassStyleSupported()
        {
            bool isDWMEnable = false;
            try
            {
                // Check that the glass is enabled by using the DwmIsCompositionEnabled. 
                // It is supported in version 6.0 or above of the operating system.
                if (System.Environment.OSVersion.Version.Major >= 6)
                {
                    // Make sure the Glass is enabled by the user.
                    NativeMethods.DwmIsCompositionEnabled(out isDWMEnable);
                }
            }
            catch { }

            return isDWMEnable;
        }
     
        /// <summary>
        /// This method makes that users can drag the form by click the extended frame.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            // Let the normal WndProc process it.
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    // The mouse is inside the client area
                    if (HTCLIENT == m.Result.ToInt32())
                    {
                        // Parse the WM_NCHITTEST message parameters
                        // get the mouse pointer coordinates (in screen coordinates)
                        Point p = new Point();
                        // low order word
                        p.X = (m.LParam.ToInt32() & 0xFFFF);
                        // high order word
                        p.Y = (m.LParam.ToInt32() >> 16);

                        // Convert screen coordinates to client area coordinates
                        p = PointToClient(p);

                        // If it's on glass, then convert it from an HTCLIENT
                        // message to an HTCAPTION message and let Windows handle it 
                        // from then on.
                        if (PointIsOnGlass(p))
                        {
                            m.Result = new IntPtr(2);
                        }
                    }
                    break;
                case WM_DWMCOMPOSITIONCHANGED:

                    // Release the resource when glass effect is not supported.
                    if (DWMCompositionChanged!=null)
                    {
                        DWMCompositionChanged(this, EventArgs.Empty);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Check that the point is inside the glass area.
        /// </summary>
        private bool PointIsOnGlass(Point p)
        {
            if (this.marginRegion == null)
            {
                return false;
            }
            else
            {
                return this.marginRegion.IsVisible(p);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (this.marginRegion != null)
            {
                marginRegion.Dispose();
            }

            if (this.BlurRegion != null)
            {
                this.BlurRegion.Dispose();
            }
        }

    }
}
