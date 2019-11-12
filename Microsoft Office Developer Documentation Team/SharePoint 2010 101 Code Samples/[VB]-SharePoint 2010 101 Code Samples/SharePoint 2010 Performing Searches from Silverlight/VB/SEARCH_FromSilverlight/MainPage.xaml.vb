Imports System.Xml
Imports System.Xml.Linq
Imports SEARCH_FromSilverlight.SharePointSearchService

''' <summary>
''' This example Silverlight application submits a query to the SharePoint search
''' web service and displays the results in a list box. To run this application in 
''' SharePoint, build it, then upload the XAP file to a SharePoint media library, 
''' such as Site Assets. Add a Silverlight Web Part to a SharePoint page, then point
''' the Web Part to the XAP file in the media library. However, it's not necessary to
''' run this application in SharePoint.
''' </summary>
''' <remarks>
''' Notice that the project contains a service reference to the SharePoint Search.asmx
''' web service. This example does minimal parsing before displaying the items in 
''' a list box so each item contains XML. In a user-facing application, you'd make
''' this look more user friendly by parse each result.
''' </remarks>
Partial Public Class MainPage
    Inherits UserControl

    Private serviceClient As QueryServiceSoapClient
    Private sr As XNamespace = "urn:Microsoft.Search.Response"
    Private srd As XNamespace = "urn:Microsoft.Search.Response.Document"

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub searchButton_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        'Create a search service proxy 
        serviceClient = New SharePointSearchService.QueryServiceSoapClient()

        'You should run the query asynchronously to avoid freezing the UI.
        'Start by adding a handler for the QueryCompleted event
        AddHandler serviceClient.QueryCompleted, AddressOf serviceClient_QueryCompleted

        'Formulate the CAML query
        Dim camlQuery As String = "<QueryPacket xmlns='urn:Microsoft.Search.Query' Revision='1000'>"
        camlQuery += "<Query domain='QDomain'>"
        camlQuery += "  <SupportedFormats>"
        camlQuery += "      <Format>urn:Microsoft.Search.Response.Document.Document</Format>"
        camlQuery += "  </SupportedFormats>"
        camlQuery += "  <Context>"
        camlQuery += "      <QueryText language='en-US' type='STRING'>" + searchTermsTextBox.Text + "</QueryText>"
        camlQuery += "  </Context>"
        camlQuery += "  <Range>"
        camlQuery += "      <StartAt>1</StartAt>"
        camlQuery += "      <Count>50</Count>"
        camlQuery += "  </Range>"
        camlQuery += "  <EnableStemming>true</EnableStemming>"
        camlQuery += "  <TrimDuplicates>true</TrimDuplicates>"
        camlQuery += "  <IgnoreAllNoiseQuery>true</IgnoreAllNoiseQuery>"
        camlQuery += "  <ImplicitAndBehavior>true</ImplicitAndBehavior>"
        camlQuery += "  <IncludeRelevanceResults>true</IncludeRelevanceResults>"
        camlQuery += "  <IncludeSpecialTermResults>true</IncludeSpecialTermResults>"
        camlQuery += "  <IncludeHighConfidenceResults>true</IncludeHighConfidenceResults>"
        camlQuery += "</Query>"
        camlQuery += "</QueryPacket>"

        'Run the query
        serviceClient.QueryAsync(camlQuery)

    End Sub

    Private Sub serviceClient_QueryCompleted(ByVal sender As Object, ByVal e As QueryCompletedEventArgs)

        'The query has executed and returned something
        Dim resultsDocument As XDocument = XDocument.Parse(e.Result.ToString())
        
        'Get a collection of items
        Dim resultItems As IEnumerable(Of XElement) = resultsDocument.Descendants(srd + "Document")
        'Add them to the list box
        resultsListBox.ItemsSource = resultItems

    End Sub

End Class