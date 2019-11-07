'****************************** Module Header ******************************\
' Module Name:  Default.aspx.vb
' Project:      VBASPNETControlCausePostback
' Copyright (c) Microsoft Corporation.
'
' The sample code demonstrates how to create a web application that can determine 
' which control causes the postback event on an Asp.net page. Sometimes, we need 
' to perform some specific actions based on the specific control which causes the 
' postback. For example, we can get controls’ id property that and do some operations, 
' such as set TextBox’s text with ViewState variable. In this sample, we can also 
' transfer some data through postbacks.
'
' This page shows how to determine which HTML control causes the postback event.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/



Partial Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack Then
            Dim builder As New StringBuilder()
            If Not [String].IsNullOrEmpty(Request("__EVENTTARGET")) AndAlso Not [String].IsNullOrEmpty(Request("__EVENTARGUMENT")) Then
                Dim target As String = TryCast(Request("__EVENTTARGET"), String)
                Dim argument As String = TryCast(Request("__EVENTARGUMENT"), String)
                builder.Append("Cause postback control:")
                builder.Append("<br />")
                builder.Append(target)
                builder.Append("<br />")
                builder.Append("<br />")
                builder.Append("Postback data:")
                builder.Append("<br />")
                builder.Append(argument)
                lbMessage.Text = builder.ToString()

            End If
        End If
    End Sub

End Class