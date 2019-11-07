'**************************** Module Header ******************************\
' Module Name:  MainModule.vb
' Project:      VBOffice365ExportEmail
' Copyright (c) Microsoft Corporation.
' 
' In this sample, we will demonstrate how to export the emails form the office 
' 365.
' If we export the emails, we can read them offline. We can follow these steps
' to implement it:
' 1. Create a search directory to collect the emails;
' 2. Get the directory;
' 3. Export the emails;
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.Net
Imports System.Net.Security
Imports System.Security.Cryptography.X509Certificates
Imports Microsoft.Exchange.WebServices.Data
Imports System.Linq
Imports System.IO

Namespace VBOffice365ExportEmail
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

            Dim subjectString As String = InputSubjectString()

            ' Use the EmailMessageSchema.Subject to filter the emails.
            Dim filters As New Dictionary(Of PropertyDefinition, String)()
            filters(EmailMessageSchema.Subject) = subjectString

            Dim folderName As String = "Subject contains for export email"
            ' Delete the duplicate folder.
            DeleteFolder(service, WellKnownFolderName.SearchFolders, folderName)
            ' Create the search folder named "Subject contains" to get the emails that received in last 30 days
            CreateSearchFolder(service, filters, folderName)
            Console.WriteLine("Create the search folder.")
            Console.WriteLine()

            ' Get the search folder.
            Dim filter As SearchFilter = New SearchFilter.IsEqualTo(FolderSchema.DisplayName, folderName)
            Dim searchFolder As SearchFolder = TryCast(GetFolder(service, filter,
                                                WellKnownFolderName.SearchFolders), SearchFolder)
            Console.WriteLine("Get the specific search folder.")
            Console.WriteLine()

            ' Export the email messages to the application directory.
            Console.WriteLine("Begin to export the emails:")
            ExportEmailMessages(searchFolder, Environment.CurrentDirectory)
            Console.WriteLine()

            Console.WriteLine("Press any key to exit......")
            Console.ReadKey()
        End Sub

        ''' <summary>
        ''' This method creates and sets the search folder.
        ''' </summary>
        Private Shared Function CreateSearchFolder(ByVal service As ExchangeService,
         ByVal filters As Dictionary(Of PropertyDefinition, String), ByVal displayName As String) As SearchFolder
            If service Is Nothing Then
                Return Nothing
            End If

            Dim filterCollection As New SearchFilter.SearchFilterCollection(LogicalOperator.And)

            ' We only search the nearest 30 days emails.
            Dim startDate As Date = Date.Now.AddDays(-30)
            Dim endDate As Date = Date.Now
            Dim startDateFilter As SearchFilter =
                New SearchFilter.IsGreaterThanOrEqualTo(EmailMessageSchema.DateTimeCreated, startDate)
            Dim endDateFilter As SearchFilter =
                New SearchFilter.IsLessThanOrEqualTo(EmailMessageSchema.DateTimeCreated, endDate)
            filterCollection.Add(startDateFilter)
            filterCollection.Add(endDateFilter)

            Dim itemClassFilter As SearchFilter =
                New SearchFilter.IsEqualTo(EmailMessageSchema.ItemClass, "IPM.Note")
            filterCollection.Add(itemClassFilter)

            ' Set the other filters.
            If filters IsNot Nothing Then
                For Each [property] As PropertyDefinition In filters.Keys
                    Dim searchFilter As SearchFilter =
                        New SearchFilter.ContainsSubstring([property], filters([property]))
                    filterCollection.Add(searchFilter)
                Next [property]
            End If

            Dim folderId As New FolderId(WellKnownFolderName.Inbox)

            Dim isDuplicateFoler As Boolean = True
            Dim duplicateFilter As SearchFilter =
                New SearchFilter.IsEqualTo(FolderSchema.DisplayName, displayName)
            Dim searchFolder As SearchFolder =
                TryCast(GetFolder(service, duplicateFilter, WellKnownFolderName.SearchFolders), SearchFolder)

            ' If there isn't the specific search folder, we create a new one.
            If searchFolder Is Nothing Then
                searchFolder = New SearchFolder(service)
                isDuplicateFoler = False
            End If
            searchFolder.SearchParameters.RootFolderIds.Add(folderId)
            searchFolder.SearchParameters.Traversal = SearchFolderTraversal.Shallow
            searchFolder.SearchParameters.SearchFilter = filterCollection

            If isDuplicateFoler Then
                searchFolder.Update()
            Else
                searchFolder.DisplayName = displayName

                searchFolder.Save(WellKnownFolderName.SearchFolders)
            End If

            Return searchFolder
        End Function

        ''' <summary>
        ''' Export the eamil messges form the specific search folder.
        ''' </summary>
        Private Shared Sub ExportEmailMessages(ByVal searchFolder As SearchFolder, ByVal filePath As String)
            If searchFolder Is Nothing Then
                Return
            End If

            Dim invalidStings() As String = {"\", ",", ":", "*", "?", """", "<", ">", "|"}

            Dim itemPorpertySet As New PropertySet(BasePropertySet.FirstClassProperties,
                                                   EmailMessageSchema.MimeContent)

            Const pageSize As Int32 = 50
            Dim itemView As New ItemView(pageSize)

            Dim findResults As FindItemsResults(Of Item) = Nothing
            Do
                findResults = searchFolder.FindItems(itemView)

                For Each item As Item In findResults.Items
                    If TypeOf item Is EmailMessage Then
                        Dim email As EmailMessage = TryCast(item, EmailMessage)
                        email.Load(itemPorpertySet)

                        Dim content() As Byte = email.MimeContent.Content
                        Dim fileName As String = email.Subject

                        ' Replace all the invaild strings.
                        For Each str As String In invalidStings
                            fileName = fileName.Replace(str, "")
                        Next str

                        ' Export the emails to the .eml files.
                        fileName = Path.Combine(filePath, fileName & ".eml")
                        File.WriteAllBytes(fileName, content)
                        Console.WriteLine("Export the email:{0}", email.Subject)
                    End If
                Next item

                itemView.Offset += pageSize
            Loop While findResults.MoreAvailable
        End Sub

        Private Shared Function InputSubjectString() As String
            Console.WriteLine("Please input the string that the email's subject contains to filter the emails:")
            Dim subjectString As String = Console.ReadLine()

            If String.IsNullOrWhiteSpace(subjectString) Then
                Console.WriteLine("Please input the vaild strings")
                subjectString = InputSubjectString()
            End If

            Return subjectString
        End Function

        Private Shared Function GetFolder(ByVal service As ExchangeService, ByVal filter As SearchFilter,
                                          ByVal folder As WellKnownFolderName) As Folder
            If service Is Nothing Then
                Return Nothing
            End If

            Dim propertySet As New PropertySet(BasePropertySet.IdOnly)

            Dim folderView As New FolderView(5)
            folderView.PropertySet = propertySet

            Dim searchResults As FindFoldersResults = service.FindFolders(folder, filter, folderView)

            Return searchResults.FirstOrDefault()
        End Function

        Private Shared Sub DeleteFolder(ByVal service As ExchangeService,
                                        ByVal parentFolder As WellKnownFolderName, ByVal folderName As String)
            Dim searchFilter As SearchFilter = New SearchFilter.IsEqualTo(FolderSchema.DisplayName, folderName)

            Dim folder As Folder = GetFolder(service, searchFilter, parentFolder)

            If folder IsNot Nothing Then
                Console.WriteLine("Delete the folder '{0}'", folderName)
                folder.Delete(DeleteMode.HardDelete)
                Console.WriteLine()
            End If
        End Sub

        Private Shared Function AutodiscoverUrl(ByVal service As ExchangeService,
                                                ByVal user As UserInfo) As Boolean
            Dim isSuccess As Boolean = False

            Try
                Console.WriteLine("Connecting the Exchange Online......")
                service.AutodiscoverUrl(user.Account, AddressOf CallbackMethods.RedirectionUrlValidationCallback)
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
