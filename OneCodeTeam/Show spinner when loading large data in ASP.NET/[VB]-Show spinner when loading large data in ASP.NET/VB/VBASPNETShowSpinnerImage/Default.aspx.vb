'**************************** Module Header ******************************\
' Module Name:  Default.aspx.vb
' Project:      VBASPNETShowSpinnerImage
' Copyright (c) Microsoft Corporation
'
' This project illustrates how to show spinner image while retrieving huge of 
' data. As we know, handle a time-consuming operate always requiring a long 
' time, we need to show a spinner image for better user experience.
' 
' This page is used to retrieve data from XML file, and include PopupProgeress
' user control. 
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'****************************************************************************


Imports System.Xml

Public Class _Default
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub btnRefresh_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnRefresh.Click
        ' Here we use Thread.Sleep() to suspend the thread for 10 seconds for imitating
        ' an expensive, time-consuming operate of retrieve data. (Such as connect network
        ' database to retrieve mass data.)
        ' So in practice application, you can remove this line. 
        System.Threading.Thread.Sleep(10000)

        ' Retrieve data from XML file as sample data.
        Dim xmlDocument As New XmlDocument()
        xmlDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "XMLFile/XMLData.xml")
        Dim tabXML As New DataTable()
        Dim columnName As New DataColumn("Name", Type.[GetType]("System.String"))
        Dim columnAge As New DataColumn("Age", Type.[GetType]("System.Int32"))
        Dim columnCountry As New DataColumn("Country", Type.[GetType]("System.String"))
        Dim columnComment As New DataColumn("Comment", Type.[GetType]("System.String"))
        tabXML.Columns.Add(columnName)
        tabXML.Columns.Add(columnAge)
        tabXML.Columns.Add(columnCountry)
        tabXML.Columns.Add(columnComment)
        Dim nodeList As XmlNodeList = xmlDocument.SelectNodes("Root/Person")
        For Each node As XmlNode In nodeList
            Dim row As DataRow = tabXML.NewRow()
            row("Name") = node.SelectSingleNode("Name").InnerText
            row("Age") = node.SelectSingleNode("Age").InnerText
            row("Country") = node.SelectSingleNode("Country").InnerText
            row("Comment") = node.SelectSingleNode("Comment").InnerText
            tabXML.Rows.Add(row)
        Next
        gvwXMLData.DataSource = tabXML
        gvwXMLData.DataBind()

    End Sub
End Class