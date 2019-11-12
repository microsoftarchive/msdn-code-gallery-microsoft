Imports System
Imports Microsoft.Office.DocumentManagement.DocumentSets
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls

Namespace Layouts.ECM_ManageDocumentSet

    ''' <summary>
    ''' This application page illustrates how to get all the document sets in a
    ''' document library and read their properties. You need a combination of objects
    ''' to do this. For example, each DocumentSet object corresponds to an SPFolder
    ''' object. Use SPFolder.Name for the Name of the set and DocumentSet for the
    ''' Welcome Page URL.
    ''' </summary>
    ''' <remarks>
    ''' This example assumes that there is a Shared Documents document library in the
    ''' current site.
    ''' </remarks>
    Partial Public Class ManageDocumentSet
        Inherits LayoutsPageBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        End Sub

        Protected Sub getDocSetsButton_Click(ByVal sender As Object, ByVal e As EventArgs)
            Dim count As Integer = 0
            'Get the Shared Documents document libary
            Dim currentWeb As SPWeb = SPContext.Current.Web
            Dim sharedDocuments As SPDocumentLibrary = TryCast(currentWeb.Lists("Shared Documents"), SPDocumentLibrary)
            'Loop though all the folders. Some of them will be document sets
            'NOTE: don't use the sharedDocuments.Folders collection! Use sharedDocuments.RootFolder.SubFolders
            For Each currentFolder As SPFolder In sharedDocuments.RootFolder.SubFolders
                'Get the corresponding document set
                Dim currentDocSet As DocumentSet = DocumentSet.GetDocumentSet(currentFolder)
                'Strangely the previous line always returns a document set object
                'Even if the current folder is not a document set. So we use the
                'following test to find out if the current folder is a document set
                If currentDocSet.Item IsNot Nothing Then
                    'This folder is a document set. Increase the count
                    count += 1
                    'Read some properties of the document set
                    resultLabel.Text += "Name: " + currentFolder.Name + "<br />"
                    resultLabel.Text += "Content Type: " + currentDocSet.ContentType.Name + "<br />"
                    resultLabel.Text += "Document Count: " + currentDocSet.Folder.ItemCount.ToString() + "<br />"
                    resultLabel.Text += "Welcome Page: " + currentDocSet.WelcomePageUrl + "<br /><br />"
                End If
            Next
            'Display the count
            docSetCountLabel.Text = count.ToString()
        End Sub

    End Class

End Namespace
