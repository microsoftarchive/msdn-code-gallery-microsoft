'****************************** Module Header ******************************\
' Module Name:    Default.aspx.vb
' Project:        VBASPNETSearchEngine
' Copyright (c) Microsoft Corporation
'
' This is the Search Page. User input one or more keywords in the text box, this
' page show the result according the input.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
'*****************************************************************************/



Public Class _Default
    Inherits System.Web.UI.Page

    ''' <summary>
    ''' The keywords input by user.
    ''' </summary>
    Protected keywords As New List(Of String)()

    Protected Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs)
        ' Turn user input to a list of keywords.
        Dim keywords As String() = tbKeyWords.Text.Split(New String() {" "}, StringSplitOptions.RemoveEmptyEntries)

        ' The basic validation.
        If keywords.Length <= 0 Then
            lbAlert.Text = "Please input keyword."
            Return
        End If
        Me.keywords = keywords.ToList()

        ' Do search operation.
        Dim dataAccess As New DataAccess()
        Dim list As List(Of Article) = dataAccess.Search(Me.keywords)

        ShowResult(list)
    End Sub

    Protected Sub btnListAll_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim dataAccess As New DataAccess()
        Dim list As List(Of Article) = dataAccess.GetAll()

        ShowResult(list)
    End Sub

#Region "Helpers"

    ''' <summary>
    ''' Display a list of records in the page.
    ''' </summary>
    ''' <param name="list"></param>
    Protected Sub ShowResult(ByVal list As List(Of Article))
        RepeaterSearchResult.DataSource = list
        RepeaterSearchResult.DataBind()
    End Sub

#End Region

End Class