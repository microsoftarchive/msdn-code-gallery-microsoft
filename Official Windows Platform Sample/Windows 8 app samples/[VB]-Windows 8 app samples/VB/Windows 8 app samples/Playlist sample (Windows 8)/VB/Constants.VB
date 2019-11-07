'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************
Imports System.Collections.Generic
Imports System
Imports PlayListSample
Imports Windows.Storage.Pickers
Imports System.Threading.Tasks
Imports Windows.Storage
Imports Windows.Media.Playlists


Namespace Global.SDKTemplate
    Partial Public Class MainPage
        Inherits SDKTemplate.Common.LayoutAwarePage
        Public Const FEATURE_NAME As String = "Playlist Sample "

        ' Change the array below to reflect the name of your scenarios.
        ' This will be used to populate the list of scenarios on the main page with
        ' which the user will choose the specific scenario that they are interested in.
        ' These should be in the form: "Navigating to a web page".
        ' The code in MainPage will take care of turning this into: "1) Navigating to a web page"
        Public Shared _ScenarioList As New List(Of Scenario) From
        {
            New Scenario() With {.Title = "Create a playlist", .ClassType = GetType(Create)},
            New Scenario() With {.Title = "Display a playlist", .ClassType = GetType(Display)},
            New Scenario() With {.Title = "Add items to a playlist", .ClassType = GetType(Add)},
            New Scenario() With {.Title = "Remove an item from a playlist", .ClassType = GetType(Remove)},
            New Scenario() With {.Title = "Clear a playlist", .ClassType = GetType(Clear)}
        }

        ''' <summary>
        ''' Playlist used by all sample scenarios
        ''' </summary>
        Public Shared playlist As Playlist = Nothing

        ''' <summary>
        ''' Audio file extensions.
        ''' </summary>
        Public Shared ReadOnly audioExtensions As String() = New String() {".wma", ".mp3", ".mp2", ".aac", ".adt", ".adts", ".m4a"}

        ''' <summary>
        ''' Playlist file extensions.
        ''' </summary>
        Public Shared ReadOnly playlistExtensions As String() = New String() {".m3u", ".wpl", ".zpl"}

        ''' <summary>
        ''' Creates a FileOpenPicker for the specified extensions. 
        ''' </summary>
        ''' <param name="extensions">File extensions to pick.</param>
        ''' <returns>FileOpenPicker instance.</returns>
        Public Shared Function CreateFilePicker(extensions As String()) As FileOpenPicker
            Dim picker As New FileOpenPicker()
            picker.SuggestedStartLocation = PickerLocationId.MusicLibrary

            For Each extension As String In extensions
                picker.FileTypeFilter.Add(extension)
            Next

            Return picker
        End Function

        ''' <summary>
        ''' Picks and loads a playlist.
        ''' </summary>
        Public Shared Async Function PickPlaylistAsync() As Task(Of Playlist)
            Dim picker As FileOpenPicker = CreateFilePicker(MainPage.playlistExtensions)
            Dim file As StorageFile = Await picker.PickSingleFileAsync()

            Dim playlist__1 As Playlist = Nothing
            If file IsNot Nothing Then
                Return Await Playlist.LoadAsync(file)
            End If

            Return playlist__1
        End Function

        Public Shared Function EnsureUnsnapped() As Boolean
            Dim success = True

            '' Try to unsnap if we are currently snapped
            If ApplicationView.Value = ApplicationViewState.Snapped Then
                success = ApplicationView.TryUnsnap()
            End If

            Return success
        End Function
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






