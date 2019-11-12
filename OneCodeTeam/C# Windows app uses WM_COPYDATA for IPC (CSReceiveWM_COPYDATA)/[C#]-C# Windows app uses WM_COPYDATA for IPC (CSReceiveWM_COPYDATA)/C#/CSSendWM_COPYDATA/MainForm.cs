/********************************** Module Header **********************************\
Module Name:  MainForm.cs
Project:      CSSendWM_COPYDATA
Copyright (c) Microsoft Corporation.

Inter-process Communication (IPC) based on the Windows message WM_COPYDATA is a 
mechanism for exchanging data among Windows applications in the local machine. The 
receiving application must be a Windows application. The data being passed must not 
contain pointers or other references to objects not accessible to the application 
receiving the data. While WM_COPYDATA is being sent, the referenced data must not be 
changed by another thread of the sending process. The receiving application should 
consider the data read-only. If the receiving application must access the data after 
SendMessage returns, it needs to copy the data into a local buffer.

This code sample demonstrates sending a custom data structure (MyStruct) to the 
receiving Windows application (CSReceiveWM_COPYDATA) by using SendMessage(WM_COPYDATA). 
If the data structure fails to be passed, the application displays the error code for 
diagnostics. A typical error code is 0x5 (Access is denied) caused by User Interface 
Privilege Isolation (UIPI). UIPI prevents processes from sending selected window
messages and other USER APIs to processes running with higher integrity. When the 
receiving application (CSReceiveWM_COPYDATA) runs at an integrity level higher than 
this sending application, you will see the "SendMessage(WM_COPYDATA) failed w/err 
0x00000005" error message.

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
using System.Security;
#endregion


namespace CSSendWM_COPYDATA
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnSendMessage_Click(object sender, EventArgs e)
        {
            // Find the target window handle.
            IntPtr hTargetWnd = NativeMethod.FindWindow(null, "CSReceiveWM_COPYDATA");
            if (hTargetWnd == IntPtr.Zero)
            {
                MessageBox.Show("Unable to find the \"CSReceiveWM_COPYDATA\" window", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Prepare the COPYDATASTRUCT struct with the data to be sent.
            MyStruct myStruct;

            int nNumber;
            if (!int.TryParse(this.tbNumber.Text, out nNumber))
            {
                MessageBox.Show("Invalid value of Number!");
                return;
            }

            myStruct.Number = nNumber;
            myStruct.Message = this.tbMessage.Text;

            // Marshal the managed struct to a native block of memory.
            int myStructSize = Marshal.SizeOf(myStruct);
            IntPtr pMyStruct = Marshal.AllocHGlobal(myStructSize);
            try
            {
                Marshal.StructureToPtr(myStruct, pMyStruct, true);

                COPYDATASTRUCT cds = new COPYDATASTRUCT();
                cds.cbData = myStructSize;
                cds.lpData = pMyStruct;

                // Send the COPYDATASTRUCT struct through the WM_COPYDATA message to 
                // the receiving window. (The application must use SendMessage, 
                // instead of PostMessage to send WM_COPYDATA because the receiving 
                // application must accept while it is guaranteed to be valid.)
                NativeMethod.SendMessage(hTargetWnd, WM_COPYDATA, this.Handle, ref cds);

                int result = Marshal.GetLastWin32Error();
                if (result != 0)
                {
                    MessageBox.Show(String.Format(
                        "SendMessage(WM_COPYDATA) failed w/err 0x{0:X}", result));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pMyStruct);
            }
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
        /// application
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


        /// <summary>
        /// The class exposes Windows APIs to be used in this code sample.
        /// </summary>
        [SuppressUnmanagedCodeSecurity]
        internal class NativeMethod
        {
            /// <summary>
            /// Sends the specified message to a window or windows. The SendMessage 
            /// function calls the window procedure for the specified window and does 
            /// not return until the window procedure has processed the message. 
            /// </summary>
            /// <param name="hWnd">
            /// Handle to the window whose window procedure will receive the message.
            /// </param>
            /// <param name="Msg">Specifies the message to be sent.</param>
            /// <param name="wParam">
            /// Specifies additional message-specific information.
            /// </param>
            /// <param name="lParam">
            /// Specifies additional message-specific information.
            /// </param>
            /// <returns></returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr SendMessage(IntPtr hWnd, int Msg,
                IntPtr wParam, ref COPYDATASTRUCT lParam);


            /// <summary>
            /// The FindWindow function retrieves a handle to the top-level window 
            /// whose class name and window name match the specified strings. This 
            /// function does not search child windows. This function does not 
            /// perform a case-sensitive search.
            /// </summary>
            /// <param name="lpClassName">Class name</param>
            /// <param name="lpWindowName">Window caption</param>
            /// <returns></returns>
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        }

        #endregion
    }
}
