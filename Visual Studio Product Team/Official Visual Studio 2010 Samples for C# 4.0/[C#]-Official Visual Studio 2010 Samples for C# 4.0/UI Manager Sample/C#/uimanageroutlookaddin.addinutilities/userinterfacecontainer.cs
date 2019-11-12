// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)


using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office;

namespace UiManagerOutlookAddIn.AddinUtilities
{
    public class UserInterfaceContainer
    {
        
        #region Contained UI elements

        private Outlook.InspectorEvents_10_Event _inspectorEvents;
        private Outlook.Inspector _inspector;
        private IFormRegionControls _formRegionControls;
        private IRibbonConnector _ribbonConnector;
        private Office.Core.CustomTaskPane _taskPane;
        private bool _isControlVisible;

        public event EventHandler InspectorClose;

        [CLSCompliant(false)]
        public Outlook.Inspector Inspector
        {
            get { return _inspector; }
            set { _inspector = value; }
        }

        [CLSCompliant(false)]
        public Office.Core.CustomTaskPane TaskPane
        {
            get { return _taskPane; }
            set { _taskPane = value; }
        }

        [CLSCompliant(false)]
        public IFormRegionControls FormRegionControls
        {
            get { return _formRegionControls; }
            set { _formRegionControls = value; }
        }


        [CLSCompliant(false)]
        public IRibbonConnector RibbonConnector
        {
            get { return _ribbonConnector; }
            set { _ribbonConnector = value; }
        }

        #endregion


        #region Ribbon control accessors

        public bool IsControlVisible
        {
            get { return _isControlVisible; }
        }

        public void ShowRibbonControl(String ribbonControlId)
        {
            _isControlVisible = true;
            _ribbonConnector.Ribbon.InvalidateControl(ribbonControlId);
        }

        public void HideRibbonControl(String ribbonControlId)
        {
            _isControlVisible = false;
            _ribbonConnector.Ribbon.InvalidateControl(ribbonControlId);
        }

        #endregion


        #region ctor

        // This method is not CLS compliant because of its Office parameters.
        [CLSCompliant(false)]
        public UserInterfaceContainer(
            Outlook.Inspector inspector, 
            Office.Core.CustomTaskPane taskPane, 
            IRibbonConnector ribbonConnector)
        {
            if (inspector != null)
            {
                _inspector = inspector;
                _taskPane = taskPane;
                _ribbonConnector = ribbonConnector;

                // Sink the InspectorClose event so that we can clean up.
                _inspectorEvents = (Outlook.InspectorEvents_10_Event)_inspector;
                _inspectorEvents.Close +=
                    new Outlook.InspectorEvents_10_CloseEventHandler(
                    _inspectorEvents_Close);
            }
        }

        #endregion


        #region Inspector Close event

        void _inspectorEvents_Close()
        {
            // Remove ourselves from the collection of UI objects, 
            // unhook the Close event, and clean up all references.
            _inspectorEvents.Close -=
                new Outlook.InspectorEvents_10_CloseEventHandler(
                _inspectorEvents_Close);

            if (InspectorClose != null)
            {
                // Tell anyone who's listening that we're being closed.
                InspectorClose(this, new EventArgs());
            }

            _inspector = null;
            _inspectorEvents = null;
            _taskPane = null;
            _formRegionControls = null;
            _ribbonConnector = null;
        }

        #endregion

    }
}
