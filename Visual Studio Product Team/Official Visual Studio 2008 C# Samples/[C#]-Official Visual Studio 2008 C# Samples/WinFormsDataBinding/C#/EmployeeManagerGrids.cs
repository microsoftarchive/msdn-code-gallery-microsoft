//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
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
    public partial class EmployeeManagerGrids : Form {
        private Northwind db;

        public EmployeeManagerGrids() {
            InitializeComponent();

            db = new Northwind(Program.connString);
            var employeeQuery = from employee in db.Employees 
                                orderby employee.LastName
                                select employee;
            //ToBindingList method converts query into a structure that supports IBindingList.
            //The Table<T> is required to convert to a binding list so that the addition and
            //deletion of entities are tracked correctly.
            employeeBindingSource.DataSource = employeeQuery;
            managerBindingSource.DataSource = employeeQuery.ToList();
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

        private void btnSubmitChanges_Click(object sender, EventArgs e) {
            //Causes the control container to validate and end editing.
            this.Validate();
            db.SubmitChanges();
        }
    }
}