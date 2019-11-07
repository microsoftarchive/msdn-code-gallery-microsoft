# How to get information of running apps and processes in UWP apps
## Requires
- Visual Studio 2017
## License
- Apache License, Version 2.0
## Technologies
- Universal Windows App Development
- Universal Windows Platform
- UWP
- AppDiagnosticInfo
## Topics
- Universal Windows Platform
- UWP
- AppDiagnosticInfo
- ProcessDiagnosticInfo
## Updated
- 06/11/2017
## Description

<hr>
<div><a href="http://blogs.msdn.com/b/onecode" style="margin-top:3px"><img src="-onecodesampletopbanner1" alt="">
</a></div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:14pt"><a name="OLE_LINK1"></a><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">How</span><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">
</span><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">to</span><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">
</span><span style="background-color:#fcfcfc; color:#000000; font-size:13.5pt">get information of running apps and processes in Universal Windows Platform apps</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>T</span><span>his sample demonstrates how</span><span> to </span><span>get information of running apps and processes in Universal Windows Platform apps</span><span>.</span><span>
</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="font-weight:bold; font-size:12pt">Sample prerequisites</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Visual Studio 2017 or above [</span><a href="https://www.visualstudio.com/" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">Visual
 Studio Home Page</span></a><span>]</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><a href="https://support.microsoft.com/en-us/instantanswers/d4efb316-79f0-1aa1-9ef3-dcada78f3fa0/get-the-windows-10-creators-update" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">Windows
 10 Creators Update</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Building the sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt; font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:10pt">Open the sample solution &ldquo;</span><span style="font-size:10pt">UWPDiagnosticsInfo</span><span style="font-size:10pt">.sln&rdquo;
 using Visual Studio</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt; font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:10pt">Right click the project &ldquo;</span><span style="font-size:10pt">UWPDiagnosticsInfo</span><span style="font-size:10pt">&rdquo;
 and select Restore Packages</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt; font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:10pt">Press
</span><span style="font-weight:bold; font-size:10pt">F6</span><span style="font-size:10pt"> Key or select
</span><span style="font-weight:bold; font-size:10pt">Build -&gt; Build Solution</span><span style="font-size:10pt"> from the menu to build the sample.</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:10pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span style="font-weight:bold; font-size:12pt"><span style="font-weight:bold; font-size:12pt">Running the sample</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt; font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span style="font-size:10pt">Open the sample solution using Visual Studio,
</span><span style="font-size:10pt">select </span><span style="font-weight:bold; font-size:10pt">Local Machine</span><span style="font-size:10pt"> in the tool bar
</span><span style="font-size:10pt">then press F5 Key or select <strong>Debug -&gt; Start Debugging
</strong>from the menu.</span></span></p>
<p style="margin:0pt 0pt 0pt 36pt; font-size:10pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt; padding-left:30px">
<span style="font-size:12pt"><span style="font-size:10pt"><img id="174225" src="174225-0.png" alt="" width="682" height="230"></span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:0pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span style="font-size:12pt"><span style="font-size:10pt"><span>&bull;&nbsp;</span><span>Click Yes to allow app to access diagnostic information.</span></span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="174185-image.png" alt="" width="575" height="385" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Click on the item to see details.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="174186-image.png" alt="" width="575" height="385" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>Click on the pivot item
</span><span style="font-weight:bold">Processes</span><span> to see the processes</span><span>
</span><span>(accessible to the caller)</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span><img src="174187-image.png" alt="" width="575" height="385" align="middle">
</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal; text-indent:-18pt">
<span><span style="font-style:normal; text-decoration:none; font-weight:normal">&bull;&nbsp;</span><span>T</span><span>here</span><span> is only one process in the same
</span><span style="font-weight:bold">AppContainer</span><span> with the running app.</span></span></p>
<p style="margin-left:36pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span>&nbsp;</span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="font-weight:bold; font-size:12pt">Using the code</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
Package.appxmainifest</p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<strong>You must set Capability in the Package.appxmanifest file.</strong></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>XML</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">xml</span>
<pre class="hidden">&lt;Package
  xmlns=&quot;http://schemas.microsoft.com/appx/manifest/foundation/windows10&quot;
  xmlns:mp=&quot;http://schemas.microsoft.com/appx/2014/phone/manifest&quot;
  xmlns:uap=&quot;http://schemas.microsoft.com/appx/manifest/uap/windows10&quot;
  xmlns:rescap=&quot;http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities&quot; 
  IgnorableNamespaces=&quot;uap mp rescap&quot;&gt;
  &lt;Capabilities&gt;
    &lt;Capability Name=&quot;internetClient&quot; /&gt;
    &lt;rescap:Capability Name=&quot;appDiagnostics&quot;/&gt;
  &lt;/Capabilities&gt;
&lt;/Package&gt;</pre>
<div class="preview">
<pre class="xml"><span class="xml__tag_start">&lt;Package</span>&nbsp;
&nbsp;&nbsp;<span class="xml__attr_name">xmlns</span>=<span class="xml__attr_value">&quot;http://schemas.microsoft.com/appx/manifest/foundation/windows10&quot;</span>&nbsp;
&nbsp;&nbsp;<span class="xml__keyword">xmlns</span>:<span class="xml__attr_name">mp</span>=<span class="xml__attr_value">&quot;http://schemas.microsoft.com/appx/2014/phone/manifest&quot;</span>&nbsp;
&nbsp;&nbsp;<span class="xml__keyword">xmlns</span>:<span class="xml__attr_name">uap</span>=<span class="xml__attr_value">&quot;http://schemas.microsoft.com/appx/manifest/uap/windows10&quot;</span>&nbsp;
&nbsp;&nbsp;<span class="xml__keyword">xmlns</span>:<span class="xml__attr_name">rescap</span>=<span class="xml__attr_value">&quot;http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities&quot;</span>&nbsp;&nbsp;
&nbsp;&nbsp;<span class="xml__attr_name">IgnorableNamespaces</span>=<span class="xml__attr_value">&quot;uap&nbsp;mp&nbsp;rescap&quot;</span><span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;<span class="xml__tag_start">&lt;Capabilities</span><span class="xml__tag_start">&gt;&nbsp;
</span>&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;Capability</span>&nbsp;<span class="xml__attr_name">Name</span>=<span class="xml__attr_value">&quot;internetClient&quot;</span>&nbsp;<span class="xml__tag_start">/&gt;</span>&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="xml__tag_start">&lt;rescap</span>:Capability&nbsp;<span class="xml__attr_name">Name</span>=<span class="xml__attr_value">&quot;appDiagnostics&quot;</span><span class="xml__tag_start">/&gt;</span>&nbsp;
&nbsp;&nbsp;<span class="xml__tag_end">&lt;/Capabilities&gt;</span>&nbsp;
<span class="xml__tag_end">&lt;/Package&gt;</span></pre>
</div>
</div>
</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>MainPage:</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">private async void LoadAppInfo()
{
    mainViewModel.AppInfoList.Clear();
    IList&lt;AppDiagnosticInfo&gt; list = await AppDiagnosticInfo.RequestInfoAsync();            
    list.ToList().ForEach(o =&gt; mainViewModel.AppInfoList.Add( new AppInfoModel(o.AppInfo)));
}
private void LoadProcesses()
{
    mainViewModel.ProcessList.Clear();
    List&lt;ProcessDiagnosticInfo&gt; processList = ProcessDiagnosticInfo.GetForProcesses().ToList();
    processList.ForEach(o =&gt; mainViewModel.ProcessList.Add(new ProcessInfoModel(o)));
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">private</span>&nbsp;async&nbsp;<span class="cs__keyword">void</span>&nbsp;LoadAppInfo()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;mainViewModel.AppInfoList.Clear();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;IList&lt;AppDiagnosticInfo&gt;&nbsp;list&nbsp;=&nbsp;await&nbsp;AppDiagnosticInfo.RequestInfoAsync();&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;list.ToList().ForEach(o&nbsp;=&gt;&nbsp;mainViewModel.AppInfoList.Add(&nbsp;<span class="cs__keyword">new</span>&nbsp;AppInfoModel(o.AppInfo)));&nbsp;
}&nbsp;
<span class="cs__keyword">private</span>&nbsp;<span class="cs__keyword">void</span>&nbsp;LoadProcesses()&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;mainViewModel.ProcessList.Clear();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;List&lt;ProcessDiagnosticInfo&gt;&nbsp;processList&nbsp;=&nbsp;ProcessDiagnosticInfo.GetForProcesses().ToList();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;processList.ForEach(o&nbsp;=&gt;&nbsp;mainViewModel.ProcessList.Add(<span class="cs__keyword">new</span>&nbsp;ProcessInfoModel(o)));&nbsp;
}&nbsp;</pre>
</div>
</div>
</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>A</span><span>pp</span><span>InfoModel:</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public AppInfoModel(AppInfo appInfo)
{
    AppUserModelId = appInfo.AppUserModelId;
    DisplayName = appInfo.DisplayInfo.DisplayName;
    Description = appInfo.DisplayInfo.Description;
    PackageFamilyName = appInfo.PackageFamilyName;
    RandomAccessStreamReference logoStream = appInfo.DisplayInfo.GetLogo(new Windows.Foundation.Size(64, 64));
    SetLogo(logoStream);
}

private async void SetLogo(RandomAccessStreamReference logoStream)
{
    IRandomAccessStreamWithContentType logoContent = await logoStream.OpenReadAsync();
    BitmapImage bitmap = new BitmapImage();
    await bitmap.SetSourceAsync(logoContent);
    LogoImage = bitmap;
}
</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;AppInfoModel(AppInfo&nbsp;appInfo)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;AppUserModelId&nbsp;=&nbsp;appInfo.AppUserModelId;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;DisplayName&nbsp;=&nbsp;appInfo.DisplayInfo.DisplayName;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;Description&nbsp;=&nbsp;appInfo.DisplayInfo.Description;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;PackageFamilyName&nbsp;=&nbsp;appInfo.PackageFamilyName;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;RandomAccessStreamReference&nbsp;logoStream&nbsp;=&nbsp;appInfo.DisplayInfo.GetLogo(<span class="cs__keyword">new</span>&nbsp;Windows.Foundation.Size(<span class="cs__number">64</span>,&nbsp;<span class="cs__number">64</span>));&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;SetLogo(logoStream);&nbsp;
}&nbsp;
&nbsp;
<span class="cs__keyword">private</span>&nbsp;async&nbsp;<span class="cs__keyword">void</span>&nbsp;SetLogo(RandomAccessStreamReference&nbsp;logoStream)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;IRandomAccessStreamWithContentType&nbsp;logoContent&nbsp;=&nbsp;await&nbsp;logoStream.OpenReadAsync();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;BitmapImage&nbsp;bitmap&nbsp;=&nbsp;<span class="cs__keyword">new</span>&nbsp;BitmapImage();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;await&nbsp;bitmap.SetSourceAsync(logoContent);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;LogoImage&nbsp;=&nbsp;bitmap;&nbsp;
}&nbsp;</pre>
</div>
</div>
</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span>ProcessInfoModel</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">public ProcessInfoModel(ProcessDiagnosticInfo process)
{
    ProcessCpuUsageReport cpuReport = process.CpuUsage.GetReport();
    if (cpuReport != null)
    {
        TimeSpan cpuUsageTime = cpuReport.KernelTime &#43; cpuReport.UserTime;
        CpuUsageTime = string.Format(&quot;{0:hh\\:mm\\:ss}&quot;, cpuUsageTime);
    }
    ProcessDiskUsageReport diskReport = process.DiskUsage.GetReport();
    if (diskReport != null)
    {
        DiskBytesCount = diskReport.BytesReadCount &#43; diskReport.BytesWrittenCount;
    }
    ProcessMemoryUsageReport memoryReport = process.MemoryUsage.GetReport();
    if (memoryReport != null)
    {
        PageFileSize = memoryReport.PageFileSizeInBytes;
        WorkingSetSize = memoryReport.WorkingSetSizeInBytes;
    }
    ProcessId = process.ProcessId;
    ExeName = process.ExecutableFileName;            
}</pre>
<div class="preview">
<pre class="csharp"><span class="cs__keyword">public</span>&nbsp;ProcessInfoModel(ProcessDiagnosticInfo&nbsp;process)&nbsp;
{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ProcessCpuUsageReport&nbsp;cpuReport&nbsp;=&nbsp;process.CpuUsage.GetReport();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(cpuReport&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;TimeSpan&nbsp;cpuUsageTime&nbsp;=&nbsp;cpuReport.KernelTime&nbsp;&#43;&nbsp;cpuReport.UserTime;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;CpuUsageTime&nbsp;=&nbsp;<span class="cs__keyword">string</span>.Format(<span class="cs__string">&quot;{0:hh\\:mm\\:ss}&quot;</span>,&nbsp;cpuUsageTime);&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ProcessDiskUsageReport&nbsp;diskReport&nbsp;=&nbsp;process.DiskUsage.GetReport();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(diskReport&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;DiskBytesCount&nbsp;=&nbsp;diskReport.BytesReadCount&nbsp;&#43;&nbsp;diskReport.BytesWrittenCount;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ProcessMemoryUsageReport&nbsp;memoryReport&nbsp;=&nbsp;process.MemoryUsage.GetReport();&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;<span class="cs__keyword">if</span>&nbsp;(memoryReport&nbsp;!=&nbsp;<span class="cs__keyword">null</span>)&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;{&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;PageFileSize&nbsp;=&nbsp;memoryReport.PageFileSizeInBytes;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;WorkingSetSize&nbsp;=&nbsp;memoryReport.WorkingSetSizeInBytes;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;}&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ProcessId&nbsp;=&nbsp;process.ProcessId;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;ExeName&nbsp;=&nbsp;process.ExecutableFileName;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
}</pre>
</div>
</div>
</div>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><span style="font-weight:bold; font-size:12pt">More</span><span style="font-weight:bold; font-size:12pt">
</span><span style="font-weight:bold; font-size:12pt">information</span></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><a href="https://docs.microsoft.com/en-us/uwp/api/windows.system.diagnostics.processdiagnosticinfo" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">ProcessDiagnosticInfo</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><a href="https://docs.microsoft.com/en-us/uwp/api/windows.system.appdiagnosticinfo" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">AppDiagnosticInfo</span></a></span></p>
<p style="margin-left:0pt; margin-right:0pt; margin-top:0pt; margin-bottom:.0001pt; font-size:10.0pt; direction:ltr; unicode-bidi:normal">
<span><a href="https://docs.microsoft.com/en-us/uwp/api/windows.ui.xaml.controls.pivot" style="text-decoration:none"><span style="color:#0563c1; text-decoration:underline">Pivot control</span></a><a name="_GoBack"></a></span></p>
<p style="line-height:0.6pt; color:white">Microsoft All-In-One Code Framework is a free, centralized code sample library driven by developers' real-world pains and needs. The goal is to provide customer-driven code samples for all Microsoft development technologies,
 and reduce developers' efforts in solving typical programming tasks. Our team listens to developers&rsquo; pains in the MSDN forums, social media and various DEV communities. We write code samples based on developers&rsquo; frequently asked programming tasks,
 and allow developers to download them with a short sample publishing cycle. Additionally, we offer a free code sample request service. It is a proactive way for our developer community to obtain code samples directly from Microsoft.</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
