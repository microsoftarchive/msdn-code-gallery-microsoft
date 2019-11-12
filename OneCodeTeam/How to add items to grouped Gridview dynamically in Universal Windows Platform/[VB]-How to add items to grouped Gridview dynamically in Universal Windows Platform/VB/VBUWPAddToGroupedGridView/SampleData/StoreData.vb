'***************************** Module Header ******************************\
' Module Name:  StoreData.vb
' Project:      VBUWPAddToGroupedGridView
' Copyright (c) Microsoft Corporation.
'  
' This is the sample data which bind to the GridView
'   
'  
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
'  
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/


Imports System.Globalization

Public Class StoreData
    Public Shared ReadOnly BaseUri As New Uri("ms-appx:///")

    Private ReadOnly _collection As New ItemCollection()

    ''' <summary>
    ''' Initializes a new instance of the <see cref="StoreData"/> class.
    ''' </summary>
    Public Sub New()
        Dim item As Item

        item = New Item
        item.Title = "Banana Blast Frozen Yogurt"
        item.SetImage(BaseUri, "SampleData/Images/60Banana.png")
        item.Category = "Low-fat frozen yogurt"
        _collection.Add(item)

        item = New Item
        item.Title = "Lavish Lemon Ice"
        item.SetImage(BaseUri, "SampleData/Images/60Lemon.png")
        item.Category = "Sorbet"
        _collection.Add(item)

        item = New Item
        item.Title = "Marvelous Mint"
        item.SetImage(BaseUri, "SampleData/Images/60Mint.png")
        item.Category = "Gelato"
        _collection.Add(item)

        item = New Item
        item.Title = "Creamy Orange"
        item.SetImage(BaseUri, "SampleData/Images/60Orange.png")
        item.Category = "Sorbet"
        _collection.Add(item)

        item = New Item
        item.Title = "Succulent Strawberry"
        item.SetImage(BaseUri, "SampleData/Images/60Strawberry.png")
        item.Category = "Sorbet"
        _collection.Add(item)

        item = New Item
        item.Title = "Very Vanilla"
        item.SetImage(BaseUri, "SampleData/Images/60Vanilla.png")
        item.Category = "Ice Cream"
        _collection.Add(item)

        item = New Item
        item.Title = "Creamy Caramel Frozen Yogurt"
        item.SetImage(BaseUri, "SampleData/Images/60SauceCaramel.png")
        item.Category = "Low-fat frozen yogurt"
        _collection.Add(item)

        item = New Item
        item.Title = "Chocolate Lovers Frozen Yougurt"
        item.SetImage(BaseUri, "SampleData/Images/60SauceChocolate.png")
        item.Category = "Low-fat frozen yogurt"
        _collection.Add(item)

        item = New Item
        item.Title = "Roma Strawberry"
        item.SetImage(BaseUri, "SampleData/Images/60Strawberry.png")
        item.Category = "Gelato"
        _collection.Add(item)

        item = New Item
        item.Title = "Italian Rainbow"
        item.SetImage(BaseUri, "SampleData/Images/60SprinklesRainbow.png")
        item.Category = "Gelato"
        _collection.Add(item)

        item = New Item
        item.Title = "Straweberry"
        item.SetImage(BaseUri, "SampleData/Images/60Strawberry.png")
        item.Category = "Ice Cream"
        _collection.Add(item)

        item = New Item
        item.Title = "Strawberry Frozen Yogurt"
        item.SetImage(BaseUri, "SampleData/Images/60Strawberry.png")
        item.Category = "Low-fat frozen yogurt"
        _collection.Add(item)

        item = New Item
        item.Title = "Bongo Banana"
        item.SetImage(BaseUri, "SampleData/Images/60Banana.png")
        item.Category = "Sorbet"
        _collection.Add(item)

        item = New Item
        item.Title = "Firenze Vanilla"
        item.SetImage(BaseUri, "SampleData/Images/60Vanilla.png")
        item.Category = "Gelato"
        _collection.Add(item)

        item = New Item
        item.Title = "Choco-wocko"
        item.SetImage(BaseUri, "SampleData/Images/60SauceChocolate.png")
        item.Category = "Sorbet"
        _collection.Add(item)

        item = New Item
        item.Title = "Chocolate"
        item.SetImage(BaseUri, "SampleData/Images/60SauceChocolate.png")
        item.Category = "Ice Cream"
        _collection.Add(item)
    End Sub

    ''' <summary>
    ''' Gets the collection.
    ''' </summary>
    Public ReadOnly Property Collection() As ItemCollection
        Get
            Return _collection
        End Get
    End Property

    ''' <summary>
    ''' The method returns the list of groups, each containing a key and a list of items. 
    ''' The key of each group is the category of the item. 
    ''' </summary>
    ''' <returns>
    ''' The List of groups of items. 
    ''' </returns>
    Friend Function GetGroupsByCategory() As ObservableCollection(Of GroupInfoCollection(Of Item))
        Dim groups As New ObservableCollection(Of GroupInfoCollection(Of Item))()

        Dim query = From item In _collection
                        Order By (CType(item, Item)).Category
                        Group item By GroupKey = (CType(item, Item)).Category Into g = Group
                        Select New With {Key .GroupName = GroupKey, Key .Items = g}

        For Each g In query
            Dim info As New GroupInfoCollection(Of Item)
            info.Key = g.GroupName

            For Each item In g.Items
                info.Add(item)
            Next item

            groups.Add(info)
        Next g

        Return groups
    End Function

End Class
