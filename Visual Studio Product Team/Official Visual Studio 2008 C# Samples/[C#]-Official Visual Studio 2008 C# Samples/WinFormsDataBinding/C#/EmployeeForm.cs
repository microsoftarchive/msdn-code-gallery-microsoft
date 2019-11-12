//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Linq;
using System.Data.Linq;
using NorthwindMapping;

namespace WinFormsDataBinding {
    public partial class EmployeeForm : Form {
        private Northwind db;

        public EmployeeForm() {
            InitializeComponent();
            db = new Northwind(Program.connString);
            var employeeQuery = from employee in db.Employees 
                                orderby employee.LastName
                                select employee;
            var managerQuery =  from manager in db.Employees 
                                orderby manager.LastName
                                select manager;
            //ToBindingList method converts query into a structure that supports IBindingList.
            //The Table<T> is required to convert to a binding list so that the addition and
            //deletion of entities are tracked correctly.
            employeeBindingSource.DataSource = employeeQuery;
            managerBindingSource.DataSource = managerQuery.ToList();
       }

        private void submitChanges_Click(object sender, EventArgs e) {
            //Causes the control container to validate and end editing.
            this.Validate();
            db.SubmitChanges();
        }

        private void employeeDataGridView_CellParsing(object sender, DataGridViewCellParsingEventArgs e) {
            string s = e.Value as string;

             //OfType is needed because the employeeBindingSource returns instances of type object.
             Employee emp = (from employee in this.managerBindingSource.OfType<Employee>()
                            where employee.ToString()==s
                            select employee).FirstOrDefault();
            
            e.Value = emp;
            e.ParsingApplied = true;
        }

        private void launchEmployeeManager_Click(object sender, EventArgs e) {
            WinFormsDataBinding.EmployeeManagerGrids form = new EmployeeManagerGrids();
            form.Visible = true;
        }
    }
}