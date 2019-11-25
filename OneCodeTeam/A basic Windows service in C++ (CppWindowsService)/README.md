# A basic Windows service in C++ (CppWindowsService)
## Requires
- Visual Studio 2008
## License
- MS-LPL
## Technologies
- Windows SDK
## Topics
- Windows Service
## Updated
- 03/01/2012
## Description

<h1>SERVICE APPLICATION (<span class="SpellE">CppWindowsService</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">This code sample demonstrates creating a very basic Windows Service application in Visual C&#43;&#43;. The
<span class="GramE">example Windows Service logs the service start and stop</span> information to the Application event log, and shows how to run the main function of the service in a thread pool worker thread. You can easily extend the Windows Service skeleton
 to meet your own business requirement.</p>
<h2>Running the Sample</h2>
<p class="MsoNormal">The following steps walk through a demonstration of the Windows Service sample.</p>
<p class="MsoNormal">Step1. After you successfully build the sample project in Visual Studio 2008, you will get a service application: CppWindowsService.exe.
</p>
<p class="MsoNormal">Step2. Run a command prompt as administrator, navigate to the output folder of the sample project, and enter the following command to install the service.
</p>
<p class="MsoNormal"><span style="">&nbsp; </span>CppWindowsService.exe -install
</p>
<p class="MsoNormal">The service is successfully installed if the process outputs:</p>
<p class="MsoNormal"><span style=""><img src="53107-image.png" alt="" width="576" height="299" align="middle">
</span></p>
<p class="MsoNormal">If you do not see this output, please look for error codes in the
<span class="SpellE">ouput</span>, and investigate the cause of failure. For example, the error code 0x431 means that the service already exists, and you need to uninstall it first.
</p>
<p class="MsoNormal">Step3. Open Service Management Console (services.msc). You should be able to find &quot;<span class="SpellE">CppWindowsService</span> Sample Service&quot; in the service list.</p>
<p class="MsoNormal"><span style=""><img src="53108-image.png" alt="" width="576" height="412" align="middle">
</span></p>
<p class="MsoNormal">Step4. <span class="GramE">Right-click the CppWindowsService service in Service Management Console and select Start to start the service.</span> Open Event Viewer, and navigate to Windows Logs / Application. You should be able to see
 this event from <span class="SpellE">CppWindowsService</span> with the information:</p>
<p class="MsoNormal"><span style=""><img src="53109-image.png" alt="" width="576" height="443" align="middle">
</span></p>
<p class="MsoNormal">Step5. <span class="GramE">Right-click the service in Service Management Console and select Stop to stop the service.</span> You will see this new event from
<span class="SpellE">CppWindowsService</span> in Event Viewer / Windows Logs / Application with the information:</p>
<p class="MsoNormal"><span style=""><img src="53110-image.png" alt="" width="576" height="443" align="middle">
</span></p>
<p class="MsoNormal">Step6. To uninstall the service, enter the following command in the command prompt running as administrator.
</p>
<p class="MsoNormal"></p>
<p class="MsoNormal"><span style="">&nbsp; </span>CppWindowsService.exe -remove
</p>
<p class="MsoNormal"></p>
<p class="MsoNormal">If the service is successfully removed, you would see this output:</p>
<p class="MsoNormal"><span style=""><img src="53111-image.png" alt="" width="576" height="299" align="middle">
</span></p>
<h2></h2>
<h2>Using the Code</h2>
<h3><span style="font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">Step1. In Visual Studio 2008, add a new Visual C&#43;&#43; / Win32 / Win32 Console Application project named
<span class="SpellE">CppWindowsService</span>. Unselect the &quot;Precompiled header&quot; option in Application Settings of the Win32 Application Wizard, and delete stdafx.h, stdafx.cpp, targetver.h files after the project is created.
</span></h3>
<h3><span style="font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">Step2. Define the settings of the service in CppWindowsService.cpp.</span></h3>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
// Internal name of the service
   #define SERVICE_NAME             L&quot;CppWindowsService&quot;


   // Displayed name of the service
   #define SERVICE_DISPLAY_NAME     L&quot;CppWindowsService Sample Service&quot;


   // Service start options.
   #define SERVICE_START_TYPE       SERVICE_DEMAND_START


   // List of service dependencies - &quot;dep1\0dep2\0\0&quot;
   #define SERVICE_DEPENDENCIES     L&quot;&quot;


   // The name of the account under which the service should run
   #define SERVICE_ACCOUNT          L&quot;NT AUTHORITY\\LocalService&quot;


   // The password to the service account name
   #define SERVICE_PASSWORD         NULL

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<h3><span style="font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">Security Note: In this code sample, the service is configured to run as
<span class="SpellE">LocalService</span>, instead of <span class="SpellE">LocalSystem</span>. The LocalSystem account has broad permissions. Use the LocalSystem account with caution, because it might increase your risk of attacks from malicious software.
 For tasks that do not need broad permissions, consider using the LocalService account, which acts as a non-privileged user on the local computer and presents anonymous credentials to any remote server.
</span></h3>
<h3><span style="font-family:&quot;Calibri&quot;,&quot;sans-serif&quot;; font-weight:normal">Step3. Replace the application's entry point (main) in CppWindowsService.cpp with the code below. According to the arguments in the command line, the function installs or uninstalls or
 starts the service by calling into different routines that will be declared and implemented in the next steps</span></h3>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
int wmain(int argc, wchar_t *argv[])
    {
        if ((argc &gt; 1) && ((*argv[1] == L'-' || (*argv[1] == L'/'))))
        {
            if (_wcsicmp(L&quot;install&quot;, argv[1] &#43; 1) == 0)
            {
                // Install the service when the command is 
                // &quot;-install&quot; or &quot;/install&quot;.
                InstallService(
                    SERVICE_NAME,               // Name of service
                    SERVICE_DISPLAY_NAME,       // Name to display
                    SERVICE_START_TYPE,         // Service start type
                    SERVICE_DEPENDENCIES,       // Dependencies
                    SERVICE_ACCOUNT,            // Service running account
                    SERVICE_PASSWORD            // Password of the account
                    );
            }
            else if (_wcsicmp(L&quot;remove&quot;, argv[1] &#43; 1) == 0)
            {
                // Uninstall the service when the command is 
                // &quot;-remove&quot; or &quot;/remove&quot;.
                UninstallService(SERVICE_NAME);
            }
        }
        else
        {
            wprintf(L&quot;Parameters:\n&quot;);
            wprintf(L&quot; -install  to install the service.\n&quot;);
            wprintf(L&quot; -remove   to remove the service.\n&quot;);


            CSampleService service(SERVICE_NAME);
            if (!CServiceBase::Run(service))
            {
                wprintf(L&quot;Service failed to run w/err 0x%08lx\n&quot;, GetLastError());
            }
        }


        return 0;
    }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step4. Add the ServiceBase.h and ServiceBase.cpp files to provide a base class for a service that will exist as part of a service application. The class is named &quot;<span class="SpellE">CServiceBase</span>&quot;. It must be derived
 from when creating a new service class. </p>
<p class="MsoNormal">The service base class has these public functions:</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
// It register the executable for a service with SCM.
  static BOOL CServiceBase::Run(CServiceBase &service)


  // This is the constructor of the service class. The optional parameters 
  // (fCanStop, fCanShutdown and fCanPauseContinue) allow you to specify 
  // whether the service can be stopped, paused and continued, or be 
  // notified when system shutdown occurs.
  CServiceBase::CServiceBase(PWSTR pszServiceName, 
      BOOL fCanStop = TRUE, 
      BOOL fCanShutdown = TRUE, 
      BOOL fCanPauseContinue = FALSE)


  // This is the virtual destructor of the service class.
  virtual ~CServiceBase::CServiceBase(void);
  
  // Funtion that stops the service.
  void CServiceBase::Stop();

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">The class also provides these virtual member functions. You can implement them in a derived class. The functions execute when the service starts, stops, pauses, resumes, and when the system is shutting down.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
virtual void OnStart(DWORD dwArgc, PWSTR *pszArgv);
 virtual void OnStop();
 virtual void OnPause();
 virtual void OnContinue();
 virtual void OnShutdown();

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"></p>
<p class="MsoNormal">Step5. Add the SampleService.h and SampleService.cpp files to provide a sample service class that derives from the service base class - CServiceBase. The sample
<span class="GramE">service logs the service start and stop</span> information to the Application log, and shows how to run the main function of the service in a thread pool worker thread.
</p>
<p class="MsoNormal"><span class="GramE">CSampleService::OnStart, which is executed when the service starts, calls
<span class="SpellE">CServiceBase</span>::<span class="SpellE">WriteEventLogEntry</span> to log the service-start information.</span> And it calls CThreadPool::QueueUserWorkItem to queue the main service function (<span class="SpellE">CSampleService</span>::<span class="SpellE">ServiceWorkerThread</span>)
 for execution in a worker thread. </p>
<p class="MsoNormal">NOTE: A service application is designed to be long running. Therefore, it usually polls or monitors something in the system. The monitoring is set up in the
<span class="SpellE">OnStart</span> method. However, OnStart does not actually do the monitoring. The OnStart method must return to the operating system after the service's operation has begun. It must not loop forever or block. To set up a simple monitoring
 mechanism, one general solution is to create a timer in OnStart. The timer would then raise events in your code periodically, at which time your service could do its monitoring. The other solution is to spawn a new
</p>
<p class="MsoNormal"><span class="GramE">thread</span> to perform the main service functions, which is demonstrated in this code sample.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
void CSampleService::OnStart(DWORD dwArgc, LPWSTR *lpszArgv)
   {
       // Log a service start message to the Application log.
       WriteEventLogEntry(L&quot;CppWindowsService in OnStart&quot;, 
           EVENTLOG_INFORMATION_TYPE);


       // Queue the main service function for execution in a worker thread.
       CThreadPool::QueueUserWorkItem(&CSampleService::ServiceWorkerThread, this);
   }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span class="GramE">CSampleService::OnStop, which is executed when the service stops, calls
<span class="SpellE">CServiceBase</span>::<span class="SpellE">WriteEventLogEntry</span> to log the service-stop information.</span> Next, it sets the member varaible m_fStopping as TRUE to indicate that the service is stopping and waits for the finish
 of the main service function that is signaled by the <span class="SpellE">m_hStoppedEvent</span> event object.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
void CSampleService::OnStop()
   {
       WriteEventLogEntry(L&quot;CppWindowsService in OnStop&quot;, 
           EVENTLOG_INFORMATION_TYPE);


       // Indicate that the service is stopping and wait for the finish of the 
       // main service function (ServiceWorkerThread).
       m_fStopping = TRUE;
       if (WaitForSingleObject(m_hStoppedEvent, INFINITE) != WAIT_OBJECT_0)
       {
           throw GetLastError();
       }
   }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">CSampleService::ServiceWorkerThread runs in a thread pool worker thread. It performs the main function of the service such as the communication with client applications through a named pipe. In order that the main function finishes gracefully
 when the service is about to stop, it should periodically check the <span class="SpellE">
m_fStopping</span> <span class="SpellE">varaible</span>. When the function detects that the service is stopping, it cleans up the work and signal the m_hStoppedEvent event object.</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
void CSampleService::ServiceWorkerThread(void)
   {
       // Periodically check if the service is stopping.
       while (!m_fStopping)
       {
           // Perform main service function here...


           ::Sleep(2000);  // Simulate some lengthy operations.
       }


       // Signal the stopped event.
       SetEvent(m_hStoppedEvent);
   }

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">Step6. Add the ServiceInstaller.h and ServiceInstaller.cpp files to declare and implement functions that install and uninstall the service:</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C&#43;&#43;</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">cplusplus</span>

<pre id="codePreview" class="cplusplus">
InstallService          Installs the service
UninstallService        Uninstalls the service 

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"></p>
<h2>More Information</h2>
<p class="MsoListParagraphCxSpFirst"></p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/ms681921(VS.85).aspx">MSDN: About Services</a>
</p>
<p class="MsoListParagraphCxSpMiddle" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/bb540476(VS.85).aspx">MSDN: The Complete Service Sample</a>
</p>
<p class="MsoListParagraphCxSpLast" style=""><span style="font-family:Symbol"><span style="">��<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://msdn.microsoft.com/en-us/library/ms810429.aspx">MSDN: Creating a Simple Win32 Service in C&#43;&#43;</a>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
