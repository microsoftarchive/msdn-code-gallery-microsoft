'*************************** Module Header ******************************'
' Module Name:  FTPFileSystem.vb
' Project:	    VBFTPDownload
' Copyright (c) Microsoft Corporation.
' 
' The class FTPFileSystem represents a file on the remote FTP server. When run
' the FTP LIST protocol method to get a detailed listing of the files on an 
' FTP server, the server will response many records of information. Each record
' represents a file. Depended on the FTP Directory Listing Style of the server,
' the record is like 
' 1. MSDOS
'    1.1. Directory
'         12-13-10  12:41PM  <DIR>  Folder A
'    1.2. File
'         12-13-10  12:41PM  [Size] File B  
'         
'   NOTE: The date segment is like "12-13-10" instead of "12-13-2010" if Four-digit
'         years is not checked in IIS.
'        
' 2. UNIX
'    2.1. Directory
'         drwxrwxrwx 1 owner group 0 Dec 1 12:00 Folder A
'    2.2. File
'         -rwxrwxrwx 1 owner group [Size] Dec 1 12:00 File B
' 
'    NOTE: The date segment does not contains year.
' 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*************************************************************************'

Imports System.Text.RegularExpressions
Imports System.Text

Public Class FTPFileSystem
    ''' <summary>
    ''' The original record string.
    ''' </summary>
    Public Property OriginalRecordString() As String

    ''' <summary>
    ''' MSDOS or UNIX.
    ''' </summary>
    Public Property DirectoryListingStyle() As FTPDirectoryListingStyle

    ''' <summary>
    ''' The server Path.
    ''' </summary>
    Public Property Url() As Uri

    ''' <summary>
    ''' The name of this FTPFileSystem instance.
    ''' </summary>
    Public Property Name() As String

    ''' <summary>
    ''' Specify whether this FTPFileSystem instance is a directory.
    ''' </summary>
    Public Property IsDirectory() As Boolean

    ''' <summary>
    ''' The last modified time of this FTPFileSystem instance.
    ''' </summary>
    Public Property ModifiedTime() As Date

    ''' <summary>
    ''' The size of this FTPFileSystem instance if it is not a directory.
    ''' </summary>
    Public Property Size() As Integer

    Private Sub New()
    End Sub

    ''' <summary>
    ''' Override the method ToString() to display a more friendly message.
    ''' </summary>
    Public Overrides Function ToString() As String
        Return String.Format("{0}" & vbTab & "{1}" & vbTab & vbTab & "{2}",
                             Me.ModifiedTime.ToString("yyyy-MM-dd HH:mm"), 
                             If(Me.IsDirectory, "<DIR>", Me.Size.ToString()), Me.Name)
    End Function

    ''' <summary>
    ''' Find out the FTP Directory Listing Style from the recordString.
    ''' </summary>
    Public Shared Function GetDirectoryListingStyle(ByVal recordString As String) _
        As FTPDirectoryListingStyle
        Dim regex_Renamed As Regex =
            New System.Text.RegularExpressions.Regex("^[d-]([r-][w-][x-]){3}$")

        Dim header As String = recordString.Substring(0, 10)

        ' If the style is UNIX, then the header is like "drwxrwxrwx".
        If regex_Renamed.IsMatch(header) Then
            Return FTPDirectoryListingStyle.UNIX
        Else
            Return FTPDirectoryListingStyle.MSDOS
        End If
    End Function

    ''' <summary>
    ''' Get an FTPFileSystem from the recordString. 
    ''' </summary>
    Public Shared Function ParseRecordString(ByVal baseUrl As Uri,
                                             ByVal recordString As String,
                                             ByVal type As FTPDirectoryListingStyle) _
                                         As FTPFileSystem
        Dim fileSystem As FTPFileSystem = Nothing

        If type = FTPDirectoryListingStyle.UNIX Then
            fileSystem = ParseUNIXRecordString(recordString)
        Else
            fileSystem = ParseMSDOSRecordString(recordString)
        End If

        ' Add "/" to the url if it is a directory
        fileSystem.Url = New Uri(baseUrl, fileSystem.Name _
                                 & (If(fileSystem.IsDirectory, "/", String.Empty)))

        Return fileSystem
    End Function

    ''' <summary>
    ''' The recordString is like
    ''' Directory: drwxrwxrwx   1 owner    group               0 Dec 13 11:25 Folder A
    ''' File:      -rwxrwxrwx   1 owner    group               1024 Dec 13 11:25 File B
    ''' NOTE: The date segment does not contains year.
    ''' </summary>
    Shared Function ParseUNIXRecordString(ByVal recordString As String) As FTPFileSystem
        Dim fileSystem As New FTPFileSystem()

        fileSystem.OriginalRecordString = recordString.Trim()
        fileSystem.DirectoryListingStyle = FTPDirectoryListingStyle.UNIX

        ' The segments is like "drwxrwxrwx", "",  "", "1", "owner", "", "", "", 
        ' "group", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", 
        ' "0", "Dec", "13", "11:25", "Folder", "A".
        Dim segments() As String = fileSystem.OriginalRecordString.Split(" "c)

        Dim index As Integer = 0

        ' The permission segment is like "drwxrwxrwx".
        Dim permissionsegment As String = segments(index)

        ' If the property start with 'd', then it means a directory.
        fileSystem.IsDirectory = permissionsegment.Chars(0) = "d"c

        ' Skip the empty segments.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' Skip the directories segment.

        ' Skip the empty segments.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' Skip the owner segment.

        ' Skip the empty segments.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' Skip the group segment.

        ' Skip the empty segments.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' If this fileSystem is a file, then the size is larger than 0. 
        fileSystem.Size = Integer.Parse(segments(index))

        ' Skip the empty segments.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' The month segment.
        Dim monthsegment As String = segments(index)

        ' Skip the empty segments.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' The day segment.
        Dim daysegment As String = segments(index)

        ' Skip the empty segments.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' The time segment.
        Dim timesegment As String = segments(index)

        fileSystem.ModifiedTime =
            Date.Parse(String.Format("{0} {1} {2} ",
                                     timesegment, monthsegment, daysegment))

        ' Skip the empty segments.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' Calculate the index of the file name part in the original string.
        Dim filenameIndex As Integer = 0

        For i As Integer = 0 To index - 1
            ' "" represents ' ' in the original string.
            If segments(i) = String.Empty Then
                filenameIndex += 1
            Else
                filenameIndex += segments(i).Length + 1
            End If
        Next i
        ' The file name may include many segments because the name can contain ' '.          
        fileSystem.Name = fileSystem.OriginalRecordString.Substring(filenameIndex).Trim()

        Return fileSystem
    End Function

    ''' <summary>
    ''' 12-13-10  12:41PM       <DIR>          Folder A
    ''' </summary>
    ''' <param name="recordString"></param>
    ''' <returns></returns>
    Shared Function ParseMSDOSRecordString(ByVal recordString As String) _
        As FTPFileSystem
        Dim fileSystem As New FTPFileSystem()

        fileSystem.OriginalRecordString = recordString.Trim()
        fileSystem.DirectoryListingStyle = FTPDirectoryListingStyle.MSDOS

        ' The segments is like "12-13-10",  "", "12:41PM", "", "","", "",
        ' "", "", "<DIR>", "", "", "", "", "", "", "", "", "", "Folder", "A".
        Dim segments() As String = fileSystem.OriginalRecordString.Split(" "c)

        Dim index As Integer = 0

        ' The date segment is like "12-13-10" instead of "12-13-2010" if Four-digit years
        ' is not checked in IIS.
        Dim dateSegment As String = segments(index)
        Dim dateSegments() As String =
            dateSegment.Split(New Char() {"-"c}, StringSplitOptions.RemoveEmptyEntries)

        Dim month As Integer = Integer.Parse(dateSegments(0))
        Dim day As Integer = Integer.Parse(dateSegments(1))
        Dim year As Integer = Integer.Parse(dateSegments(2))

        ' If year >=50 and year <100, then  it means the year 19**
        If year >= 50 AndAlso year < 100 Then
            year += 1900

            ' If year <50, then it means the year 20**
        ElseIf year < 50 Then
            year += 2000
        End If

        ' Skip the empty segments.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' The time segment.
        Dim timesegment As String = segments(index)

        fileSystem.ModifiedTime =
            Date.Parse(String.Format("{0}-{1}-{2} {3}", year, month, day, timesegment))

        ' Skip the empty segments.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' The size or directory segment.
        ' If this segment is "<DIR>", then it means a directory, else it means the
        ' file size.
        Dim sizeOrDirSegment As String = segments(index)

        fileSystem.IsDirectory =
            sizeOrDirSegment.Equals("<DIR>", StringComparison.OrdinalIgnoreCase)

        ' If this fileSystem is a file, then the size is larger than 0. 
        If Not fileSystem.IsDirectory Then
            fileSystem.Size = Integer.Parse(sizeOrDirSegment)
        End If

        ' Skip the empty segments.
        index += 1
        Do While segments(index) = String.Empty
            index += 1
        Loop

        ' Calculate the index of the file name part in the original string.
        Dim filenameIndex As Integer = 0

        For i As Integer = 0 To index - 1
            ' "" represents ' ' in the original string.
            If segments(i) = String.Empty Then
                filenameIndex += 1
            Else
                filenameIndex += segments(i).Length + 1
            End If
        Next i
        ' The file name may include many segments because the name can contain ' '.          
        fileSystem.Name = fileSystem.OriginalRecordString.Substring(filenameIndex).Trim()

        Return fileSystem
    End Function
End Class