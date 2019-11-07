' Copyright (c) Microsoft Corporation.  All rights reserved.


Imports Microsoft.VisualBasic
Imports System
Imports System.Linq
Imports EmployeeTracker.Common
Imports EmployeeTracker.Fakes
Imports EmployeeTracker.Model
Imports Microsoft.VisualStudio.TestTools.UnitTesting

Namespace Tests.Common

    ''' <summary>
    ''' Verify change traking abilities of UnitOfWork
    ''' </summary>
    <TestClass>
    Public Class UnitOfWorkTests
        ''' <summary>
        ''' Verify NullArgumentExceptions are thrown where null is invalid
        ''' </summary>
        <TestMethod>
        Public Sub NullArgumentChecks()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)

                Dim ctor As Action = Sub()
                                         Dim x = New UnitOfWork(Nothing)
                                     End Sub

                Utilities.CheckNullArgumentException(ctor, "context", "ctor")

                Utilities.CheckNullArgumentException(Sub() unit.AddEmployee(Nothing), "employee", "AddEmployee")
                Utilities.CheckNullArgumentException(Sub() unit.AddDepartment(Nothing), "department", "AddDepartment")
                Utilities.CheckNullArgumentException(Sub() unit.AddContactDetail(New Employee(), Nothing), "detail", "AddContactDetail")
                Utilities.CheckNullArgumentException(Sub() unit.AddContactDetail(Nothing, New Phone()), "employee", "AddContactDetail")

                Utilities.CheckNullArgumentException(Sub() unit.RemoveEmployee(Nothing), "employee", "RemoveEmployee")
                Utilities.CheckNullArgumentException(Sub() unit.RemoveDepartment(Nothing), "department", "RemoveDepartment")
                Utilities.CheckNullArgumentException(Sub() unit.RemoveContactDetail(Nothing, New Phone()), "employee", "RemoveContactDetail")
                Utilities.CheckNullArgumentException(Sub() unit.RemoveContactDetail(New Employee(), Nothing), "detail", "RemoveContactDetail")
            End Using
        End Sub

        ''' <summary>
        ''' Verify CreateObject returns a valid object for types in model
        ''' </summary>
        <TestMethod>
        Public Sub CreateObject()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)

                Dim entity As Object = unit.CreateObject(Of Department)()
                Assert.IsInstanceOfType(entity, GetType(Department), "Department did not get created.")

                entity = unit.CreateObject(Of Employee)()
                Assert.IsInstanceOfType(entity, GetType(Employee), "Employee did not get created.")

                entity = unit.CreateObject(Of Email)()
                Assert.IsInstanceOfType(entity, GetType(Email), "Email did not get created.")

                entity = unit.CreateObject(Of Phone)()
                Assert.IsInstanceOfType(entity, GetType(Phone), "Phone did not get created.")

                entity = unit.CreateObject(Of Address)()
                Assert.IsInstanceOfType(entity, GetType(Address), "Address did not get created.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify department gets added to underlying context
        ''' </summary>
        <TestMethod>
        Public Sub AddDepartment()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)

                Dim dep As New Department()
                unit.AddDepartment(dep)
                Assert.IsTrue(ctx.Departments.Contains(dep), "Department was not added to underlying context.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify exception on adding a department that is already in the underlying context
        ''' </summary>
        <TestMethod>
        Public Sub AddDepartmentAlreadyInUnitOfWork()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)

                Dim dep As New Department()
                unit.AddDepartment(dep)

                Try
                    unit.AddDepartment(dep)
                    Assert.Fail("Adding an Department that was already added did not throw.")
                Catch ex As InvalidOperationException
                    Assert.AreEqual("The supplied Department is already part of this Unit of Work.", ex.Message)
                End Try
            End Using
        End Sub

        ''' <summary>
        ''' Verify employee gets added to underlying context
        ''' </summary>
        <TestMethod>
        Public Sub AddEmployee()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)

                Dim emp As New Employee()
                unit.AddEmployee(emp)
                Assert.IsTrue(ctx.Employees.Contains(emp), "Employee was not added to underlying context.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify exception on adding an employee that is already in the underlying context
        ''' </summary>
        <TestMethod>
        Public Sub AddEmployeeAlreadyInUnitOfWork()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)

                Dim emp As New Employee()
                unit.AddEmployee(emp)

                Try
                    unit.AddEmployee(emp)
                    Assert.Fail("Adding an Employee that was already added did not throw.")
                Catch ex As InvalidOperationException
                    Assert.AreEqual("The supplied Employee is already part of this Unit of Work.", ex.Message)
                End Try
            End Using
        End Sub

        ''' <summary>
        ''' Verify contact detail gets added to underlying context
        ''' Contact detail created by calling constructor on class
        ''' </summary>
        <TestMethod>
        Public Sub AddContactDetailFromDefaultConstructor()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)
                Dim emp As New Employee()
                unit.AddEmployee(emp)

                Dim cd As ContactDetail = New Address()
                unit.AddContactDetail(emp, cd)
                Assert.IsTrue(ctx.ContactDetails.Contains(cd), "ContactDetail was not added to underlying context.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify exception on adding a contact detail that is already in the underlying context
        ''' </summary>
        <TestMethod>
        Public Sub AddContactDetailAlreadyInUnitOfWork()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)

                Dim emp As New Employee()
                Dim detail As ContactDetail = New Phone()
                unit.AddEmployee(emp)
                unit.AddContactDetail(emp, detail)

                Try
                    unit.AddContactDetail(emp, detail)
                    Assert.Fail("Adding an ContactDetail that was already added did not throw.")
                Catch ex As InvalidOperationException
                    Assert.AreEqual("The supplied Phone is already part of this Unit of Work.", ex.Message)
                End Try
            End Using
        End Sub

        ''' <summary>
        ''' Verify exception on adding a contact detail to an employee not in the underlying context
        ''' </summary>
        <TestMethod>
        Public Sub AddContactDetailToEmployeeOutsideUnitOfWork()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)
                Dim emp As New Employee()
                Dim detail As ContactDetail = New Email()

                Try
                    unit.AddContactDetail(emp, detail)
                    Assert.Fail("Adding a contact detail to an employee outside the Unit of Work did not throw.")
                Catch ex As InvalidOperationException
                    Assert.AreEqual("The supplied Employee is not part of this Unit of Work.", ex.Message)
                End Try
            End Using
        End Sub

        ''' <summary>
        ''' Verify department can be removed from underlying context
        ''' </summary>
        <TestMethod>
        Public Sub RemoveDepartment()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)
                Dim dep As New Department()
                unit.AddDepartment(dep)

                unit.RemoveDepartment(dep)
                Assert.IsFalse(ctx.Departments.Contains(dep), "Department was not removed from underlying context.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify employees get department cleared when their department is deleted
        ''' </summary>
        <TestMethod>
        Public Sub RemoveDepartmentWithEmployees()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)
                Dim dep As New Department()
                Dim emp As New Employee()
                unit.AddDepartment(dep)
                unit.AddEmployee(emp)
                emp.Department = dep

                unit.RemoveDepartment(dep)
                Assert.IsFalse(ctx.Departments.Contains(dep), "Department was not removed from underlying context.")
                Assert.IsNull(emp.Department, "Employee.Department property has not been nulled when deleting department.")
                Assert.IsNull(emp.DepartmentId, "Employee.DepartmentId property has not been nulled when deleting department.")
                Assert.AreEqual(0, dep.Employees.Count, "Department.Employees collection was not cleared when deleting department.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify exception when removing department not in current underlying context
        ''' </summary>
        <TestMethod>
        Public Sub RemoveDepartmentOutsideUnitOfWork()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)
                Try
                    unit.RemoveDepartment(New Department())
                    Assert.Fail("Removing a Department that was not added to Unit of Work did not throw.")
                Catch ex As InvalidOperationException
                    Assert.AreEqual("The supplied Department is not part of this Unit of Work.", ex.Message)
                End Try
            End Using
        End Sub

        ''' <summary>
        ''' Verify employee can be removed from underlying context
        ''' </summary>
        <TestMethod>
        Public Sub RemoveEmployee()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)
                Dim emp As New Employee()
                unit.AddEmployee(emp)

                unit.RemoveEmployee(emp)
                Assert.IsFalse(ctx.Employees.Contains(emp), "Employee was not removed from underlying context.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify employee can be removed from underlying context
        ''' And that employee gets un-assigned from manager
        ''' </summary>
        <TestMethod>
        Public Sub RemoveEmployeeWithManager()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)
                Dim emp As New Employee()
                Dim man As New Employee()
                unit.AddEmployee(emp)
                unit.AddEmployee(man)
                emp.Manager = man

                unit.RemoveEmployee(emp)
                Assert.IsFalse(ctx.Employees.Contains(emp), "Employee was not removed from underlying context.")
                Assert.AreEqual(0, man.Reports.Count, "Employee was not removed from managers reports.")
                Assert.IsNull(emp.Manager, "Manager property on Employee was not cleared.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify employee can be removed from underlying context
        ''' And that any reports get un-assigned
        ''' </summary>
        <TestMethod>
        Public Sub RemoveEmployeeWithReports()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)
                Dim emp As New Employee()
                Dim man As New Employee()
                unit.AddEmployee(emp)
                unit.AddEmployee(man)
                emp.Manager = man

                unit.RemoveEmployee(man)
                Assert.IsFalse(ctx.Employees.Contains(man), "Employee was not removed from underlying context.")
                Assert.AreEqual(0, man.Reports.Count, "Employee was not removed from managers reports.")
                Assert.IsNull(emp.Manager, "Manager property on Employee was not cleared.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify exception when removing employee not in current underlying context
        ''' </summary>
        <TestMethod>
        Public Sub RemoveEmployeeOutsideUnitOfWork()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)

                Try
                    unit.RemoveEmployee(New Employee())
                    Assert.Fail("Removing an Employee that was not added to Unit of Work did not throw.")
                Catch ex As InvalidOperationException
                    Assert.AreEqual("The supplied Employee is not part of this Unit of Work.", ex.Message)
                End Try
            End Using
        End Sub

        ''' <summary>
        ''' Verify contact detail can be removed from underlying context
        ''' </summary>
        <TestMethod>
        Public Sub RemoveContactDetail()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)

                Dim emp As New Employee()
                Dim detail As ContactDetail = New Phone()
                unit.AddEmployee(emp)
                unit.AddContactDetail(emp, detail)

                unit.RemoveContactDetail(emp, detail)
                Assert.IsFalse(ctx.ContactDetails.Contains(detail), "ContactDetail was not removed from underlying context.")
                Assert.IsFalse(emp.ContactDetails.Contains(detail), "ContactDetail is still in collection on Employee after being removed via Unit of Work.")
            End Using
        End Sub

        ''' <summary>
        ''' Verify exception when removing contact detail not in underlying context
        ''' </summary>
        <TestMethod>
        Public Sub RemoveContactDetailOutsideUnitOfWork()
            Using ctx As New FakeEmployeeContext()
                Dim unit As New UnitOfWork(ctx)
                Try
                    unit.RemoveContactDetail(New Employee(), New Address())
                    Assert.Fail("Removing a ContactDetail that was not added to Unit of Work did not throw.")
                Catch ex As InvalidOperationException
                    Assert.AreEqual("The supplied Address is not part of this Unit of Work.", ex.Message)
                End Try
            End Using
        End Sub
    End Class
End Namespace
