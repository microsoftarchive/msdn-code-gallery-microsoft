using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;

namespace Microsoft.Exchange.Samples.Autodiscover
{
    // This sample is for demonstration purposes only. Before you run this sample, make sure 
    // that the code meets the coding requirements of your organization. 
    class AutodiscoverUrlList : List<string>
    {
        // GUID for SCP URL keyword.
        private const string ScpUrlGuidString = @"77378F46-2C66-4aa9-A6A6-3E7A48B19596";

        // GUID for SCP pointer keyword.
        private const string ScpPtrGuidString = @"67661d7F-8FC4-4fa7-BFAC-E1D7794C1F68";

        // GenerateList
        //   This function generates a list of potential Autodiscover URLs
        //   based on the user's email address. Note that these URLs are generic,
        //   in that they have no file extension ("http://contoso.com/autodiscover/autodiscover").
        //
        // Parameters:
        //   emailAddress: The user's email address.
        //
        // Returns:
        //   None.
        //
        public void GenerateList(string emailAddress)
        {
            // Extract the domain from the email address.
            string userDomain = GetDomainFromEmailAddress(emailAddress);

            // Do an SCP lookup and add the results.
            TryAddSCPResults(null, userDomain);
            // Add the default URLs
            AddDefaultURLs(userDomain);
        }

        // GetLastDitchUrl
        //   This function generates the non-https URL, or "last ditch" URL
        //   based on the user's email address. This URL takes the form:
        //   http://autodiscover.contoso.com/autodiscover/autodiscover.xml.
        //
        // Parameters:
        //   emailAddress: The user's email address.
        //
        // Returns:
        //   A string that contains the last ditch URL.
        //
        public string GetLastDitchUrl(string emailAddress)
        {
            string userDomain = GetDomainFromEmailAddress(emailAddress);

            return "http://autodiscover." + userDomain + "/autodiscover/autodiscover.xml";
        }

        // AddUrl
        //   This function normalizes the URL and adds it to the list if it is not already present.
        //
        // Parameters:
        //   url: The URL to add.
        //
        // Returns:
        //   None.
        //
        public void AddUrl(string url)
        {
            url = NormalizeAutodiscoverUrl(url);

            if (!this.Contains(url))
            {
                this.Add(url);
            }
        }

        // NormalizeAutodiscoverUrl
        //   This function normalizes the URL. Normalization consists of:
        //     1. Converting to all lowercase.
        //     2. Removing the file extension if present.
        //
        // Parameters:
        //   url: The URL to normalize.
        //
        // Returns:
        //   The normalized version of the URL.
        //
        public static string NormalizeAutodiscoverUrl(string url)
        {
            url = url.ToLower();

            if (IsPoxUrl(url) || IsSoapUrl(url))
            {
                url = url.Remove(url.Length - 4);
            }

            return url;
        }

        // GetDomainFromEmailAddress
        //   This function extracts the domain from an email address.
        //
        // Parameters:
        //   emailAddress: The email address to extract a domain from.
        //
        // Returns:
        //   The extracted domain.
        //
        private string GetDomainFromEmailAddress(string emailAddress)
        {
            char[] emailSplitChars = { '@' };

            // Split the string at the at symbol (@) part of the email address.
            string [] emailAddressParts = emailAddress.Split(emailSplitChars);

            // There should be exactly two parts. If not, this is a very
            // unusual email address.
            if (emailAddressParts.Count() != 2)
            {
                return null;
            }

            // Return the portion to the right of the at symbol (@).
            return emailAddressParts[1];
        }

        // TryAddSCPResults
        //   This function does an SCP lookup for a specific domain and adds the results
        //   to the list.
        //   NOTE: This will only work for domain-joined computers.
        //
        // Parameters:
        //   ldapPath: A string that specifies the LDAP server. Can be null.
        //   domain: A string that specifies the domain to match.
        //
        // Returns:
        //   None.
        //
        private void TryAddSCPResults(string ldapPath, string domain)
        {
            SearchResultCollection scpEntries = null;

            string rootDSEPath = ldapPath == null ? "LDAP://RootDSE": ldapPath + "/RootDSE";

            try
            {
                // Get the root directory entry.
                DirectoryEntry rootDSE = new DirectoryEntry(rootDSEPath);

                // Get the configuration path.
                string configPath = rootDSE.Properties["configurationNamingContext"].Value as string;

                // Get the configuration entry.
                DirectoryEntry configEntry = new DirectoryEntry("LDAP://" + configPath);

                // Create a search object for the configuration entry.
                DirectorySearcher configSearch = new DirectorySearcher(configEntry);

                // Set the search filter to find SCP URLs and SCP pointers.
                configSearch.Filter = "(&(objectClass=serviceConnectionPoint)" +
                    "(|(keywords=" + ScpPtrGuidString + ")(keywords=" + ScpUrlGuidString + ")))";

                // Specify which properties you want to retrieve.
                configSearch.PropertiesToLoad.Add("keywords");
                configSearch.PropertiesToLoad.Add("serviceBindingInformation");

                scpEntries = configSearch.FindAll();
            }
            catch (Exception e)
            {
                Tracing.WriteLine("SCP lookup failed with:");
                Tracing.WriteLine(e.ToString());
            }

            // If no SCP entries were found, then exit.
            if (scpEntries == null || scpEntries.Count <= 0)
            {
                Tracing.WriteLine("No SCP records found.");
                return;
            }

            string fallBackLdapPath = null;

            // Check for SCP pointers.
            foreach (SearchResult scpEntry in scpEntries)
            {
                ResultPropertyValueCollection entryKeywords = scpEntry.Properties["keywords"];

                if (CollectionContainsExactValue(entryKeywords, ScpPtrGuidString))
                {
                    string ptrLdapPath = scpEntry.Properties["serviceBindingInformation"][0] as string;

                    // Check to determine whether this pointer is scoped to the user's domain.
                    if (CollectionContainsExactValue(entryKeywords, "Domain=" + domain))
                    {
                        Tracing.WriteLine("Found SCP pointer for " + domain + " in " + scpEntry.Path);

                        // Only restart SCP lookup if this is the first time you've found an entry
                        // scoped to the user's domain. This is to avoid endless redirection.
                        if (ldapPath == null)
                        {
                            Tracing.WriteLine("Restarting SCP lookup in " + ptrLdapPath);
                            TryAddSCPResults(ptrLdapPath, domain);
                            return;
                        }
                        else
                        {
                            Tracing.WriteLine("Skipping SCP lookup in " + ptrLdapPath);
                        }
                    }
                    else
                    {
                        // Save the first SCP pointer that is not scoped to a domain as a fallback.
                        if (entryKeywords.Count == 1 && string.IsNullOrEmpty(fallBackLdapPath))
                        {
                            fallBackLdapPath = ptrLdapPath;
                            Tracing.WriteLine("Saved fallback SCP pointer: " + fallBackLdapPath);
                        }
                    }
                }
            }

            string computerSiteName = null;

            try
            {
                // Get the name of the ActiveDirectorySite the computer
                // belongs to (if it belongs to one).
                ActiveDirectorySite site = ActiveDirectorySite.GetComputerSite();
                computerSiteName = site.Name;

                Tracing.WriteLine("Local computer in site: " + computerSiteName);
            }
            catch (Exception e)
            {
                Tracing.WriteLine("Unable to get computer site name.");
                Tracing.WriteLine(e.ToString());
            }

            if (!string.IsNullOrEmpty(computerSiteName))
            {
                // Scan the search results for SCP URLs.
                // SCP URLs fit into three tiers:
                //   Priority 1: The URL is scoped to the computer's Active Directory site.
                //   Priority 2: The URL is not scoped to any Active Directory site.
                //   Priority 3: The URL is scoped to a different Active Directory site.

                // Temporary lists to hold priority 2 and 3 URLs.
                List<string> priorityTwoUrls = new List<string>();
                List<string> priorityThreeUrls = new List<string>();

                foreach (SearchResult scpEntry in scpEntries)
                {
                    ResultPropertyValueCollection entryKeywords = scpEntry.Properties["keywords"];

                    // Check for SCP URLs.
                    if (CollectionContainsExactValue(entryKeywords, ScpUrlGuidString))
                    {
                        string scpUrl = scpEntry.Properties["serviceBindingInformation"][0] as string;
                        scpUrl = scpUrl.ToLower();

                        // Determine whether this entry is scoped to the computer's site.
                        if (CollectionContainsExactValue(entryKeywords, "Site=" + computerSiteName))
                        {
                            // Priority 1.
                            Tracing.WriteLine("Found priority 1 SCP URL: " + scpUrl);
                            
                            AddUrl(scpUrl);
                        }
                        else
                        {
                            // Determine whether this is a priority 2 or 3 URL.
                            if (CollectionContainsPrefixValue(entryKeywords, "Site="))
                            {
                                // Priority 3.
                                if (!priorityThreeUrls.Contains(scpUrl))
                                {
                                    Tracing.WriteLine("Found priority 3 SCP URL: " + scpUrl);
                                    priorityThreeUrls.Add(scpUrl);
                                }
                            }
                            else
                            {
                                // Priority 2.
                                if (!priorityTwoUrls.Contains(scpUrl))
                                {
                                    Tracing.WriteLine("Found priority 2 SCP URL: " + scpUrl);
                                    priorityTwoUrls.Insert(0, scpUrl);
                                }
                            }
                        }
                    }
                }

                // Now add the priority 2 URLs into the main list.
                foreach (string priorityTwoUrl in priorityTwoUrls)
                {
                    AddUrl(priorityTwoUrl);
                }

                // Now add the priority 3 URLs into the main list.
                foreach (string priorityThreeUrl in priorityThreeUrls)
                {
                    AddUrl(priorityThreeUrl);
                }

                // If after all this, you still have no URLs in your list,
                // try the fallback SCP pointer, if you have one.
                if (this.Count == 0 && fallBackLdapPath != null)
                {
                    TryAddSCPResults(fallBackLdapPath, domain);
                }
            }
        }

        // AddDefaultURLs
        //   This function adds the default URLS (based on the domain)
        //   to the list.
        //
        // Parameters:
        //   domain: A string that specifies the domain.
        //
        // Returns:
        //   None.
        //
        private void AddDefaultURLs(string domain)
        {
            AddUrl("https://" + domain.ToLower() + "/autodiscover/autodiscover");
            AddUrl("https://autodiscover." + domain.ToLower() + "/autodiscover/autodiscover");
        }

        // IsSoapUrl
        //   This function determines whether a given URL is a SOAP URL.
        //
        // Parameters:
        //   url: The URL to check.
        //
        // Returns:
        //   True if the URL is a SOAP URL; otherwise, false.
        //
        public static bool IsSoapUrl(string url)
        {
            return url.EndsWith(".svc", true, null);
        }

        // IsPoxUrl
        //   This function determines whether a given URL is a POX URL.
        //
        // Parameters:
        //   url: The URL to check.
        //
        // Returns:
        //   True if the URL is a POX URL; otherwise, false.
        //
        public static bool IsPoxUrl(string url)
        {
            return url.EndsWith(".xml", true, null);
        }

        // CollectionContainsExactValue
        //   This function checks the contents of a ResultPropertyValueCollection
        //   for a specific value.
        //
        // Parameters:
        //   collection: The ResultPropertyValueCollection to check.
        //   value: The value to look for.
        //
        // Returns:
        //   True if the collection contains the value; otherwise, false.
        //
        private static bool CollectionContainsExactValue(ResultPropertyValueCollection collection, string value)
        {
            foreach (object obj in collection)
            {
                string entry = obj as string;
                if (entry != null)
                {
                    if (string.Compare(value, entry, true) == 0)
                        return true;
                }
            }

            return false;
        }

        // CollectionContainsPrefixValue
        //   This function checks the contents of a ResultPropertyValueCollection
        //   for a value that begins with a specific value.
        //
        // Parameters:
        //   collection: The ResultPropertyValueCollection to check.
        //   value: The value to look for.
        //
        // Return:
        //   True if the collection contains the value; otherwise, false.
        //
        private static bool CollectionContainsPrefixValue(ResultPropertyValueCollection collection, string value)
        {
            foreach (object obj in collection)
            {
                string entry = obj as string;
                if (entry != null)
                {
                    if (entry.StartsWith(value))
                        return true;
                }
            }

            return false;
        }
    }
}
