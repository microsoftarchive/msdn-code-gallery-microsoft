﻿/****************************** Module Header ******************************\ 
* Module Name:   MainForm.cs
* Project:       CSExcelCompareCells
* Copyright (c)  Microsoft Corporation. 
*  
* The Class is Main Form
* Customers can manipulate the form to 
* Compare in different columns in the same sheet of an excel file and 
* Compare data in different sheets of an excel file
* 
* 
* This source is subject to the Microsoft Public License. 
* See http://www.microsoft.com/en-us/openness/licenses.aspx. 
* All other rights reserved. 
*  
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,  
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED  
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE. 
\***************************************************************************/


using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CSExcelCompareCells
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Select Excel File
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Excel Files(*.xls;*.xlsx)|*.xls;*.xlsx";
                openFileDialog.Title = "Select an excel file";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    txbExcelPath.Text = openFileDialog.FileName;
                }
            }
        }

        /// <summary>
        /// Compare Cells in different columns in the same sheet of an excel file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCompareCol_Click(object sender, EventArgs e)
        {
            if (!File.Exists(txbExcelPath.Text))
            {
                MessageBox.Show("Please Select valid path of word document!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Regex reg = new Regex("^[A-Z]+$");
            if (txbSourceCol.Text != string.Empty && txbTargetCol.Text != string.Empty)
            {
                if (reg.IsMatch(txbSourceCol.Text.ToUpper()) && reg.IsMatch(txbTargetCol.Text.ToUpper()))
                {
                    try
                    {
                        new CompareHelper().CompareColumns(
                            txbSourceCol.Text,
                            txbTargetCol.Text,
                            txbExcelPath.Text);

                        // Clean up the unmanaged Excel COM resources by explicitly
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                        GC.WaitForPendingFinalizers(); 
                        MessageBox.Show("Compare Columns successfully,Please check in the excel file");
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Exception occur, the Exception Message is: " + ex.Message);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Source Column and Target Column must be letter");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please input Source Column and Target Column");
                return;
            }
        }

        /// <summary>
        /// Compare Cells in different sheets of an excel file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCompareSheet_Click(object sender, EventArgs e)
        {
            if (!File.Exists(txbExcelPath.Text))
            {
                MessageBox.Show("Please Select valid path of word document!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Regex reg = new Regex("^[0-9]*$");
            if (txbSourceSheet.Text != string.Empty && txbTargetSheet.Text != string.Empty)
            {
                if (reg.IsMatch(txbSourceSheet.Text.ToUpper()) && reg.IsMatch(txbTargetSheet.Text.ToUpper()))
                {
                    try
                    {
                        new CompareHelper().CompareSheets(
                            int.Parse(txbSourceSheet.Text),
                            int.Parse(txbTargetSheet.Text),
                            txbExcelPath.Text);

                        // Clean up the unmanaged Excel COM resources by explicitly
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        GC.Collect();
                        GC.WaitForPendingFinalizers(); 
                        MessageBox.Show("Compare sheets successfully,Please check in the excel file");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exception occur, the Exception Message is: " + ex.Message);
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("Source Sheet and Target Sheet must be Number");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please input Source Sheet and Target Sheet");
                return;
            }
        }
    }
}
