'***************************** Module Header ******************************\
' Module Name:	default.aspx.vb
' Project:		VBASPNETHttpWebRequest
' Copyright (c) Microsoft Corporation.
' 
' This sample will show you how to create HTTPWebReqeust, and get HTTPWebResponse.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'**************************************************************************/
Imports System.Xml
Imports System.IO
Imports System.Net

Public Class _default
    Inherits System.Web.UI.Page
    Private Const url As String = "http://localhost:25794/RESTAPI.svc/json"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnSendRequest_Click(sender As Object, e As EventArgs) Handles btnSendGetRequest.Click
        Dim req As HttpWebRequest = DirectCast(HttpWebRequest.Create(New Uri(url & "/1")), HttpWebRequest)
        req.Method = "Get"
        Try
            Dim res As HttpWebResponse = DirectCast(req.GetResponse(), HttpWebResponse)
            Dim sw As Stream = res.GetResponseStream()
            Dim reader As New StreamReader(sw)
            Response.Write("Your GET Request response XML value:" & reader.ReadToEnd())
            res.Close()
            sw.Close()

            reader.Close()
        Catch generatedExceptionName As Exception
            Throw
        End Try
    End Sub

    Protected Sub btnSendPostRequest_Click(sender As Object, e As EventArgs) Handles btnSendPostRequest.Click
        Dim req As HttpWebRequest = Nothing
        Dim res As HttpWebResponse = Nothing
        Try
            req = DirectCast(WebRequest.Create(url), HttpWebRequest)
            req.Method = "POST"
            req.ContentType = "application/xml; charset=utf-8"
            ' req.Timeout = 30000;

            Dim xmlDoc = New XmlDocument() With { _
                 .XmlResolver = Nothing _
            }
            xmlDoc.Load(Server.MapPath("PostData.xml"))
            Dim sXml As String = xmlDoc.InnerXml
            req.ContentLength = sXml.Length
            Dim sw = New StreamWriter(req.GetRequestStream())
            sw.Write(sXml)
            sw.Close()

            res = DirectCast(req.GetResponse(), HttpWebResponse)
            Dim responseStream As Stream = res.GetResponseStream()
            Dim streamReader = New StreamReader(responseStream)

            ' Read the response into an xml document
            Dim xml = New XmlDocument()
            xml.LoadXml(streamReader.ReadToEnd())

            ' return only the xml representing the response details (inner request)
            Response.Write("Your POST Request response XML value:" & Convert.ToString(xml.InnerXml))
        Catch ex As Exception
            Response.Write(ex.Message)
        End Try
    End Sub
End Class