using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using Microsoft.SharePoint.Phone.Application;
using Microsoft.SharePoint.Client;

namespace MultipleSPListAppSingleProject
{
    public partial class App : Application
    {
        private const string TargetSiteUrl = "YOUR SITE"; // Specify your site here.

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        private static ListViewModel m_PrimaryViewModel;
        /// <summary>
        /// Provides access to AllItems ViewModel of the SharePoint List
        /// </summary>
        public static ListViewModel PrimaryViewModel
        {
            get
            {
                if (m_PrimaryViewModel == null)
                    m_PrimaryViewModel = new ListViewModel { DataProvider = App.PrimaryDataProvider };

                return m_PrimaryViewModel;
            }
            set
            {
                m_PrimaryViewModel = value;
            }
        }

        private static ListViewModel m_OrdersListViewModel;
        /// <summary>
        /// Provides access to AllItems ViewModel of the SharePoint List
        /// </summary>
        public static ListViewModel OrdersListViewModel
        {
            get
            {
                if (m_OrdersListViewModel == null)
                    m_OrdersListViewModel = new ListViewModel { DataProvider = App.OrdersListDataProvider };
                    //m_SecondaryViewModel = new ListViewModel();

                return m_OrdersListViewModel;
            }
            set
            {
                m_OrdersListViewModel = value;
            }
        }

        private static ListDataProvider m_PrimaryDataProvider;
        /// <summary>
        /// Provides operations for fetching and storing SharePoint List data
        /// </summary> 
        public static ListDataProvider PrimaryDataProvider
        {
            get
            {
                if (m_PrimaryDataProvider != null)
                    return m_PrimaryDataProvider;

                m_PrimaryDataProvider = new ListDataProvider();
                m_PrimaryDataProvider.ListTitle = "Marketing Team";  // Update the value if you have name your list orther than Marketing Team
                m_PrimaryDataProvider.SiteUrl = new Uri(TargetSiteUrl);

                return m_PrimaryDataProvider;
            }
        }

        private static ListDataProvider m_OrdersListDataProvider;
        /// <summary>
        /// Provides operations for fetching and storing SharePoint List data
        /// </summary> 
        public static ListDataProvider OrdersListDataProvider
        {
            get
            {
                if (m_OrdersListDataProvider != null)
                    return m_OrdersListDataProvider;

                m_OrdersListDataProvider = new ListDataProvider();
                m_OrdersListDataProvider.ListTitle = "Orders";
                m_OrdersListDataProvider.SiteUrl = new Uri(TargetSiteUrl);

                return m_OrdersListDataProvider;
            }
        }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are being GPU accelerated with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;
            }

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();
        }


        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
		 Converter.RegisterDisplayFieldValueConverter(Microsoft.SharePoint.Client.FieldType.Attachments,
                (string fieldName, ListItem item, ConversionContext context) =>
                {
                    return null;
                });

        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            //Restore PrimaryViewModel from transient storage.
            if (PhoneApplicationService.Current.State.ContainsKey(PrimaryDataProvider.ListTitle))
                App.PrimaryViewModel = (ListViewModel)PhoneApplicationService.Current.State[PrimaryDataProvider.ListTitle];

            //Restore SecondaryViewModel from transient storage.
            if (PhoneApplicationService.Current.State.ContainsKey(OrdersListDataProvider.ListTitle))
                App.OrdersListViewModel = (ListViewModel)PhoneApplicationService.Current.State[OrdersListDataProvider.ListTitle];
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
            //Save PrimaryViewModel to transient storage.
            if (PhoneApplicationService.Current.State.ContainsKey(PrimaryDataProvider.ListTitle))
                PhoneApplicationService.Current.State[PrimaryDataProvider.ListTitle] = App.PrimaryViewModel;
            else
                PhoneApplicationService.Current.State.Add(PrimaryDataProvider.ListTitle, App.PrimaryViewModel);

            // Save SecondaryViewModel to transient storage.
            if (PhoneApplicationService.Current.State.ContainsKey(OrdersListDataProvider.ListTitle))
                PhoneApplicationService.Current.State[OrdersListDataProvider.ListTitle] = App.OrdersListViewModel;
            else
                PhoneApplicationService.Current.State.Add(OrdersListDataProvider.ListTitle, App.OrdersListViewModel);
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {

        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}