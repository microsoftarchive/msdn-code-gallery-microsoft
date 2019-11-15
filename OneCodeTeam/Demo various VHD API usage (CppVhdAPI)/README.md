# Demo various VHD API usage (CppVhdAPI)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- WDK
- Windows Driver
## Topics
- VHD
## Updated
- 09/03/2012
## Description

<p style="font-family:Courier New">&nbsp;</p>
<h2>CONSOLE APPLICATION : CppVhdAPI Project Overview</h2>
<p style="font-family:Courier New">&nbsp;</p>
<h3>Summary:</h3>
<p style="font-family:Courier New"><br>
The code sample demonstrates various VHD API usage, such as VHD creation, <br>
attaching, detaching and getting and setting disk information.<br>
<br>
</p>
<h3>Prerequisites:</h3>
<p style="font-family:Courier New"><br>
The code sample must be run on Windows 7 / Windows Server 2008 R2 or newer. &nbsp;<br>
The minimum supported client of the VHD APIs is Windows 7; and the minimum <br>
supported server of the VHD APIs is Windows Server 2008 R2.<br>
<br>
</p>
<h3>Demo:</h3>
<p style="font-family:Courier New"><br>
&nbsp; &nbsp;CppVhdAPI.exe -[cxaomdgpe] -f &lt;vhdfile&gt; -s &lt;size&gt;<br>
&nbsp; &nbsp;-c CreateVirtualDisk............input: -f &lt;vhd file name&gt;, -s &lt;size in MB&gt;<br>
&nbsp; &nbsp;-a AttachVirtualDisk............input: -f &lt;vhd file name&gt;<br>
&nbsp; &nbsp;-d DetachVirtualDisk............input: -f &lt;vhd file name&gt;<br>
&nbsp; &nbsp;-g GetVirtualDiskInformation....input: -f &lt;vhd file name&gt;<br>
&nbsp; &nbsp;-p GetVirtualDiskPhysicalPath...input: -f &lt;vhd file name&gt; -- note: must be attached<br>
&nbsp; &nbsp;-e SetVirtualDiskInformation....input: -f &lt;vhd file name&gt;, -u &lt;new GUID&gt;<br>
<br>
&nbsp; &nbsp;Examples:<br>
&nbsp; &nbsp; &nbsp;Create a 3.6 Gb VHD named 'mytest.vhd'<br>
&nbsp; &nbsp;CppVhdAPI.exe -c -f c:\testdir\mytest.vhd -s 3600<br>
<br>
&nbsp; &nbsp; &nbsp;Attach a VHD named 'mytest.vhd'<br>
&nbsp; &nbsp;CppVhdAPI.exe -a -f c:\testdir\mytest.vhd<br>
&nbsp; &nbsp;After the virtual disk is attached, you can find the disk in Disk <br>
&nbsp; &nbsp;Management MMC (Microsoft Management Console). &nbsp;You can initialize the
<br>
&nbsp; &nbsp;disk and partition it.<br>
<br>
&nbsp; &nbsp; &nbsp;Set VHD GUID 'mytest.vhd'<br>
&nbsp; &nbsp;CppVhdAPI.exe -e -f c:\testdir\mytest.vhd -u {12345678-1234-5678-1234-000000000000}<br>
<br>
&nbsp; &nbsp; &nbsp;Detach a VHD named 'mytest.vhd'<br>
&nbsp; &nbsp;CppVhdAPI.exe -d -f c:\testdir\mytest.vhd<br>
&nbsp; &nbsp;After the virtual disk is detached, the disk will disappear in Disk <br>
&nbsp; &nbsp;Management MMC (Microsoft Management Console).<br>
<br>
</p>
<h3>Implementation:</h3>
<p style="font-family:Courier New"><br>
The main logic of this code is:<br>
<br>
1. parse and validate input parameters<br>
2. execute command and parameters resulting in the action to the VHD. &nbsp;In <br>
this code sample, the usage of the following APIs are demoed.<br>
<br>
&nbsp; &nbsp;CreateVirtualDisk<br>
&nbsp; &nbsp; &nbsp; &nbsp;- Creates a virtual hard disk (VHD) image file<br>
<br>
&nbsp; &nbsp;OpenVirtualDisk<br>
&nbsp; &nbsp; &nbsp; &nbsp;- Opens a virtual hard disk (VHD) for use<br>
<br>
&nbsp; &nbsp;AttachVirtualDisk<br>
&nbsp; &nbsp; &nbsp; &nbsp;- Attaches a virtual hard disk (VHD) by locating an appropriate VHD
<br>
&nbsp; &nbsp; &nbsp; &nbsp;provider to accomplish the attachment.<br>
<br>
&nbsp; &nbsp;DetachVirtualDisk<br>
&nbsp; &nbsp; &nbsp; &nbsp;- Detaches a virtual hard disk (VHD) by locating an appropriate VHD
<br>
&nbsp; &nbsp; &nbsp; &nbsp;provider to accomplish the operation.<br>
<br>
&nbsp; &nbsp;GetVirtualDiskInformation <br>
&nbsp; &nbsp; &nbsp; &nbsp;- Retrieves information about a virtual hard disk (VHD).<br>
<br>
&nbsp; &nbsp;GetVirtualDiskPhysicalPath<br>
&nbsp; &nbsp; &nbsp; &nbsp;- Retrieves the path to the physical device object that contains a
<br>
&nbsp; &nbsp; &nbsp; &nbsp;virtual hard disk (VHD).<br>
<br>
&nbsp; &nbsp;SetVirtualDiskInformation<br>
&nbsp; &nbsp; &nbsp; &nbsp;- Sets information about a virtual hard disk (VHD).<br>
<br>
</p>
<h3>References:</h3>
<p style="font-family:Courier New"><br>
The location for complete information on VHDs is:<br>
<a href="http://msdn.microsoft.com/en-us/library/dd323684(v=VS.85).aspx" target="_blank">http://msdn.microsoft.com/en-us/library/dd323684(v=VS.85).aspx</a><br>
<br>
The Virtual Disk API In Windows 7<br>
<a href="http://msdn.microsoft.com/en-us/magazine/dd569754.aspx" target="_blank">http://msdn.microsoft.com/en-us/magazine/dd569754.aspx</a><br>
<br>
<br>
</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="http://bit.ly/onecodelogo" alt="">
</a></div>
