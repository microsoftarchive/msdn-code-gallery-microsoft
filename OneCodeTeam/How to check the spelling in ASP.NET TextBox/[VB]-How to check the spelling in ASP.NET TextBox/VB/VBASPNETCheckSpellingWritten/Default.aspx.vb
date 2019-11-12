'**************************** Module Header ******************************\
' Module Name: Default.aspx.vb
' Project:     VBASPNETCheckSpellingWritten
' Copyright (c) Microsoft Corporation
'
' The project illustrates how to check whether the spelling written in TextBox
' is correct or not. This sample code checks the users' input words
' via MS Word CheckSpelling component.
' 
' This source is subject to the Microsoft Public License.
' See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
' All other rights reserved.
' 
' THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
' EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
' WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'****************************************************************************

Imports System.Reflection
Imports Microsoft.Office.Interop.Word

Public Class _Default
    Inherits System.Web.UI.Page
    ' Define MS Word application.
    Public applicationWord As Microsoft.Office.Interop.Word.Application


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not IsPostBack Then
            tbInput.Text = "  This article describes a All-In-One framewok sample that demonstrates a step-by-step guide " &
                "illustrating how to strip and parsse the Html code. You can download the sample packge from the donload icons below."
        End If
    End Sub

    Protected Sub btnCheck_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCheck.Click
        ' Prevent multiple checker windows.
        If applicationWord IsNot Nothing Then
            Return
        End If

        applicationWord = New Microsoft.Office.Interop.Word.Application()
        Dim errors As Integer = 0
        If tbInput.Text.Length > 0 Then
            Dim template As Object = Missing.Value
            Dim newTemplate As Object = Missing.Value
            Dim documentType As Object = Missing.Value
            Dim visible As Object = True

            ' Define a MS Word Document, then we use this document to calculate errors number and 
            ' invoke document's CheckSpelling method.
            Dim documentCheck As Microsoft.Office.Interop.Word._Document = applicationWord.Documents.Add(template, newTemplate, documentType, visible)
            applicationWord.Visible = False
            documentCheck.Words.First.InsertBefore(tbInput.Text)
            Dim spellErrorsColl As Microsoft.Office.Interop.Word.ProofreadingErrors = documentCheck.SpellingErrors
            errors = spellErrorsColl.Count

            Dim [optional] As Object = Missing.Value
            documentCheck.Activate()
            documentCheck.CheckSpelling([optional], [optional], [optional], [optional], [optional], [optional], _
             [optional], [optional], [optional], [optional], [optional], [optional])
            documentCheck.LanguageDetected = True


            ' When users close the dialog, the error message will be displayed.
            If errors = 0 Then
                lbMessage.Text = "No errors"
            Else
                lbMessage.Text = "Total errors num:" & errors
            End If

            ' Replace misspelled words of TextBox.
            Dim first As Object = 0
            Dim last As Object = documentCheck.Characters.Count - 1
            tbInput.Text = documentCheck.Range(first, last).Text
        End If

        Dim saveChanges As Object = False
        Dim originalFormat As Object = Missing.Value
        Dim routeDocument As Object = Missing.Value
        DirectCast(applicationWord, _Application).Quit(saveChanges, originalFormat, routeDocument)
        applicationWord = Nothing
    End Sub
End Class