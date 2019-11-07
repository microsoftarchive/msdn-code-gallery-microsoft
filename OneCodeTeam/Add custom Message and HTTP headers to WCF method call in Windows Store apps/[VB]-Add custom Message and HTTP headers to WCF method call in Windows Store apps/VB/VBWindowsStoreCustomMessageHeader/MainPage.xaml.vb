'****************************** Module Header ******************************\
' Module Name:  MainPage.xaml.vb
' Project:      VBWindowsStoreCustomMessageHeader
' Copyright (c) Microsoft Corporation.
' 
' This is the MainPage of the app.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Imports System.ServiceModel
Imports VBWindowsStoreCustomMessageHeader.CalculatorService
Imports System.ServiceModel.Channels

' The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class MainPage
    Inherits Page
    Public Sub New()
        Me.InitializeComponent()
        AddHandler Me.SizeChanged, AddressOf MainPage_SizeChanged
    End Sub

    Private Sub MainPage_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        If e.NewSize.Width <= 500 Then
            VisualStateManager.GoToState(Me, "MinimalLayout", True)
        ElseIf e.NewSize.Width < e.NewSize.Height Then
            VisualStateManager.GoToState(Me, "PortraitLayout", True)
        Else
            VisualStateManager.GoToState(Me, "DefaultLayout", True)
        End If
    End Sub

    ''' <summary>
    ''' Handle the button's click event.
    ''' </summary>
    ''' <param name="sender">Button</param>
    ''' <param name="e">Click</param>
    Private Async Sub btnAdd_Click(sender As Object, e As RoutedEventArgs)
        Try
            ' Create a client instance of the CustomMessageHeaderService.
            Dim client As New CalculatorServiceClient()
            Using New OperationContextScope(client.InnerChannel)
                ' We will use an instance of the custom class called UserInfo as a MessageHeader.
                Dim userInfo As New UserInfo()
                userInfo.FirstName = "John"
                userInfo.LastName = "Doe"
                userInfo.Age = 30

                ' Add a SOAP Header to an outgoing request
                Dim aMessageHeader As MessageHeader = MessageHeader.CreateHeader("UserInfo", "http://tempuri.org", userInfo)
                OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader)

                ' Add a HTTP Header to an outgoing request
                Dim requestMessage As New HttpRequestMessageProperty()
                requestMessage.Headers("MyHttpHeader") = "MyHttpHeaderValue"
                OperationContext.Current.OutgoingMessageProperties(HttpRequestMessageProperty.Name) = requestMessage

                ' Add the two number and get the result.
                Dim result As Double = Await client.AddAsync(20, 40)
                txtOut.Text = "Add result: " & result.ToString()
            End Using
        Catch oEx As Exception
            txtOut.Text = "Exception: " + oEx.Message
        End Try
    End Sub


    Private Async Sub Footer_Click(sender As Object, e As RoutedEventArgs)
        Await Windows.System.Launcher.LaunchUriAsync(New Uri(TryCast(sender, HyperlinkButton).Tag.ToString()))
    End Sub
End Class
