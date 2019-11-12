'**************************************************************************
'
'Copyright (c) Microsoft Corporation. All rights reserved.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
'
'**************************************************************************

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports System.Xml
Imports System.Threading
Imports System.Reflection
Imports System.Globalization
Imports System.Linq

Namespace Microsoft.Samples.VisualStudio.CodeSweep.Scanner
    Friend Class Scanner
        Implements IScanner
        ''' <summary>
        ''' See <c>IScanner</c> documentation.
        ''' </summary>
        Public Function Scan(ByVal filePaths As IEnumerable(Of String), ByVal termTables As IEnumerable(Of ITermTable)) As IMultiFileScanResult Implements IScanner.Scan
            Return Scan(filePaths, termTables, Nothing)
        End Function

        ''' <summary>
        ''' See <c>IScanner</c> documentation.
        ''' </summary>
        Public Function Scan(ByVal filePaths As IEnumerable(Of String), ByVal termTables As IEnumerable(Of ITermTable), ByVal callback As FileScanCompleted) As IMultiFileScanResult Implements IScanner.Scan
            Return Scan(filePaths, termTables, callback, Nothing)
        End Function

        ''' <summary>
        ''' See <c>IScanner</c> documentation.
        ''' </summary>
        Public Function Scan(ByVal filePaths As IEnumerable(Of String), ByVal termTables As IEnumerable(Of ITermTable), ByVal callback As FileScanCompleted, ByVal contentGetter As FileContentGetter) As IMultiFileScanResult Implements IScanner.Scan
            Return Scan(filePaths, termTables, callback, contentGetter, Nothing)
        End Function

        ''' <summary>
        ''' See <c>IScanner</c> documentation.
        ''' </summary>
        Public Function Scan(ByVal filePaths As IEnumerable(Of String), ByVal termTables As IEnumerable(Of ITermTable), ByVal callback As FileScanCompleted, ByVal contentGetter As FileContentGetter, ByVal stopper As ScanStopper) As IMultiFileScanResult Implements IScanner.Scan
            If filePaths Is Nothing Then
                Throw New ArgumentNullException("filePaths")
            End If
            If termTables Is Nothing Then
                Throw New ArgumentNullException("termTables")
            End If

            Dim finder As New MatchFinder(termTables)

            Dim allResults As New MultiFileScanResult()

            For Each filePath As String In filePaths
                If stopper IsNot Nothing AndAlso stopper() Then
                    Exit For
                End If

                If FileShouldBeScanned(filePath) Then
                    Dim fileResult As IScanResult = ScanFile(filePath, finder, contentGetter, stopper)
                    allResults.Append(fileResult)
                    If callback IsNot Nothing Then
                        callback(fileResult)
                    End If
                End If
            Next filePath

            Return allResults
        End Function

#Region "Private Members"

        Private Shared Function FileShouldBeScanned(ByVal filePath As String) As Boolean
            Dim extension As String
            Try
                extension = Path.GetExtension(filePath)
            Catch e1 As ArgumentException
                ' Path.GetExtension can't parse file paths that are invalid, but we still want to
                ' send them to the scanner, so we'll try to manually parse the extension.
                Dim lastDot As Integer = filePath.LastIndexOf("."c)
                Dim lastWhack As Integer = filePath.LastIndexOf("\"c)

                If lastDot < 0 OrElse lastDot < lastWhack Then
                    extension = String.Empty
                Else
                    extension = filePath.Substring(lastDot, filePath.Length - lastDot)
                End If
            End Try

            Return GetAllowedExtensions().Any(Function(item) String.Compare(extension, "." & item, StringComparison.OrdinalIgnoreCase) = 0)
        End Function

        Private Shared Function GetAllowedExtensions() As List(Of String)
            Dim document As New XmlDocument()
            document.Load(Globals.AllowedExtensionsPath)

            Dim result As New List(Of String)()

            For Each node As XmlNode In document.SelectNodes("allowedextensions/extension")
                result.Add(node.InnerText)
            Next node

            Return result
        End Function

        Private Shared Function ScanFile(ByVal filePath As String, ByVal finder As MatchFinder, ByVal contentGetter As FileContentGetter, ByVal stopper As ScanStopper) As IScanResult
            If filePath Is Nothing Then
                Throw New ArgumentNullException("filePath")
            End If

            ' See if the content getter can give us the file contents.  If so, we'll scan that
            ' string rather than loading the file from disk.
            If contentGetter IsNot Nothing Then
                Dim content As String = contentGetter(filePath)
                If content IsNot Nothing Then
                    Return ScanResult.ScanOccurred(filePath, GetScanHits(filePath, content, finder, stopper))
                End If
            End If

            Dim reader As StreamReader = Nothing

            Try
                Try
                    reader = File.OpenText(filePath)
                Catch ex As Exception
                    If TypeOf ex Is UnauthorizedAccessException OrElse TypeOf ex Is ArgumentException OrElse TypeOf ex Is ArgumentNullException OrElse TypeOf ex Is PathTooLongException OrElse TypeOf ex Is DirectoryNotFoundException OrElse TypeOf ex Is FileNotFoundException OrElse TypeOf ex Is NotSupportedException OrElse TypeOf ex Is IOException Then
                        Return ScanResult.ScanNotPossible(filePath)
                    Else
                        Throw
                    End If
                End Try

                Return ScanResult.ScanOccurred(filePath, GetScanHits(filePath, reader, finder, stopper))
            Finally
                If reader IsNot Nothing Then
                    reader.Close()
                End If
            End Try
        End Function

        Private Shared Function GetScanHits(ByVal filePath As String, ByVal reader As StreamReader, ByVal finder As MatchFinder, ByVal stopper As ScanStopper) As IEnumerable(Of IScanHit)
            Dim hits As New List(Of IScanHit)()
            Dim callback As MatchFoundCallback = Sub(term, line, column, lineText, warning) hits.Add(New ScanHit(filePath, line, column, lineText, term, warning))
            finder.Reset()

            Do While Not reader.EndOfStream
                If stopper IsNot Nothing AndAlso stopper() Then
                    Exit Do
                End If
                finder.AnalyzeNextCharacter(CChar(ChrW(reader.Read())), callback)
            Loop

            finder.Finish(callback)

            Return hits
        End Function

        Private Shared Function GetScanHits(ByVal filePath As String, ByVal content As String, ByVal finder As MatchFinder, ByVal stopper As ScanStopper) As IEnumerable(Of IScanHit)
            Dim hits As New List(Of IScanHit)()
            Dim callback As MatchFoundCallback = Sub(term, line, column, lineText, warning) hits.Add(New ScanHit(filePath, line, column, lineText, term, warning))
            finder.Reset()

            For Each c As Char In content
                If stopper IsNot Nothing AndAlso stopper() Then
                    Exit For
                End If
                finder.AnalyzeNextCharacter(c, callback)
            Next c

            finder.Finish(callback)

            Return hits
        End Function

#End Region ' Private Members
    End Class
End Namespace
