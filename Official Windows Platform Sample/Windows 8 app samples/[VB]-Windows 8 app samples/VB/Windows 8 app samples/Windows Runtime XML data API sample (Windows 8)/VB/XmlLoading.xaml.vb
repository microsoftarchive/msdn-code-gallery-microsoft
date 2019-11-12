'*********************************************************
'
' Copyright (c) Microsoft. All rights reserved.
' THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
' ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
' IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
' PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'*********************************************************

Imports Windows.UI.Xaml
Imports Windows.UI.Xaml.Controls
Imports Windows.UI.Xaml.Navigation
Imports SDKTemplate
Imports System

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class XmlLoading
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current

    Public Sub New()
        Me.InitializeComponent()

        Scenario3Init()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    Private Async Sub Scenario3Init()
        Try
            Dim doc As Windows.Data.Xml.Dom.XmlDocument = Await Scenario.LoadXmlFile("loadExternaldtd", "xmlWithExternaldtd.xml")
            Scenario.RichEditBoxSetMsg(scenario3OriginalData, doc.GetXml(), True)
        Catch
            Scenario.RichEditBoxSetError(scenario3Result, Scenario.LoadFileErrorMsg)
        End Try
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Go' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub Scenario3BtnDefault_Click(sender As Object, e As RoutedEventArgs)
        Try
    ' get load settings
    Dim loadSettings = New Windows.Data.Xml.Dom.XmlLoadSettings()
            If True = scenario3RB1.IsChecked Then
                loadSettings.ProhibitDtd = True         ' DTD is prohibited
                loadSettings.ResolveExternals = False   ' Disable the resolve to external definitions such as external DTD
            ElseIf True = scenario3RB2.IsChecked Then
                loadSettings.ProhibitDtd = False        ' DTD is not prohibited
                loadSettings.ResolveExternals = False   ' Disable the resolve to external definitions such as external DTD
            ElseIf True = scenario3RB3.IsChecked Then
                loadSettings.ProhibitDtd = False        ' DTD is not prohibited
                loadSettings.ResolveExternals = True    ' Enable the resolve to external definitions such as external DTD
            End If

            Dim folder = Await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("loadExternaldtd")
            Dim file = Await folder.GetFileAsync("xmlWithExternaldtd.xml")

            Dim doc = Await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(file, loadSettings)
            Scenario.RichEditBoxSetMsg(scenario3Result, "The file is loaded successfully.", True)
        Catch exp As Exception
            ' After loadSettings.ProhibitDtd is set to true, the exception is expected as the sample XML contains DTD
            Scenario.RichEditBoxSetError(scenario3Result, "Error: DTD is prohibited")
        End Try
    End Sub

    Private Async Sub LaunchUri(sender As Object, e As RoutedEventArgs)
        Await Windows.System.Launcher.LaunchUriAsync(New Uri(DirectCast(sender, HyperlinkButton).Tag.ToString))
    End Sub

End Class
