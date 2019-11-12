//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.Storage.Streams;
using Microsoft.MBN;
using System.Reflection;
using SDKTemplate;

namespace MobileBroadbandComApi
{
    public delegate void OnInterfaceArrivalHandler(IMbnInterface newInterface);
    public delegate void OnInterfaceRemovalHandler(IMbnInterface oldInterface);
    public delegate void OnGetPinStateCompleteHandler(IMbnPinManager mbnPinManager, MBN_PIN_INFO pinInfo, uint requestId, int status);
    public delegate void OnEnterCompleteHandler(IMbnPin mbnPin, ref MBN_PIN_INFO pinInfo, uint requestId, int status);
    public delegate void OnConnectCompleteHandler(IMbnConnection connection, uint requestId, int status);
    public delegate void OnDisconnectCompleteHandler(IMbnConnection connection, uint requestId, int status);
    public delegate void OnOpenCommandSessionCompleteHandler(IMbnDeviceService deviceService, int status, uint requestId);
    public delegate void OnQueryCommandCompleteHandler(IMbnDeviceService deviceService, uint responseId, byte[] deviceServiceData, int status, uint requestId);
    public delegate void OnCloseCommandSessionCompleteHandler(IMbnDeviceService deviceService, int status, uint requestId);

    // This class implements IMbnInterfaceManagerEvents
    class InterfaceManagerEventsSink : IMbnInterfaceManagerEvents, IDisposable
    {
        // Using weak reference of Main page as in any case we have reference
        // of that, the underlying object can be reclaimed by gargabe collection
        private WeakReference<OnInterfaceArrivalHandler> m_ArrivalCallback;
        private WeakReference<OnInterfaceRemovalHandler> m_RemovalCallback;
        private IConnectionPoint m_ConnectionPoint;
        private uint m_AdviseCookie;

        public InterfaceManagerEventsSink(
            OnInterfaceArrivalHandler arrivalCallback,
            OnInterfaceRemovalHandler removalCallback,
            IConnectionPoint connectionPoint)
        {
            m_ArrivalCallback = new WeakReference<OnInterfaceArrivalHandler>(arrivalCallback);
            m_RemovalCallback = new WeakReference<OnInterfaceRemovalHandler>(removalCallback);
            m_ConnectionPoint = connectionPoint;
            m_ConnectionPoint.Advise(this, out m_AdviseCookie);
        }

        ~InterfaceManagerEventsSink()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_AdviseCookie != 0)
            {
                m_ConnectionPoint.Unadvise(m_AdviseCookie);
                m_AdviseCookie = 0;
            }
        }

        // This will be called back when interface arrival operation is complete
        public void OnInterfaceArrival(IMbnInterface newInterface)
        {
            // Invoke main page thread to show UI
            OnInterfaceArrivalHandler callback;
            if (m_ArrivalCallback.TryGetTarget(out callback))
            {
                callback.Invoke(newInterface);
            }
        }

        // This will be called back when interface removal operation is complete
        public void OnInterfaceRemoval(IMbnInterface oldInterface)
        {
            // Invoke main page thread to show UI
            OnInterfaceRemovalHandler callback;
            if (m_RemovalCallback.TryGetTarget(out callback))
            {
                callback.Invoke(oldInterface);
            }
        }
    }

    // This class implements IMbnPinManagerEvents
    class PinManagerEventsSink : IMbnPinManagerEvents, IDisposable
    {
        // Using weak reference of Main page as in any case we have reference
        // of that, the underlying object can be reclaimed by gargabe collection
        private WeakReference<OnGetPinStateCompleteHandler> m_Callback;
        private IConnectionPoint m_ConnectionPoint;
        private uint m_AdviseCookie;

        public PinManagerEventsSink(OnGetPinStateCompleteHandler callback, IConnectionPoint connectionPoint)
        {
            m_Callback = new WeakReference<OnGetPinStateCompleteHandler>(callback);
            m_ConnectionPoint = connectionPoint;
            m_ConnectionPoint.Advise(this, out m_AdviseCookie);
        }

        ~PinManagerEventsSink()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_AdviseCookie != 0)
            {
                m_ConnectionPoint.Unadvise(m_AdviseCookie);
                m_AdviseCookie = 0;
            }
        }

        // This will be called back when get pin state operation is complete
        public void OnGetPinStateComplete(IMbnPinManager mbnPinManager, MBN_PIN_INFO pinInfo, uint requestId, int status)
        {
            // Invoke main page thread to show UI
            OnGetPinStateCompleteHandler callback;
            if (m_Callback.TryGetTarget(out callback))
            {
                callback.Invoke(mbnPinManager, pinInfo, requestId, status);
            }
        }

        // Not implemented for sample
        public void OnPinListAvailable(IMbnPinManager mbnPinManager) { }
    }

    // This class implements IMbnPinEvents
    class PinEventsSink : IMbnPinEvents, IDisposable
    {
        // Using weak reference of Main page as in any case we have reference
        // of that, the underlying object can be reclaimed by gargabe collection
        private WeakReference<OnEnterCompleteHandler> m_Callback;
        private IConnectionPoint m_ConnectionPoint;
        private uint m_AdviseCookie;

        public PinEventsSink(OnEnterCompleteHandler callback, IConnectionPoint connectionPoint)
        {
            m_Callback = new WeakReference<OnEnterCompleteHandler>(callback);
            m_ConnectionPoint = connectionPoint;
            m_ConnectionPoint.Advise(this, out m_AdviseCookie);
        }

        ~PinEventsSink()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_AdviseCookie != 0)
            {
                m_ConnectionPoint.Unadvise(m_AdviseCookie);
                m_AdviseCookie = 0;
            }
        }

        // This will be called back when pin enter operation is complete
        public void OnEnterComplete(IMbnPin mbnPin, ref MBN_PIN_INFO pinInfo, uint requestId, int status)
        {
            // Invoke main page thread to show UI
            OnEnterCompleteHandler callback;
            if (m_Callback.TryGetTarget(out callback))
            {
                callback.Invoke(mbnPin, ref pinInfo, requestId, status);
            }
        }

        // Not implemented for sample
        public void OnEnableComplete(IMbnPin mbnPin, ref MBN_PIN_INFO pinInfo, uint requestId, int status) { }
        public void OnDisableComplete(IMbnPin mbnPin, ref MBN_PIN_INFO pinInfo, uint requestId, int status) { }
        public void OnChangeComplete(IMbnPin mbnPin, ref MBN_PIN_INFO pinInfo, uint requestId, int status) { }
        public void OnUnblockComplete(IMbnPin mbnPin, ref MBN_PIN_INFO pinInfo, uint requestId, int status) { }
    }

    // This class implements IMbnConnectionEvents
    class ConnectionEventsSink : IMbnConnectionEvents, IDisposable
    {
        // Using weak reference of Main page as in any case we have reference
        // of that, the underlying object can be reclaimed by gargabe collection
        private WeakReference<OnConnectCompleteHandler> m_ConnectCallback;
        private WeakReference<OnDisconnectCompleteHandler> m_DisconnectCallback;
        private IConnectionPoint m_ConnectionPoint;
        private uint m_AdviseCookie;

        public ConnectionEventsSink(
            OnConnectCompleteHandler connectCallback,
            OnDisconnectCompleteHandler disconnectCallback,
            IConnectionPoint connectionPoint)
        {
            m_ConnectCallback = new WeakReference<OnConnectCompleteHandler>(connectCallback);
            m_DisconnectCallback = new WeakReference<OnDisconnectCompleteHandler>(disconnectCallback);
            m_ConnectionPoint = connectionPoint;
            m_ConnectionPoint.Advise(this, out m_AdviseCookie);
        }

        ~ConnectionEventsSink()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_AdviseCookie != 0)
            {
                m_ConnectionPoint.Unadvise(m_AdviseCookie);
                m_AdviseCookie = 0;
            }
        }

        // This will be called back when connect operation is complete
        public void OnConnectComplete(IMbnConnection connection, uint requestId, int status)
        {
            // Invoke main page thread to show UI
            OnConnectCompleteHandler callback;
            if (m_ConnectCallback.TryGetTarget(out callback))
            {
                callback.Invoke(connection, requestId, status);
            }
        }

        // This will be called back when disconnect operation is complete
        public void OnDisconnectComplete(IMbnConnection connection, uint requestId, int status)
        {
            // Invoke main page thread to show UI
            OnDisconnectCompleteHandler callback;
            if (m_DisconnectCallback.TryGetTarget(out callback))
            {
                callback.Invoke(connection, requestId, status);
            }
        }

        // Not implemented for sample
        public void OnConnectStateChange(IMbnConnection connection) { }
        public void OnVoiceCallStateChange(IMbnConnection connection) { }
    }

    class DeviceServicesEventsSink : IMbnDeviceServicesEvents, IDisposable
    {
        // Using weak reference of Main page as in any case we have reference
        // of that, the underlying object can be reclaimed by gargabe collection
        private WeakReference<OnOpenCommandSessionCompleteHandler> m_OnOpenCommandSessionCallback;
        private WeakReference<OnQueryCommandCompleteHandler> m_OnQueryCommandCallback;
        private WeakReference<OnCloseCommandSessionCompleteHandler> m_OnCloseCommandCallback;
        private IConnectionPoint m_ConnectionPoint;
        private uint m_AdviseCookie;

        public DeviceServicesEventsSink(
            OnOpenCommandSessionCompleteHandler onOpenDataSessionCallback,
            OnQueryCommandCompleteHandler onQueryCommandCallback,
            OnCloseCommandSessionCompleteHandler onCloseCommandCallback,
            IConnectionPoint connectionPoint)
        {
            m_OnOpenCommandSessionCallback = new WeakReference<OnOpenCommandSessionCompleteHandler>(onOpenDataSessionCallback);
            m_OnQueryCommandCallback = new WeakReference<OnQueryCommandCompleteHandler>(onQueryCommandCallback);
            m_OnCloseCommandCallback = new WeakReference<OnCloseCommandSessionCompleteHandler>(onCloseCommandCallback);
            m_ConnectionPoint = connectionPoint;
            m_ConnectionPoint.Advise(this, out m_AdviseCookie);
        }

        ~DeviceServicesEventsSink()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_AdviseCookie != 0)
            {
                m_ConnectionPoint.Unadvise(m_AdviseCookie);
                m_AdviseCookie = 0;
            }
        }

        // This will be called back when open command session operation is complete
        public void OnOpenCommandSessionComplete(IMbnDeviceService deviceService, int status, uint requestId)
        {
            // Invoke main page thread to show UI
            OnOpenCommandSessionCompleteHandler callback;
            if (m_OnOpenCommandSessionCallback.TryGetTarget(out callback))
            {
                callback.Invoke(deviceService, status, requestId);
            }
        }

        // This will be called back when query command operation is complete
        public void OnQueryCommandComplete(IMbnDeviceService deviceService, uint responseId, byte[] deviceServiceData, int status, uint requestId)
        {
            // Invoke main page thread to show UI
            OnQueryCommandCompleteHandler callback;
            if (m_OnQueryCommandCallback.TryGetTarget(out callback))
            {
                callback.Invoke(deviceService, responseId, deviceServiceData, status, requestId);
            }
        }

        // This will be called back when close command session operation is complete
        public void OnCloseCommandSessionComplete(IMbnDeviceService deviceService, int status, uint requestId)
        {
            // Invoke main page thread to show UI
            OnCloseCommandSessionCompleteHandler callback;
            if (m_OnCloseCommandCallback.TryGetTarget(out callback))
            {
                callback.Invoke(deviceService, status, requestId);
            }
        }

        // Not implemented for sample
        public void OnSetCommandComplete(IMbnDeviceService deviceService, uint responseId, byte[] deviceServiceData, int status, uint requestId) { }
        public void OnEventNotification(IMbnDeviceService deviceService, uint eventId, byte[] deviceServiceData) { }
        public void OnQuerySupportedCommandsComplete(IMbnDeviceService deviceService, uint[] commandIDList, int status, uint requestId) { }
        public void OnOpenDataSessionComplete(IMbnDeviceService deviceService, int status, uint requestId) { }
        public void OnCloseDataSessionComplete(IMbnDeviceService deviceService, int status, uint requestId) { }
        public void OnWriteDataComplete(IMbnDeviceService deviceService, int status, uint requestId) { }
        public void OnReadData(IMbnDeviceService deviceService, byte[] deviceServiceData) { }
        public void OnInterfaceStateChange(string interfaceId, MBN_DEVICE_SERVICES_INTERFACE_STATE stateChange) { }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MBApiImplementation : SDKTemplate.Common.LayoutAwarePage, IDisposable
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        private static volatile MBApiImplementation instance;
        private static object syncRoot = new Object();

        private IMbnInterfaceManager m_MbnInterfaceManager;
        private IMbnConnectionManager m_MbnConnectionManager;
        private IMbnDeviceServicesManager m_MbnDeviceServicesManager;
        private IMbnInterface m_MbnInterface;
        private IMbnConnection m_MbnConnection;
        private IMbnDeviceServicesContext m_MbnDeviceServicesContext;
        private InterfaceManagerEventsSink m_InterfaceManagerEventsSink;
        private PinManagerEventsSink m_PinManagerEventsSink;
        private PinEventsSink m_PinEventsSink;
        private ConnectionEventsSink m_ConnectionEventsSink;
        private DeviceServicesEventsSink m_DeviceServicesEventsSink;

        // Delegate objects to allow event sink to store a weak reference on them
        private OnInterfaceArrivalHandler m_OnInterfaceArrivalEventDelegate;
        private OnInterfaceRemovalHandler m_OnInterfaceRemovalEventDelegate;
        private OnGetPinStateCompleteHandler m_OnGetPinStateCompleteEventDelegate;
        private OnEnterCompleteHandler m_OnEnterCompleteEventDelegate;
        private OnConnectCompleteHandler m_OnConnectCompleteEventDelegate;
        private OnDisconnectCompleteHandler m_OnDisconnectCompleteEventDelegate;
        private OnOpenCommandSessionCompleteHandler m_OnOpenCommandSessionCompleteEventDelegate;
        private OnQueryCommandCompleteHandler m_OnQueryCommandCompleteEventDelegate;
        private OnCloseCommandSessionCompleteHandler m_OnCloseCommandSessionEventDelegate;

        // For better logging for device services scenario
        private string deviceServicesOutputStr;

        private bool disposed;

        // delegate to enable the enter button
        public delegate void EnableEnterButtonHandler(object sender, EventArgs e);

        // event for the delegate
        public event EnableEnterButtonHandler EnableEnterButton;

        // delegate/event for enable/disble buttons
        public delegate void EnableScenarioButtonsHandler(object sender, EventArgs e);
        public delegate void DisableScenarioButtonsHandler(object sender, EventArgs e);
        public event EnableScenarioButtonsHandler EnableScenarioButtons;
        public event DisableScenarioButtonsHandler DisableScenarioButtons;

        private MBApiImplementation()
        {
            App.Current.Suspending += new SuspendingEventHandler(AppSuspending);
            App.Current.Resuming += new System.EventHandler<object>(AppResuming);

            disposed = false;
        }

        ~MBApiImplementation()
        {
            this.Dispose(false);
        }

        // Get the object instance
        public static MBApiImplementation Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new MBApiImplementation();
                    }
                }
                return instance;
            }
        }

        // Returns the singleton object instance
        public static MBApiImplementation GetInstance()
        {
            return Instance;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases internal COM object reference during explicit disposal.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            // If the object hasn't been disposed
            if (disposed == false)
            {
                if (disposing)
                {
                    DeRegisterForNotifications();
                }

                disposed = true;
            }
        }

        // If App is suspending, un-register from notifications.
        void AppSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs args)
        {
            Windows.ApplicationModel.SuspendingDeferral deferral = args.SuspendingOperation.GetDeferral();

            // Unadvise
            Dispose(true);

            deferral.Complete();
        }

        // If App is resuming back, register from notifications.
        void AppResuming(object sender, object e)
        {
            RegisterForNotifications();
            disposed = false;
        }

        // Parse the common exception codes
        private string ParseExceptionCode(Exception error)
        {
            string returnStr = "";

            switch (error.HResult)
            {
                case -2147023728: // 0x80070490: ERROR_NOT_FOUND
                    returnStr += "\"The given mobile broadband interface or subscriber ID is not present\", error code: 0x80070490";
                    returnStr += "\nPlease plug in required mobile broadband device before continuing with sample.";
                    DisableScenarioButtons(this, EventArgs.Empty);
                    break;
                case -2147023834: // 0x80070426: ERROR_SERVICE_NOT_ACTIVE
                    returnStr += "\"The service has not been started\", error code: 0x80070426";
                    returnStr += "\nPlease start the wwansvc before continuing with sample.";
                    DisableScenarioButtons(this, EventArgs.Empty);
                    break;
                case -2147024875: // 0x80070015: ERROR_NOT_READY
                    returnStr += "\"The device is not ready\", error code: 0x80070015";
                    DisableScenarioButtons(this, EventArgs.Empty);
                    break;
                case -2147024841: // 0x80070037: ERROR_DEV_NOT_EXIST
                    returnStr += " \"The specified mobile broadband device is no longer available\", error code: 0x80070037";
                    DisableScenarioButtons(this, EventArgs.Empty);
                    break;
                case -2147023690: // 0x800704b6: ERROR_BAD_PROFILE
                    returnStr += "\"The network connection profile is corrupted\", error code: 0x800704b6";
                    break;
                case -2147221164: // 0x80040154: REGDB_E_CLASSNOTREG
                    returnStr += "\"Class not registered, this might be because of WwanSvc is not supported on particular SKU\", error code: 0x80040154";
                    break;
                default:
                    returnStr += "Unexpected exception occured: " + error.ToString();
                    break;
            }
            return returnStr;
        }

         // Construct profile xml to connect
        private string ConstructProfileXml(string subscriberId, string apn, string username, string password)
        {
            // Connect using any temporary profile name
            string name = "tempProfile";

            // {0} is the profile name
            // {1} is the subscriber id
            // {2} is the APN
            // {3} username if available
            // {4} password if available
            //
            string profileXml = String.Format(@"<MBNProfile xmlns='http://www.microsoft.com/networking/WWAN/profile/v1'>
    <Name>{0}</Name>
    <IsDefault>false</IsDefault>
    <SubscriberID>{1}</SubscriberID>
    <Context>
        <AccessString>{2}</AccessString>
        <UserLogonCred>
            <UserName>{3}</UserName>
            <Password>{4}</Password>
        </UserLogonCred>
    </Context>
</MBNProfile>",
            name,
            subscriberId,
            apn,
            username,
            password);

            return profileXml;
        }

        // For simplicity, this sample uses first interface
        private IMbnInterface GetFirstInterface()
        {
            foreach (IMbnInterface mbnInterface in m_MbnInterfaceManager.GetInterfaces())
            {
                return mbnInterface;
            }
            return null;
        }

        // GetMbnInterfaceManagerEventsConnectionPoint
        private IConnectionPoint GetMbnInterfaceManagerEventsConnectionPoint()
        {
            IConnectionPointContainer connectionPointContainer = m_MbnInterfaceManager as IConnectionPointContainer;
            Guid iid_IMbnInterfaceManagerEvents = typeof(IMbnInterfaceManagerEvents).GetTypeInfo().GUID;

            IConnectionPoint connectionPoint;
            connectionPointContainer.FindConnectionPoint(ref iid_IMbnInterfaceManagerEvents, out connectionPoint);

            return connectionPoint;
        }

        // GetMbnPinManagerEventsConnectionPoint
        private IConnectionPoint GetMbnPinManagerEventsConnectionPoint()
        {
            IConnectionPointContainer connectionPointContainer = m_MbnInterfaceManager as IConnectionPointContainer;
            Guid iid_IMbnPinManagerEvents = typeof(IMbnPinManagerEvents).GetTypeInfo().GUID;

            IConnectionPoint connectionPoint;
            connectionPointContainer.FindConnectionPoint(ref iid_IMbnPinManagerEvents, out connectionPoint);

            return connectionPoint;
        }

        // GetMbnPinEventsConnectionPoint
        private IConnectionPoint GetMbnPinEventsConnectionPoint()
        {
            IConnectionPointContainer connectionPointContainer = m_MbnInterfaceManager as IConnectionPointContainer;
            Guid iid_IMbnPinEvents = typeof(IMbnPinEvents).GetTypeInfo().GUID;

            IConnectionPoint connectionPoint;
            connectionPointContainer.FindConnectionPoint(ref iid_IMbnPinEvents, out connectionPoint);

            return connectionPoint;
        }

        // GetMbnConnectionEventsConnectionPoint
        private IConnectionPoint GetMbnConnectionEventsConnectionPoint()
        {
            IConnectionPointContainer connectionPointContainer = m_MbnConnectionManager as IConnectionPointContainer;

            Guid iid_IMbnConnectionEvents = typeof(IMbnConnectionEvents).GetTypeInfo().GUID;

            IConnectionPoint connectionPoint;
            connectionPointContainer.FindConnectionPoint(ref iid_IMbnConnectionEvents, out connectionPoint);

            return connectionPoint;
        }

        // GetMbnDeviceServicesEventsConnectionPoint
        private IConnectionPoint GetMbnDeviceServicesEventsConnectionPoint()
        {
            IConnectionPointContainer connectionPointContainer = m_MbnDeviceServicesManager as IConnectionPointContainer;

            Guid iid_IMbnDeviceServicesEvents = typeof(IMbnDeviceServicesEvents).GetTypeInfo().GUID;

            IConnectionPoint connectionPoint;
            connectionPointContainer.FindConnectionPoint(ref iid_IMbnDeviceServicesEvents, out connectionPoint);

            return connectionPoint;
        }

        // Initialize the MBN interfaces
        public void InitializeManagers()
        {
            lock (syncRoot)
            {
                try
                {
                    // Get MbnInterfaceManager
                    if (m_MbnInterfaceManager == null)
                    {
                        m_MbnInterfaceManager = (IMbnInterfaceManager)new MbnInterfaceManager();
                    }

                    // Get MbnConnectionManager
                    if (m_MbnConnectionManager == null)
                    {
                        m_MbnConnectionManager = (IMbnConnectionManager)new MbnConnectionManager();
                    }

                    // Get MbnDeviceServicesManager
                    if (m_MbnDeviceServicesManager == null)
                    {
                        m_MbnDeviceServicesManager = (IMbnDeviceServicesManager)new MbnDeviceServicesManager();
                    }

                    // Register for notifications
                    RegisterForNotifications();
                }
                catch (Exception e)
                {
                    rootPage.NotifyUser(ParseExceptionCode(e), NotifyType.ErrorMessage);
                }
            }
        }

        // Register for notifications
        private void RegisterForNotifications()
        {
            try
            {
                if (m_MbnInterface == null)
                {
                    // Get the interface
                    m_MbnInterface = GetFirstInterface();
                }

                if (m_MbnConnection == null)
                {
                    m_MbnConnection = m_MbnInterface.GetConnection();
                }

                if (m_MbnDeviceServicesContext == null)
                {
                    m_MbnDeviceServicesContext = m_MbnDeviceServicesManager.GetDeviceServicesContext(m_MbnInterface.InterfaceID);
                }

                // Register for IMbnInterfaceManagerEvents
                if (m_InterfaceManagerEventsSink == null)
                {
                    m_OnInterfaceArrivalEventDelegate = new OnInterfaceArrivalHandler(ProcessOnInterfaceArrival);
                    m_OnInterfaceRemovalEventDelegate = new OnInterfaceRemovalHandler(ProcessOnInterfaceRemoval);
                    m_InterfaceManagerEventsSink = new InterfaceManagerEventsSink(m_OnInterfaceArrivalEventDelegate,
                                                                                  m_OnInterfaceRemovalEventDelegate,
                                                                                  GetMbnInterfaceManagerEventsConnectionPoint());
                }

                // Register for IMbnPinManagerEvents
                if (m_PinManagerEventsSink == null)
                {
                    m_OnGetPinStateCompleteEventDelegate = new OnGetPinStateCompleteHandler(ProcessOnGetPinStateCompleteHandlerEvt);
                    m_PinManagerEventsSink = new PinManagerEventsSink(m_OnGetPinStateCompleteEventDelegate, GetMbnPinManagerEventsConnectionPoint());
                }

                // Register for IMbnPinEvents
                if (m_PinEventsSink == null)
                {
                    m_OnEnterCompleteEventDelegate = new OnEnterCompleteHandler(ProcessOnEnterComplete);
                    m_PinEventsSink = new PinEventsSink(m_OnEnterCompleteEventDelegate, GetMbnPinEventsConnectionPoint());
                }
                
                // Register for IMbnConnectionEvents
                if (m_ConnectionEventsSink == null)
                {
                    m_OnConnectCompleteEventDelegate = new OnConnectCompleteHandler(ProcessOnConnectComplete);
                    m_OnDisconnectCompleteEventDelegate = new OnDisconnectCompleteHandler(ProcessOnDisconnectComplete);
                    m_ConnectionEventsSink = new ConnectionEventsSink(m_OnConnectCompleteEventDelegate,
                                                                      m_OnDisconnectCompleteEventDelegate,
                                                                      GetMbnConnectionEventsConnectionPoint());
                }
                
                // Register for IMbnDeviceServicesEvents
                if (m_DeviceServicesEventsSink == null)
                {
                    m_OnOpenCommandSessionCompleteEventDelegate = new OnOpenCommandSessionCompleteHandler(ProcessOnOpenCommandSessionComplete);
                    m_OnQueryCommandCompleteEventDelegate = new OnQueryCommandCompleteHandler(ProcessOnQueryCommandComplete);
                    m_OnCloseCommandSessionEventDelegate = new OnCloseCommandSessionCompleteHandler(ProcessOnCloseCommandSessionComplete);
                    m_DeviceServicesEventsSink = new DeviceServicesEventsSink(
                                                    m_OnOpenCommandSessionCompleteEventDelegate,
                                                    m_OnQueryCommandCompleteEventDelegate,
                                                    m_OnCloseCommandSessionEventDelegate,
                                                    GetMbnDeviceServicesEventsConnectionPoint());
                }
            }
            catch (Exception e)
            {
                rootPage.NotifyUser(ParseExceptionCode(e), NotifyType.ErrorMessage);
            }
        }

        // Un-register notifications
        private void DeRegisterForNotifications()
        {
            try
            {
                // De-Register for IMbnInterfaceManagerEvents
                if (m_InterfaceManagerEventsSink != null)
                {
                    m_InterfaceManagerEventsSink.Dispose();
                    m_InterfaceManagerEventsSink = null;
                }

                // De-Register for IMbnPinManagerEvents
                if (m_PinManagerEventsSink != null)
                {
                    m_PinManagerEventsSink.Dispose();
                    m_PinManagerEventsSink = null;
                }

                // De-Register for IMbnPinManagerEvents
                if (m_PinEventsSink != null)
                {
                    m_PinEventsSink.Dispose();
                    m_PinEventsSink = null;
                }

                // De-Register for IMbnConnectionEvents
                if (m_ConnectionEventsSink != null)
                {
                    m_ConnectionEventsSink.Dispose();
                    m_ConnectionEventsSink = null;
                }

                // De-Register for IMbnDeviceServicesEvents
                if (m_DeviceServicesEventsSink != null)
                {
                    m_DeviceServicesEventsSink.Dispose();
                    m_DeviceServicesEventsSink = null;
                }
            }
            catch (Exception e)
            {
                rootPage.NotifyUser(ParseExceptionCode(e), NotifyType.ErrorMessage);
            }
        }

        // Find if it is already connected
        bool IsConnected()
        {
            MBN_ACTIVATION_STATE activationState;
            string profileName;
            bool connectionState = false;
            m_MbnConnection.GetConnectionState(out activationState, out profileName);

            if (activationState == MBN_ACTIVATION_STATE.MBN_ACTIVATION_STATE_ACTIVATED |
                activationState == MBN_ACTIVATION_STATE.MBN_ACTIVATION_STATE_ACTIVATING)
            {
                connectionState = true;
            }

            return connectionState;
        }

        // Parse phone book entries
        void ParseResponse(byte[] data)
        {
            // We will be using a data reader to parse the returned MBIM_PHONEBOOK_READ_INFO data
            DataReader dataReader = null;
            int elementCount = 0;
            int[] entryOffset;

            // Setup the data reader for MBIM_PHONEBOOK_READ_INFO
            dataReader = DataReader.FromBuffer(data.AsBuffer(0, data.Length));
            dataReader.ByteOrder = ByteOrder.LittleEndian;  // MBIM byte ordering is Little Endian

            // ElementCount
            elementCount = dataReader.ReadInt32();
            deviceServicesOutputStr += "\n\n================================";
            deviceServicesOutputStr += "\nNumber of Phonebook Entries: " + elementCount;

            // We will store the Offset from PhoneBookRefList for finding each entry later
            entryOffset = new int[elementCount];
            for (int i = 0; i < elementCount; i++)
            {
                // Offset
                entryOffset[i] = dataReader.ReadInt32();

                // Length
                dataReader.ReadInt32();
            }

            // Read each MBIM_PHONEBOOK_ENTRY
            for (int i = 0; i < elementCount; i++)
            {
                int numberOffset = 0, numberLength = 0;
                int nameOffset = 0, nameLength = 0;

                // Setup the data reader for structured fields in MBIM_PHONEBOOK_ENTRY. We will be reading five 4 byte values
                dataReader = DataReader.FromBuffer(data.AsBuffer(entryOffset[i], 5 * 4));
                dataReader.ByteOrder = ByteOrder.LittleEndian;  // MBIM byte ordering is Little Endian

                // EntryIndex
                deviceServicesOutputStr += "\nIndex[" + i + "] = {" + dataReader.ReadInt32() + "}";

                // NumberOffset
                numberOffset = dataReader.ReadInt32();

                // NumberLength
                numberLength = dataReader.ReadInt32();

                // NameOffset
                nameOffset = dataReader.ReadInt32();

                // NameLength
                nameLength = dataReader.ReadInt32();

                // Read Number string
                dataReader = DataReader.FromBuffer(data.AsBuffer(entryOffset[i] + numberOffset, numberLength));
                dataReader.ByteOrder = ByteOrder.LittleEndian;  // MBIM byte ordering is Little Endian
                dataReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf16LE;   // MBIM strings are UTF-16LE encoded

                string number = dataReader.ReadString((uint)(numberLength / 2));  // Length is in bytes

                deviceServicesOutputStr += "\nNumber[" + i + "] = {" + number + "}";

                // Read name string
                dataReader = DataReader.FromBuffer(data.AsBuffer(entryOffset[i] + nameOffset, nameLength));
                dataReader.ByteOrder = ByteOrder.LittleEndian;  // MBIM byte ordering is Little Endian
                dataReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf16LE;   // MBIM strings are UTF-16LE encoded

                string name = dataReader.ReadString((uint)(nameLength / 2));      // Length is in bytes

                deviceServicesOutputStr += "\nName[" + i + "] = {" + name + "}";
            }
            deviceServicesOutputStr += "\n================================\n";
        }

        async void ProcessOnInterfaceArrival(IMbnInterface newInterface)
        {
            // Dispatch to UI thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                string message = "";

                message += "\nOnInterfaceArrival event received for interface ID: " + newInterface.InterfaceID;
                message += "\nAnd hence enabling scenario buttons";

                EnableScenarioButtons(this, EventArgs.Empty);

                rootPage.NotifyUser(message, NotifyType.StatusMessage);
            });
        }

        async void ProcessOnInterfaceRemoval(IMbnInterface oldInterface)
        {
            // Dispatch to UI thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                string message = "";

                message += "\nOnInterfaceRemoval event received for interface ID: " + oldInterface.InterfaceID;
                message += "\nAnd hence disabling scenario buttons, please plug-in required mobilebroadband device";

                DisableScenarioButtons(this, EventArgs.Empty);

                rootPage.NotifyUser(message, NotifyType.StatusMessage);
            });
        }

        async void ProcessOnGetPinStateCompleteHandlerEvt(IMbnPinManager mbnPinManager, MBN_PIN_INFO pinInfo, uint requestId, int status)
        {
            // Dispatch to UI thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                string message = "";

                message += "\nOnGetPinStateComplete event received. Request ID: " + requestId.ToString() + " status: 0x" + status.ToString("X");

                message += "\n\nPinType:::             " + pinInfo.pinType.ToString();
                message += "\nPinState:::            " + pinInfo.pinState.ToString();
                message += "\nPinAttempts:::         " + pinInfo.attemptsRemaining.ToString();

                if (pinInfo.pinState == MBN_PIN_STATE.MBN_PIN_STATE_ENTER)
                {
                    message += "\n\nDevice is locked and hence required PIN to unlock. Enter required PIN.";

                    // Enable "Enter" button
                    EnableEnterButton(this, EventArgs.Empty);
                }

                rootPage.NotifyUser(message, NotifyType.StatusMessage);
            });
        }

        async void UpdateUiOnEnterComplete(uint requestId, int status)
        {
            // Dispatch to UI thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                string message = "";

                message += "\nOnEnterComplete event received. Request ID: " + requestId.ToString() + " status: 0x" + status.ToString("X");
                rootPage.NotifyUser(message, NotifyType.StatusMessage);
            });
        }

        void ProcessOnEnterComplete(IMbnPin mbnPin, ref MBN_PIN_INFO pinInfo, uint requestId, int status)
        {
            UpdateUiOnEnterComplete(requestId, status);
        }

        async void ProcessOnConnectComplete(IMbnConnection connection, uint requestId, int status)
        {
            // Dispatch to UI thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {

                string message = "";

                message += "\nOnConnectComplete event received. Request ID: " + requestId.ToString() + " status: 0x" + status.ToString("X");
                rootPage.NotifyUser(message, NotifyType.StatusMessage);
            });
        }

        async void ProcessOnDisconnectComplete(IMbnConnection connection, uint requestId, int status)
        {
            // Dispatch to UI thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                string message = "";

                message += "\nOnDisconnectComplete event received. Request ID: " + requestId.ToString() + " status: 0x" + status.ToString("X");
                rootPage.NotifyUser(message, NotifyType.StatusMessage);
            });
        }

        async void ProcessOnOpenCommandSessionComplete(IMbnDeviceService deviceService, int status, uint requestId)
        {
            // Dispatch to UI thread
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, new DispatchedHandler(() =>
            {
                try
                {
                    string deviceServiceID = deviceService.DeviceServiceID;
                    deviceServicesOutputStr += "\nOnOpenCommandSessionComplete event received for DS: " + deviceServiceID + " request ID: " + requestId.ToString() + " status: 0x" + status.ToString("X");
                    if (deviceService.IsCommandSessionOpen == 1)
                    {
                        uint queryRequestID = 0;
                        // Generate query data
                        // Create the MBIM_PHONEBOOK_READ_REQ structure to get all entries in the phonebook
                        // We use the DataWriter object for this purpose
                        DataWriter dataWriter = new DataWriter();
                        dataWriter.ByteOrder = ByteOrder.LittleEndian;  // MBIM byte ordering is Little Endian

                        dataWriter.WriteInt32(0);   // FilterFlag = MBIMPhonebookFlagAll
                        dataWriter.WriteInt32(0);   // FilterMessageIndex = 0

                        // Get the raw bytes out so that we can send it down to the API
                        byte[] data = dataWriter.DetachBuffer().ToArray();

                        uint commandID = 2;  // MBIM_CID_PHONEBOOK_READ
                        // Query command
                        deviceService.QueryCommand(commandID, data, out queryRequestID);
                        deviceServicesOutputStr += "\n\nWaiting for QueryCommand to complete for requestId: " + queryRequestID.ToString() + "\n";
                    }
                    rootPage.NotifyUser(deviceServicesOutputStr, NotifyType.StatusMessage);
                }
                catch (Exception e)
                {
                    deviceServicesOutputStr = "";
                    rootPage.NotifyUser(ParseExceptionCode(e), NotifyType.ErrorMessage);
                    if (deviceService.IsCommandSessionOpen == 1)
                    {
                        // Close command session
                        uint closeRequestID = 0;
                        deviceService.CloseCommandSession(out closeRequestID);
                        rootPage.NotifyUser("Waiting for CloseCommandSession to complete for requestId: " + closeRequestID.ToString(), NotifyType.StatusMessage);
                    }
                }
            }));
        }

        async void ProcessOnQueryCommandComplete(IMbnDeviceService deviceService, uint responseId, byte[] deviceServiceData, int status, uint requestId)
        {
            // Dispatch to UI thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                try
                {
                    string deviceServiceID = deviceService.DeviceServiceID;
                    deviceServicesOutputStr += "\nOnQueryCommandComplete event received for DS: " + deviceServiceID + " request ID: " + requestId.ToString() + " status: 0x" + status.ToString("X");
                    ParseResponse(deviceServiceData);

                    // Close command session
                    uint closeRequestID = 0;
                    deviceService.CloseCommandSession(out closeRequestID);
                    deviceServicesOutputStr += "\n\nWaiting for CloseCommandSession to complete for requestId: " + closeRequestID.ToString() + "\n";
                    rootPage.NotifyUser(deviceServicesOutputStr, NotifyType.StatusMessage);
                }
                catch (Exception e)
                {
                    deviceServicesOutputStr = "";
                    rootPage.NotifyUser(ParseExceptionCode(e), NotifyType.ErrorMessage);
                }
            });
        }

        async void ProcessOnCloseCommandSessionComplete(IMbnDeviceService deviceService, int status, uint requestId)
        {
            // Dispatch to UI thread
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                string deviceServiceID = deviceService.DeviceServiceID;
                deviceServicesOutputStr += "\nOnCloseCommandSessionComplete event received for DS: " + deviceServiceID + " request ID: " + requestId.ToString() + " status: 0x" + status.ToString("X");
                rootPage.NotifyUser(deviceServicesOutputStr, NotifyType.StatusMessage);
            });
        }

        // This will be called when GetPinState button is clicked
        public void GetPinStateButton_Click()
        {
            try
            {
                uint requestId = 0;
                // Get pin manager object
                IMbnPinManager pinManager = m_MbnInterface as IMbnPinManager;
                // Get pin state
                pinManager.GetPinState(out requestId);
                rootPage.NotifyUser("Waiting for GetPinState to complete for requestId: " + requestId.ToString(), NotifyType.StatusMessage);
            }
            catch (Exception e)
            {
                rootPage.NotifyUser(ParseExceptionCode(e), NotifyType.ErrorMessage);
            }
        }

        // This will be called when EnterPin button is clicked
        public void EnterPinButton_Click(string pinText)
        {
            try
            {
                uint requestId = 0;
                // Get pin manager object
                IMbnPinManager pinManager = m_MbnInterface as IMbnPinManager;
                // Get pin object for pin type as Pin1
                IMbnPin pin = pinManager.GetPin(MBN_PIN_TYPE.MBN_PIN_TYPE_PIN1);
                // Enter required pin to unlock device
                pin.Enter(pinText, out requestId);
                rootPage.NotifyUser("Waiting for EnterPin to complete for requestId: " + requestId.ToString() + "for pin: " + pinText, NotifyType.StatusMessage);
            }
            catch (Exception e)
            {
                rootPage.NotifyUser(ParseExceptionCode(e), NotifyType.ErrorMessage);
            }
        }

        // This will be called when Connect button is clicked
        public void Connect(string subscriberText, string accessStringText, string userNameText, string passwordText)
        {
            try
            {
                bool connectionState = IsConnected();

                string profileXml = ConstructProfileXml(subscriberText, accessStringText, userNameText, passwordText);

                if (connectionState)
                {
                    rootPage.NotifyUser("Connection is activated and hence disconnect before connecting again", NotifyType.StatusMessage);
                }
                else
                {
                    uint requestId = 0;
                    m_MbnConnection.Connect(
                        MBN_CONNECTION_MODE.MBN_CONNECTION_MODE_TMP_PROFILE,
                        profileXml,
                        out requestId);
                    rootPage.NotifyUser("Waiting for Connect to complete for requestId: " + requestId.ToString()
                        + " for following XML:\n" + profileXml, NotifyType.StatusMessage);
                }
            }
            catch (Exception e)
            {
                rootPage.NotifyUser(ParseExceptionCode(e), NotifyType.ErrorMessage);
            }
        }

        // This will be called when Disconnect button is clicked
        public void Disconnect()
        {
            try
            {
                uint requestId = 0;
                m_MbnConnection.Disconnect(out requestId);
                rootPage.NotifyUser("Waiting for Disconnect to complete for requestId: " + requestId.ToString(), NotifyType.StatusMessage);
            }
            catch (Exception e)
            {
                rootPage.NotifyUser(ParseExceptionCode(e), NotifyType.ErrorMessage);
            }
        }

        // This will be called when EnumeratePhoneBook button is clicked
        public void EnumeratePhoneBookButton()
        {
            try
            {
                deviceServicesOutputStr = "";

                IReadOnlyList<MBN_DEVICE_SERVICE> deviceServiceList = m_MbnDeviceServicesContext.EnumerateDeviceServices();
                IMbnDeviceService deviceService;
                uint requestId = 0;

                if (deviceServiceList.Count == 0)
                {
                    rootPage.NotifyUser("No device service list found", NotifyType.ErrorMessage); ;
                    return;
                }

                deviceServicesOutputStr = "Available device service ID(s): \n";

                foreach (MBN_DEVICE_SERVICE mbnDeviceService in deviceServiceList)
                {
                    deviceServicesOutputStr += mbnDeviceService.DeviceServiceID + "\n";

                    // {4bf38476-1e6a-41db-b1d8-bed289c25bdb} is Phone book GUID
                    if (String.Compare(mbnDeviceService.DeviceServiceID, ("{4bf38476-1e6a-41db-b1d8-bed289c25bdb}"), StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        deviceServicesOutputStr += "\nPhone book functionality is present";
                        deviceService = m_MbnDeviceServicesContext.GetDeviceService(mbnDeviceService.DeviceServiceID);
                        // Open command session
                        deviceService.OpenCommandSession(out requestId);
                        deviceServicesOutputStr += "\nWaiting for OpenCommandSession to complete for requestId: " + requestId.ToString() + "\n";
                    }
                }

                rootPage.NotifyUser(deviceServicesOutputStr, NotifyType.StatusMessage);
            }
            catch (Exception e)
            {
                deviceServicesOutputStr = "";
                rootPage.NotifyUser(ParseExceptionCode(e), NotifyType.ErrorMessage);
            }
        }

        // This will be called when GetDeviceCapability button is clicked
        public void GetDeviceCapability()
        {
            try
            {
                if (m_MbnInterface == null)
                {
                    m_MbnInterface = GetFirstInterface();
                }
                
                var cap = m_MbnInterface.GetInterfaceCapability();

                string message = "";
                message += "Device capabilities:";
                message += "\n\n MBN_CELLULAR_CLASS = " + cap.cellularClass.ToString();
                message += "\n MBN_VOICE_CLASS = " + cap.voiceClass.ToString();
                message += "\n Device ID = " + cap.deviceID.ToString();
                message += "\n Manufacturer = " + cap.manufacturer.ToString();
                message += "\n Model = " + cap.model.ToString();
                message += "\n Firmware Info = " + cap.firmwareInfo.ToString();

                rootPage.NotifyUser(message, NotifyType.StatusMessage);
            }
            catch (Exception e)
            {
                rootPage.NotifyUser(ParseExceptionCode(e), NotifyType.ErrorMessage);
            }
        }
    }
}
