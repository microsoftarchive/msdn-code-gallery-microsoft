' The data model defined by this file serves as a representative example of a strongly-typed
' model that supports notification when members are added, removed, or modified.  The property
' names chosen coincide with data bindings in the standard item templates.
'
' Applications may use this model as a starting point and build on it, or discard it entirely and
' replace it with something appropriate to their needs.

Imports VBWindowsStoreAppFTPDownloader.Common
Imports System.Collections.ObjectModel
Imports System.Collections.Specialized
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Media.Imaging


Namespace DataModel
    ''' <summary>
    ''' Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    ''' defines properties common to both.
    ''' </summary>
    <Windows.Foundation.Metadata.WebHostHidden>
    Public MustInherit Class SampleDataCommon
        Inherits BindableBase
        Private Shared _baseUri As New Uri("ms-appx:///")



        Public Sub New(ByVal uniqueId As String, ByVal title As String, ByVal subtitle As String, ByVal imagePath As String, ByVal description As String)
            Me._uniqueId = uniqueId
            Me._title = title
            Me._subtitle = subtitle
            Me._description = description
            Me._imagePath = imagePath
        End Sub

        Private _uniqueId As String = String.Empty
        Public Property UniqueId() As String
            Get
                Return Me._uniqueId
            End Get
            Set(ByVal value As String)
                Me.SetProperty(Me._uniqueId, value)
            End Set
        End Property

        Private _title As String = String.Empty
        Public Property Title() As String
            Get
                Return Me._title
            End Get
            Set(ByVal value As String)
                Me.SetProperty(Me._title, value)
            End Set
        End Property

        Private _subtitle As String = String.Empty
        Public Property Subtitle() As String
            Get
                Return Me._subtitle
            End Get
            Set(ByVal value As String)
                Me.SetProperty(Me._subtitle, value)
            End Set
        End Property

        Private _description As String = String.Empty
        Public Property Description() As String
            Get
                Return Me._description
            End Get
            Set(ByVal value As String)
                Me.SetProperty(Me._description, value)
            End Set
        End Property

        Private _image As ImageSource = Nothing
        Private _imagePath As String = Nothing
        Public Property Image() As ImageSource
            Get
                If Me._image Is Nothing AndAlso Me._imagePath IsNot Nothing Then
                    Me._image = New BitmapImage(New Uri(SampleDataCommon._baseUri, Me._imagePath))
                End If
                Return Me._image
            End Get

            Set(ByVal value As ImageSource)
                Me._imagePath = Nothing
                Me.SetProperty(Me._image, value)
            End Set
        End Property

        Public Sub SetImage(ByVal path As String)
            Me._image = Nothing
            Me._imagePath = path
            Me.OnPropertyChanged("Image")
        End Sub

        Public Overrides Function ToString() As String
            Return Me.Title
        End Function
    End Class

    ''' <summary>
    ''' Generic item data model.
    ''' </summary>
    Public Class SampleDataItem
        Inherits SampleDataCommon
        Public Sub New(ByVal uniqueId As String, ByVal title As String, ByVal subtitle As String, ByVal imagePath As String, ByVal description As String, ByVal content As FTP.FTPFileSystem, ByVal group As SampleDataGroup)
            MyBase.New(uniqueId, title, subtitle, imagePath, description)
            Me._content = content
            Me._group = group
        End Sub

        Private _content As FTP.FTPFileSystem = Nothing
        Public Property Content() As FTP.FTPFileSystem
            Get
                Return Me._content
            End Get
            Set(ByVal value As FTP.FTPFileSystem)
                Me.SetProperty(Me._content, value)
            End Set
        End Property

        Private _group As SampleDataGroup
        Public Property Group() As SampleDataGroup
            Get
                Return Me._group
            End Get
            Set(ByVal value As SampleDataGroup)
                Me.SetProperty(Me._group, value)
            End Set
        End Property
    End Class

    ''' <summary>
    ''' Generic group data model.
    ''' </summary>
    Public Class SampleDataGroup
        Inherits SampleDataCommon
        Public Sub New(ByVal uniqueId As String, ByVal title As String, ByVal subtitle As String, ByVal imagePath As String, ByVal description As String)
            MyBase.New(uniqueId, title, subtitle, imagePath, description)
            AddHandler Items.CollectionChanged, AddressOf ItemsCollectionChanged
        End Sub

        Private Sub ItemsCollectionChanged(ByVal sender As Object, ByVal e As System.Collections.Specialized.NotifyCollectionChangedEventArgs)
            ' Provides a subset of the full items collection to bind to from a GroupedItemsPage
            ' for two reasons: GridView will not virtualize large items collections, and it
            ' improves the user experience when browsing through groups with large numbers of
            ' items.
            '
            ' A maximum of 12 items are displayed because it results in filled grid columns
            ' whether there are 1, 2, 3, 4, or 6 rows displayed

            Select Case e.Action
                Case NotifyCollectionChangedAction.Add
                    If e.NewStartingIndex < 12 Then
                        TopItems.Insert(e.NewStartingIndex, Items(e.NewStartingIndex))
                        If TopItems.Count > 12 Then
                            TopItems.RemoveAt(12)
                        End If
                    End If
                Case NotifyCollectionChangedAction.Move
                    If e.OldStartingIndex < 12 AndAlso e.NewStartingIndex < 12 Then
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex)
                    ElseIf e.OldStartingIndex < 12 Then
                        TopItems.RemoveAt(e.OldStartingIndex)
                        TopItems.Add(Items(11))
                    ElseIf e.NewStartingIndex < 12 Then
                        TopItems.Insert(e.NewStartingIndex, Items(e.NewStartingIndex))
                        TopItems.RemoveAt(12)
                    End If
                Case NotifyCollectionChangedAction.Remove
                    If e.OldStartingIndex < 12 Then
                        TopItems.RemoveAt(e.OldStartingIndex)
                        If Items.Count >= 12 Then
                            TopItems.Add(Items(11))
                        End If
                    End If
                Case NotifyCollectionChangedAction.Replace
                    If e.OldStartingIndex < 12 Then
                        TopItems(e.OldStartingIndex) = Items(e.OldStartingIndex)
                    End If
                Case NotifyCollectionChangedAction.Reset
                    TopItems.Clear()
                    Do While TopItems.Count < Items.Count AndAlso TopItems.Count < 12
                        TopItems.Add(Items(TopItems.Count))
                    Loop
            End Select
        End Sub

        Private _items As New ObservableCollection(Of SampleDataItem)()
        Public ReadOnly Property Items() As ObservableCollection(Of SampleDataItem)
            Get
                Return Me._items
            End Get
        End Property

        Private _topItem As New ObservableCollection(Of SampleDataItem)()
        Public ReadOnly Property TopItems() As ObservableCollection(Of SampleDataItem)
            Get
                Return Me._topItem
            End Get
        End Property
    End Class
End Namespace
