Imports System
Imports System.ComponentModel
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Web.UI.WebControls.WebParts
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls

<ToolboxItemAttribute(False)> _
Public Class SPO_CreateContentTypeAndList
    Inherits WebPart

    'This Web Part demonstrates code to create a content type and 
    'custom list. This code can run in the sandbox so you can use it
    'in SharePoint Online as well as your on premise farms.

    'To test this solution before deployment, set the Site URL 
    'property of the project to match your test SharePoint farm, then
    'use F5

    'To deploy this project to your SharePoint Online site, upload
    'the SPO_SandboxedWebPart.wsp solution file from the bin/debug
    'folder to your solution gallery. Activate the solution and add
    'the web part to a page. 

    'Web Controls
    Private buttonCreateType As Button
    Private labelCreateTypeResult As Label

    Protected Overrides Sub CreateChildControls()
        'Set up the label that tells the user what happened
        labelCreateTypeResult = New Label()
        Me.Controls.Add(labelCreateTypeResult)
        'Set up the button that you click to create the objects
        buttonCreateType = New Button()
        buttonCreateType.Text = "Create Content Type and List"
        AddHandler buttonCreateType.Click, AddressOf buttonCreateType_Click
        Me.Controls.Add(buttonCreateType)
    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        EnsureChildControls()
        buttonCreateType.RenderControl(writer)
        writer.Write("<br />")
        labelCreateTypeResult.RenderControl(writer)
    End Sub

    Protected Sub buttonCreateType_Click(ByVal sender As Object, ByVal e As EventArgs)
        'Here we will create a new content type and list
        'Start by getting the current web
        Dim web As SPWeb = SPContext.Current.Web
        'Get the web's collection of content types
        Dim contentTypes As SPContentTypeCollection = web.ContentTypes
        'Create a new content type
        Dim newType As SPContentType = New SPContentType(contentTypes(SPBuiltInContentTypeId.Announcement), contentTypes, "Contoso Announcements")
        'Add it to the web 
        Try
            contentTypes.Add(newType)
        Catch ex As SPException
            'This is probably because the content type already exists
            labelCreateTypeResult.Text = ex.Message
        End Try
        'Now get the web's field collection and add a new field to it
        Dim siteFields As SPFieldCollection = web.Fields
        Try
            siteFields.Add("Product", SPFieldType.Text, False)
            web.Update()
        Catch ex As SPException
            'This is probably because the field already exists
            labelCreateTypeResult.Text = ex.Message
        End Try
        'Add the field to the new content type
        newType.FieldLinks.Add(New SPFieldLink(siteFields("Product")))
        newType.Update()
        'Get the web's list collection
        Dim lists As SPListCollection = web.Lists
        Try
            Dim newListGuid As Guid = lists.Add("Product Announcements", "Announcements about Contoso Products", SPListTemplateType.Announcements)
            Dim newList As SPList = lists(newListGuid)
            newList.ContentTypes.Add(newType)
            newList.Update()
        Catch ex As Exception
            'This is probably because the field already exists
            labelCreateTypeResult.Text = ex.Message
        End Try
        labelCreateTypeResult.Text = "Contoso Announcement content type and Product Announcements list created successfully"
    End Sub

End Class
