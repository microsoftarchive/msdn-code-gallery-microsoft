// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace DocumentProtectionExcel
{
    /// <summary>
    /// A user control for the Actions Pane which allows a user
    /// to manipulate the data displayed in the document.
    /// </summary>
    public partial class TechniqueUserControl : UserControl
    {
        /// <summary>
        /// Intitialize components and bind dataDataGridView to data source.
        /// </summary>
        public TechniqueUserControl()
        {
            InitializeComponent();
            // Binds dataDataGridView control to data source.
            this.dataDataGridView.DataSource = Globals.ThisWorkbook.custBindingSource;
        }

        /// <summary>
        /// Writes value into dateDateTimePicker control, writes value into read
        /// only dateTextBox by changing its ReadOnly property to False, setting 
        /// the Text property and restoring the property value afterwards.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void dateDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                // Writes dateDateTimePicker value into dateNamedRange 
                Globals.Sheet1.dateNamedRange.Value2 = dateDateTimePicker.Value;

                try
                {
                    // Unprotects TextBox value on Sheet1.
                    Globals.Sheet1.dateTextBox.ReadOnly = false;
                    // Writes dateDateTimePicker value into dateTextBox in ShortDatePattern.
                    Globals.Sheet1.dateTextBox.Text = dateDateTimePicker.Value.ToString("d", DateTimeFormatInfo.CurrentInfo);
                }
                finally
                {
                    // Protects TextBox value on Sheet1.
                    Globals.Sheet1.dateTextBox.ReadOnly = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                        "Error changing dateNamedRange or dateTextBox value.",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error,
                        MessageBoxDefaultButton.Button1);
            }

        }
    }
}
