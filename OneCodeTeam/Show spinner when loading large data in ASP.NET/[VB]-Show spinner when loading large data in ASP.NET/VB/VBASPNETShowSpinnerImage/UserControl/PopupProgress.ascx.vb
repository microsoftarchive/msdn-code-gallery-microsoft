'***************************** Module Header ******************************\
'* Module Name:  PopupProgress.ascx.vb
'* Project:      VBASPNETShowSpinnerImage
'* Copyright (c) Microsoft Corporation
'*
'* This project illustrates how to show spinner image while retrieving huge of 
'* data. As we know, handle a time-consuming operate always requiring a long 
'* time, we need to show a spinner image for better user experience.
'* 
'* This user control will show a popup when user click the button on 
'* Default.aspx page. 
'* 
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'\****************************************************************************



Public Class PopupProgress
    Inherits System.Web.UI.UserControl

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
    ''' <summary>
    ''' This method is used to load images of customize files and 
    ''' register JavaScript code on User Control page.
    ''' </summary>
    ''' <returns></returns>
    Public Function LoadImage() As String
        Dim strbScript As New StringBuilder()
        Dim imageUrl As String = ""

        strbScript.Append("var imgMessage = new Array();")
        strbScript.Append("var imgUrl = new Array();")
        Dim strs As String() = New String(6) {}
        strs(0) = "Image/0.jpg"
        strs(1) = "Image/1.jpg"
        strs(2) = "Image/2.jpg"
        strs(3) = "Image/3.jpg"
        strs(4) = "Image/4.jpg"
        strs(5) = "Image/5.jpg"
        strs(6) = "Image/6.jpg"
        For i As Integer = 0 To strs.Length - 1
            imageUrl = strs(i)

            strbScript.Append([String].Format("imgUrl[{0}] = '{1}';", i, imageUrl))
            strbScript.Append([String].Format("imgMessage[{0}] = '{1}';", i, imageUrl.Substring(imageUrl.LastIndexOf("."c) - 1)))
        Next
        strbScript.Append("for (var i=0; i<imgUrl.length; i++)")
        strbScript.Append("{ (new Image()).src = imgUrl[i]; }")
        Return strbScript.ToString()
    End Function

End Class