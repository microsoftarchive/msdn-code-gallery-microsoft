// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)


using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Collections;
using System.Collections.Generic;
using Office = Microsoft.Office;
using UiManagerOutlookAddIn.AddinUtilities;

namespace UiManagerOutlookAddIn
{
    // This class is not CLSCompliant because of its base class.
    [CLSCompliant(false)]
    public partial class ThisAddIn
    {

        #region Standard operations

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion


        #region Fields

        internal TaskPaneConnector _taskPaneConnector;
        internal RibbonConnector _ribbonConnector;
        private FormRegionConnector _formRegionConnector;
        private Outlook.Inspectors _inspectors;
        private const String _controlProgId = "UiManager.SimpleControl";
        private const String _controlTitle = "SimpleControl";
        internal UserInterfaceElements _uiElements;

        #endregion


        #region Request Services

        // We override RequestService to return a suitable object for each
        // of the new extensibility interfaces that we implement.
        protected override object RequestService(Guid serviceGuid)
        {
            if (serviceGuid ==
                typeof(Office.Core.ICustomTaskPaneConsumer).GUID)
            {
                if (_taskPaneConnector == null)
                {
                    _taskPaneConnector = new TaskPaneConnector();
                }
                return _taskPaneConnector;
            }

            else if (serviceGuid ==
                typeof(Office.Core.IRibbonExtensibility).GUID)
            {
                if (_ribbonConnector == null)
                {
                    _ribbonConnector = new RibbonConnector();
                }
                return _ribbonConnector;
            }

            else if (serviceGuid ==
                typeof(Outlook.FormRegionStartup).GUID)
            {
                if (_formRegionConnector == null)
                {
                    _formRegionConnector = new FormRegionConnector();
                }
                return _formRegionConnector;
            }

            return base.RequestService(serviceGuid);
        }

        #endregion


        #region Startup

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            System.Windows.Forms.Application.EnableVisualStyles();

            try
            {
                // Initialize our UI elements collection, and hook up the 
                // NewInspector event so that we can add each inspector to 
                // the collection as it is created.
                _uiElements = new UserInterfaceElements();
                _inspectors = this.Application.Inspectors;
                _inspectors.NewInspector +=
                    new Outlook.InspectorsEvents_NewInspectorEventHandler(
                    inspectors_NewInspector);
            }
            catch (COMException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        #endregion


        #region NewInspector

        // When a new Inspector opens, create a taskpane and attach 
        // it to this Inspector.
        void inspectors_NewInspector(Outlook.Inspector Inspector)
        {
            CreateTaskPane(Inspector);
        }

        // We factor this behavior out to a public method, so that 
        // it can be called independently of the NewInspector event.
        // This is to allow for the race condition where ribbon
        // callbacks can come in before the NewInspector event is
        // hooked up.
        public Office.Core.CustomTaskPane CreateTaskPane(
            Outlook.Inspector inspector)
        {
            Office.Core.CustomTaskPane taskpane = null;

            try
            {
                // Create a new task pane, and set its width to match our 
                // SimpleControl width.
                taskpane = _taskPaneConnector.CreateTaskPane(
                        _controlProgId, _controlTitle, inspector);
                if (taskpane != null)
                {
                    
                    taskpane.Width = 230;

                    // Map the task pane to the inspector and cache it 
                    // in our collection.
                    _uiElements.Add(new UserInterfaceContainer(
                        inspector, taskpane, _ribbonConnector));
                }
            }
            catch (COMException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return taskpane;
        }


        #endregion

    }
}
