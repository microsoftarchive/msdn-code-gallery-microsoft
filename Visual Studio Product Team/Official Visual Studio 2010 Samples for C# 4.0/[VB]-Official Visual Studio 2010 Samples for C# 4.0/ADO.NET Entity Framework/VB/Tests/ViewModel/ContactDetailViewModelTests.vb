' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System.Collections.Generic
Imports EmployeeTracker.Model
Imports EmployeeTracker.ViewModel
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Tests.ViewModel

    ''' <summary>
    ''' Unit tests for ContactDetailViewModel
    ''' </summary>
    <TestClass>
    Public Class ContactDetailViewModelTests
        ''' <summary>
        ''' Verify BuildViewModel can create all contact detail types
        ''' </summary>
        <TestMethod>
        Public Sub BuildViewModel()
            Dim p As New Phone()
            Dim e As New Email()
            Dim a As New Address()

            Dim pvm = ContactDetailViewModel.BuildViewModel(p)
            Assert.IsInstanceOfType(pvm, GetType(PhoneViewModel), "Factory method created wrong ViewModel type.")
            Assert.AreEqual(p, pvm.Model, "Underlying model object on ViewModel is not correct.")

            Dim evm = ContactDetailViewModel.BuildViewModel(e)
            Assert.IsInstanceOfType(evm, GetType(EmailViewModel), "Factory method created wrong ViewModel type.")
            Assert.AreEqual(e, evm.Model, "Underlying model object on ViewModel is not correct.")

            Dim avm = ContactDetailViewModel.BuildViewModel(a)
            Assert.IsInstanceOfType(avm, GetType(AddressViewModel), "Factory method created wrong ViewModel type.")
            Assert.AreEqual(a, avm.Model, "Underlying model object on ViewModel is not correct.")
        End Sub

        ''' <summary>
        ''' Verify BuildViewModel does no throw when it processes an unrecognized type
        ''' </summary>
        <TestMethod>
        Public Sub BuildViewModelUnknownType()
            Dim f = New FakeContactDetail()
            Dim fvm = ContactDetailViewModel.BuildViewModel(f)
            Assert.IsNull(fvm, "BuildViewModel should return null when it doesn't know how to handle a type.")
        End Sub

        ''' <summary>
        ''' Verify NullArgumentExceptions are thrown where null is invalid
        ''' </summary>
        <TestMethod>
        Public Sub CheckNullArgumentExceptions()
            Utilities.CheckNullArgumentException(Sub() ContactDetailViewModel.BuildViewModel(Nothing), "detail", "BuildViewModel")
        End Sub

        ''' <summary>
        ''' Fake contact type to test BuildViewModelUnknownType
        ''' </summary>
        Private Class FakeContactDetail
            Inherits ContactDetail
            ''' <summary>
            ''' Gets valid values for the usage field
            ''' Stub implementation, just returns null
            ''' </summary>
            Public Overrides ReadOnly Property ValidUsageValues() As IEnumerable(Of String)
                Get
                    Return Nothing
                End Get
            End Property
        End Class
    End Class
End Namespace
