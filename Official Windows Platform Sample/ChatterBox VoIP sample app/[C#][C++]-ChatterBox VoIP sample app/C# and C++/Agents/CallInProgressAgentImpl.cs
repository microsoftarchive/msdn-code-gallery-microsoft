/* 
    Copyright (c) 2012 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://go.microsoft.com/fwlink/?LinkID=219604 
  
*/
using System.Diagnostics;
using Microsoft.Phone.Networking.Voip;

namespace PhoneVoIPApp.Agents
{
    /// <summary>
    /// An agent that is launched when the first call becomes active and is canceled when the last call ends.
    /// </summary>
    public class CallInProgressAgentImpl : VoipCallInProgressAgent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CallInProgressAgentImpl()
            : base()
        {
        }

        /// <summary>
        /// The first call has become active.
        /// </summary>
        protected override void OnFirstCallStarting()
        {
            Debug.WriteLine("[CallInProgressAgentImpl] The first call has started.");

            // Indicate that an agent has started running
            AgentHost.OnAgentStarted();
        }

        /// <summary>
        /// The last call has ended.
        /// </summary>
        protected override void OnCancel()
        {
            Debug.WriteLine("[CallInProgressAgentImpl] The last call has ended. Calling NotifyComplete");

            // This agent is done
            base.NotifyComplete();
        }
    }
}
