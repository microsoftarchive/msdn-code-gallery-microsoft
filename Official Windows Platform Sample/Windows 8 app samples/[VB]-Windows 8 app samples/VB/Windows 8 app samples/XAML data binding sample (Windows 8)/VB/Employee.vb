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
Imports System.ComponentModel


Public Class Employee
    Implements INotifyPropertyChanged

    'Implement INotifiyPropertyChanged interface to subscribe for property change notifications
    Private _name As String
    Private _organization As String

    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(value As String)
            _name = value
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs("Name"))

        End Set
    End Property

    Public Property Organization() As String
        Get
            Return _organization
        End Get
        Set(value As String)
            _organization = value
            RaisePropertyChanged("Organization")
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

    Public Sub RaisePropertyChanged(name As String)
        RaiseEvent PropertyChanged("Organization", New PropertyChangedEventArgs(name))
    End Sub
End Class
