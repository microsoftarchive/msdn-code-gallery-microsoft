Imports System
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Taxonomy
Imports Microsoft.SharePoint.WebControls

Namespace Layouts.ECM_CreateTaxonomy

    ''' <summary>
    ''' This application page creates and populates a group of terms in the
    ''' Managed Metadata Service service application.
    ''' </summary>
    ''' <remarks>
    ''' You can see the results of this code by managing the Managed Metadata service
    ''' application in Central Administration. Also, after this code runs, users can 
    ''' add these terms as tags to pages, items, and documents in SharePoint sites.
    ''' </remarks>
    Partial Public Class CreateTaxonomy
        Inherits LayoutsPageBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        End Sub

        Protected Sub createTaxonomyButton_Click(ByVal sender As Object, ByVal e As EventArgs)

            Dim currentSite As SPSite = SPContext.Current.Site
            'To manage terms, we must create a new Taxonomy Session object
            Dim session As TaxonomySession = New TaxonomySession(currentSite)
            'Get the termstore
            Dim termstore As TermStore = session.TermStores("Managed Metadata Service")

            'Create a term group
            Dim plantsGroup As Group = termstore.CreateGroup("Plants")

            'Create a term set
            Dim flowersTermSet As TermSet = plantsGroup.CreateTermSet("Flowers")

            'Create some terms
            Dim tulipsTerm As Term = flowersTermSet.CreateTerm("Tulips", 1033)
            Dim orchidsTerm As Term = flowersTermSet.CreateTerm("Orchids", 1033)
            Dim daffodilsTerm As Term = flowersTermSet.CreateTerm("Daffodils", 1033)

            'Create a child term within the Orchids term
            Dim vanillaTerm As Term = orchidsTerm.CreateTerm("Vanilla", 1033)

            'You should set properties for every term. In this example, we'll
            'do just one for brevity
            vanillaTerm.SetDescription("A common orchid whose pods are used in desserts", 1033)
            'Use CreateLabel to add synomyns and alternates
            vanillaTerm.CreateLabel("Vanilla planifolia", 1033, False)
            vanillaTerm.CreateLabel("Flat-leaved vanilla", 1033, False)

            'When we are finished making changes, we must commit them
            Try
                termstore.CommitAll()
                resultsLabel.Text = "Taxonomy created successfully"
            Catch ex As Exception
                resultsLabel.Text = "There was an error: " + ex.Message
            End Try
           
        End Sub

    End Class

End Namespace
