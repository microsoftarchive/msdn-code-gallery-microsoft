using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Windows.UI.Popups;

namespace BTLE_Explorer
{
    /// <summary>
    /// Some useful utilities to be used by the app.
    /// </summary>
    public static class Utilities
    {
        #region ----------------------------- Alert Box ------------------------
        /// <summary>
        /// Shows a message box with the given message.
        /// </summary>
        /// <param name="message">Message to be displayed</param>
        public static void MakeAlertBox(string message)
        {
            MessageDialog box = new MessageDialog(message);
            box.Title = "BTLE Explorer has a message!";

            ShowMessageDialog(box);
        }
        #endregion // alert box

        #region ----------------------------- Exceptions ------------------------
        /// <summary>
        /// Helper function to build our standard error message.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string BuildErrorMessage(Exception ex, string message)
        {
            if (GlobalSettings.SelectedDevice != null)
            {
                message += "\nDevice: " + GlobalSettings.SelectedDevice.Name;
            }
            if (GlobalSettings.SelectedService != null)
            {
                message += "\nService: " + GlobalSettings.SelectedService.Name;
            }
            if (GlobalSettings.SelectedCharacteristic != null)
            {
                message += "\nCharacteristic: " + GlobalSettings.SelectedCharacteristic.Name;
            }

            message += "\n\nError: " + ex.Message + "\n\nStacktrace:\n" + ex.StackTrace;
            return message;
        }

        /// <summary>
        /// Make a pop up, to be used if something breaks.
        /// </summary>
        /// <param name="ex"></param>
        public static void OnException(Exception ex)
        {
            string message = "";

            message = BuildErrorMessage(ex, message);
            MessageDialog errorBox = new MessageDialog(message);
            errorBox.Title = "Unexpected exception!";

            ShowMessageDialog(errorBox);
        }

        /// <summary>
        /// Show an exception with a friendly message
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="msg"></param>
        public static void OnExceptionWithMessage(Exception ex, string msg)
        {
            string message = "Message: " + msg + "\n\n";

            message = BuildErrorMessage(ex, message);
            MessageDialog errorBox = new MessageDialog(message);
            errorBox.Title = "Known exception!";

            ShowMessageDialog(errorBox);
        }
        #endregion // Exceptions

        #region ----------------------------- Helpers ------------------------
        /// <summary>
        /// Helper function that runs a function as a task, on a new tread
        /// </summary>
        /// <param name="func">Function to be run</param>
        public static void RunFuncAsTask(Func<Task> func)
        {
            Task.Run(async () => await func());
        }

        /// <summary>
        /// Determine if the current thread is the UI thread, and if so, call
        /// the action direction.
        /// 
        /// If not, dispatch the action to the UI thread.
        /// </summary>
        /// <param name="action">Action to be run</param>
        /// <returns></returns>
        public static async Task RunActionOnUiThreadAsync(Action action)
        {
            if (CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess)
            {
                action();
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    () => action());
            }
        }

        /// <summary>
        /// Non-async version of RunActionOnUiThreadAsync
        /// </summary>
        /// <param name="action"></param>
        public static async void RunActionOnUiThread(Action action)
        {
            await RunActionOnUiThreadAsync(action);
        }

        /// <summary>
        /// Same as RunActionOnUiThreadAsync, except that it's a function instead of
        /// an action
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        public static async Task RunFuncOnUiThreadAsync(Func<Task> func)
        {
            if (CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess)
            {
                await func();
            }
            else
            {
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                    CoreDispatcherPriority.Normal,
                    async () => await func());
            }
        }

        /// <summary>
        /// Same as RunActionOnUiThread, except that it's a function instead of
        /// an action
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>        
        public static async void RunFuncOnUiThread(Func<Task> func)
        {
            await RunFuncOnUiThreadAsync(func);
        }

        /// <summary>
        /// Shows a message box
        /// </summary>
        /// <param name="dialog"></param>
        private static void ShowMessageDialog(MessageDialog dialog)
        {
            RunFuncOnUiThread(
                async () =>
                {
                    try
                    {
                        await dialog.ShowAsync();
                    }
                    catch (Exception)
                    {
                        // If multiple message boxes are shown, this can throw E_ACCESS_DENIED.
                    }
                });
        }
        #endregion // Helpers
    }
}
