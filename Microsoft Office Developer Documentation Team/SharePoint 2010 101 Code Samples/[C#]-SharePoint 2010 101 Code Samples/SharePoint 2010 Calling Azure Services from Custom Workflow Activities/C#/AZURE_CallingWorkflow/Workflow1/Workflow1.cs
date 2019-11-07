using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;

namespace AZURE_CallingWorkflow.Workflow1
{
    public sealed partial class Workflow1 : SequentialWorkflowActivity
    {
        public Workflow1()
        {
            InitializeComponent();
        }

        public Guid workflowId = default(System.Guid);
        public SPWorkflowActivationProperties workflowProperties = new SPWorkflowActivationProperties();

        private void onWorkflowActivated1_Invoked(object sender, ExternalDataEventArgs e)
        {
            //This simple workflow illustrates how to call an Azure WCF service from a 
            //workflow or workflow activity. Note that the workflow must run 
            //outside the sandbox to be able to call an external WCF service.

            //Deploy this solution to a SharePoint site that includes an Announcements
            //list. Package and deploy the AZURE_DayNamerService project before you run
            //This workflows. The workflow's getToday() method, calls the example WCF service to get 
            //today's name and adds it to the item.

            //Get the item that started the workflow
            SPListItem workflowItem = workflowProperties.Item;
            //Modify the body field
            workflowItem["Body"] += "This item was modified by a workflow on " + getToday();
            //Save the changes
            workflowItem.Update();

        }

        private string getToday()
        {
            //I used svcutil.exe to generate the proxy class for the service
            //in the generatedDayNamerProxy.cs file. I'm going to configure this
            //in code by using a channel factory.

            //Create the channel factory with a Uri, binding and endpoint
            Uri serviceUri = new Uri("http://daynamercs.cloudapp.net/dayinfoservice.svc");
            BasicHttpBinding serviceBinding = new BasicHttpBinding();
            EndpointAddress dayNamerEndPoint = new EndpointAddress(serviceUri);
            ChannelFactory<IDayInfo> channelFactory = new ChannelFactory<IDayInfo>(serviceBinding, dayNamerEndPoint);
            //Create a channel
            IDayInfo dayNamer = channelFactory.CreateChannel();
            //Now we can call the TodayIs method
            string today = dayNamer.TodayIs();
            //close the factory with all its channels
            channelFactory.Close();
            return today;
        }

    }
}
