================================================================================
       Windows APPLICATION: CSMonitorRegistryChange Overview                        
===============================================================================
/////////////////////////////////////////////////////////////////////////////
Summary:
The sample demonstrates how to monitor the registry key change event using WMI event.
The change event will be raised when one of the following operations happened. 
1 Rename or delete the key.
2 Add, rename or delete a sub key under the key.
3 Add, rename, edit or delete  a value of the key.

This WMI event does not return the changed value and type. It just tells that 
there is a change. The properties that you can get from the event are Hive, KeyPath, 
SECURITY_DESCRIPTOR and TIME_CREATED.
   
////////////////////////////////////////////////////////////////////////////////
Demo:
Step1. Build the sample project in Visual Studio 2010.

Step2. Select a hive "HKEY_LOCAL_MACHINE" in the comboBox, and then type the key
path "SOFTWARE\\Microsoft" in the textbox.
Notice that you have to use double slash "\\" in the registry path. 

Step3. Click button "Start Monitor".

Step4. Run "Regedit" in run command to open Registry Editor.

Step5. Navigate to HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft in Registry Editor. Right click 
the key and create a new key. You'll see a new item 
"The key HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft changed" in the list box.

/////////////////////////////////////////////////////////////////////////////
Code Logic:

First, initialize the combobox cmbHives that contains all supported hives. Only 
HKEY_LOCAL_MACHINE, HKEY_USERS and HKEY_CURRENT_CONFIG are supported by RegistryEvent
or classes derived from it, such as RegistryKeyChangeEvent.
 
Second, when a user typed key path and clicked Start Monitor button, create a new instance
of RegistryWatcher. The constructor of RegistryWatcher will check whether the key exists or
the user has permission to access the key, then constructs a WqlEventQuery.

Third, create a handler to listen for RegistryKeyChangeEvent of RegistryWatcher.

At last, when an registry-change event arrived, displays the notification in a listbox. 


/////////////////////////////////////////////////////////////////////////////
References:
http://msdn.microsoft.com/en-us/library/aa393040(VS.85).aspx
http://msdn.microsoft.com/en-us/library/aa392388(VS.85).aspx
http://www.codeproject.com/KB/system/WMI_RegistryMonitor.aspx
/////////////////////////////////////////////////////////////////////////////
