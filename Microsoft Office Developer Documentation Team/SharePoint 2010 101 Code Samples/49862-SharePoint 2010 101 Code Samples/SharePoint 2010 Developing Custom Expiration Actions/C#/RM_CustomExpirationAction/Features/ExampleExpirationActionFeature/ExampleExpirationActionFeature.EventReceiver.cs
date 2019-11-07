using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Office.RecordsManagement.PolicyFeatures;
using Microsoft.Office.RecordsManagement.InformationPolicy;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Security;

namespace RM_CustomExpirationAction.Features.ExampleExpirationActionFeature
{

    [Guid("0d7efc19-7c6d-470b-8ec8-8746b6ba2f14")]
    public class ExampleExpirationActionFeatureEventReceiver : SPFeatureReceiver
    {

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            //The RM_CustomExpirationAction.dll assembly will be deployed to the GAC
            //In this event of the feature receiver, we must let SharePoint know that
            //it is a custom expiration action. 

            //Start by defining an XML Manifest that describes the custom action
            string xmlManifestAction = "<PolicyResource xmlns='urn:schemas-microsoft-com:office:server:policy' " +
                                            "id='RM_CustomExpirationAction.ExampleExpirationAction' " +
                                            "featureId='Microsoft.Office.RecordsManagement.PolicyFeatures.Expiration' " +
                                            "type='Action'>" +
                                                "<Name>ExampleExpirationAction</Name>" +
                                                "<Description>This expiration action adds an announcement when an item expires</Description>" +
                                                "<AssemblyName>RM_CustomExpirationAction, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9ce8abd05eabc691</AssemblyName>" +
                                                "<ClassName>RM_CustomExpirationAction.ExampleExpirationAction</ClassName>" +
                                        "</PolicyResource>";
        
            //Validate this manifest
            PolicyResource.ValidateManifest(xmlManifestAction);

            //It checks out OK, so add it
            PolicyResourceCollection.Add(xmlManifestAction);

        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            //Delete the custom action from the policy resources collection
            PolicyResourceCollection.Delete("RM_CustomExpirationAction.ExampleExpirationAction");
        }

    }
}
