Imports System
Imports System.ComponentModel
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Publishing.Navigation
Imports Microsoft.SharePoint.WebControls

<ToolboxItemAttribute(false)> _
Public Class CachedCrossSiteQueries
    Inherits WebPart

    'Controls
    Private runQueryButton As Button
    Private resultsLabel As Label

    Protected Overrides Sub CreateChildControls()

        'Set up the Run Query button
        runQueryButton = New Button()
        runQueryButton.Text = "Run Query"
        AddHandler runQueryButton.Click, AddressOf runQueryButton_Click
        Me.Controls.Add(runQueryButton)
        'Set up the results label
        resultsLabel = New Label()
        resultsLabel.Text = "Query has not yet been run"
        Me.Controls.Add(resultsLabel)

    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

        'Render the controls
        runQueryButton.RenderControl(writer)
        writer.Write("<br />")
        resultsLabel.RenderControl(writer)

    End Sub

    Private Sub runQueryButton_Click(ByVal sender As Object, ByVal e As EventArgs)

        'The PortalSiteMapProvider class provides high-performance queries for 
        'data that changes infrequently because it makes use of SharePoint's 
        'query caches. If you frequently query for the same dataset that changes rarely,
        'you should consider using this object to run your queries.

        'A PortalSiteMapProvider object is large and takes lots of resources to set
        'up. You should not create a new one for your queries but instead use one of
        'the examples SharePoint provides by default. If you need to create your own
        'PortalSiteMapProvider you should configure the web application with web.config.

        'We'll get the example that SharePoint uses to build the breadcrumb trail
        Dim portalProvider As PortalSiteMapProvider = PortalSiteMapProvider.CurrentNavSiteMapProviderNoEncode

        'Write a introductory results line
        resultsLabel.Text = "Sites: <br />"

        'Run a query. This example gets all the child sites.
        Dim children As SiteMapNodeCollection = portalProvider.GetChildNodes(DirectCast(portalProvider.CurrentNode, PortalSiteMapNode).WebNode, _
              Microsoft.SharePoint.Publishing.NodeTypes.Area, Microsoft.SharePoint.Publishing.NodeTypes.Area)

        'Loop through the results and display the site title.
        For Each node As SiteMapNode In children
            resultsLabel.Text += node.Title + "<br />"
        Next

    End Sub

End Class
