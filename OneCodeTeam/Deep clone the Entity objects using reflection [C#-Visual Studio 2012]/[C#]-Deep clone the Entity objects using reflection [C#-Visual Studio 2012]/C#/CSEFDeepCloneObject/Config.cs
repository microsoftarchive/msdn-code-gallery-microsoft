/********************************* Module Header **********************************\
* Module Name:	Config.cs
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

using System.Windows.Forms;

namespace CSEFDeepCloneObject
{
    /// <summary>
    /// Define some global objects been used in multi forms.
    /// </summary>
    static class Config
    {
        public static EFCloneEntities Context 
        {
            get; 
            set;
        }

        public static EmployeeList EmpListForm 
        {
            get; 
            set;
        }

        public static EmployeeDetails EmpDetailsForm 
        {
            get; 
            set;
        }

        public static SalesInfo BsInfoForm 
        { 
            get; 
            set; 
        }

        public static string[] Years
        {
            get;
            set;
        }

        /// <summary>
        /// Used to validate whether there are some empty textboxs.
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public static bool ValidateTextBox(Form form)
        {
            foreach (Control ctrl in form.Controls)
            {
                if (ctrl is TextBox)
                {
                    if (ctrl.Text.Trim() == "")
                    {
                        string msg = string.Format
                            ("The {0} field {1}", 
                            ctrl.Name.Substring(3), 
                            Properties.Resources.TextBoxValidateMsg);

                        MessageBox.Show(msg);
                        ctrl.Focus();
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
