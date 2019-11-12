'****************************** Module Header ******************************\
' Module Name:    DataAccess.vb
' Project:        VBASPNETSearchEngine
' Copyright (c) Microsoft Corporation
'
' This class is a Data Access Layer which does database operations.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
'
'*****************************************************************************/

Imports System.Data.SqlClient

''' <summary>
''' This class is used to access database.
''' </summary>
Public Class DataAccess
    ''' <summary>
    ''' Retrieve an individual record from database.
    ''' </summary>
    ''' <param name="id">Record id</param>
    ''' <returns>A found record</returns>
    Public Function GetArticle(ByVal id As Long) As Article
        Dim articles As List(Of Article) = QueryList("select * from [Articles] where [ID] = " & id.ToString())

        ' Only return the first record.
        If articles.Count > 0 Then
            Return articles(0)
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Retrieve all records from database.
    ''' </summary>
    ''' <returns></returns>
    Public Function GetAll() As List(Of Article)
        Return QueryList("select * from [Articles]")
    End Function

    ''' <summary>
    ''' Search records from database.
    ''' </summary>
    ''' <param name="keywords">the list of keywords</param>
    ''' <returns>all found records</returns>
    Public Function Search(ByVal keywords As List(Of String)) As List(Of Article)
        ' Generate a complex Sql command.
        Dim sqlBuilder As New StringBuilder()
        sqlBuilder.Append("select * from [Articles] where ")
        For Each item As String In keywords
            sqlBuilder.AppendFormat("([Title] like '%{0}%' or [Content] like '%{0}%') and ", item)
        Next

        ' Remove unnecessary string " and " at the end of the command.
        Dim sql As String = sqlBuilder.ToString(0, sqlBuilder.Length - 5)

        Return QueryList(sql)
    End Function

#Region "Helpers"

    ''' <summary>
    ''' Create a connected SqlCommand object.
    ''' </summary>
    ''' <param name="cmdText">Command text</param>
    ''' <returns>SqlCommand object</returns>
    Protected Function GenerateSqlCommand(ByVal cmdText As String) As SqlCommand
        ' Read Connection String from web.config file.
        Dim conn As New SqlConnection(ConfigurationManager.ConnectionStrings("MyDatabaseConnectionString").ConnectionString)
        Dim cmd As New SqlCommand(cmdText, conn)
        cmd.Connection.Open()
        Return cmd
    End Function

    ''' <summary>
    ''' Create an Article object from a SqlDataReader object.
    ''' </summary>
    ''' <param name="reader"></param>
    ''' <returns></returns>
    Protected Function ReadArticle(ByVal reader As SqlDataReader) As Article
        Dim article As New Article()

        article.ID = CLng(reader("ID"))
        article.Title = DirectCast(reader("Title"), String)
        article.Content = DirectCast(reader("Content"), String)

        Return article
    End Function

    ''' <summary>
    ''' Excute a Sql command.
    ''' </summary>
    ''' <param name="cmdText">Command text</param>
    ''' <returns></returns>
    Protected Function QueryList(ByVal cmdText As String) As List(Of Article)
        Dim articles As New List(Of Article)()

        Dim cmd As SqlCommand = GenerateSqlCommand(cmdText)
        Using cmd.Connection
            Dim reader As SqlDataReader = cmd.ExecuteReader()

            ' Transform records to a list.
            If reader.HasRows Then
                While reader.Read()
                    articles.Add(ReadArticle(reader))
                End While
            End If
        End Using
        Return articles
    End Function

#End Region
End Class
