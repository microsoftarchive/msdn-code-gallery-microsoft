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
using Office = Microsoft.Office.Core;
using Word = Microsoft.Office.Interop.Word;

namespace DataContentControls
{
    public partial class ThisDocument
    {
        private ActionsPaneControl1 myActionsPane = new ActionsPaneControl1();
        private void ThisDocument_Startup(object sender, System.EventArgs e)
        {
            // TODO: Delete this line of code to remove the default AutoFill for 'northwindDataSet.Employees'.
            if (this.NeedsFill("northwindDataSet"))
            {
                this.employeesTableAdapter.Fill(this.northwindDataSet.Employees);
           }
            this.ActionsPane.Controls.Add(myActionsPane); 
        }

        private void ThisDocument_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Shutdown += new System.EventHandler(this.ThisDocument_Shutdown);
            this.Startup += new System.EventHandler(this.ThisDocument_Startup);

        }

        #endregion
    }
}
