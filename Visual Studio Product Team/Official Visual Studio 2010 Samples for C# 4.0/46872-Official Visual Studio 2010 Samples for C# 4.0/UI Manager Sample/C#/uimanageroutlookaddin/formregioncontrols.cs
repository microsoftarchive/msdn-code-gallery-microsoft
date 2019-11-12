// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)


using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using Microsoft.Vbe.Interop.Forms;
using UiManagerOutlookAddIn.AddinUtilities;

namespace UiManagerOutlookAddIn
{
    // The FormRegionControls class wraps the references to the controls on the
    // custom form region. We'll instantiate a fresh instance of this class for
    // each custom form region that gets opened. This way, we ensure that any
    // UI response is specific to this instance (eg, when the user clicks our
    // commandbutton, we can fetch the textbox text for this same instance.
    public class FormRegionControls : IFormRegionControls
    {

        #region Fields

        private UserForm _form;
        private Outlook.Inspector _inspector;
        private Outlook.OlkListBox _coffeeList;
        private Microsoft.Vbe.Interop.Forms.TextBox _ordersText;
        private String[] _listItems = 
            {"Colombia Supremo", "Ethiopia Longberry Harrar", 
            "Sumatra Mandehling", "Mocha Java" , "Jamaica Blue Mountain", 
            "Costa Rica Tarrazu", "Monsooned Malabar" };
        private const String _formRegionListBoxName = "coffeeListBox";
        private const String _ordersTextBoxName = "ordersTextBox";
        private const String _mailServiceGroup = "mailServiceGroup";

        [CLSCompliant(false)]
        public Outlook.Inspector Inspector
        {
            get { return _inspector; }
            set { _inspector = value; }
        }

        #endregion


        // This method is not CLSCompliant because of its input parameter.
        [CLSCompliant(false)]
        public FormRegionControls(Outlook.FormRegion region)
        {
            if (region != null)
            {
                try
                {
                    // Cache a reference to this region, this region's 
                    // inspector, and the controls on this region.
                    _inspector = region.Inspector;
                    _form = region.Form as UserForm;
                    _ordersText = _form.Controls.Item(_ordersTextBoxName) 
                        as Microsoft.Vbe.Interop.Forms.TextBox;
                    _coffeeList = _form.Controls.Item(_formRegionListBoxName) 
                        as Outlook.OlkListBox;

                    // Fill the listbox with some arbitrary strings.
                    for (int i = 0; i < _listItems.Length; i++)
                    {
                        _coffeeList.AddItem(_listItems[i], i);
                    }
                    _coffeeList.Change += new
                        Outlook.OlkListBoxEvents_ChangeEventHandler(
                        _coffeeList_Change);
                }
                catch (COMException ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                }
            }
        }


        // The user has changed the selection in the listbox in the custom 
        // form region.
        private void _coffeeList_Change()
        {
            UserInterfaceContainer uiContainer = 
                Globals.ThisAddIn._uiElements.GetUIContainerForInspector(
                _inspector);

            // Make the add-in service ribbon buttons visible.
            uiContainer.ShowRibbonControl(_mailServiceGroup);

            // Get the usercontrol in the task pane, and copy the text of the 
            // item selected in the form region's listbox into the taskpane's
            // listbox.
            SimpleControl sc = 
                (SimpleControl)uiContainer.TaskPane.ContentControl;
            sc._coffeeGroup.Visible = true;
            sc._coffeeList.Items.Add(_coffeeList.Text);
        }


        // Set the text value of the orders TextBox in the form region.
        public void SetControlText(String controlName, String text)
        {
            switch (controlName)
            {
                case _ordersTextBoxName :
                    _ordersText.Text = text;
                    break;
            }
        }

    }
}
