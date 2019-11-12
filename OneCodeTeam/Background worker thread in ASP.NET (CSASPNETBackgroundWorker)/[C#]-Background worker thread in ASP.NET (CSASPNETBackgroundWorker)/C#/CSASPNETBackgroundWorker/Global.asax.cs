/****************************** Module Header ******************************\
* Module Name:    Global.asax.cs
* Project:        CSASPNETBackgroundWorker
* Copyright (c) Microsoft Corporation
*
* When application starts up, Application_Start() method will be called. 
* In the Application_Start() method, it creates a BackgroundWorker object and 
* then stores it in Application State. Therefore, the worker_DoWork() will 
* keep executing until application ends. 
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
    public class Global : System.Web.HttpApplication
    {
        /// <summary>
        /// Create a Background Worker to run the operation
        /// whenever the application start.
        /// </summary>
        protected void Application_Start(object sender, EventArgs e)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += new BackgroundWorker.DoWorkEventHandler(worker_DoWork);
            worker.RunWorker(null);

            // This Background Worker is Applicatoin Level,
            // so it will keep working and it is shared by all users.
            Application["worker"] = worker;
        }

        /// <summary>
        /// This operation will work without the end.
        /// </summary>
        void worker_DoWork(ref int progress, 
            ref object _result, params object[] arguments)
        {
            // Do the operation every 1 second wihout the end.
            while (true)
            {
                Thread.Sleep(1000);

                // This statement will run every 1 second.
                progress++;

                // Other logic which you want it to keep running.
                // You can do some scheduled tasks here by checking DateTime.Now.
            }
        }
    }
}