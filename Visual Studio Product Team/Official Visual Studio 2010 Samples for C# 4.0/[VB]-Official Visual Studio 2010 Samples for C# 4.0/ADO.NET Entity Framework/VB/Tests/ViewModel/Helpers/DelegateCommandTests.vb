' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System
Imports EmployeeTracker.ViewModel.Helpers
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Tests.ViewModel.Helpers

    ''' <summary>
    ''' Unit tests for DelegateCommand
    ''' </summary>
    <TestClass>
    Public Class DelegateCommandTests
        ''' <summary>
        ''' Construct command with no predicate specified for CanExecute
        ''' Verify CanExecute is always true and that command executes when null is specified
        ''' </summary>
        <TestMethod>
        Public Sub ExecuteNoPredicateWithNull()
            Dim called As Boolean = False
            Dim action As Action(Of Object) = Sub(o)
                                                  called = True
                                              End Sub

            Dim cmd As New DelegateCommand(action)
            Assert.IsTrue(cmd.CanExecute(Nothing), "Command should always be able to execute when no predicate is supplied.")
            cmd.Execute(Nothing)
            Assert.IsTrue(called, "Command did not run supplied Action.")
        End Sub

        ''' <summary>
        ''' Construct command with null predicate
        ''' </summary>
        <TestMethod()>
        Public Sub ConstructorAcceptsNullPredicate()
            Dim action As Action(Of Object) = Sub(o)
                                              End Sub

            Dim cmd As New DelegateCommand(action, Nothing)
            Assert.IsTrue(cmd.CanExecute(Nothing), "Command with null specified for predicate should always be able to execute.")
        End Sub

        ''' <summary>
        ''' Construct command with no predicate specified for CanExecute
        ''' Verify CanExecute is always true and that command executes when an object is specified
        ''' </summary>
        <TestMethod>
        Public Sub ExecuteNoPredicateWithArgument()
            Dim called As Boolean = False
            Dim action As Action(Of Object) = Sub(o)
                                                  called = True
                                              End Sub

            Dim cmd As New DelegateCommand(action)
            Assert.IsTrue(cmd.CanExecute("x"), "Command should always be able to execute when no predicate is supplied.")
            cmd.Execute("x")
            Assert.IsTrue(called, "Command did not run supplied Action.")
        End Sub

        ''' <summary>
        ''' Construct command with a 'true' predicate specified for CanExecute
        ''' Verify CanExecute and that command executes
        ''' </summary>
        <TestMethod>
        Public Sub ExecuteWithPredicate()
            Dim called As Boolean = False
            Dim action As Action(Of Object) = Sub(o)
                                                  called = True
                                              End Sub

            Dim cmd As New DelegateCommand(action, Function(o) True)
            Assert.IsTrue(cmd.CanExecute(Nothing), "Command should be able to execute when predicate returns true.")
            cmd.Execute(Nothing)
            Assert.IsTrue(called, "Command did not run supplied Action.")
        End Sub

        ''' <summary>
        ''' Construct command with a 'false' predicate specified for CanExecute
        ''' Verify CanExecute and that attempting to execute throws
        ''' </summary>
        <TestMethod>
        Public Sub AttemptExecuteWithFalsePredicate()
            Dim called As Boolean = False
            Dim action As Action(Of Object) = Sub(o)
                                                  called = True
                                              End Sub

            Dim cmd As New DelegateCommand(action, Function(o) False)
            Assert.IsFalse(cmd.CanExecute(Nothing), "Command should not be able to execute when predicate returns false.")

            Try
                cmd.Execute(Nothing)
            Catch e1 As InvalidOperationException
            End Try

            Assert.IsFalse(called, "Command should not have run supplied Action.")
        End Sub

        ''' <summary>
        ''' Verify NullArgumentExceptions are thrown where null is invalid
        ''' </summary>
        <TestMethod()>
        Public Sub CheckNullArgumentExceptions()
            Dim ctor As Action = Sub()
                                     Dim x = New DelegateCommand(Nothing)
                                 End Sub
            Utilities.CheckNullArgumentException(ctor, "execute", "ctor")
        End Sub
    End Class
End Namespace
