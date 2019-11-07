// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// MICROSOFT LIMITED PUBLIC LICENSE version 1.1 (MS-LPL, http://go.microsoft.com/?linkid=9791213.)

using System;
using System.Diagnostics;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.SharePoint;

namespace Contoso.SharePointProjectItems.CustomAction
{
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(ISharePointProjectItemTypeProvider))]

    // Specifies the ID for this new project item type. This string must match the value of the 
    // Type attribute of the ProjectItem element in the .spdata file for the project item.
    [SharePointProjectItemType("Contoso.CustomAction")]

    // Specifies the icon to display with this project item in Solution Explorer.
    [SharePointProjectItemIcon("ProjectItemDefinition.CustomAction_SolutionExplorer.ico")]

    // Defines a new type of project item that can be used to create a custom action on a SharePoint site.
    internal partial class CustomActionProvider : ISharePointProjectItemTypeProvider
    {
        private ISharePointProjectService projectService;

        // Implements IProjectItemTypeProvider.InitializeType. Configures the behavior of the project item type.
        public void InitializeType(ISharePointProjectItemTypeDefinition projectItemTypeDefinition)
        {
            projectItemTypeDefinition.Name = "CustomAction";
            projectItemTypeDefinition.SupportedDeploymentScopes =
                SupportedDeploymentScopes.Site | SupportedDeploymentScopes.Web;
            projectItemTypeDefinition.SupportedTrustLevels = SupportedTrustLevels.All;

            // Get the service so that other code in this class can use it.
            projectService = projectItemTypeDefinition.ProjectService;

            // Handle some project item events.
            projectItemTypeDefinition.ProjectItemInitialized += ProjectItemInitialized;
            projectItemTypeDefinition.ProjectItemNameChanged += ProjectItemNameChanged;
            projectItemTypeDefinition.ProjectItemDisposing += ProjectItemDisposing;

            // Handle events to create a custom property and shortcut menu item for this project item.
            projectItemTypeDefinition.ProjectItemPropertiesRequested +=
                ProjectItemPropertiesRequested;
            projectItemTypeDefinition.ProjectItemMenuItemsRequested +=
                ProjectItemMenuItemsRequested;
        }

        private void ProjectItemInitialized(object sender, SharePointProjectItemEventArgs e)
        {
            // Handle a project event.
            e.ProjectItem.Project.PropertyChanged += ProjectPropertyChanged;
        }

        private void ProjectItemNameChanged(object sender, NameChangedEventArgs e)
        {
            ISharePointProjectItem projectItem = (ISharePointProjectItem)sender;
            string message = String.Format("The name of the {0} item changed to: {1}",
                e.OldName, projectItem.Name);
            projectService.Logger.WriteLine(message, LogCategory.Message);
        }

        private void ProjectPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            ISharePointProject project = (ISharePointProject)sender;
            string message = String.Format("The following property of the {0} project was changed: {1}",
                    project.Name, e.PropertyName);
            projectService.Logger.WriteLine(message, LogCategory.Message);
        }

        private void ProjectItemDisposing(object sender, SharePointProjectItemEventArgs e)
        {
            e.ProjectItem.Project.PropertyChanged -= ProjectPropertyChanged;
        }
    }
}
