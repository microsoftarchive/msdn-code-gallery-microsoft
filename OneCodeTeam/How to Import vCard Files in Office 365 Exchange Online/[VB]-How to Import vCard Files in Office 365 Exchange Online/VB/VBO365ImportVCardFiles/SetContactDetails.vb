'**************************** Module Header ******************************\
' Module Name:  SetContactDetails.vb
' Project:      VBO365ImportVCardFiles
' Copyright (c) Microsoft Corporation.
' 
' The vCard file format is supported by many email clients and email services. 
' Now Outlook Web App supports to import the single .CSV file only. In this 
' application, we will demonstrate how to import multiple vCard files in 
' Office 365 Exchange Online.
' The class includes the methods that are invoked in set contact properties.
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports Microsoft.Exchange.WebServices.Data

Namespace VBO365ImportVCardFiles
    Public NotInheritable Class SetContactDetails
        ''' <summary>
        ''' Set the address entry.
        ''' </summary>
        Private Sub New()
        End Sub
        Public Shared Sub SetAddress(ByVal key As ContactSchemaProperties,
                                     ByVal keyValue As String,
                                     ByVal addressEntry As PhysicalAddressEntry)
            Select Case key
                Case ContactSchemaProperties.BusinessAddressStreet,
                    ContactSchemaProperties.HomeAddressStreet,
                    ContactSchemaProperties.OtherAddressStreet
                    addressEntry.Street = keyValue
                Case ContactSchemaProperties.BusinessAddressCity,
                    ContactSchemaProperties.HomeAddressCity,
                    ContactSchemaProperties.OtherAddressCity
                    addressEntry.City = keyValue
                Case ContactSchemaProperties.BusinessAddressState,
                    ContactSchemaProperties.HomeAddressState,
                    ContactSchemaProperties.OtherAddressState
                    addressEntry.State = keyValue
                Case ContactSchemaProperties.BusinessAddressPostalCode,
                    ContactSchemaProperties.HomeAddressPostalCode,
                    ContactSchemaProperties.OtherAddressPostalCode
                    addressEntry.PostalCode = keyValue
                Case ContactSchemaProperties.BusinessAddressCountryOrRegion,
                    ContactSchemaProperties.HomeAddressCountryOrRegion,
                    ContactSchemaProperties.OtherAddressCountryOrRegion
                    addressEntry.CountryOrRegion = keyValue
                Case Else
            End Select
        End Sub
    End Class
End Namespace
