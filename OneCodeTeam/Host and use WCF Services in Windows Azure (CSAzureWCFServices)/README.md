# Host and use WCF Services in Windows Azure (CSAzureWCFServices)
## Requires
- Visual Studio 2010
## License
- Apache License, Version 2.0
## Technologies
- WCF
- Microsoft Azure
## Topics
- WCF Services
## Updated
- 01/17/2016
## Description

<p style="font-family:Courier New">&nbsp;<a href="http://www.microsoft.com/click/services/Redirect2.ashx?CR_CC=200144420" target="_blank"><img id="79969" src="http://i1.code.msdn.s-msft.com/csazurebingmaps-bab92df1/image/file/79969/1/120x90_azure_web_en_us.jpg" alt="" width="360" height="90"></a></p>
<h2><span style="font-family:courier new,courier; font-size:x-large">CSAzureWCFServices Overview</span></h2>
<h3><span style="font-family:courier new,courier"><span style="font-size:large">Summary</span>:</span></h3>
<p><span style="font-family:courier new,courier">This project shows how to host WCF in Windows Azure, and how to consume it.&nbsp;
</span><br>
<span style="font-family:courier new,courier">It includes</span></p>
<p><span style="font-family:courier new,courier">1) A WCF web role , which hosts WCF in IIS;</span><br>
<span style="font-family:courier new,courier">2) A work role which hosts a WCF service (self-hosting); and
</span><br>
<span style="font-family:courier new,courier">3) A windows console client that consumes the WCF services above.</span></p>
<p><span style="font-family:courier new,courier">The client application talks to the web role via http protocol defined as an
</span><br>
<span style="font-family:courier new,courier">input endpoint.&nbsp; The client application also talks to the work role directly
</span><br>
<span style="font-family:courier new,courier">via tcp protocol defined as an input point. This demonstrates how to expose
</span><br>
<span style="font-family:courier new,courier">an Azure worker role to an external connection from the internet.&nbsp; The web
</span><br>
<span style="font-family:courier new,courier">role talks to the worker role via tcp protocol defined in an internal end
</span><br>
<span style="font-family:courier new,courier">point, to demonstrate the inter-role communication within an Azure hosted
</span><br>
<span style="font-family:courier new,courier">service.</span></p>
<div align="right">
<p><a href="http://www.microsoft.com/click/services/Redirect2.ashx?CR_CC=200144420"><span style="color:windowtext; text-decoration:none"><span><img src="http://code.msdn.microsoft.com/site/view/file/67654/1/image.png" alt="" width="120" height="90" align="middle">
</span></span></a><br>
<a href="http://www.microsoft.com/click/services/Redirect2.ashx?CR_CC=200144420">Try Windows Azure for free for 90 Days!</a></p>
</div>
<h3><span style="font-family:'courier new',courier; font-size:medium">Video:</span></h3>
<p><span style="font-family:'courier new',courier; font-size:medium"><a href="https://channel9.msdn.com/Blogs/OneCode/How-to-create-and-host-WCF-services-in-Azure"><img id="147469" src="https://i1.code.msdn.s-msft.com/csazurewcfservices-20c7d9c5/image/file/147469/1/azureblob.jpg" alt="" width="588" height="308"></a><br>
</span></p>
<h3><span style="font-family:'courier new',courier; font-size:large">Demo:</span></h3>
<p><span style="font-family:courier new,courier">To use the Sample, please follow the steps below.</span></p>
<p><span style="font-family:courier new,courier">Step 1: Make sure your Azure development environment and Azure account are
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; ready. You can get the latest Windows Azure SDK from
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
<a href="http://www.microsoft.com/windowsazure/sdk/">http://www.microsoft.com/windowsazure/sdk/</a>.&nbsp;</span></p>
<p><span style="font-family:courier new,courier">Step 2: Open CSCSAzureWCFServices.sln. Rebuild it and make sure there's no
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; error.</span></p>
<p><span style="font-family:courier new,courier">Step 3: Expand project WindowsAzureProject1-&gt;roles. double click on each role
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; to check the settings. Change settings-&gt;
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString to your
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; own Azure storage account;</span></p>
<p><span style="font-family:courier new,courier">Step 4: Publish WindowsAzureProject1 project. You can directly publish it to
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; your existing hosted service, or create a deployment package only,
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; then manually deploy it via the windows azure management console.
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; [NOTE: If you run this sample on local emulation environment,
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; sometimes you may receive an error like this:
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; &quot;Windows Azure Diagnostics Agent has stopped working.&quot;
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; when you launch the project.&nbsp; Here is a thread about this issue:</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
<a href="http://social.msdn.microsoft.com/Forums/en/windowsazuretroubleshooting/thread/10d25b51-be08-48bf-b661-340dc380fcc7">
http://social.msdn.microsoft.com/Forums/en/windowsazuretroubleshooting/thread/10d25b51-be08-48bf-b661-340dc380fcc7</a> ]</span></p>
<p><span style="font-family:courier new,courier">Step 5: Verify the hosted service. Open a web browser and access
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
<a href="http://&lt;your_deployment_URL&gt;/service1.svc">http://&lt;your_deployment_URL&gt;/service1.svc</a> and
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
<a href="http://&lt;your_deployment_URL&gt;/service2.svc">http://&lt;your_deployment_URL&gt;/service2.svc</a>. Once the hosted service is
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; successfully deployed these URL shall return service description.</span></p>
<p><span style="font-family:courier new,courier">Step 6: Open solution CSAzureWCFServicesClient.sln. Refresh the two service
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; references in project CSAzureWCFServicesClient. Make servicereference1
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; point to
<a href="http://&lt;your_deployment_URL&gt;/service1.svc">http://&lt;your_deployment_URL&gt;/service1.svc</a>, and
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; servicereference2 point to
<a href="http://&lt;your_deployment_URL&gt;/service2.svc">http://&lt;your_deployment_URL&gt;/service2.svc</a>.
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Change the endpoints in app.config file as well. Make sure the
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; endpoint definition contains a complete FQDN URL of each WCF service-</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; otherwise the automatically generated endpoint uses a Azure inner URL
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; which can not be accessed outside of the hosted service. Change the
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; endpoint address in method talk2Workrole to point to your own deploy address.</span></p>
<p><span style="font-family:courier new,courier">Step 7: Run CSAzureWCFServicesClient console application to see the out put
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; of 3 communications:&nbsp; The client to the web role, the client to the
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; worker role via the web role, and the client directly to the worker
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; role.</span></p>
<h3><br>
<span style="font-family:courier new,courier; font-size:large">Implementation (The Azure WCF services only):</span></h3>
<p><span style="font-family:courier new,courier">-------------------</span><br>
<span style="font-family:courier new,courier">Creating the WCF service contract which will be shared among the web role,
</span><br>
<span style="font-family:courier new,courier">worker role and client.</span></p>
<p><span style="font-family:courier new,courier">Step 1. Create a&nbsp; Windows Class Library project in Visual Studio 2010 , name
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; it WCFContract.</span><br>
<span style="font-family:courier new,courier">Step 2. Add reference to System.ServiceModel.dll.
</span><br>
<span style="font-family:courier new,courier">Step 3. Create a Interface IContract in namespace WCFClient, make it a service
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; contract with 2 OperationContract functions GetRoleInfo and \</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; GetCommunicationChannel.</span></p>
<p><span style="font-family:courier new,courier">-------------------</span><br>
<span style="font-family:courier new,courier">Creating the Windows Azure Service project</span></p>
<p><span style="font-family:courier new,courier">Step 1. Add a new Windows Azure Project in the solution, name it
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; CSAzureWCFServices.
</span><br>
<span style="font-family:courier new,courier">Step 2. Do not add any service role.</span></p>
<p><span style="font-family:courier new,courier">-------------------</span><br>
<span style="font-family:courier new,courier">Adding the web role</span></p>
<p><span style="font-family:courier new,courier">Step 1. Add a new WCF web role to CSAzureWCFServices project. Accept the
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; default role name.
</span><br>
<span style="font-family:courier new,courier">Step 2. Add a project reference to WCFContract project;</span><br>
<span style="font-family:courier new,courier">Step 3. Remove the default IService1.cs file. Change Service1.svc.cs to
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; implement WCFContract.IContract. Implement the methods to return the
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; web role instance information and the communication channel
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; information.
</span><br>
<span style="font-family:courier new,courier">Step 4. Adjust the web.config file, ServiceModel section. Define a service at
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; address Service1, using basicHttpBinding and contract
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; WCFContract.IContract.</span></p>
<p><span style="font-family:courier new,courier">-------------------</span><br>
<span style="font-family:courier new,courier">Adding the worker role</span></p>
<p><span style="font-family:courier new,courier">Step 1. Add a new worker role to CSAzureWCFServices project. Accept the
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; default role name.
</span><br>
<span style="font-family:courier new,courier">Step 2. Define 2 new end points in the work role settings-&gt;end points page,
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; one is named &quot;External&quot;, input type. The other is named &quot;Internal&quot;,
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; internal type. Both use TCP protocol.
</span><br>
<span style="font-family:courier new,courier">Step 3. Add project reference to WCFContract project. Change the Service1.cs
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; to implement IContract interface just like that in the web role.&nbsp;
</span><br>
<span style="font-family:courier new,courier">step 4. In workrole.cs file, add a private method StartWCFService to start
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; the WCF service as a self-hosted service.&nbsp; Add an internal endpoint
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; for inter-role communication and an external end point for external
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; communication, using the endpoints defined in step 2. Both use TCP
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; protocol, and the IContract interface.</span></p>
<p><span style="font-family:courier new,courier">-------------------</span><br>
<span style="font-family:courier new,courier">Adding a second service in the web role to call the WCF service in the worker
</span><br>
<span style="font-family:courier new,courier">role via internal endpoint.</span></p>
<p><span style="font-family:courier new,courier">Step 1. Add a new WCF service in the web role, using similar steps above.&nbsp;
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; Name it Service2.svc.
</span><br>
<span style="font-family:courier new,courier">Step 2. In Service2.svc.cs file, implement the interface defined methods, in
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; each method, get the workrole information by</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; a. get the workrole by RoleEnvironment.Roles(&quot;WorkerRole1&quot;).
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; b. enumerate all workrole instances, get the internal endpoint by the
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; endpoint name. Create a channel by the endpoint address and the
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; shared contract. call the workrole's WCF service method accordingly.
</span><br>
<span style="font-family:courier new,courier">Step 3. Modify the webrole's web.config file, make sure the new service's
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; endpoint is added correctly. In this sample, it is also a
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; basicHttpBinding, using the same contract. It uses a different address
</span><br>
<span style="font-family:courier new,courier">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; in order to be separated from service1.svc.</span></p>
<p><span style="font-family:courier new,courier">Now the server side code is ready. Deploy it to Azure, then test it with the
</span><br>
<span style="font-family:courier new,courier">client console application.</span></p>
<h3><br>
<span style="font-family:courier new,courier; font-size:large">Reference:</span></h3>
<p><span style="font-family:courier new,courier">Windows Azure Platform Training Kit , available at
</span><br>
<span style="font-family:courier new,courier"><a href="http://www.microsoft.com/downloads/en/details.aspx?familyid=413e88f8-5966-4a83-b309-53b7b77edf78">http://www.microsoft.com/downloads/en/details.aspx?familyid=413e88f8-5966-4a83-b309-53b7b77edf78</a> .</span><br>
<span style="font-family:courier new,courier">Some ideas of this sample code come from the hand-on lab &quot;Worker Role
</span><br>
<span style="font-family:courier new,courier">Communication&quot;</span></p>
