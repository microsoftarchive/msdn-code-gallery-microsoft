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
Imports System.Collections.Generic
Imports SDKTemplate

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Partial Public NotInheritable Class Scenario4
    Inherits SDKTemplate.Common.LayoutAwarePage
    ' A pointer back to the main page.  This is needed if you want to call methods in MainPage such
    ' as NotifyUser()
    Private rootPage As MainPage = MainPage.Current

    Public Sub New()
        Me.InitializeComponent()
        AddHandler WebView4.ScriptNotify, AddressOf WebView4_ScriptNotify
    End Sub

    ''' <summary>
    ''' Invoked when this page is about to be displayed in a Frame.
    ''' </summary>
    ''' <param name="e">Event data that describes how this page was reached.  The Parameter
    ''' property is typically used to configure the page.</param>
    Protected Overrides Sub OnNavigatedTo(e As NavigationEventArgs)
    End Sub


    Private Sub WebView4_ScriptNotify(sender As Object, e As NotifyEventArgs)
        rootPage.NotifyUser(String.Format("Response from script: '{0}'", e.Value), NotifyType.StatusMessage)
    End Sub

    ''' <summary>
    ''' This is the click handler for the 'FireScript' button.  
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub FireScript_Click(sender As Object, e As RoutedEventArgs)
        If NavToString.IsChecked = True Then
            ' We can run script that uses window.external.notify() to send data back to the app 
            ' without having to set the AllowedScriptNotifyUris property because the app is
            ' trusted and it owns the content of the script.
            WebView4.InvokeScript("SayGoodbye", Nothing)
        Else
            If Nav.IsChecked = True Then
                ' Here we have to set the AllowedScriptNotifyUri property because we are navigating
                ' to some actual site where we don't own the content and we want to allow window.external.notify()
                ' to pass back data to our application.
                Dim allowedUris As New List(Of Uri)()
                allowedUris.Add(New Uri("http://www.bing.com"))
                WebView4.AllowedScriptNotifyUris = allowedUris

                ' Notice that this is fairly contrived but for this example to work we need to be
                ' able to navigate to a real site, but since this site does not have a function that 
                ' we can call that actually uses window.external.notify() we have to inject that into
                ' the page using eval().  See the next scenario for more information on this technique.
                Dim args As String() = {"window.external.notify('GoodBye');"}
                WebView4.InvokeScript("eval", args)
            Else
                rootPage.NotifyUser("Please choose a navigation method", NotifyType.ErrorMessage)
            End If
        End If
    End Sub

    Private Sub NavToString_Click(sender As Object, e As RoutedEventArgs)
        rootPage.NotifyUser("", NotifyType.StatusMessage)
        ' Let's create an HTML fragment that contains some javascript code that we will fire using
        ' InvokeScript().
        Dim htmlFragment As String = vbCr & vbLf & "<html>" & vbCr & vbLf & "    <head>" & vbCr & vbLf & "        <script type='text/javascript'>" & vbCr & vbLf & "            function SayGoodbye() " & vbCr & vbLf & "            {" & vbCr & vbLf & "                window.external.notify('GoodBye'); " & vbCr & vbLf & "            }" & vbCr & vbLf & "        </script>" & vbCr & vbLf & "    </head>" & vbCr & vbLf & "    <body>" & vbCr & vbLf & "        Page with 'Goodbye' script loaded.  Click the 'Fire Script' button to run the script and send data back to the application." & vbCr & vbLf & "    </body>" & vbCr & vbLf & "</html>"

        ' Load the fragment into the HTML text box so it will be visible.
        HTML4.Text = htmlFragment
        WebView4.NavigateToString(HTML4.Text)
    End Sub

    Private Sub Nav_Click(sender As Object, e As RoutedEventArgs)
        rootPage.NotifyUser("", NotifyType.StatusMessage)
        WebView4.Navigate(New System.Uri("http://www.bing.com"))
    End Sub

End Class
