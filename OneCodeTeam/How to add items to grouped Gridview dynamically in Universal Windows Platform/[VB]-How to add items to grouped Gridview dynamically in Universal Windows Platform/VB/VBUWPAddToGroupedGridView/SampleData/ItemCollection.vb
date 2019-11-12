'***************************** Module Header ******************************\
' Module Name:  ItemCollection.vb
' Project:      VBUWPAddToGroupedGridView
' Copyright (c) Microsoft Corporation.
'  
' This is the collection which stores item objects.
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

Public Class ItemCollection
    Implements IEnumerable(Of Item)
    Private ReadOnly _itemCollection As New System.Collections.ObjectModel.ObservableCollection(Of Item)()

    ''' <summary>
    ''' Gets an enumerator, enumerating the items in the collection.
    ''' </summary>
    ''' <returns>
    ''' Returns an <see cref="IEnumerator"/>, enumerating the items in the collection.
    ''' </returns>
    Public Function GetEnumerator() As IEnumerator(Of Item) Implements IEnumerable(Of Item).GetEnumerator
        Return _itemCollection.GetEnumerator()
    End Function

    ''' <summary>
    ''' Gets an un-typed enumerator, enumerating the items in the collection.
    ''' </summary>
    ''' <returns>
    ''' Returns an un-typed <see cref="IEnumerator"/>, enumerating the items in the collection.
    ''' </returns>
    Private Function System_Collections_IEnumerable_GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
        Return GetEnumerator()
    End Function

    ''' <summary>
    ''' Adds an item to the collection. 
    ''' </summary>
    ''' <param name="item">
    ''' The item to be added to the collection.
    ''' </param>
    Public Sub Add(item As Item)
        _itemCollection.Add(item)
    End Sub
End Class
