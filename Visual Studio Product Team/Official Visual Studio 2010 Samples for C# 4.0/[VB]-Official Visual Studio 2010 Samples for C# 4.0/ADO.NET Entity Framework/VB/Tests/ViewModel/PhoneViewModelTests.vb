' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System
Imports EmployeeTracker.Model
Imports EmployeeTracker.ViewModel
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Tests.ViewModel

    ''' <summary>
    ''' Unit tests for PhoneViewModel
    ''' </summary>
    <TestClass>
    Public Class PhoneViewModelTests
        ''' <summary>
        ''' Verify getters and setters on ViewModel affect underlying data and notify changes
        ''' </summary>
        <TestMethod>
        Public Sub PropertyGetAndSet()
            ' Test initial properties are surfaced in ViewModel
            Dim ph As Phone = New Phone With {.Number = "NUMBER", .Extension = "EXTENSION"}
            Dim vm As New PhoneViewModel(ph)
            Assert.AreEqual(ph, vm.Model, "Bound object property did not return object from model.")
            Assert.AreEqual(ph.ValidUsageValues, vm.ValidUsageValues, "ValidUsageValues property did not return value from model.")
            Assert.AreEqual("NUMBER", vm.Number, "Number property did not return value from model.")
            Assert.AreEqual("EXTENSION", vm.Extension, "Extension property did not return value from model.")

            ' Test changing properties updates Model and raises PropertyChanged
            Dim lastProperty As String
            AddHandler vm.PropertyChanged, Sub(sender, e) lastProperty = e.PropertyName

            lastProperty = Nothing
            vm.Number = "NEW_NUMBER"
            Assert.AreEqual("Number", lastProperty, "Setting Number property did not raise correct PropertyChanged event.")
            Assert.AreEqual("NEW_NUMBER", ph.Number, "Setting Number property did not update model.")

            lastProperty = Nothing
            vm.Extension = "NEW_EXTENSION"
            Assert.AreEqual("Extension", lastProperty, "Setting Extension property did not raise correct PropertyChanged event.")
            Assert.AreEqual("NEW_EXTENSION", ph.Extension, "Setting Extension property did not update model.")
        End Sub

        ''' <summary>
        ''' Verify getters reflect changes in model
        ''' </summary>
        <TestMethod>
        Public Sub ModelChangesFlowToProperties()
            ' Test ViewModel returns current value from model
            Dim ph As Phone = New Phone With {.Number = "NUMBER", .Extension = "EXTENSION"}
            Dim vm As New PhoneViewModel(ph)

            ph.Number = "NEW_NUMBER"
            ph.Extension = "NEW_EXTENSION"
            Assert.AreEqual("NEW_NUMBER", vm.Number, "Number property is not fetching the value from the model.")
            Assert.AreEqual("NEW_EXTENSION", vm.Extension, "Extension property is not fetching the value from the model.")
        End Sub

        ''' <summary>
        ''' Verify NullArgumentExceptions are thrown where null is invalid
        ''' </summary>
        <TestMethod>
        Public Sub CheckNullArgumentExceptions()
            Dim ctor As Action = Sub()
                                     Dim x = New PhoneViewModel(Nothing)
                                 End Sub
            Utilities.CheckNullArgumentException(ctor, "detail", "ctor")
        End Sub
    End Class
End Namespace
