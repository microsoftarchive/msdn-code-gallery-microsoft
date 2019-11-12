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

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario2
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
        ' Let's create a very simple HTML fragment
        Dim htmlFragment As String = "<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>" & vbCr & vbLf & "                <html>" & vbCr & vbLf & "                   <head>" & vbCr & vbLf & "                      <title>Map with valid credentials</title>" & vbCr & vbLf & "                      <meta http-equiv='Content-Type' content='text/html; charset=utf-8'/>" & vbCr & vbLf & "                      <script type='text/javascript' src='http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=7.0'></script>" & vbCr & vbLf & "                      <script type='text/javascript'>" & vbCr & vbLf & "                      var map = null;" & vbCr & vbLf & "                      function getMap()" & vbCr & vbLf & "                      {" & vbCr & vbLf & "                          map = new Microsoft.Maps.Map(document.getElementById('myMap'), {credentials: 'Your Bing Maps Key'});" & vbCr & vbLf & "                      } " & vbCr & vbLf & "                      </script>" & vbCr & vbLf & "                   </head>" & vbCr & vbLf & "                   <body onload='getMap();'>" & vbCr & vbLf & "                      <div id='myMap' style='position:relative; width:400px; height:400px;'></div>" & vbCr & vbLf & "                   </body>" & vbCr & vbLf & "                </html>;"

        ' And load it into the HTML text box so it will be visible
        HTML2.Text = htmlFragment
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Load' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Load_Click(sender As Object, e As RoutedEventArgs)
        ' Grab the HTML from the text box and load it into the WebView
        WebView2.NavigateToString(HTML2.Text)
    End Sub
End Class
