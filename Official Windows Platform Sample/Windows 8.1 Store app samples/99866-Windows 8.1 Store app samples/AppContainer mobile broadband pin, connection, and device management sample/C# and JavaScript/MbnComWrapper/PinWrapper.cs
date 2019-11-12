//// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
//// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
//// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//// PARTICULAR PURPOSE.
////
//// Copyright (c) Microsoft Corporation. All rights reserved

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
using System.Diagnostics;

namespace MbnComWrapper
{
    public delegate void JSCallback(String msg);
    delegate void OnGetPinStateCompleteHandler(IMbnPinManager mbnPinManager, MBN_PIN_INFO pinInfo, uint requestId, int status);
    delegate void OnEnterCompleteHandler(IMbnPin mbnPin, ref MBN_PIN_INFO pinInfo, uint requestId, int status);

    // Pin info struct which will be returned to JS
    public struct MbnPinInfo
    {
        public string msg;
        public bool isPinEnabled;
    }

    public sealed class PinWrapper
    {
        IMbnInterfaceManager m_MbnInterfaceManager;
        IMbnInterface m_MbnInterface;
        PinManagerEventsSink m_PinManagerEventsSink;
        PinEventsSink m_PinEventsSink;

        public event EventHandler<Object> RaiseOnGetPinStateCompleteEvent;
        public event EventHandler<Object> RaiseOnPinEnterCompleteEvent;

        JSCallback m_jsCallback;

        public PinWrapper(JSCallback jsCallback)
        {
            m_jsCallback = jsCallback;

            if (m_MbnInterfaceManager == null)
            {
                // Get MbnInterfaceManager
                m_MbnInterfaceManager = (IMbnInterfaceManager)new MbnInterfaceManager();
            }

            // Get the mbn interface
            m_MbnInterface = GetFirstInterface();

            if (m_PinManagerEventsSink == null)
            {
                // Advise for pin manager events
                m_PinManagerEventsSink = new PinManagerEventsSink(this.OnGetPinStateCompleteHandler, GetMbnPinManagerEventsConnectionPoint());
            }

            if (m_PinEventsSink == null)
            {
                // Advise for pin events
                m_PinEventsSink = new PinEventsSink(this.OnPinEnterComplete, GetMbnPinEventsConnectionPoint());
            }
        }

        ~PinWrapper()
        {
            if (m_PinManagerEventsSink != null)
            {
                m_PinManagerEventsSink.Dispose();
                m_PinManagerEventsSink = null;
            }

            if (m_PinEventsSink != null)
            {
                m_PinEventsSink.Dispose();
                m_PinEventsSink = null;
            }
        }

        // This will be called to get pin state
        public void GetPinState()
        {
            uint requestId = 0;
            IMbnPinManager pinManager = m_MbnInterface as IMbnPinManager;
            pinManager.GetPinState(out requestId);
            m_jsCallback("Waiting for GetPinState to complete for requestId: " + requestId.ToString());
        }

        // This will be called to enter pin
        public void EnterPin(string pinText)
        {
            uint requestId = 0;
            IMbnPinManager pinManager = m_MbnInterface as IMbnPinManager;
            IMbnPin pin = pinManager.GetPin(MBN_PIN_TYPE.MBN_PIN_TYPE_PIN1);
            pin.Enter(pinText, out requestId);

            // Display the message
            m_jsCallback("Waiting for EnterPin to complete for requestId: " + requestId.ToString());
        }

        // This will be called to get the device capabilities
        public void GetDeviceCapability()
        {
            var cap = m_MbnInterface.GetInterfaceCapability();

            string message = "";
            message += "Device capabilities:";
            message += "\n\n MBN_CELLULAR_CLASS = " + cap.cellularClass.ToString();
            message += "\n MBN_VOICE_CLASS = " + cap.voiceClass.ToString();
            message += "\n Device ID = " + cap.deviceID.ToString();
            message += "\n Manufacturer = " + cap.manufacturer.ToString();
            message += "\n Model = " + cap.model.ToString();
            message += "\n Firmware Info = " + cap.firmwareInfo.ToString();

            // Display the message
            m_jsCallback(message);
        }
        
        void OnGetPinStateCompleteHandler(IMbnPinManager mbnPinManager, MBN_PIN_INFO pinInfo, uint requestId, int status)
        {
            if (RaiseOnGetPinStateCompleteEvent != null)
            {
                string message = "";
                message += "OnGetPinStateComplete event received. Request ID: " + requestId.ToString() + " status: 0x" + status.ToString("X");
                message += "\n\nPinType:::             " + pinInfo.pinType.ToString();
                message += "\nPinState:::            " + pinInfo.pinState.ToString();
                message += "\nPinAttempts:::         " + pinInfo.attemptsRemaining.ToString();

                MbnPinInfo mbnPinInfo;
                mbnPinInfo.msg = message;
                if (pinInfo.pinState == MBN_PIN_STATE.MBN_PIN_STATE_ENTER)
                {
                    mbnPinInfo.isPinEnabled = true;
                }
                else
                {
                    mbnPinInfo.isPinEnabled = false;
                }

                // Raise event
                RaiseOnGetPinStateCompleteEvent(this, mbnPinInfo);
            }
        }

        void OnPinEnterComplete(IMbnPin mbnPin, ref MBN_PIN_INFO pinInfo, uint requestId, int status)
        {
            if (RaiseOnPinEnterCompleteEvent != null)
            {
                string message = "";
                message += "OnEnterComplete event received. Request ID: " + requestId.ToString() + " status: 0x" + status.ToString("X");
                if (status == -2141945326)
                {
                    message += "\n\nThe pin is invalid";
                }
                message += "\n\nPinType:::             " + pinInfo.pinType.ToString();
                message += "\nPinState:::            " + pinInfo.pinState.ToString();
                message += "\nPinAttempts:::         " + pinInfo.attemptsRemaining.ToString();

                MbnPinInfo mbnPinInfo;
                mbnPinInfo.msg = message;
                if (pinInfo.pinState == MBN_PIN_STATE.MBN_PIN_STATE_ENTER)
                {
                    mbnPinInfo.isPinEnabled = true;
                }
                else
                {
                    mbnPinInfo.isPinEnabled = false;
                }

                // Raise event
                RaiseOnPinEnterCompleteEvent(this, mbnPinInfo);
            }
        }

        // For simplicity, this sample uses first interface
        private IMbnInterface GetFirstInterface()
        {
            IMbnInterface[] interfaces = m_MbnInterfaceManager.GetInterfaces();
            foreach (IMbnInterface mbnInterface in m_MbnInterfaceManager.GetInterfaces())
            {
                return mbnInterface;
            }
            return null;
        }

        // Get MbnPinManagerEventsConnectionPoint
        private IConnectionPoint GetMbnPinManagerEventsConnectionPoint()
        {
            IConnectionPointContainer connectionPointContainer = (IConnectionPointContainer)m_MbnInterfaceManager;
            Guid iid_IMbnPinManagerEvents = typeof(IMbnPinManagerEvents).GetTypeInfo().GUID;

            IConnectionPoint connectionPoint;
            connectionPointContainer.FindConnectionPoint(ref iid_IMbnPinManagerEvents, out connectionPoint);

            return connectionPoint;
        }

        // Get MbnPinEventsConnectionPoint
        private IConnectionPoint GetMbnPinEventsConnectionPoint()
        {
            IConnectionPointContainer connectionPointContainer = (IConnectionPointContainer)m_MbnInterfaceManager;
            Guid iid_IMbnPinEvents = typeof(IMbnPinEvents).GetTypeInfo().GUID;

            IConnectionPoint connectionPoint;
            connectionPointContainer.FindConnectionPoint(ref iid_IMbnPinEvents, out connectionPoint);

            return connectionPoint;
        }
    }

    // This class implements IMbnPinManagerEvents
    class PinManagerEventsSink : IMbnPinManagerEvents, IDisposable
    {
        // Using weak reference of Main page as in any case we have reference
        // of that, the underlying object can be reclaimed by gargabe collection
        private WeakReference<OnGetPinStateCompleteHandler> m_Callback;
        private IConnectionPoint m_ConnectionPoint;
        private uint m_AdviceCookie;

        public PinManagerEventsSink(OnGetPinStateCompleteHandler callback, IConnectionPoint connectionPoint)
        {
            m_Callback = new WeakReference<OnGetPinStateCompleteHandler>(callback);
            m_ConnectionPoint = connectionPoint;
            m_ConnectionPoint.Advise(this, out m_AdviceCookie);
        }

        ~PinManagerEventsSink()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (m_AdviceCookie != 0)
            {
                m_ConnectionPoint.Unadvise(m_AdviceCookie);
                m_AdviceCookie = 0;
            }
        }

        // This will be called back when the get pin operation is complete
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
        public void OnPinListAvailable(IMbnPinManager mbnPinManager)
        {
        }
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

        // This will be called back when the pin enter operation is complete
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
}
