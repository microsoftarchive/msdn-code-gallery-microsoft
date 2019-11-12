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
Partial Public NotInheritable Class BuildNewRss
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current

    Public Sub New()
        Me.InitializeComponent()

        Scenario1Init()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub


    ''' <summary>
    ''' This is the click handler for the 'Scenario1BtnDefault' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Scenario1BtnDefault_Click(sender As Object, e As RoutedEventArgs)
        Dim rss As String = scenario1RssInput.Text
        If rss IsNot Nothing AndAlso "" <> rss Then
            Try
                Dim xml As String = ""
                Dim doc = New Windows.Data.Xml.Dom.XmlDocument
                scenario1OriginalData.Document.GetText(Windows.UI.Text.TextGetOptions.None, xml)
                doc.LoadXml(xml)

                ' create a rss CDataSection and insert into DOM tree
                Dim cdata = doc.CreateCDataSection(rss)
                Dim element = doc.GetElementsByTagName("content").ElementAt(0)
                element.AppendChild(cdata)

                Scenario.RichEditBoxSetMsg(scenario1Result, doc.GetXml, True)
            Catch exp As Exception
                Scenario.RichEditBoxSetError(scenario1Result, exp.Message)
            End Try
        Else
            Scenario.RichEditBoxSetError(scenario1Result, "Please type in RSS content in the [RSS Content] box firstly.")
        End If
    End Sub

    Private Async Sub Scenario1Init()
        Try
            Dim doc As Windows.Data.Xml.Dom.XmlDocument = Await Scenario.LoadXmlFile("buildRss", "rssTemplate.xml")
            Scenario.RichEditBoxSetMsg(scenario1OriginalData, doc.GetXml, True)
        Catch
            Scenario.RichEditBoxSetError(scenario1Result, Scenario.LoadFileErrorMsg)
        End Try
    End Sub
End Class
