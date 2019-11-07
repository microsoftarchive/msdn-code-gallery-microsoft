Imports System.Runtime.InteropServices
Imports Microsoft.SharePoint.UserCode

<Guid("8FC19D83-6DF0-49D9-AB86-B11F0C603290")> _
Public Class SandboxedWorkflowActivity
    'The activity's main public method must return a HashTable
    Public Function ModifyItem(ByVal context As SPUserCodeWorkflowContext) As Hashtable
        Dim results As Hashtable = New Hashtable()
        Try
            Using site As SPSite = New SPSite(context.SiteUrl)
                Using web As SPWeb = site.OpenWeb(context.WebUrl)
                    Dim list As SPList = web.Lists(context.ListId)
                    Dim workflowItem As SPListItem = list.GetItemById(context.ItemId)
                    workflowItem("Body") += "This item was modified by a sandboxed workflow"
                    results("Result") = "Success"
                    'Save the changes
                    workflowItem.Update()
                End Using
            End Using
        Catch ex As Exception
            results("Result") = "Failure"
        End Try
        Return results
    End Function
 

End Class
