// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Windows;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

[module: SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope = "member", Target = "Microsoft.Elementalist.SplashScreen+WNDCLASSEX..ctor()")]
[module: SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope = "member", Target = "Microsoft.Elementalist.SplashScreen.Open():System.Void")]
[module: SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources", Scope = "member", Target = "Microsoft.Elementalist.SplashScreen+WNDCLASSEX.hCursor")]
[module: SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources", Scope = "member", Target = "Microsoft.Elementalist.SplashScreen._splashScreenHwnd")]

namespace MsdnReader
{
    internal class SplashScreen
    {
        //-------------------------------------------------------------------
        //
        //  Public Methods
        //
        //-------------------------------------------------------------------

        public void Open()
        {
            IntPtr hInstance = Marshal.GetHINSTANCE(typeof(SplashScreen).Module);

            BITMAP bitmapInfo = new BITMAP();
            IntPtr splashScreenBitmap = GetHBitmapFromResource(hInstance, SplashScreenResourceType, SplashScreenResourceId);

            GetBitmapInformation(splashScreenBitmap, Marshal.SizeOf(typeof(BITMAP)), ref bitmapInfo);
            int top, left;
            CreateWindow(hInstance, bitmapInfo.bmWidth, bitmapInfo.bmHeight, out left, out top);
            SelectBitmap(splashScreenBitmap, bitmapInfo.bmWidth, bitmapInfo.bmHeight, left, top);
        }

        public void Close()
        {
            DestroyWindow(_splashScreenHwnd);
            _splashScreenHwnd = IntPtr.Zero;
            GlobalUnlock(_hBuffer);
            GlobalFree(_hBuffer);
            GdiplusShutdown(_gdiPlusToken);
        }

        public void SetParent(IntPtr hNewParent)
        {
            SetParent(_splashScreenHwnd, hNewParent);
        }

        //-------------------------------------------------------------------
        //
        //  Private Methods
        //
        //-------------------------------------------------------------------

        private void SelectBitmap(IntPtr hBitmap, int width, int height, int left, int top)
        {
            IntPtr screenDc = GetDC(IntPtr.Zero);
            IntPtr memDc = CreateCompatibleDC(screenDc);
            IntPtr hOldBitmap = IntPtr.Zero;

            try
            {
                hOldBitmap = SelectObject(memDc, hBitmap);

                Size newSize = new Size(width, height);
                Point sourceLocation = new Point(0, 0);
                Point newLocation = new Point(left, top);
                BLENDFUNCTION blend = new BLENDFUNCTION();
                blend.BlendOp = AC_SRC_OVER; 
                blend.BlendFlags = 0; 
                blend.SourceconstantAlpha = 255; 
                blend.AlphaFormat = AC_SRC_ALPHA;

                Bool result = UpdateLayeredWindow(this._splashScreenHwnd, screenDc, ref newLocation, ref newSize,
                    memDc, ref sourceLocation, 0, ref blend, ULW_ALPHA);
            }
            finally
            {
                ReleaseDC(IntPtr.Zero, screenDc);
                if (hBitmap != IntPtr.Zero)
                {
                    SelectObject(memDc, hOldBitmap);
                    DeleteObject(hBitmap);
                }
                DeleteDC(memDc);
            }
        }

        private void CreateWindow(IntPtr hInstance, int width, int height, out int left, out int top)
        {
            left = top = 0;

            // Prepare the window class
            WNDCLASSEX splashScreenWindowClass = new WNDCLASSEX();
            splashScreenWindowClass.cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
            splashScreenWindowClass.style = CS_HREDRAW | CS_VREDRAW;
            splashScreenWindowClass.lpfnWndProc = _splashWndProc;
            splashScreenWindowClass.cbClsExtra = 0;
            splashScreenWindowClass.cbWndExtra = 0;
            splashScreenWindowClass.hInstance = hInstance;
            splashScreenWindowClass.hCursor = LoadCursor(IntPtr.Zero, IDC_ARROW);
            splashScreenWindowClass.lpszClassName = SplashScreenClassName;
            splashScreenWindowClass.lpszMenuName = String.Empty;

            // Register the window class
            if (RegisterClassEx(ref splashScreenWindowClass) != 0)
            {
                // Calculate the window position
                int screenWidth = GetSystemMetrics(SM_CXSCREEN);
                int screenHeight = GetSystemMetrics(SM_CYSCREEN);
                int x = (screenWidth - width) / 2;
                int y = (screenHeight - height) / 2;

                // Create and display the window
                _splashScreenHwnd = CreateWindowEx(
                    WS_EX_PALETTEWINDOW | WS_EX_LAYERED, // | WS_EX_TOPMOST,
                    SplashScreenClassName,
                    String.Empty,
                    WS_POPUP | WS_VISIBLE,
                    x, y, width, height,
                    IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);

                left = x;
                top = y;
            }
        }

        private IntPtr GetHBitmapFromResource(IntPtr hInstance, string resourceType, int resourceId)
        {
            // Initialize GDIPLUS
            _gdiPlusStartupInput.GdiplusVersion = 1;
            _gdiPlusStartupInput.DebugEventCallback = IntPtr.Zero;
            _gdiPlusStartupInput.SuppressBackgroundThread = false;
            _gdiPlusStartupInput.SuppressExternalCodecs = false;
            StartupOutput output;
            GdiplusStartup(out _gdiPlusToken, ref _gdiPlusStartupInput, out output);

            IntPtr hBitmap = IntPtr.Zero;

            IntPtr hResource = FindResource(hInstance, resourceId, resourceType);
            uint size = SizeofResource(hInstance, hResource);
            IntPtr pResourceData = LoadResource(hInstance, hResource);
            pResourceData = LockResource(pResourceData);

            _hBuffer = GlobalAlloc(GMEM_MOVEABLE, size);
            if (_hBuffer != IntPtr.Zero)
            {
                IntPtr pBuffer = GlobalLock(_hBuffer);
                CopyMemory(pBuffer, pResourceData, size);

                IStream pIStream;
                if (CreateStreamOnHGlobal(_hBuffer, false, out pIStream) == 0)
                {
                    IntPtr pBmp;
                    int result = GdipCreateBitmapFromStream(pIStream, out pBmp);
                    result = GdipCreateHBITMAPFromBitmap(pBmp, out hBitmap, 0);
                }
            }

            return hBitmap;
        }

        //-------------------------------------------------------------------
        //
        //  P/Invoke
        //
        //-------------------------------------------------------------------

        #region Interop private constants

        private const int SM_CXSCREEN = 0;
        private const int SM_CYSCREEN = 1;
        private const int CS_HREDRAW = 0x0001;
        private const int CS_VREDRAW = 0x0002;
        private const uint WS_EX_PALETTEWINDOW = 0x00000100 | 0x00000080;
        private const uint WS_POPUP = 0x80000000;
        private const uint WS_VISIBLE = 0x10000000;
        private const int WS_EX_LAYERED = 0x00080000;
        private const int WS_EX_TOPMOST = 0x00000008;

        private const uint GMEM_MOVEABLE = 0x0002;

        private const Int32 HTCAPTION = 0x02;
        private const Int32 WM_NCHITTEST = 0x84;
        private const Int32 ULW_ALPHA = 0x02;
        private const byte AC_SRC_OVER = 0x00;
        private const byte AC_SRC_ALPHA = 0x01;

        #endregion

        #region DllImports

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern Bool DeleteDC(IntPtr hdc);

        [DllImport("Gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        [DllImport("Gdi32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetObjectW")]
        private static extern int GetBitmapInformation(IntPtr hgdiobj, int cbBuffer, ref BITMAP lpvObject);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        private static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int GdipCreateBitmapFromStream(IStream stream, out IntPtr pBitmap);

        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int GdipCreateHBITMAPFromBitmap(IntPtr pBitmap, out IntPtr hBitmap, int argb);

        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int GdiplusShutdown(IntPtr token);

        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode, ExactSpelling = true)]
        private static extern int GdiplusStartup(out IntPtr token, ref StartupInput input, out  StartupOutput output);

        [DllImport("kernel32.dll")]
        private static extern IntPtr CopyMemory(IntPtr destination, IntPtr source, uint length);

        [DllImport("kernel32.dll")]
        private static extern IntPtr FindResource(IntPtr hModule, int lpName, int lpType);

        [DllImport("kernel32.dll")]
        private static extern IntPtr FindResource(IntPtr hModule, int lpName, string lpType);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalAlloc(uint uFlags, uint dwBytes);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalFree(IntPtr hMem);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GlobalUnlock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("ole32.dll")]
        private static extern int CreateStreamOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease, out IStream ppstm);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern Bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Point pptDst, ref Size psize, IntPtr hdcSrc, ref Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "CreateWindowExW")]
        private static extern IntPtr CreateWindowEx(
            uint dwExStyle,
            string lpClassName,
            string lpWindowName,
            uint dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam
            );

        [DllImport("User32.dll")]
        private static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegisterClassExW")]
        [return: MarshalAs(UnmanagedType.U2)]
        private static extern short RegisterClassEx([In] ref WNDCLASSEX lpwcx);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "DefWindowProcW")]
        private static extern int DefWindowProc(IntPtr hWnd, int Msg, int wParam, int lParam);

        private delegate int WindowProc(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadCursorW")]
        private static extern IntPtr LoadCursor(
            IntPtr hInstance,
            IntPtr lpCursorName
        );

        [DllImport("User32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        #endregion

        #region Interop structures

        [StructLayout(LayoutKind.Sequential)]
        private struct StartupOutput
        {
            public IntPtr hook;
            public IntPtr unhook;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct StartupInput
        {
            public int GdiplusVersion;
            public IntPtr DebugEventCallback;
            public bool SuppressBackgroundThread;
            public bool SuppressExternalCodecs;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct WNDCLASSEX
        {
            public int cbSize;
            public int style;
            [MarshalAs(UnmanagedType.FunctionPtr)]
            public WindowProc lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hSmIcon;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct BITMAP
        {
            public int bmType;
            public int bmWidth;
            public int bmHeight;
            public int bmWidthBytes;
            public short bmPlanes;
            public short bmBitsPixel;
            public IntPtr bmBits;
        }

        private enum Bool
        {
            False = 0,
            True = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            public Int32 x;
            public Int32 y;

            public Point(Int32 x, Int32 y)
            {
                this.x = x; 
                this.y = y; 
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct Size
        {
            public Int32 cx;
            public Int32 cy;

            public Size(Int32 cx, Int32 cy)
            {
                this.cx = cx;
                this.cy = cy;
            }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct ARGB
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceconstantAlpha;
            public byte AlphaFormat;
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Private Fields
        //
        //-------------------------------------------------------------------

        private readonly static IntPtr IDC_ARROW = new IntPtr(32512);
        private readonly static WindowProc _splashWndProc = new WindowProc(DefWindowProc); // Keep the reference alive

        private const string SplashScreenClassName = "WpfSplashScreen";

        private const int SplashScreenResourceId = 101;
        private const string SplashScreenResourceType = "PNG";
        
        private IntPtr _splashScreenHwnd;
        private IntPtr _gdiPlusToken;
        private StartupInput _gdiPlusStartupInput;
        private IntPtr _hBuffer;
    }

}