using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint;
using System.Diagnostics;
using Microsoft.SharePoint.Taxonomy;
using Microsoft.SharePoint.Publishing.Navigation;
using System.Net;

namespace NavigationDemos
{
   public static class TestConfig
    {
        public const string ServerUrl = "http://contoso/sites/pub1";

        public static readonly Guid NavTermSetId = new Guid("E742CC24-69E6-428E-81CA-156119FF0979");

        public static readonly Guid TaggingTermSetId = new Guid("81C5E778-07CA-48e3-AC04-FA8AFF6F651B");

    } 
    
    public static class DemoUtilities
    {
        public static NavigationTermSet RecreateSampleNavTermSet(TestContext testContext,
            TaxonomySession taxonomySession, SPWeb web)
        {
            Console.WriteLine(testContext, "RecreateSampleNavTermSet(): START");

            // Use the first TermStore object in the list.
            if (taxonomySession.TermStores.Count == 0)
                throw new InvalidOperationException("The Taxonomy Service is offline or missing.");

            TermStore termStore = taxonomySession.TermStores[0];

            // Does the TermSet object already exist?
            TermSet existingTermSet = termStore.GetTermSet(TestConfig.NavTermSetId);

            if (existingTermSet != null)
            {
                Console.WriteLine(testContext, "RecreateSampleNavTermSet(): Deleting old TermSet");
                existingTermSet.Delete();
                termStore.CommitAll();
            }

            Console.WriteLine(testContext, "RecreateSampleNavTermSet(): Creating new TermSet");

            // Create a new TermSet object.
            Group siteCollectionGroup = termStore.GetSiteCollectionGroup(web.Site);
            TermSet termSet = siteCollectionGroup.CreateTermSet("Navigation Demo", TestConfig.NavTermSetId);

            NavigationTermSet navTermSet = NavigationTermSet.GetAsResolvedByWeb(termSet, web,
                StandardNavigationProviderNames.GlobalNavigationTaxonomyProvider);

            navTermSet.IsNavigationTermSet = true;
            navTermSet.TargetUrlForChildTerms.Value = "~site/Pages/Topics/Topic.aspx";

            NavigationTerm term1 = navTermSet.CreateTerm("Term 1", NavigationLinkType.SimpleLink);
            term1.SimpleLinkUrl = "http://www.bing.com/";

            NavigationTerm term2 = navTermSet.CreateTerm("Term 2", NavigationLinkType.FriendlyUrl);
            NavigationTerm term2a = term2.CreateTerm("Term 2 A", NavigationLinkType.FriendlyUrl);
            NavigationTerm term2b = term2.CreateTerm("Term 2 B", NavigationLinkType.FriendlyUrl);

            NavigationTerm term3 = navTermSet.CreateTerm("Term 3", NavigationLinkType.FriendlyUrl);

            termStore.CommitAll();

            Console.WriteLine(testContext, "RecreateSampleNavTermSet(): FINISH");

            return navTermSet;
        }
        /// Configures the web to use Taxonomy Navigation with the sample term set.
        public static NavigationTermSet SetUpSampleNavTermSet(TestContext testContext,
            TaxonomySession taxonomySession, SPWeb web)
        {
            NavigationTermSet termSet = RecreateSampleNavTermSet(testContext, taxonomySession, web);

            // Clear any old settings.
            WebNavigationSettings webNavigationSettings = new WebNavigationSettings(web);
            webNavigationSettings.ResetToDefaults();

            webNavigationSettings.GlobalNavigation.Source = StandardNavigationSource.TaxonomyProvider;
            webNavigationSettings.GlobalNavigation.TermStoreId = termSet.TermStoreId;
            webNavigationSettings.GlobalNavigation.TermSetId = termSet.Id;

            webNavigationSettings.CurrentNavigation.Source = StandardNavigationSource.TaxonomyProvider;
            webNavigationSettings.CurrentNavigation.TermStoreId = termSet.TermStoreId;
            webNavigationSettings.CurrentNavigation.TermSetId = termSet.Id;

            webNavigationSettings.Update(taxonomySession);

            TaxonomyNavigation.FlushSiteFromCache(web.Site);

            return termSet;
        }
       

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
        using (SPSite site = new SPSite(TestConfig.ServerUrl))
        {
            using (SPWeb web = site.OpenWeb())
            {
                TaxonomySession taxonomySession = new TaxonomySession(site, updateCache: true);

                NavigationTermSet termSet = DemoUtilities.RecreateSampleNavTermSet(
                    this.TestContext, taxonomySession, web);

                // Clear any old settings.
                WebNavigationSettings webNavigationSettings = new WebNavigationSettings(web);
                webNavigationSettings.ResetToDefaults();
                webNavigationSettings.Update(taxonomySession);

                TaxonomyNavigation.FlushSiteFromCache(site);

                this.WaitForSync();

                // Verify the TermSet object is not running.
                NavigationTermSet actualTermSet;

                actualTermSet = TaxonomyNavigation.GetTermSetForWeb(web,
                    StandardNavigationProviderNames.GlobalNavigationTaxonomyProvider,
                    includeInheritedSettings: true);

                Assert.IsTrue(actualTermSet == null);

                // Assign the new settings.
                webNavigationSettings = new WebNavigationSettings(web);

                // GlobalNavigation = top menu (aka "top nav")
                // CurrentNavigation = left menu (aka "quick launch")
                webNavigationSettings.GlobalNavigation.Source = StandardNavigationSource.TaxonomyProvider;
                webNavigationSettings.GlobalNavigation.TermStoreId = termSet.TermStoreId;
                webNavigationSettings.GlobalNavigation.TermSetId = termSet.Id;
                webNavigationSettings.Update(taxonomySession);

                TaxonomyNavigation.FlushSiteFromCache(site);

                this.WaitForSync();

                actualTermSet = TaxonomyNavigation.GetTermSetForWeb(web,
                    StandardNavigationProviderNames.GlobalNavigationTaxonomyProvider,
                    includeInheritedSettings: true);

                Assert.AreEqual(termSet.Id, actualTermSet.Id);
            }
        }
    }
    void WaitForSync()
    {

        Console.WriteLine("Loading web page from server");

        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(TestConfig.ServerUrl + "/Pages/default.aspx");
            request.UseDefaultCredentials = true;
            request.GetResponse();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
        }
    }
}

