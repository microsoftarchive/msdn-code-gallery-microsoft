'**************************** Module Header ******************************\
' Module Name:  ContactSchemaProperties.vb
' Project:      VBO365ImportVCardFiles
' Copyright (c) Microsoft Corporation.
' 
' The vCard file format is supported by many email clients and email services. 
' Now Outlook Web App supports to import the single .CSV file only. In this 
' application, we will demonstrate how to import multiple vCard files in 
' Office 365 Exchange Online.
' The enum stores the ContactSchema property definitions.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/


Namespace VBO365ImportVCardFiles
    Public Enum ContactSchemaProperties
        Surname
        GivenName
        DisplayName
        CompanyName
        Companies
        Department
        JobTitle
        BusinessPhone
        BusinessPhone2
        HomePhone
        HomePhone2
        MobilePhone
        BusinessAddressStreet
        BusinessAddressCity
        BusinessAddressState
        BusinessAddressPostalCode
        BusinessAddressCountryOrRegion
        OfficeLocation
        HomeAddressStreet
        HomeAddressCity
        HomeAddressState
        HomeAddressPostalCode
        HomeAddressCountryOrRegion
        OtherAddressStreet
        OtherAddressCity
        OtherAddressState
        OtherAddressPostalCode
        OtherAddressCountryOrRegion
        EmailAddress1
        EmailAddress2
        EmailAddress3
        Photo
        Birthday
    End Enum

End Namespace
