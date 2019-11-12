'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class SearchHomeGroup
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub


    ''' <summary>
    ''' This is the click handler for the Search button
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub Search_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then            
            Try
                Dim queryOptions As New Windows.Storage.Search.QueryOptions(Windows.Storage.Search.CommonFileQuery.OrderBySearchRank, Nothing)
                queryOptions.UserSearchFilter = SearchQuery.Text
                Dim queryResults As Windows.Storage.Search.StorageFileQueryResult = Windows.Storage.KnownFolders.HomeGroup.CreateFileQueryWithOptions(queryOptions)
                rootPage.NotifyUser("Searching for '" & SearchQuery.Text & "' ...", NotifyType.StatusMessage)
                Dim files As System.Collections.Generic.IReadOnlyList(Of Windows.Storage.StorageFile) = Await queryResults.GetFilesAsync()

                If files.Count > 0 Then
                    Dim outputString As String = "Searched for '" & SearchQuery.Text & "'" & vbLf
                    outputString += If((files.Count = 1), "One file found" & vbLf, files.Count.ToString & " files found" & vbLf)
                    For Each file As Windows.Storage.StorageFile In files
                        outputString += file.Name + vbLf
                    Next
                    rootPage.NotifyUser(outputString, NotifyType.StatusMessage)
                Else
                    rootPage.NotifyUser("Searched for '" & SearchQuery.Text & "'" & vbLf & "No files found.", NotifyType.StatusMessage)
                End If
            Catch ex As Exception
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)
            End Try
        End If
    End Sub
End Class
