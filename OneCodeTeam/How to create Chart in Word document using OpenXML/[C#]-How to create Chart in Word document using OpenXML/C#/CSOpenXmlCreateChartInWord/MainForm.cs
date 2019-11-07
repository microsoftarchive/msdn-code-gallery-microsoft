/****************************** Module Header ******************************\
* Module Name:  CreateWordChart.cs
* Project:      CSOpenXmlCreateChartInWord
* Copyright(c)  Microsoft Corporation.
* 
* The class is main Form. 
*
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/


using DocumentFormat.OpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CSOpenXmlCreateChartInWord
{
    public partial class MainForm: Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.btnCreateChart.Enabled = false;
        }

        private void btnCreateChart_Click(object sender, EventArgs e)
        {
            CreateWordChart createWordChart=null;
            try
            {
                createWordChart = new CreateWordChart(this.tbxFile.Text);
                List<ChartSubArea> chartAreas = new List<ChartSubArea>();
                chartAreas.Add(new ChartSubArea() { Color = SchemeColorValues.Accent1, Label = "1st Qtr", Value = "8.2" });
                chartAreas.Add(new ChartSubArea() { Color = SchemeColorValues.Accent2, Label = "2st Qtr", Value = "3.2" });
                chartAreas.Add(new ChartSubArea() { Color = SchemeColorValues.Accent3, Label = "3st Qtr", Value = "1.4" });
                chartAreas.Add(new ChartSubArea() { Color = SchemeColorValues.Accent4, Label = "4st Qtr", Value = "1.2" });
                createWordChart.CreateChart(chartAreas);
                MessageBox.Show("Create Chart successfully, you can check your document!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (createWordChart != null)
                {
                    createWordChart.Dispose();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveDialog = new SaveFileDialog())
            {
                saveDialog.Filter = "Word document (*.docx)|*.docx";
                saveDialog.InitialDirectory = Environment.CurrentDirectory;
                saveDialog.RestoreDirectory = true;
                saveDialog.DefaultExt = "docx";
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    FileStream fs= File.Create(saveDialog.FileName);
                    tbxFile.Text = saveDialog.FileName;
                    this.btnCreateChart.Enabled = true;
                    fs.Close();
                }
            }
        }     
    }
}
