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
Partial Public NotInheritable Class CompositeSettings
    Inherits SDKTemplate.Common.LayoutAwarePage

    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current
    Private roamingSettings As ApplicationDataContainer = Nothing

    Const settingName As String = "exampleCompositeSetting"
    Const settingName1 As String = "one"
    Const settingName2 As String = "hello"

    Public Sub New()
        Me.InitializeComponent()

        roamingSettings = ApplicationData.Current.RoamingSettings

        DisplayOutput()
    End Sub

    Private Sub WriteCompositeSetting_Click(sender As Object, e As RoutedEventArgs)
        Dim composite As New ApplicationDataCompositeValue
        composite(settingName1) = 1 'example value
        composite(settingName2) = "world" 'example value

        roamingSettings.Values(settingName) = composite

        DisplayOutput()
    End Sub

    Private Sub DeleteCompositeSetting_Click(sender As Object, e As RoutedEventArgs)
        roamingSettings.Values.Remove(settingName)

        DisplayOutput()
    End Sub

    Private Sub DisplayOutput()
        Dim composite As ApplicationDataCompositeValue = DirectCast(roamingSettings.Values(settingName), ApplicationDataCompositeValue)

        Dim output As String
        If composite Is Nothing Then
            output = "Composite Setting: <empty>"
        Else
            output = String.Format("Composite Setting: {{{0} = {1}, {2} = ""{3}""}}", settingName1, composite(settingName1), settingName2, composite(settingName2))
        End If

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
