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
Imports System.Threading.Tasks
Imports System.Net
Imports System.IO
Imports System.Xml
Imports Windows.Storage

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class XmlReaderWriterScenario
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'DoSomething' button.  You would replace this with your own handler
    ''' if you have a button or buttons on this page.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Async Sub DoSomething_Click(sender As Object, e As RoutedEventArgs)
        Dim b As Button = TryCast(sender, Button)
        If b IsNot Nothing Then
            rootPage.NotifyUser("You clicked the " & b.Content.ToString & " button", NotifyType.StatusMessage)
        End If

        Dim filename As String = "manchester_us.xml"
        Try
            Await ProcessWithReaderWriter(filename)

            ' show the content of the file just created
            Using s As Stream = Await KnownFolders.PicturesLibrary.OpenStreamForReadAsync(filename)
                Using sr As New StreamReader(s)
                    OutputTextBlock1.Text = sr.ReadToEnd()
                End Using
            End Using
        Catch ex As UnauthorizedAccessException
            OutputTextBlock1.Text = "Exception happend, Message:" & ex.Message
        Catch webEx As System.Net.WebException
            OutputTextBlock1.Text = "Exception happend, Message:" & webEx.Message & " Have you updated the bing map key in function ProcessWithReaderWriter()?"
        End Try
    End Sub


    Private Async Function ProcessWithReaderWriter(filename As String) As Task
        ' you need to acquire a Bing Maps key. See http://www.bingmapsportal.com/
        Dim bingMapKey As String = "INSERT_YOUR_BING_MAPS_KEY"
        ' the following uri will returns a response with xml content
        Dim uri As New Uri(String.Format("http://dev.virtualearth.net/REST/v1/Locations?q=manchester&o=xml&key={0}", bingMapKey))
        Dim request As WebRequest = WebRequest.Create(uri)

        ' if needed, specify credential here
        ' request.Credentials = new NetworkCredential();

        ' GetResponseAsync() returns immediately after the header is ready
        Dim response As WebResponse = Await request.GetResponseAsync()
        Dim inputStream As Stream = response.GetResponseStream()

        Dim xrs As New XmlReaderSettings() With {.Async = True, .CloseInput = True}
        Using reader As XmlReader = XmlReader.Create(inputStream, xrs)
            Dim xws As New XmlWriterSettings() With {.Async = True, .Indent = True, .CloseOutput = True}

            Dim outputStream As Stream = Await KnownFolders.PicturesLibrary.OpenStreamForWriteAsync(filename, CreationCollisionOption.OpenIfExists)

            Using writer As XmlWriter = XmlWriter.Create(outputStream, xws)
                Dim prefix As String = ""
                Dim ns As String = ""
                Await writer.WriteStartDocumentAsync()
                Await writer.WriteStartElementAsync(prefix, "Locations", ns)

                ' iterate through the REST message, and find the Mancesters in US then write to file
                While Await reader.ReadAsync()
                    ' take element nodes with name "Address"
                    If reader.NodeType = XmlNodeType.Element AndAlso reader.Name = "Address" Then
                        ' create a XmlReader from the Address element
                        Using subReader As XmlReader = reader.ReadSubtree()
                            Dim isInUS As Boolean = False
                            While Await subReader.ReadAsync()
                                ' check if the CountryRegion element contains "United States"
                                If subReader.Name = "CountryRegion" Then
                                    Dim value As String = Await subReader.ReadInnerXmlAsync()
                                    If value.Contains("United States") Then
                                        isInUS = True
                                    End If
                                End If

                                ' write the FormattedAddress node of the reader, if the address is within US
                                If isInUS AndAlso subReader.NodeType = XmlNodeType.Element AndAlso subReader.Name = "FormattedAddress" Then
                                    Await writer.WriteNodeAsync(subReader, False)
                                End If
                            End While
                        End Using
                    End If
                End While

                Await writer.WriteEndElementAsync()
                Await writer.WriteEndDocumentAsync()
            End Using
        End Using

    End Function
End Class
