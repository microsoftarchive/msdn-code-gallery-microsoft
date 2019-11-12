using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.WebControls;

namespace ECM_FindTaggedItems.Layouts.ECM_FindTaggedItems
{
    /// <summary>
    /// This application page demonstrates how to locate a term in a Managed Metadata 
    /// Service termset, then use it to locate all the items in a list that have been
    /// tagged with it. It uses a CAML query to search the list.
    /// </summary>
    /// <remarks>
    /// Before you use this sample, you must have a Managed Metadata service application
    /// connected to your site collection and have tagged items in a list with a term from
    /// the term store. Check the contents of the Taxonomy Hidden List in your site to
    /// ensure these conditions are met. This list is at /lists/TaxonomyHiddenList/AllItem.aspx
    /// in your SharePoint site collection.
    /// </remarks>
    public partial class FindTaggedItems : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void getItemsButton_Click(object sender, EventArgs e)
        {
            if ((searchForTextBox.Text != String.Empty) && (nameOfListTextbox.Text != String.Empty))
            {
                SPSite currentSite = SPContext.Current.Site;
                //We must create a new Taxonomy Session object
                TaxonomySession session = new TaxonomySession(currentSite);
                //Get the termstore
                TermStore termstore = session.TermStores["Managed Metadata Service"];

                //Now we can find the collection of terms that satisfy the user's request
                TermCollection terms = termstore.GetTerms(searchForTextBox.Text, true);

                if (terms.Count > 0)
                {
                    //For simplicity we'll just use the first term returned
                    Term firstTerm = terms[0];
                    resultLabel.Text = "Term Found. Term ID: " + firstTerm.Id + "<br /><br />";

                    //Now that we have a term object to search for, we must get the
                    //WssIds of that term in the current site collection. Note that a
                    //single term from a term store can have different WssIds in each 
                    //site collection. Use the GetWssIdsOfTerm method to findout what 
                    //they are. You can also see these in the Taxonomy Hidden List.
                    int[] WssIds = TaxonomyField.GetWssIdsOfTerm(currentSite,
                        termstore.Id, firstTerm.TermSet.Id, firstTerm.Id, false, 500);

                    if (WssIds.GetLength(0) == 0)
                    {
                        //This tag is in the tagstore but not the Taxonomy Hidden List
                        //That usually means it hasn't been used in the current site
                        resultLabel.Text += "There are no entries for that term in the taxonomy hidden list. " +
                            "Have you tagged an item in the site with the tag you search for?";
                    }
                    else
                    {
                        //Tell the user what the WSS IDs for the located term are 
                        //and formulate a CAML query
                        string camlQuery = "<Where><In><FieldRef Name='Tags' LookupId='TRUE' /><Values>";
                        resultLabel.Text += "Here are the WssIds: <br />";
                        foreach (int WssId in WssIds)
                        {
                            camlQuery += String.Format("<Value Type='Integer'>{0}</Value>", WssId);
                            resultLabel.Text += WssId.ToString() + "<br />";
                        }
                        //Complete the CAML query
                        camlQuery += @"</Values></In></Where>";
                        
                        //Get the list and create the query
                        SPList searchList = currentSite.RootWeb.Lists[nameOfListTextbox.Text];
                        SPQuery query = new SPQuery();
                        query.Query = camlQuery;
                        
                        if (searchList != null)
                        {
                            //Run the query
                            SPListItemCollection items = searchList.GetItems(query);
                            //Tell the user what tagged items were found
                            resultLabel.Text += "<br />Number of items with that tag: " + items.Count.ToString() + "<br /><br />";
                            foreach (SPItem currentItem in items)
                            {
                                resultLabel.Text += "Item Title: " + currentItem["Title"] + "<br />";
                                resultLabel.Text += "Item ID: " + currentItem.ID + "<br />";
                            }
                        }
                        else
                        {
                            //The list name the user entered did not match the name of a list
                            resultLabel.Text += "<br />There is no list by that name.";
                        }
                    }
                }
                else
                {
                    //The text the user entered did not match the name of a term in the termstore
                    resultLabel.Text = "There is no term in the termstore that matches your text";
                }
            }
        }
    }
}
