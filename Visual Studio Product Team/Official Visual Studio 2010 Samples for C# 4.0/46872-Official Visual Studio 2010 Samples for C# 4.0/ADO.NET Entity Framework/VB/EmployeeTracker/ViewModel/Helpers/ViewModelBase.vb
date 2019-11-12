' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


Imports Microsoft.VisualBasic
Imports System.ComponentModel

Namespace EmployeeTracker.ViewModel.Helpers

    ''' <summary>
    ''' Abstract base to consolidate common functionality of all ViewModels
    ''' </summary>
    Public MustInherit Class ViewModelBase
        Implements INotifyPropertyChanged
        ''' <summary>
        ''' Raised when a property on this object has a new value
        ''' </summary>
        Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged

        ''' <summary>
        ''' Raises this ViewModels PropertyChanged event
        ''' </summary>
        ''' <param name="propertyName">Name of the property that has a new value</param>
        Protected Sub OnPropertyChanged(ByVal propertyName As String)
            Me.OnPropertyChanged(New PropertyChangedEventArgs(propertyName))
        End Sub

        ''' <summary>
        ''' Raises this ViewModels PropertyChanged event
        ''' </summary>
        ''' <param name="e">Arguments detailing the change</param>
        Protected Overridable Sub OnPropertyChanged(ByVal e As PropertyChangedEventArgs)
            Dim handler = Me.PropertyChangedEvent
            If handler IsNot Nothing Then
                handler(Me, e)
            End If
        End Sub
    End Class
End Namespace
