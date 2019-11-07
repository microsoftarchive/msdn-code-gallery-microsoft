Imports Windows.UI.Xaml.Controls
Imports ListViewInteraction.Expression.Blend.SampleData.SampleDataSource


Partial Public NotInheritable Class ItemViewer
    Inherits UserControl

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' This method visualizes the placeholder state of the data item. When 
    ''' showing a placehlder, we set the opacity of other elements to zero
    ''' so that stale data is not visible to the end user.  Note that we use
    ''' Grid's background color as the placeholder background.
    ''' </summary>
    ''' <param name="item"></param>
    Public Sub ShowPlaceholder(ByVal item As Item)
        _item = item
        titleTextBlock.Opacity = 0
        categoryTextBlock.Opacity = 0
        image.Opacity = 0
    End Sub

    ''' <summary>
    ''' Visualize the Title by updating the TextBlock for Title and setting Opacity
    ''' to 1.
    ''' </summary>
    Public Sub ShowTitle()
        titleTextBlock.Text = _item.Title
        titleTextBlock.Opacity = 1
    End Sub

    ''' <summary>
    ''' Visualize category information by updating the correct TextBlock and 
    ''' setting Opacity to 1.
    ''' </summary>
    Public Sub ShowCategory()
        categoryTextBlock.Text = _item.Category
        categoryTextBlock.Opacity = 1
    End Sub

    ''' <summary>
    ''' Visualize the Image associated with the data item by updating the Image 
    ''' object and setting Opacity to 1.
    ''' </summary>
    Public Sub ShowImage()
        image.Source = _item.Image
        image.Opacity = 1
    End Sub

    ''' <summary>
    ''' Drop all refrences to the data item
    ''' </summary>
    Public Sub ClearData()
        _item = Nothing
        titleTextBlock.ClearValue(TextBlock.TextProperty)
        categoryTextBlock.ClearValue(TextBlock.TextProperty)
        image.ClearValue(image.SourceProperty)
    End Sub

    Private _item As Item
End Class
