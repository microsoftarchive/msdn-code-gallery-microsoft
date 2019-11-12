/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Phone.Networking.Voip;
using Microsoft.Phone.Scheduler;

namespace PhoneVoIPApp.Agents
{
    public class ScheduledAgentImpl : ScheduledTaskAgent
    {
        /// <remarks>
        /// ScheduledAgent constructor
        /// </remarks>
        public ScheduledAgentImpl()
        {
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        protected override void OnInvoke(ScheduledTask task)
        {
            Debug.WriteLine("[ScheduledAgentImpl] ScheduledAgentImpl has been invoked with argument of type {0}.", task.GetType());

            // Indicate that an agent has started running
            AgentHost.OnAgentStarted();

            VoipHttpIncomingCallTask incomingCallTask = task as VoipHttpIncomingCallTask;
            if (incomingCallTask != null)
            {
                this.isIncomingCallAgent = true;

                // Parse the the incoming push notification payload
                Notification pushNotification;
                using (MemoryStream ms = new MemoryStream(incomingCallTask.MessageBody))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Notification));
                    pushNotification = (Notification)xs.Deserialize(ms);
                }

                Debug.WriteLine("[{0}] Incoming call from caller {1}, number {2}", ScheduledAgentImpl.incomingCallAgentId, pushNotification.Name, pushNotification.Number);

                // Initiate incoming call processing
                // If you want to pass in additional information such as pushNotification.Number, you can
                bool incomingCallProcessingStarted = BackEnd.Globals.Instance.CallController.OnIncomingCallReceived(pushNotification.Name, pushNotification.Number, this.OnIncomingCallDialogDismissed);

                if (!incomingCallProcessingStarted)
                {
                    // For some reasons, the incoming call processing was not started.
                    // There is nothing more to do.
                    this.Complete();
                    return;
                }
            }
            else
            {
                VoipKeepAliveTask keepAliveTask = task as VoipKeepAliveTask;
                if (keepAliveTask != null)
                {
                    this.isIncomingCallAgent = false;

                    // Refresh tokens, get new certs from server, etc.
                    BackEnd.Globals.Instance.DoPeriodicKeepAlive();
                    this.Complete();
                }
                else
                {
                    throw new InvalidOperationException(string.Format("Unknown scheduled task type {0}", task.GetType()));
                }
            }
        }

        // This is a request to complete this agent
        protected override void OnCancel()
        {
            Debug.WriteLine("[{0}] Cancel requested.", this.isIncomingCallAgent ? ScheduledAgentImpl.incomingCallAgentId : ScheduledAgentImpl.keepAliveAgentId);
            this.Complete();
        }

        // This method is called when the incoming call processing is complete
        private void OnIncomingCallDialogDismissed()
        {
            Debug.WriteLine("[IncomingCallAgent] Incoming call processing is now complete.");
            this.Complete();
        }

        // Complete this agent.
        private void Complete()
        {
            Debug.WriteLine("[{0}] Calling NotifyComplete", this.isIncomingCallAgent ? ScheduledAgentImpl.incomingCallAgentId : ScheduledAgentImpl.keepAliveAgentId);

            // This agent is done
            base.NotifyComplete();
        }

        // Strings used in tracing
        private const string keepAliveAgentId = "KeepAliveAgent";
        private const string incomingCallAgentId = "IncomingCallAgent";

        // Indicates if this agent instance is handling an incoming call or not
        private bool isIncomingCallAgent;
    }
}
