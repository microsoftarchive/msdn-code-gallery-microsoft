'****************************** Module Header ******************************\
' Module Name:  HomeController.vb
' Project:      VBASPNETMVCSession
' Copyright (c) Microsoft Corporation.
' 
' This sample demonstrates how to use Session in ASPNET MVC. 
' This class is the Controller for whole project.
'
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
' All other rights reserved.
'  
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/
Public Class HomeController
    Inherits Controller
#Region "All Views' names"
    Private Enum AllViewsNames
        RazorIndex
        ASPXIndex
    End Enum

#End Region

    Shared currentViewEnum As AllViewsNames = AllViewsNames.ASPXIndex

    ' Current view name;
    Private strCurrentView As String = If(currentViewEnum = AllViewsNames.RazorIndex, "Index", "TestPage")

    Public Function Index() As ActionResult
        Return View(strCurrentView)
    End Function

    ''' <summary>
    ''' ActionResult for ordinary session(HttpContext).
    ''' </summary>
    ''' <param name="sessionValue"></param>
    ''' <returns></returns>
    Public Function SaveSession(sessionValue As String) As ActionResult
        Try
            System.Web.HttpContext.Current.Session("sessionString") = sessionValue
            Return RedirectToAction("LoadSession")
        Catch generatedExceptionName As InvalidOperationException
            Return View(strCurrentView)
        End Try
    End Function

    ''' <summary>
    ''' Load session data and redirect to TestPage.
    ''' </summary>
    ''' <returns></returns>
    Public Function LoadSession() As ActionResult
        LoadSessionObject()
        Return View(strCurrentView)
    End Function

    ''' <summary>
    ''' ActionResult for Extension method.
    ''' </summary>
    ''' <param name="sessionValue"></param>
    ''' <returns></returns>
    Public Function SaveSessionByExtensions(sessionValue As String) As ActionResult
        Try
            Session.SetDataToSession(Of String)("key1", sessionValue)
            Return RedirectToAction("LoadSession")
        Catch generatedExceptionName As InvalidOperationException
            Return View(strCurrentView)
        End Try
    End Function

    ''' <summary>
    ''' Store the session value to ViewData.
    ''' </summary>
    Private Sub LoadSessionObject()
        ' Load session from HttpContext.
        ViewData("sessionString") = TryCast(System.Web.HttpContext.Current.Session("sessionString"), [String])

        ' Load session by Extension method.
        Dim value As String = Session.GetDataFromSession(Of String)("key1")
        ViewData("sessionStringByExtensions") = value
    End Sub
End Class