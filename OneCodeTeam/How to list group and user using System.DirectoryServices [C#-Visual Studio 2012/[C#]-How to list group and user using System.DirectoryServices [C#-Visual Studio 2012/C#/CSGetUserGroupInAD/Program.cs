//****************************** Module Header ******************************'
//Module Name: Program.cs
//Project: CSGetUserGroupInAD
//Copyright (c) Microsoft Corporation.
//
//This sample application demonstrates how to perform a search on the user’s
//group membership in Active Directory. This demonstrates the recursive
//looping method. Also it shows how to get the Object SID for the group.
//
//This source is subject to the Microsoft Public License.
//See http://www.microsoft.com/en-us/openness/resources/licenses.aspx#MPL
//All other rights reserved.
//
//THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
//EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED
//WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//***************************************************************************'

using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSGetUserGroupInAD
{
    class Program
    {
        static void Main(string[] args)
        {
            string userName = "John";

            Console.WriteLine("Getting the user's group membership info...");

            //Calling the GetGroups function, by passing the root distingusihedName of the domain
            List<string> groups = GetUserGroups("DC=contoso,DC=com", userName);

            Console.WriteLine("User \"" + userName + "\" is the member of the following Groups: ");
            foreach (string group in groups)
            {
                Console.WriteLine(group + Environment.NewLine);
            }
        }

        /// <summary>
        /// This function will search the user.
        /// Once user is found, it will get it's memberOF attribute's value.
        /// </summary>
        /// <param name="domainDN">distinguishedName of the domain</param>
        /// <param name="sAMAccountName">
        /// sAMAccountName of the user for which we are searching the group membership in AD
        /// </param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static List<string> GetUserGroups(string domainDN, string sAMAccountName)
        {
            List<string> lGroups = new List<string>();
            try
            {
                //Create the DirectoryEntry object to bind the distingusihedName of your domain
                using (DirectoryEntry rootDE = new DirectoryEntry("LDAP://" + domainDN))
                {
                    //Create a DirectorySearcher for performing a search on abiove created DirectoryEntry
                    using (DirectorySearcher dSearcher = new DirectorySearcher(rootDE))
                    {
                        //Create the sAMAccountName as filter
                        dSearcher.Filter = "(&(sAMAccountName=" + sAMAccountName + ")(objectClass=User)(objectCategory=Person))";
                        dSearcher.PropertiesToLoad.Add("memberOf");
                        dSearcher.ClientTimeout.Add(new TimeSpan(0, 20, 0));
                        dSearcher.ServerTimeLimit.Add(new TimeSpan(0, 20, 0));

                        //Search the user in AD
                        SearchResult sResult = dSearcher.FindOne();
                        if (sResult == null)
                        {
                            throw new ApplicationException("No user with username " + sAMAccountName + " could be found in the domain");
                        }
                        else
                        {                            
                            //Once we get the userm let us get all the memberOF attibute's value
                            foreach (var grp in sResult.Properties["memberOf"])
                            {
                                string sGrpName = Convert.ToString(grp).Remove(0, 3);
                                //Bind to this group
                                DirectoryEntry deTempForSID = new DirectoryEntry("LDAP://" + grp.ToString().Replace("/", "\\/"));
                                try
                                {
                                    deTempForSID.RefreshCache();

                                    //Get the objectSID which is Byte array
                                    byte[] objectSid = (byte[])deTempForSID.Properties["objectSid"].Value;

                                    //Pass this Byte array to Security.Principal.SecurityIdentifier to convert this
                                    //byte array to SDDL format
                                    System.Security.Principal.SecurityIdentifier SID = new System.Security.Principal.SecurityIdentifier(objectSid, 0);

                                    if (sGrpName.Contains(",CN"))
                                    {
                                        sGrpName = sGrpName.Remove(sGrpName.IndexOf(",CN"));
                                    }
                                    else if (sGrpName.Contains(",OU"))
                                    {
                                        sGrpName = sGrpName.Remove(sGrpName.IndexOf(",OU"));
                                    }

                                    //Perform a recursive search on these groups.
                                    RecursivelyGetGroups(dSearcher, lGroups, sGrpName, SID.ToString());
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Error while binding to path : " + grp.ToString());
                                    Console.WriteLine(ex.Message.ToString());
                                }
                            }                            
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Please check the distinguishedName of the domain if it is as per your domain or not?");
                Console.WriteLine(ex.Message.ToString());
                System.Environment.Exit(0);
            }
            return lGroups;
        }

        /// <summary>
        /// This function will perform a recursive search and will add only one occurance of
        /// the group found in the enumeration.
        /// </summary>
        /// <param name="dSearcher">DirectorySearcher object to perform search</param>
        /// <param name="lGroups">List of the Groups from AD</param>
        /// <param name="sGrpName">
        /// Group name which needs to be checked inside the Groups collection
        /// </param>
        /// <param name="SID">objectSID of the object</param>
        /// <remarks></remarks>
        public static void RecursivelyGetGroups(DirectorySearcher dSearcher, List<string> lGroups, string sGrpName, string SID)
        {
            //Check if the group has already not found
            if (!lGroups.Contains(sGrpName))
            {
                lGroups.Add(sGrpName + " : " + SID);

                //Now perform the search based on this group
                dSearcher.Filter = "(&(objectClass=grp)(CN=" + sGrpName + "))".Replace("\\", "\\\\");
                dSearcher.ClientTimeout.Add(new TimeSpan(0, 2, 0));
                dSearcher.ServerTimeLimit.Add(new TimeSpan(0, 2, 0));

                //Search this group
                SearchResult GroupSearchResult = dSearcher.FindOne();
                if ((GroupSearchResult != null))
                {
                    foreach (var grp in GroupSearchResult.Properties["memberOf"])
                    {

                        string ParentGroupName = Convert.ToString(grp).Remove(0, 3);

                        //Bind to this group
                        DirectoryEntry deTempForSID = new DirectoryEntry("LDAP://" + grp.ToString().Replace("/", "\\/"));
                        try
                        {
                            //Get the objectSID which is Byte array
                            byte[] objectSid = (byte[])deTempForSID.Properties["objectSid"].Value;

                            //Pass this Byte array to Security.Principal.SecurityIdentifier to convert this
                            //byte array to SDDL format
                            System.Security.Principal.SecurityIdentifier ParentSID = new System.Security.Principal.SecurityIdentifier(objectSid, 0);

                            if (ParentGroupName.Contains(",CN"))
                            {
                                ParentGroupName = ParentGroupName.Remove(ParentGroupName.IndexOf(",CN"));
                            }
                            else if (ParentGroupName.Contains(",OU"))
                            {
                                ParentGroupName = ParentGroupName.Remove(ParentGroupName.IndexOf(",OU"));
                            }
                            RecursivelyGetGroups(dSearcher, lGroups, ParentGroupName, ParentSID.ToString());
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error while binding to path : " + grp.ToString());
                            Console.WriteLine(ex.Message.ToString());
                        }
                    }
                }
            }
        }
    }
}
