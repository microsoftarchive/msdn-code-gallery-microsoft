' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System
Imports System.Globalization
Imports EmployeeTracker.Model
Imports EmployeeTracker.ViewModel.Helpers

Namespace EmployeeTracker.ViewModel

    ''' <summary>
    ''' ViewModel of an individual Employee without associations
    ''' EmployeeViewModel should be used if associations need to be displayed or edited
    ''' </summary>
    Public Class BasicEmployeeViewModel
        Inherits ViewModelBase
        ''' <summary>
        ''' Initializes a new instance of the BasicEmployeeViewModel class.
        ''' </summary>
        ''' <param name="employee">The underlying Employee this ViewModel is to be based on</param>
        Public Sub New(ByVal employee As Employee)
            If employee Is Nothing Then
                Throw New ArgumentNullException("employee")
            End If

            Me.Model = employee
        End Sub

        ''' <summary>
        ''' Gets the underlying Employee this ViewModel is based on
        ''' </summary>
        Private privateModel As Employee
        Public Property Model() As Employee
            Get
                Return privateModel
            End Get
            Private Set(ByVal value As Employee)
                privateModel = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the first name of this employee
        ''' </summary>
        Public Property FirstName() As String
            Get
                Return Me.Model.FirstName
            End Get

            Set(ByVal value As String)
                Me.Model.FirstName = value
                Me.OnPropertyChanged("FirstName")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the title of this employee
        ''' </summary>
        Public Property Title() As String
            Get
                Return Me.Model.Title
            End Get

            Set(ByVal value As String)
                Me.Model.Title = value
                Me.OnPropertyChanged("Title")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the last name of this employee
        ''' </summary>
        Public Property LastName() As String
            Get
                Return Me.Model.LastName
            End Get

            Set(ByVal value As String)
                Me.Model.LastName = value
                Me.OnPropertyChanged("LastName")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the position this employee holds in the company
        ''' </summary>
        Public Property Position() As String
            Get
                Return Me.Model.Position
            End Get

            Set(ByVal value As String)
                Me.Model.Position = value
                Me.OnPropertyChanged("Position")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets this employees date of birth
        ''' </summary>
        Public Property BirthDate() As DateTime
            Get
                Return Me.Model.BirthDate
            End Get

            Set(ByVal value As DateTime)
                Me.Model.BirthDate = value
                Me.OnPropertyChanged("BirthDate")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the date this employee was hired by the company
        ''' </summary>
        Public Property HireDate() As DateTime
            Get
                Return Me.Model.HireDate
            End Get

            Set(ByVal value As DateTime)
                Me.Model.HireDate = value
                Me.OnPropertyChanged("HireDate")
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets the date this employee left the company
        ''' </summary>
        Public Property TerminationDate() As DateTime?
            Get
                Return Me.Model.TerminationDate
            End Get

            Set(ByVal value? As DateTime)
                Me.Model.TerminationDate = value
                Me.OnPropertyChanged("TerminationDate")
            End Set
        End Property

        ''' <summary>
        ''' Gets the text to display when referring to this employee
        ''' </summary>
        Public ReadOnly Property DisplayName() As String
            Get
                Return String.Format(CultureInfo.InvariantCulture, "{0}, {1}", Me.Model.LastName, Me.Model.FirstName)
            End Get
        End Property

        ''' <summary>
        ''' Gets the text to display for a readonly version of this employees hire date
        ''' </summary>
        Public ReadOnly Property DisplayHireDate() As String
            Get
                Return Me.Model.HireDate.ToShortDateString()
            End Get
        End Property
    End Class
End Namespace
