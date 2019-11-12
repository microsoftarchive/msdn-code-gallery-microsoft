'**************************** Module Header ******************************\
' Module Name:  ContactsHelper.vb
' Project:      VBO365ExportContacts
' Copyright (c) Microsoft Corporation.
' 
' Outlook Web App (OWA) allows us to import multiple contacts in a very simple 
' way. However, it does not allow to export contacts. In this application, we 
' will demonstrate how to export contacts from Office 365 Exchange Online.
' This file includes the helper methods of contact.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.IO
Imports Microsoft.Exchange.WebServices.Data

Namespace VBO365ExportContacts
    Public NotInheritable Class ContactsHelper
        ''' <summary>
        ''' Get the contact proerties that you want to write. 
        ''' </summary>
        Private Sub New()
        End Sub
        Public Shared Function GetSchemaList() As Dictionary(Of PropertyDefinitionBase, String)
            ' Key is the property definition, and the value is the column title of the CSV file.
            Dim schemaList As New Dictionary(Of PropertyDefinitionBase, String)()
            schemaList.Add(ContactSchema.Surname, "Last Name")
            schemaList.Add(ContactSchema.GivenName, "First Name")
            schemaList.Add(ContactSchema.CompanyName, "Company")
            schemaList.Add(ContactSchema.Department, "Department")
            schemaList.Add(ContactSchema.JobTitle, "Job Title")
            schemaList.Add(ContactSchema.BusinessPhone, "Business Phone")
            schemaList.Add(ContactSchema.HomePhone, "Home Phone")
            schemaList.Add(ContactSchema.MobilePhone, "Mobile Phone")
            schemaList.Add(ContactSchema.BusinessAddressStreet, "Business Street")
            schemaList.Add(ContactSchema.BusinessAddressCity, "Business City")
            schemaList.Add(ContactSchema.BusinessAddressState, "Business State")
            schemaList.Add(ContactSchema.BusinessAddressPostalCode, "Business Postal Code")
            schemaList.Add(ContactSchema.BusinessAddressCountryOrRegion, "Business Country/Region")
            schemaList.Add(ContactSchema.HomeAddressStreet, "Home Street")
            schemaList.Add(ContactSchema.HomeAddressCity, "Home City")
            schemaList.Add(ContactSchema.HomeAddressState, "Home State")
            schemaList.Add(ContactSchema.HomeAddressPostalCode, "Home Postal Code")
            schemaList.Add(ContactSchema.HomeAddressCountryOrRegion, "Home Country/Region")
            schemaList.Add(ContactSchema.EmailAddress1, "Email Address")

            Return schemaList
        End Function

        ''' <summary>
        ''' Write the contacts into the CSV file.
        ''' </summary>
        Public Shared Sub WriteContacts(ByVal writer As StreamWriter,
                                        ByVal proerty As PropertyDefinitionBase,
                                        ByVal contact As Contact)
            If proerty.Equals(ContactSchema.Surname) Then
                If Not String.IsNullOrWhiteSpace(contact.Surname) Then
                    writer.Write("""{0}""", contact.Surname)
                End If
            ElseIf proerty.Equals(ContactSchema.GivenName) Then
                If Not String.IsNullOrWhiteSpace(contact.GivenName) Then
                    writer.Write("""{0}""", contact.GivenName)
                End If
            ElseIf proerty.Equals(ContactSchema.CompanyName) Then
                If Not String.IsNullOrWhiteSpace(contact.CompanyName) Then
                    writer.Write("""{0}""", contact.CompanyName)
                End If
            ElseIf proerty.Equals(ContactSchema.Department) Then
                If Not String.IsNullOrWhiteSpace(contact.Department) Then
                    writer.Write("""{0}""", contact.Department)
                End If
            ElseIf proerty.Equals(ContactSchema.JobTitle) Then
                If Not String.IsNullOrWhiteSpace(contact.JobTitle) Then
                    writer.Write("""{0}""", contact.JobTitle)
                End If
            ElseIf proerty.Equals(ContactSchema.BusinessPhone) Then
                If contact.PhoneNumbers.Contains(PhoneNumberKey.BusinessPhone) Then
                    If Not String.IsNullOrWhiteSpace(
                        contact.PhoneNumbers(PhoneNumberKey.BusinessPhone)) Then
                        writer.Write("""{0}""", contact.PhoneNumbers(PhoneNumberKey.BusinessPhone))
                    End If
                End If
            ElseIf proerty.Equals(ContactSchema.HomePhone) Then
                If contact.PhoneNumbers.Contains(PhoneNumberKey.HomePhone) Then
                    If Not String.IsNullOrWhiteSpace(
                        contact.PhoneNumbers(PhoneNumberKey.HomePhone)) Then
                        writer.Write("""{0}""", contact.PhoneNumbers(PhoneNumberKey.HomePhone))
                    End If
                End If
            ElseIf proerty.Equals(ContactSchema.MobilePhone) Then
                If contact.PhoneNumbers.Contains(PhoneNumberKey.MobilePhone) Then
                    If Not String.IsNullOrWhiteSpace(
                        contact.PhoneNumbers(PhoneNumberKey.MobilePhone)) Then
                        writer.Write("""{0}""", contact.PhoneNumbers(PhoneNumberKey.MobilePhone))
                    End If
                End If
            ElseIf proerty.Equals(ContactSchema.BusinessAddressStreet) Then
                If contact.PhysicalAddresses.Contains(PhysicalAddressKey.Business) Then
                    If Not String.IsNullOrWhiteSpace(
                        contact.PhysicalAddresses(PhysicalAddressKey.Business).Street) Then
                        writer.Write("""{0}""",
                                     contact.PhysicalAddresses(PhysicalAddressKey.Business).Street)
                    End If
                End If
            ElseIf proerty.Equals(ContactSchema.BusinessAddressCity) Then
                If contact.PhysicalAddresses.Contains(PhysicalAddressKey.Business) Then
                    writer.Write("""{0}""",
                                 contact.PhysicalAddresses(PhysicalAddressKey.Business).City)
                End If
            ElseIf proerty.Equals(ContactSchema.BusinessAddressState) Then
                If contact.PhysicalAddresses.Contains(PhysicalAddressKey.Business) Then
                    writer.Write("""{0}""",
                                 contact.PhysicalAddresses(PhysicalAddressKey.Business).State)
                End If
            ElseIf proerty.Equals(ContactSchema.BusinessAddressPostalCode) Then
                If contact.PhysicalAddresses.Contains(PhysicalAddressKey.Business) Then
                    writer.Write("""{0}""",
                                 contact.PhysicalAddresses(PhysicalAddressKey.Business).PostalCode)
                End If
            ElseIf proerty.Equals(ContactSchema.BusinessAddressCountryOrRegion) Then
                If contact.PhysicalAddresses.Contains(PhysicalAddressKey.Business) Then
                    writer.Write("""{0}""",
                                 contact.PhysicalAddresses(PhysicalAddressKey.Business).CountryOrRegion)
                End If
            ElseIf proerty.Equals(ContactSchema.HomeAddressStreet) Then
                If contact.PhysicalAddresses.Contains(PhysicalAddressKey.Home) Then
                    writer.Write("""{0}""",
                                 contact.PhysicalAddresses(PhysicalAddressKey.Home).Street)
                End If
            ElseIf proerty.Equals(ContactSchema.HomeAddressCity) Then
                If contact.PhysicalAddresses.Contains(PhysicalAddressKey.Home) Then
                    writer.Write("""{0}""",
                                 contact.PhysicalAddresses(PhysicalAddressKey.Home).City)
                End If
            ElseIf proerty.Equals(ContactSchema.HomeAddressState) Then
                If contact.PhysicalAddresses.Contains(PhysicalAddressKey.Home) Then
                    writer.Write("""{0}""",
                                 contact.PhysicalAddresses(PhysicalAddressKey.Home).State)
                End If
            ElseIf proerty.Equals(ContactSchema.HomeAddressPostalCode) Then
                If contact.PhysicalAddresses.Contains(PhysicalAddressKey.Home) Then
                    writer.Write("""{0}""",
                                 contact.PhysicalAddresses(PhysicalAddressKey.Home).PostalCode)
                End If
            ElseIf proerty.Equals(ContactSchema.HomeAddressCountryOrRegion) Then
                If contact.PhysicalAddresses.Contains(PhysicalAddressKey.Home) Then
                    writer.Write("""{0}""",
                                 contact.PhysicalAddresses(PhysicalAddressKey.Home).CountryOrRegion)
                End If
            ElseIf proerty.Equals(ContactSchema.EmailAddress1) Then
                If contact.EmailAddresses.Contains(EmailAddressKey.EmailAddress1) Then
                    writer.Write("""{0}""",
                                 contact.EmailAddresses(EmailAddressKey.EmailAddress1).Address)
                End If
            End If
        End Sub

    End Class
End Namespace
