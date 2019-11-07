// ----------------------------------------------------------------------------
// <copyright company="Microsoft Corporation" file="DispatcherEx.cs">
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
    using System.Threading;
    using System.Windows.Threading;

    /// <summary>
    /// Contains extension methods for the <see cref="Dispatcher"/> class.
    /// </summary>
    public static class DispatcherEx
    {
        /// <summary>
        /// Executes an action on the UI thread.<br/>
        /// If this method is called from the UI thread, the action is executed immediately.<br/>
        /// If the method is called from another thread, the action will be enqueued on the UI thread's dispatcher
        /// and executed asynchronously.
        /// </summary>
        /// <param name="dispatcher">Dispatcher object.</param>
        /// <param name="action">Action to execute.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <see langword="null"/>.
        ///     <para>-or-</para>
        /// <paramref name="action"/> is <see langword="null"/>.
        /// </exception>
        public static void BeginInvokeOnUIThread(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.BeginInvoke(action);
            }
        }

        /// <summary>
        /// Executes an action on the UI thread, waiting for the action to complete.<br/>
        /// If this method is called from the UI thread, the action is executed immediately.<br/>
        /// If the method is called from another thread, the action will be enqueued on the UI thread's dispatcher
        /// and executed synchronously.
        /// </summary>
        /// <param name="dispatcher">Dispatcher instance.</param>
        /// <param name="action">Action to execute.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="dispatcher"/> is <see langword="null"/>.
        ///     <para>-or-</para>
        /// <paramref name="action"/> is <see langword="null"/>.
        /// </exception>
        public static void InvokeOnUIThread(this Dispatcher dispatcher, Action action)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                using (ManualResetEventSlim resetEvent = new ManualResetEventSlim(false))
                {
                    dispatcher.BeginInvoke(() =>
                    {
                        action();
                        resetEvent.Set();
                    });

                    resetEvent.Wait();
                }
            }
        }
    }
}
