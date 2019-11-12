' Copyright (c) Microsoft Corporation. All rights reserved.
''' <summary>
''' A single customer
''' </summary>
Public Class Customer

    ''' <summary>
    ''' Creates a new customer
    ''' </summary>
    ''' <param name="customerId">The ID that uniquely identifies the customer</param>
    ''' <param name="companyName">The name of this customer</param>
    ''' <param name="city">The city where this customer is located</param>
    ''' <param name="region">The region where this customer is located</param>
    ''' <param name="country">The country where this customer is located</param>
    Public Sub New(ByVal customerId As String, ByVal companyName As String, ByVal city As String, ByVal region As String, ByVal country As String)
        Initialize()
        customerIDValue = customerId
        companyNameValue = companyName
        cityValue = city
        regionValue = region
        countryValue = country
    End Sub

    Private Sub Initialize()
        ordersValue = New Orders()
    End Sub

    Private customerIDValue As String
    ''' <summary>
    ''' Identifier for the customer
    ''' </summary>
    Public Property CustomerID() As String
        Get
            Return customerIDValue
        End Get
        Set(ByVal value As String)
            customerIDValue = value
        End Set
    End Property

    Private companyNameValue As String
    ''' <summary>
    ''' The name of this customer
    ''' </summary>
    Public Property CompanyName() As String
        Get
            Return companyNameValue
        End Get
        Set(ByVal Value As String)
            companyNameValue = Value
        End Set
    End Property


    Private cityValue As String
    ''' <summary>
    ''' The city where this customer is located
    ''' </summary>
    Public Property City() As String
        Get
            Return cityValue
        End Get
        Set(ByVal Value As String)
            cityValue = Value
        End Set
    End Property

    Private regionValue As String
    ''' <summary>
    ''' The region where this customer is located
    ''' </summary>
    Public Property Region() As String
        Get
            Return regionValue
        End Get
        Set(ByVal Value As String)
            regionValue = Value
        End Set
    End Property

    Private countryValue As String
    ''' <summary>
    ''' The country where this customer is located
    ''' </summary>
    Public Property Country() As String
        Get
            Return countryValue
        End Get
        Set(ByVal Value As String)
            countryValue = Value
        End Set
    End Property

    Private WithEvents ordersValue As Orders
    ''' <summary>
    ''' The orders for this customer
    ''' </summary>
    Public Property Orders() As Orders
        Get
            Return ordersValue
        End Get
        Set(ByVal value As Orders)
            ordersValue = value
        End Set
    End Property

    Private Sub ordersValue_AddingNew(ByVal sender As Object, ByVal e As System.ComponentModel.AddingNewEventArgs) Handles ordersValue.AddingNew
        e.NewObject = New Order(999, Me.CustomerID, Date.Today, DateAdd(DateInterval.Day, 30, Date.Today))
    End Sub
End Class

''' <summary>
''' A collection of Customers
''' </summary>
Public Class Customers
    Inherits System.ComponentModel.BindingList(Of Customer)
End Class