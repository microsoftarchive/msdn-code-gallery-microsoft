' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports EmployeeTracker.Model
Imports EmployeeTracker.ViewModel.Helpers

Namespace EmployeeTracker.ViewModel

    ''' <summary>
    ''' Common functionality for ViewModels of an individual ContactDetail
    ''' </summary>
    Public MustInherit Class ContactDetailViewModel
        Inherits ViewModelBase
        ''' <summary>
        ''' Gets the values that can be assigned to the Usage property of this ViewModel
        ''' </summary>
        Public ReadOnly Property ValidUsageValues() As IEnumerable(Of String)
            Get
                Return Me.Model.ValidUsageValues
            End Get
        End Property

        ''' <summary>
        ''' Gets the underlying ContactDetail this ViewModel is based on
        ''' </summary>
        Public MustOverride ReadOnly Property Model() As ContactDetail

        ''' <summary>
        ''' Gets or sets how this detail should be used, i.e. Home/Business etc.
        ''' Possible values are available from the ValidUsageValues property
        ''' </summary>
        Public Property Usage() As String
            Get
                Return Me.Model.Usage
            End Get

            Set(ByVal value As String)
                Me.Model.Usage = value
                Me.OnPropertyChanged("Usage")
            End Set
        End Property

        ''' <summary>
        ''' Constructs a view model to represent the supplied ContactDetail
        ''' </summary>
        ''' <param name="detail">The detail to build a ViewModel for</param>
        ''' <returns>The constructed ViewModel, null if one can't be built</returns>
        Public Shared Function BuildViewModel(ByVal detail As ContactDetail) As ContactDetailViewModel
            If detail Is Nothing Then
                Throw New ArgumentNullException("detail")
            End If

            Dim e As Email = TryCast(detail, Email)
            If e IsNot Nothing Then
                Return New EmailViewModel(e)
            End If

            Dim p As Phone = TryCast(detail, Phone)
            If p IsNot Nothing Then
                Return New PhoneViewModel(p)
            End If

            Dim a As Address = TryCast(detail, Address)
            If a IsNot Nothing Then
                Return New AddressViewModel(a)
            End If

            Return Nothing
        End Function
    End Class
End Namespace
