Imports System
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Deployment
Imports Microsoft.SharePoint.WebControls

Namespace Layouts.WCM_ExportContent

    ''' <summary>
    ''' This sample SharePoint application page demonstrates how to use the Content
    ''' Deployment API to export a single List and all its contents to a Content 
    ''' Migration Package (CMP) file.
    ''' </summary>
    ''' <remarks>
    ''' This example exports the Tasks list to a CMP file in the root of the C: drive
    ''' </remarks>
    Partial Public Class ExportTasksList
        Inherits LayoutsPageBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        End Sub

        Protected Sub exportButton_Click(ByVal sender As Object, ByVal args As EventArgs)
            'Create an export settings object to configure the export
            Dim exportSettings As SPExportSettings = New SPExportSettings()
            'Set the site you want to export from. Alter this to match your site.
            exportSettings.SiteUrl = "http://intranet.contoso.com"
            'Select a full export (alternatively you could do an incremental export to just include changes)
            exportSettings.ExportMethod = SPExportMethodType.ExportAll
            'Set the location and name of the export file
            exportSettings.BaseFileName = "demoTasksExport"
            exportSettings.FileLocation = "C:\"

            'To export just the Tasks list, first create an ExportObject
            Dim tasksEO As SPExportObject = New SPExportObject()
            'We must set it to export a SharePoint List
            tasksEO.Type = SPDeploymentObjectType.List
            'We want to export everything in the list
            tasksEO.IncludeDescendants = SPIncludeDescendants.All
            'Set the URL of the list to export.
            tasksEO.Url = "http://intranet.contoso.com/lists/tasks"
            'Add that to the Export Objects collection
            exportSettings.ExportObjects.Add(tasksEO)

            'Do the export
            Dim export As SPExport = New SPExport(exportSettings)
            Try
                export.Run()
                resultsLabel.Text = "The export ran successfully"
            Catch ex As Exception
                resultsLabel.Text = "The export generated an error: " + ex.Message
            End Try

        End Sub

    End Class

End Namespace
