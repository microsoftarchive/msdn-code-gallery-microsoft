/************************************* Module Header **************************************\
* Module Name:  MainForm.cs
* Project:      CSWinFormLocalization
* Copyright (c) Microsoft Corporation.
* 
* The Windows Forms Localization example demonstrates how to localize Windows Forms 
* application.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* History:
* * 3/25/2009 3:00 PM Zhi-Xin Ye Created
\******************************************************************************************/

#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
#endregion


namespace CSWinFormLocalization
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            // The following shows you how to set the UI culture so the application 
            // displays your Chinese resources. In real-world applications, 
            // you would not hard-code the UI culture in this way. The setting for 
            // the UI culture is instead stored in a user setting or an application 
            // setting.

            // Sets the UI culture to Simplified Chinese. 
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CHS");

            InitializeComponent();
        }
    }
}
