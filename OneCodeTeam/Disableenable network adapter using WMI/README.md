# Disable/enable network adapter using WMI
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- WMI
- System Administration
- Windows Desktop App Development
## Topics
- network adapter
## Updated
- 06/13/2013
## Description

<h1><span style="">Enable\Disable Network Adapter By Using WMI In VB </span>(<span style="">VB</span>WMIEnableDisableNetworkAdapter)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">VBWMIEnableDisableNetworkAdapter shows <span style="">you how to enable and disable a Network Adapter using WMI. The Win32_NetworkAdapter WMI class represents a network adapter of a computer running a Windows operating system. We enable
 \ disable a network adapter by using the &quot;Enable&quot; or &quot;Disable&quot; method of the class. Because the NetEnabled property is not available in the some operating system (Windows Server 2003, Windows XP, Windows 2000, and Windows NT 4.0), so this
 sample does not work in these operating system. </span></p>
<h2><span style="">Building the Sample </span></h2>
<p class="MsoNormal">To build this sample</p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Open VBWMIEnableDisableNetworkAdapter.sln file in Visual Studio 2010</p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Build the Solution<span style=""> </span></p>
<h2>Running the Sample</h2>
<p class="MsoNormal"><span style="">The follow </span>screenshot<span style=""> shows the main form of the application. The form will list all network adapters information ( ProductName, Connettion Status) in the machine. You can click the button for each
 network adapter to enable\disable it. The sample must be run as administrator. </span>
</p>
<p class="MsoNormal"><span style=""><img src="84252-image.png" alt="" width="576" height="425" align="middle">
</span></p>
<h2>Using the Code<span style=""> </span></h2>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">The constructor of NetworkAdapter class </span>
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
Public Class NetworkAdapter


#Region &quot;Pribate Properties&quot;


    ''' &lt;summary&gt;
    ''' Network Adapter DeviceId
    ''' &lt;/summary&gt;
    ''' &lt;remarks&gt;&lt;/remarks&gt;
    Private _intDeviceId As Integer
    Property DeviceId As Integer
        Get
            Return _intDeviceId
        End Get
        Set(ByVal value As Integer)
            _intDeviceId = value
        End Set


    End Property


    ''' &lt;summary&gt;
    ''' Network Adapter ProductName
    ''' &lt;/summary&gt;
    ''' &lt;remarks&gt;&lt;/remarks&gt;
    Private _strName As String
    Property Name() As String
        Get
            Return _strName
        End Get
        Set(ByVal value As String)
            _strName = value
        End Set
    End Property


    ''' &lt;summary&gt;
    ''' Network Adapter Connection Status
    ''' &lt;/summary&gt;
    ''' &lt;remarks&gt;&lt;/remarks&gt;
    Private _intNetConnectionStatus As Integer
    Property NetConnectionStatus As Integer
        Get
            Return _intNetConnectionStatus
        End Get
        Set(ByVal value As Integer)
            _intNetConnectionStatus = value
        End Set
    End Property


    ''' &lt;summary&gt;
    ''' Network Adapter NetEnabled
    ''' &lt;/summary&gt;
    ''' &lt;remarks&gt;&lt;/remarks&gt;
    Private _intNetEnabled As Integer
    Property NetEnabled As Integer
        Get
            Return _intNetEnabled
        End Get
        Set(ByVal value As Integer)
            _intNetEnabled = value
        End Set
    End Property


    ''' &lt;summary&gt;
    ''' Network Adapter Connection Status Descriptions
    ''' &lt;/summary&gt;
    ''' &lt;remarks&gt;&lt;/remarks&gt;
    Public Shared SaNetConnectionStatus As String() = New String() _
    {
        NetConnectionStatus0,
        NetConnectionStatus1,
        NetConnectionStatus2,
        NetConnectionStatus3,
        NetConnectionStatus4,
        NetConnectionStatus5,
        NetConnectionStatus6,
        NetConnectionStatus7,
        NetConnectionStatus8,
        NetConnectionStatus9,
        NetConnectionStatus10,
        NetConnectionStatus11,
        NetConnectionStatus12
    }


    ''' &lt;summary&gt;
    ''' Enum The Result Of Enable Or Disable Network Adapter
    ''' &lt;/summary&gt;
    ''' &lt;remarks&gt;&lt;/remarks&gt;
    Private Enum EnumEnableDisableResult
        Fail = -1
        Success = 1
        Unknow = 0
    End Enum


    ''' &lt;summary&gt;
    ''' Enum The Network Adapter NetEnabled Status Values
    ''' &lt;/summary&gt;
    ''' &lt;remarks&gt;&lt;/remarks&gt;
    Private Enum EnumNetEnabledStatus
        Disabled = -1
        Enabled = 1
        Unknow = 0
    End Enum


#End Region


#Region &quot;Construct NetworkAdapter&quot;


    Public Sub New(ByVal deviceId As Integer,
                   ByVal name As String,
                   ByVal netEnabled As Integer,
                   ByVal netConnectionStatus As Integer)
        Me.DeviceId = deviceId
        Me.Name = name
        Me.NetEnabled = netEnabled
        Me.NetConnectionStatus = netConnectionStatus
    End Sub


#End Region


End Class

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span style="font-size:10.0pt; line-height:115%; font-family:&quot;Courier New&quot;"><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;
</span></span></span><span style="">Below is the core code to get all Network Adapters in the computer.</span><span style="font-size:10.0pt; line-height:115%; font-family:&quot;Courier New&quot;">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
Dim allNetworkAdapter As New List(Of NetworkAdapter)


Dim oQuery As New ObjectQuery(
    &quot;SELECT DeviceID,ProductName,Description,NetEnabled,NetConnectionStatus &quot; _
    & &quot;FROM Win32_NetworkAdapter WHERE Manufacturer &lt;&gt; 'Microsoft' &quot;)
Dim oSearcher As New ManagementObjectSearcher(oQuery)
Dim oReturnCollection As ManagementObjectCollection = oSearcher.Get


Dim mo As ManagementObject
For Each mo In oReturnCollection
netEnabled = IIf(Convert.ToBoolean(mo.Item(&quot;NetEnabled&quot;).ToString), 1, -1)
allNetworkAdapter.Add(
    New NetworkAdapter(
        Convert.ToInt32(mo.Item(&quot;DeviceID&quot;).ToString), 
        mo.Item(&quot;ProductName&quot;).ToString, 
        netEnabled, 
        Convert.ToInt32(mo.Item(&quot;NetConnectionStatus&quot;).ToString)))
Next


oReturnCollection.Dispose
oSearcher.Dispose


Return allNetworkAdapter

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraphCxSpFirst"><span style=""></span></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span style=""><span style="">3.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">Below is the core code to Enable or Disable a Network Adapter.
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>VB</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">vb</span>

<pre id="codePreview" class="vb">
Dim currentMObject As ManagementObject
Dim oQuery As New ObjectQuery(
    (&quot;SELECT DeviceID,ProductName,Description,NetEnabled,NetConnectionStatus &quot; _
        & &quot;FROM Win32_NetworkAdapter WHERE DeviceID = &quot; & Me.DeviceID.ToString))
Dim oSearcher As New ManagementObjectSearcher(oQuery)
Dim oReturnCollection As ManagementObjectCollection = oSearcher.Get


Dim mo As ManagementObject
For Each mo In oReturnCollection
    currentMObject = mo
Next


'Enable The Network Adapter
currentMObject.InvokeMethod(&quot;Enable&quot;, Nothing)


'Disable The Network Adapter
'currentMObject.InvokeMethod(&quot;Disable&quot;, Nothing)

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal"><span style=""></span></p>
<h2><span style="">More Information </span></h2>
<p class="MsoNormal"><span style="">The Win32_NetworkAdapter class is deprecated and use the MSFT_NetAdapter class instead. WMI class
</span>represents a network adapter: <a href="http://msdn.microsoft.com/en-us/library/windows/desktop/aa394216(v=vs.85).aspx">
http://msdn.microsoft.com/en-us/library/windows/desktop/aa394216(v=vs.85).aspx</a><span style="">
</span></p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="http://bit.ly/onecodelogo">
</a></div>
