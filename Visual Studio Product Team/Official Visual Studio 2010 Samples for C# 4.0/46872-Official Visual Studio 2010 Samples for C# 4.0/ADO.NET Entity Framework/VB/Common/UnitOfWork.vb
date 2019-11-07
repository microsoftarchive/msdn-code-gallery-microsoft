' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System
Imports System.Globalization
Imports System.Linq
Imports EmployeeTracker.Model

Namespace EmployeeTracker.Common

    ''' <summary>
    ''' Encapsulates changes to underlying data stored in an EmployeeContext
    ''' </summary>
    Public Class UnitOfWork
        Implements IUnitOfWork
        ''' <summary>
        ''' The underlying context tracking changes
        ''' </summary>
        Private underlyingContext As IEmployeeContext

        ''' <summary>
        ''' Initializes a new instance of the UnitOfWork class.
        ''' Changes registered in the UnitOfWork are recorded in the supplied context
        ''' </summary>
        ''' <param name="context">The underlying context for this UnitOfWork</param>
        Public Sub New(ByVal context As IEmployeeContext)
            If context Is Nothing Then
                Throw New ArgumentNullException("context")
            End If

            Me.underlyingContext = context
        End Sub

        ''' <summary>
        ''' Save all pending changes in this UnitOfWork
        ''' </summary>
        Public Sub Save() Implements IUnitOfWork.Save
            Me.underlyingContext.Save()
        End Sub

        ''' <summary>
        ''' Creates a new instance of the specified object type
        ''' NOTE: This pattern is used to allow the use of change tracking proxies
        '''       when running against the Entity Framework.
        ''' </summary>
        ''' <typeparam name="T">The type to create</typeparam>
        ''' <returns>The newly created object</returns>
        Public Function CreateObject(Of T As Class)() As T Implements IUnitOfWork.CreateObject
            Return Me.underlyingContext.CreateObject(Of T)()
        End Function

        ''' <summary>
        ''' Registers the addition of a new department
        ''' </summary>
        ''' <param name="department">The department to add</param>
        ''' <exception cref="InvalidOperationException">Thrown if department is already added to UnitOfWork</exception>
        Public Sub AddDepartment(ByVal department As Department) Implements IUnitOfWork.AddDepartment
            If department Is Nothing Then
                Throw New ArgumentNullException("department")
            End If

            Me.CheckEntityDoesNotBelongToUnitOfWork(department)
            Me.underlyingContext.Departments.AddObject(department)
        End Sub

        ''' <summary>
        ''' Registers the addition of a new employee
        ''' </summary>
        ''' <param name="employee">The employee to add</param>
        ''' <exception cref="InvalidOperationException">Thrown if employee is already added to UnitOfWork</exception>
        Public Sub AddEmployee(ByVal employee As Employee) Implements IUnitOfWork.AddEmployee
            If employee Is Nothing Then
                Throw New ArgumentNullException("employee")
            End If

            Me.CheckEntityDoesNotBelongToUnitOfWork(employee)
            Me.underlyingContext.Employees.AddObject(employee)
        End Sub

        ''' <summary>
        ''' Registers the addition of a new contact detail
        ''' </summary>
        ''' <param name="employee">The employee to add the contact detail to</param>
        ''' <param name="detail">The contact detail to add</param>
        ''' <exception cref="InvalidOperationException">Thrown if employee is not tracked by this UnitOfWork</exception>
        ''' <exception cref="InvalidOperationException">Thrown if contact detail is already added to UnitOfWork</exception>
        Public Sub AddContactDetail(ByVal employee As Employee, ByVal detail As ContactDetail) Implements IUnitOfWork.AddContactDetail
            If employee Is Nothing Then
                Throw New ArgumentNullException("employee")
            End If

            If detail Is Nothing Then
                Throw New ArgumentNullException("detail")
            End If

            Me.CheckEntityDoesNotBelongToUnitOfWork(detail)
            Me.CheckEntityBelongsToUnitOfWork(employee)

            Me.underlyingContext.ContactDetails.AddObject(detail)
            employee.ContactDetails.Add(detail)
        End Sub

        ''' <summary>
        ''' Registers the removal of an existing department
        ''' </summary>
        ''' <param name="department">The department to remove</param>
        ''' <exception cref="InvalidOperationException">Thrown if department is not tracked by this UnitOfWork</exception>
        Public Sub RemoveDepartment(ByVal department As Department) Implements IUnitOfWork.RemoveDepartment
            If department Is Nothing Then
                Throw New ArgumentNullException("department")
            End If

            Me.CheckEntityBelongsToUnitOfWork(department)
            For Each emp In department.Employees.ToList()
                emp.Department = Nothing
            Next emp

            Me.underlyingContext.Departments.DeleteObject(department)
        End Sub

        ''' <summary>
        ''' Registers the removal of an existing employee
        ''' </summary>
        ''' <param name="employee">The employee to remove</param>
        ''' <exception cref="InvalidOperationException">Thrown if employee is not tracked by this UnitOfWork</exception>
        Public Sub RemoveEmployee(ByVal employee As Employee) Implements IUnitOfWork.RemoveEmployee
            If employee Is Nothing Then
                Throw New ArgumentNullException("employee")
            End If

            Me.CheckEntityBelongsToUnitOfWork(employee)
            employee.Manager = Nothing
            For Each e In employee.Reports.ToList()
                e.Manager = Nothing
            Next e

            Me.underlyingContext.Employees.DeleteObject(employee)
        End Sub

        ''' <summary>
        ''' Registers the removal of an existing contact detail
        ''' </summary>
        ''' <param name="employee">The employee to remove the contact detail from</param>
        ''' <param name="detail">The contact detail to remove</param>
        ''' <exception cref="InvalidOperationException">Thrown if employee is not tracked by this UnitOfWork</exception>
        ''' <exception cref="InvalidOperationException">Thrown if contact detail is not tracked by this UnitOfWork</exception>
        Public Sub RemoveContactDetail(ByVal employee As Employee, ByVal detail As ContactDetail) Implements IUnitOfWork.RemoveContactDetail
            If employee Is Nothing Then
                Throw New ArgumentNullException("employee")
            End If

            If detail Is Nothing Then
                Throw New ArgumentNullException("detail")
            End If

            Me.CheckEntityBelongsToUnitOfWork(detail)
            Me.CheckEntityBelongsToUnitOfWork(employee)
            If Not employee.ContactDetails.Contains(detail) Then
                Throw New InvalidOperationException("The supplied ContactDetail does not belong to the supplied Employee")
            End If

            employee.ContactDetails.Remove(detail)
            Me.underlyingContext.ContactDetails.DeleteObject(detail)
        End Sub

        ''' <summary>
        ''' Verifies that the specified entity is tracked in this UnitOfWork
        ''' </summary>
        ''' <param name="entity">The object to search for</param>
        ''' <exception cref="InvalidOperationException">Thrown if object is not tracked by this UnitOfWork</exception>
        Private Sub CheckEntityBelongsToUnitOfWork(ByVal entity As Object)
            If Not Me.underlyingContext.IsObjectTracked(entity) Then
                Throw New InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "The supplied {0} is not part of this Unit of Work.", entity.GetType().Name))
            End If
        End Sub

        ''' <summary>
        ''' Verifies that the specified entity is not tracked in this UnitOfWork
        ''' </summary>
        ''' <param name="entity">The object to search for</param>
        ''' <exception cref="InvalidOperationException">Thrown if object is tracked by this UnitOfWork</exception>
        Private Sub CheckEntityDoesNotBelongToUnitOfWork(ByVal entity As Object)
            If Me.underlyingContext.IsObjectTracked(entity) Then
                Throw New InvalidOperationException(String.Format(CultureInfo.InvariantCulture, "The supplied {0} is already part of this Unit of Work.", entity.GetType().Name))
            End If
        End Sub
    End Class
End Namespace
