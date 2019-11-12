Imports System.Net
Imports System.Xml
Imports Microsoft.SharePoint.Client

Module Module1

    <STAThread()>
    Sub Main()
        'This example extends the SPO_AuthenticateUsingCSOM sample
        'After authenticating, this example connects to the Lists web
        'service and gets the items in the Documents library, then
        'lists them in the console window

        'Adjust this string to point to your site on Office 365
        Dim siteURL As String = "http://www.web-dojo.com/teamsite"
        Console.WriteLine("Opening Site: " + siteURL)

        'Call the ClaimClientContext class to do claims mode authentication
        Using context As ClientContext = ClaimsClientContext.GetAuthenticatedContext(siteURL)
            If Not context Is Nothing Then
                'We have the client context object so claims-based authentication is complete
                'Find out about the SP.Web object
                context.Load(context.Web)
                context.ExecuteQuery()
                'Display the name of the SharePoint site
                Console.WriteLine(context.Web.Title)
            End If
        End Using
        'Next, get the authentication cookie so we can use it to connect to the lists web service
        Console.WriteLine()
        Dim authCookie As CookieCollection = ClaimsClientContext.GetAuthenticatedCookies(siteURL, 700, 500)
        'listsWS is a Service Reference to the lists web service
        'In Solution Explorer edit this reference to point to an instance
        'of the service. This could be a local instance of SharePoint, rather
        'than SharePoint Online as it is just used to create a proxy class
        Dim list As listsWS.Lists = New listsWS.Lists()
        'Edit this URL to point to your SharePoint Online site
        list.Url = "http://www.web-dojo.com/teamsite/_vti_bin/Lists.asmx"
        list.CookieContainer = New CookieContainer()
        list.CookieContainer.Add(authCookie)
        'Next we create the query to list items in the Documents library
        'Edit this variable to be the name of the list you are interested in
        Dim listName As String = "Documents"
        'Put the view name here. In this case its the default view
        Dim viewName As String = ""
        'A good idea to limit the number of results
        Dim rowLimit As String = "5"
        'These are the XML elements passed to the query
        Dim xmlDoc As XmlDocument = New XmlDocument()
        Dim query As XmlElement = xmlDoc.CreateElement("Query")
        Dim viewFields As XmlElement = xmlDoc.CreateElement("ViewFields")
        Dim queryOptions As XmlElement = xmlDoc.CreateElement("QueryOptions")
        'Add CAML markup to the query elements
        query.InnerXml = "<Where><Gt><FieldRef Name=""ID"" />" + _
            "<Value Type=""Counter"">0</Value></Gt></Where>"
        viewFields.InnerXml = "<FieldRef Name =""Title"" />"
        queryOptions.InnerXml = "<IncludeMandatoryColumns>FALSE</IncludeMandatoryColumns>" + _
            "<DateInUtc>TRUE</DateInUtc>"
        'Run the query
        Dim nodes As XmlNode = list.GetListItems(listName, _
                viewName, _
                query, _
                viewFields, _
                rowLimit, _
                Nothing, _
                String.Empty)
        Dim ixml As String = list.GetList(listName).InnerXml
        Console.WriteLine("Retreiving Items...")
        'Loop through the results
        For Each node As XmlNode In nodes
            If node.Name = "rs:data" Then
                For i As Integer = 0 To node.ChildNodes.Count
                    If node.ChildNodes(i).Name = "z:row" Then
                        'This is a node that corresponds to an item in the list
                        'Tell the user its Title
                        Console.WriteLine(node.ChildNodes(i).Attributes("ows_Title").Value)
                    End If
                Next
            End If
        Next
        'This line prevents the console disappearing before you can read the result
        'Alternatively, remove this line a run the project without debugging (CTRL-F5)
        Console.ReadKey()
    End Sub

End Module
