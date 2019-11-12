using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.WebControls;

namespace ECM_CreateTaxonomy.Layouts.ECM_CreateTaxonomy
{
    /// <summary>
    /// This application page creates and populates a group of terms in the
    /// Managed Metadata Service service application.
    /// </summary>
    /// <remarks>
    /// You can see the results of this code by managing the Managed Metadata service
    /// application in Central Administration. Also, after this code runs, users can 
    /// add these terms as tags to pages, items, and documents in SharePoint sites.
    /// </remarks>
    public partial class CreateTaxonomy : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void createTaxonomyButton_Click(object sender, EventArgs e)
        {
            SPSite currentSite = SPContext.Current.Site;
            //To manage terms, we must create a new Taxonomy Session object
            TaxonomySession session = new TaxonomySession(currentSite);
            //Get the termstore
            TermStore termstore = session.TermStores["Managed Metadata Service"];

            //Create a term group
            Group plantsGroup = termstore.CreateGroup("Plants");

            //Create a term set
            TermSet flowersTermSet = plantsGroup.CreateTermSet("Flowers");

            //Create some terms
            Term tulipsTerm = flowersTermSet.CreateTerm("Tulips", 1033);
            Term orchidsTerm = flowersTermSet.CreateTerm("Orchids", 1033);
            Term daffodilsTerm = flowersTermSet.CreateTerm("Daffodils", 1033);

            //Create a child term within the Orchids term
            Term vanillaTerm = orchidsTerm.CreateTerm("Vanilla", 1033);

            //You should set properties for every term. In this example, we'll
            //do just one for brevity
            vanillaTerm.SetDescription("A common orchid whose pods are used in desserts", 1033);
            //Use CreateLabel to add synomyns and alternates
            vanillaTerm.CreateLabel("Vanilla planifolia", 1033, false);
            vanillaTerm.CreateLabel("Flat-leaved vanilla", 1033, false);

            //When we are finished making changes, we must commit them
            try
            {
                termstore.CommitAll();
                resultsLabel.Text = "Taxonomy created successfully";
            }
            catch (Exception ex)
            {
                resultsLabel.Text = "There was an error: " + ex.Message;
            }
        }
    }
}
