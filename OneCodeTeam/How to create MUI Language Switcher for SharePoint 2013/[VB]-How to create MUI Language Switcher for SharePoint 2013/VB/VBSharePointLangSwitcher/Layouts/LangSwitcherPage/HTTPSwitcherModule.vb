'****************************** Module Header ******************************\
' Module Name:    HTTPSwitcherModule.vb
' Project:        VBSharePointLangSwitcher
' Copyright (c) Microsoft Corporation
'
' This is the custom HttpModule
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'*****************************************************************************/

Imports System
Imports System.Web
Imports System.Web.UI
Imports Microsoft.SharePoint
Imports System.Threading
Imports Microsoft.SharePoint.Utilities

Namespace LangSwitcherPage
    Public Class HTTPSwitcherModule
        Implements IHttpModule

        ''' <summary>
        ''' You will need to configure this module in the web.config file of your
        ''' web and register it with IIS before being able to use it. For more information
        ''' see the following link: http://go.microsoft.com/?linkid=8101007
        ''' </summary>
#Region "IHttpModule Members"

        Public Sub Dispose() Implements IHttpModule.Dispose
            'clean-up code here.
        End Sub

        ''' <summary>
        ''' Init event
        ''' </summary>
        ''' <param name="context"></param>
        Public Sub Init(context As HttpApplication) Implements IHttpModule.Init
            AddHandler context.PreRequestHandlerExecute, AddressOf context_PreRequestHandlerExecute
        End Sub

#End Region

        ''' <summary>
        ''' Assuming the selected language is stored in a cookie. Firstly, get the selected
        ''' language from cookie. Then add the selected language to the request header. 
        ''' Finally, use the selected language for the current culture.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Private Sub context_PreRequestHandlerExecute(sender As Object, e As EventArgs)
            ' Get current application.
            Dim httpApp As HttpApplication = TryCast(sender, HttpApplication)

            ' Get all HTTP-specific information about current HTTP request.
            Dim context As HttpContext = httpApp.Context

            ' Current language.
            Dim strLanguage As String = String.Empty

            ' The key of current selected language in the cookies.
            Dim strKeyName As String = "LangSwitcher_Setting"

            Try
                ' Set the current language.
                If httpApp.Request.Cookies(strKeyName) IsNot Nothing Then
                    strLanguage = httpApp.Request.Cookies(strKeyName).Value
                Else
                    strLanguage = "en-us"
                End If

                Dim lang = context.Request.Headers("Accept-Language")
                If lang IsNot Nothing Then
                    If Not lang.Contains(strLanguage) Then
                        context.Request.Headers("Accept-Language") = (strLanguage & ",") + context.Request.Headers("Accept-Language")
                    End If

                    Dim culture = New System.Globalization.CultureInfo(strLanguage)
                    ' Apply the culture.
                    SPUtility.SetThreadCulture(culture, culture)
                End If
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine(ex.Message)
            End Try
        End Sub
    End Class
End Namespace