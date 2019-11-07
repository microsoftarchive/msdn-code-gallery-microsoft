'****************************** Module Header ******************************\
' Module Name:  IUserService.vb
' Project:      VBRESTfulWCFServiceProvider
' Copyright (c) Microsoft Corporation.
'
' WCF Service interface to define operations for UserService.svc
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx.
' All other rights reserved.
'
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/

''' <summary>
''' WCF Service interface to define operations
''' </summary>
<ServiceContract(), DataContractFormat()>
Friend Interface IUserService

    ''' <summary>
    ''' Definde operation contract
    ''' </summary>
    ''' <param name="user"></param>
    ''' <remarks></remarks>
    <OperationContract(),
        WebInvoke(Method:="POST",
                  UriTemplate:="/User/Create",
                  RequestFormat:=WebMessageFormat.Json,
                  ResponseFormat:=WebMessageFormat.Json,
                  BodyStyle:=WebMessageBodyStyle.Bare)>
    Sub CreateUser(ByVal user As User)

    <WebInvoke(Method:="DELETE",
               UriTemplate:="/User/Delete/{Id}",
               RequestFormat:=WebMessageFormat.Json,
               ResponseFormat:=WebMessageFormat.Json,
               BodyStyle:=WebMessageBodyStyle.Bare),
           OperationContract()>
    Sub DeleteUser(ByVal id As String)

    <OperationContract(),
        WebGet(UriTemplate:="/User/All",
               RequestFormat:=WebMessageFormat.Json,
               ResponseFormat:=WebMessageFormat.Json,
               BodyStyle:=WebMessageBodyStyle.Bare)>
    Function GetAllUsers() As List(Of User)

    <OperationContract(),
        WebInvoke(Method:="PUT",
                  UriTemplate:="/User/Edit",
                  RequestFormat:=WebMessageFormat.Json,
                  ResponseFormat:=WebMessageFormat.Json,
                  BodyStyle:=WebMessageBodyStyle.Bare)>
    Sub UpdateUser(ByVal user As User)
End Interface


