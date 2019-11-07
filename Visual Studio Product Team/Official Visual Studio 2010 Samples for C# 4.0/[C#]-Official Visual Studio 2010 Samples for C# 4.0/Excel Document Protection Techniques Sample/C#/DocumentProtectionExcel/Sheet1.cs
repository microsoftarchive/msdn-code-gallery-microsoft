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
    public partial class Sheet1
    {
        #region Properties
        /// <summary>
        /// Returns whether the sheet is protected.
        /// </summary>
        /// <value>
        /// Returns true if the sheet is protected,
        /// or false if it is not.
        /// </value>
        internal bool IsProtected
        {
            get
            {
                return this.ProtectContents;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Protects the sheet with no password.
        /// </summary>
        internal void ProtectSheet()
        {
            // Ensures that the sheet is not already protected.
            if (this.IsProtected)
                throw new InvalidOperationException();

            // Protects the sheet so that it can only be read and 
            // unprotected with no password.
            this.Protect(missing, missing, missing, missing,
                        missing, missing, missing, missing,
                        missing, missing, missing, missing,
                        missing, missing, missing, missing);
        }

        /// <summary>
        /// Unprotects the sheet with no password.
        /// </summary>
        internal void UnprotectSheet()
        {
            // Ensures that the sheet is not already unprotected.
            if (!this.IsProtected)
                throw new InvalidOperationException();

            // Unprotects the sheet without password.
            this.Unprotect(missing);
        }

        #endregion

        /// <summary>
        /// Handles the Startup event for the sheet. When the event fires, 
        /// the sheet will be unprotected first, customerListObject will be 
        /// bound to customerBindingSource, the sheet will be protected 
        /// afterwards.
        /// </summary>
        /// <param name="sender">Unused.</param>
        /// <param name="e">Unused.</param>
        private void Sheet1_Startup(object sender, System.EventArgs e)
        {
            try
            {
                // Unprotects sheet
                this.UnprotectSheet();

                // Creates ListObject and binds to BindingSource.
                customerListObject.AutoSetDataBoundColumnHeaders = true;
                customerListObject.SetDataBinding(Globals.ThisWorkbook.custBindingSource, "", "firstName", "lastName", "userName");
            }
            finally
            {
                // Protects sheet
                this.ProtectSheet();
            }
        }

        private void Sheet1_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(Sheet1_Startup);
            this.Shutdown += new System.EventHandler(Sheet1_Shutdown);
        }

        #endregion

    }
}
