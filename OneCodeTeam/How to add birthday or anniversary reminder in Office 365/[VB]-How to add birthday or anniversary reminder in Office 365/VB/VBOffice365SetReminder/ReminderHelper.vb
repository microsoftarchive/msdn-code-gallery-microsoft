'**************************** Module Header ******************************\
' Module Name:  ReminderHelper.vb
' Project:      VBOffice365SetReminder
' Copyright (c) Microsoft Corporation.
' 
' In this sample, we will demonstrate how to add birthday or anniversary 
' reminder for the contacts.
' This file contains all the methods that set the reminders for the contacts.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/

Imports System.Linq
Imports System.Text
Imports Microsoft.Exchange.WebServices.Data
Imports System.IO
Imports System.Globalization
Imports System.Collections.ObjectModel

Namespace VBOffice365SetReminder
    Public NotInheritable Class ReminderHelper
        ''' <summary>
        ''' This method creates the birthday or anniversary reminders for the contacts.
        ''' </summary>
        Private Sub New()
        End Sub
        Public Shared Sub SetReminder(ByVal service As ExchangeService, ByVal contactList As List(Of Contact), ByVal timeZone As TimeZoneInfo)
            If service Is Nothing OrElse contactList Is Nothing Then
                Return
            End If

            For Each contact As Contact In contactList
                Dim properties As Collection(Of PropertyDefinitionBase) = contact.GetLoadedPropertyDefinitions()

                ' If the contact gets the PropertyDefinition of Birthday, the methode will create the 
                ' birthday appointment for the contact.
                If properties.Contains(ContactSchema.Birthday) Then
                    If CreateBirthdayAppointment(service, contact, timeZone) Then
                        Console.WriteLine("It's success to create the birthday appointment of " &
                                          contact.DisplayName)
                    Else
                        Console.WriteLine("It's failed to create the birthday appointment of " &
                                          contact.DisplayName)
                    End If
                End If

                ' If the contact gets the PropertyDefinition of WeddingAnniversary, the methode will 
                ' create the anniversary appointment for the contact.
                If properties.Contains(ContactSchema.WeddingAnniversary) Then
                    If CreateAnniversaryAppointment(service, contact, timeZone) Then
                        Console.WriteLine("It's success to create the anniversary appointment of " &
                                          contact.DisplayName)
                    Else
                        Console.WriteLine("It's failed to create the anniversary appointment of " &
                                          contact.DisplayName)
                    End If
                End If
            Next contact
        End Sub

        ''' <summary>
        ''' This method gets the contacts folder basing the folder path.
        ''' </summary>
        Public Shared Function GetContactsFolder(ByVal service As ExchangeService, ByVal path As String) As Folder
            If service Is Nothing OrElse path Is Nothing Then
                Return Nothing
            End If

            Dim contactsFolder As Folder = Nothing

            ' Searching the folder starts from the Contacts folder.
            Dim searchFilter As SearchFilter.RelationalFilter = New SearchFilter.IsEqualTo(
                                                                FolderSchema.DisplayName,"Contacts")
            contactsFolder = GetFolder(service, searchFilter)

            path = path.TrimStart("\"c).TrimEnd("\"c)

            If String.IsNullOrWhiteSpace(path) Then
                Return contactsFolder
            Else
                Dim pathList() As String = path.Split("\"c)
                Const pageSize As Int32 = 10

                For Each name As String In pathList
                    searchFilter = New SearchFilter.IsEqualTo(FolderSchema.DisplayName, name)

                    Dim folderView As New FolderView(pageSize)
                    Dim propertySet As New PropertySet(BasePropertySet.IdOnly)
                    folderView.PropertySet = propertySet

                    Dim folderResults As FindFoldersResults = Nothing
                    Do
                        folderResults = contactsFolder.FindFolders(searchFilter, folderView)
                        folderView.Offset += pageSize

                        ' If the folder we find is the part of the parth, we will set the folder
                        ' as parent folder and search the next node in it.
                        If folderResults.TotalCount = 1 Then
                            contactsFolder = folderResults.Folders(0)
                        End If

                    Loop While folderResults.MoreAvailable
                Next name
            End If

            Return contactsFolder
        End Function

        ''' <summary>
        ''' This method gets the contacts that have the Birthday property definition in the 
        ''' special folder.
        ''' </summary>
        Public Shared Function GetContactsByBirthday(ByVal contactsFolder As Folder,
                                                     ByVal name As String) As List(Of Contact)
            If contactsFolder Is Nothing Then
                Return Nothing
            End If

            Return GetContacts(contactsFolder, name, ContactSchema.Birthday)
        End Function

        ''' <summary>
        ''' This method gets the contacts that have the WeddingAnniversary property definition  
        ''' in the special folder.
        ''' </summary>
        Public Shared Function GetContactsByAnniversary(ByVal contactsFolder As Folder,
                                                        ByVal name As String) As List(Of Contact)
            If contactsFolder Is Nothing Then
                Return Nothing
            End If

            Return GetContacts(contactsFolder, name, ContactSchema.WeddingAnniversary)
        End Function

        ''' <summary>
        ''' This method gets the contacts that have the Birthday and WeddingAnniversary property   
        ''' definition in the special folder.
        ''' </summary>
        Public Shared Function GetContactsByBirthdayAndAnniversary(ByVal contactsFolder As Folder,
                                                                   ByVal name As String) As List(Of Contact)
            If contactsFolder Is Nothing Then
                Return Nothing
            End If

            Return GetContacts(contactsFolder, name, ContactSchema.Birthday, ContactSchema.WeddingAnniversary)
        End Function

        ''' <summary>
        ''' This method gets the contacts that have the special DisplayName and property  
        ''' definitions in the special folder.
        ''' </summary>
        Private Shared Function GetContacts(ByVal contactsFolder As Folder, ByVal name As String,
                                            ByVal ParamArray schemas() As PropertyDefinition) As List(Of Contact)
            If contactsFolder Is Nothing Then
                Return Nothing
            End If

            Dim contacts As New List(Of Contact)()

            Dim filters As New SearchFilter.SearchFilterCollection(LogicalOperator.And)

            If Not String.IsNullOrWhiteSpace(name) Then
                Dim searchFilter As SearchFilter =
                    New SearchFilter.ContainsSubstring(ContactSchema.DisplayName, name)
                filters.Add(searchFilter)
            End If

            If schemas IsNot Nothing Then
                For Each schema As PropertyDefinition In schemas
                    Dim searchFilter As SearchFilter = New SearchFilter.Exists(schema)
                    filters.Add(searchFilter)
                Next schema
            End If

            Const pageSize As Int32 = 10
            Dim itemView As New ItemView(pageSize)
            Dim propertySet As New PropertySet(BasePropertySet.IdOnly, schemas)
            propertySet.Add(ContactSchema.DisplayName)
            itemView.PropertySet = propertySet

            Dim findResults As FindItemsResults(Of Item) = Nothing
            Do
                findResults = contactsFolder.FindItems(filters, itemView)
                itemView.Offset += pageSize

                contacts.AddRange(findResults.Cast(Of Contact)())
            Loop While findResults.MoreAvailable


            Return contacts
        End Function

        ''' <summary>
        ''' Create the birthday appointment for the contact.
        ''' </summary>
        Private Shared Function CreateBirthdayAppointment(ByVal service As ExchangeService,
                                                          ByVal contact As Contact, ByVal timeZone As TimeZoneInfo) As Boolean
            If service Is Nothing OrElse contact Is Nothing Then
                Return False
            End If

            Dim subject As String = "It's the birthday of"
            Return CreateAppointment(service, contact, subject, timeZone, Function(c) c.Birthday)
        End Function

        ''' <summary>
        ''' Create the anniversary appointment for the contact.
        ''' </summary>
        Private Shared Function CreateAnniversaryAppointment(ByVal service As ExchangeService,
                                                             ByVal contact As Contact, ByVal timeZone As TimeZoneInfo) As Boolean
            If service Is Nothing OrElse contact Is Nothing Then
                Return False
            End If

            Dim subject As String = "It's the anniversary of"
            Return CreateAppointment(service, contact, subject, timeZone, Function(c) c.WeddingAnniversary)
        End Function

        ''' <summary>
        ''' Create the appointment for the contact.
        ''' </summary>
        Private Shared Function CreateAppointment(ByVal service As ExchangeService, ByVal contact As Contact,
                                ByVal subject As String, ByVal timeZone As TimeZoneInfo, ByVal getDate As Func(Of Contact, Date?)) As Boolean
            If service Is Nothing OrElse contact Is Nothing OrElse getDate Is Nothing Then
                Return False
            End If

            Dim [date]? As Date = getDate(contact)

            If [date] Is Nothing Then
                Return False
            End If

            Dim appointmentSubject As String = subject & " " & contact.DisplayName

            ' Check if there's the duplicate appointment.
            If HaveDuplicateAppointment(service, appointmentSubject) Then
                Console.WriteLine("There's a duplicate appointment of " & contact.DisplayName)
                Return False
            End If

            Dim appointment As New Appointment(service)
            appointment.Subject = appointmentSubject
            appointment.LegacyFreeBusyStatus = LegacyFreeBusyStatus.Free
            appointment.IsAllDayEvent = True
            appointment.StartTimeZone = timeZone
            appointment.EndTimeZone = timeZone

            Dim recurrence As Recurrence = New Recurrence.YearlyPattern([date].Value,
                                        CType([date].Value.Month, Month), [date].Value.Day)
            appointment.Recurrence = recurrence

            appointment.Save(SendInvitationsMode.SendToNone)

            Return True
        End Function

        ''' <summary>
        ''' Import the contacts from the CSV file.
        ''' </summary>
        Public Shared Function ImportContactsFromCSV(ByVal service As ExchangeService,
                                                     ByVal filepath As String) As List(Of Contact)
            If service Is Nothing OrElse String.IsNullOrWhiteSpace(filepath) Then
                Return Nothing
            End If

            Dim contactsList As New List(Of Contact)()

            ' Get the DataTable that contains the value of the contacts.
            Dim contactsTable As New DataTable()
            ImportCSVFile(contactsTable, filepath)

            Dim properties = New With {
                Key .FirstName = "First Name",
                    Key .LastName = "Last Name",
                    Key .Anniversary = "Anniversary",
                    Key .Birthday = "Birthday"}

            For Each row As DataRow In contactsTable.Rows
                Dim contact As New Contact(service)

                If contactsTable.Columns.Contains(properties.FirstName) Then
                    contact.GivenName = row(properties.FirstName).ToString()
                    contact.DisplayName = contact.GivenName
                    contact.FileAs = contact.DisplayName
                End If

                If contactsTable.Columns.Contains(properties.LastName) Then
                    contact.Surname = row(properties.LastName).ToString()
                    If Not String.IsNullOrWhiteSpace(contact.GivenName) Then
                        contact.DisplayName = contact.GivenName & " " & contact.Surname
                    End If
                    contact.FileAs = contact.DisplayName
                End If

                Dim provider As New CultureInfo("en-US")
                Dim [date] As Date
                If contactsTable.Columns.Contains(properties.Anniversary) Then
                    contact.WeddingAnniversary = IIf(Date.TryParseExact(row(properties.Anniversary).ToString(),
                                        "d", provider, DateTimeStyles.None, [date]), CType([date], Date?), Nothing)
                End If

                If contactsTable.Columns.Contains(properties.Birthday) Then
                    contact.Birthday = IIf(Date.TryParseExact(row(properties.Birthday).ToString(),
                                        "d", provider, DateTimeStyles.None, [date]), CType([date], Date?), Nothing)
                End If

                contact.Save()
                contactsList.Add(contact)

                ' Load the properties that we can use when creating the appointments.
                Dim propertySet As New PropertySet(ContactSchema.DisplayName, ContactSchema.Birthday,
                                                   ContactSchema.WeddingAnniversary)
                contact.Load(propertySet)
            Next row

            Return contactsList
        End Function

        ''' <summary>
        ''' Import the CSV file into a DataTable.
        ''' </summary>
        Private Shared Sub ImportCSVFile(ByVal contactsTable As DataTable, ByVal filepath As String)
            If String.IsNullOrWhiteSpace(filepath) Then
                Return
            End If

            If contactsTable Is Nothing Then
                contactsTable = New DataTable()
            End If

            If File.Exists(filepath) Then
                Using reader As New StreamReader(filepath, Encoding.Unicode)
                    Dim strLine As String = Nothing
                    Dim isColumn As Boolean = True

                    strLine = reader.ReadLine()
                    Do While Not String.IsNullOrWhiteSpace(strLine)
                        Dim strings() As String = strLine.Replace("""", "").Split(","c)

                        If isColumn Then
                            isColumn = False

                            For Each str As String In strings
                                Dim columnName As String = str.TrimStart(""""c).TrimEnd(""""c)
                                Dim column As New DataColumn(columnName, GetType(String))

                                contactsTable.Columns.Add(column)
                            Next str
                        Else
                            Dim row As DataRow = contactsTable.NewRow()

                            For i As Integer = 0 To contactsTable.Columns.Count - 1
                                Dim rowValue As String = Nothing

                                If Not String.IsNullOrWhiteSpace(strings(i)) Then
                                    rowValue = strings(i).TrimStart(""""c).TrimEnd(""""c)
                                End If

                                row(i) = rowValue
                            Next i

                            contactsTable.Rows.Add(row)
                        End If
                        strLine = reader.ReadLine()
                    Loop
                End Using
            End If
        End Sub

        Private Shared Function GetFolder(ByVal service As ExchangeService,
                                          ByVal filter As SearchFilter.RelationalFilter) As Folder
            If service Is Nothing Then
                Return Nothing
            End If

            Dim propertySet As New PropertySet(BasePropertySet.IdOnly)

            Dim folderView As New FolderView(5)
            folderView.PropertySet = propertySet

            Dim searchResults As FindFoldersResults = service.FindFolders(WellKnownFolderName.MsgFolderRoot,
                                                                          filter, folderView)

            Return searchResults.FirstOrDefault()
        End Function

        ''' <summary>
        ''' Check if there's the duplicate appointment basing the Subject.
        ''' </summary>
        Private Shared Function HaveDuplicateAppointment(ByVal service As ExchangeService,
                                                         ByVal appointmentSubject As String) As Boolean
            If service Is Nothing OrElse String.IsNullOrWhiteSpace(appointmentSubject) Then
                Return True
            End If

            Dim searchFilter As SearchFilter.RelationalFilter =
                New SearchFilter.IsEqualTo(FolderSchema.DisplayName, "Calendar")
            Dim calendar As Folder = GetFolder(service, searchFilter)

            Const pageSize As Int32 = 10
            Dim itemView As New ItemView(pageSize)
            Dim propertySet As New PropertySet(BasePropertySet.IdOnly)
            itemView.PropertySet = propertySet

            searchFilter = New SearchFilter.IsEqualTo(AppointmentSchema.Subject, appointmentSubject)

            Dim findResults As FindItemsResults(Of Item) = calendar.FindItems(searchFilter, itemView)

            If findResults.TotalCount > 0 Then
                Return True
            End If

            Return False
        End Function
    End Class
End Namespace
