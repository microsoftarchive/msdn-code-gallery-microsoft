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
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting.Channels;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Remoting.Channels.Ipc;
using Microsoft.SubscriptionCenter.Sync;
using Microsoft.SceReader;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;
using System.Runtime.Remoting;

namespace MsdnReader
{
    /// <summary>
    /// This class is responsible for connecting to the subscription service and for synchronizing
    /// application's data updates with those performed by subscription service
    /// </summary>
    public class SubscriptionServiceManager
    {
        /// <summary>
        ///  Register client channel, connect to subscription server's channel
        /// </summary>
        public void Initialize()
        {
            // Initialize dispatcher
            _dispatcher = Dispatcher.CurrentDispatcher;
            _active = true;

            ServiceProvider.DataManager.UpdateStarted += DataManager_UpdateStarted;
            ServiceProvider.DataManager.UpdateCompleted += DataManager_UpdateCompleted;

            // Register client IPC channel
            RegisterChannelServices();

            // Wait for subscription service to activate its channel
            WaitForServiceActivation();
        }

        /// <summary>
        /// Shut down remote communication
        /// </summary>
        public void Shutdown()
        {
            _active = false;

            ServiceProvider.DataManager.UpdateStarted -= DataManager_UpdateStarted;
            ServiceProvider.DataManager.UpdateCompleted -= DataManager_UpdateCompleted;

            // Disconnect subscription from the service
            DisconnectFromServiceChannel();
            if (_serviceChannelHandle != null)
            {
                _serviceChannelHandle.Reset();
            }

            _waitingForService = false;
            _dispatcher = null;

            UnregisterChannelServices();
        }

        /// <summary>
        /// Registers IPC chnnael for remote communication
        /// </summary>
        private void RegisterChannelServices()
        {
            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider();
            BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;
            System.Collections.IDictionary props = new Dictionary<string, string>();

            props["name"] = String.Concat(SceReaderSettings.CompanyName, SceReaderSettings.ApplicationName, Environment.UserName);
            props["portName"] = MsdnReaderSettings.ChannelPortName;
            props["secure"] = "true";
            props["tokenImpersonationLevel"] = "Impersonation";
            props["exclusiveAddressUse"] = "false";

             // If channel is already registered - may happen if there is not enough time to unregister it on suspend, etc -
             // unregister and re-register it
            if (ChannelRegistered())
            {
                UnregisterChannelServices();
            }

            _channel = new IpcChannel(props, clientProvider, serverProvider);
            ChannelServices.RegisterChannel(_channel, true);
        }

        /// <summary>
        /// Disconects IPC channel for remote communication
        /// </summary>
        private void UnregisterChannelServices()
        {
            ChannelServices.UnregisterChannel(_channel);
            _channel = null;
        }

        /// <summary>
        /// Check if the current channel is registered with channel services
        /// </summary>
        /// <returns></returns>
        private bool ChannelRegistered()
        {
            bool registered = false;
            if (_channel != null)
            {
                foreach (IChannel channel in ChannelServices.RegisteredChannels)
                {
                    if (channel == _channel)
                    {
                        registered = true;
                    }
                }
            }
            return registered;
        }

        /// <summary>
        /// Spins new thread to wait for subscription service channel activation
        /// </summary>
        private void WaitForServiceActivation()
        {
            if (!_waitingForService && _active)
            {
                _waitingForService = true;
                ThreadPool.QueueUserWorkItem(WaitForServiceActivationCallback);
            }
        }

        /// <summary>
        /// Callback to wait for service activation
        /// </summary>
        private void WaitForServiceActivationCallback(object state)
        {
            if (_waitingForService && _active)
            {
                _serviceChannel = null;
                string subscriptionServiceSignalName = MsdnReaderSettings.SubscriptionServiceSignalName;
                bool createdNew = false;
                _serviceChannelHandle = new EventWaitHandle(false, EventResetMode.ManualReset, subscriptionServiceSignalName, out createdNew);

                // Wait for service to signal activation
                _serviceChannelHandle.WaitOne();

                // Queue dispatcher item to begin communication when unblocked
                // When using '_dispatcher' we need to check that it is not null, if the application is shutting down dispatcher may no longer be active
                // It is safe to do nothing if dispatcher is null
                if (_dispatcher != null)
                {
                    _dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(SubscriptionServiceChannelActivatedCallback), null);
                }
                else
                {
                    _waitingForService = false;
                }
            }
        }

        /// <summary>
        /// Connects to channel exposed by subscription service. Returns true if connection succeeded
        /// </summary>
        private bool ConnectToServiceChannel()
        {
            bool connected = false;
            try
            {
                if (_serviceChannel != null)
                {
                    connected = _serviceChannel.ConnectSubscription(MsdnReaderSettings.ApplicationName);
                }
            }
            catch (Exception e)
            {
                connected = false;
                ServiceProvider.Logger.Error(Resources.Strings.SubscriptionServiceManagerServiceError, e.Message);
                HandleRemotingException(e);
            }
            return connected;
        }

        /// <summary>
        /// Disconnects from service channel
        /// </summary>
        private void DisconnectFromServiceChannel()
        {
            try
            {
                if (_serviceChannel != null)
                {
                    _serviceChannel.DisconnectSubscription(MsdnReaderSettings.ApplicationName);
                }
            }
            catch (Exception e)
            {
                // On disconnecting there is no need to provide further exception handling such as waiting for service reactivation             
                ServiceProvider.Logger.Error(Resources.Strings.SubscriptionServiceManagerServiceError, e.Message);
            }
            _serviceChannel = null;
        }

        /// <summary>
        /// Subscribe for subscription service events
        /// </summary>
        private void AddServiceChannelHandlers()
        {
            try
            {
                // Add handler for subscription service's update completed event
                if (_serviceChannel != null)
                {
                    _serviceChannel.AddSubscriptionUpdateCompletedHandler(MsdnReaderSettings.ApplicationName,
                        SubscriptionServiceEventHandlerShim.Create(SubscriptionService_UpdateCompleted));
                    _serviceChannel.AddSubscriptionServiceDisconnectedHandler(MsdnReaderSettings.ApplicationName,
                        SubscriptionServiceEventHandlerShim.Create(SubscriptionService_Disconnected));
                }
            }
            catch (Exception e)
            {
                ServiceProvider.Logger.Error(Resources.Strings.SubscriptionServiceManagerServiceError, e.Message);
                HandleRemotingException(e);
            }
        }

        /// <summary>
        /// Handler for subscription service's update completed event
        /// </summary>
        private void SubscriptionService_UpdateCompleted(object sender, EventArgs e)
        {
            // Queue dispatcher item to load cached data
            // When using '_dispatcher' we need to check that it is not null, if the application is shutting down dispatcher may no longer be active
            // It is safe to do nothing if dispatcher is null
            if (_dispatcher != null)
            {
                _dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(ServiceUpdateCompletedCallback), null);
            }
        }

        /// <summary>
        /// Handler for subscription service's disconnected event
        /// </summary>
        private void SubscriptionService_Disconnected(object sender, EventArgs e)
        {
            // Queue dispatcher item to wait for service reconnection
            // When using '_dispatcher' we need to check that it is not null, if the application is shutting down dispatcher may no longer be active
            // It is safe to do nothing if dispatcher is null
            if (_dispatcher != null)
            {
                _dispatcher.BeginInvoke(DispatcherPriority.Input, new DispatcherOperationCallback(ServiceDisconnectedCallback), null);
            }
            else
            {
                // Set service channel to null, no need to wait for service activation since there is no dispatcher. 
                _serviceChannel = null;
            }
        }

        /// <summary>
        /// Handler for DataManager's UpdateCompleted event. Notifies subscription service that update is completed
        /// </summary>
        private void DataManager_UpdateCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            try
            {
                // Notify subscription service of update completed
                if (_serviceChannel != null)
                {
                    _serviceChannel.OnSubscriptionUpdateCompleted(MsdnReaderSettings.ApplicationName);
                }
            }
            catch (Exception exception)
            {
                ServiceProvider.Logger.Error(Resources.Strings.SubscriptionServiceManagerServiceError, exception.Message);
                HandleRemotingException(exception);
            }
        }

        /// <summary>
        /// Handler for DataManager's UpdateStarted event. Notifies subscription service that update has started
        /// </summary>
        private void DataManager_UpdateStarted(object sender, EventArgs e)
        {
            NotifyUpdateStarted();
        }

        /// <summary>
        /// When communicating with the service over IPC Channel, exceptions are logged. In the case of
        /// RemotingExceptions, the server may be down. In this case, client should block and attempt to
        /// wait for the server to re-signal
        /// </summary>
        private void HandleRemotingException(Exception e)
        {
            if (e is RemotingException)
            {
                _serviceChannel = null;

                // Reset service channel handle. If the service is still online, it should set the handle at regular intervals
                if (_serviceChannelHandle != null)
                {
                    _serviceChannelHandle.Reset();
                }

                WaitForServiceActivation();
            }
        }

        /// <summary>
        /// Callback worker for subscription service activation. Attempts to connect to service
        /// </summary>
        private object SubscriptionServiceChannelActivatedCallback(object arg)
        {
            // Reset waiting for service flag
            if (_waitingForService && _active)
            {
                _waitingForService = false;
                _serviceChannel = (ISubscriptionServiceChannel)Activator.GetObject
                (typeof(ISubscriptionServiceChannel), MsdnReaderSettings.SubscriptionServiceChannelName);

                // Try to connect to the service channel, if connection fails, continue waiting
                if (ConnectToServiceChannel())
                {
                    // If syncing, notify service channel of update started
                    if (ServiceProvider.DataManager.IsUpdateInProgress)
                    {
                        NotifyUpdateStarted();
                    }

                    AddServiceChannelHandlers();
                }
                else
                {
                    _serviceChannel = null;
                }
            }
            return null;
        }

        /// <summary>
        ///  Notify the subscription service that an update has started
        /// </summary>
        private void NotifyUpdateStarted()
        {
            // Notify subscription service of update completed
            try
            {
                if (_serviceChannel != null)
                {
                    _serviceChannel.OnSubscriptionUpdateStarted(MsdnReaderSettings.ApplicationName);
                }
            }
            catch (Exception exception)
            {
                ServiceProvider.Logger.Error(Resources.Strings.SubscriptionServiceManagerServiceError, exception.Message);
                HandleRemotingException(exception);
            }
        }

        /// <summary>
        /// Callback worker for subscription service update completed event. 
        /// When subscription service completes an update, data is reloaded from the cache if no other
        /// updates are already in progress
        /// </summary>
        private object ServiceUpdateCompletedCallback(object arg)
        {
            // If no update is in progress, load cached data
            if (!ServiceProvider.DataManager.IsUpdateInProgress)
            {
                ServiceProvider.DataManager.LoadCachedDataAsync();
            }

#if DEBUG
            ServiceProvider.Logger.Information(Resources.Strings.SubscriptionServiceManagerHandlerExecuted, "UpdateCompleted");
#endif
            return null;
        }

        /// <summary>
        /// Callback worker for subscription service disconnected event. 
        /// </summary>
        private object ServiceDisconnectedCallback(object arg)
        {
            // Wait for service to reconnect
            _serviceChannel = null;
            WaitForServiceActivation();

#if DEBUG
            ServiceProvider.Logger.Information(Resources.Strings.SubscriptionServiceManagerHandlerExecuted, "ServiceDisconnected");
#endif

            return null;
        }

        /// <summary>
        /// True if the subscription service is currently updating this application's data
        /// </summary>
        public bool IsServiceUpdateInProgress
        {
            get
            {
                bool isUpdateInProgress = false;
                try
                {
                    if (_serviceChannel != null)
                    {
                        isUpdateInProgress = _serviceChannel.IsSubscriptionUpdateInProgress(MsdnReaderSettings.ApplicationName);
                    }
                }
                catch (Exception e)
                {
                    isUpdateInProgress = false;
                    ServiceProvider.Logger.Error(Resources.Strings.SubscriptionServiceManagerServiceError, e.Message);
                    HandleRemotingException(e);
                }
                return isUpdateInProgress;
            }
        }

        private bool _waitingForService = false;
        private IpcChannel _channel;
        private ISubscriptionServiceChannel _serviceChannel;
        private Dispatcher _dispatcher;
        private  EventWaitHandle _serviceChannelHandle;
        private bool _active = false;
    }
}