// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="DispatcherHelper.cs">
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

namespace CameraSampleCS.Helpers
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// Contains helper methods to work with the current application <see cref="Dispatcher"/>.
    /// </summary>
    public static class DispatcherHelper
    {
        /// <summary>
        /// Lazily-obtained global <see cref="Dispatcher"/> instance.
        /// </summary>
        private static readonly Lazy<Dispatcher> Dispatcher = new Lazy<Dispatcher>(() => Deployment.Current.Dispatcher);

        /// <summary>
        /// Executes an action on the UI thread.<br/>
        /// If this method is called from the UI thread, the action is executed immediately.<br/>
        /// If the method is called from another thread, the action will be enqueued on the UI thread's dispatcher
        /// and executed asynchronously.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
        public static void BeginInvokeOnUIThread(Action action)
        {
            DispatcherHelper.Dispatcher.Value.BeginInvokeOnUIThread(action);
        }

        /// <summary>
        /// Executes an action on the UI thread, waiting for the action to complete.<br/>
        /// If this method is called from the UI thread, the action is executed immediately.<br/>
        /// If the method is called from another thread, the action will be enqueued on the UI thread's dispatcher
        /// and executed synchronously.
        /// </summary>
        /// <param name="action">Action to execute.</param>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
        public static void InvokeOnUIThread(Action action)
        {
            DispatcherHelper.Dispatcher.Value.InvokeOnUIThread(action);
        }
    }
}
