' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class CustomerPropertySyntax
    Private custAccount As String
    Private custFirstName As String
    Private custLastName As String

    Sub New(ByVal AccountNumber As String)

        ' This is the Constructor for this class.

        ' Normally, you would use the AccountNumber string to search a
        ' database or collection for the customer's account number.
        ' Here, we're just going to populate a Customer object with
        ' dummy data.
        '
        ' Because the account number is a ReadOnly property,
        ' we need to set the private custAccount variable
        ' directly rather than calling the property procedure.
        custAccount = "1101"
        Me.FirstName = "Carmen"
        Me.LastName = "Smith"

    End Sub

    ' The following line is the property declaration
    ' for the AccountNumber property. This line is
    ' referred to as the "property statement".
    Public ReadOnly Property AccountNumber() As String

        ' Most properties are read/write, which means they
        ' can be evaluated or changed by the calling code.
        ' When a property is read/write you have both a
        ' Get portion and a Set portion of code within
        ' the property code. The Get and Set portions of
        ' the property are referred to as the "property
        ' procedures".

        ' Because this property is flagged ReadOnly, there
        ' is only a Get property procedure with no matching 
        ' "Set" property procedure.

        ' Alternatively you can have a property that is
        ' write only by declaring the property "WriteOnly"
        ' instead of ReadOnly. In that case you would have
        ' a Set with no Get.

        ' Note: A property's data type and access are defined 
        ' in the Property statement, not in the Property 
        ' procedures. A property can have only one data type 
        ' and one accessibility. For example, you cannot define 
        ' a property to set a Decimal value, but get a Double 
        ' value. Similarly, you cannot define a Private Set and 
        ' a Public Get on the same property. To achieve this 
        ' functionality, you can define a Public ReadOnly 
        ' property and a Private setting method separate from 
        ' the property.

        Get
            Return custAccount
        End Get

    End Property

#Region "Standard Customer Properties"

    Public Property FirstName() As String
        Get
            Return custFirstName
        End Get
        Set(ByVal Value As String)
            custFirstName = Value
        End Set
    End Property

    Public Property LastName() As String
        Get
            Return custLastName
        End Get
        Set(ByVal Value As String)
            custLastName = Value
        End Set
    End Property
#End Region


End Class
