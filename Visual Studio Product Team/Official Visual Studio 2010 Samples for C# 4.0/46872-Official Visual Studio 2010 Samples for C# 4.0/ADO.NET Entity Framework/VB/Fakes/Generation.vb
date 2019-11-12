' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports EmployeeTracker.Model

Namespace EmployeeTracker.Fakes

    ''' <summary>
    ''' Helper methods to generate fake data
    ''' </summary>
    Public NotInheritable Class Generation
        ''' <summary>
        ''' Builds a FakeUnitOfWork with realistic seeded data
        ''' </summary>
        ''' <returns>Context seeded with data</returns>
        Private Sub New()
        End Sub
        Public Shared Function BuildFakeSession() As FakeEmployeeContext
            Dim sales As Department = New Department With {.DepartmentName = "Sales", .DepartmentCode = "SALES", .LastAudited = New DateTime(2009, 4, 1)}
            Dim corporate As Department = New Department With {.DepartmentName = "Corporate", .DepartmentCode = "CORP"}
            Dim purchasing As Department = New Department With {.DepartmentName = "Purchasing", .DepartmentCode = "PURCH"}
            Dim marketing As Department = New Department With {.DepartmentName = "Marketing", .DepartmentCode = "MARK", .LastAudited = New DateTime(2009, 1, 1)}

            Dim e1 As Employee = New Employee With {.Title = "Mr.", .FirstName = "Nancy", .LastName = "Davolio", .Position = "Sales Representative", .HireDate = New DateTime(1992, 5, 1), .BirthDate = New DateTime(1948, 12, 8), .Department = sales, .ContactDetails = New List(Of ContactDetail)()}
            Dim e2 As Employee = New Employee With {.Title = "Dr.", .FirstName = "Andrew", .LastName = "Fuller", .Position = "Vice President", .HireDate = New DateTime(1992, 8, 14), .BirthDate = New DateTime(1952, 2, 19), .Department = corporate, .ContactDetails = New List(Of ContactDetail)()}
            Dim e3 As Employee = New Employee With {.Title = "Ms.", .FirstName = "Janet", .LastName = "Leverling", .Position = "Sales Representative", .HireDate = New DateTime(1992, 4, 1), .BirthDate = New DateTime(1963, 8, 30), .Department = sales, .ContactDetails = New List(Of ContactDetail)()}
            Dim e4 As Employee = New Employee With {.Title = "Mrs.", .FirstName = "Margaret", .LastName = "Peacock", .Position = "Sales Representative", .HireDate = New DateTime(1993, 5, 3), .BirthDate = New DateTime(1937, 9, 19), .Department = sales, .ContactDetails = New List(Of ContactDetail)()}
            Dim e5 As Employee = New Employee With {.Title = "Mr.", .FirstName = "Steven", .LastName = "Buchanan", .Position = "Sales Manager", .HireDate = New DateTime(1993, 10, 17), .BirthDate = New DateTime(1955, 3, 4), .Department = sales, .ContactDetails = New List(Of ContactDetail)()}
            Dim e6 As Employee = New Employee With {.Title = "Mr.", .FirstName = "Michael", .LastName = "Suyama", .Position = "Sales Representative", .HireDate = New DateTime(1993, 10, 17), .BirthDate = New DateTime(1963, 7, 2), .Department = sales, .ContactDetails = New List(Of ContactDetail)()}
            Dim e7 As Employee = New Employee With {.Title = "Mr.", .FirstName = "Robert", .LastName = "King", .Position = "Sales Representative", .HireDate = New DateTime(1994, 1, 2), .BirthDate = New DateTime(1960, 5, 29), .Department = sales, .ContactDetails = New List(Of ContactDetail)()}
            Dim e8 As Employee = New Employee With {.Title = "Ms.", .FirstName = "Callahan", .LastName = "Laura", .Position = "Inside Sales Coordinator", .HireDate = New DateTime(1994, 3, 5), .BirthDate = New DateTime(1958, 1, 9), .Department = sales, .ContactDetails = New List(Of ContactDetail)()}
            Dim e9 As Employee = New Employee With {.Title = "Ms.", .FirstName = "Anne", .LastName = "Dodsworth", .Position = "Sales Representative", .HireDate = New DateTime(1994, 11, 15), .BirthDate = New DateTime(1996, 1, 27), .Department = sales, .ContactDetails = New List(Of ContactDetail)()}

            e1.ContactDetails.Add(New Address With {.Usage = "Home", .LineOne = "507 - 20th Ave. E.  Apt. 2A", .City = "Seattle", .State = "WA", .ZipCode = "98122", .Country = "USA"})
            e2.ContactDetails.Add(New Address With {.Usage = "Home", .LineOne = "908 W. Capital Wa", .City = "Tacoma", .State = "WA", .ZipCode = "98401", .Country = "USA"})
            e3.ContactDetails.Add(New Address With {.Usage = "Business", .LineOne = "722 Moss Bay Blvd.", .City = "Kirkland", .State = "WA", .ZipCode = "98033", .Country = "USA"})
            e4.ContactDetails.Add(New Address With {.Usage = "Business", .LineOne = "4110 Old Redmond Rd.", .City = "Redmond", .State = "WA", .ZipCode = "98052", .Country = "USA"})
            e5.ContactDetails.Add(New Address With {.Usage = "Business", .LineOne = "14 Garrett Hill", .City = "London", .ZipCode = "SW1 8JR", .Country = "UK"})
            e6.ContactDetails.Add(New Address With {.Usage = "Business", .LineOne = "Coventry House  Miner Rd.", .City = "London", .ZipCode = "EC2 7JR", .Country = "UK"})
            e7.ContactDetails.Add(New Address With {.Usage = "Mailing", .LineOne = "Edgeham Hollow  Winchester Way", .City = "London", .ZipCode = "RG1 9SP", .Country = "UK"})
            e8.ContactDetails.Add(New Address With {.Usage = "Mailing", .LineOne = "4726 - 11th Ave. N.E.", .City = "Seattle", .State = "WA", .ZipCode = "98105", .Country = "USA"})
            e9.ContactDetails.Add(New Address With {.Usage = "Mailing", .LineOne = "7 Houndstooth Rd.", .City = "London", .ZipCode = "WG2 7LT", .Country = "UK"})

            e1.ContactDetails.Add(New Phone With {.Usage = "Business", .Number = "(206) 555-9857", .Extension = "5467"})
            e1.ContactDetails.Add(New Phone With {.Usage = "Cell", .Number = "(71) 555-7773"})
            e2.ContactDetails.Add(New Phone With {.Usage = "Business", .Number = "(206) 555-9482", .Extension = "3457"})
            e2.ContactDetails.Add(New Phone With {.Usage = "Home", .Number = "(71) 555-5598"})
            e3.ContactDetails.Add(New Phone With {.Usage = "Business", .Number = "(206) 555-3412", .Extension = "3355"})
            e3.ContactDetails.Add(New Phone With {.Usage = "Home", .Number = "(206) 555-1189"})
            e4.ContactDetails.Add(New Phone With {.Usage = "Business", .Number = "(206) 555-8122", .Extension = "5176"})
            e4.ContactDetails.Add(New Phone With {.Usage = "Home", .Number = "(71) 555-4444"})
            e5.ContactDetails.Add(New Phone With {.Usage = "Business", .Number = "(71) 555-4848", .Extension = "3453"})
            e6.ContactDetails.Add(New Phone With {.Usage = "Cell", .Number = "(71) 555-7773"})
            e7.ContactDetails.Add(New Phone With {.Usage = "Cell", .Number = "(71) 555-5598"})
            e8.ContactDetails.Add(New Phone With {.Usage = "Cell", .Number = "(206) 555-1189"})
            e9.ContactDetails.Add(New Phone With {.Usage = "Cell", .Number = "(71) 555-4444"})

            e1.ContactDetails.Add(New Email With {.Usage = "Business", .Address = "someone@example.com"})
            e2.ContactDetails.Add(New Email With {.Usage = "Business", .Address = "someone@example.com"})
            e3.ContactDetails.Add(New Email With {.Usage = "Business", .Address = "someone@example.com"})
            e4.ContactDetails.Add(New Email With {.Usage = "Business", .Address = "someone@example.com"})
            e5.ContactDetails.Add(New Email With {.Usage = "Business", .Address = "someone@example.com"})
            e6.ContactDetails.Add(New Email With {.Usage = "Personal", .Address = "someone@example.com"})
            e7.ContactDetails.Add(New Email With {.Usage = "Personal", .Address = "someone@example.com"})
            e8.ContactDetails.Add(New Email With {.Usage = "Personal", .Address = "someone@example.com"})
            e9.ContactDetails.Add(New Email With {.Usage = "Personal", .Address = "someone@example.com"})

            e1.Manager = e2
            e3.Manager = e2
            e4.Manager = e2
            e5.Manager = e2
            e6.Manager = e5
            e7.Manager = e5
            e8.Manager = e2
            e9.Manager = e5

            Dim context As New FakeEmployeeContext(New List(Of Employee) From {e1, e2, e3, e4, e5, e6, e7, e8, e9}, New List(Of Department) From {sales, corporate, purchasing, marketing})
            Return context
        End Function
    End Class
End Namespace
