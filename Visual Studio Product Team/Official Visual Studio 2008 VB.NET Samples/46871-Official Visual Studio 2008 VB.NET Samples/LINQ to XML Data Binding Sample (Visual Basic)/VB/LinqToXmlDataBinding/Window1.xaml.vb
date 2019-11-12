Imports System.ComponentModel
Imports System.Windows.Media

Namespace LinqToXmlDataBinding

    Partial Public Class Window1
        Inherits System.Windows.Window

        Private previousBrush As Brush

        Public Sub Window1()
            Try
                Me.InitializeComponent()
            Catch ex As Exception
                MsgBox(ex.ToString())
            End Try

        End Sub

        ''' <summary>
        ''' Save MyFavorites list on closing.
        ''' </summary>
        Protected Overrides Sub OnClosing(ByVal args As CancelEventArgs)
            Dim myFavorites = CType(CType(Resources("MyFavoritesList"), ObjectDataProvider).Data, XElement)
            myFavorites.Save("..\data\MyFavorites.xml")
        End Sub

        ''' <summary>
        ''' Play button event handler
        ''' </summary>
        Sub OnPlay(ByVal sender As Object, ByVal e As EventArgs)
            videoImage.Visibility = Visibility.Hidden
            mediaElement.Play()
        End Sub


        ''' <summary>
        ''' Stop button event handler
        ''' </summary>
        Sub OnStop(ByVal sender As Object, ByVal e As EventArgs)
            mediaElement.Stop()
            videoImage.Visibility = Visibility.Visible
        End Sub


        ''' <summary>
        ''' Add button event handler, adds currently selected video to MyFavorites
        ''' </summary>
        Sub OnAdd(ByVal sender As Object, ByVal e As EventArgs)
            Dim itemsList = CType(CType(Resources("MyFavoritesList"), ObjectDataProvider).Data, XElement)
            itemsList.Add(CType(videoListBox1.SelectedItem, XElement))
        End Sub


        '''' <summary>
        '''' Delete button event handler, delets currently selected video from MyFavorites
        '''' </summary>
        Sub OnDelete(ByVal sender As Object, ByVal e As EventArgs)
            Dim selectedItem = CType(videoListBox2.SelectedItem, XElement)
            If (Not selectedItem Is Nothing) Then
                If selectedItem.PreviousNode IsNot Nothing Then
                    Me.videoListBox2.SelectedItem = selectedItem.PreviousNode
                ElseIf (selectedItem.NextNode Is Nothing) Then
                    Me.videoListBox2.SelectedItem = selectedItem.NextNode
                    selectedItem.Remove()
                End If
            End If
        End Sub

        ''' <summary>
        ''' Searchbox event handler, Search videos by user specifed input
        ''' </summary>
        Private Overloads Sub OnKeyUp(ByVal sender As Object, ByVal e As KeyEventArgs)
            If (e.Key.Equals(Key.Enter)) Then
                Dim objectDataProvider = CType(Resources("MsnVideosList"), ObjectDataProvider)
                objectDataProvider.MethodParameters(0) = "http://soapbox.msn.com/rss.aspx?searchTerm=" & searchBox.Text
                objectDataProvider.Refresh()
            End If
        End Sub

        ''' <summary>
        ''' Event handlers for search options listed on the first page, simply update the static resource
        ''' "MsnVideosList" with the new argument and refresh it.
        ''' </summary>
        Private Overloads Sub OnMouseUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
            Dim content = CType(CType(sender, Label).Content, String)
            Dim objectDataProvider = CType(Resources("MsnVideosList"), ObjectDataProvider)
            Select Case (content)
                Case "Most Viewed"
                    objectDataProvider.MethodParameters(0) = "http://soapbox.msn.com/rss.aspx?listId=MostPopular"
                    objectDataProvider.Refresh()
                Case "Most Recent"
                    objectDataProvider.MethodParameters(0) = "http://soapbox.msn.com/rss.aspx?listId=MostRecent"
                    objectDataProvider.Refresh()
                Case "Top Favorites"
                    objectDataProvider.MethodParameters(0) = "http://soapbox.msn.com/rss.aspx?listId=TopFavorites"
                    objectDataProvider.Refresh()
                Case "Top Rated"
                    objectDataProvider.MethodParameters(0) = "http://soapbox.msn.com/rss.aspx?listId=TopRated"
                    objectDataProvider.Refresh()
                Case "My Favorites"
                    Dim msn = CType(objectDataProvider.Data, XElement)
                    Dim favorites = CType(CType(Resources("MyFavoritesList"), ObjectDataProvider).Data, XElement)
                    msn.ReplaceAll(favorites.Elements("item"))
            End Select
        End Sub

        ''' <summary>
        ''' Change the color or the search links as the mouse enters and leaves to indicate
        ''' that they are clickable
        ''' </summary>
        Private Overloads Sub OnMouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs)
            Dim myLabel = CType(sender, Label)
            previousBrush = myLabel.Foreground
            myLabel.Foreground = Brushes.Blue
        End Sub

        Private Overloads Sub OnMouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs)
            Dim myLabel = CType(sender, Label)
            myLabel.Foreground = previousBrush
        End Sub

    End Class
End Namespace
