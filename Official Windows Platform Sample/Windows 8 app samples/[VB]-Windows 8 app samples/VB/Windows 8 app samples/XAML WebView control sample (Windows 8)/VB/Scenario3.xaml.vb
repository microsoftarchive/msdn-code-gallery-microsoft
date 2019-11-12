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
Partial Public NotInheritable Class Scenario3
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
        ' Let's create an HTML fragment that contains some javascript code that we will fire using
        ' InvokeScript().
        Dim htmlFragment As String = vbCr & vbLf & "<html>" & vbCr & vbLf & "    <head>" & vbCr & vbLf & "        <script type='text/javascript'>" & vbCr & vbLf & "            function SayGoodbye() " & vbCr & vbLf & "            { " & vbCr & vbLf & "                document.getElementById('myDiv').innerText = 'GoodBye'; " & vbCr & vbLf & "            }" & vbCr & vbLf & "        </script>" & vbCr & vbLf & "    </head>" & vbCr & vbLf & "    <body>" & vbCr & vbLf & "        <div id='myDiv'>Hello</div>" & vbCr & vbLf & "     </body>" & vbCr & vbLf & "</html>"

        ' Load it into the HTML text box so it will be visible.
        HTML3.Text = htmlFragment
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Load' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Load_Click(sender As Object, e As RoutedEventArgs)
        ' Grab the HTML from the text box and load it into the WebView
        WebView3.NavigateToString(HTML3.Text)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'Script' button.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub Script_Click(sender As Object, e As RoutedEventArgs)
        ' Invoke the javascript function called 'SayGoodbye' that is loaded into the WebView.
        WebView3.InvokeScript("SayGoodbye", Nothing)
    End Sub
End Class
