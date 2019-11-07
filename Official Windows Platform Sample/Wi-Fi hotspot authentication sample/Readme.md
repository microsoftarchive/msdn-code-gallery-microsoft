# Wi-Fi hotspot authentication sample
## Requires
- Visual Studio 2013
## License
- MS-LPL
## Technologies
- Windows Runtime
- Windows 8.1
- Windows Phone 8.1
## Topics
- Networking
- Mobile Broadband
- universal app
## Updated
- 11/12/2015
## Description

<div id="mainSection">
<p>This sample demonstrates how to use the Hotspot Authentication API (<a href="http://msdn.microsoft.com/library/windows/apps/br241148"><strong>Windows.Networking.NetworkOperators</strong></a>) on both Windows and Windows Phone clients.</p>
<p class="note"><strong>Note</strong>&nbsp;&nbsp;This sample was created using one of the universal app templates available in Visual Studio. It shows how its solution is structured so it can run on both Windows&nbsp;8.1 and Windows Phone 8.1. For more info
 about how to build apps that target Windows and Windows Phone with Visual Studio, see
<a href="http://msdn.microsoft.com/library/windows/apps/dn609832">Build apps that target Windows and Windows Phone 8.1 by using Visual Studio</a>.</p>
<p>For permission to use this API, please email <a href="mailto:netopperm@microsoft.com">
Network Operators API Permissions</a>.</p>
<p>For technical help with this API, please email <a href="mailto:netophelp@microsoft.com">
Network Operators API Help</a>.</p>
<p>The sample consists of four projects: Windows Phone, Windows, Shared, and a background task (HotspotAuthenticationTask). The code in the shared and background task is common to both platforms.</p>
<p>Even though a large part of the Hotspot Authentication API is converged and the code is shared between the Windows and Windows Phone projects, we want you to be aware of these key differences:</p>
<ul>
<li>
<p>Wireless Internet Service Provider roaming (WISPr) requirement for access points:</p>
<ul>
<li>
<div class="os_icon_block">
<div class="os_icon_content_block">
<p><strong>Windows </strong>To invoke the hotspot authentication flow (background task), the access point being connected to must provide a captive portal response with WISPr support claimed in the XML blob being returned to the client.</p>
</div>
</div>
</li><li>
<div class="os_icon_block">
<div class="os_icon_content_block">
<p><strong>Windows Phone </strong>There is no requirement for captive portal or WISPr support on the access point. In this case, the background task is launched as soon as the Wi-Fi connection to the access point is completed and an IP address is acquired regardless
 of whether the access point claims WISPr support or not.</p>
</div>
</div>
</li></ul>
</li><li>
<p>Authentication using WISPr:</p>
<ul>
<li>
<div class="os_icon_block">
<div class="os_icon_content_block">
<p><strong>Windows </strong>The Hotspot Authentication API supports two ways of completing authentication:</p>
<ul>
<li>Issuing WISPr credentials by using the <a href="http://msdn.microsoft.com/library/windows/apps/dn266072">
<strong>HotspotAuthenticationContext.IssueCredentialsAsync</strong></a> method, which uses the native WISPr implementation of Windows.
</li><li>Performing a custom WISPr authentication by using the info obtained through the
<a href="http://msdn.microsoft.com/library/windows/apps/hh758381"><strong>HotspotAuthenticationContext.TryGetAuthenticationContext</strong></a> method. In this case, you must call the
<a href="http://msdn.microsoft.com/library/windows/apps/hh758379"><strong>HotspotAuthenticationContext.SkipAuthentication</strong></a> method to skip the Native WISPr authentication process after the custom authentication is complete.
</li></ul>
<p>&nbsp;</p>
</div>
</div>
</li><li>
<div class="os_icon_block">
<div class="os_icon_content_block">
<p><strong>Windows Phone </strong>There is no native WISPr support. As a result, the
<a href="http://msdn.microsoft.com/library/windows/apps/dn266072"><strong>HotspotAuthenticationContext.IssueCredentialsAsync</strong></a> method isn't supported (throws
<strong>NotImplementedException</strong>). The only way to perform WISPr authentication on the Windows Phone platform is to implement it within the app and then call the
<a href="http://msdn.microsoft.com/library/windows/apps/hh758379"><strong>HotspotAuthenticationContext.SkipAuthentication</strong></a> method.</p>
</div>
</div>
</li></ul>
</li><li>
<p>Contents of <a href="http://msdn.microsoft.com/library/windows/apps/hh758372">
<strong>HotspotAuthenticationContext</strong></a> class:</p>
<ul>
<li>
<div class="os_icon_block">
<div class="os_icon_content_block">
<p><strong>Windows </strong>All of the properties of a <a href="http://msdn.microsoft.com/library/windows/apps/hh758372">
<strong>HotspotAuthenticationContext</strong></a> object are valid.</p>
</div>
</div>
</li><li>
<div class="os_icon_block">
<div class="os_icon_content_block">
<p><strong>Windows Phone </strong>The only valid properties of a <a href="http://msdn.microsoft.com/library/windows/apps/hh758372">
<strong>HotspotAuthenticationContext</strong></a> object are <a href="http://msdn.microsoft.com/library/windows/apps/hh758382">
<strong>HotspotAuthenticationContext.WirelessNetworkId</strong></a> (SSID) and <a href="http://msdn.microsoft.com/library/windows/apps/hh758376">
<strong>HotspotAuthenticationContext.NetworkAdapter</strong></a>. This is because the remaining properties are obtained from the native WISPr implementation of Windows, which isn't supported on Windows Phone.</p>
</div>
</div>
</li></ul>
</li></ul>
<p>If your supported list of access points are all WISPr-based and your app implements WISPr authentication itself, your app's solution will result in 100% sharing of Hotspot Authentication API code between Windows Phone and Windows.</p>
<p>To obtain an evaluation copy of Windows&nbsp;8.1, go to <a href="http://go.microsoft.com/fwlink/p/?linkid=301696">
Windows&nbsp;8.1</a>.</p>
<p>To obtain an evaluation copy of Microsoft Visual Studio&nbsp;2013 Update&nbsp;2, go to
<a href="http://go.microsoft.com/fwlink/p/?linkid=301697">Microsoft Visual Studio&nbsp;2013</a>.</p>
<h2><a id="related_topics"></a>Related topics</h2>
<dl><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=301754">Mobile Broadband on the Windows Hardware Dev Center</a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh758372"><strong>HotspotAuthenticationContext</strong></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/hh758383"><strong>HotspotAuthenticationEventDetails</strong></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br224794"><strong>IBackgroundTask</strong></a>
</dt><dt><a href="http://msdn.microsoft.com/library/windows/apps/br224797"><strong>IBackgroundTaskInstance</strong></a>
</dt><dt><a href="http://go.microsoft.com/fwlink/p/?LinkID=227694">Windows 8 app samples</a>
</dt></dl>
<h2>Related technologies</h2>
<a href="http://msdn.microsoft.com/library/windows/apps/br241148"><strong>Windows.Networking.NetworkOperators</strong></a> ,
<a href="http://msdn.microsoft.com/library/windows/apps/br224847"><strong>Windows.ApplicationModel.Background</strong></a>
<h2>Operating system requirements</h2>
<table>
<tbody>
<tr>
<th>Client</th>
<td><dt>Windows&nbsp;8.1 </dt></td>
</tr>
<tr>
<th>Server</th>
<td><dt>Windows Server&nbsp;2012&nbsp;R2 </dt></td>
</tr>
<tr>
<th>Phone</th>
<td><dt>Windows Phone 8.1 </dt></td>
</tr>
</tbody>
</table>
<h2>Build the sample</h2>
<p>&nbsp;</p>
<ol>
<li>Start Visual Studio&nbsp;2013 Update&nbsp;2 and select <strong>File</strong> &gt;
<strong>Open</strong> &gt; <strong>Project/Solution</strong>. </li><li>Go to the directory to which you unzipped the sample. Then go to the subdirectory containing the sample in the language you desire - either C&#43;&#43;, C#, or JavaScript. Double-click the Visual Studio&nbsp;2013 Update&nbsp;2 Solution (.sln) file.
</li><li>Select either the Windows or Windows Phone project version of the sample and either the app or task version. Press Ctrl&#43;Shift&#43;B, or select
<strong>Build</strong> &gt; <strong>Build Solution</strong>. </li></ol>
<p>&nbsp;</p>
<h2>Run the sample</h2>
<p>The Wi-Fi hotspot authentication sample has two requirements to run:</p>
<ul>
<li>Windows must be provisioned to trigger the application when authenticating with a certain hotspot. This is achieved by provisioning a WLAN profile with a corresponding configuration. The provisioning XML must either be signed to be consumed by the provisioning
 API, or this sample must be part of a privileged mobile network operator companion application.
</li><li>The provisioning XML built into this application must be modified to match the hotspot's SSID and signed before it can be applied.
</li></ul>
<p>&nbsp;</p>
<p>The steps for completing these requirements and running the sample are described below:</p>
<ol>
<li><strong>Modify the Provisioning Metadata XML file to match your hotspot by opening ProvisioningData.xml and adjusting the following fields:</strong>
<ol>
<li>CarrierProvisioning\Global\CarrierID:
<ul>
<li>If writing a mobile broadband operator app, use the same GUID that you specified as the
<strong>Service Number</strong> in the metadata authoring wizard. </li><li>If writing a hotspot-only app, generate a new GUID with Visual Studio. On the Tools menu, click
<strong>Create GUID</strong>. Click <strong>Copy</strong> to transfer the new GUID to the Clipboard.
</li></ul>
</li><li>CarrierProvisioning\Global\SubscriberID:
<ul>
<li>If writing a mobile broadband operator app, use the International Mobile Subscriber Identity (IMSI) of the mobile broadband SIM.
</li><li>If writing a hotspot-only app, use any unique identifier appropriate to your service, such as a username or account number.
</li></ul>
</li><li>CarrierProvisioning\WLANProfiles\WLANProfile\name: Name of your service or test Access Point (AP).
</li><li>CarrierProvisioning\WLANProfiles\WLANProfile\SSIDConfig\SSID\name: Configured SSID of your test hotspot.
</li><li>CarrierProvisioning\WLANProfiles\WLANProfile\MSM\security\HotspotProfile\ExtAuth\ExtensionId: Package family name of the application. To retrieve this name:
<ul>
<li>In Visual Studio, open package.appmanifest </li><li>Click on the <strong>Packaging</strong> tab </li><li>Copy the <strong>Package Family Name</strong> </li></ul>
</li></ol>
</li><li><strong>Authenticate access to the Provisioning Agent APIs</strong>
<ul>
<li><strong>Hotspot-only operators: Sign the modified Provisioning Metadata XML file</strong>
<ol>
<li>Open a PowerShell console </li><li>Type the following command to import the Provisioning tools:
<pre class="syntax"><code>Import-Module &quot;&lt;path_to_Win8_sdk&gt;\bin\&lt;arch&gt;\ProvisioningTestHelper.psd1&quot;</code></pre>
</li><li>Create and install a test Extended Validation certificate:
<pre class="syntax"><code>Install-TestEVCert -CertName &lt;Certificate Name&gt;</code></pre>
</li><li>Use the new certificate to sign the provisioning file:
<pre class="syntax"><code>ConvertTo-SignedXml -InputFile &lt;complete path to the input XML file&gt; -OutputFile &lt;complete path to the output XML file&gt; -CertName &lt;name of the certificate used to sign the xml&gt;</code></pre>
</li><li>Replace ProvisioningData.xml with the signed file. </li></ol>
</li><li><strong>Mobile broadband operators: Modify the sample to use an unsigned Provisioning Metadata XML file</strong>
<ol>
<li>Open HotspotAuthenticationApp\Initialization.xaml.cs (or Initialization.xaml.cpp, or js\initialization.js) in Visual Studio
</li><li>Locate the call to the ProvisioningAgent constructor:
<pre class="syntax"><code>// Create ProvisiongAgent Object 
var provisioningAgent = new ProvisioningAgent();
</code></pre>
</li><li>Replace this with a call to the privileged ProvisioningAgent interface:
<pre class="syntax"><code>// Create ProvisiongAgent Object 
var accountIds = Windows.Networking.NetworkOperators.MobileBroadbandAccount.AvailableNetworkAccountIds;

if( accountIds.Count == 0 ) {
  // Throw Exception here; metadata not correctly installed
}

// For simplicity, using the first account ID.
var provisioningAgent = Windows.Networking.NetworkOperators.ProvisioningAgent.CreateFromNetworkAccountId(accountIds[0]); 
</code></pre>
</li></ol>
</li></ul>
</li><li><strong>Provide appropriate credentials for your test AP</strong>
<ul>
<li><strong>If the test AP uses fixed credentials:</strong> Modify HotspotAuthenticationTask\ConfigStore.cs to return a valid username and password for your test AP.
</li><li><strong>If the test AP uses dynamic credentials:</strong> In HotspotAuthenticationTask\BackgroundTask.cs, replace the reference to ConfigStore with your own business logic to generate/retrieve appropriate credentials for the network.
</li></ul>
</li></ol>
<p>&nbsp;</p>
<p>Run this sample after completing the preceding steps.</p>
<p>The next steps depend on whether you just want to deploy the sample or you want to both deploy and run it.</p>
<p><strong>Deploying the sample</strong></p>
<ol>
<li>Select either the Windows or Windows Phone project version of the sample and either the app or task version.
</li><li>Select <strong>Build</strong> &gt; <strong>Deploy Solution</strong>. </li></ol>
<p><strong>Deploying and running the sample</strong></p>
<ol>
<li>Right-click either the Windows or Windows Phone project version and either the app or task version of the sample in
<strong>Solution Explorer</strong> and select <strong>Set as StartUp Project</strong>.
</li><li>To debug the sample and then run it, press F5 or select <strong>Debug</strong> &gt;
<strong>Start Debugging</strong>. To run the sample without debugging, press Ctrl&#43;F5 or select<strong>Debug</strong> &gt;
<strong>Start Without Debugging</strong>. </li></ol>
</div>
