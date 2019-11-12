/********************************* Module Header *********************************\
Module Name:    RibbonInfoForm.cs
Project:        CSOfficeRibbonAccessibility
Copyright (c) Microsoft Corporation.

This example illustrates how to pinvoke the Microsoft Active Accessibilty 
(MSAA) API to automate Office Ribbon controls. The code calls the following 
APIs,

    AccessibleObjectFromWindow, 
    AccessibleChildren,
    GetRoleText,
    GetStateText,

to display the whole structure of the Office ribbon, including tabs, groups,
and controls. It also shows how to nagivate to a tab and execute button 
function programmatically.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*********************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Office.Core;


namespace CSOfficeRibbonAccessibility
{
    public partial class RibbonInfoForm : Form
    {
        // Helper Data Structure. It is used to store the list box item.
        internal class ListBoxItem
        {
            public ListBoxItem(string name, IAccessible item)
            {
                this.Name = name;
                this.accItem = item;
            }

            public override string ToString()
            {
                return Name;
            }

            // Fields
            public string Name;
            public IAccessible accItem;
        }


        // The top accessible window
        private IAccessible TopWindow;


        public RibbonInfoForm()
        {
            InitializeComponent();
        }


        // In the form load event, get all accessible tab objects and list them in 
        // the lbTabs list box.
        private void RibbonInfoForm_Load(object sender, EventArgs e)
        {
            // Initialize the IAccessible interface of the top window.
            this.TopWindow = MSAAHelper.GetAccessibleObjectFromHandle(
                Process.GetCurrentProcess().MainWindowHandle);

            // Get the IAccessible object of the Ribbon property page.
            IAccessible ribbon = MSAAHelper.GetAccessibleObjectByNameAndRole(
                this.TopWindow, new Regex("Ribbon"), "property page", false);

            // Find all visible ribbon tabs and show them in the list box. 
            IAccessible[] children = MSAAHelper.GetAccessibleChildren(
                MSAAHelper.GetAccessibleObjectByNameAndRole(ribbon, 
                new Regex("Ribbon Tabs"), "page tab list", true));

            foreach (IAccessible child in children)
            {
                if (child.accChildCount > 0)
                {
                    IAccessible[] tabs = MSAAHelper.GetAccessibleChildren(child);
                    foreach (IAccessible tab in tabs)
                    {
                        String state = MSAAHelper.GetStateText(
                            (MSAAStateConstants)tab.get_accState(0));
                        if (!state.Contains("invisible"))
                        {
                            this.lbTabs.Items.Add(new ListBoxItem(tab.get_accName(0), tab));
                        }
                    }
                }
            }
        }


        // When the selected item in lbTabs changes, the code navigates to the 
        // selected tab. 
        private void lbTabs_SelectedValueChanged(object sender, EventArgs e)
        {
            ListBoxItem item = this.lbTabs.SelectedItem as ListBoxItem;
            item.accItem.accDoDefaultAction(0);
        }


        // When the btnListChildGroups button is clicked, the code probes all 
        // ribbon groups in the selected tab and list them in the lbGroups list box.
        private void btnListChildGroups_Click(object sender, EventArgs e)
        {
            this.lbGroups.Items.Clear();

            // Find the selected tab.
            ListBoxItem item = this.lbTabs.SelectedItem as ListBoxItem;
            IAccessible tab = MSAAHelper.GetAccessibleObjectByNameAndRole(
                this.TopWindow, new Regex(item.Name), "property page", true);
            if (tab == null)
            {
                MessageBox.Show("Error: the " + item.Name + " tab cannot be found");
                return;
            }

            // Get the groups, and list the groups in the lbGroups control.
            List<IAccessible> groups = new List<IAccessible>();
            MSAAHelper.GetAccessibleObjectListByRole(tab, "tool bar", ref groups, true);
            foreach (IAccessible group in groups)
            {
                this.lbGroups.Items.Add(new ListBoxItem(group.get_accName(0), group));
            }
        }


        // When the btnListChildControls button is clicked, the code probes all 
        // controls in the selected group and list them in the lbControls list box.
        private void btnListChildControls_Click(object sender, EventArgs e)
        {
            this.lbControls.Items.Clear();

            // Find the selected group.
            ListBoxItem item = this.lbGroups.SelectedItem as ListBoxItem;
            if (item != null)
            {
                IAccessible group = MSAAHelper.GetAccessibleObjectByNameAndRole(
                    this.TopWindow, new Regex(item.Name), "tool bar", true);
                if (group == null)
                {
                    MessageBox.Show("Error: the " + item.Name + " group cannot be found");
                    return;
                }

                // Get the controls, and list the controls in the lbControls control.
                IAccessible[] controls = MSAAHelper.GetAccessibleChildren(group);
                foreach (IAccessible control in controls)
                {
                    this.lbControls.Items.Add(new ListBoxItem(control.get_accName(0), control));
                }
            }
        }


        // When the btnExecuteControl is clicked, the code executes the default 
        // action of the currently selected control.
        private void btnExecuteControl_Click(object sender, EventArgs e)
        {
            ListBoxItem item = this.lbControls.SelectedItem as ListBoxItem;
            if (item != null)
            {
                item.accItem.accDoDefaultAction(0);
            }
        }
    }
}