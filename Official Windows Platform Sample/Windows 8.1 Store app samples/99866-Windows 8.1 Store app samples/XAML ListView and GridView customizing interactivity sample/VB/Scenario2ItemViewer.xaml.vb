Imports ListViewInteraction.Expression.Blend.SampleData.SampleDataSource
Imports Windows.UI.Xaml.Controls

Partial Public NotInheritable Class Scenario2ItemViewer
    Inherits UserControl

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' This method shows the Title of the data item as well as a placholder
    ''' for the image. We set the opacity of other UIElements to zero
    ''' so that stale data is not visible to the end user.  Note that we use
    ''' Border's background color as the image placeholder.
    ''' </summary>
    ''' <param name="item"></param>
    Public Sub ShowTitle(ByVal item As Item)
        _item = item
        subtitleTextBlock.Opacity = 0
        Image.Opacity = 0

        titleTextBlock.Text = _item.Title
        titleTextBlock.Opacity = 1
    End Sub

    ''' <summary>
    ''' Visualize category information by updating the correct TextBlock and 
    ''' setting Opacity to 1.
    ''' </summary>
    Public Sub ShowSubtitle()
        subtitleTextBlock.Text = _item.Subtitle
        subtitleTextBlock.Opacity = 1
    End Sub

    ''' <summary>
    ''' Visualize the Image associated with the data item by updating the Image 
    ''' object and setting Opacity to 1.
    ''' </summary>
    Public Sub ShowImage()
        Image.Source = _item.Image
        Image.Opacity = 1
    End Sub

    ''' <summary>
    ''' Drop all refrences to the data item
    ''' </summary>
    Public Sub ClearData()
        _item = Nothing
        titleTextBlock.ClearValue(TextBlock.TextProperty)
        subtitleTextBlock.ClearValue(TextBlock.TextProperty)
        Image.ClearValue(Image.SourceProperty)
    End Sub

    Private _item As Item

End Class
