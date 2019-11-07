Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.Publishing
Imports Microsoft.SharePoint.WebControls


''' <summary>
''' This custom field control is a simple textbox control for a custom site column
''' attached to the Publishing Page content type
''' </summary>
''' <remarks>
''' Before you can use this project you must enable the Publishing features at both
''' the site and site collection levels. You must also have created a custom site column
''' called "DemoCustomColumn" and added it to the Page content type. Then add a Page
''' to the Pages library. 
''' </remarks>
Public Class CustomFieldControl
    Inherits Control

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        'Check whether there in a SPContext, and if this is a List Item
        If (SPContext.Current IsNot Nothing) And (SPContext.Current.ListItem IsNot Nothing) Then
            'Check whether the current content type is or inherits from the Page content type
            Dim pageContentTypeId As SPContentTypeId = SPContext.Current.Web.AvailableContentTypes("Page").Id
            Dim currentItemTypeId As SPContentTypeId = SPContext.Current.ListItem.ContentTypeId
            If currentItemTypeId.Equals(pageContentTypeId) Or currentItemTypeId.IsChildOf(pageContentTypeId) Then
                'Get the PageHolderMain content place holder control
                Dim placeHolderMain As ContentPlaceHolder = TryCast(Me.Page.Master.FindControl("PlaceHolderMain"), ContentPlaceHolder)
                If placeHolderMain IsNot Nothing Then
                    'Check whether the current item is a publishing page and is a New item or in Edit mode
                    If (PublishingPage.IsPublishingPage(SPContext.Current.ListItem)) _
                        AndAlso ((SPContext.Current.FormContext.FormMode = SPControlMode.Edit) _
                        OrElse (SPContext.Current.FormContext.FormMode = SPControlMode.New)) Then

                        'Get the custom column
                        Dim demoCustomColumn As SPField
                        Try
                            demoCustomColumn = SPContext.Current.ListItem.Fields("DemoCustomColumn")
                        Catch ex As Exception
                            demoCustomColumn = Nothing
                        End Try

                        If demoCustomColumn IsNot Nothing Then
                            'We have a page, in edit or new mode, with the demoCustomColumn.
                            'So render the custom field control
                            Dim demoCustomColumnControl As BaseFieldControl = demoCustomColumn.FieldRenderingControl
                            demoCustomColumnControl.ID = demoCustomColumn.InternalName
                            placeHolderMain.Controls.Add(New LiteralControl("<div class=""edit-mode-panel"">"))
                            placeHolderMain.Controls.Add(demoCustomColumnControl)
                            placeHolderMain.Controls.Add(New LiteralControl("</div>"))
                        End If

                    ElseIf (PublishingPage.IsPublishingPage(SPContext.Current.ListItem)) _
                        AndAlso (SPContext.Current.FormContext.FormMode = SPControlMode.Display) Then

                        'Get the custom column
                        Dim demoCustomColumn As SPField
                        Try
                            demoCustomColumn = TryCast(SPContext.Current.ListItem.Fields("DemoCustomColumn"), SPFieldText)
                        Catch ex As Exception
                            demoCustomColumn = Nothing
                        End Try
                        If demoCustomColumn IsNot Nothing Then
                            'We have a page, in display mode, with the demoCustomColumn.
                            'So render the value of the field. You can add custom rendering markup
                            'here. In this case, the <div> tag renders the text in red.
                            placeHolderMain.Controls.Add(New LiteralControl("<div style=""color: red;"">"))
                            placeHolderMain.Controls.Add(New LiteralControl(SPContext.Current.ListItem("DemoCustomColumn").ToString()))
                            placeHolderMain.Controls.Add(New LiteralControl("</div>"))
                        End If

                    End If

                End If

            End If
        End If

        MyBase.OnInit(e)
    End Sub
End Class
