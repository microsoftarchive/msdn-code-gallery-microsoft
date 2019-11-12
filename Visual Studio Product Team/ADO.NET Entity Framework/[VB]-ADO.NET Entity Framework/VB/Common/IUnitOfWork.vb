' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System
Imports EmployeeTracker.Model

Namespace EmployeeTracker.Common

    ''' <summary>
    ''' Encapsulates changes to underlying data
    ''' </summary>
    Public Interface IUnitOfWork
        ''' <summary>
        ''' Save all pending changes in this UnitOfWork
        ''' </summary>
        Sub Save()

        ''' <summary>
        ''' Creates a new instance of the specified object type
        ''' NOTE: This pattern is used to allow the use of change tracking proxies
        '''       when running against the Entity Framework.
        ''' </summary>
        ''' <typeparam name="T">The type to create</typeparam>
        ''' <returns>The newly created object</returns>
        Function CreateObject(Of T As Class)() As T

        ''' <summary>
        ''' Registers the addition of a new department
        ''' </summary>
        ''' <param name="department">The department to add</param>
        ''' <exception cref="InvalidOperationException">Thrown if department is already added to UnitOfWork</exception>
        Sub AddDepartment(ByVal department As Department)

        ''' <summary>
        ''' Registers the addition of a new employee
        ''' </summary>
        ''' <param name="employee">The employee to add</param>
        ''' <exception cref="InvalidOperationException">Thrown if employee is already added to UnitOfWork</exception>
        Sub AddEmployee(ByVal employee As Employee)

        ''' <summary>
        ''' Registers the addition of a new contact detail
        ''' </summary>
        ''' <param name="employee">The employee to add the contact detail to</param>
        ''' <param name="detail">The contact detail to add</param>
        ''' <exception cref="InvalidOperationException">Thrown if employee is not tracked by this UnitOfWork</exception>
        ''' <exception cref="InvalidOperationException">Thrown if contact detail is already added to UnitOfWork</exception>
        Sub AddContactDetail(ByVal employee As Employee, ByVal detail As ContactDetail)

        ''' <summary>
        ''' Registers the removal of an existing department
        ''' </summary>
        ''' <param name="department">The department to remove</param>
        ''' <exception cref="InvalidOperationException">Thrown if department is not tracked by this UnitOfWork</exception>
        Sub RemoveDepartment(ByVal department As Department)

        ''' <summary>
        ''' Registers the removal of an existing employee
        ''' </summary>
        ''' <param name="employee">The employee to remove</param>
        ''' <exception cref="InvalidOperationException">Thrown if employee is not tracked by this UnitOfWork</exception>
        Sub RemoveEmployee(ByVal employee As Employee)

        ''' <summary>
        ''' Registers the removal of an existing contact detail
        ''' </summary>
        ''' <param name="employee">The employee to remove the contact detail from</param>
        ''' <param name="detail">The contact detail to remove</param>
        ''' <exception cref="InvalidOperationException">Thrown if employee is not tracked by this UnitOfWork</exception>
        ''' <exception cref="InvalidOperationException">Thrown if contact detail is not tracked by this UnitOfWork</exception>
        Sub RemoveContactDetail(ByVal employee As Employee, ByVal detail As ContactDetail)
    End Interface
End Namespace
