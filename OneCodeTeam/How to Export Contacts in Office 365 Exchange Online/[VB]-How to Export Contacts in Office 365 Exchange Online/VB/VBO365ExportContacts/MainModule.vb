'**************************** Module Header ******************************\
' Module Name:  MainModule.vb
' Project:      VBO365ExportContacts
' Copyright (c) Microsoft Corporation.
' 
' Outlook Web App (OWA) allows us to import multiple contacts in a very simple 
' way. However, it does not allow to export contacts. In this application, we 
' will demonstrate how to export contacts from Office 365 Exchange Online.
' 1. Get all the contacts from Office 365 Exchange Online.
' 2. Write the head title to the CSV file.
' 3. Write the contacts into the CSV file.
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

Namespace VBO365ExportContacts
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

            ExportContacts(service)
            Console.WriteLine()

            Console.WriteLine("Press any key to exit......")
            Console.ReadKey()
        End Sub

        ''' <summary>
        ''' Export all contacts into the CSV file from Office 365 Exchange Online.
        ''' </summary>
        Private Shared Sub ExportContacts(ByVal service As ExchangeService)
            ' Get the properties we need to write.
            Dim propertySet As New PropertySet()
            Dim schemaList As Dictionary(Of PropertyDefinitionBase, String) =
                ContactsHelper.GetSchemaList()
            propertySet.AddRange(schemaList.Keys)

            Dim results As List(Of Item) = GetItems(service, Nothing,
                                                    WellKnownFolderName.Contacts, propertySet)
            Dim folderPath As String = GetFolderPath()
            Dim filePath As String = Path.Combine(folderPath, "contacts.csv")

            Using writer As New StreamWriter(filePath)
                Dim firstCell As Boolean = True

                ' Write the head title
                For Each head As PropertyDefinitionBase In schemaList.Keys
                    If Not firstCell Then
                        writer.Write(",")
                    Else
                        firstCell = False
                    End If

                    writer.Write("""{0}""", schemaList(head))
                Next head
                writer.WriteLine()
                firstCell = True

                ' Write the contact.
                For Each item As Item In results
                    Dim contact As Contact = TryCast(item, Contact)

                    For Each proerty As PropertyDefinitionBase In schemaList.Keys
                        If Not firstCell Then
                            writer.Write(",")
                        Else
                            firstCell = False
                        End If

                        ContactsHelper.WriteContacts(writer, proerty, contact)
                    Next proerty

                    writer.WriteLine()
                    firstCell = True
                Next item
            End Using

            Console.WriteLine()
            Console.WriteLine("Export the contacts to the file:{0}", filePath)
        End Sub

        ''' <summary>
        ''' Ask the path of fodler that stores the CSV file. 
        ''' </summary>
        ''' <returns>return the folder path</returns>
        Private Shared Function GetFolderPath() As String
            Do
                Console.Write("Please input the floder path:")

                Dim path As String = Console.ReadLine()
                Dim files As New List(Of String)()
                If Directory.Exists(path) Then
                    Return path
                End If

                Console.WriteLine("The path is invaild.")
            Loop While True
            Return Nothing
        End Function

        Private Shared Function GetItems(ByVal service As ExchangeService,
                                         ByVal filter As SearchFilter,
                                         ByVal folder As WellKnownFolderName,
                                         ByVal propertySet As PropertySet) As List(Of Item)
            If service Is Nothing Then
                Return Nothing
            End If

            Dim items As New List(Of Item)()

            If propertySet Is Nothing Then
                propertySet = New PropertySet(BasePropertySet.IdOnly)
            End If

            Const pageSize As Int32 = 10
            Dim itemView As New ItemView(pageSize)
            itemView.PropertySet = propertySet

            Dim searchResults As FindItemsResults(Of Item) = Nothing
            Do
                searchResults = service.FindItems(folder, filter, itemView)
                items.AddRange(searchResults.Items)

                itemView.Offset += pageSize
            Loop While searchResults.MoreAvailable

            Return items
        End Function

        Private Shared Function AutodiscoverUrl(ByVal service As ExchangeService,
                                                ByVal user As UserInfo) As Boolean
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
