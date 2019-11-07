' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
' THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
' PARTICULAR PURPOSE.
'
' Copyright (c) Microsoft Corporation. All rights reserved

Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports Expression.Blend.SampleData.SampleDataSource

Partial Public NotInheritable Class ScenarioOutput2
    Inherits Page

    ' A pointer back to the main page which is used to gain access to the input and output frames and their content.
    Private rootPage As MainPage = Nothing
    Private storeData As StoreData = Nothing

    Public Sub New()
        InitializeComponent()

        storeData = New StoreData
        ItemGridView2.ItemsSource = storeData.Collection
    End Sub

#Region "Template-Related Code - Do not remove"
    Protected Overrides Sub OnNavigatedTo(ByVal e As NavigationEventArgs)
        ' Get a pointer to our main page.
        rootPage = TryCast(e.Parameter, MainPage)

        ' We want to be notified with the OutputFrame is loaded so we can get to the content.
        AddHandler rootPage.InputFrameLoaded, AddressOf rootPage_InputFrameLoaded
    End Sub

    Protected Overrides Sub OnNavigatedFrom(ByVal e As NavigationEventArgs)
        RemoveHandler rootPage.InputFrameLoaded, AddressOf rootPage_InputFrameLoaded
    End Sub
#End Region

#Region "Use this code if you need access to elements in the input frame - otherwise delete"
    Private Sub rootPage_InputFrameLoaded(ByVal sender As Object, ByVal e As Object)
        ' At this point, we know that the Input Frame has been loaded and we can go ahead
        ' and reference elements in the page contained in the Input Frame.

        ' Get a pointer to the content within the IntputFrame.
        Dim inputFrame As Page = CType(rootPage.InputFrame.Content, Page)

        ' Go find the elements that we need for this scenario
        ' ex:flipView1 = TryCast(outputFrame.FindName("FlipView1"), FlipView)
    End Sub
#End Region

    Private Sub Scenario2_ItemClickHandler(ByVal sender As Object, ByVal e As ItemClickEventArgs)
        Dim _item As Item = TryCast(e.ClickedItem, Item)
        rootPage.NotifyUser(String.Format("Clicked flavor of {0} is: {1}", _item.Category, _item.Title), NotifyType.StatusMessage)
    End Sub
End Class
