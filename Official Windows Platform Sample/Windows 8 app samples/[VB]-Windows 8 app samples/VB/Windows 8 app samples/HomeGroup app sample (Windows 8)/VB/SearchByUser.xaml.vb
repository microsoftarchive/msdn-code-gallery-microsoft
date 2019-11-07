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
Partial Public NotInheritable Class SearchByUser
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
    Protected Overrides Async Sub OnNavigatedTo(e As NavigationEventArgs)
        Try
            Dim hgFolders As System.Collections.Generic.IReadOnlyList(Of Windows.Storage.StorageFolder) = Await Windows.Storage.KnownFolders.HomeGroup.GetFoldersAsync()
            Dim hgFoldersEnumerator As System.Collections.Generic.IEnumerator(Of Windows.Storage.StorageFolder) = hgFolders.GetEnumerator()

            ' arbitrary limit to match number of buttons created
            Dim maxUsers As Integer = If((hgFolders.Count <= 4), hgFolders.Count, 4)

            For user As Integer = 0 To maxUsers - 1
                ' We've got a user, name a button after them and make it visible
                hgFoldersEnumerator.MoveNext()
                Select Case user
                    Case 0
                        User0.Content = hgFoldersEnumerator.Current.Name
                        User0.Visibility = Visibility.Visible
                        Exit Select
                    Case 1
                        User1.Content = hgFoldersEnumerator.Current.Name
                        User1.Visibility = Visibility.Visible
                        Exit Select
                    Case 2
                        User2.Content = hgFoldersEnumerator.Current.Name
                        User2.Visibility = Visibility.Visible
                        Exit Select
                    Case 3
                        User3.Content = hgFoldersEnumerator.Current.Name
                        User3.Visibility = Visibility.Visible
                        Exit Select
                    Case Else
                        Exit Select
                End Select
            Next
        Catch ex As Exception
            rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)
        End Try
    End Sub

    ''' <summary>
    ''' This is the click handler for all four buttons in this example.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub Default_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then        
            Try
                ' Each visible button was previously set to display a homegroup user's name
                Dim userName As String = b.Content.ToString
                Dim hgFolders As System.Collections.Generic.IReadOnlyList(Of Windows.Storage.StorageFolder) = Await Windows.Storage.KnownFolders.HomeGroup.GetFoldersAsync()
                Dim userFound As Boolean = False

                For Each folder As Windows.Storage.StorageFolder In hgFolders
                    If folder.DisplayName = userName Then
                        ' We've found the folder belonging to the target user; search for all files under it
                        userFound = True
                        Dim queryOptions As New Windows.Storage.Search.QueryOptions(Windows.Storage.Search.CommonFileQuery.OrderBySearchRank, Nothing)
                        queryOptions.UserSearchFilter = "*"
                        Dim queryResults As Windows.Storage.Search.StorageFileQueryResult = folder.CreateFileQueryWithOptions(queryOptions)
                        rootPage.NotifyUser("Searching for files belonging to " & userName & "...", NotifyType.StatusMessage)
                        Dim files As System.Collections.Generic.IReadOnlyList(Of Windows.Storage.StorageFile) = Await queryResults.GetFilesAsync()

                        If files.Count > 0 Then
                            Dim outputString As String = "Searched for files belonging to " & userName & "'" & vbLf
                            outputString += If((files.Count = 1), "One file found" & vbLf, files.Count.ToString & " files found" & vbLf)
                            For Each file As Windows.Storage.StorageFile In files
                                outputString += file.Name + vbLf
                            Next
                            rootPage.NotifyUser(outputString, NotifyType.StatusMessage)
                        Else
                            rootPage.NotifyUser("No files found.", NotifyType.StatusMessage)
                        End If
                    End If
                Next

                If Not userFound Then
                    rootPage.NotifyUser("The user " & userName & " was not found on the HomeGroup.", NotifyType.ErrorMessage)
                End If
            Catch ex As Exception
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage)
            End Try
        End If
    End Sub
End Class
