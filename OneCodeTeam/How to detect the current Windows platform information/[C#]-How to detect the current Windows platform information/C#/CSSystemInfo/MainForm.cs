/****************************** Module Header ******************************\
* Module Name:    MainForm.cs
* Project:        CSSystemInfo
* Copyright (c) Microsoft Corporation
*
* The project illustrates how to get System Information programmatically.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
*
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.DirectoryServices.ActiveDirectory;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CSSystemInfo
{
    public partial class MainForm : Form
    {
        // Declare the OSVERSIONINFO structure which will contain operating system version information
        public struct OSVERSIONINFO
        {
            public int dwOSVersionInfoSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
        }

        // <summary>
        // This function returns the service pack level on the machine this program is running on.
        // </summary>
        // <param name="o">OSVERSIONINFO structure</param>
        // <returns>Return the tservice pack level as string</returns>

        [DllImport("kernel32.Dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern short GetVersionEx(ref OSVERSIONINFO o);
        static public string GetServicePack()
        {
            OSVERSIONINFO os = new OSVERSIONINFO();
            os.dwOSVersionInfoSize = Marshal.SizeOf(typeof(OSVERSIONINFO));
            GetVersionEx(ref os);
            if (os.szCSDVersion == "")
                return "No Service Pack Installed";
            else
                return os.szCSDVersion;
        }

        // <summary>
        // This function is used to check if it is a Server version of OS on the machine.
        // </summary>
        // <returns>Return true if it is a Server version of OS else returns false</returns>
 
        public bool IsServerVersion()
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                foreach (ManagementObject managementObject in searcher.Get())
                {
                    // ProductType will be one of:
                    // 1: Workstation
                    // 2: Domain Controller
                    // 3: Server
                    uint productType = (uint)managementObject.GetPropertyValue("ProductType");
                    return productType != 1;
                }
            }
            return false;
        }

        public MainForm()
        {
            InitializeComponent();
        }

        // <summary>
        // Form Load event handler function.
        // </summary>

        private void Form1_Load(object sender, EventArgs e)
        {
            // Set textBox5 and textBox6 appropriately based on the bitness of the machine and the process
            if (Environment.Is64BitOperatingSystem)
            {
                textBox5.Text = "64-Bit";
            }
            else
            {
                textBox5.Text = "32-Bit";
            }

            if (Environment.Is64BitProcess)
            {
                textBox6.Text = "64-Bit";
            }
            else
            {
                textBox6.Text = "32-Bit";
            }

            // Set the textbox1 to the Operating System name by checking the OS version. 
            Version vs = Environment.OSVersion.Version;

            bool isServer = IsServerVersion();

            switch (vs.Major)
            {
                case 3:
                    textBox1.Text = "Windows NT 3.51";
                    break;
                case 4:
                    textBox1.Text = "Windows NT 4.0";
                    break;
                case 5:
                    if (vs.Minor == 0)
                        textBox1.Text = "Windows 2000";
                    else if (vs.Minor == 1)
                        textBox1.Text = "Windows XP";
                    else
                    {
                        if (isServer)
                        {
                            if (WindowsAPI.GetSystemMetrics(89) == 0)
                                textBox1.Text = "Windows Server 2003";
                            else
                                textBox1.Text = "Windows Server 2003 R2";
                        }
                        else
                            textBox1.Text = "Windows XP";
                    }
                    break;
                case 6:
                    if (vs.Minor == 0)
                    {
                        if (isServer)
                            textBox1.Text = "Windows Server 2008";
                        else
                            textBox1.Text = "Windows Vista";
                    }
                    else if (vs.Minor == 1)
                    {
                        if (isServer)
                            textBox1.Text = "Windows Server 2008 R2";
                        else
                            textBox1.Text = "Windows 7";
                    }
                    else if (vs.Minor == 2)
                        textBox1.Text = "Windows 8";
                    else
                    {
                        if (isServer)
                            textBox1.Text = "Windows Server 2012 R2";
                        else
                            textBox1.Text = "Windows 8.1";
                    }
                    break;
            } 

            // Set the textBox2 to the machine name
            textBox2.Text = Environment.MachineName;

            // Set the textBox4 to the domain name to which the machine is connected else set it to Workgroup
            try
            {
                textBox4.Text = System.DirectoryServices.ActiveDirectory.Domain.GetComputerDomain().Name; 
            }
            catch (ActiveDirectoryObjectNotFoundException ex)
            {
                textBox4.Text = "WORKGROUP";     
            }

            // Set textBox3 to the current service pack level installed on the machine
            textBox3.Text = GetServicePack();
        }
    }

    // Interop class to call GetSystemMetrics which will help us distinguish between Windows Server 2003 and 
    // Windows Server 2003 R2
    public partial class WindowsAPI
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetSystemMetrics(int smIndex);
    }
}
