// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;

namespace DocumentProtectionExcel
{
    public partial class Sheet2
    {
        /// <summary>
        /// A string to store the original value in the usernameNamedRange 
        /// control.
        /// </summary>
        private string userName;

        /// <summary>
        /// Handles the Startup event for the sheet. When the event fires, 
        /// the value of the usernameNamedRange control will be retrieved
        /// and saved into userName field.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void Sheet2_Startup(object sender, System.EventArgs e)
        {
            // Gets userNameNamedRange initial value
            userName = this.usernameNamedRange.Value2.ToString();
        }

        private void Sheet2_Shutdown(object sender, System.EventArgs e)
        {
        }

        /// <summary>
        /// Handles the NamedRange change event. When this event fires, the 
        /// original value of the NamedRange control will be restored and a 
        /// Message Box will be displayed indicating that the user is not 
        /// authorized to change the value.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void usernameNamedRange_Change(Microsoft.Office.Interop.Excel.Range Target)
        {
            try
            {
                // Turns off event handler 
                this.usernameNamedRange.Change -= new Microsoft.Office.Interop.Excel.DocEvents_ChangeEventHandler(this.usernameNamedRange_Change);
                this.usernameNamedRange.Value2 = userName;

                MessageBox.Show("You are not authorized to change this value.",
                    "Document Protection Techniques - Excel",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                    "Error handling NamedRange change event.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error,
                    MessageBoxDefaultButton.Button1);
            }
            finally
            {
                // Turns on event handler
                this.usernameNamedRange.Change += new Microsoft.Office.Interop.Excel.DocEvents_ChangeEventHandler(this.usernameNamedRange_Change);
            }

        }


        #region VSTO Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.usernameNamedRange.Change += new Microsoft.Office.Interop.Excel.DocEvents_ChangeEventHandler(this.usernameNamedRange_Change);
            this.Startup += new System.EventHandler(Sheet2_Startup);
            this.Shutdown += new System.EventHandler(Sheet2_Shutdown);
        }

        #endregion

    }
}
