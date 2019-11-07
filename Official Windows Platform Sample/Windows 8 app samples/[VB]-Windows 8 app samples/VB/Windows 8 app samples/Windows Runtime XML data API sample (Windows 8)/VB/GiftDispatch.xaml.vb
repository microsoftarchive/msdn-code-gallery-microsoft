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
Imports System.Text

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class GiftDispatch
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        Scenario4Init()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    Private Async Sub Scenario4Init()
        Try
            Dim doc As Windows.Data.Xml.Dom.XmlDocument = Await Scenario.LoadXmlFile("giftDispatch", "employees.xml")
            Scenario.RichEditBoxSetMsg(scenario4OriginalData, doc.GetXml, True)
        Catch
            Scenario.RichEditBoxSetError(scenario4Result, Scenario.LoadFileErrorMsg)
        End Try
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Default' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Scenario4BtnDefault_Click(sender As Object, e As RoutedEventArgs)
        Dim xml As String = ""
        scenario4OriginalData.Document.GetText(Windows.UI.Text.TextGetOptions.None, xml)

        Dim doc = New Windows.Data.Xml.Dom.XmlDocument
        doc.LoadXml(xml)

        Dim thisYear = 2012
        ' Here we don't use DateTime.Now.Year to get the current year so that all gifts can be delivered.
        Dim previousOneYear = thisYear - 1
        Dim previousFiveYear = thisYear - 5
        Dim previousTenYear = thisYear - 10

        Dim xpath = New String(2) {}
        ' select >= 1 year and < 5 years
        xpath(0) = "descendant::employee[startyear <= " & previousOneYear & " and startyear > " & previousFiveYear & "]"
        ' select >= 5 years and < 10 years
        xpath(1) = "descendant::employee[startyear <= " & previousFiveYear & " and startyear > " & previousTenYear & "]"
        ' select >= 10 years
        xpath(2) = "descendant::employee[startyear <= " & previousTenYear & "]"

        Dim Gifts = New String(2) {"Gift Card", "XBOX", "Windows Phone"}

        Dim output = New StringBuilder()
        For i As UInteger = 0 To 2
            Dim employees = doc.SelectNodes(xpath(i))
            For index As UInteger = 0 To employees.Length - 1
                Dim employeeName = employees.ElementAt(index).SelectSingleNode("descendant::name")
                Dim department = employees.ElementAt(index).SelectSingleNode("descendant::department")

                output.AppendFormat("[{0}]/[{1}]/[{2}]" & vbLf, employeeName.FirstChild.NodeValue, department.FirstChild.NodeValue, Gifts.ElementAt(i))
            Next
        Next

        Scenario.RichEditBoxSetMsg(scenario4Result, output.ToString(), True)
    End Sub

End Class
