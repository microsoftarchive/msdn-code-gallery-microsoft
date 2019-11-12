/****************************** Module Header ******************************\
* Module Name:   CustomTaskPanel.cs
* Project:       CSExcelNewEventForShapes
* Copyright (c)  Microsoft Corporation.
* 
* This Class is the custom Task Panel.
* The ListBox is used to show the event messages.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/



using System.Windows.Forms;

namespace CSExcelNewEventForShapes
{
    public partial class CustomTaskPanel : UserControl
    {
        public CustomTaskPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Add a Message into ListBox 
        /// </summary>
        /// <param name="message">message string</param>
        public void AddMessage(string message)
        {
            int index = lstMessage.Items.Add(message);
            lstMessage.SelectedIndex = index;
        }

        // Clear Message
        public void Clear()
        {
            lstMessage.Items.Clear();
            lstMessage.SelectedIndex = -1;
        }
    }
}
