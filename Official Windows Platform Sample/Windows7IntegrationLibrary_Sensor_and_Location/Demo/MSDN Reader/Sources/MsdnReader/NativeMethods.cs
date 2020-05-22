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
using System.Runtime.InteropServices;

/// <summary>
/// Native methods class - All PInvoke methods and constants should be defined here.
/// </summary>
namespace MsdnReader
{
    internal class NativeMethods
    {
        [DllImport("Gdi32.dll")]
        internal static extern bool DeleteObject(IntPtr hObject);

        [DllImport("Gdi32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetObjectW")]
        internal static extern int GetBitmapInformation(IntPtr hgdiobj, int cbBuffer, ref BITMAP lpvObject);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "CreateWindowExW")]
        internal static extern IntPtr CreateWindowEx(uint dwExStyle, string lpClassName, string lpWindowName,
                                                    uint dwStyle, int x, int y, int nWidth, int nHeight,
                                                    IntPtr hWndParent, IntPtr hMenu, IntPtr hInstance, IntPtr lpParam);

        [DllImport("User32.dll")]
        internal static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegisterClassExW")]
        internal static extern short RegisterClassEx(WNDCLASSEX lpWndClass);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "DefWindowProcW")]
        internal static extern int DefWindowProc(IntPtr hWnd, int Msg, int wParam, int lParam); 

        internal delegate int WindowProc(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("User32.dll", CharSet = CharSet.Unicode, EntryPoint = "LoadCursorW")]
        internal static extern IntPtr LoadCursor(IntPtr hInstance, IntPtr lpCursorName);

        [DllImport("kernel32.dll", SetLastError=true)]
        internal static extern IntPtr LocalFree(IntPtr hMem);

        [DllImport("User32.dll")]
        internal static extern int GetSystemMetrics(int nIndex);

        [DllImport("Shell32.dll", CharSet=CharSet.Unicode, ExactSpelling=true)]
        internal static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string cmdLine, 
            out int numArgs
        );
        
        internal static string [] CommandLineToArgvW(string cmdLine) 
        {
            string [] result = null;
            int numArgs = 0;
            IntPtr argv = IntPtr.Zero;
            try
            {
                argv = CommandLineToArgvW(cmdLine, out numArgs);
                result = new string[numArgs];                

                for (int i = 0; i < numArgs; i++)
                {
                    IntPtr currArg = Marshal.ReadIntPtr(argv, i * Marshal.SizeOf(typeof(IntPtr)));
                    result[i] = Marshal.PtrToStringUni(currArg);
                }

            }
            finally
            {
                LocalFree(argv);
            }
            
            return result;
        }
        
        internal const int SM_CXSCREEN = 0;
        internal const int SM_CYSCREEN = 1;
        internal const int CS_HREDRAW = 0x0001;
        internal const int CS_VREDRAW = 0x0002;
        internal const uint WS_EX_PALETTEWINDOW = 0x00000100 | 0x00000080;// | 0x00000008;
        internal const uint WS_POPUP = 0x80000000;
        internal const uint WS_VISIBLE = 0x10000000;

        internal static readonly IntPtr IDC_ARROW = new IntPtr(32512);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal class WNDCLASSEX
        {
            public int cbSize = Marshal.SizeOf(typeof(WNDCLASSEX));
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
        internal struct BITMAP
        {
            public int bmType;
            public int bmWidth;
            public int bmHeight;
            public int bmWidthBytes;
            public short bmPlanes;
            public short bmBitsPixel;
            public IntPtr bmBits;
        }

        /// <summary>
        ///  This is a value that, when passed as a window message, indicates that there
        /// has been some sort of change in the system power status. See the MessageProc function
        /// for more details.
        /// </summary>
        internal const int WM_POWERBROADCAST = 0x0218;
    }     
}