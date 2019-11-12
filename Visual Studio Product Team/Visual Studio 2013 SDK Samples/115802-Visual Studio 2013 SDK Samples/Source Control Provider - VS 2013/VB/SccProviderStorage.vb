'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

' SccProviderStorage.vb : The class implements a fake source control storage for the SccProvider package
'


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Text
Imports System.IO

Namespace Microsoft.Samples.VisualStudio.SourceControlIntegration.SccProvider
    ' This class defines basic source control status values.
	Public Enum SourceControlStatus
		scsUncontrolled = 0
		scsCheckedIn
		scsCheckedOut
	End Enum

	Public Class SccProviderStorage
		Private Shared _storageExtension As String = ".storage"
		Private _projectFile As String = Nothing
		Private _controlledFiles As Hashtable = Nothing

		Public Sub New(ByVal projectFile As String)
			_projectFile = projectFile.ToLower()
			_controlledFiles = New Hashtable()

            ' Read the storage file if it already exist.
			ReadStorageFile()
		End Sub

		''' <summary>
        ''' Saves the list of the "controlled" files in a file with the same name as the project but with an extra ".storage" extension.
		''' </summary>
		Private Sub WriteStorageFile()
			Dim objWriter As StreamWriter = Nothing

			Try
				Dim storageFile As String = _projectFile & _storageExtension
				objWriter = New StreamWriter(storageFile, False, System.Text.Encoding.Unicode)
				For Each strFile As String In _controlledFiles.Keys
					objWriter.Write(strFile)
					objWriter.Write(Constants.vbCrLf)
				Next strFile
			Finally
                If objWriter IsNot Nothing Then
                    objWriter.Close()
                End If
			End Try
		End Sub

		''' <summary>
		''' Reads the list of "controlled" files in the current project.
		''' </summary>
		Public Sub ReadStorageFile()
			Dim storageFile As String = _projectFile & _storageExtension
			If File.Exists(storageFile) Then
				Dim objReader As StreamReader = Nothing

				Try
					objReader = New StreamReader(storageFile, System.Text.Encoding.Unicode, False)
					Dim strLine As String
                    strLine = objReader.ReadLine()
                    Do While strLine <> Nothing
                        strLine.Trim()

                        _controlledFiles(strLine.ToLower()) = Nothing
                        strLine = objReader.ReadLine()
                    Loop
				Finally
                    If objReader IsNot Nothing Then
                        objReader.Close()
                    End If
				End Try
			End If
		End Sub

		''' <summary>
		''' Adds files to source control by adding them to the list of "controlled" files in the current project
		''' and changing their attributes to reflect the "checked in" status.
		''' </summary>
		Public Sub AddFilesToStorage(ByVal files As IList(Of String))
            ' Add the files to a hastable so we can easily check later which files are controlled.
			For Each file As String In files
				_controlledFiles(file.ToLower()) = Nothing
			Next file

            ' And save the storage file.
			WriteStorageFile()

            ' Adding the files to the store also makes the local files read only.
			For Each file As String In files
                If System.IO.File.Exists(file) Then
                    System.IO.File.SetAttributes(file, FileAttributes.ReadOnly)
                End If
			Next file
		End Sub

		''' <summary>
        ''' Renames a "controlled" file. If the project file is being renamed, rename the whole storage file.
		''' </summary>
		Public Sub RenameFileInStorage(ByVal strOldName As String, ByVal strNewName As String)
			strOldName = strOldName.ToLower()
			strNewName = strNewName.ToLower()

            ' Rename the file in the storage if it was controlled.
			If _controlledFiles.ContainsKey(strOldName) Then
				_controlledFiles.Remove(strOldName)
				_controlledFiles(strNewName) = Nothing
			End If

            ' Save the storage file to reflect changes.
			WriteStorageFile()

            ' If the project file itself is being renamed, we have to rename the storage file itself.
			If _projectFile.CompareTo(strOldName) = 0 Then
				Dim _storageOldFile As String = strOldName & _storageExtension
				Dim _storageNewFile As String = strNewName & _storageExtension
				File.Move(_storageOldFile, _storageNewFile)
			End If
		End Sub

		''' <summary>
        ''' Returns a source control status inferred from the file's attributes on local disk.
		''' </summary>
		Public Function GetFileStatus(ByVal filename As String) As SourceControlStatus
			If (Not _controlledFiles.ContainsKey(filename.ToLower())) Then
				Return SourceControlStatus.scsUncontrolled
			End If

            ' Once we know it's controlled, look at the attribute to see if it's "checked out" or not.
			If (Not File.Exists(filename)) Then
                ' Consider a non-existent file checked in.
				Return SourceControlStatus.scsCheckedIn
			Else
				If (File.GetAttributes(filename) And FileAttributes.ReadOnly) = FileAttributes.ReadOnly Then
					Return SourceControlStatus.scsCheckedIn
				Else
					Return SourceControlStatus.scsCheckedOut
				End If
			End If
		End Function

		''' <summary>
        ''' Checkin a file to store by making the file on disk read only.
		''' </summary>
		Public Sub CheckinFile(ByVal filename As String)
			If File.Exists(filename) Then
				File.SetAttributes(filename, FileAttributes.ReadOnly)
			End If
		End Sub

		''' <summary>
        ''' Checkout a file from store by making the file on disk writable.
		''' </summary>
		Public Sub CheckoutFile(ByVal filename As String)
			If File.Exists(filename) Then
				File.SetAttributes(filename, FileAttributes.Normal)
			End If
		End Sub
	End Class
End Namespace
