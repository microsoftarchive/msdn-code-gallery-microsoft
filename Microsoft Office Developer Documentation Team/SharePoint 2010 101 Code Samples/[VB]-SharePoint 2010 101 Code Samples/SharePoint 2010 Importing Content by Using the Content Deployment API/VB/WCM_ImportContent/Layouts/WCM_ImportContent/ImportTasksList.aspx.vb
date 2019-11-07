Imports System
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Deployment
Imports Microsoft.SharePoint.WebControls

Namespace Layouts.WCM_ImportContent

    ''' <summary>
    ''' This sample SharePoint application page demonstrates how to use the Content
    ''' Deployment API to import a single List and all its contents from a Content 
    ''' Migration Package (CMP) file.
    ''' </summary>
    ''' <remarks>
    ''' This example imports the content of a CMP file in the root of the C: drive
    ''' </remarks>
    Partial Public Class ImportTasksList
        Inherits LayoutsPageBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        End Sub

        Protected Sub importButton_Click(ByVal sender As Object, ByVal args As EventArgs)
            'Create an ImportSettings object to configure the import
            Dim importSettings As SPImportSettings = New SPImportSettings()
            'Set the name and location of the import file
            importSettings.BaseFileName = "demoTasksExport.cmp"
            importSettings.FileLocation = "C:\"
            'Set the URL of the site to import into
            importSettings.SiteUrl = "http://intranet.contoso.com"

            'Do the import
            Dim import As SPImport = New SPImport(importSettings)
            Try
                import.Run()
                resultsLabel.Text = "Import was successful"
            Catch ex As Exception
                resultsLabel.Text = "The Import caused an error: " + ex.Message
            End Try
        End Sub

    End Class

End Namespace
