/****************************** Module Header ******************************\
 * Module Name:  Form1.cs
 * Project:      CSWinFormPrintDataGridView
 * Copyright (c) Microsoft Corporation.
 * 
 * The code sample demonstrates how to print a DataGridView. The sample shows 
 * you the granularity as to print a single cell too.
 * 
 * This source is subject to the Microsoft Public License.
 * See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
 * All other rights reserved.
 * 
 * THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
 * EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
 * WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApplication5
{

    public partial class Form1 : Form
    {
        public Form2 f = new Form2();

        public static bool Checkifcolumn = false;

        bool[] columnsToPrint = new bool[7];

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MessageBox.Show("Entry Starts from the first day of the Month");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewCheckBoxCell chkcell = dataGridView1[e.ColumnIndex, e.RowIndex] as DataGridViewCheckBoxCell;
            if (chkcell != null)
            {
                columnsToPrint[dataGridView1.CurrentRow.Index] = Convert.ToBoolean(chkcell.EditedFormattedValue);
            }

            if (dataGridView1.RowCount > 5)
            {
                MessageBox.Show("There are a maximum of 31 days in a month buddy");
                dataGridView1.AllowUserToAddRows = false;
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int rowCounter = 0;
            int z = 0;
            StringFormat str = new StringFormat();
            str.Alignment = StringAlignment.Near;
            str.LineAlignment = StringAlignment.Center;
            str.Trimming = StringTrimming.EllipsisCharacter;

            int width = 500 / (dataGridView1.Columns.Count - 2);
            int realwidth = 100;
            int height = 40;

            int realheight = 100;

            // Please note this is where I am Printing Sunday , Monday , Tuesday.... We can also move rowCounter to 
            // the maxRowcounter loop where we are printing below 
            for (z = 0; z < dataGridView1.Columns.Count - 1; z++)
            {
                e.Graphics.FillRectangle(Brushes.AliceBlue, realwidth, realheight, width, height);
                e.Graphics.DrawRectangle(Pens.Black, realwidth, realheight, width, height);

                e.Graphics.DrawString(dataGridView1.Columns[z].HeaderText, dataGridView1.Font, Brushes.Black, realwidth, realheight);

                realwidth = realwidth + width;
            }

            z = 0;
            realheight = realheight + height;
            while (rowCounter < dataGridView1.Rows.Count)
            {
                realwidth = 100;

                if (columnsToPrint[rowCounter] == true)
                {
                    if ((f.checkBox8.Checked == true) || (f.checkBox1.Checked))
                    {
                        if (dataGridView1.Rows[rowCounter].Cells[0].Value == null)
                        {
                            dataGridView1.Rows[rowCounter].Cells[0].Value = "";
                        }
                        e.Graphics.FillRectangle(Brushes.AliceBlue, realwidth, realheight, width, height);
                        e.Graphics.DrawRectangle(Pens.Black, realwidth, realheight, width, height);

                        e.Graphics.DrawString(dataGridView1.Rows[rowCounter].Cells[0].Value.ToString(), dataGridView1.Font, Brushes.Black, realwidth, realheight);
                        realwidth = realwidth + width;

                    }
                    else
                    {
                        realwidth = realwidth + width;
                    }
                    if ((f.checkBox8.Checked == true) || (f.checkBox2.Checked))
                    {
                        if (dataGridView1.Rows[rowCounter].Cells[1].Value == null)
                        {
                            dataGridView1.Rows[rowCounter].Cells[1].Value = "";
                        }
                        e.Graphics.FillRectangle(Brushes.AliceBlue, realwidth, realheight, width, height);
                        e.Graphics.DrawRectangle(Pens.Black, realwidth, realheight, width, height);

                        e.Graphics.DrawString(dataGridView1.Rows[rowCounter].Cells[1].Value.ToString(), dataGridView1.Font, Brushes.Black, realwidth, realheight);

                        realwidth = realwidth + width;

                    }
                    else
                    {
                        realwidth = realwidth + width;
                    }

                    if ((f.checkBox8.Checked == true) || (f.checkBox3.Checked))
                    {
                        if (dataGridView1.Rows[rowCounter].Cells[2].Value == null)
                        {
                            dataGridView1.Rows[rowCounter].Cells[2].Value = "";
                        }
                        e.Graphics.FillRectangle(Brushes.AliceBlue, realwidth, realheight, width, height);
                        e.Graphics.DrawRectangle(Pens.Black, realwidth, realheight, width, height);

                        e.Graphics.DrawString(dataGridView1.Rows[rowCounter].Cells[2].Value.ToString(), dataGridView1.Font, Brushes.Black, realwidth, realheight);
                        realwidth = realwidth + width;

                    }
                    else
                    {
                        realwidth = realwidth + width;
                    }
                    if ((f.checkBox8.Checked == true) || (f.checkBox4.Checked))
                    {
                        if (dataGridView1.Rows[rowCounter].Cells[3].Value == null)
                        {
                            dataGridView1.Rows[rowCounter].Cells[3].Value = "";
                        }
                        e.Graphics.FillRectangle(Brushes.AliceBlue, realwidth, realheight, width, height);
                        e.Graphics.DrawRectangle(Pens.Black, realwidth, realheight, width, height);

                        e.Graphics.DrawString(dataGridView1.Rows[rowCounter].Cells[3].Value.ToString(), dataGridView1.Font, Brushes.Black, realwidth, realheight);
                        realwidth = realwidth + width;

                    }
                    else
                    {
                        realwidth = realwidth + width;
                    }
                    if ((f.checkBox8.Checked == true) || (f.checkBox5.Checked))
                    {
                        if (dataGridView1.Rows[rowCounter].Cells[4].Value == null)
                        {
                            dataGridView1.Rows[rowCounter].Cells[4].Value = "";
                        }
                        e.Graphics.FillRectangle(Brushes.AliceBlue, realwidth, realheight, width, height);
                        e.Graphics.DrawRectangle(Pens.Black, realwidth, realheight, width, height);

                        e.Graphics.DrawString(dataGridView1.Rows[rowCounter].Cells[4].Value.ToString(), dataGridView1.Font, Brushes.Black, realwidth, realheight);
                        realwidth = realwidth + width;

                    }
                    else
                    {
                        realwidth = realwidth + width;
                    }
                    if ((f.checkBox8.Checked == true) || (f.checkBox6.Checked))
                    {
                        if (dataGridView1.Rows[rowCounter].Cells[5].Value == null)
                        {
                            dataGridView1.Rows[rowCounter].Cells[5].Value = "";
                        }
                        e.Graphics.FillRectangle(Brushes.AliceBlue, realwidth, realheight, width, height);
                        e.Graphics.DrawRectangle(Pens.Black, realwidth, realheight, width, height);

                        e.Graphics.DrawString(dataGridView1.Rows[rowCounter].Cells[5].Value.ToString(), dataGridView1.Font, Brushes.Black, realwidth, realheight);
                        realwidth = realwidth + width;

                    }
                    else
                    {
                        realwidth = realwidth + width;
                    }
                    if ((f.checkBox8.Checked == true) || (f.checkBox7.Checked))
                    {
                        if (dataGridView1.Rows[rowCounter].Cells[6].Value == null)
                        {
                            dataGridView1.Rows[rowCounter].Cells[6].Value = "";
                        }
                        e.Graphics.FillRectangle(Brushes.AliceBlue, realwidth, realheight, width, height);
                        e.Graphics.DrawRectangle(Pens.Black, realwidth, realheight, width, height);

                        e.Graphics.DrawString(dataGridView1.Rows[rowCounter].Cells[6].Value.ToString(), dataGridView1.Font, Brushes.Black, realwidth, realheight);
                        realwidth = realwidth + width;

                    }
                    else
                    {
                        realwidth = realwidth + width;
                    }
                }
                ++rowCounter;
                realheight = realheight + height;

            }
            printDialog1.Document = printDocument1;
            printDialog1.ShowDialog();

        }

        private void confirmPrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Checkifcolumn)
            {
                this.printDocument1.Print();
            }
            else
            {
                MessageBox.Show("Please go to Which Column to Print and then either select all rows or the ones you want to print");
            }
        }

        private void chooseWhichColoumnToPrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Checkifcolumn = true;
            f.Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This sample is developed by Nandeesh Swami, Developer Support Core, Microsoft. Please read the Readme.htm for more details");
        }
    }
}
