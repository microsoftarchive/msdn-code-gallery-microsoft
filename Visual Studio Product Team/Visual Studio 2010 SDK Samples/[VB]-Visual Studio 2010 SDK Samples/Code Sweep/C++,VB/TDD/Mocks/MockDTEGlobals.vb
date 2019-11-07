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

Namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
    Friend Class MockDTEGlobals
        Implements EnvDTE.Globals
        Private _variables As New Dictionary(Of String, Object)()
        Private ReadOnly _persisted As New List(Of String)()

        Public Sub ClearNonPersistedVariables()
            Dim result As New Dictionary(Of String, Object)()

            For Each key As String In _variables.Keys
                If _persisted.Contains(key) Then
                    result.Add(key, _variables(key))
                End If
            Next key

            _variables = result
        End Sub

        Public Sub ClearAll()
            _variables.Clear()
            _persisted.Clear()
        End Sub

#Region "Globals Members"

        Public ReadOnly Property DTE() As EnvDTE.DTE Implements EnvDTE.Globals.DTE
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property Parent() As Object Implements EnvDTE.Globals.Parent
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property VariableNames() As Object Implements EnvDTE.Globals.VariableNames
            Get
                Throw New Exception("The method or operation is not implemented.")
            End Get
        End Property

        Public ReadOnly Property VariableExists(ByVal Name As String) As Boolean Implements EnvDTE.Globals.VariableExists
            Get
                Return _variables.ContainsKey(Name)
            End Get
        End Property

        Public Property VariablePersists(ByVal VariableName As String) As Boolean Implements EnvDTE.Globals.VariablePersists
            Get
                Return _persisted.Contains(VariableName)
            End Get
            Set(ByVal pVal As Boolean)
                If pVal Then
                    If (Not _persisted.Contains(VariableName)) Then
                        _persisted.Add(VariableName)
                    End If
                Else
                    _persisted.Remove(VariableName)
                End If
            End Set

        End Property


        Default Public Property Item(ByVal VariableName As String) As Object
            Get
                Return _variables(VariableName)
            End Get
            Set(ByVal value As Object)
                _variables(VariableName) = value
            End Set
        End Property

        Public Property VariableValue(ByVal VariableName As String) As Object Implements EnvDTE.Globals.VariableValue
            Get
                Return _variables(VariableName)
            End Get
            Set(ByVal value As Object)
                _variables(VariableName) = value
            End Set

        End Property


#End Region
    End Class
End Namespace
