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


	Public Class StoreData
		Public Sub New()
			Dim item As Item
			Dim LONG_LOREM_IPSUM As String = String.Format("{0}" & vbLf & vbLf & "{0}" & vbLf & vbLf & "{0}" & vbLf & vbLf & "{0}", "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat")
			Dim _baseUri As New Uri("ms-appx:///")

			item = New Item()
			item.Title = "Banana Blast Frozen Yogurt"
			item.Subtitle = "Maecenas class nam praesent cras aenean mauris aliquam nullam aptent accumsan duis nunc curae donec integer auctor sed congue amet"
			item.SetImage(_baseUri, "SampleData/Images/60Banana.png")
			item.Link = "http://www.adatum.com/"
			item.Category = "Low-fat frozen yogurt"
			item.Description = "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat"
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)

			item = New Item()
			item.Title = "Lavish Lemon Ice"
			item.Subtitle = "Quisque vivamus bibendum cursus dictum dictumst dis aliquam aliquet etiam lectus eleifend fusce libero ante facilisi ligula est"
			item.SetImage(_baseUri, "SampleData/Images/60Lemon.png")
			item.Link = "http://www.adventure-works.com/"
			item.Category = "Sorbet"
			item.Description = "Enim cursus nascetur dictum habitasse hendrerit nec gravida vestibulum pellentesque vestibulum adipiscing iaculis erat consectetuer pellentesque parturient lacinia himenaeos pharetra condimentum non sollicitudin eros dolor vestibulum per lectus pellentesque nibh imperdiet laoreet consectetuer placerat libero malesuada pellentesque fames penatibus ligula scelerisque litora nisi luctus vestibulum nisl ullamcorper sed sem natoque suspendisse felis sit condimentum pulvinar nunc posuere magnis vel scelerisque sagittis porttitor potenti tincidunt mattis ipsum adipiscing sollicitudin parturient mauris nam senectus ullamcorper mollis tristique sociosqu suspendisse ultricies montes sed condimentum dis nostra suscipit justo ornare pretium odio pellentesque lacus lorem torquent orci"
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)

			item = New Item()
			item.Title = "Marvelous Mint"
			item.Subtitle = "Litora luctus magnis arcu lorem morbi blandit faucibus mattis commodo hac habitant inceptos conubia cubilia nulla mauris diam proin augue eget dolor mollis interdum lobortis"
			item.SetImage(_baseUri, "SampleData/Images/60Mint.png")
			item.Link = "http://www.adventure-works.com/"
			item.Category = "Gelato"
			item.Description = "Vestibulum vestibulum magna scelerisque ultrices consectetuer vehicula rhoncus pellentesque massa adipiscing platea primis sodales parturient metus sollicitudin morbi vestibulum pellentesque consectetuer pellentesque volutpat rutrum sollicitudin sapien pellentesque vestibulum venenatis consectetuer viverra est aliquam semper hac maecenas integer adipiscing sociis vulputate ullamcorper curabitur pellentesque parturient praesent neque sollicitudin pellentesque vestibulum suspendisse consectetuer leo quisque phasellus pede vestibulum quam pellentesque sollicitudin quis mus adipiscing parturient pellentesque vestibulum"
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)

			item = New Item()
			item.Title = "Creamy Orange"
			item.Subtitle = "Leo mus nec nascetur dapibus non fames per felis ipsum pharetra egestas montes elit nostra placerat euismod enim justo ornare feugiat platea pulvinar sed sagittis"
			item.SetImage(_baseUri, "SampleData/Images/60Orange.png")
			item.Link = "http://www.alpineskihouse.com/"
			item.Category = "Sorbet"
			item.Description = "Consequat condimentum consectetuer vivamus urna vestibulum netus pellentesque cras nec taciti non scelerisque adipiscing parturient tellus sollicitudin per vestibulum pellentesque aliquam convallis ullamcorper nulla porta aliquet accumsan suspendisse duis bibendum nunc condimentum consectetuer pellentesque scelerisque tempor sed dictumst eleifend amet vestibulum sem tempus facilisi ullamcorper adipiscing tortor ante purus parturient sit dignissim vel nam turpis sed sollicitudin elementum arcu vestibulum risus blandit suspendisse faucibus pellentesque commodo dis condimentum consectetuer varius aenean conubia cubilia facilisis velit mauris nullam aptent dapibus habitant"
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)

			item = New Item()
			item.Title = "Succulent Strawberry"
			item.Subtitle = "Senectus sem lacus erat sociosqu eros suscipit primis nibh nisi nisl gravida torquent"
			item.SetImage(_baseUri, "SampleData/Images/60Strawberry.png")
			item.Link = "http://www.baldwinmuseumofscience.com/"
			item.Category = "Sorbet"
			item.Description = "Est auctor inceptos congue interdum egestas scelerisque pellentesque fermentum ullamcorper cursus dictum lectus suspendisse condimentum libero vitae vestibulum lobortis ligula fringilla euismod class scelerisque feugiat habitasse diam litora adipiscing sollicitudin parturient hendrerit curae himenaeos imperdiet ullamcorper suspendisse nascetur hac gravida pharetra eget donec leo mus nec non malesuada vestibulum pellentesque elit penatibus vestibulum per condimentum porttitor sed adipiscing scelerisque ullamcorper etiam iaculis enim tincidunt erat parturient sem vestibulum eros"
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)

			item = New Item()
			item.Title = "Very Vanilla"
			item.Subtitle = "Ultrices rutrum sapien vehicula semper lorem volutpat sociis sit maecenas praesent taciti magna nunc odio orci vel tellus nam sed accumsan iaculis dis est"
			item.SetImage(_baseUri, "SampleData/Images/60Vanilla.png")
			item.Link = "http://www.blueyonderairlines.com/"
			item.Category = "Ice Cream"
			item.Description = "Consectetuer lacinia vestibulum tristique sit adipiscing laoreet fusce nibh suspendisse natoque placerat pulvinar ultricies condimentum scelerisque nisi ullamcorper nisl parturient vel suspendisse nam venenatis nunc lorem sed dis sagittis pellentesque luctus sollicitudin morbi posuere vestibulum potenti magnis pellentesque vulputate mattis mauris mollis consectetuer pellentesque pretium montes vestibulum condimentum nulla adipiscing sollicitudin scelerisque ullamcorper pellentesque odio orci rhoncus pede sodales suspendisse parturient viverra curabitur proin aliquam integer augue quam condimentum quisque senectus quis urna scelerisque nostra phasellus ullamcorper cras duis suspendisse sociosqu dolor vestibulum condimentum consectetuer vivamus est fames felis suscipit hac"
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)

			item = New Item()
			item.Title = "Creamy Caramel Frozen Yogurt"
			item.Subtitle = "Maecenas class nam praesent cras aenean mauris aliquam nullam aptent accumsan duis nunc curae donec integer auctor sed congue amet"
			item.SetImage(_baseUri, "SampleData/Images/60SauceCaramel.png")
			item.Link = "http://www.adatum.com/"
			item.Category = "Low-fat frozen yogurt"
			item.Description = "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat"
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)

			item = New Item()
			item.Title = "Chocolate Lovers Frozen Yogurt"
			item.Subtitle = "Quisque vivamus bibendum cursus dictum dictumst dis aliquam aliquet etiam lectus eleifend fusce libero ante facilisi ligula est"
			item.SetImage(_baseUri, "SampleData/Images/60SauceChocolate.png")
			item.Link = "http://www.adventure-works.com/"
			item.Category = "Low-fat frozen yogurt"
			item.Description = "Enim cursus nascetur dictum habitasse hendrerit nec gravida vestibulum pellentesque vestibulum adipiscing iaculis erat consectetuer pellentesque parturient lacinia himenaeos pharetra condimentum non sollicitudin eros dolor vestibulum per lectus pellentesque nibh imperdiet laoreet consectetuer placerat libero malesuada pellentesque fames penatibus ligula scelerisque litora nisi luctus vestibulum nisl ullamcorper sed sem natoque suspendisse felis sit condimentum pulvinar nunc posuere magnis vel scelerisque sagittis porttitor potenti tincidunt mattis ipsum adipiscing sollicitudin parturient mauris nam senectus ullamcorper mollis tristique sociosqu suspendisse ultricies montes sed condimentum dis nostra suscipit justo ornare pretium odio pellentesque lacus lorem torquent orci"
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)

			item = New Item()
			item.Title = "Roma Strawberry"
			item.Subtitle = "Litora luctus magnis arcu lorem morbi blandit faucibus mattis commodo hac habitant inceptos conubia cubilia nulla mauris diam proin augue eget dolor mollis interdum lobortis"
			item.SetImage(_baseUri, "SampleData/Images/60Strawberry.png")
			item.Link = "http://www.adventure-works.com/"
			item.Category = "Gelato"
			item.Description = "Vestibulum vestibulum magna scelerisque ultrices consectetuer vehicula rhoncus pellentesque massa adipiscing platea primis sodales parturient metus sollicitudin morbi vestibulum pellentesque consectetuer pellentesque volutpat rutrum sollicitudin sapien pellentesque vestibulum venenatis consectetuer viverra est aliquam semper hac maecenas integer adipiscing sociis vulputate ullamcorper curabitur pellentesque parturient praesent neque sollicitudin pellentesque vestibulum suspendisse consectetuer leo quisque phasellus pede vestibulum quam pellentesque sollicitudin quis mus adipiscing parturient pellentesque vestibulum"
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)

			item = New Item()
			item.Title = "Italian Rainbow"
			item.Subtitle = "Leo mus nec nascetur dapibus non fames per felis ipsum pharetra egestas montes elit nostra placerat euismod enim justo ornare feugiat platea pulvinar sed sagittis"
			item.SetImage(_baseUri, "SampleData/Images/60SprinklesRainbow.png")
			item.Link = "http://www.alpineskihouse.com/"
			item.Category = "Gelato"
			item.Description = "Consequat condimentum consectetuer vivamus urna vestibulum netus pellentesque cras nec taciti non scelerisque adipiscing parturient tellus sollicitudin per vestibulum pellentesque aliquam convallis ullamcorper nulla porta aliquet accumsan suspendisse duis bibendum nunc condimentum consectetuer pellentesque scelerisque tempor sed dictumst eleifend amet vestibulum sem tempus facilisi ullamcorper adipiscing tortor ante purus parturient sit dignissim vel nam turpis sed sollicitudin elementum arcu vestibulum risus blandit suspendisse faucibus pellentesque commodo dis condimentum consectetuer varius aenean conubia cubilia facilisis velit mauris nullam aptent dapibus habitant"
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)

			item = New Item()
			item.Title = "Strawberry"
			item.Subtitle = "Ultrices rutrum sapien vehicula semper lorem volutpat sociis sit maecenas praesent taciti magna nunc odio orci vel tellus nam sed accumsan iaculis dis est"
			item.SetImage(_baseUri, "SampleData/Images/60Strawberry.png")
			item.Link = "http://www.blueyonderairlines.com/"
			item.Category = "Ice Cream"
			item.Description = "Consectetuer lacinia vestibulum tristique sit adipiscing laoreet fusce nibh suspendisse natoque placerat pulvinar ultricies condimentum scelerisque nisi ullamcorper nisl parturient vel suspendisse nam venenatis nunc lorem sed dis sagittis pellentesque luctus sollicitudin morbi posuere vestibulum potenti magnis pellentesque vulputate mattis mauris mollis consectetuer pellentesque pretium montes vestibulum condimentum nulla adipiscing sollicitudin scelerisque ullamcorper pellentesque odio orci rhoncus pede sodales suspendisse parturient viverra curabitur proin aliquam integer augue quam condimentum quisque senectus quis urna scelerisque nostra phasellus ullamcorper cras duis suspendisse sociosqu dolor vestibulum condimentum consectetuer vivamus est fames felis suscipit hac"
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)

			item = New Item()
			item.Title = "Strawberry Frozen Yogurt"
			item.Subtitle = "Maecenas class nam praesent cras aenean mauris aliquam nullam aptent accumsan duis nunc curae donec integer auctor sed congue amet"
			item.SetImage(_baseUri, "SampleData/Images/60Strawberry.png")
			item.Link = "http://www.adatum.com/"
			item.Category = "Low-fat frozen yogurt"
			item.Description = "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat"
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)

			item = New Item()
			item.Title = "Bongo Banana"
			item.Subtitle = "Quisque vivamus bibendum cursus dictum dictumst dis aliquam aliquet etiam lectus eleifend fusce libero ante facilisi ligula est"
			item.SetImage(_baseUri, "SampleData/Images/60Banana.png")
			item.Link = "http://www.adventure-works.com/"
			item.Category = "Sorbet"
			item.Description = "Enim cursus nascetur dictum habitasse hendrerit nec gravida vestibulum pellentesque vestibulum adipiscing iaculis erat consectetuer pellentesque parturient lacinia himenaeos pharetra condimentum non sollicitudin eros dolor vestibulum per lectus pellentesque nibh imperdiet laoreet consectetuer placerat libero malesuada pellentesque fames penatibus ligula scelerisque litora nisi luctus vestibulum nisl ullamcorper sed sem natoque suspendisse felis sit condimentum pulvinar nunc posuere magnis vel scelerisque sagittis porttitor potenti tincidunt mattis ipsum adipiscing sollicitudin parturient mauris nam senectus ullamcorper mollis tristique sociosqu suspendisse ultricies montes sed condimentum dis nostra suscipit justo ornare pretium odio pellentesque lacus lorem torquent orci"
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)

			item = New Item()
			item.Title = "Firenze Vanilla"
			item.Subtitle = "Litora luctus magnis arcu lorem morbi blandit faucibus mattis commodo hac habitant inceptos conubia cubilia nulla mauris diam proin augue eget dolor mollis interdum lobortis"
			item.SetImage(_baseUri, "SampleData/Images/60Vanilla.png")
			item.Link = "http://www.adventure-works.com/"
			item.Category = "Gelato"
			item.Description = "Vestibulum vestibulum magna scelerisque ultrices consectetuer vehicula rhoncus pellentesque massa adipiscing platea primis sodales parturient metus sollicitudin morbi vestibulum pellentesque consectetuer pellentesque volutpat rutrum sollicitudin sapien pellentesque vestibulum venenatis consectetuer viverra est aliquam semper hac maecenas integer adipiscing sociis vulputate ullamcorper curabitur pellentesque parturient praesent neque sollicitudin pellentesque vestibulum suspendisse consectetuer leo quisque phasellus pede vestibulum quam pellentesque sollicitudin quis mus adipiscing parturient pellentesque vestibulum"
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)

			item = New Item()
			item.Title = "Choco-wocko"
			item.Subtitle = "Leo mus nec nascetur dapibus non fames per felis ipsum pharetra egestas montes elit nostra placerat euismod enim justo ornare feugiat platea pulvinar sed sagittis"
			item.SetImage(_baseUri, "SampleData/Images/60SauceChocolate.png")
			item.Link = "http://www.alpineskihouse.com/"
			item.Category = "Sorbet"
			item.Description = "Consequat condimentum consectetuer vivamus urna vestibulum netus pellentesque cras nec taciti non scelerisque adipiscing parturient tellus sollicitudin per vestibulum pellentesque aliquam convallis ullamcorper nulla porta aliquet accumsan suspendisse duis bibendum nunc condimentum consectetuer pellentesque scelerisque tempor sed dictumst eleifend amet vestibulum sem tempus facilisi ullamcorper adipiscing tortor ante purus parturient sit dignissim vel nam turpis sed sollicitudin elementum arcu vestibulum risus blandit suspendisse faucibus pellentesque commodo dis condimentum consectetuer varius aenean conubia cubilia facilisis velit mauris nullam aptent dapibus habitant"
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)

			item = New Item()
			item.Title = "Chocolate"
			item.Subtitle = "Ultrices rutrum sapien vehicula semper lorem volutpat sociis sit maecenas praesent taciti magna nunc odio orci vel tellus nam sed accumsan iaculis dis est"
			item.SetImage(_baseUri, "SampleData/Images/60SauceChocolate.png")
			item.Link = "http://www.blueyonderairlines.com/"
			item.Category = "Ice Cream"
			item.Description = "Consectetuer lacinia vestibulum tristique sit adipiscing laoreet fusce nibh suspendisse natoque placerat pulvinar ultricies condimentum scelerisque nisi ullamcorper nisl parturient vel suspendisse nam venenatis nunc lorem sed dis sagittis pellentesque luctus sollicitudin morbi posuere vestibulum potenti magnis pellentesque vulputate mattis mauris mollis consectetuer pellentesque pretium montes vestibulum condimentum nulla adipiscing sollicitudin scelerisque ullamcorper pellentesque odio orci rhoncus pede sodales suspendisse parturient viverra curabitur proin aliquam integer augue quam condimentum quisque senectus quis urna scelerisque nostra phasellus ullamcorper cras duis suspendisse sociosqu dolor vestibulum condimentum consectetuer vivamus est fames felis suscipit hac"
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)
		End Sub



		Private _Collection As New ItemCollection()

		Public ReadOnly Property Collection() As ItemCollection
			Get
				Return Me._Collection
			End Get
		End Property

		Friend Function GetGroupsByCategory() As List(Of GroupInfoList(Of Object))
			Dim groups As New List(Of GroupInfoList(Of Object))()

			Dim query = From item In Collection _
			            Order By(CType(item, Item)).Category _
			            Group item By GroupKey = CType(item, Item).Category Into g = Group _
			            Select New With {Key .GroupName = GroupKey, Key .Items = g}
			For Each g In query
				Dim info As New GroupInfoList(Of Object)()
				info.Key = g.GroupName
				For Each item In g.Items
					info.Add(item)
				Next item
				groups.Add(info)
			Next g

			Return groups

		End Function

		Friend Function GetGroupsByLetter() As List(Of GroupInfoList(Of Object))
			Dim groups As New List(Of GroupInfoList(Of Object))()

			Dim query = From item In Collection _
			            Order By(CType(item, Item)).Title _
			            Group item By GroupKey = CType(item, Item).Title.Chars(0) Into g = Group _
			            Select New With {Key .GroupName = GroupKey, Key .Items = g}
			For Each g In query
				Dim info As New GroupInfoList(Of Object)()
				info.Key = g.GroupName
				For Each item In g.Items
					info.Add(item)
				Next item
				groups.Add(info)
			Next g

			Return groups

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
			item.SetImage(_baseUri, "SampleData/Images/60Mail01.png")
			item.Content = "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum"
			Collection.Add(item)

			item = New Item()
			item.Title = "Check out this topping!"
			item.Subtitle = "David Alexander"
			item.SetImage(_baseUri, "SampleData/Images/60Mail01.png")
			item.Content = "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue"
			Collection.Add(item)

			item = New Item()
			item.Title = "Come to the Ice Cream Party"
			item.Subtitle = "Josh Bailey"
			item.SetImage(_baseUri, "SampleData/Images/60Mail01.png")
			item.Content = "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse"
			Collection.Add(item)

			item = New Item()
			item.Title = "How about gluten free?"
			item.Subtitle = "Chris Berry"
			item.SetImage(_baseUri, "SampleData/Images/60Mail01.png")
			item.Content = LONG_LOREM_IPSUM
			Collection.Add(item)

			item = New Item()
			item.Title = "Summer promotion - BYGO"
			item.Subtitle = "Sean Bentley"
			item.SetImage(_baseUri, "SampleData/Images/60Mail01.png")
			item.Content = "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat"
			Collection.Add(item)

			item = New Item()
			item.Title = "Awesome flavor combination"
			item.Subtitle = "Adrian Lannin"
			item.SetImage(_baseUri, "SampleData/Images/60Mail01.png")
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

	Public Class CommentData
		Public Sub New()
			Dim item As Item
			Dim _baseUri As New Uri("ms-appx:///")

			item = New Item()
			item.Title = "The Smoothest"
			item.Content = "I loved this ice cream. I thought it was maybe the smoothest ice cream that i have ever had!"
			Collection.Add(item)

			item = New Item()
			item.Title = "What a great flavor!"
			item.Content = "Although the texture was a bit lacking, this one has the best flavor I have tasted!"
			Collection.Add(item)

			item = New Item()
			item.Title = "Didn't like the 'choco bits"
			item.Content = "The little bits of chocolate just weren't working for me"
			Collection.Add(item)

			item = New Item()
			item.Title = "Loved the peanut butter"
			item.Content = "The peanut butter was the best part of this delicious snack"
			Collection.Add(item)

			item = New Item()
			item.Title = "Wish there was more sugar"
			item.Content = "This wasn't sweet enough for me. I will have to try your other flavors, but maybe this is too healthy for me"
			Collection.Add(item)

			item = New Item()
			item.Title = "Texture was perfect"
			item.Content = "This was the smoothest ice cream I have ever had"
			Collection.Add(item)

			item = New Item()
			item.Title = "Kept wishing there was more"
			item.Content = "When I got to the end of each carton I kept wishing there was more ice cream. It was delicious!"
			Collection.Add(item)

		End Sub
		Private _Collection As New ItemCollection()

		Public ReadOnly Property Collection() As ItemCollection
			Get
				Return Me._Collection
			End Get
		End Property
	End Class

	Public Class ToppingsData
		Public Sub New()
			Dim item As Item
			Dim _baseUri As New Uri("ms-appx:///")

			item = New Item()
			item.Title = "Caramel Sauce"
			item.Category = "Sauces"
			item.SetImage(_baseUri, "SampleData/Images/60SauceCaramel.png")
			Collection.Add(item)

			item = New Item()
			item.Title = "Chocolate Sauce"
			item.Category = "Sauces"
			item.SetImage(_baseUri, "SampleData/Images/60SauceChocolate.png")
			Collection.Add(item)

			item = New Item()
			item.Title = "Strawberry Sauce"
			item.Category = "Sauces"
			item.SetImage(_baseUri, "SampleData/Images/60SauceStrawberry.png")
			Collection.Add(item)

			item = New Item()
			item.Title = "Chocolate Sprinkles"
			item.Category = "Sprinkles"
			item.SetImage(_baseUri, "SampleData/Images/60SprinklesChocolate.png")
			Collection.Add(item)

			item = New Item()
			item.Title = "Rainbow Sprinkles"
			item.Category = "Sprinkles"
			item.SetImage(_baseUri, "SampleData/Images/60SprinklesRainbow.png")
			Collection.Add(item)

			item = New Item()
			item.Title = "Vanilla Sprinkles"
			item.Category = "Sprinkles"
			item.SetImage(_baseUri, "SampleData/Images/60SprinklesVanilla.png")
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
