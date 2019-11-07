'***************************** Module Header ******************************\
' Module Name:	IRESTAPI.vb
' Project:		WCFRESTService
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
Imports System.ServiceModel
Imports System.ServiceModel.Web

' NOTE: You can use the "Rename" command on the context menu to change the interface name "IRESTAPI" in both code and config file together.
<ServiceContract()>
Public Interface IRESTAPI

    <OperationContract> _
      <WebInvoke(Method:="GET", ResponseFormat:=WebMessageFormat.Xml, BodyStyle:=WebMessageBodyStyle.Wrapped, UriTemplate:="json/{id}")> _
    Function GetUserNameByID(id As String) As String

    <OperationContract> _
    <WebInvoke(Method:="POST", ResponseFormat:=WebMessageFormat.Xml, RequestFormat:=WebMessageFormat.Xml, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="json")> _
    Function GetUserNameByPostID(data As UserData) As UserData

End Interface
