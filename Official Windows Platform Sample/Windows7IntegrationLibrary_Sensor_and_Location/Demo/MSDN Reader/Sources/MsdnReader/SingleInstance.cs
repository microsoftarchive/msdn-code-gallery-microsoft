// Copyright (c) Microsoft Corporation.  All rights reserved.

//---------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Lifetime;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Security;
using System.IO;
using System.Collections.Generic;
using Microsoft.SceReader;
using System.Runtime.Serialization.Formatters;

namespace MsdnReader
{
    // Note: this class should be used with some caution, because it does no
    // security checking. For example, if one instance of an app that uses this class
    // is running as Administrator, any other instance, even if it is not
    // running as Administrator, can activate it with command line arguments.
    // For most apps, this will not be much of an issue.

    public static class SingleInstance
    {
        //-------------------------------------------------------------------
        //
        //  Public Methods
        //
        //-------------------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// Checks if the instance of the application attempting to start is the first instance. If not, activates the first instance
        /// </summary>
        /// <returns>True if this is the first instance of the application</returns>
        public static bool InitializeAsFirstInstance()
        {
            bool isFirstInstance;

            _singleInstanceMutex = new Mutex(true, ApplicationIdentifier, out isFirstInstance);

            if (isFirstInstance)
            {
                CreateRemoteService();
            }
            else
            {
                SignalFirstInstance(GetCommandLineArgs());
            }

            return isFirstInstance;
        }

        /// <summary>
        /// Cleans up single-instance code, clearing shared resources, mutexes, etc
        /// </summary>
        public static void Cleanup()
        {
            _singleInstanceMutex.Close();

            if (_channel != null)
            {
                ChannelServices.UnregisterChannel(_channel);
            }
        }

        /// <summary>
        /// Gets command line args - for ClickOnce deployed applications, command line args may not be passed directly, they have to be retrieved
        /// </summary>
        /// <returns>List of command line arg strings</returns>
        public static IList<string> GetCommandLineArgs()
        {
            string[] args = null;
            if (AppDomain.CurrentDomain.ActivationContext == null)
            {
                //The application was not clickonce deployed, get args from standard API's
                args = Environment.GetCommandLineArgs();
            }
            else
            {
                //The application was clickonce deployed
                //Clickonce deployd apps cannot recieve traditional commandline arguments
                //As a workaround commandline arguments can be written to a shared location before 
                //the app is launched and the app can obtain its commandline arguments from the 
                //shared location               
                string appFolderPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Path.Combine(SceReaderSettings.CompanyName, SceReaderSettings.ApplicationName)
                );

                string cmdLinePath = Path.Combine(appFolderPath, "cmdline.txt");
                if (File.Exists(cmdLinePath))
                {
                    try
                    {
                        using (TextReader reader = new StreamReader(cmdLinePath, System.Text.Encoding.Unicode))
                        {
                            args = NativeMethods.CommandLineToArgvW(reader.ReadToEnd());
                        }
                        File.Delete(cmdLinePath);
                    }
                    catch (IOException)
                    {
                    }

                }
            }

            if (args == null)
            {
                args = new string[] { };
            }

            return new List<string>(args);
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Private Methods
        //
        //-------------------------------------------------------------------

        #region Private Methods

        private static void CreateRemoteService()
        {
            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;
            System.Collections.IDictionary props = new Dictionary<string, string>();

            props["name"] = ChannelName;
            props["portName"] = ChannelName;
            props["exclusiveAddressUse"] = "false";

            // Create the IPC Server channel with the channel properties
            _channel = new IpcServerChannel(props, serverProvider);

            // Register the channel with the channel services
            ChannelServices.RegisterChannel(_channel, true);

            // Expose the remote service with the REMOTE_SERVICE_NAME
            IPCRemoteService remoteService = new IPCRemoteService();
            RemotingServices.Marshal(remoteService, RemoteServiceName);
        }

        /// <summary>
        /// Creates a client channel and obtains a reference to the remoting service exposed by the server - 
        /// in this case, the remoting service exposed by the first instance. Calls a function of the remoting service 
        /// class to pass on command line arguments from the second instance to the first and cause it to activate itself
        /// </summary>
        /// <param name="args">
        /// Command line arguments for the second instance, passed to the first instance to take appropriate action
        /// </param>
        private static void SignalFirstInstance(IList<string> args)
        {
            string channelName = ChannelName;

            IpcClientChannel secondInstanceChannel = new IpcClientChannel();
            ChannelServices.RegisterChannel(secondInstanceChannel, true);

            string remotingServiceUrl = IpcProtocol + channelName + "/" + RemoteServiceName;

            // Obtain a reference to the remoting service exposed by the server i.e the first instance of the application
            IPCRemoteService firstInstanceRemoteServiceReference = (IPCRemoteService)RemotingServices.Connect(typeof(IPCRemoteService), remotingServiceUrl);

            // Check that the remote service exists, in some cases the first instance may not yet have created one, in which case
            // the second instance should just exit
            if (firstInstanceRemoteServiceReference != null)
            {
                // Invoke a method of the remote service exposed by the first instance passing on the command line
                // arguments and causing the first instance to activate itself
                firstInstanceRemoteServiceReference.InvokeFirstInstance(args);
            }
        }

        /// <summary>
        /// Callback for activating first instance of the application
        /// </summary>
        private static object ActivateFirstInstanceCallback(object arg)
        {
            // Get command line args to be passed to first instance
            IList<string> args = arg as IList<string>;
            ActivateFirstInstance(args);
            return null;
        }

        /// <summary>
        /// Activates the first instance of the application with arguments from a second instance
        /// </summary>
        private static void ActivateFirstInstance(IList<string> args)
        {
            // Set main window state and process command line args
            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                Application.Current.MainWindow.Activate();
                ServiceProvider.ViewManager.ProcessCommandLineArgs(args);
            }
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Private Properties
        //
        //-------------------------------------------------------------------

        #region Private Properties

        private static string ApplicationIdentifier
        {
            get
            {
                if (string.IsNullOrEmpty(_applicationIdentifier))
                {
                    _applicationIdentifier = String.Concat(SceReaderSettings.CompanyName, SceReaderSettings.ApplicationName, Environment.UserName);
                }

                return _applicationIdentifier;
            }
        }

        private static string ChannelName
        {
            get
            {
                return ApplicationIdentifier + Delimiter + ChannelNameSuffix;
            }
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Private classes
        //
        //-------------------------------------------------------------------

        #region Private Classes

        /// <summary>
        /// Remoting service class which is exposed by the server i.e the first instance and called by the second instance
        /// to pass on the command line arguments to the first instance and cause it to activate itself
        /// </summary>
        private class IPCRemoteService : MarshalByRefObject
        {
            public void InvokeFirstInstance(IList<string> args)
            {
                if (Application.Current != null)
                {
                    // Do an asynchronous call to ActivateFirstInstance function
                    Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Normal,
                        new DispatcherOperationCallback(SingleInstance.ActivateFirstInstanceCallback), args);
                }
            }

            // Remoting Object's ease expires after every 5 minutes by default. We need to override the InitializeLifetimeService class
            // to ensure that lease never expires
            public override object InitializeLifetimeService()
            {
                return null;
            }
        }

        #endregion

        #region Private Fields

        //-------------------------------------------------------------------
        //
        //  Private fields
        //
        //-------------------------------------------------------------------

        private const string Delimiter = ":";
        private const string ChannelNameSuffix = "SingeInstanceIPCChannel";
        private const string RemoteServiceName = "SingleInstanceApplicationService";
        private const string IpcProtocol = "ipc://";

        private static string _applicationIdentifier;
        private static Mutex _singleInstanceMutex;
        private static IpcServerChannel _channel;

        #endregion
    }
}