using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Net;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using Microsoft.SharePoint.Client.Publishing.Navigation;

namespace NavigationDemos
{
    public static class TestConfig
    {
        public const string ServerUrl = "http://contoso/sites/pub1";

        public static readonly Guid NavTermSetId = new Guid("E742CC24-69E6-428E-81CA-156119FF0979");

        public static readonly Guid TaggingTermSetId = new Guid("81C5E778-07CA-48e3-AC04-FA8AFF6F651B");

    }
}
public static class DemoUtilities
{
    /// Creates a TermSet object for demonstration purposes.  If it already exists, it will be deleted
    /// and then recreated.

    public static NavigationTermSet RecreateSampleNavTermSet(
            ClientContext clientContext, TaxonomySession taxonomySession, Web web)
    {
        clientContext.Load(taxonomySession, ts => ts.TermStores);
        clientContext.ExecuteQuery();

        // Use the first TermStore object in the list.
        if (taxonomySession.TermStores.Count == 0)
            throw new InvalidOperationException("The Taxonomy Service is offline or missing.");

        TermStore termStore = taxonomySession.TermStores[0];
        clientContext.Load(termStore,
            ts => ts.Name,
            ts => ts.WorkingLanguage);

        // Does the TermSet object already exist?
        TermSet existingTermSet;

        clientContext.ExecuteQuery();

        if (!existingTermSet.ServerObjectIsNull.Value)
        {
            existingTermSet.DeleteObject();

            termStore.CommitAll();
            clientContext.ExecuteQuery();
        }

        // Create a new TermSet object.
        TermGroup siteCollectionGroup = termStore.GetSiteCollectionGroup(clientContext.Site,
            createIfMissing: true);
        TermSet termSet = siteCollectionGroup.CreateTermSet("Navigation Demo", TestConfig.NavTermSetId,
            termStore.WorkingLanguage);

        termStore.CommitAll();
        clientContext.ExecuteQuery();

        NavigationTermSet navTermSet = NavigationTermSet.GetAsResolvedByWeb(clientContext,
            termSet, clientContext.Web, "GlobalNavigationTaxonomyProvider");

        navTermSet.IsNavigationTermSet = true;
        navTermSet.TargetUrlForChildTerms.Value = "~site/Pages/Topics/Topic.aspx";

        termStore.CommitAll();
        clientContext.ExecuteQuery();

        NavigationTerm term1 = navTermSet.CreateTerm("Term 1", NavigationLinkType.SimpleLink, Guid.NewGuid());
        term1.SimpleLinkUrl = "http://www.bing.com/";

        NavigationTerm term2 = navTermSet.CreateTerm("Term 2", NavigationLinkType.FriendlyUrl, Guid.NewGuid());

        NavigationTerm term2a = term2.CreateTerm("Term 2 A", NavigationLinkType.FriendlyUrl, Guid.NewGuid());
        NavigationTerm term2b = term2.CreateTerm("Term 2 B", NavigationLinkType.FriendlyUrl, Guid.NewGuid());

        NavigationTerm term3 = navTermSet.CreateTerm("Term 3", NavigationLinkType.FriendlyUrl, Guid.NewGuid());

        termStore.CommitAll();
        clientContext.ExecuteQuery();

        return navTermSet;
    }
}
public class WebNavigationSettingsTests
{
    public TestContext TestContext { get; set; } // Automatically assigned by Visual Studio

    void Console.WriteLine(string message)
    {
        Trace.WriteLine(message);
        this.TestContext.WriteLine(message);
    }

    public void ConfigureTaxonomyNavigation()
    {
        ClientContext clientContext = new ClientContext(TestConfig.ServerUrl);
        TaxonomySession taxonomySession = TaxonomySession.GetTaxonomySession(clientContext);
        taxonomySession.UpdateCache();

        NavigationTermSet termSet = DemoUtilities.RecreateSampleNavTermSet(
            this.TestContext, clientContext, taxonomySession, clientContext.Web);

        // Clear out any old settings
        WebNavigationSettings webNavigationSettings = new WebNavigationSettings(clientContext, clientContext.Web);
        webNavigationSettings.ResetToDefaults();
        webNavigationSettings.Update(taxonomySession);

        TaxonomyNavigation.FlushSiteFromCache(clientContext, clientContext.Site);
        clientContext.ExecuteQuery();

        this.WaitForSync();

        // Verify the TermSet is not running
        NavigationTermSet actualTermSet;
        ExceptionHandlingScope scope = new ExceptionHandlingScope(clientContext);
        using (scope.StartScope())
        {
            using (scope.StartTry())
            {
                actualTermSet = TaxonomyNavigation.GetTermSetForWeb(clientContext, clientContext.Web,
                    "GlobalNavigationTaxonomyProvider", includeInheritedSettings: true);
            }
            using (scope.StartCatch())
            {
            }
        }
        clientContext.ExecuteQuery();

        Assert.IsTrue(actualTermSet.ServerObjectIsNull.Value);

        // Assign the new settings
        webNavigationSettings = new WebNavigationSettings(clientContext, clientContext.Web);

        clientContext.Load(webNavigationSettings,
            w => w.GlobalNavigation,
            w => w.CurrentNavigation
        );
        clientContext.Load(termSet,
            ts => ts.TermStoreId,
            ts => ts.Id
        );
        clientContext.ExecuteQuery();

        // GlobalNavigation = top menu (aka "top nav")
        // CurrentNavigation = left menu (aka "quick launch")
        webNavigationSettings.GlobalNavigation.Source = StandardNavigationSource.TaxonomyProvider;
        webNavigationSettings.GlobalNavigation.TermStoreId = termSet.TermStoreId;
        webNavigationSettings.GlobalNavigation.TermSetId = termSet.Id;
        webNavigationSettings.Update(taxonomySession);

        TaxonomyNavigation.FlushSiteFromCache(clientContext, clientContext.Site);
        clientContext.ExecuteQuery();

        this.WaitForSync();

        actualTermSet = TaxonomyNavigation.GetTermSetForWeb(clientContext, clientContext.Web,
            "GlobalNavigationTaxonomyProvider", includeInheritedSettings: true);
        clientContext.Load(actualTermSet, ts => ts.Id);
        clientContext.ExecuteQuery();

        Assert.AreEqual(termSet.Id, actualTermSet.Id);
    }
    void WaitForSync()
    {
        Console.WriteLine("Waiting...");
        System.Threading.Thread.Sleep(5000);
        Console.WriteLine("Finished waiting");
    }
}
