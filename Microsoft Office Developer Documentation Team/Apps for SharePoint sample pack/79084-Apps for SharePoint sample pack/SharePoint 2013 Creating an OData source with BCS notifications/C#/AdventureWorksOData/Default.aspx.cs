using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Data;
using System.Data.SqlClient;

namespace AdventureWorksOData
{
    public partial class Default : System.Web.UI.Page
    {
        public string subscriptionStorePath = @"\\SPHVM-19159\SubscriptionStore\SubscriptionStore.xml";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public string Subscribe(string deliveryUrl, string eventType)
        {
            string subscriptionId = Guid.NewGuid().ToString();

            XmlDocument subscriptionStore = new XmlDocument();
            subscriptionStore.Load(subscriptionStorePath);

            // Add a new subscription
            XmlNode newSubNode = subscriptionStore.CreateElement("Subscription");

            XmlNode subscriptionIdStart = subscriptionStore.CreateElement("SubscriptionID");
            subscriptionIdStart.InnerText = subscriptionId;
            newSubNode.AppendChild(subscriptionIdStart);

            XmlNode deliveryAddressStart = subscriptionStore.CreateElement("DeliveryAddress");
            deliveryAddressStart.InnerText = deliveryUrl;
            newSubNode.AppendChild(deliveryAddressStart);

            XmlNode eventTypeStart = subscriptionStore.CreateElement("EventType");
            eventTypeStart.InnerText = eventType;
            newSubNode.AppendChild(eventTypeStart);

            subscriptionStore.DocumentElement.AppendChild(newSubNode);

            subscriptionStore.Save(subscriptionStorePath);

            return subscriptionId;
        }

        public void Unsubscribe(string subscriptionId)
        {
            XmlDocument subscriptionStore = new XmlDocument();
            subscriptionStore.Load(subscriptionStorePath);

            XmlNodeList subscriptions = subscriptionStore.DocumentElement.ChildNodes;
            foreach (XmlNode subscription in subscriptions)
            {
                XmlNodeList subscriptionList = subscription.ChildNodes;
                if (subscriptionList.Item(0).InnerText == subscriptionId)
                {
                    subscriptionStore.DocumentElement.RemoveChild(subscription);
                    break;
                }
            }

            subscriptionStore.Save(subscriptionStorePath);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Subscribe("mydeliveryurl", "epic fail");

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Unsubscribe("11c10f54-718e-4771-963f-4a67708c1fc4");
        }


    }
}