//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.DebuggerVisualizers;
using System.Linq.Expressions;
using ExpressionVisualizer;

namespace GuiHost
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void showVisualizerButton_Click(object sender, EventArgs e)
        {
            Expression<Func<int, bool>> expr = x => x == 10;

            VisualizerDevelopmentHost host = new VisualizerDevelopmentHost(expr,
                                                 typeof(ExpressionTreeVisualizer),
                                                 typeof(ExpressionTreeVisualizerObjectSource));
            host.ShowVisualizer(this);   
        }
    }
}