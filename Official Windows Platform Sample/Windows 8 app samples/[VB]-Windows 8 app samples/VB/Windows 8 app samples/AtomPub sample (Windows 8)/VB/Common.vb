Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Text
Imports System.Threading.Tasks
Imports Windows.Security.Credentials
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.Web.AtomPub
Imports Windows.Web.Syndication

Class CommonData
    ' The default values for the WordPress site.
    Private Shared baseUri As String = "http://<YourWordPressSite>.wordpress.com/"
    Private Shared user As String = ""
    Private Shared password As String = ""

    ' The default Service Document and Edit 'URIs' for WordPress.
    Private Const m_editUri As String = "./wp-app.php/posts"
    Private Const m_serviceDocUri As String = "./wp-app.php/service"
    Private Const m_feedUri As String = "./?feed=atom"

    Public Shared Sub Restore(inputFrame As Page)
        ' Set authentication fields.
        TryCast(inputFrame.FindName("ServiceAddressField"), TextBox).Text = baseUri
        TryCast(inputFrame.FindName("UserNameField"), TextBox).Text = user
        TryCast(inputFrame.FindName("PasswordField"), PasswordBox).Password = password
    End Sub

    Public Shared Sub Save(inputFrame As Page)
        ' Keep values of authentication fields.
        baseUri = TryCast(inputFrame.FindName("ServiceAddressField"), TextBox).Text
        user = TryCast(inputFrame.FindName("UserNameField"), TextBox).Text
        password = TryCast(inputFrame.FindName("PasswordField"), PasswordBox).Password
    End Sub

    Public Shared Function GetClient() As AtomPubClient
        Dim client As AtomPubClient

        client = New AtomPubClient
        client.BypassCacheOnRetrieve = True

        ' Assume the credentials have changed.
        client.ServerCredential = New PasswordCredential With {
            .UserName = user, _
            .Password = password _
        }

        Return client
    End Function

    Public Shared ReadOnly Property EditUri As String
        Get
            Return m_editUri
        End Get
    End Property

    Public Shared ReadOnly Property ServiceDocUri As String
        Get
            Return m_serviceDocUri
        End Get
    End Property

    Public Shared ReadOnly Property FeedUri As String
        Get
            Return m_feedUri
        End Get
    End Property
End Class

Class SyndicationItemIterator
    Private feed As SyndicationFeed
    Private index As Integer

    Public Sub New()
        Me.feed = Nothing
        Me.index = 0
    End Sub

    Public Sub AttachFeed(feed As SyndicationFeed)
        Me.feed = feed
        Me.index = 0
    End Sub

    Public Sub MoveNext()
        If feed IsNot Nothing AndAlso index < feed.Items.Count - 1 Then
            index += 1
        End If
    End Sub

    Public Sub MovePrevious()
        If feed IsNot Nothing AndAlso index > 0 Then
            index -= 1
        End If
    End Sub

    Public Function HasElements() As Boolean
        Return feed IsNot Nothing AndAlso feed.Items.Count > 0
    End Function

    Public Function GetTitle() As String
        ' Nothing to return yet.
        If Not HasElements() Then
            Return "(no title)"
        End If

        If feed.Items(index).Title IsNot Nothing Then
            Return WebUtility.HtmlDecode(feed.Items(index).Title.Text)
        End If

        Return "(no title)"
    End Function

    Public Function GetContent() As String
        ' Nothing to return yet.
        If Not HasElements() Then
            Return "(no value)"
        End If

        If feed.Items(index).Content.Text IsNot Nothing Then
            System.Diagnostics.Debug.WriteLine(feed.Items(index).Content.Text)
            Return feed.Items(index).Content.Text
        End If

        Return "(no value)"
    End Function

    Public Function GetIndexDescription() As String
        ' Nothing to return yet.
        If Not HasElements() Then
            Return "0 of 0"
        End If

        Return String.Format("{0} of {1}", index + 1, feed.Items.Count)
    End Function

    Public Function GetEditUri() As Uri
        ' Nothing to return yet.
        If Not HasElements() Then
            Return Nothing
        End If

        Return feed.Items(index).EditUri
    End Function

    Public Function GetSyndicationItem() As SyndicationItem
        ' Nothing to return yet.
        If Not HasElements() Then
            Return Nothing
        End If

        Return feed.Items(index)
    End Function
End Class
