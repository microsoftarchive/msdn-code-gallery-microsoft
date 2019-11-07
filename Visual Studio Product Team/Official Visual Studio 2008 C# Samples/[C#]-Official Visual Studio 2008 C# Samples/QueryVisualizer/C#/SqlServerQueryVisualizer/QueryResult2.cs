//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace LinqToSqlQueryVisualizer {
    public partial class QueryResult2 : Form {
        public QueryResult2() {
            InitializeComponent();
        }

        public void SetDataSets(DataSet ds1, DataSet ds2) {
            this.dataGridView1.DataSource = ds1.Tables[0];
            this.dataGridView2.DataSource = ds2.Tables[0];
        }
    }
}