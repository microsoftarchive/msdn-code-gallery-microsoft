Imports System.Web
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Publishing
Imports Microsoft.SharePoint.Publishing.Navigation

''' <summary>
''' This class is an example of a custom site map provider that inherits from the 
''' fast PortalSiteMapProvider class. It adds link to MSDN and TechNet.
''' </summary>
''' <remarks>
''' This control overrides the standard top navigation provider on the v4.master 
''' master page by using the TopNavigationDataSource delegate control. The control
''' tag in Elements.xml is responsible for this. This class is deployed to the GAC.
''' You must add the following entry to the siteMap/providers element in web.config
''' before you can successfully deploy this project
''' <add name="ExampleCustomSiteMapProvider" description="This provider is an example of a custom navigation provider" type="WCM_CustomNavProvider.ExampleNavProvider, WCM_CustomNavProvider, Version=1.0.0.0, Culture=neutral, PublicKeyToken=7fcc63b456c6f649" NavigationType="Current" />
''' </remarks>
Public Class ExampleNavProvider
    Inherits PortalSiteMapProvider

    'Get the existing collection of site nodes
    Public Overrides Function GetChildNodes(ByVal node As SiteMapNode) As SiteMapNodeCollection
        'Store the node passed to the method and the current node and PortalSiteMapNodes
        Dim pNode As PortalSiteMapNode = TryCast(node, PortalSiteMapNode)
        Dim currentNode As PortalSiteMapNode = TryCast(Me.CurrentNode, PortalSiteMapNode)
        If (pNode IsNot Nothing) AndAlso (currentNode IsNot Nothing) Then
            'Check that we the node passed in is a SharePoint Site (SPWeb)
            If pNode.Type = NodeTypes.Area Then
                'Get the site's node collection
                Dim nodeCollection As SiteMapNodeCollection = MyBase.GetChildNodes(pNode)
                'Create two new nodes
                Dim newChildNode1 As PortalSiteMapNode = New PortalSiteMapNode(currentNode.WebNode, "customnodemsdn", NodeTypes.Custom, "http://msdn.microsoft.com", "MSDN", "Microsoft Solution Developer Network")
                Dim newChildNode2 As PortalSiteMapNode = New PortalSiteMapNode(currentNode.WebNode, "customnodetechnet", NodeTypes.Custom, "http://technet.microsoft.com", "TechNet", "Microsoft IT Pro documentation")
                'Add them
                nodeCollection.Add(newChildNode1)
                nodeCollection.Add(newChildNode2)
                'Return the collection
                Return nodeCollection
            Else
                'The node passed into this method is not a SharePoint site so we take no action
                Return MyBase.GetChildNodes(pNode)
            End If
        Else
            'Couldn't find either the node passed in or the current node. Return an empty collection
            Return New SiteMapNodeCollection()
        End If
    End Function

End Class
