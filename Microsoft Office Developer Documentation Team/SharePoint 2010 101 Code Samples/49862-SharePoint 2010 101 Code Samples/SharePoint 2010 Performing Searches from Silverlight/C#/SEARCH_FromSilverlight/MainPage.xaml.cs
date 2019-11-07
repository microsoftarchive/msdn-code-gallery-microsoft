using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using SEARCH_FromSilverlight.SharePointSearchService;

namespace SEARCH_FromSilverlight
{

    /// <summary>
    /// This example Silverlight application submits a query to the SharePoint search
    /// web service and displays the results in a list box. To run this application in 
    /// SharePoint, build it, then upload the XAP file to a SharePoint media library, 
    /// such as Site Assets. Add a Silverlight Web Part to a SharePoint page, then point
    /// the Web Part to the XAP file in the media library. However, it's not necessary to
    /// run this application in SharePoint.
    /// </summary>
    /// <remarks>
    /// Notice that the project contains a service reference to the SharePoint Search.asmx
    /// web service. This example does minimal parsing before displaying the items in 
    /// a list box so each item contains XML. In a user-facing application, you'd make
    /// this look more user friendly by parse each result.
    /// </remarks>
    public partial class MainPage : UserControl
    {

        QueryServiceSoapClient serviceClient;
        private XNamespace sr = "urn:Microsoft.Search.Response";
        private XNamespace srd = "urn:Microsoft.Search.Response.Document";

        public MainPage()
        {
            InitializeComponent();
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            //Create a search service proxy 
            serviceClient = new SharePointSearchService.QueryServiceSoapClient();

            //You should run the query asynchronously to avoid freezing the UI.
            //Start by adding a handler for the QueryCompleted event
            serviceClient.QueryCompleted += new EventHandler<QueryCompletedEventArgs>(serviceClient_QueryCompleted);
            
            //Formulate the CAML query
            string camlQuery = String.Format(@"<QueryPacket xmlns='urn:Microsoft.Search.Query' Revision='1000'>
                                                    <Query domain='QDomain'>
                                                        <SupportedFormats>
                                                            <Format>urn:Microsoft.Search.Response.Document.Document</Format>
                                                        </SupportedFormats>
                                                        <Context>
                                                            <QueryText language='en-US' type='STRING'>{0}</QueryText>
                                                        </Context>
                                                        <Range>
                                                            <StartAt>1</StartAt>
                                                            <Count>50</Count>
                                                        </Range>
                                                        <EnableStemming>true</EnableStemming>
                                                        <TrimDuplicates>true</TrimDuplicates>
                                                        <IgnoreAllNoiseQuery>true</IgnoreAllNoiseQuery>
                                                        <ImplicitAndBehavior>true</ImplicitAndBehavior>
                                                        <IncludeRelevanceResults>true</IncludeRelevanceResults>
                                                        <IncludeSpecialTermResults>true</IncludeSpecialTermResults>
                                                        <IncludeHighConfidenceResults>true</IncludeHighConfidenceResults>
                                                    </Query>
                                                </QueryPacket>", searchTermsTextBox.Text);

            //Run the query
            serviceClient.QueryAsync(camlQuery);

        }

        void serviceClient_QueryCompleted(object sender, QueryCompletedEventArgs e)
        {
            //The query has executed and returned something
            XDocument resultsDocument = XDocument.Parse(e.Result.ToString());
            //Check the status value 
            string status = resultsDocument.Descendants(sr + "Status").First<XElement>().Value;

            // If the results are successful, display them
            if (status.ToLower() == "success")
            {
                //Get a collection of items
                IEnumerable<XElement> resultItems = resultsDocument.Descendants(srd + "Document");
                //Add them to the list box
                resultsListBox.ItemsSource = resultItems;
            }
            
        }

    }
}
