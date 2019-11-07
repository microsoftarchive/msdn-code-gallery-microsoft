// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)


using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Office = Microsoft.Office;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace UiManagerOutlookAddIn.AddinUtilities
{
    public class UserInterfaceElements
    {

        #region Initialization

        private List<UserInterfaceContainer> _items;

        public UserInterfaceElements()
        {
            _items = new List<UserInterfaceContainer>();
        }

        #endregion


        #region Collection updates

        public void Add(UserInterfaceContainer uiContainer)
        {
            _items.Add(uiContainer);
            uiContainer.InspectorClose += 
                new EventHandler(uiContainer_InspectorClose);
        }

        // When the inspector is closed, so is the form region, so we need 
        // to remove this instance from our collection.
        void uiContainer_InspectorClose(object sender, EventArgs e)
        {
            _items.Remove(sender as UserInterfaceContainer);
        }

        public bool Remove(UserInterfaceContainer uiContainer)
        {
            return _items.Remove(uiContainer);
        }

        // Update the UI container object in our collection indicated by the
        // given inspector, by attaching the given FormRegionControls object.
        // This method is not CLS compliant because of its Office parameters.
        [CLSCompliant(false)]
        public bool AttachFormRegion(
            Outlook.Inspector inspector, 
            IFormRegionControls formRegionControls)
        {
            bool updateOK = false;

            // Find this inspector in the our collection of containers.
            UserInterfaceContainer uiContainer = 
                GetUIContainerForInspector(inspector);
            if (uiContainer != null)
            {
                uiContainer.FormRegionControls = formRegionControls;
                updateOK = true;
            }
            return updateOK;
        }

        #endregion


        #region Collection searches

        // Given an inspector, find the matching UI container object.
        // This method is not CLS compliant because of its Office parameter.
        [CLSCompliant(false)]
        public UserInterfaceContainer GetUIContainerForInspector(
            Outlook.Inspector inspector)
        {
            UserInterfaceContainer uiContainer = null;

            foreach (UserInterfaceContainer uic in _items)
            {
                if (uic.Inspector == inspector)
                {
                    uiContainer = uic;
                    break;
                }
            }
            return uiContainer;
        }

        // Given an inspector, return the matching task pane.
        // This method is not CLS compliant because of its Office parameter.
        [CLSCompliant(false)]
        public Office.Core.CustomTaskPane GetTaskPaneForInspector(
            Outlook.Inspector inspector)
        {
            Office.Core.CustomTaskPane taskpane = null;

            foreach (UserInterfaceContainer uic in _items)
            {
                if (uic.Inspector == inspector)
                {
                    taskpane = uic.TaskPane;
                    break;
                }
            }
            return taskpane;
        }

        // Given an inspector, return the matching ribbon.
        // This method is not CLS compliant because of its Office parameter.
        [CLSCompliant(false)]
        public IRibbonConnector GetRibbonForInspector(
            Outlook.Inspector inspector)
        {
            IRibbonConnector ribbonConnector = null;

            foreach (UserInterfaceContainer uic in _items)
            {
                if (uic.Inspector == inspector)
                {
                    ribbonConnector = uic.RibbonConnector;
                    break;
                }
            }
            return ribbonConnector;
        }

        // Given a UserControl, return the matching ribbon.
        // This method is not CLS compliant because of its return type.
        [CLSCompliant(false)]
        public IRibbonConnector GetRibbonForUserControl(
            UserControl userControl)
        {
            IRibbonConnector ribbonConnector = null;

            foreach (UserInterfaceContainer uic in _items)
            {
                if (uic.TaskPane.ContentControl == userControl)
                {
                    ribbonConnector = uic.RibbonConnector;
                    break;
                }
            }
            return ribbonConnector;
        }

        // Given a UserControl, return the matching UI container object.
        public UserInterfaceContainer GetUIContainerForUserControl(
            UserControl userControl)
        {
            UserInterfaceContainer uiContainer = null;

            foreach (UserInterfaceContainer uic in _items)
            {
                if (uic.TaskPane.ContentControl == userControl)
                {
                    uiContainer = uic;
                    break;
                }
            }
            return uiContainer;
        }

        #endregion

    }
}
