/****************************** Module Header ******************************\
*Module Name:  Global.asax.cs
*Project:      CSAzureReceiveSalesMessageViaTopic
*Copyright (c) Microsoft Corporation.
* 
*In contrast to queues, in which each message is processed by a single consumer,
*topics and subscriptions provide a one-to-many form of communication, in a publish/subscribe pattern.
*
*This project will automatically receive messages that the sales department send when the sales order is built.
*
*This source is subject to the Microsoft Public License.
*See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
*All other rights reserved.
* 
*THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
*EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
*WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace CSAzureReceiveSalesMessageViaTopic
{
    public class Global : System.Web.HttpApplication
    {
         string pathClothes = AppDomain.CurrentDomain.BaseDirectory + "bin" + "\\ConsoleReceiveSalesClothesMessage.exe"; 
        string pathFootWear = AppDomain.CurrentDomain.BaseDirectory + "bin" + "\\ConsoleReceiveSalesFootWearMessage.exe";
            string strClothesName = "";
        string strFootwear = "";
        
        /// <summary>
        /// Starts two processes to receive messages that the sales department send.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Start(object sender, EventArgs e)
        {

            ProcessStartInfo processClothes = new ProcessStartInfo(pathClothes, "");
            processClothes.CreateNoWindow = true;
            processClothes.UseShellExecute = false;
            processClothes.WindowStyle = ProcessWindowStyle.Hidden;
            Process myProcessClothes = Process.Start(processClothes);
            strClothesName = myProcessClothes.ProcessName;

           
            ProcessStartInfo processFootWear = new ProcessStartInfo(pathFootWear, "");
            processFootWear.CreateNoWindow = true;
            processFootWear.UseShellExecute = false;
            processFootWear.WindowStyle = ProcessWindowStyle.Hidden;
            Process myProcessFootWear = Process.Start(processFootWear);
          
            strFootwear = myProcessFootWear.ProcessName;
           
        }

        protected void Application_End(object sender,EventArgs e)
        {
            Process[] proList = Process.GetProcesses(".");

            for (int i = 0; i < proList.Length; i++)
            {

                if (proList[i].ProcessName.Contains(strClothesName))
                {
                    proList[i].Kill();
                }
                else if (proList[i].ProcessName.Contains(strFootwear))
                {
                    proList[i].Kill();
                }
            }
        }


    }
}