=============================================================================
  CONSOLE APPLICATION : CSWindowsServiceRecoveryProperty Project Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

CSWindowsServiceRecoveryProperty example demonstrates how to use 
ChangeServiceConfig2 to configure the service "Recovery" properties in C#. 
This example operates all the options you can see on the service "Recovery" 
tab, including setting the "Enable actions for stops with errors" option in 
Windows Vista and later operating systems. This example also include how to 
grant the shut down privilege to the process, so that we can configure a 
special option in the "Recovery" tab - "Restart Computer Options...".


/////////////////////////////////////////////////////////////////////////////
Prerequisites:

1.  The code sample must run on Windows Vista and later operating systems. 
2.  The code sample need to run as administrator.
3.  You need to install the CSWindowsService sample service before you run 
    this code sample. 


/////////////////////////////////////////////////////////////////////////////
Demo:

1.  Install CSWindowsService service on your system.
2.  Build this project and run it as administrator.
3.  Use services.msc command to view the services on your system and find the 
    CSWindowsService service, and then you can double click on that service 
    to open the property window.
4.  In the Recovery tab, you can find the recovery properties had been 
    configured by the sample application.


/////////////////////////////////////////////////////////////////////////////
Code Logic:

1.  Open the service control manager with a high permission.
2.  Open the service with the service name and a high permission.
3.	Create three actions for the service control manager perform at these 
    three situations "First failure", "Second failure" and "Subsequent 
    failures".
4.  Grant shut down privilege to the process, so that we can configure the 
    "Restart Computer Options...".
    a.  Retrieve a pseudo handle for the current process.
    b.  Open the access token associated with the process.
    c.  Retrieve the locally unique identifier (LUID) used on a specified 
        system to locally represent the specified privilege name.
    d.  Enable privileges in the specified access token.
5.  Construct a service failure actions struct with the above actions, the 
    time after which to reset the failure count to zero if there are no 
    failures, in seconds, reboot message, the program with or without the 
    command line parameters will execute with we set the failure action to 
    Run_Command after the service failed and the fail count.
6.  Enable actions for stops with errors.
7.  Call the ChangeServiceConfig2 method to set the service recovery 
    properties.


/////////////////////////////////////////////////////////////////////////////
References:

CSWindowsService
http://1code.codeplex.com/SourceControl/changeset/view/55574#628216

Service Security and Access Rights
http://msdn.microsoft.com/en-us/library/ms685981(VS.85).aspx

Privilege Constants
http://msdn.microsoft.com/en-us/library/bb530716(v=VS.85).aspx

System Error Codes (0-499)
http://msdn.microsoft.com/en-us/library/ms681382(VS.85).aspx

SC_ACTION_TYPE
http://msdn.microsoft.com/en-us/library/bb401750.aspx

SC_ACTION Structure
http://msdn.microsoft.com/en-us/library/ms685126(VS.85).aspx

SERVICE_FAILURE_ACTIONS Structure
http://msdn.microsoft.com/en-us/library/ms685939(VS.85).aspx

OpenSCManager Function
http://msdn.microsoft.com/en-us/library/ms684323(VS.85).aspx

OpenService Function
http://msdn.microsoft.com/en-us/library/ms684330(VS.85).aspx

ChangeServiceConfig2 Function
http://msdn.microsoft.com/en-us/library/ms681988(VS.85).aspx

DllImportAttribute Class
http://msdn.microsoft.com/en-us/library/system.runtime.interopservices.dllimportattribute.aspx

TOKEN_PRIVILEGES Structure
http://msdn.microsoft.com/en-us/library/aa379630(VS.85).aspx

GetCurrentProcess Function
http://msdn.microsoft.com/en-us/library/ms683179(VS.85).aspx

OpenProcessToken Function
http://msdn.microsoft.com/en-us/library/aa379295(VS.85).aspx

LookupPrivilegeValue Function
http://msdn.microsoft.com/en-us/library/aa379180(VS.85).aspx

AdjustTokenPrivileges Function
http://msdn.microsoft.com/en-us/library/aa375202(VS.85).aspx

Set up Recovery Actions to Take Place When a Service Fails
http://technet.microsoft.com/en-us/library/cc753662.aspx


/////////////////////////////////////////////////////////////////////////////