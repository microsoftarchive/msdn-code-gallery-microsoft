'***************************** Module Header ******************************\
' Module Name:  Item.vb
' Project:      VBUWPAddToGroupedGridView
' Copyright (c) Microsoft Corporation.
'  
' This is the sample data which stored in the collection
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

Public Class Item
    Implements System.ComponentModel.INotifyPropertyChanged

    Public Event PropertyChanged(sender As Object, e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

    Private _title As String = String.Empty
    Private _subtitle As String = String.Empty
    Private _image As ImageSource
    Private _link As String = String.Empty
    Private _category As String = String.Empty
    Private _description As String = String.Empty
    Private _content As String = String.Empty

    ''' <summary>
    ''' Gets or sets the title of the item.
    ''' </summary>
    Public Property Title() As String
        Get
            Return _title
        End Get

        Set(value As String)
            If _title <> value Then
                _title = value
                OnPropertyChanged("Title")
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the image showing the item.
    ''' </summary>
    Public Property Image() As ImageSource
        Get
            Return _image
        End Get

        Set(value As ImageSource)
            If _image IsNot value Then
                _image = value
                OnPropertyChanged("Image")
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the category of the item.
    ''' If used in the grouped grid view samples, the items will be grouped by this property.
    ''' </summary>
    Public Property Category() As String
        Get
            Return _category
        End Get

        Set(value As String)
            If _category <> value Then
                _category = value
                OnPropertyChanged("Category")
            End If
        End Set
    End Property

    ''' <summary>
    ''' The method loads an image into a <see cref="BitmapImage"/> 
    ''' and sets the <see cref="Image"/> property. 
    ''' </summary>
    ''' <param name="baseUri">
    ''' The base URI of the image to load.
    ''' </param>
    ''' <param name="path">
    ''' The path of the image to load.
    ''' </param>
    Public Sub SetImage(baseUri As Uri, path As String)
        Image = New BitmapImage(New Uri(baseUri, path))
    End Sub

    ''' <summary>
    ''' If handlers are subscribed to the event, the handlers will be invoked.
    ''' </summary>
    ''' <param name="propertyName">
    ''' The name of the property that changed.
    ''' </param>
    Protected Overridable Sub OnPropertyChanged(propertyName As String)
        RaiseEvent PropertyChanged(Me, New System.ComponentModel.PropertyChangedEventArgs(propertyName))
    End Sub
End Class
