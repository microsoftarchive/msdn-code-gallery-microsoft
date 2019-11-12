/****************************** Module Header ******************************\
Module Name:  Ribbon1.cs
Project:      CSOfficeRibbonAccessibility
Copyright (c) Microsoft Corporation.

This source is subject to the Microsoft Public License.
See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
All other rights reserved.

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Office.Tools.Ribbon;


namespace CSOfficeRibbonAccessibility
{
    public partial class Ribbon1 : OfficeRibbon
    {
        public Ribbon1()
        {
            InitializeComponent();
        }

        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
        }

        private void btnShowForm_Click(object sender, RibbonControlEventArgs e)
        {
            RibbonInfoForm form = new RibbonInfoForm();
            form.Show();
        }
    }
}
