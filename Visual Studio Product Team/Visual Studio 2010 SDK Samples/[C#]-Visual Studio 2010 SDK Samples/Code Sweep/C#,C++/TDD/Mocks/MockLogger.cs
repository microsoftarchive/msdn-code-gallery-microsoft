/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Framework;

namespace Microsoft.Samples.VisualStudio.CodeSweep.UnitTests
{
    class MockLogger : ILogger
    {
        public event EventHandler<BuildErrorEventArgs> OnError;
        public event EventHandler<BuildWarningEventArgs> OnWarning;
        public event EventHandler<BuildMessageEventArgs> OnMessage;
        public event EventHandler<BuildStartedEventArgs> OnBuildStart;

        IEventSource _eventSource;

        #region ILogger Members

        public void Initialize(IEventSource eventSource)
        {
            _eventSource = eventSource;
            eventSource.ErrorRaised += new BuildErrorEventHandler(eventSource_ErrorRaised);
            eventSource.WarningRaised += new BuildWarningEventHandler(eventSource_WarningRaised);
            eventSource.MessageRaised += new BuildMessageEventHandler(eventSource_MessageRaised);
            eventSource.BuildStarted += new BuildStartedEventHandler(eventSource_BuildStarted);
        }

        void eventSource_BuildStarted(object sender, BuildStartedEventArgs e)
        {
            if (OnBuildStart != null)
            {
                OnBuildStart(this, e);
            }
        }

        void eventSource_MessageRaised(object sender, BuildMessageEventArgs e)
        {
            if (OnMessage != null)
            {
                OnMessage(this, e);
            }
        }

        void eventSource_WarningRaised(object sender, BuildWarningEventArgs e)
        {
            if (OnWarning != null)
            {
                OnWarning(this, e);
            }
        }

        void eventSource_ErrorRaised(object sender, BuildErrorEventArgs e)
        {
            if (OnError != null)
            {
                OnError(this, e);
            }
        }

        public string Parameters
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        public void Shutdown()
        {
        }

        public LoggerVerbosity Verbosity
        {
            get
            {
                return LoggerVerbosity.Normal;
            }
            set
            {
            }
        }

        #endregion
    }
}
