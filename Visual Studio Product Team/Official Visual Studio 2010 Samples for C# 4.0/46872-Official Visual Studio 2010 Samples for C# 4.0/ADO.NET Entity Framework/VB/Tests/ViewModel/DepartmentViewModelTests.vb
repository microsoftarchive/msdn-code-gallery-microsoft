' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System
Imports EmployeeTracker.Model
Imports EmployeeTracker.ViewModel
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Tests

Namespace Tests.ViewModel

    ''' <summary>
    ''' Unit tests for DepartmentViewModel
    ''' </summary>
    <TestClass>
    Public Class DepartmentViewModelTests
        ''' <summary>
        ''' Verify getters and setters on ViewModel affect underlying data and notify changes
        ''' </summary>
        <TestMethod>
        Public Sub PropertyGetAndSet()
            ' Test initial properties are surfaced in ViewModel
            Dim dep As Department = New Department With {.DepartmentName = "DepartmentName", .DepartmentCode = "DepartmentCode"}
            Dim vm As New DepartmentViewModel(dep)
            Assert.AreEqual(dep, vm.Model, "Bound object property did not return object from model.")
            Assert.AreEqual("DepartmentName", vm.DepartmentName, "DepartmentName property did not return value from model.")
            Assert.AreEqual("DepartmentCode", vm.DepartmentCode, "DepartmentCode property did not return value from model.")

            ' Test changing properties updates Model and raises PropertyChanged
            Dim lastProperty As String
            AddHandler vm.PropertyChanged, Sub(sender, e) lastProperty = e.PropertyName

            lastProperty = Nothing
            vm.DepartmentName = "DepartmentName_NEW"
            Assert.AreEqual("DepartmentName", lastProperty, "Setting DepartmentName property did not raise correct PropertyChanged event.")
            Assert.AreEqual("DepartmentName_NEW", dep.DepartmentName, "Setting DepartmentName property did not update model.")

            lastProperty = Nothing
            vm.DepartmentCode = "DepartmentCode_NEW"
            Assert.AreEqual("DepartmentCode", lastProperty, "Setting DepartmentName property did not raise correct PropertyChanged event.")
            Assert.AreEqual("DepartmentCode_NEW", dep.DepartmentCode, "Setting DepartmentCode property did not update model.")
        End Sub

        ''' <summary>
        ''' Verify getters reflect changes in model
        ''' </summary>
        <TestMethod>
        Public Sub ModelChangesFlowToProperties()
            ' Test ViewModel returns current value from model
            Dim dep As Department = New Department With {.DepartmentName = "DepartmentName", .DepartmentCode = "DepartmentCode"}
            Dim vm As New DepartmentViewModel(dep)

            vm.DepartmentName = "DepartmentName_NEW"
            Assert.AreEqual("DepartmentName_NEW", dep.DepartmentName, "DepartmentName property is not fetching the value from the model.")
            vm.DepartmentCode = "DepartmentCode_NEW"
            Assert.AreEqual("DepartmentCode_NEW", dep.DepartmentCode, "DepartmentCode property is not fetching the value from the model.")
        End Sub

        ''' <summary>
        ''' Verify NullArgumentExceptions are thrown where null is invalid
        ''' </summary>
        <TestMethod>
        Public Sub CheckNullArgumentExceptions()
            Dim ctor As Action = Sub()
                                     Dim x = New DepartmentViewModel(Nothing)
                                 End Sub

            Utilities.CheckNullArgumentException(ctor, "department", "ctor")
        End Sub
    End Class
End Namespace
