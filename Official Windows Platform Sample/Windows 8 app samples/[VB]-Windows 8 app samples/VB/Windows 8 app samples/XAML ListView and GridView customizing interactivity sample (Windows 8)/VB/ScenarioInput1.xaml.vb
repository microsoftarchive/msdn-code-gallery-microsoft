' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports Expression.Blend.SampleData.SampleDataSource

Partial Public NotInheritable Class ScenarioInput1
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing
    Private ShoppingCart As TextBlock = Nothing
    Private ItemGridView As GridView = Nothing

    Public Sub New()
        InitializeComponent()
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Get a pointer to our main page
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        RemoveHandler rootPage.OutputFrameLoaded, AddressOf rootPage_OutputFrameLoaded
    End Sub
#End Region

#Region "Use this code if you need access to elements in the output frame - otherwise delete"
    Private Sub rootPage_OutputFrameLoaded(ByVal sender As Object, ByVal e As Object)
        ' At this point, we know that the Output Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Output Frame.

        ' Get a pointer to the content within the OutputFrame.
        Dim outputFrame As Page = CType(rootPage.OutputFrame.Content, Page)

        ShoppingCart = TryCast(outputFrame.FindName("ShoppingCart"), TextBlock)
        ItemGridView = TryCast(outputFrame.FindName("ItemGridView"), GridView)
    End Sub
#End Region

    Private Sub AddToCart_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
        rootPage.NotifyUser("", NotifyType.StatusMessage)
        ShoppingCart.Text = "Cart Contents: "
        Dim charsToTrim() As Char = {","c, " "c}

        If ItemGridView.SelectedItems.Count <> 0 Then
            For Each item  In ItemGridView.SelectedItems
                ShoppingCart.Text &= item.Title & ", "
            Next item
            ShoppingCart.Text = ShoppingCart.Text.TrimEnd(charsToTrim)
            ShoppingCart.Text &= " added to cart"
        Else
            rootPage.NotifyUser("Please select items to place in the cart", NotifyType.ErrorMessage)
        End If
    End Sub
End Class
