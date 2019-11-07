'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports SDKTemplate
Imports System
Imports Windows.ApplicationModel.Search
Imports Windows.Storage
Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation

Partial Public NotInheritable Class Scenario4
    Inherits SDKTemplate.Common.LayoutAwarePage
    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Sub SetLocalContentSuggestions(isLocal As Boolean)
        Dim settings = New LocalContentSuggestionSettings()
        settings.Enabled = isLocal
        If isLocal Then
            settings.Locations.Add(KnownFolders.MusicLibrary)
            settings.AqsFilter = "kind:Music"
        End If
        SearchPane.GetForCurrentView().SetLocalContentSuggestionSettings(settings)
    End Sub

    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
        MainPage.Current.NotifyUser("Use the search pane to submit a query", NotifyType.StatusMessage)
        SetLocalContentSuggestions(True)
    End Sub

    Protected Overrides Sub OnNavigatedFrom(e As NavigationEventArgs)
        SetLocalContentSuggestions(False)
    End Sub
End Class
