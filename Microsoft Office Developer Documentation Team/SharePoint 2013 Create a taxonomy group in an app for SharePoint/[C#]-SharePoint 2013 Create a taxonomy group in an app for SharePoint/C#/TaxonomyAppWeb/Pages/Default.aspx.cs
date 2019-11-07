using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;

namespace TaxonomyAppWeb.Pages
{
    public partial class Default : System.Web.UI.Page
    {
        private string ContextToken { get; set; }
        private string HostWeb { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            // The following code gets the client context and Title property by using TokenHelper.
            // To access other properties, you may need to request permissions on the host web.

            ContextToken = TokenHelper.GetContextTokenFromRequest(Page.Request);
            // save the context token in case we need it on subsequent requests
            if (ContextToken != null) Session["contextToken"] = ContextToken;
            // look in session to see if context token is present
            else if (Session["contextToken"] != null) ContextToken = Session["contextToken"].ToString();

            HostWeb = Page.Request["SPHostUrl"];
            // save the hostweb url in case we need it on subsequent requests
            if (HostWeb != null) Session["hostWeb"] = HostWeb;
            // look in session to see if host web url is present
            else if (Session["hostWeb"] != null) HostWeb = Session["hostWeb"].ToString();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            CreatePlantTaxonomy(ContextToken, HostWeb);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            DisplayPlantTaxonomy(ContextToken, HostWeb);
        }

        private void CreatePlantTaxonomy(string contextToken, string hostWeb)
        {
            using (var clientContext 
                = TokenHelper.GetClientContextWithContextToken(hostWeb, contextToken, Request.Url.Authority))
            {
                //Create Taxonomy
                //To manage terms, we must create a new Taxonomy Session object
                TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);

                //Get the termstore
                //TermStore termstore = session.TermStores["Managed Metadata Service"];
                TermStore termStore = session.GetDefaultSiteCollectionTermStore();

                //Create a term group
                //Group plantsGroup = termstore.CreateGroup("Plants", Guid.NewGuid());
                TermGroup plantsGroup = termStore.CreateGroup("Plants", Guid.NewGuid());

                //Create a term set
                TermSet flowersTermSet = plantsGroup.CreateTermSet("Flowers", Guid.NewGuid(), 1033);

                //Create some terms
                Term tulipsTerm = flowersTermSet.CreateTerm("Tulips", 1033, Guid.NewGuid());
                Term orchidsTerm = flowersTermSet.CreateTerm("Orchids", 1033, Guid.NewGuid());
                Term daffodilsTerm = flowersTermSet.CreateTerm("Daffodils", 1033, Guid.NewGuid());

                //Create a child term within the Orchids term
                Term vanillaTerm = orchidsTerm.CreateTerm("Vanilla", 1033, Guid.NewGuid());

                //You should set properties for every term. In this example, we'll
                //do just one for brevity
                vanillaTerm.SetDescription("A common orchid whose pods are used in desserts", 1033);
                //Use CreateLabel to add synomyns and alternates
                vanillaTerm.CreateLabel("Vanilla planifolia", 1033, false);
                vanillaTerm.CreateLabel("Flat-leaved vanilla", 1033, false);

                //When we are finished making changes, we must commit them
                try
                {
                    //termstore.CommitAll();
                    clientContext.ExecuteQuery();
                    resultsLabel.Text = "Taxonomy created successfully";
                }
                catch (Exception ex)
                {
                    resultsLabel.Text = "There was an error: " + ex.Message;
                }

            }
        }

        private void DisplayPlantTaxonomy(string contextToken, string hostWeb)
        {
            using (var clientContext = TokenHelper.GetClientContextWithContextToken(hostWeb, contextToken, Request.Url.Authority))
            {
                //Create Taxonomy
                //To manage terms, we must create a new Taxonomy Session object
                TaxonomySession session = TaxonomySession.GetTaxonomySession(clientContext);

                //Get the termstore
                //TermStore termstore = session.TermStores["Managed Metadata Service"];
                TermStore termStore = session.GetDefaultSiteCollectionTermStore();

                clientContext.Load(termStore,
                    store => store.Name,
                    store => store.Groups.Include(
                        group => group.Name,
                        group => group.TermSets.Include(
                            termSet => termSet.Name,
                             termSet => termSet.Terms.Include(
                                term => term.Name)
                        )
                    ).Where(group => group.Name == "Plants")
                );
                clientContext.ExecuteQuery();

                FormatTermStoreAsHtml(termStore);
            }
        }

        private void FormatTermStoreAsHtml(TermStore termStore)
        {
            string taxonomyHtml = string.Empty;

            foreach (TermGroup group in termStore.Groups)
            {
                taxonomyHtml = taxonomyHtml + "<h2>Group: " + group.Name + "</h2>";

                foreach (TermSet termSet in group.TermSets)
                {
                    taxonomyHtml = taxonomyHtml + "<h3>TermSet: " + termSet.Name + "</h3>";

                    foreach (Term term in termSet.Terms)
                    {
                        taxonomyHtml = taxonomyHtml + "<p>Term: " + term.Name + "</p>";
                    }
                }
            }
            plantTaxonomy.Text = taxonomyHtml;
        }
    }
}
