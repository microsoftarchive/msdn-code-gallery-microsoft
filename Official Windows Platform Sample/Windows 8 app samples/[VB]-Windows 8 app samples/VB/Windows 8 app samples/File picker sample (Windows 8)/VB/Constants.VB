'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage

        Public Const FEATURE_NAME As String = "File Picker Sample"

        Public Shared _ScenarioList As New List(Of Scenario) From
        {
            New Scenario With {.Title = "Pick a single photo", .ClassType = GetType(Global.FilePicker.Scenario1)},
            New Scenario With {.Title = "Pick multiple files", .ClassType = GetType(Global.FilePicker.Scenario2)},
            New Scenario With {.Title = "Pick a folder", .ClassType = GetType(Global.FilePicker.Scenario3)},
            New Scenario With {.Title = "Save a file", .ClassType = GetType(Global.FilePicker.Scenario4)},
            New Scenario With {.Title = "Open a cached file", .ClassType = GetType(Global.FilePicker.Scenario5)},
            New Scenario With {.Title = "Update a cached file", .ClassType = GetType(Global.FilePicker.Scenario6)}
        }

        Friend Function EnsureUnsnapped() As Boolean
            ' FilePicker APIs will not work if the application is in a snapped state.
            ' If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
            Dim unsnapped As Boolean = ((ApplicationView.Value <> ApplicationViewState.Snapped) OrElse ApplicationView.TryUnsnap)
            If Not unsnapped Then
                NotifyUser("Cannot unsnap the sample.", NotifyType.StatusMessage)
            End If

            Return unsnapped
        End Function

        Friend Sub ResetScenarioOutput(output As TextBlock)
            ' clear Error/Status
            NotifyUser("", NotifyType.ErrorMessage)
            NotifyUser("", NotifyType.StatusMessage)
            ' clear scenario output
            output.Text = ""
        End Sub
    End Class

    Public Class Scenario
        Public Property Title As String
            Get
                Return m_Title
            End Get
            Set(value As String)
                m_Title = value
            End Set
        End Property
        Private m_Title As String

        Public Property ClassType As Type
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
