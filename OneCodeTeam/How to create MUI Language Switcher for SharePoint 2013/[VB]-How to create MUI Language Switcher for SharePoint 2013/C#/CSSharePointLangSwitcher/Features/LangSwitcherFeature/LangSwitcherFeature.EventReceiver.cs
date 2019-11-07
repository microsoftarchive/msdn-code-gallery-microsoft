/****************************** Module Header ******************************\
* Module Name:    LangSwitcherFeatureEventReceiver.cs
* Project:        CSSharePointLangSwitcher
* Copyright (c) Microsoft Corporation
*
* Add or remove the SwitcherModule in Web.config.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/

using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace CSSharePointLangSwitcher.Features.Feature2
{
    /// <summary>
    /// This class handles events raised during feature activation, deactivation, installation, uninstallation, and upgrade.
    /// </summary>
    /// <remarks>
    /// The GUID attached to this class may be used during packaging and should not be modified.
    /// </remarks>

    [Guid("7493972d-d22a-4e56-8f54-be77cb3a63fb")]
    public class LangSwitcherFeatureEventReceiver : SPFeatureReceiver
    {
        private const string WebConfigModificationOwner = "MyTestOwner";

        private static readonly SPWebConfigModification[] Modifications = {
            // For not so obvious reasons web.config modifications inside collections 
            // are added based on the value of the key attribute in alphabetic order.
            // Because we need to add the DualLayout module after the 
            // PublishingHttpModule, we prefix the name with 'Q-'.
            new SPWebConfigModification()
                { 
                    // The owner of the web.config modification, useful for removing a 
                    // group of modifications
                    Owner = WebConfigModificationOwner, 
                    // Make sure that the name is a unique XPath selector for the element 
                    // we are adding. This name is used for removing the element
                    Name = "add[@name='HTTPSwitcherModule']",
                    // We are going to add a new XML node to web.config
                    Type = SPWebConfigModification.SPWebConfigModificationType.EnsureChildNode, 
                    // The XPath to the location of the parent node in web.config
                     Path = "configuration/system.webServer/modules",   
                    // Sequence is important if there are multiple equal nodes that 
                    // can't be identified with an XPath expression
                    Sequence = 0,
                    // The XML to insert as child node, make sure that used names match the Name selector
                     Value = "<add name='HTTPSwitcherModule' type='CSSharePointLangSwitcher.LangSwitcherPage.HTTPSwitcherModule, CSSharePointLangSwitcher, Version=1.0.0.0, Culture=neutral, PublicKeyToken=f63674e4aada6b73' />" 
                }                        
        };

        public override void FeatureInstalled(SPFeatureReceiverProperties properties)
        {
        }

        public override void FeatureUninstalling(SPFeatureReceiverProperties properties)
        {
        }

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPSite sps = (SPSite)properties.Feature.Parent;
            SPWebApplication webApp = sps.WebApplication;
          
            if (webApp != null)
            {
                AddWebConfigModifications(webApp, Modifications);              
            }         
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            SPWebApplication webApp = properties.Feature.Parent as SPWebApplication;
            if (webApp != null)
            {
                RemoveWebConfigModificationsByOwner(webApp, WebConfigModificationOwner);
            }
        }

        /// <summary>
        /// Add a collection of web modifications to the web application.
        /// </summary>
        /// <param name="webApp">The web application to add the modifications to</param>
        /// <param name="modifications">The collection of modifications</param>
        private void AddWebConfigModifications(SPWebApplication webApp, IEnumerable<SPWebConfigModification> modifications)
        {
            foreach (SPWebConfigModification modification in modifications)
            {
                webApp.WebConfigModifications.Add(modification);
            }

            // Commit modification additions to the specified web application.
            webApp.Update();
            // Push modifications through the farm.
            webApp.WebService.ApplyWebConfigModifications();
        }

        /// <summary>
        /// Remove modifications from the web application.
        /// </summary>
        /// <param name="webApp">The web application to remove the modifications from.</param>
        /// <param name="owner"Remove all modifications that belong to the owner></param>
        private void RemoveWebConfigModificationsByOwner(SPWebApplication webApp, string owner)
        {
            Collection<SPWebConfigModification> modificationCollection = webApp.WebConfigModifications;
            Collection<SPWebConfigModification> removeCollection = new Collection<SPWebConfigModification>();

            int count = modificationCollection.Count;
            for (int i = 0; i < count; i++)
            {
                SPWebConfigModification modification = modificationCollection[i];
                if (modification.Owner == owner)
                {
                    // Collect modifications to delete.
                    removeCollection.Add(modification);
                }
            }

            // Delete the modifications from the web application.
            if (removeCollection.Count > 0)
            {
                foreach (SPWebConfigModification modificationItem in removeCollection)
                {
                    webApp.WebConfigModifications.Remove(modificationItem);
                }

                // Commit modification removals to the specified web application.
                webApp.Update();
                // Push modifications through the farm.
                webApp.WebService.ApplyWebConfigModifications();
            }
        }

    }
}
