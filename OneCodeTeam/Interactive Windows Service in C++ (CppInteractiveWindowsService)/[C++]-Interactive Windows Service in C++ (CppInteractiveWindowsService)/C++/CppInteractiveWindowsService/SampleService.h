/****************************** Module Header ******************************\
* Module Name:  SampleService.h
* Project:      CppInteractiveWindowsService
* Copyright (c) Microsoft Corporation.
* 
* Provides a sample service class that derives from the service base class - 
* CServiceBase. The sample service logs the service start and stop 
* information to the Application event log, and shows how to run the main 
* function of the service in a thread pool worker thread.
* 
* As mentioned above, any service that assumes the user is in the same session is going to have problems interacting with users.  Services that show dialogs like this should use alternative methods; for example, use the Terminal Services APIs such as WTSSendMessage to send messages to the appropriate session or use CreateProcessAsUser to create a new process in the user¡¯s session. 
* 
* Use the WTSSendMessage function to create a simple message box on the user¡¯s desktop. This allows the service to give the user a notification and request a simple response. 

For more complex UI, use the CreateProcessAsUser function to create a process in the user's session. 

* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

#pragma once

#include "ServiceBase.h"


class CSampleService : public CServiceBase
{
public:

    CSampleService(PWSTR pszServiceName, 
        BOOL fCanStop = TRUE, 
        BOOL fCanShutdown = TRUE, 
        BOOL fCanPauseContinue = FALSE);
    virtual ~CSampleService(void);

protected:

    virtual void OnStart(DWORD dwArgc, PWSTR *pszArgv);
    virtual void OnStop();

    void ServiceWorkerThread(void);

private:

    HANDLE m_hFinishedEvent;
};