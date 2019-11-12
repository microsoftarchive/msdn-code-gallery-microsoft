'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System
Imports Controls_FlipView.Data

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario3
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Shared Current As Scenario3

    Public Sub New()
        Me.InitializeComponent()
        Current = Me
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        ' Get our sample data
        Dim sampleData = New Controls_FlipView.Data.SampleDataSource()

        ' Construct the table of contents used for navigating the FlipView
        ' Create a StackPanel to host the TOC
        Dim sp As New StackPanel()
        sp.Orientation = Orientation.Vertical
        sp.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center

        ' Add the TOC title
        Dim tb As New TextBlock()
        tb.Text = "Table of Contents"
        tb.Style = TryCast(Me.Resources("TOCTitle"), Style)
        sp.Children.Add(tb)

        ' Create the TOC from the data
        ' Use buttons for each TOC entry using the Tag property
        ' to contain the index of the target
        Dim i As Integer = 0
        For Each item As SampleDataItem In sampleData.Items
            Dim b As New Button()
            b.Style = TryCast(Me.Resources("ButtonStyle1"), Style)
            b.Content = item.Title
            AddHandler b.Click, AddressOf TOCButton_Click
            b.Tag = (System.Threading.Interlocked.Increment(i)).ToString
            sp.Children.Add(b)
        Next

        ' Add the TOC to our data set
        sampleData.Items.Insert(0, sp)

        ' Use a template selector to style the TOC entry differently from the other data entries
        FlipView3.ItemTemplateSelector = New ItemSelector()
        FlipView3.ItemsSource = sampleData.Items
    End Sub

    Private Sub TOCButton_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            FlipView3.SelectedIndex = Convert.ToInt32(b.Tag)
        End If
    End Sub
End Class

Public NotInheritable Class ItemSelector
    Inherits DataTemplateSelector
    Protected Overrides Function SelectTemplateCore(item As Object, container As DependencyObject) As DataTemplate
        Dim dataItem As SampleDataItem = TryCast(item, SampleDataItem)
        Dim itemTemplate As DataTemplate = TryCast(Scenario3.Current.Resources("ImageTemplate"), DataTemplate)
        Dim tocTemplate As DataTemplate = TryCast(Scenario3.Current.Resources("TOCTemplate"), DataTemplate)

        If dataItem IsNot Nothing Then
            Return itemTemplate
        Else
            Return tocTemplate
        End If
    End Function
End Class
