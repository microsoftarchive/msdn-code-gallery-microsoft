Imports ListViewInteraction.Expression.Blend.SampleData.SampleDataSource
Imports System
Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports System.Runtime.InteropServices.WindowsRuntime
Imports Windows.Foundation
Imports Windows.Foundation.Collections
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Controls.Primitives
Imports Windows.UI.Xaml.Data
Imports Windows.UI.Xaml.Input
Imports Windows.UI.Xaml.Media
Imports Windows.UI.Xaml.Navigation

' The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

Partial Public NotInheritable Class Scenario3ItemViewer
    Inherits UserControl

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' This method shows the Title of the data item. We set the opacity of other 
    ''' UIElements to zero so that stale data is not visible to the end user.
    ''' </summary>
    ''' <param name="item"></param>
    Public Sub ShowTitle(ByVal item As Item)
        _item = item
        contentTextBlock.Opacity = 0

        titleTextBlock.Text = _item.Title
        titleTextBlock.Opacity = 1
    End Sub

    ''' <summary>
    ''' Visualize category information by updating the correct TextBlock and 
    ''' setting Opacity to 1.
    ''' </summary>
    Public Sub ShowContent()
        contentTextBlock.Text = _item.Content
        contentTextBlock.Opacity = 1
    End Sub


    ''' <summary>
    ''' Drop all refrences to the data item
    ''' </summary>
    Public Sub ClearData()
        _item = Nothing
        titleTextBlock.ClearValue(TextBlock.TextProperty)
        contentTextBlock.ClearValue(TextBlock.TextProperty)
    End Sub

    Private _item As Item

End Class
