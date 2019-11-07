'**************************** Module Header ********************************\
' Module Name:    ClientInfoDataSource.vb
' Project:        VBASPNETDetectBrowserCloseEvent
' Copyright (c) Microsoft Corporation
'
' As we know, HTTP is a stateless protocol, so the browser doesn't keep connecting 
' to the server. When users try to close the browser using alt-f4, browser close(X) 
' and right click on browser and close, all these methods are working fine, 
' but it's not possible to tell the server that the browser is closed.
'
' This class is used as simple data source class.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/
Public Class ClientInfoDataSource
    Private Shared filePath As String = HttpContext.Current.Server.MapPath("~/App_Data/ClientInfos.xml")

    Private Shared clientInfosXDoc As XDocument

    Public Sub New()
        clientInfosXDoc = XDocument.Load(filePath)
    End Sub

    ''' <summary>
    ''' Get ClientInfo by ClientId
    ''' </summary>
    ''' <param name="clientID"></param>
    ''' <returns></returns>
    Public Function GetClientInfoByClientId(clientID As String) As ClientInfo
        Dim query = From clientInfoXml In clientInfosXDoc.Descendants("ClientID") Where clientInfoXml.Value = clientID Select clientInfoXml.Parent

        Return convertToClientInfo(query.FirstOrDefault())
    End Function



    ''' <summary>
    ''' Insert ClientInfo message to XML file
    ''' </summary>
    ''' <param name="clientInfo"></param>
    Public Sub InsertClientInfo(clientInfo As ClientInfo)
        clientInfosXDoc.Root.Add(convertToClientInfoXElement(clientInfo))
    End Sub

    ''' <summary>
    ''' Update ActiveTime and RefreshTime
    ''' </summary>
    ''' <param name="clientInfo"></param>
    Public Sub UpdateClientInfo(clientInfo As ClientInfo)
        Dim query = From x In clientInfosXDoc.Root.Elements() Where x.Element("ClientID").Value = clientInfo.ClientID Select x
        If query.Count() > 0 Then
            query.FirstOrDefault().Element("ActiveTime").Value = clientInfo.ActiveTime.ToString("MM/dd/yyyy HH:mm:ss")
            query.FirstOrDefault().Element("RefreshTime").Value = clientInfo.RefreshTime.ToString("MM/dd/yyyy HH:mm:ss")
        End If
    End Sub

    ''' <summary>
    ''' Save data source changes
    ''' </summary>
    Public Sub Save()
        clientInfosXDoc.Save(filePath)
    End Sub

    ''' <summary>
    ''' Convert XML message to Class
    ''' </summary>
    ''' <param name="clientInfoXml"></param>
    ''' <returns></returns>
    Private Function convertToClientInfo(clientInfoXml As XElement) As ClientInfo
        If clientInfoXml IsNot Nothing Then
            Dim clientInfo As New ClientInfo()
            clientInfo.ClientID = clientInfoXml.Element("ClientID").Value
            clientInfo.ActiveTime = DateTime.Parse(clientInfoXml.Element("ActiveTime").Value)
            clientInfo.RefreshTime = DateTime.Parse(clientInfoXml.Element("RefreshTime").Value)
            Return clientInfo
        End If
        Return Nothing
    End Function
    ''' <summary>
    ''' Convert Class to XML message
    ''' </summary>
    ''' <param name="clientInfo"></param>
    ''' <returns></returns>
    Private Function convertToClientInfoXElement(clientInfo As ClientInfo) As XElement
        If clientInfo IsNot Nothing Then
            Dim xDoc As New XElement("ClientInfo", New XElement("ClientID", clientInfo.ClientID), New XElement("ActiveTime", clientInfo.ActiveTime.ToString("MM/dd/yyyy HH:mm:ss")), New XElement("RefreshTime", clientInfo.RefreshTime.ToString("MM/dd/yyyy HH:mm:ss")))
            Return xDoc
        End If
        Return Nothing
    End Function
End Class