/****************************** Module Header ******************************\
* Module Name:    Default.aspx.cs
* Project:        CSASPNETBackgroundWorker
* Copyright (c) Microsoft Corporation
*
* This page displays a TextBox. When the user clicks the Button, the page creates 
* a BackgroundWorker object then starts it by passing the value which is inputed 
* through the TextBox. At last, the BackgroundWorker object is stored in Session 
* State, so that it will keep working even the current request ended.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
*
\*****************************************************************************/

using System;
using System.Threading;

namespace CSASPNETBackgroundWorker
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Timer1_Tick(object sender, EventArgs e)
        {
            // Show the progress of current operation.
            BackgroundWorker worker = (BackgroundWorker)Session["worker"];
            if (worker != null)
            {
                // Display the progress of the operation.
                lbProgress.Text = "Running: " + worker.Progress.ToString() + "%";

                btnStart.Enabled = !worker.IsRunning;
                Timer1.Enabled = worker.IsRunning;

                // Display the result when the operation completed.
                if (worker.Progress >= 100)
                {
                    lbProgress.Text = (string)worker.Result;
                }
            }
        }

        /// <summary>
        /// Create a Background Worker to run the operation when button clicked.
        /// </summary>
        protected void btnStart_Click(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new BackgroundWorker.DoWorkEventHandler(worker_DoWork);
            worker.RunWorker(txtParameter.Text);

            // It needs Session Mode is "InProc"
            // to keep the Background Worker working.
            Session["worker"] = worker;

            // Enable the timer to update the status of the operation.
            Timer1.Enabled = true;
        }

        /// <summary>
        /// This method is the operation that needs long time to complete.
        /// </summary>
        void worker_DoWork(ref int progress, 
            ref object result, params object[] arguments)
        {
            // Get the value which passed to this operation.
            string input = string.Empty;
            if (arguments.Length > 0)
            {
                input = arguments[0].ToString();
            }

            // Need 10 seconds to complete this operation.
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(100);

                progress += 1;
            }

            // The operation is completed.
            progress = 100;
            result = "Operation is completed. The input is \"" + input + "\".";
        }
    }
}