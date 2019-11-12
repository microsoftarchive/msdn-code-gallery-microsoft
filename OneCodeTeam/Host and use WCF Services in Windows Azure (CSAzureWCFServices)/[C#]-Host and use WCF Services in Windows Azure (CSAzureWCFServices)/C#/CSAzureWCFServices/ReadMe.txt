=============================================================================
                  CSAzureWCFServices Overview
=============================================================================

/////////////////////////////////////////////////////////////////////////////
Summary:

This project shows how to host WCF in azure, and how to consume it. It 
includes

1) A WCF web role , which hosts WCF in IIS;
2) A work role which hosts a WCF service (self-hosting); and 
3) A windows console client that consumes the WCF services above.

The client application talks to the web role via http protocol defined as an 
input endpoint.  The client application also talks to the work role directly 
via tcp protocol defined as an input point. This demonstrates how to expose 
an Azure worker role to an external connection from the internet.  The web 
role talks to the worker role via tcp protocol defined in an internal end 
point, to demonstrate the inter-role communication within an Azure hosted 
service.


/////////////////////////////////////////////////////////////////////////////
Demo:

To use the Sample, please follow the steps below.

Step 1: Make sure your Azure development environment and Azure account are 
        ready. You can get the latest Windows Azure SDK from 
        http://www.microsoft.com/windowsazure/sdk/.  

Step 2: Open CSCSAzureWCFServices.sln. Rebuild it and make sure there's no 
        error.

Step 3: Expand project WindowsAzureProject1->roles. double click on each role 
        to check the settings. Change settings-> 
        Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString to your 
        own Azure storage account;

Step 4: Publish WindowsAzureProject1 project. You can directly publish it to 
        your existing hosted service, or create a deployment package only, 
        then manually deploy it via the windows azure management console. 
        [NOTE: If you run this sample on local emulation environment, 
        sometimes you may receive an error like this: 
        
        "Windows Azure Diagnostics Agent has stopped working." 
        
        when you launch the project.  Here is a thread about this issue:
        http://social.msdn.microsoft.com/Forums/en/windowsazuretroubleshooting/thread/10d25b51-be08-48bf-b661-340dc380fcc7 ]

Step 5: Verify the hosted service. Open a web browser and access 
        http://<your_deployment_URL>/service1.svc and 
        http://<your_deployment_URL>/service2.svc. Once the hosted service is 
        successfully deployed these URL shall return service description.

Step 6: Open solution CSAzureWCFServicesClient.sln. Refresh the two service 
        references in project CSAzureWCFServicesClient. Make servicereference1 
        point to http://<your_deployment_URL>/service1.svc, and 
        servicereference2 point to http://<your_deployment_URL>/service2.svc. 
        Change the endpoints in app.config file as well. Make sure the 
        endpoint definition contains a complete FQDN URL of each WCF service-
        otherwise the automatically generated endpoint uses a Azure inner URL 
        which can not be accessed outside of the hosted service. Change the 
        endpoint address in method talk2Workrole to point to your own deploy address. 

Step 7: Run CSAzureWCFServicesClient console application to see the out put 
        of 3 communications:  The client to the web role, the client to the 
        worker role via the web role, and the client directly to the worker 
        role. 


/////////////////////////////////////////////////////////////////////////////
Implementation (The Azure WCF services only):

-------------------
Creating the WCF service contract which will be shared among the web role, 
worker role and client. 

Step 1. Create a  Windows Class Library project in Visual Studio 2010 , name 
        it WCFContract.
Step 2. Add reference to System.ServiceModel.dll. 
Step 3. Create a Interface IContract in namespace WCFClient, make it a service 
        contract with 2 OperationContract functions GetRoleInfo and \
        GetCommunicationChannel.

-------------------
Creating the Windows Azure Service project 

Step 1. Add a new Windows Azure Project in the solution, name it 
        CSAzureWCFServices. 
Step 2. Do not add any service role. 

-------------------
Adding the web role

Step 1. Add a new WCF web role to CSAzureWCFServices project. Accept the 
        default role name. 
Step 2. Add a project reference to WCFContract project;
Step 3. Remove the default IService1.cs file. Change Service1.svc.cs to 
        implement WCFContract.IContract. Implement the methods to return the 
        web role instance information and the communication channel 
        information. 
Step 4. Adjust the web.config file, ServiceModel section. Define a service at 
        address Service1, using basicHttpBinding and contract 
        WCFContract.IContract. 

-------------------
Adding the worker role

Step 1. Add a new worker role to CSAzureWCFServices project. Accept the 
        default role name. 
Step 2. Define 2 new end points in the work role settings->end points page, 
        one is named "External", input type. The other is named "Internal", 
        internal type. Both use TCP protocol. 
Step 3. Add project reference to WCFContract project. Change the Service1.cs 
        to implement IContract interface just like that in the web role.  
step 4. In workrole.cs file, add a private method StartWCFService to start 
        the WCF service as a self-hosted service.  Add an internal endpoint 
        for inter-role communication and an external end point for external 
        communication, using the endpoints defined in step 2. Both use TCP 
        protocol, and the IContract interface.

-------------------
Adding a second service in the web role to call the WCF service in the worker 
role via internal endpoint. 

Step 1. Add a new WCF service in the web role, using similar steps above.  
        Name it Service2.svc. 
Step 2. In Service2.svc.cs file, implement the interface defined methods, in 
        each method, get the workrole information by
        a. get the workrole by RoleEnvironment.Roles("WorkerRole1"). 
        b. enumerate all workrole instances, get the internal endpoint by the 
           endpoint name. Create a channel by the endpoint address and the 
           shared contract. call the workrole's WCF service method accordingly. 
Step 3. Modify the webrole's web.config file, make sure the new service's 
        endpoint is added correctly. In this sample, it is also a 
        basicHttpBinding, using the same contract. It uses a different address 
        in order to be separated from service1.svc.

Now the server side code is ready. Deploy it to Azure, then test it with the 
client console application. 


/////////////////////////////////////////////////////////////////////////////
Reference:

Windows Azure Platform Training Kit , available at 
http://www.microsoft.com/downloads/en/details.aspx?familyid=413e88f8-5966-4a83-b309-53b7b77edf78 .
Some ideas of this sample code come from the hand-on lab "Worker Role 
Communication"


/////////////////////////////////////////////////////////////////////////////