'****************************** Module Header ******************************\
' Module Name:  MainPage.xaml.vb
' Project:      VBUWPWebViewZoom
' Copyright (c) Microsoft Corporation.
'
' This sample demonstrates how to resize the content of the WebView in Universal Windows Platform.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

Public NotInheritable Class MainPage
    Inherits Page
    Private Async Sub FooterClick(sender As Object, e As RoutedEventArgs)
        Await Windows.System.Launcher.LaunchUriAsync(New Uri(TryCast(sender, HyperlinkButton).Tag.ToString()))
    End Sub

    Private Async Sub Slider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs)
        If MyWebView IsNot Nothing Then
            Await MyWebView.InvokeScriptAsync("eval", New String() {"ZoomFunction(" + e.NewValue.ToString() + ");"})
        End If

    End Sub
End Class
