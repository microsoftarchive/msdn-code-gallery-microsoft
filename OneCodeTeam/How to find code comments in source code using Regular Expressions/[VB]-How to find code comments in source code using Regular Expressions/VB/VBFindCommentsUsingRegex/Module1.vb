'****************************** Module Header ******************************\
'* Module Name:  Module1.vb
'* Project:      VBFindCommentsUsingRegex
'* Copyright (c) Microsoft Corporation.
'* 
'* The Main program.
'*
'* This source is subject to the Microsoft Public License.
'* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
'* All other rights reserved.
'* 
'* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
'* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
'* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
'***************************************************************************/
Imports System.Text.RegularExpressions
Imports System.IO

Module Module1

    Sub Main()

        'file variable points to the .cs file where we want to know the code comments from
        Dim file As String = "../../SourceCode.vb"
        Dim pattern As String = "REM((\t| ).*$|$)|^\'[^\r\n]+$|''[^\r\n]+$"

        ' local variable to get hold of single or multi-line code comments
        Dim sLine As String = String.Empty
        Dim mLine As String = String.Empty

        ' Read the file stream
        Dim stream As New FileStream(file, FileMode.Open, FileAccess.Read)
        Dim reader As New StreamReader(stream)

        While reader.Peek() <> -1
            ' Read a line of the stream reader
            sLine = reader.ReadLine()

            ' Trim the space in the start of the line
            sLine = If((sLine IsNot Nothing), sLine.Trim(), sLine)

            Dim res = Regex.Matches(sLine, pattern)
            If res.Count > 0 Then
                Console.WriteLine(sLine)
            End If
        End While


        ' Dispose the stream objects
        reader.Close()
        stream.Close()

        Console.ReadLine()

    End Sub

End Module
