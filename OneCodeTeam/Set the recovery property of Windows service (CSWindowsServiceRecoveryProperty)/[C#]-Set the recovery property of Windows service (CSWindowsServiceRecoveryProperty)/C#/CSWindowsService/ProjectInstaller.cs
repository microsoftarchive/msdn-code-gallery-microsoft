/****************************** Module Header ******************************\
* Module Name:  ProjectInstaller.cs
* Project:      CSWindowsService
* Copyright (c) Microsoft Corporation.
* 
* The ProjectInstaller component contains two installers by default.
* 
* * serviceInstaller1 - installer for your service. The ServiceInstaller 
*   does work specific to the service with which it is associated. It is used 
*   by the installation utility to write registry values associated with the 
*   service to a subkey within the 
*   HKEY_LOCAL_MACHINE\System\CurrentControlSet\Services registry key. The 
*   service is identified by its ServiceName within this subkey. The subkey 
*   also includes the name of the executable or .dll to which the service 
*   belongs. If you have muliple services in one process, you should add 
*   multiple ServiceInstaller components to be associated with each service.
* 
* * serviceProcessInstaller1 -  installer for the service's associated 
*   process. The ServiceProcessInstaller does work common to all services in 
*   an executable. It is used by the installation utility to write registry 
*   values associated with services you want to install.
* 
* You can register events (e.g. AfterInstall) of ServiceInstaller and 
* ServiceProcessInstaller components, and handle the events in 
* ProjectInstaller.cs.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#region Using directives
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
#endregion


namespace CSWindowsService
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}