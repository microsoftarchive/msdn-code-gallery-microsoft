Imports Windows.UI.Xaml.Media.Imaging
Imports Windows.UI.Xaml.Data
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Windows.UI.Xaml.Media

Namespace Expression.Blend.SampleData.SampleDataSource

    ' To significantly reduce the sample data footprint in your production application, you can set
    ' the DISABLE_SAMPLE_DATA conditional compilation constant and disable sample data at runtime.
#If DISABLE_SAMPLE_DATA Then
    Friend Class SampleDataSource
    End Class
#Else

    Public Class Item
        Implements System.ComponentModel.INotifyPropertyChanged

        Public Event PropertyChanged As System.ComponentModel.PropertyChangedEventHandler Implements System.ComponentModel.INotifyPropertyChanged.PropertyChanged

        Protected Overridable Sub OnPropertyChanged(ByVal propertyName As String)
            RaiseEvent PropertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
        End Sub

        Private _Title As String = String.Empty
        Public Property Title() As String
            Get
                Return Me._Title
            End Get

            Set(ByVal value As String)
                If Me._Title <> value Then
                    Me._Title = value
                    Me.OnPropertyChanged("Title")
                End If
            End Set
        End Property

        Private _Subtitle As String = String.Empty
        Public Property Subtitle() As String
            Get
                Return Me._Subtitle
            End Get

            Set(ByVal value As String)
                If Me._Subtitle <> value Then
                    Me._Subtitle = value
                    Me.OnPropertyChanged("Subtitle")
                End If
            End Set
        End Property

        Private _Image As ImageSource = Nothing
        Public Property Image() As ImageSource
            Get
                Return Me._Image
            End Get

            Set(ByVal value As ImageSource)
                If Me._Image IsNot value Then
                    Me._Image = value
                    Me.OnPropertyChanged("Image")
                End If
            End Set
        End Property

        Public Sub SetImage(ByVal baseUri As Uri, ByVal path As String)
            Image = New BitmapImage(New Uri(baseUri, path))
        End Sub

        Private _Link As String = String.Empty
        Public Property Link() As String
            Get
                Return Me._Link
            End Get

            Set(ByVal value As String)
                If Me._Link <> value Then
                    Me._Link = value
                    Me.OnPropertyChanged("Link")
                End If
            End Set
        End Property

        Private _Category As String = String.Empty
        Public Property Category() As String
            Get
                Return Me._Category
            End Get

            Set(ByVal value As String)
                If Me._Category <> value Then
                    Me._Category = value
                    Me.OnPropertyChanged("Category")
                End If
            End Set
        End Property

        Private _Description As String = String.Empty
        Public Property Description() As String
            Get
                Return Me._Description
            End Get

            Set(ByVal value As String)
                If Me._Description <> value Then
                    Me._Description = value
                    Me.OnPropertyChanged("Description")
                End If
            End Set
        End Property

        Private _Content As String = String.Empty
        Public Property Content() As String
            Get
                Return Me._Content
            End Get

            Set(ByVal value As String)
                If Me._Content <> value Then
                    Me._Content = value
                    Me.OnPropertyChanged("Content")
                End If
            End Set
        End Property
    End Class

    Public Class GroupInfoList(Of T)
        Inherits List(Of Object)

        Public Property Key() As Object


        Public Shadows Function GetEnumerator() As IEnumerator(Of Object)
            Return CType(MyBase.GetEnumerator(), System.Collections.Generic.IEnumerator(Of Object))
        End Function
    End Class

    Public Class MessageData
        Public Sub New()
            Dim item As Item
            Dim LONG_LOREM_IPSUM As String = String.Format("{0}" & vbLf & vbLf & "{0}" & vbLf & vbLf & "{0}" & vbLf & vbLf & "{0}", "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat")
            Dim _baseUri As New Uri("ms-appx:///")

            item = New Item()
            item.Title = "New Flavors out this week!"
            item.Subtitle = "Adam Barr"
            item.SetImage(_baseUri, "SampleData/Images/image1.jpg")
            item.Content = "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum"
            Collection.Add(item)

            item = New Item()
            item.Title = "Check out this topping!"
            item.Subtitle = "David Alexander"
            item.SetImage(_baseUri, "SampleData/Images/image2.jpg")
            item.Content = "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue"
            Collection.Add(item)

            item = New Item()
            item.Title = "Come to the Ice Cream Party"
            item.Subtitle = "Josh Bailey"
            item.SetImage(_baseUri, "SampleData/Images/image3.jpg")
            item.Content = "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse"
            Collection.Add(item)

            item = New Item()
            item.Title = "How about gluten free?"
            item.Subtitle = "Chris Berry"
            item.SetImage(_baseUri, "SampleData/Images/image4.jpg")
            item.Content = LONG_LOREM_IPSUM
            Collection.Add(item)

            item = New Item()
            item.Title = "Summer promotion - BYGO"
            item.Subtitle = "Sean Bentley"
            item.SetImage(_baseUri, "SampleData/Images/image5.jpg")
            item.Content = "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat"
            Collection.Add(item)

            item = New Item()
            item.Title = "Awesome flavor combination"
            item.Subtitle = "Adrian Lannin"
            item.SetImage(_baseUri, "SampleData/Images/image6.jpg")
            item.Content = "Curabitur class aliquam vestibulum nam curae maecenas sed integer"
            Collection.Add(item)

        End Sub
        Private _Collection As New ItemCollection()

        Public ReadOnly Property Collection() As ItemCollection
            Get
                Return Me._Collection
            End Get
        End Property
    End Class

    ' Workaround: data binding works best with an enumeration of objects that does not implement IList
    Public Class ItemCollection
        Implements IEnumerable(Of Object)

        Private itemCollection As New System.Collections.ObjectModel.ObservableCollection(Of Item)()

        Public Function GetEnumerator() As IEnumerator(Of Object) Implements IEnumerable(Of Object).GetEnumerator
            Return itemCollection.GetEnumerator()
        End Function

        Private Function IEnumerable_GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return GetEnumerator()
        End Function

        Public Sub Add(ByVal item As Item)
            itemCollection.Add(item)
        End Sub
    End Class
#End If
End Namespace
