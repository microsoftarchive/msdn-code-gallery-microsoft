' Copyright © Microsoft Corporation.  All Rights Reserved.
' This code released under the terms of the 
' Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)



Imports Microsoft.VisualBasic
Imports System.Diagnostics.CodeAnalysis
Imports System.Windows
Imports EmployeeTracker.Common
Imports EmployeeTracker.EntityFramework
Imports EmployeeTracker.Fakes
Imports EmployeeTracker.Model.Interfaces
Imports EmployeeTracker.View
Imports EmployeeTracker.ViewModel

Namespace EmployeeTracker

    ''' <summary>
    ''' Interaction logic for App.xaml
    ''' </summary>
    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification := "Context is disposed when app exits.")>
    Partial Public Class App
        Inherits Application
        ''' <summary>
        ''' The unit of work co-ordinating changes for the application
        ''' </summary>
        Private context As IEmployeeContext

        ''' <summary>
        ''' If true an fake in-memory context will be used
        ''' If false an ADO.Net Entity Framework context will be used
        ''' </summary>
        Private useFakes As Boolean = False

        ''' <summary>
        ''' Lauches the entry form on startup
        ''' </summary>
        ''' <param name="e">Arguments of the startup event</param>
        Protected Overrides Sub OnStartup(ByVal e As StartupEventArgs)
            MyBase.OnStartup(e)

            If Me.useFakes Then
                Me.context = Generation.BuildFakeSession()
            Else
                ' NOTE: If there is not a Microsoft SQL Server Express instance available at .\SQLEXPRESS 
                '       you will need to update the "EmployeeEntities" connection string in App.config
                Me.context = New EmployeeEntities()
            End If

            Dim departmentRepository As IDepartmentRepository = New DepartmentRepository(Me.context)
            Dim employeeRepository As IEmployeeRepository = New EmployeeRepository(Me.context)
            Dim unit As IUnitOfWork = New UnitOfWork(Me.context)

            Dim main As New MainViewModel(unit, departmentRepository, employeeRepository)
            Dim window As MainView = New View.MainView With {.DataContext = main}
            window.Show()
        End Sub

        ''' <summary>
        ''' Cleans up any resources on exit
        ''' </summary>
        ''' <param name="e">Arguments of the exit event</param>
        Protected Overrides Sub OnExit(ByVal e As ExitEventArgs)
            Me.context.Dispose()

            MyBase.OnExit(e)
        End Sub
    End Class
End Namespace
