' Copyright (c) Microsoft Corporation. All rights reserved.
Public Class MainForm


    Private Sub cmdConstructors_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdConstructors.Click

        ' A constructor is code that executes when
        ' a class is instanced. The constructor may or may not require
        ' parameters to be passed into the constructor.

        ' Here is the syntax for instantiating a class with a
        ' constructor in a single line. This is the recommended way to
        ' instantiate a class with a constructor:

        Dim cust As New CustomerWithConstructor("1101", "Dale", "Sleppy")

        ' Alternatively you can instantiate a class after creating the
        ' variable:

        Dim cust2 As CustomerWithConstructor
        cust2 = New CustomerWithConstructor("1101", "Dale", "Sleppy")

        ' You can overload the Sub New procedure in a class if you want.
        ' By doing this, you can create different versions of the
        ' constructor that can be called from different client code.
        ' For example, you may want a constructor that can be called
        ' by external code, in which case you would use Public Overloads Sub
        ' New(). Then you might want to add a separate constructor that can
        ' only be called from code within the class (for example, if you
        ' were implementing a Clone method). In that case you would use
        ' Private Overloads Sub New().
        '
        ' Another thing you can do with constructors in VB.NET is perform 
        ' access control for your class. For example, suppose you wanted to
        ' develop a class that is exposed to external code, but not creatable
        ' directly. You would create a public class and add a constructor to the 
        ' class that is declared Friend. 

    End Sub

    Private Sub cmdOverloads_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdOverloads.Click
        Dim myCust1 As Customer
        Dim myCust2 As Customer

        ' The purpose of the "Overloads" keyword is to allow
        ' you to create multiple procedures that have the same name but
        ' execute separate code.
        '
        ' Suppose that you want to create a way of looking up a customer
        ' from either a database or a collection. You also want to be able
        ' to look up the customer by either last name or the customer's ID
        ' number. 
        '
        ' You can create two functions with the same name by using
        ' the Overloads keyword. Each version of the function can have a different
        ' set of parameters.
        '
        ' When calling an overloaded procedure, you will see the different
        ' versions that are available through Intellisense. At the
        ' beginning of the Intellisense message you will see up and down
        ' arrows with an indicator to show how many overloaded versions of
        ' the procedure exist, and which one you are currently viewing.
        '
        ' You can see an example of the Intellisense for an overloaded
        ' procedure by changing the parameters below. Put your cursor after
        ' the number 1101 in the second GetCustomer call and hit the space
        ' bar.

        myCust1 = GetCustomer("Sleppy")
        myCust2 = GetCustomer(1101)

        ' Scroll down to the GetCustomer procedures below to see how to
        ' implement the Overloads keyword.

    End Sub

    Private Sub cmdParamProperties_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdParamProperties.Click

        Dim cust As New CustomerWithParameterizedProperty()

        ' To create a parameterized property, you add a parameter to the
        ' Property statement. See 
        ' CustomerWithParameterizedProperty.DefaultQuantity for more details.
        '
        ' The parameter can be of any type, but is normally an integer for
        ' use as an index.
        '
        ' You can see below that when you implement a parameterized property,
        ' you must specify the parameter even if the property procedure
        ' doesn't use the parameter, as in this case. We set up the property
        ' procedure to use the parameter as a multiplier to the customer's
        ' default quantity. If you pass a 5, it multiplies the customer's
        ' default quantity by five.
        '
        ' When setting the DefaultQuantity value, we must pass in the 
        ' multiplier parameter even though it is not used.


        cust.DefaultQuantity(0) = 100

        MessageBox.Show("The default quantity for this customer is " & cust.DefaultQuantity(3) & ".", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)


    End Sub

    Private Sub cmdPropertySyntax_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdPropertySyntax.Click

        ' The code below creates an instance of the 
        ' CustomerPropertySyntax class and retrieves the value
        ' from a read-only property, AccountNumber. Step into the
        ' code to see more details on property syntax.

        Dim myAccount As String
        Dim cust As New CustomerPropertySyntax("1101")

        myAccount = cust.AccountNumber

    End Sub

    Private Sub cmdSharedMembers_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdSharedMembers.Click

        ' There are two main types of shared members in a class.
        ' The first type are shared variables, and the second
        ' type are shared procedures.
        '
        ' Below we create two instances of the customer class
        ' and change the shared variable "CompanyName". When
        ' you run this code you should see a message that the
        ' company name is Tailspin Toys. This shows that
        ' changing the variable on any instance of the class
        ' actually changes its value for all instances.

        Dim cust1 As New CustomerWithSharedMembers()
        Dim cust2 As New CustomerWithSharedMembers()

        ' You can use the standard object syntax as follows:
        CustomerWithSharedMembers.CompanyName = "Wingtip Toys"
        CustomerWithSharedMembers.CompanyName = "Tailspin Toys"
        MessageBox.Show("The company name is " & CustomerWithSharedMembers.CompanyName & ".", Me.Text)

        ' The next type of shared member is the shared procedure.
        MessageBox.Show("The last order was placed on " & _
            CustomerWithSharedMembers.LastOrderDate & ".", Me.Text, MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Overloads Function GetCustomer(ByVal CustID As Integer) As Customer
        ' This is the function that is called if you pass in an integer.
        Dim cust As New Customer()

        ' Normally, you would use the CustID integer to search a
        ' database or collection for the customer's ID number.
        ' Here, we're just going to populate a Customer object with
        ' fake data.
        cust.AccountNumber = "1101"
        cust.FirstName = "Dale"
        cust.LastName = "Sleppy"

        Return cust
    End Function

    Private Overloads Function GetCustomer(ByVal CustLastName As String) As Customer
        ' This is the function that is called if you pass in a string.
        Dim cust As New Customer()
        ' Normally, you would use the CustLastName string to search a
        ' database or collection for the customer's last name.
        ' Here, we're just going to populate a Customer object with
        ' fake data.
        cust.AccountNumber = "1101"
        cust.FirstName = "Dale"
        cust.LastName = "Sleppy"

        Return cust

    End Function



    Private Sub exitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles exitToolStripMenuItem.Click
        Me.Close()
    End Sub
End Class
