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

Namespace Microsoft.Samples.VisualStudio.CodeSweep.BuildTask
    ''' <summary>
    ''' A representation of a specific instance of a term which should be ignored.
    ''' </summary>
    Public Interface IIgnoreInstance
        ''' <summary>
        ''' Gets the full path of the file in which this instance occurs.
        ''' </summary>
        ReadOnly Property FilePath() As String

        ''' <summary>
        ''' Gets the full text of the line on which this instance occurs.
        ''' </summary>
        ReadOnly Property IgnoredLine() As String

        ''' <summary>
        ''' Gets the column at which the instance begins, relative to the first non-whitespace character on the line.
        ''' </summary>
        ReadOnly Property PositionOfIgnoredTerm() As Integer

        ''' <summary>
        ''' Gets the text of the term which is ignored.
        ''' </summary>
        ReadOnly Property IgnoredTerm() As String

        ''' <summary>
        ''' Returns a string representation of this instance.
        ''' </summary>
        ''' <param name="projectFolderForRelativization">Used to convert the file path to a relative path.</param>
        Function Serialize(ByVal projectFolderForRelativization As String) As String
    End Interface
End Namespace
