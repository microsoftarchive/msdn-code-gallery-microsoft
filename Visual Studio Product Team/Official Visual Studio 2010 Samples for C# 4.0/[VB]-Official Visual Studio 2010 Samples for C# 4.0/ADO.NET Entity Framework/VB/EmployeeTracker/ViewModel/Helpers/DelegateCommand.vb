' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics
Imports System.Windows.Input

Namespace EmployeeTracker.ViewModel.Helpers

    ''' <summary>
    ''' ICommand implementation based on delegates
    ''' </summary>
    Public Class DelegateCommand
        Implements ICommand
        ''' <summary>
        ''' Action to be performed when this command is executed
        ''' </summary>
        Private executionAction As Action(Of Object)

        ''' <summary>
        ''' Predicate to determine if the command is valid for execution
        ''' </summary>
        Private canExecutePredicate As Predicate(Of Object)

        ''' <summary>
        ''' Initializes a new instance of the DelegateCommand class.
        ''' The command will always be valid for execution.
        ''' </summary>
        ''' <param name="execute">The delegate to call on execution</param>
        Public Sub New(ByVal execute As Action(Of Object))
            Me.New(execute, Nothing)
        End Sub

        ''' <summary>
        ''' Initializes a new instance of the DelegateCommand class.
        ''' </summary>
        ''' <param name="execute">The delegate to call on execution</param>
        ''' <param name="canExecute">The predicate to determine if command is valid for execution</param>
        Public Sub New(ByVal execute As Action(Of Object), ByVal canExecute As Predicate(Of Object))
            If execute Is Nothing Then
                Throw New ArgumentNullException("execute")
            End If

            Me.executionAction = execute
            Me.canExecutePredicate = canExecute
        End Sub

        ''' <summary>
        ''' Raised when CanExecute is changed
        ''' </summary>
        Public Custom Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
            AddHandler(ByVal value As EventHandler)
                AddHandler CommandManager.RequerySuggested, value
            End AddHandler
            RemoveHandler(ByVal value As EventHandler)
                RemoveHandler CommandManager.RequerySuggested, value
            End RemoveHandler
            RaiseEvent(ByVal sender As System.Object, ByVal e As System.EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' Executes the delegate backing this DelegateCommand
        ''' </summary>
        ''' <param name="parameter">parameter to pass to predicate</param>
        ''' <returns>True if command is valid for execution</returns>
        Public Function CanExecute(ByVal parameter As Object) As Boolean Implements ICommand.CanExecute
            Return If(Me.canExecutePredicate Is Nothing, True, Me.canExecutePredicate(parameter))
        End Function

        ''' <summary>
        ''' Executes the delegate backing this DelegateCommand
        ''' </summary>
        ''' <param name="parameter">parameter to pass to delegate</param>
        ''' <exception cref="InvalidOperationException">Thrown if CanExecute returns false</exception>
        Public Sub Execute(ByVal parameter As Object) Implements ICommand.Execute
            If Not Me.CanExecute(parameter) Then
                Throw New InvalidOperationException("The command is not valid for execution, check the CanExecute method before attempting to execute.")
            End If

            Me.executionAction(parameter)
        End Sub
    End Class
End Namespace
