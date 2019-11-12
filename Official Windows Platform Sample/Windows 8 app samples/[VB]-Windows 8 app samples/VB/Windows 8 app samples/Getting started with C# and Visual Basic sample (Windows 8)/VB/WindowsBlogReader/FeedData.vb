'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.Web.Syndication

' FeedData
' Holds info for a single blog feed, including a list of blog posts (FeedItem).
Public Class FeedData
    Public Property Title() As String = String.Empty
    Public Property Description() As String = String.Empty
    Public Property PubDate() As DateTime

    Private _Items As New List(Of FeedItem)()
    Public ReadOnly Property Items() As List(Of FeedItem)
        Get
            Return Me._Items
        End Get
    End Property
End Class

' FeedItem
' Holds info for a single blog post.
Public Class FeedItem
    Public Property Title() As String = String.Empty
    Public Property Author() As String = String.Empty
    Public Property Content() As String = String.Empty
    Public Property PubDate() As DateTime
    Public Property Link() As Uri
End Class

' FeedDataSource
' Holds a collection of blog feeds (FeedData), and contains methods needed to
' retreive the feeds.
Public Class FeedDataSource
    Private _Feeds As New ObservableCollection(Of FeedData)()
    Public ReadOnly Property Feeds() As ObservableCollection(Of FeedData)
        Get
            Return Me._Feeds
        End Get
    End Property

    Public Async Function GetFeedsAsync() As Task
        Dim feed1 As Task(Of FeedData) =
            GetFeedAsync("http://blogs.windows.com/skydrive/b/skydrive/atom.aspx")
        Dim feed2 As Task(Of FeedData) =
            GetFeedAsync("http://blogs.windows.com/windows/b/windowsexperience/atom.aspx")
        Dim feed3 As Task(Of FeedData) =
            GetFeedAsync("http://blogs.windows.com/windows/b/extremewindows/atom.aspx")
        Dim feed4 As Task(Of FeedData) =
            GetFeedAsync("http://blogs.windows.com/windows/b/business/atom.aspx")
        Dim feed5 As Task(Of FeedData) =
            GetFeedAsync("http://blogs.windows.com/windows/b/bloggingwindows/atom.aspx")
        Dim feed6 As Task(Of FeedData) =
            GetFeedAsync("http://blogs.windows.com/windows/b/windowssecurity/atom.aspx")
        Dim feed7 As Task(Of FeedData) =
            GetFeedAsync("http://blogs.windows.com/windows/b/springboard/atom.aspx")
        Dim feed8 As Task(Of FeedData) =
            GetFeedAsync("http://blogs.windows.com/windows/b/windowshomeserver/atom.aspx")
        ' There is no Atom feed for this blog, so use the RSS feed.
        Dim feed9 As Task(Of FeedData) =
            GetFeedAsync("http://blogs.windows.com/windows_live/b/windowslive/rss.aspx")
        Dim feed10 As Task(Of FeedData) =
            GetFeedAsync("http://blogs.windows.com/windows_live/b/developer/atom.aspx")
        Dim feed11 As Task(Of FeedData) =
            GetFeedAsync("http://blogs.windows.com/ie/b/ie/atom.aspx")
        Dim feed12 As Task(Of FeedData) =
            GetFeedAsync("http://blogs.windows.com/windows_phone/b/wpdev/atom.aspx")
        Dim feed13 As Task(Of FeedData) =
            GetFeedAsync("http://blogs.windows.com/windows_phone/b/wmdev/atom.aspx")
        Dim feed14 As Task(Of FeedData) =
            GetFeedAsync("http://blogs.windows.com/windows_phone/b/windowsphone/atom.aspx")

        Me.Feeds.Add(Await feed1)
        Me.Feeds.Add(Await feed2)
        Me.Feeds.Add(Await feed3)
        Me.Feeds.Add(Await feed4)
        Me.Feeds.Add(Await feed5)
        Me.Feeds.Add(Await feed6)
        Me.Feeds.Add(Await feed7)
        Me.Feeds.Add(Await feed8)
        Me.Feeds.Add(Await feed9)
        Me.Feeds.Add(Await feed10)
        Me.Feeds.Add(Await feed11)
        Me.Feeds.Add(Await feed12)
        Me.Feeds.Add(Await feed13)
        Me.Feeds.Add(Await feed14)
    End Function

    Private Async Function GetFeedAsync(feedUriString As String) As Task(Of FeedData)
        Dim Client As New SyndicationClient
        Dim FeedUri As New Uri(feedUriString)

        Try
            Dim Feed As SyndicationFeed = Await Client.RetrieveFeedAsync(FeedUri)

            ' This code is executed after RetrieveFeedAsync returns the SyndicationFeed.
            ' Process the feed and copy the data you want into the FeedData and FeedItem classes.
            Dim FeedData As New FeedData

            If Feed.Title IsNot Nothing AndAlso Feed.Title.Text IsNot Nothing Then
                FeedData.Title = Feed.Title.Text
            End If
            If Feed.Subtitle IsNot Nothing AndAlso Feed.Subtitle.Text IsNot Nothing Then
                FeedData.Description = Feed.Subtitle.Text
            End If
            If Feed.Items IsNot Nothing AndAlso Feed.Items.Count > 0 Then
                ' Use the date of the latest post as the last updated date.
                FeedData.PubDate = Feed.Items(0).PublishedDate.DateTime
                For Each Item As SyndicationItem In Feed.Items
                    Dim FeedItem As New FeedItem
                    If Item.Title IsNot Nothing AndAlso Item.Title.Text IsNot Nothing Then
                        FeedItem.Title = Item.Title.Text
                    End If
                    FeedItem.PubDate = Item.PublishedDate.DateTime
                    If Item.Authors IsNot Nothing AndAlso Item.Authors.Count > 0 Then
                        FeedItem.Author = Item.Authors(0).Name.ToString()
                    End If
                    ' Handle the differences between RSS and Atom feeds.
                    If Feed.SourceFormat = SyndicationFormat.Atom10 Then
                        If Item.Content IsNot Nothing AndAlso Item.Content.Text IsNot Nothing Then
                            FeedItem.Content = Item.Content.Text
                        End If
                        If Item.Id IsNot Nothing Then
                            FeedItem.Link = New Uri(Item.Id)
                        End If
                    ElseIf Feed.SourceFormat = SyndicationFormat.Rss20 Then
                        If Item.Summary IsNot Nothing AndAlso Item.Summary.Text IsNot Nothing Then
                            FeedItem.Content = Item.Summary.Text
                        End If
                        If Item.Links IsNot Nothing AndAlso Item.Links.Count > 0 Then
                            FeedItem.Link = Item.Links(0).Uri
                        End If
                    End If
                    FeedData.Items.Add(FeedItem)
                Next
            End If
            Return FeedData

        Catch Ex As Exception
            Return Nothing
        End Try
    End Function

    Public Shared Function GetFeed(title As String) As FeedData
        ' Simple linear search is acceptable for small data sets
        Dim _feedDataSource = DirectCast(App.Current.Resources("feedDataSource"), FeedDataSource)

        Dim matches = _feedDataSource.Feeds.Where(Function(feed) feed IsNot Nothing AndAlso feed.Title.Equals(title))
        If matches.Count() = 1 Then
            Return matches.First()
        End If
        Return Nothing
    End Function

    Public Shared Function GetItem(uniqueId As String) As FeedItem
        ' Simple linear search is acceptable for small data sets
        Dim _feedDataSource = DirectCast(App.Current.Resources("feedDataSource"), FeedDataSource)

        Dim matches = _feedDataSource.Feeds.
            Where(Function(group) group IsNot Nothing).
            SelectMany(Function(group) group.Items).Where(Function(item) item.Title.Equals(uniqueId))

        If matches.Count() = 1 Then
            Return matches.First()
        End If
        Return Nothing
    End Function
End Class
