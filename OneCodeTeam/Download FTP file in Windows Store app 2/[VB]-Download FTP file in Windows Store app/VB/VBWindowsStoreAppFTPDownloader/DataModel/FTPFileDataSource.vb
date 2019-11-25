'''**************************** Module Header ******************************\
' Module Name:  FTPFileDataSource.vb
' Project:	     VBWindowsStoreAppFTPDownloader
' Copyright (c) Microsoft Corporation.
' 
' The datasource of the UI which supports group. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************

Imports VBWindowsStoreAppFTPDownloader.Common
Imports System.Collections.ObjectModel

Namespace DataModel
    Friend Class FTPFileDataSource
        Inherits BindableBase
        'INSTANT VB NOTE: The variable allGroups was renamed since Visual Basic does not allow class members with the same name:
        Private _allGroups As New ObservableCollection(Of SampleDataGroup)()
        Public ReadOnly Property AllGroups() As ObservableCollection(Of SampleDataGroup)
            Get
                Return Me._allGroups
            End Get
        End Property

        ''' <summary>
        ''' Group all FTP items.
        ''' 1. Directory Group
        ''' 2. File Group.
        ''' </summary>
        ''' <param name="fileList"></param>
        Public Sub New(ByVal fileList As IEnumerable(Of FTP.FTPFileSystem))
            Dim dirGroup As New SampleDataGroup("Directory Group", "Directories", "Sub directories in this folder", "Assets/Folder.png", "all sub directories in this folder") ' + Guid.NewGuid(),

            Dim fileGroup As New SampleDataGroup("File Group", "Files", "Files in this folder", "Assets/File.png", "all files in this folder") ' + Guid.NewGuid(),

            Me._allGroups.Add(dirGroup)
            Me._allGroups.Add(fileGroup)


            For Each item As FTP.FTPFileSystem In fileList
                If item.IsDirectory Then
                    Dim subDir = New SampleDataItem(item.Url.ToString(), item.Name, String.Empty, "Assets/Folder.png", String.Format("{0}", item.ModifiedTime), item, dirGroup)

                    subDir.SetImage("ms-appx:///Assets/Folder.png")

                    dirGroup.Items.Add(subDir)
                Else
                    Dim file = New SampleDataItem(item.Url.ToString(), item.Name, String.Format("{0}KB", Math.Ceiling(CDbl(item.Size) / 1024)), "Assers/File.png", String.Format("{0}KB", item.ModifiedTime), item, fileGroup)

                    file.SetImage("ms-appx:///Assets/File.png")
                    fileGroup.Items.Add(file)
                End If

            Next item
        End Sub


    End Class
End Namespace
