// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MasterDetailsRelationships
{
    public partial class CompanyData
    {
        public partial class StatusDataTable
        {
            public StatusRow FindByStatus(string Status)
            {
                DataRow[] statusTableRows = this.Select(String.Format("Status ='{0}'", Status));
                System.Diagnostics.Debug.Assert(statusTableRows.Length == 1, "Status table data is invalid.");
                return statusTableRows[0] as StatusRow;
            }
        }

        public partial class ProductsDataTable
        {
            public ProductsRow FindByName(string Name)
            {
                DataRow[] productsTableRows = this.Select(String.Format("ProductName ='{0}'", Name));
                System.Diagnostics.Debug.Assert(productsTableRows.Length == 1, "Products table data is invalid.");
                return productsTableRows[0] as ProductsRow;
            }
        }
    }
}
