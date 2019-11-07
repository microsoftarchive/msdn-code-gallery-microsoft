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
    ''' Factory methods for creation of objects whose implementation is internal to the BuildTask project.
    ''' </summary>
    Public Class Factory
        ''' <summary>
        ''' Creates an IIgnoreInstance implementation from the specified arguments.
        ''' </summary>
        Private Sub New()
        End Sub
        Public Shared Function GetIgnoreInstance(ByVal file As String, ByVal lineText As String, ByVal term As String, ByVal column As Integer) As IIgnoreInstance
            Return New IgnoreInstance(file, lineText, term, column)
        End Function

        ''' <summary>
        ''' Creates an IIgnoreInstance implementation from the specified arguments.
        ''' </summary>
        Public Shared Function DeserializeIgnoreInstance(ByVal serializedInstance As String, ByVal projectFolderForRelativization As String) As IIgnoreInstance
            Return New IgnoreInstance(serializedInstance, projectFolderForRelativization)
        End Function

        ''' <summary>
        ''' Creates a collection of IIgnoreInstance implementations from the specified arguments.
        ''' </summary>
        ''' <remarks>
        ''' This is the opposite of SerializeIgnoreInstances; you can pass its output to this method.
        ''' </remarks>
        Public Shared Function DeserializeIgnoreInstances(ByVal serializedInstances As String, ByVal projectFolderForRelativization As String) As IEnumerable(Of IIgnoreInstance)
            Dim results As New List(Of IIgnoreInstance)
            If serializedInstances IsNot Nothing Then
                For Each ignoreInstance As String In Utilities.ParseEscaped(serializedInstances, ";"c)
                    If ignoreInstance IsNot Nothing AndAlso ignoreInstance.Length > 0 Then
                        results.Add(Factory.DeserializeIgnoreInstance(ignoreInstance, projectFolderForRelativization))
                    End If
                Next ignoreInstance
            End If
            Return results
        End Function

        ''' <summary>
        ''' Creates a string representation of a collection of IIgnoreInstance objects.
        ''' </summary>
        ''' <remarks>
        ''' This is the opposite of DeserializeIgnoreInstances; you can pass its output to this method.
        ''' </remarks>
        Public Shared Function SerializeIgnoreInstances(ByVal instances As IEnumerable(Of IIgnoreInstance), ByVal projectFolderForRelativization As String) As String
            Dim result As New StringBuilder()

            For Each instance As BuildTask.IIgnoreInstance In instances
                If result.Length > 0 Then
                    result.Append(";"c)
                End If
                result.Append(Utilities.EscapeChar(instance.Serialize(projectFolderForRelativization), ";"c))
            Next instance

            Return result.ToString()
        End Function
    End Class
End Namespace
