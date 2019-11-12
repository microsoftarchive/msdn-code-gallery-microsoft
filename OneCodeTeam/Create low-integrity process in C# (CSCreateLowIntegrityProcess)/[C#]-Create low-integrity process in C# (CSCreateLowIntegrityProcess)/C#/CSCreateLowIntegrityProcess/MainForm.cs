/********************************** Module Header **********************************\
 Module Name:  MainForm.cs
 Project:      CSCreateLowIntegrityProcess
 Copyright (c) Microsoft Corporation.

 The code sample demonstrates how to start a low-integrity process. The application
 launches itself at the low integrity level when you click the "Launch myself at low
 integrity level" button on the application. Low integrity processes can only write 
 to low integrity locations, such as the %USER PROFILE%\AppData\LocalLow folder or 
 the HKEY_CURRENT_USER\Software\AppDataLow key. If you attempt to gain write access 
 to objects at a higher integrity levels, you will get an access denied error even 
 though the user's SID is granted write access in the discretionary access control 
 list (DACL). 

 By default, child processes inherit the integrity level of their parent process. To 
 start a low-integrity process, you must start a new child process with a low-
 integrity access token by using CreateProcessAsUser. Please refer to the 
 CreateLowIntegrityProcess sample function for details.

 This source is subject to the Microsoft Public License.
 See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
 All other rights reserved.

 THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
 EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
 MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***********************************************************************************/

#region Using directives
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
#endregion


namespace CSCreateLowIntegrityProcess
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // The sample application must be run on Windows Vista or any newer 
            // operating systems that support User Account Control (UAC). 
            if (Environment.OSVersion.Version.Major < 6)
            {
                MessageBox.Show(
                    "The sample application must be run on Windows Vista or any " +
                    "newer operating systems that support User Account Control " +
                    "(UAC). The application will exit.");
                Close();  // Exit the application
            }

            try
            {
                // Get and display the current process integrity level.
                int IL = GetProcessIntegrityLevel();
                switch (IL)
                {
                    case NativeMethod.SECURITY_MANDATORY_UNTRUSTED_RID:
                        this.lbIntegrityLevel.Text = "Untrusted"; break;
                    case NativeMethod.SECURITY_MANDATORY_LOW_RID:
                        this.lbIntegrityLevel.Text = "Low"; break;
                    case NativeMethod.SECURITY_MANDATORY_MEDIUM_RID:
                        this.lbIntegrityLevel.Text = "Medium"; break;
                    case NativeMethod.SECURITY_MANDATORY_HIGH_RID:
                        this.lbIntegrityLevel.Text = "High"; break;
                    case NativeMethod.SECURITY_MANDATORY_SYSTEM_RID:
                        this.lbIntegrityLevel.Text = "System"; break;
                    default:
                        this.lbIntegrityLevel.Text = "Unknown"; break;
                }
            }
            catch (Exception ex)
            {
                this.lbIntegrityLevel.Text = "N/A";
                MessageBox.Show(ex.Message, "GetProcessIntegrityLevel Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreateLowProcess_Click(object sender, EventArgs e)
        {
            try
            {
                // Try to launch a new instance of the current application at the low 
                // integrity level.
                CreateLowIntegrityProcess(Application.ExecutablePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "CreateLowIntegrityProcess Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWriteLocalAppData_Click(object sender, EventArgs e)
        {
            try
            {
                // Test file path: %USERPROFILE%\AppData\Local\testfile.txt
                string filePath = KnownFolder.LocalAppData + @"\testfile.txt";

                // Try to create and write to the test file.
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.Write("CSCreateLowIntegrityProcess Test File");
                }

                MessageBox.Show("Successfully write to the test file: " + filePath,
                    "Test Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Writing to LocalAppData Failed", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnWriteLocalAppDataLow_Click(object sender, EventArgs e)
        {
            try
            {
                // Test file path: %USERPROFILE%\AppData\LocalLow\testfile.txt
                string filePath = KnownFolder.LocalAppDataLow + @"\testfile.txt";

                // Try to create and write to the test file.
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    sw.Write("CSCreateLowIntegrityProcess Test File");
                }

                MessageBox.Show("Successfully write to the test file: " + filePath,
                    "Test Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Writing to LocalAppDataLow Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        #region Helper Functions related to Process Integrity Level

        /// <summary>
        /// The function launches an application at low integrity level. 
        /// </summary>
        /// <param name="commandLine">
        /// The command line to be executed. The maximum length of this string is 32K 
        /// characters. 
        /// </param>
        /// <remarks>
        /// To start a low-integrity process, 
        /// 1) Duplicate the handle of the current process, which is at medium 
        ///    integrity level.
        /// 2) Use SetTokenInformation to set the integrity level in the access token 
        ///    to Low.
        /// 3) Use CreateProcessAsUser to create a new process using the handle to 
        ///    the low integrity access token.
        /// </remarks>
        internal void CreateLowIntegrityProcess(string commandLine)
        {
            SafeTokenHandle hToken = null;
            SafeTokenHandle hNewToken = null;
            IntPtr pIntegritySid = IntPtr.Zero;
            int cbTokenInfo = 0;
            IntPtr pTokenInfo = IntPtr.Zero;
            STARTUPINFO si = new STARTUPINFO();
            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            try
            {
                // Open the primary access token of the process.
                if (!NativeMethod.OpenProcessToken(Process.GetCurrentProcess().Handle,
                    NativeMethod.TOKEN_DUPLICATE | NativeMethod.TOKEN_ADJUST_DEFAULT |
                    NativeMethod.TOKEN_QUERY | NativeMethod.TOKEN_ASSIGN_PRIMARY,
                    out hToken))
                {
                    throw new Win32Exception();
                }

                // Duplicate the primary token of the current process.
                if (!NativeMethod.DuplicateTokenEx(hToken, 0, IntPtr.Zero,
                    SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation, 
                    TOKEN_TYPE.TokenPrimary, out hNewToken))
                {
                    throw new Win32Exception();
                }

                // Create the low integrity SID.
                if (!NativeMethod.AllocateAndInitializeSid(
                    ref NativeMethod.SECURITY_MANDATORY_LABEL_AUTHORITY, 1, 
                    NativeMethod.SECURITY_MANDATORY_LOW_RID,
                    0, 0, 0, 0, 0, 0, 0, out pIntegritySid))
                {
                    throw new Win32Exception();
                }

                TOKEN_MANDATORY_LABEL tml;
                tml.Label.Attributes = NativeMethod.SE_GROUP_INTEGRITY;
                tml.Label.Sid = pIntegritySid;

                // Marshal the TOKEN_MANDATORY_LABEL struct to the native memory.
                cbTokenInfo = Marshal.SizeOf(tml);
                pTokenInfo = Marshal.AllocHGlobal(cbTokenInfo);
                Marshal.StructureToPtr(tml, pTokenInfo, false);

                // Set the integrity level in the access token to low.
                if (!NativeMethod.SetTokenInformation(hNewToken,
                    TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, pTokenInfo,
                    cbTokenInfo + NativeMethod.GetLengthSid(pIntegritySid)))
                {
                    throw new Win32Exception();
                }

                // Create the new process at the Low integrity level.
                si.cb = Marshal.SizeOf(si);
                if (!NativeMethod.CreateProcessAsUser(hNewToken, null, commandLine, 
                    IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, null, ref si, 
                    out pi))
                {
                    throw new Win32Exception();
                }
            }
            finally
            {
                // Centralized cleanup for all allocated resources. 
                if (hToken != null)
                {
                    hToken.Close();
                    hToken = null;
                }
                if (hNewToken != null)
                {
                    hNewToken.Close();
                    hNewToken = null;
                }
                if (pIntegritySid != IntPtr.Zero)
                {
                    NativeMethod.FreeSid(pIntegritySid);
                    pIntegritySid = IntPtr.Zero;
                }
                if (pTokenInfo != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pTokenInfo);
                    pTokenInfo = IntPtr.Zero;
                    cbTokenInfo = 0;
                }
                if (pi.hProcess != IntPtr.Zero)
                {
                    NativeMethod.CloseHandle(pi.hProcess);
                    pi.hProcess = IntPtr.Zero;
                }
                if (pi.hThread != IntPtr.Zero)
                {
                    NativeMethod.CloseHandle(pi.hThread);
                    pi.hThread = IntPtr.Zero;
                }
            }
        }


        /// <summary>
        /// The function gets the integrity level of the current process. Integrity 
        /// level is only available on Windows Vista and newer operating systems, thus 
        /// GetProcessIntegrityLevel throws a C++ exception if it is called on systems 
        /// prior to Windows Vista.
        /// </summary>
        /// <returns>
        /// Returns the integrity level of the current process. It is usually one of 
        /// these values:
        /// 
        ///    SECURITY_MANDATORY_UNTRUSTED_RID - means untrusted level. It is used 
        ///    by processes started by the Anonymous group. Blocks most write access.
        ///    (SID: S-1-16-0x0)
        ///    
        ///    SECURITY_MANDATORY_LOW_RID - means low integrity level. It is used by
        ///    Protected Mode Internet Explorer. Blocks write acess to most objects 
        ///    (such as files and registry keys) on the system. (SID: S-1-16-0x1000)
        /// 
        ///    SECURITY_MANDATORY_MEDIUM_RID - means medium integrity level. It is 
        ///    used by normal applications being launched while UAC is enabled. 
        ///    (SID: S-1-16-0x2000)
        ///    
        ///    SECURITY_MANDATORY_HIGH_RID - means high integrity level. It is used 
        ///    by administrative applications launched through elevation when UAC is 
        ///    enabled, or normal applications if UAC is disabled and the user is an 
        ///    administrator. (SID: S-1-16-0x3000)
        ///    
        ///    SECURITY_MANDATORY_SYSTEM_RID - means system integrity level. It is 
        ///    used by services and other system-level applications (such as Wininit, 
        ///    Winlogon, Smss, etc.)  (SID: S-1-16-0x4000)
        /// 
        /// </returns>
        /// <exception cref="System.ComponentModel.Win32Exception">
        /// When any native Windows API call fails, the function throws a Win32Exception 
        /// with the last error code.
        /// </exception>
        internal int GetProcessIntegrityLevel()
        {
            int IL = -1;
            SafeTokenHandle hToken = null;
            int cbTokenIL = 0;
            IntPtr pTokenIL = IntPtr.Zero;

            try
            {
                // Open the access token of the current process with TOKEN_QUERY.
                if (!NativeMethod.OpenProcessToken(Process.GetCurrentProcess().Handle,
                    NativeMethod.TOKEN_QUERY, out hToken))
                {
                    throw new Win32Exception();
                }

                // Then we must query the size of the integrity level information 
                // associated with the token. Note that we expect GetTokenInformation 
                // to return false with the ERROR_INSUFFICIENT_BUFFER error code 
                // because we've given it a null buffer. On exit cbTokenIL will tell 
                // the size of the group information.
                if (!NativeMethod.GetTokenInformation(hToken,
                    TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, IntPtr.Zero, 0,
                    out cbTokenIL))
                {
                    int error = Marshal.GetLastWin32Error();
                    if (error != NativeMethod.ERROR_INSUFFICIENT_BUFFER)
                    {
                        // When the process is run on operating systems prior to 
                        // Windows Vista, GetTokenInformation returns false with the 
                        // ERROR_INVALID_PARAMETER error code because 
                        // TokenIntegrityLevel is not supported on those OS's.
                        throw new Win32Exception(error);
                    }
                }

                // Now we allocate a buffer for the integrity level information.
                pTokenIL = Marshal.AllocHGlobal(cbTokenIL);
                if (pTokenIL == IntPtr.Zero)
                {
                    throw new Win32Exception();
                }

                // Now we ask for the integrity level information again. This may fail 
                // if an administrator has added this account to an additional group 
                // between our first call to GetTokenInformation and this one.
                if (!NativeMethod.GetTokenInformation(hToken,
                    TOKEN_INFORMATION_CLASS.TokenIntegrityLevel, pTokenIL, cbTokenIL,
                    out cbTokenIL))
                {
                    throw new Win32Exception();
                }

                // Marshal the TOKEN_MANDATORY_LABEL struct from native to .NET object.
                TOKEN_MANDATORY_LABEL tokenIL = (TOKEN_MANDATORY_LABEL)
                    Marshal.PtrToStructure(pTokenIL, typeof(TOKEN_MANDATORY_LABEL));

                // Integrity Level SIDs are in the form of S-1-16-0xXXXX. (e.g. 
                // S-1-16-0x1000 stands for low integrity level SID). There is one 
                // and only one subauthority.
                IntPtr pIL = NativeMethod.GetSidSubAuthority(tokenIL.Label.Sid, 0);
                IL = Marshal.ReadInt32(pIL);
            }
            finally
            {
                // Centralized cleanup for all allocated resources. 
                if (hToken != null)
                {
                    hToken.Close();
                    hToken = null;
                }
                if (pTokenIL != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(pTokenIL);
                    pTokenIL = IntPtr.Zero;
                    cbTokenIL = 0;
                }
            }

            return IL;
        }

        #endregion
    }
}
