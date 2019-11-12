// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

using Microsoft.VisualStudio.SharePoint;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Contoso.SharePointProjectItems.CustomAction
{
    internal partial class CustomActionProvider
    {
        private void ProjectItemPropertiesRequested(object sender,
            SharePointProjectItemPropertiesRequestedEventArgs e)
        {
            CustomActionProperties properties;

            // If the properties object already exists, get it from the project item's annotations.
            if (!e.ProjectItem.Annotations.TryGetValue(out properties))
            {
                // Otherwise, create a new properties object and add it to the annotations.
                properties = new CustomActionProperties(e.ProjectItem);
                e.ProjectItem.Annotations.Add(properties);
            }

            e.PropertySources.Add(properties);
        }
    }

    internal class CustomActionProperties
    {
        private ISharePointProjectItem projectItem;
        private const string testPropertyId = "Contoso.CustomActionTestProperty";
        private const string propertyDefaultValue = "This is a test value.";

        internal CustomActionProperties(ISharePointProjectItem projectItem)
        {
            this.projectItem = projectItem;
        }

        // Gets or sets a simple string property. The property value is stored in the ExtensionData property
        // of the project item. Data in the ExtensionData property persists when the project is closed.
        [DisplayName("Custom Action Property")]
        [DescriptionAttribute("This is a test property for the Contoso Custom Action project item.")]
        [DefaultValue(propertyDefaultValue)]
        public string TestProperty
        {
            get
            {
                string propertyValue;

                // Get the current property value if it already exists; otherwise, return a default value.
                if (!projectItem.ExtensionData.TryGetValue(testPropertyId, out propertyValue))
                {
                    propertyValue = propertyDefaultValue;
                }
                return propertyValue;
            }
            set
            {
                if (value != propertyDefaultValue)
                {
                    projectItem.ExtensionData[testPropertyId] = value;
                }
                else
                {
                    // Do not save the default value.
                    projectItem.ExtensionData.Remove(testPropertyId);
                }
            }
        }
    }
}
