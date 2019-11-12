using System;
using Microsoft.Office.RecordsManagement.RecordsRepository;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace RM_DeclareRecords.Layouts.RM_DeclareRecords
{
    /// <summary>
    /// This application page demonstrates how to evaluate, declare and un-declare
    /// records in-place. It loops through all the items in a list, counts records
    /// and non-records, then declares or undeclares all items as records depending
    /// on which button you click
    /// </summary>
    /// <remarks>
    /// You must enable the In-Place Records feature at the site-collection level 
    /// before executing this code. Also it's helpful to try manually declaring a
    /// record.
    /// </remarks>
    public partial class DeclareRecords : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void declareRecordsButton_Click(object sender, EventArgs e)
        {
            //These integers are to count records and non-records
            int existingRecords = 0;
            int existingNonRecords = 0;

            //Get the list
            SPWeb currentWeb = SPContext.Current.Web;
            SPList list = currentWeb.Lists[listNameTextbox.Text];

            if (list != null)
            {
                //Found a list, loop through the items
                foreach (SPListItem item in list.Items)
                {
                    //Is this item already a record? Use the static Records.IsRecord method to find out
                    if (Records.IsRecord(item))
                    {
                        //This is already a record. Count it but take no action
                        existingRecords++;
                    }
                    else
                    {
                        //This is not a record. 
                        existingNonRecords++;
                        //Declare this as a record using the static Records.DeclareItemAsRecord method
                        Records.DeclareItemAsRecord(item);
                    }
                }
                //Tell the user what happened.
                resultsLabel.Text = String.Format("There were {0} records and {1} non-records. <br />", existingRecords, existingNonRecords);
                resultsLabel.Text += "All items are now declared as records.";
            }
            else
            {
                //Couldn't find the list
                resultsLabel.Text = "Could not find a list by that name";
            }
        }

        protected void undeclareRecordsButton_Click(object sender, EventArgs e)
        {
            //These integers are to count records and non-records
            int existingRecords = 0;
            int existingNonRecords = 0;

            //Get the list
            SPWeb currentWeb = SPContext.Current.Web;
            SPList list = currentWeb.Lists[listNameTextbox.Text];

            if (list != null)
            {
                //Found a list, loop through the items
                foreach (SPListItem item in list.Items)
                {
                    //Is this item already a record? Use the static Records.IsRecord method to find out
                    if (Records.IsRecord(item))
                    {
                        //This is already a record. 
                        existingRecords++;
                        //Undeclare this record
                        Records.UndeclareItemAsRecord(item);
                    }
                    else
                    {
                        //This is not a record. Count it 
                        existingNonRecords++;
                    }
                }
                //Tell the user what happened.
                resultsLabel.Text = String.Format("There were {0} records and {1} non-records. <br />", existingRecords, existingNonRecords);
                resultsLabel.Text += "All items are now undeclared as records.";
            }
            else
            {
                //Couldn't find the list
                resultsLabel.Text = "Could not find a list by that name";
            }
        }
    }
}
