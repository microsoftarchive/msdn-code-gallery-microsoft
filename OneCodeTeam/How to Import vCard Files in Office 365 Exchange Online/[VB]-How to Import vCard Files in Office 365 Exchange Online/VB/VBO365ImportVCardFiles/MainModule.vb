'**************************** Module Header ******************************\
' Module Name:  MainModule.vb
' Project:      VBO365ImportVCardFiles
' Copyright (c) Microsoft Corporation.
' 
' The vCard file format is supported by many email clients and email services. 
' Now Outlook Web App supports to import the single .CSV file only. In this 
' application, we will demonstrate how to import multiple vCard files in 
' Office 365 Exchange Online.
' 1. Get a single file or all the vCard files in the folder;
' 2. Read the contact information from the vCard file;
' 3. Create a new contact and set the properties;
' 4. Save the contact
' 5. Process 2-4 steps for all the vCard files.
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
Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports System.IO
Imports System.Text

Namespace VBO365ImportVCardFiles
    Friend Class MainModule
        Shared Sub Main(ByVal args() As String)
            ServicePointManager.ServerCertificateValidationCallback =
                AddressOf CallbackMethods.CertificateValidationCallBack
            Dim service As New ExchangeService(ExchangeVersion.Exchange2010_SP2)

            ' Get the information of the account.
            Dim user As New UserInfo()
            service.Credentials = New WebCredentials(user.Account, user.Pwd)

            ' Set the url of server.
            If Not AutodiscoverUrl(service, user) Then
                Return
            End If
            Console.WriteLine()

            Dim files As List(Of String) = GetFiles()
            Console.WriteLine()

            CreateContacts(service, files)
            Console.WriteLine()

            Console.WriteLine("Press any key to exit......")
            Console.ReadKey()
        End Sub


        ''' <summary>
        ''' Get the vCard file or all the vCard file in the folder
        ''' </summary>
        ''' <returns></returns>
        Private Shared Function GetFiles() As List(Of String)
            Console.WriteLine("Input the file/floder path:")

            Dim path As String = Console.ReadLine()
            Dim files As New List(Of String)()
            If Directory.Exists(path) Then
                files.AddRange(Directory.GetFiles(path, "*.vcf"))
                Console.WriteLine("Get files.")
            Else
                If File.Exists(path) AndAlso path.ToLower().EndsWith(".vcf") Then
                    files.Add(path)
                    Console.WriteLine("Get the file.")
                End If
            End If

            Return files
        End Function

        ''' <summary>
        ''' Read the contact information from vCard files and creat a new contact in Exchange Online.
        ''' </summary>
        Private Shared Sub CreateContacts(ByVal service As ExchangeService, ByVal files As List(Of String))
            For Each file As String In files
                Using reader As New StreamReader(file)
                    Dim line As String
                    ' Store the contact information
                    Dim contactInfo As New Dictionary(Of ContactSchemaProperties, String)()
                    Dim isSupport As Boolean = True
                    Do
                        line = reader.ReadLine()

                        If String.IsNullOrWhiteSpace(line) Then
                            Continue Do
                        End If

                        Dim firstColonIndex As Int32 = line.IndexOf(":")

                        If firstColonIndex < 0 Then
                            Continue Do
                        End If

                        Dim keyName As String = Nothing, keyValue As String = Nothing
                        ' Read the contact information
                        If line.StartsWith("N:") OrElse line.StartsWith("N;LANGUAGE") Then
                            keyName = "Names"
                            keyValue = line.Substring(firstColonIndex + 1)
                        Else
                            keyName = line.Substring(0, firstColonIndex)
                            keyValue = line.Substring(firstColonIndex + 1)
                        End If

                        If keyName.StartsWith("VERSION") AndAlso (Not keyValue.StartsWith("2.1")) Then
                            isSupport = False
                            Console.WriteLine("This application only supports VCard Version 2.1 files.")
                            Exit Do
                        End If

                        ImportContactDetail(contactInfo, keyName, keyValue, reader)

                    Loop While line IsNot Nothing

                    If isSupport Then
                        CreateContact(service, contactInfo)
                        Console.WriteLine("Import the contact {0}.", file)
                    End If
                End Using
            Next file
        End Sub

        ''' <summary>
        ''' Read the information and store it.
        ''' </summary>
        Private Shared Sub ImportContactDetail(
            ByVal contactInfo As Dictionary(Of ContactSchemaProperties, String),
            ByVal keyName As String, ByVal keyValue As String, ByVal reader As StreamReader)
            If keyName.StartsWith("Names") Then
                Dim names() As String = keyValue.Split(";"c)
                If names.Length >= 2 Then
                    contactInfo.Add(ContactSchemaProperties.Surname, names(0))
                    contactInfo.Add(ContactSchemaProperties.GivenName, names(1))
                End If
            ElseIf keyName.StartsWith("FN") Then
                contactInfo.Add(ContactSchemaProperties.DisplayName, keyValue)
            ElseIf keyName.StartsWith("ORG") Then
                Dim comDep() As String = keyValue.Split(";"c)

                contactInfo.Add(ContactSchemaProperties.CompanyName, comDep(0))
                contactInfo.Add(ContactSchemaProperties.Companies, comDep(0))
                contactInfo.Add(ContactSchemaProperties.Department, comDep(1))
            ElseIf keyName.StartsWith("TITLE") Then
                contactInfo.Add(ContactSchemaProperties.JobTitle, keyValue)
            ElseIf keyName.StartsWith("PHOTO") Then
                ImportContactDetails.ImportPhoto(contactInfo, keyName, reader)
            ElseIf keyName.StartsWith("TEL") Then
                ImportContactDetails.ImportTelephone(contactInfo, keyName, keyValue)
            ElseIf keyName.StartsWith("ADR") Then
                ImportContactDetails.ImportAddress(contactInfo, keyName, keyValue)
            ElseIf keyName.StartsWith("EMAIL") Then
                ImportContactDetails.ImportEmail(contactInfo, keyName, keyValue)
            End If
        End Sub

        ''' <summary>
        ''' Create a new contact and save it.
        ''' </summary>
        Private Shared Sub CreateContact(ByVal service As ExchangeService,
                    ByVal contactInfo As Dictionary(Of ContactSchemaProperties, String))
            Dim newContact As New Contact(service)
            Dim businessAddressEntry As PhysicalAddressEntry = Nothing
            Dim homeAddressEntry As PhysicalAddressEntry = Nothing
            Dim otherAddressEntry As PhysicalAddressEntry = Nothing

            For Each key As ContactSchemaProperties In contactInfo.Keys
                Select Case key
                    Case ContactSchemaProperties.Surname
                        newContact.Surname = contactInfo(key)
                    Case ContactSchemaProperties.GivenName
                        newContact.GivenName = contactInfo(key)
                    Case ContactSchemaProperties.DisplayName
                        newContact.DisplayName = contactInfo(key)
                    Case ContactSchemaProperties.JobTitle
                        newContact.JobTitle = contactInfo(key)
                    Case ContactSchemaProperties.Birthday
                        Dim birthday As Date
                        newContact.Birthday =
                            IIf(Date.TryParse(contactInfo(key), birthday), birthday, Nothing)
                    Case ContactSchemaProperties.CompanyName
                        newContact.CompanyName = contactInfo(key)
                    Case ContactSchemaProperties.Companies
                        Dim stringList As New StringList()
                        stringList.Add(contactInfo(key))
                        newContact.Companies = stringList
                    Case ContactSchemaProperties.Department
                        newContact.Department = contactInfo(key)
                    Case ContactSchemaProperties.EmailAddress1
                        newContact.EmailAddresses(EmailAddressKey.EmailAddress1) = contactInfo(key)
                    Case ContactSchemaProperties.EmailAddress2
                        newContact.EmailAddresses(EmailAddressKey.EmailAddress2) = contactInfo(key)
                    Case ContactSchemaProperties.EmailAddress3
                        newContact.EmailAddresses(EmailAddressKey.EmailAddress3) = contactInfo(key)
                    Case ContactSchemaProperties.BusinessAddressStreet,
                        ContactSchemaProperties.BusinessAddressCity,
                        ContactSchemaProperties.BusinessAddressState,
                        ContactSchemaProperties.BusinessAddressPostalCode,
                        ContactSchemaProperties.BusinessAddressCountryOrRegion
                        If businessAddressEntry Is Nothing Then
                            businessAddressEntry = New PhysicalAddressEntry()
                        End If

                        SetContactDetails.SetAddress(key, contactInfo(key), businessAddressEntry)
                    Case ContactSchemaProperties.HomeAddressStreet,
                        ContactSchemaProperties.HomeAddressCity,
                        ContactSchemaProperties.HomeAddressState,
                        ContactSchemaProperties.HomeAddressPostalCode,
                        ContactSchemaProperties.HomeAddressCountryOrRegion
                        If homeAddressEntry Is Nothing Then
                            homeAddressEntry = New PhysicalAddressEntry()
                        End If
                        SetContactDetails.SetAddress(key, contactInfo(key), homeAddressEntry)
                    Case ContactSchemaProperties.OtherAddressStreet,
                        ContactSchemaProperties.OtherAddressCity,
                        ContactSchemaProperties.OtherAddressState,
                        ContactSchemaProperties.OtherAddressPostalCode,
                        ContactSchemaProperties.OtherAddressCountryOrRegion
                        If otherAddressEntry Is Nothing Then
                            otherAddressEntry = New PhysicalAddressEntry()
                        End If
                        SetContactDetails.SetAddress(key, contactInfo(key), otherAddressEntry)
                    Case ContactSchemaProperties.BusinessPhone
                        newContact.PhoneNumbers(PhoneNumberKey.BusinessPhone) = contactInfo(key)
                    Case ContactSchemaProperties.BusinessPhone2
                        newContact.PhoneNumbers(PhoneNumberKey.BusinessPhone2) = contactInfo(key)
                    Case ContactSchemaProperties.HomePhone
                        newContact.PhoneNumbers(PhoneNumberKey.HomePhone) = contactInfo(key)
                    Case ContactSchemaProperties.HomePhone2
                        newContact.PhoneNumbers(PhoneNumberKey.HomePhone2) = contactInfo(key)
                    Case ContactSchemaProperties.MobilePhone
                        newContact.PhoneNumbers(PhoneNumberKey.MobilePhone) = contactInfo(key)
                    Case ContactSchemaProperties.Photo
                        Dim picture() As Byte = Convert.FromBase64String(contactInfo(key))
                        newContact.SetContactPicture(picture)
                    Case Else
                End Select
            Next key

            ' We'll add the addresses if they exist.
            If businessAddressEntry IsNot Nothing Then
                newContact.PhysicalAddresses(PhysicalAddressKey.Business) = businessAddressEntry
            End If
            If homeAddressEntry IsNot Nothing Then
                newContact.PhysicalAddresses(PhysicalAddressKey.Home) = homeAddressEntry
            End If
            If otherAddressEntry IsNot Nothing Then
                newContact.PhysicalAddresses(PhysicalAddressKey.Other) = otherAddressEntry
            End If

            newContact.FileAsMapping = FileAsMapping.GivenNameSpaceSurname

            newContact.Save(WellKnownFolderName.Contacts)
        End Sub

        Private Shared Function AutodiscoverUrl(
                        ByVal service As ExchangeService, ByVal user As UserInfo) As Boolean
            Dim isSuccess As Boolean = False

            Try
                Console.WriteLine("Connecting the Exchange Online......")
                service.AutodiscoverUrl(user.Account,
                                        AddressOf CallbackMethods.RedirectionUrlValidationCallback)
                Console.WriteLine()
                Console.WriteLine("It's success to connect the Exchange Online.")

                isSuccess = True
            Catch e As Exception
                Console.WriteLine("There's an error.")
                Console.WriteLine(e.Message)
            End Try

            Return isSuccess
        End Function
    End Class
End Namespace
