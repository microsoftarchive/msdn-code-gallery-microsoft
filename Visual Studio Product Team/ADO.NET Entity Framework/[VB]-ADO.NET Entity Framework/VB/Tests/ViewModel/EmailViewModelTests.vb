' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System
Imports System.Diagnostics.CodeAnalysis
Imports EmployeeTracker.Model
Imports EmployeeTracker.ViewModel
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Tests

Namespace Tests.ViewModel

    ''' <summary>
    ''' Unit tests for EmailViewModel
    ''' </summary>
    <TestClass>
    Public Class EmailViewModelTests
        ''' <summary>
        ''' Verify getters and setters on ViewModel affect underlying data and notify changes
        ''' </summary>
        <TestMethod>
        Public Sub PropertyGetAndSet()
            ' Test initial properties are surfaced in ViewModel
            Dim em As Email = New Email With {.Address = "EMAIL"}
            Dim vm As New EmailViewModel(em)
            Assert.AreEqual(em, vm.Model, "Bound object property did not return object from model.")
            Assert.AreEqual(em.ValidUsageValues, vm.ValidUsageValues, "ValidUsageValues property did not return value from model.")
            Assert.AreEqual("EMAIL", vm.Address, "Address property did not return value from model.")

            ' Test changing properties updates Model and raises PropertyChanged
            Dim lastProperty As String
            AddHandler vm.PropertyChanged, Sub(sender, e) lastProperty = e.PropertyName

            lastProperty = Nothing
            vm.Address = "NEW_EMAIL"
            Assert.AreEqual("Address", lastProperty, "Setting Address property did not raise correct PropertyChanged event.")
            Assert.AreEqual("NEW_EMAIL", em.Address, "Setting Address property did not update model.")
        End Sub

        ''' <summary>
        ''' Verify getters reflect changes in model
        ''' </summary>
        <TestMethod>
        Public Sub ModelChangesFlowToProperties()
            ' Test ViewModel returns current value from model
            Dim em As Email = New Email With {.Address = "EMAIL"}
            Dim vm As New EmailViewModel(em)

            em.Address = "NEW_EMAIL"
            Assert.AreEqual("NEW_EMAIL", vm.Address, "Address property is not fetching the value from the model.")
        End Sub

        ''' <summary>
        ''' Verify NullArgumentExceptions are thrown where null is invalid
        ''' </summary>
        <TestMethod>
        Public Sub CheckNullArgumentExceptions()
            Dim ctor As Action = Sub()
                                     Dim x = New EmailViewModel(Nothing)
                                 End Sub

            Utilities.CheckNullArgumentException(ctor, "detail", "ctor")
        End Sub
    End Class
End Namespace
