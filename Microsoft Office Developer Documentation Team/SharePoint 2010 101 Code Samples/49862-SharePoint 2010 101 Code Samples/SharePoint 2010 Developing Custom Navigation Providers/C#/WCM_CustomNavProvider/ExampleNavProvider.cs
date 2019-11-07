using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Publishing;
using Microsoft.SharePoint.Publishing.Navigation;

namespace WCM_CustomNavProvider 
{
    /// <summary>
    /// This class is an example of a custom site map provider that inherits from the 
    /// fast PortalSiteMapProvider class. It adds link to MSDN and TechNet.
    /// </summary>
    /// <remarks>
    /// This control overrides the standard top navigation provider on the v4.master 
    /// master page by using the TopNavigationDataSource delegate control. The control
    /// tag in Elements.xml is responsible for this. This class is deployed to the GAC.
    /// You must add the following entry to the siteMap/providers element in web.config
    /// before you can successfully deploy this project:
    /// <add name="ExampleCustomSiteMapProvider" description="This provider is an example of a custom navigation provider" type="WCM_CustomNavProvider.ExampleNavProvider, WCM_CustomNavProvider, Version=1.0.0.0, Culture=neutral, PublicKeyToken=d3b8807902624f9a" NavigationType="Current" />
    /// </remarks>
    public class ExampleNavProvider : PortalSiteMapProvider
    {
        //Get the existing collection of site nodes
        public override SiteMapNodeCollection GetChildNodes(SiteMapNode node)
        {
            //Store the node passed to the method and the current node and PortalSiteMapNodes
            PortalSiteMapNode pNode = node as PortalSiteMapNode;
            PortalSiteMapNode currentNode = CurrentNode as PortalSiteMapNode;
            if ((pNode != null) && (currentNode != null))
            {
                //Check that we the node passed in is a SharePoint Site (SPWeb)
                if (pNode.Type == NodeTypes.Area)
                {
                    //Get the site's node collection
                    SiteMapNodeCollection nodeCollection = base.GetChildNodes(pNode);
                    //Create two new nodes
                    PortalSiteMapNode newChildNode1 = new PortalSiteMapNode(currentNode.WebNode, "customnodemsdn", NodeTypes.Custom, "http://msdn.microsoft.com", "MSDN", "Microsoft Solution Developer Network");
                    PortalSiteMapNode newChildNode2 = new PortalSiteMapNode(currentNode.WebNode, "customnodetechnet", NodeTypes.Custom, "http://technet.microsoft.com", "TechNet", "Microsoft IT Pro documentation");
                    //Add them
                    nodeCollection.Add(newChildNode1);
                    nodeCollection.Add(newChildNode2);
                    //Return the collection
                    return nodeCollection;
                }
                else
                {
                    //The node passed into this method is not a SharePoint site so we take no action
                    return base.GetChildNodes(pNode);
                }
            }
            else
            {
                //Couldn't find either the node passed in or the current node. Return an empty collection
                return new SiteMapNodeCollection();
            }
        }
    }
}
