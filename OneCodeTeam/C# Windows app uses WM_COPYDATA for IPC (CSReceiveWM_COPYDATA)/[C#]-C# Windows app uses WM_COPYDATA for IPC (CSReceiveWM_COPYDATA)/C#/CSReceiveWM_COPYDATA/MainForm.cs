/********************************** Module Header **********************************\
Module Name:  MainForm.cs
Project:      CSReceiveWM_COPYDATA
Copyright (c) Microsoft Corporation.

Inter-process Communication (IPC) based on the Windows message WM_COPYDATA is a 
mechanism for exchanging data among Windows applications in the local machine. The 
receiving application must be a Windows application. The data being passed must not 
contain pointers or other references to objects not accessible to the application 
receiving the data. While WM_COPYDATA is being sent, the referenced data must not be 
changed by another thread of the sending process. The receiving application should 
consider the data read-only. If the receiving application must access the data after 
SendMessage returns, it needs to copy the data into a local buffer.

This code sample demonstrates receiving a custom data structure (MyStruct) from the 
sending application (CSSendWM_COPYDATA) by handling WM_COPYDATA messages.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

#region Using directives
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
#endregion


namespace CSReceiveWM_COPYDATA
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_COPYDATA)
            {
                // Get the COPYDATASTRUCT struct from lParam.
                COPYDATASTRUCT cds = (COPYDATASTRUCT)m.GetLParam(typeof(COPYDATASTRUCT));

                // If the size matches
                if (cds.cbData == Marshal.SizeOf(typeof(MyStruct)))
                {
                    // Marshal the data from the unmanaged memory block to a 
                    // MyStruct managed struct.
                    MyStruct myStruct = (MyStruct)Marshal.PtrToStructure(cds.lpData, 
                        typeof(MyStruct));

                    // Display the MyStruct data members.
                    this.lbNumber.Text = myStruct.Number.ToString();
                    this.lbMessage.Text = myStruct.Message;
                }
            }

            base.WndProc(ref m);
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct MyStruct
        {
            public int Number;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Message;
        }


        #region Native API Signatures and Types

        /// <summary>
        /// An application sends the WM_COPYDATA message to pass data to another 
        /// application.
        /// </summary>
        internal const int WM_COPYDATA = 0x004A;


        /// <summary>
        /// The COPYDATASTRUCT structure contains data to be passed to another 
        /// application by the WM_COPYDATA message. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct COPYDATASTRUCT
        {
            public IntPtr dwData;       // Specifies data to be passed
            public int cbData;          // Specifies the data size in bytes
            public IntPtr lpData;       // Pointer to data to be passed
        }

        #endregion
    }
}
