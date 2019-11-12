'**************************** Module Header ******************************\
' Module Name:  ImportContactDetails.vb
' Project:      VBO365ImportVCardFiles
' Copyright (c) Microsoft Corporation.
' 
' The vCard file format is supported by many email clients and email services. 
' Now Outlook Web App supports to import the single .CSV file only. In this 
' application, we will demonstrate how to import multiple vCard files in 
' Office 365 Exchange Online.
' The class includes the methods that are invoked in storing information.
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
Imports System.Text

Namespace VBO365ImportVCardFiles
    Public NotInheritable Class ImportContactDetails
        ''' <summary>
        ''' Store the Telephone information
        ''' </summary>
        Private Sub New()
        End Sub
        Public Shared Sub ImportTelephone(
                            ByVal contactInfo As Dictionary(Of ContactSchemaProperties, String),
                            ByVal keyName As String, ByVal keyValue As String)
            Dim telType As String = keyName.Split(";"c)(1)
            Select Case telType
                Case "WORK"
                    If Not contactInfo.ContainsKey(ContactSchemaProperties.BusinessPhone) Then
                        contactInfo.Add(ContactSchemaProperties.BusinessPhone, keyValue)
                    Else
                        contactInfo.Add(ContactSchemaProperties.BusinessPhone2, keyValue)
                    End If
                Case "HOME"
                    If Not contactInfo.ContainsKey(ContactSchemaProperties.HomePhone) Then
                        contactInfo.Add(ContactSchemaProperties.HomePhone, keyValue)
                    Else
                        contactInfo.Add(ContactSchemaProperties.HomePhone2, keyValue)
                    End If
                Case "CELL"
                    contactInfo.Add(ContactSchemaProperties.MobilePhone, keyValue)
                Case Else
            End Select
        End Sub

        ''' <summary>
        ''' Store the Address information
        ''' </summary>
        Public Shared Sub ImportAddress(
                            ByVal contactInfo As Dictionary(Of ContactSchemaProperties, String),
                            ByVal keyName As String, ByVal keyValue As String)
            Dim addrType As String = keyName.Split(";"c)(1)
            Select Case addrType
                Case "WORK"
                    Dim bussinessAdr() As String = keyValue.Split(";"c)
                    If bussinessAdr.Length >= 1 Then
                        contactInfo.Add(ContactSchemaProperties.OfficeLocation, bussinessAdr(1))
                    End If

                    Dim properties() As ContactSchemaProperties = {
                        ContactSchemaProperties.BusinessAddressStreet,
                        ContactSchemaProperties.BusinessAddressCity,
                        ContactSchemaProperties.BusinessAddressState,
                        ContactSchemaProperties.BusinessAddressPostalCode,
                        ContactSchemaProperties.BusinessAddressCountryOrRegion}

                    For i As Int32 = 2 To bussinessAdr.Length - 1
                        contactInfo.Add(properties(i - 2), bussinessAdr(i))
                    Next i
                Case "HOME"
                    Dim homeAdr() As String = keyValue.Split(";"c)

                    Dim properties() As ContactSchemaProperties = {
                        ContactSchemaProperties.HomeAddressStreet,
                        ContactSchemaProperties.HomeAddressCity,
                        ContactSchemaProperties.HomeAddressState,
                        ContactSchemaProperties.HomeAddressPostalCode,
                        ContactSchemaProperties.HomeAddressCountryOrRegion}

                    For i As Int32 = 2 To homeAdr.Length - 1
                        contactInfo.Add(properties(i - 2), homeAdr(i))
                    Next i
                Case "POSTAL"
                    Dim postalAdr() As String = keyValue.Split(";"c)

                    Dim properties() As ContactSchemaProperties = {
                        ContactSchemaProperties.OtherAddressStreet,
                        ContactSchemaProperties.OtherAddressCity,
                        ContactSchemaProperties.OtherAddressState,
                        ContactSchemaProperties.OtherAddressPostalCode,
                        ContactSchemaProperties.OtherAddressCountryOrRegion}

                    For i As Int32 = 2 To postalAdr.Length - 1
                        contactInfo.Add(properties(i - 2), postalAdr(i))
                    Next i
                Case Else
            End Select
        End Sub

        ''' <summary>
        ''' Store the Email information
        ''' </summary>
        Public Shared Sub ImportEmail(ByVal contactInfo As Dictionary(Of ContactSchemaProperties, String),
                                      ByVal keyName As String, ByVal keyValue As String)
            If Not contactInfo.ContainsKey(ContactSchemaProperties.EmailAddress1) Then
                contactInfo.Add(ContactSchemaProperties.EmailAddress1, keyValue)
            ElseIf Not contactInfo.ContainsKey(ContactSchemaProperties.EmailAddress2) Then
                contactInfo.Add(ContactSchemaProperties.EmailAddress2, keyValue)
            Else
                contactInfo.Add(ContactSchemaProperties.EmailAddress3, keyValue)
            End If
        End Sub

        ''' <summary>
        ''' Store the Photo Information
        ''' </summary>
        Public Shared Sub ImportPhoto(ByVal contactInfo As Dictionary(Of ContactSchemaProperties, String),
                                      ByVal keyName As String, ByVal reader As StreamReader)
            Dim builder As New StringBuilder()

            If Not keyName.Contains("ENCODING") Then
                Return
            Else
                Dim photoLine As String = reader.ReadLine()

                Do While Not String.IsNullOrWhiteSpace(photoLine)
                    builder.Append(photoLine.Trim())

                    photoLine = reader.ReadLine()
                Loop

                If builder.Length > 0 Then
                    contactInfo.Add(ContactSchemaProperties.Photo, builder.ToString())
                End If
            End If
        End Sub
    End Class
End Namespace
