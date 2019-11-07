# Change AppPool identity programmatically (CSAzureChangeAppPoolIdentity)
## Requires
- Visual Studio 2010
## License
- MS-LPL
## Technologies
- Microsoft Azure
## Topics
- ApplicationPool
## Updated
- 04/16/2013
## Description

<p style="font-family:Courier New">&nbsp;<a href="http://www.microsoft.com/click/services/Redirect2.ashx?CR_CC=200144420" target="_blank"><img id="79969" src="79969-120x90_azure_web_en_us.jpg" alt="" width="360" height="90"></a></p>
<h1>Change <span class="SpellE">AppPool</span> identity programmatically (<span class="SpellE">CSAzureChangeAppPoolIdentity</span>)</h1>
<h2>Introduction</h2>
<p class="MsoNormal"><span style="font-size:8.0pt; line-height:115%; font-family:&quot;Arial&quot;,&quot;sans-serif&quot;; color:#384b38">​</span>Most of customers test their applications to connect to cloud entities like storage, SQL Azure,
<span class="SpellE">AppFabric</span> services via compute emulator environment. If the customer's machine is behind proxy that does not allow traffic from non-authenticated users, their connections fail. One of the workaround is to change the application
 identity. This cannot be done manually for Azure scenario since the app pool is created by Azure when it is actually running the service. Hence, I have written sample customers can use to change the
<span class="SpellE">AppPool</span> identity programmatically.</p>
<p class="MsoNormal" style="text-align:right"><a href="http://www.microsoft.com/click/services/Redirect2.ashx?CR_CC=200144420"><span style="color:windowtext; text-decoration:none"><span><img src="67678-image.png" alt="" width="120" height="90" align="middle">
</span></span></a><br>
<a href="http://www.microsoft.com/click/services/Redirect2.ashx?CR_CC=200144420">Try Windows Azure for free for 90 Days!</a></p>
<h2>Building the Sample</h2>
<p class="MsoNormal">This sample needs to be configured with <span class="SpellE">
sitename</span>, domain user/password, before running it.</p>
<p class="MsoNormal"><span>Under <span class="SpellE"><span class="GramE">OnStart</span></span><span class="GramE">(</span>) Method, you will find three variables as mentioned below. These three variables needs to be configured by user before running
 the sample.</span></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:green"><span>&nbsp; </span>
</span></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas">&nbsp;</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">// Name of the site. Default name Azure gives to website is &quot;Web&quot;. If this is changed, 
// you would need to assign the name of the site to siteName variable. This can be 
// obtained from ServiceDefinition.def file.
var siteName = &quot;Web&quot;;


// Please change the domain\user to domain account that you would like to configure 
// for AppPool to run under
var userName = @&quot;Domain\user&quot;;  


// Password of the above specified domain user
var password = &quot;********&quot;; //***This must be changed 

</pre>
<pre id="codePreview" class="csharp">// Name of the site. Default name Azure gives to website is &quot;Web&quot;. If this is changed, 
// you would need to assign the name of the site to siteName variable. This can be 
// obtained from ServiceDefinition.def file.
var siteName = &quot;Web&quot;;


// Please change the domain\user to domain account that you would like to configure 
// for AppPool to run under
var userName = @&quot;Domain\user&quot;;  


// Password of the above specified domain user
var password = &quot;********&quot;; //***This must be changed 

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas">&nbsp;</span></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="color:black">For non &ndash; Azure scenarios, one additional step is needed. Under
<span class="SpellE"><span class="GramE">OnStart</span></span><span class="GramE">(</span>) method , locate below line of code.
</span></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas">&nbsp;</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">//Get the name of the appPool that is created by Azure
                appPoolName = serverManager.Sites[RoleEnvironment.CurrentRoleInstance.Id &#43; &quot;_&quot; &#43; siteName].Applications.First().ApplicationPoolName;

</pre>
<pre id="codePreview" class="csharp">//Get the name of the appPool that is created by Azure
                appPoolName = serverManager.Sites[RoleEnvironment.CurrentRoleInstance.Id &#43; &quot;_&quot; &#43; siteName].Applications.First().ApplicationPoolName;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="color:black">&nbsp;</span></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="color:black">And change the above line to </span></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas">&nbsp;</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">appPoolName = serverManager.Sites[siteName].Applications.First().ApplicationPoolName;

</pre>
<pre id="codePreview" class="csharp">appPoolName = serverManager.Sites[siteName].Applications.First().ApplicationPoolName;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="color:black">&nbsp;</span></p>
<p class="MsoNormal">Configure the variables as mentioned in the &quot;Building the sample&quot; section and then run the sample by clicking F5 in VS or build the sample and run the exe. Once you confirm that the sample is working, take the code from
<span class="SpellE"><span class="GramE">OnStart</span></span><span class="GramE">(</span>) method and incorporate with actual application.</p>
<p class="MsoNormal">Add references to <span class="SpellE">Microsoft.Web.Administration</span> (location: &lt;<span class="SpellE">systemdrive</span>&gt;\system32\<span class="SpellE">inetsrv</span>),
<span class="SpellE">System.DirectoryServices</span> (Location: .Net framework installation directory) assemblies and add below using statements to your project.<span style="font-size:13.0pt; line-height:115%; font-family:&quot;Cambria&quot;,&quot;serif&quot;">
</span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">using Microsoft.Web.Administration;
using System.DirectoryServices;

</pre>
<pre id="codePreview" class="csharp">using Microsoft.Web.Administration;
using System.DirectoryServices;

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p class="MsoNormal">&nbsp;</p>
<p class="MsoNormal">Code that gets <span class="SpellE">AppPool</span> using given parameters and changes its identity to configured user.</p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas"><span>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></p>
<div class="scriptcode">
<div class="pluginEditHolder" pluginCommand="mceScriptCode">
<div class="title"><span>C#</span></div>
<div class="pluginLinkHolder"><span class="pluginEditHolderLink">Edit</span>|<span class="pluginRemoveHolderLink">Remove</span></div>
<span class="hidden">csharp</span>
<pre class="hidden">// Name of the site. Default name Azure gives to website is &quot;Web&quot;. If this is changed, 
// you would need to assign the name of the site to siteName variable. This can be 
// obtained from ServiceDefinition.def file.
var siteName = &quot;Web&quot;;


// Please change the domain\user to domain account that you would like to configure 
// for AppPool to run under
var userName = @&quot;Domain\user&quot;;  


// Password of the above specified domain user
var password = &quot;********&quot;; //***This must be changed 


// This variable is used to iterate through list of Application pools
var metabasePath = &quot;IIS://localhost/W3SVC/AppPools&quot;;


// This variable is to get the name of AppPool that is created by Azure for current Azure service
var appPoolName = &quot;&quot;;




using (ServerManager serverManager = new ServerManager())
{
    //Get the name of the appPool that is created by Azure
    appPoolName = serverManager.Sites[RoleEnvironment.CurrentRoleInstance.Id &#43; &quot;_&quot; &#43; siteName].Applications.First().ApplicationPoolName;
}


// Get list of appPools at specified metabasePath location
using (DirectoryEntry appPools = new DirectoryEntry(metabasePath))
{
    // From the list of appPools, Search and get the appPool that is created by Azure 
    using (DirectoryEntry azureAppPool = appPools.Children.Find(appPoolName, &quot;IIsApplicationPool&quot;))
    {
        if (azureAppPool != null)
        {


            // Set the AppPoolIdentityType to 3. This is equalient to MD_APPPOOL_IDENTITY_TYPE_SPECIFICUSER -  
            // The application pool runs as a specified user account.
            // Refer to:
            // http://www.microsoft.com/technet/prodtechnol/WindowsServer2003/Library/IIS/e3a60d16-1f4d-44a4-9866-5aded450956f.mspx?mfr=true, 
            // http://learn.iis.net/page.aspx/624/application-pool-identities/ 
            // for more info on AppPoolIdentityType
            azureAppPool.InvokeSet(&quot;AppPoolIdentityType&quot;, new Object[] { 3 });
            
            // Configure username for the AppPool with above specified username                      
            azureAppPool.InvokeSet(&quot;WAMUserName&quot;, new Object[] { userName });
            
            // Configure password for the AppPool with above specified password                      
            azureAppPool.InvokeSet(&quot;WAMUserPass&quot;, new Object[] { password });
            
            // Write above settings to IIS metabase
            azureAppPool.Invoke(&quot;SetInfo&quot;, null);
            
            // Commit the above configuration changes that are written to metabase
            azureAppPool.CommitChanges();
        }


    }
    
    return base.OnStart();
}

</pre>
<pre id="codePreview" class="csharp">// Name of the site. Default name Azure gives to website is &quot;Web&quot;. If this is changed, 
// you would need to assign the name of the site to siteName variable. This can be 
// obtained from ServiceDefinition.def file.
var siteName = &quot;Web&quot;;


// Please change the domain\user to domain account that you would like to configure 
// for AppPool to run under
var userName = @&quot;Domain\user&quot;;  


// Password of the above specified domain user
var password = &quot;********&quot;; //***This must be changed 


// This variable is used to iterate through list of Application pools
var metabasePath = &quot;IIS://localhost/W3SVC/AppPools&quot;;


// This variable is to get the name of AppPool that is created by Azure for current Azure service
var appPoolName = &quot;&quot;;




using (ServerManager serverManager = new ServerManager())
{
    //Get the name of the appPool that is created by Azure
    appPoolName = serverManager.Sites[RoleEnvironment.CurrentRoleInstance.Id &#43; &quot;_&quot; &#43; siteName].Applications.First().ApplicationPoolName;
}


// Get list of appPools at specified metabasePath location
using (DirectoryEntry appPools = new DirectoryEntry(metabasePath))
{
    // From the list of appPools, Search and get the appPool that is created by Azure 
    using (DirectoryEntry azureAppPool = appPools.Children.Find(appPoolName, &quot;IIsApplicationPool&quot;))
    {
        if (azureAppPool != null)
        {


            // Set the AppPoolIdentityType to 3. This is equalient to MD_APPPOOL_IDENTITY_TYPE_SPECIFICUSER -  
            // The application pool runs as a specified user account.
            // Refer to:
            // http://www.microsoft.com/technet/prodtechnol/WindowsServer2003/Library/IIS/e3a60d16-1f4d-44a4-9866-5aded450956f.mspx?mfr=true, 
            // http://learn.iis.net/page.aspx/624/application-pool-identities/ 
            // for more info on AppPoolIdentityType
            azureAppPool.InvokeSet(&quot;AppPoolIdentityType&quot;, new Object[] { 3 });
            
            // Configure username for the AppPool with above specified username                      
            azureAppPool.InvokeSet(&quot;WAMUserName&quot;, new Object[] { userName });
            
            // Configure password for the AppPool with above specified password                      
            azureAppPool.InvokeSet(&quot;WAMUserPass&quot;, new Object[] { password });
            
            // Write above settings to IIS metabase
            azureAppPool.Invoke(&quot;SetInfo&quot;, null);
            
            // Commit the above configuration changes that are written to metabase
            azureAppPool.CommitChanges();
        }


    }
    
    return base.OnStart();
}

</pre>
</div>
</div>
<div class="endscriptcode">&nbsp;</div>
<p><a name="OLE_LINK1"><span>&nbsp;</span></a></p>
<p class="MsoNormal">&nbsp;</p>
<p class="MsoNormal">For more information on <span class="SpellE">AppPoolIdentityTypes</span> refer to</p>
<p class="MsoListParagraphCxSpFirst" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><a href="http://www.microsoft.com/technet/prodtechnol/WindowsServer2003/Library/IIS/e3a60d16-1f4d-44a4-9866-5aded450956f.mspx?mfr=true"><span class="SpellE">AppPoolIdentityType</span>
<span class="SpellE">Metabase</span> Property (IIS 6.0)</a></p>
<p class="MsoListParagraphCxSpLast" style="text-indent:-.25in"><span style="font-family:Symbol"><span>&bull;<span style="font:7.0pt &quot;Times New Roman&quot;">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span></span></span><span style="color:green"><a href="http://learn.iis.net/page.aspx/624/application-pool-identities">Application Pool Identities<span>&nbsp;
</span></a><span>&nbsp;</span></span></p>
<p class="MsoNormal" style="margin-bottom:.0001pt; line-height:normal; text-autospace:none">
<span style="font-size:9.5pt; font-family:Consolas; color:green">&nbsp;</span></p>
<p class="MsoNormal">&nbsp;</p>
<hr>
<div><a href="http://go.microsoft.com/?linkid=9759640" style="margin-top:3px"><img src="-onecodelogo" alt="">
</a></div>
