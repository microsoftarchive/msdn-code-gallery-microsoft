'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports System
Imports System.Collections.Generic
Imports System.IO
Imports Windows.Storage

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage
        Public Const FEATURE_NAME As String = "File Access Sample"

        Public Shared _ScenarioList As New List(Of Scenario) From {New Scenario With {.Title = "Creating a file", .ClassType = GetType(Global.FileAccess.Scenario1)},
                                                          New Scenario With {.Title = "Writing and reading text in a file", .ClassType = GetType(Global.FileAccess.Scenario2)},
                                                          New Scenario With {.Title = "Writing and reading bytes in a file", .ClassType = GetType(Global.FileAccess.Scenario3)},
                                                          New Scenario With {.Title = "Writing and reading using a stream", .ClassType = GetType(Global.FileAccess.Scenario4)},
                                                          New Scenario With {.Title = "Displaying file properties", .ClassType = GetType(Global.FileAccess.Scenario5)},
                                                          New Scenario With {.Title = "Persisting access to a storage item for future use", .ClassType = GetType(Global.FileAccess.Scenario6)},
                                                          New Scenario With {.Title = "Copying a file", .ClassType = GetType(Global.FileAccess.Scenario7)},
                                                          New Scenario With {.Title = "Deleting a file", .ClassType = GetType(Global.FileAccess.Scenario8)}
                                                        }

        Public Const filename As String = "sample.dat"
        Public sampleFile As StorageFile = Nothing
        Public mruToken As String = Nothing
        Public falToken As String = Nothing

        ''' <summary>
        ''' Checks if sample file already exists, if it does assign it to sampleFile
        ''' </summary>
        Private Async Sub Initialize()
            Try
                sampleFile = Await Windows.Storage.KnownFolders.DocumentsLibrary.GetFileAsync(filename)
                ' sample file doesn't exist so scenario one must be run
            Catch generatedExceptionName As FileNotFoundException
            End Try
        End Sub

        Private Sub ValidateFile(scenarioClass As Type)
            If scenarioClass IsNot GetType(FileAccess.Scenario1) AndAlso sampleFile Is Nothing Then
                NotifyUserFileNotExist()
            End If
        End Sub

        Public Sub NotifyUserFileNotExist()
            NotifyUser("The file '" & filename & "' does not exist. Use scenario one to create this file.", NotifyType.ErrorMessage)
        End Sub

        Friend Sub ResetScenarioOutput(output As TextBlock)
            ' clear Error/Status
            NotifyUser("", NotifyType.ErrorMessage)
            NotifyUser("", NotifyType.StatusMessage)
            ' clear scenario output
            output.Text = ""
        End Sub
    End Class

    Public Class Scenario
        Public Property Title() As String
            Get
                Return m_Title
            End Get
            Set(value As String)
                m_Title = value
            End Set
        End Property
        Private m_Title As String

        Public Property ClassType() As Type
            Get
                Return m_ClassType
            End Get
            Set(value As Type)
                m_ClassType = value
            End Set
        End Property
        Private m_ClassType As Type

        Public Overrides Function ToString() As String
            Return Title
        End Function
    End Class
End Namespace

