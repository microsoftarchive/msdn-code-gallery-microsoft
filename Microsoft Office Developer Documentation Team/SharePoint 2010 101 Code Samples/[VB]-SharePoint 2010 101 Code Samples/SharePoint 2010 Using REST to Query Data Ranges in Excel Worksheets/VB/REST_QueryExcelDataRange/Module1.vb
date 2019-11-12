'This namespace is useful for processing the ATOM feed
Imports System.Xml.Linq
'This namespace contains the XMLUrlResolver class used below
Imports System.Xml
'This namespace contains stream classes
Imports System.IO
'This namespace contains the CredentialCache class
Imports System.Net

Module Module1
    'Store namespaces to ease later code
    Const atomNameSpace As String = "http://www.w3.org/2005/Atom"
    Const xlsvcNameSpace As String = "http://schemas.microsoft.com/office/2008/07/excelservices/rest"

    'Alter these values to match your site, document library, and spreadsheet name
    Const sharepointSite As String = "http://intranet.contoso.com"
    Const docLibrary As String = "shared%20documents"
    Const spreadsheetFileName As String = "cyclepartsales.xlsx"
    Const rangeName As String = "ProductAndSalesData"

    Sub Main()
        'This example gets all the cells in the specified data range and lists them
        'Note: Check that the Excel Services service application is running before you run this example

        'Get a reference to the atom namespace
        Dim atomNS As XNamespace = atomNameSpace
        'Build the relative Url to the excel workbook model
        Dim relativeUrl As String = "/_vti_bin/ExcelRest.aspx/" + docLibrary + "/" + spreadsheetFileName + "/model/Ranges('" + rangeName + "')?$format=atom"
        'Use an XMLUrlResolver object to pass credentials to the REST service
        Dim resolver As XmlUrlResolver = New XmlUrlResolver()
        resolver.Credentials = CredentialCache.DefaultCredentials
        'Build the URI to pass the the resolver
        Dim baseUri As Uri = New Uri(sharepointSite)
        Dim fullUri As Uri = resolver.ResolveUri(baseUri, relativeUrl)
        'Tell the user what the stream address is
        Console.WriteLine("Opening this stream: " + fullUri.ToString())
        'Call the resolver and receive the ATOM feed as a result
        Dim atomStream As Stream = DirectCast(resolver.GetEntity(fullUri, Nothing, GetType(Stream)), Stream)
        'Load the stream into an XDocument
        Dim atomResults As XDocument = XDocument.Load(atomStream)
        Dim cells = From c In atomResults.Elements()
                Select c
        Console.WriteLine("Cells: ")
        'Display each of the table
        For Each cell As XElement In cells
            Console.WriteLine(cell.Value)
        Next
        'This line prevents the console disappearing before you can read the result
        'Alternatively, remove this line a run the project without debugging (CTRL-F5)
        Console.ReadKey()
    End Sub

End Module
