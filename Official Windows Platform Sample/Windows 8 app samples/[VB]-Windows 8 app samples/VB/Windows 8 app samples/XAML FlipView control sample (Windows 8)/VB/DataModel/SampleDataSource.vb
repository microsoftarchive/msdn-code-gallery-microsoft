Imports System
Imports System.Linq
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Windows.ApplicationModel.Resources.Core
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Media.Imaging

' The data model defined by this file serves as a representative example of a strongly-typed
' model that supports notification when members are added, removed, or modified.  The property
' names chosen coincide with data bindings in the standard item templates.
'
' Applications may use this model as a starting point and build on it, or discard it entirely and
' replace it with something appropriate to their needs.

Namespace Data
    ''' <summary>
    ''' Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    ''' defines properties common to both.
    ''' </summary>
    <Windows.Foundation.Metadata.WebHostHidden> _
    Public MustInherit Class SampleDataCommon
        Inherits SDKTemplate.Common.BindableBase

        Private Shared _baseUri As New Uri("ms-appx:///")

        Public Sub New(title As String, type As String, picture As String)
            Me._title = title
            Me._type = type
            Me._picture = picture
        End Sub

        Private _title As String = String.Empty
        Public Property Title() As String
            Get
                Return Me._title
            End Get
            Set(value As String)
                Me.SetProperty(Me._title, value)
            End Set
        End Property

        Private _type As String = String.Empty
        Public Property Type() As String
            Get
                Return Me._type
            End Get
            Set(value As String)
                Me.SetProperty(Me._type, value)
            End Set
        End Property

        Private _image As Uri = Nothing
        Private _picture As String = Nothing
        Public Property Image() As Uri
            Get
                Return New Uri(SampleDataCommon._baseUri, Me._picture)
            End Get

            Set(value As Uri)
                Me._picture = Nothing
                Me.SetProperty(Me._image, value)
            End Set
        End Property
    End Class

    ''' <summary>
    ''' Generic item data model.
    ''' </summary>
    Public Class SampleDataItem
        Inherits SampleDataCommon
        Public Sub New(title As String, type As String, picture As String)
            MyBase.New(title, type, picture)
        End Sub

    End Class

    ''' <summary>
    ''' Creates a collection of groups and items with hard-coded content.
    ''' </summary>
    Public NotInheritable Class SampleDataSource
        Private _items As New ObservableCollection(Of Object)()
        Public ReadOnly Property Items() As ObservableCollection(Of Object)
            Get
                Return Me._items
            End Get
        End Property

        Public Sub New()
            Items.Add(New SampleDataItem("Cliff", "item", "Assets/Cliff.jpg"))
            Items.Add(New SampleDataItem("Grapes", "item", "Assets/Grapes.jpg"))
            Items.Add(New SampleDataItem("Rainier", "item", "Assets/Rainier.jpg"))
            Items.Add(New SampleDataItem("Sunset", "item", "Assets/Sunset.jpg"))
            Items.Add(New SampleDataItem("Valley", "item", "Assets/Valley.jpg"))
        End Sub
    End Class
End Namespace
