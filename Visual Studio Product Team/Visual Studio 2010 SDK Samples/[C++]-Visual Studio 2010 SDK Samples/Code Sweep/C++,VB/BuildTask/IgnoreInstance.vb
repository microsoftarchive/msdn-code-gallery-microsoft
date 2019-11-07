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
Imports System.Globalization
Imports Microsoft.Samples.VisualStudio.CodeSweep.BuildTask.Properties

Namespace Microsoft.Samples.VisualStudio.CodeSweep.BuildTask
    Friend Class IgnoreInstance
        Implements IIgnoreInstance, IComparable(Of IIgnoreInstance)
        Private _file As String
        Private _lineText As String
        Private _term As String
        Private _column As Integer

        ''' <summary>
        ''' Creates an IgnoreInstance object.
        ''' </summary>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>file</c>, <c>lineText</c>, or <c>term</c> is null.</exception>
        ''' <exception cref="System.ArgumentException">Thrown if <c>file</c>, <c>lineText</c>, or <c>term</c> is empty; or if <c>column</c> is less than zero or greater than or equal to <c>lineText.Length</c>.</exception>
        Public Sub New(ByVal file As String, ByVal lineText As String, ByVal term As String, ByVal column As Integer)
            Init(file, lineText, term, column)
        End Sub

        ''' <summary>
        ''' Creates an IgnoreInstance object from a serialized representation.
        ''' </summary>
        ''' <param name="serialization">The file, term, column, and line text, separated by commas.</param>
        ''' <param name="projectFolderForDerelativization">The project folder used to convert the serialized relative file path to a rooted file path.</param>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>serialization</c> or <c>projectFolderForDerelativization</c> is null.</exception>
        ''' <exception cref="System.ArgumentException">Thrown if <c>serialization</c> does not contain four comma-delimited fields; if any of the string fields is empty; if the column field cannot be parsed; or if the column field is less than zero or greater than or equal to the line text length.</exception>
        Public Sub New(ByVal serialization As String, ByVal projectFolderForDerelativization As String)
            If serialization Is Nothing Then
                Throw New ArgumentNullException("serialization")
            End If

            Dim fields As IList(Of String) = Utilities.ParseEscaped(serialization, ","c)

            If fields.Count <> 4 Then
                Throw New ArgumentException(Resources.InvalidSerializationStringForIgnoreInstance)
            End If

            If fields(0).Length = 0 Then
                Throw New ArgumentException(Resources.EmptyFileField)
            End If

            Dim column As Integer
            If Int32.TryParse(fields(2), column) Then
                Init(Utilities.AbsolutePathFromRelative(fields(0), projectFolderForDerelativization), fields(3), fields(1), column)
            Else
                Throw New ArgumentException(Resources.BadColumnField, "serialization")
            End If
        End Sub

        ''' <summary>
        ''' Returns a serialized representation of this object.
        ''' </summary>
        ''' <returns>A string containing four fields delimited by commas.  Commas within the fields are escaped with backslashes.</returns>
        Public Function Serialize(ByVal projectFolderForRelativization As String) As String Implements IIgnoreInstance.Serialize
            Dim relativePath As String = Utilities.RelativePathFromAbsolute(FilePath, projectFolderForRelativization)

            Return Utilities.EscapeChar(relativePath, ","c) & ","c + Utilities.EscapeChar(IgnoredTerm, ","c) & ","c + PositionOfIgnoredTerm.ToString(CultureInfo.InvariantCulture) & ","c + Utilities.EscapeChar(IgnoredLine, ","c)
        End Function

#Region "IIgnoreInstance Members"

        Public ReadOnly Property FilePath() As String Implements IIgnoreInstance.FilePath
            Get
                Return _file
            End Get
        End Property

        Public ReadOnly Property IgnoredLine() As String Implements IIgnoreInstance.IgnoredLine
            Get
                Return _lineText
            End Get
        End Property

        Public ReadOnly Property PositionOfIgnoredTerm() As Integer Implements IIgnoreInstance.PositionOfIgnoredTerm
            Get
                Return _column
            End Get
        End Property

        Public ReadOnly Property IgnoredTerm() As String Implements IIgnoreInstance.IgnoredTerm
            Get
                Return _term
            End Get
        End Property

#End Region

#Region "IComparable<IIgnoreInstance> Members"

        ''' <summary>
        ''' Compares this ignore instance to another, and returns the result.
        ''' </summary>
        ''' <exception cref="System.ArgumentNullException">Thrown if <c>other</c> is null.</exception>
        ''' <remarks>
        ''' The order of the comparison is largely arbitrary; the important part is whether it
        ''' returns zero or nonzero.
        ''' </remarks>
        Public Function CompareTo(ByVal other As IIgnoreInstance) As Integer Implements IComparable(Of IIgnoreInstance).CompareTo
            If other Is Nothing Then
                Throw New ArgumentNullException("other")
            End If

            Dim result As Integer = 0

            result = String.Compare(FilePath, other.FilePath, StringComparison.OrdinalIgnoreCase)
            If result <> 0 Then
                Return result
            End If

            result = String.CompareOrdinal(IgnoredLine, other.IgnoredLine)
            If result <> 0 Then
                Return result
            End If

            result = String.Compare(IgnoredTerm, other.IgnoredTerm, StringComparison.OrdinalIgnoreCase)
            If result <> 0 Then
                Return result
            End If

            Return PositionOfIgnoredTerm - other.PositionOfIgnoredTerm
        End Function

#End Region

#Region "Private Members"

        Private Sub Init(ByVal file As String, ByVal lineText As String, ByVal term As String, ByVal column As Integer)
            If file Is Nothing Then
                Throw New ArgumentNullException("file")
            End If
            If lineText Is Nothing Then
                Throw New ArgumentNullException("lineText")
            End If
            If term Is Nothing Then
                Throw New ArgumentNullException("term")
            End If
            If file.Length = 0 Then
                Throw New ArgumentException(Resources.EmptyString, "file")
            End If
            column -= LeadingWhitespace(lineText)
            lineText = lineText.Trim()
            If lineText.Length = 0 Then
                Throw New ArgumentException(Resources.EmptyString, "lineText")
            End If
            If term.Length = 0 Then
                Throw New ArgumentException(Resources.EmptyString, "term")
            End If
            If column < 0 OrElse column >= lineText.Length Then
                Throw New ArgumentOutOfRangeException("column", column, Resources.ColumnOutOfRange)
            End If

            _file = file
            _lineText = lineText
            _term = term
            _column = column
        End Sub

        Private Shared Function LeadingWhitespace(ByVal text As String) As Integer
            Dim count As Integer = 0
            Do While count < text.Length AndAlso Char.IsWhiteSpace(text.Chars(count))
                count += 1
            Loop
            Return count
        End Function

#End Region
    End Class
End Namespace
