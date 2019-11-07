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

<h1><span style="">Enable\Disable Network Adapter By Using WMI In C# </span>(CSWMIEnableDisableNetworkAdapter)</h1>
<h2>Introduction</h2>
<p class="MsoNormal">CSWMIEnableDisableNetworkAdapter shows <span style="">you how to enable and disable a Network Adapter using WMI. The Win32_NetworkAdapter WMI class represents a network adapter of a computer running a Windows operating system. We enable
 \ disable a network adapter by using the &quot;Enable&quot; or &quot;Disable&quot; method of the class. Because the NetEnabled property is not available in the some operating system (Windows Server 2003, Windows XP, Windows 2000, and Windows NT 4.0), so this
 sample does not work in these operating system. </span></p>
<h2><span style="">Building the Sample </span></h2>
<p class="MsoNormal">To build this sample</p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Open CSWMIEnableDisableNetworkAdapter.sln file in Visual Studio 2010</p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span style=""><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span>Build the Solution<span style=""> </span></p>
<h2>Running the Sample</h2>
<p class="MsoNormal"><span style="">The follow </span>screenshot<span style=""> shows the main form of the application. The form will list all network adapters information ( ProductName, Connettion Status) in the machine. You can click the button for each
 network adapter to enable\disable it. The sample must be run as administrator. </span>
</p>
<p class="MsoNormal"><span style=""><img src="84591-image.png" alt="" width="576" height="425" align="middle">
</span></p>
<h2>Using the Code<span style=""> </span></h2>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style=""><span style="">1.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="">The constructor of NetworkAdapter class </span>
</p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
internal class NetworkAdapter
{
    #region NetworkAdapter Properties


    /// &lt;summary&gt;
    /// The DeviceId of the NetworkAdapter
    /// &lt;/summary&gt;
    public int DeviceId
    {
        get;
        private set;
    }
    
    /// &lt;summary&gt;
    /// The ProductName of the NetworkAdapter
    /// &lt;/summary&gt;
    public string Name
    {
        get;
        private set;
    }


    /// &lt;summary&gt;
    /// The NetEnabled status of the NetworkAdapter
    /// &lt;/summary&gt;
    public int NetEnabled
    {
        get;
        private set;
    }


    /// &lt;summary&gt;
    /// The Net Connection Status Value
    /// &lt;/summary&gt;
    public int NetConnectionStatus
    {
        get;
        private set;
    }


    /// &lt;summary&gt;
    /// The Net Connection Status Description
    /// &lt;/summary&gt;
    public static string[] SaNetConnectionStatus = 
    { 
        Resources.NetConnectionStatus0,
        Resources.NetConnectionStatus1,
        Resources.NetConnectionStatus2,
        Resources.NetConnectionStatus3,
        Resources.NetConnectionStatus4,
        Resources.NetConnectionStatus5,
        Resources.NetConnectionStatus6,
        Resources.NetConnectionStatus7,
        Resources.NetConnectionStatus8,
        Resources.NetConnectionStatus9,
        Resources.NetConnectionStatus10,
        Resources.NetConnectionStatus11,
        Resources.NetConnectionStatus12
    };


    /// &lt;summary&gt;
    /// Enum the NetEnabled Status
    /// &lt;/summary&gt;
    private enum EnumNetEnabledStatus
    { 
        Disabled = -1,
        Unknow,
        Enabled
    }


    /// &lt;summary&gt;
    /// Enum the Operation reuslt of Enable and Disable  Network Adapter
    /// &lt;/summary&gt;
    private enum EnumEnableDisableResult
    {
        Fail = -1,
        Unknow,
        Success
    }


    #endregion


    #region Construct NetworkAdapter


    public NetworkAdapter(int deviceId,
        string name,  
        int netEnabled, 
        int netConnectionStatus)
    {
        DeviceId = deviceId;
        Name = name;
        NetEnabled = netEnabled;
        NetConnectionStatus = netConnectionStatus;
    }
}


</pre>
<pre id="codePreview" class="csharp">
internal class NetworkAdapter
{
    #region NetworkAdapter Properties


    /// &lt;summary&gt;
    /// The DeviceId of the NetworkAdapter
    /// &lt;/summary&gt;
    public int DeviceId
    {
        get;
        private set;
    }
    
    /// &lt;summary&gt;
    /// The ProductName of the NetworkAdapter
    /// &lt;/summary&gt;
    public string Name
    {
        get;
        private set;
    }


    /// &lt;summary&gt;
    /// The NetEnabled status of the NetworkAdapter
    /// &lt;/summary&gt;
    public int NetEnabled
    {
        get;
        private set;
    }


    /// &lt;summary&gt;
    /// The Net Connection Status Value
    /// &lt;/summary&gt;
    public int NetConnectionStatus
    {
        get;
        private set;
    }


    /// &lt;summary&gt;
    /// The Net Connection Status Description
    /// &lt;/summary&gt;
    public static string[] SaNetConnectionStatus = 
    { 
        Resources.NetConnectionStatus0,
        Resources.NetConnectionStatus1,
        Resources.NetConnectionStatus2,
        Resources.NetConnectionStatus3,
        Resources.NetConnectionStatus4,
        Resources.NetConnectionStatus5,
        Resources.NetConnectionStatus6,
        Resources.NetConnectionStatus7,
        Resources.NetConnectionStatus8,
        Resources.NetConnectionStatus9,
        Resources.NetConnectionStatus10,
        Resources.NetConnectionStatus11,
        Resources.NetConnectionStatus12
    };


    /// &lt;summary&gt;
    /// Enum the NetEnabled Status
    /// &lt;/summary&gt;
    private enum EnumNetEnabledStatus
    { 
        Disabled = -1,
        Unknow,
        Enabled
    }


    /// &lt;summary&gt;
    /// Enum the Operation reuslt of Enable and Disable  Network Adapter
    /// &lt;/summary&gt;
    private enum EnumEnableDisableResult
    {
        Fail = -1,
        Unknow,
        Success
    }


    #endregion


    #region Construct NetworkAdapter


    public NetworkAdapter(int deviceId,
        string name,  
        int netEnabled, 
        int netConnectionStatus)
    {
        DeviceId = deviceId;
        Name = name;
        NetEnabled = netEnabled;
        NetConnectionStatus = netConnectionStatus;
    }
}


</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoListParagraph" style="text-indent:-.25in"><span style="font-size:10.0pt; line-height:115%; font-family:&quot;Courier New&quot;"><span style="">2.<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;
</span></span></span><span style="">Below is the core code to get all Network Adapters in the computer.</span><span style="font-size:10.0pt; line-height:115%; font-family:&quot;Courier New&quot;">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
List&lt;NetworkAdapter&gt; allNetworkAdapter = new List&lt;NetworkAdapter&gt;();


string strWQuery = &quot;SELECT DeviceID, ProductName, Description, &quot; 
    &#43; &quot;NetEnabled, NetConnectionStatus &quot;
    &#43; &quot;FROM Win32_NetworkAdapter &quot; 
    &#43; &quot;WHERE Manufacturer &lt;&gt; 'Microsoft' &quot;;
ObjectQuery oQuery = new System.Management.ObjectQuery(strWQuery);
ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oQuery);
ManagementObjectCollection oReturnCollection = oSearcher.Get();


foreach (ManagementObject mo in oReturnCollection)
{
    netEnabled = (Convert.ToBoolean(mo[&quot;NetEnabled&quot;].ToString())) ? 1 : -1;
    allNetworkAdapter.Add(new NetworkAdapter(
        Convert.ToInt32(mo[&quot;DeviceID&quot;].ToString()), 
        mo[&quot;ProductName&quot;].ToString(),
        mo[&quot;Description&quot;].ToString(), 
        netEnabled, 
        Convert.ToInt32(mo[&quot;NetConnectionStatus&quot;].ToString())));
};
return allNetworkAdapter;

</pre>
<pre id="codePreview" class="csharp">
List&lt;NetworkAdapter&gt; allNetworkAdapter = new List&lt;NetworkAdapter&gt;();


string strWQuery = &quot;SELECT DeviceID, ProductName, Description, &quot; 
    &#43; &quot;NetEnabled, NetConnectionStatus &quot;
    &#43; &quot;FROM Win32_NetworkAdapter &quot; 
    &#43; &quot;WHERE Manufacturer &lt;&gt; 'Microsoft' &quot;;
ObjectQuery oQuery = new System.Management.ObjectQuery(strWQuery);
ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oQuery);
ManagementObjectCollection oReturnCollection = oSearcher.Get();


foreach (ManagementObject mo in oReturnCollection)
{
    netEnabled = (Convert.ToBoolean(mo[&quot;NetEnabled&quot;].ToString())) ? 1 : -1;
    allNetworkAdapter.Add(new NetworkAdapter(
        Convert.ToInt32(mo[&quot;DeviceID&quot;].ToString()), 
        mo[&quot;ProductName&quot;].ToString(),
        mo[&quot;Description&quot;].ToString(), 
        netEnabled, 
        Convert.ToInt32(mo[&quot;NetConnectionStatus&quot;].ToString())));
};
return allNetworkAdapter;

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
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span>
</div>
<span class="hidden">csharp</span>
<pre class="hidden">
ManagementObject currentMObject = new ManagementObject();
string strWQuery = &quot;SELECT DeviceID,ProductName,Description,NetEnabled &quot;
    &#43; &quot;FROM Win32_NetworkAdapter &quot; 
    &#43; &quot;WHERE DeviceID = &quot; &#43; this.DeviceID.ToString();
ObjectQuery oQuery = new System.Management.ObjectQuery(strWQuery);
ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oQuery);
ManagementObjectCollection oReturnCollection = oSearcher.Get();


foreach (ManagementObject mo in oReturnCollection)
{
    currentMObject = mo;
}


//Enable the Network Adapter
currentMObject.InvokeMethod(&quot;Enable&quot;, null);


//Disable the Network Adapter
//currentMObject.InvokeMethod(&quot;Disable&quot;, null);

</pre>
<pre id="codePreview" class="csharp">
ManagementObject currentMObject = new ManagementObject();
string strWQuery = &quot;SELECT DeviceID,ProductName,Description,NetEnabled &quot;
    &#43; &quot;FROM Win32_NetworkAdapter &quot; 
    &#43; &quot;WHERE DeviceID = &quot; &#43; this.DeviceID.ToString();
ObjectQuery oQuery = new System.Management.ObjectQuery(strWQuery);
ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(oQuery);
ManagementObjectCollection oReturnCollection = oSearcher.Get();


foreach (ManagementObject mo in oReturnCollection)
{
    currentMObject = mo;
}


//Enable the Network Adapter
currentMObject.InvokeMethod(&quot;Enable&quot;, null);


//Disable the Network Adapter
//currentMObject.InvokeMethod(&quot;Disable&quot;, null);

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
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img alt="" src="-onecodelogo">
</a></div>
