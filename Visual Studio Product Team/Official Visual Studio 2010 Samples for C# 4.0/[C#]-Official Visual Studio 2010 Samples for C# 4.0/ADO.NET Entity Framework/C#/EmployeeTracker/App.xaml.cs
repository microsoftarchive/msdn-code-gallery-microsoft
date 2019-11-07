// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)


namespace EmployeeTracker
{
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using EmployeeTracker.Common;
    using EmployeeTracker.EntityFramework;
    using EmployeeTracker.Fakes;
    using EmployeeTracker.Model.Interfaces;
    using EmployeeTracker.View;
    using EmployeeTracker.ViewModel;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Context is disposed when app exits.")]
    public partial class App : Application
    {
        /// <summary>
        /// The unit of work co-ordinating changes for the application
        /// </summary>
        private IEmployeeContext context;

        /// <summary>
        /// If true an fake in-memory context will be used
        /// If false an ADO.Net Entity Framework context will be used
        /// </summary>
        private bool useFakes = false;

        /// <summary>
        /// Lauches the entry form on startup
        /// </summary>
        /// <param name="e">Arguments of the startup event</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (this.useFakes)
            {
                this.context =  Generation.BuildFakeSession();
            }
            else
            {
                //NOTE: If there is not a Microsoft SQL Server Express instance available at .\SQLEXPRESS 
                //      you will need to update the "EmployeeEntities" connection string in App.config
                this.context = new EmployeeEntities();
            }

            IDepartmentRepository departmentRepository = new DepartmentRepository(this.context);
            IEmployeeRepository employeeRepository = new EmployeeRepository(this.context);
            IUnitOfWork unit = new UnitOfWork(this.context);

            MainViewModel main = new MainViewModel(unit, departmentRepository, employeeRepository);
            MainView window = new View.MainView { DataContext = main };
            window.Show();
        }

        /// <summary>
        /// Cleans up any resources on exit
        /// </summary>
        /// <param name="e">Arguments of the exit event</param>
        protected override void OnExit(ExitEventArgs e)
        {
            this.context.Dispose();

            base.OnExit(e);
        }
    }
}
