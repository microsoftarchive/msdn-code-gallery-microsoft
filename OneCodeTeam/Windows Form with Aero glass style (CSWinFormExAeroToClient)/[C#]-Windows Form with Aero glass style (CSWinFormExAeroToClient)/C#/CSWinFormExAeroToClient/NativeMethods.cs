/************************************ Module Header ***********************************\
 * Module Name:       NativeMethods.cs
 * Project:           CSWinFormExAeroToClient
 * Copyright (c)      Microsoft Corporation.
 * 
 * This class wraps the DwmIsCompositionEnabled, DwmExtendFrameIntoClientArea and 
 * DwmEnableBlurBehindWindow Functions in dwmapi.dll.
 *  
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\**************************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace CSWinFormExAeroToClient
{
    public static class NativeMethods
    {
        /// <summary>
        /// Obtain a value that indicates whether Desktop Window Manager
        /// (DWM) composition is enabled. 
        /// </summary>
        [DllImport("dwmapi.dll", CharSet = CharSet.Auto, PreserveSig = false,
            SetLastError = true)]
        internal static extern void DwmIsCompositionEnabled(out bool pfEnable);

        /// <summary>
        /// Extend the window frame into the client area.
        /// </summary>
        [DllImport("dwmapi.dll", CharSet = CharSet.Auto, PreserveSig = false,
            SetLastError = true)]
        internal static extern void DwmExtendFrameIntoClientArea(
            IntPtr hWnd,
            [In] ref MARGINS margins);

        /// <summary>
        /// Enable the blur effect on a specified window.
        /// </summary>
        [DllImport("dwmapi.dll", CharSet = CharSet.Auto, PreserveSig = false,
            SetLastError = true)]
        internal static extern void DwmEnableBlurBehindWindow(IntPtr hWnd,
            DWM_BLURBEHIND pBlurBehind);

        /// <summary>
        /// The point of MARGINS structure that describes the margins to use when
        /// extending the frame into the client area.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MARGINS
        {
            // Width of the left border that retains its size.
            public int cxLeftWidth;

            // Width of the right border that retains its size.
            public int cxRightWidth;

            // Height of the top border that retains its size.
            public int cyTopHeight;

            // Height of the bottom border that retains its size.
            public int cyBottomHeight;

            public MARGINS(int margin)
            {
                cxLeftWidth = margin;
                cxRightWidth = margin;
                cyTopHeight = margin;
                cyBottomHeight = margin;
            }

            public MARGINS(int leftWidth, int rightWidth, 
                int topHeight, int bottomHeight)
            {
                cxLeftWidth = leftWidth;
                cxRightWidth = rightWidth;
                cyTopHeight = topHeight;
                cyBottomHeight = bottomHeight;
            }

            /// <summary>
            /// Determine whether there is a negative value, or the value is valid
            /// for a Form.
            /// </summary>
            public bool IsNegativeOrOverride(System.Drawing.Size formClientSize)
            {
                return cxLeftWidth < 0
                    || cxRightWidth < 0
                    || cyBottomHeight < 0
                    || cyTopHeight < 0
                    || (cxLeftWidth + cxRightWidth) > formClientSize.Width
                    || (cyTopHeight + cyBottomHeight) > formClientSize.Height;
            }
        }

        /// <summary>
        /// Specify Desktop Window Manager (DWM) blur-behind properties. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class DWM_BLURBEHIND
        {
            // Indicate the members of this structure have been set.
            public uint dwFlags;

            // The flag specify  whether the subsequent compositions of the window
            // blurring the content behind it or not.
            [MarshalAs(UnmanagedType.Bool)]
            public bool fEnable;

            // The region where the glass style will be applied.
            public IntPtr hRegionBlur;

            // Whether the windows color should be transited to match the maximized 
            // windows or not.
            [MarshalAs(UnmanagedType.Bool)]
            public bool fTransitionOnMaximized;

            // Flags used to indicate the  members contain valid information.
            public const uint DWM_BB_ENABLE = 0x00000001;
            public const uint DWM_BB_BLURREGION = 0x00000002;
            public const uint DWM_BB_TRANSITIONONMAXIMIZED = 0x00000004;
        }

    }
}
