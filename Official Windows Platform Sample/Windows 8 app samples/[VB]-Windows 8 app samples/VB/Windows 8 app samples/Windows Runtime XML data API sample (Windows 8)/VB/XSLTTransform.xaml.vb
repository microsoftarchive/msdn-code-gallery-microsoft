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
Imports Windows.UI.Xaml.Media
Imports XML
Imports System
Imports SDKTemplate

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class XSLTTransform
    Inherits Global.SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As Global.SDKTemplate.MainPage = Global.SDKTemplate.MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        Scenario5Init()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    Private Async Sub XsltInit()
        Try
            Dim storageFolder As Windows.Storage.StorageFolder = Await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("xsltTransform")
            Dim xmlFile As Windows.Storage.StorageFile = Await storageFolder.GetFileAsync("xmlContent.xml")
            Dim xsltFile As Windows.Storage.StorageFile = Await storageFolder.GetFileAsync("xslContent.xml")

            ' load xml file
            Dim doc As Windows.Data.Xml.Dom.XmlDocument = Await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(xmlFile)
            Global.SDKTemplate.Scenario.RichEditBoxSetMsg(scenario5Xml, doc.GetXml(), False)

            ' load xml file
            doc = Await Windows.Data.Xml.Dom.XmlDocument.LoadFromFileAsync(xsltFile)

            Scenario.RichEditBoxSetMsg(scenario5Xslt, doc.GetXml(), False)
            Scenario.RichEditBoxSetMsg(scenario5Result, "", True)
        Catch
            Scenario.RichEditBoxSetMsg(scenario5Xml, "", False)
            Scenario.RichEditBoxSetMsg(scenario5Xslt, "", False)
            Scenario.RichEditBoxSetError(scenario5Result, Scenario.LoadFileErrorMsg)
        End Try
    End Sub

    Private Sub Scenario5Init()
        XsltInit()
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Scenario5BtnDefault' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Scenario5BtnDefault_Click(sender As Object, e As RoutedEventArgs)
        scenario5Xml.Foreground = New SolidColorBrush(Windows.UI.Colors.Black)
        scenario5Xslt.Foreground = New SolidColorBrush(Windows.UI.Colors.Black)
        Scenario.RichEditBoxSetMsg(scenario5Result, "", True)

        Dim doc As Windows.Data.Xml.Dom.XmlDocument, xsltDoc As Windows.Data.Xml.Dom.XmlDocument
        Dim xml As String = ""
        Dim xslt As String = ""

        ' Get xml content from xml input field
        scenario5Xml.Document.GetText(Windows.UI.Text.TextGetOptions.None, xml)

        ' Get xslt content from xslt input field
        scenario5Xslt.Document.GetText(Windows.UI.Text.TextGetOptions.None, xslt)

        If xml Is Nothing OrElse "" = xml.Trim() Then
            Scenario.RichEditBoxSetError(scenario5Result, "Source XML can't be empty")
            Return
        End If

        If xslt Is Nothing OrElse "" = xslt.Trim() Then
            Scenario.RichEditBoxSetError(scenario5Result, "XSL content can't be empty")
            Return
        End If

        Try
            ' Load xml content
            doc = New Windows.Data.Xml.Dom.XmlDocument()
            doc.LoadXml(xml)
        Catch exp As Exception
            scenario5Xml.Foreground = New SolidColorBrush(Windows.UI.Colors.Red)
            Scenario.RichEditBoxSetError(scenario5Result, exp.Message)
            Return
        End Try

        Try
            ' Load xslt content
            xsltDoc = New Windows.Data.Xml.Dom.XmlDocument()
            xsltDoc.LoadXml(xslt)
        Catch exp As Exception
            scenario5Xslt.Foreground = New SolidColorBrush(Windows.UI.Colors.Red)
            Scenario.RichEditBoxSetError(scenario5Result, exp.Message)
            Return
        End Try

        Try
            ' Transform xml according to the style sheet declaration specified in xslt file
            Dim xsltProcessor = New Windows.Data.Xml.Xsl.XsltProcessor(xsltDoc)
            Dim transformedStr As String = xsltProcessor.TransformToString(doc)
            Scenario.RichEditBoxSetMsg(scenario5Result, transformedStr, True)
        Catch exp As Exception
            Scenario.RichEditBoxSetError(scenario5Result, exp.Message)
            Return
        End Try
    End Sub

End Class
