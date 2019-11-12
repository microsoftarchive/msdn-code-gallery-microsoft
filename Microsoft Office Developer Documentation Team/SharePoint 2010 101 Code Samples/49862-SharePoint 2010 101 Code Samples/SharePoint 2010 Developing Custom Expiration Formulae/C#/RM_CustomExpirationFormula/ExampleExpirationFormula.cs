using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Office.RecordsManagement.PolicyFeatures;
using Microsoft.SharePoint;

namespace RM_CustomExpirationFormula
{
    /// <summary>
    /// An Information Management Policy is a set of rules that govern how a certain content
    /// type in a certain list or library behaves. Out of the box, a policy can have up 
    /// to four policy items in it. These define 1. how user actions are audited, 2. how 
    /// items are labelled, 3. how items get barcodes for id purposes, 4. How long items
    /// are retained and what happens when they expire. Each of these 4 is called a Policy
    /// Feature and you can define extra policy features in code. 
    /// For the expiration and retention policy item, you can specify when an item expires.
    /// Out of the box, this must be based on the date it was create, modified, declared as
    /// a record and you can specify a number of days, months, or years from that date. If
    /// you want two calculate on a different field or make more complex logic, you must
    /// develop a custom expiration formula. This solution demonstrates how to do that.
    /// </summary>
    /// <remarks>
    /// To create and deploy a custom expiration formula you must define a class that 
    /// implements the IExpirationFormula interface and its ComputeExpireDate method. Then 
    /// use a feature receiver to install the formula. 
    /// </remarks>
    class ExampleExpirationFormula : IExpirationFormula
    {
        public DateTime? ComputeExpireDate(SPListItem item, XmlNode parametersData)
        {
            //This example returns the last day of the current month. 
            //Therefore all items will expire on that date if they use this expiration formula
            DateTime modifiedDate = (DateTime)item["Modified"];
            if (modifiedDate != null)
            {
                DateTime lastDayOfThisMonth = new DateTime(modifiedDate.Year, modifiedDate.Month, 1).AddMonths(1).AddDays(-1);
                return lastDayOfThisMonth;
            }
            else
            {
                //There isn't a modified date field so just return null
                return null;
            }
        }
    }
}
