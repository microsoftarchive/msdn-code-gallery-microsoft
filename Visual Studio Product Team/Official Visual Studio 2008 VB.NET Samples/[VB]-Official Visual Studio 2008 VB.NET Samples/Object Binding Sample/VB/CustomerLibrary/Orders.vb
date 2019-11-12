' Copyright (c) Microsoft Corporation. All rights reserved.
''' <summary>
''' A single order
''' </summary>
Public Class Order

    Public Sub New()
    End Sub

    ''' <summary>
    ''' Creates a new order
    ''' </summary>
    ''' <param name="orderid">The identifier for this order</param>
    ''' <param name="customerID">The customer who placed this order</param>
    ''' <param name="orderDate">The date the order was placed</param>
    ''' <param name="shippedDate">The date the order was shipped</param>
    Public Sub New(ByVal orderid As Integer, ByVal customerID As String, ByVal orderDate As DateTime, ByVal shippedDate As DateTime)
        orderIDValue = orderid
        customerIDValue = customerID
        orderDateValue = orderDate
        shippedDateValue = shippedDate
    End Sub

    Private orderIDValue As String
    ''' <summary>
    ''' Identifier for the order
    ''' </summary>
    Public ReadOnly Property OrderID() As String
        Get
            Return orderIDValue
        End Get
    End Property

    Private customerIDValue As String
    ''' <summary>
    ''' The customer who placed this order
    ''' </summary>
    Public Property CustomerID() As String
        Get
            Return customerIDValue
        End Get
        Set(ByVal Value As String)
            customerIDValue = Value
        End Set
    End Property

    Private orderDateValue As DateTime
    ''' <summary>
    ''' The date the order was placed
    ''' </summary>
    Public Property OrderDate() As DateTime
        Get
            Return orderDateValue
        End Get
        Set(ByVal Value As DateTime)
            orderDateValue = Value
        End Set
    End Property

    Private shippedDateValue As DateTime
    ''' <summary>
    ''' The date the order was shipped
    ''' </summary>
    Public Property ShippedDate() As DateTime
        Get
            Return shippedDateValue
        End Get
        Set(ByVal Value As DateTime)
            shippedDateValue = Value
        End Set
    End Property
End Class

''' <summary>
''' A collection of Orders
''' </summary>
Public Class Orders
    Inherits System.ComponentModel.BindingList(Of Order)
End Class