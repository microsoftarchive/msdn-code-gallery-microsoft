'**************************************************************************

'Copyright (c) Microsoft Corporation. All rights reserved.
'This code is licensed under the Visual Studio SDK license terms.
'THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
'ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
'IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
'PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

'**************************************************************************


Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.IO
Imports System.Globalization
Imports Microsoft.Samples.VisualStudio.CodeSweep.Properties

Namespace Microsoft.Samples.VisualStudio.CodeSweep
    ''' <summary>
    ''' General utility methods.
    ''' </summary>
    Public Class Utilities
        ''' <summary>
        ''' Concatenates a collection of strings.
        ''' </summary>
        ''' <param name="inputs">The strings to concatenate.</param>
        ''' <param name="separator">The separator text that will be placed in between the individual strings.</param>
        Private Sub New()
        End Sub
        Public Shared Function Concatenate(ByVal inputs As IEnumerable(Of String), ByVal separator As String) As String
            Dim result As New StringBuilder()

            For Each input As String In inputs
                If result.Length > 0 Then
                    result.Append(separator)
                End If
                result.Append(input)
            Next input

            Return result.ToString()
        End Function

        ''' <summary>
        ''' "Escapes" all instances of the specified character by inserting backslashes before
        ''' them.  In addition, backslashes are transformed to double-backslashes.
        ''' </summary>
        Public Shared Function EscapeChar(ByVal text As String, ByVal toEscape As Char) As String
            If text Is Nothing Then
                Throw New ArgumentNullException("text")
            End If

            Dim result As New StringBuilder()

            Dim spanStart As Integer = 0

            Dim chars() As Char = {toEscape, "\"c}

            Dim spanStop As Integer = text.IndexOfAny(chars, spanStart)
            Do While spanStop >= 0
                result.Append(text.Substring(spanStart, spanStop - spanStart))
                result.Append("\")
                result.Append(text.Chars(spanStop))

                spanStart = spanStop + 1
                spanStop = text.IndexOfAny(chars, spanStart)
            Loop

            result.Append(text.Substring(spanStart))

            Return result.ToString()
        End Function

        ''' <summary>
        ''' Splits a string into several fields.
        ''' </summary>
        ''' <param name="text"></param>
        ''' <param name="separator"></param>
        ''' <returns></returns>
        ''' <remarks>
        ''' Instances of <c>separator</c> alone are treated as field separators.  Escaped instances
        ''' of <c>separator</c> (prefixed by backslashes) are unescaped, as are double-backslashes.
        ''' </remarks>
        Public Shared Function ParseEscaped(ByVal text As String, ByVal separator As Char) As IList(Of String)
            Dim result As New List(Of String)()

            Dim current As New StringBuilder()

            Dim chars() As Char = {separator, "\"c}

            Dim spanStart As Integer = 0

            Dim spanStop As Integer = text.IndexOfAny(chars, spanStart)
            Do While spanStop >= 0
                current.Append(text.Substring(spanStart, spanStop - spanStart))
                If text.Chars(spanStop) = separator Then
                    ' This is a separator on its own, since it would already have been dealt with
                    ' if it had been preceeded by an escape operator.
                    result.Add(current.ToString())
                    current.Length = 0
                Else
                    ' We found an instance of the escape operator, '\'
                    If spanStop + 1 < text.Length Then
                        If text.Chars(spanStop + 1) = separator Then
                            ' An escaped separator is transformed into a non-escaped separator.
                            current.Append(separator)
                            spanStop += 1
                        ElseIf text.Chars(spanStop + 1) = "\"c Then
                            ' A double-escape is transformed into the escape operator.
                            current.Append("\"c)
                            spanStop += 1
                        End If
                    End If
                End If

                spanStart = spanStop + 1
                spanStop = text.IndexOfAny(chars, spanStart)
            Loop

            If spanStart < text.Length Then
                current.Append(text.Substring(spanStart))
            End If

            If current.Length > 0 Then
                result.Add(current.ToString())
            End If

            Return result
        End Function

        ''' <summary>
        ''' Transforms a relative path to an absolute one based on a specified base folder.
        ''' </summary>
        Public Shared Function AbsolutePathFromRelative(ByVal relativePath As String, ByVal baseFolderForDerelativization As String) As String
            If relativePath Is Nothing Then
                Throw New ArgumentNullException("relativePath")
            End If
            If baseFolderForDerelativization Is Nothing Then
                Throw New ArgumentNullException("baseFolderForDerelativization")
            End If
            If Path.IsPathRooted(relativePath) Then
                Throw New ArgumentException(Resources.PathNotRelative, "relativePath")
            End If
            If (Not Path.IsPathRooted(baseFolderForDerelativization)) Then
                Throw New ArgumentException(Resources.BaseFolderMustBeRooted, "baseFolderForDerelativization")
            End If

            Dim result As New StringBuilder(baseFolderForDerelativization)

            If result(result.Length - 1) <> Path.DirectorySeparatorChar Then
                result.Append(Path.DirectorySeparatorChar)
            End If

            Dim spanStart As Integer = 0

            Do While spanStart < relativePath.Length
                Dim spanStop As Integer = relativePath.IndexOf(Path.DirectorySeparatorChar, spanStart)

                If spanStop = -1 Then
                    spanStop = relativePath.Length
                End If

                Dim span As String = relativePath.Substring(spanStart, spanStop - spanStart)

                If span = ".." Then
                    ' The result string should end with a directory separator at this point.  We
                    ' want to search for the one previous to that, which is why we subtract 2.
                    Dim previousSeparator As Integer = result.ToString().LastIndexOf(Path.DirectorySeparatorChar, result.Length - 2)
                    If previousSeparator = -1 Then
                        Throw New ArgumentException(Resources.BackTooFar)
                    End If
                    result.Remove(previousSeparator, result.Length - previousSeparator)
                ElseIf span <> "." Then
                    result.Append(span)
                End If

                If spanStop < relativePath.Length Then
                    result.Append(Path.DirectorySeparatorChar)
                End If

                spanStart = spanStop + 1
            Loop

            Return result.ToString()
        End Function

        ''' <summary>
        ''' Enumerates over a collection of rooted file paths, creating a new collection containing the relative versions.
        ''' </summary>
        ''' <remarks>
        ''' If any of the paths cannot be relativized (because it does not have the same root as
        ''' the base path), the absolute version is added to the collection that's returned.
        ''' </remarks>
        Public Shared Function RelativizePathsIfPossible(ByVal absolutePaths As IEnumerable(Of String), ByVal basePath As String) As List(Of String)
            Dim relativePaths As New List(Of String)()

            For Each absolutePath As String In absolutePaths
                If CanRelativize(absolutePath, basePath) Then
                    relativePaths.Add(RelativePathFromAbsolute(absolutePath, basePath))
                Else
                    relativePaths.Add(absolutePath)
                End If
            Next absolutePath

            Return relativePaths
        End Function

        Private Shared Function CanRelativize(ByVal absolutePath As String, ByVal basePath As String) As Boolean
            If absolutePath Is Nothing Then
                Throw New ArgumentNullException("pathToRelativize")
            End If
            If basePath Is Nothing Then
                Throw New ArgumentNullException("basePath")
            End If

            If (Not Path.IsPathRooted(absolutePath)) OrElse (Not Path.IsPathRooted(basePath)) Then
                Throw New ArgumentException(Resources.BothMustBeRooted)
            End If

            Return String.Compare(Path.GetPathRoot(absolutePath), Path.GetPathRoot(basePath), StringComparison.OrdinalIgnoreCase) = 0
        End Function

        ''' <summary>
        ''' Transforms an absolute path to a relative one based on a specified base folder.
        ''' </summary>
        Public Shared Function RelativePathFromAbsolute(ByVal pathToRelativize As String, ByVal basePath As String) As String
            If pathToRelativize Is Nothing Then
                Throw New ArgumentNullException("pathToRelativize")
            End If
            If basePath Is Nothing Then
                Throw New ArgumentNullException("basePath")
            End If

            If (Not Path.IsPathRooted(pathToRelativize)) OrElse (Not Path.IsPathRooted(basePath)) Then
                Throw New ArgumentException(Resources.BothMustBeRooted)
            End If

            If String.Compare(Path.GetPathRoot(pathToRelativize), Path.GetPathRoot(basePath), StringComparison.OrdinalIgnoreCase) <> 0 Then
                Throw New ArgumentException(Resources.BothMustHaveSameRoot)
            End If

            Dim commonBase As String = FindCommonSubstring(pathToRelativize, basePath, True)

            If commonBase.Length = basePath.Length Then
                Dim result As String = pathToRelativize.Substring(commonBase.Length)

                If result.Chars(0) = Path.DirectorySeparatorChar Then
                    result = result.Substring(1, result.Length - 1)
                End If
                Return result
            Else
                Dim backOutCount As Integer = CountInstances(basePath.Substring(commonBase.Length), Path.DirectorySeparatorChar) + 1
                Dim result As String = Duplicate(".." & Path.DirectorySeparatorChar, backOutCount) & pathToRelativize.Substring(commonBase.Length)
                Return result
            End If
        End Function

        ''' <summary>
        ''' Duplicates a specified string a specified number of times.
        ''' </summary>
        Public Shared Function Duplicate(ByVal text As String, ByVal count As Integer) As String
            If text Is Nothing Then
                Throw New ArgumentNullException("text")
            End If

            Dim result As New StringBuilder(text.Length * count)

            For i As Integer = 0 To count - 1
                result.Append(text)
            Next i

            Return result.ToString()
        End Function

        ''' <summary>
        ''' Returns the number of instances of a given character in a string.
        ''' </summary>
        Public Shared Function CountInstances(ByVal text As String, ByVal toFind As Char) As Integer
            If text Is Nothing Then
                Throw New ArgumentNullException("text")
            End If

            Dim result As Integer = 0

            For Each c As Char In text
                If c = toFind Then
                    result += 1
                End If
            Next c

            Return result
        End Function

        ''' <summary>
        ''' Returns the longest string <c>first</c> and <c>second</c> have in common beginning at index 0.
        ''' </summary>
        Public Shared Function FindCommonSubstring(ByVal first As String, ByVal second As String, ByVal ignoreCase As Boolean) As String
            If first Is Nothing Then
                Throw New ArgumentNullException("first")
            End If
            If second Is Nothing Then
                Throw New ArgumentNullException("second")
            End If

            Dim length As Integer = 0

            Do While length < first.Length AndAlso length < second.Length
                If ignoreCase Then
                    If Char.ToLowerInvariant(first.Chars(length)) <> Char.ToLowerInvariant(second.Chars(length)) Then
                        Exit Do
                    End If
                Else
                    If first.Chars(length) <> second.Chars(length) Then
                        Exit Do
                    End If
                End If
                length += 1
            Loop

            Return first.Substring(0, length)
        End Function

        Public Shared Function UnorderedCollectionsAreEqual(Of T)(ByVal first As ICollection(Of T), ByVal second As ICollection(Of T)) As Boolean
            If first Is Nothing Then
                Throw New ArgumentNullException("first")
            End If
            If second Is Nothing Then
                Throw New ArgumentNullException("second")
            End If

            If first.Count <> second.Count Then
                Return False
            End If

            For Each item As T In first
                If (Not second.Contains(item)) Then
                    Return False
                End If
            Next item

            Return True
        End Function

        Public Shared Function OrderedCollectionsAreEqual(Of T)(ByVal first As IList(Of T), ByVal second As IList(Of T)) As Boolean
            If first Is Nothing Then
                Throw New ArgumentNullException("first")
            End If
            If second Is Nothing Then
                Throw New ArgumentNullException("second")
            End If

            If first.Count <> second.Count Then
                Return False
            End If

            For i As Integer = 0 To first.Count - 1
                If second.IndexOf(first(i)) <> i Then
                    Return False
                End If
            Next i

            Return True
        End Function

        Public Shared Function EncodeProgramFilesVar(ByVal path As String) As String
            If String.IsNullOrEmpty(path) Then
                Return path
            End If
            Dim programFiles As String = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles)

            If path.StartsWith(programFiles, StringComparison.OrdinalIgnoreCase) Then
                Return "$(ProgramFiles)" & path.Substring(programFiles.Length)
            Else
                Return path
            End If
        End Function
    End Class
End Namespace
