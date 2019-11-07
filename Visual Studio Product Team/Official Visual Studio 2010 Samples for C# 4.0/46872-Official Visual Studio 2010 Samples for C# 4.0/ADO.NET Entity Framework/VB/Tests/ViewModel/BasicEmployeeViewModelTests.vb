' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System
Imports EmployeeTracker.Model
Imports EmployeeTracker.ViewModel
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Tests

Namespace Tests.ViewModel

    ''' <summary>
    ''' Unit tests for BasicEmployeeViewModelTests
    ''' </summary>
    <TestClass>
    Public Class BasicEmployeeViewModelTests
        ''' <summary>
        ''' Verify getters and setters on ViewModel affect underlying data and notify changes
        ''' </summary>
        <TestMethod>
        Public Sub PropertyGetAndSet()
            ' Test initial properties are surfaced in ViewModel
            Dim em As Employee = New Employee With {.Title = "Title", .FirstName = "FirstName", .LastName = "LastName", .Position = "Position", .BirthDate = New DateTime(2000, 1, 1), .HireDate = New DateTime(2000, 2, 2)}
            Dim vm As New BasicEmployeeViewModel(em)
            Assert.AreEqual(em, vm.Model, "Bound object property did not return object from model.")
            Assert.AreEqual("Title", vm.Title, "Title property did not return value from model.")
            Assert.AreEqual("FirstName", vm.FirstName, "FirstName property did not return value from model.")
            Assert.AreEqual("LastName", vm.LastName, "LastName property did not return value from model.")
            Assert.AreEqual("Position", vm.Position, "Position property did not return value from model.")
            Assert.AreEqual(New DateTime(2000, 1, 1), vm.BirthDate, "BirthDate property did not return value from model.")
            Assert.AreEqual(New DateTime(2000, 2, 2), vm.HireDate, "HireDate property did not return value from model.")
            Assert.AreEqual(Nothing, vm.TerminationDate, "TerminationDate property did not return value from model.")

            ' Test changing properties updates Model and raises PropertyChanged
            Dim lastProperty As String
            AddHandler vm.PropertyChanged, Sub(sender, e) lastProperty = e.PropertyName

            lastProperty = Nothing
            vm.Title = "NEW_Title"
            Assert.AreEqual("Title", lastProperty, "Setting Title property did not raise correct PropertyChanged event.")
            Assert.AreEqual("NEW_Title", em.Title, "Setting Title property did not update model.")

            lastProperty = Nothing
            vm.FirstName = "NEW_FirstName"
            Assert.AreEqual("FirstName", lastProperty, "Setting FirstName property did not raise correct PropertyChanged event.")
            Assert.AreEqual("NEW_FirstName", em.FirstName, "Setting FirstName property did not update model.")

            lastProperty = Nothing
            vm.LastName = "NEW_LastName"
            Assert.AreEqual("LastName", lastProperty, "Setting LastName property did not raise correct PropertyChanged event.")
            Assert.AreEqual("NEW_LastName", em.LastName, "Setting LastName property did not update model.")

            lastProperty = Nothing
            vm.Position = "NEW_Position"
            Assert.AreEqual("Position", lastProperty, "Setting Position property did not raise correct PropertyChanged event.")
            Assert.AreEqual("NEW_Position", em.Position, "Setting Position property did not update model.")

            lastProperty = Nothing
            vm.BirthDate = New DateTime(2001, 1, 1)
            Assert.AreEqual("BirthDate", lastProperty, "Setting BirthDate property did not raise correct PropertyChanged event.")
            Assert.AreEqual(New DateTime(2001, 1, 1), em.BirthDate, "Setting BirthDate property did not update model.")

            lastProperty = Nothing
            vm.HireDate = New DateTime(2001, 2, 2)
            Assert.AreEqual("HireDate", lastProperty, "Setting HireDate property did not raise correct PropertyChanged event.")
            Assert.AreEqual(New DateTime(2001, 2, 2), em.HireDate, "Setting HireDate property did not update model.")

            lastProperty = Nothing
            vm.TerminationDate = New DateTime(2001, 3, 3)
            Assert.AreEqual("TerminationDate", lastProperty, "Setting TerminationDate property did not raise correct PropertyChanged event.")
            Assert.AreEqual(New DateTime(2001, 3, 3), em.TerminationDate, "Setting TerminationDate property did not update model.")
        End Sub

        ''' <summary>
        ''' Verify getters reflect changes in model
        ''' </summary>
        <TestMethod>
        Public Sub ModelChangesFlowToProperties()
            ' Test ViewModel returns current value from model
            Dim em As Employee = New Employee With {.Title = "Title", .FirstName = "FirstName", .LastName = "LastName", .Position = "Position", .BirthDate = New DateTime(2000, 1, 1), .HireDate = New DateTime(2000, 2, 2)}
            Dim vm As New BasicEmployeeViewModel(em)

            em.Title = "NEW_Title"
            Assert.AreEqual("NEW_Title", vm.Title, "Title property is not fetching the value from the model.")
            em.FirstName = "NEW_FirstName"
            Assert.AreEqual("NEW_FirstName", vm.FirstName, "FirstName property is not fetching the value from the model.")
            em.LastName = "NEW_LastName"
            Assert.AreEqual("NEW_LastName", vm.LastName, "LastName property is not fetching the value from the model.")
            em.Position = "NEW_Position"
            Assert.AreEqual("NEW_Position", vm.Position, "Position property is not fetching the value from the model.")
            em.BirthDate = New DateTime(2001, 1, 1)
            Assert.AreEqual(New DateTime(2001, 1, 1), vm.BirthDate, "BirthDate property is not fetching the value from the model.")
            em.HireDate = New DateTime(2001, 2, 2)
            Assert.AreEqual(New DateTime(2001, 2, 2), vm.HireDate, "HireDate property is not fetching the value from the model.")
            em.TerminationDate = New DateTime(2001, 3, 3)
            Assert.AreEqual(New DateTime(2001, 3, 3), vm.TerminationDate, "TerminationDate property is not fetching the value from the model.")
        End Sub

        ''' <summary>
        ''' Verify NullArgumentExceptions are thrown where null is invalid
        ''' </summary>
        <TestMethod>
        Public Sub CheckNullArgumentExceptions()
            Dim ctor As Action = Sub()
                                     Dim x = New BasicEmployeeViewModel(Nothing)
                                 End Sub

            Utilities.CheckNullArgumentException(ctor, "employee", "ctor")
        End Sub
    End Class
End Namespace
