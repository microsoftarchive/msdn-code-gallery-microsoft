'****************************** Module Header ******************************\
' Module Name:  MainPage.xaml.vb
' Project:      VBWindowsStoreAppHttpClientPostJson
' Copyright (c) Microsoft Corporation.
' 
' This sample demonstrates how to use HttpClient to post Json data in 
' Windows Store app.
'  
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

' The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Runtime.Serialization.Json
Imports System.Text

''' <summary>
''' A basic page that provides characteristics common to most applications.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Common.LayoutAwarePage

    Dim httpClient As HttpClient
    ''' <summary>
    ''' Populates the page with content passed during navigation.  Any saved state is also
    ''' provided when recreating a page from a prior session.
    ''' </summary>
    ''' <param name="navigationParameter">The parameter value passed to
    ''' <see cref="Frame.Navigate"/> when this page was initially requested.
    ''' </param>
    ''' <param name="pageState">A dictionary of state preserved by this page during an earlier
    ''' session.  This will be null the first time a page is visited.</param>
    Protected Overrides Sub LoadState(navigationParameter As Object, pageState As Dictionary(Of String, Object))

    End Sub

    ''' <summary>
    ''' Preserves state associated with this page in case the application is suspended or the
    ''' page is discarded from the navigation cache.  Values must conform to the serialization
    ''' requirements of <see cref="Common.SuspensionManager.SessionState"/>.
    ''' </summary>
    ''' <param name="pageState">An empty dictionary to be populated with serializable state.</param>
    Protected Overrides Sub SaveState(pageState As Dictionary(Of String, Object))

    End Sub

    ''' <summary>
    ''' Start to Call WCF service
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Async Function Start_Click(sender As Object, e As RoutedEventArgs) As Task
        ' Clear text of Output textbox 
        Me.OutputField.Text = String.Empty
        Me.StatusBlock.Text = String.Empty

        Me.StartButton.IsEnabled = False
        httpClient = New HttpClient()
        Try
            Dim resourceAddress As String = "http://localhost:29506/WCFService.svc/GetData"
            Dim age As Integer = Convert.ToInt32(Me.Agetxt.Text)
            If age > 120 OrElse age < 0 Then
                Throw New Exception("Age must be between 0 and 120")
            End If
            Dim p As New Person() With {
                 .Name = Me.Nametxt.Text,
                .Age = age
            }
            Dim postBody As String = JsonSerializer(p)
            httpClient.DefaultRequestHeaders.Accept.Add(New MediaTypeWithQualityHeaderValue("application/json"))
            Dim wcfResponse As HttpResponseMessage = Await httpClient.PostAsync(resourceAddress, New StringContent(postBody, Encoding.UTF8, "application/json"))
            Await DisplayTextResult(wcfResponse, OutputField)

        Catch hre As HttpRequestException
            NotifyUser("Error:" + hre.Message)
        Catch generatedExceptionName As TaskCanceledException
            NotifyUser("Request canceled.")
        Catch ex As Exception
            NotifyUser(ex.Message)
        Finally
            Me.StartButton.IsEnabled = True
            If httpClient IsNot Nothing Then
                httpClient.Dispose()
                httpClient = Nothing
            End If
        End Try
    End Function

    ''' <summary>
    ''' Display Result which returns from WCF Service in "OutputField" Textbox
    ''' </summary>
    ''' <param name="response">Http response Message</param>
    ''' <param name="output">Show result control</param>
    ''' <returns></returns>
    Private Async Function DisplayTextResult(response As HttpResponseMessage, output As TextBox) As Task
        Dim responJsonText As String = Await response.Content.ReadAsStringAsync()
        GetJsonValue(responJsonText)
        output.Text += GetJsonValue(responJsonText)
    End Function

    ''' <summary>
    ''' Serialize Person object to Json string
    ''' </summary>
    ''' <param name="objectToSerialize">Person object instance</param>
    ''' <returns>return Json String</returns>
    Public Function JsonSerializer(objectToSerialize As Person) As String
        If objectToSerialize Is Nothing Then
            Throw New ArgumentException("objectToSerialize must not be null")
        End If
        Dim ms As MemoryStream = Nothing

        Dim serializer As New DataContractJsonSerializer(objectToSerialize.[GetType]())
        ms = New MemoryStream()
        serializer.WriteObject(ms, objectToSerialize)
        ms.Seek(0, SeekOrigin.Begin)
        Dim sr As New StreamReader(ms)
        Return sr.ReadToEnd()
    End Function

    ''' <summary>
    ''' Get Result from Json String
    ''' </summary>
    ''' <param name="jsonString">Json string which returns from WCF Service</param>
    ''' <returns>Result string</returns>
    Public Function GetJsonValue(jsonString As String) As String
        Dim ValueLength As Integer = jsonString.LastIndexOf("""") - (jsonString.IndexOf(":") + 2)
        Dim value As String = jsonString.Substring(jsonString.IndexOf(":") + 2, ValueLength)
        Return value

    End Function
#Region "Common methods"

    ''' <summary>
    ''' The the event handler for the click event of the link in the footer. 
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Async Function FooterClick(sender As Object, e As RoutedEventArgs) As Task
        Dim hyperlinkButton As HyperlinkButton = TryCast(sender, HyperlinkButton)
        If hyperlinkButton IsNot Nothing Then
            Await Windows.System.Launcher.LaunchUriAsync(New Uri(hyperlinkButton.Tag.ToString()))
        End If
    End Function

    Public Sub NotifyUser(message As String)
        Me.StatusBlock.Text = message
    End Sub
#End Region

End Class
