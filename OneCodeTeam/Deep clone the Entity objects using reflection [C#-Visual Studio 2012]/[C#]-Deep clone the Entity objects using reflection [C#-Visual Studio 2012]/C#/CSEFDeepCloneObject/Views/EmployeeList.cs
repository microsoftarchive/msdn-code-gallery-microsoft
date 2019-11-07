/********************************* Module Header **********************************\
* Module Name:	EmployeeList.cs
* Project:		CSEFDeepCloneObject
* Copyright (c) Microsoft Corporation.
* 
* This sample demonstrates how to implement deep clone/duplicate entity objects using 
* serialization and reflection.
* This module is used to show the list of the employees and the related objects.
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
using System.Linq;
using System.Windows.Forms;

namespace CSEFDeepCloneObject
{
    public partial class EmployeeList : Form
    {
        #region Constructors

        public EmployeeList()
        {
            InitializeComponent();
        }

        #endregion

        #region Public Function

        /// <summary>
        /// Refresh the gridviews when the db's records has been changed.
        /// </summary>
        public void RefreshData()
        {
            BindDataSource();
        }

        #endregion

        #region Form Event

        private void EmployeeList_Load(object sender, EventArgs e)
        {
            BindDataSource();
            employeeGridView.SelectionChanged += empSelection_Changed;
        }

        #endregion

        #region GridView Event

        /// <summary>
        /// Select the related records in the address, bascic sales and detail sales 
        /// when the selected item in the employee grid view changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void empSelection_Changed(object sender, EventArgs e)
        {
            if (employeeGridView.Rows.Count > 0 && 
                employeeGridView.SelectedRows.Count > 0)
            {
                DataGridViewRow emp = employeeGridView.SelectedRows[0];
                ClearGridViewSelection();

                SelectRows(empAddressGridView, emp.Cells[0].Value.ToString());

                SelectRows(bsInfoGridView, emp.Cells[0].Value.ToString());

                if (bsInfoGridView.SelectedRows.Count != 0)
                {
                    SelectRows(dsInfoGridView, 
                        bsInfoGridView.SelectedRows[0].Cells[1].Value.ToString());
                }

            }

        }

        #endregion

        #region Button Event

        /// <summary>
        /// Show the Create Employee form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreate_Click(object sender, EventArgs e)
        {
            ShowNextForm(Config.EmpDetailsForm);
        }

        /// <summary>
        /// Show the Sales information form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSales_Click(object sender, EventArgs e)
        {
            ShowNextForm(Config.BsInfoForm);
        }

        /// <summary>
        /// Clone the selected employee and related child object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClone_Click(object sender, EventArgs e)
        {
            int currentEmpId = 
                Convert.ToInt32(employeeGridView.SelectedRows[0].Cells[0].Value);

            var emp = (from a in Config.Context.Employees
                       where a.EmpId == currentEmpId
                       select a).FirstOrDefault();

            var empNew = emp.Clone();

            empNew.ClearEntityReference(true);

            Config.Context.AddToEmployees(empNew);
            Config.Context.SaveChanges();

            RefreshData();

            employeeGridView.ClearSelection();
            SelectRows(employeeGridView,
                (from a in Config.Context.Employees select a.EmpId).Max().ToString());
        }

        #endregion

        #region Private Function

        /// <summary>
        /// Bind data to the gridview when the form was loaded or show again.
        /// </summary>
        private void BindDataSource()
        {
            var emps = (from a in Config.Context.Employees
                        select new
                        {
                            a.EmpId,
                            a.FirstName,
                            a.LastName,
                            a.Age,
                            a.Sex
                        });

            var empAddress = (from a in Config.Context.EmpAddresses
                              select new
                              {
                                  a.EmpId,
                                  a.AddressLine,
                                  a.City,
                                  a.State
                              });

            var bsSalesInfo = (from a in Config.Context.BasicSalesInfoes
                               select new
                               {
                                   a.EmpId,
                                   a.SalInfoId,
                                   a.Total
                               });

            var dsSalesInfo = (from a in Config.Context.DetailSalesInfoes
                               select new
                               {
                                   a.BasicSalInfoId,
                                   a.Sale,
                                   a.Year.Value.Year
                               });

            employeeGridView.DataSource = emps;
            empAddressGridView.DataSource = empAddress;
            bsInfoGridView.DataSource = bsSalesInfo;
            dsInfoGridView.DataSource = dsSalesInfo;
            
        }

        /// <summary>
        /// Clear the selection of the gridview.
        /// </summary>
        private void ClearGridViewSelection()
        {
            empAddressGridView.ClearSelection();
            bsInfoGridView.ClearSelection();
            dsInfoGridView.ClearSelection();
        }

        /// <summary>
        /// Select the related item accord the selected employee id.
        /// </summary>
        /// <param name="gridview"></param>
        /// <param name="selectedId"></param>
        private void SelectRows(DataGridView gridview, string selectedId)
        {
            if (gridview == null || string.IsNullOrEmpty(selectedId))
            {
                MessageBox.Show(Properties.Resources.Msg);
            }
            foreach (DataGridViewRow selectedRow in gridview.Rows)
            {
                if (selectedRow.Cells[0].Value.ToString()
                    .Equals(selectedId, StringComparison.OrdinalIgnoreCase))
                {
                    selectedRow.Selected = true;
                }
            }
        }

        private void ShowNextForm(Form from)
        {
            from.ShowDialog();
        }

        #endregion

    }
}
