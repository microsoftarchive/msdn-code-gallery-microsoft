'**************************** Module Header ******************************\
' Module Name:  MainModule.vb
' Project:      VBO365ExportGroup
' Copyright (c) Microsoft Corporation.
' 
' Sometimes we need to export the distribution groups and their members, but 
' Outlook Web App (OWA) doesn’t provide the function. In this application, we 
' will demonstrate how to export the Distribution Groups and their members.
' 1. We get the members of the root group.
' 2. We export all the mailbox in the group.
' 3. We can choose to process the following up steps recursively for the nested 
' groups.
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
Imports System.Text.RegularExpressions

Namespace VBO365ExportGroup
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

            Dim groupAddress As String = GetGroupAddress()
            Console.WriteLine()
            Dim filePath As String = GetFilePath()
            Console.WriteLine()
            Using writer As New StreamWriter(filePath)
                Console.WriteLine(groupAddress)
                writer.WriteLine("""{0}"",""{1}""", "DistributionGroupAddress", "MemberAddresss")

                ExportGroup(service, groupAddress, Nothing, True, writer)
            End Using
            Console.WriteLine()

            Console.WriteLine("Press any key to exit......")
            Console.ReadKey()
        End Sub

        ''' <summary>
        ''' We will export the members of the group.
        ''' </summary>
        Private Shared Sub ExportGroup(ByVal service As ExchangeService, ByVal groupAddress As String,
                                       ByVal pad As String, ByVal isRecursive As Boolean,
                                       ByVal writer As StreamWriter)
            Dim groupMembers As ExpandGroupResults = service.ExpandGroup(groupAddress)

            If String.IsNullOrEmpty(pad) Then
                pad = ""
            End If

            ' Add spaces for view
            pad &= "   "
            For Each member As EmailAddress In groupMembers
                ' If we need recursion, and the member is group, we will process the method recursively.
                If isRecursive And (member.MailboxType = MailboxType.ContactGroup OrElse
                                    member.MailboxType = MailboxType.PublicGroup) Then
                    Console.WriteLine(pad & "{0,-50}{1,-11}", member.Address, member.MailboxType)
                    ExportGroup(service, member, pad, isRecursive, writer)
                Else
                    Console.WriteLine(pad & "{0,-50}{1,-11}", member.Address, member.MailboxType)
                    writer.WriteLine("""{0}"",""{1}""", groupAddress, member.Address)
                End If
            Next member
        End Sub

        Private Shared Sub ExportGroup(ByVal service As ExchangeService,
                                       ByVal groupAddress As EmailAddress, ByVal pad As String,
                                       ByVal isRecursive As Boolean, ByVal writer As StreamWriter)
            If groupAddress.MailboxType = MailboxType.ContactGroup OrElse
                groupAddress.MailboxType = MailboxType.PublicGroup Then
                ExportGroup(service, groupAddress.Address, pad, isRecursive, writer)
            End If
        End Sub

        ''' <summary>
        ''' Check the file path 
        ''' </summary>
        Private Shared Function GetFilePath() As String
            Do
                Console.Write("Please input the file path:")

                Dim filePath As String = Console.ReadLine()
                Dim directoryPath As String = Path.GetDirectoryName(filePath)
                If Directory.Exists(directoryPath) Then
                    Return filePath
                End If

                Console.WriteLine("The path is invaild.")
            Loop While True
        End Function

        ''' <summary>
        ''' Check the email address
        ''' </summary>
        Private Shared Function GetGroupAddress() As String
            Dim pattern As String = "\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
            Dim regex As New Regex(pattern)
            Do
                Console.Write("Please input the Distribution Group Address:")

                Dim address As String = Console.ReadLine()
                If regex.IsMatch(address) Then
                    Return address
                End If

                Console.WriteLine("The Email address is invaild.")
            Loop While True
        End Function

        Private Shared Function AutodiscoverUrl(ByVal service As ExchangeService,
                                                ByVal user As UserInfo) As Boolean
            Dim isSuccess As Boolean = False

            Try
                Console.WriteLine("Connecting the Exchange Online......")
                service.AutodiscoverUrl(user.Account, AddressOf CallbackMethods.RedirectionUrlValidationCallback)
                Console.WriteLine()
                Console.WriteLine("Connected the Exchange Online successfully.")

                isSuccess = True
            Catch e As Exception
                Console.WriteLine("There's an error.")
                Console.WriteLine(e.Message)
            End Try

            Return isSuccess
        End Function
    End Class
End Namespace
