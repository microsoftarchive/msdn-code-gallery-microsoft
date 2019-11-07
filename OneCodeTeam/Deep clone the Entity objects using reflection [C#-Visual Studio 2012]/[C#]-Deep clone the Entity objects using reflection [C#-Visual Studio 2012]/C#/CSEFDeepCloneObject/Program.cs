/********************************* Module Header **********************************\
* Module Name:	Program.cs
* Project:		CSEFDeepCloneObject
* Copyright (c) Microsoft Corporation.
* 
* This sample demonstrates how to implement deep clone/duplicate entity objects 
* using serialization and reflection.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
* **********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CSEFDeepCloneObject
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            InitConfig();
            Application.Run(Config.EmpListForm);
        }

        /// <summary>
        /// Initialize the Config class for the global object.
        /// </summary>
        private static void InitConfig()
        {
            Config.EmpListForm = new EmployeeList();
            Config.EmpDetailsForm = new EmployeeDetails();
            Config.BsInfoForm = new SalesInfo();
            Config.Context = new EFCloneEntities();
            Config.Years = new string[] { 
                "2006", "2007", "2008", "2009", "2010", "2011", "2012"};
        }
    }
}
