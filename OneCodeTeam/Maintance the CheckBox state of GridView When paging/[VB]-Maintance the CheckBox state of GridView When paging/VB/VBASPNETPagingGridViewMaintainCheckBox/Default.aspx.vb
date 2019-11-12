'****************************** Module Header ******************************\
' Module Name:    Default.aspx.vb
' Project:        VBASPNETPagingGridViewMaintainCheckBox
' Copyright (c) Microsoft Corporation
'
' This demo is mainly showing you how to make the CheckBox in the each row of 
' GridView "keep its state" when paging.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/

Public Class _Default
    Inherits System.Web.UI.Page
    ' A list to store check state of CheckBox.
    Private isChecked As List(Of Boolean) = Nothing

    ''' <summary>
    ''' Initializing to bind with the generated data table.
    ''' </summary>
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            MyDataBind()
        End If
    End Sub

    ''' <summary>
    ''' Create a dynamic datatable and store it into ViewState 
    ''' for further use.
    ''' </summary>
    Private Sub MyDataBind()
        If ViewState("dt") Is Nothing Then
            Dim dt As New DataTable()
            dt.Columns.Add("Id", GetType(Integer))
            dt.Columns.Add("Name", GetType(String))
            isChecked = New List(Of Boolean)()

            For i As Integer = 1 To 40
                dt.Rows.Add(i, "Name" & i)
                isChecked.Add(False)
            Next
            ViewState("dt") = dt
            ViewState("CheckList") = isChecked
        End If

        gvData.DataSource = TryCast(ViewState("dt"), DataTable)
        gvData.DataBind()
        isChecked = TryCast(ViewState("CheckList"), List(Of Boolean))
    End Sub

    ''' <summary>
    ''' Change the current GridView's PageIndex
    ''' </summary>
    Protected Sub gvData_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gvData.PageIndexChanging
        gvData.PageIndex = e.NewPageIndex
    End Sub

    ''' <summary>
    ''' After changing the current GridView's PageIndex, rebind to the GridView.
    ''' </summary>
    Protected Sub gvData_PageIndexChanged(sender As Object, e As EventArgs) Handles gvData.PageIndexChanged
        MyDataBind()
        gvData.SelectedIndex = -1
    End Sub

    ''' <summary>
    ''' When choosing a CheckBox button, get the selected row's primary key's value.      
    ''' </summary> 
    Protected Sub chbChoice_CheckedChanged(sender As Object, e As EventArgs)
        Dim gr As GridViewRow = TryCast(DirectCast(sender, Control).NamingContainer, GridViewRow)
        isChecked = TryCast(ViewState("CheckList"), List(Of Boolean))
        isChecked(gr.RowIndex + (gvData.PageIndex) * gvData.PageSize) = True
        ViewState("CheckList") = isChecked
    End Sub

    ''' <summary>
    ''' According to the List(Of Boolean) that been saved in ViewState to set the state of current row's CheckBox.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub gvData_RowDataBound(sender As Object, e As GridViewRowEventArgs)
        isChecked = TryCast(ViewState("CheckList"), List(Of Boolean))

        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim chk As CheckBox = DirectCast(e.Row.FindControl("chbChoice"), CheckBox)
            chk.Checked = isChecked(e.Row.RowIndex + gvData.PageIndex * gvData.PageSize)
        End If
    End Sub
End Class