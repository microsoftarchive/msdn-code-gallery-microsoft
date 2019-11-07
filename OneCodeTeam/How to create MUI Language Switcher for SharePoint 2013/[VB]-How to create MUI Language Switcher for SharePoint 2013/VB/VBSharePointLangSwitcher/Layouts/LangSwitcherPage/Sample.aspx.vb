'****************************** Module Header ******************************\
' Module Name:    Sample.aspx.vb
' Project:        VBSharePointLangSwitcher
' Copyright (c) Microsoft Corporation
'
' This is the test page.
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
Imports Microsoft.SharePoint
Imports Microsoft.SharePoint.WebControls
Imports System.Web

Namespace Layouts.LangSwitcherPage

    Partial Public Class Sample
        Inherits LayoutsPageBase

        ' The key of current selected language in the cookies.
        Private Shared strKeyName As String = "LangSwitcher_Setting"
        Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
            If Not IsPostBack Then
                ' Gets the list of installed languages and bind them to DropDownList control.
                Dim languages As SPLanguageCollection = SPLanguageSettings.GetGlobalInstalledLanguages(15)
                ddlLanguages.DataSource = languages
                ddlLanguages.DataBind()
                ' Add a item at the top of the DropDownList and and set it selected by default. 
                ddlLanguages.Items.Insert(0, "NotSelected")
                ddlLanguages.SelectedIndex = 0
            End If
        End Sub

        ''' <summary>
        ''' Save current selected language to cookie.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        Protected Sub btnSave_Click(sender As Object, e As EventArgs)
            If ddlLanguages.SelectedIndex > 0 Then
                ' Selected language.
                Dim strLanguage As String = ddlLanguages.SelectedValue

                ' Set the Cookies.
                Dim acookie As New HttpCookie(strKeyName)
                acookie.Value = strLanguage
                acookie.Expires = DateTime.MaxValue
                Response.Cookies.Add(acookie)

                Response.Redirect(Request.RawUrl)
            End If
        End Sub

    End Class

End Namespace
