# Monitor registry changes (CSMonitorRegistryChange)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- Windows General
## Topics
- Registry Monitor
## Updated
- 05/05/2011
## Description

<p style="font-family:Courier New"></p>
<h2>Windows APPLICATION: CSMonitorRegistryChange Overview </h2>
<p style="font-family:Courier New"></p>
<h3>Summary:</h3>
<p style="font-family:Courier New">The sample demonstrates how to monitor the registry key change event using WMI event.<br>
The change event will be raised when one of the following operations happened. <br>
1 Rename or delete the key.<br>
2 Add, rename or delete a sub key under the key.<br>
3 Add, rename, edit or delete &nbsp;a value of the key.<br>
<br>
This WMI event does not return the changed value and type. It just tells that <br>
there is a change. The properties that you can get from the event are Hive, KeyPath,
<br>
SECURITY_DESCRIPTOR and TIME_CREATED.<br>
&nbsp; </p>
<h3>Demo:</h3>
<p style="font-family:Courier New">Step1. Build the sample project in Visual Studio 2010.<br>
<br>
Step2. Select a hive &quot;HKEY_LOCAL_MACHINE&quot; in the comboBox, and then type the key<br>
path &quot;SOFTWARE\\Microsoft&quot; in the textbox.<br>
Notice that you have to use double slash &quot;\\&quot; in the registry path. <br>
<br>
Step3. Click button &quot;Start Monitor&quot;.<br>
<br>
Step4. Run &quot;Regedit&quot; in run command to open Registry Editor.<br>
<br>
Step5. Navigate to HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft in Registry Editor. Right click
<br>
the key and create a new key. You'll see a new item <br>
&quot;The key HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft changed&quot; in the list box.<br>
</p>
<h3>Code Logic:</h3>
<p style="font-family:Courier New"><br>
First, initialize the combobox cmbHives that contains all supported hives. Only <br>
HKEY_LOCAL_MACHINE, HKEY_USERS and HKEY_CURRENT_CONFIG are supported by RegistryEvent<br>
or classes derived from it, such as RegistryKeyChangeEvent.<br>
<br>
Second, when a user typed key path and clicked Start Monitor button, create a new instance<br>
of RegistryWatcher. The constructor of RegistryWatcher will check whether the key exists or<br>
the user has permission to access the key, then constructs a WqlEventQuery.<br>
<br>
Third, create a handler to listen for RegistryKeyChangeEvent of RegistryWatcher.<br>
<br>
At last, when an registry-change event arrived, displays the notification in a listbox.
<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><a target="_blank" href="http://msdn.microsoft.com/en-us/library/aa393040(VS.85).aspx">http://msdn.microsoft.com/en-us/library/aa393040(VS.85).aspx</a><br>
<a target="_blank" href="http://msdn.microsoft.com/en-us/library/aa392388(VS.85).aspx">http://msdn.microsoft.com/en-us/library/aa392388(VS.85).aspx</a><br>
<a target="_blank" href="http://www.codeproject.com/KB/system/WMI_RegistryMonitor.aspx">http://www.codeproject.com/KB/system/WMI_RegistryMonitor.aspx</a><br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo">
</a></div>
