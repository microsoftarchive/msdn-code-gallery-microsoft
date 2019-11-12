// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office;
using System.Globalization;
using UiManagerOutlookAddIn.AddinUtilities;

namespace UiManagerOutlookAddIn
{

    [ComVisible(true)]
    [Guid("66299133-E2CC-46c1-8C47-B73DA7203067")]
    [ProgId("UiManager.RibbonConnector")]
    public class RibbonConnector : 
        Office.Core.IRibbonExtensibility, IRibbonConnector 
    {

        #region Standard operations

        public RibbonConnector()
        {
        }

        // This method is not CLSCompliant because of its Office parameter.
        [CLSCompliant(false)]
        public void OnLoad(Office.Core.IRibbonUI ribbonUI)
        {
            this._ribbon = ribbonUI;
        }

        public string GetCustomUI(string RibbonID)
        {
            string xml = null;

            // We have two different ribbon customizations: one for Task
            // inspectors, and one for all other inspectors.
            switch (RibbonID)
            {
                case _taskItemName:
                    xml = Properties.Resources.TaskRibbon;
                    break;
                default:
                    xml = Properties.Resources.SimpleRibbon;
                    break;
            }
            return xml;
        }

        // Our ribbon XML specifies that the addinServiceButtons group, which
        // contains the sendList controls, is conditionally visible. This 
        // method is not CLSCompliant because of its Office parameter.
        [CLSCompliant(false)]
        public bool GetVisible(Office.Core.IRibbonControl control)
        {
            if (control == null)
            {
                return false;
            }

            // Match up this control instance (determined by its inspector) in
            // the collection, and return the current value of the cached 
            // visibility state.
            Outlook.Inspector inspector = control.Context as Outlook.Inspector;
            UserInterfaceContainer uiContainer =
                Globals.ThisAddIn._uiElements.GetUIContainerForInspector(inspector);
            return uiContainer.IsControlVisible;
        }

        #endregion

        #region Fields

        private Office.Core.IRibbonUI _ribbon;


        private const String _taskItemName = "Microsoft.Outlook.Task";
        private const String _mailAddressee = "someone@example.com";
        private const String _orderName = "Coffee";
        private const String _ordersTextBoxName = "ordersTextBox";
        private static int _orderCount;

        #endregion


        #region OnToggleTaskPane

        // This method is not CLSCompliant because of its input parameter.
        [CLSCompliant(false)]
        public void OnToggleTaskPane(
            Office.Core.IRibbonControl control, bool isPressed)
        {
            // Toggle the visibility of the custom taskpane.
            if (control != null)
            {
                // Find the inspector for this ribbon, so that we can find the 
                // corresponding task pane from our collection.
                Outlook.Inspector inspector = 
                    (Outlook.Inspector)control.Context;
                Office.Core.CustomTaskPane taskpane =
                    Globals.ThisAddIn._uiElements.GetTaskPaneForInspector(
                    inspector);

                // If we've been called before we've had a chance to add this 
                // Inspector/task pane to the collection, we can add it now.
                if (taskpane == null)
                {
                    taskpane = Globals.ThisAddIn.CreateTaskPane(inspector);
                }

                taskpane.Visible = isPressed;
            }
        }

        #endregion


        #region Button handler

        private String GetTextFromTaskPane(Outlook.Inspector inspector)
        {
            String coffeeText = null;

            if (inspector != null)
            {
                // Get the usercontrol in the task pane.
                Office.Core.CustomTaskPane taskpane =
                    Globals.ThisAddIn._uiElements.GetTaskPaneForInspector(inspector);
                SimpleControl sc = (SimpleControl)taskpane.ContentControl;

                // Compose the mail body from the strings in the task pane listbox.
                StringBuilder builder = new StringBuilder();
                foreach (string s in sc._coffeeList.Items)
                {
                    builder.AppendLine(s);
                }
                coffeeText = builder.ToString();
            }

            return coffeeText;
        }

        // This method is not CLSCompliant because of its Office parameter.
        [CLSCompliant(false)]
        public void OnSendList(Office.Core.IRibbonControl control)
        {
            if (control != null)
            {
                try
                {
                    Outlook.Inspector inspector =
                        (Outlook.Inspector)control.Context;
                    String coffeeText = GetTextFromTaskPane(inspector);

                    // Create a new email from the input parameters, and send it.
                    Outlook._MailItem mi =
                        (Outlook._MailItem)
                        Globals.ThisAddIn.Application.CreateItem(
                        Outlook.OlItemType.olMailItem);
                    mi.Subject = _orderName;
                    mi.Body = coffeeText;
                    mi.To = _mailAddressee;
                    mi.Send();

                    // Update the count of orders in the form region.
                    UserInterfaceContainer uiContainer =
                        Globals.ThisAddIn._uiElements.GetUIContainerForInspector(
                        inspector);
                    CultureInfo cultureInfo = new CultureInfo("en-us");
                    uiContainer.FormRegionControls.SetControlText(
                        _ordersTextBoxName, (++_orderCount).ToString(cultureInfo));
                }
                catch (COMException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
        }

        #endregion


        #region IRibbonConnector Members

        [CLSCompliant(false)]
        public Office.Core.IRibbonUI Ribbon
        {
            get { return _ribbon; }
            set { _ribbon = value; }
        }

        #endregion
    }
}
