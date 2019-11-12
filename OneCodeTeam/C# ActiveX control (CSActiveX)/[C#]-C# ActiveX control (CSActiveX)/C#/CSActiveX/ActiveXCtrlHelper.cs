/********************************** Module Header ***********************************\
* Module Name:  ActiveXCtrlHelper.cs
* Project:      CSActiveX
* Copyright (c) Microsoft Corporation.
* 
* ActiveXCtrlHelper provides the helper functions to register/unregister an ActiveX 
* control, and helps to handle the focus and tabbing across the container and the 
* .NET controls.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\************************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Drawing;
using Microsoft.VisualBasic.Devices;
#endregion


[ComVisible(false)]
internal class ActiveXCtrlHelper : AxHost
{
    internal ActiveXCtrlHelper()
        : base(null)
    {
    }

    #region ActiveX Control Registration

    #region OLEMISC Enumeration

    // Ref: http://msdn.microsoft.com/en-us/library/ms678497.aspx
    const int OLEMISC_RECOMPOSEONRESIZE     = 1;
    const int OLEMISC_CANTLINKINSIDE        = 16;
    const int OLEMISC_INSIDEOUT             = 128;
    const int OLEMISC_ACTIVATEWHENVISIBLE   = 256;
    const int OLEMISC_SETCLIENTSITEFIRST    = 131072;
    
    #endregion

    /// <summary>
    /// Register the control as an ActiveX control.
    /// </summary>
    /// <param name="t"></param>
    public static void RegasmRegisterControl(Type t)
    {
        // Check the argument
        GuardNullType(t, "t");
        GuardTypeIsControl(t);

        // Open the CLSID key of the control
        using (RegistryKey keyCLSID = Registry.ClassesRoot.OpenSubKey(
            @"CLSID\" + t.GUID.ToString("B"), /*writable*/true))
        {
            RegistryKey subkey = null;

            // Set "InprocServer32" to register a 32-bit in-process server.
            // InprocServer32 = <path to 32-bit inproc server>
            // Ref: http://msdn.microsoft.com/en-us/library/ms683844.aspx
            
            subkey = keyCLSID.OpenSubKey("InprocServer32", /*writable*/true);
            if (subkey != null) 
                // .NET runtime engine (mscoree.dll) for .NET assemblies
                subkey.SetValue(null, Environment.SystemDirectory + @"\mscoree.dll");

            
            // Create "Control" to identify it as an ActiveX Control.
            // Ref: http://msdn.microsoft.com/en-us/library/ms680056.aspx

            using (subkey = keyCLSID.CreateSubKey("Control")) { };

            
            // Create "MiscStatus" to specify how to create/display an object. 
            // MiscStatus = <combination of values from OLEMISC enumeration>
            // Ref: http://msdn.microsoft.com/en-us/library/ms683733.aspx
            
            using (subkey = keyCLSID.CreateSubKey("MiscStatus"))
            {
                int nMiscStatus = OLEMISC_RECOMPOSEONRESIZE +
                    OLEMISC_CANTLINKINSIDE + OLEMISC_INSIDEOUT +
                    OLEMISC_ACTIVATEWHENVISIBLE + OLEMISC_SETCLIENTSITEFIRST;

                subkey.SetValue("", nMiscStatus.ToString(), RegistryValueKind.String);
            }


            // Create "ToolBoxBitmap32" to identify the module name and the resource  
            // ID for a 16 x 16 bitmap as the toolbar button face.
            // ToolBoxBitmap32 = <filename>.<ext>, <resourceID>
            // Ref: http://msdn.microsoft.com/en-us/library/ms687316.aspx
            
            using (subkey = keyCLSID.CreateSubKey("ToolBoxBitmap32"))
            {
                // If you want different icons for each control in the assembly you 
                // can modify this section to specify a different icon each time. 
                // Each specified icon must be embedded as a win32 resource in the 
                // assembly; the default one is at the index 101, but you can use 
                // additional ones.
                subkey.SetValue("", Assembly.GetExecutingAssembly().Location + ", 101",
                    RegistryValueKind.String);
            }


            // Create "TypeLib" to specify the typelib GUID associated with the class. 

            using (subkey = keyCLSID.CreateSubKey("TypeLib"))
            {
                Guid libId = Marshal.GetTypeLibGuidForAssembly(t.Assembly);
                subkey.SetValue("", libId.ToString("B"), RegistryValueKind.String);
            }


            // Create "Version" to specify the version of the control. 
            // Ref: http://msdn.microsoft.com/en-us/library/ms686568.aspx
            
            using (subkey = keyCLSID.CreateSubKey("Version"))
            {
                int nMajor, nMinor;
                Marshal.GetTypeLibVersionForAssembly(t.Assembly, out nMajor, out nMinor);
                subkey.SetValue("", String.Format("{0}.{1}", nMajor, nMinor));
            }

        }
    }

    /// <summary>
    /// Unregister the control.
    /// </summary>
    /// <param name="t"></param>
    public static void RegasmUnregisterControl(Type t)
    {
        // Check the argument
        GuardNullType(t, "t");
        GuardTypeIsControl(t);

        // Delete the CLSID key of the control
        Registry.ClassesRoot.DeleteSubKeyTree(@"CLSID\" + t.GUID.ToString("B"));
    }

    private static void GuardNullType(Type t, String param)
    {
        if (t == null)
        {
            throw new ArgumentException("The CLR type must be specified.", param);
        }
    }

    private static void GuardTypeIsControl(Type t)
    {
        if (!typeof(Control).IsAssignableFrom(t))
        {
            throw new ArgumentException(
                "Type argument must be a Windows Forms control.");
        }
    }

    #endregion

    #region Type Converter

    /// <summary>
    /// Convert System.Drawing.Color to OleColor 
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    internal static new int GetOleColorFromColor(Color color)
    {
        return CUIntToInt(AxHost.GetOleColorFromColor(color));
    }

    /// <summary>
    /// Convert OleColor to System.Drawing.Color
    /// </summary>
    /// <param name="oleColor"></param>
    /// <returns></returns>
    internal static Color GetColorFromOleColor(int oleColor)
    {
        return AxHost.GetColorFromOleColor(CIntToUInt(oleColor));
    }

    internal static int CUIntToInt(uint uiArg)
    {
        if (uiArg <= int.MaxValue)
        {
            return (int)uiArg;
        }
        return (int)(uiArg - unchecked(2 * ((uint)(int.MaxValue) + 1)));
    }

    internal static uint CIntToUInt(int iArg)
    {
        if (iArg < 0)
        {
            return (uint)(uint.MaxValue + iArg + 1);
        }
        return (uint)iArg;
    }

    #endregion

    #region Tab Handler

    /// <summary>
    /// Register tab handler and focus-related event handlers for the control and its 
    /// child controls.
    /// </summary>
    /// <param name="ctrl"></param>
    /// <param name="ValidationHandler"></param>
    internal static void WireUpHandlers(Control ctrl, EventHandler ValidationHandler)
    {
        if (ctrl != null)
        {
            ctrl.KeyDown += new KeyEventHandler(TabHandler);
            ctrl.LostFocus += new EventHandler(ValidationHandler);

            if (ctrl.HasChildren)
            {
                foreach (Control child in ctrl.Controls)
                {
                    WireUpHandlers(child, ValidationHandler);
                }
            }
        }
    }

    /// <summary>
    /// Handler of "Tab" and "Shift"+"Tab".
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void TabHandler(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Tab)
        {
            Control ctrl = sender as Control;
            UserControl usrCtrl = GetParentUserControl(ctrl);
            Control firstCtrl = usrCtrl.GetNextControl(null, true);
            do
            {
                firstCtrl = usrCtrl.GetNextControl(firstCtrl, true);
            } while (firstCtrl != null && !firstCtrl.CanSelect);

            Control lastCtrl = usrCtrl.GetNextControl(null, false);
            do
            {
                lastCtrl = usrCtrl.GetNextControl(lastCtrl, false);
            } while (lastCtrl != null && lastCtrl.CanSelect);

            if (ctrl.Equals(lastCtrl) || ctrl.Equals(firstCtrl) || 
                lastCtrl.Contains(ctrl) || firstCtrl.Contains(ctrl))
            {
                usrCtrl.SelectNextControl((Control)sender, 
                    lastCtrl.Equals(usrCtrl.ActiveControl), true, true, true);
            }
        }
    }

    private static UserControl GetParentUserControl(Control ctrl)
    {
        if (ctrl == null)
            return null;

        do
        {
            ctrl = ctrl.Parent;
        } while (ctrl.Parent != null);

        if (ctrl != null)
            return (UserControl)ctrl;

        return null;
    }

    #endregion

    #region Focus Handler

    /// <summary>
    /// Handle the focus of the ActiveX control, including its child controls
    /// </summary>
    /// <param name="usrCtrl">the ActiveX control</param>
    internal static void HandleFocus(UserControl usrCtrl)
    {
        Keyboard keyboard = new Keyboard();
        if (keyboard.AltKeyDown)
        {
            // Handle accessor key
            HandleAccessorKey(usrCtrl.GetNextControl(null, true), usrCtrl);
        }
        else
        {
            // Move to the first control that can receive focus, taking into account 
            // the possibility that the user pressed <Shift>+<Tab>, in which case we 
            // need to start at the end and work backwards.
            for (System.Windows.Forms.Control ctrl =
                usrCtrl.GetNextControl(null, !keyboard.ShiftKeyDown);
                ctrl != null;
                ctrl = usrCtrl.GetNextControl(ctrl, !keyboard.ShiftKeyDown))
            {
                if (ctrl.Enabled && ctrl.CanSelect)
                {
                    ctrl.Focus();
                    break;
                }
            }
        }

    }

    private const int KEY_PRESSED = 0x1000;
    [DllImport("user32.dll")]
    static extern short GetKeyState(int nVirtKey);

    /// <summary>
    /// Get X in the accessor key "Alt + X"
    /// </summary>
    /// <returns></returns>
    private static int CheckForAccessorKey()
    {
        Keyboard keyboard = new Keyboard();
        if (keyboard.AltKeyDown)
        {
            for (int i = (int)Keys.A; i <= (int)Keys.Z; i++)
            {
                if ((GetKeyState(i) != 0 && KEY_PRESSED != 0))
                {
                    return i;
                }
            }
        }
        return -1;
    }

    /// <summary>
    /// Check the accessor key, find the next selectable control that matches the 
    /// accessor key and give it the focus.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="usrCtrl"></param>
    private static void HandleAccessorKey(object sender, UserControl usrCtrl)
    {
        // Get X in the accessor key <Alt + X>
        int key = CheckForAccessorKey();
        if (key == -1) return;

        Control ctrl = usrCtrl.GetNextControl((Control)sender, false);

        do
        {
            ctrl = usrCtrl.GetNextControl(ctrl, true);
            if (ctrl != null &&
                Control.IsMnemonic(Convert.ToChar(key), ctrl.Text) &&
                !KeyConflict(Convert.ToChar(key), usrCtrl))
            {
                // If we land on a non-selectable control then go to the next 
                // control in the tab order.
                if (!ctrl.CanSelect)
                {
                    Control ctlAfterLabel = usrCtrl.GetNextControl(ctrl, true);
                    if (ctlAfterLabel != null && ctlAfterLabel.CanFocus)
                        ctlAfterLabel.Focus();
                }
                else
                {
                    ctrl.Focus();
                }
                break;
            }
            // Loop until we hit the end of the tab order. If we have hit the end  
            // of the tab order we do not want to loop back because the parent 
            // form's controls come next in the tab order.
        } while (ctrl != null);
    }

    private static bool KeyConflict(char key, UserControl u)
    {
        bool flag = false;
        foreach (Control ctl in u.Controls)
        {
            if (Control.IsMnemonic(key, ctl.Text))
            {
                if (flag)
                {
                    return true;
                }
                flag = true;
            }
        }
        return false;
    }

    #endregion
}

