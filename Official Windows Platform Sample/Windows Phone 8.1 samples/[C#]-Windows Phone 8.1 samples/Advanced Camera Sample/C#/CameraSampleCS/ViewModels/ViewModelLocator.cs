// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="ViewModelLocator.cs">
//   Copyright (c) 2014 Microsoft Corporation. All rights reserved.
// </copyright>
// <summary>
//   Use of this sample source code is subject to the terms of the Microsoft license
//   agreement under which you licensed this sample source code and is provided AS-IS.
//   If you did not accept the terms of the license agreement, you are not authorized
//   to use this sample source code. For the terms of the license, please see the
//   license agreement between you and Microsoft.<br/><br/>
//   To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604.
// </summary>
// ----------------------------------------------------------------------------

namespace CameraSampleCS.ViewModels
{
    using System;
    using System.Collections.Generic;
    using CameraSampleCS.Services.Camera;
    using CameraSampleCS.Services.Navigation;
    using CameraSampleCS.Services.Settings;
    using CameraSampleCS.Services.Storage;
    using CameraSampleCS.Views;

    /// <summary>
    /// This class contains static references to all view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    /// <remarks>
    /// In the real-world application you may want to use <c>IoC</c> container
    /// to control objects lifetime.
    /// </remarks>
    public sealed class ViewModelLocator
    {
        #region Fields

        /// <summary>
        /// Lazily created <see cref="INavigationService"/> implementation.
        /// </summary>
        private static Lazy<INavigationService> LazyNavigationService = new Lazy<INavigationService>(() => new NavigationService());

        /// <summary>
        /// Lazily created <see cref="IStorageService"/> implementation.
        /// </summary>
        private static Lazy<IStorageService> LazyStorageService = new Lazy<IStorageService>(() => new StorageService());

        /// <summary>
        /// Lazily created <see cref="ISettingsProvider"/> implementation.
        /// </summary>
        private static Lazy<ISettingsProvider> LazySettingsProvider = new Lazy<ISettingsProvider>(() => new SettingsProvider());

        /// <summary>
        /// Lazily created <see cref="ICameraProvider"/> implementation.
        /// </summary>
        private static Lazy<ICameraProvider> LazyCameraProvider = new Lazy<ICameraProvider>(() => new CameraProvider());

        /// <summary>
        /// Lazily created view model for the <see cref="MainPage"/>.
        /// </summary>
        private static Lazy<MainPageViewModel> LazyMainPageViewModel = new Lazy<MainPageViewModel>(() => new MainPageViewModel(
                ViewModelLocator.LazyNavigationService.Value,
                ViewModelLocator.LazyCameraProvider.Value,
                ViewModelLocator.LazyStorageService.Value,
                ViewModelLocator.LazySettingsProvider.Value));

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets the view model for the <see cref="MainPage"/>.
        /// </summary>
        public MainPageViewModel MainPageViewModel
        {
            get
            {
                return ViewModelLocator.LazyMainPageViewModel.Value;
            }
        }

        #endregion // Properties

        #region Public methods

        /// <summary>
        /// Cleans up the existing view models.
        /// </summary>
        public static void Cleanup()
        {
            ViewModelLocator.CleanupInstances(new object[]
            {
                ViewModelLocator.LazyNavigationService != null ? ViewModelLocator.LazyNavigationService.Value : null,
                ViewModelLocator.LazySettingsProvider  != null ? ViewModelLocator.LazySettingsProvider.Value  : null,
                ViewModelLocator.LazyStorageService    != null ? ViewModelLocator.LazyStorageService.Value    : null,
                ViewModelLocator.LazyCameraProvider    != null ? ViewModelLocator.LazyCameraProvider.Value    : null,
                ViewModelLocator.LazyMainPageViewModel != null ? ViewModelLocator.LazyMainPageViewModel.Value : null
            });

            ViewModelLocator.LazyNavigationService  = null;
            ViewModelLocator.LazySettingsProvider   = null;
            ViewModelLocator.LazyStorageService     = null;
            ViewModelLocator.LazyCameraProvider     = null;
            ViewModelLocator.LazyMainPageViewModel  = null;
        }

        #endregion // Public methods

        #region Private methods

        /// <summary>
        /// Helper method to invoke cleanup methods on all <paramref name="instances"/> given.
        /// </summary>
        /// <param name="instances">Collection of instances to cleanup.</param>
        private static void CleanupInstances(IEnumerable<object> instances)
        {
            foreach (object instance in instances)
            {
                if (instance == null)
                {
                    continue;
                }

                IDisposable disposable = instance as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                    continue;
                }
            }
        }

        #endregion // Private methods
    }
}
