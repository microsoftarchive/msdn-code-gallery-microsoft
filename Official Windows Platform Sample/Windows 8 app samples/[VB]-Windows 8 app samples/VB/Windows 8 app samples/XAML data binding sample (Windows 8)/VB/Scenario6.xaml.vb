'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Partial Public NotInheritable Class Scenario6
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        Scenario6Reset(Nothing, Nothing)
    End Sub

    Private Sub Scenario6Reset(sender As Object, e As RoutedEventArgs)
        Dim teams As New Teams()

        Dim result = From t In teams
                       Group t By t.City Into g = Group
                       Order By City
                       Select New GroupInfo(Of String, Team)(City, g)

        groupInfoCVS.Source = result

    End Sub
End Class


Public Class GroupInfo(Of TKey, TItem) 'interface required to show grouped data
Implements IGrouping(Of TKey, TItem)

    Private Property _source As IEnumerable(Of TItem)
    Private _key As TKey

    'takes a LINQ IGrouping and converts it to a GroupInfo
    Public Sub New(key As TKey, Items As IEnumerable(Of TItem))
        _key = key
        _source = Items
    End Sub

    Private Function IEnumerable_GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return _source.GetEnumerator
    End Function

    Public Function GetEnumerator() As IEnumerator(Of TItem) Implements IEnumerable(Of TItem).GetEnumerator
        Return _source.Cast(Of TItem).GetEnumerator()
    End Function

    Public ReadOnly Property Key As TKey Implements IGrouping(Of TKey, TItem).Key
        Get
            Return _key
        End Get
    End Property

    Public ReadOnly Property Items As IEnumerable(Of TItem)
        Get
            Return __source
        End Get
    End Property
End Class