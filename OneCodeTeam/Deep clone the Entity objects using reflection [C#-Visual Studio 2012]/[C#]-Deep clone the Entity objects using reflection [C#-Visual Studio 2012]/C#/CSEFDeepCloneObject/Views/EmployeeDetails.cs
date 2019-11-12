/********************************* Module Header **********************************\
* Module Name:	EmployeeDetails.cs
* Project:		CSEFDeepCloneObject
* Copyright (c) Microsoft Corporation.
* 
* This sample demonstrates how to implement deep clone/duplicate entity objects using 
* serialization and reflection.
* This module is used to show the detail information of the employee.
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
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CSEFDeepCloneObject
{
    public partial class EmployeeDetails : Form
    {
        #region Consturctors

        public EmployeeDetails()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        /// <summary>
        /// Save all the employee related information when click the save button.
        /// First save the employee basic information 
        /// then save the ralated address information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (Config.ValidateTextBox(this))
            {
                SaveEmployee();
                SaveEmpAddress();

                Hide();
                Config.EmpListForm.RefreshData();
            }
        }

        /// <summary>
        /// Ensure that the age is numberic and can't be null.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbxAge_Validating(object sender, CancelEventArgs e)
        {
            bool isValid = true;

            if (!Regex.IsMatch(tbxAge.Text, @"^\+?[1-9][0-9]*$"))
            {
                isValid = false;
            }
            else
            {
                int age = Convert.ToInt32(tbxAge.Text);
                if (!(age > 0 && age < 100))
                {
                    isValid = false;
                }
            }

            if (!isValid)
            {
                e.Cancel = true;
                tbxAge.Select(0, tbxAge.Text.Length);
                errorProvider.SetError(tbxAge, Properties.Resources.AgeMsg);
            }

        }

        /// <summary>
        /// Clear the error provider's information.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbxAge_Validated(object sender, EventArgs e)
        {
            errorProvider.SetError(tbxAge,"");
        }

        #endregion

        #region Private Function

        /// <summary>
        /// Save the new employee information to the database.
        /// </summary>
        private void SaveEmployee()
        {
            var emp = new Employee
            {
                FirstName = tbxFirstName.Text,
                LastName = tbxLastName.Text,
                Age = Convert.ToDouble(tbxAge.Text),
                Sex = cbxSex.Text
            };

            Config.Context.AddToEmployees(emp);
            Config.Context.SaveChanges();
        }

        /// <summary>
        /// Save the new employee's address information to the database.
        /// </summary>
        private void SaveEmpAddress()
        {
            int currentEmpId = (from a in Config.Context.Employees select a.EmpId).Max();

            var empAddress = new EmpAddress
            {
                AddressLine = tbxAddress.Text,
                City = tbxCity.Text,
                State = tbxState.Text,
                EmpId = currentEmpId,
            };

            Config.Context.AddToEmpAddresses(empAddress);
            Config.Context.SaveChanges();
        }

        #endregion 

    }
}
