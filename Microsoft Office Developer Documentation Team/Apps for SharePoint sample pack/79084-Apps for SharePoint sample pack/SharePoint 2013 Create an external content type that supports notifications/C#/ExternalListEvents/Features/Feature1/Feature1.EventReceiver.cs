using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;

namespace ExternalListEvents.Features.Feature1
{
    [Guid("2c508519-12b1-44bd-ae50-e48372bcb06a")]
    public class Feature1EventReceiver : SPFeatureReceiver
    {

        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            string assembly = "ExternalListEvents, Culture=neutral, Version=1.0.0.0, PublicKeyToken=f3e998418ec415ce";
            string className = "ExternalListEvents.ActivityEventReceiver";

            using (SPSite siteCollection = new SPSite("http://dev.wingtip13.com/bcs"))
            {
                using (SPWeb site = siteCollection.OpenWeb())
                {
                    SPList customers = site.Lists["Customers"];
                    customers.EventReceivers.Add(SPEventReceiverType.ItemAdded, assembly, className);
                }
            }
        }

        public override void FeatureDeactivating(SPFeatureReceiverProperties properties)
        {
            string assembly = "ExternalListEvents, Culture=neutral, Version=1.0.0.0, PublicKeyToken=f3e998418ec415ce";

            using (SPSite siteCollection = new SPSite("http://dev.wingtip13.com/bcs"))
            {
                using (SPWeb site = siteCollection.OpenWeb())
                {
                    SPList customers = site.Lists["Customers"];
                    SPEventReceiverDefinitionCollection receivers = customers.EventReceivers;
                    SPEventReceiverDefinition activityReceiver = null;
                    foreach (SPEventReceiverDefinition receiver in receivers)
                    {
                        if (receiver.Assembly.Equals(assembly, StringComparison.OrdinalIgnoreCase))
                        {
                            activityReceiver = receiver;
                            break;
                        }
                    }

                    if (activityReceiver != null)
                    {
                        activityReceiver.Delete();
                    }

                }
            }
        }


    }
}
