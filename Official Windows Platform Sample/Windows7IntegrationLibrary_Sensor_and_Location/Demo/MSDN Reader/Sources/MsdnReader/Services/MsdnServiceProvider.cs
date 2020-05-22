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
using Microsoft.SceReader;
using System.Xml.XPath;
using System.Windows.Documents;

namespace MsdnReader
{
    /// <summary>
    /// Msdn ServiceProvider
    /// </summary>
    public class MsdnServiceProvider : ServiceProvider
    {

        #region Public Properties

        /// <summary>
        /// SubscriptionServiceManager manages connections and synchronization with the subscription service
        /// </summary>
        public static SubscriptionServiceManager SubscriptionServiceManager
        {
            get
            {
                VerifyInitialized();
                if (((MsdnServiceProvider)Instance).SubscriptionServiceManagerInternal == null)
                {
                    ((MsdnServiceProvider)Instance).SubscriptionServiceManagerInternal = new SubscriptionServiceManager();
                }
                return ((MsdnServiceProvider)Instance).SubscriptionServiceManagerInternal;
            }
        }

        /// <summary>
        /// ApplicationNavigationHandler handles global navigation events
        /// </summary>
        public static ApplicationInputHandler ApplicationInputHandler
        {
            get
            {
                VerifyInitialized();
                if (((MsdnServiceProvider)Instance).ApplicationInputHandlerInternal == null)
                {
                    ((MsdnServiceProvider)Instance).ApplicationInputHandlerInternal = new ApplicationInputHandler();
                }
                return ((MsdnServiceProvider)Instance).ApplicationInputHandlerInternal;
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// SubscriptionServiceManager manages connections and synchronization with the subscription service
        /// </summary>
        private SubscriptionServiceManager SubscriptionServiceManagerInternal
        {
            get { return _subscriptionServiceManager; }
            set { _subscriptionServiceManager = value; }
        }

        /// <summary>
        /// ApplicationNavigationHandler handles global navigation events
        /// </summary>
        private ApplicationInputHandler ApplicationInputHandlerInternal
        {
            get { return _applicationInputHandler; }
            set { _applicationInputHandler = value; }
        }

        #endregion

        //-------------------------------------------------------------------
        //
        //  Protected Methods
        //
        //-------------------------------------------------------------------

        #region Protected Methods

        /// <summary>
        /// Register Msdn converters
        /// </summary>
        protected override void RegisterConverters()
        {
            base.RegisterConverters();
            ConverterManager.UnRegister(typeof(XPathDocument), typeof(FlowDocument));
            ConverterManager.Register(typeof(XPathDocument), typeof(FlowDocument), new MsdnStoryToFlowDocumentConverter());
        }

        /// <summary>
        /// Initialize MSDN print manager
        /// </summary>
        protected override void OnInitializePrintManager()
        {
            PrintManagerInternal = new MsdnPrintManager();
        }

        /// <summary>
        /// Initialize MSDN view manager
        /// </summary>
        protected override void OnInitializeViewManager()
        {
            ViewManagerInternal = new MsdnViewManager();
        }    

        #endregion

        #region Private Fields

        private SubscriptionServiceManager _subscriptionServiceManager;
        private ApplicationInputHandler _applicationInputHandler;

        #endregion
    }
}