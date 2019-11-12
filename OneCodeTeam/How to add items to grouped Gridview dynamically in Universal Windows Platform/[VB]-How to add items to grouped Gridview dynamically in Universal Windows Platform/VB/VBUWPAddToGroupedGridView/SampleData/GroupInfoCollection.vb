'***************************** Module Header ******************************\
' Module Name:  GroupInfoCollection.vb
' Project:      VBUWPAddToGroupedGridView
' Copyright (c) Microsoft Corporation.
' 
' This is the grouped collection.
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

''' <summary>
''' The group info list.
''' </summary>
''' <typeparam name="T">
''' The type of the items in the list. 
''' </typeparam>
Public Class GroupInfoCollection(Of T)
    Inherits ObservableCollection(Of T)
    ''' <summary>
    ''' Gets or sets the key of the group.
    ''' </summary>
    Public Property Key() As String

    ''' <summary>
    ''' Gets the enumerator, enumerating the list of items.
    ''' </summary>
    ''' <returns>
    ''' Returns the enumerator, enumerating the list of items. 
    ''' </returns>
    Public Shadows Function GetEnumerator() As IEnumerator(Of T)
        Return MyBase.GetEnumerator()
    End Function
End Class