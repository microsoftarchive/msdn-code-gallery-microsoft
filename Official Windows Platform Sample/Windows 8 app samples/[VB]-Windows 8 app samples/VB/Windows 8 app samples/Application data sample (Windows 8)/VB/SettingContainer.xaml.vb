'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports Windows.Storage

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class SettingContainer
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private localSettings As ApplicationDataContainer = Nothing

    Const containerName As String = "exampleContainer"
    Const settingName As String = "exampleSetting"

    Public Sub New()
        Me.InitializeComponent()

        localSettings = ApplicationData.Current.LocalSettings

        DisplayOutput()
    End Sub

    Private Sub CreateContainer_Click(sender As Object, e As RoutedEventArgs)
        Dim container As ApplicationDataContainer = localSettings.CreateContainer(containerName, ApplicationDataCreateDisposition.Always)

        DisplayOutput()
    End Sub

    Private Sub DeleteContainer_Click(sender As Object, e As RoutedEventArgs)
        localSettings.DeleteContainer(containerName)

        DisplayOutput()
    End Sub

    Private Sub WriteSetting_Click(sender As Object, e As RoutedEventArgs)
        If localSettings.Containers.ContainsKey(containerName) Then
            localSettings.Containers(containerName).Values(settingName) = "Hello World" 'example value
        End If

        DisplayOutput()
    End Sub

    Private Sub DeleteSetting_Click(sender As Object, e As RoutedEventArgs)
        If localSettings.Containers.ContainsKey(containerName) Then
            localSettings.Containers(containerName).Values.Remove(settingName)
        End If

        DisplayOutput()
    End Sub

    Private Sub DisplayOutput()
        Dim hasContainer As Boolean = localSettings.Containers.ContainsKey(containerName)
        Dim hasSetting As Boolean = If(hasContainer, localSettings.Containers(containerName).Values.ContainsKey(settingName), False)

        Dim output As String = String.Format("Container Exists: {0}" & vbCrLf & "Setting Exists: {1}", If(hasContainer, "true", "false"), If(hasSetting, "true", "false"))

        OutputTextBlock.Text = output
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub
End Class
