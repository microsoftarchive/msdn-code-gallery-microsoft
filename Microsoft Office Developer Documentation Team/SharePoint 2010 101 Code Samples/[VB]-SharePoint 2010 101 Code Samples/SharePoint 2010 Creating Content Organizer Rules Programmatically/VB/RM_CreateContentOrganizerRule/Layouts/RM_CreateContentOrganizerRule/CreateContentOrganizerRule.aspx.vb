Imports System
Imports Microsoft.Office.RecordsManagement.RecordsRepository
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls

Namespace Layouts.RM_CreateContentOrganizerRule

    ''' <summary>
    ''' This application page demonstrates how to create and configure a Content
    ''' Organizater Routing Rule in SharePoint. It creates a rule that copies
    ''' Documents from the Shared Documents library to a folder of that library 
    ''' called DocumentDestination. 
    ''' </summary>
    ''' <remarks>
    ''' For this code to work, you must have created the folder and library described
    ''' above and enabled the Content Organizer feature at the site level. You can view
    ''' the Content Organizer rule created in Site Actions/Site Settings/Site Administration
    ''' /Content Organizer Rules and you can test it by creating a new Document in the
    ''' Shared Document library.
    ''' </remarks>
    Partial Public Class CreateContentOrganizerRule
        Inherits LayoutsPageBase

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        End Sub

        Protected Sub createRuleButton_Click(ByVal sender As Object, ByVal e As EventArgs)

            Dim currentWeb As SPWeb = SPContext.Current.Web
            'We must get the EcmDocumentRoutingWeb object that corresponds to the current SPWeb
            Dim routingWeb As EcmDocumentRoutingWeb = New EcmDocumentRoutingWeb(currentWeb)

            'Get handles on the Document content type and the Shared Documents list
            Dim contentTypeForRule As SPContentType = currentWeb.ContentTypes("Document")
            Dim listForRule As SPList = currentWeb.Lists("Shared Documents")
            Dim folderForRule As SPFolder = currentWeb.GetFolder(currentWeb.Url + "/Shared%20Documents/DocumentDestination")

            'Check that the content type is included in that library
            If listForRule.ContentTypes.BestMatch(contentTypeForRule.Id) = Nothing Then

                resultsLabel.Text = "The Document content type is not available in the " + _
                    "Shared Documents folder so the rule cannot be created"

            Else

                'Create a rule object
                Dim newRule As EcmDocumentRouterRule = New EcmDocumentRouterRule(currentWeb)

                'Set the properties
                newRule.Name = "Move all Documents"
                newRule.Description = "This rule was create by C# code."
                newRule.ContentTypeString = contentTypeForRule.Name
                newRule.RouteToExternalLocation = False
                newRule.Priority = "5"
                newRule.TargetPath = folderForRule.ServerRelativeUrl

                'Commit your changes
                newRule.Update()

            End If

            'Tell the user what happened
            resultsLabel.Text = "Rule created successfully."

        End Sub

    End Class

End Namespace
