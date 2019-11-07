Imports System
Imports Microsoft.Office.DocumentManagement.DocumentSets
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls

Namespace Layouts.ECM_CreateDocumentSet

    ''' <summary>
    ''' This application page creates a document set based on the default document set
    ''' content type. 
    ''' </summary>
    ''' <remarks>
    ''' You must have enabled the site collection level Document Sets feature, and added
    ''' the Document Set content type to the document library, before you can create 
    ''' document sets. 
    ''' </remarks>
    Partial Public Class CreateDocumentSet
        Inherits LayoutsPageBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        End Sub

        Protected Sub createDocSetButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            'Get the Shared Documents document library
            Dim currentWeb As SPWeb = SPContext.Current.Web
            Dim sharedDocsLib As SPDocumentLibrary = TryCast(currentWeb.Lists("Shared Documents"), SPDocumentLibrary)
            'You can use a hashtable to populate properties of the document set
            Dim docsetProperties As Hashtable = New Hashtable()
            docsetProperties.Add("Name", nameTextbox.Text)
            docsetProperties.Add("Description", descriptionTextbox.Text)
            'Create the document set
            Try
                Dim newDocSet As DocumentSet = DocumentSet.Create(sharedDocsLib.RootFolder, _
                    nameTextbox.Text, sharedDocsLib.ContentTypes("Document Set").Id, _
                    docsetProperties, True)
                resultLabel.Text = "Document set created"
            Catch ex As Exception
                resultLabel.Text = "An error occurred: " + ex.Message
            End Try
            
        End Sub

    End Class

End Namespace
